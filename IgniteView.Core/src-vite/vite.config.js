import { defineConfig } from 'vite'
import { resolve } from 'node:path'

export default defineConfig({
    base: "/igniteview",
    build: {
        lib: {
            entry: resolve("src", "injected.js"),
            name: "IgniteView.Core",
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