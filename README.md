# Text Anonymizer

A simple desktop tool that uses a local AI model to strip sensitive information from text logs, error messages, and snippets.

**Why use this?**
Great for sanitizing stack traces or logs before pasting them into online LLMs, public forums, or chats. It replaces IPs, URLs, paths, and names with placeholders like `[IP]`, `[PATH]`, etc.

**⚠️ Disclaimer**
This tool uses a small local LLM (`qwen2.5:1.5b`). It is not perfect. Always double-check the output before sharing really sensitive data.

## Getting Started

### Prerequisites
1. **.NET 10.0 SDK**.
2. Install [Ollama](https://ollama.com/).
3. Pull the model:
   ```bash
   ollama pull qwen2.5:1.5b
   ```

### Running the App
```bash
dotnet run
```

### Configuration
You can change the Ollama URL or Model in `appsettings.json`.

### Tech Stack
- **Avalonia UI** (Cross-platform capability)
- **Microsoft Semantic Kernel** (AI Orchestrator)
- **Ollama** (Local Inference)

### Troubleshooting
- **Error connecting?** Ensure Ollama is running (`ollama serve`).
- **Wrong output?** Use `appsettings.json` to switch to a larger model like `llama3.2` if needed.
