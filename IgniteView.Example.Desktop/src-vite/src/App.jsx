import React, { useState } from 'react'
import './App.css'
import Titlebar from './Titlebar'

function App() {

    return (
        <>
            <Titlebar></Titlebar>
            <div className="app">
                <h1>{igniteView.withReact(React).useCommandResult("getUsernameAsync")}</h1>
            </div>
        </>
    )
}

export default App
