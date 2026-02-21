import path from "path";
import { fileURLToPath } from "url";
import { resolveAndParseDocs, generateSidebar } from "../.vitepress/plugins/csharp-api/index.js";
import { generateMarkdown } from "../.vitepress/plugins/csharp-api/generateMarkdown.js";
import fs from "fs";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const DOCS_DIR = path.resolve(__dirname, "..");

const XML_PATH = "../src/Signal.Bot/bin/Release/*/Signal.Bot.xml";
const OUTPUT_DIR = "api";
const EXCLUDED_NAMESPACES = ["System", "Microsoft", "Internal"];

const outputPath = path.resolve(DOCS_DIR, OUTPUT_DIR);

console.log("🔧 Pre-generating API docs...");

const result = await resolveAndParseDocs(XML_PATH, EXCLUDED_NAMESPACES);

if (!result) {
  console.error("❌ No XML documentation found!");
  process.exit(1);
}

if (fs.existsSync(outputPath)) {
  fs.rmSync(outputPath, { recursive: true });
}
fs.mkdirSync(outputPath, { recursive: true });

const generatedFiles = await generateMarkdown(result.docs, outputPath, OUTPUT_DIR);
console.log(`✅ Generated ${generatedFiles.length} API documentation files`);

const sidebar = generateSidebar(result.docs, OUTPUT_DIR);
const sidebarPath = path.join(outputPath, "_sidebar.json");
fs.writeFileSync(sidebarPath, JSON.stringify(sidebar, null, 2));
console.log("📋 Generated sidebar configuration");