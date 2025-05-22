using System;
using DiceGame.GameModules;
using Spectre.Console;

namespace DiceGame
{
    /// <summary>
    /// Entry point for the Non-Transitive Dice Game.
    /// Parses command-line arguments and starts the game engine.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                // Separate the dice (6 sided each)
                var parser = new DiceParser();
                var dices = parser.Parse(args);

                // start engine
                var engine = new GameEngine(dices);
                engine.Start();
            }
            catch (ArgumentException ex)
            {
                // Shows erroneous entries
                AnsiConsole.Markup("[red]Input Error:[/]\n");
                AnsiConsole.Markup(ex.Message + "\n");
                AnsiConsole.Markup("Example: \"2,2,4,4,9,9\" \"6,8,1,1,8,6\" \"7,5,3,7,5,3\"\n");
            }
            catch (Exception ex)
            {
                // For other unexpected errors
                AnsiConsole.Markup("[red]Unexpected error occurred: [/]");
                AnsiConsole.Markup(ex.Message + "\n");
                AnsiConsole.Markup("Please check your input\n");
            }
        }
    }
}