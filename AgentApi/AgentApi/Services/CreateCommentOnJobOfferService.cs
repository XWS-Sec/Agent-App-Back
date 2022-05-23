using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgentApi.Dtos;
using AgentApi.Model;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace AgentApi.Services
{
    public class CreateCommentOnJobOfferService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMongoCollection<User> _userCollection;

        public CreateCommentOnJobOfferService(UserManager<User> userManager, IMongoClient client)
        {
            _userManager = userManager;
            _userCollection = client.GetDatabase("AgentApi").GetCollection<User>("users");
        }

        public async Task<Comment> Create(string userId, CommentDto newComment)
        {
            var user = await FindUserByJobOffer(newComment.JobOfferId);

            if (user == null)
                throw new Exception("That job offer does not exist");

            var comment = new Comment()
            {
                Id = Guid.NewGuid(),
                Text = newComment.Text,
                DateCreated = DateTime.Now,
                UserId = Guid.Parse(userId)
            };

            var jobOffer = user.Company.JobOffers.First(x => x.Id == newComment.JobOfferId);
            if (jobOffer.Comments == null)
            {
                jobOffer.Comments = new List<Comment>();
            }
            
            jobOffer.Comments.Add(comment);

            await _userManager.UpdateAsync(user);
            return comment;
        }

        private async Task<User> FindUserByJobOffer(Guid jobOffer)
        {
            User foundUser = null;

            var filter = Builders<User>.Filter.Where(x => x.Company != null &&
                                                          x.Company.IsVerified);

            var users = await _userCollection.FindAsync(filter);

            foreach (var user in users.ToEnumerable())
            {
                if (user.Company.JobOffers != null && user.Company.JobOffers.FirstOrDefault(x => x.Id == jobOffer) != null)
                {
                    foundUser = user;
                    break;
                }
            }

            return foundUser;
        }
    }
}