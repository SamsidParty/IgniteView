# Changing The Icon

## Icon Creation Guidelines

There are some things to keep in mind when making an icon for IgniteView apps:
- The icon must be in `.png` format, IgniteView won't work with `.ico` or `.icns` files
- Use a square aspect ratio, we recommend 256x256 but any 1:1 image will work
- Try to minimize the padding on the icon, make it fill the entire area of the image (except for macOS icons)

## Add The Icon To Your Project

The recommended way to set the icon of your window is to add a `favicon.png` file to the root of your web content.

For vite projects:
    - Add the `favicon.png` file to src-vite/public/favicon.png

For raw HTML projects:
    - Add the `favicon.png` file to wwwroot/favicon.png