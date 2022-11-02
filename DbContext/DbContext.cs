namespace DbContext
{
    public class DbContext:System.Data.Entity.DbContext
    {
        public DbContext(DbContextOptions<DbContext> options) : base(options)
        {

        }
    }
}