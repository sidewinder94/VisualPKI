using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Utils.Text;
using VisualPKI.Resources.Lang;
using VisualPKI.Views;

namespace VisualPKI.Generation
{
    public class PrivateKey
    {
        private readonly List<int> _keyStrenghts = new List<int>() { 128, 256, 512, 1024, 2048, 4096 };

        public static readonly Dictionary<String, IAsymmetricCipherKeyPairGenerator> SKeyAlgorithms = new Dictionary<String, IAsymmetricCipherKeyPairGenerator>()
        {
            {Algorithms.DHBasic,new DHBasicKeyPairGenerator()},
            {Algorithms.DH,new DHKeyPairGenerator()},
            {Algorithms.DSA,new DsaKeyPairGenerator()},
            {Algorithms.EC,new ECKeyPairGenerator()},
            {Algorithms.ELGammal,new ElGamalKeyPairGenerator()},
            {Algorithms.Gost3410,new Gost3410KeyPairGenerator()},
            {Algorithms.NaccacheStern,new NaccacheSternKeyPairGenerator()},
            {Algorithms.RSA,new RsaKeyPairGenerator()}
        };

        public List<int> KeyStrengths { get { return _keyStrenghts; } }
        public List<String> KeyAlgorithms
        {
            get { return SKeyAlgorithms.Keys.ToList(); }
        }

        public PrivateKey()
        {

        }

        public static AsymmetricCipherKeyPair GenerateKeyPair(IAsymmetricCipherKeyPairGenerator generator, int strength)
        {
            generator.Init(new KeyGenerationParameters(new SecureRandom(new CryptoApiRandomGenerator()), strength));
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
                    file.Position = 0;
                    try
                    {
                        var reader = new PemReader(new StreamReader(file), finder);
                        result = (AsymmetricCipherKeyPair)reader.ReadObject();
                    }
                    catch (PasswordException)
                    {
                        ((PasswordFinder)finder).ShowDialog();
                    }
                    catch (InvalidCipherTextException)
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


        public static void WritePrivateKey(AsymmetricCipherKeyPair keyPair, String path, char[] password = null)
        {
            using (var writer = new StreamWriter(File.Open(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None)))
            {
                var pemWriter = new PemWriter(writer);
                if (password != null && !new String(password).IsEmpty())
                {
                    pemWriter.WriteObject(keyPair.Private, "DES-EDE3-CBC", password, new SecureRandom(new CryptoApiRandomGenerator()));

                }
                else
                {
                    pemWriter.WriteObject(keyPair.Private);
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