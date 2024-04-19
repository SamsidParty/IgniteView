

function App() {
    
    var [rerender, setRerender] = React.useState(Math.random());
    window.setRerender = setRerender;

    return (
        <>
        
            <h1>IgniteView Feature Demo</h1>

            <div className="featureCards">
                <div className="featureCard">
                    <h2>JS-C# Interop</h2>
                    <p>
                        The Following Button Will Invoke A C# Method To Close This App.
                    </p>
                    <button onClick={() => CallCSharp("WebFramework.AppManager, WebFramework", "Quit")}>Close</button>
                </div>
                <div className="featureCard">
                    <button onClick={() => CallCSharp("WebFramework.Test.TestScript, WebFramework.Test", "OpenFilePicker")}>File Picker</button>
                </div>
                <div className="featureCard">
                
                </div>
                <div className="featureCard">
                
                </div>
            </div>

            <div className="footer">
                <p>©️ SamsidParty {new Date().getFullYear()}</p>
                <a style={{ marginLeft: "auto" }} href="https://github.com/SamsidParty/IgniteView" target="_blank">GitHub</a>
                <a href="https://samsidparty.com" target="_blank">Website</a>
            </div>

        </>
    )
}


var domNode = document.getElementById('root');
var root = ReactDOM.createRoot(domNode);
root.render(<App/>);
document.body.dispatchEvent(new Event("reactReady"));
setTimeout(() => document.body.dispatchEvent(new Event("reactReady")), 1000);