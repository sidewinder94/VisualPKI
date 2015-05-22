using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using VisualPKI.Annotations;
using VisualPKI.DataStructures;


namespace VisualPKI.Views
{
    /// <summary>
    /// Logique d'interaction pour DisplayCertificateInformations.xaml
    /// </summary>
    public partial class DisplayCertificateInformations : Window, INotifyPropertyChanged
    {
        private static SigningRequestData _csrData;

        public SigningRequestData CSRData
        {
            get { return _csrData; }
            set
            {
                _csrData = value;
                OnPropertyChanged();
            }
        }

        public DisplayCertificateInformations()
        {
            InitializeComponent();
            //TODO: Present Data more conveniently for the issuer
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            MainWindow.WindowClosed(GetType());
        }
    }
}
