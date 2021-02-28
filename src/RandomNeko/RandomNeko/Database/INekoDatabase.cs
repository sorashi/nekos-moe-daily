using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomNeko.Database
{
    interface INekoDatabase : IDisposable
    {
        public Task ConnectAsync();
        public Task<bool> HasBeenPostedAsync(string nekoId);
        public Task PostNekoAsync(string nekoId);

        public IAsyncEnumerable<string> GetTelegramClientIdsAsync();
        /// <returns>false if the client was already in the list, true otherwise</returns>
        public Task<bool> AddTelegramClientAsync(string clientId);
        /// <returns>whether clientId was found and removed</returns>
        public Task<bool> RemoveTelegramClientAsync(string clientId);
    }
}
