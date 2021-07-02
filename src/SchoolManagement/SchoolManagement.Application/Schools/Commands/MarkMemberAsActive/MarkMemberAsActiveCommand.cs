using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using Dapper;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.Commands.MarkMemberAsActive
{
    internal sealed class MarkMemberAsActiveCommand : IInternalCommand
    {
        public string Subject { get; }

        public MarkMemberAsActiveCommand(string subject)
        {
            Subject = subject;
        }
    }

    internal sealed class MarkMemberAsActiveCommandHandler : IRequestHandler<MarkMemberAsActiveCommand, Result>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly ISchoolRepository _schoolRepository;

        public MarkMemberAsActiveCommandHandler(
            ISqlConnectionFactory sqlConnectionFactory,
            ISchoolRepository schoolRepository)
        {
            _sqlConnectionFactory = Guard.Against.Null(sqlConnectionFactory, nameof(sqlConnectionFactory));
            _schoolRepository = Guard.Against.Null(schoolRepository, nameof(schoolRepository));
        }

        public async Task<Result> Handle(MarkMemberAsActiveCommand request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.Subject, out Guid memberIdAsGuid))
                return Result.Failure($"Subject'{request.Subject}' not in Guid format!");

            using var connection = _sqlConnectionFactory.GetOpenConnection();


            const string sql = "SELECT [Member].[SchoolId] " +
                               "FROM [management].[Members] AS [Member] " +
                               "WHERE [Member].[Id] = @Id";

            var schoolIdOrNone = await connection.QuerySingleOrDefaultAsync<Guid?>(sql, new { Id = memberIdAsGuid });
            if (!schoolIdOrNone.HasValue)
                return Result.Failure($"SchoolId for member (Id: '{memberIdAsGuid}') not found!");

            var schoolId = new SchoolId(schoolIdOrNone.Value);
            var schoolOrNone = await _schoolRepository.GetByIdWithMembersAsync(schoolId, cancellationToken);

            if (schoolOrNone.HasNoValue)
                return Result.Failure($"School with id '{schoolId}' not found!");

            var memberId = new MemberId(memberIdAsGuid);

            if (schoolOrNone.Value.Members.All(m => m.Id != memberId))
                return Result.Failure($"Member with Id '{memberId}' not found!");

            var result = schoolOrNone.Value.MarkMemberAsActive(memberId);

            return result;
        }
    }
}