using Diplom.Models;
using Diplom.Properties;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для Page1.xaml
    /// </summary>
    public partial class Bindings : Page
    {
        public Bindings()
        {
            InitializeComponent();
            utils.getAllBindings()
                .Select(kv => new BindingView(kv.Key, kv.Value) { Width = 600, Height = 80 })
                .ToList()
                .ForEach(view => bindings.Children.Add(view));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Dictionary<string, string> newbinds = bindings.Children.OfType<BindingView>().ToDictionary(b => b.key.Text, b => b.name.Text);
            var list = newbinds.Values.Where(x => x != "").ToList();
            var hashset = newbinds.Values.Where(x => x != "").ToHashSet();
            if (list.Count != hashset.Count)
            {
                MessageBoxCustom.Show("Наименования привязок не должны совпадать");
                return;
            }
            foreach (string binds in newbinds.Values.Where(x => x != ""))
                if (!utils.ValidateTitle(binds, "Наименование клавиши"))
                    return;
            serializer.serialize(newbinds, Settings.Default.BINDINGS_PATH);
            MessageBoxCustom.Show("Изменения успешно сохранены");
        }
    }
}
