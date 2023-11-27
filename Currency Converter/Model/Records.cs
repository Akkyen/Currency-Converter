namespace Currency_Converter.Model;

public record CurrencyExchangeRates(DateOnly Date, Dictionary<String, double> Rates);