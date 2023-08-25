import React, { useState } from 'react'

const ViewStory = ({ story, backToGame }) => {
    const [board, setBoard] = useState([' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '])
    const [currentIndex, setCurrentIndex] = useState(0);
    const [message, setMessage] = useState(" ")

    const winner = ["Winner is X", "winner is O", "Draw!"]

    let temp  =  [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ']
    

    const updateArrayWithDelay = (index) => {
        if (index === 0){
          setMessage(" ") 
          setBoard(temp)
        }
        if (index >= story.Steps.length) {
          return;
        }
    
        const newArray = temp;
        newArray[story.Steps[index].Idx] = story.Steps[index].Symbol;
        setBoard(newArray);
    
        setCurrentIndex(index + 1);   

        if (index + 1 < board.length) {
          setTimeout(() => {
            updateArrayWithDelay(index + 1);
          }, 1000);
        }

        if (index + 1 === story.Steps.length) {
          setMessage(winner[story.Winner])
        }
    
      };
    
      const startUpdating = () => {
        updateArrayWithDelay(0);
      };

    return (
      <div className="game">
          <div className='text'>{message}</div>
          <div className='board'>
          { 
              board.map((cell, index) => (
                  <div className="cell" key={index} >{cell}</div>
              ))
          }
          </div>
          <button className='back-btn' onClick={backToGame}>Back</button>
          <button className='run-btn' onClick={startUpdating}>Run</button>
      </div>
    )
}

export default ViewStory