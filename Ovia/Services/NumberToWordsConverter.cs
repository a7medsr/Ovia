using System.Runtime.CompilerServices;
using System.Text;


namespace Ovia.Services
{
    public class NumberToWordsConverter
    {
        private static readonly string[] OnesPlace = { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };
        private static readonly string[] Teens = { "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
        private static readonly string[] TensPlace = { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
        private static readonly string[] ThousandsGroups = { "", " Thousand", " Million", " Billion" };

        public static string ConvertToWords(decimal number)
        {
            if (number == 0)
                return "Zero";

            var result = new StringBuilder();
            int group = 0;

            while (number > 0)
            {
                int thousands = (int)number % 1000;
                if (thousands != 0)
                {
                    string thousandsWords = ConvertGroupToWords(thousands);
                    result.Insert(0, thousandsWords + ThousandsGroups[group] + " ");
                }

                number /= 1000;
                group++;
            }

            return result.ToString().Trim();
        }

        private static string ConvertGroupToWords(int number)
        {
            var result = new StringBuilder();

            int hundreds = number / 100;
            if (hundreds > 0)
            {
                result.Append(OnesPlace[hundreds] + " Hundred ");
                number %= 100;
            }

            if (number >= 10 && number <= 19)
            {
                result.Append(Teens[number % 10]);
            }
            else
            {
                int tens = number / 10;
                if (tens > 0)
                {
                    result.Append(TensPlace[tens] + " ");
                    number %= 10;
                }

                if (number > 0)
                {
                    result.Append(OnesPlace[number]);
                }
            }

            return result.ToString().Trim();
        }
    }
}
