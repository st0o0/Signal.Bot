import fs from 'fs'
import path from 'path'
import { sanitizeFileName } from './parseXmlDocs.js'

export async function generateMarkdown(parsedDocs, outputDir) {
  const generatedFiles = []
  
  const availableTypes = new Set(parsedDocs.types.map(t => t.fullName))
  
  const indexPath = path.join(outputDir, 'index.md')
  fs.writeFileSync(indexPath, generateIndexPage(parsedDocs, availableTypes))
  generatedFiles.push(indexPath)
  
  const typesByNamespace = {}
  for (const type of parsedDocs.types) {
    const namespace = type.fullName.substring(0, type.fullName.lastIndexOf('.'))
    if (!typesByNamespace[namespace]) {
      typesByNamespace[namespace] = []
    }
    typesByNamespace[namespace].push(type)
  }
  
  for (const type of parsedDocs.types) {
    const fileName = sanitizeFileName(type.fullName) + '.md'
    const filePath = path.join(outputDir, fileName)
    
    const typeMethods = parsedDocs.methods.filter(m => m.className === type.fullName)
    const typeProperties = parsedDocs.properties.filter(p => p.className === type.fullName)
    
    fs.writeFileSync(filePath, generateTypePage(type, typeMethods, typeProperties, availableTypes))
    generatedFiles.push(filePath)
  }
  
  for (const [namespace, types] of Object.entries(typesByNamespace)) {
    const fileName = sanitizeFileName(namespace) + '.namespace.md'
    const filePath = path.join(outputDir, fileName)
    fs.writeFileSync(filePath, generateNamespacePage(namespace, types, availableTypes))
    generatedFiles.push(filePath)
  }
  
  return generatedFiles
}

function generateIndexPage(parsedDocs, availableTypes) {
  const { assembly, types } = parsedDocs
  
  const namespaces = {}
  types.forEach(type => {
    const ns = type.fullName.substring(0, type.fullName.lastIndexOf('.'))
    if (!namespaces[ns]) namespaces[ns] = []
    namespaces[ns].push(type)
  })
  
  let md = `# API Reference

This documentation was automatically generated from XML documentation comments.

## Namespaces

`
  
  for (const [namespace, nsTypes] of Object.entries(namespaces)) {
    md += `### ${namespace}\n\n`
    md += `[View Namespace →](./${sanitizeFileName(namespace)}.namespace)\n\n`
    
    for (const type of nsTypes.slice(0, 5)) {
      md += `- [\`${type.shortName}\`](./${sanitizeFileName(type.fullName)})`
      const summary = extractText(type.summary, availableTypes)
      if (summary) {
        md += ` - ${summary.split('\n')[0]}`
      }
      md += '\n'
    }
    
    if (nsTypes.length > 5) {
      md += `\n*... and ${nsTypes.length - 5} more*\n`
    }
    md += '\n'
  }
  
  return md
}

function generateNamespacePage(namespace, types, availableTypes) {
  let md = `# ${namespace} Namespace\n\n`
  
  md += `## Types\n\n`
  
  for (const type of types) {
    md += `### [\`${type.shortName}\`](./${sanitizeFileName(type.fullName)})\n\n`
    const summary = extractText(type.summary, availableTypes)
    if (summary) {
      md += `${summary}\n\n`
    }
  }
  
  return md
}

