using System;
using System.Text;

namespace UnitTestProject {
    internal static class Generator {
        public static string MakeRandomString(int argLength) {
            if (argLength < 1)
                return String.Empty;

            var stringBuilder = new StringBuilder();
            var random = new Random(argLength);
            for (int index = 0; index < argLength; index++) {
                var character = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                stringBuilder.Append(character);
            }

            return stringBuilder.ToString();
        }

        public static string MakeMaxLengthRandomString(int argLength) {
            if (argLength < 1)
                return String.Empty;

            var maxLength = RandomNumberMaxValue(argLength);

            var stringBuilder = new StringBuilder();
            var random = new Random(maxLength);

            for (int index = 0; index < maxLength; index++) {
                var character = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                stringBuilder.Append(character);
            }

            return stringBuilder.ToString();
        }

        private static Int32 RandomNumberMaxValue(int argMaxValue) {
            var random = new Random(argMaxValue);
            return random.Next(1, argMaxValue);
        }
    }
}
