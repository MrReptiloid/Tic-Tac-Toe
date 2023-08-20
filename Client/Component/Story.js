import React from 'react'

const Story = ({ story, selectStory }) => {
    var count = [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',]

    let startTime = story.StartTime.split("T")
    let endTime = story.EndTime.split("T")

    const Winner = ["X", "O", "Draw"]

    for (let i = 0; i < count.length; i++){
        try{
            count[story.Steps[i].Idx] = story.Steps[i].Symbol 
        }
        catch{}
    }
    
    return (
        <div className="story" title="Click to view step by step">
            <div className="story-info">
                <div>Story Id: {story.StoryId}</div>
                <div>Start Match: {startTime[0].replaceAll('-','.')} {startTime[1].split('.')[0]}</div>
                <div>End Match: {endTime[0].replaceAll('-','.')} {endTime[1].split('.')[0]}</div>
                <div>Winner: {Winner[story.Winner]}</div>
            </div>
            <div className='right'>
                <div className="mini-board">
                    {
                        count.map((cell, index) => 
                            <div key={index} className="mini-cell">{cell}</div>
                        )
                    }
                    
                </div>
                <button className="replay-btn" onClick={() => selectStory(story)}>Replay</button>
            </div>
            
        </div>
    )
} 

export default Story