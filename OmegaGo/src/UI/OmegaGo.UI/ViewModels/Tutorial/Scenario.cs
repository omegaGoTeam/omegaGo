using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core;
using OmegaGo.Core.AI.Common;
using OmegaGo.UI.Services.Game;

namespace OmegaGo.UI.ViewModels.Tutorial
{
    public abstract class Scenario
    {
        private List<ScenarioCommand> _commands;
        private ScenarioCommand _lastCommandExecuted;
        private int _nextCommand = 0;
        private GameTreeNode _currentGameTreeNode = null;


        public event EventHandler<string> SenseiMessageChanged;
        public event EventHandler<string> NextButtonTextChanged;
        public event EventHandler<Tuple<string, string>> SetChoices;
        public event EventHandler NextButtonShown;
        public event EventHandler ScenarioCompleted;
        public event EventHandler<GameTreeNode> GameTreeNodeChanged;
        public event EventHandler<Tuple<Position, bool>> ShiningChanged;
        

        internal void OnScenarioCompleted()
        {
            ScenarioCompleted?.Invoke(this, EventArgs.Empty);
        }

        internal void OnSetChoices(string one, string two)
        {
            SetChoices?.Invoke(this, new Tuple<string, string>(one, two));
        }

        internal void ClearBoard()
        {
            _currentGameTreeNode = new GameTreeNode(Move.Pass(StoneColor.Black));
            _currentGameTreeNode.BoardState = new StoneColor[9, 9];
            GameTreeNodeChanged?.Invoke(this, _currentGameTreeNode);
        }
        internal void PlaceStone(StoneColor stoneColor, Position position)
        {
            var newNode =  new GameTreeNode(Move.PlaceStone(stoneColor, position));
            StoneColor[,] fullBoardPosition;
            if (_currentGameTreeNode == null)
            {
                _currentGameTreeNode = newNode;
                fullBoardPosition = new StoneColor[9, 9];
            }
            else
            {
                _currentGameTreeNode.Branches.AddNode(newNode);
                fullBoardPosition = _currentGameTreeNode.BoardState;
                _currentGameTreeNode = newNode;
            }
            StoneColor[,] newBoardPosition = FastBoard.CloneBoard(fullBoardPosition);
            newBoardPosition[position.X, position.Y] = stoneColor;
            _currentGameTreeNode.BoardState = newBoardPosition;
            GameTreeNodeChanged?.Invoke(this, _currentGameTreeNode);
        }
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

        protected void LoadCommandsFromText(string data)
        {
            _commands = ScenarioLoader.LoadFromText(data, this);
        }

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

      
    }
}
