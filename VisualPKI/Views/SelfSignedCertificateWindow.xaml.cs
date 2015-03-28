using System;
using System.ComponentModel;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Win32;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Utils.Misc;
using Utils.Text;
using Utils.WPF;
using VisualPKI.DataStructures;
using VisualPKI.Generation;
using VisualPKI.Properties;
using VisualPKI.Resources.Lang;

namespace VisualPKI.Views
{
    /// <summary>
    /// Logique d'interaction pour SelfSignedCertificateWindow.xaml
    /// </summary>
    public partial class SelfSignedCertificateWindow : Window, INotifyPropertyChanged
    {
        #region Attributes and auto properties
        private DateTime _endDate;
        private DateTime _startDate;
        private String _privateKeyPath;
        private String _signatureAlgorithm;
        public String KeyAlgorithm { get; set; }
        public int KeyStrength { get; set; }

        public String SignatureAlgorithm
        {
            get { return _signatureAlgorithm; }
            set
            {
                _signatureAlgorithm = value;
                OnPropertyChanged();
            }
        }

        public String HashAlgorithm { get; set; }
        public String SavePath { get; set; }

        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                if (value < StartDate)
                {
                    MessageBox.Show(Strings.IncorrectEndDate,
                                    Strings.InputError,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Exclamation);
                }
                else
                {
                    _endDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public SigningRequestData CSRData { get; private set; }

        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                if (value > EndDate)
                {
                    MessageBox.Show(Strings.IncorrectStartDate,
                                    Strings.InputError,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Exclamation);
                }
                else
                {
                    _startDate = value;
                    OnPropertyChanged();
                }

            }
        }

        public int SerialNumber { get; private set; }

        public String PrivateKeyPath
        {
            get { return _privateKeyPath; }
            set
            {
                _privateKeyPath = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public SelfSignedCertificateWindow()
        {
            _startDate = DateTime.Now;
            _endDate = DateTime.Now.AddDays(1);
            CSRData = new SigningRequestData();
            SerialNumber = Settings.Default.LastGeneratedSerial;

            InitializeComponent();
            DataContext = this;
        }


        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var keyParameter = new Tuple<String, int>(KeyAlgorithm, KeyStrength);
            var signatureAlgorithm = String.Format("{0}with{1}", HashAlgorithm, SignatureAlgorithm);

            var couple = SelfSignedCertificate.Create(StartDate, EndDate, CSRData, _privateKeyPath, keyParameter, signatureAlgorithm);

            String baseName = SavePath.RegExpReplace(@"\.\w+$", "");

            SelfSignedCertificate.WriteCertificate(couple.Right(), SavePath);
            var finder = new PasswordFinder(true);

            finder.ShowDialog();

            PrivateKey.WritePrivateKey(couple.Left(), String.Format("{0}.key", baseName), finder.GetPassword());

            Settings.Default.Save();
        }




        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindow.InstantiatedWindows.Remove(GetType());
        }
    }
}
