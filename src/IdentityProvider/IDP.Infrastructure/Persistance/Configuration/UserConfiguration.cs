﻿using System;
using IDP.Domain.UserAggregate.Entities;
using IDP.Domain.UserAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Domain.ValueObjects;
using SharedKernel.Infrastructure.Utils;

namespace IDP.Infrastructure.Persistance.Configuration
{
    internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> b)
        {
            b.ToTable("Users", SchemaNames.Authentiaction).HasKey(p => p.Id);

            b.Property(p => p.Subject).IsRequired();
            b.HasAlternateKey(p => p.Subject);
            b.Property(p =>
                    p.HashedPassword) //null value isn't passed to concerted, getting assigned straightforward to property (defualut ef.core 3 behaviour)
                .HasConversion(p => p.Value, p => HashedPassword.CheckHashAndConvert(p))
                .HasColumnName("HashedPassword");
            b.Property(p => p.IsActive);
            b.OwnsOne(p => p.SecurityCode, p =>
            {
                p.Property(pp => pp.Value).HasColumnName("SecurityCode");
                p.Property(pp => pp.IssuedAt).HasColumnName("SecurityCodeIssuedAt");
                p.Property<DateTime?>("_expirationDate").HasColumnName("SecurityCodeExpirationDate");
                p.Ignore(pp => pp.ExpirationDate);
            });
            b.Property(p => p.Email)
                .HasConversion(p => p.Value, p => Email.Create(p).Value)
                .HasColumnName("Email")
                .IsRequired();
            b.HasIndex(p => p.Email)
                .IsUnique();
            b.HasMany(p => p.Claims).WithOne()
                .HasPrincipalKey(p => p.Subject)
                .HasForeignKey("UserSubject")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.Ignore(p => p.DomainEvents);
        }
    }
}