using System;

namespace roguelikeobjecteditor.src
{
	public class Editor
	{
		GUI gui;
        FileManager fileManager;

		public Editor ()
		{
			gui = new GUI ();
            fileManager = new FileManager(gui); 
		}

		public void Run() {

            fileManager.LoadUserPrefs();
            Console.ReadLine();

            /*
			Console.CursorVisible = false;
			gui.InitialiseProgressBar (2, 2, 60);
			Console.ReadLine ();
			for (int temp = 0; temp <= 10000; ++temp) {
				gui.UpdateProgressBar (temp, 10000);
			}
			Console.ReadLine ();
            string[] test = new string[]{"wew", "lad", "ses"};
            string returnedValue = gui.DisplayList("Top Jej", test);
            Console.WriteLine(returnedValue);
             * */
		}

	}
}

