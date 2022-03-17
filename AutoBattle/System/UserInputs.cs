using static AutoBattle.Utils;

namespace AutoBattle
{
    /// <summary>
    /// The UserInput class is responsible for dealing with the input communication Human Computer Interaction (HCI);
    /// </summary>
    public static class UserInputs
    {
        // Reference to a hud element
        private static HUD _hud = new HUD();

        /// <summary>
        /// This method is responsible for getting the user input and validating it
        /// </summary>
        /// <param name="outputMessage">Message to be displayed on console along with the reading</param>
        /// <param name="output">the output value out of this validation as an integer</param>
        public static void GetUserInput(string outputMessage, out int output)
        {
            //Checks if the input is an integer
            string userInput = _hud.MessageInputer(outputMessage);
            int userInputNumeric = parsedInputAsInteger(userInput);

            //Constraining the matrix size to not allow an unitary matrix or a matrix bigger than the max Side value. Keeps showing the message until a valid input
            if (userInputNumeric <= 1 || userInputNumeric > Grid.maxSide)
                GetUserInput(
                    _hud.MessageFormatterContains(outputMessage, "I apologize, I cannot use this value. ", false),
                    out output
                );

            else
                output = userInputNumeric;
        }

        /// <summary>
        /// Validates the user team input
        /// </summary>
        /// <param name="outputMessage">Message to be displayed on console along with the reading</param>
        /// <param name="height">The grid's height dimension</param>
        /// <param name="width">The grid's width dimension</param>
        /// <param name="output">the output value out of this validation as an integer</param>
        public static void GetUserInputTeams(string outputMessage, int height, int width, out int output)
        {
            //Checks if the input is an integer
            string userInput = _hud.MessageInputer(outputMessage);
            int userInputNumeric = parsedInputAsInteger(userInput);

            //Check for userInput lesser than two. Otherwise there would be just one big team
            if (userInputNumeric < 2)
            {
                GetUserInputTeams(
                   _hud.MessageFormatterContains(outputMessage, "I apologize, I cannot use this value. How shall they fight like that? They are not frieds... They must battle! ", false),
                   height,
                   width,
                   out output
               );
            }

            //Battlefield is smaller than [3, 3]. If the battlefield is [2, 3], [2,2], [3, 2] there's no room for more than 3 teams
            else if (height * width < 9 && userInputNumeric >= 3)
            {
                GetUserInputTeams(
                    _hud.MessageFormatterContains(outputMessage, "I apologize, I cannot use this value. The battlefield is too small for that many people. ", false),
                    height,
                    width,
                    out output
                );
            }

            //Constraining the user team input to be smaller than 5 teams 
            else if (userInputNumeric <= 1 || userInputNumeric > Program.maxTeamCount - 1)
                GetUserInputTeams(
                    _hud.MessageFormatterContains(outputMessage, "I apologize, I cannot use this value. ", false),
                    height,
                    width,
                    out output
                );

            else
                output = userInputNumeric;
        }


    }

}