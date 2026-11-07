using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SecureLab.Api.Data;

public sealed class SecureLabDesignTimeDbContextFactory
    : IDesignTimeDbContextFactory<SecureLabDbContext>
{
    private const string LocalStudyConnectionString =
        "Host=127.0.0.1;Port=54329;Database=securelab;Username=securelab;Password=local-study-password;Include Error Detail=false";

    public SecureLabDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__SecureLab")
            ?? LocalStudyConnectionString;
        var options = new DbContextOptionsBuilder<SecureLabDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new SecureLabDbContext(options);
    }
}
