using System;
using System.Collections.Generic;
using System.Text;

namespace AutoBattle
{
    /// <summary>
    /// The Types file unifies many different structs and classes that are used as data structures for the game logic
    /// </summary>
    public class Types
    {
        /// <summary>
        /// Struct model to the characterclass modifiers
        /// </summary>
        public struct CharacterClassSpecific
        {
            public CharacterClass characterClass;
            public int hpModifier;
            public int classDamage;
            public CharacterSkills[] skills;

        }

        /// <summary>
        /// Vector2 struct created to make it easier handling x and y datasets
        /// </summary>
        public struct Vector2 : IComparable
        {
            public int xIndex;
            public int yIndex;

            public Vector2(int xIndex, int yIndex)
            {
                this.xIndex = xIndex;
                this.yIndex = yIndex;
            }

            /// <summary>
            /// Compares two vectors based on their x and y values
            /// </summary>
            /// <param name="obj">Object to be compared</param>
            /// <returns></returns>
            public int CompareTo(object obj)
            {
                Vector2 other = (Vector2)obj;
                return this.xIndex == other.xIndex && this.yIndex == other.yIndex ? 1 : -1;
            }

            /// <summary>
            /// formats the x and y positions in a string
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return $"[{xIndex}, {yIndex}]";
            }
        }

        /// <summary>
        /// Struct that represents the Grid cell, with references to its position, index and state
        /// </summary>
        public struct GridBox
        {
            public Vector2 position;
            public bool isOccupied;
            public int index;

            public GridBox(int x, int y, bool ocupied, int index)
            {
                this.position = new Vector2(x, y);
                this.isOccupied = ocupied;
                this.index = index;
            }

        }

        /// <summary>
        /// Struct that represents a character skill model
        /// </summary>
        public struct CharacterSkills
        {
            public CharacterSkills(string name, int damage, int damageMultiplier)
            {
                this.name = name;
                this.damage = damage;
                this.damageMultiplier = damageMultiplier;
            }
            public string name;
            public int damage;
            public int damageMultiplier;
        }

        /// <summary>
        /// Character classes possible
        /// </summary>
        public enum CharacterClass : uint
        {
            Paladin = 1,
            Warrior = 2,
            Cleric = 3,
            Archer = 4
        }


    }
}
