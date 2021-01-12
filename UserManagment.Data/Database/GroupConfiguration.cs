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

            b.Property(p => p.CodeLetter);
            b.Property(p => p.CodeNumber);
            b.HasOne(p => p.School).WithMany(p => p.Groups).IsRequired();
            b.HasMany(p => p.Members).WithOne(p => p.Group).OnDelete(DeleteBehavior.ClientSetNull);
            b.HasOne(p => p.FormTutor).WithOne().HasForeignKey<Group>("FormTutorId").OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