function generateTypePage(type, methods, properties, availableTypes) {
  let md = `# ${type.shortName}\n\n`
  
  md += `**Namespace:** \`${type.fullName.substring(0, type.fullName.lastIndexOf('.'))}\`\n\n`
  md += `**Full Name:** \`${type.fullName}\`\n\n`
  
  const summary = extractText(type.summary, availableTypes)
  if (summary) {
    md += `## Summary\n\n${summary}\n\n`
  }
  
  const remarks = extractText(type.remarks, availableTypes)
  if (remarks) {
    md += `## Remarks\n\n${remarks}\n\n`
  }
  
  if (type.enumValues && type.enumValues.length > 0) {
    md += `## Values\n\n`
    
    for (const value of type.enumValues) {
      md += `### ${value.fieldName}\n\n`
      const valueSummary = extractText(value.summary, availableTypes)
      if (valueSummary) {
        md += `${valueSummary}\n\n`
      }
    }
  }
  
  if (properties.length > 0) {
    md += `## Properties\n\n`
    
    for (const prop of properties) {
      md += `### ${prop.propertyName}\n\n`
      const propSummary = extractText(prop.summary, availableTypes)
      if (propSummary) {
        md += `${propSummary}\n\n`
      }
    }
  }
  
  if (methods.length > 0) {
    md += `## Methods\n\n`
    
    for (const method of methods) {
      md += `### ${method.methodName}\n\n`
      
      const params = method.params.map(p => `${p.name}`).join(', ')
      md += '```csharp\n'
      md += `${method.methodName}(${params})\n`
      md += '```\n\n'
      
      const methodSummary = extractText(method.summary, availableTypes)
      if (methodSummary) {
        md += `${methodSummary}\n\n`
      }
      
      if (method.params.length > 0) {
        md += `**Parameters:**\n\n`
        for (const param of method.params) {
          md += `- \`${param.name}\``
          const paramDesc = extractText(param.description, availableTypes)
          if (paramDesc) {
            md += ` - ${paramDesc}`
          }
          md += '\n'
        }
        md += '\n'
      }
      
      const returns = extractText(method.returns, availableTypes)
      if (returns) {
        md += `**Returns:** ${returns}\n\n`
      }
      
      if (method.exceptions.length > 0) {
        md += `**Exceptions:**\n\n`
        for (const ex of method.exceptions) {
          md += `- \`${ex.type}\``
          const exDesc = extractText(ex.description, availableTypes)
          if (exDesc) {
            md += ` - ${exDesc}`
          }
          md += '\n'
        }
        md += '\n'
      }
      
      const example = extractText(method.example, availableTypes)
      if (example) {
        md += `**Example:**\n\n`
        md += '```csharp\n'
        md += example
        md += '\n```\n\n'
      }
    }
  }
  
  const typeExample = extractText(type.example, availableTypes)
  if (typeExample) {
    md += `## Example\n\n`
    md += '```csharp\n'
    md += typeExample
    md += '\n```\n\n'
  }
  
  return md
}

function extractText(element, availableTypes) {
  if (!element) return null
  
  if (Array.isArray(element)) {
    if (element.length === 0) return null
    return extractText(element[0], availableTypes)
  }
  
  if (typeof element === 'string') {
    return postProcessText(element.trim(), availableTypes)
  }
  
  if (typeof element === 'object') {
    // Handle explicitChildren format ($$)
    if (element.$$) {
      return processMixedContent(element, availableTypes)
    }
    
    // Fallback to text content
    if (element._ && typeof element._ === 'string') {
      return postProcessText(element._.trim(), availableTypes)
    }
    
    return processXmlContent(element, availableTypes)
  }
  
  return null
}

function processMixedContent(element, availableTypes) {
  if (!element || !element.$$) return ''
  
  let result = ''
  
  for (const child of element.$$) {
    if (!child) continue // Safety check
    
    const tagName = child['#name']
    
    if (tagName === '__text__') {
      // Plain text node
      result += child._ || ''
    } else if (tagName === 'see') {
      // <see cref="..."/> or <see langword="..."/> or <see href="..."/>
      if (!child.$) {
        console.warn('⚠️  <see> tag without attributes found')
        continue
      }
      
      // Handle <see langword="..."/>
      if (child.$.langword) {
        const langword = child.$.langword
        // Format language keywords like null, true, false, etc.
        result += `\`${langword}\``
      }
      // Handle <see href="..."/>
      else if (child.$.href) {
        const href = child.$.href
        const linkText = child._ || href
        result += `[${linkText}](${href})`
      }
      // Handle <see cref="..."/>
      else if (child.$.cref) {
        const cref = child.$.cref.replace(/^[TPMF]:/, '')
        const linkText = cref.split('.').pop()
        
        if (availableTypes && availableTypes.has(cref)) {
          const fileName = sanitizeFileName(cref)
          result += `[\`${linkText}\`](/api/${fileName})`
        } else {
          result += `\`${linkText}\``
        }
      }
      else {
        console.warn('⚠️  <see> tag without cref, langword, or href attribute')
      }
    } else if (tagName === 'seealso') {
      // <seealso cref="..."/> or <seealso href="..."/>
      if (!child.$) continue
      
      if (child.$.cref) {
        const cref = child.$.cref.replace(/^[TPMF]:/, '')
        const linkText = cref.split('.').pop()
        
        if (availableTypes && availableTypes.has(cref)) {
          const fileName = sanitizeFileName(cref)
          result += `[\`${linkText}\`](/api/${fileName})`
        } else {
          result += `\`${linkText}\``
        }
      } else if (child.$.href) {
        const href = child.$.href
        const linkText = child._ || href
        result += `[${linkText}](${href})`
      }
    } else if (tagName === 'paramref') {
      // <paramref name="..."/>
      if (child.$ && child.$.name) {
        result += `\`${child.$.name}\``
      }
    } else if (tagName === 'typeparamref') {
      // <typeparamref name="..."/>
      if (child.$ && child.$.name) {
        result += `\`${child.$.name}\``
      }
    } else if (tagName === 'c') {
      // <c>code</c>
      if (child.$$) {
        result += `\`${processMixedContent(child, availableTypes)}\``
      } else if (child._) {
        result += `\`${child._}\``
      }
    } else if (tagName === 'code') {
      // <code>...</code>
      if (child.$$) {
        result += '\n```\n' + processMixedContent(child, availableTypes) + '\n```\n'
      } else if (child._) {
        result += '\n```\n' + child._ + '\n```\n'
      }
    } else if (tagName === 'para') {
      // <para>...</para>
      result += '\n\n' + processMixedContent(child, availableTypes) + '\n\n'
    } else if (tagName === 'list') {
      // <list>...</list>
      result += '\n\n' + processListElement(child, availableTypes)
    } else if (tagName === 'example') {
      // <example>...</example>
      result += '\n\n**Example:**\n\n' + processMixedContent(child, availableTypes) + '\n\n'
    } else if (tagName === 'remarks') {
      // <remarks>...</remarks>
      result += '\n\n**Remarks:**\n\n' + processMixedContent(child, availableTypes) + '\n\n'
    }
  }
  
  return result.trim()
}

