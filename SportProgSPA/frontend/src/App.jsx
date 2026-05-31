
import { useCallback, useEffect, useMemo, useState } from "react";
import axios from "axios";
import {
  BrowserRouter,
  Link,
  NavLink,
  Route,
  Routes,
  useNavigate,
  useParams,
  useSearchParams,
} from "react-router-dom";

const AUTH_KEY = "sportprog_auth";

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || "/api",
});

api.interceptors.request.use((config) => {
  const saved = localStorage.getItem(AUTH_KEY);
  if (saved) {
    try {
      const auth = JSON.parse(saved);
      if (auth.token) {
        config.headers.Authorization = `Bearer ${auth.token}`;
      }
    } catch {
      localStorage.removeItem(AUTH_KEY);
    }
  }
  return config;
});

const navItems = [
  { to: "/", label: "Главная" },
  { to: "/algorithms", label: "Алгоритмы" },
  { to: "/tasks", label: "Задачи" },
  { to: "/collections", label: "Подборки" },
  { to: "/rating", label: "Рейтинг" },
  { to: "/favorites", label: "Избранное" },
  { to: "/profile", label: "Профиль" },
];

const difficulties = ["Легко", "Средне", "Сложно"];
const categories = ["Основы", "Графы", "DP", "Структуры данных", "Строки"];
const languages = ["C++17", "Python 3", "Java 17", "C# 12"];

function loadAuth() {
  try {
    return JSON.parse(localStorage.getItem(AUTH_KEY)) || { token: "", user: null };
  } catch {
    return { token: "", user: null };
  }
}

function getErrorMessage(error) {
  return error?.response?.data?.message || error?.message || "Не удалось выполнить запрос.";
}

function buildQuery(base, values) {
  const params = new URLSearchParams();
  Object.entries(values).forEach(([key, value]) => {
    if (value) {
      params.set(key, value);
    }
  });
  const query = params.toString();
  return query ? `${base}?${query}` : base;
}

function useRemote(url, initialValue) {
  const [data, setData] = useState(initialValue);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [reloadKey, setReloadKey] = useState(0);

  const reload = useCallback(() => setReloadKey((value) => value + 1), []);

  useEffect(() => {
    let active = true;
    setLoading(true);
    setError("");

    api
      .get(url)
      .then((response) => {
        if (active) {
          setData(response.data);
        }
      })
      .catch((requestError) => {
        if (active) {
          setError(getErrorMessage(requestError));
        }
      })
      .finally(() => {
        if (active) {
          setLoading(false);
        }
      });

    return () => {
      active = false;
    };
  }, [url, reloadKey]);

  return { data, setData, loading, error, reload };
}

function App() {
  const [auth, setAuth] = useState(loadAuth);

  const applyAuth = useCallback((payload) => {
    localStorage.setItem(AUTH_KEY, JSON.stringify(payload));
    setAuth(payload);
  }, []);

  const logout = useCallback(() => {
    localStorage.removeItem(AUTH_KEY);
    setAuth({ token: "", user: null });
  }, []);

  const patchUser = useCallback((updates) => {
    setAuth((current) => {
      if (!current.user) {
        return current;
      }
      const next = { ...current, user: { ...current.user, ...updates } };
      localStorage.setItem(AUTH_KEY, JSON.stringify(next));
      return next;
    });
  }, []);

  return (
    <BrowserRouter>
      <Layout auth={auth} onLogout={logout}>
        <Routes>
          <Route path="/" element={<HomePage auth={auth} />} />
          <Route path="/algorithms" element={<AlgorithmsPage auth={auth} />} />
          <Route path="/algorithms/:slug" element={<AlgorithmDetailPage auth={auth} />} />
          <Route path="/tasks" element={<TasksPage auth={auth} onUserPatch={patchUser} />} />
          <Route path="/collections" element={<CollectionsPage />} />
          <Route path="/collections/:slug" element={<CollectionDetailPage />} />
          <Route path="/rating" element={<RatingPage />} />
          <Route path="/favorites" element={<FavoritesPage auth={auth} />} />
          <Route path="/profile" element={<ProfilePage auth={auth} onUserPatch={patchUser} />} />
          <Route path="/auth" element={<AuthPage onAuth={applyAuth} />} />
          <Route path="*" element={<NotFoundPage />} />
        </Routes>
      </Layout>
    </BrowserRouter>
  );
}

