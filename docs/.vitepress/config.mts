import { defineConfig } from 'vitepress'
import { csharpApiPlugin } from './plugins/csharp-api/index.js'
import fs from 'fs'
import path from 'path'

let apiSidebar = []
const sidebarPath = path.join(process.cwd(), 'api', '_sidebar.json')
if (fs.existsSync(sidebarPath)) {
  apiSidebar = JSON.parse(fs.readFileSync(sidebarPath, 'utf-8'))
}

export default defineConfig({
  title: "Signal.Bot",
  description: "A .NET Signal Messenger Bot Client",
  lang: 'en-US',
  lastUpdated: true,
  
  vite: {
    plugins: [
      csharpApiPlugin({
        xmlPath: '../src/Signal.Bot/bin/Release/*/Signal.Bot.xml',
        outputDir: 'api',
        autoSidebar: true,
        watch: true
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
	    '/api/': apiSidebar,
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