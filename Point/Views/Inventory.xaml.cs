using System;
using System.IO;
using Point.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using SQLite.Net.Attributes;
using System.Diagnostics;

namespace Point.Views
{
    public sealed partial class MainPage : Page
    {
        string path;
        SQLite.Net.SQLiteConnection db;

        public MainPage()
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
            var query = db.Table<Customer>();
            string id = "";
            string name = "";
            string age = "";
             
            foreach (var message in query)
            {
                id = id + " " + message.Id;
                name = name + " " + message.Name;
            }

            textBlock2.Text = "ID: " + id + "\nName: " + name + "\nAge: ";
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {

            var s = db.Insert(new Customer()
            {
                Name = textBox.Text,
                Debt = 10.0,
                Info = "No Info",
                Items = "No Items"
            });

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
