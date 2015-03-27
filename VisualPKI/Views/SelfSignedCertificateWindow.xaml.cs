using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Win32;
using Utils.Misc;
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
        private DateTime _endDate;
        private DateTime _startDate;
        private String _privateKeyPath;

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

        public SelfSignedCertificateWindow()
        {
            _startDate = DateTime.Now;
            _endDate = DateTime.Now.AddDays(1);
            CSRData = new SigningRequestData();
            PrivateKeyPath = "ttttttt";
            SerialNumber = Settings.Default.LastGeneratedSerial;

            InitializeComponent();
            DataContext = this;
        }


        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var couple = SelfSignedCertificate.Create(StartDate, EndDate, CSRData, _privateKeyPath);
            //TODO : Ajouter des listes déroulantes pour proposer les différents algorithmes de clés/signatures/etc...
        }




        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
