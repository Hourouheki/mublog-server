﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mublog.Server.Domain.Common.Helpers;
using Mublog.Server.Domain.Data;
using Mublog.Server.Domain.Data.Entities;
using Mublog.Server.Domain.Data.Repositories;

namespace Mublog.Server.Infrastructure.Data.Repositories
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        private readonly AutoMapper.IMapper _mapper;
        private readonly ILogger<PostRepository> _logger;

        public PostRepository(AppDbContext context, AutoMapper.IMapper mapper, ILogger<PostRepository> logger) : base(context)
        {
            _mapper = mapper;
            _logger = logger;
        }

        public override PagedList<Post> GetPaged(QueryParameters queryParameters)
        {
            return PagedList<Post>.ToPagedList(Context.Posts.AsNoTracking().Include(p => p.Owner),
                queryParameters.Page, queryParameters.Size);
        }

        public PagedList<PostWithLike> GetPagedWithLikes(PostQueryParameters queryParameters, Profile user = null)
        {
            PagedList<Post> posts;

            IQueryable<Post> postSet;

            if (queryParameters.Profile != null)
            {
                postSet = Context.Posts.Where(p => p.OwnerId == queryParameters.Profile.Id)
                    .OrderBy(p => p.Id);
            }
            else
            {
                postSet = Context.Posts.OrderBy(p => p.Id);
            }

            if (user != null)
            {
                posts = PagedList<Post>.ToPagedList(
                    postSet.Include(p => p.Owner)
                        .Include(p => p.Likes),
                    queryParameters.Page, queryParameters.Size);
            }
            else
            {
                posts = PagedList<Post>.ToPagedList(
                    postSet.AsNoTracking().Include(p => p.Owner),
                    queryParameters.Page, queryParameters.Size);
            }


            var postWithLikes = _mapper.Map<List<PostWithLike>>(posts.ToList());

            if (user != null)
            {
                foreach (var post in postWithLikes)
                    post.Liked = post.Likes.Contains(user);
            }
            else
            {
                foreach (var post in postWithLikes)
                {
                    post.Liked = false;
                }
            }

            return PagedList<PostWithLike>.ToPagedList
                (postWithLikes, queryParameters.Page, queryParameters.Size);
        }

        public async Task<PostWithLike> GetByPublicId(int id, Profile user) =>
            await GetByPublicId(id, user?.Username);

        public async Task<PostWithLike> GetByPublicId(int id, string username = null)
        {
            var post = await Context.Posts
                .AsNoTracking()
                .Include(p => p.Owner).Include(p => p.Likes)
                .FirstOrDefaultAsync(p => p.PublicId == id);

            if (post == null) return null;

            var postWithLike = _mapper.Map<PostWithLike>(post);

            if (username == null)
            {
                postWithLike.Liked = false;
                return postWithLike;
            }

            postWithLike.Liked = postWithLike
                .Likes.Any(u => u.Username == username);

            return postWithLike;
        }

        public async Task<bool> AddLike(Post post, Profile user) =>
            await AddLike(post.PublicId, user);

        public async Task<bool> AddLike(int publicId, Profile user)
        {
            var post = await Context.Posts.Include(p => p.Likes)
                .FirstOrDefaultAsync(p => p.PublicId == publicId);

            if (post.Likes.Contains(user)) return true;

            post.Likes.Add(user);

            return await Update(post);
        }

        public override async Task<bool> Remove(Post entity)
        {
            try
            {
                await Context.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM public.posts WHERE \"Id\" = {entity.Id};");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return false;
            }
            return true;
        }

        public async Task<bool> RemoveLike(Post post, Profile user) =>
            await RemoveLike(post.PublicId, user);

        public async Task<bool> RemoveLike(int publicId, Profile user)
        {
            var post = await Context.Posts.Include(p => p.Likes)
                .FirstOrDefaultAsync(p => p.PublicId == publicId);

            if (!post.Likes.Contains(user)) return true;

            post.Likes.Remove(user);

            return await Update(post);
        }
    }
}