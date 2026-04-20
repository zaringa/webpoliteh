const SUBJECTS = [
  { key: "math", label: "Математика" },
  { key: "programming", label: "Программирование" },
  { key: "design", label: "Дизайн" },
  { key: "databases", label: "Базы данных" },
];

const INITIAL_STUDENTS = [
  {
    id: "st-01",
    name: "Анна Ильина",
    group: "ИС-32",
    grades: { math: 5, programming: 5, design: 4, databases: 5 },
  },
  {
    id: "st-02",
    name: "Максим Рогов",
    group: "ИС-32",
    grades: { math: 4, programming: 5, design: 4, databases: 4 },
  },
  {
    id: "st-03",
    name: "Екатерина Смирнова",
    group: "ИС-32",
    grades: { math: 5, programming: 4, design: 5, databases: 5 },
  },
  {
    id: "st-04",
    name: "Дмитрий Волков",
    group: "ИС-32",
    grades: { math: 3, programming: 4, design: 4, databases: 3 },
  },
  {
    id: "st-05",
    name: "Софья Федорова",
    group: "ИС-32",
    grades: { math: 4, programming: 5, design: 5, databases: 4 },
  },
  {
    id: "st-06",
    name: "Артем Захаров",
    group: "ИС-32",
    grades: { math: 5, programming: 4, design: 3, databases: 4 },
  },
];

const elements = {
  tableBody: document.getElementById("score-table-body"),
  controls: document.getElementById("controls"),
  reorderStatus: document.getElementById("reorder-status"),
  studentsTotal: document.getElementById("students-total"),
  classAverage: document.getElementById("class-average"),
  topStudent: document.getElementById("top-student"),
};

const state = {
  students: INITIAL_STUDENTS.map((student) => ({
    ...student,
    grades: { ...student.grades },
  })),
  activeSort: "rating",
};

let previousPositions = new Map();
let previousOrder = new Map();

function averageScore(student) {
  const values = Object.values(student.grades);
  const sum = values.reduce((acc, value) => acc + value, 0);
  return sum / values.length;
}

function progressPercent(student) {
  return Math.round((averageScore(student) / 5) * 100);
}

function formatAverage(value) {
  return value.toFixed(2);
}

function getGradeTone(grade) {
  if (grade >= 5) {
    return "border-emerald-300/60 bg-emerald-500/20 text-emerald-100";
  }

  if (grade === 4) {
    return "border-cyan-300/60 bg-cyan-500/20 text-cyan-100";
  }

  if (grade === 3) {
    return "border-amber-300/60 bg-amber-500/20 text-amber-100";
  }

  return "border-rose-300/60 bg-rose-500/20 text-rose-100";
}

function randomizeOrder(list) {
  const copy = [...list];

  for (let i = copy.length - 1; i > 0; i -= 1) {
    const randomIndex = Math.floor(Math.random() * (i + 1));
    [copy[i], copy[randomIndex]] = [copy[randomIndex], copy[i]];
  }

  return copy;
}

function updateSummary() {
  const count = state.students.length;
  const classAverageValue =
    state.students.reduce((acc, student) => acc + averageScore(student), 0) / count;

  const top = [...state.students].sort((a, b) => averageScore(b) - averageScore(a))[0];

  elements.studentsTotal.textContent = String(count);
  elements.classAverage.textContent = formatAverage(classAverageValue);
  elements.topStudent.textContent = `${top.name} (${formatAverage(averageScore(top))})`;
}

function createGradeCell(student, subject) {
  const grade = student.grades[subject.key];
  const toneClass = getGradeTone(grade);

  return `
    <td class="px-4 py-3">
      <button
        type="button"
        class="grade-cell w-full rounded-xl border ${toneClass} px-3 py-2 text-sm font-extrabold tracking-wide transition hover:-translate-y-0.5 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-cyan-300"
        data-grade-cell
        aria-label="${subject.label}: ${grade}"
      >
        <span>${grade}</span>
      </button>
    </td>
  `;
}

