import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'

const proxyTarget = process.env.ASPNETCORE_HTTP_PORT
  ? `http://localhost:${process.env.ASPNETCORE_HTTP_PORT}`
  : process.env.ASPNETCORE_URLS
    ? process.env.ASPNETCORE_URLS.split(';')[0]
    : 'http://localhost:5112'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue(), vueDevTools()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
  server: {
    port: 51234,
    strictPort: true,

    proxy: {
      '^/api': {
        target: proxyTarget,
        secure: false,
      },
      '^/scalar': {
        target: proxyTarget,
        secure: false,
      },
    },
  },
})
