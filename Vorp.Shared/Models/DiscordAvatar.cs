using System.Runtime.Serialization;

namespace Vorp.Shared.Models
{
    [DataContract]
    public class DiscordAvatar
    {
        [DataMember(Name = "id")]
        public string Id;

        [DataMember(Name = "username")]
        public string Username;

        [DataMember(Name = "avatar")]
        public string Avatar;

        [DataMember(Name = "discriminator")]
        public string Discriminator;

        [DataMember(Name = "public_flags")]
        public int PublicFlags;

        [DataMember(Name = "banner")]
        public object Banner;

        [DataMember(Name = "banner_color")]
        public object BannerColor;

        [DataMember(Name = "accent_color")]
        public object AccentColor;

        [DataMember(Name = "avatarurl")]
        public string Avatarurl;

    }
}
