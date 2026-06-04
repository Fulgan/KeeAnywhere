using System.Runtime.Serialization;

namespace KeeAnywhere.Configuration
{
    [DataContract]
    public class AppCredential
    {
        [DataMember]
        public string ClientId { get; set; }

        [DataMember]
        public string ClientSecret { get; set; }
    }
}
