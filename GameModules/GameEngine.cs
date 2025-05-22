using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using DiceGame.GameModules;
using Spectre.Console;

namespace DiceGame.GameModules
{
    /// <summary>
    /// Manages the game flow, including dice selection, fair rolls, and result determination.
    /// </summary>
    public class GameEngine
    {
        private readonly List<Dice> _dices;
        private readonly FairNumberGenerator _fairGenerator;
        private readonly ProbabilityTableGenerator _tableGenerator;
        private readonly ProbabilityCalculator _probabilityCalculator;

        public GameEngine(List<Dice> dices)
        {
            if (dices == null || dices.Count < 3)
                throw new ArgumentException("At least 3 dice configurations are required. Example: \"2,2,4,4,9,9\" \"6,8,1,1,8,6\" \"7,5,3,7,5,3\".");

            _dices = dices;
            _fairGenerator = new FairNumberGenerator(6); // 0-5 range for dice faces
            _tableGenerator = new ProbabilityTableGenerator();
            _probabilityCalculator = new ProbabilityCalculator();
        }

        public void Start()
        {
            bool playAgain = true;
            while (playAgain)
            {
                bool gamePlayed = DetermineFirstMove();
                if (!gamePlayed)
                {
                    AnsiConsole.MarkupLine("[green]Thank you for playing![/]");
                    return;
                }
                playAgain = AskPlayAgain();
            }
            AnsiConsole.MarkupLine("[green]Thank you for playing![/]");
        }

        private bool DetermineFirstMove()
        {
            AnsiConsole.MarkupLine("[yellow]Let's determine who makes the first move.[/]");
            var fairGenFirst = new FairNumberGenerator(2); // 0-1 range for first move
            var serverSeed = fairGenFirst.CreateServerSeed();
            var computerChoice = fairGenFirst.GenerateComputerNumber();
            var hmac = fairGenFirst.ComputeHmac(serverSeed, computerChoice);

            AnsiConsole.MarkupLine($"I selected a random value in range 0..1 (HMAC={hmac}).");
            AnsiConsole.MarkupLine("Try to guess my selection:");
            AnsiConsole.MarkupLine("0 - 0");
            AnsiConsole.MarkupLine("1 - 1");
            AnsiConsole.MarkupLine("X - exit");
            AnsiConsole.MarkupLine("? - help");

            string userInput = AnsiConsole.Ask<string>("Your selection: ");
            if (userInput.ToUpper() == "X") return false;
            if (userInput == "?")
            {
                ShowHelp();
                return DetermineFirstMove();
            }

            if (!int.TryParse(userInput, out int userChoice) || userChoice < 0 || userChoice > 1)
            {
                AnsiConsole.MarkupLine("[red]Invalid input. Please enter 0,1, X, or ?.[/]");
                return DetermineFirstMove();
            }

            AnsiConsole.MarkupLine($"My selection: {computerChoice} (KEY={BitConverter.ToString(serverSeed).Replace("-", "").ToUpper()}).");
            if (userChoice == computerChoice)
            {
                AnsiConsole.MarkupLine("[green]You make the first move![/]");
                UserSelectsFirst();
            }
            else
            {
                AnsiConsole.MarkupLine("[green]I make the first move![/]");
                ComputerSelectsFirst();
            }
            return true;
        }

        private void UserSelectsFirst()
        {
            int computerDiceIndex = SelectComputerDice();
            int userDiceIndex = ShowDiceMenu("Choose your dice:");
            if (userDiceIndex == -1) return; // Exit
            if (userDiceIndex == -2) { ShowHelp(); UserSelectsFirst(); return; } // help

            PlayRound(userDiceIndex, computerDiceIndex);
        }

        private void ComputerSelectsFirst()
        {
            int computerDiceIndex = SelectComputerDice();
            AnsiConsole.MarkupLine($"I choose the [[{_dices[computerDiceIndex].ToString()}]] dice.");
            int userDiceIndex = ShowDiceMenu("Choose your dice (cannot select my dice):");
            if (userDiceIndex == -1) return; // Exit
            if (userDiceIndex == -2) { ShowHelp(); ComputerSelectsFirst(); return; } // Help
            if (userDiceIndex == computerDiceIndex)
            {
                AnsiConsole.MarkupLine("[red]You cannot select my dice. Please choose again.[/]");
                ComputerSelectsFirst();
                return;
            }

            PlayRound(userDiceIndex, computerDiceIndex);
        }

