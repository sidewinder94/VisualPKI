using System;
using System.CodeDom;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Navigation;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using VisualPKI.Views;

namespace VisualPKI.Generation
{
    public static class PrivateKey
    {
        public static AsymmetricCipherKeyPair GenerateKeyPair()
        {
            var generator = new RsaKeyPairGenerator();
            generator.Init(new KeyGenerationParameters(new SecureRandom(new CryptoApiRandomGenerator()), 2048));
            return generator.GenerateKeyPair();
        }

        public static AsymmetricCipherKeyPair ReadFromFile(FileStream file)
        {
            AsymmetricCipherKeyPair result = null;
            IPasswordFinder finder = new PasswordFinder();
            try
            {
                while (result == null)
                {

                    try
                    {
                        var reader = new PemReader(new StreamReader(file), finder);
                        result = (AsymmetricCipherKeyPair)reader.ReadObject();
                    }
                    catch (PasswordException)
                    {
                        ((PasswordFinder)finder).ShowDialog();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                file.Close();
            }
            return result;
        }


        public static void WritePrivateKey(AsymmetricCipherKeyPair keyPair, String path, String password = null)
        {
            using (var writer = new StreamWriter(File.Open(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None)))
            {
                var pemWriter = new PemWriter(writer);
                if (password != null)
                {
                    pemWriter.WriteObject(keyPair, "DES-EDE3-CBC", password.ToCharArray(), new SecureRandom(new CryptoApiRandomGenerator()));

                }
                else
                {
                    pemWriter.WriteObject(keyPair);
                }
            }
        }

        public static void WritePublicKey(AsymmetricCipherKeyPair keyPair, String path)
        {
            //TODO : implement
            throw new NotImplementedException("Waiting for BouncyCastle to implement");
        }
    }
}