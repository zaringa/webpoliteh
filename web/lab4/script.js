const form = document.getElementById("comment-form");
const formError = document.getElementById("form-error");
const fillDemoButton = document.getElementById("fill-demo-btn");
const commentsList = document.getElementById("comments-list");
const commentsCount = document.getElementById("comments-count");
const loadStatus = document.getElementById("load-status");

const fields = {
  author: document.getElementById("author"),
  book: document.getElementById("book"),
  message: document.getElementById("message"),
  rating: document.getElementById("rating"),
  notify: document.getElementById("notify"),
};

const preview = {
  author: document.getElementById("preview-author"),
  book: document.getElementById("preview-book"),
  message: document.getElementById("preview-message"),
  rating: document.getElementById("preview-rating"),
  notify: document.getElementById("preview-notify"),
};

const initialFormState = {
  author: "",
  book: "",
  message: "",
  rating: "5",
  notify: true,
};

const state = {
  form: { ...initialFormState },
  comments: [],
  isLoading: false,
  error: null,
};

function escapeHtml(value) {
  return String(value)
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#39;");
}

function generateId() {
  if (typeof crypto !== "undefined" && typeof crypto.randomUUID === "function") {
    return crypto.randomUUID();
  }

  return `comment-${Date.now()}-${Math.floor(Math.random() * 10000)}`;
}

function fetchExistingComments() {
  const existingComments = [
    {
      id: "c-201",
      author: "Елена",
      book: "1984",
      message: "Очень сильная антиутопия. После чтения долго обдумывала сюжет.",
      rating: 5,
      notify: true,
      createdAt: "09:12",
    },
    {
      id: "c-202",
      author: "Максим",
      book: "Мастер и Маргарита",
      message: "Перечитываю второй раз, каждый раз открываю новые смыслы.",
      rating: 5,
      notify: false,
      createdAt: "10:26",
    },
    {
      id: "c-203",
      author: "Дарья",
      book: "Три товарища",
      message: "Теплая и грустная книга. Герои запоминаются надолго.",
      rating: 4,
      notify: true,
      createdAt: "11:03",
    },
  ];

  return new Promise((resolve) => {
    setTimeout(() => {
      resolve(existingComments);
    }, 1100);
  });
}

function formatCommentsCount(value) {
  if (value % 10 === 1 && value % 100 !== 11) {
    return `${value} комментарий`;
  }

  if ([2, 3, 4].includes(value % 10) && ![12, 13, 14].includes(value % 100)) {
    return `${value} комментария`;
  }

  return `${value} комментариев`;
}

function applyFormStateToInputs() {
  fields.author.value = state.form.author;
  fields.book.value = state.form.book;
  fields.message.value = state.form.message;
  fields.rating.value = state.form.rating;
  fields.notify.checked = state.form.notify;
}

function renderPreview() {
  preview.author.textContent = `Автор: ${state.form.author.trim() || "не указан"}`;
  preview.book.textContent = state.form.book.trim() || "Книга не указана";
  preview.message.textContent = state.form.message.trim() || "Комментарий пока пуст.";
  preview.rating.textContent = `Оценка: ${state.form.rating}/5`;

  if (state.form.notify) {
    preview.notify.className =
      "rounded-full border border-emerald-300 bg-emerald-50 px-3 py-1 text-xs font-semibold text-emerald-700";
    preview.notify.textContent = "Уведомления: включены";
    return;
  }

  preview.notify.className =
    "rounded-full border border-slate-300 bg-slate-50 px-3 py-1 text-xs font-semibold text-slate-600";
  preview.notify.textContent = "Уведомления: выключены";
}

function renderLoadStatus() {
  if (state.isLoading) {
    loadStatus.innerHTML = `
      <div class="flex items-center gap-3">
        <span class="inline-block h-4 w-4 animate-spin rounded-full border-2 border-forum-600 border-r-transparent"></span>
        <p class="font-medium">Загрузка существующих комментариев...</p>
      </div>
    `;
    return;
  }

  if (state.error) {
    loadStatus.innerHTML = `
      <div class="flex flex-wrap items-center justify-between gap-3">
        <p class="font-medium text-red-700">${state.error}</p>
        <button
          type="button"
          id="retry-load-btn"
          class="rounded-full border border-red-300 bg-white px-4 py-2 text-xs font-bold text-red-700 transition hover:border-red-500 hover:bg-red-50"
        >
          Повторить
        </button>
      </div>
    `;
    return;
  }

  loadStatus.innerHTML =
    '<p class="font-medium text-emerald-700">Комментарии успешно загружены и готовы к работе.</p>';
}

