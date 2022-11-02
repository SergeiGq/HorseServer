using ClassLibrary.Model;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary;

public class DbHorse : DbContext
{
    private bool _initialized;

    public DbHorse()
    {
        
    }

    public DbHorse(DbContextOptions options) : base(options)
    {
        _initialized = true;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!_initialized)
        {
            optionsBuilder.UseNpgsql();
        }
    }


    public DbSet<Horse> Horses { get; set; }
    public DbSet<Auth> Auths { get; set; }
}