function processXmlContent(element, availableTypes) {
  if (!element) return ''
  
  let result = ''
  
  if (element._ && typeof element._ === 'string') {
    result += element._
  }
  
  if (element.see) {
    const sees = Array.isArray(element.see) ? element.see : [element.see]
    for (const see of sees) {
      if (see && see.$ && see.$.cref) {
        const cref = see.$.cref.replace(/^[TPMF]:/, '')
        const linkText = cref.split('.').pop()
        
        if (availableTypes && availableTypes.has(cref)) {
          const fileName = sanitizeFileName(cref)
          result += `[\`${linkText}\`](/api/${fileName})`
        } else {
          result += `\`${linkText}\``
        }
      }
    }
  }
  
  if (element.paramref) {
    const paramrefs = Array.isArray(element.paramref) ? element.paramref : [element.paramref]
    for (const paramref of paramrefs) {
      if (paramref && paramref.$ && paramref.$.name) {
        result += `\`${paramref.$.name}\``
      }
    }
  }
  
  if (element.para) {
    const paras = Array.isArray(element.para) ? element.para : [element.para]
    for (const para of paras) {
      if (para) {
        result += '\n\n' + (typeof para === 'string' ? para : processXmlContent(para, availableTypes))
      }
    }
  }
  
  if (element.list) {
    result += '\n\n' + processListElement(element.list, availableTypes)
  }
  
  return result.trim()
}

function processListElement(listElement, availableTypes) {
  if (!listElement || !listElement.item) return ''
  
  const items = Array.isArray(listElement.item) ? listElement.item : [listElement.item]
  const listType = listElement.$?.type || 'bullet'
  
  let result = ''
  items.forEach((item, index) => {
    const bullet = listType === 'number' ? `${index + 1}.` : '-'
    
    if (typeof item === 'string') {
      result += `${bullet} ${item}\n`
    } else if (item.$$ && item.term && item.description) {
      // Definition list with mixed content
      const term = processMixedContent(item.term[0], availableTypes)
      const desc = processMixedContent(item.description[0], availableTypes)
      result += `${bullet} **${term}** - ${desc}\n`
    } else if (item.term && item.description) {
      // Definition list
      const term = typeof item.term === 'string' ? item.term : processXmlContent(item.term[0], availableTypes)
      const desc = typeof item.description === 'string' ? item.description : processXmlContent(item.description[0], availableTypes)
      result += `${bullet} **${term}** - ${desc}\n`
    } else if (item.$$) {
      result += `${bullet} ${processMixedContent(item, availableTypes)}\n`
    } else if (item._) {
      result += `${bullet} ${item._}\n`
    } else {
      result += `${bullet} ${processXmlContent(item, availableTypes)}\n`
    }
  })
  
  return result
}

function postProcessText(text, availableTypes) {
  if (!text) return text
  
  text = text.replace(/<see cref="([TPMF]:)?([^"]+)"(\s*\/)?>/g, (match, prefix, cref) => {
    const cleanRef = cref.replace(/^[TPMF]:/, '')
    const linkText = cleanRef.split('.').pop()
    
    if (availableTypes.has(cleanRef)) {
      const fileName = sanitizeFileName(cleanRef)
      return `[\`${linkText}\`](/api/${fileName})`
    } else {
      return `\`${linkText}\``
    }
  })
  
  text = text.replace(/<paramref name="([^"]+)"(\s*\/)?>/g, (match, name) => {
    return `\`${name}\``
  })
  
  text = text.replace(/<typeparamref name="([^"]+)"(\s*\/)?>/g, (match, name) => {
    return `\`${name}\``
  })
  
  text = text.replace(/<c>([^<]+)<\/c>/g, (match, code) => {
    return `\`${code}\``
  })
  
  return text
}