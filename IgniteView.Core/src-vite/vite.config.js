import { defineConfig } from 'vite'

export default defineConfig({
    build: {
        rollupOptions: {
            input: {
                app: './core.html'
            },
            output: {
                assetFileNames: 'igniteview/[name][extname]',
                chunkFileNames: 'igniteview/[name].js',
                entryFileNames: 'igniteview/injected.js',
            },
        },
    },
});