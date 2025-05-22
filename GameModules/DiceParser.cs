using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceGame.GameModules
{
    /// <summary>
    /// Parses command-line arguments into a list of Dice objects.
    /// Provides validation and error handling for dice configurations.
    /// </summary>
    public class DiceParser
    {
        private readonly int _requiredFaceCount;
        private const int MinDiceCount = 3;
        private const int DefaultFaceCount = 6;

        /// <summary>
        /// Initializes a new instance of the DiceParser with a specified required face count.
        /// </summary>
        /// <param name="requiredFaceCount">The number of faces each dice must have (default is 6).</param>
        public DiceParser(int requiredFaceCount = DefaultFaceCount)
        {
            if (requiredFaceCount <= 0)
                throw new ArgumentException("Required face count must be a positive integer.");

            _requiredFaceCount = requiredFaceCount;
        }

        /// <summary>
        /// Parses command-line arguments into a list of Dice objects.
        /// Each argument should contain comma-separated positive integers.
        /// </summary>
        /// <param name="commandLineArgs">Array of strings representing dice faces.</param>
        /// <returns>List of validated Dice instances.</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
        public List<Dice> Parse(string[] commandLineArgs)
        {
            if (commandLineArgs == null || commandLineArgs.Length < MinDiceCount)
            {
                throw new ArgumentException(
                    $"At least {MinDiceCount} dice are required. Example: \"2,2,4,4,9,9\" \"6,8,1,1,8,6\" \"7,5,3,7,5,3\". " +
                    "Use quotes around each dice argument to avoid parsing issues.");
            }

            var diceList = new List<Dice>();
            var allErrors = new List<string>();

            foreach (var arg in commandLineArgs)
            {
                if (string.IsNullOrWhiteSpace(arg))
                {
                    allErrors.Add($"Empty or whitespace input for dice: \"{arg}\".");
                    continue;
                }

                var parts = arg.Split(',')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToArray();

                if (parts.Length != _requiredFaceCount)
                {
                    allErrors.Add($"Invalid face count ({parts.Length}) for input: \"{arg}\". Each dice must have {_requiredFaceCount} numbers.");
                    continue;
                }

                var invalidValues = new List<string>();
                var diceFaces = new List<int>();
                foreach (var part in parts)
                {
                    if (!int.TryParse(part, out int value))
                    {
                        invalidValues.Add($"\"{part}\" (not an integer)");
                    }
                    else if (value <= 0)
                    {
                        invalidValues.Add($"\"{part}\" (non-positive)");
                    }
                    else
                    {
                        diceFaces.Add(value);
                    }
                }

                if (invalidValues.Count > 0)
                {
                    allErrors.Add($"Invalid values: {string.Join(", ", invalidValues)} in input: \"{arg}\".");
                }
                else
                {
                    diceList.Add(new Dice(diceFaces));
                }
            }

            if (allErrors.Count > 0)
            {
                throw new ArgumentException(
                    $"Validation failed:{Environment.NewLine}{string.Join(Environment.NewLine, allErrors)}. " +
                    "All dice faces must be positive integers greater than zero. " +
                    "Example: \"2,2,4,4,9,9\" \"6,8,1,1,8,6\" \"7,5,3,7,5,3\".");
            }

            return diceList;
        }
    }
}