using System;
using System.IO;
using Point.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Data;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MyToolkit.Model;
using MyToolkit.Paging;
using System.ComponentModel;
using Point.Model;
using MyToolkit.Controls;
using System.Linq;

namespace Point.Views
{
    public sealed partial class Inventory : Page
    {

        private Data data = new Data();
        public CurrentSale currentSale = new CurrentSale();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UpdateTable();

        }

        public Inventory()
        {
            this.InitializeComponent();
        }

        private void UpdateTable()
        { 
            DataGrid.ItemsSource = data.getItems();
        }

        private void newItemButton_Click(object sender, RoutedEventArgs e)
        {
            newItemSplitView.IsPaneOpen = !newItemSplitView.IsPaneOpen;
        }

        private void addNewItem_Click(object sender, RoutedEventArgs e)
        {
            data.addNewItem(
                Brand: BrandBox.Text,
                Model: ModelBox.Text,
                Color: ColorBox.Text,
                Size: SizeBox.Text,
                Num: int.Parse(NumBox.Text),
                Cost: int.Parse(CostBox.Text),
                Price: int.Parse(PriceBox.Text),
                Category: CategoryBox.SelectedItem.ToString(),
                Shortcut: ShortcutBox.Text
                );
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

        private void NewSaleButton_Click(object sender, RoutedEventArgs e)
        {
            NewSaleSplitView.IsPaneOpen = !NewSaleSplitView.IsPaneOpen;
           
        }

        private void addToCurrentSale(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var j = DataGrid.SelectedItem as Item;
            currentSale.addItem(j);
            totalTextBlock.Text = currentSale.total;
            DataGrid.SelectedItem = null;
            cartListView.ItemsSource = null;
            cartListView.ItemsSource = currentSale.items;
        }

    }
}
