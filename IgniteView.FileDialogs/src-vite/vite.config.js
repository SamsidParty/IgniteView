import { resolve } from 'node:path';
import { defineConfig } from 'vite';

export default defineConfig({
    base: "/igniteview_filedialogs",
    build: {
        lib: {
            entry: resolve("src", "main.js"),
            name: "IgniteView.FileDialogs",
            formats: ["iife"],
            fileName: (f, e) => e + ".js"
        },
        rollupOptions: {
            output: {
                assetFileNames: '[name][extname]',
                chunkFileNames: '[name].js',
            },
        },
    }
});