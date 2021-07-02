using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using IDP.Application.Common.Abstractions;
using IDP.Application.Common.Interfaces;
using IDP.Application.Common.Models;
using IDP.Domain.UserAggregate.ValueObjects;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace IDP.Application.Users.Commands.RemoveUser
{
    internal sealed class RemoveUserCommand : IInternalCommand
    {
        public string Subject { get; }

        public RemoveUserCommand(string subject)
        {
            Subject = subject;
        }
    }

    internal sealed class RemoveUserCommandHandler : IRequestHandler<RemoveUserCommand, Result>
    {
        private readonly IUserRepository _userRepository;

        public RemoveUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = Guard.Against.Null(userRepository, nameof(userRepository));
        }


        public async Task<Result> Handle(RemoveUserCommand request, CancellationToken cancellationToken)
        {
            var subject = Subject.Create(request.Subject).Value;

            var userOrNone = await _userRepository.GetUserBySubjectAsync(subject, cancellationToken);

            if (userOrNone.HasValue)
                _userRepository.Remove(userOrNone.Value);

            return Result.Success();
        }
    }
}