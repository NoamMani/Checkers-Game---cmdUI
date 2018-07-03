using System.Collections.Generic;

namespace B18_Ex02
{
    public class BoardCell
    {
        public enum eSigns
        {
            Empty = ' ',
            FirstPlayerSign = 'X',
            FirstPlayerKingSign = 'K',
            SecondPlayerSign = 'O',
            SecondPlayerKingSign = 'U',
            SepratorSign = '=',
        }

        public enum eDirection
        {
            Up,
            Down,
            UpAndDown,
            None,
        }

        private string                              m_Identifier = string.Empty;
        private Dictionary<string, Game.eMoveType>  m_MovingOptions = null;
        private eSigns                              m_Sign;
        private eDirection                          m_Direction = eDirection.None;
        private Game.ePlayerId                      m_OwnerId = Game.ePlayerId.None;

        public static eSigns GetSignFromPlayerId(Game.ePlayerId i_PlayerId)
        {
            eSigns sign;

            if (i_PlayerId == Game.ePlayerId.None)
            {
                sign = eSigns.Empty;
            }
            else
            {
                sign = i_PlayerId == Game.ePlayerId.Player1 ? eSigns.FirstPlayerSign : eSigns.SecondPlayerSign;
            }

            return sign;
        }

        public static eSigns GetKingSignFromPlayerId(Game.ePlayerId i_PlayerId)
        {
            return i_PlayerId == Game.ePlayerId.Player1 ? eSigns.FirstPlayerKingSign : eSigns.SecondPlayerKingSign;
        }

        public eSigns Sign
        {
            get { return m_Sign; }
            set { m_Sign = value; }
        }

        public eDirection Direction
        {
            get { return m_Direction; }
            set { m_Direction = value; }
        }

        public Dictionary<string, Game.eMoveType> OptionsList
        {
            get { return m_MovingOptions; }
            set { m_MovingOptions = value; }
        }

        public string Id
        {
            get { return m_Identifier; }
        }

        public int Row
        {
            get { return m_Identifier[Board.k_Row] - Board.k_FirstRowId; }
        }

        public int Col
        {
            get { return m_Identifier[Board.k_Col] - Board.k_FirstColId; }
        }

        public Game.ePlayerId OwnerId
        {
            get { return m_OwnerId; }
            set { m_OwnerId = value; }
        }

        public bool NeedToJumpOver()
        {
            return m_MovingOptions.ContainsValue(Game.eMoveType.JumpOver);
        }

        public bool IsPossiableMove(string i_NextPosId)
        {
            return m_MovingOptions != null ? m_MovingOptions.ContainsKey(i_NextPosId) : false;
        }

        public void InitializeCell(int i_Row, int i_Col, Game.ePlayerId i_OwnerId)
        {
            m_OwnerId = i_OwnerId;
            m_Sign = GetSignFromPlayerId(i_OwnerId);
            m_Identifier = getCellIdByRowCol(i_Row, i_Col);
            if (m_Sign != eSigns.Empty)
            {
                if (m_Sign == eSigns.FirstPlayerSign)
                {
                    m_Direction = eDirection.Up;
                }
                else if (m_Sign == eSigns.SecondPlayerSign)
                {
                    m_Direction = eDirection.Down;
                }

                m_MovingOptions = new Dictionary<string, Game.eMoveType>();
            }
        }

        public void ClearList()
        {
            if (m_MovingOptions != null)
            {
                m_MovingOptions.Clear();
            }
        }

        private string getCellIdByRowCol(int i_Row, int i_Col)
        {
            return string.Format("{0}{1}", (char)(Board.k_FirstColId + i_Col), (char)(Board.k_FirstRowId + i_Row));
        }
    }
}
