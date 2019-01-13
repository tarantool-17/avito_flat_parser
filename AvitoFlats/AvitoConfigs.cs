using System.Collections.Generic;

namespace AvitoFlats
{
    public class AvitoConfigs
    {
        public string BaseUrl { get; set; }
        public int NotificationPeriod { get; set; }
        public Dictionary<string, string> QueryParams { get; set; }
        public EmailConfigs EmailConfig { get; set; }
        public string LogPath { get; set; }
    }

    public class EmailConfigs
    {
        public List<string> ToEmails { get; set; }
        public string FromEmail { get; set; }
        public string SendGridKey { get; set; }
    }
}