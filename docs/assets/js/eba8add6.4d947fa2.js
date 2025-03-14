"use strict";(self.webpackChunkigniteview=self.webpackChunkigniteview||[]).push([[220],{7234:(e,n,i)=>{i.r(n),i.d(n,{assets:()=>c,contentTitle:()=>o,default:()=>h,frontMatter:()=>a,metadata:()=>r,toc:()=>l});const r=JSON.parse('{"id":"quickstart/manual_vite","title":"Manual Setup With Vite","description":"Creating The C# Project","source":"@site/docs/quickstart/manual_vite.md","sourceDirName":"quickstart","slug":"/quickstart/manual_vite","permalink":"/IgniteView/docs/quickstart/manual_vite","draft":false,"unlisted":false,"tags":[],"version":"current","frontMatter":{},"sidebar":"tutorialSidebar","previous":{"title":"Requirements","permalink":"/IgniteView/docs/quickstart/requirements"},"next":{"title":"Window Customization","permalink":"/IgniteView/docs/category/window-customization"}}');var t=i(4848),s=i(8453);const a={},o="Manual Setup With Vite",c={},l=[{value:"Creating The C# Project",id:"creating-the-c-project",level:2},{value:"Installing IgniteView",id:"installing-igniteview",level:2},{value:"Snippet Explanation",id:"snippet-explanation",level:2},{value:"Add Some Code",id:"add-some-code",level:2},{value:"Code Explanation",id:"code-explanation",level:2},{value:"Initial Run",id:"initial-run",level:2}];function d(e){const n={a:"a",admonition:"admonition",code:"code",em:"em",h1:"h1",h2:"h2",header:"header",li:"li",p:"p",pre:"pre",ul:"ul",...(0,s.R)(),...e.components};return(0,t.jsxs)(t.Fragment,{children:[(0,t.jsx)(n.header,{children:(0,t.jsx)(n.h1,{id:"manual-setup-with-vite",children:"Manual Setup With Vite"})}),"\n",(0,t.jsx)(n.h2,{id:"creating-the-c-project",children:"Creating The C# Project"}),"\n",(0,t.jsxs)(n.p,{children:["If you already have a C# project created, or prefer Visual Studio over dotnet cli, you can skip this section.\r\nTo manually create an IgniteView project, you can use the ",(0,t.jsx)(n.code,{children:"console"})," template, which we can build up from easily."]}),"\n",(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-bash",children:"mdkir myproject\r\ncd myproject\r\ndotnet new console\n"})}),"\n",(0,t.jsx)(n.h2,{id:"installing-igniteview",children:"Installing IgniteView"}),"\n",(0,t.jsxs)(n.p,{children:["IgniteView requires a little more setup than other NuGet packages, so just installing it as usual ",(0,t.jsx)(n.em,{children:"won't"})," work."]}),"\n",(0,t.jsxs)(n.p,{children:["To install IgniteView and setup the scripts needed, simply insert the following snippet into your ",(0,t.jsx)(n.code,{children:".csproj"})," file, inside the ",(0,t.jsx)(n.code,{children:"<Project>"})," tag:"]}),"\n",(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-xml",metastring:'title="MyProject.csproj"',children:'\x3c!-- IgniteView --\x3e\r\n<ItemGroup>\r\n    <PackageReference Include="IgniteView.Desktop" Version="*" />\r\n</ItemGroup>\r\n<PropertyGroup>\r\n    <DisableFastUpToDateCheck>true</DisableFastUpToDateCheck>\r\n    <JSFramework>react</JSFramework> \x3c!-- This is required for IgniteView to know what js framework you are using --\x3e\r\n    <ScriptsURL>https://raw.githubusercontent.com/SamsidParty/IgniteView/refs/heads/main/IgniteView.Scripts</ScriptsURL>\r\n    <PreBuildCommand>node -e "fetch(\'$(ScriptsURL)/Prebuild.js\').then((c) =&gt; c.text().then(eval))" "$(ScriptsURL)" "$(MSBuildProjectDirectory.Replace(\'\\\', \'\\\\\'))" "$(Configuration)" "$(JSFramework)"</PreBuildCommand>\r\n    <PostBuildCommand>node -e "fetch(\'$(ScriptsURL)/Postbuild.js\').then((c) =&gt; c.text().then(eval))" "$(ScriptsURL)" "$(MSBuildProjectDirectory.Replace(\'\\\', \'\\\\\'))" "$(Configuration)" "$(JSFramework)"</PostBuildCommand>\r\n</PropertyGroup>\r\n<Target Name="PreBuild" BeforeTargets="PreBuildEvent">\r\n    <Exec Command="$(PreBuildCommand) &quot;$(OutputPath.Replace(\'\\\', \'\\\\\'))&quot;" />\r\n</Target>\r\n<Target Name="PostBuild" AfterTargets="PostBuildEvent">\r\n    <Exec Command="$(PostBuildCommand) &quot;$(OutputPath.Replace(\'\\\', \'\\\\\'))&quot;" />\r\n</Target>\n'})}),"\n",(0,t.jsx)(n.h2,{id:"snippet-explanation",children:"Snippet Explanation"}),"\n",(0,t.jsx)(n.p,{children:"Let's break down this snippet, line by line."}),"\n",(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-xml",children:'<PackageReference Include="IgniteView.Desktop" Version="*" />\n'})}),"\n",(0,t.jsxs)(n.p,{children:["This will include the latest version of ",(0,t.jsx)(n.code,{children:"IgniteView.Desktop"})," from NuGet in the project.\r\nYou may replace ",(0,t.jsx)(n.code,{children:"*"})," with a specific version number."]}),"\n",(0,t.jsx)("br",{}),"\n",(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-xml",children:"<DisableFastUpToDateCheck>true</DisableFastUpToDateCheck>\n"})}),"\n",(0,t.jsx)(n.p,{children:"This will make sure modified web files are up-to-date in the build."}),"\n",(0,t.jsx)("br",{}),"\n",(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-xml",children:"<JSFramework>react</JSFramework>\n"})}),"\n",(0,t.jsxs)(n.p,{children:["This determines the vite project type of your app, you ",(0,t.jsx)(n.em,{children:"MUST"})," set this value properly.\r\nYou can also set it to ",(0,t.jsx)(n.code,{children:"raw"}),", which will disable vite entirely and simply use raw HTML/JS/CSS files."]}),"\n",(0,t.jsxs)(n.admonition,{type:"tip",children:[(0,t.jsx)(n.p,{children:"Some of the supported frameworks include:"}),(0,t.jsxs)(n.ul,{children:["\n",(0,t.jsx)(n.li,{children:(0,t.jsx)(n.code,{children:"vanilla"})}),"\n",(0,t.jsx)(n.li,{children:(0,t.jsx)(n.code,{children:"vanilla-ts"})}),"\n",(0,t.jsx)(n.li,{children:(0,t.jsx)(n.code,{children:"vue"})}),"\n",(0,t.jsx)(n.li,{children:(0,t.jsx)(n.code,{children:"vue-ts"})}),"\n",(0,t.jsx)(n.li,{children:(0,t.jsx)(n.code,{children:"react"})}),"\n",(0,t.jsx)(n.li,{children:(0,t.jsx)(n.code,{children:"react-ts"})}),"\n",(0,t.jsx)(n.li,{children:(0,t.jsx)(n.code,{children:"preact"})}),"\n",(0,t.jsx)(n.li,{children:(0,t.jsx)(n.code,{children:"preact-ts"})}),"\n",(0,t.jsx)(n.li,{children:(0,t.jsx)(n.code,{children:"svelte"})}),"\n",(0,t.jsx)(n.li,{children:(0,t.jsx)(n.code,{children:"svelte-ts"})}),"\n"]}),(0,t.jsxs)(n.p,{children:["For a complete list of supported frameworks, see the ",(0,t.jsx)(n.a,{href:"https://vite.dev/guide/#scaffolding-your-first-vite-project",children:"Vite documentation"}),"."]})]}),"\n",(0,t.jsx)("br",{}),"\n",(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-xml",children:"<ScriptsURL>https://raw.githubusercontent.com/SamsidParty/IgniteView/refs/heads/main/IgniteView.Scripts</ScriptsURL>\n"})}),"\n",(0,t.jsx)(n.p,{children:"This tag sets the URL where the prebuild and postbuild scripts are downloaded from. These scripts are needed to properly prepare your project, and are crucial for IgniteView to work."}),"\n",(0,t.jsx)(n.p,{children:"If you are concerned about possible breaking changes (or you are security-conscious), you can replace the URL with a versioned one:"}),"\n",(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-xml",children:"\x3c!-- Replace COMMIT_HASH_HERE with the hash of the commit you want to use --\x3e\r\n<ScriptsURL>https://raw.githubusercontent.com/SamsidParty/IgniteView/COMMIT_HASH_HERE/IgniteView.Scripts</ScriptsURL>\n"})}),"\n",(0,t.jsx)(n.p,{children:"This will ensure the contents of the scripts can't be changed on GitHub."}),"\n",(0,t.jsx)("br",{}),"\n",(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-xml",children:'<PreBuildCommand>...</PreBuildCommand>\r\n<PostBuildCommand>...</PostBuildCommand>\r\n<Target Name="PreBuild" ... />\r\n<Target Name="PostBuild" ... />\n'})}),"\n",(0,t.jsxs)(n.p,{children:["These long commands will fetch the build scripts from the provided ",(0,t.jsx)(n.code,{children:"ScriptsURL"})," and execute them before and after compiling."]}),"\n",(0,t.jsx)(n.h2,{id:"add-some-code",children:"Add Some Code"}),"\n",(0,t.jsx)(n.p,{children:"The basic code to get your app working with IgniteView is quite simple:"}),"\n",(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-csharp",metastring:'title="Program.cs"',children:'using IgniteView.Core;\r\nusing IgniteView.Desktop;\r\n\r\npublic class Program\r\n{\r\n    [STAThread]\r\n    static void Main(string[] args)\r\n    {\r\n        DesktopPlatformManager.Activate();\r\n        var app = new ViteAppManager();\r\n\r\n        var mainWindow =\r\n            WebWindow.Create()\r\n            .WithTitle("Hello, world")\r\n            .Show();\r\n\r\n        app.Run();\r\n    }\r\n}\n'})}),"\n",(0,t.jsx)(n.h2,{id:"code-explanation",children:"Code Explanation"}),"\n",(0,t.jsx)(n.p,{children:"There are a few things to note here:"}),"\n",(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-csharp",children:"[STAThread]\n"})}),"\n",(0,t.jsxs)(n.p,{children:["This is required for the app to function on windows, because it doesn't play nicely with MTAThreads.\r\nFor more info, see ",(0,t.jsx)(n.a,{href:"https://learn.microsoft.com/en-us/windows/win32/com/processes--threads--and-apartments",children:"Microsoft Documentation"})]}),"\n",(0,t.jsx)("br",{}),"\n",(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-csharp",children:"DesktopPlatformManager.Activate();\n"})}),"\n",(0,t.jsx)(n.p,{children:"This call registers the desktop version of IgniteView with the core, which tells it what webview implementation to use."}),"\n",(0,t.jsx)("br",{}),"\n",(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-csharp",children:"var app = new ViteAppManager();\r\n\r\n...\r\n\r\napp.Run();\n"})}),"\n",(0,t.jsxs)(n.p,{children:["This code creates the ",(0,t.jsx)(n.code,{children:"AppManager"})," using Vite and starts the main loop. The ",(0,t.jsx)(n.code,{children:"AppManager"})," manages the lifecycle of the app."]}),"\n",(0,t.jsx)("br",{}),"\n",(0,t.jsx)(n.pre,{children:(0,t.jsx)(n.code,{className:"language-csharp",children:'var mainWindow =\r\n    WebWindow.Create()\r\n    .WithTitle("Hello, world")\r\n    .Show();\n'})}),"\n",(0,t.jsx)(n.p,{children:"This code is responsible for creating and spawning the main window of the app."}),"\n",(0,t.jsx)(n.h2,{id:"initial-run",children:"Initial Run"}),"\n",(0,t.jsxs)(n.p,{children:["The first time you press run in Visual Studio (or do dotnet run), IgniteView will automatically create an Vite project for you using NPM.\r\nThe JavaScript framework used will depend on the value of ",(0,t.jsx)(n.code,{children:"JSFramework"})," that was set in the ",(0,t.jsx)(n.code,{children:".csproj"})," file."]}),"\n",(0,t.jsxs)(n.p,{children:["Once everything is set up, you can navigate to ",(0,t.jsx)(n.code,{children:"src-vite"})," and start developing your app!"]})]})}function h(e={}){const{wrapper:n}={...(0,s.R)(),...e.components};return n?(0,t.jsx)(n,{...e,children:(0,t.jsx)(d,{...e})}):d(e)}},8453:(e,n,i)=>{i.d(n,{R:()=>a,x:()=>o});var r=i(6540);const t={},s=r.createContext(t);function a(e){const n=r.useContext(s);return r.useMemo((function(){return"function"==typeof e?e(n):{...n,...e}}),[n,e])}function o(e){let n;return n=e.disableParentContext?"function"==typeof e.components?e.components(t):e.components||t:a(e.components),r.createElement(s.Provider,{value:n},e.children)}}}]);