import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/api/draft': {
        target: 'http://localhost:8080',
        rewrite: (path) => path.replace(/^\/api\/draft/, '/Draft')
      },
      '/api/publisher': {
        target: 'http://localhost:8081',
        rewrite: (path) => path.replace(/^\/api\/publisher/, '/api/publisher')
      }
    },
  },
})
