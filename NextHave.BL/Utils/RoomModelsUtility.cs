namespace NextHave.BL.Utils
{
    public static class RoomModelUtility
    {
        public static short ParseInput(this string input)
            => input switch
            {
                "0" => 0,
                "1" => 1,
                "2" => 2,
                "3" => 3,
                "4" => 4,
                "5" => 5,
                "6" => 6,
                "7" => 7,
                "8" => 8,
                "9" => 9,
                "a" => 10,
                "b" => 11,
                "c" => 12,
                "d" => 13,
                "e" => 14,
                "f" => 15,
                "g" => 16,
                "h" => 17,
                "i" => 18,
                "j" => 19,
                "k" => 20,
                "l" => 21,
                "m" => 22,
                "n" => 23,
                "o" => 24,
                "p" => 25,
                "q" => 26,
                "r" => 27,
                "s" => 28,
                "t" => 29,
                "u" => 30,
                "v" => 31,
                "w" => 32,
                "y" => 34,
                "z" => 35,
                _ => throw new FormatException("The input was not in a correct format: input must be a number between 0 and 9"),
            };
    }
}