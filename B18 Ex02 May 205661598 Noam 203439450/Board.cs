using System.Collections.Generic;

namespace B18_Ex02
{
    public class Board
    {
        internal const char  k_FirstRowId = 'a';
        internal const char  k_FirstColId = 'A';
        internal const int   k_Row = 1;
        internal const int   k_Col = 0;
        internal const int   k_ToRow = 4;
        internal const int   k_ToCol = 3;
        internal const int   k_FirstRow = 0;
        internal const int   k_Down = 1;
        internal const int   k_Up = -1;
        internal const int   k_Left = -1;
        internal const int   k_Right = 1;

        private BoardCell[,] m_Board;

        public Board(int i_BoardSize)
        {
            m_Board = new BoardCell[i_BoardSize, i_BoardSize];
        }

        public BoardCell this[string i_CellId]
        {
            get
            {
                int row = i_CellId[k_Row] - k_FirstRowId;
                int col = i_CellId[k_Col] - k_FirstColId;

                return m_Board[row, col];
            }
        }

        public BoardCell this[int i_Row, int i_Col]
        {
            get { return m_Board[i_Row, i_Col]; }
            set { m_Board[i_Row, i_Col] = value; }
        }

        public int BoardSize
        {
            get { return m_Board.GetLength(k_FirstRow); }
        }

        public bool IsInBoardBorders(string i_CellId)
        {
            bool isInBorders = false;

            if (i_CellId[k_Col] >= k_FirstColId && i_CellId[k_Col] < k_FirstColId + BoardSize)
            {
                isInBorders = i_CellId[k_Row] >= k_FirstRowId && i_CellId[k_Row] < k_FirstRowId + BoardSize;
            }

            return isInBorders;
        }

        private bool needToBeKing(string i_CellId)
        {
            return i_CellId[k_Row] == k_FirstRowId || i_CellId[k_Row] == k_FirstRowId + BoardSize - 1;
        }

        public void MakeCellEmpty(BoardCell i_CellToDelete)
        {
            i_CellToDelete.Sign = BoardCell.eSigns.Empty;
            i_CellToDelete.Direction = BoardCell.eDirection.None;
            i_CellToDelete.OwnerId = Game.ePlayerId.None;
            i_CellToDelete.OptionsList.Clear();
        }

        public void MoveCheckerOnBoard(string i_CurrentCellId, string i_NextCellId)
        {
            if (needToBeKing(i_NextCellId))
            {
                this[i_NextCellId].Sign = BoardCell.GetKingSignFromPlayerId(this[i_CurrentCellId].OwnerId);
                this[i_NextCellId].Direction = BoardCell.eDirection.UpAndDown;
            }
            else
            {
                this[i_NextCellId].Sign = this[i_CurrentCellId].Sign;
                this[i_NextCellId].Direction = this[i_CurrentCellId].Direction;
            }

            this[i_NextCellId].OwnerId = this[i_CurrentCellId].OwnerId;
            if (this[i_NextCellId].OptionsList == null)
            {
                this[i_NextCellId].OptionsList = new Dictionary<string, Game.eMoveType>();
            }

            MakeCellEmpty(this[i_CurrentCellId]);
        }

        public void FindOptions(BoardCell i_Cell)
        {
            BoardCell.eDirection cellDirection = i_Cell.Direction;

            i_Cell.OptionsList.Clear();
            if (cellDirection == BoardCell.eDirection.Down || cellDirection == BoardCell.eDirection.UpAndDown)
            {
                checkNextDiagonalCell(i_Cell, k_Down, k_Left);
                checkNextDiagonalCell(i_Cell, k_Down, k_Right);
            }

            if (cellDirection == BoardCell.eDirection.Up || cellDirection == BoardCell.eDirection.UpAndDown)
            {
                checkNextDiagonalCell(i_Cell, k_Up, k_Left);
                checkNextDiagonalCell(i_Cell, k_Up, k_Right);
            }
        }

        private void checkNextDiagonalCell(BoardCell i_Cell, int i_IndentationRow, int i_IndentationCol)
        {
            char   nextCellColLetter = (char)(i_Cell.Id[k_Col] + i_IndentationCol);
            char   nextCellRowLetter = (char)(i_Cell.Id[k_Row] + i_IndentationRow);
            string nexCellId = Game.GetCellIdString(nextCellColLetter, nextCellRowLetter);

            if (IsInBoardBorders(nexCellId))
            {
                if (!tryUpdateCellOption(i_Cell, nexCellId, Game.eMoveType.SimpleMove) && isOpponentCell(i_Cell, this[nexCellId]))
                {
                    nextCellColLetter = (char)(i_Cell.Id[k_Col] + (i_IndentationCol * 2));
                    nextCellRowLetter = (char)(i_Cell.Id[k_Row] + (i_IndentationRow * 2));
                    nexCellId = Game.GetCellIdString(nextCellColLetter, nextCellRowLetter);
                    if (IsInBoardBorders(nexCellId))
                    {
                        tryUpdateCellOption(i_Cell, nexCellId, Game.eMoveType.JumpOver);
                    }
                }
            }
        }

        private bool tryUpdateCellOption(BoardCell i_Cell, string i_OptionalCellId, Game.eMoveType i_MoveType)
        {
            bool isUpdateSuccessed = false;

            if (this[i_OptionalCellId].Sign == BoardCell.eSigns.Empty)
            {
                if (i_Cell.OptionsList == null)
                {
                    i_Cell.OptionsList = new Dictionary<string, Game.eMoveType>();
                }

                i_Cell.OptionsList.Add(i_OptionalCellId, i_MoveType);
                isUpdateSuccessed = true;
            }

            return isUpdateSuccessed;
        }

        private bool isOpponentCell(BoardCell i_StartCell, BoardCell i_DestinationCell)
        {
            bool isOpponentCell = i_StartCell.OwnerId != i_DestinationCell.OwnerId;

            return isOpponentCell && i_DestinationCell.OwnerId != Game.ePlayerId.None;
        }
    }
}
