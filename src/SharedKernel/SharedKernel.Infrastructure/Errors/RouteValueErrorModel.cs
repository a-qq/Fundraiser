namespace SharedKernel.Infrastructure.Errors
{
    public sealed class RouteValueErrorModel
    {
        public string RouteValue { get; }
        public string Message { get; }

        public RouteValueErrorModel(string routeValue, string message)
        {
            RouteValue = routeValue;
            Message = message;
        }
    }
}
