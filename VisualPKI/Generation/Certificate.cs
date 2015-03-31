using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;
using Utils.Misc;
using VisualPKI.DataStructures;
using Utils.Text;
using VisualPKI.Properties;

namespace VisualPKI.Generation
{
    public class Certificate
    {
        public static readonly Dictionary<String, List<String>> SignatureAlgorithmsAndAssociatedHashes = new Dictionary<string, List<string>>()
        {
#region algorithms
            {
                "DSA", new List<String>()
                {
                    "SHA1"  
                }
            },
            {
                "ECDSA", new List<String>()
                {
                    "SHA1",
                    "SHA224",
                    "SHA256",
                    "SHA384",
                    "SHA512"
                }
            },
            {
                "GOST3410", new List<String>()
                {
                    "GOST3411"
                }
            },
            {
                "ECGOST3410", new List<String>()
                {
                    "GOST3411"
                }
            },
            {
                "RSA", new List<String>()
                {
                    "MD2",
                    "MD5",
                    "SHA1",
                    "SHA224",
                    "SHA256",
                    "SHA384",
                    "SHA512",
                    "RIPEMD128",
                    "RIPEMD160",
                    "RIPEMD256"
                }
            }
#endregion
        };

        public static List<String> SignatureAlgorithms
        {
            get { return SignatureAlgorithmsAndAssociatedHashes.Keys.ToList(); }
        }

        public Certificate()
        {

        }

        public static Tuple<AsymmetricCipherKeyPair, X509Certificate> CreateSelfSigned(DateTime startDate,
            DateTime endDate,
            SigningRequestData csrData,
            string privateKeyPath,
            Tuple<String, int> keyParams,
            String signatureAlgorithm)
        {
            AsymmetricCipherKeyPair keyPair = null;
            if (privateKeyPath.IsEmpty() || !File.Exists(privateKeyPath))
            {
                keyPair = PrivateKey.GenerateKeyPair(PrivateKey.SKeyAlgorithms[keyParams.Left()], keyParams.Right());
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
            certGen.SetSignatureAlgorithm(signatureAlgorithm);
            X509Certificate cert = certGen.Generate(keyPair.Private);
            return new Tuple<AsymmetricCipherKeyPair, X509Certificate>(keyPair, cert);
        }

        public static Tuple<AsymmetricCipherKeyPair, X509Certificate> CreateIntermediate(DateTime startDate,
                                                                                         DateTime endDate,
                                                                                         SigningRequestData csrData,
                                                                                         Tuple<String, int> keyParams,
                                                                                         String signatureAlgorithm,
                                                                                         X509Certificate caCertificate,
                                                                                         AsymmetricCipherKeyPair caKeyPair)
        {
            var generatedKeyPair = PrivateKey.GenerateKeyPair(PrivateKey.SKeyAlgorithms[keyParams.Left()],
                keyParams.Right());

            return new Tuple<AsymmetricCipherKeyPair, X509Certificate>(generatedKeyPair,
                SignCertificate(startDate, endDate, csrData, signatureAlgorithm,
                                caCertificate, caKeyPair, generatedKeyPair.Public));
        }

        public static X509Certificate SignCertificate(DateTime startDate, DateTime endDate, SigningRequestData csrData,
            string signatureAlgorithm, X509Certificate caCertificate, AsymmetricCipherKeyPair caKeyPair,
            AsymmetricKeyParameter clientKey)
        {
            if (clientKey.IsPrivate)
            {
                throw new ArgumentException("Should be a public key");
            }

            var certGen = new X509V3CertificateGenerator();
            Settings.Default.LastGeneratedSerial += 1;
            certGen.SetSerialNumber(new BigInteger(Settings.Default.LastGeneratedSerial.ToString()));
            certGen.SetIssuerDN(caCertificate.SubjectDN);
            certGen.SetNotBefore(startDate);
            certGen.SetNotAfter(endDate);
            certGen.SetSubjectDN(csrData.GetX509Name());
            certGen.SetPublicKey(clientKey);
            certGen.SetSignatureAlgorithm(signatureAlgorithm);
            certGen.AddExtension(X509Extensions.AuthorityKeyIdentifier, false,
                new AuthorityKeyIdentifierStructure(caCertificate));
            certGen.AddExtension(X509Extensions.SubjectKeyIdentifier, false,
                new SubjectKeyIdentifierStructure(clientKey));

            X509Certificate cert = certGen.Generate(caKeyPair.Private, new SecureRandom(new CryptoApiRandomGenerator()));

            return cert;
        }

        public static void WriteCertificate(X509Certificate cert, String path, String password = null)
        {
            //TODO: Export in PKCS formats
            using (var writer = new StreamWriter(File.Open(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None)))
            {
                var pemWriter = new PemWriter(writer);
                pemWriter.WriteObject(cert);
            }
        }


        public static X509Certificate ReadCertificate(String path)
        {
            X509Certificate result = null;
            try
            {
                using (
                    var textReader = new StreamReader(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    var pemReader = new PemReader(textReader);
                    result = (X509Certificate)pemReader.ReadObject();
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