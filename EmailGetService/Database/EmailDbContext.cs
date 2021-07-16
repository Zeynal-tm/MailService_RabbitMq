using EmailGetService.Models;
using Microsoft.EntityFrameworkCore;

namespace EmailGetService.Database
{
    public class EmailDbContext : DbContext
    {
        public DbSet<MailBox> MailBoxes { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<Attachment> Attachments { get; set; }

        public EmailDbContext(DbContextOptions<EmailDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Email>()
                .HasIndex(i => new { i.MailBoxId, i.MessageId })
                .IsUnique();

            modelBuilder.Entity<MailBox>()
                .HasIndex(i => i.UserName)
                .IsUnique();

            modelBuilder.Entity<MailBox>()
                .HasMany(p => p.Emails)
                .WithOne(p => p.MailBox)
                .HasForeignKey(p => p.MailBoxId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Email>()
                .HasMany(p => p.Attachments)
                .WithOne(p => p.Email)
                .HasForeignKey(p => p.EmailId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
