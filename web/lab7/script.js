const { useEffect, useMemo, useRef, useState } = React;

const STORAGE_KEY = 'lab7-game-state-v1';

const DEFAULT_STATE = {
  timeLeft: 60,
  points: 0,
  lives: 3,
  running: false,
  bestScore: 0,
};

let audioContext = null;

function safeNumber(value, fallback) {
  return Number.isFinite(value) ? value : fallback;
}

function loadState() {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) {
      return { ...DEFAULT_STATE };
    }

    const parsed = JSON.parse(raw);
    return {
      timeLeft: Math.max(0, safeNumber(parsed.timeLeft, DEFAULT_STATE.timeLeft)),
      points: Math.max(0, safeNumber(parsed.points, DEFAULT_STATE.points)),
      lives: Math.max(0, safeNumber(parsed.lives, DEFAULT_STATE.lives)),
      running: Boolean(parsed.running),
      bestScore: Math.max(0, safeNumber(parsed.bestScore, DEFAULT_STATE.bestScore)),
    };
  } catch {
    return { ...DEFAULT_STATE };
  }
}

function saveState(state) {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(state));
}

function ensureAudioContext() {
  const Ctx = window.AudioContext || window.webkitAudioContext;
  if (!Ctx) {
    return null;
  }

  if (!audioContext) {
    audioContext = new Ctx();
  }

  if (audioContext.state === 'suspended') {
    audioContext.resume();
  }

  return audioContext;
}

function playBeep(frequency = 720, durationMs = 120) {
  const ctx = ensureAudioContext();
  if (!ctx) {
    return;
  }

  const osc = ctx.createOscillator();
  const gain = ctx.createGain();

  osc.type = 'sine';
  osc.frequency.value = frequency;

  gain.gain.value = 0.0001;
  gain.gain.exponentialRampToValueAtTime(0.08, ctx.currentTime + 0.01);
  gain.gain.exponentialRampToValueAtTime(0.0001, ctx.currentTime + durationMs / 1000);

  osc.connect(gain);
  gain.connect(ctx.destination);

  osc.start();
  osc.stop(ctx.currentTime + durationMs / 1000);
}

function CounterCard({ title, value, hint, tone }) {
  const toneClassByType = {
    time: 'border-sky-400/40 bg-sky-500/10 text-sky-100',
    points: 'border-emerald-400/40 bg-emerald-500/10 text-emerald-100',
    lives: 'border-rose-400/40 bg-rose-500/10 text-rose-100',
  };

  const toneClass = toneClassByType[tone] ?? toneClassByType.time;

  return (
    <article className={`rounded-2xl border p-4 ${toneClass}`}>
      <p className="text-xs uppercase tracking-[0.1em] opacity-80">{title}</p>
      <p className="mt-1 text-3xl font-extrabold">{value}</p>
      <p className="mt-2 text-xs opacity-80">{hint}</p>
    </article>
  );
}

function ControlButton({ onClick, children, variant = 'default' }) {
  const classByVariant = {
    default: 'border-slate-600 bg-slate-800 text-slate-100 hover:border-slate-400',
    accent: 'border-violet-400 bg-violet-500 text-white hover:bg-violet-400',
    danger: 'border-rose-400 bg-rose-500 text-white hover:bg-rose-400',
  };

  return (
    <button
      type="button"
      onClick={onClick}
      className={`rounded-full border px-4 py-2 text-sm font-bold transition ${classByVariant[variant]}`}
    >
      {children}
    </button>
  );
}

function ControlPanel({
  running,
  onToggle,
  onResetRound,
  onAddPoints,
  onLoseLife,
}) {
  return (
    <div className="flex flex-wrap gap-2">
      <ControlButton onClick={onToggle} variant="accent">
        {running ? 'Пауза' : 'Старт'}
      </ControlButton>
      <ControlButton onClick={onAddPoints}>+10 очков</ControlButton>
      <ControlButton onClick={onLoseLife}>-1 жизнь</ControlButton>
      <ControlButton onClick={onResetRound} variant="danger">
        Перезапуск раунда
      </ControlButton>
    </div>
  );
}

