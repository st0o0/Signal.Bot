import { parseXmlDocs } from './parseXmlDocs.js'
import { generateMarkdown } from './generateMarkdown.js'
import { generateSidebar } from './sidebar-generator.js'
import fs from 'fs'
import path from 'path'
import { fileURLToPath } from 'url'
import { glob } from 'glob'

const __filename = fileURLToPath(import.meta.url)
const __dirname = path.dirname(__filename)

export function csharpApiPlugin(options = {}) {
  const {
    xmlPath,
    outputDir = './api-generated',
    autoSidebar = true,
    watch = true,
    excludeNamespaces = ['System'] // Default: exclude System namespaces
  } = options

  let config
  let parsedDocs
  let generatedFiles = []
  let resolvedXmlPath = null

  // Helper function to filter out excluded namespaces
  function filterNamespaces(docs) {
    if (!excludeNamespaces || excludeNamespaces.length === 0) {
      return docs
    }

    const filteredTypes = docs.types.filter(type => {
      return !excludeNamespaces.some(excluded => 
        type.fullName?.startsWith(excluded + '.')
      )
    })

    const filteredMethods = docs.methods.filter(method => {
      return !excludeNamespaces.some(excluded => 
        method.className?.startsWith(excluded + '.')
      )
    })

    const filteredProperties = docs.properties.filter(prop => {
      return !excludeNamespaces.some(excluded => 
        prop.className?.startsWith(excluded + '.')
      )
    })

    const filteredFields = docs.fields.filter(field => {
      return !excludeNamespaces.some(excluded => 
        field.className?.startsWith(excluded + '.')
      )
    })

    const filteredMembers = docs.members.filter(member => {
      const name = member.name
      if (name.startsWith('T:')) {
        const fullName = name.substring(2)
        return !excludeNamespaces.some(excluded => fullName.startsWith(excluded + '.'))
      }
      if (name.startsWith('M:') || name.startsWith('P:') || name.startsWith('F:')) {
        const withoutPrefix = name.substring(2)
        const className = withoutPrefix.substring(0, withoutPrefix.lastIndexOf('.'))
        return !excludeNamespaces.some(excluded => className.startsWith(excluded + '.'))
      }
      return true
    })

    return {
      ...docs,
      types: filteredTypes,
      methods: filteredMethods,
      properties: filteredProperties,
      fields: filteredFields,
      members: filteredMembers
    }
  }

  return {
    name: 'vitepress-csharp-api',
    
    enforce: /** @type {'pre'} */ ('pre'),
    configResolved(resolvedConfig) {
      config = resolvedConfig
    },

    async buildStart() {
      console.log('🔧 C# API Plugin: Starting...')
      
      if (!xmlPath) {
        console.warn('⚠️  No XML path provided, skipping API generation')
        return
      }

      // Resolve glob pattern
      const searchPattern = path.resolve(process.cwd(), xmlPath)
      const matches = await glob(searchPattern, { windowsPathsNoEscape: true })
      
      if (matches.length === 0) {
        console.warn(`⚠️  No XML files found matching: ${searchPattern}`)
        console.warn('   Run "dotnet build" first to generate XML documentation')
        console.warn('   Searching for: ' + xmlPath)
        return
      }

      // Use the first match (or most recent if multiple)
      if (matches.length > 1) {
        console.log(`ℹ️  Found ${matches.length} XML files, using: ${matches[0]}`)
      }
      
      resolvedXmlPath = matches[0]
      
      if (!fs.existsSync(resolvedXmlPath)) {
        console.warn(`⚠️  XML file not found: ${resolvedXmlPath}`)
        return
      }

      try {
        console.log(`📖 Parsing XML documentation from: ${resolvedXmlPath}`)
        const rawDocs = await parseXmlDocs(resolvedXmlPath)
        // Filter out excluded namespaces
        parsedDocs = filterNamespaces(rawDocs)

        if (excludeNamespaces.length > 0) {
          console.log(`🔍 Filtered out namespaces: ${excludeNamespaces.join(', ')}`)
          console.log(`   Removed ${rawDocs.types.length - parsedDocs.types.length} types`)
          console.log(`   Removed ${rawDocs.methods.length - parsedDocs.methods.length} methods`)
          console.log(`   Removed ${rawDocs.properties.length - parsedDocs.properties.length} properties`)
          console.log(`   Removed ${rawDocs.fields.length - parsedDocs.fields.length} fields`)
          console.log(`   Removed ${rawDocs.members.length - parsedDocs.members.length} total members`)
        }
        
        console.log(`✨ Found ${parsedDocs.types.length} types, ${parsedDocs.members.length} members`)
        
        // Clear output directory
        const outputPath = path.resolve(process.cwd(), outputDir)
        if (fs.existsSync(outputPath)) {
          fs.rmSync(outputPath, { recursive: true })
        }
        fs.mkdirSync(outputPath, { recursive: true })
        
        // Generate markdown files
        console.log('📝 Generating markdown files...')
        generatedFiles = await generateMarkdown(parsedDocs, outputPath)
        
        console.log(`✅ Generated ${generatedFiles.length} API documentation files`)
        
        // Generate sidebar config if requested
        if (autoSidebar) {
          const sidebarConfig = generateSidebar(parsedDocs, outputDir)
          const configPath = path.join(outputPath, '_sidebar.json')
          fs.writeFileSync(configPath, JSON.stringify(sidebarConfig, null, 2))
          console.log('📋 Generated sidebar configuration')
        }
        
      } catch (error) {
        console.error('❌ Error generating API docs:', error)
        console.error('Stack trace:', error.stack)
        throw error
      }
    },

    configureServer(server) {
      if (!watch || !xmlPath) return

      // Initial resolve of XML path
      const searchPattern = path.resolve(process.cwd(), xmlPath)
      
      // Watch for XML file changes
      glob(searchPattern, { windowsPathsNoEscape: true }).then(matches => {
        if (matches.length === 0) return
        
        resolvedXmlPath = matches[0]
        server.watcher.add(resolvedXmlPath)
        
        // Also watch the directory for new builds
        const xmlDir = path.dirname(resolvedXmlPath)
        server.watcher.add(xmlDir)
      })
      
      server.watcher.on('change', async (file) => {
        // Re-resolve the path in case it changed
        const matches = await glob(searchPattern, { windowsPathsNoEscape: true })
        if (matches.length === 0) return
        
        const currentXmlPath = matches[0]
        
        if (file === currentXmlPath || file.endsWith('.xml')) {
          console.log('📖 XML documentation changed, regenerating...')
          
          try {
            const rawDocs = await parseXmlDocs(currentXmlPath)
            parsedDocs = filterNamespaces(rawDocs)
            
            const outputPath = path.resolve(process.cwd(), outputDir)
            generatedFiles = await generateMarkdown(parsedDocs, outputPath)
            
            console.log('✅ API documentation regenerated')
            
            // Trigger HMR for all generated files
            generatedFiles.forEach(file => {
              server.moduleGraph.onFileChange(file)
            })
            
            server.ws.send({
              type: 'full-reload',
              path: '*'
            })
          } catch (error) {
            console.error('❌ Error regenerating docs:', error)
            console.error('Stack trace:', error.stack)
          }
        }
      })
    }
  }
}

export { generateSidebar } from './sidebar-generator.js'