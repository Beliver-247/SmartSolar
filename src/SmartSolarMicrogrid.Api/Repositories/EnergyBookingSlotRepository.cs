/*
 * Purpose: Booking slot repository implementation.
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
    public class EnergyBookingSlotRepository : IEnergyBookingSlotRepository
    {
        private readonly IMongoCollection<EnergyBookingSlot> _slotsCollection;

        public EnergyBookingSlotRepository(IOptions<MongoDbSettings> mongoDbSettings)
        {
            // Initializes the MongoDB connection and gets the collection
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _slotsCollection = mongoDatabase.GetCollection<EnergyBookingSlot>("EnergyBookingSlots");
        }

        public async Task<EnergyBookingSlot?> GetByIdAsync(string id)
        {
            // Gets a slot by id
            return await _slotsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<EnergyBookingSlot>> GetByStationIdAsync(string stationId)
        {
            // Gets all slots for a station
            return await _slotsCollection.Find(x => x.StationId == stationId).ToListAsync();
        }

        public async Task CreateAsync(EnergyBookingSlot slot)
        {
            // Creates a new slot
            await _slotsCollection.InsertOneAsync(slot);
        }

        public async Task UpdateAsync(EnergyBookingSlot slot)
        {
            // Updates an existing slot
            await _slotsCollection.ReplaceOneAsync(x => x.Id == slot.Id, slot);
        }

        public async Task<bool> UpdateStatusAtomicAsync(string id, string expectedStatus, string newStatus)
        {
            // Atomic update to avoid race conditions. Rule 9.
            var filter = Builders<EnergyBookingSlot>.Filter.And(
                Builders<EnergyBookingSlot>.Filter.Eq(x => x.Id, id),
                Builders<EnergyBookingSlot>.Filter.Eq(x => x.Status, expectedStatus)
            );

            var update = Builders<EnergyBookingSlot>.Update.Set(x => x.Status, newStatus);
            var result = await _slotsCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }
    }
}
