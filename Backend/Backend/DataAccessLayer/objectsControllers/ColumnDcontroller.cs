using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using IntroSE.Kanban.Backend.DataAccessLayer.ObjectsD;

namespace IntroSE.Kanban.Backend.DataAccessLayer.objectsControllers
{
    class ColumnDcontroller:DalController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string columnTableName = "Column";
        public ColumnDcontroller() : base(columnTableName)
        {
        }

        ////converts given data from the dataBase to dataBase object
        protected override DalObject ConvertReaderToObject(SQLiteDataReader reader)
        {
            
            return new ColumnD((long)reader.GetValue(0), (long)reader.GetValue(1), (long)reader.GetValue(2), (string)reader.GetString(3), (long)reader.GetValue(4));
        }

        ////inserts new column data to the column's table in the dataBase
        public long Insert(ColumnD D)
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
                    command.CommandText = $"INSERT INTO {columnTableName} ({ColumnD.BoardIDColumnName},{ColumnD.ColumnOrdinalColumnName},{ColumnD.NameColumnName},{ColumnD.LimitColumnName}) " +
                                      $"VALUES (@BoardIdVal,@columnOrdinalVal,@NameVal,@LimitVal);";

                    SQLiteParameter BoardIdParam = new SQLiteParameter(@"BoardIdVal", D.BoardID);
                    SQLiteParameter OrdinalParam = new SQLiteParameter(@"columnOrdinalVal", D.ColumnOrdinal);
                    SQLiteParameter NameParam = new SQLiteParameter(@"NameVal", D.Name);
                    SQLiteParameter LimitParam = new SQLiteParameter(@"LimitVal", D.Limit);


                    command.Parameters.Add(BoardIdParam);
                    command.Parameters.Add(OrdinalParam);
                    command.Parameters.Add(NameParam);
                    command.Parameters.Add(LimitParam);
                    command.Prepare();
                    command.ExecuteNonQuery();
                    res = IDofTheLastRow();
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

        //Loads the tables of column's tables in the dataBase
        internal List<ColumnD> LoadColumns(long BoardID)
        {
            return SelectByID(BoardID, ColumnD.BoardIDColumnName).Cast<ColumnD>().ToList();
        }
    }
}
