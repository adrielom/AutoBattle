using System.Collections.Generic;
using System;
using static AutoBattle.Utils;
using System.Linq;

namespace AutoBattle
{
    /// <summary>
    /// The Team class is responsible for managing the Teams' states.
    /// </summary>
    public class Team
    {
        #region Variables and Properties
        //List of all the members on this team
        List<Character> _members = new List<Character>();
        //health property is set to be the sum of all the members healths
        public int Health { get => _members.Sum(m => m.Health); }
        public string Name { get; set; }
        public ConsoleColor TeamColour { get; set; }
        public List<Character> Members { get => _members; set => _members = value; }
        public int ID { get; set; }
        HUD _hud = new HUD();
        #endregion

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="name">Team name</param>
        /// <param name="id">Team ID</param>
        public Team(string name, int id)
        {
            Name = name;
            ID = id;
            //Team colour is randomly selected
            int colourIndex = GetRandomInt(0, AvalilableColours.Count);
            TeamColour = AvalilableColours[colourIndex];
            AvalilableColours.RemoveAt(colourIndex);
        }

        /// <summary>
        /// Gets a character in the members list given an index
        /// </summary>
        /// <param name="index">elements index</param>
        /// <returns>Returns character at index position or null</returns>
        public Character GetMemeberAt(int index)
        {
            if (Members[index] != null) return Members[index];
            else return null;
        }

        //List of all available team colours
        public static List<ConsoleColor> AvalilableColours = new List<ConsoleColor>(new ConsoleColor[] {
            ConsoleColor.DarkRed,
            ConsoleColor.DarkBlue,
            ConsoleColor.DarkCyan,
            ConsoleColor.DarkGreen,
            ConsoleColor.DarkYellow
        });

        /// <summary>
        /// Helper method to access the members count easily
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return _members.Count;
        }

        /// <summary>
        /// Adds a character to the members list
        /// </summary>
        /// <param name="character">Character to be added</param>
        public void AddMember(Character character)
        {
            Members.Add(character);
        }

        /// <summary>
        /// Removes a character from the members list
        /// </summary>
        /// <param name="character">Character to be removed</param>
        public void RemoveMember(Character character)
        {
            //Removes the dead character from the team's list of members
            Members.Remove(character);
            //If the list of members is empty, the team has died
            if (Members.Count < 1)
            {
                //Remove the team from the list of all teams
                Program.AllTeams.Remove(this);
            }
            //Remove the dead character from the list of all players
            Program.AllPlayers.Remove(character);
        }

        /// <summary>
        /// Prints all the teams on the console
        /// </summary>
        public static void PrintAllTeams()
        {
            Program.AllTeams.ForEach(t =>
            {
                System.Console.WriteLine(t.ToString());
            });
        }

        /// <summary>
        /// Formats the team in a string to be outputed
        /// </summary>
        /// <returns>formatted team info as a string</returns>
        public override string ToString()
        {
            _hud.ConsoleForeground(TeamColour);
            _hud.MessageFormatter($"Team {Name} ------------ \n");
            _hud.ResetForeground();
            string returnString = "";
            returnString += $"Team colour: {TeamColour} \n";
            returnString += $"Team count: {Members.Count} \n";
            returnString += "Members are: \n";
            returnString += "::Name: \n";
            Members.ForEach(member =>
            {
                returnString += $"\t• {member.Name}\n";
            });

            returnString += "------------------------\n";

            return returnString;
        }
    }
}