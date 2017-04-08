﻿// 
// This file is auto-generated by LocalizerGenerator.
// 
// Do not edit this file. Edit LocalizedStrings.resx and use LocalizerGenerator instead.
//
using OmegaGo.UI.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable InconsistentNaming

namespace OmegaGo.UI.Services.Localization
{
    /// <summary>
    /// Default localizer for the LocalizedStrings resources
    /// </summary>
    public class Localizer : LocalizationService
    {

        /// <summary>
        /// Initializes localizer
        /// </summary>
        public Localizer() : base(LocalizedStrings.ResourceManager)
        {

        }

        public string GoBack => LocalizeCaller();

        public string NewGame => LocalizeCaller();

        public string MainMenu => LocalizeCaller();

        public string ThemesPanel => LocalizeCaller();

		/// <summary>
		/// Fluffy uses an n-ply alpha-beta tree to figure out the best move. This is often called the minimax algorithm. In the last layer, heuristics are used to score the board position.
		/// Fluffy will always and only pass in response to its opponent passing.
		/// You may set the depth the AI will search. Increasing the depth will increase the AI's thinking time.
		/// </summary>
		public string AI_Fluffy_Description => LocalizeCaller();
		/// <summary>
		/// Fluffy (minimax)
		/// </summary>
		public string AI_Fluffy_Name => LocalizeCaller();
		/// <summary>
		/// The strongest AI program in this game, the Defeatist will resign the first time it gets the chance to. You are simply not worth its time.
		/// After you make your first move, or even before that, the AI will conclude that it's much stronger than you and just resign in order to not play a game with a foregone conclusion.
		/// </summary>
		public string AI_DefeatistAI_Description => LocalizeCaller();
		/// <summary>
		/// Defeatist
		/// </summary>
		public string AI_DefeatistAI_Name => LocalizeCaller();
		/// <summary>
		/// Fuego is a well-known open-source Go-playing engine written at the University of Alberta in Canada.
		/// It uses Monte Carlo tree search to make moves. It's capable of placing stones, passing and resigning, as the situation calls for.
		/// We recommend you use this AI program for all of your games.
		/// </summary>
		public string AI_Fuego_Description => LocalizeCaller();
		/// <summary>
		/// Fuego
		/// </summary>
		public string AI_Fuego_Name => LocalizeCaller();
		/// <summary>
		/// This AI uses a simple heuristic to determine where to play the next move. The heuristic is based on influence and on all previous stone placements in the game history.
		/// This AI will pass only in response to its opponent passing, in which case it will always pass.
		/// </summary>
		public string AI_HeuristicPlayerWrapper_Description => LocalizeCaller();
		/// <summary>
		/// The Puppy (heuristic)
		/// </summary>
		public string AI_HeuristicPlayerWrapper_Name => LocalizeCaller();
		/// <summary>
		/// This AI will select a random position at which it can play and it will play there. It will pass as soon its opponent passes.
		/// </summary>
		public string AI_RandomAI_Description => LocalizeCaller();
		/// <summary>
		/// Random
		/// </summary>
		public string AI_RandomAI_Name => LocalizeCaller();
		/// <summary>
		/// This AI plays random moves while the board is not empty.
		/// I dare you to let two Fish play against each other. The game will never end (unless you're very lucky).
		/// </summary>
		public string AI_RandomPlayerWrapper_Description => LocalizeCaller();
		/// <summary>
		/// The Fish
		/// </summary>
		public string AI_RandomPlayerWrapper_Name => LocalizeCaller();
		/// <summary>
		/// A sound effect should play:
		/// </summary>
		public string ASoundEffectShouldPlay => LocalizeCaller();
		/// <summary>
		/// Assistant
		/// </summary>
		public string Assistant => LocalizeCaller();
		/// <summary>
		/// Assistant AI program
		/// </summary>
		public string AssistantAIProgram => LocalizeCaller();
		/// <summary>
		/// Artificial intelligence
		/// </summary>
		public string AssistPanel => LocalizeCaller();
		/// <summary>
		/// Audio
		/// </summary>
		public string AudioPanel => LocalizeCaller();
		/// <summary>
		/// Auto
		/// </summary>
		public string AutoLanguage => LocalizeCaller();
		/// <summary>
		/// Back
		/// </summary>
		public string Back => LocalizeCaller();
		/// <summary>
		/// Background Color
		/// </summary>
		public string BackgroundColor => LocalizeCaller();
		/// <summary>
		/// Basic orange
		/// </summary>
		public string BackgroundColor_Basic => LocalizeCaller();
		/// <summary>
		/// Green
		/// </summary>
		public string BackgroundColor_Green => LocalizeCaller();
		/// <summary>
		/// None
		/// </summary>
		public string BackgroundColor_None => LocalizeCaller();
		/// <summary>
		/// Background Image
		/// </summary>
		public string BackgroundImage => LocalizeCaller();
		/// <summary>
		/// Bamboo forest
		/// </summary>
		public string BackgroundImage_Forest => LocalizeCaller();
		/// <summary>
		/// Go board
		/// </summary>
		public string BackgroundImage_Go => LocalizeCaller();
		/// <summary>
		/// None
		/// </summary>
		public string BackgroundImage_None => LocalizeCaller();
		/// <summary>
		/// Shinto shrine
		/// </summary>
		public string BackgroundImage_Shrine => LocalizeCaller();
		/// <summary>
		/// Zen temple
		/// </summary>
		public string BackgroundImage_Temple => LocalizeCaller();
		/// <summary>
		/// Black
		/// </summary>
		public string Black => LocalizeCaller();
		/// <summary>
		/// Board border thickness
		/// </summary>
		public string BoardBorderThickness => LocalizeCaller();
		/// <summary>
		/// Board settings
		/// </summary>
		public string BoardSettings => LocalizeCaller();
		/// <summary>
		/// Board size
		/// </summary>
		public string BoardSize => LocalizeCaller();
		/// <summary>
		/// Board Theme
		/// </summary>
		public string BoardTheme => LocalizeCaller();
		/// <summary>
		/// Kaya wood
		/// </summary>
		public string BoardTheme_KayaWood => LocalizeCaller();
		/// <summary>
		/// Oak wood
		/// </summary>
		public string BoardTheme_OakWood => LocalizeCaller();
		/// <summary>
		/// Sabaki
		/// </summary>
		public string BoardTheme_SabakiBoard => LocalizeCaller();
		/// <summary>
		/// Solid color
		/// </summary>
		public string BoardTheme_SolidColor => LocalizeCaller();
		/// <summary>
		/// Virtual board
		/// </summary>
		public string BoardTheme_VirtualBoard => LocalizeCaller();
		/// <summary>
		/// Cell size
		/// </summary>
		public string CellSize => LocalizeCaller();
		/// <summary>
		/// Change language
		/// </summary>
		public string ChangeLanguage => LocalizeCaller();
		/// <summary>
		/// Credits
		/// </summary>
		public string Credits => LocalizeCaller();
		/// <summary>
		/// View information about the game authors and license information.
		/// </summary>
		public string CreditsToolTip => LocalizeCaller();
		/// <summary>
		/// Delete
		/// </summary>
		public string Delete => LocalizeCaller();
		/// <summary>
		/// Delete selection
		/// </summary>
		public string DeleteSelection => LocalizeCaller();
		/// <summary>
		/// Difficulty
		/// </summary>
		public string Difficulty => LocalizeCaller();
		/// <summary>
		/// Easy
		/// </summary>
		public string Easy => LocalizeCaller();
		/// <summary>
		/// Enable even in online games
		/// </summary>
		public string EnableEvenInOnlineGames => LocalizeCaller();
		/// <summary>
		/// It is frowned upon to use AI assistance during games against human opponents without their knowledge.
		/// </summary>
		public string EnableEvenInOnlineGamesTooltip => LocalizeCaller();
		/// <summary>
		/// Enable hints
		/// </summary>
		public string EnableHints => LocalizeCaller();
		/// <summary>
		/// Filter by source
		/// </summary>
		public string FilterBySource => LocalizeCaller();
		/// <summary>
		/// Game
		/// </summary>
		public string Game => LocalizeCaller();
		/// <summary>
		/// Game library
		/// </summary>
		public string GameLibrary => LocalizeCaller();
		/// <summary>
		/// Library of your past games and all your SGF files
		/// </summary>
		public string GameLibraryToolTip => LocalizeCaller();
		/// <summary>
		/// Gameplay
		/// </summary>
		public string Gameplay => LocalizeCaller();
		/// <summary>
		/// Hard
		/// </summary>
		public string Hard => LocalizeCaller();
		/// <summary>
		/// Help
		/// </summary>
		public string Help => LocalizeCaller();
		/// <summary>
		/// Read the rules, documentation and other interesting information.
		/// </summary>
		public string HelpToolTip => LocalizeCaller();
		/// <summary>
		/// Highlight illegal Ko moves
		/// </summary>
		public string HighlightIllegalKoMoves => LocalizeCaller();
		/// <summary>
		/// If you can't place a stone on a point because it would violate the Ko rule, the point will be highlighted.
		/// </summary>
		public string HighlightIllegalKoMovesTooltip => LocalizeCaller();
		/// <summary>
		/// Highlight last move
		/// </summary>
		public string HighlightLastMove => LocalizeCaller();
		/// <summary>
		/// Highlight recent captures
		/// </summary>
		public string HighlightRecentCaptures => LocalizeCaller();
		/// <summary>
		/// If a stone was captured during the previous move, its intersection will be highlighted.
		/// </summary>
		public string HighlightRecentCapturesTooltip => LocalizeCaller();
		/// <summary>
		/// Input
		/// </summary>
		public string InputPanel => LocalizeCaller();
		/// <summary>
		/// Language
		/// </summary>
		public string Language => LocalizeCaller();
		/// <summary>
		/// Please, restart the game to see the language you have chosen.
		/// </summary>
		public string LanguageChangeInfo => LocalizeCaller();
		/// <summary>
		/// Library
		/// </summary>
		public string Library => LocalizeCaller();
		/// <summary>
		/// Load
		/// </summary>
		public string Load => LocalizeCaller();
		/// <summary>
		/// Load folder
		/// </summary>
		public string LoadFolder => LocalizeCaller();
		/// <summary>
		/// Loading...
		/// </summary>
		public string Loading => LocalizeCaller();
		/// <summary>
		/// Local game
		/// </summary>
		public string LocalGame => LocalizeCaller();
		/// <summary>
		/// Play Go against another player on the same device or against an AI.
		/// </summary>
		public string LocalGameToolTip => LocalizeCaller();
		/// <summary>
		/// Making a move requires a confirmation click
		/// </summary>
		public string MakingAMoveRequiresAConfirmationClick => LocalizeCaller();
		/// <summary>
		/// Master volume
		/// </summary>
		public string MasterVolume => LocalizeCaller();
		/// <summary>
		/// Medium
		/// </summary>
		public string Medium => LocalizeCaller();
		/// <summary>
		/// Music volume
		/// </summary>
		public string MusicVolume => LocalizeCaller();
		/// <summary>
		/// Mute all
		/// </summary>
		public string MuteAll => LocalizeCaller();
		/// <summary>
		/// omegaGo
		/// </summary>
		public string OmegaGo => LocalizeCaller();
		/// <summary>
		/// On-line game
		/// </summary>
		public string OnlineGame => LocalizeCaller();
		/// <summary>
		/// Play Go against players on the IGS or OGS online servers.
		/// </summary>
		public string OnlineGameToolTip => LocalizeCaller();
		/// <summary>
		/// Open
		/// </summary>
		public string Open => LocalizeCaller();
		/// <summary>
		/// Play
		/// </summary>
		public string Play => LocalizeCaller();
		/// <summary>
		/// Player
		/// </summary>
		public string Player => LocalizeCaller();
		/// <summary>
		/// Rename
		/// </summary>
		public string Rename => LocalizeCaller();
		/// <summary>
		/// Rules
		/// </summary>
		public string Rules => LocalizeCaller();
		/// <summary>
		/// AGA
		/// </summary>
		public string RulesetType_AGA => LocalizeCaller();
		/// <summary>
		/// Chinese
		/// </summary>
		public string RulesetType_Chinese => LocalizeCaller();
		/// <summary>
		/// Japanese
		/// </summary>
		public string RulesetType_Japanese => LocalizeCaller();
		/// <summary>
		/// Select
		/// </summary>
		public string Select => LocalizeCaller();
		/// <summary>
		/// Settings
		/// </summary>
		public string Settings => LocalizeCaller();
		/// <summary>
		/// Change language, audio, visual style and many other things.
		/// </summary>
		public string SettingsToolTip => LocalizeCaller();
		/// <summary>
		/// Sound effects volume
		/// </summary>
		public string SfxVolume => LocalizeCaller();
		/// <summary>
		/// Show coordinates
		/// </summary>
		public string ShowCoordinates => LocalizeCaller();
		/// <summary>
		/// Show the tutorial button in main menu
		/// </summary>
		public string ShowTutorialButton => LocalizeCaller();
		/// <summary>
		/// Singleplayer
		/// </summary>
		public string Singleplayer => LocalizeCaller();
		/// <summary>
		/// Learn Go, play against an AI, solve problems or gain experience completing quests.
		/// </summary>
		public string SingleplayerToolTip => LocalizeCaller();
		/// <summary>
		/// Sounds
		/// </summary>
		public string Sounds => LocalizeCaller();
		/// <summary>
		/// Statistics
		/// </summary>
		public string Statistics => LocalizeCaller();
		/// <summary>
		/// View your winrate and other statistics.
		/// </summary>
		public string StatisticsToolTip => LocalizeCaller();
		/// <summary>
		/// Stones Theme
		/// </summary>
		public string StonesTheme => LocalizeCaller();
		/// <summary>
		/// Polished stones
		/// </summary>
		public string StoneTheme_PolishedBitmap => LocalizeCaller();
		/// <summary>
		/// Sabaki
		/// </summary>
		public string StoneTheme_Sabaki => LocalizeCaller();
		/// <summary>
		/// Solid color
		/// </summary>
		public string StoneTheme_SolidColor => LocalizeCaller();
		/// <summary>
		/// Strength
		/// </summary>
		public string Strength => LocalizeCaller();
		/// <summary>
		/// Themes
		/// </summary>
		public string ThemesPanel => LocalizeCaller();
		/// <summary>
		/// Tips
		/// </summary>
		public string Tips => LocalizeCaller();
		/// <summary>
		/// Show or hide tips.
		/// </summary>
		public string TipsToolTip => LocalizeCaller();
		/// <summary>
		/// Toggle fullscreen mode. (You may also use Alt+Enter on a PC.)
		/// </summary>
		public string ToggleFullscreenTooltip => LocalizeCaller();
		/// <summary>
		/// Total games played
		/// </summary>
		public string TotalGamesPlayed => LocalizeCaller();
		/// <summary>
		/// Tutorial
		/// </summary>
		public string Tutorial => LocalizeCaller();
		/// <summary>
		/// This is the best place to learn Go if you don't know anything at all about the game.
		/// </summary>
		public string TutorialToolTip => LocalizeCaller();
		/// <summary>
		/// Display
		/// </summary>
		public string UserInterface => LocalizeCaller();
		/// <summary>
		/// When another player places a stone
		/// </summary>
		public string WhenAnotherPlayerPlacesAStone => LocalizeCaller();
		/// <summary>
		/// When I place a stone
		/// </summary>
		public string WhenIPlaceAStone => LocalizeCaller();
		/// <summary>
		/// When I receive a notification
		/// </summary>
		public string WhenIReceiveANotification => LocalizeCaller();
		/// <summary>
		/// White
		/// </summary>
		public string White => LocalizeCaller();
		/// <summary>
		/// Handicap
		/// </summary>
		public string WhiteHandicap => LocalizeCaller();
		/// <summary>
		/// Fullscreen Mode
		/// </summary>
		public string FullscreenModeCheckbox => LocalizeCaller();
		/// <summary>
		/// Hotseat games played
		/// </summary>
		public string HotseatGamesPlayed => LocalizeCaller();
		/// <summary>
		/// omegaGo rank
		/// </summary>
		public string OmegaGoRank => LocalizeCaller();
		/// <summary>
		/// Online games played
		/// </summary>
		public string OnlineGamesPlayed => LocalizeCaller();
		/// <summary>
		/// Online games won
		/// </summary>
		public string OnlineGamesWon => LocalizeCaller();
		/// <summary>
		/// Points
		/// </summary>
		public string Points => LocalizeCaller();
		/// <summary>
		/// Quests completed
		/// </summary>
		public string QuestsCompleted => LocalizeCaller();
		/// <summary>
		/// Solo games played
		/// </summary>
		public string SoloGamesPlayed => LocalizeCaller();
		/// <summary>
		/// Solo games won
		/// </summary>
		public string SoloGamesWon => LocalizeCaller();
		/// <summary>
		/// Total games won
		/// </summary>
		public string TotalGamesWon => LocalizeCaller();
		/// <summary>
		/// Tsumego problems solved
		/// </summary>
		public string TsumegoProblemsSolved => LocalizeCaller();
		/// <summary>
		/// Omega Novice
		/// </summary>
		public string Rank1 => LocalizeCaller();
		/// <summary>
		/// Omega Apprentice
		/// </summary>
		public string Rank2 => LocalizeCaller();
		/// <summary>
		/// Omega Expert
		/// </summary>
		public string Rank3 => LocalizeCaller();
		/// <summary>
		/// Omega Master
		/// </summary>
		public string Rank4 => LocalizeCaller();
		/// <summary>
		/// Omega Graduate
		/// </summary>
		public string Rank5 => LocalizeCaller();
		/// <summary>
		/// You have advanced to a new omegaGo rank: {0}. Congratulations!
		/// </summary>
		public string YouHaveAdvancedToNewRankX => LocalizeCaller();
		/// <summary>
		/// +{0} points (now you have {1})
		/// </summary>
		public string YouHaveGainedXPointsNowYouHaveY => LocalizeCaller();
		/// <summary>
		/// Reset all progress?
		/// </summary>
		public string ResetAllProgress_Caption => LocalizeCaller();
		/// <summary>
		/// All game counters will be reduced to zero. All points will be lost. You will have the lowest rank. Information about what tsumego problems you solved will be lost. You will keep your current in-progress quests.
		/// </summary>
		public string ResetAllProgress_Content => LocalizeCaller();
		/// <summary>
		/// Cancel
		/// </summary>
		public string ResetAllProgress_No => LocalizeCaller();
		/// <summary>
		/// Reset all progress!
		/// </summary>
		public string ResetAllProgress_Yes => LocalizeCaller();
		/// <summary>
		/// Black to play.
		/// </summary>
		public string Tsumego_BlackToPlay => LocalizeCaller();
		/// <summary>
		/// Solved
		/// </summary>
		public string Tsumego_CorrectPanel => LocalizeCaller();
		/// <summary>
		/// Current node status:
		/// </summary>
		public string Tsumego_CurrentNodeStatus => LocalizeCaller();
		/// <summary>
		/// Current problem status:
		/// </summary>
		public string Tsumego_CurrentProblemStatus => LocalizeCaller();
		/// <summary>
		/// How to solve tsumego:
		/// </summary>
		public string Tsumego_HowToSolveTsumegoCaption => LocalizeCaller();
		/// <summary>
		/// Make the best moves possible against a virtual opponent who also makes the best moves. When you resolve the situation, you will be told it's solved. For more information, access help pages from the main menu.
		/// </summary>
		public string Tsumego_HowToSolveTsumegoText => LocalizeCaller();
		/// <summary>
		/// Instructions:
		/// </summary>
		public string Tsumego_Instructions => LocalizeCaller();
		/// <summary>
		/// (more moves available)
		/// </summary>
		public string Tsumego_MoreMovesAvailable => LocalizeCaller();
		/// <summary>
		/// Next problem
		/// </summary>
		public string Tsumego_NextProblem => LocalizeCaller();
		/// <summary>
		/// Not yet solved
		/// </summary>
		public string Tsumego_NotYetSolved => LocalizeCaller();
		/// <summary>
		/// Previous problem
		/// </summary>
		public string Tsumego_PreviousProblem => LocalizeCaller();
		/// <summary>
		/// Problem name:
		/// </summary>
		public string Tsumego_ProblemName => LocalizeCaller();
		/// <summary>
		/// Show possible moves
		/// </summary>
		public string Tsumego_ShowPossibleMoves => LocalizeCaller();
		/// <summary>
		/// If checked, the app will highlight positions where it might make sense to play. They might not be all correct plays, but all non-highlighted positions are certainly incorrect plays.
		/// </summary>
		public string Tsumego_ShowPossibleMoves_Tooltip => LocalizeCaller();
		/// <summary>
		/// Continue...
		/// </summary>
		public string Tsumego_StatusContinue => LocalizeCaller();
		/// <summary>
		/// Correct!
		/// </summary>
		public string Tsumego_StatusCorrect => LocalizeCaller();
		/// <summary>
		/// Wrong.
		/// </summary>
		public string Tsumego_StatusWrong => LocalizeCaller();
		/// <summary>
		/// Undo last move
		/// </summary>
		public string Tsumego_UndoOneMove => LocalizeCaller();
		/// <summary>
		/// (unexpected)
		/// </summary>
		public string Tsumego_Unexpected => LocalizeCaller();
		/// <summary>
		/// Tsumego
		/// </summary>
		public string Tsumego_ViewCaption => LocalizeCaller();
		/// <summary>
		/// White to play.
		/// </summary>
		public string Tsumego_WhiteToPlay => LocalizeCaller();
		/// <summary>
		/// Wrong
		/// </summary>
		public string Tsumego_WrongPanel => LocalizeCaller();
		/// <summary>
		/// Solved (you have previously solved this problem).
		/// </summary>
		public string Tsumego_YouHavePreviouslySolvedThisProblem => LocalizeCaller();
		/// <summary>
		/// You have solved this problem!
		/// </summary>
		public string Tsumego_YouHaveSolvedThisProblem => LocalizeCaller();
		/// <summary>
		/// IGS rank
		/// </summary>
		public string IgsRank => LocalizeCaller();
		/// <summary>
		/// KGS rank
		/// </summary>
		public string KgsRank => LocalizeCaller();
		/// <summary>
		/// Toggle muting all sounds (icon shows current state)
		/// </summary>
		public string MuteTooltip => LocalizeCaller();
		/// <summary>
		/// Next rank at:
		/// </summary>
		public string NextRankAtColon => LocalizeCaller();
		/// <summary>
		/// Play against AI
		/// </summary>
		public string PlayAgainstAI => LocalizeCaller();
		/// <summary>
		/// Quests
		/// </summary>
		public string Quests => LocalizeCaller();
		/// <summary>
		/// You receive a new quest every midnight, local time, unless you already have 3 quests.
		/// </summary>
		public string QuestsFooter => LocalizeCaller();
		/// <summary>
		/// Replace
		/// </summary>
		public string Quest_Replace => LocalizeCaller();
		/// <summary>
		/// You will lose this quest and any progress towards it, but you will immediately gain a different random quest.
		/// </summary>
		public string Quest_ReplaceTooltip => LocalizeCaller();
		/// <summary>
		/// Try this now
		/// </summary>
		public string Quest_TryThisNow => LocalizeCaller();
		/// <summary>
		/// This will take you to a screen where you can work on this quest.
		/// </summary>
		public string Quest_TryThisNowTooltip => LocalizeCaller();
		/// <summary>
		/// Cancel
		/// </summary>
		public string QuitCancel => LocalizeCaller();
		/// <summary>
		/// Quit game?
		/// </summary>
		public string QuitCaption => LocalizeCaller();
		/// <summary>
		/// Quit
		/// </summary>
		public string QuitConfirm => LocalizeCaller();
		/// <summary>
		/// Do you really want to quit the game?
		/// </summary>
		public string QuitText => LocalizeCaller();
		/// <summary>
		/// Singleplayer Menu
		/// </summary>
		public string SingleplayerMenu => LocalizeCaller();
		/// <summary>
		/// Solved!
		/// </summary>
		public string SolvedExclamationPoint => LocalizeCaller();
		/// <summary>
		/// Solve puzzles
		/// </summary>
		public string SolvePuzzles => LocalizeCaller();
		/// <summary>
		/// Solve tsumego problems
		/// </summary>
		public string TsumegoMenuCaption => LocalizeCaller();
		/// <summary>
		/// Next
		/// </summary>
		public string Tutorial_Next => LocalizeCaller();
		/// <summary>
		/// Reset all progress
		/// </summary>
		public string ResetAllProgress => LocalizeCaller();
		/// <summary>
		/// Operating System
		/// </summary>
		public string ControlStyle_OperatingSystem => LocalizeCaller();
		/// <summary>
		/// Wood
		/// </summary>
		public string ControlStyle_Wood => LocalizeCaller();
		/// <summary>
		/// Control Style
		/// </summary>
		public string ControlStyle => LocalizeCaller();
		/// <summary>
		/// Lite
		/// </summary>
		public string ControlStyle_Lite => LocalizeCaller();
		/// <summary>
		/// omegaGo rank:
		/// </summary>
		public string OmegaGoRankColon => LocalizeCaller();
		/// <summary>
		/// Points:
		/// </summary>
		public string PointsColon => LocalizeCaller();
		/// <summary>
		/// Create a new Pandanet account (opens a browser window)
		/// </summary>
		public string IgsHyperlink => LocalizeCaller();
		/// <summary>
		/// Pandanet - Internet Go Server
		/// </summary>
		public string IgsServerCaption => LocalizeCaller();
		/// <summary>
		/// Pandanet is recommended if you don't have a KGS account and don't want to download an additional application to play Go online.
		/// </summary>
		public string IgsServerInfo => LocalizeCaller();
		/// <summary>
		/// Pandanet username
		/// </summary>
		public string IgsUsernameCaption => LocalizeCaller();
		/// <summary>
		/// Download the official KGS client
		/// </summary>
		public string KgsHyperlink => LocalizeCaller();
		/// <summary>
		/// KGS Go Server
		/// </summary>
		public string KgsServerCaption => LocalizeCaller();
		/// <summary>
		/// You first need to create an account using the official KGS client. Download it on a desktop computer, login as a guest and then click User->Register.
		/// </summary>
		public string KgsServerInfo => LocalizeCaller();
		/// <summary>
		/// KGS Username
		/// </summary>
		public string KgsUsernameCaption => LocalizeCaller();
		/// <summary>
		/// Log in whenever you start omegaGo
		/// </summary>
		public string LoginAtStartupCaption => LocalizeCaller();
		/// <summary>
		/// Log In
		/// </summary>
		public string LoginButtonCaption => LocalizeCaller();
		/// <summary>
		/// Login
		/// </summary>
		public string LoginFormCaption => LocalizeCaller();
		/// <summary>
		/// Password
		/// </summary>
		public string PasswordCaption => LocalizeCaller();
		/// <summary>
		/// Remember password
		/// </summary>
		public string RememberPasswordCaption => LocalizeCaller();
		/// <summary>
		/// Background image opacity
		/// </summary>
		public string BackgroundImageOpacity => LocalizeCaller();
		/// <summary>
		/// Theme
		/// </summary>
		public string AppTheme => LocalizeCaller();
		/// <summary>
		/// Dark
		/// </summary>
		public string AppTheme_Dark => LocalizeCaller();
		/// <summary>
		/// Light
		/// </summary>
		public string AppTheme_Light => LocalizeCaller();
		/// <summary>
		/// To apply control style change, please restart the application.
		/// </summary>
		public string ControlStyleChangeInfo => LocalizeCaller();
		/// <summary>
		/// Can resign
		/// </summary>
		public string CanResign => LocalizeCaller();
		/// <summary>
		/// If checked, Fuego will resign when it has little chance to win. Fuego will never resign in handicap games.
		/// </summary>
		public string CanResignTooltip => LocalizeCaller();
		/// <summary>
		/// Configure...
		/// </summary>
		public string ConfigureEllipsis => LocalizeCaller();
		/// <summary>
		/// Maximum board size:
		/// </summary>
		public string MaximumBoardSize => LocalizeCaller();
		/// <summary>
		/// Max playouts:
		/// </summary>
		public string MaxPlayoutsColon => LocalizeCaller();
		/// <summary>
		/// More playouts means more intelligent moves, but also more time taken to make a move.
		/// </summary>
		public string MaxPlayoutsTooltip => LocalizeCaller();
		/// <summary>
		/// Minimum board size:
		/// </summary>
		public string MinimumBoardSize => LocalizeCaller();
		/// <summary>
		/// More...
		/// </summary>
		public string MoreEllipsis => LocalizeCaller();
		/// <summary>
		/// No
		/// </summary>
		public string No => LocalizeCaller();
		/// <summary>
		/// Non-square boards permitted:
		/// </summary>
		public string NonSquareBoardsPermitted => LocalizeCaller();
		/// <summary>
		/// Think during opponent's turn
		/// </summary>
		public string Ponder => LocalizeCaller();
		/// <summary>
		/// If checked, Fuego will use continue thinking even when it's not its turn. This will increase Fuego's strength in timed games.
		/// </summary>
		public string PonderTooltip => LocalizeCaller();
		/// <summary>
		/// Tree depth:
		/// </summary>
		public string TreeDepthColon => LocalizeCaller();
		/// <summary>
		/// A greater tree depth means more intelligent moves, but also more time taken to make a move.
		/// </summary>
		public string TreeDepthTooltip => LocalizeCaller();
		/// <summary>
		/// Yes
		/// </summary>
		public string Yes => LocalizeCaller();
		/// <summary>
		/// Finish any game on a 25x25 or larger board.
		/// </summary>
		public string EpicPlayQuest_Description => LocalizeCaller();
		/// <summary>
		/// Epic
		/// </summary>
		public string EpicPlayQuest_Name => LocalizeCaller();
		/// <summary>
		/// Win a solo game against Fuego.
		/// </summary>
		public string GettingStrongerQuest_Description => LocalizeCaller();
		/// <summary>
		/// Getting Stronger
		/// </summary>
		public string GettingStrongerQuest_Name => LocalizeCaller();
		/// <summary>
		/// Solve 20 tsumego problems.
		/// </summary>
		public string GreatLearnerQuest_Description => LocalizeCaller();
		/// <summary>
		/// Great Learner
		/// </summary>
		public string GreatLearnerQuest_Name => LocalizeCaller();
		/// <summary>
		/// Your quest transformed! Solve any one tsumego problem.
		/// </summary>
		public string HiddenQuest_Description => LocalizeCaller();
		/// <summary>
		/// Hidden Quest!
		/// </summary>
		public string HiddenQuest_Name => LocalizeCaller();
		/// <summary>
		/// Finish 3 games on the Pandanet server.
		/// </summary>
		public string IgsChallengeQuest_Description => LocalizeCaller();
		/// <summary>
		/// Pandanet Challenge
		/// </summary>
		public string IgsChallengeQuest_Name => LocalizeCaller();
		/// <summary>
		/// Finish 3 games on the KGS server.
		/// </summary>
		public string KgsChallengeQuest_Description => LocalizeCaller();
		/// <summary>
		/// KGS Challenge
		/// </summary>
		public string KgsChallengeQuest_Name => LocalizeCaller();
		/// <summary>
		/// Solve 5 tsumego problems.
		/// </summary>
		public string LearnerQuest_Description => LocalizeCaller();
		/// <summary>
		/// Learner
		/// </summary>
		public string LearnerQuest_Name => LocalizeCaller();
		/// <summary>
		/// Finish an online game on a 19x19 board.
		/// </summary>
		public string OnlineTraditionalQuest_Description => LocalizeCaller();
		/// <summary>
		/// Net Traditional
		/// </summary>
		public string OnlineTraditionalQuest_Name => LocalizeCaller();
		/// <summary>
		/// "Win a solo game against Fuego without handicap.
		/// </summary>
		public string PureSkillQuest_Description => LocalizeCaller();
		/// <summary>
		/// Pure Skill
		/// </summary>
		public string PureSkillQuest_Name => LocalizeCaller();
		/// <summary>
		/// Win 3 solo games against an AI program.
		/// </summary>
		public string SoloVictoriesQuest_Description => LocalizeCaller();
		/// <summary>
		/// Human > Computer
		/// </summary>
		public string SoloVictoriesQuest_Name => LocalizeCaller();
		/// <summary>
		/// Win a solo game against Fuego where Fuego is playing black and has a handicap of 3 stones or more.
		/// </summary>
		public string TotalMasteryQuest_Description => LocalizeCaller();
		/// <summary>
		/// Total Mastery
		/// </summary>
		public string TotalMasteryQuest_Name => LocalizeCaller();
		/// <summary>
		/// Finish any game on a 19x19 board.
		/// </summary>
		public string TraditionalQuest_Description => LocalizeCaller();
		/// <summary>
		/// Traditional
		/// </summary>
		public string TraditionalQuest_Name => LocalizeCaller();
		/// <summary>
		/// Finish two solo or online games where one player has a handicap of two stones or more.
		/// </summary>
		public string UnevenStrengthQuest_Description => LocalizeCaller();
		/// <summary>
		/// Uneven Strength
		/// </summary>
		public string UnevenStrengthQuest_Name => LocalizeCaller();
		/// <summary>
		/// OK
		/// </summary>
		public string CloseFlyout => LocalizeCaller();
		/// <summary>
		/// Challenge selected player...
		/// </summary>
		public string Igs_ChallengeSelectedPlayer => LocalizeCaller();
		/// <summary>
		/// Issue a challenge
		/// </summary>
		public string Igs_IssueAChallenge => LocalizeCaller();
		/// <summary>
		/// I am looking for a game
		/// </summary>
		public string Igs_LFG => LocalizeCaller();
		/// <summary>
		/// Only show players looking for a game
		/// </summary>
		public string Igs_OnlyShowLfgers => LocalizeCaller();
		/// <summary>
		/// Refresh list of games
		/// </summary>
		public string Igs_RefreshGames => LocalizeCaller();
		/// <summary>
		/// Refresh list of users
		/// </summary>
		public string Igs_RefreshUsers => LocalizeCaller();
		/// <summary>
		/// Refuse all incoming match requests
		/// </summary>
		public string Igs_RefuseAll => LocalizeCaller();
		/// <summary>
		/// Spectate a game
		/// </summary>
		public string Igs_SpectateAGame => LocalizeCaller();
		/// <summary>
		/// Logout
		/// </summary>
		public string Logout => LocalizeCaller();
		/// <summary>
		/// Observe this game
		/// </summary>
		public string ObserveThisGame => LocalizeCaller();
		/// <summary>
		/// Sort by:
		/// </summary>
		public string SortBy => LocalizeCaller();
		/// <summary>
		/// Black's name
		/// </summary>
		public string SortBy_Black => LocalizeCaller();
		/// <summary>
		/// Name
		/// </summary>
		public string SortBy_Name => LocalizeCaller();
		/// <summary>
		/// Number of observers (highest first)
		/// </summary>
		public string SortBy_NumberOfObservers => LocalizeCaller();
		/// <summary>
		/// Rank (lowest first)
		/// </summary>
		public string SortBy_RankAscending => LocalizeCaller();
		/// <summary>
		/// Rank (highest first)
		/// </summary>
		public string SortBy_RankDescending => LocalizeCaller();
		/// <summary>
		/// White's name
		/// </summary>
		public string SortBy_White => LocalizeCaller();
		/// <summary>
		/// You are logged in as:
		/// </summary>
		public string YouAreLoggedInAs => LocalizeCaller();

        // 
        // This file is auto-generated by LocalizerGenerator.
        // 
        // Do not edit this file. Edit LocalizedStrings.resx and use LocalizerGenerator instead.
        //
    }
}

