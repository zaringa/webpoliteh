const FILTERS = [
  { id: "all", label: "Все" },
  { id: "available", label: "Только доступные" },
  { id: "unavailable", label: "Только выданные" },
];

const state = {
  books: [],
  activeFilter: "all",
  isLoading: false,
  error: null,
};

const filterControls = document.getElementById("filter-controls");
const statusBox = document.getElementById("status-box");
const booksList = document.getElementById("books-list");
const countAll = document.getElementById("count-all");
const countAvailable = document.getElementById("count-available");
const countUnavailable = document.getElementById("count-unavailable");

function fetchBooksFromServer() {
  const serverBooks = [
    {
      id: "b-101",
      title: "Чистая архитектура",
      author: "Роберт Мартин",
      year: 2018,
      genre: "Разработка ПО",
      available: true,
    },
    {
      id: "b-102",
      title: "Грокаем алгоритмы",
      author: "Адитья Бхаргава",
      year: 2017,
      genre: "Алгоритмы",
      available: false,
    },
    {
      id: "b-103",
      title: "Паттерны объектно-ориентированного проектирования",
      author: "Гамма, Хелм, Джонсон, Влиссидес",
      year: 2020,
      genre: "Архитектура",
      available: true,
    },
    {
      id: "b-104",
      title: "Совершенный код",
      author: "Стив Макконнелл",
      year: 2021,
      genre: "Практика программирования",
      available: false,
    },
    {
      id: "b-105",
      title: "Docker для профессионалов",
      author: "Скотт Джонстон",
      year: 2022,
      genre: "DevOps",
      available: true,
    },
    {
      id: "b-106",
      title: "Computer Networking: A Top-Down Approach",
      author: "Kurose, Ross",
      year: 2021,
      genre: "Сети",
      available: false,
    },
  ];

  return new Promise((resolve) => {
    setTimeout(() => {
      resolve(serverBooks);
    }, 1300);
  });
}

function getFilteredBooks() {
  if (state.activeFilter === "available") {
    return state.books.filter((book) => book.available);
  }

  if (state.activeFilter === "unavailable") {
    return state.books.filter((book) => !book.available);
  }

  return state.books;
}

function updateCounters() {
  const availableCount = state.books.filter((book) => book.available).length;

  countAll.textContent = String(state.books.length);
  countAvailable.textContent = String(availableCount);
  countUnavailable.textContent = String(state.books.length - availableCount);
}

function renderFilters() {
  filterControls.innerHTML = FILTERS.map((filter) => {
    const isActive = filter.id === state.activeFilter;
    const baseClasses =
      "rounded-full border px-4 py-2 text-sm font-semibold transition focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-cyan-400";
    const activeClasses = isActive
      ? "border-cyan-600 bg-cyan-600 text-white"
      : "border-slate-300 bg-white text-slate-700 hover:border-cyan-400 hover:text-cyan-700";

    return `
      <button
        type="button"
        class="${baseClasses} ${activeClasses}"
        data-filter="${filter.id}"
        role="tab"
        aria-selected="${isActive}"
      >
        ${filter.label}
      </button>
    `;
  }).join("");
}

function renderStatus() {
  if (state.isLoading) {
    statusBox.innerHTML = `
      <div class="flex items-center gap-3">
        <span class="inline-block h-5 w-5 animate-spin rounded-full border-2 border-cyan-600 border-r-transparent"></span>
        <p class="font-medium">Загрузка книг с сервера...</p>
      </div>
    `;
    return;
  }

  if (state.error) {
    statusBox.innerHTML = `
      <div class="flex flex-wrap items-center justify-between gap-3">
        <p class="font-medium text-red-700">${state.error}</p>
        <button
          type="button"
          id="retry-btn"
          class="rounded-full border border-red-300 bg-white px-4 py-2 text-sm font-semibold text-red-700 transition hover:border-red-500 hover:bg-red-50"
        >
          Повторить загрузку
        </button>
      </div>
    `;
    return;
  }

  const filteredBooks = getFilteredBooks();

  if (filteredBooks.length === 0 && state.books.length > 0) {
    statusBox.innerHTML =
      '<p class="font-medium">По выбранному фильтру книги не найдены.</p>';
    return;
  }

  if (state.books.length === 0) {
    statusBox.innerHTML = '<p class="font-medium">Каталог пока пуст.</p>';
    return;
  }

  statusBox.innerHTML = `
    <p class="font-medium">
      Показано книг: <span class="font-extrabold text-cyan-700">${filteredBooks.length}</span> из
      <span class="font-extrabold text-cyan-700">${state.books.length}</span>.
    </p>
  `;
}

function renderBooks() {
  if (state.isLoading || state.error) {
    booksList.innerHTML = "";
    return;
  }

  const filteredBooks = getFilteredBooks();

  if (filteredBooks.length === 0) {
    booksList.innerHTML = "";
    return;
  }

  booksList.innerHTML = filteredBooks
    .map((book) => {
      const availabilityText = book.available ? "Доступна" : "Выдана";
      const availabilityClasses = book.available
        ? "border-emerald-200 bg-emerald-50 text-emerald-700"
        : "border-orange-200 bg-orange-50 text-orange-700";

      return `
        <li class="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm transition hover:-translate-y-0.5 hover:shadow-md">
          <div class="flex items-start justify-between gap-2">
            <p class="text-sm font-semibold text-slate-500">${book.genre}</p>
            <span class="rounded-full border px-3 py-1 text-xs font-bold ${availabilityClasses}">
              ${availabilityText}
            </span>
          </div>

          <h2 class="mt-3 text-lg font-bold leading-snug">${book.title}</h2>
          <p class="mt-2 text-sm text-slate-600">${book.author}</p>

          <div class="mt-4 border-t border-slate-100 pt-3 text-sm text-slate-500">
            Год издания: <span class="font-semibold text-slate-700">${book.year}</span>
          </div>
        </li>
      `;
    })
    .join("");
}

function render() {
  renderFilters();
  updateCounters();
  renderStatus();
  renderBooks();
}

filterControls.addEventListener("click", (event) => {
  const filterButton = event.target.closest("[data-filter]");
  if (!filterButton) {
    return;
  }

  state.activeFilter = filterButton.dataset.filter;
  render();
});

statusBox.addEventListener("click", (event) => {
  const retryButton = event.target.closest("#retry-btn");
  if (!retryButton) {
    return;
  }

  loadBooks();
});

async function loadBooks() {
  state.isLoading = true;
  state.error = null;
  render();

  try {
    const booksFromServer = await fetchBooksFromServer();
    state.books = booksFromServer;
  } catch (error) {
    state.error = "Не удалось получить каталог. Проверьте подключение и попробуйте снова.";
  } finally {
    state.isLoading = false;
    render();
  }
}

loadBooks();
