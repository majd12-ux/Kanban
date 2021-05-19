using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using IntroSE.Kanban.Backend.DataAccessLayer.ObjectsD;

namespace IntroSE.Kanban.Backend.DataAccessLayer.objectsControllers
{

    class UserDcontroller : DalController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string UserTableName = "User";
        public UserDcontroller() : base(UserTableName)
        {
        }

        protected override DalObject ConvertReaderToObject(SQLiteDataReader reader)
        {
            return new UserD((long)reader.GetValue(0), (string)reader.GetString(1), (string)reader.GetString(2));
        }
        public long Insert(UserD D)
        {
            long res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                SQLiteCommand commandLastId = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {UserTableName} ({UserD.EmailColumnName},{UserD.PasswordColumnName}) " +
                    $"VALUES (@EmailVal,@PasswordVal);";

                    SQLiteParameter emailParam = new SQLiteParameter(@"EmailVal", D.Email);
                    SQLiteParameter PasswordParam = new SQLiteParameter(@"PasswordVal", D._password);
                    command.Parameters.Add(emailParam);
                    command.Parameters.Add(PasswordParam);
                    command.Prepare();
                    command.ExecuteNonQuery();
                    res = IDofTheLastRow();
                }
                catch(Exception ex)
                {
                    log.Error("insertion faild" + ex.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
            return res;
        }
        internal List<UserD> LoadAllUsers()
        {
            List<UserD> result = this.Select().Cast<UserD>().ToList();
            return result;
        }
    }
}
