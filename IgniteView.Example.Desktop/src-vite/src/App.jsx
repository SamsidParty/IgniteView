import { useState } from 'react'
import './App.css'
import Titlebar from './Titlebar'

function App() {
    return (
        <>
            <Titlebar></Titlebar>
            <video muted controls className="app" src={igniteView.commandBridge.streamedCommandTest()}></video>
        </>
    )
}

export default App
