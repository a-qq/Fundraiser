using Fundraiser.SharedKernel.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Core.SchoolAggregate.Schools;

namespace SchoolManagement.Data.Database
{
    internal sealed class SchoolConfiguration : IEntityTypeConfiguration<School>
    {
        public void Configure(EntityTypeBuilder<School> b)
        {
            b.ToTable("Schools", SchemaNames.Management).HasKey(p => p.Id);
            b.Property(p => p.Name).HasConversion(p => p.Value, p => Name.Create(p).Value).IsRequired().HasMaxLength(500);
            b.Property(p => p.Description).HasConversion(p => p.Value, p => Description.Create(p).Value).HasMaxLength(3000);
            b.Property(p => p.LogoId).HasMaxLength(36);
            b.HasMany(p => p.Members).WithOne(p => p.School).OnDelete(DeleteBehavior.Cascade);
            b.HasMany(p => p.Groups).WithOne(p => p.School).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
