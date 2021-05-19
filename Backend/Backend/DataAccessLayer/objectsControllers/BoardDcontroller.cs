using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using IntroSE.Kanban.Backend.DataAccessLayer.ObjectsD;
namespace IntroSE.Kanban.Backend.DataAccessLayer.objectsControllers
{
    class BoardDcontroller : DalController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string BoardTableName = "Board";

        private const string HostBoardsTableName = "HostBoards";
        public BoardDcontroller() : base(BoardTableName)
        {
        }

        //inserts new board data to the board's table in the dataBase
        public long Insert(BoardD D)
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
                    command.CommandText = $"INSERT INTO {BoardTableName} ({BoardD.EmailColumnName}) " +
                                      $"VALUES (@EmailVal);";

                    SQLiteParameter emailParam = new SQLiteParameter(@"EmailVal", D.Email);
                    command.Parameters.Add(emailParam);
                    command.Prepare();
                    command.ExecuteNonQuery();
                    res = IDofTheLastRow();
                    
                }
                //Exception cases handeler
                catch(Exception ex)
                {
                    log.Error("insertion faild:" + ex.Message);
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

        public List<BoardD> SelectAllBoards()
        {
            List<BoardD> result = Select().Cast<BoardD>().ToList();
            return result;
        }
        public long InsertToHosts(string BoardId, string email)
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
                    command.CommandText = $"INSERT INTO {HostBoardsTableName} ({BoardD.IDColumnName},{BoardD.EmailColumnName}) " +
                                      $"VALUES (@Id,@EmailVal);";


                    SQLiteParameter IdParam = new SQLiteParameter(@"Id", BoardId);
                    SQLiteParameter emailParam = new SQLiteParameter(@"EmailVal", email);
                    command.Parameters.Add(IdParam);
                    command.Parameters.Add(emailParam);
                    command.Prepare();
                    res = command.ExecuteNonQuery();
                }
                //Exception cases handeler
                catch (Exception ex)
                {
                    log.Error("insertion faild:" + ex.Message);
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

        public List<string> getBoardEmails(long Id)
        {
            List<string> results = new List<string>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"SELECT * FROM {HostBoardsTableName} WHERE {BoardD.IDColumnName}={Id};";
                SQLiteDataReader dataReader = null;
                try
                {
                    connection.Open();
                    dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {

                        results.Add(dataReader.GetString(1));

                    }
                }
                catch (Exception e)
                {
                    log.Error("selecting faild" + e.Message);
                }
                finally
                {
                    if (dataReader != null)
                    {
                        dataReader.Close();
                    }

                    command.Dispose();
                    connection.Close();
                }

            }
            return results;
        }



        //converts given data from the dataBase to dataBase object
        protected override DalObject ConvertReaderToObject(SQLiteDataReader reader)
        {
            return new BoardD((long)reader.GetValue(0), (string)reader.GetString(1));
        }

        //Loads the tables of board's tables in the dataBase
        internal List<BoardD> LoadAllBoards()
        {
            return Select().Cast<BoardD>().ToList();
        }
        
    }
}
