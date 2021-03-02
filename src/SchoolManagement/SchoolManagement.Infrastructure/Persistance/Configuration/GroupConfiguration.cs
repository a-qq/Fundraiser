using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SharedKernel.Infrastructure.Utils;

namespace SchoolManagement.Infrastructure.Persistance.Configuration
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> b)
        {
            b.ToTable("Groups", SchemaNames.Management).HasKey(p => p.Id);

            b.Property(p => p.Number).HasConversion(p => p.Value, p => Number.Create(p).Value).HasColumnName("Number")
                .IsRequired();
            b.Property(p => p.Sign).HasConversion(p => p.Value, p => Sign.Create(p).Value).HasColumnName("Sign")
                .HasMaxLength(4).IsRequired();
            b.Ignore(p => p.Code);
            b.HasIndex(p => new {p.Number, p.Sign}).HasName("Index_Code");
            b.HasOne(p => p.School).WithMany(p => p.Groups).IsRequired();
            b.Property(p => p.IsArchived);
            b.HasMany(p => p.Students).WithOne(p => p.Group).OnDelete(DeleteBehavior.ClientSetNull);
            b.HasOne(p => p.FormTutor).WithMany().HasForeignKey("FormTutorId").OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired(false);
            b.HasOne(p => p.Treasurer).WithOne().HasForeignKey<Group>("TreasurerId")
                .OnDelete(DeleteBehavior.ClientSetNull);

            b.HasQueryFilter(p => !p.IsArchived);
        }
    }
}