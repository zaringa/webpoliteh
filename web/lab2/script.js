const CATEGORIES = [
  {
    id: "politics",
    label: "Политика",
    heading: "Политическая повестка дня",
    description:
      "Следите за ключевыми решениями властей и изменениями в городской политике.",
  },
  {
    id: "technology",
    label: "Технологии",
    heading: "Технологические новости портала",
    description:
      "Обновления из мира IT, разработки и цифровой инфраструктуры региона.",
  },
  {
    id: "society",
    label: "Общество",
    heading: "Социальная повестка и городская среда",
    description:
      "События общественной жизни, инициативы жителей и новые сервисы для горожан.",
  },
  {
    id: "sport",
    label: "Спорт",
    heading: "Спортивная хроника",
    description:
      "Результаты соревнований, подготовка команд и анонсы городских спортивных событий.",
  },
];

const NEWS_BY_CATEGORY = {
  politics: [
    {
      id: "pol-1",
      title: "Городской совет утвердил программу цифровых госуслуг",
      content:
        "Принят новый пакет сервисов для жителей: подача заявлений, отслеживание статуса обращений и онлайн-оплата муниципальных услуг.",
      time: "09:20",
      top: true,
    },
    {
      id: "pol-2",
      title: "Обсуждение транспортной реформы вынесено на публичные слушания",
      content:
        "Жители могут оставить предложения по перераспределению маршрутов и обновлению схемы движения общественного транспорта.",
      time: "11:05",
      top: false,
    },
    {
      id: "pol-3",
      title: "Расширен список мер поддержки социальных НКО",
      content:
        "Финансирование проектов увеличено, а конкурсные процедуры для небольших организаций упрощены.",
      time: "13:40",
      top: false,
    },
  ],
  technology: [
    {
      id: "tech-1",
      title: "Технопарк открыл лабораторию прикладного ИИ",
      content:
        "Новая площадка поддерживает студенческие команды и стартапы, работающие с анализом больших данных и компьютерным зрением.",
      time: "10:10",
      top: true,
    },
    {
      id: "tech-2",
      title: "В кампусе запустили пилотную сеть умных датчиков",
      content:
        "Система в реальном времени контролирует освещение, энергопотребление и заполняемость учебных аудиторий.",
      time: "12:25",
      top: false,
    },
    {
      id: "tech-3",
      title: "IT-компании региона расширяют программу стажировок",
      content:
        "Увеличено число мест для начинающих разработчиков и аналитиков, добавлены наставнические треки.",
      time: "15:00",
      top: false,
    },
  ],
  society: [
    {
      id: "soc-1",
      title: "Открылся центр общественных инициатив в новом формате",
      content:
        "Центр объединяет волонтерские проекты, консультации и образовательные лекции для жителей разных возрастов.",
      time: "09:45",
      top: true,
    },
    {
      id: "soc-2",
      title: "Запущена городская акция по благоустройству дворов",
      content:
        "Команды волонтеров вместе с коммунальными службами обновляют детские зоны и общественные пространства.",
      time: "12:00",
      top: false,
    },
    {
      id: "soc-3",
      title: "Культурные площадки объявили неделю открытых мероприятий",
      content:
        "В программе бесплатные мастер-классы, встречи с экспертами и вечерние кинопоказы под открытым небом.",
      time: "16:20",
      top: false,
    },
  ],
  sport: [
    {
      id: "sport-1",
      title: "Сборная университета вышла в финал регионального чемпионата",
      content:
        "После уверенной победы в полуфинале команда готовится к решающему матчу на домашней арене.",
      time: "08:55",
      top: true,
    },
    {
      id: "sport-2",
      title: "Открыта регистрация на городской полумарафон",
      content:
        "Участникам доступны дистанции 5 и 21 км, а для новичков подготовлен отдельный тренировочный блок.",
      time: "11:30",
      top: false,
    },
    {
      id: "sport-3",
      title: "Спортивные секции получили новое тренировочное оборудование",
      content:
        "Муниципальная программа обновления инвентаря охватила 14 детско-юношеских клубов.",
      time: "14:50",
      top: false,
    },
  ],
};

