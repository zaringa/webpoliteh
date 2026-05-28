using System;
using System.Collections.Generic;
using System.Linq;

namespace ObserverGame
{
    
    // 1. Категории событий
    

    public enum GameEventCategory
    {
        Player,
        Shop,
        Economy,
        System
    }

    
    // 2. Интерфейс события
    

    public interface IGameEvent
    {
        GameEventCategory Category { get; }
        DateTime Time { get; }
    }

    
    // 3. EventBus
    

    public static class GameEventBus
    {
        private static readonly Dictionary<Type, Delegate> typedSubscribers =
            new Dictionary<Type, Delegate>();

        private static readonly Dictionary<GameEventCategory,
            Action<IGameEvent>> categorySubscribers =
                new Dictionary<GameEventCategory, Action<IGameEvent>>();

        private static Action<IGameEvent> allSubscribers;

        

        public static void Subscribe<TEvent>(Action<TEvent> listener)
            where TEvent : IGameEvent
        {
            Type eventType = typeof(TEvent);

            if (typedSubscribers.TryGetValue(eventType, out Delegate existing))
            {
                typedSubscribers[eventType] =
                    Delegate.Combine(existing, listener);
            }
            else
            {
                typedSubscribers[eventType] = listener;
            }
        }

        

        public static void Unsubscribe<TEvent>(Action<TEvent> listener)
            where TEvent : IGameEvent
        {
            Type eventType = typeof(TEvent);

            if (!typedSubscribers.TryGetValue(eventType, out Delegate existing))
                return;

            Delegate current = Delegate.Remove(existing, listener);

            if (current == null)
            {
                typedSubscribers.Remove(eventType);
            }
            else
            {
                typedSubscribers[eventType] = current;
            }
        }

        

        public static void SubscribeToCategory(
            GameEventCategory category,
            Action<IGameEvent> listener)
        {
            if (categorySubscribers.TryGetValue(category, out var existing))
            {
                categorySubscribers[category] = existing + listener;
            }
            else
            {
                categorySubscribers[category] = listener;
            }
        }

        

        public static void SubscribeToAll(Action<IGameEvent> listener)
        {
            allSubscribers += listener;
        }

        

        public static void Publish<TEvent>(TEvent gameEvent)
            where TEvent : IGameEvent
        {
            PublishTyped(gameEvent);
            PublishCategory(gameEvent);
            PublishAll(gameEvent);
        }

        