        private int SelectComputerDice()
        {
            byte[] randomBytes = new byte[4];
            RandomNumberGenerator.Fill(randomBytes);
            int randomValue = BitConverter.ToInt32(randomBytes, 0);
            return Math.Abs(randomValue) % _dices.Count;
        }

        private int ShowDiceMenu(string prompt)
        {
            AnsiConsole.Markup($"{prompt}{Environment.NewLine}");
            for (int i = 0; i < _dices.Count; i++)
            {
                AnsiConsole.MarkupLine($"{i} - [[{_dices[i].ToString()}]]");
            }
            AnsiConsole.MarkupLine("X - exit");
            AnsiConsole.MarkupLine("? - help");

            string input = AnsiConsole.Ask<string>("Your selection: ");
            if (input.ToUpper() == "X") return -1;
            if (input == "?") return -2;

            if (!int.TryParse(input, out int choice) || choice < 0 || choice >= _dices.Count)
            {
                AnsiConsole.MarkupLine("[red]Invalid input. Please enter a valid dice index, X, or ?.[/]");
                return ShowDiceMenu(prompt);
            }
            return choice;
        }

        private void PlayRound(int userDiceIndex, int computerDiceIndex)
        {
            try
            {
                AnsiConsole.MarkupLine("It's time for your roll.");
                int userRoll = PerformFairRoll(_dices[userDiceIndex]);
                if (userRoll == -1) return; // Exit

                AnsiConsole.MarkupLine("It's time for my roll.");
                int computerRoll = PerformFairRoll(_dices[computerDiceIndex]);
                if (computerRoll == -1) return; // Exit

                AnsiConsole.MarkupLine($"You choose the [[{_dices[userDiceIndex].ToString()}]] dice. Your roll result is {userRoll}.");
                AnsiConsole.MarkupLine($"I choose the [[{_dices[computerDiceIndex].ToString()}]] dice. My roll result is {computerRoll}.");

                if (userRoll > computerRoll)
                    AnsiConsole.MarkupLine($"[green]You win ({userRoll} > {computerRoll})![/]");
                else if (computerRoll > userRoll)
                    AnsiConsole.MarkupLine($"[green]I win ({computerRoll} > {userRoll})![/]");
                else
                    AnsiConsole.MarkupLine($"[yellow]It's a tie ({userRoll} = {computerRoll})![/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error in game round: {ex.Message}[/]");
            }
        }

        private int PerformFairRoll(Dice dice)
        {
            if (dice == null)
                throw new ArgumentNullException(nameof(dice));

            var serverSeed = _fairGenerator.CreateServerSeed();
            var computerNumber = _fairGenerator.GenerateComputerNumber();
            var hmac = _fairGenerator.ComputeHmac(serverSeed, computerNumber);

            AnsiConsole.MarkupLine($"I selected a random value in range 0..{_fairGenerator.Range - 1} (HMAC={hmac}).");
            AnsiConsole.MarkupLine("Add your number modulo 6:");
            for (int i = 0; i < _fairGenerator.Range; i++) AnsiConsole.MarkupLine($"{i} - {i}");
            AnsiConsole.MarkupLine("X - exit");
            AnsiConsole.MarkupLine("? - help");

            string userInput = AnsiConsole.Ask<string>("Your selection: ");
            if (userInput.ToUpper() == "X") return -1;
            if (userInput == "?")
            {
                ShowHelp();
                return PerformFairRoll(dice);
            }

            if (!int.TryParse(userInput, out int userNumber) || userNumber < 0 || userNumber >= _fairGenerator.Range)
            {
                AnsiConsole.MarkupLine($"[red]Invalid input. Please enter a number between 0 and {_fairGenerator.Range - 1}.[/]");
                return PerformFairRoll(dice);
            }

            int result = _fairGenerator.CombineWithUserInput(computerNumber, userNumber);
            AnsiConsole.MarkupLine($"My number: {computerNumber} (KEY={BitConverter.ToString(serverSeed).Replace("-", "").ToUpper()}).");
            AnsiConsole.MarkupLine($"The fair number generation result is {computerNumber} + {userNumber} = {result} (mod {_fairGenerator.Range}).");
            int rollResult = dice.GetFace(result);
            return rollResult;
        }

        private void ShowHelp()
        {
            var probabilities = _probabilityCalculator.CalculateWinProbabilities(_dices);
            _tableGenerator.GenerateProbabilityTable(_dices, probabilities);
        }

        private bool AskPlayAgain()
        {
            string response = AnsiConsole.Ask<string>("Play again? (Y/N): ").ToUpper();
            return response == "Y";
        }
    }
}