using System.Runtime.Serialization;

namespace RandomNeko.Telegram.Model
{
    [DataContract]
    class PhotoSize
    {
        [DataMember(Name = "file_id")]
        public string FileId { get; set; }

        [DataMember(Name = "width")]
        public int Width { get; set; }

        [DataMember(Name = "height")]
        public int Height { get; set; }
    }
}
