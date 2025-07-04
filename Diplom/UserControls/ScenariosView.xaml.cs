using Diplom.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Diplom
{
    public partial class ScenariosView : UserControl
    {
        Scenarios_ scenarios_;
        public int index = 0;
        MainWindow parent;
        public ScenariosView(Scenarios_ scenarios, int index, MainWindow main)
        {
            InitializeComponent();
            parent = main;
            this.index = index;
            scenarios_ = scenarios;
            title_scenarios.Text = scenarios.name;
            count_actions.Text = scenarios.actions.Count.ToString() + " шт.";
            scenarios.actions.Select(action => action.value).ToList().ForEach(action => actions_list.Children.Add(new TextBlock()
            {
                Text = action,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(5),
                Style = (Style)Application.Current.FindResource("subtitle")
            }));
        }

        private void Button_Click(object sender, RoutedEventArgs e) => parent.content.Content = new UpdateScenarios(parent, false, title_scenarios.Text);
    }
}
