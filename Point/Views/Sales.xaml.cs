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
using System.Globalization;

namespace Point.Views
{
    public sealed partial class Sales : Page
    {
        public Sales()
        {
            this.InitializeComponent();
            CultureInfo.CurrentCulture = new CultureInfo("es-MX");
            DataGridDay.ItemsSource = DateTime.Now.ToLocalTime().Sales();
            TotalSalesByDay.Text = DateTime.Now.ToLocalTime().Total();
            DateTextBlock.Text = DateTime.Now.ToLocalTime().ToString("D");

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UpdateTable();
        }

        private void UpdateTable()
        {
            DataGrid.ItemsSource = Sale.Table;
        }

        private void TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                DataGrid.SetFilter<Sale>(p =>
                        (p.Items.ToLower().Contains(sender.Text.ToLower()) && (bool)InfoCheckBox.IsChecked) ||
                        (p.Date.ToLocalTime().ToString("D").Contains(sender.Text.ToLower()) && (bool)DayCheckBox.IsChecked) ||
                        (p.Date.ToLocalTime().ToString("d").Contains(sender.Text.ToLower()) && (bool)DayCheckBox.IsChecked) ||
                        (p.Date.ToLocalTime().ToString("t").Contains(sender.Text.ToLower()) && (bool)HourCheckBox.IsChecked) ||
                        (p.Total.ToString().Contains(sender.Text.ToLower()) && (bool)TotalCheckBox.IsChecked));
            }
        }






        private void CalendarView_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            if (sender.SelectedDates.Count > 0)
            {
                DataGridDay.ItemsSource = args.AddedDates[0].LocalDateTime.Sales();
                TotalSalesByDay.Text = args.AddedDates[0].LocalDateTime.Total();
                DateTextBlock.Text = args.AddedDates[0].LocalDateTime.ToString("D");
            }
            GridDay.Visibility = Visibility.Visible;
            salesViewGrid.Visibility = Visibility.Collapsed;

        }

        private void AllSales_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.Visibility = Visibility.Visible;
            GridDay.Visibility = Visibility.Collapsed;
            salesViewGrid.Visibility = Visibility.Visible;
            UpdateTable();
        }
    }

}
