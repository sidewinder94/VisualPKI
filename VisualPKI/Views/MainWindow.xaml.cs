using System;
using System.Collections.Generic;
using System.Windows;
using Utils.WPF;
using VisualPKI.Properties;

namespace VisualPKI.Views
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly Dictionary<Type, Window> InstantiatedWindows = new Dictionary<Type, Window>();
        public MainWindow()
        {
            InitializeComponent();
            this.CenterWindow();
        }

        private void SelfSignButton_Click(object sender, RoutedEventArgs e)
        {
            ShowWindow<SelfSignedCertificateWindow>();
        }

        private void IntermediateCertificateButton_Click(object sender, RoutedEventArgs e)
        {
            ShowWindow<IntermediateCertificateWindow>();
        }


        private void SignCertificateRequestButton_Click(object sender, RoutedEventArgs e)
        {
            ShowWindow<SignCrWindow>();
        }

        private void DisplayLastCertificateInfo_Click(object sender, RoutedEventArgs e)
        {
            ShowWindow<DisplayCertificateInformations>();
        }

        public static void ShowWindow<T>() where T : Window, new()
        {
            T window;
            if (!InstantiatedWindows.ContainsKey(typeof(T)))
            {
                window = new T();
                InstantiatedWindows.Add(window.GetType(), window);
            }
            else
            {
                window = (T)InstantiatedWindows[typeof(T)];
            }
            window.Show();
            window.Focus();
        }

        public static T GetWindow<T>() where T : Window
        {
            return (T)InstantiatedWindows[typeof(T)];
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.Save();
        }
    }
}
