using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Threading.Tasks;

namespace TextAnonymizer
{
    public partial class MainWindow : Window
    {
        private readonly OllamaService _ollamaService;

        public MainWindow()
        {
            InitializeComponent();
            _ollamaService = new OllamaService();
        }

        private async void OnAnonymizeClick(object? sender, RoutedEventArgs e)
        {
            var inputText = InputTextBox.Text;

            if (string.IsNullOrWhiteSpace(inputText))
            {
                OutputTextBox.Text = "Please enter some text to anonymize.";
                return;
            }

            try
            {
                // Show loading state
                LoadingOverlay.IsVisible = true;
                AnonymizeButton.IsEnabled = false;

                // Call service
                var anonymizedText = await _ollamaService.AnonymizeAsync(inputText);

                // Update UI
                OutputTextBox.Text = anonymizedText;
            }
            catch (Exception ex)
            {
                OutputTextBox.Text = $"An error occurred: {ex.Message}";
            }
            finally
            {
                // Hide loading state
                LoadingOverlay.IsVisible = false;
                AnonymizeButton.IsEnabled = true;
            }
        }
    }
}