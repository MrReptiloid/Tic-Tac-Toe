import React from 'react'

const Board = ({ symbol, currentSymbol, message, gameState, makeMove }) => {

    return (
        <div className="game">
            <div className='text'>Your symbol is {symbol}</div>
            <div className='text'>Now go {currentSymbol}</div>
            <div className='text'>{message}</div>
 
            <div className='board'>
            {
                gameState.map((cell, index) => (
                    <div className="cell" key={index} onClick={() => makeMove(index)}>{cell}
                    </div>
                ))
            }
            </div>
            
        </div>
    )
} 

export default Board