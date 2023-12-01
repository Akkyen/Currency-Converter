using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Currency_Converter.Model;
using Newtonsoft.Json;

namespace Currency_Converter.ViewModel;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private DateOnly _date;

    private ObservableCollection<Currency> _currencies;

    private string _sourceAmount;
    private string _targetAmount;

    private Currency? _sourceSelectedCurrency;
    private Currency? _targetSelectedCurrency;

    public MainWindowViewModel()
    {
        _currencies = new();

        _date = DateOnly.FromDateTime(DateTime.Now);
        
        _sourceAmount = "0";
        _targetAmount = "0";

        ObtainCurrencies();
    }

    /// <summary>
    /// This method extracts all available currencies from the used WebApi.
    /// </summary>
    /// <exception cref="Exception">
    /// Thrown if the deserialization of the received JSON (containing the currencies) didn't work or if the WebApi call failed to return the available currencies.
    /// </exception>
    private void ObtainCurrencies()
    {
        Task<string> task = Utils.WebApiCall(ApiCalls.ApiCallAllCurrencies);

        string json = task.Result;

        if (json != "")
        {
            // The received data consists of tuples which contain the currency code and the name of the currency.
            // The data is temporally saved in a sorted dict
            SortedDictionary<string, string>? currencyDict =
                JsonConvert.DeserializeObject<SortedDictionary<string, string>>(json);

            if (currencyDict != null)
            {
                // The data is used to create objects of type Currency, which are then inserted into the ObservableCollection _currencies.
                foreach (var pair in currencyDict)
                {
                    if (pair.Key != "")
                    {
                        _currencies.Add(new Currency(pair.Key, pair.Value));
                    }
                }
                // After all available currencies have been added, the View is getting notified about the changes to the property Currencies
                OnPropertyChanged(nameof(Currencies));
            }
            else
            {
                throw new Exception("JSON deserialization didn't work!");
            }
        }
        else
        {
            throw new Exception("WebApi call failed to return the available currencies!");
        }
    }

    /// <summary>
    /// Triggers if the target currency gets changed
    /// Updates the exchange rates of the source currency
    /// </summary>
    /// <param name="sender">Combobox containing the currently selected target currency</param>
    /// <param name="e"></param>
    public void CbSourceCurChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SourceSelectedCurrency != null)
        {
            Regex regex = new Regex("(\"" + SourceSelectedCurrency.CurrencyCode + "\")");

            string json = Utils.WebApiCall(ApiCalls.ApiCallRatesStartTemplate +
                                        SourceSelectedCurrency.CurrencyCode +
                                        ApiCalls.ApiCallRatesEndTemplate).Result;

            json = regex.Replace(json, "Rates", 1);

            if (json != "")
            {
                // Checks if there are no entries in ExchangeRates or if they are out of date
                if (SourceSelectedCurrency.ExchangeRates.Count == 0 || !_date.Equals(SourceSelectedCurrency.Date))
                {
                    CurrencyExchangeRates? cer = JsonConvert.DeserializeObject<CurrencyExchangeRates>(json);

                    if (cer != null)
                    {
                        // If the date is either null or older than the provided one it updates it
                        if (!_date.Equals(SourceSelectedCurrency.Date))
                        {
                            SourceSelectedCurrency.Date = cer.Date;
                        }

                        SourceSelectedCurrency.ExchangeRates = cer.Rates;
                    }
                }

                UpdateCurAmount();
            }
        }
    }

    /// <summary>
    /// Triggers if the target currency gets changed
    /// </summary>
    /// <param name="sender">Combobox containing the currently selected target currency</param>
    /// <param name="e"></param>
    public void CbTargetCurChanged(object sender, SelectionChangedEventArgs e)
    {
        UpdateCurAmount();
    }

    public void BtSwitchCurOnClick(object sender, RoutedEventArgs e)
    {
        if (SourceSelectedCurrency != null && TargetSelectedCurrency != null)
        {
            (SourceAmount, TargetAmount) = (TargetAmount, SourceAmount);
            (SourceSelectedCurrency, TargetSelectedCurrency) = (TargetSelectedCurrency, SourceSelectedCurrency);
        }
    }

    public void BtUpdateCurAmountOnClick(object sender, RoutedEventArgs e)
    {
        UpdateCurAmount();
    }

    /// <summary>
    /// Updates based on the available text in the target or source amount textbox either of the two.
    /// If the data in 
    /// </summary>
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