import fs from "fs";
import path from "path";
import { glob } from "glob";
import { parseXmlDocs } from "./parseXmlDocs.js";
import { generateMarkdown } from "./generateMarkdown.js";
import { generateSidebar } from "./generateSidebar.js";

export { generateSidebar } from "./generateSidebar.js";
export { parseXmlDocs } from "./parseXmlDocs.js";

// Module-level cache — survives across buildStart calls within the same process
let cachedDocs = null;
let generatedFiles = [];
let isGenerating = false;

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

export async function resolveAndParseDocs(xmlPath, excludeNamespaces = []) {
  if (cachedDocs) {
    console.log("📦 Using cached XML docs");
    return cachedDocs;
  }

  const searchPattern = path.resolve(process.cwd(), xmlPath);
  const matches = await glob(searchPattern, { windowsPathsNoEscape: true });

  if (matches.length === 0) {
    console.warn(`⚠️  No XML files found matching: ${searchPattern}`);
    return null;
  }

  if (matches.length > 1) {
    console.log(`ℹ️  Found ${matches.length} XML files, using: ${matches[0]}`);
  }

  const resolvedPath = matches[0];
  if (!fs.existsSync(resolvedPath)) {
    console.warn(`⚠️  XML file not found: ${resolvedPath}`);
    return null;
  }

  console.log(`📖 Parsing XML documentation from: ${resolvedPath}`);
  const rawDocs = await parseXmlDocs(resolvedPath);
  const filteredDocs = filterNamespaces(rawDocs, excludeNamespaces);

  if (excludeNamespaces.length > 0) {
    const removedTypes = rawDocs.types.length - filteredDocs.types.length;
    console.log(
      `🔍 Filtered out namespaces: ${excludeNamespaces.join(", ")} (${removedTypes} types removed)`,
    );
  }

  console.log(
    `✨ Found ${filteredDocs.types.length} types, ${filteredDocs.members.length} members`,
  );

  cachedDocs = { docs: filteredDocs, resolvedPath };
  return cachedDocs;
}

export function csharpApiPlugin(options = {}) {
  const {
    xmlPath,
    outputDir = "./api-generated",
    autoSidebar = true,
    watch = true,
    excludeNamespaces = [],
  } = options;

  async function generate(forceRegenerate = false) {
    if (isGenerating) return;
    isGenerating = true;

    try {
      // Clear cache when force regenerating (watch mode)
      if (forceRegenerate) cachedDocs = null;

      const result = await resolveAndParseDocs(xmlPath, excludeNamespaces);
      if (!result) return;

      const { docs } = result;
      const outputPath = path.resolve(process.cwd(), outputDir);

      // Skip if already generated in this process (e.g. called by config.ts first)
      if (
        !forceRegenerate &&
        generatedFiles.length > 0 &&
        fs.existsSync(path.join(outputPath, "index.md"))
      ) {
        console.log("📦 API docs already generated, skipping");
        return;
      }

      if (fs.existsSync(outputPath)) {
        fs.rmSync(outputPath, { recursive: true });
      }
      fs.mkdirSync(outputPath, { recursive: true });

      console.log("📝 Generating markdown files...");
      generatedFiles = await generateMarkdown(docs, outputPath, outputDir);
      console.log(
        `✅ Generated ${generatedFiles.length} API documentation files`,
      );

      if (autoSidebar) {
        const sidebarConfig = generateSidebar(docs, outputDir);
        const configPath = path.join(outputPath, "_sidebar.json");
        fs.writeFileSync(configPath, JSON.stringify(sidebarConfig, null, 2));
        console.log("📋 Generated sidebar configuration");
      }
    } catch (error) {
      console.error("❌ Error generating API docs:", error);
      throw error;
    } finally {
      isGenerating = false;
    }
  }

  return {
    name: "vitepress-csharp-api",
    enforce: /** @type {'pre'} */ ("pre"),

    async buildStart() {
      if (!xmlPath) {
        console.warn("⚠️  No XML path provided, skipping API generation");
        return;
      }
      console.log("🔧 C# API Plugin: Starting...");
      await generate();
    },

    configureServer(server) {
      if (!watch || !xmlPath) return;

      const searchPattern = path.resolve(process.cwd(), xmlPath);

      glob(searchPattern, { windowsPathsNoEscape: true }).then((matches) => {
        if (matches.length === 0) return;
        server.watcher.add(matches[0]);
        server.watcher.add(path.dirname(matches[0]));
      });

      server.watcher.on("change", async (file) => {
        const matches = await glob(searchPattern, {
          windowsPathsNoEscape: true,
        });
        if (matches.length === 0) return;

        if (file === matches[0] || file.endsWith(".xml")) {
          console.log("📖 XML documentation changed, regenerating...");
          await generate(true); // force regenerate + clear cache

          generatedFiles.forEach((f) => server.moduleGraph.onFileChange(f));
          server.ws.send({ type: "full-reload", path: "*" });
        }
      });
    },
  };
}
