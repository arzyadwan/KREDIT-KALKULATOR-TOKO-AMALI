using CommunityToolkit.Mvvm.ComponentModel;

namespace CreditSimulator.Models
{
    public partial class TenorComparisonRecord : ObservableObject
    {
        [ObservableProperty]
        private int tenor;

        [ObservableProperty]
        private double factor;

        [ObservableProperty]
        private double monthlyInstallment;

        [ObservableProperty]
        private double totalCredit;

        [ObservableProperty]
        private double margin;

        [ObservableProperty]
        private double marginPercentage;

        [ObservableProperty]
        private bool isSelected;
        
        [ObservableProperty]
        private double barWidthPercentage;
        
        [ObservableProperty]
        private Microsoft.UI.Xaml.Visibility marginVisibility = Microsoft.UI.Xaml.Visibility.Visible;
        
        [ObservableProperty]
        private Microsoft.UI.Xaml.GridLength factorColumnWidth;

        [ObservableProperty]
        private Microsoft.UI.Xaml.GridLength marginColumnWidth;

        [ObservableProperty]
        private Microsoft.UI.Xaml.GridLength percentColumnWidth;

        // formatters
        private readonly System.Globalization.CultureInfo _culture = new System.Globalization.CultureInfo("id-ID");
        public string FormattedMonthlyInstallment => MonthlyInstallment.ToString("C0", _culture);
        public string FormattedTotalCredit => TotalCredit.ToString("C0", _culture);
        public string FormattedMargin => Margin.ToString("C0", _culture);
        public string FormattedMarginPercentage => MarginPercentage.ToString("0.0") + "%";
        
        public string Title => $"{Tenor} Bulan";
        public string FormattedFactor => Factor.ToString("0.0000"); 

        public Microsoft.UI.Xaml.Media.SolidColorBrush BadgeBackground => IsSelected 
            ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 15, 157, 88))
            : new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 248, 249, 250));

        public Microsoft.UI.Xaml.Media.SolidColorBrush BadgeForeground => IsSelected 
            ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White)
            : new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 95, 99, 104));

        public Microsoft.UI.Xaml.Media.SolidColorBrush BarForeground => IsSelected 
            ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 15, 157, 88))
            : new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 204, 217, 232));
    }
}
