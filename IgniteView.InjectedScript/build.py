
import os
import json

def add_to_cs_file(script_data):
    csharp_code = ""
    with open("./InjectedScript.cs", "r") as f:
        csharp_code = f.read()

    csharp_code = csharp_code.replace("\"%SCRIPT_DATA%\"", json.dumps(script_data))

    # Write the new C# code to ../IgniteView.Core/Types/InjectedScript.cs
    with open("../IgniteView.Core/Types/InjectedScript.cs", "w") as f:
        f.write(csharp_code)
    
    print("Wrote to ../IgniteView.Core/Types/InjectedScript.cs")

def combine_js_files():
    # Find all js files in the current folder and concat them together
    js_code = ""
    js_files = [f for f in os.listdir() if f.endswith(".js")]

    # Sort js_files by file name in alphabetical order
    js_files.sort()

    for js_file in js_files:
        with open(js_file, "r") as f:
            print("Adding file " + f.name)
            js_code += "\n" + f.read()
    
    return js_code

def main():
    add_to_cs_file(combine_js_files())
    print("Build successful")

if (__name__ == "__main__"):
    main()