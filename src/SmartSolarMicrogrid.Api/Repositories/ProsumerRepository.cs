/*
 * Purpose: Prosumer repository implementation.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SmartSolarMicrogrid.Api.Configuration;
using SmartSolarMicrogrid.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSolarMicrogrid.Api.Repositories
{
    public class ProsumerRepository : IProsumerRepository
    {
        private readonly IMongoCollection<Prosumer> _prosumersCollection;

        public ProsumerRepository(IOptions<MongoDbSettings> mongoDbSettings)
        {
            // Initializes the MongoDB connection and gets the collection
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _prosumersCollection = mongoDatabase.GetCollection<Prosumer>("Prosumers");
        }

        public async Task<Prosumer?> GetByNicAsync(string nic)
        {
            // Gets a prosumer by NIC
            return await _prosumersCollection.Find(x => x.Nic == nic).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Prosumer>> GetAllAsync(int skip, int limit)
        {
            // Gets paginated list of prosumers
            return await _prosumersCollection.Find(_ => true).Skip(skip).Limit(limit).ToListAsync();
        }

        public async Task CreateAsync(Prosumer prosumer)
        {
            // Creates a new prosumer
            await _prosumersCollection.InsertOneAsync(prosumer);
        }

        public async Task UpdateAsync(Prosumer prosumer)
        {
            // Updates an existing prosumer
            await _prosumersCollection.ReplaceOneAsync(x => x.Nic == prosumer.Nic, prosumer);
        }
    }
}
