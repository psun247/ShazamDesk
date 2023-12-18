namespace WinUI3Shazam.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}
