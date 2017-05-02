using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Help
{
    public class HelpPageMenuItem
    {
        private readonly int _index;

        public HelpPageMenuItem( int index, HelpPage helpPage )
        {
            _index = index;
            HelpPage = helpPage;
        }

        /// <summary>
        /// Roman numeral index
        /// </summary>
        public string Number => $"{RomanNumeralFrom(_index)}.";

        /// <summary>
        /// Help page
        /// </summary>
        public HelpPage HelpPage { get; }

        /// <summary>
        /// Converts number to roman numeral
        /// </summary>
        /// <param name="number">Number</param>
        /// <returns>Roman numeral form of the number</returns>
        private string RomanNumeralFrom(int number)
        {
            return
                new string('I', number)
                    .Replace(new string('I', 1000), "M")
                    .Replace(new string('I', 900), "CM")
                    .Replace(new string('I', 500), "D")
                    .Replace(new string('I', 400), "CD")
                    .Replace(new string('I', 100), "C")
                    .Replace(new string('I', 90), "XC")
                    .Replace(new string('I', 50), "L")
                    .Replace(new string('I', 40), "XL")
                    .Replace(new string('I', 10), "X")
                    .Replace(new string('I', 9), "IX")
                    .Replace(new string('I', 5), "V")
                    .Replace(new string('I', 4), "IV");
        }
    }
}
