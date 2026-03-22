const categories = [
  { id: "politics", label: "Политика" },
  { id: "technology", label: "Технологии" },
  { id: "society", label: "Общество" },
  { id: "sport", label: "Спорт" },
];

const newsByCategory = {
  politics: [
    {
      id: "pol-1",
      title: "Город утвердил новую программу поддержки малого бизнеса",
      content:
        "Муниципальные власти запустили трехлетний пакет льгот для предпринимателей: налоговые каникулы, гранты на цифровизацию и ускоренное согласование документов.",
      time: "18:30",
      top: true,
    },
    {
      id: "pol-2",
      title: "Обновлен порядок общественных обсуждений городских проектов",
      content:
        "Публичные консультации по крупным инфраструктурным инициативам теперь проходят в гибридном формате с обязательной публикацией итоговых протоколов.",
      time: "16:45",
      top: false,
    },
    {
      id: "pol-3",
      title: "В регионе стартует набор наблюдателей на местные выборы",
      content:
        "Общественные организации открыли онлайн-регистрацию волонтеров для мониторинга избирательных участков и подсчета голосов.",
      time: "14:10",
      top: false,
    },
  ],
  technology: [
    {
      id: "tech-1",
      title: "Технопарк представил платформу для анализа городского трафика",
      content:
        "Новая система агрегирует данные с камер и датчиков в реальном времени, чтобы снижать нагрузку на магистрали в часы пик.",
      time: "19:05",
      top: true,
    },
    {
      id: "tech-2",
      title: "Университет запустил лабораторию робототехники для студентов",
      content:
        "В лаборатории доступны промышленные манипуляторы и симуляторы для проектной работы по автоматизации производственных процессов.",
      time: "15:20",
      top: false,
    },
    {
      id: "tech-3",
      title: "IT-компании региона договорились о совместной стажировке",
      content:
        "Программа объединяет шесть работодателей и включает менторские треки по фронтенду, аналитике данных и кибербезопасности.",
      time: "12:50",
      top: false,
    },
  ],
  society: [
    {
      id: "soc-1",
      title: "В городе открыли круглосуточный центр семейной поддержки",
      content:
        "Центр оказывает психологическую и юридическую помощь, а также консультации по социальным выплатам в режиме одного окна.",
      time: "17:40",
      top: true,
    },
    {
      id: "soc-2",
      title: "Запущена волонтерская акция по благоустройству дворов",
      content:
        "Жители вместе с муниципальными службами проводят субботники, высаживают деревья и обновляют детские площадки.",
      time: "13:35",
      top: false,
    },
    {
      id: "soc-3",
      title: "Культурные центры проведут неделю открытых мастер-классов",
      content:
        "В программе лекции, мастерские по дизайну и бесплатные занятия для школьников и взрослых по вечерам.",
      time: "11:15",
      top: false,
    },
  ],
  sport: [
    {
      id: "sport-1",
      title: "Сборная региона вышла в финал национального турнира",
      content:
        "Команда одержала победу в полуфинале со счетом 3:1 и теперь готовится к решающему матчу на домашней арене.",
      time: "20:10",
      top: true,
    },
    {
      id: "sport-2",
      title: "Открыта запись на городской полумарафон",
      content:
        "Маршрут на 21 км пройдет через центральные улицы, а для новичков подготовлена отдельная дистанция на 5 км.",
      time: "15:55",
      top: false,
    },
    {
      id: "sport-3",
      title: "Детские секции получат новое оборудование к летнему сезону",
      content:
        "Спортивные школы обновляют инвентарь и расширяют расписание тренировок в рамках городской программы поддержки.",
      time: "10:40",
      top: false,
    },
  ],
};

const state = {
  activeCategory: "politics",
  activeNewsId: null,
};

const categorySwitch = document.getElementById("category-switch");
const newsList = document.getElementById("news-list");
const featuredCategory = document.getElementById("featured-category");
const featuredTitle = document.getElementById("featured-title");
const featuredContent = document.getElementById("featured-content");
const featuredTime = document.getElementById("featured-time");
const featuredPriority = document.getElementById("featured-priority");

function getCurrentCategoryNews() {
  return newsByCategory[state.activeCategory] ?? [];
}

function getCurrentCategoryLabel() {
  return (
    categories.find((category) => category.id === state.activeCategory)?.label ?? ""
  );
}

function getActiveNews() {
  const currentNews = getCurrentCategoryNews();
  return (
    currentNews.find((article) => article.id === state.activeNewsId) ?? currentNews[0]
  );
}

function renderCategorySwitch() {
  categorySwitch.innerHTML = categories
    .map((category) => {
      const isActive = category.id === state.activeCategory;
      return `
        <button
          type="button"
          class="category-btn ${isActive ? "category-btn--active" : ""}"
          data-category="${category.id}"
          role="tab"
          aria-selected="${isActive}"
        >
          ${category.label}
        </button>
      `;
    })
    .join("");
}

function renderFeaturedNews() {
  const activeNews = getActiveNews();
  const categoryLabel = getCurrentCategoryLabel();

  if (!activeNews) {
    featuredCategory.textContent = categoryLabel;
    featuredTitle.textContent = "В этой категории пока нет материалов";
    featuredContent.textContent = "";
    featuredTime.textContent = "";
    featuredPriority.textContent = "";
    featuredPriority.className = "";
    return;
  }

  featuredCategory.textContent = `Категория: ${categoryLabel}`;
  featuredTitle.textContent = activeNews.title;
  featuredContent.textContent = activeNews.content;

  featuredTime.className = "meta-pill";
  featuredTime.textContent = `Обновлено: ${activeNews.time}`;

  featuredPriority.className = activeNews.top
    ? "meta-pill meta-pill--top"
    : "meta-pill";
  featuredPriority.textContent = activeNews.top ? "Топ-новость" : "Стандарт";
}

function renderNewsList() {
  const currentNews = getCurrentCategoryNews();

  newsList.innerHTML = currentNews
    .map((article) => {
      const isActive = article.id === state.activeNewsId;
      return `
        <li class="news-item ${isActive ? "news-item--active" : ""} ${article.top ? "news-item--top" : ""}">
          <button type="button" class="news-item__button" data-news-id="${article.id}">
            <span class="news-item__title">${article.title}</span>
            <span class="news-item__excerpt">${article.content}</span>
            <span class="news-item__meta">
              <span class="badge">${article.time}</span>
              ${article.top ? '<span class="badge badge--top">TOP</span>' : ""}
            </span>
          </button>
        </li>
      `;
    })
    .join("");
}

function render() {
  const currentNews = getCurrentCategoryNews();
  if (!currentNews.find((article) => article.id === state.activeNewsId)) {
    state.activeNewsId = currentNews[0]?.id ?? null;
  }

  renderCategorySwitch();
  renderFeaturedNews();
  renderNewsList();
}

categorySwitch.addEventListener("click", (event) => {
  const button = event.target.closest("[data-category]");
  if (!button) {
    return;
  }

  state.activeCategory = button.dataset.category;
  state.activeNewsId = getCurrentCategoryNews()[0]?.id ?? null;
  render();
});

newsList.addEventListener("click", (event) => {
  const button = event.target.closest("[data-news-id]");
  if (!button) {
    return;
  }

  state.activeNewsId = button.dataset.newsId;
  render();
});

render();
