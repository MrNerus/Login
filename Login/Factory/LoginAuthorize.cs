using System.Data.SQLite;

namespace Login.Factory;

public class LoginAuthorize {
    private string username = " ";
    private string password = " ";
    public string loginData(string uname, string pword)
    {
        username = uname;
        password = pword;
        return "Ok";
    }
    private string DatabaseCheck() {
        string response = " ";
        var connectionString = "data source='DataStorage/database.db'; New=False";
        var connection = new SQLiteConnection(connectionString);
        connection.Open();
        using (var cmd = new SQLiteCommand(@$"SELECT password.password FROM usernames INNER JOIN password ON usernames.id=password.usernames_id WHERE usernames.username=""{username}""", connection))
        {
            string hashedPassword = SHA256Hash.GetHashString(password);
            string? passwordOnDB = cmd.ExecuteScalar()?.ToString();
            if (passwordOnDB == hashedPassword)
            {
                response = "Ok";
            }
            else
            {
                response = "DataError_WrongPassword";
            }
        }
        connection.Close();
        return response;
    }
}

