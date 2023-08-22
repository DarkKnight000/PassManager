using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager
{
    class Crypt
    {

        public static string key = "K1b8JNBL6PrqBRyb";

        static char[] characters = new char[]           // 11x13+5 = 148!
        {
            'Ь', 'х', 'F', 'Ю', '2', ':', '*', 'ё', 'Л', 'r', '$', 'v', 'щ',
            'Ч', 'У', 'Е', 'э', 'ф', '!', 'a', 'ъ', '_', 'V', 'о', 'Ц', '&', 
            'x', 'k', '0', 'А', 'Д', 'Я', 'я', '8', 'Э', 'h', 'а', 'Ш', 'Г', 
            'М', 'п', 'Т', 'Q', 'р', '1', 'n', '+', 'E', 'b', 'л', ' ', 'y', 
            'H', '%', 'г', '/', 'C', 'Z', 'б', 'ю', 'g', 's', 'T', '5', 'j', 
            '^', 'w', 't', 'з', 'у', '6', 'Ж', 'I', 'U', 'W', 'X', 'D', '#', 
            'т', 'A', 'K', 'N', 'С', '4', 'К', '7', 'Б', 'Ы', 'в', 'z', 'Ъ', 
            '@', 'd', 'ы', 'й', 'Y', 'P', 'И', 'ц', 'Н', 'l', 'е', 'B', '3', 
            'Й', 'и', 'ч', 'S', 'c', 'к', '=', 'ь', 'p', 'ж', 'L', 'f', 'Щ', 
            'О', 'н', 'Х', 'o', 'З', 'i', 'M', 'В', 'Р', 'q', 'П', 'O', '(',
            '9', 'R', '-', 'u', 'G', 'с', 'J', 'Ф', 'м', 'Ё', 'e', 'д', 'ш',
            'm', ')', '.', ',', '?'
        };

        int N = characters.Length;

        // Шифровка:
        public string Encode(string input, string keyword)
        {
            string result = "";

            int c;
            int keyword_index = 0;

            foreach (char symbol in input)
            {
                if (Array.IndexOf(characters, symbol) < 0)
                {
                    result += symbol;
                }
                else
                {
                    c = (Array.IndexOf(characters, symbol) +
                         Array.IndexOf(characters, keyword[keyword_index])) % N;

                    result += characters[c];
                }

                keyword_index++;

                if ((keyword_index + 1) == keyword.Length)
                    keyword_index = 0;
            }

            return result;
        }

        // Дешифровка:
        public string Decode(string input, string keyword)
        {
            string result = "";

            int p;
            int keyword_index = 0;

            foreach (char symbol in input)
            {
                if (Array.IndexOf(characters, symbol) < 0)
                {
                    result += symbol;
                }
                else
                {
                    p = (Array.IndexOf(characters, symbol) + N -
                         Array.IndexOf(characters, keyword[keyword_index])) % N;

                    result += characters[p];
                }

                keyword_index++;

                if ((keyword_index + 1) == keyword.Length)
                    keyword_index = 0;
            }

            return result;
        }

        // Дешифровка таблицы:
        public void Encrypt(DataTable table, int str, int kol)
        {
            for (int i = 0; i < str; i++)
            {
                for (int j = 2; j <= kol; j++)
                {
                    table.Rows[i][j] = Decode(table.Rows[i][j].ToString(), key);
                }
            }
        }

        /*public void Encrypt(int str, int kol)
        {
            for (int i = 0; i < str; i++)
            {
                for (int j = 2; j <= kol; j++)
                {
                    Data.dt_user.Rows[i][j] = Decode(Data.dt_user.Rows[i][j].ToString(), key);
                }
            }
        }*/
    }
}
