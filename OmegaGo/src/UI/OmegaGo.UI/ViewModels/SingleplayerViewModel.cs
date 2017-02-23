using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.Core;
using OmegaGo.Core.Rules;
using OmegaGo.UI.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.Services.GameCreationBundle;
using OmegaGo.UI.Services.Quests;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.Services.Tsumego;

namespace OmegaGo.UI.ViewModels
{
    public class SingleplayerViewModel : ViewModelBase
    {
        private readonly IGameSettings _gameSettings;

        public SingleplayerViewModel( IGameSettings gameSettings )
        {
            _gameSettings = gameSettings;
        }

        public IMvxCommand GoToTutorial => new MvxCommand(() => ShowViewModel<TutorialViewModel>());
        public MvxCommand GoToStatistics => new MvxCommand(() => ShowViewModel<StatisticsViewModel>());
        public MvxCommand GoToTsumegoMenu => new MvxCommand(() => ShowViewModel<TsumegoMenuViewModel>());

        public MvxCommand GoToLocalGame => new MvxCommand(() =>
        {
            Mvx.RegisterSingleton<GameCreationBundle>(new SoloBundle());
            ShowViewModel<GameCreationViewModel>();
        }
            );

        public int Points => this._gameSettings.Quests.Points;

        public ObservableCollection<ActiveQuest> ActiveQuests { get; set; }
            = new ObservableCollection<ActiveQuest>();

      
        public void Load()
        {
            foreach(var quest in _gameSettings.Quests.ActiveQuests)
            {
                ActiveQuests.Add(quest);
            }
            while (_gameSettings.Quests.LastQuestReceivedWhen.AddDays(1) < DateTime.Now &&
                _gameSettings.Quests.ActiveQuests.Count() < Quest.MAXIMUM_NUMBER_OF_QUESTS)
                {
                    AddAQuest();
                }
            
        }

        private void AddAQuest()
        {
            _gameSettings.Quests.LastQuestReceivedWhen = DateTime.Now; // TODO Petr : make it so that quests can stack in history, but not limitlessly

            var nq = Quest.SpawnRandomQuest();
            _gameSettings.Quests.AddQuest(nq);
            ActiveQuests.Add(nq);
        }

        public void ExchangeQuest(ActiveQuest activeQuest)
        {
            if (_gameSettings.Quests.LastQuestExchangedWhen.AddDays(1) < DateTime.Now)
            {
                _gameSettings.Quests.LoseQuest(activeQuest);
                ActiveQuests.Remove(activeQuest);
                _gameSettings.Quests.AddQuest(Quest.SpawnRandomQuest());
                ActiveQuests.Add(activeQuest);
                _gameSettings.Quests.LastQuestExchangedWhen = DateTime.Now;
            }
            else
            {
                throw new Exception("The button should not have been displayed.");
            }
        }

        public void TryThisNow(ActiveQuest activeQuest)
        {
            if (activeQuest.Quest.GetViewModelToTry() != null)
            {
                ShowViewModel(activeQuest.Quest.GetViewModelToTry());
            }
        }
    }
}
