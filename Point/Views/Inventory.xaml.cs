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
using System.Globalization;

namespace Point.Views
{

    public sealed partial class Inventory : Page
    {
        private bool? brandCB {get;set;}
        private Data data = new Data();
        public CurrentSale currentSale = new CurrentSale();
        private Item selectedItemToEdit;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UpdateTable();

        }

        public Inventory()
        {
            this.InitializeComponent();
            data.addNewItemToSaleByDay(new Item()
            {
                Brand = "hdfsdf",
                Model = "blatyrtygsbla",
                Color = "pop",
                Size = "M",
                Num = 1,
                Cost = 3,
                Price = 5,
                Category = "Caca",
                DateAdded = DateTime.Now
            }
            );
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
            if (vStr(BrandBox.Text) && vStr(ModelBox.Text) && vStr(ColorBox.Text) && vStr(SizeBox.Text) && CategoryBox.SelectedItem != null && vStr(NumBox.Text) && vStr(PriceBox.Text) && vStr(CostBox.Text))
            {
                data.addNewItem(
                    Brand: BrandBox.Text,
                    Model: ModelBox.Text,
                    Color: ColorBox.Text,
                    Size: SizeBox.Text,
                    Num: int.Parse(NumBox.Text),
                    Cost: double.Parse(CostBox.Text),
                    Price: double.Parse(PriceBox.Text),
                    Category: CategoryBox.SelectedItem.ToString()
                    );
                BrandBox.Text = String.Empty;
                ModelBox.Text = String.Empty;
                ColorBox.Text = String.Empty;
                SizeBox.Text = String.Empty;
                NumBox.Text = String.Empty;
                CostBox.Text = String.Empty;
                PriceBox.Text = String.Empty;
                CategoryBox.SelectedItem = null;
                UpdateTable();
            }
        }

        public bool vStr(string text)
        {
            return !string.IsNullOrWhiteSpace(text);
        }
        

