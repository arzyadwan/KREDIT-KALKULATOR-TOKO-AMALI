namespace CreditSimulator.Converters
{
    public class TenorToBrushConverter : Microsoft.UI.Xaml.Data.IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, string language)
        {
            if (value is int selectedTenor && parameter is string targetTenorStr && int.TryParse(targetTenorStr, out int targetTenor))
            {
                return selectedTenor == targetTenor 
                    ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 15, 157, 88)) 
                    : new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
            }
            return new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
        }
        public object ConvertBack(object value, System.Type targetType, object parameter, string language) => throw new System.NotImplementedException();
    }
    
    public class TenorToForegroundConverter : Microsoft.UI.Xaml.Data.IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, string language)
        {
            if (value is int selected && parameter is string target && int.TryParse(target, out int t))
            {
                return selected == t 
                    ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White) 
                    : new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black);
            }
            return new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black);
        }
        public object ConvertBack(object value, System.Type targetType, object parameter, string language) => throw new System.NotImplementedException();
    }
    
    public class BoolToBrushConverter : Microsoft.UI.Xaml.Data.IValueConverter
    {
        public Microsoft.UI.Xaml.Media.Brush TrueBrush { get; set; }
        public Microsoft.UI.Xaml.Media.Brush FalseBrush { get; set; }

        public object Convert(object value, System.Type targetType, object parameter, string language) => (value is bool b && b) ? TrueBrush : FalseBrush;
        public object ConvertBack(object value, System.Type targetType, object parameter, string language) => throw new System.NotImplementedException();
    }
}
