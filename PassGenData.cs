using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager
{
    class PassGenData
    {
        public static string[] low = new string[26]
        {
            "q", "w", "e", "r", "t", "y", "u", "i", "o", "p", "a", "s", "d",
            "f", "g", "h", "j", "k", "l", "z", "x", "c", "v", "b", "n", "m"
        };

        public static string[] up = new string[26]
        {
            "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "A", "S", "D",
            "F", "G", "H", "J", "K", "L", "Z", "X", "C", "V", "B", "N", "M"
        };

        public static string[] num = new string[10]
        {
            "0","1","2","3","4","5","6","7","8","9"
        };

        public static string[] symb = new string[12]
        {
            "!","@","#","$","%","^","&","*","_","-","+","="
        };
    }
}
