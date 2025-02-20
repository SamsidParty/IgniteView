import { useState } from 'react'
import './App.css'

function App() {
    return (
        <>
            <div className="navBar">
                <img src="/Images/IgniteViewSmall.svg"></img>
                <h1>IgniteView</h1>
                <div className="seperator"></div>
                <button onClick={() => window.open("https://samsidparty.com/")}>
                    <img src="/Images/PartyIconMono.png"></img>
                </button>
                <button onClick={() => window.open("https://github.com/SamsidParty/IgniteView")}>
                    <img src="/Images/GitHub.png"></img>
                </button>
            </div>
            <div className="content">

            </div>
        </>
    )
}

export default App
