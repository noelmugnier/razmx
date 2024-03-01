using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<Contact> Contacts { get; set; }
}

public class Contact
{
    public int Id { get; set; }
    [Required] public string Name { get; set; }
    [Required] [EmailAddress] public string Email { get; set; }
    [Required] public string City { get; set; }
    [Required] public string Phone { get; set; }
}