const readline = require('readline');
const spawnSync = require('child_process').spawnSync;
const fs = require('fs');
const path = require('path');

const interface = readline.createInterface({
    input: process.stdin,
    output: process.stdout,
});

function CreateViteProject() {
    console.clear();
    spawnSync(/^win/.test(process.platform) ? 'npm.cmd' : 'npm', ['create', 'vite@latest', "./"], { stdio: 'inherit' });
}

function InstallDependencies() {
    console.clear();
    spawnSync(/^win/.test(process.platform) ? 'npm.cmd' : 'npm', ['i'], { stdio: 'inherit' });
}

async function Main() {
    console.log("\n-------- IgniteView \x1b[96mVite\x1b[0m Wizard --------\n");
    console.log("Working Directory: " + process.cwd() + "\n");

    await new Promise((r) => interface.question("Press \x1b[92m\x1b[1menter\x1b[0m to initialize \x1b[96mVite\x1b[0m with this IgniteView project: ", r));

    while (!fs.existsSync('./package.json')) {
        CreateViteProject();
    }

    console.log("Created \x1b[96mVite\x1b[0m Project, installing dependencies...");

    InstallDependencies();

    console.log("Installed npm dependencies, continuing with IgniteView prebuild...");

    process.exit(0);
}

Main();