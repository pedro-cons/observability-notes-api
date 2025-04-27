using Microsoft.EntityFrameworkCore;
using Notes.API.Entities;

namespace Notes.API.Database;

//dotnet ef migrations add AddDatabaseAndNotesTableSchema --project Notes.API --startup-project Notes.API
//dotnet ef database update --project Notes.API --startup-project Notes.API

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Note> Notes => Set<Note>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Description).IsRequired().HasMaxLength(255);
        });
    }
}
