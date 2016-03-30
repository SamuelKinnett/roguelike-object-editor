using System;

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

		public void InitialiseProgressBar(int x, int y, int width) {
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
				Console.SetCursorPosition (c, y);
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
				if (c >= previousBarSize)
					Console.Write ("█");
				else
					Console.SetCursorPosition (progressBarX + 1 + c, progressBarY + 1);
			}
			if (currentValue == totalValue) {
				Console.BackgroundColor = ConsoleColor.White;
				Console.ForegroundColor = ConsoleColor.Black;
				Console.SetCursorPosition (progressBarX + (progressBarWidth / 2) - 2, progressBarY + 1);
				Console.Write ("DONE");
				Console.ForegroundColor = ConsoleColor.White;
				Console.BackgroundColor = ConsoleColor.Black;
			} else {
				string percentageToWrite = (int)((currentValue / totalValue) * 100) + "%";
				for (int c = progressBarWidth / 2 - (percentageToWrite.Length / 2); c < progressBarWidth / 2 ; ++c) {
					if (c <= barSize) {
						Console.ForegroundColor = ConsoleColor.Black;
						Console.BackgroundColor = ConsoleColor.White;
					} else {
						Console.ForegroundColor = ConsoleColor.White;
						Console.ForegroundColor = ConsoleColor.Black;
					}

				}
			}
			previousBarSize = barSize;
			Console.SetCursorPosition (0, 0);
		}

        public string DisplayList(string[] titlePrompt, string[] items)
        {
            int selectedItem = 0;
            int pageNumber = 0;
            int pages = 0;
            int itemsPerPage = 0;
            int numberOfItems = items.Length;

            Console.CursorVisible = false;

            itemsPerPage = Console.WindowHeight - titlePrompt.Length;
            pages = (int)Math.Ceiling((float)items.Length / itemsPerPage);

            bool itemSelected = false;

            //Add spaces to the prompts to make them fill the width of the console
            for (int curPrompt = 0; curPrompt < titlePrompt.Length; ++curPrompt)
                if (titlePrompt[curPrompt].Length < Console.WindowWidth - 1)
                {
                    int requiredSpaces = Console.WindowWidth - 1 - titlePrompt[curPrompt].Length;
                    for (int temp = 0; temp < requiredSpaces; ++temp)
                        titlePrompt[curPrompt] += " ";
                }
                else
                    titlePrompt[curPrompt] = titlePrompt[curPrompt].Substring(0, Console.WindowWidth - 1);

            while (!itemSelected) {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Magenta;
                for (int curPrompt = 0; curPrompt < titlePrompt.Length; ++curPrompt)
                {
                    Console.WriteLine(titlePrompt[curPrompt]);
                }
                if (pages > 1)
                {
                    string pageInfo = "page " + (pageNumber + 1) + " / " + pages;
                    Console.SetCursorPosition(Console.WindowWidth - 1 - pageInfo.Length, 0);
                    Console.Write(pageInfo);
                    Console.SetCursorPosition(0, titlePrompt.Length);
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;

                for (int c = (pageNumber * itemsPerPage); c < (pageNumber * itemsPerPage) + itemsPerPage; ++c) {
                    if (c == selectedItem)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.WriteLine(items[c]);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    else if (c < numberOfItems)
                    {
                        Console.WriteLine(items[c]);
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

            Console.Clear();
            Console.CursorVisible = true;

            return items[selectedItem];
        }
	}
}

