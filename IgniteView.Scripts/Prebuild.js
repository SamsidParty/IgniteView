const fs = require('fs');
const path = require('path');
const exec = require('child_process').exec;
const spawnSync = require('child_process').spawnSync;

const scriptsURL = process.argv[1]; // $(ScriptsURL)
const projectDirectory = process.argv[2]; // $(MSBuildProjectDirectory)
const buildConfiguration = process.argv[3]; // $(Configuration)
process.chdir(projectDirectory); // cd into the project directory

var commandPrefix = "start \"\" cmd.exe /C ";

function CreateViteProject() {
    console.log("No vite project found. Creating a new one...");
    return new Promise((resolve, reject) => {
        exec(commandPrefix + `node -e "fetch('${scriptsURL}/CreateViteProject.js').then((c) => c.text().then(eval))"`, (_, stdout, __) => { resolve(stdout); });
    });
}

function BuildViteProject() {
    console.log("Building vite project...");
    spawnSync(/^win/.test(process.platform) ? 'npx.cmd' : 'npx', ['vite', 'build', '--emptyOutDir', '--outDir', '../dist'], { stdio: 'inherit' });
}

function DevViteProject() {
    
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

    if (buildConfiguration.toLowerCase().includes("debug")) {
        console.log("Detected debug mode");
    }
    else {
        console.log("Detected release mode");
        BuildViteProject();
    }
}

async function Main() {

    console.log("\n-------- IgniteView Prebuild Version 0.0.1 --------\n");

    // Determine the project type
    if (fs.existsSync(path.join(projectDirectory, 'wwwroot'))) {
        process.chdir(path.join(process.cwd(), "wwwroot"));
        console.log("Detected Project Type: Static HTML");
    }
    else {
        process.chdir(path.join(process.cwd(), "src-vite"));
        await PrebuildVite();
    }
}

Main();