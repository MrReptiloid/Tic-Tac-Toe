using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using MongoDB.Driver.Core.Connections;
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
            return base.OnConnectedAsync();
        }

        public void LeaveRoom(string _default)
        {
            _game.RemovePlayer(Context.ConnectionId);
            _playersRooms.Remove(Context.ConnectionId);
        }

        public void JoinRoom(string _default)
        {
            var room = _game.Add(Context.ConnectionId);

            SubJoin(room);

            SendState(Context.ConnectionId);
        }

        public void JoinById(int room)
        {
            _game.AddById(Context.ConnectionId, room);

            SubJoin(room);

            SendState(Context.ConnectionId);
        }
        
        public void RestoreJoin(string restoreId)
        {
            UpdateConnectionId(restoreId, Context.ConnectionId);

            SendState(Context.ConnectionId);
        }

        private void SubJoin(int room)
        {
            _game.AddPlayerToRoom(Context.ConnectionId, room);
            _playersRooms[Context.ConnectionId] = room;
            Groups.AddToGroupAsync(Context.ConnectionId, room.ToString());
        }

        public void MakeMove(int idx)
        {
            var room = _playersRooms.GetValueOrDefault(Context.ConnectionId);

            var currentPlayer = _game.GetPlayer(Context.ConnectionId);

            if (currentPlayer != null && _game.MakeMove(idx, currentPlayer, room))
            {
                Clients.Group(room.ToString()).SendAsync("UpdateGameState", _game.GetGameState(room), _game.GetCurrentSymbol(), room);
                var outcome = _game.CheckEnd(room);
                if (outcome != Outcome.Skip)
                {
                    string storiesJson = GetJsonStories(room);

                    Clients.Group(room.ToString()).SendAsync("UpdateWinner", outcome);

                    Thread.Sleep(2000);

                    Clients.Group(room.ToString()).SendAsync("UpdateData", storiesJson);
                    Clients.Group(room.ToString()).SendAsync("UpdateGameState", _game.GetGameState(room), _game.GetCurrentSymbol(), room);
                }
            }
        }

        public string GetJsonStories(int room)
        {
            var stories = Story.ToSerializabledStories(_storyService.GetByRoomAsync(room).Result);
            string storiesJson = JsonConvert.SerializeObject(stories);

            return storiesJson;
        }


        private void SendState(string connectionId)
        {
            var room = _playersRooms.GetValueOrDefault(connectionId);

            string storiesJson = GetJsonStories(room);

            Clients.Client(Context.ConnectionId).SendAsync("UpdateData", storiesJson);
            Clients.Client(Context.ConnectionId).SendAsync("UpdateGameState", _game.GetGameState(room), _game.GetCurrentSymbol(), room);
            Clients.Client(Context.ConnectionId).SendAsync("UpdateSymbol", _game.GetPlayerSymbol(connectionId), _game.GetRoom(room));
        }

        public void UpdateConnectionId(string oldConnectionId, string newConnectionId)
        {
            int room = _playersRooms[oldConnectionId];
            _playersRooms.Remove(oldConnectionId);
            _playersRooms[newConnectionId] = room;

            _game.UpdatePlayer(oldConnectionId, newConnectionId);

            Groups.RemoveFromGroupAsync(oldConnectionId, room.ToString());
            Groups.AddToGroupAsync(newConnectionId, room.ToString());
        }
    }
}