function App() {
  const initialState = useMemo(() => loadState(), []);

  const [timeLeft, setTimeLeft] = useState(initialState.timeLeft);
  const [points, setPoints] = useState(initialState.points);
  const [lives, setLives] = useState(initialState.lives);
  const [running, setRunning] = useState(initialState.running);
  const [bestScore, setBestScore] = useState(initialState.bestScore);
  const [bgMode, setBgMode] = useState('normal');

  const previousTimeRef = useRef(timeLeft);
  const previousLivesRef = useRef(lives);

  const gameOver = lives <= 0 || timeLeft <= 0;

  const panelClass =
    bgMode === 'danger'
      ? 'border-rose-500/40 bg-rose-950/40'
      : bgMode === 'warning'
        ? 'border-amber-500/40 bg-amber-950/30'
        : 'border-slate-800 bg-slate-900/85';

  useEffect(() => {
    saveState({
      timeLeft,
      points,
      lives,
      running,
      bestScore,
    });
  }, [timeLeft, points, lives, running, bestScore]);

  useEffect(() => {
    if (points > bestScore) {
      setBestScore(points);
    }

    return undefined;
  }, [points, bestScore]);

  useEffect(() => {
    if (lives <= 1) {
      setBgMode('danger');
      return;
    }

    if (timeLeft <= 15) {
      setBgMode('warning');
      return;
    }

    setBgMode('normal');
  }, [timeLeft, lives]);

  useEffect(() => {
    if (!running || gameOver) {
      return undefined;
    }

    const intervalId = setInterval(() => {
      setTimeLeft((prev) => Math.max(prev - 1, 0));
      setPoints((prev) => prev + 1);
    }, 1000);

    return () => clearInterval(intervalId);
  }, [running, gameOver]);

  useEffect(() => {
    if (gameOver && running) {
      setRunning(false);
    }
  }, [gameOver, running]);

  useEffect(() => {
    const previousTime = previousTimeRef.current;

    if (running && previousTime - timeLeft === 1) {
      if (timeLeft > 0 && timeLeft <= 10) {
        playBeep(540, 90);
      }

      if (timeLeft > 0 && timeLeft % 15 === 0) {
        setLives((prev) => Math.max(prev - 1, 0));
      }
    }

    previousTimeRef.current = timeLeft;
  }, [timeLeft, running]);

  useEffect(() => {
    const previousLives = previousLivesRef.current;

    if (lives < previousLives) {
      playBeep(260, 200);
    }

    previousLivesRef.current = lives;
  }, [lives]);

  function toggleRunning() {
    ensureAudioContext();

    if (gameOver) {
      return;
    }

    setRunning((prev) => !prev);
  }

  function resetRound() {
    setTimeLeft(DEFAULT_STATE.timeLeft);
    setPoints(DEFAULT_STATE.points);
    setLives(DEFAULT_STATE.lives);
    setRunning(false);
    setBgMode('normal');
  }

  function addPoints() {
    ensureAudioContext();
    setPoints((prev) => prev + 10);
  }

  function loseLife() {
    ensureAudioContext();
    setLives((prev) => Math.max(prev - 1, 0));
  }

  const statusText = gameOver
    ? lives <= 0
      ? 'Игра окончена: жизни закончились.'
      : 'Игра окончена: время вышло.'
    : running
      ? 'Раунд активен.'
      : 'Пауза.';

  return (
    <main className="mx-auto w-full max-w-4xl px-4 py-8 md:py-10">
      <section className={`rounded-3xl border p-6 shadow-2xl transition-colors md:p-8 ${panelClass}`}>
        <header className="border-b border-slate-800 pb-5">
          <p className="inline-flex rounded-full border border-violet-300/40 bg-violet-500/10 px-3 py-1 text-xs font-bold uppercase tracking-[0.12em] text-violet-200">
            Лабораторная работа №7
          </p>
          <h1 className="mt-4 text-2xl font-extrabold md:text-4xl">Игровой таймер</h1>
          <p className="mt-3 text-sm text-slate-300 md:text-base">
            useState + useEffect: сохранение состояния, реакция на изменения и автоматические игровые эффекты.
          </p>
        </header>

        <section className="mt-6 grid gap-3 sm:grid-cols-3">
          <CounterCard
            title="Основной таймер"
            value={`${timeLeft} сек`}
            hint="Каждую секунду уменьшается"
            tone="time"
          />
          <CounterCard
            title="Очки"
            value={points}
            hint="Растут во время раунда"
            tone="points"
          />
          <CounterCard
            title="Жизни"
            value={lives}
            hint="Уменьшаются при ошибках"
            tone="lives"
          />
        </section>

        <section className="mt-6">
          <ControlPanel
            running={running}
            onToggle={toggleRunning}
            onResetRound={resetRound}
            onAddPoints={addPoints}
            onLoseLife={loseLife}
          />
        </section>

        <section className="mt-6 grid gap-3 md:grid-cols-2">
          <article className="rounded-2xl border border-slate-700 bg-slate-800/70 p-4">
            <p className="text-xs uppercase tracking-[0.1em] text-slate-400">Состояние раунда</p>
            <p className="mt-2 text-lg font-bold text-slate-100">{statusText}</p>
            <p className="mt-2 text-sm text-slate-300">
              Счётчики синхронизированы: при каждом тике таймера очки увеличиваются, а каждые 15 секунд теряется 1 жизнь.
            </p>
          </article>

          <article className="rounded-2xl border border-slate-700 bg-slate-800/70 p-4 transition">
            <p className="text-xs uppercase tracking-[0.1em] text-slate-400">Рекорд</p>
            <p className="mt-2 text-3xl font-extrabold text-emerald-200">{bestScore}</p>
            <p className="mt-2 text-sm text-slate-300">Автоматически обновляется и сохраняется в localStorage.</p>
          </article>
        </section>
      </section>
    </main>
  );
}

ReactDOM.createRoot(document.getElementById('root')).render(<App />);
