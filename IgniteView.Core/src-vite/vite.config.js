import { resolve } from 'node:path';
import { defineConfig } from 'vite';
import dts from 'vite-plugin-dts';

export default defineConfig({
    base: "/igniteview",
    plugins: [
        dts({
            include: ['src/api.ts', 'src/polyfills.ts', 'src/reactHelpers.ts', 'src/sharedContext.ts'],
            insertTypesEntry: false,
            beforeWriteFile(filePath, content) {
                if (!filePath.endsWith('api.d.ts')) {
                    return;
                }

                return {
                    filePath: filePath.replace(/api\.d\.ts$/, 'injected.d.ts'),
                    content
                };
            }
        })
    ],
    build: {
        lib: {
            entry: resolve("src", "injected.ts"),
            name: "IgniteView.Core",
            formats: ["iife"],
            fileName: () => "injected.js"
        },
        rollupOptions: {
            output: {
                assetFileNames: '[name][extname]',
                chunkFileNames: '[name].js',
            },
        },
    }
});