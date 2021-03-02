using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Domain.Utils;

namespace SchoolManagement.Infrastructure.Database
{
    internal sealed class SchoolConfiguration : IEntityTypeConfiguration<School>
    {
        public void Configure(EntityTypeBuilder<School> b)
        {
            b.ToTable("Schools", SchemaNames.Management).HasKey(p => p.Id);
            b.Property(p => p.Name).HasConversion(p => p.Value, p => Name.Create(p).Value).IsRequired().HasMaxLength(500);
            b.Property(p => p.YearsOfEducation).HasConversion(p => p.Value, p => YearsOfEducation.Create(p).Value).IsRequired();
            b.Property(p => p.Description).HasConversion(p => p.Value, p => Description.Create(p).Value).HasMaxLength(3000);
            b.Property(p => p.GroupMembersLimit).HasConversion(p => p.Value, p => GroupMembersLimit.Create(p.Value).Value);
            b.Property(p => p.LogoId).HasMaxLength(36);
            b.HasMany(p => p.Members).WithOne(p => p.School).OnDelete(DeleteBehavior.Cascade);
            b.HasMany(p => p.Groups).WithOne(p => p.School).OnDelete(DeleteBehavior.Cascade);
            b.Ignore(e => e.DomainEvents);
        }
    }
}
