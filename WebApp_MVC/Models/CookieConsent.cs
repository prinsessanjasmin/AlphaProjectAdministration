using System.Text.Json.Serialization;

namespace WebApp_MVC.Models;

public class CookieConsent
{
    [JsonPropertyName("essential")]
    public bool Essential { get; set; }

    [JsonPropertyName("functional")]
    public bool Functional { get; set; }

    [JsonPropertyName("analytical")]
    public bool Analytical { get; set; }

    [JsonPropertyName("marketing")]
    public bool Marketing { get; set; }

}