function Layout({ auth, onLogout, children }) {
  return (
    <div className="app-shell">
      <header className="topbar">
        <Link className="brand" to="/">
          <span className="brand-mark">SP</span>
          <span>
            <strong>SportProg</strong>
            <small>алгоритмы и задачи</small>
          </span>
        </Link>

        <nav className="main-nav" aria-label="Основная навигация">
          {navItems.map((item) => (
            <NavLink key={item.to} to={item.to} className={({ isActive }) => (isActive ? "active" : "")}>
              {item.label}
            </NavLink>
          ))}
        </nav>

        <div className="account-box">
          {auth.user ? (
            <>
              <Link className="user-chip" to="/profile">
                <span className="avatar" style={{ backgroundColor: auth.user.avatarColor }}>
                  {auth.user.name?.slice(0, 1) || "U"}
                </span>
                <span>{auth.user.rating}</span>
              </Link>
              <button className="ghost-button" type="button" onClick={onLogout}>
                Выйти
              </button>
            </>
          ) : (
            <Link className="primary-button" to="/auth">
              Войти
            </Link>
          )}
        </div>
      </header>

      <main className="page-wrap">{children}</main>
      <footer className="footer">SPA-проект: каталог алгоритмов, задач, подборок и пользовательского прогресса.</footer>
    </div>
  );
}

function HomePage({ auth }) {
  const { data: algorithms, loading: algorithmsLoading } = useRemote("/algorithms", []);
  const { data: tasks, loading: tasksLoading } = useRemote("/tasks", []);
  const { data: rating } = useRemote("/profile/rating", []);

  const topTopics = algorithms.slice(0, 4);
  const acceptedAverage = tasks.length
    ? Math.round(tasks.reduce((sum, task) => sum + task.acceptanceRate, 0) / tasks.length)
    : 0;

  return (
    <div className="stack">
      <section className="workspace-hero">
        <div className="hero-copy">
          <p className="eyebrow">Рабочий стол</p>
          <h1>Каталог спортивного программирования</h1>
          <p>
            Темы, тренировочные задачи и подборки собраны в одном SPA: можно изучать алгоритмы, сохранять избранное,
            отправлять решения и смотреть рейтинг участников.
          </p>
          <div className="hero-actions">
            <Link className="primary-button" to="/algorithms">
              Открыть алгоритмы
            </Link>
            <Link className="secondary-button" to="/tasks">
              Решать задачи
            </Link>
          </div>
        </div>
        <AlgorithmMap />
      </section>

      <section className="stats-grid" aria-label="Статистика проекта">
        <StatCard title="Темы" value={algorithmsLoading ? "..." : algorithms.length} hint="в каталоге" accent="blue" />
        <StatCard title="Задачи" value={tasksLoading ? "..." : tasks.length} hint="с примерами" accent="green" />
        <StatCard title="Средний AC" value={`${acceptedAverage}%`} hint="по базе задач" accent="amber" />
        <StatCard title="Участники" value={rating.length} hint="в рейтинге" accent="red" />
      </section>

      <section className="split-layout">
        <div>
          <div className="section-head">
            <div>
              <p className="eyebrow">Популярное</p>
              <h2>Темы для старта</h2>
            </div>
            <Link to="/algorithms">Все темы</Link>
          </div>
          <div className="topic-list">
            {topTopics.map((topic) => (
              <TopicRow key={topic.id} topic={topic} />
            ))}
          </div>
        </div>

        <div className="progress-panel">
          <p className="eyebrow">Аккаунт</p>
          {auth.user ? (
            <>
              <h2>{auth.user.name}</h2>
              <div className="profile-metrics">
                <Metric label="Рейтинг" value={auth.user.rating} />
                <Metric label="Решено" value={auth.user.solvedCount} />
              </div>
              <Link className="secondary-button full-width" to="/profile">
                Перейти в профиль
              </Link>
            </>
          ) : (
            <>
              <h2>Сохраняйте прогресс</h2>
              <p>После входа доступны избранные темы, профиль и отправка решений на проверку.</p>
              <Link className="primary-button full-width" to="/auth">
                Создать аккаунт
              </Link>
            </>
          )}
        </div>
      </section>
    </div>
  );
}

