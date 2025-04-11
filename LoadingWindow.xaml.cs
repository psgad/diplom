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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            Storyboard loadingAnimation = (Storyboard)FindResource("LoadingAnimation");
            loadingAnimation.Begin();
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
