using MongoDB.Driver.Core.Connections;
using tic_tac_toe.backend.Models;
using tic_tac_toe.backend.Services;

namespace tic_tac_toe.backend
{
    public class TicTacToeGame
    {
        private readonly RoomService _roomService = new RoomService();
        private readonly StoryService _storyService = new StoryService();

        private const int BoardSize = 9;
        private static Dictionary<int, char[]> _rooms = new Dictionary<int, char[]>();
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

        private List<Step> _steps = new List<Step>(); 
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

        public void AddPlayerToRoom(string connectionId, int room)
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

        public int Add(string connectionId)
        {
            Room room = _roomService.GetFree();

            InitPlayer(connectionId, room);

            return room.RoomId;
        }

        public void AddById(string connectionId, int roomId)
        {
            Room room = _roomService.GetAsync(roomId).Result;

            InitPlayer(connectionId, room);
        }

        private void InitPlayer(string connectionId, Room room)
        {
            if (room.Players.Count == 0)
            {
                room.Players.Add(connectionId);
                _players[connectionId].Symbol = 'X';
            }
            else
            {
                room.Players.Add(connectionId);
                _players[connectionId].Symbol = GetPlayer(room.Players[0]).Symbol == 'X' ? 'O' : 'X';
            }

            _ = _roomService.UpdateAsync(room);
        }

        public void RemovePlayer(string connectionId)
        {
            var room = _roomService.GetRoomByPlayerAsync(connectionId).Result;

            room.Players.Remove(connectionId);

            Console.WriteLine(room.Players.Count);

            if (room.Players.Count == 0)
                ResetGame(room.RoomId);

            Task.Run(() => _roomService.UpdateAsync(room));


            if (_players.ContainsKey(connectionId))
            {
                _players[connectionId].IsInRoom = false;
                _players[connectionId].Room = 0;
            }
        }
        
        public Player GetPlayer(string connectionId)
        {
            if (_players.ContainsKey(connectionId))
            {
                return _players[connectionId];
            }
            return null;
        }
        
        public bool MakeMove(int idx, Player player, int room)
        {
            if (!IsRoomFull(room))
            {
                return !true;
            }


            if (player != null && player.Room == room && IsValidMove(idx, room) && player.Symbol == currentPlayer)
            {

                _steps.Add(new Step(idx, currentPlayer));

                _rooms[room][idx] = player.Symbol;

                currentPlayer = (currentPlayer == 'X') ? 'O' : 'X';

                return true;
            }
            return false;
        }

        public Outcome CheckEnd(int room)
        {
            var result = CheckWin(room);
            if (result != Outcome.Skip)
            {
                ResetGame(room);
            }
            return result;
        }

        public bool IsRoomFull(int room)
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

        public char[] GetGameState(int room)
        {
            return _rooms.GetValueOrDefault(room);
        }

        private bool IsValidMove(int idx, int room)
        {
            return idx >= 0 && idx < BoardSize && _rooms.GetValueOrDefault(room)[idx] == ' ';
        }

        private Outcome CheckWin(int roomId)
        {
            var board = _rooms.GetValueOrDefault(roomId);
            var room = _roomService.GetAsync(roomId).Result;

            if (room == null || board == null)
                return Outcome.Skip;

            for (var i = 0; i < winPos.GetLength(0); i++)
            {
                if (board[winPos[i, 0]] == 'X' && board[winPos[i, 1]] == 'X' && board[winPos[i, 2]] == 'X')
                    return OutcomeUpdate(room, Outcome.WinX);

                else if (board[winPos[i, 0]] == 'O' && board[winPos[i, 1]] == 'O' && board[winPos[i, 2]] == 'O')
                    return OutcomeUpdate(room, Outcome.WinO);
            }

            if (!board.Contains(' '))
                return OutcomeUpdate(room, Outcome.Draw);

            return Outcome.Skip;
        }

        public Room GetRoom(int roomId) =>
            _roomService.GetAsync(roomId).Result;


        private Outcome OutcomeUpdate(Room room, Outcome outcome)
        {
            room.Stats.UpdateStats(outcome);
            _ = _roomService.UpdateAsync(room);

            var _currentStory = new Story();

            _currentStory.StoryId = _storyService.GetLastIdOrDefault().Result + 1;
            _currentStory.SetWinner(outcome);
            _currentStory.EndTime = DateTime.Now;
            _currentStory.RoomId = room.RoomId;
            _currentStory.AddSteps(_steps);
            

            Task.Run(() => _storyService.CreateAsync(_currentStory));

            _steps = new List<Step>();

            return outcome;
        }

        private void ResetGame(int room)
        {
            _rooms[room] = new string(' ', BoardSize).ToCharArray();
        }

        public void UpdatePlayer(string oldConnectionId, string newConnectionId)
        {
            Player player = _players[oldConnectionId];
            player.ConnectionId = newConnectionId;
            _players.Remove(oldConnectionId);
            _players[newConnectionId] = player;

            var room = _roomService.GetRoomByPlayerAsync(oldConnectionId).Result;
            room.Players.Remove(oldConnectionId);
            room.Players.Add(newConnectionId);
            Task.Run(() => _roomService.UpdateAsync(room));
        }
    }

    public enum Outcome{
        WinX,
        WinO,
        Draw,
        Skip
    }
}
