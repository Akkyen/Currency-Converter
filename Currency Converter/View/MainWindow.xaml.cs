using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Currency_Converter.ViewModel;

namespace Currency_Converter.View
{
    public partial class MainWindow : Window
    {
        public readonly MainWindowViewModel MainWindowViewModel = new();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = MainWindowViewModel;

            CbSourceCur.SelectionChanged += MainWindowViewModel.CbSourceCurChanged;
            CbTargetCur.SelectionChanged += MainWindowViewModel.CbTargetCurChanged;

            BtSwitchCur.Click += MainWindowViewModel.BtSwitchCurOnClick;
            BtUpdateCurAmount.Click += MainWindowViewModel.BtUpdateCurAmountOnClick;

            TbSourceCurAmount.PreviewTextInput += ValidateTextBoxInput;
            TbTargetCurAmount.PreviewTextInput += ValidateTextBoxInput;
        }

        public void ValidateTextBoxInput(object sender, TextCompositionEventArgs textCompositionEventArgs)
        {
            TextBox senderBox = sender as TextBox ?? throw new InvalidOperationException("ValidateTextBoxInput shouldn't be call form anything besides a TextBox!");

            Regex allowedChars = new Regex("[^0-9" + CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator + "]+");
            Regex culturalNumberDecimalSeparator = new Regex(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

            if (culturalNumberDecimalSeparator.Count(textCompositionEventArgs.Text) > 0 && culturalNumberDecimalSeparator.Count(senderBox.Text) > 0)
            {
                textCompositionEventArgs.Handled = true;
            }
            else
            {
                textCompositionEventArgs.Handled = allowedChars.IsMatch(textCompositionEventArgs.Text);
            }
        }
    }
}