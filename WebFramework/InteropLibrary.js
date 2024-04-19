
//This Library Helps Interop With The C# Part Of The Codebase

//A Dict Of <UUID as a string, Element>
var JSI_Elements = {};
JSI_Elements["body"] = document.body;
JSI_Elements["head"] = document.head;
JSI_Elements["root"] = document.querySelector(":root");

JSI_AddObservers();
JSI_TitleChanged(document.title);

function JSI_Ready() {
    JSI_LoadScripts();
}

function JSI_LoadScripts() {
    console.log("[SCRIPT LOADER] Loader Ready");
    var scripts = document.querySelectorAll("webscript");

    scripts.forEach((s) => {
        var sn = s.getAttribute("name");
        console.log("[SCRIPT LOADER] Attaching Script With Name " + sn);
        JSI_Send("attach", sn);
    });
}

function JSI_EventListener(eventToListen, eventID) {
    addEventListener(eventToListen, (e) => { JSI_OnEvent(e, eventID); });
}

function JSI_LocalEventListener(eventToListen, cb) {
    addEventListener(eventToListen, (event) => { eval(cb); });
}

function JSI_On(eventToListen, eventID, el) {
    if(!JSI_Elements[el]) { return ""; }
    JSI_Elements[el].addEventListener(eventToListen, (e) => { JSI_OnEvent(e, eventID); });
}

function JSI_LocalOn(eventToListen, cb, el) {
    if(!JSI_Elements[el]) { return ""; }
    JSI_Elements[el].addEventListener(eventToListen, (event) => { eval(cb); });
}

function JSI_OnEvent(event, eventID) {
    console.log(JSI_SerEvent(event));
    JSI_Send("event", eventID, JSI_SerEvent(event));
}

//Register An Element For Interop With C#
function JSI_RegisterInteropElement(el, id) {
    if (!document.querySelector(el)) { return "false"; }
    JSI_Elements[id] = document.querySelector(el);
    return "true";
}

function JSI_AppendChild(pid, cid) {
    if (!JSI_Elements[pid]) { return ""; }
    if (!JSI_Elements[cid]) { return ""; }

    JSI_Elements[pid].appendChild(JSI_Elements[cid]);
}

function JSI_RemoveInteropElement(id) {
    if (!JSI_Elements[id]) { return ""; }
    delete JSI_Elements[id];
}

function JSI_GetProperty(el, prop) {
    if (!JSI_Elements[el]) { return ""; }
    return JSI_Elements[el][prop];
}

function JSI_SetProperty(el, prop, val) {
    if (!JSI_Elements[el]) { return ""; }
    JSI_Elements[el][prop] = val;
    return val;
}

function JSI_SetCSSOnElement(el, prop, val) {
    if (!JSI_Elements[el]) { return ""; }
    JSI_Elements[el].style.setProperty(prop, val);
    return val;
}

function JSI_GetCSSOnElement(el, prop) {
    if (!JSI_Elements[el]) { return ""; }
    return JSI_Elements[el].style.getProperty(prop);
}

function JSI_RunFunctionOnElement(el, fn, ...args) {
    if (!JSI_Elements[el]) { return ""; }
    return JSI_Elements[el][fn](...args);
}

//Send Data To C#
function JSI_Send(type, param1, param2, param3) {
    var data = {
        "Type": type,
        "Param1": param1,
        "Param2": param2,
        "Param3": param3
    }

    console.log(JSON.stringify(data));
    window.external.sendMessage(JSON.stringify(data));
}

//Set CSS Property
function JSI_CSS(selector, prop, value) {
    document.querySelector(selector).style.setProperty(prop, value);
}

//Inject CSS
function JSI_CSSRaw(styleString) {
    let style = document.createElement('style');
    style.textContent = styleString;
    document.head.append(style);
}

function JSI_SerEvent(object, depth = 0, max_depth = 8) {

    if (depth > max_depth)
        return 'Object';

    const obj = {};
    for (let key in object) {
        let value = object[key];
        if (value instanceof Node)
            value = { id: value.id };
        else if (value instanceof Window)
            value = 'Window';
        else if (value instanceof Object)
            value = JSI_SerEvent(value, depth + 1, max_depth);

        obj[key] = value;
    }

    return depth ? obj : JSON.stringify(obj);
}

function JSI_AddObservers() {
    new MutationObserver(function (mutations) {
        JSI_TitleChanged(document.title);
    }).observe(
        document.querySelector('title'),
        { subtree: true, characterData: true, childList: true }
    );
}

function JSI_CreateElement(type) {
    var el = document.createElement(type);
    var id = crypto.randomUUID();
    JSI_Elements[id] = el;
    return id;
}

function JSI_TitleChanged(newTitle) {
    JSI_Send("title", newTitle);
}

window.CallCSharp = function () {
    var args = Array.from(arguments);
    if (args.length == 0) {
        return;
    }

    JSI_Send("reflect", args.shift(), args.shift(), JSON.stringify(args));
}

var ready = document.createEvent('CustomEvent');
ready.initCustomEvent('jsiready', true, false);
document.dispatchEvent(ready);
window.JSI_IsReady = true;

