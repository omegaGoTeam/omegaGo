﻿using MvvmCross.Core.ViewModels;
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

        public bool ExchangeIsPossible => QuestCooldownActions.IsExchangePossible(_gameSettings);

        public ObservableCollection<ActiveQuest> ActiveQuests { get; set; }
            = new ObservableCollection<ActiveQuest>();

      
        public void Load()
        {
            ActiveQuests.Clear();
            QuestCooldownActions.CheckForNewQuests(_gameSettings);
            foreach(var quest in _gameSettings.Quests.ActiveQuests)
            {
                ActiveQuests.Add(quest);
            }
            
        }

        public void ExchangeQuest(ActiveQuest activeQuest)
        {
            if (ExchangeIsPossible)
            {
                var newQuest = Quest.SpawnRandomQuest(_gameSettings.Quests.ActiveQuests.Select(q => q.QuestID));
                _gameSettings.Quests.LoseQuest(activeQuest);
                ActiveQuests.Remove(activeQuest);
                _gameSettings.Quests.AddQuest(newQuest);
                ActiveQuests.Add(newQuest);
                _gameSettings.Quests.LastQuestExchangedWhen = DateTime.Now;
            }
            RaisePropertyChanged(nameof(ExchangeIsPossible));
        }

        public void TryThisNow(ActiveQuest activeQuest)
        {
            Type model = activeQuest.Quest.GetViewModelToTry();
            if (model != null)
            {
                ShowViewModel(model);
            }
        }
    }
}
