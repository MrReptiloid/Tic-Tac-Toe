import React, { useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';
import './App.css';

type GameState = string[];

const App: React.FC = () => {
  const [connection, setConnection] = useState<signalR.HubConnection>();
  const [gameState, setGameState] = useState<GameState>([]);
  const [inRoom, setInRoom] = useState(false);
  const [symbol, setSymbol] = useState<string>();
  const [currentSymbol, setCurrentSymbol] = useState<string>();
  const [message, setMessage] = useState<string>(' ');

  useEffect(() => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7263/gameHub')
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);
  }, []);

  useEffect(() => {
    if (connection) {
      connection.start().then()

      connection.on('UpdateGameState', (state: GameState, _currentSymbol: string) => {
        setCurrentSymbol(_currentSymbol);
        setGameState(state);
        checkWin(state);
      });

      connection.on('UpdateSymbol', (userSymbol: string) => {
        setSymbol(userSymbol);
      });
    }
  }, [connection]);

  const joinRoom = (room: string) => {
    setInRoom(true);
    connection?.invoke('JoinRoom', room);
  };

  const makeMove = (index: number) => {
    connection?.invoke('MakeMove', index);
  };

  const checkWin = (state: GameState) => {
    const winningCombinations: number[][] = [
      [0, 1, 2],
      [3, 4, 5],
      [6, 7, 8],
      [0, 3, 6],
      [1, 4, 7],
      [2, 5, 8],
      [0, 4, 8],
      [2, 4, 6],
    ];
    winningCombinations.forEach(combination => {
      if (
        state[combination[0]] === 'X' &&
        state[combination[1]] === 'X' &&
        state[combination[2]] === 'X'
      ) {
        setTimeout(() => setMessage(el => ' '), 2000);
        setMessage(prev => 'X is winner');
      } else if (
        state[combination[0]] === 'O' &&
        state[combination[1]] === 'O' &&
        state[combination[2]] === 'O'
      ) {
        setTimeout(() => setMessage(el => ' '), 2000);
        setMessage(prev => 'O is winner');
      } else if (state.indexOf(' ') === -1) {
        setTimeout(() => setMessage(el => ' '), 2000);
        setMessage(prev => 'Draw!!!');
      }
    });
  };

  return (
    <div className="container">
      {!inRoom ? (
        <button className="joinBtn" onClick={() => joinRoom(' ')}>
          Join Room
        </button>
      ) : (
        <div className="board">
          <div className="text">Your symbol is {symbol}</div>
          <div className="text">Now go {currentSymbol}</div>
          <div className="text">{message}</div>
          {gameState.map((cell, index) => (
            <div className="cell" key={index} onClick={() => makeMove(index)}>
              {cell}
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default App;
