const fs = require('fs');
const path = require('path');
const exec = require('child_process').exec;

const scriptsURL = process.argv[1]; // $(ScriptsURL)
const projectDirectory = process.argv[2]; // $(MSBuildProjectDirectory)
process.chdir(projectDirectory);

function CreateViteProject() {
    console.log("No vite project found. Creating a new one...");
    return new Promise((resolve, reject) => {
        var prefix = "start \"\" cmd.exe /C ";
        exec(prefix + `node -e "fetch('${scriptsURL}/CreateViteProject.js').then((c) => c.text().then(eval))"`, (_, stdout, __) => { resolve(stdout); });
    });
}

async function PrebuildVite() {
    console.log("Detected Project Type: Vite");
    var sourcePath = path.join(projectDirectory, 'src-vite');

    if (!fs.existsSync(sourcePath)) {
        fs.mkdirSync(sourcePath);
    }

    var packageJsonPath = path.join(sourcePath, 'package.json');
    
    if (!fs.existsSync(packageJsonPath)) {
        await CreateViteProject();
    }
}

async function Main() {

    console.log("\n-------- IgniteView Prebuild Version 0.0.1 --------\n");

    // Determine the project type
    if (fs.existsSync(path.join(projectDirectory, 'wwwroot'))) {
        console.log("Detected Project Type: Static HTML");
    }
    else {
        await PrebuildVite();
    }
}

Main();