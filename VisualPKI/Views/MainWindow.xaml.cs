using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using Utils.WPF;

namespace VisualPKI.Views
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<Type, Window> _instantiatedWindows = new Dictionary<Type, Window>();
        public MainWindow()
        {
            InitializeComponent();
            this.CenterWindow();
        }

        private void SelfSignButton_Click(object sender, RoutedEventArgs e)
        {
            SelfSignedCertificateWindow window = null;
            if (!_instantiatedWindows.ContainsKey(typeof(SelfSignedCertificateWindow)))
            {
                window = new SelfSignedCertificateWindow();
                this._instantiatedWindows.Add(window.GetType(), window);
            }
            else
            {
                window = (SelfSignedCertificateWindow)_instantiatedWindows[typeof(SelfSignedCertificateWindow)];
            }

            window.Show();
            window.Focus();
        }

    }
}
