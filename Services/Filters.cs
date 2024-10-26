using MongoDB.Driver;
using Contracts;
using Documents;

namespace Services
{
    public static class Filters
    {
          public static class User
        {
            public static FilterDefinition<UserDocument> ById(string id)
            {
                return Builders<UserDocument>.Filter.Eq(doc => doc.Id, id);
            }

            public static FilterDefinition<UserDocument> ByAccountTypeIn(HashSet<AccountType> accountTypes)
            {
                return Builders<UserDocument>.Filter.In(doc => doc.AccountType, accountTypes);
            }
            public static FilterDefinition<UserDocument> ByEmail(string email)
            {
                return Builders<UserDocument>.Filter.Eq(doc => doc.Email, email);
            }
        }
    }
}