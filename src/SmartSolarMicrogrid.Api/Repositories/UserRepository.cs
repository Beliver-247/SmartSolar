/*
 * Purpose: User repository implementation.
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
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _usersCollection;

        public UserRepository(IOptions<MongoDbSettings> mongoDbSettings)
        {
            // Initializes the MongoDB connection and gets the collection
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _usersCollection = mongoDatabase.GetCollection<User>("Users");
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            // Gets a user by object id
            return await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            // Gets a user by username
            return await _usersCollection.Find(x => x.Username == username).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsync(int skip, int limit)
        {
            // Gets paginated list of users
            return await _usersCollection.Find(_ => true).Skip(skip).Limit(limit).ToListAsync();
        }

        public async Task CreateAsync(User user)
        {
            // Creates a new user
            await _usersCollection.InsertOneAsync(user);
        }

        public async Task UpdateAsync(User user)
        {
            // Updates an existing user
            await _usersCollection.ReplaceOneAsync(x => x.Id == user.Id, user);
        }

        public async Task DeleteAsync(string id)
        {
            // Deletes a user by id
            await _usersCollection.DeleteOneAsync(x => x.Id == id);
        }
    }
}
