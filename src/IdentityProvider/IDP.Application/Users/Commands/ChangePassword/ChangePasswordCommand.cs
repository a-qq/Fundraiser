using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using Destructurama.Attributed;
using IDP.Application.Common.Abstractions;
using IDP.Application.Common.Interfaces;
using IDP.Domain.UserAggregate.Entities;
using IDP.Domain.UserAggregate.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Domain.ValueObjects;
using System.Threading;
using System.Threading.Tasks;

namespace IDP.Application.Users.Commands.ChangePassword
{
    public sealed class ChangePasswordCommand : IUserCommand
    {
        public string Email { get; }
        [NotLogged]
        public string OldPassword { get; }
        [NotLogged]
        public string NewPassword { get; }

        public ChangePasswordCommand(
            string email, string oldPassword, string newPassword)
        {
            Email = email;
            OldPassword = oldPassword;
            NewPassword = newPassword;
        }
    }

    internal sealed class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, Result<Unit>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public ChangePasswordHandler(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher)
        {
            _userRepository = Guard.Against.Null(userRepository, nameof(userRepository));
            _passwordHasher = Guard.Against.Null(passwordHasher, nameof(passwordHasher));
        }
        public async Task<Result<Unit>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var email = Email.Create(request.Email).Value;
            var userOrNone = await _userRepository.GetUserByEmailAsync(email, cancellationToken);

            if (userOrNone.HasNoValue)
                return Result.Failure<Unit>($"User with email {email} not found! Please re-login and try again!");

            var hashedNewPassword = HashedPassword.Create(request.NewPassword, _passwordHasher).Value;

            var result = userOrNone.Value.ChangePassword(request.OldPassword, hashedNewPassword, _passwordHasher);

            if (result.IsFailure)
                return result.ConvertFailure<Unit>();

            return Unit.Value;
        }
    }
}