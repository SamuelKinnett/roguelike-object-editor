using System;

namespace roguelikeobjecteditor.src
{
	public class Editor
	{
		GUI gui;
        FileManager fileManager;
        bool NPCFileExists;

		public Editor ()
		{
			gui = new GUI ();
            fileManager = new FileManager(gui);
            NPCFileExists = false;
		}

		public void Run() {

            //fileManager.LoadUserPrefs();
            //Console.ReadLine();

            string test1 = gui.GetInput(2, 2, "Test");
            Console.Clear();
            Console.WriteLine(test1);
            Console.ReadLine();

            bool exitProgram = false;

            while (!exitProgram)
            {
                Console.Clear();
                string menuChoice = gui.DisplayList(0, 0, Console.WindowWidth - 1, Console.WindowHeight - 1, "Main Menu",
                    new string[] { "Add NPC", "Modify NPC", "Options", "Exit" });
                switch (menuChoice)
                {
                    case "Add NPC":
                        fileManager.AddNPC();
                        break;

                    case "Modify NPC":
                        if (NPCFileExists) ;
                        else
                            gui.DisplayMessageBox("No NPC file was found", "ERROR");
                        break;

                    case "Options":

                        break;

                    case "Exit":
                        gui.DisplayMessageBox("Shutting Down");
                        exitProgram = true;
                        break;
                }
            }
		}
	}
}

