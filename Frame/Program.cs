using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame
{
    static class Program
    {
        [STAThread] //prevent crashing
        static void Main(string[] args)
        {
            using (Game0 game = new Game0())
            {
                game.Run();
            }
        }
    }
}
