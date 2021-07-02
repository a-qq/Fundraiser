using FundraiserManagement.Domain.MemberAggregate;
using FundraiserManagement.Domain.MemberAggregate.Cards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Infrastructure.Utils;

namespace FundraiserManagement.Infrastructure.Persistence.Configuration
{
    internal sealed class MemberConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> b)
        {
            b.ToTable("Members", SchemaNames.Fundraising);

            b.HasKey(p => p.Id);

            b.Property(p => p.SchoolId)
                .IsRequired();

            b.Property(p => p.GroupId)
                .IsRequired(false);

            b.Property(p => p.Role)
                .IsRequired();

            b.Property(p => p.Gender)
                .IsRequired();

            b.Property(p => p.IsFormTutor)
                .IsRequired();

            b.Property(p => p.IsTreasurer)
                .IsRequired();

            b.Property(p => p.IsArchived)
                .IsRequired();

            b.OwnsOne(p => p.Card, c =>
            {
                c.Property(x => x.Number)
                    .HasConversion(p => p.Value, p => Number.Create(p, nameof(Number)).Value)
                    .HasColumnName("Card_Number")
                    .IsRequired(false);

                c.Property(x => x.Cvc)
                    .HasConversion(p => p.Value, p => Cvc.Create(p, nameof(Cvc)).Value)
                    .HasColumnName("Card_CVC")
                    .IsRequired(false);

                c.Property(x => x.Month)
                    .HasConversion(p => p.Value, p => Month.Create(p.Value, nameof(Month)).Value)
                    .HasColumnName("Card_Month")
                    .IsRequired(false);

                c.Property(x => x.Year)
                    .HasConversion(p => p.Value, p => Year.Create(p.Value, nameof(Year)).Value)
                    .HasColumnName("Card_Year")
                    .IsRequired(false);
            });

            b.Ignore(p => p.DomainEvents);
        }
    }
}