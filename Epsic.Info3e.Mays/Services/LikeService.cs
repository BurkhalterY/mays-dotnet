﻿
using Epsic.Info3e.Mays.DbContext;
using Epsic.Info3e.Mays.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Epsic.Info3e.Mays.Services
{
    public class LikeService : ILikeService
    {
        private readonly MaysDbContext _context;

        public LikeService(MaysDbContext context)
        {
            _context = context;
        }

        public bool AddLike(string userId, string postId)
        {
            var like = new Like() { PostId = postId, UserId = userId };

            if (!LikeExists(like))
            {
                _context.Like.Add(like);
                _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public IEnumerable<Like> GetPostLikes(string postId)
        {
            return _context.Like.Where(l => l.PostId == postId).ToList();
        }

        public IEnumerable<Like> GetUserLikes(string userId)
        {
            return _context.Like.Where(l => l.UserId == userId).ToList();
        }

        public bool HasLiked(string userId, string postId)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveLike(string userId, string postId)
        {
            var like = _context.Like.FirstOrDefault(x => x.PostId == postId && x.UserId == userId);
            if (like == null)
            {
                return;
            }

            _context.Like.Remove(like);
            _context.SaveChanges();
        }

        private bool LikeExists(Like like)
        {
            return _context.Like.Any(e => e.PostId == like.PostId && e.UserId == like.UserId);
        }
    }
}