function AlgorithmMap() {
  const nodes = ["Big O", "BFS", "DP", "DSU", "KMP"];
  return (
    <div className="algorithm-map" aria-label="Карта тем">
      <div className="map-line line-one" />
      <div className="map-line line-two" />
      {nodes.map((node, index) => (
        <span key={node} className={`map-node node-${index + 1}`}>
          {node}
        </span>
      ))}
    </div>
  );
}

function StatCard({ title, value, hint, accent }) {
  return (
    <article className={`stat-card ${accent}`}>
      <span>{title}</span>
      <strong>{value}</strong>
      <small>{hint}</small>
    </article>
  );
}

function AlgorithmsPage({ auth }) {
  const navigate = useNavigate();
  const [search, setSearch] = useState("");
  const [difficulty, setDifficulty] = useState("");
  const [category, setCategory] = useState("");
  const [tag, setTag] = useState("");

  const url = useMemo(
    () => buildQuery("/algorithms", { search, difficulty, category, tag }),
    [search, difficulty, category, tag]
  );
  const { data: algorithms, setData, loading, error } = useRemote(url, []);

  const toggleFavorite = async (topic) => {
    if (!auth.user) {
      navigate("/auth");
      return;
    }

    if (topic.isFavorite) {
      await api.delete(`/favorites/${topic.id}`);
    } else {
      await api.post(`/favorites/${topic.id}`);
    }

    setData((items) =>
      items.map((item) => (item.id === topic.id ? { ...item, isFavorite: !item.isFavorite } : item))
    );
  };

  return (
    <div className="stack">
      <PageTitle
        eyebrow="Каталог"
        title="Алгоритмы"
        text="Фильтруйте темы как в медиакаталоге: по сложности, разделу и тегам."
      />

      <FilterBar>
        <input value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Поиск по теме или тегу" />
        <Select value={difficulty} onChange={setDifficulty} placeholder="Любая сложность" options={difficulties} />
        <Select value={category} onChange={setCategory} placeholder="Все разделы" options={categories} />
        <input value={tag} onChange={(event) => setTag(event.target.value)} placeholder="Тег: графы, dp, xor" />
      </FilterBar>

      <RemoteState loading={loading} error={error} empty={!algorithms.length} emptyText="Темы не найдены.">
        <div className="cards-grid">
          {algorithms.map((topic) => (
            <TopicCard key={topic.id} topic={topic} onFavorite={() => toggleFavorite(topic)} />
          ))}
        </div>
      </RemoteState>
    </div>
  );
}

function AlgorithmDetailPage({ auth }) {
  const { slug } = useParams();
  const navigate = useNavigate();
  const { data: topic, setData, loading, error } = useRemote(`/algorithms/${slug}`, null);

  const toggleFavorite = async () => {
    if (!auth.user) {
      navigate("/auth");
      return;
    }

    if (topic.isFavorite) {
      await api.delete(`/favorites/${topic.id}`);
    } else {
      await api.post(`/favorites/${topic.id}`);
    }

    setData({ ...topic, isFavorite: !topic.isFavorite });
  };

  return (
    <RemoteState loading={loading} error={error} empty={!topic} emptyText="Тема не найдена.">
      {topic && (
        <div className="stack">
          <section className="detail-head">
            <div>
              <p className="eyebrow">{topic.category}</p>
              <h1>{topic.title}</h1>
              <p>{topic.summary}</p>
            </div>
            <button className="favorite-button large" type="button" onClick={toggleFavorite}>
              {topic.isFavorite ? "★ В избранном" : "☆ В избранное"}
            </button>
          </section>

          <section className="detail-grid">
            <article className="content-panel">
              <h2>Теория</h2>
              <p>{topic.theory}</p>
              <div className="tags-row">
                {topic.tags.map((item) => (
                  <span key={item}>{item}</span>
                ))}
              </div>
            </article>
            <article className="content-panel compact">
              <h2>Оценка</h2>
              <strong>{topic.complexity}</strong>
              <p>Сложность: {topic.difficulty}</p>
              <p>Популярность: {topic.popularity}/100</p>
            </article>
          </section>

          <section>
            <div className="section-head">
              <div>
                <p className="eyebrow">Практика</p>
                <h2>Задачи по теме</h2>
              </div>
              <Link to={`/tasks?topic=${topic.slug}`}>Открыть в разделе задач</Link>
            </div>
            <div className="task-list">
              {topic.tasks.map((task) => (
                <TaskPreview key={task.id} task={task} />
              ))}
            </div>
          </section>
        </div>
      )}
    </RemoteState>
  );
}

