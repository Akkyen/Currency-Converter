using System.Text.RegularExpressions;
using System.Windows;
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
            Regex regex = new Regex("[^0-9.,]+");
            textCompositionEventArgs.Handled = regex.IsMatch(textCompositionEventArgs.Text);
        }
    }
}