const SUBJECTS = [
  { key: 'math', label: 'Математика' },
  { key: 'programming', label: 'Программирование' },
  { key: 'design', label: 'Дизайн' },
  { key: 'databases', label: 'Базы данных' },
];

const INITIAL_STUDENTS = [
  {
    id: 'st-01',
    name: 'Анна Ильина',
    group: 'ИС-32',
    grades: { math: 5, programming: 5, design: 4, databases: 5 },
  },
  {
    id: 'st-02',
    name: 'Максим Рогов',
    group: 'ИС-32',
    grades: { math: 4, programming: 5, design: 4, databases: 4 },
  },
  {
    id: 'st-03',
    name: 'Екатерина Смирнова',
    group: 'ИС-32',
    grades: { math: 5, programming: 4, design: 5, databases: 5 },
  },
  {
    id: 'st-04',
    name: 'Дмитрий Волков',
    group: 'ИС-32',
    grades: { math: 3, programming: 4, design: 4, databases: 3 },
  },
  {
    id: 'st-05',
    name: 'Софья Федорова',
    group: 'ИС-32',
    grades: { math: 4, programming: 5, design: 5, databases: 4 },
  },
  {
    id: 'st-06',
    name: 'Артем Захаров',
    group: 'ИС-32',
    grades: { math: 5, programming: 4, design: 3, databases: 4 },
  },
];

function cloneStudents(students) {
  return students.map((student) => ({
    ...student,
    grades: { ...student.grades },
  }));
}

function averageScore(student) {
  const values = Object.values(student.grades);
  const sum = values.reduce((acc, value) => acc + value, 0);
  return sum / values.length;
}

function formatAverage(value) {
  return value.toFixed(2);
}

function progressPercent(student) {
  return Math.round((averageScore(student) / 5) * 100);
}

function sortByRating(students) {
  return [...students].sort((left, right) => {
    const diff = averageScore(right) - averageScore(left);
    if (Math.abs(diff) > 0.001) {
      return diff;
    }

    return left.name.localeCompare(right.name, 'ru');
  });
}

function sortByName(students) {
  return [...students].sort((left, right) => left.name.localeCompare(right.name, 'ru'));
}

function randomizeOrder(students) {
  const copy = [...students];

  for (let i = copy.length - 1; i > 0; i -= 1) {
    const randomIndex = Math.floor(Math.random() * (i + 1));
    [copy[i], copy[randomIndex]] = [copy[randomIndex], copy[i]];
  }

  return copy;
}

const SummaryCards = {
  name: 'SummaryCards',
  props: {
    studentsTotal: {
      type: Number,
      required: true,
    },
    classAverage: {
      type: String,
      required: true,
    },
    topStudent: {
      type: String,
      required: true,
    },
  },
  template: `
    <div class="grid gap-3 sm:grid-cols-3">
      <article class="rounded-2xl border border-slate-800 bg-slate-900/80 p-4">
        <p class="text-xs uppercase tracking-[0.1em] text-slate-400">Студентов</p>
        <p class="mt-1 text-2xl font-extrabold text-cyan-200">{{ studentsTotal }}</p>
      </article>
      <article class="rounded-2xl border border-slate-800 bg-slate-900/80 p-4">
        <p class="text-xs uppercase tracking-[0.1em] text-slate-400">Средний балл группы</p>
        <p class="mt-1 text-2xl font-extrabold text-emerald-200">{{ classAverage }}</p>
      </article>
      <article class="rounded-2xl border border-slate-800 bg-slate-900/80 p-4">
        <p class="text-xs uppercase tracking-[0.1em] text-slate-400">Лучший результат</p>
        <p class="mt-1 text-base font-bold text-amber-200">{{ topStudent }}</p>
      </article>
    </div>
  `,
};

