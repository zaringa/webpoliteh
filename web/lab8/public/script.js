const TOKEN_KEY = 'lab8_jwt_token';

const elements = {
  authView: document.getElementById('auth-view'),
  dashboardView: document.getElementById('dashboard-view'),
  modeLoginBtn: document.getElementById('mode-login-btn'),
  modeRegisterBtn: document.getElementById('mode-register-btn'),
  form: document.getElementById('auth-form'),
  fullNameWrap: document.getElementById('full-name-wrap'),
  fullNameInput: document.getElementById('full-name-input'),
  emailInput: document.getElementById('email-input'),
  passwordInput: document.getElementById('password-input'),
  submitBtn: document.getElementById('submit-btn'),
  welcomeText: document.getElementById('welcome-text'),
  emailText: document.getElementById('email-text'),
  protectedText: document.getElementById('protected-text'),
  refreshBtn: document.getElementById('refresh-btn'),
  logoutBtn: document.getElementById('logout-btn'),
  statusText: document.getElementById('status-text'),
};

const state = {
  mode: 'login',
  token: localStorage.getItem(TOKEN_KEY) || '',
  user: null,
};

function setStatus(message, type = 'neutral') {
  elements.statusText.textContent = message;

  if (type === 'error') {
    elements.statusText.className = 'mt-5 text-sm font-medium text-rose-300';
    return;
  }

  if (type === 'success') {
    elements.statusText.className = 'mt-5 text-sm font-medium text-emerald-300';
    return;
  }

  elements.statusText.className = 'mt-5 text-sm font-medium text-slate-300';
}

function saveToken(token) {
  state.token = token;
  localStorage.setItem(TOKEN_KEY, token);
}

function clearToken() {
  state.token = '';
  localStorage.removeItem(TOKEN_KEY);
}

function applyMode() {
  const isRegister = state.mode === 'register';

  elements.fullNameWrap.classList.toggle('hidden', !isRegister);
  elements.submitBtn.textContent = isRegister ? 'Зарегистрироваться' : 'Войти';

  elements.modeLoginBtn.className =
    state.mode === 'login'
      ? 'rounded-full border border-cyan-300/50 bg-cyan-500/20 px-4 py-2 text-sm font-bold text-cyan-100'
      : 'rounded-full border border-slate-700 bg-slate-800 px-4 py-2 text-sm font-bold text-slate-200';

  elements.modeRegisterBtn.className =
    state.mode === 'register'
      ? 'rounded-full border border-cyan-300/50 bg-cyan-500/20 px-4 py-2 text-sm font-bold text-cyan-100'
      : 'rounded-full border border-slate-700 bg-slate-800 px-4 py-2 text-sm font-bold text-slate-200';
}

function showAuthView() {
  elements.authView.classList.remove('hidden');
  elements.dashboardView.classList.add('hidden');
}

function showDashboardView() {
  elements.authView.classList.add('hidden');
  elements.dashboardView.classList.remove('hidden');
}

function updateProfileUI() {
  if (!state.user) {
    elements.welcomeText.textContent = '-';
    elements.emailText.textContent = '-';
    return;
  }

  elements.welcomeText.textContent = `Здравствуйте, ${state.user.fullName}`;
  elements.emailText.textContent = `Email: ${state.user.email}`;
}

async function apiRequest(url, options = {}) {
  const headers = {
    'Content-Type': 'application/json',
    ...(options.headers || {}),
  };

  if (state.token) {
    headers.Authorization = `Bearer ${state.token}`;
  }

  const response = await fetch(url, {
    ...options,
    headers,
  });

  const payload = await response.json().catch(() => ({}));

  if (!response.ok) {
    const message = payload.message || 'Ошибка запроса.';
    throw new Error(message);
  }

  return payload;
}

async function refreshProtectedData() {
  const data = await apiRequest('/api/protected/dashboard', { method: 'GET' });
  elements.protectedText.textContent = `${data.message} Серверное время: ${data.serverTime}`;
}

async function restoreSession() {
  if (!state.token) {
    showAuthView();
    setStatus('Введите данные для входа или регистрации.');
    return;
  }

  try {
    const me = await apiRequest('/api/me', { method: 'GET' });
    state.user = me.user;
    updateProfileUI();
    await refreshProtectedData();
    showDashboardView();
    setStatus('Сессия восстановлена.', 'success');
  } catch (err) {
    clearToken();
    state.user = null;
    showAuthView();
    setStatus('Сессия истекла. Выполните вход снова.', 'error');
  }
}

async function submitAuthForm(event) {
  event.preventDefault();

  const email = elements.emailInput.value.trim();
  const password = elements.passwordInput.value;
  const fullName = elements.fullNameInput.value.trim();

  if (!email || !password) {
    setStatus('Заполните email и пароль.', 'error');
    return;
  }

  if (state.mode === 'register' && fullName.length < 2) {
    setStatus('Для регистрации укажите ФИО (минимум 2 символа).', 'error');
    return;
  }

  try {
    const endpoint = state.mode === 'register' ? '/api/auth/register' : '/api/auth/login';
    const body =
      state.mode === 'register'
        ? { fullName, email, password }
        : { email, password };

    const data = await apiRequest(endpoint, {
      method: 'POST',
      body: JSON.stringify(body),
    });

    saveToken(data.token);
    state.user = data.user;
    updateProfileUI();
    await refreshProtectedData();
    showDashboardView();

    elements.passwordInput.value = '';
    setStatus(data.message || 'Успешная авторизация.', 'success');
  } catch (err) {
    setStatus(err.message, 'error');
  }
}

async function logout() {
  try {
    if (state.token) {
      await apiRequest('/api/auth/logout', { method: 'POST' });
    }
  } catch {
    // ignore network/auth errors on logout
  }

  clearToken();
  state.user = null;
  elements.protectedText.textContent = '-';
  showAuthView();
  setStatus('Вы вышли из системы. Токен очищен.', 'success');
}

elements.modeLoginBtn.addEventListener('click', () => {
  state.mode = 'login';
  applyMode();
  setStatus('Режим входа.');
});

elements.modeRegisterBtn.addEventListener('click', () => {
  state.mode = 'register';
  applyMode();
  setStatus('Режим регистрации.');
});

elements.form.addEventListener('submit', submitAuthForm);

elements.refreshBtn.addEventListener('click', async () => {
  try {
    const me = await apiRequest('/api/me', { method: 'GET' });
    state.user = me.user;
    updateProfileUI();
    await refreshProtectedData();
    setStatus('Данные обновлены.', 'success');
  } catch (err) {
    setStatus(err.message, 'error');
  }
});

elements.logoutBtn.addEventListener('click', logout);

applyMode();
restoreSession();
