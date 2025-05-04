using Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore; 

namespace Data.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<ClientEntity> Clients { get; set; }
    public DbSet<ProjectEmployeeEntity> ProjectEmployees { get; set; }
    public DbSet<ProjectEntity> Projects { get; set; }
    public DbSet<StatusEntity> Statuses { get; set; }
    public DbSet<AddressEntity> Addresses { get; set; }
    public DbSet<NotificationEntity> Notifications { get; set; }
    public DbSet<NotificationTypeEntity> NotificationTypes { get; set; }
    public DbSet<NotificationDismissedEntity> DismissedNotifications { get; set; }
    public DbSet<TargetGroupEntity> TargetGroups { get; set; }
    public DbSet<UserPreferenceEntity> UserPreferences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProjectEmployeeEntity>()
        .HasKey(x => new { x.ProjectId, x.EmployeeId });

        //I had som guidance from ChatGPT 4o on configuring the one-to-many relationships. 
       
        modelBuilder.Entity<ProjectEmployeeEntity>()
            .HasOne(pe => pe.Project)
            .WithMany(p => p.TeamMembers)
            .HasForeignKey(pe => pe.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProjectEmployeeEntity>()
            .HasOne(pe => pe.Employee)
            .WithMany(p => p.EmployeeProjects)
            .HasForeignKey(pe => pe.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProjectEntity>()
            .HasOne(p => p.Status)
            .WithMany(s => s.Projects)
            .HasForeignKey(p => p.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProjectEntity>()
            .HasOne(p => p.Client)
            .WithMany(c => c.Projects)
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProjectEntity>()
            .Property(p => p.Budget)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ApplicationUser>()
            .HasOne(e => e.Address)
            .WithMany(ad => ad.Employees)
            .HasForeignKey(e => e.AddressId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
