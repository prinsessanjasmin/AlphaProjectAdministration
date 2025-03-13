using Data.Entities;
using Microsoft.EntityFrameworkCore; 

namespace Data.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<ClientEntity> Clients { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<EmployeeEntity> Employees { get; set; }
    public DbSet<ProjectEmployeeEntity> ProjectEmployees { get; set; }
    public DbSet<ProjectEntity> Projects { get; set; }
    public DbSet<StatusEntity> Statuses { get; set; }
}
