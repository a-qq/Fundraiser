using FundraiserManagement.Domain.FundraiserAggregate.Participations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Infrastructure.Utils;

namespace FundraiserManagement.Infrastructure.Persistence.Configuration
{
    internal sealed class ParticipationConfiguration : IEntityTypeConfiguration<Participation>
    {
        public void Configure(EntityTypeBuilder<Participation> b)
        {
            b.ToTable("Participations", SchemaNames.Fundraising);

            b.HasKey(p => p.Id);

            b.HasOne(p => p.Fundraising)
                .WithMany(p => p.Participations)
                .HasForeignKey("FundraisingId")
                .IsRequired();

            b.HasOne(p => p.Participant)
                .WithMany()
                .HasForeignKey("ParticipantId")
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired(false);

            b.HasMany(p => p.Payments)
                .WithOne()
                .HasForeignKey("ParticipationId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}
