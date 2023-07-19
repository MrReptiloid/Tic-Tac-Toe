namespace tic_tac_toe.backend
{
    public class TicTacToeGame
    {
        private const int BoardSize = 9;
        private static Dictionary<string, char[]> _rooms = new Dictionary<string, char[]>();
        private static Dictionary<string, Player> _players = new Dictionary<string, Player>();
        private static char currentPlayer = 'X';
        private static int[,] winPos = new int[8, 3] {
            { 0, 1, 2 },
            { 3, 4, 5 },
            { 6, 7, 8 },
            { 0, 3, 6 },
            { 1, 4, 7 },
            { 2, 5, 8 },
            { 0, 4, 8 },
            { 2, 4, 6 } };

        public TicTacToeGame() { }

        public char GetPlayerSymbol(string connectionId)
        {
            if (_players.ContainsKey(connectionId) && _players[connectionId].IsInRoom)
            {
                return _players[connectionId].Symbol;
            }
            return '\0';
        }

        public char GetCurrentSymbol()
        {
            return currentPlayer;
        }

        public void AddPlayer(string connectionId)
        {
            _players[connectionId] = new Player(connectionId);
        }

        public void AddPlayerToRoom(string connectionId, string room)
        {

            if (!_rooms.ContainsKey(room))
            {
                _rooms[room] = new string(' ', BoardSize).ToCharArray();
            }

            if (_rooms[room] != null && !_players[connectionId].IsInRoom)
            {
                _players[connectionId].Room = room;
                _players[connectionId].IsInRoom = true;
            }
        }

        public string Add(Dictionary<string, string> playerRooms, string connectionId)
        {
            foreach (var room in playerRooms.Values)
            {
                if (!IsRoomFull(room))
                {
                    _players[connectionId].Symbol = 'O';
                    return room;
                }
            }
            _players[connectionId].Symbol = 'X';
            var newRoom = NewRoom();
            return newRoom;
        }

        public void RemovePlayer(string connectionId, string room)
        {
            if (_players.ContainsKey(connectionId) && _players[connectionId].IsInRoom && _players[connectionId].Room == room)
            {
                _players[connectionId].IsInRoom = false;
                _players[connectionId].Room = null;
            }
        }

        public Player GetPlayer(string connectionId, string room)
        {
            if (_players.ContainsKey(connectionId) && _players[connectionId].IsInRoom && _players[connectionId].Room == room)
            {
                return _players[connectionId];
            }
            return null;
        }

        public bool MakeMove(int idx, Player player, string room)
        {
            if (!IsRoomFull(room))
            {
                return !true;
            }
            if (player != null && player.Room == room && IsValidMove(idx, room) && player.Symbol == currentPlayer)
            {
                _rooms[room][idx] = player.Symbol;

                currentPlayer = (currentPlayer == 'X') ? 'O' : 'X';

                return true;
            }
            return false;
        }

        public bool CheckEnd(Player player, string room)
        {
            if (CheckWin(player, room) || IsBoardFull(room))
            {
                ResetGame(room);
                return true;
            }
            return false;
        }

        public bool IsRoomFull(string room)
        {
            if (_rooms.ContainsKey(room))
            {
                int count = 0;
                foreach (var player in _players.Values)
                {
                    if (player.IsInRoom && player.Room == room)
                    {
                        count++;
                    }
                }
                return count >= 2;
            }
            return false;
        }

        public char[] GetGameState(string room)
        {
            return _rooms.GetValueOrDefault(room);
        }

        private bool IsValidMove(int idx, string room)
        {
            return idx >= 0 && idx < BoardSize && _rooms.GetValueOrDefault(room)[idx] == ' ';
        }

        private bool CheckWin(Player player, string room)
        {
            var board = _rooms.GetValueOrDefault(room);
            var symbol = player.Symbol;

            for (var i = 0; i < winPos.GetLength(0); i++)
            {
                if (board[winPos[i, 0]] == symbol && board[winPos[i, 1]] == symbol && board[winPos[i, 2]] == symbol)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsBoardFull(string room)
        {
            var board = _rooms.GetValueOrDefault(room);
            for (int idx = 0; idx < BoardSize; idx++)
            {
                if (board[idx] == ' ')
                {
                    return false;
                }
            }
            return true;
        }

        private void ResetGame(string room)
        {
            _rooms[room] = new string(' ', BoardSize).ToCharArray();
        }

        public string NewRoom()
        {
            string roomname = Guid.NewGuid().ToString();
            return roomname;
        }
    }
}
