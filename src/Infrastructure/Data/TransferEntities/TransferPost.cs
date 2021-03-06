using System;
using System.Collections.Generic;
using Mublog.Server.Domain.Data.Entities;
using NpgsqlTypes;

namespace Mublog.Server.Infrastructure.Data.TransferEntities
{
    public class TransferPost
    {
        public long Id { get; set; }
        public NpgsqlDateTime CreatedDate { get; set; }
        public NpgsqlDateTime UpdatedDate { get; set; }
        public int PublicId { get; set; }
        public string Content { get; set; }
        public int OwnerId { get; set; }
        public NpgsqlDateTime PostEditedDate { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public Guid ProfileImagePublicId { get; set; }
        public bool Liked { get; set; }
        public IEnumerable<long> Likes { get; set; }
        public long LikesCount { get; set; }
        public long CommentsCount { get; set; }

        public Post ToPost(Profile profile = null)
        {
            var profileImage = new Media
            {
                PublicId = ProfileImagePublicId
            };

            var owner = new Profile
            {
                Id = OwnerId,
                Username = Username,
                DisplayName = DisplayName,
                ProfileImage = profileImage
            };

            var post = new Post
            {
                Id = Id,
                CreatedDate = CreatedDate.ToDateTime(),
                UpdatedDate = UpdatedDate.ToDateTime(),
                PublicId = PublicId,
                Content = Content,
                OwnerId = OwnerId,
                Owner = owner,
                PostEditedDate = PostEditedDate.ToDateTime(),
                LikesCount = LikesCount,
                CommentsCount = CommentsCount
            };

            post.Likes = new List<Profile>();

            
            if (profile != null && post.Id != default && Liked)
            {
                post.Likes.Add(profile);
            }

            return post;
        }
    }
}