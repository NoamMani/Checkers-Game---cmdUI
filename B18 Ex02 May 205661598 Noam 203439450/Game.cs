using System;
using System.Collections.Generic;

namespace B18_Ex02
{
    public class Game
    {
        internal const char  k_Quit = 'Q';
        internal const int   k_Empty = 0;
        internal const int   k_CheckerValue = 1;
        internal const int   k_KingValue = 4;

        public enum eMoveType
        {
            SimpleMove,
            JumpOver,
        }

        public enum ePlayerId
        {
            Player1,
            Player2,
            Computer,
            None,
        }

        public enum eMessage
        {
            InstructionNotValid,
            MustJumpOver,
            MustJumpOverAgain,
            EndTurn,
            PlayerQuit,
            PlayerCantQuit,
            StartGame,
            EndGame,
            NewGame,
        }

        private Board    m_Board;
        private Player[] m_Players;

        public Game(string i_FirstPlayerName, string i_SecondPlayerName, int i_BoardSize)
        {
            m_Players = new Player[3];
            m_Players[(int)ePlayerId.Player1] = new Player(i_FirstPlayerName, ePlayerId.Player1);
            if (i_SecondPlayerName == null)
            {
                m_Players[(int)ePlayerId.Computer] = new Player("Computer", ePlayerId.Computer);
            }
            else
            {
                m_Players[(int)ePlayerId.Player2] = new Player(i_SecondPlayerName, ePlayerId.Player2);
            }

            m_Board = new Board(i_BoardSize);
        }

        // $G$ DSN-002 (-10) You should seperate UI and Logical parts 
        public static string GetCellIdString(char i_ColIdentifier, char i_RowIndetifier)
        {
            return string.Format("{0}{1}", i_ColIdentifier, i_RowIndetifier);
        }

        public int BoardSize
        {
            get { return m_Board.BoardSize; }
        }

        public BoardCell.eSigns BoardCellSign(int i_Row, int i_Col)
        {
            return m_Board[i_Row, i_Col].Sign;
        }

        public ePlayerId FirstPlayerId
        {
            get { return m_Players[(int)ePlayerId.Player1].Id; }
        }

        public ePlayerId SecondPlayerId
        {
            get { return m_Players[(int)ePlayerId.Player2] == null ? ePlayerId.Computer : ePlayerId.Player2; }
        }

        public string FirstPlayerName
        {
            get { return m_Players[(int)FirstPlayerId].Name; }
        }

        public string SecondPlayerName
        {
            get { return m_Players[(int)SecondPlayerId].Name; }
        }

        public string GetPlayerNameById(ePlayerId i_PlayerId)
        {
            return m_Players[(int)i_PlayerId].Name;
        }

        public int FirstPlayerScore
        {
            get { return m_Players[(int)FirstPlayerId].Score; }
        }

        public int SecondPlayerScore
        {
            get { return m_Players[(int)SecondPlayerId].Score; }
        }

        private ePlayerId getOpponentId(ePlayerId i_CurrentPlayerId)
        {
            return i_CurrentPlayerId != FirstPlayerId ? FirstPlayerId : SecondPlayerId;
        }

        public void StartNewGame()
        {
            InitializePlayersOnBoard();
        }

        private void InitializePlayersOnBoard()
        {
            int secondPlayerLastRow = m_Board.BoardSize / 2;
            int firstPlayerFirstRow = m_Board.BoardSize / 2;
            int lastRow = m_Board.BoardSize;

            initializePlayer(firstPlayerFirstRow, lastRow, FirstPlayerId);
            initializePlayer(Board.k_FirstRow, secondPlayerLastRow, SecondPlayerId);
            initializeOptionsLists();
        }

        private void initializePlayer(int i_From, int i_To, ePlayerId i_PlayerId)
        {
            BoardCell.eSigns sign = BoardCell.GetSignFromPlayerId(i_PlayerId);
            bool             needToInitialize = i_From != Board.k_FirstRow;

            m_Players[(int)i_PlayerId].ChekersList.Clear();
            for (int i = i_From; i < i_To; i++)
            {
                for (int j = 0; j < m_Board.BoardSize; j++)
                {
                    m_Board[i, j] = new BoardCell();
                    if (!isEmptyRow(i) && needToSetChecker(i, j))
                    {
                        m_Board[i, j].InitializeCell(i, j, i_PlayerId);
                        m_Players[(int)i_PlayerId].AddChecker(m_Board[i, j].Id);
                    }
                    else
                    {
                        m_Board[i, j].InitializeCell(i, j, ePlayerId.None);
                    }
                }
            }
        }

