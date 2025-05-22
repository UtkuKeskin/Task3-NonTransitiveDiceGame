# 🌪️ Non-Transitive Dice Game – Provably Fair CLI App

This project is a solution to **Itransition Intern Developer Task #3**, implementing a generalized non-transitive dice game as a secure and user-friendly console application. It is developed in **C# (.NET 8)** and emphasizes:

* ✅ **Provably fair random number generation** using HMAC-SHA3-256
* ✅ **Modular architecture** with well-isolated responsibility-specific classes
* ✅ **Interactive CLI** using Spectre.Console with color-coded output and ASCII probability tables
* ✅ Support for **3 or more dice**, each with **6 custom face values**


---

🎯 Features

- [x] HMAC-SHA3-256 for verifiable fairness
- [x] Modular architecture with clean OOP design
- [x] CLI-based dice selection and provable random rolls
- [x] Interactive help menu showing win probabilities
- [x] Full input validation with helpful error messages

---

## 🧰 Project Overview

The game simulates non-transitive dice competition and follows strict rules to ensure transparency and fairness. Key features include:

* **Command-line input:** Dice configurations are passed as arguments:

  ```bash
  dotnet run -- "2,2,4,4,9,9" "6,8,1,1,8,6" "7,5,3,7,5,3"
  ```
* **Custom dice:** Each dice must contain 6 positive integers.
* **Fairness via cryptography:**

  * A secure 256-bit key is generated.
  * A number in range (0-5) is created.
  * HMAC-SHA3-256 is computed and shown **before** the user's choice.
  * After user input, the key and number are revealed for verification.
* **Game flow:**

  1. Fair coin toss to determine who selects first
  2. Each player picks different dice
  3. Each performs a roll using collaborative randomness (x + y mod 6)
  4. Results are compared
* **CLI interaction:**

  * Users can select dice, roll, view help (?) or exit (X)
* **Help Option:**

  * Displays win probability table between each dice pair in ASCII
* **Robust error handling:**

  * Invalid dice formats (non-integer, incorrect face count, <3 dice) are caught and shown with examples

---

## 🛠️ Requirements

* [.NET 8 SDK](https://dotnet.microsoft.com/download)
* Terminal (CMD, PowerShell, macOS Terminal, Linux Bash)

---

## 📦 Dependencies

This project uses the following third-party NuGet packages:

| Package                                                                  | Purpose                                                          |
| ------------------------------------------------------------------------ | ---------------------------------------------------------------- |
| [Spectre.Console](https://www.nuget.org/packages/Spectre.Console)        | Rich, color-coded CLI output & ASCII table rendering             |
| [BouncyCastle.Cryptography](https://www.nuget.org/packages/BouncyCastle) | Cryptographic HMAC-SHA3-256 implementation for provable fairness |

Install via:

```bash
dotnet add package Spectre.Console
dotnet add package BouncyCastle.Cryptography
```

---

## 🚀 How to Run

### 1. Clone Repository

```bash
git clone https://github.com/UtkuKeskin/Task3-NonTransitiveDiceGame.git
cd Task3-NonTransitiveDiceGame
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Run the Game

```bash
dotnet run -- "2,2,4,4,9,9" "6,8,1,1,8,6" "7,5,3,7,5,3"
```

Each argument must be a quoted string containing 6 comma-separated integers.

---

## 💻 Sample Output

### Valid Run

```text
Let's determine who makes the first move.
I selected a random value in range 0..1 (HMAC=...)
Your selection: 1
My selection: 0 (KEY=...)
I make the first move!
I choose the [6,8,1,1,8,6] dice.
Choose your dice:
0 - [2,2,4,4,9,9]
1 - [6,8,1,1,8,6]
2 - [7,5,3,7,5,3]
...
You choose the [7,5,3,7,5,3] dice. Your roll result is 7.
I choose the [6,8,1,1,8,6] dice. My roll result is 3.
You win (7 > 3)!
```

### Invalid Input

```bash
dotnet run -- "2,2,4,4,9"
```

```text
Input Error:
Validation failed:
Invalid face count (5) for input: "2,2,4,4,9". Each dice must have 6 numbers.
Example: "2,2,4,4,9,9" "6,8,1,1,8,6" "7,5,3,7,5,3"
```

---

## 📊 Probability Table (on `?` selection)

```
Probability of the win for the user:
╭───────────────────────┬───────────────────────┬───────────────────────┬───────────────────────╮
│ User dice v           │ Dice #0 → 2,2,4,4,9,9 │ Dice #1 → 6,8,1,1,8,6 │ Dice #2 → 7,5,3,7,5,3 │
├───────────────────────┼───────────────────────┼───────────────────────┼───────────────────────┤
│ Dice #0 → 2,2,4,4,9,9 │ -(0.3333)             │ 0.5556                │ 0.4444                │
│ Dice #1 → 6,8,1,1,8,6 │ 0.4444                │ -(0.3333)             │ 0.5556                │
│ Dice #2 → 7,5,3,7,5,3 │ 0.5556                │ 0.4444                │ -(0.3333)             │
╰───────────────────────┴───────────────────────┴───────────────────────┴───────────────────────╯
```

---

## 📄 Architecture

| Class                          | Responsibility                                               |
| ------------------------------ | ------------------------------------------------------------ |
| `Dice.cs`                      | Represents a dice and its face values                        |
| `DiceParser.cs`                | Parses command-line arguments and validates input            |
| `FairNumberGenerator.cs`       | Handles HMAC key generation, SHA3 digest, and fairness logic |
| `ProbabilityCalculator.cs`     | Calculates win probabilities between dice pairs              |
| `ProbabilityTableGenerator.cs` | Renders ASCII win table via Spectre.Console                  |
| `GameEngine.cs`                | Controls game flow (menu, selection, rolling, results)       |
| `Program.cs`                   | Entry point: input handling, error display, and game startup |

---

## 🔒 Provably Fair Randomness

Each random number generation involves both parties:

1. Computer securely generates `x ∈ [0,5]` and a 256-bit secret key.
2. Calculates and displays `HMAC(key, x)`.
3. User selects `y ∈ [0,5]`.
4. Computer reveals key and `x`, allowing user to verify HMAC.
5. Final result is `(x + y) % 6`.

This protocol prevents tampering and ensures provable fairness.

---


## 🧠 Learning Outcomes

Through this project I gained experience in:

* Cryptographic HMAC and secure RNG practices
* Modular architecture and clean separation of concerns
* CLI app design with usability in mind
* Handling real-world input validation & error feedback

---

## ▶️ Demo Video  
🎥 Watch here: [YouTube Demo Video](https://www.youtube.com/watch?v=O3MIRhV6DtI)
