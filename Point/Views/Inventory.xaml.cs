using System;
using System.IO;
using Point.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using SQLite.Net.Attributes;
using SQLite.Net;
using System.Diagnostics;
using System.Threading.Tasks;
using MyToolkit.Model;
using MyToolkit.Paging;
using System.ComponentModel;
namespace Point.Views
{
    public sealed partial class Inventory : Page
    {
        string path;
        SQLite.Net.SQLiteConnection db;
        private string filter;

        public ObservableCollection<Item> Items { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            UpdateTable();
        }

        public Inventory()
        {
            this.InitializeComponent();
            path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path,
               "db.sqlite");
            db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            db.CreateTable<Customer>();
            db.CreateTable<Item>();
            db.CreateTable<Sale>();
        }

        private void Retrieve_Click(object sender, RoutedEventArgs e)
        {
            UpdateTable();
        }

        private async void UpdateTable()
        {
            await Task.Delay(1);
            var query = db.Table<Item>();
            Items = new ObservableCollection<Item>(query);
            DataGrid.ItemsSource = Items;
        }


        private void Add_Click(object sender, RoutedEventArgs e)
        {

            var s = db.Insert(new Customer()
            {
                Name = "textBox.Text",
                Debt = 10.0,
                Info = "No Info",
                Items = "No Items"
            });

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            inventorySplitView.IsPaneOpen = !inventorySplitView.IsPaneOpen;
        }

        private void addNewItem_Click(object sender, RoutedEventArgs e)
        {
            var s = db.Insert(new Item()
            {
                Brand = BrandBox.Text,
                Model = ModelBox.Text,
                Color = ColorBox.Text,
                Size = SizeBox.Text,
                Num = int.Parse(NumBox.Text),
                Cost = int.Parse(CostBox.Text),
                Price = int.Parse(PriceBox.Text),
                Category = CategoryBox.SelectedItem.ToString(),
                Shortcut = ShortcutBox.Text,
                DateAdded = DateTime.Now
            });
        }

        private void textChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                DataGrid.SetFilter<Item>(p =>
                        p.Brand.ToLower().Contains(sender.Text.ToLower()) ||
                        p.Model.ToLower().Contains(sender.Text.ToLower()) ||
                        p.Category.ToLower().Contains(sender.Text.ToLower()));
            }
        }
    }

    public partial class Customer
    {
        [PrimaryKey, AutoIncrement]
        public Int64 Id { get; set; }

        [MaxLength(100)]
        [NotNull]
        public String Name { get; set; }

        [NotNull]
        public Double Debt { get; set; }

        [MaxLength(100)]
        [NotNull]
        public String Info { get; set; }

        [NotNull]
        public String Items { get; set; }

    }

    public partial class Item
    {
        [PrimaryKey, AutoIncrement]
        public Int64 Id { get; set; }
        [MaxLength(100)]
        [NotNull]
        public String Brand { get; set; }
        [MaxLength(100)]
        [NotNull]
        public String Model { get; set; }
        [MaxLength(100)]
        [NotNull]
        public String Color { get; set; }
        [MaxLength(100)]
        [NotNull]
        public String Size { get; set; }
        [NotNull]
        public Int32 Num { get; set; }
        [NotNull]
        public Double Cost { get; set; }
        [NotNull]
        public Double Price { get; set; }
        [MaxLength(100)]
        [NotNull]
        public String Category { get; set; }
        [MaxLength(100)]
        [NotNull]
        public String Shortcut { get; set; }
        [NotNull]
        public DateTime DateAdded { get; set; }
    }

    public partial class Sale
    {
        [PrimaryKey, AutoIncrement]
        public Int64 Id { get; set; }

        [NotNull]
        public String Items { get; set; }

        [NotNull]
        public DateTime Date { get; set; }

        [NotNull]
        public Double Total { get; set; }

    }

}
