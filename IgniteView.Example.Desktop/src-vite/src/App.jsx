import React from 'react';
import './App.css';
import Titlebar from './Titlebar';

function App() {

    return (
        <>
            <Titlebar></Titlebar>
            <div className="app">
                <div className="sidebar">
                    {JSON.stringify(igniteView.withReact(React).useCommandResult("igniteview_list_platform_hints"))}
                    {igniteView.withReact(React).useSharedContext("Test").value}
                </div>
                <div className="content">

                </div>
            </div>
        </>
    )
}

export default App
