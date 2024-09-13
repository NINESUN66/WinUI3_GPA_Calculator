using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using t.Pages;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace t
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private static MainWindow _instance;

        public MainWindow()
        {
            this.InitializeComponent();
            this.Title = "IMU_GPA_Calculator";
            navView = navView1;
            _instance = this;
        }

        public static MainWindow GetMainWindow()
        {
            return _instance;
        }

        public NavigationView navView { get; private set; }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (NavigationViewItem)args.SelectedItem;
            if ((string)selectedItem.Tag == "LoginPage") contentFrame.Navigate(typeof(LoginPage));
            else if ((string)selectedItem.Tag == "GradePage") contentFrame.Navigate(typeof(GradePage));
            else if ((string)selectedItem.Tag == "HistoryPage") contentFrame.Navigate(typeof(HistoryPage));
            else if ((string)selectedItem.Tag == "ComparePage") contentFrame.Navigate(typeof(ComparePage));
        }
    }

}