const fs = require('fs');
const path = require('path');

const scriptsURL = process.argv[1]; // $(ScriptsURL)
const projectDirectory = process.argv[2]; // $(MSBuildProjectDirectory)
const buildConfiguration = process.argv[3]; // $(Configuration)
const jsFramework = process.argv[4]; // $(JSFramework)
const outputDirectory = path.join(projectDirectory, process.argv[5]); // $(OutputPath)
process.chdir(projectDirectory); // cd into the project directory

// Copies a folder to the output directory if it exists
function CopyIfExists(folderName) {
    var folderPath = path.join(projectDirectory, folderName);
    var targetPath = path.join(outputDirectory, folderName);

    // Remove old one
    if (fs.existsSync(targetPath)) {
        console.log("Removing old output folder: " + folderName);
        fs.rmSync(targetPath, { recursive: true });
    }

    // Copy new one
    if (fs.existsSync(folderPath)) {
        console.log("Copying new output folder: " + folderName);
        fs.cpSync(folderPath, targetPath, { recursive: true });
    }
}

async function Main() {

    console.log("\n-------- IgniteView Postbuild Version 2.0.4 --------\n");

    CopyIfExists("dist");
    CopyIfExists("wwwroot");
    CopyIfExists("WWW");
}

Main();