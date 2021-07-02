using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Utils;

namespace SchoolManagement.Infrastructure.Persistence.Configuration
{
    internal sealed class SchoolConfiguration : IEntityTypeConfiguration<School>
    {
        public void Configure(EntityTypeBuilder<School> b)
        {
            b.ToTable("Schools", SchemaNames.Management).HasKey(p => p.Id);
            b.Property(p => p.Name).HasConversion(p => p.Value, p => Name.Create(p, nameof(Name)).Value).IsRequired()
                .HasMaxLength(500);
            b.Property(p => p.YearsOfEducation).HasConversion(p => p.Value, p => YearsOfEducation.Create(p, nameof(YearsOfEducation)).Value)
                .IsRequired();
            b.Property(p => p.Description).HasConversion(p => p.Value, p => Description.Create(p, nameof(Description)).Value)
                .HasMaxLength(3000);
            b.Property(p => p.GroupMembersLimit)
                .HasConversion(p => p.Value, p => GroupMembersLimit.Create(p.Value, nameof(GroupMembersLimit)).Value)
                .IsRequired(false);
            b.Property(p => p.LogoId).HasMaxLength(36);
            b.HasMany(p => p.Members).WithOne(p => p.School).OnDelete(DeleteBehavior.Cascade);
            b.HasMany(p => p.Groups).WithOne(p => p.School).OnDelete(DeleteBehavior.Cascade);
            b.Ignore(e => e.DomainEvents);
        }
    }
}