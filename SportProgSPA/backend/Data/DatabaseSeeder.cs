using Microsoft.EntityFrameworkCore;
using SportProg.Api.Models;
using SportProg.Api.Services;

namespace SportProg.Api.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(SportProgDbContext db, PasswordService passwords)
    {
        await db.Database.EnsureCreatedAsync();

        if (await db.AlgorithmTopics.AnyAsync())
        {
            return;
        }

        var asymptotics = Topic(
            "asymptotic-complexity",
            "Асимптотическая сложность",
            "Основы",
            "Легко",
            "Как оценивать время и память алгоритма до запуска программы.",
            "Big O помогает сравнивать решения по росту количества операций. В спортивном программировании это главный фильтр перед написанием кода.",
            "O(1), O(log n), O(n), O(n log n), O(n^2)",
            "асимптотика,big-o,оценка",
            96,
            Task("Сумма на префиксе", "Легко", "Дан массив. Нужно быстро отвечать на запросы суммы на отрезке.", "n, q и массив, затем q пар l r.", "Суммы для каждого запроса.", "5 2\n1 2 3 4 5\n1 3\n2 5", "6\n14", "A-101", 180, 78));

        var sorting = Topic(
            "sorting",
            "Сортировка",
            "Основы",
            "Легко",
            "Упорядочивание данных и выбор устойчивой стратегии сравнения.",
            "Сортировка часто является первым шагом к жадному решению, двум указателям или бинарному поиску по ответу.",
            "O(n log n)",
            "сортировка,компаратор,жадные",
            91,
            Task("Медали олимпиады", "Легко", "Участников нужно вывести по убыванию результата, а при равенстве по имени.", "n и пары имя/баллы.", "Отсортированный список.", "3\nIra 80\nMax 95\nAnn 95", "Ann 95\nMax 95\nIra 80", "S-204", 142, 81));

        var binarySearch = Topic(
            "binary-search",
            "Бинарный поиск",
            "Основы",
            "Средне",
            "Поиск элемента или минимального подходящего ответа на монотонном условии.",
            "Если ответ можно проверить функцией true/false и эта функция монотонна, часто применим бинарный поиск по ответу.",
            "O(log n) или O(n log V)",
            "бинарный поиск,монотонность,ответ",
            88,
            Task("Высота стены", "Средне", "Найдите минимальную высоту, при которой хватает блоков для всех рядов.", "Количество рядов и блоков.", "Минимальная высота.", "6 21", "6", "B-118", 97, 63));

        var graph = Topic(
            "graph-representation",
            "Представление графа",
            "Графы",
            "Легко",
            "Матрица и списки смежности, выбор структуры под ограничения.",
            "Списки смежности экономят память для разреженных графов, матрица быстро отвечает на вопрос о наличии ребра.",
            "O(n + m) для обхода списков",
            "графы,списки смежности,матрица",
            80,
            Task("Дороги района", "Легко", "По списку дорог построить степени всех вершин.", "n, m и m ребер.", "Степени вершин.", "4 3\n1 2\n2 3\n2 4", "1 3 1 1", "G-011", 121, 74));

        var bfs = Topic(
            "bfs",
            "Обход в ширину (BFS)",
            "Графы",
            "Средне",
            "Кратчайшие расстояния в невзвешенном графе и волновой обход клеток.",
            "BFS использует очередь и гарантирует минимальное число ребер от старта до каждой достижимой вершины.",
            "O(n + m)",
            "bfs,очередь,кратчайший путь",
            87,
            Task("Лабиринт школы", "Средне", "Найдите минимальное число шагов от S до F по клетчатому полю.", "h, w и поле.", "Длина пути или -1.", "3 4\nS..#\n.#..\n...F", "5", "G-120", 166, 69));

        var dfs = Topic(
            "dfs",
            "Обход в глубину (DFS)",
            "Графы",
            "Средне",
            "Рекурсивный или стековый обход для компонент, циклов и топологии.",
            "DFS удобно раскрывает структуру графа: времена входа, компоненты связности, дерево обхода.",
            "O(n + m)",
            "dfs,рекурсия,компоненты",
            83,
            Task("Острова", "Средне", "Посчитайте количество компонент из клеток земли на карте.", "h, w и карта из . и #.", "Количество островов.", "4 5\n##...\n.#...\n...##\n...##", "2", "G-131", 132, 66));

        var dp = Topic(
            "dynamic-programming",
            "Динамическое программирование",
            "DP",
            "Сложно",
            "Разбиение задачи на состояния и переходы без повторного перебора.",
            "DP появляется, когда оптимальный ответ можно выразить через меньшие подзадачи. Важно явно определить состояние, базу и переход.",
            "Обычно O(количество состояний * переходы)",
            "dp,динамика,состояния",
            94,
            Task("Лестница программиста", "Средне", "На i-й ступеньке лежат очки. Можно шагать на 1 или 2 ступени, нужно максимум очков.", "n и очки на ступеньках.", "Максимальная сумма.", "5\n1 2 9 4 5", "15", "D-045", 214, 71));

        var prefix = Topic(
            "prefix-sums",
            "Префиксные суммы и XOR",
            "Структуры данных",
            "Легко",
            "Предподсчет для быстрых запросов на отрезках.",
            "Префиксные суммы переводят каждый запрос суммы из O(n) в O(1). XOR работает похожим образом благодаря обратимости операции.",
            "O(n + q)",
            "префиксы,xor,отрезки",
            86,
            Task("Контрольные суммы", "Легко", "Ответьте на запросы XOR на отрезке массива.", "n, q, массив и запросы.", "XOR каждого отрезка.", "4 2\n1 3 4 8\n1 2\n2 4", "2\n15", "P-080", 103, 77));

        var dijkstra = Topic(
            "dijkstra",
            "Алгоритм Дейкстры",
            "Графы",
            "Сложно",
            "Кратчайшие пути во взвешенном графе с неотрицательными ребрами.",
            "Очередь с приоритетом позволяет всегда расширять вершину с текущим минимальным расстоянием.",
            "O((n + m) log n)",
            "дейкстра,графы,куча",
            77,
            Task("Доставка задач", "Сложно", "Найдите кратчайшее время доставки от сервера до всех аудиторий.", "n, m, старт и ребра с весами.", "Расстояния или -1.", "4 4 1\n1 2 5\n1 3 2\n3 2 1\n2 4 7", "0 3 2 10", "W-501", 76, 55));

        var dsu = Topic(
            "dsu",
            "DSU",
            "Структуры данных",
            "Средне",
            "Система непересекающихся множеств для динамических объединений.",
            "DSU хранит представителя компоненты и почти за O(1) объединяет множества с эвристиками сжатия пути и ранга.",
            "Амортизированно O(alpha(n))",
            "dsu,union-find,компоненты",
            79,
            Task("Команды турнира", "Средне", "Обрабатывайте запросы объединения команд и проверки, в одной ли они группе.", "n, q и запросы union/get.", "YES или NO.", "4 4\nunion 1 2\nget 1 3\nunion 2 3\nget 1 3", "NO\nYES", "U-207", 89, 68));

        var segmentTree = Topic(
            "segment-tree",
            "Дерево отрезков",
            "Структуры данных",
            "Сложно",
            "Гибкая структура для запросов и обновлений на отрезках.",
            "Дерево отрезков делит массив на интервалы и пересчитывает только O(log n) узлов после изменения.",
            "O(log n) на запрос и обновление",
            "дерево отрезков,range query,update",
            72,
            Task("Рекорды тренировок", "Сложно", "Поддерживайте обновления результата и запрос максимума на отрезке.", "n, q, массив и операции set/max.", "Ответы на max.", "5 3\n1 5 2 4 3\nmax 2 5\nset 3 9\nmax 2 5", "5\n9", "T-610", 64, 49));

        var kmp = Topic(
            "kmp",
            "Префикс-функция и КМП",
            "Строки",
            "Сложно",
            "Быстрый поиск подстроки и анализ повторов через префикс-функцию.",
            "Префикс-функция хранит длину максимального собственного префикса, который является суффиксом текущей строки.",
            "O(n + m)",
            "строки,кмп,префикс-функция",
            68,
            Task("Шифр жюри", "Сложно", "Найдите все позиции вхождения шаблона в протокол.", "Шаблон и текст.", "Позиции вхождений.", "aba\nabacaba", "1 5", "K-404", 57, 52));

        db.AlgorithmTopics.AddRange(
            asymptotics,
            sorting,
            binarySearch,
            graph,
            bfs,
            dfs,
            dp,
            prefix,
            dijkstra,
            dsu,
            segmentTree,
            kmp);

        var beginner = Collection("starter-track", "Старт в олимпиадном программировании", "Первые темы, которые дают базу для задач уровня A-B.", "Начальный", asymptotics, sorting, prefix, binarySearch);
        var graphs = Collection("graph-track", "Графы без паники", "Маршрут от представления графа до кратчайших путей.", "Средний", graph, bfs, dfs, dijkstra);
        var structures = Collection("data-structures-track", "Структуры данных для рейтинга", "DSU, деревья и приемы для задач с большим числом запросов.", "Продвинутый", dsu, segmentTree, kmp, dp);
        db.LearningCollections.AddRange(beginner, graphs, structures);

        var (hash, salt) = passwords.CreateHash("demo123");
        var user = new User
        {
            Name = "Зарина",
            Email = "student@sportprog.local",
            PasswordHash = hash,
            PasswordSalt = salt,
            City = "Брест",
            AvatarColor = "#16a34a",
            Rating = 1280,
            SolvedCount = 17
        };

        var (maxHash, maxSalt) = passwords.CreateHash("demo123");
        var max = new User
        {
            Name = "Максим",
            Email = "max@sportprog.local",
            PasswordHash = maxHash,
            PasswordSalt = maxSalt,
            City = "Минск",
            AvatarColor = "#dc2626",
            Rating = 1510,
            SolvedCount = 31
        };

        var (annHash, annSalt) = passwords.CreateHash("demo123");
        var ann = new User
        {
            Name = "Анна",
            Email = "anna@sportprog.local",
            PasswordHash = annHash,
            PasswordSalt = annSalt,
            City = "Гродно",
            AvatarColor = "#7c3aed",
            Rating = 1425,
            SolvedCount = 25
        };

        db.Users.AddRange(user, max, ann);
        await db.SaveChangesAsync();

        db.FavoriteAlgorithms.AddRange(
            new FavoriteAlgorithm { UserId = user.Id, AlgorithmTopicId = dp.Id },
            new FavoriteAlgorithm { UserId = user.Id, AlgorithmTopicId = bfs.Id },
            new FavoriteAlgorithm { UserId = user.Id, AlgorithmTopicId = segmentTree.Id });

        db.Submissions.AddRange(
            new Submission { UserId = user.Id, ChallengeTaskId = dp.Tasks.First().Id, Language = "C++17", Status = "Accepted", Points = 100 },
            new Submission { UserId = user.Id, ChallengeTaskId = bfs.Tasks.First().Id, Language = "Python 3", Status = "Accepted", Points = 100 },
            new Submission { UserId = user.Id, ChallengeTaskId = segmentTree.Tasks.First().Id, Language = "C++17", Status = "Needs review", Points = 35 });

        await db.SaveChangesAsync();
    }

    private static AlgorithmTopic Topic(
        string slug,
        string title,
        string category,
        string difficulty,
        string summary,
        string theory,
        string complexity,
        string tags,
        int popularity,
        params ChallengeTask[] tasks)
    {
        return new AlgorithmTopic
        {
            Slug = slug,
            Title = title,
            Category = category,
            Difficulty = difficulty,
            Summary = summary,
            Theory = theory,
            Complexity = complexity,
            Tags = tags,
            Popularity = popularity,
            Tasks = tasks
        };
    }

    private static ChallengeTask Task(
        string title,
        string difficulty,
        string statement,
        string input,
        string output,
        string exampleInput,
        string exampleOutput,
        string externalId,
        int solved,
        double acceptance)
    {
        return new ChallengeTask
        {
            Title = title,
            Difficulty = difficulty,
            Statement = statement,
            InputFormat = input,
            OutputFormat = output,
            ExampleInput = exampleInput,
            ExampleOutput = exampleOutput,
            ExternalId = externalId,
            SolvedCount = solved,
            AcceptanceRate = acceptance
        };
    }

    private static LearningCollection Collection(string slug, string title, string description, string level, params AlgorithmTopic[] topics)
    {
        var collection = new LearningCollection
        {
            Slug = slug,
            Title = title,
            Description = description,
            Level = level
        };

        for (var i = 0; i < topics.Length; i++)
        {
            collection.Items.Add(new CollectionItem
            {
                AlgorithmTopic = topics[i],
                Position = i + 1
            });
        }

        return collection;
    }
}
