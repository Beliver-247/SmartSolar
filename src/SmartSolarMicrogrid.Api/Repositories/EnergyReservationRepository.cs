/*
 * Purpose: Energy reservation repository implementation.
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
    public class EnergyReservationRepository : IEnergyReservationRepository
    {
        private readonly IMongoCollection<EnergyReservation> _reservationsCollection;

        public EnergyReservationRepository(IOptions<MongoDbSettings> mongoDbSettings)
        {
            // Initializes the MongoDB connection and gets the collection
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _reservationsCollection = mongoDatabase.GetCollection<EnergyReservation>("EnergyReservations");
        }

        public async Task<EnergyReservation?> GetByIdAsync(string id)
        {
            // Gets a reservation by id
            return await _reservationsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<EnergyReservation>> GetAllAsync(int skip, int limit)
        {
            // Gets paginated list of reservations
            return await _reservationsCollection.Find(_ => true).Skip(skip).Limit(limit).ToListAsync();
        }

        public async Task<IEnumerable<EnergyReservation>> GetByNicAsync(string nic, int skip, int limit)
        {
            // Gets paginated list of reservations for a prosumer
            return await _reservationsCollection.Find(x => x.Nic == nic).Skip(skip).Limit(limit).ToListAsync();
        }

        public async Task<IEnumerable<EnergyReservation>> GetByStatusAsync(string status, int skip, int limit)
        {
            // Gets paginated list of reservations by status
            return await _reservationsCollection.Find(x => x.Status == status).Skip(skip).Limit(limit).ToListAsync();
        }

        public async Task<bool> HasActiveReservationsForStationAsync(string stationId)
        {
            // Checks if a station has active (Pending or Approved) reservations. Rule 3.
            var filter = Builders<EnergyReservation>.Filter.And(
                Builders<EnergyReservation>.Filter.Eq(x => x.StationId, stationId),
                Builders<EnergyReservation>.Filter.In(x => x.Status, new[] { ReservationStatus.Pending, ReservationStatus.Approved })
            );

            return await _reservationsCollection.Find(filter).AnyAsync();
        }

        public async Task<long> CountActiveReservationsAsync(string stationId, string bookingDate, string timeSlot)
        {
            var filter = Builders<EnergyReservation>.Filter.And(
                Builders<EnergyReservation>.Filter.Eq(x => x.StationId, stationId),
                Builders<EnergyReservation>.Filter.Eq(x => x.BookingDate, bookingDate),
                Builders<EnergyReservation>.Filter.Eq(x => x.TimeSlot, timeSlot),
                Builders<EnergyReservation>.Filter.In(x => x.Status, new[] { ReservationStatus.Pending, ReservationStatus.Approved })
            );
            return await _reservationsCollection.CountDocumentsAsync(filter);
        }

        public async Task<IEnumerable<EnergyReservation>> GetActiveReservationsForDateAsync(string stationId, string bookingDate)
        {
            var filter = Builders<EnergyReservation>.Filter.And(
                Builders<EnergyReservation>.Filter.Eq(x => x.StationId, stationId),
                Builders<EnergyReservation>.Filter.Eq(x => x.BookingDate, bookingDate),
                Builders<EnergyReservation>.Filter.In(x => x.Status, new[] { ReservationStatus.Pending, ReservationStatus.Approved })
            );
            return await _reservationsCollection.Find(filter).ToListAsync();
        }

        public async Task CreateAsync(EnergyReservation reservation)
        {
            // Creates a new reservation
            await _reservationsCollection.InsertOneAsync(reservation);
        }

        public async Task UpdateAsync(EnergyReservation reservation)
        {
            // Updates an existing reservation
            await _reservationsCollection.ReplaceOneAsync(x => x.Id == reservation.Id, reservation);
        }

        public async Task DeleteAsync(string id)
        {
            // Deletes a reservation by id
            await _reservationsCollection.DeleteOneAsync(x => x.Id == id);
        }
    }
}
