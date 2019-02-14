using MySql.Data.MySqlClient;
using System;
using System.Text;
using System.Windows.Forms;

namespace Train
{
    //Авторизация
    class Authorization
    {
        private static string login = string.Empty;
        private static string password = string.Empty;
        private static bool success = false;
        private string connStr = string.Empty;
        private int priv = 2;

        public Authorization(string _login, string _password, string _connStr)
        {
            login = _login;
            password = _password;
            connStr = _connStr;
        }

        //Чекаем юзера
        public void Check()
        {
            using (MySqlConnection con = new MySqlConnection(connStr))
            {
                try
                {
                    con.Open();
                    string sql = "Select pass, priv From users Where login = @login";
                    MySqlCommand cmd = new MySqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@login", login);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string p = reader[0].ToString();
                            priv = Convert.ToInt32(reader[1]);
                            if (password == p)
                            {
                                success = true;
                            }
                            else
                            {
                                MessageBox.Show("Неверный пароль!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                success = false;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Пользователя с таким именем не т в бд", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        success = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    success = false;
                }
            }
        }

        //Возврат статуса
        public bool GetStatus()
        {
            return success;
        }
        //Возврат роли
        public int GetRole()
        {
            return priv;
        }
    }
}
