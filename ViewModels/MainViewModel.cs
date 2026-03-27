using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CreditSimulator.Models;
using CreditSimulator.Services;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace CreditSimulator.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly DatabaseService _dbService;

        // Input
        [ObservableProperty]
        private double productPrice = 0;

        [ObservableProperty]
        private double downPayment = 0;

        [ObservableProperty]
        private int selectedTenor = 12;
        
        [ObservableProperty]
        private bool isCustomerMode = true;

        [ObservableProperty]
        private bool isStartupEnabled;

        private const string StartupKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "CreditSimulator";

        partial void OnProductPriceChanged(double value) 
        {
            DownPayment = value * 0.2;
            UpdateCalculations();
        }
        partial void OnDownPaymentChanged(double value) => UpdateCalculations();
        partial void OnIsCustomerModeChanged(bool value) 
        {
            OnPropertyChanged(nameof(AdminVisibility));
            OnPropertyChanged(nameof(FactorColumnWidth));
            OnPropertyChanged(nameof(MarginColumnWidth));
            OnPropertyChanged(nameof(PercentColumnWidth));
            OnPropertyChanged(nameof(TableContentFontSize));
            OnPropertyChanged(nameof(TableHeaderFontSize));
            UpdateCalculations();
        }
        
        public string SelectedTenorSubtitle => $"x {SelectedTenor} bulan";
        partial void OnSelectedTenorChanged(int value)
        {
            OnPropertyChanged(nameof(SelectedTenorSubtitle));
            OnPropertyChanged(nameof(Tenor3Bg)); OnPropertyChanged(nameof(Tenor6Bg)); OnPropertyChanged(nameof(Tenor9Bg)); OnPropertyChanged(nameof(Tenor12Bg));
            OnPropertyChanged(nameof(Tenor3Fg)); OnPropertyChanged(nameof(Tenor6Fg)); OnPropertyChanged(nameof(Tenor9Fg)); OnPropertyChanged(nameof(Tenor12Fg));
            UpdateCalculations();
        }

        public Microsoft.UI.Xaml.Media.SolidColorBrush Tenor3Bg => SelectedTenor == 3 ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 15, 157, 88)) : new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
        public Microsoft.UI.Xaml.Media.SolidColorBrush Tenor6Bg => SelectedTenor == 6 ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 15, 157, 88)) : new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
        public Microsoft.UI.Xaml.Media.SolidColorBrush Tenor9Bg => SelectedTenor == 9 ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 15, 157, 88)) : new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
        public Microsoft.UI.Xaml.Media.SolidColorBrush Tenor12Bg => SelectedTenor == 12 ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 15, 157, 88)) : new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);

        public Microsoft.UI.Xaml.Media.SolidColorBrush Tenor3Fg => SelectedTenor == 3 ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White) : new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black);
        public Microsoft.UI.Xaml.Media.SolidColorBrush Tenor6Fg => SelectedTenor == 6 ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White) : new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black);
        public Microsoft.UI.Xaml.Media.SolidColorBrush Tenor9Fg => SelectedTenor == 9 ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White) : new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black);
        public Microsoft.UI.Xaml.Media.SolidColorBrush Tenor12Fg => SelectedTenor == 12 ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White) : new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black);

        // Selected Outputs
        [ObservableProperty]
        private double principal;

        [ObservableProperty]
        private double monthlyInstallment;

        [ObservableProperty]
        private double totalMargin;

        [ObservableProperty]
        private double grandTotal;

        [ObservableProperty]
        private double activeFactor;
        
        [ObservableProperty]
        private double marginPercentageRaw;

        [ObservableProperty]
        private string errorMessage;

        public bool IsErrorVisible => !string.IsNullOrEmpty(ErrorMessage);
        partial void OnErrorMessageChanged(string value) => OnPropertyChanged(nameof(IsErrorVisible));

        // Formatted outputs for UI
        private readonly CultureInfo _culture = new CultureInfo("id-ID");
        
        public string FormattedProductPrice => ProductPrice.ToString("C0", _culture);
        public string FormattedDownPayment => DownPayment.ToString("C0", _culture);
        public string FormattedPrincipal => Principal.ToString("C0", _culture);
        public string FormattedMonthlyInstallment => MonthlyInstallment.ToString("C0", _culture);
        public string FormattedTotalMargin => TotalMargin.ToString("C0", _culture);
        public string FormattedFirstPayment => FirstPayment.ToString("C0", _culture);
        public string FormattedAdminFee => CreditCalculatorService.AdminFee.ToString("C0", _culture);
        public string FormattedActiveFactor => ActiveFactor.ToString("0.0000");
        public string FormattedMarginPercentage => MarginPercentageRaw.ToString("0.0") + "%";
        
        public Microsoft.UI.Xaml.Visibility AdminVisibility => IsCustomerMode ? Microsoft.UI.Xaml.Visibility.Collapsed : Microsoft.UI.Xaml.Visibility.Visible;
        
        public Microsoft.UI.Xaml.GridLength FactorColumnWidth => IsCustomerMode ? new Microsoft.UI.Xaml.GridLength(0) : new Microsoft.UI.Xaml.GridLength(2, Microsoft.UI.Xaml.GridUnitType.Star);
        public Microsoft.UI.Xaml.GridLength MarginColumnWidth => IsCustomerMode ? new Microsoft.UI.Xaml.GridLength(0) : new Microsoft.UI.Xaml.GridLength(3, Microsoft.UI.Xaml.GridUnitType.Star);
        public Microsoft.UI.Xaml.GridLength PercentColumnWidth => IsCustomerMode ? new Microsoft.UI.Xaml.GridLength(0) : new Microsoft.UI.Xaml.GridLength(2, Microsoft.UI.Xaml.GridUnitType.Star);
        
        public double TableContentFontSize => IsCustomerMode ? 48 : 16;
        public double TableHeaderFontSize => IsCustomerMode ? 20 : 13;
        
        public double FirstPayment => DownPayment + CreditCalculatorService.AdminFee;
        public string FirstPaymentSubtitle => $"Rp {DownPayment.ToString("N0", _culture)} (DP) + Rp {CreditCalculatorService.AdminFee.ToString("N0", _culture)} (Admin)";

        partial void OnPrincipalChanged(double value) => OnPropertyChanged(nameof(FormattedPrincipal));
        partial void OnMonthlyInstallmentChanged(double value) => OnPropertyChanged(nameof(FormattedMonthlyInstallment));
        partial void OnTotalMarginChanged(double value) => OnPropertyChanged(nameof(FormattedTotalMargin));
        partial void OnActiveFactorChanged(double value) => OnPropertyChanged(nameof(FormattedActiveFactor));
        partial void OnMarginPercentageRawChanged(double value) => OnPropertyChanged(nameof(FormattedMarginPercentage));

        // Comparison Table
        public ObservableCollection<TenorComparisonRecord> Comparisons { get; } = new();

        // Early Payoff properties
        [ObservableProperty]
        private int earlyPayoffTargetTenor = 3;

        [ObservableProperty]
        private int monthsAlreadyPaid = 3;

        [ObservableProperty]
        private double earlyPayoffAmount;
        
        public string FormattedEarlyPayoffAmount => EarlyPayoffAmount.ToString("C0", _culture);
        partial void OnEarlyPayoffAmountChanged(double value) => OnPropertyChanged(nameof(FormattedEarlyPayoffAmount));

        partial void OnEarlyPayoffTargetTenorChanged(int value) 
        {
            OnPropertyChanged(nameof(Target3Bg)); OnPropertyChanged(nameof(Target6Bg)); OnPropertyChanged(nameof(Target9Bg));
            OnPropertyChanged(nameof(Target3Fg)); OnPropertyChanged(nameof(Target6Fg)); OnPropertyChanged(nameof(Target9Fg));
            CalculateEarlyPayoff();
        }
        partial void OnMonthsAlreadyPaidChanged(int value) => CalculateEarlyPayoff();
        
        public Microsoft.UI.Xaml.Media.SolidColorBrush Target3Bg => EarlyPayoffTargetTenor == 3 ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 15, 157, 88)) : new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
        public Microsoft.UI.Xaml.Media.SolidColorBrush Target6Bg => EarlyPayoffTargetTenor == 6 ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 15, 157, 88)) : new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
        public Microsoft.UI.Xaml.Media.SolidColorBrush Target9Bg => EarlyPayoffTargetTenor == 9 ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 15, 157, 88)) : new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);

        public Microsoft.UI.Xaml.Media.SolidColorBrush Target3Fg => EarlyPayoffTargetTenor == 3 ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White) : new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black);
        public Microsoft.UI.Xaml.Media.SolidColorBrush Target6Fg => EarlyPayoffTargetTenor == 6 ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White) : new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black);
        public Microsoft.UI.Xaml.Media.SolidColorBrush Target9Fg => EarlyPayoffTargetTenor == 9 ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White) : new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black);
        
        public string EarlyPayoffFormulaText => $"Formula: (P \u00d7 F_{EarlyPayoffTargetTenor} \u00d7 {EarlyPayoffTargetTenor}) - (Cicilan \u00d7 {MonthsAlreadyPaid})";

        public MainViewModel()
        {
            _dbService = new DatabaseService();
            CheckStartupStatus();
            // trigger on first run
            UpdateCalculations();
        }

        private void CheckStartupStatus()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKeyPath, false))
                {
                    if (key != null)
                    {
                        object value = key.GetValue(AppName);
                        // Check if the path matches our current executable
                        IsStartupEnabled = value != null && value.ToString() == Environment.ProcessPath;
                    }
                }
            }
            catch { }
        }

        partial void OnIsStartupEnabledChanged(bool value)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKeyPath, true))
                {
                    if (key != null)
                    {
                        if (value)
                        {
                            key.SetValue(AppName, Environment.ProcessPath);
                        }
                        else
                        {
                            key.DeleteValue(AppName, false);
                        }
                    }
                }
            }
            catch { }
        }

        public int[] AvailableTenors { get; } = { 3, 6, 9, 12 };

        public void UpdateCalculations()
        {
            ErrorMessage = string.Empty;
            
            OnPropertyChanged(nameof(FormattedProductPrice));
            OnPropertyChanged(nameof(FormattedDownPayment));

            try
            {
                var result = CreditCalculatorService.Calculate(ProductPrice, DownPayment, SelectedTenor);
                Principal = result.Principal;
                MonthlyInstallment = result.Installment;
                TotalMargin = result.Margin;
                GrandTotal = result.GrandTotal;
                ActiveFactor = CreditCalculatorService.GetFactor(SelectedTenor);
                MarginPercentageRaw = (TotalMargin / Principal) * 100;
                
                OnPropertyChanged(nameof(FirstPayment));
                OnPropertyChanged(nameof(FormattedFirstPayment));
                OnPropertyChanged(nameof(FirstPaymentSubtitle));

                Comparisons.Clear();
                double maxCredit = 0;
                foreach (var t in AvailableTenors)
                {
                    try
                    {
                        var tResult = CreditCalculatorService.Calculate(ProductPrice, DownPayment, t);
                        if (tResult.TotalCredit > maxCredit) maxCredit = tResult.TotalCredit;
                    }
                    catch { }
                }

                foreach (var t in AvailableTenors)
                {
                    try
                    {
                        var stepRes = CreditCalculatorService.Calculate(ProductPrice, DownPayment, t);
                        Comparisons.Add(new TenorComparisonRecord
                        {
                            Tenor = t,
                            Factor = CreditCalculatorService.GetFactor(t),
                            MonthlyInstallment = stepRes.Installment,
                            TotalCredit = stepRes.TotalCredit,
                            Margin = stepRes.Margin,
                            MarginPercentage = (stepRes.Margin / stepRes.Principal) * 100,
                            IsSelected = (t == SelectedTenor),
                            BarWidthPercentage = maxCredit > 0 ? (stepRes.TotalCredit / maxCredit) * 100 : 0,
                            MarginVisibility = AdminVisibility,
                            FactorColumnWidth = FactorColumnWidth,
                            MarginColumnWidth = MarginColumnWidth,
                            PercentColumnWidth = PercentColumnWidth
                        });
                    }
                    catch { }
                }

                CalculateEarlyPayoff();
                
                // Fire and forget save
                _ = _dbService.SaveSimulationAsync(new SimulationRecord
                {
                    CustomerName = "Guest",
                    ProductPrice = ProductPrice,
                    DownPayment = DownPayment,
                    Principal = Principal,
                    TenorMonths = SelectedTenor,
                    MonthlyInstallment = MonthlyInstallment,
                    Margin = TotalMargin,
                    AdminFee = CreditCalculatorService.AdminFee,
                    TotalCredit = result.TotalCredit,
                    GrandTotal = GrandTotal,
                    CreatedAt = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Principal = 0;
                MonthlyInstallment = 0;
                TotalMargin = 0;
                GrandTotal = 0;
                MarginPercentageRaw = 0;
                ActiveFactor = 0;
                Comparisons.Clear();
                EarlyPayoffAmount = 0;
            }
        }

        public void CalculateEarlyPayoff()
        {
            if (Principal > 0 && MonthsAlreadyPaid > 0 && EarlyPayoffTargetTenor > 0)
            {
                EarlyPayoffAmount = CreditCalculatorService.CalculateEarlyPayoff(Principal, EarlyPayoffTargetTenor, MonthlyInstallment, MonthsAlreadyPaid);
                OnPropertyChanged(nameof(EarlyPayoffFormulaText));
            }
            else
            {
                EarlyPayoffAmount = 0;
            }
        }
        
        [RelayCommand]
        public void SelectTenor(string tenorStr)
        {
            if(int.TryParse(tenorStr, out int t))
            {
                SelectedTenor = t;
            }
        }
        
        [RelayCommand]
        public void SelectTargetTenor(string tenorStr)
        {
            if(int.TryParse(tenorStr, out int t))
            {
                EarlyPayoffTargetTenor = t;
            }
        }
    }
}
