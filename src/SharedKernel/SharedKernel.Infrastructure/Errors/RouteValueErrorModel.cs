namespace SharedKernel.Infrastructure.Errors
{
    public sealed class RouteValueErrorModel
    {
        public RouteValueErrorModel(string routeValue, string message)
        {
            RouteValue = routeValue;
            Message = message;
        }

        public string RouteValue { get; }
        public string Message { get; }
    }
}