import { sanitizeFileName } from "./utils.js";

export function generateSidebar(parsedDocs, outputDir) {
  const { types } = parsedDocs;

  const typesByNamespace = {};
  for (const type of types) {
    const namespace = type.fullName.substring(
      0,
      type.fullName.lastIndexOf("."),
    );
    if (!typesByNamespace[namespace]) {
      typesByNamespace[namespace] = [];
    }
    typesByNamespace[namespace].push(type);
  }

  const sidebar = [];

  sidebar.push({
    text: "API Overview",
    link: `/${outputDir}/index`,
  });

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
