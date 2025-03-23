import { useState } from 'react'
import './App.css'
import Titlebar from './Titlebar'

function App() {
    return (
        <>
            <Titlebar></Titlebar>
            <iframe className="app" src="https://www.samsidparty.com/"></iframe>
        </>
    )
}

export default App
