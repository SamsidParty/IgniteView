

function App() {
    
    var [rerender, setRerender] = React.useState(Math.random());
    window.setRerender = setRerender;

    return (
        <>
        
            <div className="footer">
                <p>©️ SamsidParty</p>
            </div>

        </>
    )
}


var domNode = document.getElementById('root');
var root = ReactDOM.createRoot(domNode);
root.render(<App/>);
document.body.dispatchEvent(new Event("reactReady"));
setTimeout(() => document.body.dispatchEvent(new Event("reactReady")), 1000);