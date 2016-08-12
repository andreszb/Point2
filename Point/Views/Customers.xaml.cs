using System;
using System.IO;
using Point.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using SQLite.Net.Attributes;
using System.Diagnostics;
using MyToolkit.Controls;
using Point.Model;
using System.Threading.Tasks;

namespace Point.Views
{
    public sealed partial class Customers : Page
    {
        private Data data = new Data();

        public Customers()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UpdateTable();
        }

        private void UpdateTable()
        {
            CustomersDataGrid.ItemsSource = data.getCustomers();
        }

        private void textChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            //if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            //{
            //    DataGrid.SetFilter<Item>(p =>
            //            (p.Brand.ToLower().Contains(sender.Text.ToLower()) && (bool)brandCheckBox.IsChecked) ||
            //            (p.Model.ToLower().Contains(sender.Text.ToLower()) && (bool)modelCheckBox.IsChecked) ||
            //            (p.Color.ToLower().Contains(sender.Text.ToLower()) && (bool)colorCheckBox.IsChecked) ||
            //            (p.Size.ToLower().Contains(sender.Text.ToLower()) && (bool)sizeCheckBox.IsChecked) ||
            //            (p.Num.ToString().Contains(sender.Text.ToLower()) && (bool)numCheckBox.IsChecked) ||
            //            (p.Cost.ToString().Contains(sender.Text.ToLower()) && (bool)costCheckBox.IsChecked) ||
            //            (p.Price.ToString().Contains(sender.Text.ToLower()) && (bool)priceCheckBox.IsChecked) ||
            //            (p.Category.ToLower().Contains(sender.Text.ToLower()) && (bool)categoryCheckBox.IsChecked));

            //}
        }
              
    }

}
