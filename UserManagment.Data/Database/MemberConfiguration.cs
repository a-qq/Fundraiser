using Fundraiser.SharedKernel.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Core.SchoolAggregate.Members;

namespace SchoolManagement.Data.Database
{
    internal sealed class MemberConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> b)
        {
            b.ToTable("Members", SchemaNames.Management).HasKey(p => p.Id);

            b.Property(p => p.Id).ValueGeneratedNever();

            b.Property(p => p.FirstName)
                .HasConversion(p => p.Value, p => FirstName.Create(p).Value)
                .HasColumnName("FirstName")
                .HasMaxLength(200)
                .IsRequired();

            b.Property(p => p.LastName)
                .HasConversion(p => p.Value, p => LastName.Create(p).Value)
                .HasColumnName("LastName")
                .HasMaxLength(200)
                .IsRequired();

            b.Property(p => p.Email)
                .HasConversion(p => p.Value, p => Email.Create(p).Value)
                .HasColumnName("Email")
                .HasMaxLength(200)
                .IsRequired();

            b.HasIndex(p => p.Email)
                .IsUnique();

            b.Property(p => p.Gender)
                .HasConversion(p => p.Value, p => Gender.Create(p.ToString()).Value)
                .HasColumnName("Gender")
                .IsRequired();

            b.Property(p => p.Role)
                .HasConversion(p => p.Value, p => Role.Create(p.ToString()).Value)
                .HasColumnName("Role")
                .IsRequired();

            b.Property(p => p.IsActive);

            b.HasOne(p => p.School).WithMany(p => p.Members).IsRequired();

            b.HasOne(p => p.Group).WithMany(p => p.Members).IsRequired(false);
                
        }
    }
}
