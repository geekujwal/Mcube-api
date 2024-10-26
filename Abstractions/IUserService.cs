namespace Abstractions;

public interface IUserService
{
    Task CreateServiceAccount(CancellationToken cancellationToken);
}
