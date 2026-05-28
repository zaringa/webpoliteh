const path = require('path');
const fs = require('fs');
const express = require('express');
const sqlite3 = require('sqlite3').verbose();
const bcrypt = require('bcryptjs');
const jwt = require('jsonwebtoken');

const app = express();

const PORT = Number(process.env.PORT) || 5600;
const JWT_SECRET = process.env.JWT_SECRET || 'lab8_dev_secret_change_me';
const DB_DIR = path.join(__dirname, 'data');
const DB_PATH = path.join(DB_DIR, 'lab8.db');

if (!fs.existsSync(DB_DIR)) {
  fs.mkdirSync(DB_DIR, { recursive: true });
}

const db = new sqlite3.Database(DB_PATH);
const revokedTokens = new Set();

function run(sql, params = []) {
  return new Promise((resolve, reject) => {
    db.run(sql, params, function onRun(err) {
      if (err) {
        reject(err);
        return;
      }

      resolve(this);
    });
  });
}

function get(sql, params = []) {
  return new Promise((resolve, reject) => {
    db.get(sql, params, (err, row) => {
      if (err) {
        reject(err);
        return;
      }

      resolve(row ?? null);
    });
  });
}

async function initDb() {
  await run(`
    CREATE TABLE IF NOT EXISTS users (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      full_name TEXT NOT NULL,
      email TEXT NOT NULL UNIQUE,
      password_hash TEXT NOT NULL,
      created_at TEXT NOT NULL
    )
  `);
}

function normalizeEmail(email) {
  return String(email || '').trim().toLowerCase();
}

function isValidEmail(email) {
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
}

function publicUser(user) {
  return {
    id: user.id,
    fullName: user.full_name,
    email: user.email,
    createdAt: user.created_at,
  };
}

function createToken(user) {
  return jwt.sign(
    {
      sub: user.id,
      email: user.email,
    },
    JWT_SECRET,
    {
      expiresIn: '8h',
    },
  );
}

function getBearerToken(req) {
  const authHeader = req.headers.authorization || '';
  const [scheme, token] = authHeader.split(' ');

  if (scheme !== 'Bearer' || !token) {
    return null;
  }

  return token;
}

async function authMiddleware(req, res, next) {
  const token = getBearerToken(req);
  if (!token) {
    res.status(401).json({ message: 'Требуется токен авторизации.' });
    return;
  }

  if (revokedTokens.has(token)) {
    res.status(401).json({ message: 'Токен недействителен. Войдите снова.' });
    return;
  }

  try {
    const payload = jwt.verify(token, JWT_SECRET);

    const user = await get('SELECT * FROM users WHERE id = ?', [payload.sub]);
    if (!user) {
      res.status(401).json({ message: 'Пользователь не найден.' });
      return;
    }

    req.token = token;
    req.user = user;
    next();
  } catch (err) {
    res.status(401).json({ message: 'Неверный или просроченный токен.' });
  }
}

app.use(express.json());
app.use(express.static(path.join(__dirname, 'public')));

app.post('/api/auth/register', async (req, res) => {
  try {
    const fullName = String(req.body.fullName || '').trim();
    const email = normalizeEmail(req.body.email);
    const password = String(req.body.password || '');

    if (fullName.length < 2) {
      res.status(400).json({ message: 'Имя должно содержать минимум 2 символа.' });
      return;
    }

    if (!isValidEmail(email)) {
      res.status(400).json({ message: 'Некорректный email.' });
      return;
    }

    if (password.length < 6) {
      res.status(400).json({ message: 'Пароль должен содержать минимум 6 символов.' });
      return;
    }

    const existing = await get('SELECT id FROM users WHERE email = ?', [email]);
    if (existing) {
      res.status(409).json({ message: 'Пользователь с таким email уже существует.' });
      return;
    }

    const passwordHash = await bcrypt.hash(password, 10);
    const createdAt = new Date().toISOString();

    const insertResult = await run(
      'INSERT INTO users (full_name, email, password_hash, created_at) VALUES (?, ?, ?, ?)',
      [fullName, email, passwordHash, createdAt],
    );

    const user = await get('SELECT * FROM users WHERE id = ?', [insertResult.lastID]);
    const token = createToken(user);

    res.status(201).json({
      message: 'Регистрация успешна.',
      token,
      user: publicUser(user),
    });
  } catch (err) {
    res.status(500).json({ message: 'Ошибка сервера при регистрации.' });
  }
});

app.post('/api/auth/login', async (req, res) => {
  try {
    const email = normalizeEmail(req.body.email);
    const password = String(req.body.password || '');

    if (!isValidEmail(email) || !password) {
      res.status(400).json({ message: 'Введите корректные данные для входа.' });
      return;
    }

    const user = await get('SELECT * FROM users WHERE email = ?', [email]);
    if (!user) {
      res.status(401).json({ message: 'Неверный email или пароль.' });
      return;
    }

    const ok = await bcrypt.compare(password, user.password_hash);
    if (!ok) {
      res.status(401).json({ message: 'Неверный email или пароль.' });
      return;
    }

    const token = createToken(user);

    res.json({
      message: 'Вход выполнен.',
      token,
      user: publicUser(user),
    });
  } catch (err) {
    res.status(500).json({ message: 'Ошибка сервера при входе.' });
  }
});

app.get('/api/me', authMiddleware, async (req, res) => {
  res.json({
    user: publicUser(req.user),
  });
});

app.get('/api/protected/dashboard', authMiddleware, async (req, res) => {
  res.json({
    message: 'Доступ к защищенному маршруту подтвержден.',
    serverTime: new Date().toISOString(),
    user: publicUser(req.user),
  });
});

app.post('/api/auth/logout', authMiddleware, async (req, res) => {
  revokedTokens.add(req.token);
  res.json({ message: 'Выход выполнен. Токен отозван.' });
});

app.get('*', (req, res) => {
  res.sendFile(path.join(__dirname, 'public', 'index.html'));
});

initDb()
  .then(() => {
    app.listen(PORT, () => {
      console.log(`Lab8 server started: http://localhost:${PORT}`);
    });
  })
  .catch((err) => {
    console.error('DB init failed', err);
    process.exit(1);
  });

process.on('SIGINT', () => {
  db.close(() => {
    process.exit(0);
  });
});
