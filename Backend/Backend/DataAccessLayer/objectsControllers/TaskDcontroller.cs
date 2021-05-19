using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using IntroSE.Kanban.Backend.DataAccessLayer.ObjectsD;

namespace IntroSE.Kanban.Backend.DataAccessLayer.objectsControllers
{
    
    class TaskDcontroller : DalController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string TaskTableName = "Task";
        public TaskDcontroller() : base(TaskTableName)
        {
            
        }

        // //converts given data from the dataBase to dataBase object
        protected override DalObject ConvertReaderToObject(SQLiteDataReader reader)
        {
            return new TaskD((long)reader.GetValue(0), (long)reader.GetValue(1),(string)reader.GetString(2), (string)reader.GetString(3), (string)reader.GetString(4), (string)reader.GetString(5), (string)reader.GetString(6));
        }

        //inserts new board data to the board's table in the dataBase
        public long Insert(TaskD D)
        {
            long res = -1;
            //lets us hand a link (pointer) to the dataBase
            using (var connection = new SQLiteConnection(_connectionString))
            {
                //lets us send requests for changes to the dataBase
                SQLiteCommand command = new SQLiteCommand(null, connection);
                SQLiteCommand commandLastId = new SQLiteCommand(null, connection);
                try
                {
                    //start connection with dataBase
                    connection.Open();
                    //request to send
                    command.CommandText = $"INSERT INTO {TaskTableName} ({TaskD.ColumnIDColumnName},{TaskD.CreationDateColumnName},{TaskD.DueDateColumnName},{TaskD.TitleColumnName},{TaskD.DescriptionColumnName},{TaskD.AssigneeColumnName}) " +
                                      $"VALUES (@ColumnIdVal,@creationDateVal,@DueDateVal,@TitleVal,@DescriptionVal,@Assignee);";

                    SQLiteParameter ColumnIdParam = new SQLiteParameter(@"ColumnIdVal", D.ColumnId);
                    SQLiteParameter CreationDateParam = new SQLiteParameter(@"creationDateVal", D.CreationDate);
                    SQLiteParameter DueDateParam = new SQLiteParameter(@"DueDateVal", D.DueDate);
                    SQLiteParameter TitleParam = new SQLiteParameter(@"TitleVal", D.Title);
                    SQLiteParameter DescriptionParam = new SQLiteParameter(@"DescriptionVal", D.Description);
                    SQLiteParameter AssigneeParam = new SQLiteParameter(@"Assignee", D.Assignee);
                    command.Parameters.Add(ColumnIdParam);
                    command.Parameters.Add(CreationDateParam);
                    command.Parameters.Add(DueDateParam);
                    command.Parameters.Add(TitleParam);
                    command.Parameters.Add(DescriptionParam);
                    command.Parameters.Add(AssigneeParam);
                    command.Prepare();
                    command.ExecuteNonQuery();
                    res = IDofTheLastRow();
                }
                //Exception cases handeler
                catch (Exception ex)
                {
                    log.Error("insertion faild" + ex.Message);
                }
                //ends the connection with the dataBase
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
            return res;
        }

        //Loads the tables of task's tables from the dataBase
        internal List<TaskD> LoadTasks(long ColumnID)
        {
            return SelectByID(ColumnID,TaskD.ColumnIDColumnName).Cast<TaskD>().ToList();
        }

    }
}
