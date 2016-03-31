using System;
using System.Collections.Generic;

namespace roguelikeobjecteditor.src
{
	public class GUI
	{
		int progressBarX;
		int progressBarY;
		int progressBarWidth;
		int previousBarSize;


		public GUI ()
		{
		}

		public void InitialiseProgressBar(int x, int y, int width, string title = "") {
			progressBarX = x;
			progressBarY = y;
			progressBarWidth = width;

			Console.SetCursorPosition (x, y);
			Console.Write ("┌");
			Console.SetCursorPosition (x + width, y);
			Console.Write ("┐");
			Console.SetCursorPosition (x, y + 2);
			Console.Write ("└");
			Console.SetCursorPosition (x + width, y + 2);
			Console.Write ("┘");

			for (int c = x + 1; c < x + width; ++c) {
                //If a title has been provided and we need to draw a letter here then do so.
                Console.SetCursorPosition(c, y);
                if (title != "" && c - (x + 1) < title.Length)
                    Console.Write(title.Substring(c - (x + 1), 1));
                else
				    Console.Write ("─");
				Console.SetCursorPosition (c, y + 2);
				Console.Write ("─");
			}
			Console.SetCursorPosition (x, y + 1);
			Console.Write ("│");
			Console.SetCursorPosition (x + width, y + 1);
			Console.Write ("│");

			previousBarSize = 0;
		}

		public void UpdateProgressBar(float currentValue, float totalValue) {
			int barSize = (int)((currentValue / totalValue) * (progressBarWidth));
			Console.SetCursorPosition (progressBarX + 1, progressBarY + 1);
			for (int c = 0; c < barSize; ++c) {
                if (c >= previousBarSize && barSize < progressBarWidth)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.SetCursorPosition(progressBarX + 1 + c, progressBarY + 1);
                    Console.Write("█");
                    Console.ForegroundColor = ConsoleColor.White;
                }
			}
			if (currentValue == totalValue) {
				Console.BackgroundColor = ConsoleColor.Magenta;
				Console.ForegroundColor = ConsoleColor.White;
				Console.SetCursorPosition (progressBarX + (progressBarWidth / 2) - 2, progressBarY + 1);
				Console.Write ("DONE");
				Console.ForegroundColor = ConsoleColor.White;
				Console.BackgroundColor = ConsoleColor.Black;
			} else {
				string percentageToWrite = (int)((currentValue / totalValue) * 100) + "%";
                int curChar = 0;
                int startPos = (progressBarWidth - 2) / 2 - (percentageToWrite.Length / 2);
                for (int c = startPos; c < startPos + percentageToWrite.Length; ++c) {
					if (c <= barSize) {
						Console.ForegroundColor = ConsoleColor.White;
						Console.BackgroundColor = ConsoleColor.Magenta;
                        Console.SetCursorPosition(progressBarX + c + 1, progressBarY + 1);
                        Console.Write(percentageToWrite.Substring(curChar, 1));
                        ++curChar;
					} else {
						Console.ForegroundColor = ConsoleColor.White;
						Console.BackgroundColor = ConsoleColor.Black;
                        Console.SetCursorPosition(progressBarX + c + 1, progressBarY + 1);
                        Console.Write(percentageToWrite.Substring(curChar, 1));
                        ++curChar;
					}
				}
			}
			previousBarSize = barSize;
			Console.SetCursorPosition (0, 0);
		}

        public string DisplayList(int x, int y, int width, int height, string title, string[] items)
        {
            int selectedItem = 0;
            int pageNumber = 0;
            int pages = 0;
            int itemsPerPage = 0;
            int numberOfItems = items.Length;
            string clearString = ""; //The string written to clear a line in the menu

            Console.CursorVisible = false;

            itemsPerPage = height - 1;
            pages = (int)Math.Ceiling((float)items.Length / itemsPerPage);

            bool itemSelected = false;

            Console.ForegroundColor = ConsoleColor.White;

            DrawBox(x, y, width, height, title);

            //Populate the clear string
            for (int i = x + 1; i < x + width; ++i)
                clearString += " ";

            while (!itemSelected) {
                Console.SetCursorPosition(x + 1, y + 1);
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;

                //If there are multiple pages, write this information in the top right corner
                if (pages > 1)
                {
                    string pageInfo = "page " + (pageNumber + 1) + "/" + pages;
                    Console.SetCursorPosition(width - 1 - pageInfo.Length, y);
                    Console.Write(pageInfo);
                }

                for (int c = (pageNumber * itemsPerPage); c < (pageNumber * itemsPerPage) + itemsPerPage; ++c) {

                    string textToWrite = "";
                    Console.SetCursorPosition(x + 1, y + 1 + (c - (pageNumber * itemsPerPage)));
                    Console.Write(clearString);
                    Console.SetCursorPosition(x + 1, y + 1 + (c - (pageNumber * itemsPerPage)));

                    if (c < numberOfItems)
                    {
                        if (items[c].Length > width - 2)
                            textToWrite = items[c].Substring(0, width - 2);
                        else
                            textToWrite = items[c];

                        if (c == selectedItem)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Magenta;
                            Console.Write(textToWrite);
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Black;
                        }
                        else
                        {
                            Console.Write(textToWrite);
                        }
                    }
                }

                Console.SetCursorPosition(0, 0);

                ConsoleKey pressedKey = Console.ReadKey(true).Key;

                switch (pressedKey)
                {
                    case ConsoleKey.UpArrow:
                        if (selectedItem > 0)
                            --selectedItem;
                        break;

                    case ConsoleKey.DownArrow:
                        if (selectedItem < items.Length - 1)
                            selectedItem++;
                        break;

                    case ConsoleKey.RightArrow:
                        if (selectedItem < items.Length - 5)
                            selectedItem += 5;
                        else
                            selectedItem = items.Length - 1;
                        break;

                    case ConsoleKey.LeftArrow:
                        if (selectedItem - 5 > 0)
                            selectedItem -= 5;
                        else
                            selectedItem = 0;
                        break;

                    case ConsoleKey.Enter:
                        itemSelected = true;
                        break;

                    case ConsoleKey.S:
                        itemSelected = true;
                        items[selectedItem] = "ALTSELECT";
                        break;

                    case ConsoleKey.PageDown:
                        if (pageNumber < pages - 1)
                        {
                            ++pageNumber;
                            selectedItem = pageNumber * itemsPerPage;
                        }
                        break;

                    case ConsoleKey.PageUp:
                        if (pageNumber > 0)
                        {
                            --pageNumber;
                            selectedItem = pageNumber * itemsPerPage;
                        }
                        break;
                }

                pageNumber = (int)Math.Floor((float)selectedItem / (float)itemsPerPage);

            }

