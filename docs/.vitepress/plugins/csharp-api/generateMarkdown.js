import fs from "fs";
import path from "path";
import { sanitizeFileName } from "./utils.js";

export async function generateMarkdown(parsedDocs, outputPath, outputDir) {
  const generatedFiles = [];
  const availableTypes = new Set(parsedDocs.types.map((t) => t.fullName));

  const indexPath = path.join(outputPath, "index.md");
  fs.writeFileSync(
    indexPath,
    generateIndexPage(parsedDocs, availableTypes, outputDir),
  );
  generatedFiles.push(indexPath);

  const typesByNamespace = {};
  for (const type of parsedDocs.types) {
    const namespace = type.fullName.substring(
      0,
      type.fullName.lastIndexOf("."),
    );
    if (!typesByNamespace[namespace]) typesByNamespace[namespace] = [];
    typesByNamespace[namespace].push(type);
  }

  for (const type of parsedDocs.types) {
    const fileName = sanitizeFileName(type.fullName) + ".md";
    const filePath = path.join(outputPath, fileName);
    const typeMethods = parsedDocs.methods.filter(
      (m) => m.className === type.fullName,
    );
    const typeProperties = parsedDocs.properties.filter(
      (p) => p.className === type.fullName,
    );
    fs.writeFileSync(
      filePath,
      generateTypePage(
        type,
        typeMethods,
        typeProperties,
        availableTypes,
        outputDir,
      ),
    );
    generatedFiles.push(filePath);
  }

  for (const [namespace, types] of Object.entries(typesByNamespace)) {
    const fileName = sanitizeFileName(namespace) + ".Namespace.md";
    const filePath = path.join(outputPath, fileName);
    fs.writeFileSync(
      filePath,
      generateNamespacePage(namespace, types, availableTypes, outputDir),
    );
    generatedFiles.push(filePath);
  }

  return generatedFiles;
}

function generateIndexPage(parsedDocs, availableTypes, outputDir) {
  const { types } = parsedDocs;

  const namespaces = {};
  types.forEach((type) => {
    const ns = type.fullName.substring(0, type.fullName.lastIndexOf("."));
    if (!namespaces[ns]) namespaces[ns] = [];
    namespaces[ns].push(type);
  });

  let md = `# API Reference\n\nThis documentation was automatically generated from XML documentation comments.\n\n## Namespaces\n\n`;

  for (const [namespace, nsTypes] of Object.entries(namespaces)) {
    md += `### ${namespace}\n\n`;
    md += `[View Namespace →](./${sanitizeFileName(namespace)}.Namespace)\n\n`;

    for (const type of nsTypes.slice(0, 5)) {
      md += `- [\`${type.shortName}\`](./${sanitizeFileName(type.fullName)})`;
      const summary = extractText(type.summary, availableTypes, outputDir);
      if (summary) md += ` - ${summary.split("\n")[0]}`;
      md += "\n";
    }

    if (nsTypes.length > 5) md += `\n*... and ${nsTypes.length - 5} more*\n`;
    md += "\n";
  }

  return md;
}

function generateNamespacePage(namespace, types, availableTypes, outputDir) {
  let md = `# ${namespace} Namespace\n\n## Types\n\n`;

  for (const type of types) {
    md += `### [\`${type.shortName}\`](./${sanitizeFileName(type.fullName)})\n\n`;
    const summary = extractText(type.summary, availableTypes, outputDir);
    if (summary) md += `${summary}\n\n`;
  }

  return md;
}

