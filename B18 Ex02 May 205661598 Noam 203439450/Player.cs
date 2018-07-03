using System.Collections.Generic;

namespace B18_Ex02
{
    public class Player
    {
        // $G$ DSN-999 (-3) m_PlayerCheckers should be readonly.
        private string         m_Name;
        private int            m_Score;
        private List<string>   m_PlayerCheckers = null;
        private Game.ePlayerId m_Id;

        public Player(string i_Name, Game.ePlayerId i_PlayerSerial)
        {
            m_Name = i_Name;
            m_Id = i_PlayerSerial;
            m_PlayerCheckers = new List<string>();
        }

        public int Score
        {
            get { return m_Score; }
            set { m_Score = value; }
        }

        public string Name
        {
            get { return m_Name; }
        }

        public List<string> ChekersList
        {
            get { return m_PlayerCheckers; }
        }

        public Game.ePlayerId Id
        {
            get { return m_Id; }
        }

        public void AddChecker(string i_CellId)
        {
            m_PlayerCheckers.Add(i_CellId);
        }

        public void RemoveChecker(string i_CellId)
        {
            m_PlayerCheckers.Remove(i_CellId);
        }

        public void Move(string i_CurrentId, string i_NextId)
        {
            RemoveChecker(i_CurrentId);
            AddChecker(i_NextId);
        }

        public bool IsMyChecker(string i_CheckerId)
        {
            return m_PlayerCheckers.Contains(i_CheckerId);
        }

        public void ClearPlayerChekers()
        {
            m_PlayerCheckers.Clear();
        }
    }
}
