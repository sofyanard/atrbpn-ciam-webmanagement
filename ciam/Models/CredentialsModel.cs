using System;

namespace Ciam.Models
{
    public class CredentialsModel
    {
        public string type { get; set; }
        public string value { get; set; }
        public bool temporary { get; set; }
    }
}