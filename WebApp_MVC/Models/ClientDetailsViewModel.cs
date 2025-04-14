using Data.Entities;

namespace WebApp_MVC.Models;

public class ClientDetailsViewModel
{
    public int Id { get; set; }
    public string ClientName { get; set; } = null!;
    public List<ProjectEntity> Projects { get; set; } = [];

}