function generateTypePage(
  type,
  methods,
  properties,
  availableTypes,
  outputDir,
) {
  let md = `# ${type.shortName}\n\n`;
  md += `**Namespace:** \`${type.fullName.substring(0, type.fullName.lastIndexOf("."))}\`\n\n`;
  md += `**Full Name:** \`${type.fullName}\`\n\n`;

  const summary = extractText(type.summary, availableTypes, outputDir);
  if (summary) md += `## Summary\n\n${summary}\n\n`;

  const remarks = extractText(type.remarks, availableTypes, outputDir);
  if (remarks) md += `## Remarks\n\n${remarks}\n\n`;

  if (type.enumValues?.length > 0) {
    md += `## Values\n\n`;
    for (const value of type.enumValues) {
      md += `### ${value.fieldName}\n\n`;
      const valueSummary = extractText(
        value.summary,
        availableTypes,
        outputDir,
      );
      if (valueSummary) md += `${valueSummary}\n\n`;
    }
  }

  if (properties.length > 0) {
    md += `## Properties\n\n`;
    for (const prop of properties) {
      md += `### ${prop.propertyName}\n\n`;
      const propSummary = extractText(prop.summary, availableTypes, outputDir);
      if (propSummary) md += `${propSummary}\n\n`;
    }
  }

  if (methods.length > 0) {
    md += `## Methods\n\n`;
    for (const method of methods) {
      md += `### ${method.methodName}\n\n`;
      const params = method.params.map((p) => p.name).join(", ");
      md += "```csharp\n" + `${method.methodName}(${params})` + "\n```\n\n";

      const methodSummary = extractText(
        method.summary,
        availableTypes,
        outputDir,
      );
      if (methodSummary) md += `${methodSummary}\n\n`;

      if (method.params.length > 0) {
        md += `**Parameters:**\n\n`;
        for (const param of method.params) {
          md += `- \`${param.name}\``;
          const paramDesc = extractText(
            param.description,
            availableTypes,
            outputDir,
          );
          if (paramDesc) md += ` - ${paramDesc}`;
          md += "\n";
        }
        md += "\n";
      }

      const returns = extractText(method.returns, availableTypes, outputDir);
      if (returns) md += `**Returns:** ${returns}\n\n`;

      if (method.exceptions.length > 0) {
        md += `**Exceptions:**\n\n`;
        for (const ex of method.exceptions) {
          md += `- \`${ex.type}\``;
          const exDesc = extractText(ex.description, availableTypes, outputDir);
          if (exDesc) md += ` - ${exDesc}`;
          md += "\n";
        }
        md += "\n";
      }

      const example = extractText(method.example, availableTypes, outputDir);
      if (example) md += `**Example:**\n\n\`\`\`csharp\n${example}\n\`\`\`\n\n`;
    }
  }

  const typeExample = extractText(type.example, availableTypes, outputDir);
  if (typeExample)
    md += `## Example\n\n\`\`\`csharp\n${typeExample}\n\`\`\`\n\n`;

  return md;
}

function extractText(element, availableTypes, outputDir) {
  if (!element) return null;
  if (Array.isArray(element)) {
    if (element.length === 0) return null;
    return extractText(element[0], availableTypes, outputDir);
  }
  if (typeof element === "string")
    return postProcessText(element.trim(), availableTypes, outputDir);
  if (typeof element === "object") {
    if (element.$$)
      return processMixedContent(element, availableTypes, outputDir);
    if (element._ && typeof element._ === "string")
      return postProcessText(element._.trim(), availableTypes, outputDir);
    return processXmlContent(element, availableTypes, outputDir);
  }
  return null;
}

function processMixedContent(element, availableTypes, outputDir) {
  if (!element?.$$) return "";

  let result = "";

  for (const child of element.$$) {
    if (!child) continue;
    const tagName = child["#name"];

    if (tagName === "__text__") {
      result += child._ || "";
    } else if (tagName === "see") {
      if (!child.$) {
        console.warn("⚠️  <see> tag without attributes");
        continue;
      }
      if (child.$.langword) {
        result += `\`${child.$.langword}\``;
      } else if (child.$.href) {
        result += `[${child._ || child.$.href}](${child.$.href})`;
      } else if (child.$.cref) {
        result += formatCref(child.$.cref, availableTypes, outputDir);
      }
    } else if (tagName === "seealso") {
      if (!child.$) continue;
      if (child.$.cref) {
        result += formatCref(child.$.cref, availableTypes, outputDir);
      } else if (child.$.href) {
        result += `[${child._ || child.$.href}](${child.$.href})`;
      }
    } else if (tagName === "paramref" || tagName === "typeparamref") {
      if (child.$?.name) result += `\`${child.$.name}\``;
    } else if (tagName === "c") {
      result += `\`${child.$$ ? processMixedContent(child, availableTypes, outputDir) : child._ || ""}\``;
    } else if (tagName === "code") {
      const code = child.$$
        ? processMixedContent(child, availableTypes, outputDir)
        : child._ || "";
      result += `\n\`\`\`\n${code}\n\`\`\`\n`;
    } else if (tagName === "para") {
      result +=
        "\n\n" + processMixedContent(child, availableTypes, outputDir) + "\n\n";
    } else if (tagName === "list") {
      result += "\n\n" + processListElement(child, availableTypes, outputDir);
    } else if (tagName === "example") {
      result +=
        "\n\n**Example:**\n\n" +
        processMixedContent(child, availableTypes, outputDir) +
        "\n\n";
    } else if (tagName === "remarks") {
      result +=
        "\n\n**Remarks:**\n\n" +
        processMixedContent(child, availableTypes, outputDir) +
        "\n\n";
    }
  }

  return result.trim();
}

