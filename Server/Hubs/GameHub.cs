using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using tic_tac_toe.backend;

namespace tic_tac_toe.backend
{
    public class TicTacToeHub : Hub
    {
        private static TicTacToeGame _game = new TicTacToeGame();
        private static Dictionary<string, string> _playersRooms = new Dictionary<string, string>();

        public TicTacToeHub() { }

        public override Task OnConnectedAsync()
        {
            _game.AddPlayer(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var room = _playersRooms.GetValueOrDefault(Context.ConnectionId);
            if (!string.IsNullOrEmpty(room))
            {
                _game.RemovePlayer(Context.ConnectionId, room);
                _playersRooms.Remove(Context.ConnectionId);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public void JoinRoom(string a)
        {
            
            var room = _game.Add(_playersRooms, Context.ConnectionId);
            if (!_game.IsRoomFull(room))
            {
                _game.AddPlayerToRoom(Context.ConnectionId, room);
                _playersRooms[Context.ConnectionId] = room;
                Groups.AddToGroupAsync(Context.ConnectionId, room);
                Clients.Client(Context.ConnectionId).SendAsync("UpdateGameState", _game.GetGameState(room), _game.GetCurrentSymbol());
                Clients.Client(Context.ConnectionId).SendAsync("UpdateSymbol", _game.GetPlayerSymbol(Context.ConnectionId));
            }

        }

        public void MakeMove(int idx)
        {
            var room = _playersRooms.GetValueOrDefault(Context.ConnectionId);
            if (!string.IsNullOrEmpty(room))
            {
                var currentPlayer = _game.GetPlayer(Context.ConnectionId, room);
                if (currentPlayer != null && _game.MakeMove(idx, currentPlayer, room))
                {
                    Clients.Group(room).SendAsync("UpdateGameState", _game.GetGameState(room), _game.GetCurrentSymbol());
                    if(_game.CheckEnd(currentPlayer, room))
                    {
                        Thread.Sleep(2000);
                        Clients.Group(room).SendAsync("UpdateGameState", _game.GetGameState(room), _game.GetCurrentSymbol());
                    }
                }
            }
        }
    }
}