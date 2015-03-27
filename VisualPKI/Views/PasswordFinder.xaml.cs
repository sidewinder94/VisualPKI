using System;
using System.Windows;
using VisualPKI.Resources.Lang;
using Org.BouncyCastle.OpenSsl;

namespace VisualPKI.Views
{
    /// <summary>
    /// Logique d'interaction pour PasswordFinder.xaml
    /// </summary>
    public partial class PasswordFinder : Window, IPasswordFinder
    {

        public Visibility SetPassword { get; set; }

        public PasswordFinder(Boolean setPassword = false)
        {
            SetPassword = setPassword ? Visibility.Visible : Visibility.Collapsed;
            InitializeComponent();
            if (!setPassword)
            {
                this.Height = 150.0d;
            }
        }

        #region Implementation of IPasswordFinder

        public char[] GetPassword()
        {
            return PasswordBox.Password.ToCharArray();
        }

        #endregion

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (SetPassword == Visibility.Visible && !PasswordBox.Password.Equals(PasswordBoxConfirmation.Password))
            {
                MessageBox.Show(Strings.NotMatchingPasswords,
                                Strings.InputError,
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
            }
            else
            {
                Close();
            }
        }

    }
}
