using SharedKernel.Infrastructure.Errors;

namespace FundraiserManagement.Application.Common.Models
{
    public sealed class FundraiserManagementRequestError : RequestError
    {
        private FundraiserManagementRequestError(string code, dynamic message)
        {
            Code = code;
            Message = message;
        }

        public static class Payments
        {
            public static RequestError PaymentGatewayError(string message)
            {
                return new FundraiserManagementRequestError("payment.gateway.error", message);
            }
        }
    }
}
