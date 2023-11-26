using MySql.Data.MySqlClient;

namespace Catalogo.DbContexts
{
    public class ContextBase
    {
        public string ConnectionString { get; set; }
        public MySqlTransaction Transaction { get; set; }
        public MySqlConnection Connection { get; set; }
        private string DbName { get; set; }

        public ContextBase() { }

        public ContextBase(string connectionString)
        {
            ConnectionString = connectionString;

        }

        public void GetConnection()
        {
            Connection = new MySqlConnection(ConnectionString);
            if (Connection != null)
                DbName = Connection.Database;
        }

        public void GetConnectionTransaction()
        {
            Connection = new MySqlConnection(ConnectionString);
            DbName = Connection.Database;
            Open();
            Transaction = Connection.BeginTransaction();
        }

        public void Commit()
        {
            if (Transaction != null)
                Transaction.Commit();

            Close();
        }

        public void Rollback()
        {
            if (Transaction != null && Transaction.Connection.State.Equals(System.Data.ConnectionState.Open))
                Transaction.Rollback();

            Close();
        }

        public void Open()
        {
            if (Connection != null)
                Connection.Open();
        }

        public void Close()
        {
            if (Connection != null && Connection.State.Equals(System.Data.ConnectionState.Open))
                Connection.Close();
        }

        public string GetNameDB()
        {
            return DbName;
        }
    }
}
