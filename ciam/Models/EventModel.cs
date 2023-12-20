namespace Ciam.Models
{
    public class EventModel
    {
        public long time { get; set; }
        public string type { get; set; }
        public string realmId { get; set; }
        public string clientId { get; set; }
        public string userId { get; set; }
        public string sessionId { get; set; }
        public string ipAddress { get; set; }
        public EventDetailsModel details { get; set; }
    }
}
