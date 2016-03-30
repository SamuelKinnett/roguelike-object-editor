using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace roguelikeobjecteditor.src
{

    enum ItemType
    {
        TinyBlade,
        SmallBlade,
        LargeBlade,
        SmallBlunt,
        LargeBlunt,
        LightShield,
        MediumShield,
        HeavyShield,
        LightArmour,
        MediumArmour,
        HeavyArmour,
        Food,
        Ammunition,
        Trinket
    }

    enum AttackType
    {
        slash,
        bash,
        pierce
    }

    enum PlayerStats
    {
        MaxHP,
        MaxHunger,
        Strength,
        Toughness,
        WeaponCompetence,
        ArmourCompetence,
        Agility,
        Perception,
        Dexterity
    }
}
