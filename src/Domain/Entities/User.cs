using System.Collections.Generic;
using Mublog.Server.Domain.Enums;

namespace Mublog.Server.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public int ProfileImageId { get; set; }
        public ProfileImage ProfileImage { get; set; }
        public UserState UserState { get; set; }
        public ICollection<Post> Posts { get; set; }
        public ICollection<Post> LikedPosts { get; set; }
        public ICollection<PostImage> Mediae { get; set; }
    }
}