function TasksPage({ auth, onUserPatch }) {
  const [searchParams] = useSearchParams();
  const [search, setSearch] = useState("");
  const [difficulty, setDifficulty] = useState("");
  const [selectedTask, setSelectedTask] = useState(null);
  const topic = searchParams.get("topic") || "";
  const url = useMemo(() => buildQuery("/tasks", { search, difficulty, topic }), [search, difficulty, topic]);
  const { data: tasks, loading, error } = useRemote(url, []);

  useEffect(() => {
    if (!selectedTask && tasks.length) {
      setSelectedTask(tasks[0]);
    }
  }, [selectedTask, tasks]);

  return (
    <div className="stack">
      <PageTitle
        eyebrow="Практика"
        title="Задачи"
        text="Формат похож на Codeforces: условие, примеры, статистика и отправка решения."
      />

      <FilterBar>
        <input value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Поиск по задаче" />
        <Select value={difficulty} onChange={setDifficulty} placeholder="Любая сложность" options={difficulties} />
      </FilterBar>

      <RemoteState loading={loading} error={error} empty={!tasks.length} emptyText="Задачи не найдены.">
        <div className="task-workspace">
          <aside className="task-sidebar">
            {tasks.map((task) => (
              <button
                key={task.id}
                className={selectedTask?.id === task.id ? "task-tab active" : "task-tab"}
                type="button"
                onClick={() => setSelectedTask(task)}
              >
                <span>{task.externalId}</span>
                <strong>{task.title}</strong>
                <small>{task.difficulty}</small>
              </button>
            ))}
          </aside>

          {selectedTask && (
            <div className="task-detail">
              <TaskStatement task={selectedTask} />
              <CodeSubmission task={selectedTask} auth={auth} onUserPatch={onUserPatch} />
            </div>
          )}
        </div>
      </RemoteState>
    </div>
  );
}

function CollectionsPage() {
  const { data: collections, loading, error } = useRemote("/collections", []);

  return (
    <div className="stack">
      <PageTitle
        eyebrow="Маршруты"
        title="Подборки"
        text="Готовые учебные треки помогают двигаться от базы к сложным структурам."
      />

      <RemoteState loading={loading} error={error} empty={!collections.length} emptyText="Подборки не найдены.">
        <div className="cards-grid three">
          {collections.map((collection) => (
            <Link className="collection-card" key={collection.id} to={`/collections/${collection.slug}`}>
              <span>{collection.level}</span>
              <h2>{collection.title}</h2>
              <p>{collection.description}</p>
              <strong>{collection.topics.length} темы</strong>
            </Link>
          ))}
        </div>
      </RemoteState>
    </div>
  );
}

function CollectionDetailPage() {
  const { slug } = useParams();
  const { data: collection, loading, error } = useRemote(`/collections/${slug}`, null);

  return (
    <RemoteState loading={loading} error={error} empty={!collection} emptyText="Подборка не найдена.">
      {collection && (
        <div className="stack">
          <PageTitle eyebrow={collection.level} title={collection.title} text={collection.description} />
          <div className="timeline">
            {collection.topics.map((topic, index) => (
              <Link className="timeline-item" key={topic.id} to={`/algorithms/${topic.slug}`}>
                <span>{String(index + 1).padStart(2, "0")}</span>
                <div>
                  <h2>{topic.title}</h2>
                  <p>{topic.summary}</p>
                  <small>{topic.difficulty}</small>
                </div>
              </Link>
            ))}
          </div>
        </div>
      )}
    </RemoteState>
  );
}