        private bool isEmptyRow(int i_Row)
        {
            int halfBoard = m_Board.BoardSize / 2;

            return i_Row == halfBoard || i_Row == halfBoard - 1;
        }

        private bool needToSetChecker(int i_Row, int i_Col)
        {
            return (i_Row + i_Col) % 2 != 0;
        }

        private void initializeOptionsLists()
        {
            for (int i = 0; i < m_Board.BoardSize; i++)
            {
                for (int j = 0; j < m_Board.BoardSize; j++)
                {
                    if (m_Board[i, j].OwnerId != ePlayerId.None)
                    {
                        m_Board.FindOptions(m_Board[i, j]);
                    }
                }
            }
        }

        public eMessage PlayTurn(string i_Instruction, ePlayerId i_PlayerId)
        {
            eMessage outputMessage;

            if (i_Instruction[0] == k_Quit)
            {
                outputMessage = tryQuit(i_PlayerId);
            }
            else
            {
                outputMessage = continuePlayerTurn(i_Instruction, i_PlayerId);
            }

            return outputMessage;
        }

        public string GetComputerInstruction()
        {
            string startCellId, destinationCellId, instruction;
            bool   needToJumpOver = mustJumpOver(ePlayerId.Computer);
            bool   isSimpleMove = !needToJumpOver;

            getRandInstruction(out startCellId, out destinationCellId, needToJumpOver);
            instruction = string.Format("{0}>{1}", startCellId, destinationCellId);

            return instruction;
        }

        private void getRandInstruction(out string o_StartCellId, out string o_DestinationCellId, bool i_NeedToJumpOver)
        {
            Random    rndNum = new Random();
            int       countCheckers = m_Players[(int)ePlayerId.Computer].ChekersList.Count;
            int       rndChecker = rndNum.Next(0, countCheckers);
            eMoveType moveType = i_NeedToJumpOver ? eMoveType.JumpOver : eMoveType.SimpleMove;

            o_DestinationCellId = null;
            o_StartCellId = m_Players[(int)ePlayerId.Computer].ChekersList[rndChecker];
            while ((i_NeedToJumpOver && !m_Board[o_StartCellId].NeedToJumpOver()) || m_Board[o_StartCellId].OptionsList.Count == k_Empty)
            {
                rndChecker = (rndChecker + 1) % countCheckers;
                o_StartCellId = m_Players[(int)ePlayerId.Computer].ChekersList[rndChecker];
            }

            foreach (string optionStr in m_Board[o_StartCellId].OptionsList.Keys)
            {
                if (m_Board[o_StartCellId].OptionsList[optionStr] == moveType)
                {
                    o_DestinationCellId = optionStr;
                    break;
                }
            }
        }

        private eMessage tryQuit(ePlayerId i_PlayerId)
        {
            eMessage outputMessage;

            if (m_Players[(int)i_PlayerId].ChekersList.Count < m_Players[(int)getOpponentId(i_PlayerId)].ChekersList.Count)
            {
                outputMessage = eMessage.PlayerQuit;
            }
            else
            {
                outputMessage = eMessage.PlayerCantQuit;
            }

            return outputMessage;
        }

        private eMessage continuePlayerTurn(string i_Instruction, ePlayerId i_PlayerId)
        {
            string   startCellId = GetCellIdString(i_Instruction[Board.k_Col], i_Instruction[Board.k_Row]);
            string   destinationCellId = GetCellIdString(i_Instruction[Board.k_ToCol], i_Instruction[Board.k_ToRow]);
            bool     isSimpleMove;
            eMessage outputMessage;

            if (isValidMoveForPlayer(startCellId, destinationCellId, i_PlayerId))
            {
                isSimpleMove = m_Board[startCellId].OptionsList[destinationCellId] == eMoveType.SimpleMove;
                if (isSimpleMove && mustJumpOver(i_PlayerId))
                {
                    outputMessage = eMessage.MustJumpOver;
                }
                else
                {
                    move(startCellId, destinationCellId, i_PlayerId);
                    outputMessage = needToJumpOverAgain(destinationCellId, isSimpleMove);
                }
            }
            else
            {
                outputMessage = eMessage.InstructionNotValid;
            }

            return outputMessage;
        }

        private bool isValidMoveForPlayer(string i_StartCellId, string i_DestinationCellId, ePlayerId i_PlayerId)
        {
            bool isValidMove = m_Board[i_StartCellId].IsPossiableMove(i_DestinationCellId);

            if (isValidMove)
            {
                isValidMove = m_Players[(int)i_PlayerId].IsMyChecker(i_StartCellId);
            }

            return isValidMove;
        }

