using Zad11.Models;
using Microsoft.EntityFrameworkCore;

namespace Zad11.Context;

public class ContextDB : DbContext
{
    protected ContextDB()
    {
        
    }

    public ContextDB(DbContextOptions<ContextDB> options)
        : base(options)
    {
        
    }
    
    public DbSet<Application> Users { get; set; }
}