function createProgressSvg(student) {
  const radius = 19;
  const circumference = 2 * Math.PI * radius;
  const percent = progressPercent(student);
  const offset = circumference * (1 - percent / 100);
  const gradientId = `progress-${student.id}`;

  return `
    <div class="flex items-center gap-3">
      <svg viewBox="0 0 52 52" class="h-12 w-12" aria-hidden="true">
        <defs>
          <linearGradient id="${gradientId}" x1="0%" y1="0%" x2="100%" y2="100%">
            <stop offset="0%" stop-color="#22d3ee"></stop>
            <stop offset="100%" stop-color="#34d399"></stop>
          </linearGradient>
        </defs>

        <circle cx="26" cy="26" r="${radius}" fill="none" stroke="#1e293b" stroke-width="6"></circle>
        <circle
          class="progress-ring"
          cx="26"
          cy="26"
          r="${radius}"
          fill="none"
          stroke="url(#${gradientId})"
          stroke-width="6"
          stroke-linecap="round"
          transform="rotate(-90 26 26)"
          data-final-offset="${offset}"
          style="stroke-dasharray:${circumference}; stroke-dashoffset:${circumference};"
        ></circle>
      </svg>

      <div>
        <p class="text-sm font-bold text-cyan-100">${percent}%</p>
        <p class="text-xs text-slate-400">успеваемость</p>
      </div>
    </div>
  `;
}

function animateProgressRings() {
  requestAnimationFrame(() => {
    elements.tableBody.querySelectorAll(".progress-ring").forEach((ring) => {
      ring.style.strokeDashoffset = ring.dataset.finalOffset;
    });
  });
}

function renderRows() {
  elements.tableBody.innerHTML = state.students
    .map((student, index) => {
      const avg = averageScore(student);

      return `
        <tr class="score-row" data-student-id="${student.id}">
          <td class="px-4 py-3">
            <span
              class="inline-flex h-9 w-9 items-center justify-center rounded-full border border-cyan-300/40 bg-cyan-500/15 text-sm font-extrabold text-cyan-100"
              data-place-badge
            >
              ${index + 1}
            </span>
          </td>

          <td class="px-4 py-3">
            <p class="text-sm font-bold text-slate-100">${student.name}</p>
            <p class="mt-1 text-xs text-slate-400">Группа ${student.group}</p>
          </td>

          ${SUBJECTS.map((subject) => createGradeCell(student, subject)).join("")}

          <td class="px-4 py-3">
            <span class="rounded-full border border-slate-700 bg-slate-800 px-3 py-1 text-sm font-extrabold text-slate-100">
              ${formatAverage(avg)}
            </span>
          </td>

          <td class="px-4 py-3">
            ${createProgressSvg(student)}
          </td>
        </tr>
      `;
    })
    .join("");
}

function captureSnapshot() {
  const rows = [...elements.tableBody.querySelectorAll("[data-student-id]")];

  previousPositions = new Map(
    rows.map((row) => [row.dataset.studentId, row.getBoundingClientRect().top]),
  );

  previousOrder = new Map(rows.map((row, index) => [row.dataset.studentId, index]));
}

function applyRowReorderAnimation() {
  const rows = [...elements.tableBody.querySelectorAll("[data-student-id]")];

  rows.forEach((row, newIndex) => {
    const studentId = row.dataset.studentId;
    const previousTop = previousPositions.get(studentId);
    const currentTop = row.getBoundingClientRect().top;

    if (typeof previousTop === "number") {
      const deltaY = previousTop - currentTop;

      if (Math.abs(deltaY) > 1) {
        row.animate(
          [
            { transform: `translateY(${deltaY}px)` },
            { transform: "translateY(0px)" },
          ],
          {
            duration: 560,
            easing: "cubic-bezier(0.22, 1, 0.36, 1)",
          },
        );
      }
    }

    const previousIndex = previousOrder.get(studentId);
    if (typeof previousIndex === "number" && previousIndex !== newIndex) {
      const badge = row.querySelector("[data-place-badge]");
      if (badge) {
        badge.animate(
          [
            { transform: "scale(1)", backgroundColor: "rgba(6, 182, 212, 0.15)" },
            { transform: "scale(1.18)", backgroundColor: "rgba(34, 211, 238, 0.35)" },
            { transform: "scale(1)", backgroundColor: "rgba(6, 182, 212, 0.15)" },
          ],
          {
            duration: 420,
            easing: "ease-out",
          },
        );
      }
    }
  });
}

