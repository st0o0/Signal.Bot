import { defineConfig } from "vitepress";
import { csharpApiPlugin } from "./plugins/csharp-api/index.js";
import { githubChangelogMDPlugin } from "./plugins/github-releases-changelog/index.js";
import { createRequire } from "module";
import { existsSync } from "fs";
import { resolve, dirname } from "path";
import { fileURLToPath } from "url";

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

const sidebarPath = resolve(__dirname, "../api/_sidebar.json");
const sidebarJson = existsSync(sidebarPath)
  ? createRequire(import.meta.url)(sidebarPath)
  : {};

const EXCLUDED_NAMESPACES = ["System", "Microsoft", "Internal"];
const XML_PATH = "../src/Signal.Bot/bin/Release/*/Signal.Bot.xml";
const OUTPUT_DIR = "api";

export default defineConfig({
  title: "Signal.Bot",
  description: "A .NET Signal Messenger Bot Client",
  lang: "en-US",
  lastUpdated: true,
  cleanUrls: true,
  base: "/Signal.Bot/",
  srcDir: ".",
  srcExclude: ["node_modules", ".vitepress/cache", "scripts"],
  head: [["link", { rel: "icon", href: "/Signal.Bot/logo_small.png" }]],
  markdown: { lineNumbers: true },
  sitemap: { hostname: "https://st0o0.github.io/Signal.Bot/" },

  vite: {
    plugins: [
      githubChangelogMDPlugin({
        owner: "st0o0",
        repo: "Signal.Bot",
        output: "changelog.md",
        maxHighlights: 5,
      }),
      csharpApiPlugin({
        xmlPath: XML_PATH,
        outputDir: OUTPUT_DIR,
        autoSidebar: true,
        watch: true,
        excludeNamespaces: EXCLUDED_NAMESPACES,
      }),
    ],
  },

  themeConfig: {
    logo: "/logo_small.png",
    nav: [
      { text: "Home", link: "/" },
      { text: "Guide", link: "/guide/getting-started" },
      { text: "Examples", link: "/examples/" },
      { text: "API Reference", link: "/api/" },
      { text: "Changelog", link: "/changelog" },
    ],
    sidebar: {
      "/api/": sidebarJson,
      "/guide/": [
        {
          text: "Guide",
          items: [
            { text: "Getting Started", link: "/guide/getting-started" },
            { text: "Sending Messages", link: "/guide/sending-messages" },
            { text: "Receiving Messages", link: "/guide/receiving-messages" },
            { text: "Groups", link: "/guide/groups" },
            { text: "Attachments", link: "/guide/attachments" },
            { text: "Profiles", link: "/guide/profiles" },
          ],
        },
      ],
      "/examples/": [
        {
          text: "Examples",
          items: [
            { text: "Overview", link: "/examples/" },
            { text: "Echo Bot", link: "/examples/echo-bot" },
            { text: "Command Bot", link: "/examples/command-bot" },
          ],
        },
      ],
    },
    socialLinks: [
      { icon: "github", link: "https://github.com/st0o0/Signal.Bot" },
    ],
    footer: {
      message:
        'Released under the <a href="https://mit-license.org/">MIT License</a>.',
      copyright: `Copyright © 2025-${new Date().getFullYear()} <a href="https://github.com/st0o0">st0o0</a>`,
    },
    search: { provider: "local" },
  },
});
