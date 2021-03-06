﻿//using System;
//using System.Collections.Generic;
//using System.Collections.Immutable;
//using System.Linq;
//using MediatR;
//using SchoolManagement.Infrastructure.Authorization.Validators.Abstract;
//using SharedKernel.Infrastructure.Abstractions.Common;

//namespace SchoolManagement.Infrastructure.Authorization.Validators.Concrete
//{
//    internal sealed class ClaimPrincipalValidatorsFactory : IClaimPrincipalValidatorsFactory
//    {
//        private readonly IReadOnlyDictionary<string, IClaimsPrincipalValidator> _claimsPrinicpalValidators;

//        public ClaimPrincipalValidatorsFactory(ISender mediator, IAdministratorsProvider administratorProvider)
//        {
//            var claimsPrincipalValidatorType = typeof(IClaimsPrincipalValidator);
//            _claimsPrinicpalValidators = claimsPrincipalValidatorType.Assembly.DefinedTypes
//                .Where(x => claimsPrincipalValidatorType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
//                .Select(x =>
//                {
//                    var adminValidatorConstructor = x.GetConstructors().SingleOrDefault(c =>
//                        c.GetParameters().Select(p => p.ParameterType).Contains(typeof(IAdministratorsProvider)));

//                    return adminValidatorConstructor is null
//                        ? Activator.CreateInstance(x, mediator)
//                        : Activator.CreateInstance(x, administratorProvider);
//                })
//                .Cast<IClaimsPrincipalValidator>()
//                .ToImmutableDictionary(x => x.RoleRequirement, x => x);
//        }

//        public IClaimsPrincipalValidator GetValidatorByRequiredRole(string role)
//        {
//            var validator = _claimsPrinicpalValidators.GetValueOrDefault(role);
//            return validator ?? throw new NotImplementedException($"Validator for role is not implemented '{role}'!");
//        }
//    }
//}