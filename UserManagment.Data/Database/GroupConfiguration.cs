using Fundraiser.SharedKernel.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Core.SchoolAggregate.Groups;

namespace SchoolManagement.Data.Database
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> b)
        {
            b.ToTable("Groups", SchemaNames.Management).HasKey(p => p.Id);

            b.Property(p => p.Number).HasConversion(p => p.Value, p => Number.Create(p).Value).HasColumnName("Number").IsRequired();
            b.Property(p => p.Sign).HasConversion(p => p.Value, p => Sign.Create(p).Value).HasColumnName("Sign").HasMaxLength(4).IsRequired();
            b.Ignore(p => p.Code);
            b.HasIndex(p => new { p.Id, p.Number, p.Sign }).HasName("Index_Code").IsUnique(); 
            b.HasOne(p => p.School).WithMany(p => p.Groups).IsRequired();
            b.HasMany(p => p.Members).WithOne(p => p.Group).OnDelete(DeleteBehavior.ClientSetNull);
            b.HasOne(p => p.FormTutor).WithOne().HasForeignKey<Group>("FormTutorId").OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
