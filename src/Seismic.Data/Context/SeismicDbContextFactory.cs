using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Seismic.Data.Context;

public sealed class SeismicDbContextFactory : IDesignTimeDbContextFactory<SeismicDbContext>
{
    public SeismicDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SeismicDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=SeismicAnalytics;Trusted_Connection=True;TrustServerCertificate=True;");

        return new SeismicDbContext(optionsBuilder.Options);
    }
}
