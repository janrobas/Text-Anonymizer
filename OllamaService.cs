using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace TextAnonymizer
{
    public class OllamaService
    {
        private readonly Kernel _kernel;
        private readonly OllamaSettings _settings;

        public OllamaService(Kernel kernel, IOptions<OllamaSettings> settings)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        }

        public async IAsyncEnumerable<string> AnonymizeStreamAsync(string input, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(input))
                yield break;

            var prompt = $$"""
You are a Text Anonymizer.
Task: Replace IPs, URLs, file paths, and names with placeholders [IP], [URL], [PATH], [NAME], [SECRET].
CRITICAL RULES:
1. PRESERVE THE EXACT FORMAT AND LENGTH. DO NOT OMIT ANY LINES.
2. DO NOT output markdown code blocks (```). 
3. DO NOT output prefixes like "Output:", "Anonymized Text:".
4. ONLY output the raw anonymized text.
5. Do NOT replace normal words, error codes, or configuration values (e.g. 'block-all-mixed-content') unless they are actual secrets/keys.
6. Do NOT convert data formats (e.g. keep Object matches as Object, do not convert to JSON).

Example 1:
Input:
Pinging 192.168.1.1 with 32 bytes of data:
Reply from 192.168.1.1: bytes=32 time=2ms

Output:
Pinging [IP] with 32 bytes of data:
Reply from [IP]: bytes=32 time=2ms

Example 2 (Preserve harmless text):
Input:
Content-Security-Policy: Ignoring 'unsafe-inline'.
Error: Key 'abc-123-xyz' is invalid.

Output:
Content-Security-Policy: Ignoring 'unsafe-inline'.
Error: Key [SECRET] is invalid.

Example 3 (Multi-line preservation):
Input:
PS C:\Users\janro> ping 8.8.8.8
Pinging 8.8.8.8 with 32 bytes of data:
Reply from 8.8.8.8: bytes=32 time=5ms

Output:
PS [PATH]> ping [IP]
Pinging [IP] with 32 bytes of data:
Reply from [IP]: bytes=32 time=5ms

Example 4 (Complex Objects):
Input:
Object { msg: "An error has occurred.", experimentName: "smartlinks", user: "janro" }

Output:
Object { msg: "An error has occurred.", experimentName: "smartlinks", user: "[NAME]" }

Task: Anonymize the following text (Raw text only):
Input:
{{input}}

Output:
""";

            var settings = new OpenAIPromptExecutionSettings { Temperature = _settings.Temperature };

            await foreach (var update in _kernel.InvokePromptStreamingAsync(prompt, new KernelArguments(settings), cancellationToken: cancellationToken))
            {
                if (update != null)
                {
                    yield return update.ToString();
                }
            }
        }
    }
}
