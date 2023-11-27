using System.Windows;
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
        }
    }
}