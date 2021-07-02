using System;
using CSharpFunctionalExtensions;
using SchoolManagement.Application.Common.Interfaces;
using SK = SharedKernel.Infrastructure.Abstractions.Requests;

namespace SchoolManagement.Application.Common.Models
{
    public class IdentifiedCommand<T> : SK.IdentifiedCommand<T>, ICommand<Result>
        where T : IInternalCommand
    {
        public IdentifiedCommand(T command, Guid id) 
            : base(command, id) { }
    }
}