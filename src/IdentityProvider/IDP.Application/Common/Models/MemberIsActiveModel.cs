using System;

namespace IDP.Application.Common.Models
{
    public class MemberIsActiveModel
    {
        public string MemberId { get; }
        public bool IsActive { get; }

        public MemberIsActiveModel(string memberId, bool isActive)
        {
            MemberId = memberId;
            IsActive = isActive;
        }
    }
}