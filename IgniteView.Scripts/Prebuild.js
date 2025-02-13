const fs = require('fs');
const path = require('path');
const execSync = require('child_process').execSync;
const spawn = require('child_process').spawn;
const spawnSync = require('child_process').spawnSync;

const scriptsURL = process.argv[1]; // $(ScriptsURL)
const projectDirectory = process.argv[2]; // $(MSBuildProjectDirectory)
const buildConfiguration = process.argv[3]; // $(Configuration)
const jsFramework = process.argv[4]; // $(JSFramework)
process.chdir(projectDirectory); // cd into the project directory

var supportedFrameworks = ["vanilla", "vanilla-ts", "vue", "vue-ts", "react", "react-ts", "react-swc", "react-swc-ts", "preact", "preact-ts", "lit", "lit-ts", "svelte", "svelte-ts", "solid", "solid-ts", "qwik", "qwik-ts"]

if (!supportedFrameworks.includes(jsFramework)) {
    console.error(`The JavaScript framework "${jsFramework}" is not supported by IgniteView. Supported frameworks are: ${supportedFrameworks.join(", ")}`);
    process.exit(1);
}

function CreateViteProject() {
    console.log("No vite project found. Creating a new one...");
    execSync(`node -e "fetch('${scriptsURL}/CreateViteProject.js').then((c) => c.text().then(eval))" "${jsFramework}"`, { stdio: 'inherit' });
}

function BuildViteProject() {
    console.log("Building vite project...");
    spawnSync(/^win/.test(process.platform) ? 'npx.cmd' : 'npx', ['vite', 'build', '--emptyOutDir', '--outDir', '../dist'], { stdio: 'inherit' });
}

function DevViteProject() {
    console.log("Running vite project in dev mode");
    spawn(/^win/.test(process.platform) ? 'npx.cmd' : 'npx', ['vite', '.'], { shell: true, detached: true, stdio: "ignore" }).unref();
}

async function PrebuildVite() {
    console.log("Detected Project Type: Vite");
    var sourcePath = path.join(projectDirectory, 'src-vite');

    if (!fs.existsSync(sourcePath)) {
        fs.mkdirSync(sourcePath);
    }

    process.chdir(path.join(process.cwd(), "src-vite"));

    var packageJsonPath = path.join(sourcePath, 'package.json');
    
    if (!fs.existsSync(packageJsonPath)) {
        CreateViteProject();
    }

    if (buildConfiguration.toLowerCase().includes("debug")) {
        console.log("Detected debug mode");
        DevViteProject();
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
        await PrebuildVite();
    }
}

Main();