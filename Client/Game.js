import React, { useEffect, useState } from 'react'
import * as signalR from '@microsoft/signalr'
import Board from './Component/Board'
import Stories from './Component/Stories'
import "./App.css" 
import ViewStory from './Component/ViewStory'

const Game = () => {
  const [connection, setConnection] = useState()
  const [gameState, setGameState] = useState([' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '])
  const [inRoom, setInRoom] = useState(false)
  const [symbol, setSymbol] = useState()
  const [currentSymbol, setCurrentSymbol] = useState()
  const [winner, setWinner] = useState(' ')
  const [stories, setStories]= useState([]) 
  const [isReplay, setIsReplay] = useState(false)
  const [currentStory, setCurrentStory] = useState()

  const winners = ['X is winner', 'O is winner', 'Draw!']

  useEffect(() => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7115/gameHub') 
      .withAutomaticReconnect()
      .build()
      
    setConnection(newConnection);
  }, [])

   useEffect(() => {
    if (connection) {
      connection.start()

      connection.on('UpdateGameState',(state, _currentSymbol) => {
        setCurrentSymbol(_currentSymbol)
        setGameState(state)
      })

      connection.on('UpdateSymbol', (userSymbol) => {
        setSymbol(userSymbol)
      })
 
      connection.on('UpdateData', (_stories) => {
        setStories(JSON.parse(_stories))
      })

      connection.on('UpdateWinner', (_winner) => {
        setTimeout(() => setWinner((el) => " "),2000)
        setWinner(winners[_winner])
      })
    }
  }, [connection])

  const joinRoom = (room) => {
    setInRoom(true)
    connection.invoke('JoinRoom', room)
  }

  const joinById = () => {
    setInRoom(true)
    var roomId = document.getElementById("roomId").value
    connection.invoke('JoinById',Number(roomId))
  }

  const makeMove = (index) => {
    connection.invoke('MakeMove', index)
  }

  const selectStory = (_story) => {
    setCurrentStory(_story)
    setIsReplay(true)
  }

  const backToGame = () => {
    setIsReplay(false)
  }

  return (
    <div className='container'>
    {
      !inRoom
      ? <div> 
          <button className='joinBtn' onClick={() => joinRoom(" ")}>Join Room</button>
          <div className='joinById'>
            <input placeholder='Join By Room Id' id="roomId" className='joinById-input'/>
            <input type="button" value="Go!" className='joinById-btn' onClick={() => joinById()}/>
          </div>
        </div> 
      
      : <div className="content">
        {
          !isReplay
          ? <>
              <Board symbol={symbol} currentSymbol={currentSymbol} message={winner } gameState={gameState} makeMove={makeMove} />
              <Stories stories={stories} selectStory={selectStory} />
            </>
          : <ViewStory story={currentStory} backToGame={backToGame}/>
        }  
        </div>
    }
    </div>
  )
}

export default Game