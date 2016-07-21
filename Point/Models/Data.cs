using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;
using SQLite.Net;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;

namespace Point.Model
{
    class Data
    {
        private string path;
        private SQLite.Net.SQLiteConnection db;

        public Data() {
            path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path,
               "db.sqlite");
            using (db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                db.CreateTable<Customer>();
                db.CreateTable<Item>();
                db.CreateTable<Sale>();
            }

        }



        public List<Item> getItems()
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                var query = db.Table<Item>();
                return new List<Item>(query);
            }
        }

        public void addNewItem(string Brand, string Model, string Color, string Size, int Num, double Cost, double Price, string Category)
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                var s = db.Insert(new Item()
                {
                    Brand = Brand,
                    Model = Model,
                    Color = Color,
                    Size = Size,
                    Num = Num,
                    Cost = Cost,
                    Price = Price,
                    Category = Category,
                    DateAdded = DateTime.Now
                });
            }

        }

        public void updateItem(Item item)
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                db.Update(item);
            }

        }

        public void deleteItem(Item item)
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                db.Delete(item);
            }
        }

        public void addNewDebt(string Name, double Debt, string Info, string Items)
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
              SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                var s = db.Insert(new Customer()
                {
                    Name = Name,
                    Debt = Debt,
                    Info = Info,
                    Items = Items,
                });
            }
        }

        public void addNewSale(CurrentSale sale)
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
              SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                var s = db.Insert(new Sale()
                {
                    Items = sale.getItems(),
                    Total = sale.total,
                    Date = DateTime.Now
                });
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

    public class CurrentSale
    {
        public CurrentSale()
        {
            items = new ObservableCollection<uniqueItem>();
        }


        public ObservableCollection<uniqueItem> items { get; set; }
        private Customer newCustomer = new Customer();

        public class uniqueItem
        {
            private int _qty = 1;
            public Item item { get; set; }
            public string qty { get { return _qty.ToString() + "×"; } }
            public int intQty { get { return _qty; } set { _qty = value; } }
            public void incrementQty() { _qty++; }

        }

        public string getItems()
        {
            string returnString = "";
            CultureInfo culture = new CultureInfo("es-MX");
            foreach (uniqueItem ui in items)
            {
                returnString += ui.qty + ui.item.Model + "\t" + String.Format(culture, "{0:C2}", ui.item.Price) + "\n";
            }
            return returnString;
        }



        public Customer customer
        {
            get { return newCustomer; }
            set { newCustomer = value; }
        }

        public double total
        {
            get { return (items.Sum(x => ((x as uniqueItem).item.Price * (x as uniqueItem).intQty))); }
        }

        public bool addItem(Item item)
        {
            if (item != null)
            {
                foreach (uniqueItem i in items)
                {
                    if (i.item == item)
                    {
                        i.incrementQty();
                        return true;
                    }
                }
                items.Add(new uniqueItem { item = item });
                return true;
            }
            return false;
        }
    }

}
