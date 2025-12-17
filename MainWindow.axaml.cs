using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Threading.Tasks;

namespace TextAnonymizer
{
    public partial class MainWindow : Window
    {
        private readonly OllamaService _ollamaService;
        private bool _isProcessing;

        public MainWindow()
        {
            InitializeComponent();
            _ollamaService = new OllamaService();
        }

        private async void OnAnonymizeClick(object? sender, RoutedEventArgs e)
        {
            await ProcessAnonymization();
        }

        private async Task ProcessAnonymization()
        {
            var inputText = InputTextBox.Text;

            if (string.IsNullOrWhiteSpace(inputText))
            {
                OutputTextBox.Text = "Please enter some text to anonymize.";
                return;
            }

            if (_isProcessing) return;

            try
            {
                _isProcessing = true;
                
                AnonymizeButton.IsEnabled = false;
                AnonymizeButton.Content = "Processing...";
                OutputTextBox.Text = "";

                await foreach (var chunk in _ollamaService.AnonymizeStreamAsync(inputText))
                {
                    OutputTextBox.Text += chunk;
                }
            }
            catch (Exception ex)
            {
                OutputTextBox.Text = $"An error occurred: {ex.Message}";
            }
            finally
            {
                _isProcessing = false;
                
                AnonymizeButton.IsEnabled = true;
                AnonymizeButton.Content = "Anonymize Text";
            }
        }
    }
}