using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core;

namespace OmegaGo.UI.ViewModels.Tutorial
{
    class ScenarioLoader
    {
        public static List<ScenarioCommand> LoadFromText(string data, Scenario scenario)
        {
            List<ScenarioCommand> commands = new List<ScenarioCommand>();
            // TODO HACK the menu parsing system is a little.... fragiles
            string[] lines = data.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                ParsedLine parsedLine = ParsedLine.Parse(line);
                if (parsedLine.Command == "menu")
                {
                    ParsedLine option1 = ParsedLine.Parse(lines[i + 1]);
                    ParsedLine option1then = ParsedLine.Parse(lines[i + 2]);
                    ParsedLine option2 = ParsedLine.Parse(lines[i + 3]);
                    ParsedLine option2then = ParsedLine.Parse(lines[i + 4]);
                    i += 4;
                    commands.Add(new MenuCommand(option1, option1then, option2, option2then));
                }
                else
                {
                    commands.AddRange(ParseCommand(parsedLine));
                }

            }
            commands.Add(new EndScenarioCommand());
            return commands;
        }

        private static IEnumerable<ScenarioCommand> ParseCommand(ParsedLine parsedLine)
        {
            /*
             * Possible commands:
             * # comment
             * s [sensei's line]
             * menu (handled elsewhere)
             * next
             * do [position] ==> shine [position] & expect [position]
             * white [position]...
             * black [position]...
             * clear [position]...
             * flash
             * expect [position] ==> require [position] & black [position]
             * shine [position]
             * expect_failure
             * suicidal_move_message
             * button [button_text]
             */ 
            switch (parsedLine.Command)
            {
                case "s":
                    yield return new SayCommand(parsedLine.FullArgument);
                    break;
                case "next":
                    yield return new NextCommand();
                    break;
                case "do":
                    yield return new ShineCommand(parsedLine.FullArgument);
                    yield return new RequireCommand(parsedLine.FullArgument);
                    yield return new PlaceCommand(StoneColor.Black, parsedLine.FullArgument);
                    break;
                case "shine":
                    yield return new ShineCommand(parsedLine.FullArgument);
                    break;
                case "expect_failure":
                    yield return new RequireCommand(parsedLine.FullArgument);
                    break;
                case "expect":
                    yield return new RequireCommand(parsedLine.FullArgument);
                    yield return new PlaceCommand(StoneColor.Black, parsedLine.FullArgument);
                    break;
                case "button":
                    yield return new ButtonNextTextCommand(parsedLine.FullArgument);
                    break;
                case "suicidal_move_message":
                    yield return new DummyCommand();
                    break;
                case "black":
                    yield return new PlaceCommand(StoneColor.Black, parsedLine.Arguments);
                    break;
                case "white":
                    yield return new PlaceCommand(StoneColor.White, parsedLine.Arguments);
                    break;
                case "clear":
                    yield return new PlaceCommand(StoneColor.None, parsedLine.Arguments);
                    break;
                case "flash":
                    yield return new FlashCommand();
                    break;
                case "#":
                    // Comment
                    break;
                default:
                    throw new Exception("This command is not implemented: " + parsedLine.Command);
            }
        }

        public class ParsedLine
        {
            public string Command;
            public string FullArgument;
            public string[] Arguments;

            public static ParsedLine Parse(string line)
            {
                string trimmed = line.Trim();
                string[] parts = trimmed.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string fullArgument = trimmed.Contains(" ") ? trimmed.Substring(trimmed.IndexOf(' ') + 1) : "";
                string command = parts[0];
                string[] arguments = parts.Skip(1).ToArray();
                return new ParsedLine(command, fullArgument, arguments);
            }

            private ParsedLine(string command, string fullArgument, string[] arguments)
            {
                this.Command = command;
                this.FullArgument = fullArgument;
                this.Arguments = arguments;
            }
        }
    }

    internal class FlashCommand : ScenarioCommand
    {
        public override LoopControl Execute(Scenario scenario)
        {
            scenario.ClearBoard();
            return LoopControl.Continue;
        }
    }
}
