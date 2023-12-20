namespace Ciam.Models
{
    public class UserSessionClientModel
    {
        public string id { get; set; }
        public string username { get; set; }
        public string userId { get; set; }
        public string ipAddress { get; set; }
        public long start { get; set; }
        public long lastAccess { get; set; }
    }
}
