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
    public static class ExtensionMethods
    {
        private static string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path,
               "db.sqlite");

        public static String Total(this DateTime date)
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
                  SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                var stringDate = date.ToLocalTime().ToString("s").Substring(2, 8);
                SalesByDay today = (from s in db.Table<SalesByDay>() where s.Day == stringDate select s).FirstOrDefault();
                Double val = today != null ? today.Total : 0;
                CultureInfo culture = new CultureInfo("es-MX");
                return String.Format(culture, "{0:C2}", val);
            }
        }

        public static List<StringItem> Sales(this DateTime date)
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
                  SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                var stringDate = date.ToLocalTime().ToString("s").Substring(2, 8);
                SalesByDay today = new SalesByDay();
                today = (from s in db.Table<SalesByDay>() where s.Day == stringDate select s).FirstOrDefault();
                
                return today != null ? today.Table : null;
            }
        }

        public static String IncreaseByOne(this String str)
        {
            string[] col = str.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
            col[0] = (int.Parse(col[0]) + 1).ToString();
            col[1] = (double.Parse(col[1]) + double.Parse(col[6])).ToString();
            return String.Join("\t", col);
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
        public String PayedDebtInfo { get; set; }

        [NotNull]
        public Double Total { get; set; }      

        [NotNull]
        public DateTime Date { get; set; }

        public Customer()
        {
            PayedDebt = 0;
            PayedDebtInfo = "";
            using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                db.CreateTable<Customer>();
            }
        }

        public override string ToString()
        {
            return Name;
        }

        private static string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path,
               "db.sqlite");

        public void AddToTable()
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                Customer returnedCustomer = (from s in db.Table<Customer>() where s.Id.Equals(Id) select s).FirstOrDefault();
                if (returnedCustomer != null)
                {
                    //Join debts here
                    this.Items = returnedCustomer.Items + this.Items;
                    db.Update(this);
                } else
                {
                    db.Insert(this);
                }
            }
        }

        

        public void UpdateInTable()
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                db.Update(this);
            }
        }

        public void DeleteFromTable()
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                db.Delete(this);
            }

        }

        public static List<Customer> Contains(String search)
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
                  SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                var query = (from s in db.Table<Customer>() where s.Name.Contains(search) select s);
                try { return new List<Customer>(query); }
                catch { return null; }
            }
        }

        public static List<Customer> Table
        {
            get
            {
                using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
                {
                    var query = db.Table<Customer>();
                    try { return new List<Customer>(query); }
                    catch { return null; }
                }
            }
        }

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

        

        private static string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path,
               "db.sqlite");

        [Ignore]
        public string Description
        {
            get
            {
                var culture = new CultureInfo("es-MX");
                return Type + "\t" + Size + "\t" + Code + "\t" + Brand + "\t" + Price;
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

        public Item()
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                db.CreateTable<Item>();
            }
        }

        public void AddToTable()
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                var s = db.Insert(this);
            }
        }

        public void UpdateInTable()
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                db.Update(this);
            }
        }


        public void DeleteFromTable()
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                db.Delete(this);
            }
        }

        public void DeleteOne()
        {
            if (this.Num > 0)
            {
                this.Num--;
                this.UpdateInTable();
            }
        }

        public void AddToSalesByDay()
        {
            SalesByDay today = SalesByDay.Today();
            string[] AllItems = today.ItemsString.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < AllItems.Length; i++)
            {
                if (AllItems[i].Contains(Description))
                {
                    AllItems[i] = AllItems[i].IncreaseByOne();
                    today.ItemsString = string.Join("\n", AllItems);
                    today.Total += Price;
                    today.UpdateInTable();                
                    return;
                } 
            }
            today.ItemsString += "\n1\t" + Price + "\t" + Description;
            today.Total += Price;
            today.UpdateInTable();                            
        }

        public static List<Item> Table
        {
            get
            {
                using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
                {
                    var query = db.Table<Item>();
                    try { return new List<Item>(query); }
                    catch { return null; }
                }
            }
        }

    }

    public class StringItem
    {
        public string qty { get; set; }
        public string brand { get; set; }
        public string model { get; set; }
        public string color { get; set; }
        public string size { get; set; }
        public string price { get; set; }
        public string total { get; set; }
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

        [Ignore]
        public Customer Customer { get; set; }

        public Sale()
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                db.CreateTable<Sale>();
            }
        }

        public ObservableCollection<UniqueItem> CartList = new ObservableCollection<UniqueItem>();

        public string ItemsString {
            get
            {
                string returnString = "";
                foreach (UniqueItem ui in CartList)
                {
                    returnString += ui.QtyString + ui.Item.ShortDescription + ui.Item.Price.ToString("C") + "\r";
                }
                return returnString;
            }
        }

        private static string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path,
               "db.sqlite");

        [Ignore]
        public Double TotalValue
        {
            get {
                try { return (CartList.Sum(x => ((x as UniqueItem).Item.Price * (x as UniqueItem).Qty))); }
                catch { return 0; }
            }
        }

        public void AddToTable()
        {
            Items = ItemsString;
            Date = DateTime.Now;
            Total = TotalValue;
            //Insert new sale to Sale Table
            using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                var s = db.Insert(this);
            }
            //Add each item in the cart to the sale of the day table and delete from the inventory.
            foreach (UniqueItem ui in CartList)
            {
                ui.AddToSalesByDay();
                ui.DeleteFromInventory();
            }
            if(Customer != null)
            {
                Customer.Date = DateTime.Now;
                Customer.Debt = TotalValue;
                Customer.Total = Customer.PayedDebt - Customer.Debt;
                Customer.Items = ItemsString;
                Customer.AddToTable();
            }

        }


        public bool AddItem(Item item)
        {
            if (item != null)
            {
                foreach (UniqueItem i in CartList)
                {
                    if (i.Item == item)
                    {
                        if (i.Qty < item.Num)
                        {
                            i.Qty++;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                
                CartList.Add(new UniqueItem() { Item = item,Qty=1});
                return true;

            }
            return false;
        }

        public bool RemoveItem(UniqueItem item)
        {
            if (item != null)
            {
                foreach (UniqueItem i in CartList)
                {
                    if (i == item)
                    {
                        if (i.Qty < item.Item.Num)
                        {
                            i.Qty--;
                            return true;
                        }
                        else
                        {
                            CartList.Remove(i);
                            return false;
                        }
                    }
                }
            }
            return false;
        }       

        [Ignore]
        public static List<Sale> Table
        {
            
            get
            {
                using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
                {
                    var query = db.Table<Sale>();
                    return new List<Sale>(query);
                }
            }
        }

        

    }

    public class UniqueItem
    {
        public int Qty = 1;

        public String QtyString { get
            {
                return Qty.ToString() + "×";
            }
        }

        public Item Item { get; set; }
        
        public void AddToSalesByDay()
        {
            for (int i = 0; i < Qty; i++)
            {
                Item.AddToSalesByDay();
            }
        }

        public void DeleteFromInventory()
        {
            for (int i = 0; i < Qty; i++)
            {
                Item.DeleteOne();
            }
        }
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

        public List<StringItem> Table
        {
            get
            {
                List<StringItem> list = new List<StringItem>();
                var str = ItemsString;
                string[] rows = str.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in rows)
                {
                    string[] cols = s.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    Debug.WriteLine(cols[3]);
                    list.Add(new StringItem() { qty = cols[0], total = cols[1], brand = cols[2], model = cols[3], color = cols[4], size = cols[5], price = cols[6] });
                }
                return list;
            }
        }

        private static string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path,
               "db.sqlite");

        public static SalesByDay Today()
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
              SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                String today = DateTime.Now.ToLocalTime().ToString("s").Substring(2, 8);
                new SalesByDay();
                var query = (from s in db.Table<SalesByDay>() where s.Day == today select s).FirstOrDefault();
                if (query == null)
                {
                    SalesByDay newDay = new SalesByDay() { Day = today, ItemsString = String.Empty, Total = 0 };
                    db.Insert(newDay);
                    return SalesByDay.Today();
                }
                else return query;
            }             
        }

        public SalesByDay()
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                db.CreateTable<SalesByDay>();
            }
        }


        public void AddToTable()
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                var s = db.Insert(this);
            }
        }

        public void UpdateInTable()
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                db.Update(this);
            }
        }
        
    }

    
   
}
