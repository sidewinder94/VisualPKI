using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VisualPKI.Views
{
    /// <summary>
    /// Logique d'interaction pour IntermediateCertificateWindow.xaml
    /// </summary>
    public partial class IntermediateCertificateWindow : Window
    {
        public IntermediateCertificateWindow()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindow.InstantiatedWindows.Remove(GetType());
        }
    }
}
