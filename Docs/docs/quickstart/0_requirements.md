# Requirements

IgniteView needs a few dependencies to be installed when developing your app:
- .NET 9.0 or later
- Node.JS version 22.3.0 or later, along with NPM

## Windows Requirements

- WebView2 evergreen runtime

## Linux Requirements

- gtk4
- webkitgtk-6.0

On newer Ubuntu systems, you may get an error related to "bwrap". In case this happens, you will have to run the following commands:

```bash
sudo sysctl -w kernel.apparmor_restrict_unprivileged_unconfined=0
sudo sysctl -w kernel.apparmor_restrict_unprivileged_userns=0
```