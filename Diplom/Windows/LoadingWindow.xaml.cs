using System;
using System.Threading.Tasks;
using System.Windows;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        public bool isLoading = true;
        public LoadingWindow(Func<Task> func)
        {
            InitializeComponent();
            new Task(() =>
            {
                while (isLoading) ;
                Dispatcher.Invoke(() => Close());
            }).Start();
            new Task(() =>
            {
                Dispatcher.Invoke(async () => { await func(); isLoading = false; });
            }).Start();

        }
    }
}
