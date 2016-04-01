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
            npcs = new List<NPC>();
        }

        StreamReader streamReader;
        StreamWriter streamWriter;
        GUI gui;
        UserPrefs userPrefs;

        public UserPrefs LoadUserPrefs()
        {
            userPrefs = new UserPrefs();

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

        public bool LoadNPCData()
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

                    for (int i = 0; i < 10; ++i)
                        npc.biasValues[i] = float.Parse(npcData[22 + i]);

                    gui.UpdateProgressBar(curNPC * 4 + 2, totalOps);

                    //Load in the drop types

                    if (npcData[32] != "END")
                    {
                        npc.drops = new List<ItemType>();
                        int curIndex = 32;
                        while (npcData[curIndex] != "END")
                        {
                            npc.drops.Add((ItemType)Enum.Parse(typeof(ItemType), npcData[curIndex]));
                            ++curIndex;
                        }
                    }

                    npcs.Add(npc);

                    gui.UpdateProgressBar(curNPC * 4 + 3, totalOps);
                }
                streamReader.Close();
                return true;
            }
            return false;
        }

        public void SaveNPCData()
        {
            streamWriter = new StreamWriter(userPrefs.folderLocation + "\\NPC.txt", false);
            Console.Clear();

            gui.InitialiseProgressBar(0, 0, Console.WindowWidth - 1, "Saving NPC Data");
            float totalOps = npcs.Count * 3;
            int count = 0;

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
                    gui.UpdateProgressBar(count, totalOps);
                    ++count;
                    for (int i = 0; i < 10; ++i)
                    {
                        streamWriter.Write(npc.biasValues[i] + ";");
                    }
                    gui.UpdateProgressBar(count, totalOps);
                    ++count;
                    for (int i = 0; i < npc.drops.Count; ++i)
                    {
                        streamWriter.Write(npc.drops[i] + ";");
                    }
                    gui.UpdateProgressBar(count, totalOps);
                    ++count;
                    streamWriter.Write("END");
                    streamWriter.Write("\n");
                }
            }

            streamWriter.Close();
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

            npcs.Add(newNPC);
            ModifyNPC(npcs[npcs.Count - 1]);

            return false;
        }

        public void SelectNPCToModify()
        {
            Console.Clear();
            NPC[] npcList = npcs.ToArray();
            string[] npcNameList = new string[npcList.Length];

            for (int i = 0; i < npcList.Length; ++i)
                npcNameList[i] = npcList[i].name;

            string selectedNPC = gui.DisplayList(0, 0, Console.WindowWidth - 1, Console.WindowHeight - 1, "Select NPC to Modify", npcNameList);

            for (int i = 0; i < npcs.Count; ++i)
                if (npcs[i].name == selectedNPC)
                    ModifyNPC(npcs[i]);
           
        }

        void ModifyNPC(NPC npc)
        {
            bool Finished = false;
            int previousSelectedIndex = 0;

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
                Console.Write(npc.stats.hp);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(" [" + npc.biasValues[0].ToString("0.00") + "]");
                Console.ForegroundColor = ConsoleColor.White;

                Console.SetCursorPosition(32, 8);
                Console.Write(npc.stats.combatSkill);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(" [" + npc.biasValues[1].ToString("0.00") + "]");
                Console.ForegroundColor = ConsoleColor.White;

                Console.SetCursorPosition(32, 9);
                Console.Write(npc.stats.damage[0]);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(" [" + npc.biasValues[2].ToString("0.00") + "] ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("/ " + npc.stats.damage[1]);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(" [" + npc.biasValues[3].ToString("0.00") + "] ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("/ " + npc.stats.damage[2]);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(" [" + npc.biasValues[4].ToString("0.00") + "]");
                Console.ForegroundColor = ConsoleColor.White;

                Console.SetCursorPosition(32, 10);
                Console.Write(npc.stats.defence[0]);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(" [" + npc.biasValues[5].ToString("0.00") + "] ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("/ " + npc.stats.defence[1]);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(" [" + npc.biasValues[6].ToString("0.00") + "] ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("/ " + npc.stats.defence[2]);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(" [" + npc.biasValues[7].ToString("0.00") + "] ");
                Console.ForegroundColor = ConsoleColor.White;

                Console.SetCursorPosition(32, 11);
                Console.Write(npc.stats.preference[0].ToString("0.00") + " / " + npc.stats.preference[1].ToString("0.00") + " / " + npc.stats.preference[2].ToString("0.00"));

                Console.SetCursorPosition(32, 12);
                Console.Write(npc.stats.agility);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(" [" + npc.biasValues[8].ToString("0.00") + "]");
                Console.ForegroundColor = ConsoleColor.White;

                Console.SetCursorPosition(32, 13);
                Console.Write(npc.stats.perception);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(" [" + npc.biasValues[9].ToString("0.00") + "]");
                Console.ForegroundColor = ConsoleColor.White;

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
                        "Modify Bias Values",
                        "Save and Return"
                    }, previousSelectedIndex);

                string subChoice = "";
                float[] choices;

                switch (menuChoice)
                {
                    case "Name":
                        npc.name = gui.GetInput(1, 2, "Enter New Name");
                        previousSelectedIndex = 0;
                        break;

                    case "Level Range":
                        subChoice = gui.DisplayList(1, 3, 20, 4, "", new string[] { "Lower Bound", "Upper Bound", "Cancel" });

                        switch (subChoice) {

                            case "Lower Bound":
                                npc.levelRange[0] = int.Parse(gui.GetInput(2, 5, "Enter Lower Level Bound"));
                                if (npc.levelRange[0] < 0)
                                {
                                    npc.levelRange[0] = 0;
                                    gui.DrawMessageBox(2, 8, 25, 2, "Clamped Lower Level to 0", "");
                                }
                                else if (npc.levelRange[0] > 100)
                                {
                                    npc.levelRange[0] = 100;
                                    gui.DrawMessageBox(2, 7, 27, 2, "Clamped Lower Level to 100", "");
                                }
                                else if (npc.levelRange[0] > npc.levelRange[1])
                                {
                                    npc.levelRange[0] = npc.levelRange[1];
                                    gui.DrawMessageBox(2, 7, 35, 2, "Clamped Lower Bound to Upper Bound", "");
                                }
                                break;

                            case "Upper Bound":
                                npc.levelRange[1] = int.Parse(gui.GetInput(2, 6, "Enter Upper Level Bound"));
                                if (npc.levelRange[1] < 0)
                                {
                                    npc.levelRange[1] = 0;
                                    gui.DrawMessageBox(2, 9, 25, 2, "Clamped Upper Level to 0", "");
                                }
                                else if (npc.levelRange[1] > 100)
                                {
                                    npc.levelRange[1] = 100;
                                    gui.DrawMessageBox(2, 9, 27, 2, "Clamped Upper Level to 100", "");
                                }
                                else if (npc.levelRange[1] < npc.levelRange[0])
                                {
                                    npc.levelRange[1] = npc.levelRange[0];
                                    gui.DrawMessageBox(2, 9, 35, 2, "Clamped Upper Level to Lower Level", "");
                                }
                                break;
                        }
                        previousSelectedIndex = 1;
                        break;

                    case "Base Experience Points":
                        npc.baseEXP = float.Parse(gui.GetInput(1, 4, "Enter Base Experience Points"));
                        if (npc.baseEXP < 0)
                        {
                            npc.baseEXP = 0;
                            gui.DrawMessageBox(1, 7, 29, 2, "Clamped Base Experience to 0", "");
                        }
                        previousSelectedIndex = 2;
                        break;

                    case "Experience Modifier":
                        npc.EXPModifier = float.Parse(gui.GetInput(1, 5, "Enter Experience Modifier"));
                        if (npc.EXPModifier < 1)
                        {
                            npc.EXPModifier = 1;
                            gui.DrawMessageBox(1, 8, 33, 2, "Clamped Experience Modifier to 1", "");
                        }
                        previousSelectedIndex = 3;
                        break;

                    case "Drop Level Range":
                        subChoice = gui.DisplayList(1, 6, 20, 4, "", new string[] { "Lower Bound", "Upper Bound", "Cancel" });

                        switch (subChoice) {

                            case "Lower Bound":
                                npc.dropLevelRange[0] = int.Parse(gui.GetInput(2, 8, "Enter Lower Level Bound"));
                                if (npc.dropLevelRange[0] < 0)
                                {
                                    npc.dropLevelRange[0] = 0;
                                    gui.DrawMessageBox(2, 11, 25, 2, "Clamped Lower Level to 0", "");
                                }
                                else if (npc.dropLevelRange[0] > 100)
                                {
                                    npc.dropLevelRange[0] = 100;
                                    gui.DrawMessageBox(2, 11, 27, 2, "Clamped Lower Level to 100", "");
                                }
                                break;

                            case "Upper Bound":
                                npc.dropLevelRange[1] = int.Parse(gui.GetInput(2, 9, "Enter Upper Level Bound"));
                                if (npc.dropLevelRange[1] < 0)
                                {
                                    npc.dropLevelRange[1] = 0;
                                    gui.DrawMessageBox(2, 12, 25, 2, "Clamped Upper Level to 0", "");
                                }
                                else if (npc.dropLevelRange[1] > 100)
                                {
                                    npc.dropLevelRange[1] = 100;
                                    gui.DrawMessageBox(2, 12, 27, 2, "Clamped Upper Level to 100", "");
                                }
                                else if (npc.dropLevelRange[1] < npc.dropLevelRange[0])
                                {
                                    npc.dropLevelRange[1] = npc.dropLevelRange[0];
                                    gui.DrawMessageBox(2, 12, 35, 2, "Clamped Upper Level to Lower Level", "");
                                }
                                break;
                        }
                        previousSelectedIndex = 4;
                        break;

                    case "Drop Types":
                        subChoice = gui.DisplayList(1, 7, 20, 4, "", new string[] { "Add Drop Type", "Remove Drop Type", "Cancel" });
                        string dropChoice = "";

                        switch (subChoice)
                        {
                            case "Add Drop Type":
                                dropChoice = gui.DisplayList(2, 9, 20, 10, "", Enum.GetNames(typeof(ItemType)));
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

                                    dropChoice = gui.DisplayList(2, 10, 20, currentDrops.Length + 1, "", currentDrops);
                                    if (npc.drops.Contains((ItemType)Enum.Parse(typeof(ItemType), dropChoice)))
                                    {
                                        npc.drops.Remove((ItemType)Enum.Parse(typeof(ItemType), dropChoice));
                                        gui.DisplayMessageBox("Removed " + dropChoice + " from Drops");
                                    }
                                }
                                break;
                        }
                        previousSelectedIndex = 5;
                        break;

                    case "HP":
                        npc.stats.hp = int.Parse(gui.GetInput(1, 8, "Enter HP"));
                        if (npc.stats.hp < 0)
                        {
                            npc.stats.hp = 0;
                            gui.DrawMessageBox(1, 11, 16, 2, "Clamped HP to 0", "");
                        }
                        else if (npc.stats.hp > 9999)
                        {
                            npc.stats.hp = 9999;
                            gui.DrawMessageBox(1, 11, 19, 2, "Clamped HP to 9999", "");
                        }
                        previousSelectedIndex = 6;
                        break;

                    case "Combat Skill":
                        npc.stats.combatSkill = float.Parse(gui.GetInput(1, 9, "Enter Combat Skill"));
                        if (npc.stats.combatSkill < 0)
                        {
                            npc.stats.combatSkill = 0;
                            gui.DrawMessageBox(1, 12, 26, 2, "Clamped Combat Skill to 0", "");
                        }
                        else if (npc.stats.combatSkill > 100)
                        {
                            npc.stats.combatSkill = 100;
                            gui.DrawMessageBox(1, 12, 28, 2, "Clamped Combat Skill to 100", "");
                        }
                        previousSelectedIndex = 7;
                        break;

                    case "Damage (S/B/P)":
                        subChoice = gui.DisplayList(1, 10, 20, 4, "Modify Which?", new string[] { "Slashing", "Bashing", "Piercing" });
                        switch (subChoice)
                        {
                            case "Slashing":
                                npc.stats.damage[0] = float.Parse(gui.GetInput(2, 12, "Enter Slashing Damage"));
                                if (npc.stats.damage[0] < 0)
                                {
                                    npc.stats.damage[0] = 0;
                                    gui.DrawMessageBox(2, 15, 29, 2, "Clamped Slashing Damage to 0", "");
                                }
                                else if (npc.stats.damage[0] > 100)
                                {
                                    npc.stats.damage[0] = 100;
                                    gui.DrawMessageBox(2, 15, 31, 2, "Clamped Slashing Damage to 100", "");
                                }
                                break;

                            case "Bashing":
                                npc.stats.damage[1] = float.Parse(gui.GetInput(2, 13, "Enter Bashing Damage"));
                                if (npc.stats.damage[1] < 0)
                                {
                                    npc.stats.damage[1] = 0;
                                    gui.DrawMessageBox(2, 16, 28, 2, "Clamped Bashing Damage to 0", "");
                                }
                                else if (npc.stats.damage[1] > 100)
                                {
                                    npc.stats.damage[1] = 100;
                                    gui.DrawMessageBox(2, 16, 30, 2, "Clamped Bashing Damage to 100", "");
                                }
                                break;

                            case "Piercing":
                                npc.stats.damage[2] = float.Parse(gui.GetInput(2, 14, "Enter Piercing Damage"));
                                if (npc.stats.damage[2] < 0)
                                {
                                    npc.stats.damage[2] = 0;
                                    gui.DrawMessageBox(2, 17, 29, 2, "Clamped Piercing Damage to 0", "");
                                }
                                else if (npc.stats.damage[2] > 100)
                                {
                                    npc.stats.damage[2] = 100;
                                    gui.DrawMessageBox(2, 17, 31, 2, "Clamped Piercing Damage to 100", "");
                                }
                                break;
                        }
                        previousSelectedIndex = 8;
                        break;

                    case "Defence (S/B/P)":
                        subChoice = gui.DisplayList(1, 11, 20, 4, "Modify Which?", new string[] { "Slashing", "Bashing", "Piercing" });
                        switch (subChoice)
                        {
                            case "Slashing":
                                npc.stats.defence[0] = float.Parse(gui.GetInput(2, 13, "Enter Slashing Defence"));
                                if (npc.stats.defence[0] < 0)
                                {
                                    npc.stats.defence[0] = 0;
                                    gui.DrawMessageBox(2, 16, 30, 2, "Clamped Slashing Defence to 0", "");
                                }
                                else if (npc.stats.defence[0] > 100)
                                {
                                    npc.stats.defence[0] = 100;
                                    gui.DrawMessageBox(2, 16, 32, 2, "Clamped Slashing Defence to 100", "");
                                }
                                break;

                            case "Bashing":
                                npc.stats.defence[1] = float.Parse(gui.GetInput(2, 14, "Enter Bashing Defence"));
                                if (npc.stats.defence[1] < 0)
                                {
                                    npc.stats.defence[1] = 0;
                                    gui.DrawMessageBox(2, 17, 29, 2, "Clamped Bashing Defence to 0", "");
                                }
                                else if (npc.stats.defence[1] > 100)
                                {
                                    npc.stats.defence[1] = 100;
                                    gui.DrawMessageBox(2, 17, 31, 2, "Clamped Bashing Defence to 100", "");
                                }
                                break;

                            case "Piercing":
                                npc.stats.defence[2] = float.Parse(gui.GetInput(2, 15, "Enter Piercing Defence"));
                                if (npc.stats.defence[2] < 0)
                                {
                                    npc.stats.defence[2] = 0;
                                    gui.DrawMessageBox(2, 18, 30, 2, "Clamped Piercing Defence to 0", "");
                                }
                                else if (npc.stats.defence[2] > 100)
                                {
                                    npc.stats.defence[2] = 100;
                                    gui.DrawMessageBox(2, 18, 32, 2, "Clamped Piercing Defence to 100", "");
                                }
                                break;
                        }
                        previousSelectedIndex = 9;
                        break;

                    case "Preference (S/B/P)":
                        choices = new float[3];
                        choices[0] = float.Parse(gui.GetInput(1, 12, "Enter Slashing Preference"));
                        if (choices[0] < 0)
                            choices[0] = 0;
                        choices[1] = float.Parse(gui.GetInput(1, 15, "Enter Bashing Preference"));
                        if (choices[1] < 0)
                            choices[1] = 0;
                        choices[2] = float.Parse(gui.GetInput(1, 18, "Enter Piercing Preference"));
                        if (choices[2] < 0)
                            choices[2] = 0;

                        //Normalise
                        float total = choices[0] + choices[1] + choices[2];

                        if (total > 0)
                        {
                            npc.stats.preference[0] = choices[0] / total;
                            npc.stats.preference[1] = choices[1] / total;
                            npc.stats.preference[2] = choices[2] / total;
                            gui.DisplayMessageBox("Normalised Preferences");
                        }
                        else
                        {
                            gui.DisplayMessageBox("You Must Input Preference Values", "ERROR");
                        }
                        previousSelectedIndex = 10;
                        break;

                    case "Agility":
                        npc.stats.agility = float.Parse(gui.GetInput(1, 13, "Enter Agility"));
                        if (npc.stats.agility < 0)
                        {
                            npc.stats.agility = 0;
                            gui.DrawMessageBox(1, 16, 21, 2, "Clamped Agility to 0", "");
                        }
                        else if (npc.stats.agility > 100)
                        {
                            npc.stats.agility = 100;
                            gui.DrawMessageBox(1, 16, 23, 2, "Clamped Agility to 100", "");
                        }
                        previousSelectedIndex = 11;
                        break;

                    case "Perception":
                        npc.stats.perception = int.Parse(gui.GetInput(1, 14, "Enter Perception"));
                        if (npc.stats.perception < 0)
                        {
                            npc.stats.perception = 0;
                            gui.DrawMessageBox(1, 17, 24, 2, "Clamped Perception to 0", "");
                        }
                        else if (npc.stats.perception > 100)
                        {
                            npc.stats.perception = 100;
                            gui.DrawMessageBox(1, 17, 26, 2, "Clamped Perception to 100", "");
                        }
                        previousSelectedIndex = 12;
                        break;

                    case "Aggressive":
                        npc.stats.aggressive = bool.Parse(gui.DisplayList(1, 15, 20, 3, "Aggressive Value", new string[] { "True", "False" }));
                        previousSelectedIndex = 13;
                        break;

                    case "Hostile":
                        npc.stats.hostile = bool.Parse(gui.DisplayList(1, 16, 20, 3, "Hostile Value", new string[] { "True", "False" }));
                        previousSelectedIndex = 14;
                        break;

                    case "Modify Bias Values":
                        choices = new float[10];

                        choices[0] = float.Parse(gui.GetInput(1, 1, "Enter HP Bias"));
                        if (choices[0] < 0)
                            choices[0] = 0;
                        choices[1] = float.Parse(gui.GetInput(1, 4, "Enter Combat Skill Bias"));
                        if (choices[1] < 0)
                            choices[1] = 0;
                        choices[2] = float.Parse(gui.GetInput(1, 7, "Enter Slashing Damage Bias"));
                        if (choices[2] < 0)
                            choices[2] = 0;
                        choices[3] = float.Parse(gui.GetInput(1, 10, "Enter Bashing Damage Bias"));
                        if (choices[3] < 0)
                            choices[3] = 0;
                        choices[4] = float.Parse(gui.GetInput(1, 13, "Enter Piercing Damage Bias"));
                        if (choices[4] < 0)
                            choices[4] = 0;
                        choices[5] = float.Parse(gui.GetInput(1, 16, "Enter Slashing Defence Bias"));
                        if (choices[5] < 0)
                            choices[5] = 0;
                        choices[6] = float.Parse(gui.GetInput(30, 1, "Enter Bashing Defence Bias"));
                        if (choices[6] < 0)
                            choices[6] = 0;
                        choices[7] = float.Parse(gui.GetInput(30, 4, "Enter Piercing Defence Bias"));
                        if (choices[7] < 0)
                            choices[7] = 0;
                        choices[8] = float.Parse(gui.GetInput(30, 7, "Enter Agility Bias"));
                        if (choices[8] < 0)
                            choices[8] = 0;
                        choices[9] = float.Parse(gui.GetInput(30, 10, "Enter Perception Bias"));
                        if (choices[9] < 0)
                            choices[9] = 0;

                        //Normalise

                        float tempTotal = 0;

                        for (int i = 0; i < 10; ++i)
                            tempTotal += choices[i];

                        for (int i = 0; i < 10; ++i)
                            npc.biasValues[i] = choices[i] / tempTotal;

                        gui.DisplayMessageBox("Normalised Bias Values");
                        previousSelectedIndex = 15;
                        break;

                    case "Save and Return":
                        if (npc.name != "")
                        {
                            if (npc.EXPModifier >= 1)
                            {
                                if (npc.stats.hp > 0)
                                {
                                    if (npc.stats.preference[0] + npc.stats.preference[1] + npc.stats.preference[2] > 0)
                                    {
                                        float testTotal = 0;
                                        for (int i = 0; i < 10; ++i)
                                            testTotal += npc.biasValues[i];
                                        if (testTotal > 0)
                                        {
                                            npcs.Add(npc);
                                            Finished = true;
                                        } else
                                        {
                                            gui.DisplayMessageBox("Please Enter Bias Values", "ERROR");
                                        }
                                    }
                                    else
                                    {
                                        gui.DisplayMessageBox("Please Enter Attack Preferences", "ERROR");
                                    }
                                }
                                else
                                {
                                    gui.DisplayMessageBox("Please Enter a HP Value", "ERROR");
                                }
                            }
                            else
                            {
                                gui.DisplayMessageBox("Please Enter an EXP Modifier", "ERROR");
                            }
                        }
                        else
                        {
                            gui.DisplayMessageBox("Please Enter a Name", "ERROR");
                        }

                        break;
                }
            }
        }

    }
}
