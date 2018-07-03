using System;

namespace B18_Ex02
{
    public class UI
    {
        internal const int    k_SmallBoard = 6;
        internal const int    k_MediumBoard = 8;
        internal const int    k_LargeBoard = 10;
        internal const int    k_MessageArrSize = 6;
        internal const int    k_SperateIndention = 4;
        internal const int    k_InstructionLength = 5;
        internal const int    k_SingleLetter = 1;
        internal const char   k_ArrowSign = '>';
        internal const char   k_Yes = 'Y';
        internal const char   k_No = 'N';
        internal const char   k_Quit = 'Q';

        public static string[] s_Messages = new string[k_MessageArrSize]
        {
         "The Instruction is not valid",
         "The player must jump over!",
         "Please jump over again",
         "End turn",
         "The player quit",
         "Player cant quit",
            };

        public static void PrintBoard(Game i_Game)
        {
            printColumnsIdentifier(i_Game.BoardSize);
            printFullBoard(i_Game);
        }

        private static void printColumnsIdentifier(int i_BoardSize)
        {
            Console.Write((char)BoardCell.eSigns.Empty);
            for (int i = 0; i < i_BoardSize; i++)
            {
                Console.Write("  {0} ", (char)(Board.k_FirstColId + i));
            }

            printSeparteRow(i_BoardSize);
        }

        private static void printFullBoard(Game i_Game)
        {
            int boardSize = i_Game.BoardSize;

            for (int i = 0; i < boardSize; i++)
            {
                printRowIdentifiers(i);
                for (int j = 0; j < boardSize; j++)
                {
                    Console.Write("| {0} ", (char)i_Game.BoardCellSign(i, j));
                }

                Console.Write("|");
                printSeparteRow(boardSize);
            }
        }

        private static void printRowIdentifiers(int i_Identifier)
        {
            Console.Write("{0}", (char)(Board.k_FirstRowId + i_Identifier));
        }

        private static void printSeparteRow(int i_BoardSize)
        {
            Console.Write(Environment.NewLine);
            Console.Write((char)BoardCell.eSigns.Empty);
            for (int i = 0; i <= i_BoardSize * k_SperateIndention; i++)
            {
                Console.Write((char)BoardCell.eSigns.SepratorSign);
            }

            Console.Write(Environment.NewLine);
        }

        public static int GetBoardSize()
        {
            string boardSizeStr;
            int    boardSize;

            do
            {
                Console.WriteLine("Please enter the board size (6/8/10)");
                boardSizeStr = Console.ReadLine();
            }
            while (!int.TryParse(boardSizeStr, out boardSize) || !isValidBoardSize(boardSize));

            return boardSize;
        }

        private static bool isValidBoardSize(int i_BoardSize)
        {
            return i_BoardSize == k_SmallBoard || i_BoardSize == k_MediumBoard || i_BoardSize == k_LargeBoard;
        }

        public static string GetPlayerName()
        {
            string name;

            Console.WriteLine("Please enter Player's name: ");
            name = Console.ReadLine();

            return name;
        }

        public static bool IsAgainstComputer()
        {
            string input;

            Console.WriteLine("Do you have Second player? Y/N ");
            input = Console.ReadLine();
            while (input.Length != k_SingleLetter || (input[0] != k_Yes && input[0] != k_No))
            {
                Console.WriteLine("Wrong input");
                Console.WriteLine("Do you have Second player? Y/N ");
                input = Console.ReadLine();
            }

            return input[0] == k_Yes ? false : true;
        }

        public static string GetInstruction(string i_PlayerName, BoardCell.eSigns i_PlayerSign)
        {
            string instruction;
            string message = string.Format("{0}'s Turn ({1}): ", i_PlayerName, (char)i_PlayerSign);

            Console.Write(message);
            instruction = Console.ReadLine();

            while (!isValidGameInput(instruction))
            {
                PrintMessage(Game.eMessage.InstructionNotValid);
                Console.Write(message);
                instruction = Console.ReadLine();
            }

            return instruction;
        }

        private static bool isValidGameInput(string i_InputStr)
        {
            bool isValidInput = false;

            if (i_InputStr.Length == k_InstructionLength)
            {
                isValidInput = isValidInstruction(i_InputStr);
            }
            else if (i_InputStr.Length == k_SingleLetter && i_InputStr[0] == k_Quit)
            {
                isValidInput = true;
            }

            return isValidInput;
        }

        private static bool isValidInstruction(string i_InputStr)
        {
            bool isValid = char.IsUpper(i_InputStr[Board.k_Col]) && char.IsUpper(i_InputStr[Board.k_ToCol]);

            isValid = char.IsLower(i_InputStr[Board.k_Row]) && char.IsLower(i_InputStr[Board.k_ToRow]);

            return isValid && i_InputStr[2] == k_ArrowSign;
        }

        public static void PrintMessage(Game.eMessage i_MessageId)
        {
            Console.WriteLine(s_Messages[(int)i_MessageId]);
        }

        public static void PrintEndRoundDetails(Game i_Game, string i_WinnerName)
        {
            string scoresMessage;

            Ex02.ConsoleUtils.Screen.Clear();
            scoresMessage = string.Format(
                @"End Round!
The winner in this round is: {0}, Well done!
The scores after this round:
{1}'s Score: {2}
{3}'s Score: {4}",
i_WinnerName,
i_Game.FirstPlayerName,
i_Game.FirstPlayerScore,
i_Game.SecondPlayerName,
i_Game.SecondPlayerScore);
            Console.WriteLine(scoresMessage);
        }

        public static Game.eMessage AnotherGame()
        {
            string input;

            Console.WriteLine("Do you want to start a new round? Y/N");
            input = Console.ReadLine();
            while (input.Length != k_SingleLetter || (input[0] != k_Yes && input[0] != k_No))
            {
                Console.WriteLine("Wrong input");
                Console.WriteLine("Do you want to start a new round? Y/N");
                input = Console.ReadLine();
            }

            return input[0] == k_Yes ? Game.eMessage.NewGame : Game.eMessage.EndGame;
        }

        public static void PrintInsruction(BoardCell.eSigns i_PlayerSign, string i_PlayerName, string i_Instruction)
        {
            string message = string.Format("{0}'s move was ({1}): {2}", i_PlayerName, (char)i_PlayerSign, i_Instruction);

            Console.WriteLine(message);
        }
    }
}