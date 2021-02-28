using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RandomNeko.Telegram.Model
{
    [DataContract]
    class Payload<T>
    {
        [DataMember(Name = "ok")]
        public bool Ok { get; set; }
        [DataMember(Name = "result")]
        public T Result { get; set; }
        [DataMember(Name = "description")]
        public string Description { get; set; }
    }
}
