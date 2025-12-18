using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TextAnonymizer
{
    public partial class MainWindow : Window
    {
        private readonly OllamaService _ollamaService;
        private bool _isProcessing;
        private CancellationTokenSource? _cts;

        public MainWindow(OllamaService ollamaService)
        {
            InitializeComponent();
            _ollamaService = ollamaService;
        }

        private async void OnAnonymizeClick(object? sender, RoutedEventArgs e)
        {
            await ProcessAnonymization();
        }

        private void OnStopClick(object? sender, RoutedEventArgs e)
        {
            _cts?.Cancel();
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
                _cts = new CancellationTokenSource();
                
                AnonymizeButton.IsEnabled = false;
                AnonymizeButton.Content = "Anonymizing...";
                StopButton.IsVisible = true;
                OutputTextBox.Text = "";

                await foreach (var chunk in _ollamaService.AnonymizeStreamAsync(inputText, _cts.Token))
                {
                    OutputTextBox.Text += chunk;
                }
            }
            catch (OperationCanceledException)
            {
                OutputTextBox.Text += "\n\n[Anonymization stopped by user]";
            }
            catch (Exception ex)
            {
                OutputTextBox.Text = $"An error occurred: {ex.Message}";
            }
            finally
            {
                _isProcessing = false;
                _cts?.Dispose();
                _cts = null;
                
                AnonymizeButton.IsEnabled = true;
                AnonymizeButton.Content = "Anonymize Text";
                StopButton.IsVisible = false;
            }
        }
    }
}
