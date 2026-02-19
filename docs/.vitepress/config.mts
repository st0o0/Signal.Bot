// docs/.vitepress/config.mts
import { defineConfig } from 'vitepress'
import { csharpApiPlugin, generateSidebar } from './plugins/csharp-api/index.js'
import { codeSnippetsPlugin } from './plugins/code-snippets/index.js'
import fs from 'fs'
import path from 'path'

// Funktion um API Sidebar zu laden (mit Fallback)
function loadApiSidebar() {
  const sidebarPath = path.join(process.cwd(), 'api', '_sidebar.json')
  
  if (fs.existsSync(sidebarPath)) {
    try {
      return JSON.parse(fs.readFileSync(sidebarPath, 'utf-8'))
    } catch (error) {
      console.warn('⚠️  Failed to load API sidebar:', error)
    }
  }
  
  // Fallback: Leere Sidebar
  return [
    {
      text: 'API Reference',
      items: [
        { text: 'Documentation is being generated...', link: '/api/' }
      ]
    }
  ]
}

export default defineConfig({
  title: "Signal.Bot",
  description: "A .NET Signal Messenger Bot Client",
  lang: 'en-US',
  lastUpdated: true,
  base: '/Signal.Bot/',
  srcDir: '.',
  srcExclude: ['node_modules', '.vitepress/cache'],
  
  vite: {
    plugins: [
      csharpApiPlugin({
        xmlPath: '../src/Signal.Bot/bin/Release/*/Signal.Bot.xml',
        outputDir: 'api',
        autoSidebar: true,
        watch: true,
        excludeNamespaces: ['System', 'Microsoft', 'Internal']
      })
    ]
  },
  
  themeConfig: {
    logo: '/logo_small.png',
    search: {
      provider: 'local'
    },
    nav: [
      { text: 'Home', link: '/' },
      { text: 'Guide', link: '/guide/getting-started' },
      { text: 'API Reference', link: '/api/' },
      { text: 'Examples', link: '/examples/' }
    ],
    
    sidebar: {
      '/api/': loadApiSidebar(),
      
      '/guide/': [
        {
          text: 'Guide',
          items: [
            { text: 'Getting Started', link: '/guide/getting-started' },
            { text: 'Sending Messages', link: '/guide/sending-messages' },
            { text: 'Receiving Messages', link: '/guide/receiving-messages' },
            { text: 'Groups', link: '/guide/groups' },
            { text: 'Attachments', link: '/guide/attachments' },
            { text: 'Profiles', link: '/guide/profiles' }
          ]
        }
      ],
      '/examples/': [
        {
          text: 'Examples',
          items: [
            { text: 'Overview', link: '/examples/' },
            { text: 'Echo Bot', link: '/examples/echo-bot' },
            { text: 'Command Bot', link: '/examples/command-bot' }
          ]
        }
      ]
    },
    
    socialLinks: [
      { icon: 'github', link: 'https://github.com/st0o0/Signal.Bot' }
    ]
  }
})