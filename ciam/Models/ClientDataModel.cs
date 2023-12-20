namespace Ciam.Models
{
    public class ClientDataModel
    {
        public string id { get; set; }
        public string clientId { get; set; }
        public bool surrogateAuthRequired { get; set; }
        public bool alwaysDisplayInConsole { get; set; }
        public bool enabled { get; set; }
        public string clientAuthenticatorType { get; set; }
        public string[] redirectUris { get; set; }
        public int notBefore { get; set; }
        public bool bearerOnly { get; set; }
        public bool consentRequired { get; set; }
        public bool standardFlowEnabled { get; set; }
        public bool implicitFlowEnabled { get; set; }
        public bool directAccessGrantsEnabled { get; set; }
        public bool serviceAccountsEnabled { get; set; }
        public bool publicClient { get; set; }
        public bool frontchannelLogout { get; set; }
        public string protocol { get; set; }
        public bool fullScopeAllowed { get; set; }
        public int nodeReRegistrationTimeout { get; set; }
        public string[] defaultClientScopes { get; set; }
        public string[] optionalClientScopes { get; set; }
        public ClientAccessModel access { get; set; }
    }
}
