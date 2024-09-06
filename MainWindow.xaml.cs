using Microsoft.Win32;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace WPF_Brickstore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<System.Collections.IDictionary> Items = [];
        DataTable table = new();
        public MainWindow()
        {
            InitializeComponent();
        }
        async void LoadBSX(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                Filter = "XML Markup (*.bsx) |*.bsx"
            };
            bool? result = dialog.ShowDialog();
            if (result == true ) { 
            
                if(!dialog.FileName.EndsWith("bsx"))
                {
                    MessageBox.Show("Error, The file is not a valid format (BSX)!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                using (var stream = dialog.OpenFile()) {
                    XDocument Doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
                    foreach (var item in Doc.Descendants("Item")) {
                        Dictionary<string, object> newItem = [];
                        item.Elements().ToList().ForEach(x => newItem[x.Name.ToString()] = x.Value);
                        Items.Add(newItem);
                        
                    }
                }
                GenerateColumns();
                return;
            }
            
            MessageBox.Show("The dialog has been closed!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;

        }

        void SortTable(object sender, TextChangedEventArgs e)
        {
           if(sender is TextBox tb)
            {
                string sortBy = tb.Tag as string ?? "";
                DataView view = table.AsDataView();
                view.RowFilter = $"{sortBy} LIKE '%{tb.Text}%'";
                dg_LegoItems.ItemsSource = view;
            }
            return;
        }

        private void GenerateColumns()
        {
            foreach(string key in Items.First().Keys)
            {
                table.Columns.Add(key);
            }
            foreach (var dicts in Items) { 
                var Row = table.NewRow();
                foreach (DictionaryEntry value in dicts) {
                    Row[value.Key.ToString()!] = value.Value; 
                }
                table.Rows.Add(Row);
            }
            dg_LegoItems.ItemsSource = table.DefaultView;
        }
    }
}