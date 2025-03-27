import { useState } from 'react'
import './App.css'
import Titlebar from './Titlebar'

function App() {

    return (
        <>
            <Titlebar></Titlebar>
            <div className="app">
                <h1>{}</h1>
            </div>
        </>
    )
}

export default App
