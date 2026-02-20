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
    async buildEnd() {
      try {
        const url = `https://api.github.com/repos/${owner}/${repo}/releases`;

        // Native fetch in Node 18+
        const res = await globalThis.fetch(url, {
          headers: { Accept: "application/vnd.github.v3+json" },
        });

        if (!res.ok) {
          console.error("[changelog] Github API Error:", await res.text());
          return;
        }

        const releases = await res.json();

        releases.sort(
          (a, b) => new Date(b.published_at) - new Date(a.published_at),
        );

        let markdown = `---
outline: deep
---

# Changelog

All notable releases of ${owner}/${repo}.

`;

        if (releases.length === 0) {
          markdown += `No releases found. Check [GitHub releases](https://github.com/${owner}/${repo}/releases).\n`;
        } else {
          for (const r of releases) {
            const date = new Date(r.published_at).toISOString().split("T")[0];
            const name = r.name || r.tag_name;
            const prereleaseBadge = r.prerelease ? " **pre-release**" : "";

            const highlights = (r.body || "")
              .split("\n")
              .filter((l) => l.startsWith("- ") || l.startsWith("* "))
              .map((l) =>
                l
                  .slice(2)
                  .replace(/`/g, "")
                  .replace(/\*\*/g, "")
                  .replace(/https:\/\/github\.com\/[^\s]+\/pull\/(\d+)/g, `[#$1](https://github.com/${owner}/${repo}/pull/$1)`),
              )
              .slice(0, maxHighlights);

            markdown += `## ${name} - ${date}${prereleaseBadge}

[View full release notes on GitHub →](${r.html_url})

`;

            if (highlights.length) {
              markdown += highlights.map((h) => `- ${h}`).join("\n") + "\n";
            }

            markdown += "\n";
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
