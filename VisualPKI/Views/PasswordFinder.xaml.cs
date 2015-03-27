using System;
using System.Windows;

using Org.BouncyCastle.OpenSsl;

namespace VisualPKI.Views
{
    /// <summary>
    /// Logique d'interaction pour PasswordFinder.xaml
    /// </summary>
    public partial class PasswordFinder : Window, IPasswordFinder
    {

        public PasswordFinder()
        {
            InitializeComponent();
        }

        #region Implementation of IPasswordFinder

        public char[] GetPassword()
        {
            return PasswordBox.Password.ToCharArray();
        }

        #endregion

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
