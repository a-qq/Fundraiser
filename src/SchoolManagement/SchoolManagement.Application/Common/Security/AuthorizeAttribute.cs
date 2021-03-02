using System;

namespace SchoolManagement.Application.Common.Security
{
    /// <summary>
    ///     Specifies the class this attribute is applied to requires authorization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class AuthorizeAttribute : Attribute
    {
        /// <summary>
        ///     Gets or sets the policy name that determines access to the resource.
        /// </summary>
        public string Policy { get; set; }
    }
}