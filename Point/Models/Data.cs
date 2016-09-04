using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;
using SQLite.Net;
using System.Collections.ObjectModel;
using System.Globalization;
using Template10.Mvvm;


namespace Point.Model
{
    
    public static class ExtensionMethods
    {
        //Database location
        private static string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path,
               "db.sqlite");

        // Returns total from SalesByDay Table for a specific date.
        public static String Total(this DateTime date)
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
                  SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                
                var stringDate = date.ToLocalTime().ToString("s").Substring(2, 8);
                //Search for date in table
                SalesByDay today = (from s in db.Table<SalesByDay>() where s.Day == stringDate select s).FirstOrDefault();
                //Set value to total property if not null
                Double val = today != null ? today.Total : 0;
                CultureInfo culture = new CultureInfo("es-MX");
                //Return as string formated as currency.
                return String.Format(culture, "{0:C2}", val);
            }
        }

        public static String TotalOwed(this DateTime date)
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
                  SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                var stringDate = date.ToLocalTime().ToString("s").Substring(2, 8);
                SalesByDay today = (from s in db.Table<SalesByDay>() where s.Day == stringDate select s).FirstOrDefault();
                Double val = today != null ? today.TotalOwed : 0;
                CultureInfo culture = new CultureInfo("es-MX");
                return String.Format(culture, "{0:C2}", val);
            }
        }

        //Gets sales for a specific date as a list.
        public static List<StringItem> Sales(this DateTime date)
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
                  SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                var stringDate = date.ToLocalTime().ToString("s").Substring(2, 8);
                SalesByDay today = new SalesByDay();
                today = (from s in db.Table<SalesByDay>() where s.Day == stringDate select s).FirstOrDefault();
                //Return list if found              
                return today != null ? today.Table : null;
            }
        }

        //Increments the qty by one
        public static String IncreaseByOne(this String str)
        {
            //Splits each item into different columns

            string[] col = str.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
            //Column 0 contains quantity, column 1 contains total for the item and column 6 contains the price for the item.
            col[0] = (int.Parse(col[0]) + 1).ToString();
            col[1] = (double.Parse(col[1]) + double.Parse(col[6])).ToString();
            //Rejoin all columns and return
            return String.Join("\t", col);
        }

        public static String DecreaseByOne(this String str)
        {
            string[] col = str.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
            col[0] = (int.Parse(col[0]) - 1).ToString();
            col[1] = (double.Parse(col[1]) - double.Parse(col[6])).ToString();
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
            //Every time a customer is created if the table is not found, it is created.
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
            //Override ToString to return the name of the customer.
            return Name;
        }

        // Database location
        private static string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path,
               "db.sqlite");

        //Inserts a new customer if the customer is not already in the table, if it is, it updates it.
        public void AddToTable()
        {
            using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                //Look for customer in table
                Customer returnedCustomer = (from s in db.Table<Customer>() where s.Id.Equals(Id) select s).FirstOrDefault();
                if (returnedCustomer != null)
                {
                    //Add new items to items in items column.
                    this.Items = returnedCustomer.Items + "\r(" + DateTime.Now.ToLocalTime().ToString("d") + ")\r" +this.Items;
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

        //Return all customers whose name has string.
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

        //Return a list with all customers in database
        public static List<Customer> Table
        {
            get
            {
                using (var db = new SQLite.Net.SQLiteConnection(new
               SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
                {
                    var query = db.Table<Customer>();
                    //Return null if the table doesn't exist in database.
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
            DateAdded = DateTime.Now;
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

        //Decreases the Num column from the current item by one.
        public void DeleteOne()
        {
            if (this.Num > 0)
            {
                this.Num--;
                this.UpdateInTable();
            }
        }


        //Increases the Num column from current item by one.
        public void AddOne()
        {            
            this.Num++;
            this.UpdateInTable();         
        }

        //Adds the current item to today's row from SalesByDay table.
        public void AddToSalesByDay()
        {
            //Get today's row
            SalesByDay today = SalesByDay.Today();
            //Separate items
            string[] AllItems = today.ItemsString.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < AllItems.Length; i++)
            {
                //Look for this item in array of items. If found, increase current item by one, rejoin and update table.
                if (AllItems[i].Contains(Description))
                {
                    AllItems[i] = AllItems[i].IncreaseByOne();
                    today.ItemsString = string.Join("\n", AllItems);
                    today.Total += Price;
                    today.UpdateInTable();                
                    return;
                } 
            }
            //If not found, set the value to this item's info and update.
            today.ItemsString += "\n1\t" + Price + "\t" + Description;
            today.Total += Price;
            today.UpdateInTable();                            
        }

        //Same as above but increments TotalOwed instead of Total.
        public void AddToSalesByDayAsDebt()
        {
            SalesByDay today = SalesByDay.Today();
            string[] AllItems = today.ItemsString.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < AllItems.Length; i++)
            {
                if (AllItems[i].Contains(Description))
                {
                    AllItems[i] = AllItems[i].IncreaseByOne();
                    today.ItemsString = string.Join("\n", AllItems);
                    today.TotalOwed += Price;
                    today.UpdateInTable();
                    return;
                }
            }
            today.ItemsString += "\n1\t" + Price + "\t" + Description;
            today.TotalOwed += Price;
            today.UpdateInTable();
        }

        //Same as AddToSalesByDay but decreases Qty and Total instead.
        public void DeleteFromSalesByDay()
        {
            SalesByDay today = SalesByDay.Today();
            string[] AllItems = today.ItemsString.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < AllItems.Length; i++)
            {
                if (AllItems[i].Contains(Description))
                {
                    AllItems[i] = AllItems[i].DecreaseByOne();
                    today.ItemsString = string.Join("\n", AllItems);
                    today.Total -= Price;
                    today.UpdateInTable();
                    return;
                }
            }
            today.ItemsString += "\n1\t" + Price + "\t" + Description;
            today.Total -= Price;
            today.UpdateInTable();
        }

        public void DeleteFromSalesByDayAsDebt()
        {
            SalesByDay today = SalesByDay.Today();
            string[] AllItems = today.ItemsString.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < AllItems.Length; i++)
            {
                if (AllItems[i].Contains(Description))
                {
                    AllItems[i] = AllItems[i].DecreaseByOne();
                    today.ItemsString = string.Join("\n", AllItems);
                    today.TotalOwed -= Price;
                    today.UpdateInTable();
                    return;
                }
            }
            today.ItemsString += "\n1\t" + Price + "\t" + Description;
            today.TotalOwed -= Price;
            today.UpdateInTable();
        }

        //Returns all items in Item table in database.
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

        //Calculates the total from CartList by adding each item multiplied by it's quantity.
        [Ignore]
        public Double TotalValue
        {
            get {
                try { return (CartList.Sum(x => ((x as UniqueItem).Item.Price * (x as UniqueItem).Qty))); }
                catch { return 0; }
            }
        }

        //Adds a new sale to the Sales table and to SalesByDay table. Updates customer if necessary.
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
                ui.DeleteFromInventory();
            }
            //If there is a customer set, add as debt instead.
            if(Customer != null)
            {
                foreach (UniqueItem ui in CartList)
                {
                    ui.AddToSalesByDayAsDebt();
                }
                //Set all customer properties.
                Customer.Date = DateTime.Now;
                Customer.Debt = Customer.Debt + TotalValue;
                Customer.Total = Customer.PayedDebt - Customer.Debt;
                Customer.Items = ItemsString;
                Customer.AddToTable();
            } else
            {
                foreach (UniqueItem ui in CartList)
                {
                    ui.AddToSalesByDay();
                }
            }
        }

        //Adds a new record to the Sales table.
        public void AddRecordToTable()
        {
            if (Items != null && Date != null)
            {
                using (var db = new SQLite.Net.SQLiteConnection(new
                   SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
                {
                    var s = db.Insert(this);
                }
            }
        }

        //Increments a UniqueItem's Qty that corresponds to the received parameter.
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
                        } else 
                        {
                            i.Qty--;
                            return true;
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
            for (int i = 0; i > Qty; i--)
            {
                Item.DeleteFromSalesByDay();
            }
        }

        public void AddToSalesByDayAsDebt()
        {
            for (int i = 0; i < Qty; i++)
            {
                Item.AddToSalesByDayAsDebt();
            }
            for (int i = 0; i > Qty; i--)
            {
                Item.DeleteFromSalesByDayAsDebt();
            }
        }

        public void DeleteFromInventory()
        {
            for (int i = 0; i < Qty; i++)
            {
                Item.DeleteOne();
            }
            for (int i = 0; i > Qty; i--)
            {
                Item.AddOne();
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

        [NotNull]
        public Double TotalOwed { get; set; }

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
                    SalesByDay newDay = new SalesByDay() { Day = today, ItemsString = String.Empty, Total = 0, TotalOwed = 0 };
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
