export function generateSidebar(parsedDocs, outputDir) {
  const { types } = parsedDocs
  
  // Group types by namespace
  const typesByNamespace = {}
  for (const type of types) {
    const namespace = type.fullName.substring(0, type.fullName.lastIndexOf('.'))
    if (!typesByNamespace[namespace]) {
      typesByNamespace[namespace] = []
    }
    typesByNamespace[namespace].push(type)
  }
  
  // Generate sidebar structure
  const sidebar = []
  
  // Add overview
  sidebar.push({
    text: 'API Overview',
    link: `/${outputDir}/index`
  })
  
  // Add namespaces
  for (const [namespace, nsTypes] of Object.entries(typesByNamespace)) {
    const namespaceItem = {
      text: namespace,
      collapsed: true,
      items: []
    }
    
    // Add overview for this namespace
    namespaceItem.items.push({
      text: 'Overview',
      link: `/${outputDir}/${sanitizeFileName(namespace)}.namespace`
    })
    
    // Add types
    for (const type of nsTypes) {
      namespaceItem.items.push({
        text: type.shortName,
        link: `/${outputDir}/${sanitizeFileName(type.fullName)}`
      })
    }
    
    sidebar.push(namespaceItem)
  }
  
  return sidebar
}

export function getSidebarConfig(outputDir = 'api-generated') {
  // This can be imported and used in VitePress config
  const fs = require('fs')
  const path = require('path')
  
  const configPath = path.join(process.cwd(), outputDir, '_sidebar.json')
  
  if (fs.existsSync(configPath)) {
    const content = fs.readFileSync(configPath, 'utf-8')
    return JSON.parse(content)
  }
  
  return []
}

function sanitizeFileName(name) {
  return name.replace(/[<>:"/\\|?*]/g, '_').replace(/`/g, '')
}