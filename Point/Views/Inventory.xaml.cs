using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Point.Model;
using MyToolkit.Controls;
using System.Globalization;

namespace Point.Views
{
    public sealed partial class Inventory : Page
    {
        private Item SelectedItemToEdit;
        public Sale Cart = new Sale();

        public Inventory()
        {
            CultureInfo.CurrentCulture = new CultureInfo("es-MX");
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
            NSSplitView.IsPaneOpen = false;
            EISplitView.IsPaneOpen = false;
        }

        private void EditItemButton_Click(object sender, RoutedEventArgs e)
        {
            EISplitView.IsPaneOpen = !EISplitView.IsPaneOpen;
            NSSplitView.IsPaneOpen = false;
        }

        private void NewSaleButton_Click(object sender, RoutedEventArgs e)
        {
            NSSplitView.IsPaneOpen = !NSSplitView.IsPaneOpen;
            EISplitView.IsPaneOpen = false;
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
                newItem.AddToTable();
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
            NSTotalTextBlock.Text = String.Format(culture, "{0:C2}", Cart.TotalValue);
            NSCartList.ItemsSource = null;
            NSCartList.ItemsSource = Cart.CartList;
        }

        private void NSAcceptButton_Click(object sender, RoutedEventArgs e)
        {
            Cart.AddToTable();
            NSCancelButton_Click(null, null);
            UpdateTable();
        }

        private void NSCancelButton_Click(object sender, RoutedEventArgs e)
        {
            Cart = new Sale();
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
                Cart.RemoveItem(e.ClickedItem as UniqueItem);                
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
                Cart.Customer = Dialog.Debtor;
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
                SelectedItemToEdit.Brand = EIBrandBox.Text;
                SelectedItemToEdit.Price = double.Parse(EIPriceBox.Text);
                SelectedItemToEdit.Num = int.Parse(EINumBox.Text);
                SelectedItemToEdit.UpdateInTable();
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
                SelectedItemToEdit.DeleteFromTable();
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
                DataGrid.ItemsSource = Item.Table;
                DataGrid.SelectColumn(Column);
                DataGrid.SelectColumn(Column);
            } else
            {
                DataGrid.ItemsSource = Item.Table;
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
                    if (SelectedItem.Num >= 0)
                    {                       
                        Cart.AddItem(SelectedItem);
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
