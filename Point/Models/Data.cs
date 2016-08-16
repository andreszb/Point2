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
using Template10.Mvvm;


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
                db.CreateTable<SalesByDay>();              
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

        public List<Sale> getSales()
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                var query = db.Table<Sale>();
                return new List<Sale>(query);
            }
        }

        public List<Customer> getCustomers()
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                var query = db.Table<Customer>();
                return new List<Customer>(query);
            }
        }

        public void addNewItem(string Type, string Size, string Code, string Brand, double Price, int Num)
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                var s = db.Insert(new Item()
                {
                    Type = Type,
                    Size = Size,
                    Code = Code,
                    Brand = Brand,
                    Price = Price,
                    Num = Num,
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

        public void UpdateCustomer(Customer customer)
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                db.Update(customer);
            }

        }

        public void DeleteCustomer(Customer customer)
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                db.Delete(customer);
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
        

        public void addNewSale(CurrentSale sale)
        {
           if (!String.IsNullOrEmpty(sale.CustomerName))
            {
                using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
                {
                    var s = db.Insert(new Customer()
                    {
                        Name = sale.CustomerName,
                        Items = sale.getItems(),
                        Debt = sale.total,
                        Address = sale.Address,
                        PhoneNumber = sale.PhoneNumber,
                        Notes = sale.Notes,
                        Date = DateTime.Now,
                        PayedDebt = 0,
                        Total = -sale.total                       
                    });
                }
            } 
            using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                var s = db.Insert(new Sale()
                {
                    Items = sale.getItems(),
                    Total = String.IsNullOrEmpty(sale.CustomerName) ? sale.total : -sale.total,
                    Date = DateTime.Now
                });
            }
            sale.addCurrentSaleToDB();
        }

        public List<ItemAsStrings> getSalesByDay(DateTime date)
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
                  SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                var stringDate = date.ToLocalTime().ToString("s").Substring(2, 8);
                SalesByDay today = (from s in db.Table<SalesByDay>() where s.Day == stringDate select s).FirstOrDefault();
                return today != null ? today.getList() : null;
            }
        }     

        public String getTotalFromDayAsString(DateTime date)
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
                  SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                var stringDate = date.ToLocalTime().ToString("s").Substring(2, 8);
                SalesByDay today = (from s in db.Table<SalesByDay>() where s.Day == stringDate select s).FirstOrDefault();
                Double val = today != null ? today.Total : 0;
                CultureInfo culture = new CultureInfo("es-MX");
                return String.Format(culture,"{0:C2}", val);
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

        [MaxLength(100)]
        [NotNull]
        public String Address { get; set; }

        [MaxLength(100)]
        [NotNull]
        public String PhoneNumber { get; set; }

        [MaxLength(100)]
        [NotNull]
        public String Notes { get; set; }

        [NotNull]
        public String Items { get; set; }

        [NotNull]
        public Double Debt { get; set; }

        [NotNull]
        public Double PayedDebt { get; set; }

        [NotNull]
        public Double Total { get; set; }      

        [NotNull]
        public DateTime Date { get; set; }

    }

    public partial class Item
    {
        [PrimaryKey, AutoIncrement]
        public Int64 Id { get; set; }
        [MaxLength(100)]
        [NotNull]
        public String Type { get; set; }
        [MaxLength(100)]
        [NotNull]
        public String Size { get; set; }
        [MaxLength(100)]
        [NotNull]
        public String Code { get; set; }
        [MaxLength(100)]
        [NotNull]
        public String Brand { get; set; }
        [NotNull]
        public Double Price { get; set; }
        [NotNull]
        public Int32 Num { get; set; }
        [NotNull]
        public DateTime DateAdded { get; set; }

        [Ignore]
        public string Description
        {
            get
            {
                var culture = new CultureInfo("es-MX");
                return Type + "\t" + Size + "\t" + Code + "\t" + Brand + "\t" + String.Format(culture, "{0:C2}", Price);
            }
        }

        [Ignore]
        public string ShortDescription
        {
            get
            {
                var culture = new CultureInfo("es-MX");
                return Type + " " + Size + " " + Code + " " + Brand + " ";
            }
        }
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

    public partial class SalesByDay
    {
        [PrimaryKey, AutoIncrement]
        public Int64 Id { get; set; }

        [NotNull][Unique]
        public String Day { get; set; }

        [NotNull]
        public String ItemsString { get; set; }

        [NotNull]
        public Double Total { get; set; }

        public List<ItemAsStrings> getList()
        {
            List<ItemAsStrings> list = new List<ItemAsStrings>();
            var str = ItemsString;
            string[] rows = str.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in rows)
            {
                string[] cols = s.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                list.Add(new ItemAsStrings() { qty = cols[0], total = cols[1], brand = cols[2], model = cols[3], color = cols[4], size = cols[5], price = cols[6] });
            }
            return list;
        }      

    }

    public class ItemAsStrings
    {
        public string qty   { get; set; }
        public string brand { get; set; }
        public string model { get; set; }
        public string color { get; set; }
        public string size  { get; set; }
        public string price { get; set; }
        public string total { get; set; }
    }


    public class CurrentSale
    {
        
        public CurrentSale()
        {
            items = new ObservableCollection<uniqueItem>();
            CustomerName = "";
            Address = "";
            PhoneNumber = "";
            Notes = "";
        }

        public  void addCurrentSaleToDB()
        {
            foreach (uniqueItem ui in items)
            {
                ui.addUniqueItemToSalesByDay();
                ui.deleteUniqueItemFromInventory();
            }
        }


        public ObservableCollection<uniqueItem> items { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Notes { get; set; }


        public class uniqueItem
        {
            string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path,
               "db.sqlite");
            private int _qty = 1;
            public Item item { get; set; }
            public string qty { get { return _qty.ToString() + "×"; } }
            public int intQty { get { return _qty; } set { _qty = value; } }
            public void incrementQty() { _qty++; }

            public void addUniqueItemToSalesByDay()
            {
                for(int i = 0; i < intQty; i++)
                {
                    addNewItemToSaleByDay(item);
                }
            }

            public void deleteUniqueItemFromInventory()
            {
                for (int i = 0; i < intQty; i++)
                {
                    deleteItemFromInventory(item);
                }
            }

            private void deleteItemFromInventory(Item item)
            {
                if (item.Num > 0) {
                    using (var db = new SQLite.Net.SQLiteConnection(new
   SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
                    {
                        item.Num--;
                        db.Update(item);
                    }
                }
                

            }

            private void addNewItemToSaleByDay(Item item)
            {
                using (var db = new SQLite.Net.SQLiteConnection(new
                   SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
                {
                    var date = DateTime.Now;
                    var stringDate = date.ToLocalTime().ToString("s").Substring(2, 8);
                    SalesByDay today = (from s in db.Table<SalesByDay>() where s.Day == stringDate select s).FirstOrDefault();
                    if (today == null)
                    {
                        db.Insert(new SalesByDay() { Day = stringDate, ItemsString = "\n1\t" + item.Price + '\t' + item.Description, Total = item.Price });
                    }
                    else
                    {
                        string[] AllItems = today.ItemsString.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < AllItems.Length; i++)
                        {
                            if (AllItems[i].Contains(item.Description))
                            {
                                AllItems[i] = increaseStringByOne(AllItems[i]);
                                today.ItemsString = String.Join("\n", AllItems);
                                today.Total += item.Price;
                                db.Update(today);
                                return;
                            }
                        }
                        today.Total += item.Price;
                        db.Update(today);                        
                    }
                }
            }

            private String increaseStringByOne(String str)
            {
                string[] col = str.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                col[0] = (int.Parse(col[0]) + 1).ToString();
                col[1] = (double.Parse(col[1]) + item.Price).ToString();
                return String.Join("\t", col);
            }
        }

        public string getItems()
        {
            string returnString = "";
            foreach (uniqueItem ui in items)
            {
                returnString += ui.qty + ui.item.Description + "\r";
            }
            return returnString;
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
                            if (i.intQty < item.Num)
                            {
                                i.incrementQty();
                                return true;
                            } else {
                                return false;
                            }
                        }
                    }
                    items.Add(new uniqueItem { item = item });
                    return true;
                
            }
            return false;
        }
    }

}
