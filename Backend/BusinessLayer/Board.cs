using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.ServiceLayer;
using log4net;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    internal class Board
    {
        private column[] arrayofcol;
        private string id;//id of board
        private string name;//name of board
        private string creatorEmail;
        private int finishId; // unique id to column
        private int IsDone = 2;//isdone =2
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        internal Board(string name, string email)
        {
            isValidNameAndEmail(name, email);

            this.name = name;
            creatorEmail = email;
            arrayofcol = new column[3];
            arrayofcol[0] = new column(0, id );           
            arrayofcol[1] = new column(1, id);           
            arrayofcol[2] = new column(2, id);        

            this.id = name + "@" + email;
            finishId = 0;
        }
        public int GetfinishId()
        {
            return finishId;
        }
        public void advancefinishId()
        {
            finishId = finishId + 1;
        }

        internal column GetColumn(int columnOrdinal)
        {
            if (columnOrdinal < 0 | columnOrdinal > IsDone)
            {
                log.Debug("columnOrdinal is ilegal");
                throw new Exception("columnOrdinal is ilegal");
            }
            return arrayofcol[columnOrdinal];
        }

        private void isValidAdvanceTask(int columnOrdinal, int taskId)
        {
            if (columnOrdinal < 0 | columnOrdinal > IsDone)
            {
                log.Debug("columnOrdinal is ilegal");
                throw new Exception("columnOrdinal is ilegal");
            }
            if (columnOrdinal == IsDone)
            {
                log.Debug("cant advance task that alredy done");
                throw new Exception("cant advance task that alredy done");
            }
            if (!arrayofcol[columnOrdinal + 1].isNotFull())
            {
                log.Debug("cant advance task, the next column is full");
                throw new Exception("cant advance task, the next column is full");
            }
        }

        private void isValidNameAndEmail(string name, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new Exception("IsNullOrWhiteSpace(email)");
            }
            if (name is null /*name==null*/)
            {
                log.Debug("board name is null");
                throw new Exception("board name is null");
            }
            if (name.Length == 0)
            {
                log.Debug("the length of board name is zero");
                throw new Exception("the length of board name is zero");
            }
        }
        internal void AdvanceTask(int columnOrdinal, int taskId)
        {
            GetColumn(columnOrdinal).getTask(taskId);
            isValidAdvanceTask(columnOrdinal, taskId);
            arrayofcol[columnOrdinal + 1].addTask(arrayofcol[columnOrdinal].getTask(taskId));
            arrayofcol[columnOrdinal].removeTask(taskId);

        }








    }



}