        private static void PublishTyped<TEvent>(TEvent gameEvent)
            where TEvent : IGameEvent
        {
            Type eventType = typeof(TEvent);

            if (!typedSubscribers.TryGetValue(eventType, out Delegate listeners))
                return;

            foreach (Delegate listener in listeners.GetInvocationList())
            {
                try
                {
                    ((Action<TEvent>)listener).Invoke(gameEvent);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        

        private static void PublishCategory(IGameEvent gameEvent)
        {
            if (!categorySubscribers.TryGetValue(
                    gameEvent.Category,
                    out Action<IGameEvent> listeners))
                return;

            foreach (Delegate listener in listeners.GetInvocationList())
            {
                try
                {
                    ((Action<IGameEvent>)listener).Invoke(gameEvent);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        

        private static void PublishAll(IGameEvent gameEvent)
        {
            if (allSubscribers == null)
                return;

            foreach (Delegate listener in allSubscribers.GetInvocationList())
            {
                try
                {
                    ((Action<IGameEvent>)listener).Invoke(gameEvent);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }

    
    // 4. Игровые события
    

    public struct CoinCollectedEvent : IGameEvent
    {
        public string PlayerName { get; }
        public int Amount { get; }

        public GameEventCategory Category => GameEventCategory.Economy;

        public DateTime Time { get; }

        public CoinCollectedEvent(string playerName, int amount)
        {
            PlayerName = playerName;
            Amount = amount;
            Time = DateTime.Now;
        }
    }


    public struct ItemBoughtEvent : IGameEvent
    {
        public string BuyerName { get; }
        public string ItemName { get; }
        public int Price { get; }

        public GameEventCategory Category => GameEventCategory.Shop;

        public DateTime Time { get; }

        public ItemBoughtEvent(
            string buyerName,
            string itemName,
            int price)
        {
            BuyerName = buyerName;
            ItemName = itemName;
            Price = price;

            Time = DateTime.Now;
        }
    }


    public struct SubscriptionChangedEvent : IGameEvent
    {
        public string PlayerName { get; }

        public bool IsSubscribed { get; }

        public GameEventCategory Category => GameEventCategory.System;

        public DateTime Time { get; }

        public SubscriptionChangedEvent(
            string playerName,
            bool isSubscribed)
        {
            PlayerName = playerName;
            IsSubscribed = isSubscribed;

            Time = DateTime.Now;
        }
    }

    
    // 5. Наследование персонажей
    

    public abstract class Character
    {
        protected static readonly Random random = new Random();

        public string Name { get; }

        public int Coins { get; protected set; }

        protected Character(string name)
        {
            Name = name;
        }

        public virtual void CollectCoins()
        {
            int collected = random.Next(5, 8);

            Coins += collected;

            GameEventBus.Publish(
                new CoinCollectedEvent(Name, collected));
        }
    }


    public class Player : Character
    {
        public bool IsSubscribed { get; private set; }

        public Player(string name)
            : base(name)
        {
        }

        public void Subscribe()
        {
            IsSubscribed = true;

            GameEventBus.Publish(
                new SubscriptionChangedEvent(Name, true));
        }

        public void Unsubscribe()
        {
            IsSubscribed = false;

            GameEventBus.Publish(
                new SubscriptionChangedEvent(Name, false));
        }

        public override void CollectCoins()
        {
            int collected = random.Next(5, 8);

            Coins += collected;

            Console.WriteLine(
                $"{Name} собрал {collected} коинов");

            GameEventBus.Publish(
                new CoinCollectedEvent(Name, collected));
        }

        public void BuyItem(ShopItem item)
        {
            if (item.Stock <= 0)
            {
                Console.WriteLine("Товар закончился");
                return;
            }

            if (Coins < item.Price)
            {
                Console.WriteLine("Недостаточно коинов");
                return;
            }

            Coins -= item.Price;

            item.Stock--;

            Console.WriteLine(
                $"{Name} купил: {item.Name}");

            GameEventBus.Publish(
                new ItemBoughtEvent(
                    Name,
                    item.Name,
                    item.Price));
        }
    }



    public class BotPlayer : Character
    {
        public BotPlayer(string name)
            : base(name)
        {
        }

        public override void CollectCoins()
        {
            int collected = random.Next(5, 8);

            Coins += collected;

            Console.WriteLine(
                $"{Name} автоматически собрал {collected} коинов");

            GameEventBus.Publish(
                new CoinCollectedEvent(Name, collected));
        }
    }

    
    // 6. Магазин
    

    public class ShopItem
    {
        public string Name { get; }

        public int Price { get; }

        public int Stock { get; set; }

        public ShopItem(
            string name,
            int price,
            int stock)
        {
            Name = name;
            Price = price;
            Stock = stock;
        }
    }


    public class ShopSystem
    {
        private readonly List<ShopItem> items =
            new List<ShopItem>();

        public ShopSystem()
        {
            items.Add(new ShopItem("Носки", 3, 40));
            items.Add(new ShopItem("Шапка клоуна", 6, 3));
            items.Add(new ShopItem("Старая консоль", 20, 5));
            items.Add(new ShopItem("PlayStation", 120, 2));
            items.Add(new ShopItem("Квартира в Москве", 200, 1));

            items.Add(new ShopItem("Энергетик", 5, 10));
            items.Add(new ShopItem("Кот", 25, 2));
            items.Add(new ShopItem("RTX 5090", 350, 1));
            items.Add(new ShopItem("Ламборгини", 500, 1));
            items.Add(new ShopItem("Космический корабль", 2000, 1));

            GameEventBus.Subscribe<ItemBoughtEvent>(OnItemBought);
        }

        private void OnItemBought(ItemBoughtEvent itemEvent)
        {
            Console.WriteLine(
                $"[SHOP EVENT] {itemEvent.BuyerName} купил " +
                $"{itemEvent.ItemName} за {itemEvent.Price}");
        }

        public void ShowAvailableItems(Player player)
        {
            Console.WriteLine();
            Console.WriteLine("===== МАГАЗИН =====");

            List<ShopItem> available =
                items.Where(i => i.Price <= player.Coins && i.Stock > 0)
                     .ToList();

            if (available.Count == 0)
            {
                Console.WriteLine("Вы слишком бедны");
                return;
            }

            for (int i = 0; i < available.Count; i++)
            {
                Console.WriteLine(
                    $"{i + 1}. {available[i].Name} - " +
                    $"{available[i].Price} коинов | " +
                    $"Осталось: {available[i].Stock}");
            }

            Console.WriteLine("0. Ничего не покупать");

            Console.Write("Выбор: ");

            string input = Console.ReadLine();

            if (!int.TryParse(input, out int index))
                return;

            if (index == 0)
                return;

            if (index < 1 || index > available.Count)
                return;

            player.BuyItem(available[index - 1]);
        }
    }

    
    // 7. Логгер событий
    

    public class EventLogger
    {
        public EventLogger()
        {
            GameEventBus.SubscribeToAll(OnAnyEvent);
        }

        private void OnAnyEvent(IGameEvent gameEvent)
        {
            Console.WriteLine(
                $"[EVENT] {gameEvent.GetType().Name} | " +
                $"Категория: {gameEvent.Category}");
        }
    }

    
    // 8. Главная программа
    

    internal class Program
    {
        private static void Main(string[] args)
        {
            Player player = new Player("Игрок");

            BotPlayer bot1 = new BotPlayer("Бот_1");
            BotPlayer bot2 = new BotPlayer("Бот_2");

            ShopSystem shop = new ShopSystem();

            EventLogger logger = new EventLogger();

            Console.WriteLine(
                "=== Observer Pattern / EventBus ===");

            Console.WriteLine();

            bool firstAsk = true;



            for (int tick = 1; tick <= 10; tick++)
            {
                Console.WriteLine();
                Console.WriteLine($"===== ТАКТ {tick} =====");


                // Подписка


                if (firstAsk || !player.IsSubscribed)
                {
                    Console.WriteLine();
                    Console.Write("Подписаться на рассылку магазина? (y/n): ");

                    string subInput = Console.ReadLine();

                    if (subInput.ToLower() == "y")
                    {
                        player.Subscribe();
                    }
                    else
                    {
                        player.Unsubscribe();
                    }

                    firstAsk = false;
                }
                // Сбор коинов


                Console.WriteLine();

                player.CollectCoins();

                bot1.CollectCoins();

                bot2.CollectCoins();

                Console.WriteLine();
                Console.WriteLine(
                    $"Ваш баланс: {player.Coins}");

                // Магазин

                if (player.IsSubscribed)
                {
                    shop.ShowAvailableItems(player);

                    Console.WriteLine();

                    Console.Write("Отписаться? (y/n): ");

                    string unsub = Console.ReadLine();

                    if (unsub.ToLower() == "y")
                    {
                        player.Unsubscribe();
                    }
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine(
                        "Вы богатый голый бомж");
                }
            }


            Console.WriteLine();
            Console.WriteLine("===== ИГРА ОКОНЧЕНА =====");

            Console.WriteLine(
                $"Ваш итоговый баланс: {player.Coins}");

            Console.ReadKey();
        }
    }
}