import fs from "fs";
import path from "path";
import { glob } from "glob";
import { fileURLToPath } from "url";
import { parseStringPromise } from "xml2js";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const DOCS_DIR = path.resolve(__dirname, "..");

// ── Config ───────────────────────────────────────────────────────────────────
const XML_PATH = "../src/Signal.Bot/bin/Release/*/Signal.Bot.xml";
const OUTPUT_DIR = "api";
const EXCLUDED_NAMESPACES = ["System", "Microsoft", "Internal"];
const GITHUB_OWNER = "st0o0";
const GITHUB_REPO = "Signal.Bot";
const CHANGELOG_MAX_HIGHLIGHTS = 5;

// ── Utils ────────────────────────────────────────────────────────────────────
function sanitizeFileName(name) {
  return name.replace(/[<>:"/\\|?*]/g, "_").replace(/`/g, "");
}

// ── XML Parsing ──────────────────────────────────────────────────────────────
async function parseXmlDocs(xmlPath) {
  const xmlContent = fs.readFileSync(xmlPath, "utf-8");
  const parsed = await parseStringPromise(xmlContent, {
    explicitChildren: true,
    preserveChildrenOrder: true,
    charsAsChildren: true,
  });

  const assembly = parsed.doc.assembly[0].name[0];
  const members = parsed.doc.members[0].member || [];

  const types = [];
  const methods = [];
  const properties = [];
  const fields = [];
  const allMembers = [];

  for (const member of members) {
    const name = member.$.name;
    const memberData = {
      name,
      summary: member.summary,
      remarks: member.remarks,
      example: member.example,
      returns: member.returns,
      params: extractParams(member.param),
      exceptions: extractExceptions(member.exception),
      seeAlso: extractSeeAlso(member.seealso),
    };

    allMembers.push(memberData);

    if (name.startsWith("T:")) {
      types.push({
        ...memberData,
        type: "type",
        fullName: name.substring(2),
        shortName: name.substring(2).split(".").pop(),
      });
    } else if (name.startsWith("M:")) {
      const methodInfo = parseMethodSignature(name);
      methods.push({ ...memberData, type: "method", ...methodInfo });
    } else if (name.startsWith("P:")) {
      const propName = name.substring(2);
      properties.push({
        ...memberData,
        type: "property",
        fullName: propName,
        className: propName.substring(0, propName.lastIndexOf(".")),
        propertyName: propName.split(".").pop(),
      });
    } else if (name.startsWith("F:")) {
      const fieldName = name.substring(2);
      fields.push({
        ...memberData,
        type: "field",
        fullName: fieldName,
        className: fieldName.substring(0, fieldName.lastIndexOf(".")),
        fieldName: fieldName.split(".").pop(),
      });
    }
  }

  for (const type of types) {
    const enumValues = fields.filter((f) => f.className === type.fullName);
    if (enumValues.length > 0) type.enumValues = enumValues;
  }

  return { assembly, types, methods, properties, fields, members: allMembers };
}

function extractParams(params) {
  if (!params) return [];
  return params.map((p) => ({ name: p.$.name, description: p }));
}

function extractExceptions(exceptions) {
  if (!exceptions) return [];
  return exceptions.map((e) => ({
    type: e.$.cref?.replace("T:", ""),
    description: e,
  }));
}

function extractSeeAlso(seeAlso) {
  if (!seeAlso) return [];
  return seeAlso.map((s) => s.$.cref?.replace(/[TPM]:/, ""));
}

function parseMethodSignature(signature) {
  const withoutPrefix = signature.substring(2);
  const parenIndex = withoutPrefix.indexOf("(");
  if (parenIndex === -1) {
    const parts = withoutPrefix.split(".");
    return {
      fullName: withoutPrefix,
      className: parts.slice(0, -1).join("."),
      methodName: parts[parts.length - 1],
      parameters: [],
    };
  }
  const fullName = withoutPrefix.substring(0, parenIndex);
  const parts = fullName.split(".");
  const methodName = parts[parts.length - 1];
  const className = parts.slice(0, -1).join(".");
  const paramString = withoutPrefix.substring(
    parenIndex + 1,
    withoutPrefix.length - 1,
  );
  const parameters = paramString
    ? paramString.split(",").map((p) => ({ type: p.trim().split(".").pop() }))
    : [];
  return { fullName, className, methodName, parameters };
}

// ── Namespace Filter ─────────────────────────────────────────────────────────
function filterNamespaces(docs, excludeNamespaces) {
  if (!excludeNamespaces?.length) return docs;
  const isExcluded = (name) =>
    excludeNamespaces.some((ex) => name?.startsWith(ex + "."));
  return {
    ...docs,
    types: docs.types.filter((t) => !isExcluded(t.fullName)),
    methods: docs.methods.filter((m) => !isExcluded(m.className)),
    properties: docs.properties.filter((p) => !isExcluded(p.className)),
    fields: docs.fields.filter((f) => !isExcluded(f.className)),
    members: docs.members.filter((m) => {
      const name = m.name;
      if (name.startsWith("T:")) return !isExcluded(name.substring(2));
      if (/^[MPF]:/.test(name)) {
        const withoutPrefix = name.substring(2);
        const className = withoutPrefix.substring(
          0,
          withoutPrefix.lastIndexOf("."),
        );
        return !isExcluded(className);
      }
      return true;
    }),
  };
}

// ── Markdown Generation ──────────────────────────────────────────────────────
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

function processMixedContent(element, availableTypes, outputDir) {
  if (!element?.$$) return "";
  let result = "";
  for (const child of element.$$) {
    if (!child) continue;
    const tagName = child["#name"];
    if (tagName === "__text__") {
      result += child._ || "";
    } else if (tagName === "see") {
      if (!child.$) continue;
      if (child.$.langword) result += `\`${child.$.langword}\``;
      else if (child.$.href)
        result += `[${child._ || child.$.href}](${child.$.href})`;
      else if (child.$.cref)
        result += formatCref(child.$.cref, availableTypes, outputDir);
    } else if (tagName === "seealso") {
      if (!child.$) continue;
      if (child.$.cref)
        result += formatCref(child.$.cref, availableTypes, outputDir);
      else if (child.$.href)
        result += `[${child._ || child.$.href}](${child.$.href})`;
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

async function generateAllMarkdown(parsedDocs, outputPath, outputDir) {
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

// ── Sidebar Generation ───────────────────────────────────────────────────────
function generateSidebar(parsedDocs, outputDir) {
  const { types } = parsedDocs;
  const typesByNamespace = {};
  for (const type of types) {
    const namespace = type.fullName.substring(
      0,
      type.fullName.lastIndexOf("."),
    );
    if (!typesByNamespace[namespace]) typesByNamespace[namespace] = [];
    typesByNamespace[namespace].push(type);
  }

  const sidebar = [];
  sidebar.push({ text: "API Overview", link: `/${outputDir}/index` });

  for (const [namespace, nsTypes] of Object.entries(typesByNamespace).sort(
    ([a], [b]) => {
      const aDepth = a.split(".").length;
      const bDepth = b.split(".").length;
      if (aDepth !== bDepth) return aDepth - bDepth;
      return a.localeCompare(b);
    },
  )) {
    const namespaceItem = {
      text: namespace,
      collapsed: true,
      items: [],
    };
    namespaceItem.items.push({
      text: "Overview",
      link: `/${outputDir}/${sanitizeFileName(namespace)}.Namespace`,
    });
    for (const type of nsTypes) {
      namespaceItem.items.push({
        text: type.shortName,
        link: `/${outputDir}/${sanitizeFileName(type.fullName)}`,
      });
    }
    sidebar.push(namespaceItem);
  }

  return sidebar;
}

// ── Changelog Generation ─────────────────────────────────────────────────────
function formatChangelogLine(content, owner, repo) {
  return content
    .replace(
      /https:\/\/github\.com\/[^\s)]+\/pull\/(\d+)/g,
      `[#$1](https://github.com/${owner}/${repo}/pull/$1)`,
    )
    .replace(
      /https:\/\/github\.com\/[^\s)]+\/issues\/(\d+)/g,
      `[#$1](https://github.com/${owner}/${repo}/issues/$1)`,
    )
    .replace(
      /(?<!\[)(?<!\/)#(\d+)(?!\])/g,
      `[#$1](https://github.com/${owner}/${repo}/pull/$1)`,
    )
    .replace(/@([a-zA-Z0-9\-]+)\[bot\]/g, "`@$1[bot]`")
    .replace(/@([a-zA-Z0-9\-]+)/g, `[@$1](https://github.com/$1)`);
}

async function generateChangelog() {
  console.log("📦 Fetching GitHub releases for changelog...");

  const url = `https://api.github.com/repos/${GITHUB_OWNER}/${GITHUB_REPO}/releases`;
  const res = await fetch(url, {
    headers: {
      Accept: "application/vnd.github.v3+json",
      ...(process.env.GITHUB_TOKEN && {
        Authorization: `Bearer ${process.env.GITHUB_TOKEN}`,
      }),
    },
  });

  if (!res.ok) {
    console.error("[changelog] GitHub API error:", await res.text());
    process.exit(1);
  }

  const releases = await res.json();
  releases.sort((a, b) => new Date(b.published_at) - new Date(a.published_at));

  let markdown = `---
outline: 2
---
# Changelog
All notable releases of [${GITHUB_OWNER}/${GITHUB_REPO}](https://github.com/${GITHUB_OWNER}/${GITHUB_REPO}).

`;

  if (releases.length === 0) {
    markdown += `No releases found. Check [GitHub releases](https://github.com/${GITHUB_OWNER}/${GITHUB_REPO}/releases).\n`;
  } else {
    for (let i = 0; i < releases.length; i++) {
      const r = releases[i];
      const isLatest = i === 0 && !r.prerelease;
      const date = new Date(r.published_at).toLocaleDateString("en-US", {
        year: "numeric",
        month: "long",
        day: "numeric",
      });
      const name = r.name || r.tag_name;
      let badge = "";
      if (isLatest) badge = ` <Badge type="tip" text="latest" />`;
      else if (r.prerelease)
        badge = ` <Badge type="warning" text="pre-release" />`;

      const lines = (r.body || "").split("\n");
      const highlights = [];
      const firstContributions = [];

      for (const l of lines) {
        if (!l.startsWith("- ") && !l.startsWith("* ")) continue;
        const raw = l.slice(2).trim().replace(/\*\*/g, "");
        const isFirstContrib = /made their first contribution/i.test(raw);
        const isBot = /@[a-zA-Z0-9\-]+\[bot\]/.test(raw);
        if (isFirstContrib) {
          if (!isBot)
            firstContributions.push(
              formatChangelogLine(raw, GITHUB_OWNER, GITHUB_REPO),
            );
        } else {
          highlights.push(formatChangelogLine(raw, GITHUB_OWNER, GITHUB_REPO));
        }
      }

      markdown += `## ${name}${badge}\n\n`;
      markdown += `${date} · [View full release notes on GitHub →](${r.html_url})\n\n`;
      if (highlights.length) {
        markdown +=
          highlights
            .slice(0, CHANGELOG_MAX_HIGHLIGHTS)
            .map((h) => `- ${h}`)
            .join("\n") + "\n\n";
      }
      if (firstContributions.length > 0) {
        markdown += `**🎉 New Contributors:**\n\n`;
        markdown += firstContributions.map((c) => `- ${c}`).join("\n") + "\n\n";
      }
      markdown += "---\n\n";
    }
  }

  const changelogPath = path.resolve(DOCS_DIR, "changelog.md");
  fs.writeFileSync(changelogPath, markdown, "utf-8");
  console.log(`✅ Generated changelog.md with ${releases.length} releases`);
}

// ── Main ─────────────────────────────────────────────────────────────────────
console.log("🚀 Starting prebuild...\n");

// 1. API Docs
console.log("🔧 Pre-generating API docs...");
const searchPattern = path.resolve(DOCS_DIR, XML_PATH);
const matches = await glob(searchPattern, { windowsPathsNoEscape: true });

if (!matches.length) {
  console.error("❌ No XML documentation found!");
  process.exit(1);
}

console.log(`📖 Parsing XML documentation from: ${matches[0]}`);
const rawDocs = await parseXmlDocs(matches[0]);
const filteredDocs = filterNamespaces(rawDocs, EXCLUDED_NAMESPACES);

const removedTypes = rawDocs.types.length - filteredDocs.types.length;
console.log(`🔍 Filtered ${removedTypes} types from excluded namespaces`);
console.log(
  `✨ Found ${filteredDocs.types.length} types, ${filteredDocs.members.length} members`,
);

const outputPath = path.resolve(DOCS_DIR, OUTPUT_DIR);
if (fs.existsSync(outputPath)) fs.rmSync(outputPath, { recursive: true });
fs.mkdirSync(outputPath, { recursive: true });

const generatedFiles = await generateAllMarkdown(
  filteredDocs,
  outputPath,
  OUTPUT_DIR,
);
console.log(`✅ Generated ${generatedFiles.length} API documentation files`);

const sidebar = generateSidebar(filteredDocs, OUTPUT_DIR);
const sidebarPath = path.join(outputPath, "_sidebar.json");
fs.writeFileSync(sidebarPath, JSON.stringify(sidebar, null, 2));
console.log("📋 Generated sidebar configuration");

// 2. Changelog
console.log("");
await generateChangelog();

console.log("\n✅ Prebuild complete!");
