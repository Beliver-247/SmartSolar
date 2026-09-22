/*
 * Purpose: Station repository implementation.
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
    public class StationRepository : IStationRepository
    {
        private readonly IMongoCollection<SolarStationInfo> _stationsCollection;

        public StationRepository(IOptions<MongoDbSettings> mongoDbSettings)
        {
            // Initializes the MongoDB connection and gets the collection
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _stationsCollection = mongoDatabase.GetCollection<SolarStationInfo>("SolarStationInfo");
        }

        public async Task<SolarStationInfo?> GetByIdAsync(string id)
        {
            // Gets a station by object id
            return await _stationsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<SolarStationInfo>> GetAllAsync(int skip, int limit)
        {
            // Gets paginated list of stations
            return await _stationsCollection.Find(_ => true).Skip(skip).Limit(limit).ToListAsync();
        }

        public async Task CreateAsync(SolarStationInfo station)
        {
            // Creates a new station
            await _stationsCollection.InsertOneAsync(station);
        }

        public async Task UpdateAsync(SolarStationInfo station)
        {
            // Updates an existing station
            await _stationsCollection.ReplaceOneAsync(x => x.Id == station.Id, station);
        }

        public async Task DeleteAsync(string id)
        {
            // Deletes a station
            await _stationsCollection.DeleteOneAsync(x => x.Id == id);
        }
    }
}
