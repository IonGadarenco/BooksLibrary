using Microsoft.Extensions.Options;

namespace BooksLibrary.API
{
    public class UserOptionsSetup : IConfigureOptions<UserOption>
    {
        private readonly IConfiguration _configuration;
        public UserOptionsSetup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(UserOption option)
        {
            _configuration.GetSection(nameof(UserOption)).Bind(option);
        }
    }
}