function processXmlContent(element, availableTypes, outputDir) {
  if (!element) return "";
  let result = "";

  if (element._ && typeof element._ === "string") result += element._;

  if (element.see) {
    const sees = Array.isArray(element.see) ? element.see : [element.see];
    for (const see of sees) {
      if (see?.$?.cref)
        result += formatCref(see.$.cref, availableTypes, outputDir);
    }
  }

  if (element.paramref) {
    const refs = Array.isArray(element.paramref)
      ? element.paramref
      : [element.paramref];
    for (const ref of refs) {
      if (ref?.$?.name) result += `\`${ref.$.name}\``;
    }
  }

  if (element.para) {
    const paras = Array.isArray(element.para) ? element.para : [element.para];
    for (const para of paras) {
      if (para)
        result +=
          "\n\n" +
          (typeof para === "string"
            ? para
            : processXmlContent(para, availableTypes, outputDir));
    }
  }

  if (element.list)
    result +=
      "\n\n" + processListElement(element.list, availableTypes, outputDir);

  return result.trim();
}

function processListElement(listElement, availableTypes, outputDir) {
  if (!listElement?.item) return "";
  const items = Array.isArray(listElement.item)
    ? listElement.item
    : [listElement.item];
  const listType = listElement.$?.type || "bullet";

  return (
    items
      .map((item, index) => {
        const bullet = listType === "number" ? `${index + 1}.` : "-";
        if (typeof item === "string") return `${bullet} ${item}`;
        if (item.term && item.description) {
          const term =
            typeof item.term === "string"
              ? item.term
              : processXmlContent(item.term[0], availableTypes, outputDir);
          const desc =
            typeof item.description === "string"
              ? item.description
              : processXmlContent(
                  item.description[0],
                  availableTypes,
                  outputDir,
                );
          return `${bullet} **${term}** - ${desc}`;
        }
        if (item.$$)
          return `${bullet} ${processMixedContent(item, availableTypes, outputDir)}`;
        if (item._) return `${bullet} ${item._}`;
        return `${bullet} ${processXmlContent(item, availableTypes, outputDir)}`;
      })
      .join("\n") + "\n"
  );
}

function formatCref(cref, availableTypes, outputDir) {
  const clean = cref.replace(/^[TPMF]:/, "");
  const linkText = clean.split(".").pop();
  if (availableTypes?.has(clean)) {
    return `[\`${linkText}\`](/${outputDir}/${sanitizeFileName(clean)})`;
  }
  return `\`${linkText}\``;
}

function postProcessText(text, availableTypes, outputDir) {
  if (!text) return text;

  text = text.replace(
    /<see cref="([TPMF]:)?([^"]+)"(\s*\/)?>/g,
    (_, _prefix, cref) => formatCref(cref, availableTypes, outputDir),
  );
  text = text.replace(
    /<paramref name="([^"]+)"(\s*\/)?>/g,
    (_, name) => `\`${name}\``,
  );
  text = text.replace(
    /<typeparamref name="([^"]+)"(\s*\/)?>/g,
    (_, name) => `\`${name}\``,
  );
  text = text.replace(/<c>([^<]+)<\/c>/g, (_, code) => `\`${code}\``);

  return text;
}
