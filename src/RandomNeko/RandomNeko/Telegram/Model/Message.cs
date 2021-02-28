using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;

namespace RandomNeko.Telegram.Model
{
    [DataContract]
    class Message
    {
        [DataMember(Name = "message_id")]
        public ulong MessageId { get; set; }
        [DataMember(Name = "text")]
        public string Text { get; set; }
        [DataMember(Name = "chat")]
        public Chat Chat { get; set; }
        /// <summary>
        /// Message is a photo, available sizes of the photo
        /// </summary>
        [DataMember(Name = "photo", EmitDefaultValue = false)]
        public List<PhotoSize> Photo { get; set; }
        /// <summary>
        /// Caption for the animation, audio, document, photo, video or voice, 0-1024 characters
        /// </summary>
        [DataMember(Name = "caption", EmitDefaultValue = false)]
        public string Caption { get; set; }
    }
}
