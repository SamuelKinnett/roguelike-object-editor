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
                string newDirectory = gui.DisplayList(new string[] {"(S)elect Location for Game Files.", currentPath}, choices);
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
            if (File.Exists(userPrefs.folderLocation + "\\NPC.txt")) {
                streamReader = new StreamReader(userPrefs.folderLocation + "\\NPC.txt");
                List<String> rawData = new List<string>();
                while (true)
                {
                    string tempString = streamReader.ReadLine();
                    if (tempString == ";")
                        break;
                    else
                        rawData.Add(tempString);
                }
            }
            return false;
        }

        public void SaveNPCData(UserPrefs userPrefs)
        {
            streamWriter = new StreamWriter(userPrefs.folderLocation + "\\NPC.txt", false);
            foreach (NPC npc in npcs)
            {
                streamWriter.Write(npc.name + ";" + npc.levelRange[0] + ";" + npc.levelRange[1] + ";" 
                    + npc.baseEXP + ";" + npc.EXPModifier + ";" + npc.dropLevelRange[0] + ";" + npc.dropLevelRange[1] + ";");
                streamWriter.Write(npc.stats.hp + ";"
                    + npc.stats.combatSkill + ";"
                    + npc.stats.damage[0] + ";" + npc.stats.damage[1] + ";" + npc.stats.damage[2] + ";"
                    + npc.stats.defence[0] + ";" + npc.stats.defence[1] + ";" + npc.stats.defence[2] + ";"
                    + npc.stats.preference[0] + ";" + npc.stats.preference[1] + ";" + npc.stats.preference[2] + ";"
                    + npc.stats.agility + ";");
                for (int i = 0; i < 10; ++i)
                {
                    streamWriter.Write(npc.biasValues[i] + ";");
                }
                for (int i = 0; i < npc.drops.Length; ++i)
                {
                    streamWriter.Write(npc.drops[i] + ";");
                }

                streamWriter.Write("\n");
            }
            streamWriter.Write(";");
        }

    }
}
