using OmegaGo.Core.Game.Markup;
using OmegaGo.Core.Game.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Game
{
    public sealed class MarkupInfo
    {
        private Dictionary<Type, System.Collections.IList> _markups; 
        private int _count;
        
        public MarkupInfo()
        {
            _markups = new Dictionary<Type, System.Collections.IList>();
            _count = 0;
        }

        /// <summary>
        /// Indicates whether the node contains any markup
        /// </summary>
        public bool IsEmpty => _count == 0;

        /// <summary>
        /// Adds markup to the node.
        /// </summary>
        /// <typeparam name="T">Type of markup (Label, Circle, Cross, Square, Triangle)</typeparam>
        /// <param name="markup">Markup to add.</param>
        public void AddMarkup<T>(T markup) where T : IMarkup
        {
            if (!_markups.ContainsKey(typeof(T)))
                _markups[typeof(T)] = new List<T>();

            _markups[typeof(T)].Add(markup);
            _count++;
        }

        /// <summary>
        /// Removes markup from markup drictionary.
        /// </summary>
        /// <typeparam name="T">Type of markup (Label, Circle, Cross, Square, Triangle)</typeparam>
        /// <param name="markup">Markup to remove.</param>
        public void RemoveMarkup<T>(T markup) where T : IMarkup
        {
            if (!_markups.ContainsKey(typeof(T)))
                throw new ArgumentException("Provided markup does not exist.");

            _markups[typeof(T)].Remove(markup);
            _count--;
        }

        /// <summary>
        /// Returns list of markups of given type.
        /// </summary>
        /// <typeparam name="T">Type of markup (Label, Circle, Cross, Square, Triangle)</typeparam>
        /// <returns>List of markups.</returns>
        public IEnumerable<T> GetMarkups<T>() where T : IMarkup
        {
            if (!_markups.ContainsKey(typeof(T)))
                return Enumerable.Empty<T>();

            // Should we deal with it being castable to List and then modifiable?
            return (IEnumerable<T>)_markups[typeof(T)];
        }
        

        /// <summary>
        /// Removes any markup (if exists) on a provided position and returns its kind.
        /// </summary>
        /// <param name="position">Position to clear</param>
        /// <returns>Kind of markup located on the provided position</returns>
        public MarkupKind RemoveMarkupOnPosition(Position position)
        {
            foreach (var circle in GetMarkups<Circle>())
                if (circle.Position == position)
                {
                    RemoveMarkup<Circle>(circle);
                    return MarkupKind.Circle;
                }

            foreach (var cross in GetMarkups<Cross>())
                if (cross.Position == position)
                {
                    RemoveMarkup<Cross>(cross);
                    return MarkupKind.Cross;
                }

            foreach (var label in GetMarkups<Label>())
                if (label.Position == position)
                {
                    RemoveMarkup<Label>(label);
                    return MarkupKind.Label;
                }

            foreach (var square in GetMarkups<Square>())
                if (square.Position == position)
                {
                    RemoveMarkup<Square>(square);
                    return MarkupKind.Square;
                }

            foreach (var triangle in GetMarkups<Triangle>())
                if (triangle.Position == position)
                {
                    RemoveMarkup<Triangle>(triangle);
                    return MarkupKind.Triangle;
                }

            return MarkupKind.None;
        }

        /// <summary>
        /// Fills the map of shadow items based on the state of markups.  
        /// </summary>
        /// <param name="boardSize">Size of game board.</param>
        /// <param name="kind">Kind of sequence markup. (Letter, Number)</param>
        /// <returns>Map of shadow items.</returns>
        public string[,] FillSequenceShadowMap(GameBoardSize boardSize, SequenceMarkupKind kind)
        {
            string shadow;
            string[,] shadows = new string[boardSize.Width, boardSize.Height];
            if (kind == SequenceMarkupKind.Letter)
                shadow = GetSmallestUnusedLetter().ToString();
            else
                shadow = GetSmallestUnusedNumber().ToString();

            for (int x = 0; x < boardSize.Width; x++)
                for (int y = 0; y < boardSize.Height; y++)
                    shadows[x, y] = shadow;

            foreach (var label in GetMarkups<Label>())
                shadows[label.Position.X, label.Position.Y] = "r";

            return shadows;    
        }

        /// <summary>
        /// Fills the map of shadow items based on the state of markups.  
        /// </summary>
        /// <param name="boardSize">Size of game board.</param>
        /// <param name="kind">Kind of simple markup. (Circle, Cross, Square, Triangle)</param>
        /// <returns>Map of shadow items.</returns>
        public char[,] FillSimpleShadowMap(GameBoardSize boardSize, SimpleMarkupKind kind)
        {
            char[,] shadows = new char[boardSize.Width, boardSize.Height];

            switch (kind)
            {
                case SimpleMarkupKind.Circle:
                    {
                        for (int x = 0; x < boardSize.Width; x++)
                            for (int y = 0; y < boardSize.Height; y++)
                                shadows[x, y] = 'c';

                        foreach (var circle in GetMarkups<Circle>())
                            shadows[circle.Position.X, circle.Position.Y] = 'r';
                    }
                    return shadows;
                case SimpleMarkupKind.Cross:
                    {
                        for (int x = 0; x < boardSize.Width; x++)
                            for (int y = 0; y < boardSize.Height; y++)
                                shadows[x, y] = 'x';

                        foreach (var cross in GetMarkups<Cross>())
                            shadows[cross.Position.X, cross.Position.Y] = 'r';
                    }
                    return shadows;
                case SimpleMarkupKind.Square:
                    {
                        for (int x = 0; x < boardSize.Width; x++)
                            for (int y = 0; y < boardSize.Height; y++)
                                shadows[x, y] = 's';

                        foreach (var square in GetMarkups<Square>())
                            shadows[square.Position.X, square.Position.Y] = 'r';
                    }
                    return shadows;
                case SimpleMarkupKind.Triangle:
                    {
                        for (int x = 0; x < boardSize.Width; x++)
                            for (int y = 0; y < boardSize.Height; y++)
                                shadows[x, y] = 't';

                        foreach (var triangle in GetMarkups<Triangle>())
                            shadows[triangle.Position.X, triangle.Position.Y] = 'r';
                    }
                    return shadows;
            }

            return shadows;
        }

        /// <summary>
        /// Returns the smallest unused capital letter based on the state of markups. Smallest means the character with smallest ASCII code.
        /// </summary>
        /// <returns>Unused capital letter or '0', if each capital letter is used.</returns>
        public char GetSmallestUnusedLetter()
        {
            bool[] isLetterUsed = new bool[26];
            foreach (var label in GetMarkups<Label>())
            {
                char letter;
                char.TryParse(label.Text, out letter);
                if (letter >= 'A' && letter <= 'Z')
                    isLetterUsed[letter - 65] = true;
            }

            for (int i = 0; i < isLetterUsed.Length; i++)
                if (!isLetterUsed[i])
                    return Convert.ToChar(i + 65);
            
            return '0';
        }

        /// <summary>
        /// Returns the next unused capital letter based on the state of markups.
        /// </summary>
        /// <returns>Unused capital letter or '0', if 'Z' is used.</returns>
        public char GetNextLetter()
        {
            char maxUsedLetter = '@';
            foreach (var label in GetMarkups<Label>())
            {
                char letter;
                char.TryParse(label.Text, out letter);
                if (letter >= 'A' && letter <= 'Z')
                    if (letter > maxUsedLetter)
                        maxUsedLetter = letter;
            }

            if (maxUsedLetter < 'Z')
                return (++maxUsedLetter);
            else
                return '0';
        }

        /// <summary>
        /// Returns the smallest unused integer number based on the state of markups. 
        /// The smallest number is 1.
        /// </summary>
        /// <returns>Unused number or 0, if each number up to 700 is used.</returns>
        public int GetSmallestUnusedNumber()
        {
            bool[] isNumberUsed = new bool[700];
            foreach (var label in GetMarkups<Label>())
            {
                int number;
                int.TryParse(label.Text, out number);
                if (number != 0)
                    isNumberUsed[number] = true;
            }

            for (int i = 1; i < isNumberUsed.Length; i++)
                if (!isNumberUsed[i])
                    return i;

            return 0;
        }

        /// <summary>
        /// Returns the next unused number based on the state of markups.
        /// </summary>
        /// <returns>Unused number.</returns>
        public int GetNextNumber()
        {
            int maxUsedNumber = 0;
            foreach (var label in GetMarkups<Label>())
            {
                int number;
                int.TryParse(label.Text, out number);
                if (number != 0)
                    if (number > maxUsedNumber)
                        maxUsedNumber = number;
            }

            return (++maxUsedNumber);
        }
    }
}
