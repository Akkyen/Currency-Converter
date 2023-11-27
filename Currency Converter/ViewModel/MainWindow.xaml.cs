using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Currency_Converter.Model;
using Newtonsoft.Json;

namespace Currency_Converter.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly object _currenciesLock = new object();

        public List<Currency> Currencies;

        public MainWindow()
        {
            InitializeComponent();

            Currencies = new List<Currency>();

            CbSourceCur.ItemsSource = Currencies;
            CbTargetCur.ItemsSource = Currencies;

            CbSourceCur.SelectionChanged += CbSourceCurChanged;
            CbTargetCur.SelectionChanged += CbTargetCurChanged;

            BtSwitchCur.Click += BtSwitchCurOnClick;

            TbSourceCurAmount.LostFocus += TbSourceCurAmountUpdate;
            TbTargetCurAmount.LostFocus += TbTargetCurAmountUpdate;

            Setup();
        }

        private void Setup()
        {
            Task<String> task = Utils.ApiCall(ApiCalls.ApiCallAllCurrencies);

            String json = task.Result;

            if (json != "")
            {
                SortedDictionary<string, string> currencyDict = JsonConvert.DeserializeObject<SortedDictionary<string, string>>(json);

                lock (_currenciesLock)
                {
                    if (currencyDict != null)
                    {
                        foreach (var pair in currencyDict)
                        {
                            Currencies.Add(new Currency(pair.Key, pair.Value));
                        }
                    }
                    else
                    {
                        throw new Exception("JSON wasn't converted correctly!");
                    }
                }
            }
            else
            {
                throw new Exception("ApiCall failed to return available currencies!");
            }
        }

        private void CbSourceCurChanged(object sender, SelectionChangedEventArgs e)
        {
            Task<String> task = Utils.ApiCall(ApiCalls.ApiCallRatesStartTemplate +
                                              ((Currency)CbSourceCur.SelectedItem).CurrencyCode +
                                              ApiCalls.ApiCallRatesEndTemplate);

            String json = task.Result;

            if (json != "")
            {
                Regex regex = new Regex("(\"" + ((Currency)CbSourceCur.SelectedItem).CurrencyCode + "\")");
                
                json = regex.Replace(json, "Rates", 1);

                CurrencyExchangeRates cer = JsonConvert.DeserializeObject<CurrencyExchangeRates>(json);

                foreach (var pair in cer.Rates)
                {
                    ((Currency)CbSourceCur.SelectedItem).ExchangeRates = cer.Rates;
                }

                if (CbSourceCur.SelectedIndex > -1 && CbTargetCur.SelectedIndex > -1)
                {
                    if (double.TryParse(TbSourceCurAmount.Text, out double srcAmount))
                    {
                        TbTargetCurAmount.Text = "" + srcAmount * ((Currency)CbSourceCur.SelectedItem).ExchangeRates[((Currency)CbTargetCur.SelectedItem).CurrencyCode];
                    }
                    else if (double.TryParse(TbTargetCurAmount.Text, out double targetAmount))
                    {
                        TbSourceCurAmount.Text = "" + targetAmount * 1 / ((Currency)CbSourceCur.SelectedItem).ExchangeRates[((Currency)CbTargetCur.SelectedItem).CurrencyCode];
                    }
                }
            }
        }

        private void CbTargetCurChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbSourceCur.SelectedIndex > -1 && CbTargetCur.SelectedIndex > -1)
            {
                if (double.TryParse(TbSourceCurAmount.Text, out double srcAmount))
                {
                    TbTargetCurAmount.Text = "" + srcAmount * ((Currency)CbSourceCur.SelectedItem).ExchangeRates[((Currency)CbTargetCur.SelectedItem).CurrencyCode];
                }
                else if (double.TryParse(TbTargetCurAmount.Text, out double targetAmount))
                {
                    TbSourceCurAmount.Text = "" + targetAmount * 1 / ((Currency)CbSourceCur.SelectedItem).ExchangeRates[((Currency)CbTargetCur.SelectedItem).CurrencyCode];
                }
            }
        }

        private void BtSwitchCurOnClick(object sender, RoutedEventArgs e)
        {
            (TbSourceCurAmount.Text, TbTargetCurAmount.Text) = (TbTargetCurAmount.Text, TbSourceCurAmount.Text);
            (CbSourceCur.SelectedIndex, CbTargetCur.SelectedIndex) = (CbTargetCur.SelectedIndex, CbSourceCur.SelectedIndex);
        }

        private void TbTargetCurAmountUpdate(object sender, RoutedEventArgs e)
        {
            if (CbSourceCur.SelectedIndex > -1 && CbTargetCur.SelectedIndex > -1)
            {
                if (double.TryParse(TbSourceCurAmount.Text, out double srcAmount))
                {
                    TbTargetCurAmount.Text = "" + srcAmount * ((Currency)CbSourceCur.SelectedItem).ExchangeRates[((Currency)CbTargetCur.SelectedItem).CurrencyCode];
                }
                else if (double.TryParse(TbTargetCurAmount.Text, out double targetAmount))
                {
                    TbSourceCurAmount.Text = "" + targetAmount * 1 / ((Currency)CbSourceCur.SelectedItem).ExchangeRates[((Currency)CbTargetCur.SelectedItem).CurrencyCode];
                }
            }
        }

        private void TbSourceCurAmountUpdate(object sender, RoutedEventArgs e)
        {
            if (CbSourceCur.SelectedIndex > -1 && CbTargetCur.SelectedIndex > -1)
            {
                if (double.TryParse(TbSourceCurAmount.Text, out double srcAmount))
                {
                    TbTargetCurAmount.Text = "" + srcAmount * ((Currency)CbSourceCur.SelectedItem).ExchangeRates[((Currency)CbTargetCur.SelectedItem).CurrencyCode];
                }
                else if (double.TryParse(TbTargetCurAmount.Text, out double targetAmount))
                {
                    TbSourceCurAmount.Text = "" + targetAmount * 1 / ((Currency)CbSourceCur.SelectedItem).ExchangeRates[((Currency)CbTargetCur.SelectedItem).CurrencyCode];
                }
            }
        }
    }
}