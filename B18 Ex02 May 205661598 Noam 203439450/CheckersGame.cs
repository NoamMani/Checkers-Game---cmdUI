using System;
using System.Threading;

namespace B18_Ex02
{
    public class CheckersGame
    {
        internal const int k_SleepTime = 1250;
        private static bool s_IsVsComputer;

        private static Game s_Game;

        // $G$ DSN-999 (-10) Why all methods are static? , next time create an instance of CheckersGame
        public static void Run()
        {
            Game.eMessage outputMessage = Game.eMessage.StartGame;

            getGameDetails();
            s_Game.StartNewGame();
            UI.PrintBoard(s_Game);
            while (outputMessage != Game.eMessage.EndGame)
            {
                if (outputMessage == Game.eMessage.NewGame)
                {
                    s_Game.StartNewGame();
                    printUpdatedBoard();
                }

                if (!s_Game.PlayerCanPlay(s_Game.FirstPlayerId) || outputMessage == Game.eMessage.PlayerQuit)
                {
                    outputMessage = endRound();
                }
                else
                {
                    outputMessage = play(s_Game.FirstPlayerId);
                    if (s_Game.PlayerCanPlay(s_Game.SecondPlayerId) && outputMessage != Game.eMessage.PlayerQuit)
                    {
                        outputMessage = play(s_Game.SecondPlayerId);
                    }
                    else
                    {
                        outputMessage = endRound();
                    }
                }
            }
        }

        private static void getGameDetails()
        {
            string firstPlayerName = UI.GetPlayerName(), secondPlayerName = null;
            int    boardSize = UI.GetBoardSize();

            s_IsVsComputer = UI.IsAgainstComputer();
            if (s_IsVsComputer == false)
            {
                secondPlayerName = UI.GetPlayerName();
            }

            Ex02.ConsoleUtils.Screen.Clear();
            s_Game = new Game(firstPlayerName, secondPlayerName, boardSize);
        }

        private static Game.eMessage play(Game.ePlayerId i_PlayerId)
        {
            bool             isTurnFinished = false;
            string           input = string.Empty;
            string           playerName = s_Game.GetPlayerNameById(i_PlayerId);
            BoardCell.eSigns playerSign = BoardCell.GetSignFromPlayerId(i_PlayerId);
            Game.eMessage    outputMessage = Game.eMessage.StartGame;

            while (!isTurnFinished)
            {
                if (i_PlayerId == Game.ePlayerId.Computer)
                {
                    Thread.Sleep(k_SleepTime);
                    input = s_Game.GetComputerInstruction();
                }
                else
                {
                    input = UI.GetInstruction(playerName, playerSign);
                }

                outputMessage = s_Game.PlayTurn(input, i_PlayerId);
                isTurnFinished = isPlayerTurnFinished(outputMessage);
                if (outputMessage == Game.eMessage.MustJumpOverAgain)
                {
                    printUpdatedBoard();
                    printLastInsruction(i_PlayerId, input);
                    UI.PrintMessage(outputMessage);
                }
            }

            printUpdatedBoard();
            printLastInsruction(i_PlayerId, input);

            return outputMessage;
        }

        private static Game.eMessage endRound()
        {
            Game.eMessage outputMessage;
            string        winnerName = s_Game.UpdateScoreAndWinner();

            UI.PrintEndRoundDetails(s_Game, winnerName);
            outputMessage = UI.AnotherGame();

            return outputMessage;
        }

        private static void printUpdatedBoard()
        {
            Ex02.ConsoleUtils.Screen.Clear();
            UI.PrintBoard(s_Game);
        }

        private static void printLastInsruction(Game.ePlayerId i_PlayerId, string i_Instruction)
        {
            string           playerName;
            BoardCell.eSigns playerSign;

            if (i_Instruction.Length != UI.k_SingleLetter)
            {
                playerName = s_Game.GetPlayerNameById(i_PlayerId);
                playerSign = BoardCell.GetSignFromPlayerId(i_PlayerId);
                UI.PrintInsruction(playerSign, playerName, i_Instruction);
            }
        }

        private static bool isPlayerTurnFinished(Game.eMessage i_Message)
        {
            UI.PrintMessage(i_Message);

            return i_Message == Game.eMessage.EndTurn || i_Message == Game.eMessage.PlayerQuit;
        }
    }
}
