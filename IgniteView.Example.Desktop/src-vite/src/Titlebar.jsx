import "./Titlebar.css";

export default function Titlebar() {
    return (
        <div className="titleBar">
            <h1>{document.title}</h1>
            <button>
                <svg width="24" height="24" fill="none" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path d="M3.997 13H20a1 1 0 1 0 0-2H3.997a1 1 0 1 0 0 2Z" fill="currentColor" /></svg>
            </button>
            <div onMouseOver={window.igniteView.dragWindow} className="dragArea"></div>
            <WindowButtons></WindowButtons>
        </div>
    )
}

function WindowButtons() {

    var isMaximized = window.innerWidth == screen.width;

    return (
        <div className="windowButtons">
            <button onClick={window.hide}>
                <svg width="24" height="24" fill="none" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path d="M3.997 13H20a1 1 0 1 0 0-2H3.997a1 1 0 1 0 0 2Z" fill="currentColor" /></svg>
            </button>
            <button onClick={window.toggleMaximize}>
                {
                    isMaximized ?
                    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor"><path d="M6.99979 7V3C6.99979 2.44772 7.4475 2 7.99979 2H20.9998C21.5521 2 21.9998 2.44772 21.9998 3V16C21.9998 16.5523 21.5521 17 20.9998 17H17V20.9925C17 21.5489 16.551 22 15.9925 22H3.00728C2.45086 22 2 21.5511 2 20.9925L2.00276 8.00748C2.00288 7.45107 2.4518 7 3.01025 7H6.99979ZM8.99979 7H15.9927C16.549 7 17 7.44892 17 8.00748V15H19.9998V4H8.99979V7ZM4.00255 9L4.00021 20H15V9H4.00255Z"></path></svg> :
                    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor"><path d="M4 3H20C20.5523 3 21 3.44772 21 4V20C21 20.5523 20.5523 21 20 21H4C3.44772 21 3 20.5523 3 20V4C3 3.44772 3.44772 3 4 3ZM5 5V19H19V5H5Z"></path></svg>
                }
            </button>
            <button className="closeButton" onClick={window.close}>
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor"><path d="M10.5859 12L2.79297 4.20706L4.20718 2.79285L12.0001 10.5857L19.793 2.79285L21.2072 4.20706L13.4143 12L21.2072 19.7928L19.793 21.2071L12.0001 13.4142L4.20718 21.2071L2.79297 19.7928L10.5859 12Z"></path></svg>
            </button>
        </div>
    )
}