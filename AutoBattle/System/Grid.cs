using System;
using System.Collections.Generic;
using System.Linq;
using static AutoBattle.Types;
using static AutoBattle.Utils;

namespace AutoBattle
{
    /// <summary>
    /// The Grid class is responsible for creating and managing the Grid positioning and rendering.
    /// </summary>
    public class Grid
    {
        //All variables used on this class. 
        //List of all the gridboxes that make up the grid 
        private List<GridBox> _grids = new List<GridBox>();
        //Reference to the grid's height and width. 
        private int _xLenght;
        private int _yLength;
        //A hud instance to communicate with the player, 
        private HUD _hud = new HUD();
        //Maximum side for the grid
        public static int maxSide = 8;

        /// <summary>
        /// Constructor that populates x and y variables and creates the grid populating it with a new Gridbox for each grid cell. The newly created gridbox is added to the list of gridboxes and a fresh render of the grid is shown for feedback purposes
        /// </summary>
        /// <param name="Lines">The amount of lines that make up the grid</param>
        /// <param name="Columns">The amount of columns that make up the grid</param>
        public Grid(int Lines, int Columns)
        {
            XLenght = Lines;
            YLength = Columns;
            for (int i = 0; i < Lines; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    GridBox newBox = new GridBox(j, i, false, (Columns * i + j));
                    Grids.Add(newBox);
                }
            }
            _hud.ClearConsole();
            _hud.MessageFormatter("\n\n");
            DrawBattlefield(XLenght, YLength, false, false);
            _hud.MessageFormatter($"The battlefield has been created...", "\n\n");
        }

        #region GridProperties
        public List<GridBox> Grids { get => _grids; set => _grids = value; }
        public int XLenght { get => _xLenght; set => _xLenght = value; }
        public int YLength { get => _yLength; set => _yLength = value; }
        #endregion
        #region HelperFunctions
        /// <summary>
        /// Returns the number of elements in the grid
        /// </summary>
        public int GetGridSize() => Grids.Count;
        /// <summary>
        /// Gets the gridbox element in the grid list by index
        /// </summary>
        /// <param name="index">integer representing the element position in the list</param>
        /// <returns>Returns the Gridbox found. Null if not found</returns>
        public GridBox GetElementAt(int index) => Grids.ElementAt(index);
        /// <summary>
        /// Updates the grid's element position at a certain index
        /// </summary>
        /// <param name="index">integer representing the element position in the list</param>
        /// <param name="element">New value to be updated</param>
        public void SetElementAt(int index, GridBox element) => Grids[index] = element;

        /// <summary>
        /// Finds the index of an element based on its x and y coordinates
        /// This approach is given by the flattening of a bidimensional array (matrix) on a unidimensional one. 
        /// Given the matrix |00 10 20|   the flattening process is given by the creation
        ///                  |01 11 21|   of a new array such as the following
        ///              3x3 |02 12 22|   [00 10 20 01 11 21 02 12 22] 1x9
        /// 
        /// Hence the matrix is a rectangle, to get its area you must multiply w * h, where w is the element x index position and h is the gridsize height. That's gonna give you the x position of the element in the grid
        /// e.g.                      to get the (3,2) position we first need to find its
        ///           |0    1    2|   position on the x axis. 2 * 2 = 4  and we add it to
        ///       3x2 |3    4  (5)|   the element index height 4 + 1 = 5
        ///      
        ///  For a further reading see the link 
        /// </summary>
        /// <param name="xIndex">elements position x value</param>
        /// <param name="yIndex">elements position y value</param>
        /// <link>https://en.wikipedia.org/wiki/Row-_and_column-major_order</link>
        /// <returns></returns>
        public int GetMatrixElementIndexByX_Y(int xIndex, int yIndex) => (xIndex * YLength) + yIndex;
        /// <summary>
        /// Returns the gridbox element out of the grid list given a condition
        /// </summary>
        /// <param name="condition">Predicate condition to be met on finding the element</param>
        /// <returns>Gives back the Gridbox found or null</returns>
        public GridBox FindCell(Predicate<GridBox> condition) => Grids.Find(condition);
        #endregion

        /// <summary>
        /// Prints the battlefield created to the console
        /// </summary>
        /// <param name="Lines">Number of lines </param>
        /// <param name="Columns">Number of columns</param>
        /// <param name="shouldDrawBattlefield">Battlefield rendering condition</param>
        /// <param name="clearTheConsole">Should it clear the console</param>
        public void DrawBattlefield(int Lines, int Columns, bool shouldDrawBattlefield = true, bool clearTheConsole = true)
        {
            //Handling clearing the console and printing the games status feedbacks
            if (clearTheConsole) Console.Clear();
            if (shouldDrawBattlefield)
                //The status starting position is given by the amount of columns the grid has and its height is centered on the lines. The offset corresponds to the amount of characters that make up the printing "[ ]     "
                _hud.RenderStatus(new Vector2(Columns, (int)(Lines / 2)), new Vector2(Columns * 5, 2));

            //References to the variables used on this method
            Console.CursorTop = 0;
            GridBox currentgrid = new GridBox();
            int currentGridElementIndex = 0;
            Character character = null;
            Team team = null;
            Console.CursorLeft = 0;

            //prints out the battlefield title and some # to fit the battlefield
            _hud.MessageFormatter("BATTLEFIELD  ");
            if (Columns > 3)
            {
                for (var i = 0; i < (Columns - 3) * 5; i++)
                {
                    _hud.MessageFormatter("#");
                }
            }
            _hud.MessageFormatter("\n\n");

            //Loops through the matrix one line at a time
            for (int i = 0; i < Lines; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    //Gets the current element index by the i and j values
                    currentGridElementIndex = GetMatrixElementIndexByX_Y(i, j);
                    //and retrieves the element from the grids array
                    currentgrid = GetElementAt(currentGridElementIndex);
                    //If the grid position is occupied, prints out the character's class initial
                    if (currentgrid.isOccupied)
                    {
                        //Retrieves the character from the list of all characters
                        character = Program.AllPlayers.Find(el => el.PlayerIndex == currentgrid.index);
                        if (character == null)
                        {
                            _hud.ClearConsole();
                        }
                        //Gets the character's team for printing him in his team colour
                        team = Program.AllTeams.Find(t => t.ID == character.TeamIndex);

                        //Sets the colour and prints the initial
                        _hud.ConsoleForeground(team.TeamColour);
                        Console.Write($"[{GetCharacterClassInitial(character.ClassSpecific.characterClass)}]  ");
                        _hud.ResetForeground();
                    }
                    else
                    {
                        //Otherwise just prints an blank space
                        Console.Write($"[ ]  ");
                    }
                }
                Console.Write(Environment.NewLine + Environment.NewLine);
            }
            Console.Write(Environment.NewLine + Environment.NewLine);
        }

    }
}
