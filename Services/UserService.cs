using System.Security.Cryptography;
using Abstractions;
using Contracts;
using Documents;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Services;
public class UserService : IUserService
{
    private readonly IMongoCollection<UserDocument> _userDocument;

    private readonly IOptionsMonitor<ServiceAccountSecret> _serviceAccountConfiguration;

    private readonly IConfiguration _configuration;
    public UserService(
        MongoDbContext context,
        IOptionsMonitor<ServiceAccountSecret> serviceAccountConfiguration,
        IConfiguration configuration
        )
    {
        _userDocument = context.GetCollection<UserDocument>(nameof(UserDocument));
        _serviceAccountConfiguration = serviceAccountConfiguration;
        _configuration = configuration;
    }

    public async Task CreateServiceAccount(CancellationToken cancellationToken)
    {
        string email = _serviceAccountConfiguration.CurrentValue.Email;
        string password = _serviceAccountConfiguration.CurrentValue.Password;
        FilterDefinition<UserDocument> filter = Filters.User.ByEmail(email);
        var user = (await _userDocument.FindAsync(filter, cancellationToken: cancellationToken))?.ToList(cancellationToken: cancellationToken)?.FirstOrDefault();
        if (user is null)
        {
            await CreateAccountAsync(new() { AccountType = AccountType.Admin, Email = email, Password = password, FullName = "Admin" }, cancellationToken);
        }
    }
    
    private async Task<CreateModeratorUserResponse> CreateAccountAsync(CreateModeratorUserRequest request, CancellationToken cancellationToken)
    {
        UserDocument userDocument = new()
        {
            Id = Guid.NewGuid().ToString(),
            Email = request.Email,
            AccountType = request.AccountType,
            Hash = HashPassword(request.Password),
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber
        };
        var insertOptions = new InsertOneOptions();

        await _userDocument.InsertOneAsync(userDocument, insertOptions, cancellationToken);
        return new()
        {
            Id = userDocument.Id
        };
    }

    private static string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);

        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password!,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));
        return $"{Convert.ToBase64String(salt)}:{hashed}";
    }
}
