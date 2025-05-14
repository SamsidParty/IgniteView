
window.igniteView.withReact = (React) => ({
    useCommandResult: function() {
        var [result, setResult] = React.useState(null);
        var [loadingState, setLoadingState] = React.useState("none");

        if (loadingState == "none") {
            setLoadingState("loading");
            setTimeout(async () => {
                try {
                    setResult(await igniteView.commandBridge.invoke(...arguments));
                    setLoadingState("success");
                }
                catch {
                    setLoadingState("failed");
                }
            }, 0);
        }

        return result;
    }
})