using FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Infrastructure.Utils;

namespace FundraiserManagement.Infrastructure.Persistence.Configuration
{
    internal sealed class FundraisingConfiguration : IEntityTypeConfiguration<Fundraiser>
    {
        public void Configure(EntityTypeBuilder<Fundraiser> b)
        {
            RelationalEntityTypeBuilderExtensions.ToTable((EntityTypeBuilder) b, "Fundraisers", SchemaNames.Fundraising);

            b.HasKey(p => p.Id);

            b.Property(p => p.Name)
                .HasConversion(p => p.Value, p => Name.Create(p, nameof(Name)).Value)
                .IsRequired()
                .HasMaxLength(500);

            b.Property(p => p.Description)
                .HasConversion(p => p.Value, p => Description.Create(p, nameof(Description)).Value)
                .HasMaxLength(3000);

            b.Property(p => p.SchoolId).IsRequired();

            b.Property(p => p.GroupId).IsRequired(false);

            b.Property(p => p.Range).IsRequired();

            b.Property(p => p.State).IsRequired();

            b.Property(p => p.Type).IsRequired();

            b.HasOne(p => p.Manager).WithMany()
                .HasForeignKey("ManagerId")
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired(false);

            b.HasMany(p => p.Participations)
                .WithOne(p => p.Fundraising)
                .OnDelete(DeleteBehavior.Cascade);

            b.OwnsOne(p => p.Goal, g =>
            {
                g.Property(x => x.Value).HasColumnName("Goal").IsRequired();
                g.Property(x => x.IsShared).HasColumnName("IsGoalShared").IsRequired();
            });

            b.Ignore(p=>p.DomainEvents);
        }
    }
}
