using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RandomNeko.NekosMoe.Model;

namespace RandomNeko
{
    interface IPosterService
    {
        public Task PostAsync(Image neko);
    }
}
