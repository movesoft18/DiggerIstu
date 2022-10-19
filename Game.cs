using System.Collections.Generic;
using System.Windows.Forms;

namespace Digger
{
    public static class Game
    {
        private const string mapWithPlayerTerrain = @"
TTT T
TTP T
T T T
TT TT";

        private const string mapWithPlayerTerrainSackGold = @"
PTTGTT TS
TST  TSTT
TTTTTTSTT
T TSTS TT
T TTTG ST
TSTSTT TT";

        // m - монстр M - командир
        private const string mapWithPlayerTerrainSackGoldMonster = @"
PTTGTT TST
TST  T TTm
TTT TT TTT
T TSTS TTT
T TTTGmSTS
T TMT m TS
TSTSTTmTTT
S TTST  TG
 TGST mTTT
 T  TMTTTT";

        public static ICreature[,] Map;
        public static int Scores;
        public static bool IsOver;

        public static Keys KeyPressed;
        public static int MapWidth => Map.GetLength(0);
        public static int MapHeight => Map.GetLength(1);

        //список монстров
        public static List<Monster> monsters = new List<Monster>();
        // список командиров
        public static List<CommanderMonster> commanderMonsters = new List<CommanderMonster>();

        public static void CreateMap()
        {
            Map = CreatureMapCreator.CreateMap(mapWithPlayerTerrainSackGoldMonster);
        }
    }
}