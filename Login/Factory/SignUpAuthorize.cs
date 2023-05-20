using System.Data.SQLite;

namespace Login.Factory;

public class SignUpAuthorize {
    private Login.Models.SignupModel s;
    public SignUpAuthorize(Login.Models.SignupModel data) {
        s = data;
    }
    public string DatabaseInsert() {
        Console.WriteLine("Insert into DB.");
        string hashedPassword = SHA256Hash.GetHashString(s.password);
        string response = " ";
        var connectionString = "data source='./Factory/DataStorage/database.db'; New=False";
        var connection = new SQLiteConnection(connectionString);
        Console.WriteLine("Opening Connecrion");
        connection.Open();
        Console.WriteLine("Opened Connection");
        using (var cmd = new SQLiteCommand(@$"SELECT COUNT(username) FROM usernames WHERE username = ""{s.username}""", connection)) { 
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            if (count > 0) {return "DataError_AlreadyUser";}
        }
        using (var transaction = connection.BeginTransaction())
        {
            bool transactionSucceeded = true;
            try
            {
                string? affectedId = "";
                using (var cmd = new SQLiteCommand(@$"INSERT INTO usernames (username) VALUES (""{s.username}"")",                                    connection, transaction)) { cmd.ExecuteNonQuery(); }
                using (var cmd = new SQLiteCommand(@$"SELECT id FROM usernames WHERE username = ""{s.username}""",                                    connection, transaction)) { affectedId = cmd.ExecuteScalar().ToString(); }
                using (var cmd = new SQLiteCommand(@$"INSERT INTO userRoleAssignment (usernames_id, role_id) VALUES (""{affectedId}"", ""1"")",                             connection, transaction)) { cmd.ExecuteNonQuery(); }
                using (var cmd = new SQLiteCommand(@$"INSERT INTO password (usernames_id, password) VALUES (""{affectedId}"", ""{hashedPassword}"")", connection, transaction)) { cmd.ExecuteNonQuery(); }
                using (var cmd = new SQLiteCommand(@$"INSERT INTO userinfo (usernames_id, name, email, address, phone_no) VALUES (""{affectedId}"", ""{s.name}"", ""{s.email}"", ""{s.address}"", ""{s.phone}"")", connection, transaction)) { cmd.ExecuteNonQuery(); }
            }
            catch (Exception e)
            {
                transactionSucceeded = false;
                Console.WriteLine(e);
            }
            if (transactionSucceeded)
            {
                transaction.Commit();
                response = "Ok";
            }
            else
            {
                transaction.Rollback();
                response = "DataError_UserCreation";
            }
        }
        connection.Close();
        return response;
    }
}

