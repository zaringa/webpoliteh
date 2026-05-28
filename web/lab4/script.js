const INITIAL_FORM = {
  author: '',
  book: '',
  message: '',
  rating: '5',
  notify: true,
};

function fetchExistingComments() {
  const existingComments = [
    {
      id: 'c-201',
      author: 'Елена',
      book: '1984',
      message: 'Очень сильная антиутопия. После чтения долго обдумывала сюжет.',
      rating: 5,
      notify: true,
      createdAt: '09:12',
    },
    {
      id: 'c-202',
      author: 'Максим',
      book: 'Мастер и Маргарита',
      message: 'Перечитываю второй раз, каждый раз открываю новые смыслы.',
      rating: 5,
      notify: false,
      createdAt: '10:26',
    },
    {
      id: 'c-203',
      author: 'Дарья',
      book: 'Три товарища',
      message: 'Теплая и грустная книга. Герои запоминаются надолго.',
      rating: 4,
      notify: true,
      createdAt: '11:03',
    },
  ];

  return new Promise((resolve) => {
    setTimeout(() => {
      resolve(existingComments);
    }, 1000);
  });
}

function generateId() {
  if (typeof crypto !== 'undefined' && typeof crypto.randomUUID === 'function') {
    return crypto.randomUUID();
  }

  return `comment-${Date.now()}-${Math.floor(Math.random() * 10000)}`;
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

function currentTime() {
  return new Date().toLocaleTimeString('ru-RU', {
    hour: '2-digit',
    minute: '2-digit',
  });
}

const { createApp, reactive, ref, computed, onMounted } = Vue;

createApp({
  setup() {
    const form = reactive({ ...INITIAL_FORM });
    const comments = ref([]);
    const isLoading = ref(false);
    const loadError = ref('');
    const formError = ref('');

    const previewAuthor = computed(() => form.author || 'не указан');
    const previewBook = computed(() => form.book || 'Книга не указана');
    const previewMessage = computed(() => form.message || 'Комментарий пока пуст.');
    const commentsCountLabel = computed(() => formatCommentsCount(comments.value.length));

    function validateForm() {
      if (!form.author) {
        return 'Введите имя автора комментария.';
      }

      if (!form.book) {
        return 'Введите название книги.';
      }

      if (!form.message) {
        return 'Введите текст комментария.';
      }

      return '';
    }

    function resetForm() {
      Object.assign(form, INITIAL_FORM);
      formError.value = '';
    }

    function fillDemo() {
      Object.assign(form, {
        author: 'Никита',
        book: 'Тонкое искусство пофигизма',
        message: 'Легкий стиль и спорные мысли, но читать интересно.',
        rating: '4',
        notify: true,
      });

      formError.value = '';
    }

    function submitComment() {
      const validationError = validateForm();
      if (validationError) {
        formError.value = validationError;
        return;
      }

      comments.value.unshift({
        id: generateId(),
        author: form.author,
        book: form.book,
        message: form.message,
        rating: Number(form.rating),
        notify: form.notify,
        createdAt: currentTime(),
      });

      resetForm();
    }

    async function loadComments() {
      isLoading.value = true;
      loadError.value = '';

      try {
        comments.value = await fetchExistingComments();
      } catch (error) {
        loadError.value = 'Не удалось загрузить комментарии. Попробуйте снова.';
      } finally {
        isLoading.value = false;
      }
    }

    onMounted(() => {
      loadComments();
    });

    return {
      form,
      comments,
      isLoading,
      loadError,
      formError,
      previewAuthor,
      previewBook,
      previewMessage,
      commentsCountLabel,
      submitComment,
      resetForm,
      fillDemo,
      loadComments,
    };
  },
}).mount('#app');
