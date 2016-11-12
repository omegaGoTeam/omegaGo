using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Forms;
using OmegaGo.Core;
using OmegaGo.Core.Agents;
using OmegaGo.Core.AI;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Rules;
using OmegaGo.Core.AI.Common;
using GoColor = OmegaGo.Core.StoneColor;

namespace QuickPrototype
{
    public partial class InGameForm : Form
    {
        private async void LoopDecisionRequest()
        {
            _game.NumberOfMovesPlayed = 0;
            _playerToMove = _game.Players[0];
            while (true)
            {
                lblTurnPlayer.Text = _playerToMove.Name;
                SystemLog("Asking " + _playerToMove + " to make a move...");
                if (_playerToMove.Agent is AIAgent)
                {
                    ((AIAgent)_playerToMove.Agent).Strength = (int)this.nAiStrength.Value;
                }
                AgentDecision decision = await _playerToMove.Agent.RequestMove(_game);
                SystemLog(_playerToMove + " does: " + decision);

                if (decision.Kind == AgentDecisionKind.Resign)
                {
                    panelEnd.Visible = true;
                    lblEndCaption.Text = _playerToMove + " resigned!";
                    lblGameEndReason.Text = "The player resignation reason: '" + decision.Explanation + "'";
                    SystemLog("Game is over by resignation.");
                    break;
                }
                if (decision.Kind == AgentDecisionKind.Move)
                {
                    Move moveToMake = decision.Move;
                    bool willWeAcceptTheMove = true;
                    if (moveToMake.Kind == MoveKind.PlaceStone)
                    {
                        if (moveToMake.Coordinates.X < 0 || moveToMake.Coordinates.Y < 0 ||
                            moveToMake.Coordinates.X >= _game.BoardSize.Width || moveToMake.Coordinates.Y >= _game.BoardSize.Height)
                        {
                            SystemLog("Illegal Move - Outside the board");
                            willWeAcceptTheMove = false;
                        }
                    }
                    if (willWeAcceptTheMove)
                    {
                        // So far, we're not providing Ko information
                        MoveResult canWeMakeIt =
                            _game.Ruleset?.ControlMove(FastBoard.CloneBoard(_truePositions), moveToMake, new List<GoColor[,]>()) ?? MoveResult.Legal;
                        // If there is no ruleset, moves are automatically legal.
                        if (canWeMakeIt != MoveResult.Legal && canWeMakeIt != MoveResult.LifeDeadConfirmationPhase)
                        {
                            willWeAcceptTheMove = false;
                            switch (canWeMakeIt)
                            {
                                case MoveResult.Ko:
                                    SystemLog("Illegal Move - Ko");
                                    break;
                                case MoveResult.OccupiedPosition:
                                    SystemLog("That intersection is already occupied!");
                                    break;
                                case MoveResult.SelfCapture:
                                    SystemLog("Illegal Move - Suicide");
                                    break;
                                case MoveResult.SuperKo:
                                    SystemLog("Illegal Move - Superko");
                                    break;
                            }
                        }
                    }
                    if (!willWeAcceptTheMove && _playerToMove.Agent.HowToHandleIllegalMove == IllegalMoveHandling.PermitItAnyway)
                    {
                        SystemLog("The agent asked us to make an ILLEGAL MOVE and we are DOING IT ANYWAY!");
                        willWeAcceptTheMove = true;
                    }
                    if (!willWeAcceptTheMove)
                    {
                        if (this._igs == null)
                        {
                            if (this.chEnforceRules.Checked || MessageBox.Show("The player " + _playerToMove + " made a move (" + moveToMake + ") that the ruleset thinks is illegal. Should the move be PERMITTED?", "Allow illegal move?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                            {
                                // Move is forbidden.
                                if (_playerToMove.Agent.HowToHandleIllegalMove == IllegalMoveHandling.Retry)
                                {
                                    SystemLog("Illegal move - retrying.");
                                    continue; // retry
                                }
                                else if (_playerToMove.Agent.HowToHandleIllegalMove == IllegalMoveHandling.MakeRandomMove)
                                {

                                    SystemLog("Illegal move - making a random move instead.");
                                    GoColor actorColor = (_playerToMove == _game.Players[0]) ? GoColor.Black : GoColor.White;
                                    List<Position> possibleMoves = _game.Ruleset?.GetAllLegalMoves(actorColor,
                                        FastBoard.CloneBoard(_truePositions), new List<GoColor[,]>()) ??
                                                                   new List<Position>();

                                    // TODO add history
                                    if (possibleMoves.Count == 0)
                                    {
                                        SystemLog("NO MORE MOVES!");
                                        break;
                                    }
                                    else
                                    {
                                        Position randomTargetposition = possibleMoves[Randomness.Next(possibleMoves.Count)];
                                        decision = AgentDecision.MakeMove(OmegaGo.Core.Move.Create(actorColor, randomTargetposition),
                                            "A random move was made because the AI supplied an illegal move.");
                                    }
                                }
                                else
                                {
                                    throw new Exception("This agent does not provide information on how to handle its illegal move.");
                                }
                            }
                            else
                            {
                                // Server overrides rules.
                            }
                        }
                        else
                        {
                            // Ok.
                        }
                    }
                    if (decision.Move.Kind == MoveKind.PlaceStone)
                    {
                        SystemLog("Adding " + decision.Move + " to primary timeline.");
                        _game.GameTree.AddMoveToEnd(decision.Move);
                        if (_igs != null && !(_playerToMove.Agent is OnlineAgent))
                        {
                            _igs.MakeMove(_game, decision.Move);
                        }
                    }
                    else if (decision.Move.Kind == MoveKind.Pass)
                    {
                        SystemLog(_playerToMove + " passed!");
                    }
                    else
                    {
                        throw new InvalidOperationException("An agent should not use any other move kinds except for placing stones and passing.");
                    }
                    // THE MOVE STANDS
                    _game.NumberOfMovesPlayed++;
                    RefreshBoard();
                    _playerToMove = _game.OpponentOf(_playerToMove);
                }

            }
        }
    }
}
