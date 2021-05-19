using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using IntroSE.Kanban.Backend.DataAccessLayer.ObjectsD;
namespace IntroSE.Kanban.Backend.DataAccessLayer
{
   internal abstract class DalController 
    {
        //fields 
        //tables in database///
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string BoardTableName = "Board";
        private const string columnTableName = "Column";
        private const string TaskTableName = "Task";
        private const string UserTableName = "User";
        private const string HostBoardsTableName = "HostBoards";
        private const string IDColumnName = "Id";
        private const string AutoIncrementTable = "sequence";
        protected readonly string _connectionString;
        private readonly string _tableName;

        //Constructor
        public DalController(string tableName)//gave name if table
        {

              string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "kanban.db"));//find the path
            //string path = "database.db";
            this._connectionString = $"Data Source={path};Version=3;";//connection with database with virsion 3
            this._tableName = tableName;
            if (!File.Exists(path)) this.openDB();//if not exist we will open this database.

        }

        // Updates the value in a given attributeName column
        public bool Update(long id, string attributeName, string attributeValue)
        {
            int res = -1;//how many rows we have.
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand//new command
                {
                    Connection = connection,
                    CommandText = $"UPDATE {this._tableName} SET[{attributeName}]=(@attribute) WHERE {DalObject.IDColumnName}={id}"
                };
                try
                {

                    command.Parameters.Add(new SQLiteParameter(@"attribute", attributeValue));
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    log.Error("updating faild" + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }
            return res > 0;
        }

        // Updates the value in a given attributeName column
        public bool Update(long id, string attributeName, long attributeValue)
        {

            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"UPDATE {_tableName} SET[{attributeName}]=(@attribute) WHERE {DalObject.IDColumnName}={id}"
                };
                try
                {

                    command.Parameters.Add(new SQLiteParameter(@"attribute", attributeValue));
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    log.Error("updating faild" + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();

                }

            }
            return res > 0;
        }

        protected List<DalObject> Select()
        {

            List<DalObject> results = new List<DalObject>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"SELECT * FROM {_tableName};";
                SQLiteDataReader dataReader = null;
                try
                {

                    connection.Open();
                    dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        results.Add(ConvertReaderToObject(dataReader));
                    }

                }
                catch (Exception e)
                {
                    log.Error(e.Message);
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


        //Converts the reader value to a dalObject
        protected abstract DalObject ConvertReaderToObject(SQLiteDataReader reader);






        //Deletes the given object
        public bool Delete(DalObject D)
        {
            int res = -1;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"DELETE FROM {_tableName} WHERE {DalObject.IDColumnName}={D.Id}"
                };
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    log.Error("converting faild" + ex.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }
            return res > 0;
        }

        // Delets all the stored data
        public bool DeleteAll()
        {
            int res = -1;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"DELETE FROM {UserTableName};"
                    + $"DELETE FROM {BoardTableName};"
                    + $"DELETE FROM {columnTableName};"
                    + $"DELETE FROM {TaskTableName};"
                    + $"DELETE FROM {HostBoardsTableName};"
                };
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    log.Error("deleting faild" + ex.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }
            return res > 0;
        }

        // returns the id of the object
        protected long IDofTheLastRow()
        {
            long res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"SELECT * FROM {AutoIncrementTable};";
                SQLiteDataReader dataReader = null;
                try
                {

                    connection.Open();

                    dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        if (dataReader.GetString(0).Equals(_tableName))
                            res = dataReader.GetInt64(1);

                    }

                }
                catch (Exception e)
                {
                    log.Error(e.Message);

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
            return res;
        }

        // select objects by given id
        protected List<DalObject> SelectByID(long Id, string ColumnName)
        {
            List<DalObject> results = new List<DalObject>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"SELECT * FROM {_tableName} WHERE {ColumnName}={Id};";
                SQLiteDataReader dataReader = null;
                try
                {
                    connection.Open();
                    dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {

                        results.Add(ConvertReaderToObject(dataReader));

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

        //Create the database file with new tables
        private void openDB()
        {
            SQLiteConnection.CreateFile("kanban.db");
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"CREATE TABLE {BoardTableName} ({IDColumnName} INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, {BoardD.EmailColumnName} TEXT NOT NULL UNIQUE);" +
                    $"CREATE TABLE {columnTableName} ({IDColumnName} INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, {ColumnD.BoardIDColumnName}	INTEGER NOT NULL, {ColumnD.ColumnOrdinalColumnName}   INTEGER NOT NULL,   {ColumnD.NameColumnName}  INTEGER NOT NULL,   {ColumnD.LimitColumnName } INTEGER NOT NULL);" +
                    $"CREATE TABLE {TaskTableName} ({IDColumnName} INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,{TaskD.ColumnIDColumnName}	INTEGER NOT NULL,  {TaskD.CreationDateColumnName}  TEXT NOT NULL,{TaskD.DueDateColumnName}  TEXT NOT NULL, {TaskD.TitleColumnName} TEXT NOT NULL,{TaskD.DescriptionColumnName}   TEXT, {TaskD.AssigneeColumnName} TEXT NOT NULL);" +
                    $"CREATE TABLE {UserTableName} ({IDColumnName} INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, {UserD.EmailColumnName} TEXT NOT NULL UNIQUE, {UserD.PasswordColumnName}  TEXT NOT NULL);" +
                    $"CREATE TABLE {HostBoardsTableName} ({IDColumnName} INTEGER NOT NULL, {BoardD.EmailColumnName} TEXT NOT NULL UNIQUE);"
                };
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    log.Error("creating file faild" + ex.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }

        }


    }
}
