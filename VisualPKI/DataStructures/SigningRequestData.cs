using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using Utils.Text;

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
        public SigningRequestData Issuer { get; private set; }

        public static SigningRequestData FromX509Certificate(X509Certificate cert)
        {
            var issuer = cert.IssuerDN;
            var subject = cert.SubjectDN;


            var result = GetSubjectData(subject);
            result.Issuer = GetSubjectData(issuer);

            return result;
        }

        public static SigningRequestData FromCSR(Pkcs10CertificationRequest pkcs10CertificationRequest)
        {

            var subject = pkcs10CertificationRequest.GetCertificationRequestInfo().Subject;
            var result = GetSubjectData(subject);

            return result;
        }

        private static SigningRequestData GetSubjectData(X509Name subject)
        {
            var result = new SigningRequestData()
            {
                DistinguishedName = subject.GetValueList(X509Name.CN)[0].ToString().RegExpReplace(@"/emailAddress=.*$", "").ReEncodeString("iso-8859-1", "utf-8"),
                Organization = subject.GetValueList(X509Name.O)[0].ToString().ReEncodeString("iso-8859-1", "utf-8"),
                OrganizationalUnit = subject.GetValueList(X509Name.OU)[0].ToString().ReEncodeString("iso-8859-1", "utf-8"),
                City = subject.GetValueList(X509Name.L)[0].ToString().ReEncodeString("iso-8859-1", "utf-8"),
                State = subject.GetValueList(X509Name.ST)[0].ToString().ReEncodeString("iso-8859-1", "utf-8"),
                Country = subject.GetValueList(X509Name.C)[0].ToString().ReEncodeString("iso-8859-1", "utf-8")
            };

            result.MailAddress =
                Regex.Match(subject.GetValueList(X509Name.CN)[0].ToString().ReEncodeString("iso-8859-1", "utf-8"),
                    @"^.*/emailAddress=(.*)").Groups[1].Value;
            return result;
        }

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

            if (DistinguishedName != null && !MailAddress.IsEmpty())
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