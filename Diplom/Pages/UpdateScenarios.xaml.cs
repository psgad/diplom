using Diplom.Models;
using Diplom.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Diplom
{
    public partial class UpdateScenarios : Page
    {
        private readonly MainWindow parent;
        private UIElement placeholder;
        private string scenarioName;
        private DispatcherTimer scrollTimer;
        private double scrollSpeed;
        private int index_scenarios = -1;
        private bool create;

        public string ScenariosName
        {
            get => scenarioName;
            set => scenarioName = value;
        }

        public UpdateScenarios(MainWindow mainWindow, bool create = true, string scenarioName = null)
        {
            InitializeComponent();
            parent = mainWindow;
            this.create = create;
            if (!create)
            {
                var scenarios_list = utils.getAllScenarios();
                for (int i = 0; i < scenarios_list.Count; i++)
                {
                    if (scenarios_list[i].name == scenarioName) index_scenarios = i;
                    break;
                }
                LoadScenario(scenarioName);
            }
            ScenariosName = scenarioName ?? "NewScenario";
            DataContext = this;
            scrollTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };
            scrollTimer.Tick += ScrollTimer_Tick;
        }

        private void BackToApps(object sender, RoutedEventArgs e)
        {
            parent.content.Content = new Scenarios(parent);
        }

        private void AddAction_Click(object sender, RoutedEventArgs e)
        {
            ActionView actionView = new ActionView();
            actionView.deleteAction.Click += (s, ec) => actions.Children.Remove(actionView);
            actions.Children.Add(actionView);
        }

        private void WrapPanel_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(ActionView)) is ActionView)
            {
                Point dropPoint = e.GetPosition(actions);
                int dropIndex = GetDropIndex(dropPoint);

                if (placeholder == null)
                {
                    placeholder = new Border
                    {
                        Width = 900,
                        Height = 150,
                        Margin = new Thickness(10),
                        BorderBrush = Brushes.White,
                        BorderThickness = new Thickness(2),
                        Background = new SolidColorBrush(Color.FromArgb(50, 255, 255, 255))
                    };
                }

                if (actions.Children.Contains(placeholder))
                    actions.Children.Remove(placeholder);

                actions.Children.Insert(dropIndex, placeholder);
                var scrollViewer = VisualTreeHelper.GetParent(actions) as ScrollViewer;
                if (scrollViewer != null)
                {
                    Point relativePoint = e.GetPosition(scrollViewer);
                    double scrollMargin = 50;
                    double scrollMaxSpeed = 20;

                    if (relativePoint.Y < scrollMargin && scrollViewer.VerticalOffset > 0)
                    {
                        scrollSpeed = -scrollMaxSpeed * (scrollMargin - relativePoint.Y) / scrollMargin;
                        scrollTimer.Start();
                    }
                    else if (relativePoint.Y > scrollViewer.ActualHeight - scrollMargin &&
                             scrollViewer.VerticalOffset < scrollViewer.ScrollableHeight)
                    {
                        scrollSpeed = scrollMaxSpeed * (relativePoint.Y - (scrollViewer.ActualHeight - scrollMargin)) / scrollMargin;
                        scrollTimer.Start();
                    }
                    else
                    {
                        scrollTimer.Stop();
                        scrollSpeed = 0;
                    }
                }

                e.Handled = true;
            }
        }

        private void ScrollTimer_Tick(object sender, EventArgs e)
        {
            var scrollViewer = VisualTreeHelper.GetParent(actions) as ScrollViewer;
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + scrollSpeed);
            }
        }

        private void WrapPanel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(ActionView)) is ActionView draggedElement)
            {
                Point dropPoint = e.GetPosition(actions);
                int dropIndex = GetDropIndex(dropPoint);
                if (dropIndex <= actions.Children.Count - 2)
                {
                    actions.Children.Remove(draggedElement);
                    actions.Children.Remove(placeholder);
                    actions.Children.Insert(dropIndex, draggedElement);
                    placeholder = null;
                    scrollTimer.Stop();
                }
            }
            e.Handled = true;
        }

        private int GetDropIndex(Point dropPoint)
        {
            double itemHeight = 170;
            double y = dropPoint.Y;
            int index = (int)(y / itemHeight);
            double itemWidth = 920;
            int itemsPerRow = (int)(actions.ActualWidth / itemWidth);
            if (itemsPerRow < 1) itemsPerRow = 1;
            int row = (int)(y / itemHeight);
            int col = (int)(dropPoint.X / itemWidth);
            index = row * itemsPerRow + col;
            int maxIndex = actions.Children.Count + (actions.Children.Contains(placeholder) ? 0 : 1);
            return Math.Max(0, Math.Min(index, maxIndex));
        }

        private void SaveScenario_Click(object sender, RoutedEventArgs e)
        {
            if (utils.ValidateScenariosName(ScenariosName, !create))
            {
                MessageBoxCustom.Show("Название сценариев не должны повторяться");
                return;
            }
            if (!utils.ValidateTitle(ScenariosName, "Название сценария")) return;
            Scenarios_ scenario = new Scenarios_(
                ScenariosName,
                actions.Children.OfType<ActionView>().Select(av => new ActionScenarios(
                    av.SelectedType,
                    av.ActionValue,
                    float.TryParse(av.CounterText, out float delay) ? delay : 0f)).ToList());

            List<Scenarios_> scenarios_list = utils.getAllScenarios();
            if (create)
                scenarios_list.Add(scenario);
            else
                scenarios_list[index_scenarios] = scenario;
            serializer.serialize(scenarios_list, Settings.Default.SCENARIOS_PATH);
            MessageBoxCustom.Show("Сценарий сохранён.");
        }

        private void LoadScenario(string scenarioName)
        {
            Scenarios_ updatedScenarios = utils.getAllScenarios().FirstOrDefault(scenaries => scenaries.name == scenarioName);
            foreach (var action in updatedScenarios.actions)
            {
                var av = new ActionView(action.type, action.value, action.delay);
                av.deleteAction.Click += (s, e) => actions.Children.Cast<ActionView>().ToList().Remove(av);
                actions.Children.Add(av);
            }
        }
    }
}