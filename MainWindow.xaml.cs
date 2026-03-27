using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using WinRT.Interop;
using CreditSimulator.ViewModels;
using System;
using System.Linq;

namespace CreditSimulator
{
    public sealed partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; }

        public MainWindow()
        {
            ViewModel = new MainViewModel();
            this.InitializeComponent();
            var hWnd = WindowNative.GetWindowHandle(this);
            var wndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(wndId);

            // Mengatur Ikon Taskbar & Jendela
            appWindow.SetIcon("Assets/app_icon.ico"); 
            
            if (appWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.Maximize();
            }

            if (this.Content is Microsoft.UI.Xaml.FrameworkElement rootElement)
            {
                rootElement.DataContext = ViewModel;
            }

            // Use DecimalFormatter for dot separators without problematic 'Rp' prefix
            var formatter = new Windows.Globalization.NumberFormatting.DecimalFormatter(new[] { "id-ID" }, "ID")
            {
                FractionDigits = 0,
                IsGrouped = true
            };
            
            priceInput.SetValue(Microsoft.UI.Xaml.Controls.NumberBox.NumberFormatterProperty, formatter);
            dpInput.SetValue(Microsoft.UI.Xaml.Controls.NumberBox.NumberFormatterProperty, formatter);

            // Hook for Live Masking (As-You-Type dots)
            priceInput.Loaded += (s, e) => SetupLiveMask(priceInput);
            dpInput.Loaded += (s, e) => SetupLiveMask(dpInput);
        }

        private void SetupLiveMask(Microsoft.UI.Xaml.Controls.NumberBox nb)
        {
            var textBox = FindInternalTextBox(nb);
            if (textBox != null)
            {
                textBox.TextChanging += (s, e) =>
                {
                    string oldText = s.Text;
                    string raw = new string(oldText.Where(char.IsDigit).ToArray());
                    
                    if (string.IsNullOrEmpty(raw))
                    {
                        if (oldText != "0") s.Text = "0";
                        return;
                    }

                    if (double.TryParse(raw, out double amount))
                    {
                        string formatted = amount.ToString("N0", new System.Globalization.CultureInfo("id-ID"));
                        
                        if (oldText != formatted)
                        {
                            int selectionStart = s.SelectionStart;
                            int oldLen = oldText.Length;
                            
                            s.Text = formatted;
                            
                            // Adjust caret position
                            int newLen = formatted.Length;
                            int diff = newLen - oldLen;
                            s.SelectionStart = Math.Max(0, selectionStart + diff);
                        }
                    }
                };
            }
        }

        private Microsoft.UI.Xaml.Controls.TextBox FindInternalTextBox(DependencyObject parent)
        {
            for (int i = 0; i < Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChild(parent, i);
                if (child is Microsoft.UI.Xaml.Controls.TextBox tb) return tb;
                var result = FindInternalTextBox(child);
                if (result != null) return result;
            }
            return null;
        }
    }
}
