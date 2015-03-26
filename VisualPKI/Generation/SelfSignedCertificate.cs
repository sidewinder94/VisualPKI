using System;
using System.IO;
using Org.BouncyCastle.X509;
using VisualPKI.DataStructures;
using Utils.Text;

namespace VisualPKI.Generation
{
    public static class SelfSignedCertificate
    {
        public static void Create(DateTime startDate, DateTime endDate, SigningRequestData csrData, string privateKeyPath)
        {
            if (privateKeyPath.IsEmpty() || !File.Exists(privateKeyPath))
            {
                PrivateKey.Generate();
            }

            var certGen = new X509V1CertificateGenerator();
        }
    }
}