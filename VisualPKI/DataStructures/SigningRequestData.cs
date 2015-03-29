using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using Org.BouncyCastle.Asn1;
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
            var dict = new Dictionary<DerObjectIdentifier, String>(7);

            if (Country != null)
            {
                dict.Add(X509Name.C, Country);
            }
            if (State != null)
            {
                dict.Add(X509Name.ST, State);
            }
            if (City != null)
            {
                dict.Add(X509Name.L, City);
            }
            if (Organization != null)
            {
                dict.Add(X509Name.O, Organization);
            }
            if (OrganizationalUnit != null)
            {
                dict.Add(X509Name.OU, OrganizationalUnit);
            }
            if (DistinguishedName == null && MailAddress == null) return new X509Name(dict.Keys.ToList(), dict);

            if (DistinguishedName != null && MailAddress != null)
            {
                dict.Add(X509Name.CN, String.Format("{0}/emailAddress={1}", DistinguishedName, MailAddress));
            }
            else
            {
                dict.Add(X509Name.CN, DistinguishedName ?? MailAddress);
            }


            return new X509Name(dict.Keys.ToList(), dict);
        }
    }
}