import { defineConfig } from 'vite'

export default defineConfig({
    base: "/igniteview",
    build: {
        rollupOptions: {
            input: {
                app: './core.html'
            },
            output: {
                assetFileNames: 'name][extname]',
                chunkFileNames: '[name].js',
                entryFileNames: 'injected.js',
            },
        },
    },
});