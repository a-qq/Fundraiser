using CSharpFunctionalExtensions;
using FluentValidation.Validators;
using Fundraiser.SharedKernel.Utils;

namespace Fundraiser.SharedKernel.PropertyValidators
{
    public sealed class EmailValidator<T> : PropertyValidator
    {

        public EmailValidator()
            : base("{ErrorMessage}") { }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            string email = context.PropertyValue as string;
            Result<Email> emailResult = Email.Create(email);
            if (emailResult.IsFailure)
            {
                context.MessageFormatter.AppendArgument("ErrorMessage", emailResult.Error);
                return false;
            }

            return true;
        }
    }
}