function renderComments() {
  commentsCount.textContent = formatCommentsCount(state.comments.length);

  if (state.comments.length === 0) {
    commentsList.innerHTML = `
      <li class="rounded-2xl border border-slate-200 bg-slate-50 p-4 text-sm text-slate-600">
        Комментариев пока нет. Опубликуйте первый отзыв.
      </li>
    `;
    return;
  }

  commentsList.innerHTML = state.comments
    .map((comment) => {
      const notifyClass = comment.notify
        ? "border-emerald-300 bg-emerald-50 text-emerald-700"
        : "border-slate-300 bg-slate-50 text-slate-600";
      const notifyText = comment.notify ? "Уведомления: включены" : "Уведомления: выключены";

      return `
        <li class="rounded-2xl border border-slate-200 bg-white p-4">
          <div class="flex flex-wrap items-center justify-between gap-2">
            <h3 class="text-base font-bold text-slate-900">${escapeHtml(comment.book)}</h3>
            <span class="rounded-full border border-amber-300 bg-amber-50 px-3 py-1 text-xs font-semibold text-amber-700">
              ${comment.rating}/5
            </span>
          </div>

          <p class="mt-2 text-sm leading-relaxed text-slate-700">${escapeHtml(comment.message)}</p>

          <div class="mt-3 flex flex-wrap items-center gap-2 text-xs">
            <span class="rounded-full border border-slate-300 bg-slate-50 px-3 py-1 font-semibold text-slate-600">
              Автор: ${escapeHtml(comment.author)}
            </span>
            <span class="rounded-full border ${notifyClass} px-3 py-1 font-semibold">
              ${notifyText}
            </span>
            <span class="rounded-full border border-slate-300 bg-white px-3 py-1 font-semibold text-slate-500">
              ${comment.createdAt}
            </span>
          </div>
        </li>
      `;
    })
    .join("");
}

function render() {
  applyFormStateToInputs();
  renderPreview();
  renderLoadStatus();
  renderComments();
}

function updateFormStateByField(fieldName, value) {
  state.form = {
    ...state.form,
    [fieldName]: value,
  };

  render();
}

function resetFormState() {
  state.form = { ...initialFormState };
  render();
}

function clearFormError() {
  formError.classList.add("hidden");
  formError.textContent = "";
}

function showFormError(message) {
  formError.classList.remove("hidden");
  formError.textContent = message;
}

function validateFormState() {
  if (!state.form.author.trim()) {
    return "Введите имя автора комментария.";
  }

  if (!state.form.book.trim()) {
    return "Введите название книги.";
  }

  if (!state.form.message.trim()) {
    return "Введите текст комментария.";
  }

  return null;
}

function createCommentFromState() {
  const createdAt = new Date().toLocaleTimeString("ru-RU", {
    hour: "2-digit",
    minute: "2-digit",
  });

  return {
    id: generateId(),
    author: state.form.author.trim(),
    book: state.form.book.trim(),
    message: state.form.message.trim(),
    rating: Number(state.form.rating),
    notify: state.form.notify,
    createdAt,
  };
}

async function loadComments() {
  state.isLoading = true;
  state.error = null;
  render();

  try {
    state.comments = await fetchExistingComments();
  } catch (error) {
    state.error = "Не удалось загрузить комментарии. Попробуйте еще раз.";
  } finally {
    state.isLoading = false;
    render();
  }
}

form.addEventListener("input", (event) => {
  const field = event.target;
  if (!(field instanceof HTMLInputElement || field instanceof HTMLTextAreaElement)) {
    return;
  }

  if (!field.name || !(field.name in state.form)) {
    return;
  }

  if (field.type === "checkbox") {
    updateFormStateByField(field.name, field.checked);
  } else {
    updateFormStateByField(field.name, field.value);
  }

  clearFormError();
});

form.addEventListener("change", (event) => {
  const field = event.target;
  if (!(field instanceof HTMLSelectElement) || field.name !== "rating") {
    return;
  }

  updateFormStateByField("rating", field.value);
  clearFormError();
});

form.addEventListener("submit", (event) => {
  event.preventDefault();

  const validationError = validateFormState();
  if (validationError) {
    showFormError(validationError);
    return;
  }

  const newComment = createCommentFromState();
  state.comments = [newComment, ...state.comments];
  resetFormState();
  clearFormError();
});

form.addEventListener("reset", () => {
  resetFormState();
  clearFormError();
});

fillDemoButton.addEventListener("click", () => {
  state.form = {
    author: "Никита",
    book: "Тонкое искусство пофигизма",
    message: "Легкий стиль, много спорных моментов, но читается быстро и дает пищу для размышлений.",
    rating: "4",
    notify: true,
  };

  render();
  clearFormError();
});

loadStatus.addEventListener("click", (event) => {
  const retryButton = event.target.closest("#retry-load-btn");
  if (!retryButton) {
    return;
  }

  loadComments();
});

render();
loadComments();
