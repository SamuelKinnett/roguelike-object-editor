using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace roguelikeobjecteditor.src
{
    struct NPCStats
    {
        public int hp;
        public float combatSkill;
        public float[] damage;
        public float[] defence;
        public float[] preference;
        public float agility;
        public float perception;
        public bool aggressive;
        public bool hostile;
    }

    struct NPC
    {
        public string name;
        public int[] levelRange;
        public NPCStats stats;
        public float baseEXP;
        public float EXPModifier;
        public float[] biasValues;
        public List<ItemType> drops;
        public int[] dropLevelRange;
    }

    struct Weapon
    {
        public string name;
        public bool twoHanded;  //if true, the weapon requires two hands. Otherwise, it requires 1.
        public float[] damage;
        public PlayerStats bonusStat;
        public float[] bonusModifier;
        public float weight;
        public float value;
    }

    struct UserPrefs
    {
        public string folderLocation;  //The folder containing the game files
    }
}
