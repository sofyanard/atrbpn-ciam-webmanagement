using System;

namespace Ciam.Models
{
    public class ClientModel
    {
        public string id { get; set; }
        public string clientId { get; set; }
        public string name { get; set; }
        public string rootUrl { get; set; }
        public string baseUrl { get; set; }
        public bool surrogateAuthRequired { get; set; }
        public bool enabled { get; set; }
        public string protocol { get; set; }
        public string clientAuthenticatorType { get; set; }
    }
}