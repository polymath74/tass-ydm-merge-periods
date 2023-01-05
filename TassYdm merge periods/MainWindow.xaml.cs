using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TassYdm_merge_periods
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //DataContext = new ViewModel();
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModel viewModel)
            {
                await viewModel.LoadConfigAsync();
                await viewModel.LoadSourceAsync();
            }
        }

        bool isClosing = false;
        bool canClose = false;

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is ViewModel viewModel)
            {
                if (isClosing)
                {
                    e.Cancel = !canClose;
                    isClosing = false;
                    canClose = false;
                    return;
                }

                e.Cancel = true;

                isClosing = true;
                canClose = false;

                canClose = await viewModel.OnWindowClosing();
                if (canClose)
                    _ = Dispatcher.InvokeAsync(Close, System.Windows.Threading.DispatcherPriority.Normal);
                else
                    isClosing = false;
            }
        }
    }
}
