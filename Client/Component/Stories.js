import React from 'react'
import Story from './Story'
import "../App.css"

const Stories = ({ stories, selectStory }) => {
    return (
        <div className="stories">
            {
                stories.map((story, index) => (
                    <Story key={index} story={story} selectStory={selectStory}/>
                ))
            }
        </div>
    )
} 

export default Stories