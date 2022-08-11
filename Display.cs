﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFX
{
    public class Display
    {
        public Display()
        {

        }

        public void setPixel(Color c, int x, int y)
        {
            int index = (c.R > 128 | c.G > 128 | c.B > 128) ? 8 : 0;
            index |= (c.R > 64) ? 4 : 0;
            index |= (c.G > 64) ? 2 : 0;
            index |= (c.B > 64) ? 1 : 0;
            Console.BackgroundColor = (System.ConsoleColor)index;
            int left = Console.CursorLeft;
            int top = Console.CursorTop;
            ConsoleColor bgc = Console.BackgroundColor;
            Console.SetCursorPosition(x, y);
            Console.Write(" ");
            Console.SetCursorPosition(left, top);
            Console.BackgroundColor = bgc;
        }
    }
}
