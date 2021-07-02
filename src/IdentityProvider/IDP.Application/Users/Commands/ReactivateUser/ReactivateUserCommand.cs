using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using IDP.Application.Common.Abstractions;
using IDP.Application.Common.Interfaces;
using IDP.Application.Common.Models;
using IDP.Domain.UserAggregate.ValueObjects;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace IDP.Application.Users.Commands.ReactivateUser
{
    internal sealed class ReactivateUserCommand : IInternalCommand
    {
        public string Subject { get; }

        public ReactivateUserCommand(string subject)
        {
            Subject = subject;
        }
    }

    internal sealed class ReactivateUserCommandHandler : IRequestHandler<ReactivateUserCommand, Result>
    {
        private readonly IUserRepository _userRepository;

        public ReactivateUserCommandHandler(
            IUserRepository userRepository)
        {
            _userRepository = Guard.Against.Null(userRepository, nameof(userRepository));
        }

        public async Task<Result> Handle(ReactivateUserCommand request, CancellationToken cancellationToken)
        {
            var subject = Subject.Create(request.Subject).Value;

            var userOrNone = await _userRepository.GetUserBySubjectAsync(subject, cancellationToken);

            if (userOrNone.HasNoValue)
                return IdentityProviderApplicationError.User.NotFound(subject);

            var result = userOrNone.Value.Reactivate();

            return result;
        }
    }
}