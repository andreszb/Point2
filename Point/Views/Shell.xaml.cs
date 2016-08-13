using System;
using System.ComponentModel;
using System.Linq;
using Template10.Common;
using Template10.Controls;
using Template10.Services.NavigationService;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;

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
            LoginModal.IsModal = true;
        }

        private void LoginHide(object sender, System.EventArgs e)
        {
            LoginButton.IsEnabled = true;
            LoginModal.IsModal = false;
        }

        private void LoginLoggedIn(object sender, EventArgs e)
        {
            LoginButton.IsEnabled = false;
            LoginModal.IsModal = false;
            App.isAdminLoggedIn = true;
            MyHamburgerMenu.NavigationService.Refresh();
        }

        #endregion
    }
}

