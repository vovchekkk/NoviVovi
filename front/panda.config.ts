import { defineConfig } from '@pandacss/dev'

export default defineConfig({
  preflight: true,
  include: ['./src/**/*.{js,jsx,ts,tsx}'],
  jsxFramework: 'react',
  outdir: 'styled-system',

  theme: {
    extend: {
      tokens: {
        colors: {
          background: { value: '#1A0C19' },
          card:       { value: '#FADCD4' },     // светло-розовый блок под видео
          primary:    { value: '#ff4d94' },     // ярко-розовый акцент
          white:      { value: '#ffffff' },
          text:       { value: '#ffffff' },
          border:     { value: '#333333' },
          'text-secondary': { value: '#aaaaaa' },
          'border-light': { value: '#222222' },
        },
        shadows: {
          glow: {
            value: '0 0 25px rgba(250, 220, 212, 0.5), 0 0 45px rgba(250, 220, 212, 0.3)'
          },
          'glow-hover': {
            value: '0 0 40px rgba(250, 220, 212, 0.5)'
          },
          card: { value: '0 10px 30px rgba(0, 0, 0, 0.6)' },
        },
        radii: {
          button: { value: '9999px' },
          card:   { value: '16px' },
        },
        sizes: {
          'video-max': { value: '1180px' },
          'content-max': { value: '1000px' },
        },
        spacing: {
          section: { value: '60px' },
        }
      }
    }
  }
})