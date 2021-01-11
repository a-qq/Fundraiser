using Fundraiser.SharedKernel.Utils;
using IDP.Core.UserAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IDP.Infrastructure.Database
{
    internal sealed class ClaimConfiguration : IEntityTypeConfiguration<Claim>
    {
        public void Configure(EntityTypeBuilder<Claim> b)
        {
            b.ToTable("Claims", SchemaNames.Authentiaction).HasKey(p => p.Id);

            b.Property(p => p.Id).HasColumnName("ClaimId");
            b.Property(p => p.Type).IsRequired();
            b.Property(p => p.Value).IsRequired();
            b.Property<string>("UserSubject").IsRequired();
        }
    }
}
