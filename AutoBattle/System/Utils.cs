using System;
using static AutoBattle.Types;

namespace AutoBattle
{
    /// <summary>
    /// The Utils class is gives a toolset of functions that can be accessed by multiple other classes interchangeably
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Method that parses the inputed message to a valid integer
        /// </summary>
        /// <param name="message">Message inputed by the player</param>
        /// <returns></returns>
        public static int parsedInputAsInteger(string message)
        {
            Int16 parsedInputCell = 0;
            try
            {
                parsedInputCell = Int16.Parse(message);
            }
            catch
            {
                Console.WriteLine("Not a valid dimension for this battlefield! I cannot summon it...\n");
            }
            return parsedInputCell;
        }

        /// <summary>
        /// Method responsible for returning a random number given the min - max thresholds
        /// </summary>
        /// <param name="min">minimum number</param>
        /// <param name="max">maximum number</param>
        /// <returns>an integer between min and max - 1</returns>
        public static int GetRandomInt(int min, int max)
        {
            var rand = new Random();
            int randomInteger = rand.Next(min, max);
            return randomInteger;
        }

        /// <summary>
        /// This method is responsible for getting the initial name of the CharacterClass given.
        /// </summary>
        /// <param name="characterClass">Character class reference</param>
        /// <returns>First character in said class name</returns>
        public static string GetCharacterClassInitial(CharacterClass characterClass) => Enum.GetName(typeof(CharacterClass), characterClass)[0].ToString();
    }
}