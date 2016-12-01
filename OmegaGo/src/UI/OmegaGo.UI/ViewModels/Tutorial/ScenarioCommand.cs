using OmegaGo.Core;

namespace OmegaGo.UI.ViewModels.Tutorial
{
    /// <summary>
    /// Represents a command of the scenario domain-specific language. A command may be, for example, the instruction
    /// to change the caption of the Next button or the instruction to place a stone on the board, or to ask the player
    /// to take an action.
    /// </summary>
    internal abstract class ScenarioCommand
    {
        /// <summary>
        /// Executes this command. The return value determines whether the following command should immediately be executed afterwards.
        /// </summary>
        /// <param name="scenario">The scenario that the command is part of.</param>
        public abstract LoopControl Execute(Scenario scenario);

        /// <summary>
        /// Called from the user interface when this is the last command that was executed before the user clicked on the board.
        /// </summary>
        /// <param name="position">The position the user clicked on.</param>
        /// <param name="scenario">The scenario that the command is part of.</param>
        public virtual void BoardClick(Position position, Scenario scenario)
        {

        }
        /// <summary>
        /// Called from the user interface when this is the last command that was executed before the user selected an option
        /// from the dialogue menu.
        /// </summary>
        /// <param name="index">The index of the option (zero or one).</param>
        /// <param name="scenario">The scenario that the command is part of.</param>
        public virtual void OptionClick(int index, Scenario scenario)
        {
            
        }
        /// <summary>
        /// Called from the user interface when this is the last command that was executed before the user clicked on the
        /// Next button.
        /// </summary>      
        /// <param name="scenario">The scenario that the command is part of.</param>
        public virtual void ButtonClick(Scenario scenario)
        {
            
        }
    }
}