function setActiveControlButton() {
  elements.controls.querySelectorAll("button[data-sort]").forEach((button) => {
    const isActive = button.dataset.sort === state.activeSort;

    if (isActive) {
      button.className =
        "rounded-full border border-cyan-300/50 bg-cyan-500/20 px-4 py-2 text-xs font-extrabold uppercase tracking-[0.1em] text-cyan-100";
      return;
    }

    button.className =
      "rounded-full border border-slate-700 bg-slate-800 px-4 py-2 text-xs font-extrabold uppercase tracking-[0.1em] text-slate-200 transition hover:border-slate-500";
  });
}

function updateStatusLabel() {
  const actionTextBySort = {
    rating: "по рейтингу",
    name: "по имени",
    shuffle: "случайная перестановка",
  };

  const now = new Date().toLocaleTimeString("ru-RU", {
    hour: "2-digit",
    minute: "2-digit",
    second: "2-digit",
  });

  elements.reorderStatus.textContent = `Текущий порядок: ${actionTextBySort[state.activeSort]}. Обновлено в ${now}.`;
}

function attachGradeInteractions() {
  elements.tableBody.querySelectorAll("[data-grade-cell]").forEach((cell) => {
    cell.addEventListener("pointermove", (event) => {
      const rect = cell.getBoundingClientRect();
      const x = ((event.clientX - rect.left) / rect.width) * 100;
      const y = ((event.clientY - rect.top) / rect.height) * 100;

      cell.style.setProperty("--spot-x", `${x}%`);
      cell.style.setProperty("--spot-y", `${y}%`);
    });

    cell.addEventListener("pointerleave", () => {
      cell.style.setProperty("--spot-x", "50%");
      cell.style.setProperty("--spot-y", "50%");
    });

    cell.addEventListener("click", () => {
      cell.animate(
        [
          { transform: "translateY(0) scale(1)" },
          { transform: "translateY(-2px) scale(1.1)" },
          { transform: "translateY(0) scale(1)" },
        ],
        {
          duration: 280,
          easing: "ease-out",
        },
      );
    });
  });
}

function render({ animateRows = false } = {}) {
  renderRows();
  animateProgressRings();

  if (animateRows) {
    applyRowReorderAnimation();
  }

  captureSnapshot();
  attachGradeInteractions();
  updateSummary();
  setActiveControlButton();
  updateStatusLabel();
}

function sortByRating(students) {
  return [...students].sort((left, right) => {
    const diff = averageScore(right) - averageScore(left);
    if (Math.abs(diff) > 0.001) {
      return diff;
    }

    return left.name.localeCompare(right.name, "ru");
  });
}

function sortByName(students) {
  return [...students].sort((left, right) => left.name.localeCompare(right.name, "ru"));
}

function applySort(mode) {
  if (!["rating", "name", "shuffle"].includes(mode)) {
    return;
  }

  captureSnapshot();

  if (mode === "rating") {
    state.students = sortByRating(state.students);
  }

  if (mode === "name") {
    state.students = sortByName(state.students);
  }

  if (mode === "shuffle") {
    state.students = randomizeOrder(state.students);
  }

  state.activeSort = mode;
  render({ animateRows: true });
}

elements.controls.addEventListener("click", (event) => {
  const button = event.target.closest("button[data-sort]");
  if (!button) {
    return;
  }

  applySort(button.dataset.sort);
});

state.students = sortByRating(state.students);
render();
