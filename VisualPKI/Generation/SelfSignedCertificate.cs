using System;
using System.Data;
using System.IO;
using System.Windows.Controls;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.X509;
using VisualPKI.DataStructures;
using Utils.Text;
using VisualPKI.Properties;

namespace VisualPKI.Generation
{
    public static class SelfSignedCertificate
    {
        public static Tuple<AsymmetricCipherKeyPair, X509Certificate> Create(DateTime startDate,
            DateTime endDate,
            SigningRequestData csrData,
            string privateKeyPath)
        {
            AsymmetricCipherKeyPair keyPair = null;
            if (privateKeyPath.IsEmpty() || !File.Exists(privateKeyPath))
            {
                keyPair = PrivateKey.GenerateKeyPair();
            }
            else
            {
                keyPair = PrivateKey.ReadFromFile(File.Open(privateKeyPath, FileMode.Open, FileAccess.Read, FileShare.Read));
            }

            var certGen = new X509V1CertificateGenerator();
            Settings.Default.LastGeneratedSerial += 1;
            certGen.SetSerialNumber(new BigInteger(Settings.Default.LastGeneratedSerial.ToString()));
            certGen.SetIssuerDN(csrData.GetX509Name());
            certGen.SetSubjectDN(csrData.GetX509Name());
            certGen.SetNotBefore(startDate);
            certGen.SetNotAfter(endDate);
            certGen.SetPublicKey(keyPair.Public);
            certGen.SetSignatureAlgorithm("SHA1withRSA");
            X509Certificate cert = certGen.Generate(keyPair.Private);
            return new Tuple<AsymmetricCipherKeyPair, X509Certificate>(keyPair, cert);
        }

        public static void WriteCertificate(String path, String password = null)
        {
            //TODO: Serialize Certificate on Disk
        }
    }
}