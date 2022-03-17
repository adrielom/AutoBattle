# Game AutoBattle - DevLog

This is an RPG-ish game that automatically runs a battle fight between two characters of different types.

### Logs

1. The folder stucture was modified, some extra files - duplicated - such as the Character.cs and the Grid.cs were deleted.
2. The project core is located at the Program.cs file. It's going to run the game main thread and the game logic.
3. The HUD class is responsible for dealing with the output communication Human Computer Interaction (HCI);
4. The UserInput class is responsible for dealing with the input communication Human Computer Interaction (HCI);
5. The Grid class is responsible for creating and managing the Grid positioning and rendering.
6. The Character class is responsible for handling concrete implementation of the character's multiple states throughout the game
7. The Team class is responsible for managing the Teams' states.
8. The Utils class is gives a toolset of functions that can be accessed by multiple other classes interchangeably
9. The Types file unifies many different structs and classes that are used as data structures for the game logic
10. Based on my birthday date (Feb) I had to implement a team based system.
