namespace Ciam.Models
{
    public class ResetPasswordModel
    {
        public string type { get; set; }
        public string value { get; set; }
        public bool temporary { get; set; }
    }
}