function RatingPage() {
  const { data: users, loading, error } = useRemote("/profile/rating", []);

  return (
    <div className="stack">
      <PageTitle
        eyebrow="Соревнование"
        title="Рейтинг"
        text="Таблица участников строится на данных backend и меняется после принятых решений."
      />

      <RemoteState loading={loading} error={error} empty={!users.length} emptyText="Рейтинг пуст.">
        <div className="rating-table">
          <div className="rating-row heading">
            <span>#</span>
            <span>Участник</span>
            <span>Город</span>
            <span>Решено</span>
            <span>Рейтинг</span>
          </div>
          {users.map((user, index) => (
            <div className="rating-row" key={user.id}>
              <span>{index + 1}</span>
              <span className="person">
                <span className="avatar" style={{ backgroundColor: user.avatarColor }}>
                  {user.name.slice(0, 1)}
                </span>
                {user.name}
              </span>
              <span>{user.city}</span>
              <span>{user.solvedCount}</span>
              <strong>{user.rating}</strong>
            </div>
          ))}
        </div>
      </RemoteState>
    </div>
  );
}

function FavoritesPage({ auth }) {
  if (!auth.user) {
    return <AuthRequired title="Избранное доступно после входа" />;
  }

  return <FavoritesContent />;
}

function FavoritesContent() {
  const { data: topics, setData, loading, error } = useRemote("/favorites", []);

  const removeFavorite = async (topic) => {
    await api.delete(`/favorites/${topic.id}`);
    setData((items) => items.filter((item) => item.id !== topic.id));
  };

  return (
    <div className="stack">
      <PageTitle
        eyebrow="Личная полка"
        title="Избранное"
        text="Сохраняйте темы, к которым нужно вернуться перед контестом."
      />
      <RemoteState loading={loading} error={error} empty={!topics.length} emptyText="В избранном пока нет тем.">
        <div className="cards-grid">
          {topics.map((topic) => (
            <TopicCard key={topic.id} topic={topic} onFavorite={() => removeFavorite(topic)} />
          ))}
        </div>
      </RemoteState>
    </div>
  );
}

function ProfilePage({ auth, onUserPatch }) {
  if (!auth.user) {
    return <AuthRequired title="Профиль доступен после входа" />;
  }

  return <ProfileContent onUserPatch={onUserPatch} />;
}

function ProfileContent({ onUserPatch }) {
  const { data: profile, setData, loading, error } = useRemote("/profile/me", null);
  const [form, setForm] = useState({ name: "", city: "" });
  const [message, setMessage] = useState("");

  useEffect(() => {
    if (profile) {
      setForm({ name: profile.name, city: profile.city });
    }
  }, [profile]);

  const submit = async (event) => {
    event.preventDefault();
    setMessage("");
    const response = await api.put("/profile/me", form);
    setData(response.data);
    onUserPatch(response.data);
    setMessage("Профиль обновлен.");
  };

  return (
    <RemoteState loading={loading} error={error} empty={!profile} emptyText="Профиль не найден.">
      {profile && (
        <div className="profile-grid">
          <section className="profile-card">
            <span className="avatar huge" style={{ backgroundColor: profile.avatarColor }}>
              {profile.name.slice(0, 1)}
            </span>
            <h1>{profile.name}</h1>
            <p>{profile.email}</p>
            <div className="profile-metrics">
              <Metric label="Рейтинг" value={profile.rating} />
              <Metric label="Решено" value={profile.solvedCount} />
            </div>
          </section>

          <section className="content-panel">
            <h2>Настройки</h2>
            <form className="form-stack" onSubmit={submit}>
              <label>
                Имя
                <input value={form.name} onChange={(event) => setForm({ ...form, name: event.target.value })} />
              </label>
              <label>
                Город
                <input value={form.city} onChange={(event) => setForm({ ...form, city: event.target.value })} />
              </label>
              <button className="primary-button" type="submit">
                Сохранить
              </button>
              {message && <p className="success-message">{message}</p>}
            </form>
          </section>

          <section className="content-panel profile-history">
            <h2>Последние отправки</h2>
            {profile.recentSubmissions.length ? (
              <div className="submission-list">
                {profile.recentSubmissions.map((submission) => (
                  <div className="submission-item" key={submission.id}>
                    <div>
                      <strong>{submission.taskTitle}</strong>
                      <small>{submission.language}</small>
                    </div>
                    <span className={submission.status === "Accepted" ? "status accepted" : "status review"}>
                      {submission.status}
                    </span>
                    <b>{submission.points}</b>
                  </div>
                ))}
              </div>
            ) : (
              <p>Отправок пока нет.</p>
            )}
          </section>
        </div>
      )}
    </RemoteState>
  );
}

