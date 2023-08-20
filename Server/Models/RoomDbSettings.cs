namespace tic_tac_toe.backend.Models
{
    public class RoomDbSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string RoomCollectionName { get; set; } = null!;
    }
}
