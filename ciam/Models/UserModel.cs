using System;

namespace Ciam.Models
{
    public class UserModel
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string enabled { get; set; }
        public CredentialsModel[] credentials { get; set; }
    }
}