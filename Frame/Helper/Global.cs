using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame
{
    static class Global
    {
        public static Game0 game;
        public static Random random = new Random();
        public static string levelname;

        public static void Initialize(Game0 inputGame)
        {
            game = inputGame;
        }
    }
}