        private void move(string i_StartCellId, string i_DestinationCellId, ePlayerId i_PlayerId)
        {
            if (m_Board[i_StartCellId].OptionsList[i_DestinationCellId] == eMoveType.JumpOver)
            {
                removeCheckerFromOpponent(i_StartCellId, i_DestinationCellId, i_PlayerId);
            }

            m_Players[(int)i_PlayerId].Move(i_StartCellId, i_DestinationCellId);
            m_Board.MoveCheckerOnBoard(i_StartCellId, i_DestinationCellId);
            updateOptionsOnBoard();
        }

        private eMessage needToJumpOverAgain(string i_DestinationCellId, bool i_IsSimpleMove)
        {
            eMessage outputMessage;

            if (!i_IsSimpleMove && m_Board[i_DestinationCellId].OptionsList.ContainsValue(eMoveType.JumpOver))
            {
                outputMessage = eMessage.MustJumpOverAgain;
            }
            else
            {
                outputMessage = eMessage.EndTurn;
            }

            return outputMessage;
        }

        private bool mustJumpOver(ePlayerId i_PlayerId)
        {
            bool mustJumpOver = false;

            foreach (string checkerId in m_Players[(int)i_PlayerId].ChekersList)
            {
                mustJumpOver |= m_Board[checkerId].NeedToJumpOver();
            }

            return mustJumpOver;
        }

        private void removeCheckerFromOpponent(string i_StartCellId, string i_DestinationCellId, ePlayerId i_PlayerId)
        {
            string skippedCellId = getSkippedCell(i_StartCellId, i_DestinationCellId);

            m_Players[(int)getOpponentId(i_PlayerId)].ChekersList.Remove(skippedCellId);
            m_Board.MakeCellEmpty(m_Board[skippedCellId]);
        }

        private string getSkippedCell(string i_StartCellID, string i_DestinationCellId)
        {
            int skippedRow, skippedCol;
            int diffRow = m_Board[i_DestinationCellId].Row - m_Board[i_StartCellID].Row;
            int diffCol = m_Board[i_DestinationCellId].Col - m_Board[i_StartCellID].Col;

            skippedRow = (diffRow / 2) + m_Board[i_StartCellID].Row;
            skippedCol = (diffCol / 2) + m_Board[i_StartCellID].Col;

            return m_Board[skippedRow, skippedCol].Id;
        }

        private void updateOptionsOnBoard()
        {
            updatePlayerOptions(FirstPlayerId);
            updatePlayerOptions(SecondPlayerId);
        }

        private void updatePlayerOptions(ePlayerId i_PlayerId)
        {
            foreach (string optionId in m_Players[(int)i_PlayerId].ChekersList)
            {
                m_Board.FindOptions(m_Board[optionId]);
            }
        }

        public bool PlayerCanPlay(ePlayerId i_PlayerId)
        {
            bool hasMoves = false;

            foreach (string checkerId in m_Players[(int)i_PlayerId].ChekersList)
            {
                hasMoves |= m_Board[checkerId].OptionsList.Count != k_Empty;
            }

            return hasMoves;
        }

        public string UpdateScoreAndWinner()
        {
            string winnerName = string.Empty;
            int    firstPlayerValue = calculatePlayerValue(FirstPlayerId);
            int    secondPlayerValue = calculatePlayerValue(SecondPlayerId);
            bool   firstPlayerCanPlay = PlayerCanPlay(FirstPlayerId);
            bool   secondPlayerCanPlay = PlayerCanPlay(SecondPlayerId);

            if (!firstPlayerCanPlay)
            {
                if (secondPlayerCanPlay)
                {
                    m_Players[(int)SecondPlayerId].Score += secondPlayerValue - firstPlayerValue;
                    winnerName = SecondPlayerName;
                }
            }
            else if (!secondPlayerCanPlay || firstPlayerValue > secondPlayerValue)
            {
                m_Players[(int)FirstPlayerId].Score += firstPlayerValue - secondPlayerValue;
                winnerName = FirstPlayerName;
            }
            else
            {
                m_Players[(int)SecondPlayerId].Score += secondPlayerValue - firstPlayerValue;
                winnerName = SecondPlayerName;
            }

            return winnerName;
        }

        private int calculatePlayerValue(ePlayerId i_PlayerId)
        {
            int value = 0;

            foreach (string checkerId in m_Players[(int)i_PlayerId].ChekersList)
            {
                if (m_Board[checkerId].Direction == BoardCell.eDirection.UpAndDown)
                {
                    value += k_KingValue;
                }
                else
                {
                    value += k_CheckerValue;
                }
            }

            return value;
        }
    }
}