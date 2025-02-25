# Getting Started With Vite

## Creating The C# Project

If you already have a C# project created, or prefer Visual Studio over dotnet cli, you can skip this section.
The recommended template for IgniteView projects is the barebones `console` template, which we can build up from easily.

```bash
mdkir myproject
cd myproject
dotnet new console
```

## Installing IgniteView

IgniteView requires a little more setup than other NuGet packages, so just installing it as usual *won't* work.


To install IgniteView and setup the scripts needed, simply insert the following snippet into your `.csproj` file, inside the `<Project>` tag:

```xml title="MyProject.csproj"
<!-- IgniteView -->
<ItemGroup>
    <PackageReference Include="IgniteView.Desktop" Version="*" />
</ItemGroup>
<PropertyGroup>
    <DisableFastUpToDateCheck>true</DisableFastUpToDateCheck>
    <JSFramework>react</JSFramework> <!-- This is required for IgniteView to know what js framework you are using -->
    <ScriptsURL>https://raw.githubusercontent.com/SamsidParty/IgniteView/refs/heads/main/IgniteView.Scripts</ScriptsURL>
    <PreBuildCommand>node -e "fetch('$(ScriptsURL)/Prebuild.js').then((c) =&gt; c.text().then(eval))" "$(ScriptsURL)" "$(MSBuildProjectDirectory.Replace('\', '\\'))" "$(Configuration)" "$(JSFramework)"</PreBuildCommand>
    <PostBuildCommand>node -e "fetch('$(ScriptsURL)/Postbuild.js').then((c) =&gt; c.text().then(eval))" "$(ScriptsURL)" "$(MSBuildProjectDirectory.Replace('\', '\\'))" "$(Configuration)" "$(JSFramework)"</PostBuildCommand>
</PropertyGroup>
<Target Name="PreBuild" BeforeTargets="PreBuildEvent">
    <Exec Command="$(PreBuildCommand) &quot;$(OutputPath.Replace('\', '\\'))&quot;" />
</Target>
<Target Name="PostBuild" AfterTargets="PostBuildEvent">
    <Exec Command="$(PostBuildCommand) &quot;$(OutputPath.Replace('\', '\\'))&quot;" />
</Target>
```

## Snippet Explanation

Let's break down this snippet, line by line.

```xml
<PackageReference Include="IgniteView.Desktop" Version="*" />
```
This will include the latest version of `IgniteView.Desktop` from NuGet in the project.
You may replace `*` with a specific version number.

<br></br>

```xml
<DisableFastUpToDateCheck>true</DisableFastUpToDateCheck>
```
This will make sure modified web files are up-to-date in the build.

<br></br>

```xml
<JSFramework>react</JSFramework>
```
This determines the vite project type of your app, you *MUST* set this value properly.
You can also set it to `raw`, which will disable vite entirely and simply use raw HTML/JS/CSS files.

:::tip

Some of the supported frameworks include:
- `vanilla`
- `vanilla-ts`
- `vue`
- `vue-ts`
- `react`
- `react-ts`
- `preact`
- `preact-ts`
- `svelte`
- `svelte-ts`

For a complete list of supported frameworks, see the [Vite documentation](https://vite.dev/guide/#scaffolding-your-first-vite-project).

:::


<br></br>

```xml
<ScriptsURL>https://raw.githubusercontent.com/SamsidParty/IgniteView/refs/heads/main/IgniteView.Scripts</ScriptsURL>
```
This tag sets the URL where the prebuild and postbuild scripts are downloaded from. These scripts are needed to properly prepare your project, and are crucial for IgniteView to work.

If you are concerned about possible breaking changes (or you are security-conscious), you can replace the URL with a versioned one:

```xml
<!-- Replace COMMIT_HASH_HERE with the hash of the commit you want to use -->
<ScriptsURL>https://raw.githubusercontent.com/SamsidParty/IgniteView/COMMIT_HASH_HERE/IgniteView.Scripts</ScriptsURL>
```

This will ensure the contents of the scripts can't be changed on GitHub.

<br></br>

```xml
<PreBuildEvent>...</PreBuildEvent>
<PostBuildEvent>...</PostBuildEvent>
```
These long commands will fetch the build scripts from the provided `ScriptsURL` and execute them before and after compiling.