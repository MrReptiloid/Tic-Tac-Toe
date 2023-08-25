using MongoDB.Driver;
using tic_tac_toe.backend.Models;
using Newtonsoft.Json;

namespace tic_tac_toe.backend.Services
{
    [JsonObject(MemberSerialization.Fields)]
    public class StoryService
    {
        private const string connectionString = "mongodb://localhost:27017";

        private IMongoClient _client;
        private IMongoDatabase _database;
        private readonly IMongoCollection<Story> _storiesCollections;

        public StoryService()
        {
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase("Tic-Tac-Toe");
            _storiesCollections = _database.GetCollection<Story>("Stories");
        }

        public async Task CreateAsync(Story story)
        {
            await _storiesCollections.InsertOneAsync(story);
        }
            
            
        public async Task<List<Story>> GetByRoomAsync(int roomId) =>
            await _storiesCollections.Find(x => x.RoomId == roomId).ToListAsync();

        public async Task<int> GetLastIdOrDefault()
        {
            var stories = await _storiesCollections.Find(_ => true).ToListAsync();
            try
            {
                return stories.Last().StoryId;
            }
            catch
            {
                return 1000;
            }
        }
    }
}
