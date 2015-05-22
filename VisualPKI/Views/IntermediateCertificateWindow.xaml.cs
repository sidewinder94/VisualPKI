using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using Utils.Misc;
using Utils.Text;
using VisualPKI.Annotations;
using VisualPKI.DataStructures;
using VisualPKI.Generation;
using VisualPKI.Properties;
using VisualPKI.Resources.Lang;

namespace VisualPKI.Views
{
    /// <summary>
    /// Logique d'interaction pour IntermediateCertificateWindow.xaml
    /// </summary>
    public partial class IntermediateCertificateWindow : Window, INotifyPropertyChanged
    {

        #region properties

        private X509Certificate _caX509Certificate;

        private AsymmetricCipherKeyPair _caKeyPair;


        private String _caCertificatePath;
        private String _caPkeyPath;
        private DateTime _startDate;
        private DateTime _endDate;

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
                    if (_caX509Certificate != null && !_caX509Certificate.IsValid(value))
                    {
                        MessageBox.Show(String.Format(Strings.CACertificateExpired, _caX509Certificate.NotAfter),
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
        }

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
                    if (_caX509Certificate != null && !_caX509Certificate.IsValid(value))
                    {
                        MessageBox.Show(String.Format(Strings.CACertificateNotStarted, _caX509Certificate.NotBefore),
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
        }

        public String CACertificatePath
        {
            get
            {
                return _caCertificatePath;

            }
            set
            {
                if (File.Exists(value))
                {
                    _caX509Certificate = Certificate.ReadCertificate(value);
                }
                if (_caX509Certificate != null)
                {
                    _caCertificatePath = value;
                    if (_caX509Certificate != null)
                    {
                        MainWindow.ShowWindow<DisplayCertificateInformations>();
                        MainWindow.GetWindow<DisplayCertificateInformations>().CSRData =
                            SigningRequestData.FromX509Certificate(_caX509Certificate);
                    }
                }
                else
                {
                    MessageBox.Show(Strings.InvalidCert,
                                    Strings.InputError,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Exclamation);
                }

            }
        }

        public String CAPkeyPath
        {
            get
            {
                return _caPkeyPath;

            }
            set
            {

                if (File.Exists(value))
                {
                    _caKeyPair = PrivateKey.ReadFromFile(File.Open(value, FileMode.Open,
                                                                          FileAccess.Read,
                                                                          FileShare.Read));
                }

                if (_caKeyPair != null)
                {
                    _caPkeyPath = value;
                }
                else
                {
                    MessageBox.Show(Strings.InvalidPkey,
                                    Strings.InputError,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Exclamation);
                }

            }
        }

        public String KeyAlgorithm { get; set; }

        public int KeyStrength { get; set; }

        public SigningRequestData CSRData { get; set; }

        public String SignatureAlgorithm { get; set; }

        public String HashAlgorithm { get; set; }

        public int SerialNumber { get; set; }

        #endregion

        public IntermediateCertificateWindow()
        {
            _startDate = DateTime.Now;
            _endDate = _startDate.AddDays(1);
            CSRData = new SigningRequestData();
            SerialNumber = Settings.Default.LastGeneratedSerial;
            InitializeComponent();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var keyParameter = new Tuple<String, int>(KeyAlgorithm, KeyStrength);
            var signatureAlgorithm = String.Format("{0}with{1}", HashAlgorithm, SignatureAlgorithm);
            var couple = Certificate.CreateIntermediate(StartDate, EndDate, CSRData,
                                                        keyParameter, signatureAlgorithm,
                                                        _caX509Certificate,
                                                        _caKeyPair);


            String baseName = SavePath.RegExpReplace(@"\.\w+$", "");


            Certificate.WriteCertificate(couple.Right(), SavePath);
            var finder = new PasswordFinder(true);

            finder.ShowDialog();

            PrivateKey.WritePrivateKey(couple.Left(), String.Format("{0}.key", baseName), finder.GetPassword());

            Settings.Default.Save();
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindow.WindowClosed(GetType());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
