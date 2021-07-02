using Autofac;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Application.Common.Models;
using FundraiserManagement.Infrastructure.Services;
using System;

namespace FundraiserManagement.Infrastructure
{
    public sealed class PaymentsModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(new StripeOptions()
            {
                SecretKey = Environment.GetEnvironmentVariable("Stripe__Secret_Key"),
                PublicKey = Environment.GetEnvironmentVariable("Stripe__Public_Key"),
                WebhookSecret = Environment.GetEnvironmentVariable("Stripe__Webhook_Secret")
            });

            builder.RegisterType<PaymentGateway>()
                .As<IPaymentGateway>()
                .InstancePerLifetimeScope();
        }
    }
}