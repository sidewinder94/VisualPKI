using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using Org.BouncyCastle.Asn1.X509;

namespace VisualPKI.DataStructures
{
    public class SigningRequestData
    {
        public String DistinguishedName { get; set; }
        public String Organization { get; set; }
        public String OrganizationalUnit { get; set; }
        public String City { get; set; }
        public String State { get; set; }
        public String Country { get; set; }
        public String MailAddress { get; set; }

        public X509Name GetX509Name()
        {
            var dict = new Dictionary<String, String>(7)
            {
                {"C", Country},
                {"ST", State},
                {"L", City},
                {"O", Organization},
                {"OU", OrganizationalUnit},
                {"CN", String.Format("{0}/emailAddress={1}", DistinguishedName, MailAddress)}
            };

            return new X509Name(dict.Keys.ToList(), dict);
        }
    }
}