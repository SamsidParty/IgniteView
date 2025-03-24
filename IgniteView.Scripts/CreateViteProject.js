const spawnSync = require('child_process').spawnSync;
const fs = require('fs');
const path = require('path');

const jsFramework = process.argv[1];

function CreateViteProject() {
    console.clear();
    spawnSync(/^win/.test(process.platform) ? 'npm.cmd' : 'npm', ['create', 'vite@latest', "--yes", "./", "--", "--template", jsFramework], { stdio: 'inherit', shell: true });
}

function InstallDependencies() {
    console.clear();
    spawnSync(/^win/.test(process.platform) ? 'npm.cmd' : 'npm', ['i'], { stdio: 'inherit', shell: true });
}

async function Main() {
    console.log("\n-------- IgniteView Vite Wizard --------\n");
    console.log("Working Directory: " + process.cwd() + "\n");

    while (!fs.existsSync('./package.json')) {
        CreateViteProject();
    }

    console.log("Created Vite Project, installing dependencies...");

    InstallDependencies();

    console.log("Installed npm dependencies, continuing with IgniteView prebuild...");

    process.exit(0);
}

Main();
