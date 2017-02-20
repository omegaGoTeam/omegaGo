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
        private IGameSettings _settings = Mvx.Resolve<IGameSettings>();
        public IMvxCommand GoToTutorial => new MvxCommand(() => ShowViewModel<TutorialViewModel>());
        public MvxCommand GoToStatistics => new MvxCommand(() => ShowViewModel<StatisticsViewModel>());
        public MvxCommand GoToTsumegoMenu => new MvxCommand(() => ShowViewModel<TsumegoMenuViewModel>());

        public MvxCommand GoToLocalGame => new MvxCommand(() =>
        {
            Mvx.RegisterSingleton<GameCreationBundle>(new SoloBundle());
            ShowViewModel<GameCreationViewModel>();
        }
            );

        public int Points => this._settings.Quests.Points;

        public bool ExchangeIsPossible => QuestCooldownActions.IsExchangePossible(_settings);

        public ObservableCollection<ActiveQuest> ActiveQuests { get; set; }
            = new ObservableCollection<ActiveQuest>();

      
        public void Load()
        {
            ActiveQuests.Clear();
            QuestCooldownActions.CheckForNewQuests(_settings);
            foreach(var quest in _settings.Quests.ActiveQuests)
            {
                ActiveQuests.Add(quest);
            }
            
        }

        public void ExchangeQuest(ActiveQuest activeQuest)
        {
            if (ExchangeIsPossible)
            {
                var newQuest = Quest.SpawnRandomQuest(_settings.Quests.ActiveQuests.Select(q => q.QuestID));
                _settings.Quests.LoseQuest(activeQuest);
                ActiveQuests.Remove(activeQuest);
                _settings.Quests.AddQuest(newQuest);
                ActiveQuests.Add(newQuest);
                _settings.Quests.LastQuestExchangedWhen = DateTime.Now;
            }
            RaisePropertyChanged(nameof(ExchangeIsPossible));
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
