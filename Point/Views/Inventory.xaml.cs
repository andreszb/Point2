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
        private Data data = new Data();
        private Item SelectedItemToEdit;
        public CurrentSale Cart = new CurrentSale();

        public Inventory()
        {
            this.InitializeComponent();          
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UpdateTable();
            if (App.isAdminLoggedIn)
            {
                EditItemButton.Visibility = Visibility.Visible;
                NewItemButton.Visibility = Visibility.Visible;
            } else
            {
                EditItemButton.Visibility = Visibility.Collapsed;
                NewItemButton.Visibility = Visibility.Collapsed;
            }
        }
       
        #region Command Buttons

        private void NewItemButton_Click(object sender, RoutedEventArgs e)
        {
            NISplitView.IsPaneOpen = !NISplitView.IsPaneOpen;  
        }

        private void EditItemButton_Click(object sender, RoutedEventArgs e)
        {
            EISplitView.IsPaneOpen = !EISplitView.IsPaneOpen;
        }

        private void NewSaleButton_Click(object sender, RoutedEventArgs e)
        {
            NSSplitView.IsPaneOpen = !NSSplitView.IsPaneOpen;
            // If the pane is opened, deselect item from datagrid and clear previous data.
            if (NSSplitView.IsPaneOpen)
            {
                NSCancelButton_Click(null, null);
                DataGrid.SelectedItem = null;
            }
        }

        #endregion

        #region New Item Event Handlers & Methods

        private void NIAddButton_Click(object sender, RoutedEventArgs e)
        {
            // Check all fields have a value.
            if (!string.IsNullOrWhiteSpace(NITypeBox.Text) && !string.IsNullOrWhiteSpace(NISizeBox.Text) && !string.IsNullOrWhiteSpace(NICodeBox.Text) && !string.IsNullOrWhiteSpace(NIBrandBox.Text)  && !string.IsNullOrWhiteSpace(NIPriceBox.Text) && !string.IsNullOrWhiteSpace(NINumBox.Text))
            {
                 var newItem = new Item() {
                    Type = NITypeBox.Text,
                    Size = NISizeBox.Text,
                    Code = NICodeBox.Text,
                    Brand = NIBrandBox.Text,
                    Price = double.Parse(NIPriceBox.Text),
                    Num = int.Parse(NINumBox.Text)
                };
                newItem.AddToInventory();
                NICancelButton_Click(null, null);                
                UpdateTable();
            } else
            {
                NIErrorTextBlock.Visibility = Visibility.Visible;
            }
        }

        // Clears all fields.
        private void NICancelButton_Click(object sender, RoutedEventArgs e)
        {
            NITypeBox.Text = String.Empty;
            NISizeBox.Text = String.Empty;
            NICodeBox.Text = String.Empty;
            NIBrandBox.Text = String.Empty;
            NIPriceBox.Text = String.Empty;
            NINumBox.Text = String.Empty;
            NIErrorTextBlock.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region New Sale Event Handlers & Methods

        private void UpdateCart()
        {
            CultureInfo culture = new CultureInfo("es-MX");
            NSTotalTextBlock.Text = String.Format(culture, "{0:C2}", Cart.total);
            NSCartList.ItemsSource = null;
            NSCartList.ItemsSource = Cart.items;
        }

        private void NSAcceptButton_Click(object sender, RoutedEventArgs e)
        {
            data.addNewSale(Cart);
            NSCancelButton_Click(null, null);
            UpdateTable();
        }

        private void NSCancelButton_Click(object sender, RoutedEventArgs e)
        {
            Cart = new CurrentSale();
            NSTitleTextBlock.Text = "Nueva venta";
            NSCustomerNameTextBlock.Text = String.Empty;
            NSCustomerNameTextBlock.Visibility = Visibility.Collapsed;
            UpdateCart();
        }

        // Deletes one item from the cart.
        private void NSCartList_ItemClick(object sender, ItemClickEventArgs e)
        {
            
            if (e.ClickedItem != null)
            {
                CurrentSale.uniqueItem itemClicked = e.ClickedItem as CurrentSale.uniqueItem;
                if (itemClicked.intQty > 1)
                {
                    itemClicked.intQty--;
                }
                else
                {
                    Cart.items.Remove(itemClicked);
                }
                UpdateCart();
            }
        }

        private async void NSCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            CustomerDialog Dialog = new CustomerDialog();
            await Dialog.ShowAsync();
            if (Dialog.Debtor != null)
            {
                NSTitleTextBlock.Text = "Nueva deuda";
                Cart.Person = Dialog.Debtor;
                NSCustomerNameTextBlock.Text = Dialog.Debtor.Name;
                NSCustomerNameTextBlock.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region Edit Item Event Handler & Methods

        private void EIConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItemToEdit != null)
            {
                SelectedItemToEdit.Type = EITypeBox.Text;
                SelectedItemToEdit.Size = EISizeBox.Text;
                SelectedItemToEdit.Code = EICodeBox.Text;
                SelectedItemToEdit.Price = double.Parse(EIPriceBox.Text);
                SelectedItemToEdit.Num = int.Parse(EINumBox.Text);
                SelectedItemToEdit.UpdateInInventory();
                UpdateTable();
            }
        }

        private void EICancelButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedItemToEdit = null;
            EITypeBox.Text = String.Empty;
            EISizeBox.Text = String.Empty;
            EICodeBox.Text = String.Empty;
            EIBrandBox.Text = String.Empty;
            EIPriceBox.Text = String.Empty;
            EINumBox.Text = String.Empty;
            DataGrid.SelectedItem = null;
        }

        private void EIConfirmDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItemToEdit != null)
            {
                SelectedItemToEdit.DeleteFromInventory();
                EICancelButton_Click(null, null);
                UpdateTable();
                EIDeleteFlyout.Hide();
            }
        }

        #endregion

        //Gets inventory data from database and sets it as the datagrid's source.
        private void UpdateTable()
        {
            if (DataGrid.SelectedColumn != null) {
                DataGridColumnBase Column = DataGrid.SelectedColumn;
                DataGrid.ItemsSource = data.Items;
                DataGrid.SelectColumn(Column);
                DataGrid.SelectColumn(Column);
            } else
            {
                DataGrid.ItemsSource = data.Items;
            }
        }

        

        // Takes a textbox's input and makes sure the content can still be parsed to a double when adding a new character. 
        // Stops the user from setting the content to anything other than a number.
        private void CheckDouble(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            double dtemp;
            // If the condition fails, it removes the character.
            if (!double.TryParse(sender.Text, out dtemp) && sender.Text != "")
            {
                int pos = sender.SelectionStart - 1;
                sender.Text = sender.Text.Remove(pos, 1);
                sender.SelectionStart = pos;
            }
        }

        private void TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
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

        private void AddToCart(object sender, NavigationListEventArgs e)
        {
            Item SelectedItem = DataGrid.SelectedItem as Item;

            //If a new sale is active, add selected item to the cart.
            if (NSSplitView.IsPaneOpen)
            {
                DataGrid.SelectedItem = null;
                if (SelectedItem != null)
                {
                    if (SelectedItem.Num > 0)//check this later
                    {
                        Cart.addItem(SelectedItem);
                        UpdateCart();
                    }
                }

            }
            else if (EISplitView.IsPaneOpen)
            {
                if (SelectedItem != null)
                {
                    EIDeleteButton.Visibility = Visibility.Visible;
                    if (SelectedItemToEdit != SelectedItem)
                    {
                        SelectedItemToEdit = SelectedItem as Item;
                        EITypeBox.Text = SelectedItemToEdit.Type;
                        EISizeBox.Text = SelectedItemToEdit.Size;
                        EICodeBox.Text = SelectedItemToEdit.Code;
                        EIBrandBox.Text = SelectedItemToEdit.Brand;
                        EIPriceBox.Text = SelectedItemToEdit.Price.ToString();
                        EINumBox.Text = SelectedItemToEdit.Num.ToString();
                    }
                }
                else
                {
                    EIDeleteButton.Visibility = Visibility.Collapsed;
                }
            }
        }
    }


}
