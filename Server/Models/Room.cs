using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace tic_tac_toe.backend.Models
{
    public class Room
    {
        public Room(int roomId)
        {
            RoomId = roomId;
            Players = new List<string>();
            Stats = new RoomStats();   
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)] 
        public string? Id { get; set; }
        public int RoomId { get; set; }
        public List<string> Players { get; set; }
        public RoomStats Stats { get; set; }

        public bool IsRoomFull()
        {
            if (Players is null)
                return false;

            return Players.Count >= 2 ? true : false;
        }
    }
}
