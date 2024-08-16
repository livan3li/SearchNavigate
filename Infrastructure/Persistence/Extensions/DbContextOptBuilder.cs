using Microsoft.EntityFrameworkCore;

namespace SearchNavigate.Infrastructure.Persistence.Extensions;
/// <summary>
/// 
/// </summary>
public class DbContextOptBuilder : DbContextOptionsBuilder
{
    public DbContextOptBuilder(string connString)
    {
        this.UseNpgsql(connString);
    }
}