using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace roguelikeobjecteditor.src
{

    class FileManager
    {

        List<NPC> npcs;

        public FileManager(GUI gui)
        {
            this.gui = gui;
        }

        StreamReader streamReader;
        StreamWriter streamWriter;
        GUI gui;

        public UserPrefs LoadUserPrefs()
        {
            UserPrefs userPrefs = new UserPrefs();

            if (File.Exists(Directory.GetCurrentDirectory() + "\\prefs.txt"))
            {
                streamReader = new StreamReader(Directory.GetCurrentDirectory() + "\\prefs.txt");
                userPrefs.folderLocation = streamReader.ReadLine();
                streamReader.Close();
            }
            else
            {
                CreateUserPrefs(SelectFolderLocation());
                streamReader = new StreamReader(Directory.GetCurrentDirectory() + "\\prefs.txt");
                userPrefs.folderLocation = streamReader.ReadLine();
                streamReader.Close();
            }


            return userPrefs;
        }

        string SelectFolderLocation()
        {
            bool folderSelected = false;
            string currentPath = Directory.GetCurrentDirectory();

            while (!folderSelected)
            {
                string[] subDirectories = Directory.GetDirectories(currentPath);
                string[] choices = new string[subDirectories.Length + 1];

                //Add an option to go up a directory level
                choices[0] = "↑";
                subDirectories.CopyTo(choices, 1);
                //Remove the preceding file path
                for (int curDir = 1; curDir < choices.Length; ++curDir) {
                    choices[curDir] = choices[curDir].Substring(currentPath.Length);
                    choices[curDir] = choices[curDir].Trim('\\');
                }
                string newDirectory = gui.DisplayList(0, 0, Console.WindowWidth - 1, Console.WindowHeight - 1,"(S)elect Location for Game Files.", choices);
                if (newDirectory == "ALTSELECT")
                {
                    folderSelected = true;
                }
                else if (newDirectory == "↑")
                {
                    string[] tempStringArray = currentPath.Split('\\');
                    if (tempStringArray.Length > 2)
                    {
                        currentPath = tempStringArray[0];
                        for (int temp = 1; temp < tempStringArray.Length - 1; ++temp)
                        {
                            currentPath += "\\" + tempStringArray[temp];
                        }
                    } else {
                        currentPath = tempStringArray[0] + "\\";
                    }
                } 
                else
                {
                    currentPath += "\\" + newDirectory;
                }
            }

            return currentPath;
        }

        public void CreateUserPrefs(string folderLocation)
        {
            streamWriter = new StreamWriter(Directory.GetCurrentDirectory() + "\\prefs.txt");
            streamWriter.WriteLine(folderLocation);
            streamWriter.Close();
        }

        public bool LoadNPCData(UserPrefs userPrefs)
        {
            Console.Clear();
            gui.InitialiseProgressBar(1, 1, Console.WindowWidth - 3, "Loading Raw NPC Data");
            if (File.Exists(userPrefs.folderLocation + "\\NPC.txt")) {
                streamReader = new StreamReader(userPrefs.folderLocation + "\\NPC.txt");
                int npcCount = int.Parse(streamReader.ReadLine());
                List<string> rawData = new List<string>();
                for (int currentNPCData = 0; currentNPCData < npcCount; ++currentNPCData)
                {
                    rawData.Add(streamReader.ReadLine());
                    gui.UpdateProgressBar(currentNPCData, npcCount);
                }

                gui.InitialiseProgressBar(1, 5, Console.WindowWidth - 3, "Parsing Raw Data");
                int totalOps = npcCount * 4;
                for (int curNPC = 0; curNPC < npcCount; ++curNPC)
                {
                    string[] npcData = rawData[curNPC].Split(';');
                    NPC npc = new NPC();

                    npc.name = npcData[0];
                    npc.levelRange = new int[2];
                    npc.levelRange[0] = int.Parse(npcData[1]);
                    npc.levelRange[1] = int.Parse(npcData[2]);
                    npc.baseEXP = float.Parse(npcData[3]);
                    npc.EXPModifier = float.Parse(npcData[4]);
                    npc.dropLevelRange = new int[2];
                    npc.dropLevelRange[0] = int.Parse(npcData[5]);
                    npc.dropLevelRange[1] = int.Parse(npcData[6]);

                    gui.UpdateProgressBar(curNPC * 4, totalOps);

                    //Now load in the NPC's stats

                    npc.stats = new NPCStats();
                    npc.stats.hp = int.Parse(npcData[7]);
                    npc.stats.combatSkill = float.Parse(npcData[8]);
                    npc.stats.damage = new float[3];
                    npc.stats.damage[0] = float.Parse(npcData[9]);
                    npc.stats.damage[1] = float.Parse(npcData[10]);
                    npc.stats.damage[2] = float.Parse(npcData[11]);
                    npc.stats.defence = new float[3];
                    npc.stats.defence[0] = float.Parse(npcData[12]);
                    npc.stats.defence[1] = float.Parse(npcData[13]);
                    npc.stats.defence[2] = float.Parse(npcData[14]);
                    npc.stats.preference = new float[3];
                    npc.stats.preference[0] = float.Parse(npcData[15]);
                    npc.stats.preference[1] = float.Parse(npcData[16]);
                    npc.stats.preference[2] = float.Parse(npcData[17]);
                    npc.stats.agility = float.Parse(npcData[18]);
                    npc.stats.perception = float.Parse(npcData[19]);
                    npc.stats.aggressive = bool.Parse(npcData[20]);
                    npc.stats.hostile = bool.Parse(npcData[21]);

                    gui.UpdateProgressBar(curNPC * 4 + 1, totalOps);

                    //Load in the bias values

                    npc.biasValues = new float[10];

                    for (int i = 0; i < 11; ++i)
                        npc.biasValues[i] = float.Parse(npcData[22 + i]);

                    gui.UpdateProgressBar(curNPC * 4 + 2, totalOps);

                    //Load in the drop types

                    if (npcData[33] != "END")
                    {
                        npc.drops = new List<ItemType>();
                        int curIndex = 33;
                        while (npcData[curIndex] != "END")
                            npc.drops.Add((ItemType)int.Parse(npcData[curIndex]));
                    }

                    gui.UpdateProgressBar(curNPC * 4 + 3, totalOps);
                }
            }
            return false;
        }

        public void SaveNPCData(UserPrefs userPrefs)
        {
            streamWriter = new StreamWriter(userPrefs.folderLocation + "\\NPC.txt", false);
            if (npcs.Count > 0)
            {
                streamWriter.WriteLine(npcs.Count);
                foreach (NPC npc in npcs)
                {
                    streamWriter.Write(npc.name + ";" + npc.levelRange[0] + ";" + npc.levelRange[1] + ";"
                        + npc.baseEXP + ";" + npc.EXPModifier + ";" + npc.dropLevelRange[0] + ";" + npc.dropLevelRange[1] + ";");
                    streamWriter.Write(npc.stats.hp + ";"
                        + npc.stats.combatSkill + ";"
                        + npc.stats.damage[0] + ";" + npc.stats.damage[1] + ";" + npc.stats.damage[2] + ";"
                        + npc.stats.defence[0] + ";" + npc.stats.defence[1] + ";" + npc.stats.defence[2] + ";"
                        + npc.stats.preference[0] + ";" + npc.stats.preference[1] + ";" + npc.stats.preference[2] + ";"
                        + npc.stats.agility + ";"
                        + npc.stats.perception + ";"
                        + npc.stats.aggressive + ";"
                        + npc.stats.hostile + ";");
                    for (int i = 0; i < 11; ++i)
                    {
                        streamWriter.Write(npc.biasValues[i] + ";");
                    }
                    for (int i = 0; i < npc.drops.Count; ++i)
                    {
                        streamWriter.Write(npc.drops[i] + ";");
                    }
                    streamWriter.Write("END");
                    streamWriter.Write("\n");
                }
            }
        }

        public bool AddNPC ()
        {
            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Add New NPC");
            Console.BackgroundColor = ConsoleColor.White;

            NPC newNPC = new NPC();
            newNPC.stats = new NPCStats();
            newNPC.levelRange = new int[2];
            newNPC.dropLevelRange = new int[2];
            newNPC.biasValues = new float[10];
            newNPC.stats.damage = new float[3];
            newNPC.stats.defence = new float[3];
            newNPC.stats.preference = new float[3];
            newNPC.drops = new List<ItemType>();

            ModifyNPC(newNPC);

            return false;
        }

        void ModifyNPC(NPC npc)
        {
            bool Finished = false;

            while (!Finished)
            {

                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();

                //Draw the current parameters to the right of the menu
                gui.DrawBox(31, 0, Console.WindowWidth - 32, Console.WindowHeight - 1, "Current Values");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(32, 1);
                Console.Write(npc.name);
                Console.SetCursorPosition(32, 2);
                Console.Write(npc.levelRange[0] + " - " + npc.levelRange[1]);
                Console.SetCursorPosition(32, 3);
                Console.Write(npc.baseEXP);
                Console.SetCursorPosition(32, 4);
                Console.Write(npc.EXPModifier);
                Console.SetCursorPosition(32, 5);
                Console.Write(npc.dropLevelRange[0] + " - " + npc.dropLevelRange[1]);
                Console.SetCursorPosition(32, 6);
                foreach (ItemType drop in npc.drops)
                    Console.Write(drop.ToString() + ", ");
                Console.SetCursorPosition(32, 7);
                Console.Write(npc.stats.hp + " (" + npc.biasValues[0] + ")");
                Console.SetCursorPosition(32, 8);
                Console.Write(npc.stats.combatSkill + " (" + npc.biasValues[1] + ")");
                Console.SetCursorPosition(32, 9);
                Console.Write(npc.stats.damage[0] + " (" + npc.biasValues[2] + ") "
                    + " / " + npc.stats.damage[1] + " (" + npc.biasValues[3] + ") "
                    + " / " + npc.stats.damage[2] + " (" + npc.biasValues[4] + ") ");
                Console.SetCursorPosition(32, 10);
                Console.Write(npc.stats.defence[0] + " (" + npc.biasValues[5] + ") "
                    + " / " + npc.stats.defence[1] + " (" + npc.biasValues[6] + ") "
                    + " / " + npc.stats.defence[2] + " (" + npc.biasValues[7] + ") ");
                Console.SetCursorPosition(32, 11);
                Console.Write(npc.stats.preference[0] + " / " + npc.stats.preference[1] + " / " + npc.stats.preference[2]);
                Console.SetCursorPosition(32, 12);
                Console.Write(npc.stats.agility + " (" + npc.biasValues[8] + ")");
                Console.SetCursorPosition(32, 13);
                Console.Write(npc.stats.perception + " (" + npc.biasValues[9] + ")");
                Console.SetCursorPosition(32, 14);
                Console.Write(npc.stats.aggressive);
                Console.SetCursorPosition(32, 15);
                Console.Write(npc.stats.hostile);

                //Draw the menu
                string menuChoice = gui.DisplayList(0, 0, 30, Console.WindowHeight - 1, "Select Parameter",
                    new string[]
                    {
                        "Name",
                        "Level Range",
                        "Base Experience Points",
                        "Experience Modifier",
                        "Drop Level Range",
                        "Drop Types",
                        "HP",
                        "Combat Skill",
                        "Damage (S/B/P)",
                        "Defence (S/B/P)",
                        "Preference (S/B/P)",
                        "Agility",
                        "Perception",
                        "Aggressive",
                        "Hostile",
                        "Save and Return"
                    });

                string subChoice = "";

                switch (menuChoice)
                {
                    case "Name":
                        Console.Clear();
                        Console.Write("Enter New Name: ");
                        npc.name = Console.ReadLine();
                        break;

                    case "Level Range":
                        subChoice = gui.DisplayList(1, 3, 20, 4, "", new string[] { "Lower Bound", "Upper Bound", "Cancel" });

                        switch (subChoice) {

                            case "Lower Bound":
                                Console.Clear();
                                Console.Write("Lower Level Bound: ");
                                npc.levelRange[0] = int.Parse(Console.ReadLine());
                                if (npc.levelRange[0] < 0)
                                {
                                    npc.levelRange[0] = 0;
                                    Console.WriteLine("Clamped Lower Level to 0");
                                }
                                else if (npc.levelRange[0] > 100)
                                {
                                    npc.levelRange[0] = 100;
                                    Console.WriteLine("Clamped Lower Level to 100");
                                }
                                break;

                            case "Upper Bound":
                                Console.Clear();
                                Console.Write("Upper Level Bound: ");
                                npc.levelRange[1] = int.Parse(Console.ReadLine());
                                if (npc.levelRange[1] < 0)
                                {
                                    npc.levelRange[1] = 0;
                                    Console.WriteLine("Clamped Lower Level to 0");
                                }
                                else if (npc.levelRange[1] > 100)
                                {
                                    npc.levelRange[1] = 100;
                                    Console.WriteLine("Clamped Lower Level to 100");
                                }
                                else if (npc.levelRange[1] < npc.levelRange[0])
                                {
                                    npc.levelRange[1] = npc.levelRange[0];
                                    Console.WriteLine("Clamped Upper Bound to Lower Bound");
                                }
                                break;
                        }
                        break;

                    case "Base Experience Points":
                        Console.Clear();
                        Console.Write("Base Experience Points: ");
                        npc.baseEXP = int.Parse(Console.ReadLine());
                        if (npc.baseEXP < 0)
                        {
                            npc.baseEXP = 0;
                            Console.WriteLine("Clamped Base Experience Points to 0");
                        }
                        break;

                    case "Experience Modifier":
                        Console.Write("Experience Modifier: ");
                        npc.EXPModifier = float.Parse(Console.ReadLine());
                        if (npc.EXPModifier < 1)
                        {
                            npc.EXPModifier = 1;
                            Console.WriteLine("Clamped Experience Modifier to 1");
                        }
                        break;

                    case "Drop Level Range":
                        subChoice = gui.DisplayList(1, 6, 20, 4, "", new string[] { "Lower Bound", "Upper Bound", "Cancel" });

                        switch (subChoice) {

                            case "Lower Bound":
                                Console.Clear();
                                Console.Write("Lower Level Bound: ");
                                npc.dropLevelRange[0] = int.Parse(Console.ReadLine());
                                if (npc.dropLevelRange[0] < 0)
                                {
                                    npc.dropLevelRange[0] = 0;
                                    Console.WriteLine("Clamped Lower Level to 0");
                                }
                                else if (npc.dropLevelRange[0] > 100)
                                {
                                    npc.dropLevelRange[0] = 100;
                                    Console.WriteLine("Clamped Lower Level to 100");
                                }
                                break;

                        case "Upper Bound":
                                Console.Clear();
                                Console.Write("Upper Level Bound: ");
                                npc.dropLevelRange[1] = int.Parse(Console.ReadLine());
                                if (npc.dropLevelRange[1] < 0)
                                {
                                    npc.dropLevelRange[1] = 0;
                                    Console.WriteLine("Clamped Lower Level to 0");
                                }
                                else if (npc.dropLevelRange[1] > 100)
                                {
                                    npc.dropLevelRange[1] = 100;
                                    Console.WriteLine("Clamped Lower Level to 100");
                                }
                                else if (npc.dropLevelRange[1] < npc.dropLevelRange[0])
                                {
                                    npc.dropLevelRange[1] = npc.dropLevelRange[0];
                                    Console.WriteLine("Clamped Upper Bound to Lower Bound");
                                }
                                break;
                }


                        break;

                    case "Drop Types":
                        subChoice = gui.DisplayList(1, 7, 20, 4, "", new string[] { "Add Drop Type", "Remove Drop Type", "Cancel" });
                        string dropChoice = "";

                        switch (subChoice)
                        {
                            case "Add Drop Type":
                                dropChoice = gui.DisplayList(2, 8, 20, 15, "", Enum.GetNames(typeof(ItemType)));
                                if (npc.drops != null)
                                {
                                    if (!npc.drops.Contains((ItemType)Enum.Parse(typeof(ItemType), dropChoice)))
                                    {
                                        npc.drops.Add((ItemType)Enum.Parse(typeof(ItemType), dropChoice));
                                        gui.DisplayMessageBox("Added Drop Type: " + dropChoice);
                                    }
                                    else
                                        gui.DisplayMessageBox("Duplicate Item, Ignoring Addition", "ERROR");
                                } else
                                {
                                    npc.drops.Add((ItemType)Enum.Parse(typeof(ItemType), dropChoice));
                                    gui.DisplayMessageBox("Added Drop Type: " + dropChoice);
                                }
                                break;

                            case "Remove Drop Type":
                                if (npc.drops == null || npc.drops.Count == 0)
                                {
                                    gui.DisplayMessageBox("No Drop Types to Remove", "ERROR");
                                }
                                else
                                {
                                    string[] currentDrops = new string[npc.drops.Count];
                                    for (int temp = 0; temp < npc.drops.Count; ++temp)
                                        currentDrops[temp] = npc.drops.ToArray()[temp].ToString();

                                    dropChoice = gui.DisplayList(2, 8, 20, currentDrops.Length + 1, "", currentDrops);
                                    if (npc.drops.Contains((ItemType)Enum.Parse(typeof(ItemType), dropChoice)))
                                    {
                                        npc.drops.Remove((ItemType)Enum.Parse(typeof(ItemType), dropChoice));
                                        gui.DisplayMessageBox("Removed " + dropChoice + " from Drops");
                                    }
                                }
                                break;
                        }
                        break;
                }

            }
        }
    }
}
