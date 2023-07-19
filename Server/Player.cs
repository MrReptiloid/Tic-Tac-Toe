namespace tic_tac_toe.backend
{
    public class Player
    {
        public string ConnectionId { get; }
        public char Symbol { get; set; }
        public string Room { get; set; }
        public bool IsInRoom { get; set; }

        public Player(string connectionId)
        {
            ConnectionId = connectionId;
            Room = null;
            IsInRoom = false;
        }
    }
}
