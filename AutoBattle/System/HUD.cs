using System;
using System.Linq;
using static AutoBattle.Types;

namespace AutoBattle
{
    /// <summary>
    /// The HUD class is responsible for dealing with the output communication Human Computer Interaction (HCI)
    /// </summary>
    public class HUD
    {
        //Caching the default background and foreground colours and setting them on the constructor
        public ConsoleColor defaultBackgroundColor;
        public ConsoleColor defaultForegroundColor;

        public HUD()
        {
            defaultBackgroundColor = Console.BackgroundColor;
            defaultForegroundColor = Console.ForegroundColor;
        }

        #region IO and Formatting Methods

        /// <summary>
        /// Turns all the parametres passed by as an string and prints it to the console
        /// </summary>
        /// <param name="messages">messages to be printed</param>
        public void MessageFormatter(params string[] messages)
        {
            string printableMessage = "";
            foreach (var message in messages)
            {
                printableMessage += message;
            }
            Console.Write(printableMessage);
        }

        /// <summary>
        ///Turns all the parametres passed by as an string and prints it to the console from a given point on
        /// </summary>
        /// <param name="consoleTop">Printing starting position</param>
        /// <param name="messages">messages to be printed</param>
        public void TextFormatter(int consoleTop, params string[] messages)
        {
            string printableMessage = "";
            foreach (var message in messages)
            {
                printableMessage += message;
            }
            printableMessage += "";
            Console.CursorTop = consoleTop;
            Console.Write(printableMessage);
        }

        /// <summary>
        /// Clears the Console
        /// </summary>
        public void ClearConsole() => Console.Clear();

        /// <summary>
        /// Given the positions x and y, clears the console
        /// </summary>
        /// <param name="x">Starting X position</param>
        /// <param name="y">Starting Y position</param>
        public void ClearTextArea(int x, int y)
        {
            //Sets the cursor to the X and Y positions
            Console.CursorLeft = x;
            Console.CursorTop = y;
            //Writes black text 
            for (var i = 0; i < 10; i++)
            {
                Console.CursorTop = y + i;
                for (var j = 0; j < Console.WindowWidth; j++)
                {
                    System.Console.Write(" ");
                }
            }
            //Resets the cursor to the top
            Console.CursorTop = y;
        }

        /// <summary>
        /// Checks if a message contains an extract of text, if not, concatenates
        /// </summary>
        /// <param name="message">Message to be search on</param>
        /// <param name="extractToSearch">Message to search on</param>
        /// <param name="toTheEnd">Should the text be concatenated on the start or end of the message</param>
        /// <returns></returns>
        public string MessageFormatterContains(string message, string extractToSearch, bool toTheEnd = true)
        {
            if (message.Contains(extractToSearch))
                return message;
            else
                return toTheEnd ? string.Concat(message, extractToSearch) : string.Concat(extractToSearch, message);
        }

        /// <summary>
        /// Displays a message and read the input
        /// </summary>
        /// <param name="messages">Message to be printed</param>
        /// <returns>returns the user input as a string</returns>
        public string MessageInputer(params string[] messages)
        {
            MessageFormatter(messages);
            return Console.ReadLine();
        }

        /// <summary>
        /// Method that outputs a message and waits for player's input
        /// </summary>
        /// <param name="message">Message to be outputed</param>
        /// <param name="callback">Callback function</param>
        public void ReadKeyboardInput(string message = "", Action callback = null)
        {

            if (!String.IsNullOrEmpty(message) || !String.IsNullOrWhiteSpace(message))
            {
                MessageFormatter(message);
            }
            ConsoleKeyInfo key = Console.ReadKey();
            if (callback != null) callback();
        }

