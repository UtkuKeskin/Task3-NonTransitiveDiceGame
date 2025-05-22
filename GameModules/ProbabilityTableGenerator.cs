using System;
using System.Collections.Generic;
using Spectre.Console;

namespace DiceGame.GameModules
{
    /// <summary>
    /// Generates an ASCII table displaying win probabilities between dice pairs using Spectre.Console.
    /// </summary>
    public class ProbabilityTableGenerator
    {
        /// <summary>
        /// Generates an ASCII table displaying win probabilities between dice pairs.
        /// </summary>
        /// <param name="dices">List of Dice objects to compare.</param>
        /// <param name="probabilities">2D array of win probabilities.</param>
        /// <exception cref="ArgumentNullException">Thrown when dices or probabilities is null.</exception>
        public void GenerateProbabilityTable(List<Dice> dices, double[,] probabilities)
        {
            if (dices == null) throw new ArgumentNullException(nameof(dices));
            if (probabilities == null) throw new ArgumentNullException(nameof(probabilities));

            int n = dices.Count;
            AnsiConsole.MarkupLine("[yellow]Probability of the win for the user:[/]");

            var table = new Table();
            table.Title = new TableTitle("Win Probabilities");
            table.AddColumn(new TableColumn("[green]User dice v[/]"));

            // Column headings Dice #0 → 2,2,2,4,4,4,9,9 format
            for (int i = 0; i < n; i++)
            {
                var label = $"Dice #{i} → {string.Join(",", dices[i].Faces)}";
                table.AddColumn(new TableColumn($"[green]{label}[/]")); // Green title
            }

            // Rows Probabilities for each die
            for (int i = 0; i < n; i++)
            {
                var row = new List<string> { $"Dice #{i} → {string.Join(",", dices[i].Faces)}" };
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                        row.Add("-(0.3333)"); // Diagonal cells different format
                    else
                        row.Add(probabilities[i, j].ToString("F4"));
                }
                table.AddRow(row.ToArray());
            }

            table.Border(TableBorder.Rounded);
            AnsiConsole.Write(table);
        }
    }
}