using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.ServiceLayer;
using log4net;
using IntroSE.Kanban.Backend.DataAccessLayer;
namespace IntroSE.Kanban.Backend.BusinessLayer
{
    internal class Board
    {//fields
        private column[] arrayofcol;
        private string id;//id of board
        private string name;//name of board
        private string creatorEmail;
        private int finishId; // unique id to column
        private int IsDone = 2;//isdone =2
        private BoardD boardD;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string Name { get => name; set => name = value; }
        public string CreatorEmail { get => creatorEmail; set => creatorEmail = value; }
        public string Id { get => id; set => id = value; }

        //constrcutor
        internal Board(string name, string email)
        {
            isValidNameAndEmail(name, email);

            this.Name = name;
            CreatorEmail = email;
            this.boardD = new BoardD(email);
            boardD.save();
           // this.id = boardD.Id.ToString();//check that majd.
            arrayofcol = new column[3];
            arrayofcol[0] = new column(0, "backlog");           
            arrayofcol[1] = new column(1, "in progress");           
            arrayofcol[2] = new column(2, "done");        

            this.Id = name + "@" + email;
            finishId = 0;
        }
       //cotructor to boardD
        internal Board(BoardD BD)
        {
            arrayofcol = new column[3];
            this.CreatorEmail = BD.Email;
            this.Id = BD.Id.ToString();//we have to check that..
            this.boardD = BD;
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
/*
//test
//i should replace the console to class library***
namespace hello_world
{
    using IntroSE.Kanban.Backend.BusinessLayer;
    class program
    {
        static void Main(string [] args)
        {
            Console.WriteLine("1");
            Board b = new Board("michel", "mich@hotmail.com");
            Console.WriteLine("the name is: " + b.Name +"  the email is :"+ b.CreatorEmail);
            DateTime dateTime = new DateTime(2013, 2, 12);
            Task task = new Task(dateTime, "hiii", "majd boklo", "in progress", 2);

            Console.WriteLine("the datetime is : " + task.DueDate + "  title is   :" + task.Title+ "the description is :"+task.Description+ "the boardId IS :"+ task.BoardId+ "THE TASK id:  "+task.TaskId);
            
        }
    }
}
*/

