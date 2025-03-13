using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Contexts;

public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Initial Catalog=Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Projects\AlphaProjectAdministration\Data\Databases\ProjectDatabase.mdf; Integrated Security = True");
        
        return new DataContext(optionsBuilder.Options);
    }
}
