using System;
using System.Collections.Generic;
using static AutoBattle.Types;
using static AutoBattle.Utils;

namespace AutoBattle
{
    /// <summary>
    ///  The Character class is responsible for handling concrete implementation of the character's multiple states throughout the game
    /// </summary>
    public class Character : IComparable
    {
        #region Variables and Properties
        public string Name { get; set; }
        public int Health { get; set; }
        public int BaseDamage { get; set; }
        public int DamageMultiplier { get; set; }
        public GridBox currentBox;
        public int PlayerIndex { get; set; }
        public int TeamIndex { get; set; }
        public Character Target { get; set; }
        CharacterClass _CharacterClass { get; set; }
        public CharacterClassSpecific ClassSpecific = new CharacterClassSpecific();
        Grid _battlefield;
        HUD _hud = new HUD();
        #endregion

        /// <summary>
        /// Constructor to set some of the characters skills and other values
        /// </summary>
        /// <param name="characterClass">CharacterClass</param>
        /// <param name="grid">Grid's reference</param>
        public Character(CharacterClass characterClass, Grid grid)
        {
            ClassSpecific.characterClass = characterClass;
            Health = 100;
            BaseDamage = 20;
            _battlefield = grid;
            switch (characterClass)
            {
                case CharacterClass.Paladin:
                    DamageMultiplier = 2;
                    ClassSpecific.skills = new CharacterSkills[] { new CharacterSkills("Long Sword", 10, 10) };
                    BaseDamage *= (DamageMultiplier + ClassSpecific.classDamage);
                    break;
                case CharacterClass.Warrior:
                    DamageMultiplier = 5;
                    ClassSpecific.skills = new CharacterSkills[] { new CharacterSkills("Super speed arrow drawing", 12, 2) };
                    BaseDamage *= DamageMultiplier + ClassSpecific.skills[0].damageMultiplier;
                    break;
                case CharacterClass.Archer:
                    DamageMultiplier = 3;
                    BaseDamage *= DamageMultiplier;
                    ClassSpecific.skills = new CharacterSkills[] { new CharacterSkills("Inquisition Attack", 5, 10) };

                    break;
                case CharacterClass.Cleric:
                    BaseDamage = 10;
                    ClassSpecific.hpModifier = 20;
                    Health += ClassSpecific.hpModifier;
                    DamageMultiplier = 2;
                    ClassSpecific.skills = new CharacterSkills[] { new CharacterSkills("Inquisition Attack", 5, 10) };
                    BaseDamage *= DamageMultiplier;
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// This method is responsible for handling damage infliction on this object
        /// </summary>
        /// <param name="amount">How much damage was inflicted</param>
        /// <param name="attacker">who attacted the player</param>
        /// <returns>returns true if the character's died, false if he hasn't</returns>
        public bool TakeDamage(int amount, Character attacker)
        {
            //Decreases the damage amount from the characters health
            if ((Health -= amount) <= 0)
            {
                //If health is lesser or equals to 0, calls die method and returns true
                Die(attacker);
                return true;
            }
            _hud.MessageFormatter($"{this.Name} has {this.Health} points of Health left\n");
            return false;
        }

        /// <summary>
        /// Method that handles the character's death state
        /// </summary>
        /// <param name="attacker">who attacked the character</param>
        public void Die(Character attacker)
        {
            //The character team is found
            Team characterTeam = Program.AllTeams.Find(t => t.ID == this.TeamIndex);

            //Reset of Character's currentbox state to false, resetting the element on the battlefield and removing him from his team
            this.currentBox.isOccupied = false;
            _battlefield.SetElementAt(PlayerIndex, this.currentBox);
            characterTeam.RemoveMember(this);

            //A new target is assigned for all the other characters that were targeting this dead character
            Program.AllPlayers.FindAll(x => x.Target.PlayerIndex == PlayerIndex).ForEach(element =>
            {
                FindNewTarget(element);
            });
            Console.ReadKey();
            _hud.ReadKeyboardInput($"\nThe {Name} has fallen");
            //redrawing the battlefield without the dead character
            _battlefield?.DrawBattlefield(_battlefield.XLenght, _battlefield.YLength, true, true);
        }

        /// <summary>
        /// Finds a new target to the character that was previously attacking the character that died
        /// </summary>
        /// <param name="attacker">Attacking character that has no longer a target</param>
        public void FindNewTarget(Character attacker)
        {
            //If there's only one team, no need to look for a target, the game will be over
            Team targetTeam = null;
            if (Program.AllTeams.Count == 1) return;

            //A new target team index is randomly selected if its different from the attacker's
            int newTargetTeamIndex = GetRandomInt(0, Program.AllTeams.Count);
            while (newTargetTeamIndex == attacker.TeamIndex)
            {
                newTargetTeamIndex = GetRandomInt(0, Program.AllTeams.Count);
            }
            //The character is searched on the list of all teams and a re-check is run, then the character is set, otherwise another element is set straight from the list
            if (Program.AllTeams[newTargetTeamIndex].ID != attacker.TeamIndex)
                targetTeam = Program.AllTeams[newTargetTeamIndex];
            else
                targetTeam = Program.AllTeams.Find(t => t.ID != attacker.TeamIndex);
            //A random element from the randomly selected team is get
            Character newTarget = targetTeam.GetMemeberAt(GetRandomInt(0, targetTeam.Members.Count));
            //Another check is run to make sure the newly selected characted is a valid enemy from another team
            while (newTarget.PlayerIndex == attacker.PlayerIndex && newTarget.TeamIndex == attacker.TeamIndex)
            {
                newTarget = targetTeam.GetMemeberAt(GetRandomInt(0, targetTeam.Members.Count));
            }
            attacker.Target = newTarget;
        }

        /// <summary>
        /// Starting point for the character's turn
        /// </summary>
        public void StartTurn()
        {
            //If the character has targets close to him he may attack
            _hud.MessageFormatter($"{Name} has now to choose...\n");
            if (CheckCloseTargets(_battlefield))
            {
                _hud.MessageFormatter($"Tan tan tan nanan... ♫♩♪ It's battle time!\n");
                Attack(Target);
            }
            //Otherwise he moves closer to his target
            else
            {
                _hud.MessageFormatter($"{Target.Name} is not within {Name}'s attacking range...\n");
                MoveTo(_battlefield);
            }

        }

        /// <summary>
        /// This method handles the characters movement to another space in the grid
        /// </summary>
        /// <param name="battlefield"></param>
        public void MoveTo(Grid battlefield)
        {
            //Checks if the characters current position is one of the grid's edges
            bool leftEdge = this.currentBox.position.xIndex - 1 < 0;
            bool rightEdge = this.currentBox.position.xIndex + 1 >= battlefield.XLenght;
            bool upEdge = this.currentBox.position.yIndex - 1 < 0;
            bool downEdge = this.currentBox.position.yIndex + 1 >= battlefield.YLength;

            //Gets direction he may or may not move to
            bool moveRight = (this.currentBox.position.xIndex < Target.currentBox.position.xIndex);
            bool moveLeft = (this.currentBox.position.xIndex > Target.currentBox.position.xIndex);
            bool moveDown = (this.currentBox.position.yIndex < Target.currentBox.position.yIndex);
            bool moveUp = (this.currentBox.position.yIndex > Target.currentBox.position.yIndex);

            //Handles X Axis movement
            //Checks for right first
            if (moveRight && !rightEdge)
            {
                //Gets the referece to the cell on his right
                GridBox cellToTheRight = battlefield.FindCell(x => x.index == currentBox.index + 1);
                //the position to the right is empty
                if (!cellToTheRight.isOccupied)
                {
                    _hud.MessageFormatter("He's moved to the right, closer to the target");
                    SwapGridBox(battlefield, this.currentBox, battlefield.FindCell(x => x.index == currentBox.index + 1));
                    return;
                }

            }
            //Checks for left then
            else if (moveLeft && !leftEdge)
            {
                GridBox cellToTheLeft = battlefield.FindCell(x => x.index == currentBox.index - 1);

                //the position to the left is empty
                if (!cellToTheLeft.isOccupied)
                {
                    _hud.MessageFormatter("He's moved to the left, closer to the target");
                    SwapGridBox(battlefield, this.currentBox, battlefield.FindCell(x => x.index == currentBox.index - 1));
                    return;
                }
            }

            //Handles Y Axis movement
            //Checks for down first
            if (!moveUp && !downEdge)
            {
                GridBox cellDownwards = battlefield.FindCell(x => x.index == currentBox.index + battlefield.XLenght);
                //the position downwards is empty
                if (!cellDownwards.isOccupied)
                {
                    _hud.MessageFormatter("He's moved downwards, closer to the target");
                    SwapGridBox(battlefield, this.currentBox, battlefield.FindCell(x => x.index == currentBox.index + battlefield.XLenght));
                    return;
                }

            }
            //Checks for up then
            else if (moveUp && !upEdge)
            {
                GridBox cellUpwards = battlefield.FindCell(x => x.index == currentBox.index - battlefield.XLenght);
                //the position upwards is empty
                if (!cellUpwards.isOccupied)
                {
                    _hud.MessageFormatter("He's moved upwards, closer to the target");
                    SwapGridBox(battlefield, this.currentBox, battlefield.FindCell(x => x.index == currentBox.index - battlefield.XLenght));
                    return;
                }

            }

            _hud.MessageFormatter($"It seems like {Name} has skipped his turn... \n");
        }

        /// <summary>
        /// This method checks if the character at a position is a possible character target 
        /// </summary>
        /// <param name="position">the position to look for</param>
        /// <returns>returns true either is a possible target or false if not</returns>
        bool isTarget(Vector2 position)
        {
            //gets the character at the position
            Character character = Program.AllPlayers.Find(el => el.currentBox.position.CompareTo(position) > 0);
            //if the character is a possible target return him 
            bool isPossibleTarget = character != null && character.TeamIndex != this.TeamIndex;
            Target = isPossibleTarget ? character : Target;
            return isPossibleTarget;
        }

        /// <summary>
        /// Method that performs the grid swaping
        /// </summary>
        /// <param name="battlefield">grid reference</param>
        /// <param name="from">starting gridbox</param>
        /// <param name="to">ending gridbox</param>
        void SwapGridBox(Grid battlefield, GridBox from, GridBox to)
        {
            //Resets the "from" gridbox, sets the character's properties to the "to" gridbox
            from.isOccupied = false;
            to.isOccupied = true;
            this.currentBox = to;
            this.PlayerIndex = to.index;
            //Updates the battlefield with the changes
            battlefield.SetElementAt(this.currentBox.index, this.currentBox);
            battlefield.SetElementAt(from.index, from);
            //Updates the all players list with the changes
            Program.AllPlayers.Find(e => e.PlayerIndex == this.PlayerIndex).currentBox = to;
            _hud.ReadKeyboardInput($"\nThe {Name} has moved towards the {Target.Name}");
            //Re-draws the battlefield
            battlefield.DrawBattlefield(battlefield.XLenght, battlefield.YLength, true, true);

        }

        /// <summary>
        /// Checks every direction for a new target
        /// </summary>
        /// <param name="battlefield">Reference to the grid</param>
        /// <returns>returns true either there's a target, else in case there isn't</returns>
        bool CheckCloseTargets(Grid battlefield)
        {
            //Given the current position, find the grid edges
            Vector2 currentPosition = currentBox.position;
            bool leftEdge = currentPosition.xIndex - 1 < 0;
            bool rightEdge = currentPosition.xIndex + 1 >= battlefield.XLenght;
            bool upEdge = currentPosition.yIndex - 1 < 0;
            bool downEdge = currentPosition.yIndex + 1 >= battlefield.YLength;

            //true if a position to the left is not an edge and is a valid target
            bool left = (battlefield.FindCell(x =>
                x.position.CompareTo(new Vector2(currentPosition.xIndex - 1, currentPosition.yIndex)) > 0 &&
                !leftEdge && isTarget(new Vector2(currentPosition.xIndex - 1, currentPosition.yIndex))
            ).isOccupied);
            //true if a position to the right is not an edge and is a valid target
            bool right = (battlefield.FindCell(x =>
                x.position.CompareTo(new Vector2(currentPosition.xIndex + 1, currentPosition.yIndex)) > 0 &&
                !rightEdge && isTarget(new Vector2(currentPosition.xIndex + 1, currentPosition.yIndex))
            ).isOccupied);
            //true if a position down is not an edge and is a valid target
            bool down = (battlefield.FindCell(x =>
                x.position.CompareTo(new Vector2(currentPosition.xIndex, currentPosition.yIndex + 1)) > 0 &&
                !downEdge && isTarget(new Vector2(currentPosition.xIndex, currentPosition.yIndex + 1))
            ).isOccupied);
            //true if a position up is not an edge and is a valid target
            bool up = (battlefield.FindCell(x =>
                x.position.CompareTo(new Vector2(currentPosition.xIndex, currentPosition.yIndex - 1)) > 0 &&
                !upEdge && isTarget(new Vector2(currentPosition.xIndex, currentPosition.yIndex - 1))
            ).isOccupied);

            return left || right || down || up;
        }

        /// <summary>
        /// Attacking method
        /// </summary>
        /// <param name="target">Reference to the character target</param>
        public void Attack(Character target)
        {
            //the attacking power is randomly set up to the target'ss base damage
            _hud.MessageFormatter($"{Name}'s target is {Target.Name} - positioned ({Target.currentBox.position.ToString()})\n");
            int attackPower = GetRandomInt(0, target.BaseDamage);
            _hud.MessageFormatter($"{Name} will inflict {attackPower} points of damage with his {target.ClassSpecific.skills[0].name} attack!\n");
            _hud.MessageFormatter($"{Name} is attacking the {Target.Name} and inflicted {attackPower} points of damage\n");
            //character attacks
            target.TakeDamage(attackPower, this);
            _hud.MessageFormatter($"{Name} finished the attack\n");
            //waits for input and then re-renders the status updating the team's healths
            _hud.ReadKeyboardInput();
            _hud.RenderStatus(new Vector2(_battlefield.XLenght, (int)(_battlefield.YLength / 2)), new Vector2(_battlefield.XLenght * 5, 2));
        }

        /// <summary>
        /// Method that compares two characters based upon their indexes
        /// </summary>
        /// <param name="obj">Object to be compared to</param>
        /// <returns>returns 1 if they're equal, -1 if not</returns>
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            Character otherChar = (Character)obj;
            return this.PlayerIndex.CompareTo(otherChar.PlayerIndex);
        }
    }
}
