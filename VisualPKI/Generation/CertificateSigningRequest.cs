using System;
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;

namespace VisualPKI.Generation
{
    class CertificateSigningRequest
    {
        public static Pkcs10CertificationRequest ReadFromFile(String path)
        {
            Pkcs10CertificationRequest result = null;
            try
            {
                using (
                    var textReader = new StreamReader(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    var pemReader = new PemReader(textReader);
                    result = (Pkcs10CertificationRequest)pemReader.ReadObject();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return result;
        }
    }
}
