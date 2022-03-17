using System;
using System.Collections.Generic;
using static AutoBattle.Types;
using static AutoBattle.UserInputs;
using static AutoBattle.Utils;
namespace AutoBattle
{
    /// <summary>
    /// Class responsible for the core game logic
    /// </summary>
    class Program
    {
        #region References Declaration
        // References to a PlayerCharacter and EnemyCharacter
        public static Character PlayerCharacter;
        public static Character EnemyCharacter;
        // For Layout reasons, limiting the maximun amount of teams to be 4
        public static int maxTeamCount = 5;
        // References to all characters in game, all teams playing and the winner team
        public static List<Character> AllPlayers = new List<Character>();
        public static List<Team> AllTeams = new List<Team>();
        public static Team winnerTeam = null;
        #endregion
        static void Main(string[] args)
        {
            #region UserInput Gathering 
            // New instance of the HUD class used to print out messages in this class
            HUD hud = new HUD();
            // Default attribution to some variables
            int gridHeight = 5, gridWidth = 5, numberOfTeams = 2;
            //Clearing the console and showing Welcoming messages
            hud.ClearConsole();
            hud.MessageFormatter("\n", "----------", " Welcome to the AutoBattle! ", "----------", "\n\n", "Here you must fight to the death...", "\n");

            // Gathering information relevant to the grid size in height and width, and the number of teams from the user
            GetUserInput("Let's summon a new battlefield! How large should it be? [2-8]\t", out gridWidth);
            GetUserInput("How tall should the battlefield be? [2-8]\t", out gridHeight);
            GetUserInputTeams("How many teams maximum, are in for the blood bath? [2-4]\t", gridHeight, gridWidth, out numberOfTeams);

            // Creating an instance of the grid Class.
            Grid grid = new Grid(gridHeight, gridWidth);

            // Character Factory instance instantiated.
            CharacterFactory characterFactory = new CharacterFactory(grid);
            #endregion

            Setup();

            #region GameLogic

            void Setup() => GetPlayerChoice();

            /// <summary>
            /// First method in a chain that's going to run until the start turn where the game main thread is going to run
            /// </summary>
            void GetPlayerChoice()
            {
                // Asks the player to choose between for possible classes via console.
                hud.MessageFormatter("Now, choose between one of the following Classes:", "\n", "[1] Paladin, [2] Warrior, [3] Cleric, [4] Archer", "\n");

                // Store the player choice in a variable
                string choice = Console.ReadLine();
                // Parsing the player choice into an integer and then parsing it into a Character Class option
                Int32 choiceValue = 0;
                Int32.TryParse(choice, out choiceValue);
                CharacterClass playersChosenClass = (CharacterClass)choiceValue;

                //Depending on the class selected a new Player will be created, otherwise this method will be called recursively
                switch (playersChosenClass)
                {
                    case CharacterClass.Paladin:
                        CreatePlayerCharacter(playersChosenClass);
                        break;
                    case CharacterClass.Warrior:
                        CreatePlayerCharacter(playersChosenClass);
                        break;
                    case CharacterClass.Cleric:
                        CreatePlayerCharacter(playersChosenClass);
                        break;
                    case CharacterClass.Archer:
                        CreatePlayerCharacter(playersChosenClass);
                        break;
                    default:
                        GetPlayerChoice();
                        break;
                }
            }


            /// <summary>
            /// Given the player choice inputed a new Player class will be created by the factory and the Method 'CreateEnemyCharacter' will be called creating enemies. The next method on the chain called is the StartGame method.
            /// </summary>
            /// <param name="choice">ChacaterClass typed input</param>
            void CreatePlayerCharacter(CharacterClass choice)
            {
                // A Character is created and added to the list of all players
                PlayerCharacter = characterFactory.CreatePlayer(choice);
                AllPlayers.Add(PlayerCharacter);
                hud.MessageFormatter($"{PlayerCharacter.Name} has entered the game!\n\n");

                // Creation of random enemies to be added to the battlefield. If the battlefield dimesion is too small, it should prevent the system from creating too many enemies. Max of enemies 5
                bool isGridSizeIsSmallerThan3x3 = gridHeight * gridWidth > 9;
                // Given the number of teams set by the player, the enemies count should be proportional to a parcel of the grid
                int enemiesForSmallGrids = maxTeamCount + (int)(gridHeight + gridWidth) / 4;
                // The maximum number of enemies is the grid dimension with a gap so they can move in need
                int enemiesForBiggerGrids = gridHeight * gridWidth - 2;

                //The number of enemies is given and Enemies are created up to that amount
                int maximumNumberOfEnemies = isGridSizeIsSmallerThan3x3 ? enemiesForSmallGrids : enemiesForBiggerGrids;

                int maxEnemiesCount = GetRandomInt(numberOfTeams - 1, maximumNumberOfEnemies);
                for (var i = 0; i < maxEnemiesCount; i++)
                {
                    CreateEnemyCharacter();
                }

                StartGame();
            }

            /// <summary>
            /// This method allocates the characters within the grid, set them up into different teams and gives all characters a default target. Also, it's responsible for the first render of the battlefield. The next method in the chain is 'StartTurn'.
            /// </summary>
            void StartGame()
            {
                AllocatePlayers();
                SetUpTeams();
                SetTargets();

                //Snapshot of the beggining battlefield
                grid.DrawBattlefield(gridHeight, gridWidth);
                hud.ReadKeyboardInput("\nAll set for battle... Brace yourselves!\nPress any key to continue...");

                StartTurn();

            }

            /// <summary>
            /// 
            /// </summary>
            void SetTargets()
            {
                AllPlayers.ForEach(player =>
                {
                    Character target = AllPlayers[GetRandomInt(0, AllPlayers.Count)];

                    while (player.Target == null)
                    {
                        if (target.PlayerIndex != player.PlayerIndex && target.TeamIndex != player.TeamIndex)
                        {
                            player.Target = target;
                        }
                        target = AllPlayers[GetRandomInt(0, AllPlayers.Count)];
                    }

                });
            }

            /// <summary>
            /// Method responsible for creating all the teams in the game. Each new team is added to the list of all teams and after that the players are allocated into teams
            /// </summary>
            void SetUpTeams()
            {
                for (var i = 0; i < numberOfTeams; i++)
                {
                    AllTeams.Add(new Team($"Team {i + 1}", i));
                }
                hud.MessageFormatter($"{AllTeams.Count} teams have been created\n\n");
                AlocatePlayersInTeams();
                hud.ReadKeyboardInput("Press any key to continue...");
            }

            /// <summary>
            /// Method responsible for distributing randomly all the characters into teams
            /// </summary>
            void AlocatePlayersInTeams()
            {
                //Definition of all the variables used in this method
                List<Character> copyPlayers = new List<Character>(AllPlayers);
                int randomPlayerIdex = 0;
                int allTeamsIndexer = 0;
                Character currentCharacter = null;
                Team currentTeam = null;

                // A copy of the AllPlayers list is created and its looped through whilst there are elements in it
                while (copyPlayers.Count > 0)
                {
                    //A random element of the AllPlayers copy list is selected and its index is cached. Then, the character is set to currentCharacter as a reference
                    randomPlayerIdex = GetRandomInt(0, copyPlayers.Count);
                    currentCharacter = copyPlayers[randomPlayerIdex];

                    //Looping through all positions of the AllTeams list, so no team is left empty, and adding the random player selected to said team. 'allTeamsIndexer' will do as a threshold on the iteration. 
                    currentTeam = AllTeams[allTeamsIndexer];

                    //The current character is added to members and its TeamIndex is set to be the allTeamsIndexer
                    currentTeam.AddMember(currentCharacter);
                    currentCharacter.TeamIndex = allTeamsIndexer;
                    //Removing the player already sorted into a team from the outer loop - 'while condition' and increasing the threshold 'allTeamsIndexer'
                    copyPlayers.RemoveAt(randomPlayerIdex);
                    allTeamsIndexer++;

                    //Once all teams are visited its indexer is reset so the remaining players can be once more alocated
                    if (allTeamsIndexer == AllTeams.Count)
                    {
                        allTeamsIndexer = 0;
                    }
                }
                //Printing a resume of all teams
                Team.PrintAllTeams();
            }

            /// <summary>
            /// The Character Factory creates a new Random Character and it is added to the list of all players
            /// </summary>
            void CreateEnemyCharacter()
            {
                EnemyCharacter = characterFactory.Create();
                AllPlayers.Add(EnemyCharacter);
            }

            /// <summary>
            /// Key method. It's responsible for setting the conditions to make each team have a new turn, selecting a different character as the actor of its turn
            /// </summary>
            void StartTurn()
            {
                //All variables used in this method
                int teamsCount = 0;
                Team currentTeam = null;

                int indexer = 0, playersCount = AllPlayers.Count;

                //While there isn't a winnerTeam, this method will run
                while ((winnerTeam == null))
                {
                    //The game checks for a winner before a new turn is started
                    CheckForWinner();

                    //All the teams are going to be run through. The first team will have its turn, then the next team, and so on.
                    ///E.g:             ↓ = indexer      
                    ///  i →   team 0 [[0] [1] [2]    ]
                    ///        team 1 [[0] [1]        ]
                    ///        team 2 [[0] [1] [2] [3]]
                    /// 
                    for (int i = 0; i < AllTeams.Count; i++)
                    {
                        //Another check for winners before starting the teams' new turn
                        CheckForWinner();
                        //A text area below the grid is cleared for new information
                        hud.ClearTextArea(0, gridHeight + 8);

                        //Reference to the current team selected by the for loop 
                        currentTeam = AllTeams[i];
                        if (indexer < currentTeam.Count())
                        {
                            //A new character is selected on the turns current Team
                            Character ch = currentTeam.GetMemeberAt(indexer);

                            hud.TextFormatter(gridHeight + 8, $"the champion selected for this turn is:");
                            hud.ConsoleForeground(currentTeam.TeamColour);
                            hud.TextFormatter(gridHeight + 8, $" {currentTeam.Name}'s {ch.Name} - positioned on ({ch.currentBox.position.ToString()})");
                            hud.ResetForeground();
                            // If the reference is null, it means there are no characters on that position left on the team
                            if (ch != null)
                            {
                                //The character will have its turn
                                hud.ReadKeyboardInput($"\nIt's the beginning of {currentTeam.Name}'s new turn...\n");
                                ch.StartTurn();
                            }
                        }

                    }
                    //If the indexer is greater than the teamsCount it means that all the elements on all teams have been met
                    if (indexer > teamsCount)
                        indexer = 0;
                    else
                        indexer++;
                }

            }

            /// <summary>
            /// This method is responsible for looking for winners and stopping the game once a winner is found
            /// </summary>
            void CheckForWinner()
            {
                if (AllTeams.Count > 1) return;
                winnerTeam = AllTeams[0];
                hud.TextFormatter(gridHeight + 8, $"\nWOW! The {AllTeams[0].Name} was the great winner!\n");
                hud.ReadKeyboardInput("Thank you for running this playful experience. See you next time");
                System.Environment.Exit(1);

            }

            /// <summary>
            /// A new chain of methods that will allocate all the players within the grid
            /// </summary>
            void AllocatePlayers()
            {
                AllocatePlayerCharacter();
            }

            /// <summary>
            /// This method will look for a random position in the grid and place the player there. Next method on the chain is 'AllocateEnemyCharacter'
            /// </summary>
            void AllocatePlayerCharacter()
            {
                //Getting a random index from 0 - the grid size
                int random = PlayerCharacter.PlayerIndex;

                //Getting a reference to the location found
                GridBox RandomLocation = grid.GetElementAt(random);
                GridBox PlayerCurrentLocation;

                //If the position found is occupied, run this method once more
                if (!RandomLocation.isOccupied)
                {
                    //The reference to the currentLocation is set
                    PlayerCurrentLocation = RandomLocation;
                    PlayerCurrentLocation.isOccupied = true;
                    //The player character reference's current box is set to this new random location
                    PlayerCharacter.currentBox = RandomLocation;
                    //The grid position is set to the random location selected
                    grid.SetElementAt(random, PlayerCurrentLocation);
                    //Time to allocate the enemy
                    AllocateEnemyCharacter();
                }
                else
                    AllocatePlayerCharacter();
            }
            /// <summary>
            /// Method responsible for allocating the All the other characters in the grid
            /// </summary>
            void AllocateEnemyCharacter()
            {
                //Looping through all players
                for (int i = 0; i < AllPlayers.Count; i++)
                {
                    //Getting the enemies, skipping the player that has already been allocated
                    if (AllPlayers[i].PlayerIndex != PlayerCharacter.PlayerIndex)
                    {
                        //Getting a reference to the current enemy
                        Character enemy = AllPlayers[i];
                        //Reference to the current enemy's player index
                        int enemyIndex = enemy.PlayerIndex;

                        //Finding the Gridbox given the current enemy's index - The position he is supposed to be at.
                        GridBox RandomLocation = grid.GetElementAt(enemyIndex);

                        //Once the enemy is not in the grid of occupied and the gridbox to be is empty we set the enemy's currentbox to be occupied and equals to the random location selected
                        if (!RandomLocation.isOccupied)
                        {
                            enemy.currentBox = RandomLocation;
                            enemy.currentBox.isOccupied = true;
                            //Updating the list of gridboxes with the newly updated box
                            grid.SetElementAt(enemyIndex, enemy.currentBox);
                        }
                    }
                }
            }
            #endregion
        }
    }
}
