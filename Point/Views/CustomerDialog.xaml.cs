using Point.Model;
using System;

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using Windows.UI.Xaml.Media;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Point.Views
{
    public sealed partial class CustomerDialog : ContentDialog
    {
        
        public Customer Debtor { get; set; }
        private bool isReturningCustomer = false;
        public CustomerDialog()
        {
            this.InitializeComponent();
            
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            
                if (!isReturningCustomer)
                {
                    Debtor = new Customer() { Name = NameBox.Text, Address = AddressBox.Text, PhoneNumber = PhoneBox.Text, Notes = NoteBox.Text };
                }                 
            
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Debtor = null;
        }

        private void NameBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (String.IsNullOrEmpty(sender.Text))
                {
                    sender.ItemsSource = null;
                } else if (isReturningCustomer)
                {
                    sender.ItemsSource = Customer.Contains(sender.Text);
                    isReturningCustomer = false;
                    AddressBox.Text = String.Empty;
                    PhoneBox.Text = String.Empty;
                    NoteBox.Text = String.Empty;
                    AddressBox.IsEnabled = true;
                    PhoneBox.IsEnabled = true;
                    NoteBox.IsEnabled = true;
                }

                else
                {
                    sender.ItemsSource = Customer.Contains(sender.Text);
                }
            }
        }

        private void NameBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            Debtor = args.SelectedItem as Customer;
            NameBox.Text = (args.SelectedItem as Customer).Name;
            AddressBox.Text = App.isAdminLoggedIn ? (args.SelectedItem as Customer).Address : "Direccion de " + (args.SelectedItem as Customer).Name;
            PhoneBox.Text = App.isAdminLoggedIn ? (args.SelectedItem as Customer).PhoneNumber : "Terminacion " + (args.SelectedItem as Customer).PhoneNumber.Substring(Math.Max(0, (args.SelectedItem as Customer).PhoneNumber.Length - 4));
            NoteBox.Text = (args.SelectedItem as Customer).Notes;
            AddressBox.IsEnabled = false;
            PhoneBox.IsEnabled = false;
            NoteBox.IsEnabled = false;
            isReturningCustomer = true;
        }

        private void NameBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {

        }
       
        private void BoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(NameBox.Text) || String.IsNullOrEmpty(AddressBox.Text) || String.IsNullOrEmpty(PhoneBox.Text) || String.IsNullOrEmpty(NoteBox.Text))
            {
                this.IsPrimaryButtonEnabled = false;
            }
            else
            {
                this.IsPrimaryButtonEnabled = true;
            }
            var redBorder = new SolidColorBrush(Colors.Red);
            var alphaBorder = new SolidColorBrush(Colors.Transparent);
            NameBox.BorderBrush = (String.IsNullOrEmpty(NameBox.Text)) ? redBorder : alphaBorder;
            AddressBox.BorderBrush = (String.IsNullOrEmpty(AddressBox.Text)) ? redBorder : alphaBorder;
            PhoneBox.BorderBrush = (String.IsNullOrEmpty(PhoneBox.Text)) ? redBorder : alphaBorder;
            NoteBox.BorderBrush = (String.IsNullOrEmpty(NoteBox.Text)) ? redBorder : alphaBorder;
        }
    }
}
