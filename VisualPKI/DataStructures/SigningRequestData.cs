using System;
using System.Security.RightsManagement;

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
    }
}