using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using Utils.Misc;
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

        public String SavePath { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

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
            MainWindow.InstantiatedWindows.Remove(GetType());
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
