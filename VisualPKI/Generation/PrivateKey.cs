using System.Windows.Navigation;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;

namespace VisualPKI.Generation
{
    public class PrivateKey
    {
        public static AsymmetricCipherKeyPair GenerateKeyPair()
        {
            var generator = new ECKeyPairGenerator();
            return generator.GenerateKeyPair();
        }
    }
}