const CategoryTabs = {
  name: "CategoryTabs",
  props: {
    categories: {
      type: Array,
      required: true,
    },
    modelValue: {
      type: String,
      required: true,
    },
  },
  emits: ["update:modelValue"],
  template: `
    <div class="flex flex-wrap gap-2" role="tablist" aria-label="Категории новостей">
      <button
        v-for="category in categories"
        :key="category.id"
        type="button"
        role="tab"
        :aria-selected="category.id === modelValue"
        @click="$emit('update:modelValue', category.id)"
        class="rounded-full border px-4 py-2 text-sm font-semibold transition focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-rose-300"
        :class="category.id === modelValue
          ? 'border-slate-900 bg-slate-900 text-white'
          : 'border-slate-300 bg-white text-slate-700 hover:border-slate-500'"
      >
        {{ category.label }}
      </button>
    </div>
  `,
};

const NewsList = {
  name: "NewsList",
  props: {
    articles: {
      type: Array,
      required: true,
    },
    activeId: {
      type: String,
      default: "",
    },
  },
  emits: ["select"],
  template: `
    <ul class="grid gap-3">
      <li
        v-for="(article, index) in articles"
        :key="article.id"
        class="list-reveal"
        :style="{ animationDelay: \`\${index * 80}ms\` }"
      >
        <button
          type="button"
          class="w-full rounded-2xl border p-4 text-left transition"
          @click="$emit('select', article.id)"
          :class="[
            article.id === activeId
              ? 'border-slate-900 bg-slate-900 text-slate-100 shadow-md'
              : 'border-slate-200 bg-white hover:border-slate-400',
            article.top && article.id !== activeId
              ? 'ring-1 ring-rose-300 bg-rose-50/50'
              : '',
          ]"
        >
          <div class="flex flex-wrap items-center justify-between gap-2">
            <h4 class="text-sm font-bold md:text-base">{{ article.title }}</h4>
            <span
              v-if="article.top"
              class="rounded-full px-2.5 py-1 text-[11px] font-bold uppercase tracking-[0.08em]"
              :class="article.id === activeId ? 'bg-rose-200 text-rose-800' : 'bg-rose-600 text-white'"
            >
              TOP
            </span>
          </div>

          <p
            class="mt-2 text-sm"
            :class="article.id === activeId ? 'text-slate-200' : 'text-slate-600'"
          >
            {{ article.content }}
          </p>

          <p
            class="mt-3 text-xs font-semibold uppercase tracking-[0.08em]"
            :class="article.id === activeId ? 'text-slate-300' : 'text-slate-500'"
          >
            Обновлено: {{ article.time }}
          </p>
        </button>
      </li>
    </ul>
  `,
};

const { createApp, computed, ref, watch } = Vue;

createApp({
  components: {
    CategoryTabs,
    NewsList,
  },
  setup() {
    const activeCategoryId = ref(CATEGORIES[0].id);
    const activeArticleId = ref("");

    const categoryNews = computed(() => NEWS_BY_CATEGORY[activeCategoryId.value] ?? []);

    const activeCategory = computed(() => {
      return (
        CATEGORIES.find((category) => category.id === activeCategoryId.value) ??
        CATEGORIES[0]
      );
    });

    function resolveDefaultArticleId(categoryId) {
      const newsList = NEWS_BY_CATEGORY[categoryId] ?? [];
      return newsList.find((article) => article.top)?.id ?? newsList[0]?.id ?? "";
    }

    watch(
      activeCategoryId,
      (nextCategoryId) => {
        activeArticleId.value = resolveDefaultArticleId(nextCategoryId);
      },
      { immediate: true },
    );

    const activeArticle = computed(() => {
      return (
        categoryNews.value.find((article) => article.id === activeArticleId.value) ??
        categoryNews.value[0] ??
        null
      );
    });

    const topNewsCount = computed(() => {
      return categoryNews.value.filter((article) => article.top).length;
    });

    function setCategory(nextCategoryId) {
      activeCategoryId.value = nextCategoryId;
    }

    function setActiveArticle(nextArticleId) {
      activeArticleId.value = nextArticleId;
    }

    return {
      categories: CATEGORIES,
      activeCategoryId,
      activeArticleId,
      categoryNews,
      activeCategory,
      activeArticle,
      topNewsCount,
      setCategory,
      setActiveArticle,
    };
  },
}).mount("#app");