const SortControls = {
  name: 'SortControls',
  props: {
    activeSort: {
      type: String,
      required: true,
    },
  },
  emits: ['change-sort'],
  template: `
    <div class="flex flex-wrap gap-2">
      <button
        type="button"
        class="rounded-full border px-4 py-2 text-xs font-extrabold uppercase tracking-[0.1em] transition"
        :class="activeSort === 'rating'
          ? 'border-cyan-300/50 bg-cyan-500/20 text-cyan-100'
          : 'border-slate-700 bg-slate-800 text-slate-200 hover:border-slate-500'"
        @click="$emit('change-sort', 'rating')"
      >
        По рейтингу
      </button>

      <button
        type="button"
        class="rounded-full border px-4 py-2 text-xs font-extrabold uppercase tracking-[0.1em] transition"
        :class="activeSort === 'name'
          ? 'border-cyan-300/50 bg-cyan-500/20 text-cyan-100'
          : 'border-slate-700 bg-slate-800 text-slate-200 hover:border-slate-500'"
        @click="$emit('change-sort', 'name')"
      >
        По имени
      </button>

      <button
        type="button"
        class="rounded-full border px-4 py-2 text-xs font-extrabold uppercase tracking-[0.1em] transition"
        :class="activeSort === 'shuffle'
          ? 'border-cyan-300/50 bg-cyan-500/20 text-cyan-100'
          : 'border-slate-700 bg-slate-800 text-slate-200 hover:border-slate-500'"
        @click="$emit('change-sort', 'shuffle')"
      >
        Случайная перестановка
      </button>
    </div>
  `,
};

const ScoreTable = {
  name: 'ScoreTable',
  props: {
    students: {
      type: Array,
      required: true,
    },
    subjects: {
      type: Array,
      required: true,
    },
    animateRings: {
      type: Boolean,
      required: true,
    },
  },
  methods: {
    averageScore,
    formatAverage,
    progressPercent,
    getGradeTone(grade) {
      if (grade >= 5) {
        return 'border-emerald-300/60 bg-emerald-500/20 text-emerald-100';
      }

      if (grade === 4) {
        return 'border-cyan-300/60 bg-cyan-500/20 text-cyan-100';
      }

      if (grade === 3) {
        return 'border-amber-300/60 bg-amber-500/20 text-amber-100';
      }

      return 'border-rose-300/60 bg-rose-500/20 text-rose-100';
    },
    ringGeometry(student) {
      const radius = 19;
      const circumference = 2 * Math.PI * radius;
      const offset = circumference * (1 - this.progressPercent(student) / 100);

      return {
        radius,
        circumference,
        offset,
      };
    },
    onCellMove(event) {
      const cell = event.currentTarget;
      const rect = cell.getBoundingClientRect();
      const x = ((event.clientX - rect.left) / rect.width) * 100;
      const y = ((event.clientY - rect.top) / rect.height) * 100;

      cell.style.setProperty('--spot-x', `${x}%`);
      cell.style.setProperty('--spot-y', `${y}%`);
    },
    onCellLeave(event) {
      const cell = event.currentTarget;
      cell.style.setProperty('--spot-x', '50%');
      cell.style.setProperty('--spot-y', '50%');
    },
    onCellClick(event) {
      const cell = event.currentTarget;
      cell.animate(
        [
          { transform: 'translateY(0) scale(1)' },
          { transform: 'translateY(-2px) scale(1.1)' },
          { transform: 'translateY(0) scale(1)' },
        ],
        {
          duration: 280,
          easing: 'ease-out',
        },
      );
    },
  },
  template: `
    <div class="overflow-x-auto rounded-2xl border border-slate-800">
      <table class="min-w-[1020px] w-full border-collapse">
        <thead class="bg-slate-900">
          <tr class="text-left text-xs uppercase tracking-[0.1em] text-slate-400">
            <th class="px-4 py-3">Место</th>
            <th class="px-4 py-3">Студент</th>
            <th v-for="subject in subjects" :key="subject.key" class="px-4 py-3">{{ subject.label }}</th>
            <th class="px-4 py-3">Средний балл</th>
            <th class="px-4 py-3">Прогресс</th>
          </tr>
        </thead>

        <transition-group
          tag="tbody"
          name="row"
          class="divide-y divide-slate-800 bg-slate-900/70"
        >
          <tr
            v-for="(student, index) in students"
            :key="student.id"
            class="score-row"
          >
            <td class="px-4 py-3">
              <span class="inline-flex h-9 w-9 items-center justify-center rounded-full border border-cyan-300/40 bg-cyan-500/15 text-sm font-extrabold text-cyan-100">
                {{ index + 1 }}
              </span>
            </td>

            <td class="px-4 py-3">
              <p class="text-sm font-bold text-slate-100">{{ student.name }}</p>
              <p class="mt-1 text-xs text-slate-400">Группа {{ student.group }}</p>
            </td>

            <td v-for="subject in subjects" :key="subject.key" class="px-4 py-3">
              <button
                type="button"
                class="grade-cell w-full rounded-xl border px-3 py-2 text-sm font-extrabold tracking-wide transition hover:-translate-y-0.5 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-cyan-300"
                :class="getGradeTone(student.grades[subject.key])"
                @pointermove="onCellMove"
                @pointerleave="onCellLeave"
                @click="onCellClick"
              >
                <span>{{ student.grades[subject.key] }}</span>
              </button>
            </td>

            <td class="px-4 py-3">
              <span class="rounded-full border border-slate-700 bg-slate-800 px-3 py-1 text-sm font-extrabold text-slate-100">
                {{ formatAverage(averageScore(student)) }}
              </span>
            </td>

            <td class="px-4 py-3">
              <div class="flex items-center gap-3">
                <svg viewBox="0 0 52 52" class="h-12 w-12" aria-hidden="true">
                  <defs>
                    <linearGradient :id="'progress-' + student.id" x1="0%" y1="0%" x2="100%" y2="100%">
                      <stop offset="0%" stop-color="#22d3ee"></stop>
                      <stop offset="100%" stop-color="#34d399"></stop>
                    </linearGradient>
                  </defs>

                  <circle
                    cx="26"
                    cy="26"
                    :r="ringGeometry(student).radius"
                    fill="none"
                    stroke="#1e293b"
                    stroke-width="6"
                  ></circle>

                  <circle
                    class="progress-ring"
                    cx="26"
                    cy="26"
                    :r="ringGeometry(student).radius"
                    fill="none"
                    :stroke="'url(#progress-' + student.id + ')'"
                    stroke-width="6"
                    stroke-linecap="round"
                    transform="rotate(-90 26 26)"
                    :stroke-dasharray="ringGeometry(student).circumference"
                    :stroke-dashoffset="animateRings ? ringGeometry(student).offset : ringGeometry(student).circumference"
                  ></circle>
                </svg>

                <div>
                  <p class="text-sm font-bold text-cyan-100">{{ progressPercent(student) }}%</p>
                  <p class="text-xs text-slate-400">успеваемость</p>
                </div>
              </div>
            </td>
          </tr>
        </transition-group>
      </table>
    </div>
  `,
};

