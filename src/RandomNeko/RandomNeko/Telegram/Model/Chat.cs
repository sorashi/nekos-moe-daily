using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RandomNeko.Telegram.Model
{
    [DataContract]
    class Chat
    {
        [DataMember(Name = "id")]
        public ulong Id { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "username")]
        public string Username { get; set; }
    }
}