            Console.CursorVisible = true;

            return items[selectedItem];
        }

        public void DisplayMessageBox(string message, int width, string title = "")
        {
            int x, y;
            int height = (int)Math.Ceiling(message.Length / (float)(width - 1)) + 1;

            //make the box centred
            y = Console.WindowHeight / 2 - height / 2;
            x = Console.WindowWidth / 2 - width / 2;

            DrawMessageBox(x, y, width, height, message, title);
        }

        public void DisplayMessageBox(string message, string title = "")
        {
            int width = message.Length + 1;
            int x, y;
            int height = (int)Math.Ceiling(message.Length / (float)(width - 1)) + 1;

            //make the box centred
            y = Console.WindowHeight / 2 - height / 2;
            x = Console.WindowWidth / 2 - width / 2;

            DrawMessageBox(x, y, width, height, message, title);
        }

        public void DisplayMessageBox(int x, int y, int width, string message, string title = "")
        {
            int height = (int)Math.Ceiling(message.Length / (float)(width - 1)) + 1;
            DrawMessageBox(x, y, width, height, message, title);
        }

        public void DrawBox(int x, int y, int width, int height, string title = "")
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;

            //Draw the box border
            Console.SetCursorPosition(x, y);
            Console.Write("┌");
            Console.SetCursorPosition(x + width, y);
            Console.Write("┐");
            Console.SetCursorPosition(x, y + height);
            Console.Write("└");
            Console.SetCursorPosition(x + width, y + height);
            Console.Write("┘");

            for (int c = x + 1; c < x + width; ++c)
            {
                //If a title has been provided and we need to draw a letter here then do so.
                Console.SetCursorPosition(c, y);
                if (title != "" && c - (x + 1) < title.Length)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(title.Substring(c - (x + 1), 1));
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.Write("─");
                }
                Console.SetCursorPosition(c, y + height);
                Console.Write("─");
            }

            for (int i = y + 1; i < y + height; ++i)
            {
                Console.SetCursorPosition(x, i);
                Console.Write("│");
                Console.SetCursorPosition(x + width, i);
                Console.Write("│");
            }
        }

        public void DrawMessageBox(int x, int y, int width, int height, string message, string title)
        {

            Console.CursorVisible = false;

            Console.ForegroundColor = ConsoleColor.White;

            DrawBox(x, y, width, height, title);

            for (int i = y + 1; i < y + height; ++i)
            {
                Console.SetCursorPosition(x + 1, i);
                int chunkSize = width - 1;
                int startPos = ((i - (y + 1)) * chunkSize);

                if (startPos + chunkSize < message.Length)
                    Console.Write(message.Substring(startPos, chunkSize));
                else
                    Console.Write(message.Substring(startPos));
            }

            Console.SetCursorPosition(0, 0);
            Console.ReadLine();
            Console.CursorVisible = true;
        }

        public string GetInput(int x, int y, string prompt = "")
        {
            List<char> input = new List<char>();
            int width = prompt.Length + 1;
            bool inputCompleted = false;

            while (!inputCompleted)
            {
                string clearString = "";

                for (int i = 0; i <= width; ++i)
                    clearString += " ";

                Console.SetCursorPosition(x, y);
                Console.Write(clearString);
                Console.SetCursorPosition(x, y + 1);
                Console.Write(clearString);
                Console.SetCursorPosition(x, y + 2);
                Console.Write(clearString);

                if (prompt.Length + 1 > input.Count + 1)
                    width = prompt.Length + 1;
                else
                    width = input.Count + 1;

                DrawBox(x, y, width, 2, prompt);
                Console.SetCursorPosition(x + 1, y + 1);
                foreach(char curChar in input)
                    Console.Write(curChar);

                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.KeyChar > 32 && key.KeyChar < 127) {
                    /*
                    if ((key.Modifiers & ConsoleModifiers.Shift) != 0)
                        input.Add((char)(int)key.KeyChar);
                    else
                        input.Add((char)((int)key.KeyChar + 32));
                    */
                    input.Add(key.KeyChar);
                }
                else
                {
                    switch (key.Key)
                    {
                        case ConsoleKey.Backspace:
                        if (input.Count > 0)
                            input.RemoveAt(input.Count - 1);
                            break;

                        case ConsoleKey.Enter:
                            inputCompleted = true;
                            break;

                        case ConsoleKey.Spacebar:
                            input.Add(' ');
                            break;
                    {

                    }
                }
                }


            }

            string returnString = "";
            foreach (char curChar in input)
                returnString += curChar;
            return returnString;
        }
	}
}

