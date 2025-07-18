# System Styling

IgniteView makes it easy for your app to replicate the native look and feel of the system it's running on.
Many CSS variables are exposed that allow you to match specific colors and fonts from the system.

## System Font

The system font is exposed as `--system-font`, so you can simply do:
```css
font-family: var(--system-font);
```

## System Accent Color

Windows, Mac, and Linux all allow the user to define an accent color, typically used for UI elements like buttons and switches.
IgniteView exposes `--system-accent` and `--system-accent-foreground` variables for this.

Here's how they should be used:

```css
button {
    background-color: var(--system-accent);
    color: var(--system-accent-foreground);
}
```

## Window Backgrounds and Acrylic

Windows and macOS both support blurred windows, and using them with IgniteView is easy:

```css
body, html {
    background-color: var(--system-body)
}
```

By default, this will use [mica](https://learn.microsoft.com/en-us/windows/apps/design/style/mica) on Windows, [vibrancy](https://developer.apple.com/documentation/UIKit/UIVibrancyEffect) on macOS, and a solid background based on the color scheme on other systems.

## Other Exposed Colors

- `--system-background`: A solid-color background that goes on top of the body
- `--system-background2`: A solid-color background that goes on top of `--system-background`
- `--system-foreground`: The color used for text, icons, and other foreground elements
- `--system-outline`: The color used for outlines between sections