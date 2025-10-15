/// <reference types="vitest" />
import { defineConfig } from 'vitest/config';
import { resolve } from 'path';

export default defineConfig({
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: ['src/test-setup.ts'],
    include: ['src/**/*.{test,spec}.{js,mjs,cjs,ts,mts,cts,jsx,tsx}'],
    exclude: ['node_modules', 'dist'],
    css: true,
    server: {
      deps: {
        inline: ['zone.js'],
      },
    },
  },
  resolve: {
    alias: {
      '@': resolve(__dirname, './src'),
    },
    conditions: ['development', 'default'],
  },
  define: {
    'import.meta.vitest': undefined,
  },
  assetsInclude: ['**/*.html', '**/*.scss'],
  plugins: [
    {
      name: 'angular-template-loader',
      transform(code, id) {
        // Mock HTML files to return empty string
        if (id.endsWith('.html')) {
          return `export default '';`;
        }
        // Mock SCSS files to return empty string
        if (id.endsWith('.scss')) {
          return `export default '';`;
        }
      },
    },
  ],
});
