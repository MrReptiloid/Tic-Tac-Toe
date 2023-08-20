using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using tic_tac_toe.backend.Models;
using tic_tac_toe.backend.Services;


namespace tic_tac_toe.backend
{
    public class TicTacToeHub : Hub
    {
        private readonly StoryService _storyService = new StoryService();
        private static TicTacToeGame _game = new TicTacToeGame();
        private static Dictionary<string, int> _playersRooms = new Dictionary<string, int>();

        public override Task OnConnectedAsync()
        {
            _game.AddPlayer(Context.ConnectionId);
            Console.WriteLine(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _game.RemovePlayer(Context.ConnectionId);
            _playersRooms.Remove(Context.ConnectionId);

            

            return base.OnDisconnectedAsync(exception);
        }

        public void JoinRoom(string _default)
        { 
            var room = _game.Add(Context.ConnectionId);

            SubJoin(room);
        }

        public void JoinById(int room)
        {
            _game.AddById(Context.ConnectionId, room);

            SubJoin(room);
        }

        private void SubJoin(int room)
        {
            _game.AddPlayerToRoom(Context.ConnectionId, room);
            _playersRooms[Context.ConnectionId] = room;
            Groups.AddToGroupAsync(Context.ConnectionId, room.ToString());

            string storiesJson = GetJsonStories(room);

            Clients.Client(Context.ConnectionId).SendAsync("UpdateData", storiesJson);
            Clients.Client(Context.ConnectionId).SendAsync("UpdateGameState", _game.GetGameState(room), _game.GetCurrentSymbol());
            Clients.Client(Context.ConnectionId).SendAsync("UpdateSymbol", _game.GetPlayerSymbol(Context.ConnectionId), _game.GetRoom(room));
        }

        public void MakeMove(int idx)
        {
            var room = _playersRooms.GetValueOrDefault(Context.ConnectionId);

            var currentPlayer = _game.GetPlayer(Context.ConnectionId);
            if (currentPlayer != null && _game.MakeMove(idx, currentPlayer, room))
            {
                Clients.Group(room.ToString()).SendAsync("UpdateGameState", _game.GetGameState(room), _game.GetCurrentSymbol());
                var outcome = _game.CheckEnd(room);
                if (outcome != Outcome.Skip)
                {
                    Thread.Sleep(2000);
                    string storiesJson = GetJsonStories(room);

                    Clients.Group(room.ToString()).SendAsync("UpdateData", storiesJson);
                    Clients.Group(room.ToString()).SendAsync("UpdateGameState", _game.GetGameState(room), _game.GetCurrentSymbol());
                    Clients.Group(room.ToString()).SendAsync("UpdateWinner", outcome);    
                }
            }
        }

        public string GetJsonStories(int room)
        {
            var stories = Story.ToSerializabledStories(_storyService.GetByRoomAsync(room).Result);
            string storiesJson = JsonConvert.SerializeObject(stories);

            return storiesJson;
        }
    }
}