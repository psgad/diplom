using Diplom.Models;
using Diplom.Properties;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для Page1.xaml
    /// </summary>
    public partial class Scenarios : Page
    {
        MainWindow parent;

        void initialize()
        {

            var scenarios = serializer.deserialize<List<Scenarios_>>(Settings.Default.SCENARIOS_PATH);
            this.scenarios.Children.Clear();
            for (int i = 0; i < scenarios.Count; i++)
            {

                ScenariosView scenariosView = new ScenariosView(scenarios[i], i, parent);
                scenariosView.deleteButton.Click += (s, e) =>
                {
                    scenarios.RemoveAt(scenariosView.index);
                    serializer.serialize(scenarios, Settings.Default.SCENARIOS_PATH);
                    initialize();

                };
                this.scenarios.Children.Add(scenariosView);
            }
        }
        public Scenarios(MainWindow mainWindow)
        {
            InitializeComponent();
            parent = mainWindow;
            initialize();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e) => parent.content.Content = new UpdateScenarios(parent);
    }
}