        private void textChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                DataGrid.SetFilter<Item>(p =>
                        (p.Brand.ToLower().Contains(sender.Text.ToLower())&&(bool)brandCheckBox.IsChecked) ||
                        (p.Model.ToLower().Contains(sender.Text.ToLower()) && (bool)modelCheckBox.IsChecked) ||
                        (p.Color.ToLower().Contains(sender.Text.ToLower()) && (bool)colorCheckBox.IsChecked) ||
                        (p.Size.ToLower().Contains(sender.Text.ToLower()) && (bool)sizeCheckBox.IsChecked) ||
                        (p.Num.ToString().Contains(sender.Text.ToLower()) && (bool)numCheckBox.IsChecked) ||
                        (p.Cost.ToString().Contains(sender.Text.ToLower()) && (bool)costCheckBox.IsChecked) ||
                        (p.Price.ToString().Contains(sender.Text.ToLower()) && (bool)priceCheckBox.IsChecked) ||
                        (p.Category.ToLower().Contains(sender.Text.ToLower()) && (bool)categoryCheckBox.IsChecked));

            }
        }

        private void NewSaleButton_Click(object sender, RoutedEventArgs e)
        {
            NewSaleSplitView.IsPaneOpen = !NewSaleSplitView.IsPaneOpen;
            if (NewSaleSplitView.IsPaneOpen)
            {
                cancelNewSale_Click(null,null);
                DataGrid.SelectedItem = null;
            }
           
        }


        

        private void updateNewSalePane()
        {
            CultureInfo culture = new CultureInfo("es-MX"); 
            totalTextBlock.Text = String.Format(culture, "{0:C2}", currentSale.total);
            cartListView.ItemsSource = null;
            cartListView.ItemsSource = currentSale.items;
        }

        private void addToCurrentSale(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (NewSaleSplitView.IsPaneOpen)
            {
 
                var j = DataGrid.SelectedItem as Item;
                currentSale.addItem(j);
                DataGrid.SelectedItem = null;
                updateNewSalePane();
            } else if (EditItemSplitView.IsPaneOpen)
            {
                if (DataGrid.SelectedItem != null)
                {
                    deleteItemFromDBButton.Visibility = Visibility.Visible;
                    if (selectedItemToEdit != DataGrid.SelectedItem)
                    {
                        selectedItemToEdit = DataGrid.SelectedItem as Item;
                        editBrandBox.Text = selectedItemToEdit.Brand;
                        editModelBox.Text = selectedItemToEdit.Model;
                        editColorBox.Text = selectedItemToEdit.Color;
                        editSizeBox.Text = selectedItemToEdit.Size;
                        editNumBox.Text = selectedItemToEdit.Num.ToString();
                        editCostBox.Text = selectedItemToEdit.Cost.ToString();
                        editPriceBox.Text = selectedItemToEdit.Price.ToString();
                        editCategoryBox.PlaceholderText = selectedItemToEdit.Category;
                    }
                } else
                {
                    deleteItemFromDBButton.Visibility = Visibility.Collapsed;
                }
                
            }
            
        }

        private void checkDouble(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            double dtemp;
            if (!double.TryParse(sender.Text, out dtemp) && sender.Text != "")
            {
                int pos = sender.SelectionStart - 1;
                sender.Text = sender.Text.Remove(pos, 1);
                sender.SelectionStart = pos;
            }
        }

        private void cancelNewItem_Click(object sender, RoutedEventArgs e)
        {
            BrandBox.Text = String.Empty;
            ModelBox.Text = String.Empty;
            ColorBox.Text = String.Empty;
            SizeBox.Text = String.Empty;
            NumBox.Text = String.Empty;
            CostBox.Text = String.Empty;
            PriceBox.Text = String.Empty;
            CategoryBox.SelectedItem = null;
        }

        private void cancelNewSale_Click(object sender, RoutedEventArgs e)
        {
            currentSale = new CurrentSale();
            updateNewSalePane();
        }

        private void cartListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem != null)
            {
                CurrentSale.uniqueItem itemClicked = e.ClickedItem as CurrentSale.uniqueItem;
                if (itemClicked.intQty > 1)
                {
                    itemClicked.intQty--;
                } else
                {
                    currentSale.items.Remove(itemClicked);
                }
                updateNewSalePane();
            }
        }

        private void editItemButton_Click(object sender, RoutedEventArgs e)
        {
            EditItemSplitView.IsPaneOpen = !EditItemSplitView.IsPaneOpen;

        }

        private void confirmEditButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedItemToEdit != null)
            {
                selectedItemToEdit.Brand = editBrandBox.Text;
                selectedItemToEdit.Model = editModelBox.Text;
                selectedItemToEdit.Color = editColorBox.Text;
                selectedItemToEdit.Size = editSizeBox.Text;
                selectedItemToEdit.Num = int.Parse(editNumBox.Text);
                selectedItemToEdit.Cost = double.Parse(editCostBox.Text);
                selectedItemToEdit.Price = double.Parse(editPriceBox.Text);
                selectedItemToEdit.Category = editCategoryBox.SelectedItem == null ? editCategoryBox.PlaceholderText : editCategoryBox.SelectedItem.ToString();
                data.updateItem(selectedItemToEdit);
                UpdateTable();
            }
        }

        private void cancelEditItem_Click(object sender, RoutedEventArgs e)
        {
            selectedItemToEdit = null;
            editBrandBox.Text = String.Empty;
            editBrandBox.Text = String.Empty;
            editModelBox.Text = String.Empty;
            editColorBox.Text = String.Empty;
            editSizeBox.Text = String.Empty;
            editNumBox.Text = String.Empty;
            editCostBox.Text = String.Empty;
            editPriceBox.Text = String.Empty;
            editCategoryBox.SelectedItem = null;
            editCategoryBox.PlaceholderText = "Categoria";
            DataGrid.SelectedItem = null;
        }

        private void deleteItemFromDBButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedItemToEdit != null)
            {
                data.deleteItem(selectedItemToEdit);
                cancelEditItem_Click(null, null);
                UpdateTable();
                deleteFlyout.Hide();
            
            }
        }

        private void addNewSaleButton_Click(object sender, RoutedEventArgs e)
        {
            data.addNewSale(currentSale);
            cancelNewSale_Click(null, null);
            
        }
    }


}
