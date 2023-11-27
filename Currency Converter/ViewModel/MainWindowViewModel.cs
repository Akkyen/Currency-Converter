using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Currency_Converter.Model;
using Currency_Converter.View;
using Newtonsoft.Json;

namespace Currency_Converter.ViewModel;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private ObservableCollection<Currency> _currencies;

    private string _sourceAmount;
    private string _targetAmount;

    private Currency? _sourceSelectedCurrency;
    private Currency? _targetSelectedCurrency;

    public MainWindowViewModel()
    {
        _currencies = new();
        
        _sourceAmount = "0";
        _targetAmount = "0";

        Setup();
    }

    private void Setup()
    {
        Task<string> task = Utils.ApiCall(ApiCalls.ApiCallAllCurrencies);

        string json = task.Result;

        if (json != "")
        {
            SortedDictionary<string, string>? currencyDict =
                JsonConvert.DeserializeObject<SortedDictionary<string, string>>(json);

            if (currencyDict != null)
            {
                foreach (var pair in currencyDict)
                {
                    if (pair.Key != "")
                    {
                        _currencies.Add(new Currency(pair.Key, pair.Value));
                    }
                }

                OnPropertyChanged(nameof(Currencies));
            }
            else
            {
                throw new Exception("JSON wasn't converted correctly!");
            }
        }
        else
        {
            throw new Exception("ApiCall failed to return available currencies!");
        }
    }

    public void CbSourceCurChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SourceSelectedCurrency != null)
        {
            Regex regex = new Regex("(\"" + SourceSelectedCurrency.CurrencyCode + "\")");

            string json = Utils.ApiCall(ApiCalls.ApiCallRatesStartTemplate +
                                        SourceSelectedCurrency.CurrencyCode +
                                        ApiCalls.ApiCallRatesEndTemplate).Result;

            json = regex.Replace(json, "Rates", 1);

            if (json != "")
            {
                if (SourceSelectedCurrency.ExchangeRates.Count == 0)
                {
                    CurrencyExchangeRates? cer = JsonConvert.DeserializeObject<CurrencyExchangeRates>(json);

                    if (cer != null)
                    {
                        SourceSelectedCurrency.ExchangeRates = cer.Rates;
                    }
                }

                //UpdateCurAmount();
            }
        }
    }

    public void CbTargetCurChanged(object sender, SelectionChangedEventArgs e)
    {
        //UpdateCurAmount();
    }

    public void BtSwitchCurOnClick(object sender, RoutedEventArgs e)
    {
        (SourceAmount, TargetAmount) = (TargetAmount, SourceAmount);
        (SourceSelectedCurrency, TargetSelectedCurrency) = (TargetSelectedCurrency, SourceSelectedCurrency);
    }

    public void BtUpdateCurAmountOnClick(object sender, RoutedEventArgs e)
    {
        UpdateCurAmount();
    }

    private void UpdateCurAmount()
    {
        if (TargetSelectedCurrency != null && SourceSelectedCurrency != null)
        {
            if (double.TryParse(SourceAmount, out double srcAmount))
            {
                TargetAmount = "" + srcAmount *
                    SourceSelectedCurrency.ExchangeRates[
                        TargetSelectedCurrency.CurrencyCode];
            }
            else if (double.TryParse(TargetAmount, out double targetAmount))
            {
                SourceAmount = "" + targetAmount * 1 /
                    SourceSelectedCurrency.ExchangeRates[
                        TargetSelectedCurrency.CurrencyCode];
            }
        }
    }

    public ObservableCollection<Currency> Currencies
    {
        get => _currencies;
        set
        {
            if (Equals(value, _currencies)) return;
            _currencies = value ?? throw new ArgumentNullException(nameof(value));
            OnPropertyChanged();
        }
    }

    public string SourceAmount
    {
        get => _sourceAmount;
        set
        {
            if (value == _sourceAmount) return;
            _sourceAmount = value ?? throw new ArgumentNullException(nameof(value));
            OnPropertyChanged();
        }
    }

    public string TargetAmount
    {
        get => _targetAmount;
        set
        {
            if (value == _targetAmount) return;
            _targetAmount = value ?? throw new ArgumentNullException(nameof(value));
            OnPropertyChanged();
        }
    }

    public Currency? SourceSelectedCurrency
    {
        get => _sourceSelectedCurrency;
        set
        {
            if (Equals(value, _sourceSelectedCurrency)) return;
            _sourceSelectedCurrency = value ?? throw new ArgumentNullException(nameof(value));
            OnPropertyChanged();
        }
    }

    public Currency? TargetSelectedCurrency
    {
        get => _targetSelectedCurrency;
        set
        {
            if (Equals(value, _targetSelectedCurrency)) return;
            _targetSelectedCurrency = value ?? throw new ArgumentNullException(nameof(value));
            OnPropertyChanged();
        }
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    
}