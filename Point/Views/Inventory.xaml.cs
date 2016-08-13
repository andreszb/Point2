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

            

        public Inventory()
        {
            this.InitializeComponent();          
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UpdateTable();
            if (App.isAdminLoggedIn)
            {
                editItemButton.Visibility = Visibility.Visible;
                newItemButton.Visibility = Visibility.Visible;
            } else
            {
                editItemButton.Visibility = Visibility.Collapsed;
                newItemButton.Visibility = Visibility.Collapsed;
            }
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
            if (!string.IsNullOrWhiteSpace(TypeBox.Text) && !string.IsNullOrWhiteSpace(SizeBox.Text) && !string.IsNullOrWhiteSpace(CodeBox.Text) && !string.IsNullOrWhiteSpace(BrandBox.Text)  && !string.IsNullOrWhiteSpace(PriceBox.Text) && !string.IsNullOrWhiteSpace(NumBox.Text))
            {
                data.addNewItem(
                    Type: TypeBox.Text,
                    Size: SizeBox.Text,
                    Code: CodeBox.Text,
                    Brand: BrandBox.Text,
                    Price: double.Parse(PriceBox.Text),
                    Num: int.Parse(NumBox.Text)
                );
                cancelNewItem_Click(null, null);                
                UpdateTable();
            }
        }
      
        private void textChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                DataGrid.SetFilter<Item>(p =>
                        (p.Type.ToLower().Contains(sender.Text.ToLower())&&(bool)TypeCheckBox.IsChecked) ||
                        (p.Size.ToLower().Contains(sender.Text.ToLower()) && (bool)SizeCheckBox.IsChecked) ||
                        (p.Code.ToLower().Contains(sender.Text.ToLower()) && (bool)CodeCheckBox.IsChecked) ||
                        (p.Brand.ToLower().Contains(sender.Text.ToLower()) && (bool)BrandCheckBox.IsChecked) ||
                        (p.Num.ToString().Contains(sender.Text.ToLower()) && (bool)NumCheckBox.IsChecked) ||
                        (p.Price.ToString().Contains(sender.Text.ToLower()) && (bool)PriceCheckBox.IsChecked));
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
                if (j != null) {
                    if (j.Num > 0)
                    {
                        //var itemInSale = currentSale.items.First(x => (x as CurrentSale.uniqueItem).item == j);
                        currentSale.addItem(j);
                        updateNewSalePane();
                    }
                }
                DataGrid.SelectedItem = null;
            }
            else if (EditItemSplitView.IsPaneOpen)
            {
                if (DataGrid.SelectedItem != null)
                {
                    deleteItemFromDBButton.Visibility = Visibility.Visible;
                    if (selectedItemToEdit != DataGrid.SelectedItem)
                    {
                        selectedItemToEdit = DataGrid.SelectedItem as Item;
                        EditTypeBox.Text = selectedItemToEdit.Type;
                        EditSizeBox.Text = selectedItemToEdit.Size;
                        EditCodeBox.Text = selectedItemToEdit.Code;
                        EditBrandBox.Text = selectedItemToEdit.Brand;
                        EditPriceBox.Text = selectedItemToEdit.Price.ToString();
                        EditNumBox.Text = selectedItemToEdit.Num.ToString();
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
            TypeBox.Text = String.Empty;
            SizeBox.Text = String.Empty;
            CodeBox.Text = String.Empty;
            BrandBox.Text = String.Empty;
            PriceBox.Text = String.Empty;
            NumBox.Text = String.Empty;            
        }

        private void cancelNewSale_Click(object sender, RoutedEventArgs e)
        {
            currentSale = new CurrentSale();
            SalePaneTitle.Text = "Nueva venta";
            ClientNameTextBlock.Text = String.Empty;
            ClientNameTextBlock.Visibility = Visibility.Collapsed;
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
                selectedItemToEdit.Type = EditTypeBox.Text;
                selectedItemToEdit.Size = EditSizeBox.Text;
                selectedItemToEdit.Code = EditCodeBox.Text;
                selectedItemToEdit.Size = EditSizeBox.Text;
                selectedItemToEdit.Price = double.Parse(EditPriceBox.Text);
                selectedItemToEdit.Num = int.Parse(EditNumBox.Text);
                data.updateItem(selectedItemToEdit);
                UpdateTable();
            }
        }

        private void cancelEditItem_Click(object sender, RoutedEventArgs e)
        {
            selectedItemToEdit = null;
            EditTypeBox.Text = String.Empty;
            EditSizeBox.Text = String.Empty;
            EditCodeBox.Text = String.Empty;
            EditBrandBox.Text = String.Empty;
            EditPriceBox.Text = String.Empty;
            EditNumBox.Text = String.Empty;          
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
            UpdateTable();
        }

        private async void CustomerButton_Click(object sender, RoutedEventArgs e)
        {
            CustomerDialog dialog = new CustomerDialog();
            await dialog.ShowAsync();
            if (!String.IsNullOrEmpty(dialog.CustomerName))
            {
                SalePaneTitle.Text = "Nueva deuda";
                currentSale.CustomerName = dialog.CustomerName;
                currentSale.ContactInfo = dialog.ContactInfo;
                ClientNameTextBlock.Text = dialog.CustomerName;
                ClientNameTextBlock.Visibility = Visibility.Visible;
            }
        }

    }


}
