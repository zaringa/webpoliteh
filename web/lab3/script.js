const FILTERS = [
  { id: 'all', label: 'Все книги' },
  { id: 'available', label: 'Только доступные' },
  { id: 'unavailable', label: 'Только выданные' },
];

const SERVER_BOOKS = [
  {
    id: 'book-101',
    title: 'Чистая архитектура',
    author: 'Роберт Мартин',
    genre: 'Разработка ПО',
    year: 2018,
    shelf: 'A-12',
    available: true,
    borrowedUntil: null,
  },
  {
    id: 'book-102',
    title: 'Грокаем алгоритмы',
    author: 'Адитья Бхаргава',
    genre: 'Алгоритмы',
    year: 2017,
    shelf: 'B-07',
    available: false,
    borrowedUntil: '03.05.2026',
  },
  {
    id: 'book-103',
    title: 'Паттерны объектно-ориентированного проектирования',
    author: 'Э. Гамма, Р. Хелм, Р. Джонсон, Д. Влиссидес',
    genre: 'Архитектура',
    year: 2020,
    shelf: 'A-03',
    available: true,
    borrowedUntil: null,
  },
  {
    id: 'book-104',
    title: 'Совершенный код',
    author: 'Стив Макконнелл',
    genre: 'Практика программирования',
    year: 2021,
    shelf: 'C-18',
    available: false,
    borrowedUntil: '29.04.2026',
  },
  {
    id: 'book-105',
    title: 'Refactoring',
    author: 'Martin Fowler',
    genre: 'Инженерные практики',
    year: 2019,
    shelf: 'B-01',
    available: true,
    borrowedUntil: null,
  },
  {
    id: 'book-106',
    title: 'Designing Data-Intensive Applications',
    author: 'Martin Kleppmann',
    genre: 'Системный дизайн',
    year: 2018,
    shelf: 'D-05',
    available: false,
    borrowedUntil: '06.05.2026',
  },
  {
    id: 'book-107',
    title: 'Computer Networking: A Top-Down Approach',
    author: 'James Kurose, Keith Ross',
    genre: 'Сети',
    year: 2021,
    shelf: 'C-02',
    available: true,
    borrowedUntil: null,
  },
  {
    id: 'book-108',
    title: 'Введение в теорию баз данных',
    author: 'К. Дж. Дейт',
    genre: 'Базы данных',
    year: 2022,
    shelf: 'A-19',
    available: true,
    borrowedUntil: null,
  },
];

function fetchBooksFromServer() {
  return new Promise((resolve) => {
    setTimeout(() => {
      const payload = SERVER_BOOKS.map((book) => ({ ...book }));
      resolve(payload);
    }, 1200);
  });
}

const LibraryStats = {
  name: 'LibraryStats',
  props: {
    total: {
      type: Number,
      required: true,
    },
    available: {
      type: Number,
      required: true,
    },
    unavailable: {
      type: Number,
      required: true,
    },
  },
  template: `
    <div class="grid gap-3 sm:grid-cols-3">
      <article class="rounded-2xl border border-slate-200 bg-slate-50/80 p-4">
        <p class="text-xs uppercase tracking-[0.1em] text-slate-500">Всего книг</p>
        <p class="mt-1 text-2xl font-extrabold text-slate-900">{{ total }}</p>
      </article>

      <article class="rounded-2xl border border-emerald-200 bg-emerald-50/80 p-4">
        <p class="text-xs uppercase tracking-[0.1em] text-emerald-700">Доступно</p>
        <p class="mt-1 text-2xl font-extrabold text-emerald-700">{{ available }}</p>
      </article>

      <article class="rounded-2xl border border-orange-200 bg-orange-50/80 p-4">
        <p class="text-xs uppercase tracking-[0.1em] text-orange-700">Выдано</p>
        <p class="mt-1 text-2xl font-extrabold text-orange-700">{{ unavailable }}</p>
      </article>
    </div>
  `,
};