function AuthPage({ onAuth }) {
  const navigate = useNavigate();
  const [mode, setMode] = useState("login");
  const [form, setForm] = useState({ name: "", email: "student@sportprog.local", password: "demo123" });
  const [error, setError] = useState("");

  const submit = async (event) => {
    event.preventDefault();
    setError("");

    try {
      const endpoint = mode === "login" ? "/auth/login" : "/auth/register";
      const body =
        mode === "login"
          ? { email: form.email, password: form.password }
          : { name: form.name, email: form.email, password: form.password };
      const response = await api.post(endpoint, body);
      onAuth(response.data);
      navigate("/profile");
    } catch (requestError) {
      setError(getErrorMessage(requestError));
    }
  };

  return (
    <div className="auth-layout">
      <section className="auth-copy">
        <p className="eyebrow">Регистрация и авторизация</p>
        <h1>Личный прогресс в алгоритмах</h1>
        <p>Демо-аккаунт уже создан: student@sportprog.local / demo123.</p>
      </section>

      <section className="auth-panel">
        <div className="segmented">
          <button className={mode === "login" ? "active" : ""} type="button" onClick={() => setMode("login")}>
            Вход
          </button>
          <button className={mode === "register" ? "active" : ""} type="button" onClick={() => setMode("register")}>
            Регистрация
          </button>
        </div>

        <form className="form-stack" onSubmit={submit}>
          {mode === "register" && (
            <label>
              Имя
              <input
                value={form.name}
                onChange={(event) => setForm({ ...form, name: event.target.value })}
                placeholder="Ваше имя"
              />
            </label>
          )}
          <label>
            Email
            <input
              value={form.email}
              onChange={(event) => setForm({ ...form, email: event.target.value })}
              type="email"
              placeholder="name@example.com"
            />
          </label>
          <label>
            Пароль
            <input
              value={form.password}
              onChange={(event) => setForm({ ...form, password: event.target.value })}
              type="password"
              placeholder="Минимум 6 символов"
            />
          </label>
          {error && <p className="error-message">{error}</p>}
          <button className="primary-button full-width" type="submit">
            {mode === "login" ? "Войти" : "Зарегистрироваться"}
          </button>
        </form>
      </section>
    </div>
  );
}

function CodeSubmission({ task, auth, onUserPatch }) {
  const [language, setLanguage] = useState(languages[0]);
  const [code, setCode] = useState("#include <bits/stdc++.h>\nusing namespace std;\n\nint main() {\n    return 0;\n}");
  const [result, setResult] = useState(null);
  const [error, setError] = useState("");
  const [submitting, setSubmitting] = useState(false);

  const submit = async (event) => {
    event.preventDefault();
    setSubmitting(true);
    setError("");
    setResult(null);

    try {
      const response = await api.post(`/tasks/${task.id}/submit`, { language, code });
      setResult(response.data);
      onUserPatch({ rating: response.data.rating, solvedCount: response.data.solvedCount });
    } catch (requestError) {
      setError(getErrorMessage(requestError));
    } finally {
      setSubmitting(false);
    }
  };

  if (!auth.user) {
    return (
      <section className="content-panel compact">
        <h2>Отправка решения</h2>
        <p>Войдите в аккаунт, чтобы сохранять отправки и обновлять рейтинг.</p>
        <Link className="primary-button" to="/auth">
          Войти
        </Link>
      </section>
    );
  }

  return (
    <section className="content-panel">
      <h2>Отправка решения</h2>
      <form className="form-stack" onSubmit={submit}>
        <label>
          Язык
          <select value={language} onChange={(event) => setLanguage(event.target.value)}>
            {languages.map((item) => (
              <option key={item} value={item}>
                {item}
              </option>
            ))}
          </select>
        </label>
        <label>
          Код
          <textarea value={code} onChange={(event) => setCode(event.target.value)} rows={10} />
        </label>
        {error && <p className="error-message">{error}</p>}
        {result && (
          <p className={result.status === "Accepted" ? "success-message" : "notice-message"}>
            {result.status}: {result.points} баллов
          </p>
        )}
        <button className="primary-button" type="submit" disabled={submitting}>
          {submitting ? "Проверка..." : "Отправить"}
        </button>
      </form>
    </section>
  );
}

