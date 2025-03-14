# Changing The Icon

## Icon Creation Guidelines

There are some things to keep in mind when making an icon for IgniteView apps:
- The icon must be in `.png` format, IgniteView won't work with `.ico` or `.icns` files
- Use a square aspect ratio, we recommend 256x256 but any 1:1 image will work
- Try to minimize the padding on the icon, make it fill the entire area of the image

## Icon Creation Guidelines (macOS)

IgniteView allows you to have a seperate `.png` file specifically for macOS, they have different requirements:
- The icon still must be in `.png` format, don't try to use an `.icns` file
- Use a square aspect ratio, but this time we recommend 512x512 to look sharp on a retina display
- The image should have a 50px of padding (for a 512x512 image), otherwise it will look too big
- The inner portion of the image should be a "squircle" (Apple's version of rounded rect)
- The squircle should have a drop shadow to be consistent with other apps in the dock

:::tip
We recommend simply downloading the [IgniteView macOS icon template](./img/icontemplate_mac.png) and overlaying your icon in the center.

:::

## Add The Icon To Your Project

The recommended way to set the icon of your window is to add a `favicon.png` file to the root of your web content.

For vite projects:
    - Add the `favicon.png` file to `src-vite/public/favicon.png`

For raw HTML projects:
    - Add the `favicon.png` file to `wwwroot/favicon.png`

:::info

If you have a separate icon for macOS, you may add `favicon_mac.png` alongside `favicon.png` and it will automatically be used based on the current OS.

:::