const FilterTabs = {
  name: 'FilterTabs',
  props: {
    filters: {
      type: Array,
      required: true,
    },
    counts: {
      type: Object,
      required: true,
    },
    modelValue: {
      type: String,
      required: true,
    },
  },
  emits: ['update:modelValue'],
  template: `
    <div class="flex flex-wrap gap-2" role="tablist" aria-label="Фильтр книг">
      <button
        v-for="filter in filters"
        :key="filter.id"
        type="button"
        role="tab"
        class="inline-flex items-center gap-2 rounded-full border px-4 py-2 text-sm font-semibold transition focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-cyan-300"
        :class="filter.id === modelValue
          ? 'border-slate-900 bg-slate-900 text-white'
          : 'border-slate-300 bg-white text-slate-700 hover:border-slate-500'"
        :aria-selected="filter.id === modelValue"
        @click="$emit('update:modelValue', filter.id)"
      >
        <span>{{ filter.label }}</span>
        <span
          class="rounded-full px-2 py-0.5 text-xs"
          :class="filter.id === modelValue ? 'bg-white/20 text-white' : 'bg-slate-100 text-slate-600'"
        >
          {{ counts[filter.id] ?? 0 }}
        </span>
      </button>
    </div>
  `,
};

const BookGrid = {
  name: 'BookGrid',
  props: {
    books: {
      type: Array,
      required: true,
    },
  },
  template: `
    <ul class="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
      <li
        v-for="book in books"
        :key="book.id"
        class="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm transition hover:-translate-y-0.5 hover:shadow-md"
      >
        <div class="flex items-start justify-between gap-3">
          <p class="text-xs font-semibold uppercase tracking-[0.1em] text-slate-500">{{ book.genre }}</p>
          <span
            class="rounded-full border px-3 py-1 text-xs font-bold"
            :class="book.available
              ? 'border-emerald-200 bg-emerald-50 text-emerald-700'
              : 'border-orange-200 bg-orange-50 text-orange-700'"
          >
            {{ book.available ? 'Доступна' : 'Выдана' }}
          </span>
        </div>

        <h2 class="mt-3 text-lg font-bold leading-snug text-slate-900">{{ book.title }}</h2>
        <p class="mt-1 text-sm text-slate-600">{{ book.author }}</p>

        <div class="mt-4 grid grid-cols-2 gap-2 text-sm text-slate-600">
          <p class="rounded-lg bg-slate-50 px-3 py-2">Год: <span class="font-semibold text-slate-800">{{ book.year }}</span></p>
          <p class="rounded-lg bg-slate-50 px-3 py-2">Полка: <span class="font-semibold text-slate-800">{{ book.shelf }}</span></p>
        </div>

        <p
          v-if="book.available"
          class="mt-3 text-sm font-medium text-emerald-700"
        >
          Можно взять в читальном зале прямо сейчас.
        </p>
        <p
          v-else
          class="mt-3 text-sm font-medium text-orange-700"
        >
          Возврат ожидается: {{ book.borrowedUntil }}.
        </p>
      </li>
    </ul>
  `,
};

const { createApp, ref, computed, onMounted } = Vue;

createApp({
  components: {
    LibraryStats,
    FilterTabs,
    BookGrid,
  },
  setup() {
    const books = ref([]);
    const activeFilter = ref('all');
    const isLoading = ref(false);
    const errorMessage = ref('');

    const availableCount = computed(() => {
      return books.value.filter((book) => book.available).length;
    });

    const unavailableCount = computed(() => {
      return books.value.length - availableCount.value;
    });

    const filterCounts = computed(() => {
      return {
        all: books.value.length,
        available: availableCount.value,
        unavailable: unavailableCount.value,
      };
    });

    const filteredBooks = computed(() => {
      if (activeFilter.value === 'available') {
        return books.value.filter((book) => book.available);
      }

      if (activeFilter.value === 'unavailable') {
        return books.value.filter((book) => !book.available);
      }

      return books.value;
    });

    function setFilter(nextFilter) {
      activeFilter.value = nextFilter;
    }

    async function loadBooks() {
      isLoading.value = true;
      errorMessage.value = '';

      try {
        const serverBooks = await fetchBooksFromServer();
        books.value = serverBooks;
      } catch (error) {
        errorMessage.value =
          'Не удалось загрузить каталог. Проверьте подключение и попробуйте снова.';
      } finally {
        isLoading.value = false;
      }
    }

    onMounted(() => {
      loadBooks();
    });

    return {
      filters: FILTERS,
      books,
      activeFilter,
      isLoading,
      errorMessage,
      availableCount,
      unavailableCount,
      filterCounts,
      filteredBooks,
      setFilter,
      loadBooks,
    };
  },
}).mount('#app');
