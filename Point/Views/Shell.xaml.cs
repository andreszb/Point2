using System;
using Template10.Controls;
using Template10.Services.NavigationService;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI;
namespace Point.Views
{
    

    public sealed partial class Shell : Page
    {
        public static Shell Instance { get; set; }
        public static HamburgerMenu HamburgerMenu => Instance.MyHamburgerMenu;

        public Shell()
        {
            Instance = this;
            InitializeComponent();
            LoginModal.IsModal = false;
        }

        public Shell(INavigationService navigationService) : this()
        {
            SetNavigationService(navigationService);
        }

        public void SetNavigationService(INavigationService navigationService)
        {
            MyHamburgerMenu.NavigationService = navigationService;
        }
       
        #region Login

        private void LoginTapped(object sender, RoutedEventArgs e)
        {
            if (App.isAdminLoggedIn)
            {
                App.isAdminLoggedIn = false;
                MyHamburgerMenu.NavigationService.Refresh();
                LoginTextBlock.Text = "Administrador";
                LoginTextBlock.Foreground = new SolidColorBrush(Colors.White);
                LoginIcon.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
                LoginModal.IsModal = true;
                LoginTextBlock.Text = "Cerrar sesion";
                LoginTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                LoginIcon.Foreground = new SolidColorBrush(Colors.Red);

            }
        }

        private void LoginHide(object sender, System.EventArgs e)
        {
            LoginModal.IsModal = false;
            LoginTextBlock.Text = "Administrador";
            LoginTextBlock.Foreground = new SolidColorBrush(Colors.White);
            LoginIcon.Foreground = new SolidColorBrush(Colors.White);
        }

        private void LoginLoggedIn(object sender, EventArgs e)
        {
            LoginModal.IsModal = false;
            App.isAdminLoggedIn = true;
            MyHamburgerMenu.NavigationService.Refresh();

        }

        #endregion
    }
}

