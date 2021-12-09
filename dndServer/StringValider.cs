using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Server
{
    class StringValider
    {
        public static StringValider instance;

        char[] AllowedSigns = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'r', 's', 't', 'u', 'w', 'v', 'x', 'z',
                            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'R', 'S', 'T', 'U', 'W', 'V', 'X', 'Z',
                            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                            ' ', ',', '.', '/', '\n', '-', '_', '+', '=', '!'};

        public bool isTextFine(string _msg)
        {
            foreach (char c in _msg)
            {
                if (!AllowedSigns.Contains(c))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
