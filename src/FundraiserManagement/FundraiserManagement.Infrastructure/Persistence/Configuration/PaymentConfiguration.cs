using FundraiserManagement.Domain.FundraiserAggregate.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Infrastructure.Utils;

namespace FundraiserManagement.Infrastructure.Persistence.Configuration
{
    internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> b)
        {
            b.ToTable("Payments", SchemaNames.Fundraising);

            b.HasKey(p => p.Id);

            b.Property(p => p.Amount)
                .HasConversion(p => p.Value, p => Amount.Create(p, nameof(Amount)).Value)
                .IsRequired();

            b.Property(p => p.Status)
                .IsRequired();

            b.Property(p => p.AddedAt)
                .IsRequired();

            b.Property(p => p.InCash)
                .IsRequired();

            b.Property(p => p.ProcessedAt)
                .IsRequired(false);
        }
    }
}
