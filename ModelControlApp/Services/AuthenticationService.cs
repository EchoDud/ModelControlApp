using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public async Task<bool> LoginAsync(string username, string password)
        {
            
            return true; 
        }

        public async Task<bool> RegisterAsync(string username, string password)
        {
            return true; 
        }
    }
}
