namespace TextAnonymizer
{
    public class OllamaSettings
    {
        public string Url { get; set; } = "http://localhost:11434/api/generate";
        public string ModelName { get; set; } = "qwen2.5:1.5b";
    }
}
