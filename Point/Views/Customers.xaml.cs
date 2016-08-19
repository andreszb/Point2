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
    public sealed partial class Customers : Page
    {
        private Customer SelectedCustomerToEdit;

        public Customers()
        {
            this.InitializeComponent();
            CultureInfo.CurrentCulture = new CultureInfo("es-MX");

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (App.isAdminLoggedIn)
            {
                ECScrollViewer.Visibility = Visibility.Visible;
                PDScrollViewer.Visibility = Visibility.Collapsed;
            }
            else
            {
                ECScrollViewer.Visibility = Visibility.Collapsed;
                PDScrollViewer.Visibility = Visibility.Visible;
            }
            UpdateTable();
        }

        private void EditDebtButton_Click(object sender, RoutedEventArgs e)
        {
            ECSplitView.IsPaneOpen = !ECSplitView.IsPaneOpen;
        }

        private void UpdateTable()
        {
            DataGrid.ItemsSource = Customer.Table;
        }

        private void TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                DataGrid.SetFilter<Customer>(p =>
                        (p.Name.ToLower().Contains(sender.Text.ToLower()) && (bool)NameCheckBox.IsChecked) ||
                        (p.Date.ToLocalTime().ToString("D").Contains(sender.Text.ToLower()) && (bool)DateCheckBox.IsChecked) ||
                        (p.Date.ToLocalTime().ToString("d").Contains(sender.Text.ToLower()) && (bool)DateCheckBox.IsChecked) ||
                        (p.Address.ToLower().Contains(sender.Text.ToLower()) && (bool)AddressCheckBox.IsChecked) ||
                        (p.PhoneNumber.ToLower().Contains(sender.Text.ToLower()) && (bool)PhoneNumberCheckBox.IsChecked) ||
                        (p.Notes.ToLower().Contains(sender.Text.ToLower()) && (bool)NotesCheckBox.IsChecked) ||
                        (p.Items.ToLower().Contains(sender.Text.ToLower()) && (bool)ItemsCheckBox.IsChecked) ||
                        (p.Debt.ToString().Contains(sender.Text.ToLower()) && (bool)DebtCheckBox.IsChecked));
            }
        }

        

        #region Edit Item Event Handler & Methods

        private void ECConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCustomerToEdit != null)
            {
                if (App.isAdminLoggedIn)
                {
                    SelectedCustomerToEdit.Name = ECNameBox.Text;
                    SelectedCustomerToEdit.Items = ECItemsBox.Text;
                    SelectedCustomerToEdit.Address = ECAddressBox.Text;
                    SelectedCustomerToEdit.PhoneNumber = ECPhoneNumberBox.Text;
                    SelectedCustomerToEdit.Notes = ECNotesBox.Text;
                    SelectedCustomerToEdit.Debt = double.Parse(ECDebtBox.Text);
                    SelectedCustomerToEdit.PayedDebt = double.Parse(ECPayedDebtBox.Text);
                    SelectedCustomerToEdit.Total = SelectedCustomerToEdit.PayedDebt - SelectedCustomerToEdit.Debt;
                    SelectedCustomerToEdit.UpdateInTable();
                    ECCancelButton_Click(null, null);
                    UpdateTable();
                }
                else
                {
                    double payment = double.Parse(PDPayedDebtBox.Text);
                   if (-SelectedCustomerToEdit.Total >= payment && SelectedCustomerToEdit.Total != 0)
                    {                        
                            SelectedCustomerToEdit.PayedDebt = SelectedCustomerToEdit.PayedDebt + double.Parse(PDPayedDebtBox.Text);
                            SelectedCustomerToEdit.Total = SelectedCustomerToEdit.PayedDebt - SelectedCustomerToEdit.Debt;
                        SelectedCustomerToEdit.PayedDebtInfo += double.Parse(PDPayedDebtBox.Text).ToString("c") + " " + DateTime.Now.ToLocalTime().ToString("d") + "\r";
                        SelectedCustomerToEdit.UpdateInTable();
                        ECCancelButton_Click(null, null);
                            UpdateTable();        
                    } else
                    {
                        ECErrorTextBlock.Visibility = Visibility.Visible;
                    }
                        
                    
                    
                }
                
            }
        }

        private void ECCancelButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedCustomerToEdit = null;
            ECNameBox.Text = String.Empty;
            ECItemsBox.Text = String.Empty;
            ECAddressBox.Text = String.Empty;
            ECPhoneNumberBox.Text = String.Empty;
            ECNotesBox.Text = String.Empty;
        
            ECDebtBox.Text = String.Empty;
            ECPayedDebtBox.Text = String.Empty;
            DataGrid.SelectedItem = null;
            ECNameBox.Header = String.Empty;
            ECAddressBox.Header = String.Empty;
            ECPhoneNumberBox.Header = String.Empty;
            ECNotesBox.Header = String.Empty;
            ECItemsBox.Header = String.Empty;
            ECDebtBox.Header = String.Empty;
            ECPayedDebtBox.Header = String.Empty;
            PDPayedDebtBox.Text = String.Empty;
            PDNameTextBlock.Text = "Nombre";
            PDItemsTextBlock.Text = "Compra";
            PDDebtBox.Text = "Deuda";
            ECDeleteButton.Visibility = Visibility.Collapsed;
            ECErrorTextBlock.Visibility = Visibility.Collapsed;            
        }

        private void ECConfirmDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCustomerToEdit != null)
            {
                SelectedCustomerToEdit.DeleteFromTable();
                ECCancelButton_Click(null, null);
                UpdateTable();
                ECDeleteFlyout.Hide();
            }
        }

        #endregion

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
            ECErrorTextBlock.Visibility = Visibility.Collapsed;

        }



        private void SelectCustomer(object sender, NavigationListEventArgs e)
        {
            Customer SelectedCustomer = DataGrid.SelectedItem as Customer;
            
                if (SelectedCustomer != null)
                {
                    if (App.isAdminLoggedIn)
                    {
                        ECDeleteButton.Visibility = Visibility.Visible;
                        ECNameBox.Header = "Nombre";
                        ECAddressBox.Header = "Direccion";
                    ECPhoneNumberBox.Header = "Telefono";
                    ECNotesBox.Header = "Comentarios";
                        ECItemsBox.Header = "Compra";
                        ECDebtBox.Header = "Deuda";
                        ECPayedDebtBox.Header = "Pagado";

                        if (SelectedCustomerToEdit != SelectedCustomer)
                        {
                            SelectedCustomerToEdit = SelectedCustomer as Customer;
                            ECNameBox.Text = SelectedCustomerToEdit.Name;
                            ECAddressBox.Text = SelectedCustomerToEdit.Address;
                        ECPhoneNumberBox.Text = SelectedCustomerToEdit.PhoneNumber;
                        ECNotesBox.Text = SelectedCustomerToEdit.Notes;
                            ECItemsBox.Text = SelectedCustomerToEdit.Items;
                            ECDebtBox.Text = SelectedCustomerToEdit.Debt.ToString();
                            ECPayedDebtBox.Text = SelectedCustomerToEdit.PayedDebt.ToString();
                        }
                    } else
                    {
                        if (SelectedCustomerToEdit != SelectedCustomer)
                        {
                            SelectedCustomerToEdit = SelectedCustomer as Customer;
                            PDNameTextBlock.Text = SelectedCustomerToEdit.Name;
                            PDItemsTextBlock.Text = SelectedCustomerToEdit.Items;
                            PDDebtBox.Text = SelectedCustomerToEdit.Total.ToString("c");
                        }
                    }
                    
                }
                else
                {
                    ECCancelButton_Click(null, null);
                }
            
            
        }
    }

}
