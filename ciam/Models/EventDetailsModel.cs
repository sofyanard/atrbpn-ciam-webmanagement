namespace Ciam.Models
{
    public class EventDetailsModel
    {
        public string token_id { get; set; }
        public string grant_type { get; set; }
        public string scope { get; set; }
        public string client_auth_method { get; set; }
        public string username { get; set; }
    }
}
