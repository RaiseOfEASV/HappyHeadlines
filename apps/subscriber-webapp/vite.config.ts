import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/api/articles': {
        target: 'http://localhost:5200',
        rewrite: (path) => path.replace(/^\/api\/articles/, '/api/articles')
      },
      '/api/comment': {
        target: 'http://localhost:5542',
        rewrite: (path) => path.replace(/^\/api\/comment/, '/api/comment')
      },
      '/api/subscriber': {
        target: 'http://localhost:5500',
        rewrite: (path) => path.replace(/^\/api\/subscriber/, '/api/subscriber')
      }
    },
  },
})
