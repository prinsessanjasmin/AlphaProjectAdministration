
namespace Data.Entities; 

//This entity is a suggestion from Claude AI for solving my problem of cookie consent and dark mode functionality remain with the current user 
public class UserPreferenceEntity
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string PreferenceKey { get; set; }
    public string PreferenceValue { get; set; }
}
