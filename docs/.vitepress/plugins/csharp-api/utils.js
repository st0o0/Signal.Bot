export function sanitizeFileName(name) {
  return name.replace(/[<>:"/\\|?*]/g, "_").replace(/`/g, "");
}
