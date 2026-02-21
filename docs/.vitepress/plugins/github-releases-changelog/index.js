import fs from "fs";
import path from "path";

export function githubChangelogMDPlugin({
  owner,
  repo,
  output = "changelog.md",
  maxHighlights = 5,
}) {
  return {
    name: "vitepress:github-changelog-md",

    async buildStart() {
      try {
        const url = `https://api.github.com/repos/${owner}/${repo}/releases`;
        const res = await globalThis.fetch(url, {
          headers: { Accept: "application/vnd.github.v3+json" },
        });

        if (!res.ok) {
          console.error("[changelog] GitHub API error:", await res.text());
          return;
        }

        const releases = await res.json();
        releases.sort(
          (a, b) => new Date(b.published_at) - new Date(a.published_at),
        );

        let markdown = `---
outline: 2
---

# Changelog

All notable releases of [${owner}/${repo}](https://github.com/${owner}/${repo}).

`;

        if (releases.length === 0) {
          markdown += `No releases found. Check [GitHub releases](https://github.com/${owner}/${repo}/releases).\n`;
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
            if (isLatest) {
              badge = ` <Badge type="tip" text="latest" />`;
            } else if (r.prerelease) {
              badge = ` <Badge type="warning" text="pre-release" />`;
            }

            const lines = (r.body || "").split("\n");

            const highlights = [];
            const firstContributions = [];

            for (const l of lines) {
              if (!l.startsWith("- ") && !l.startsWith("* ")) continue;

              const raw = l.slice(2).trim().replace(/\*\*/g, "");
              const isFirstContrib = /made their first contribution/i.test(raw);
              const isBot = /@[a-zA-Z0-9\-]+\[bot\]/.test(raw);

              if (isFirstContrib) {
                if (!isBot) {
                  firstContributions.push(formatLine(raw, owner, repo));
                }
              } else {
                highlights.push(formatLine(raw, owner, repo));
              }
            }

            markdown += `## ${name}${badge}\n\n`;
            markdown += `${date} · [View full release notes on GitHub →](${r.html_url})\n\n`;

            if (highlights.length) {
              markdown +=
                highlights
                  .slice(0, maxHighlights)
                  .map((h) => `- ${h}`)
                  .join("\n") + "\n\n";
            }

            if (firstContributions.length > 0) {
              markdown += `**🎉 New Contributors:**\n\n`;
              markdown +=
                firstContributions.map((c) => `- ${c}`).join("\n") + "\n\n";
            }

            markdown += "---\n\n";
          }
        }

        const outPath = path.join(process.cwd(), output);
        fs.writeFileSync(outPath, markdown, "utf-8");
        console.log(
          `[changelog] Generated ${outPath} with ${releases.length} releases`,
        );
      } catch (err) {
        console.error("[changelog] Error generating changelog.md:", err);
      }
    },
  };
}

function formatLine(content, owner, repo) {
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
