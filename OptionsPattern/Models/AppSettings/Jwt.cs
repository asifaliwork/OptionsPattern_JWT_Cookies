namespace OptionsPattern.Models.AppSettings
{
    public class Jwt
    {
        public string? ValidAudience { get; set; }

        public string? ValidIssuer { get; set; }

        public string? Key { get; set; }
    }
}
