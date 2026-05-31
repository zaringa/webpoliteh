function buildQuery(params) {
  const searchParams = new URLSearchParams();

  if (params.search) searchParams.set("search", params.search);
  if (params.category) searchParams.set("category", params.category);
  if (params.minPrice) searchParams.set("minPrice", params.minPrice);
  if (params.maxPrice) searchParams.set("maxPrice", params.maxPrice);

  const query = searchParams.toString();
  return query ? `?${query}` : "";
}

async function getJson(url) {
  const response = await fetch(url);
  if (!response.ok) {
    throw new Error(`HTTP ${response.status}: ${response.statusText}`);
  }

  return response.json();
}

export async function fetchCategories() {
  return getJson("/api/catalog/categories");
}

export async function fetchCatalog(params) {
  return getJson(`/api/catalog${buildQuery(params)}`);
}

export async function fetchSummary(params) {
  return getJson(`/api/catalog/summary${buildQuery(params)}`);
}

export async function requestErrorDemo() {
  return getJson("/api/catalog/error-demo");
}
