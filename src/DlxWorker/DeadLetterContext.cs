using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Infrastructure.Utils;

namespace DlxWorker
{
    public class DeadLetterContext : DbContext
    {
        public DeadLetterContext(DbContextOptions<DeadLetterContext> options) : base(options)
        {
        }

        public DbSet<DeadLetterEntry> DeadLetterEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<DeadLetterEntry>(ConfigureDeadLetterEntryEntry);
        }

        void ConfigureDeadLetterEntryEntry(EntityTypeBuilder<DeadLetterEntry> builder)
        {
            builder.ToTable("DeadLetterEvents", SchemaNames.DeadLetterExchange);

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .IsRequired();

            builder.Property(e => e.Content)
                .IsRequired();

            builder.Property(e => e.CreationTime)
                .IsRequired();

            builder.Property(e => e.HandledAt)
                .IsRequired();

            builder.Property(e => e.Type)
                .IsRequired();

            builder.Property(e => e.Reason)
                .IsRequired();
        }
    }
}