import fs from 'fs'
import { parseStringPromise } from 'xml2js'

export async function parseXmlDocs(xmlPath) {
  const xmlContent = fs.readFileSync(xmlPath, 'utf-8')
  
  // Configure parser to handle mixed content properly
  const parsed = await parseStringPromise(xmlContent, {
    explicitChildren: true,
    preserveChildrenOrder: true,
    charsAsChildren: true
  })
  
  const assembly = parsed.doc.assembly[0].name[0]
  const members = parsed.doc.members[0].member || []
  
  const types = []
  const methods = []
  const properties = []
  const fields = []
  const allMembers = []
  
  for (const member of members) {
    const name = member.$.name
    const memberData = {
      name,
      summary: member.summary,
      remarks: member.remarks,
      example: member.example,
      returns: member.returns,
      params: extractParams(member.param),
      exceptions: extractExceptions(member.exception),
      seeAlso: extractSeeAlso(member.seealso)
    }
    
    allMembers.push(memberData)
    
    if (name.startsWith('T:')) {
      types.push({
        ...memberData,
        type: 'type',
        fullName: name.substring(2),
        shortName: name.substring(2).split('.').pop()
      })
    } else if (name.startsWith('M:')) {
      const methodInfo = parseMethodSignature(name)
      methods.push({
        ...memberData,
        type: 'method',
        ...methodInfo
      })
    } else if (name.startsWith('P:')) {
      const propName = name.substring(2)
      properties.push({
        ...memberData,
        type: 'property',
        fullName: propName,
        className: propName.substring(0, propName.lastIndexOf('.')),
        propertyName: propName.split('.').pop()
      })
    } else if (name.startsWith('F:')) {
      const fieldName = name.substring(2)
      fields.push({
        ...memberData,
        type: 'field',
        fullName: fieldName,
        className: fieldName.substring(0, fieldName.lastIndexOf('.')),
        fieldName: fieldName.split('.').pop()
      })
    }
  }
  
  // Attach enum values to their parent types
  for (const type of types) {
    const enumValues = fields.filter(f => f.className === type.fullName)
    if (enumValues.length > 0) {
      type.enumValues = enumValues
    }
  }
  
  return {
    assembly,
    types,
    methods,
    properties,
    fields,
    members: allMembers
  }
}

function extractParams(params) {
  if (!params) return []
  
  return params.map(p => ({
    name: p.$.name,
    description: p
  }))
}

function extractExceptions(exceptions) {
  if (!exceptions) return []
  
  return exceptions.map(e => ({
    type: e.$.cref?.replace('T:', ''),
    description: e
  }))
}

function extractSeeAlso(seeAlso) {
  if (!seeAlso) return []
  
  return seeAlso.map(s => s.$.cref?.replace(/[TPM]:/, ''))
}

function parseMethodSignature(signature) {
  const withoutPrefix = signature.substring(2)
  const parenIndex = withoutPrefix.indexOf('(')
  
  if (parenIndex === -1) {
    const parts = withoutPrefix.split('.')
    return {
      fullName: withoutPrefix,
      className: parts.slice(0, -1).join('.'),
      methodName: parts[parts.length - 1],
      parameters: []
    }
  }
  
  const fullName = withoutPrefix.substring(0, parenIndex)
  const parts = fullName.split('.')
  const methodName = parts[parts.length - 1]
  const className = parts.slice(0, -1).join('.')
  
  const paramString = withoutPrefix.substring(parenIndex + 1, withoutPrefix.length - 1)
  const parameters = paramString ? paramString.split(',').map(p => {
    const typeName = p.trim().split('.').pop()
    return { type: typeName }
  }) : []
  
  return {
    fullName,
    className,
    methodName,
    parameters
  }
}

function sanitizeFileName(name) {
  return name.replace(/[<>:"/\\|?*]/g, '_').replace(/`/g, '')
}

export { sanitizeFileName }