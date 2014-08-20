using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ngot.Utils
{
    public class Randoms
    {
        private static Random _rando = new Random();
        private static int Rand(int min, int max)
        {
            return _rando.Next(min, max + 1);
        }
        private static string[] Hash = { "a", "z", "e", "r", "t", "y", "u", "i", "o", "p", "q", "s", "d", "f", "g", "h", "j", "k", "l", "m", "w", "x", "c", "v", "b", "n" };
        private static Random Randomizer = new Random();

        public static string RandomString(int lenght)
        {
            string str = string.Empty;
            for (int i = 1; i <= lenght; i++)
            {
                int randomInt = Randomizer.Next(0, Hash.Length);
                str += Hash[randomInt];
            }
            return str;
        }
    }
}
