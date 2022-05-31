using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgentApi.Dtos;
using AgentApi.Model;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace AgentApi.Services
{
    public class SearchOfferService
    {
        private readonly IMongoClient _client;

        public SearchOfferService(IMongoClient client)
        {
            _client = client;
        }

        public JobOffer GetById(Guid id)
        {
            var companies = GetCompanies();
            
            var offers = new List<JobOffer>();
            
            companies
                .Where(x => x.JobOffers != null && x.JobOffers.Count != 0)
                .ToList()
                .ForEach(x => offers.AddRange(x.JobOffers));

            return offers.FirstOrDefault(x => x.Id == id);
        }
        
        public IEnumerable<JobOffer> FindJobOffers(SearchOfferDto criteria)
        {
            var companies = GetCompanies();

            if (!string.IsNullOrEmpty(criteria.Company))
            {
                companies = companies.Where(x => x.Name.Contains(criteria.Company)).ToList();
            }

            var offers = new List<JobOffer>();
            
            companies
                .Where(x => x.JobOffers != null && x.JobOffers.Count != 0)
                .ToList()
                .ForEach(x => offers.AddRange(x.JobOffers));

            if (!string.IsNullOrEmpty(criteria.Title))
            {
                offers = offers.Where(x => x.JobTitle.Contains(criteria.Title)).ToList();
            }

            if (!string.IsNullOrEmpty(criteria.Prerequisites))
            {
                offers = offers.Where(x => x.Prerequisites.Contains(criteria.Prerequisites)).ToList();
            }

            return offers;
        }

        private List<Company> GetCompanies()
        {
            var collection = _client.GetDatabase("AgentApi").GetCollection<User>("users");
            var filter = Builders<User>.Filter.Where(x => x.Company != null);

            return collection.Find(filter).Project(x => x.Company).ToList();
        }
    }
}