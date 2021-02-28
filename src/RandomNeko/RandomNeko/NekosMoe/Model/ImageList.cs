using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RandomNeko.NekosMoe.Model
{
    [DataContract]
    class ImageList
    {
        [DataMember(Name = "images")]
        public List<Image> Images { get; set; }
    }
}
