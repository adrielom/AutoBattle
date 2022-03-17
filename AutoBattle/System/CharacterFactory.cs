using System;
using static AutoBattle.Types;
using static AutoBattle.Utils;

namespace AutoBattle
{
    /// <summary>
    /// Class responsible for creating instances of characters
    /// </summary>
    public class CharacterFactory
    {
        //Variables used in the class, the grid reference is set upon factory creation
        Grid _grid;
        HUD _hud = new HUD();
        public CharacterFactory(Grid grid)
        {
            _grid = grid;
        }

        /// <summary>
        /// Player creation Method
        /// </summary>
        /// <param name="characterClass">Player CharacterClass</param>
        /// <returns>Returns a brand new Player Character</returns>
        public Character CreatePlayer(CharacterClass characterClass)
        {
            //Creates an instance of the Character player
            Character newCharacter = new Character(characterClass, this._grid);
            _hud.ClearConsole();
            _hud.MessageFormatter($"Player Class Chosen: {characterClass}\n\n");
            //Sets the character's name and index
            newCharacter.Name = $"[{characterClass} (Player)]";
            newCharacter.PlayerIndex = GetRandomInt(0, this._grid.GetGridSize());

            return newCharacter;
        }

        /// <summary>
        /// General character creation Method
        /// </summary>
        /// <returns>Returns a brand new Character</returns>
        public Character Create()
        {
            //randomly chooses the enemy class based on the amount of characters on the enum
            int randomCharacterClassIndex = GetRandomInt(1, Enum.GetNames(typeof(CharacterClass)).Length + 1);
            //Creates an instance of the character and sets the class to be the newly set class
            CharacterClass characterClass = (CharacterClass)randomCharacterClassIndex;
            Character newCharacter = new Character(characterClass, this._grid);
            //Sets the character name and index
            newCharacter.Name = $"[{characterClass} {Program.AllPlayers.Count - 1}]";
            _hud.MessageFormatter($"{newCharacter.Name} has entered the game!\n\n");
            int randomPlayerIndex = GetRandomInt(0, this._grid.GetGridSize());

            // Searchs in the all players list to find if that index is already set and keep trying if not
            while (Program.AllPlayers.Find(element => element.PlayerIndex == randomPlayerIndex) != null)
            {
                randomPlayerIndex = GetRandomInt(0, this._grid.GetGridSize());
            }

            newCharacter.PlayerIndex = randomPlayerIndex;
            return newCharacter;
        }
    }
}