        /// <summary>
        /// Sets the console foreground color
        /// </summary>
        /// <param name="color">Color to be set</param>
        public void ConsoleForeground(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        /// <summary>
        /// Resets the foreground color to the default one
        /// </summary>
        public void ResetForeground()
        {
            Console.ForegroundColor = defaultForegroundColor;
        }

        /// <summary>
        /// Offsets the console cursor to a vector2 position and fires a callback
        /// </summary>
        /// <param name="vector">Position to be set to</param>
        /// <param name="callback">Callback function</param>
        public void OffsetConsole(Vector2 vector, Action callback)
        {
            Console.CursorLeft = vector.xIndex;
            Console.CursorTop = vector.yIndex;
            callback();
        }

        #endregion

        #region Status Methods

        /// <summary>
        /// Prints the Game Status (Team's status) and Classes information on the console
        /// </summary>
        /// <param name="pos">Printing starting position on console</param>
        /// <param name="offset">Offset to be added to the position</param>
        public void RenderStatus(Vector2 pos, Vector2 offset)
        {
            //A reference to the current team is set
            Team team = null;
            //Drawable area width
            ///     grid     drawableArea
            /// | [######] [**************] |
            int drawAreaBounds = (Console.WindowWidth - offset.xIndex);
            //Drawable area center
            ///     grid     drawableAreaCenter
            /// | [######] [*******0*******] |
            int drawAreaCenter = drawAreaBounds / 2;
            // Team area column 
            ///     grid     # drawableAreaColumn 
            /// | [######] [**##***##***##***##**] |
            int drawAreaColumn = drawAreaBounds / Program.AllTeams.Count;
            int marginLeft = drawAreaColumn / Program.AllTeams.Count;
            //Prints info about the classes and anchors it fixed to the console right side
            RenderClassesInfo(pos, Console.WindowWidth - 50, offset.yIndex);
            //Offset to a team info block of text
            int teamBlockOffset = 0;

            // If there's only one team, center its information status and exit
            if (Program.AllTeams.Count == 1)
            {
                team = Program.AllTeams[0];
                PrintTeamStatus(team, offset, drawAreaCenter, pos.yIndex + offset.yIndex);
                return;
            }

            //For all the teams on the game
            for (var i = 0; i < Program.AllTeams.Count; i++)
            {
                //Gets a reference to the team in matter
                team = Program.AllTeams[i];
                //Checks if the team has members
                if (team.Count() > 0)
                {
                    //The offset to this block of text is proportional to the column position element with a bit of margin
                    teamBlockOffset = (i * drawAreaColumn) + marginLeft;
                    //Prints the information on the console
                    PrintTeamStatus(team, offset, teamBlockOffset, pos.yIndex + offset.yIndex);
                }
            }
        }

        /// <summary>
        /// Renders classes information
        /// </summary>
        /// <param name="pos">Starting position</param>
        /// <param name="xOffset">width offset</param>
        /// <param name="yOffset">height offset</param>
        public void RenderClassesInfo(Vector2 pos, int xOffset, int yOffset)
        {
            //Prints the information on the console as a callback to the offsetConsole
            OffsetConsole(new Vector2(pos.xIndex + xOffset, yOffset), () => { MessageFormatter($"[P]aladin, [W]arrior, [C]leric, [A]rcher "); });
        }

        /// <summary>
        /// Prints all of teams' status to the console
        /// </summary>
        /// <param name="team">Team to have its status printed</param>
        /// <param name="pos">Starting position</param>
        /// <param name="xOffset">width offset</param>
        /// <param name="yOffset">height offset</param>
        public void PrintTeamStatus(Team team, Vector2 pos, int xOffset, int yOffset)
        {
            //Sets the foreground colour to be the teams'
            ConsoleForeground(team.TeamColour);
            //Prints the teams name as a callback to the offsetConsole
            OffsetConsole(new Vector2(pos.xIndex + xOffset, pos.yIndex + yOffset), () => { MessageFormatter($":{team.Name} Status "); });
            ResetForeground();
            //Clears the line and then prints the teams health as a callback to the offsetConsole
            OffsetConsole(new Vector2(pos.xIndex + xOffset, pos.yIndex + yOffset + 1), () => { MessageFormatter($"Health:         "); });
            OffsetConsole(new Vector2(pos.xIndex + xOffset, pos.yIndex + yOffset + 1), () => { MessageFormatter($"Health: {team.Health}"); });
            //Prints the team member count as a callback to the offsetConsole
            OffsetConsole(new Vector2(pos.xIndex + xOffset, pos.yIndex + yOffset + 2), () => { MessageFormatter($"membersCount: {team.Count()}"); ; });
        }

        #endregion

    }
}