const { createApp, ref, computed, nextTick, onMounted } = Vue;

createApp({
  components: {
    SummaryCards,
    SortControls,
    ScoreTable,
  },
  setup() {
    const students = ref(cloneStudents(INITIAL_STUDENTS));
    const activeSort = ref('rating');
    const reorderStatus = ref('Текущий порядок: по рейтингу.');
    const animateRings = ref(false);

    const studentsTotal = computed(() => students.value.length);

    const classAverageText = computed(() => {
      if (students.value.length === 0) {
        return '0.00';
      }

      const total = students.value.reduce((acc, student) => acc + averageScore(student), 0);
      return formatAverage(total / students.value.length);
    });

    const topStudentLabel = computed(() => {
      if (students.value.length === 0) {
        return '-';
      }

      const top = [...students.value].sort((a, b) => averageScore(b) - averageScore(a))[0];
      return `${top.name} (${formatAverage(averageScore(top))})`;
    });

    function updateStatusLabel() {
      const actionTextBySort = {
        rating: 'по рейтингу',
        name: 'по имени',
        shuffle: 'случайная перестановка',
      };

      const now = new Date().toLocaleTimeString('ru-RU', {
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit',
      });

      reorderStatus.value = `Текущий порядок: ${actionTextBySort[activeSort.value]}. Обновлено в ${now}.`;
    }

    function replayRings() {
      animateRings.value = false;

      nextTick(() => {
        animateRings.value = true;
      });
    }

    function applySort(mode) {
      if (!['rating', 'name', 'shuffle'].includes(mode)) {
        return;
      }

      if (mode === 'rating') {
        students.value = sortByRating(students.value);
      }

      if (mode === 'name') {
        students.value = sortByName(students.value);
      }

      if (mode === 'shuffle') {
        students.value = randomizeOrder(students.value);
      }

      activeSort.value = mode;
      updateStatusLabel();
      replayRings();
    }

    onMounted(() => {
      students.value = sortByRating(students.value);
      activeSort.value = 'rating';
      updateStatusLabel();
      replayRings();
    });

    return {
      subjects: SUBJECTS,
      students,
      activeSort,
      reorderStatus,
      animateRings,
      studentsTotal,
      classAverageText,
      topStudentLabel,
      applySort,
    };
  },
}).mount('#app');
