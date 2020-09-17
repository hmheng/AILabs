using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormRecognizerApi.Services
{
    public interface ISignalRHubService
    {
        Task SendAsync(string method, string user, string message);
        Task SendAsync(string method, string message);
    }
}
