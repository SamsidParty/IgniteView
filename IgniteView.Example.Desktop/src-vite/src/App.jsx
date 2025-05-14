import React, { useState } from 'react'
import './App.css'
import Titlebar from './Titlebar'

function App() {

    return (
        <>
            <Titlebar></Titlebar>
            <div className="app">
                <div className="sidebar">
                    {JSON.stringify(igniteView.withReact(React).useCommandResult("igniteview_list_platform_hints"))}
                </div>
            </div>
        </>
    )
}

export default App
