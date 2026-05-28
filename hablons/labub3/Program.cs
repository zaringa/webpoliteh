using System;
using System.Collections.Generic;

namespace FlyweightGame
{
    
    // 1. Интерфейс Flyweight
    

    public interface IGameAsset
    {
        void Render(
            string ownerName,
            string uniqueColor,
            int level,
            int durability);
    }

    
    // 2. Конкретный Flyweight
    

    public class WeaponSkin : IGameAsset
    {
        
        // ОБЩИЕ ДАННЫЕ (shared state)
        

        public string SkinName { get; }

        public string WeaponType { get; }

        public string TextureFile { get; }

        public int BaseDamage { get; }

        

        public WeaponSkin(
            string skinName,
            string weaponType,
            string textureFile,
            int baseDamage)
        {
            SkinName = skinName;
            WeaponType = weaponType;
            TextureFile = textureFile;
            BaseDamage = baseDamage;

            Console.WriteLine(
                $"[LOAD] Загружен новый asset: {SkinName}");
        }

        
        // УНИКАЛЬНЫЕ ДАННЫЕ передаются отдельно
        

        public void Render(
            string ownerName,
            string uniqueColor,
            int level,
            int durability)
        {
            Console.WriteLine(
                $"Игрок: {ownerName}");

            Console.WriteLine(
                $"Скин: {SkinName}");

            Console.WriteLine(
                $"Оружие: {WeaponType}");

            Console.WriteLine(
                $"Текстура: {TextureFile}");

            Console.WriteLine(
                $"Базовый урон: {BaseDamage}");

            Console.WriteLine(
                $"Цвет: {uniqueColor}");

            Console.WriteLine(
                $"Уровень: {level}");

            Console.WriteLine(
                $"Прочность: {durability}");

            Console.WriteLine(
                $"--------------------------------");
        }
    }

    
    // 3. Flyweight Factory
    

    public static class AssetFactory
    {
        private static readonly Dictionary<string, WeaponSkin> assets =
            new Dictionary<string, WeaponSkin>();

        

        public static WeaponSkin GetWeaponSkin(
            string skinName,
            string weaponType,
            string textureFile,
            int baseDamage)
        {
            if (assets.ContainsKey(skinName))
            {
                Console.WriteLine(
                    $"[CACHE] Используется существующий asset: {skinName}");

                return assets[skinName];
            }

            WeaponSkin newSkin =
                new WeaponSkin(
                    skinName,
                    weaponType,
                    textureFile,
                    baseDamage);

            assets.Add(skinName, newSkin);

            return newSkin;
        }

        

        public static void ShowStatistics()
        {
            Console.WriteLine();
            Console.WriteLine("===== СТАТИСТИКА =====");

            Console.WriteLine(
                $"Всего уникальных flyweight объектов: {assets.Count}");

            Console.WriteLine();
        }
    }

    
    // 4. Внешнее состояние объекта
    

    public class PlayerWeapon
    {
        
        // ССЫЛКА НА FLYWEIGHT
        

        private readonly WeaponSkin skin;

        
        // УНИКАЛЬНОЕ СОСТОЯНИЕ
        

        public string OwnerName { get; }

        public string UniqueColor { get; }

        public int Level { get; }

        public int Durability { get; }

        

        public PlayerWeapon(
            WeaponSkin skin,
            string ownerName,
            string uniqueColor,
            int level,
            int durability)
        {
            this.skin = skin;

            OwnerName = ownerName;

            UniqueColor = uniqueColor;

            Level = level;

            Durability = durability;
        }

        

        public void Draw()
        {
            skin.Render(
                OwnerName,
                UniqueColor,
                Level,
                Durability);
        }
    }

    
    // 5. Главная программа
    

    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine(
                "===== FLYWEIGHT PATTERN DEMO =====");

            Console.WriteLine();

            
            // СОЗДАНИЕ FLYWEIGHT ОБЪЕКТОВ
            

            WeaponSkin dragonSkin =
                AssetFactory.GetWeaponSkin(
                    "Dragon Fire",
                    "Sword",
                    "dragon_texture.png",
                    120);

            WeaponSkin iceSkin =
                AssetFactory.GetWeaponSkin(
                    "Ice Storm",
                    "Bow",
                    "ice_texture.png",
                    90);

            WeaponSkin cyberSkin =
                AssetFactory.GetWeaponSkin(
                    "Cyber Neon",
                    "Gun",
                    "cyber_texture.png",
                    150);

            
            // ПОВТОРНОЕ ИСПОЛЬЗОВАНИЕ
            

            WeaponSkin reusedDragon =
                AssetFactory.GetWeaponSkin(
                    "Dragon Fire",
                    "Sword",
                    "dragon_texture.png",
                    120);

            
            // ИГРОКИ
            

            List<PlayerWeapon> weapons =
                new List<PlayerWeapon>();

            weapons.Add(
                new PlayerWeapon(
                    dragonSkin,
                    "Игрок_1",
                    "Красный",
                    10,
                    90));

            weapons.Add(
                new PlayerWeapon(
                    reusedDragon,
                    "Игрок_2",
                    "Синий",
                    7,
                    70));

            weapons.Add(
                new PlayerWeapon(
                    dragonSkin,
                    "Игрок_3",
                    "Черный",
                    15,
                    100));

            weapons.Add(
                new PlayerWeapon(
                    iceSkin,
                    "Игрок_4",
                    "Белый",
                    3,
                    45));

            weapons.Add(
                new PlayerWeapon(
                    cyberSkin,
                    "Игрок_5",
                    "Фиолетовый",
                    20,
                    99));

            
            // РЕНДЕР
            

            Console.WriteLine();
            Console.WriteLine("===== РЕНДЕР ОБЪЕКТОВ =====");
            Console.WriteLine();

            foreach (PlayerWeapon weapon in weapons)
            {
                weapon.Draw();
            }

            
            // СТАТИСТИКА
            

            AssetFactory.ShowStatistics();

            Console.WriteLine(
                "Без Flyweight было бы создано 5 тяжелых объектов.");

            Console.WriteLine(
                "С Flyweight создано только 3 объекта.");

            Console.WriteLine();
            Console.WriteLine(
                "Память используется эффективнее.");

            Console.ReadKey();
        }
    }
}