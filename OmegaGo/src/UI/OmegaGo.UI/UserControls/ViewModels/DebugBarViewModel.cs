using MvvmCross.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.UserControls.ViewModels
{
    public sealed class DebugBarViewModel : ControlViewModelBase
    {
        private string _fuegoCommandText;

        // Debug stuff
        private IMvxCommand _fillCommand;
        private IMvxCommand _saveSGFCommand;
        // Life and death
        private IMvxCommand _lifeAndDeathDoneCommand;
        private IMvxCommand _resumeGameCommand;
        private IMvxCommand _requestUndoDeathMarksCommand;
        // Moves
        private IMvxCommand _passCommand;
        private IMvxCommand _resignCommand;
        private IMvxCommand _undoCommand;
        // AI Hint
        private IMvxCommand _getHintCommand;
        // Fuego commands
        private IMvxCommand _sendGTPCommand;

        public DebugBarViewModel()
        {

        }

        public string FuegoCommandText
        {
            get { return _fuegoCommandText; }
            set { SetProperty(ref _fuegoCommandText, value); }
        }
        
        public IMvxCommand FillCommand
            => _lifeAndDeathDoneCommand ?? (_lifeAndDeathDoneCommand = new MvxCommand(Fill));

        public IMvxCommand SaveSGFCommand
            => _resumeGameCommand ?? (_resumeGameCommand = new MvxCommand(SaveSGF));


        public IMvxCommand LifeAndDeathDoneCommand
            => _lifeAndDeathDoneCommand ?? (_lifeAndDeathDoneCommand = new MvxCommand(LifeAndDeathDone));

        public IMvxCommand ResumeGameCommand
            => _resumeGameCommand ?? (_resumeGameCommand = new MvxCommand(ResumeGame));

        public IMvxCommand RequestUndoDeathMarksCommand
            => _requestUndoDeathMarksCommand ??
            (_requestUndoDeathMarksCommand = new MvxCommand(RequestUndoDeathMarks));


        public IMvxCommand PassCommand
            => _lifeAndDeathDoneCommand ?? (_lifeAndDeathDoneCommand = new MvxCommand(Pass));

        public IMvxCommand ResignCommand
            => _resumeGameCommand ?? (_resumeGameCommand = new MvxCommand(Resign));
       
        public IMvxCommand UndoCommand
            => _requestUndoDeathMarksCommand ??
            (_requestUndoDeathMarksCommand = new MvxCommand(Undo));


        public IMvxCommand GetHintCommand
            => _requestUndoDeathMarksCommand ??
            (_requestUndoDeathMarksCommand = new MvxCommand(GetHint));


        public IMvxCommand SendGTPCommand
            => _requestUndoDeathMarksCommand ??
            (_requestUndoDeathMarksCommand = new MvxCommand(SendGTP));

        private void Fill()
        { }
        private void SaveSGF()
        { }

        private void LifeAndDeathDone()
        { }
        private void ResumeGame()
        { }
        private void RequestUndoDeathMarks()
        { }

        private void Pass()
        { }
        private void Resign()
        { }
        private void Undo()
        { }

        private void GetHint()
        { }

        private void SendGTP()
        { }


        /* TODO Migrate
         
        private void DebugFill(object sender, RoutedEventArgs e)
        {
            for (int x = 1; x < VM.BoardViewModel.BoardControlState.BoardWidth; x += 3)
            {
                for (int xi = x; xi <= x + 1; xi++)
                {
                    for (int y = 1; y < VM.BoardViewModel.BoardControlState.BoardHeight - 1; y += 1)
                    {
                        (VM.Game.Controller.TurnPlayer.Agent as IHumanAgentActions)?.PlaceStone(new Position(
                            xi, y));

                    }
                }
            }
        }

        private void UpdateSystemLog(object sender, RoutedEventArgs e)
        {
            SystemLog.Text = VM.SystemLog;
        }

        private async void SendFuegoCommand(object sender, RoutedEventArgs e)
        {
            foreach(var player in VM.Game.Controller.Players)
            {
                if (player.Agent is AiAgent)
                {
                    var fuego = (OmegaGo.Core.AI.FuegoSpace.Fuego) ((AiAgent) player.Agent).AI;
                    var response = fuego.SendCommand(this.FuegoCommand.Text);
                    await Mvx.Resolve<IDialogService>().ShowAsync(response.Text, "Fuego response");
                }
            }
        }
         
         
         */
    }
}
