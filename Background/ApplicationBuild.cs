
using Abstractions;

namespace Background
{
    public class ApplicationBuild : BackgroundService
    {
    private readonly IUserService _userService;
        public ApplicationBuild(IUserService userService)
        {
            _userService = userService;
        }
        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _userService.CreateServiceAccount(cancellationToken);
        }
    }
}