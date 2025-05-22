using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceGame.GameModules
{
    /// <summary>
    /// Calculates win probabilities for each pair of non-transitive dice.
    /// </summary>
    public class ProbabilityCalculator
    {
        /// <summary>
        /// Calculates win probabilities for each pair of dice in the given list.
        /// </summary>
        /// <param name="dices">List of Dice objects to compare.</param>
        /// <returns>A 2D array where probabilities[i,j] is the probability of dice i beating dice j.</returns>
        /// <exception cref="ArgumentException">Thrown when the dice list is null or empty.</exception>
        public double[,] CalculateWinProbabilities(List<Dice> dices)
        {
            if (dices == null || dices.Count == 0)
                throw new ArgumentException("Dice list cannot be null or empty.");

            int n = dices.Count;
            double[,] probabilities = new double[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                    {
                        probabilities[i, j] = 0.0;
                        continue;
                    }

                    // Get dice faces (dynamic face count support)
                    var facesA = dices[i].Faces; // IReadOnlyList<int>
                    var facesB = dices[j].Faces;

                    // Total possible results (depends on the number of faces)
                    double totalOutcomes = facesA.Count * facesB.Count;

                    // Calculate the number of wins with LINQ:
                    // Generate all A-B combinations with SelectMany (e.g. 6x6 = 36 results)
                    // Find the cases where A exceeds B with (x, y) => x > y
                    // Calculate the number of wins with Count
                    int winCount = facesA.SelectMany(_ => facesB, (x, y) => x > y).Count(x => x);

                    // Calculate possibility : P(A beats B) = (# A wins) / (total outcomes)
                    probabilities[i, j] = winCount / totalOutcomes;
                }
            }

            return probabilities;
        }
    }
}