using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.ApiClients
{
    public interface IBaseApiClient
    {
        void SetToken(string token);
    }
}
