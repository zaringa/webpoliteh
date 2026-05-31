import { fetchCatalog, fetchCategories, fetchSummary, requestErrorDemo } from "./api.js";

const elements = {
  searchInput: document.getElementById("searchInput"),
  categorySelect: document.getElementById("categorySelect"),
  minPriceInput: document.getElementById("minPriceInput"),
  maxPriceInput: document.getElementById("maxPriceInput"),
  loadButton: document.getElementById("loadButton"),
  sortButton: document.getElementById("sortButton"),
  errorButton: document.getElementById("errorButton"),
  statusText: document.getElementById("statusText"),
  productsBody: document.getElementById("productsBody"),
  summaryCards: document.getElementById("summaryCards"),
  barsContainer: document.getElementById("barsContainer")
};

const state = {
  sortAscending: true,
  items: []
};

function getFilters() {
  return {
    search: elements.searchInput.value.trim(),
    category: elements.categorySelect.value.trim(),
    minPrice: elements.minPriceInput.value.trim(),
    maxPrice: elements.maxPriceInput.value.trim()
  };
}

function updateStatus(text, isError = false) {
  elements.statusText.textContent = text;
  elements.statusText.style.color = isError ? "#b42d1f" : "#5f6f81";
}

function formatCurrency(value) {
  return new Intl.NumberFormat("ru-RU", {
    style: "currency",
    currency: "RUB",
    maximumFractionDigits: 0
  }).format(value);
}

function renderProducts(items) {
  if (!items.length) {
    elements.productsBody.innerHTML = `
      <tr>
        <td colspan="6">По фильтрам ничего не найдено.</td>
      </tr>`;
    return;
  }

  elements.productsBody.innerHTML = items
    .map((item) => `
      <tr>
        <td>${item.id}</td>
        <td>${item.name}</td>
        <td>${item.category}</td>
        <td>${formatCurrency(item.price)}</td>
        <td>${item.quantity}</td>
        <td>${item.rating.toFixed(1)}</td>
      </tr>`)
    .join("");
}

function renderSummary(summary) {
  const cards = [
    { title: "Товаров", value: summary.count },
    { title: "Единиц на складе", value: summary.totalInventoryUnits },
    { title: "Средний рейтинг", value: summary.averageRating.toFixed(2) },
    { title: "Стоимость остатков", value: formatCurrency(summary.inventoryValue) }
  ];

  elements.summaryCards.innerHTML = cards
    .map((card) => `
      <article class="card">
        <div class="card-title">${card.title}</div>
        <div class="card-value">${card.value}</div>
      </article>`)
    .join("");

  const maxCount = Math.max(...summary.categories.map((item) => item.count), 1);

  elements.barsContainer.innerHTML = summary.categories
    .map((item) => {
      const width = Math.round((item.count / maxCount) * 100);
      return `
        <div class="bar-row">
          <span>${item.category}</span>
          <div class="bar-track">
            <div class="bar-fill" style="width: ${width}%"></div>
          </div>
          <strong>${item.count}</strong>
        </div>
      `;
    })
    .join("");
}

function sortItems(items) {
  return [...items].sort((a, b) => {
    if (state.sortAscending) {
      return a.price - b.price;
    }

    return b.price - a.price;
  });
}

async function loadCategories() {
  const categories = await fetchCategories();

  for (const category of categories) {
    const option = document.createElement("option");
    option.value = category;
    option.textContent = category;
    elements.categorySelect.append(option);
  }
}

async function loadData() {
  updateStatus("Загрузка данных...");

  try {
    const filters = getFilters();
    const [catalogResponse, summary] = await Promise.all([
      fetchCatalog(filters),
      fetchSummary(filters)
    ]);

    state.items = catalogResponse.items;
    const sortedItems = sortItems(state.items);

    renderProducts(sortedItems);
    renderSummary(summary);

    updateStatus(`Данные обновлены. Получено записей: ${catalogResponse.total}.`);
  } catch (error) {
    updateStatus(`Ошибка загрузки: ${error.message}`, true);
  }
}

function toggleSort() {
  state.sortAscending = !state.sortAscending;
  elements.sortButton.textContent = state.sortAscending
    ? "Сортировка: цена по возрастанию"
    : "Сортировка: цена по убыванию";

  renderProducts(sortItems(state.items));
}

async function showBackendError() {
  updateStatus("Запрос к endpoint с ошибкой...");

  try {
    await requestErrorDemo();
  } catch (error) {
    updateStatus(`Backend вернул ошибку (ожидаемо): ${error.message}`, true);
  }
}

async function bootstrap() {
  elements.loadButton.addEventListener("click", loadData);
  elements.sortButton.addEventListener("click", toggleSort);
  elements.errorButton.addEventListener("click", showBackendError);

  await loadCategories();
  await loadData();
}

bootstrap();
