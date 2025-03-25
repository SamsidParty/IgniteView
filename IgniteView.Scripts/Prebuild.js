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

function BuildViteProject(outDir) {
    console.log("Building vite project...");
    spawnSync(/^win/.test(process.platform) ? 'npx.cmd' : 'npx', ['--yes', 'vite', 'build', '--emptyOutDir', '--outDir', `"${outDir}"`], { stdio: 'inherit', shell: true });
}

function NPMInstall() {
    console.log("Installing NPM dependencies...");
    spawnSync(/^win/.test(process.platform) ? 'npm.cmd' : 'npm', ['install'], { stdio: 'inherit',  shell: true });
}


async function PrebuildVite() {
    console.log("Detected Project Type: Vite");
    var sourcePath = path.join(projectDirectory, 'src-vite');
    var outDir = path.join(projectDirectory, 'dist');

    if (!fs.existsSync(sourcePath)) {
        fs.mkdirSync(sourcePath);
    }

    process.chdir(fs.realpathSync(sourcePath)); // Follows symlinks
    sourcePath = process.cwd();

    var packageJsonPath = path.join(sourcePath, 'package.json');
    var nodeModulesPath = path.join(sourcePath, 'node_modules');
    
    if (!fs.existsSync(packageJsonPath)) {
        CreateViteProject();
    }

    if (!fs.existsSync(nodeModulesPath)) {
        NPMInstall();
    }

    if (buildConfiguration.toLowerCase().includes("debug")) {
        console.log("Detected debug mode");
        BuildViteProject(outDir);

        // Write the .vitedev file to the dist folder
        // The C# code will read this file and launch the vite dev server
        fs.writeFileSync(path.join(outDir, '.vitedev'), sourcePath);
    }
    else {
        console.log("Detected release mode");
        BuildViteProject(outDir);

        // Remove the .vitedev file if it exists, we don't want it in production
        if (fs.existsSync(path.join(outDir, '.vitedev'))) {
            fs.unlinkSync(path.join(outDir, '.vitedev'));
        }
    }

    // Copy and rename the package.json file into the dist folder
    fs.copyFileSync(path.join(sourcePath, 'package.json'), path.join(outDir, 'igniteview_package.json'))
}

async function Main() {

    console.log("\n-------- IgniteView Prebuild Version 2.1.0 --------\n");

    // Determine the project type
    if (jsFramework == "raw" || jsFramework == "") {

        if (!fs.existsSync(path.join(process.cwd(), "wwwroot"))) {
            console.error("Couldn't find the wwwroot directory! Make sure it exists.");
            process.exit(1);
        }

        fs.cpSync(path.join(process.cwd(), "wwwroot"), path.join(process.cwd(), "dist"));
        process.chdir(path.join(process.cwd(), "dist"));
        console.log("Detected project type: Static HTML");
    }
    else {
        await PrebuildVite();
    }

    // Generate the tar file (with .igniteview extension)
    if (!fs.existsSync(path.join(projectDirectory, "iv2runtime"))) {
        fs.mkdirSync(path.join(projectDirectory, "iv2runtime"));
    }

    console.log("Creating main.igniteview file")
    process.chdir(path.join(projectDirectory, "dist"));
    var fileName = path.dirname(projectDirectory) + ".igniteview";
    spawnSync('tar', ['-cf', `"${path.join(projectDirectory, "iv2runtime", fileName)}"`, `"."`], { stdio: 'inherit', shell: true });
}

Main();
