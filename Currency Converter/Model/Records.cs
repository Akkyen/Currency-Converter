namespace Currency_Converter.Model;

public record CurrencyExchangeRates(DateOnly Date, Dictionary<string, double> Rates);