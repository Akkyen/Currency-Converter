using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Currency_Converter.Model;

public class Currency(String currencyCode, String currencyName) : INotifyPropertyChanged
{
    public DateOnly Date;

    public readonly String CurrencyCode = currencyCode;
    public readonly String CurrencyName = currencyName;

    //String is the to be exchanged to currency code
    private Dictionary<String, double> _exchangeRates = new();

    public Currency(string currencyCode, string currencyName, Dictionary<string, double> ExchangeRates) : this(currencyCode, currencyName)
    {
        this.ExchangeRates = ExchangeRates;
    }

    public Dictionary<string, double> ExchangeRates
    {
        get
        {
            return _exchangeRates;
        }
        set
        {
            _exchangeRates = value;
            OnPropertyChanged("ExchangeRates");
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

    public override string ToString()
    {
        return currencyCode + " (" + currencyName + ")";
    }
}