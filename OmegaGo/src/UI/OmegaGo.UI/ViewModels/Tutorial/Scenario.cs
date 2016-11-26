using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core;

namespace OmegaGo.UI.ViewModels.Tutorial
{
    public class Scenario : INotifyPropertyChanged
    {
        public DialogueLine FirstLine { get; protected set; }
        private DialogueLine _currentLine;
        private GameTreeNode _currentGameTreeNode = null;

        public DialogueLine CurrentLine
        {
            get { return _currentLine; }
            set
            {
                _currentLine = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentLine)));
            }
        }

        public event EventHandler<DialogueLine> NewLineSpoken;
        public event EventHandler<bool> NextButtonVisibilityChanged;
        public event EventHandler ScenarioCompleted;
        public event EventHandler<Tuple<string, string>> SetChoices;
        public event EventHandler ClearChoices;
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<GameTreeNode> GameTreeNodeChanged;

        internal void OnNextButtonVisibilityChanged(bool newVisibility)
        {
            NextButtonVisibilityChanged?.Invoke(this, newVisibility);
        }

        internal void OnNewLineSpoken(DialogueLine dialogueLine)
        {
            NewLineSpoken?.Invoke(this, dialogueLine);
        }

        internal void OnScenarioCompleted()
        {
            ScenarioCompleted?.Invoke(this, EventArgs.Empty);
        }

        public void Highlight(string position)
        {
            
            // TODO do the highlight
        }

        internal void OnSetChoices(string one, string two)
        {
            SetChoices?.Invoke(this, new Tuple<string, string>(one, two));
        }

        internal void OnClearChoices()
        {
            ClearChoices?.Invoke(this, EventArgs.Empty);
        }

        public void ClearHighlights()
        {
            // TODO do the highlight
        }

        public void PlaceStone(StoneColor stoneColor, string coordinate)
        {
            Position position = Position.FromIgsCoordinates(coordinate);
            var newNode =  new GameTreeNode(Move.PlaceStone(stoneColor, position));
            if (_currentGameTreeNode == null)
            {
                _currentGameTreeNode = newNode;
            }
            else
            {
                _currentGameTreeNode.Branches.AddNode(newNode);
                _currentGameTreeNode = newNode;
            }
            GameTreeNodeChanged?.Invoke(this, _currentGameTreeNode);
        }
    }
}
