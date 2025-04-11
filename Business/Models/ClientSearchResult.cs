

namespace Business.Models;

public class ClientSearchResult
{
    public int Id { get; set; }
    public string? ClientName { get; set; }
    public string DisplayProperty
    {
        get { return $"{ClientName}"; }
    }
    public string DetailsUrl
    {
        get { return $"/Clients/Details/{Id}"; }
    }
}
