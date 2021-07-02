using IDP.Domain.UserAggregate.Entities;
using IDP.Domain.UserAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Infrastructure.Utils;

namespace IDP.Infrastructure.Persistence.Configuration
{
    internal sealed class ClaimConfiguration : IEntityTypeConfiguration<Claim>
    {
        public void Configure(EntityTypeBuilder<Claim> b)
        {
            b.ToTable("Claims", SchemaNames.Authentication).HasKey(p => p.Id);

            b.Property(p => p.Id).HasColumnName("ClaimId");
            b.Property(p => p.Type).IsRequired();
            b.Property(p => p.Value).IsRequired();
            b.Property<Subject>("UserSubject").HasConversion<string>().IsRequired();
        }
    }
}