function TaskStatement({ task }) {
  return (
    <section className="content-panel">
      <div className="task-title-row">
        <div>
          <p className="eyebrow">{task.topicTitle}</p>
          <h2>{task.title}</h2>
        </div>
        <span className="difficulty">{task.difficulty}</span>
      </div>
      <p>{task.statement}</p>
      <div className="statement-grid">
        <div>
          <h3>Ввод</h3>
          <p>{task.inputFormat}</p>
        </div>
        <div>
          <h3>Вывод</h3>
          <p>{task.outputFormat}</p>
        </div>
      </div>
      <div className="example-grid">
        <pre>{task.exampleInput}</pre>
        <pre>{task.exampleOutput}</pre>
      </div>
      <div className="task-meta">
        <span>{task.externalId}</span>
        <span>Решили: {task.solvedCount}</span>
        <span>AC: {Math.round(task.acceptanceRate)}%</span>
      </div>
    </section>
  );
}

function TaskPreview({ task }) {
  return (
    <article className="task-preview">
      <div>
        <span>{task.externalId}</span>
        <h3>{task.title}</h3>
        <p>{task.statement}</p>
      </div>
      <Link className="secondary-button" to="/tasks">
        Решать
      </Link>
    </article>
  );
}

function TopicCard({ topic, onFavorite }) {
  return (
    <article className="topic-card">
      <div className="card-topline">
        <span>{topic.category}</span>
        <button className="favorite-button" type="button" onClick={onFavorite} aria-label="Переключить избранное">
          {topic.isFavorite ? "★" : "☆"}
        </button>
      </div>
      <Link to={`/algorithms/${topic.slug}`}>
        <h2>{topic.title}</h2>
        <p>{topic.summary}</p>
      </Link>
      <div className="tags-row">
        {topic.tags.slice(0, 3).map((tag) => (
          <span key={tag}>{tag}</span>
        ))}
      </div>
      <div className="card-footer">
        <span>{topic.difficulty}</span>
        <span>{topic.popularity}/100</span>
      </div>
    </article>
  );
}

function TopicRow({ topic }) {
  return (
    <Link className="topic-row" to={`/algorithms/${topic.slug}`}>
      <div>
        <strong>{topic.title}</strong>
        <small>{topic.category}</small>
      </div>
      <span>{topic.difficulty}</span>
    </Link>
  );
}

function PageTitle({ eyebrow, title, text }) {
  return (
    <section className="page-title">
      <p className="eyebrow">{eyebrow}</p>
      <h1>{title}</h1>
      <p>{text}</p>
    </section>
  );
}

function FilterBar({ children }) {
  return <section className="filter-bar">{children}</section>;
}

function Select({ value, onChange, placeholder, options }) {
  return (
    <select value={value} onChange={(event) => onChange(event.target.value)}>
      <option value="">{placeholder}</option>
      {options.map((option) => (
        <option key={option} value={option}>
          {option}
        </option>
      ))}
    </select>
  );
}

function RemoteState({ loading, error, empty, emptyText, children }) {
  if (loading) {
    return <div className="state-box">Загрузка...</div>;
  }

  if (error) {
    return <div className="state-box error">{error}</div>;
  }

  if (empty) {
    return <div className="state-box">{emptyText}</div>;
  }

  return children;
}

function Metric({ label, value }) {
  return (
    <div className="metric">
      <strong>{value}</strong>
      <span>{label}</span>
    </div>
  );
}

function AuthRequired({ title }) {
  return (
    <section className="auth-required">
      <h1>{title}</h1>
      <p>Авторизация нужна для личных данных, избранного и истории решений.</p>
      <Link className="primary-button" to="/auth">
        Войти или зарегистрироваться
      </Link>
    </section>
  );
}

function NotFoundPage() {
  return (
    <section className="auth-required">
      <h1>Страница не найдена</h1>
      <Link className="primary-button" to="/">
        На главную
      </Link>
    </section>
  );
}

export default App;
