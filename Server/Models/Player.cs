namespace tic_tac_toe.backend.Models
{
    public class Player
    {
        public string ConnectionId { get; set; }
        public char Symbol { get; set; }
        public int Room { get; set; }
        public bool IsInRoom { get; set; }

        public Player(string connectionId)
        {
            ConnectionId = connectionId;
            IsInRoom = false;
        }
    }
}
