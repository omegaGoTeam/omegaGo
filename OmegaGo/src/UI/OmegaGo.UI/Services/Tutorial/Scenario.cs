using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core;
using OmegaGo.Core.Game;
using OmegaGo.UI.Services.Game;

namespace OmegaGo.UI.ViewModels.Tutorial
{
    /// <summary>
    /// Represents a story-driven experience where the player follows a dialogue with a teacher while making moves as directed
    /// on the Go board. We only have a single scenario, the <see cref="BeginnerScenario"/>. 
    /// </summary>
    public abstract class Scenario
    {
        private List<ScenarioCommand> _commands;
        private ScenarioCommand _lastCommandExecuted;


        private int _nextCommand;
        private GameTreeNode _currentGameTreeNode;


        public event EventHandler<string> SenseiMessageChanged;
        public event EventHandler<string> NextButtonTextChanged;
        public event EventHandler<Tuple<string, string>> SetChoices;
        public event EventHandler NextButtonShown;
        public event EventHandler ScenarioCompleted;
        public event EventHandler<Position> ShiningPositionChanged;
        public event EventHandler<GameTreeNode> GameTreeNodeChanged;

        protected void LoadCommandsFromText(string data)
        {
            _commands = ScenarioLoader.LoadFromText(data);
        }

        // Event Invocation Methods

        internal void OnSetShiningPosition(Position position)
        {
            ShiningPositionChanged?.Invoke(this, position);
        }
        internal void OnScenarioCompleted()
        {
            ScenarioCompleted?.Invoke(this, EventArgs.Empty);
        }
        internal void OnSetChoices(string one, string two)
        {
            SetChoices?.Invoke(this, new Tuple<string, string>(one, two));
        }
        internal void OnSenseiMessageChanged(string sayWhat)
        {
            this.SenseiMessageChanged?.Invoke(this, sayWhat);
        }
        internal void OnNextButtonShown()
        {
            this.NextButtonShown?.Invoke(this, EventArgs.Empty);
        }
        internal void OnNextButtonTextChanged(string newText)
        {
            this.NextButtonTextChanged?.Invoke(this, newText);
        }

        // Game Board Manipulation

        internal void ClearBoard()
        {
            _currentGameTreeNode = new GameTreeNode(Move.Pass(StoneColor.Black));
            _currentGameTreeNode.BoardState = new GameBoard(new GameBoardSize(9));
            GameTreeNodeChanged?.Invoke(this, _currentGameTreeNode);
        }
        internal void PlaceStone(StoneColor stoneColor, Position position)
        {
            var newNode =  new GameTreeNode(Move.PlaceStone(stoneColor, position));
            GameBoard fullBoardPosition;
            if (_currentGameTreeNode == null)
            {
                _currentGameTreeNode = newNode;
                fullBoardPosition = new GameBoard(new GameBoardSize(9));
            }
            else
            {
                _currentGameTreeNode.Branches.AddNode(newNode);
                fullBoardPosition = _currentGameTreeNode.BoardState;
                _currentGameTreeNode = newNode;
            }
            GameBoard newBoardPosition = new GameBoard(fullBoardPosition);
            newBoardPosition[position.X, position.Y] = stoneColor;
            _currentGameTreeNode.BoardState = newBoardPosition;
            GameTreeNodeChanged?.Invoke(this, _currentGameTreeNode);
        }

        // External Access Points

        public void ClickNext()
        {
             _lastCommandExecuted.ButtonClick(this);
        }
        public void ClickOption(int optionNumber)
        {
            _lastCommandExecuted.OptionClick(optionNumber, this);
        }
        public void ClickPosition(Position position)
        {
            _lastCommandExecuted.BoardClick(position, this);
        }

        /// <summary>
        /// Executes the next command in the scenario, and then continues executing further commands until a command
        /// returns <see cref="LoopControl.Stop"/> as its return value. 
        /// </summary>
        public void ExecuteCommand()
        {
            ScenarioCommand command = _commands[_nextCommand];
            _nextCommand++;
            LoopControl whatNext = command.Execute(this);
            _lastCommandExecuted = command;
            if (whatNext == LoopControl.Continue)
            {
                ExecuteCommand();
            }
        }
    }
}
