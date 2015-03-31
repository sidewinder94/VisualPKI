using System;

using System.ComponentModel;
using System.IO;

using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Windows;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
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
    /// Logique d'interaction pour SignCRWindow.xaml
    /// </summary>
    public partial class SignCrWindow : Window, INotifyPropertyChanged
    {

        private X509Certificate _caX509Certificate;

        private AsymmetricCipherKeyPair _caKeyPair;

        private Pkcs10CertificationRequest _csr;



        private SigningRequestData _csrData;
        private String _caCertificatePath;
        private String _caPkeyPath;
        private String _csrPath;
        private DateTime _startDate;
        private DateTime _endDate;

        public String CSRequestPath
        {
            get { return _csrPath; }
            set
            {
                _csr = null;
                if (File.Exists(value))
                {
                    _csr = CertificateSigningRequest.ReadFromFile(value);
                }
                if (_csr != null)
                {
                    _csrPath = value;
                    CSRData = SigningRequestData.FromCSR(_csr);
                    OnPropertyChanged();
                }
                else
                {
                    MessageBox.Show(Strings.InvalidCSR,
                                    Strings.InputError,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Exclamation);

                }
            }
        }

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
                    MainWindow.ShowWindow<DisplayCertificateInformations>();
                    MainWindow.GetWindow<DisplayCertificateInformations>().CSRData =
                        SigningRequestData.FromX509Certificate(_caX509Certificate);
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

        public SigningRequestData CSRData
        {
            get { return _csrData ?? (_csrData = new SigningRequestData()); }
            set
            {
                _csrData = value;
                OnPropertyChanged();
            }
        }

        public String SignatureAlgorithm { get; set; }

        public String HashAlgorithm { get; set; }

        public int SerialNumber { get; set; }



        public SignCrWindow()
        {
            _startDate = DateTime.Now;
            _endDate = _startDate.AddDays(1);
            CSRData = new SigningRequestData();
            InitializeComponent();
        }


        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var signatureAlgorithm = String.Format("{0}with{1}", HashAlgorithm, SignatureAlgorithm);
            var couple = Certificate.SignCertificate(StartDate, EndDate, CSRData,
                                                     signatureAlgorithm, _caX509Certificate,
                                                     _caKeyPair, _csr.GetPublicKey());

            Certificate.WriteCertificate(couple, SavePath);

            Settings.Default.Save();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindow.InstantiatedWindows.Remove(GetType());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
