// docs/.vitepress/plugins/code-snippets/index.js
import fs from "fs";
import path from "path";
import { glob } from "glob";
import { fileURLToPath } from "url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));

/**
 * @returns {import('vite').Plugin}
 */
export function codeSnippetsPlugin(options = {}) {
  const {
    sourceDir = "../src",
    exclude = ["**/obj/**", "**/bin/**", "**/node_modules/**"],
    verbose = true,
  } = options;

  const snippets = new Map();
  let basePath = "";

  return {
    name: "vitepress-code-snippets",

    enforce: /** @type {'pre'} */ ("pre"),

    configResolved(config) {
      basePath = config.root;
      console.log("🔍 Config resolved:");
      console.log("   Root:", basePath);
      console.log("   Source dir (relative):", sourceDir);
      console.log(
        "   Resolved source path:",
        path.resolve(basePath, sourceDir),
      );
    },

    async buildStart() {
      console.log("📝 Code Snippets Plugin: Starting extraction...");

      const searchPath = path.resolve(basePath, sourceDir);
      const pattern = path.join(searchPath, "**/*.cs").replace(/\\/g, "/");

      console.log("🔍 Search details:");
      console.log("   Base path:", basePath);
      console.log("   Search path:", searchPath);
      console.log("   Pattern:", pattern);
      console.log("   Exclude:", exclude);

      if (!fs.existsSync(searchPath)) {
        console.error(`❌ Source directory does not exist: ${searchPath}`);
        return;
      }

      try {
        const files = await glob(pattern, {
          ignore: exclude,
          absolute: true,
          dot: false,
        });

        console.log(`📁 Found ${files.length} C# files:`);
        files.forEach((file) => {
          console.log(`   - ${path.relative(searchPath, file)}`);
        });

        let totalSnippets = 0;

        for (const file of files) {
          console.log(`\n📄 Processing: ${path.relative(searchPath, file)}`);

          const content = fs.readFileSync(file, "utf-8");

          const regionCount = (content.match(/#region/g) || []).length;
          console.log(`   Found ${regionCount} #region tags`);

          const regions = extractRegions(content, file);

          console.log(`   Extracted ${regions.size} snippets:`);
          for (const [name, snippet] of regions) {
            console.log(
              `   ✓ ${name} (${snippet.code.split("\n").length} lines)`,
            );

            if (snippets.has(name)) {
              console.warn(`   ⚠️  Duplicate snippet name: ${name}`);
            }
            snippets.set(name, snippet);
            totalSnippets++;
          }
        }

        console.log(`\n✅ Total extracted: ${totalSnippets} snippets`);
        console.log("📋 All snippet names:");
        Array.from(snippets.keys()).forEach((name) => {
          console.log(`   - ${name}`);
        });
      } catch (error) {
        console.error("❌ Error extracting snippets:", error);
        console.error("Stack trace:", error.stack);
      }
    },

    transform(code, id) {
      if (!id.endsWith(".md")) return null;

      const snippetRegex =
        /<!-- snippet:\s*(\w+)(?:\s+show-source)?\s*-->\s*<!-- endSnippet -->/g;

      let hasChanges = false;
      const transformed = code.replace(snippetRegex, (match, name) => {
        const snippet = snippets.get(name);

        if (!snippet) {
          console.warn(`⚠️  Snippet not found: ${name}`);
          return match;
        }

        hasChanges = true;

        const showSource = match.includes("show-source");
        const sourceInfo = showSource
          ? `\n*Source: \`${path.basename(snippet.file)}\` (Line ${snippet.line})*\n`
          : "";

        return `<!-- snippet: ${name} -->\n\`\`\`csharp\n${snippet.code}\n\`\`\`${sourceInfo}\n<!-- endSnippet -->`;
      });

      return hasChanges ? transformed : null;
    },
  };
}

function extractRegions(content, filepath) {
  const regions = new Map();

  const regionRegex = /#region\s+(\w+)\s*\n([\s\S]*?)#endregion/g;

  let match;
  while ((match = regionRegex.exec(content)) !== null) {
    const [fullMatch, name, code] = match;

    console.log(`      Found region: "${name}"`);

    const lines = code.split("\n");
    const trimmedLines = trimIndentation(lines);
    const trimmedCode = trimmedLines.join("\n").trim();

    regions.set(name, {
      code: trimmedCode,
      file: filepath,
      line: getLineNumber(content, match.index),
    });
  }

  return regions;
}

function trimIndentation(lines) {
  const nonEmptyLines = lines.filter((line) => line.trim().length > 0);
  if (nonEmptyLines.length === 0) return lines;

  const minIndent = Math.min(
    ...nonEmptyLines.map((line) => line.match(/^\s*/)[0].length),
  );

  return lines.map((line) =>
    line.length > 0 ? line.substring(minIndent) : line,
  );
}

function getLineNumber(content, index) {
  return content.substring(0, index).split("\n").length;
}

export default codeSnippetsPlugin;
