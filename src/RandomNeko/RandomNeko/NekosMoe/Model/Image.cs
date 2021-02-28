using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RandomNeko.NekosMoe.Model
{
    [DataContract]
    public class Image
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "nsfw")]
        public bool Nsfw { get; set; }

        [DataMember(Name = "tags")]
        public List<string> Tags { get; set; }

        /// <summary>
        /// Concatenates tags with ", " and truncates them to <paramref name="length"/>, adding ... to the end.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public string TagsTruncated(int length = 800) {
            var tags = string.Join(", ", Tags);
            if (tags.Length > length) {
                tags = tags[..(length - 3)] + "...";
            }

            return tags;
        }
    }
}
