using MongoDB.Driver;
using Microsoft.Extensions.Options;
using tic_tac_toe.backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace tic_tac_toe.backend.Services
{
    public class RoomService
    {
        private const string connectionString = "mongodb://localhost:27017";

        private IMongoClient _client;
        private IMongoDatabase _database;
        private readonly IMongoCollection<Room> _roomsCollections;

        public RoomService()
        {
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase("Tic-Tac-Toe");
            _roomsCollections = _database.GetCollection<Room>("Rooms");
        }

        public async Task<Room?> GetAsync(int roomId) => 
            await _roomsCollections.Find(x => x.RoomId == roomId).FirstAsync();

        public async Task CreateAsync(Room room) =>
            await _roomsCollections.InsertOneAsync(room);

        public async Task UpdateAsync(Room updateRoom) =>
            await _roomsCollections.ReplaceOneAsync(x => x.RoomId == updateRoom.RoomId, updateRoom);

        public async Task<Room?> GetRoomByPlayerAsync(string connectionId) =>
            await _roomsCollections.Find(x => x.Players.Contains(connectionId) == true).FirstAsync();

        public Room GetFree()
        {
            foreach (Room room in _roomsCollections.AsQueryable())
            {
                if (!room.IsRoomFull())
                    return room;
            }
            var newRoom = new Room(GetLastIdOrDefault().Result + 1);
            Task.Run(() => CreateAsync(newRoom));
            return newRoom;
        }

        public async Task<int> GetLastIdOrDefault()
        {
            var rooms = await _roomsCollections.Find(_ => true).ToListAsync();
            try
            {
                return rooms.Last().RoomId;
            }
            catch
            {
                return 1000;
            }
        }
    }
}
