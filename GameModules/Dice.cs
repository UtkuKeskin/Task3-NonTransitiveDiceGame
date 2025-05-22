using System;
using System.Collections.Generic;

namespace DiceGame.GameModules
{
    /// <summary>
    /// Represents a dice with 6 predefined face values.
    /// </summary>
    public class Dice
    {
        private readonly int[] _faces;

        /// <summary>
        /// Initializes a new Dice instance with given face values.
        /// </summary>
        /// <param name="faces">List of 6 integers representing the dice faces.</param>
        public Dice(IList<int> faces)
        {
            if (faces == null || faces.Count != 6)
                throw new ArgumentException("Dice must have exactly 6 faces. Example: new int[] {2,2,4,4,9,9}");

            _faces = faces.ToArray();
        }

        /// <summary>
        /// Returns the value of the face at the given index (0 to 5).
        /// </summary>
        /// <param name="index">Face index (0-based).</param>
        /// <returns>Integer value on the selected face.</returns>
        public int GetFace(int index)
        {
            if (_faces == null)
                throw new InvalidOperationException("Dice faces are not initialized.");
            if (index < 0 || index >= _faces.Length)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be between 0 and 5. Example: 0 to 5 for 6 faces.");

            return _faces[index];
        }

        /// <summary>
        /// Gets the readonly array of face values.
        /// </summary>
        public IReadOnlyList<int> Faces => _faces;

        /// <summary>
        /// Returns the dice face values as a comma-separated string.
        /// </summary>
        public override string ToString()
        {
            return _faces != null ? string.Join(",", _faces) : "Invalid Dice";
        }
    }
}