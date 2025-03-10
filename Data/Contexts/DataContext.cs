using Microsoft.EntityFrameworkCore; 

namespace Data.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
}
