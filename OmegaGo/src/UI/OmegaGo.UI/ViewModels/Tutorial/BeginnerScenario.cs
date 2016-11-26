using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core;

namespace OmegaGo.UI.ViewModels.Tutorial
{
    public class BeginnerScenario : Scenario
    {
        public BeginnerScenario()
        {
            var endline = new ActionButtonDialogueLine("This is the end for now.", "Return to Main Menu",
                (sc) => sc.OnScenarioCompleted());
            var part1 =
                new ComplexLine(
                    "You will use black stones. Black goes first. Begin by placing a stone on the highlighted intersection, please.",
                    (scenario)=>scenario.Highlight("E5"),
                    (scenario, position) => position == Position.FromIgsCoordinates("E5"),
                    new ComplexLine(
                        "Now it’s my turn. I play my stone adjacent to yours. Now it’s your turn again. In Go, stones cannot be moved - you may only ever play new ones. Now put a stone at the highlighted intersection again, please.",
                        (scenario)=>
                        {
                            scenario.ClearHighlights();
                            scenario.PlaceStone(StoneColor.Black, "E5");
                            scenario.PlaceStone(StoneColor.White, "E6");
                            scenario.Highlight("F6");
                        },
                        (scenario, position)=>position == Position.FromIgsCoordinates("F6"),
                        endline
                        ));
                    
            FirstLine =
                new ChoiceDialogueLine(
                    "Welcome to my class, apprentice. Are you ready to learn to play the ancient game of Go?",
                    "It will be my honor to learn from you, honorable master.",
                    new SimpleDialogueLine(
                        "And my privilege to teach you. [i]Go[/i] is a game about surrounding territory. Your goal is to have the largest territory surrounded by stones of your color.",
                        part1),
                    "Sure. Let’s get on with it, old man.",
                    new SimpleDialogueLine(
                        "Blitz games are not for beginners and you should show more respect. But we shall do as you say. Let’s begin.",
                        part1));


            
            /*
            FirstLine = 
                new SimpleDialogueLine("Hello.",
                new SimpleDialogueLine("Welcome to",
                new SimpleDialogueLine("GO!!",
                new ActionButtonDialogueLine("This is the end.",
                "Return to Main Menu", (scenario) => {
                                                         scenario.OnScenarioCompleted();
                }))));
                */
        }
    }
}
