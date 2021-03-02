namespace Backend.API.Authorization.Validators.Absrtact
{
    internal interface IClaimPrincipalValidatorsFactory
    {
        IClaimsPrincipalValidator GetValidatorByRequiredRole(string role);
    }
}