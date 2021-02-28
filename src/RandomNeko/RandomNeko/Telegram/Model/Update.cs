using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RandomNeko.Telegram.Model
{
    [DataContract]
    class Update
    {
        [DataMember(Name = "update_id")]
        public ulong UpdateId { get; set; }
        [DataMember(Name = "message")]
        public Message Message { get; set; }
    }
}
