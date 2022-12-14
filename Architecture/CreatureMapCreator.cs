using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Digger
{
    public static class CreatureMapCreator
    {
        private static readonly Dictionary<string, Func<ICreature>> factory = new Dictionary<string, Func<ICreature>>();

        public static ICreature[,] CreateMap(string map, string separator = "\r\n")
        {
            var rows = map.Split(new[] {separator}, StringSplitOptions.RemoveEmptyEntries);
            if (rows.Select(z => z.Length).Distinct().Count() != 1)
                throw new Exception($"Wrong test map '{map}'");
            var result = new ICreature[rows[0].Length, rows.Length];
            for (var x = 0; x < rows[0].Length; x++)
            {
                for (var y = 0; y < rows.Length; y++)
                {
                    result[x, y] = CreateCreatureBySymbol(rows[y][x]);
                    // Проверяем создали ли монстра или командира и добавляем их в соответствующие списки
                    // Чтобы потом распределить подчиненных по командирам
                    if (result[x, y] is CommanderMonster)
                    {
                        Game.commanderMonsters.Add(result[x, y] as CommanderMonster);
                    }
                    else if (result[x, y] is Monster)
                    {
                        Game.monsters.Add(result[x, y] as Monster);
                    }
                }
            }
            DistributeMonstersByCommanders();
            return result;
        }

        // Распределяем подчиненных по командирам (кому сколько достанется)
        private static void DistributeMonstersByCommanders() 
        {
            int ccount = Game.commanderMonsters.Count;
            int mcount = Game.monsters.Count;
            int index = 0;
            while (index < mcount)
            {
                for (int c = 0; c < ccount; c++)
                {
                    Game.commanderMonsters[c].AddSlaveMonster(Game.monsters[index++]);
                    if (index >= mcount) break;
                }
            }

        }

        private static ICreature CreateCreatureByTypeName(string name)
        {
            // Это использование механизма рефлексии. 
            // Ему посвящена одна из последних лекций второй части курса Основы программирования
            // В обычном коде можно было обойтись без нее, но нам нужно было написать такой код,
            // который работал бы, даже если вы ещё не создали класс Monster или Gold. 
            // Просто написать new Gold() мы не могли, потому что это не скомпилировалось бы до создания класса Gold.
            if (!factory.ContainsKey(name))
            {
                var type = Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .FirstOrDefault(z => z.Name == name);
                if (type == null)
                    throw new Exception($"Can't find type '{name}'");
                factory[name] = () => (ICreature) Activator.CreateInstance(type);
            }

            return factory[name]();
        }


        private static ICreature CreateCreatureBySymbol(char c)
        {
            switch (c)
            {
                case 'P':
                    return CreateCreatureByTypeName("Player");
                case 'T':
                    return CreateCreatureByTypeName("Terrain");
                case 'G':
                    return CreateCreatureByTypeName("Gold");
                case 'S':
                    return CreateCreatureByTypeName("Sack");
                case 'm':
                    return CreateCreatureByTypeName("Monster");
                case 'M':
                    return CreateCreatureByTypeName("CommanderMonster");
                case ' ':
                    return null;
                default:
                    throw new Exception($"wrong character for ICreature {c}");
            }
        }
    }
}