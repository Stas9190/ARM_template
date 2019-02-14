using System;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;

namespace Train
{
    public partial class Main : Form
    {
        //Строка подключения
        const string connStr = "server=localhost; user=root; database=railway; password=;charset=cp1251;";
        //датасет
        static public DataSet ds = new DataSet();
        static public MySqlDataAdapter MySqlDataAdapter;
        //сорцы
        BindingSource trains = new BindingSource();
        BindingSource types = new BindingSource();
        BindingSource costs = new BindingSource();
        BindingSource schedule = new BindingSource();
        BindingSource schedule_info = new BindingSource();
        BindingSource passengers = new BindingSource();
        BindingSource stations = new BindingSource();
        BindingSource users = new BindingSource();
        BindingSource schedule_trains = new BindingSource();

        public Main()
        {
            InitializeComponent();
            wizardPages1.SelectedTab = Registration;
            AddTables();
            LoadGrids();
            LoadForms();
        }

        //Добавление таблиц в датасет
        private void AddTables()
        {
            ds.Tables.Add("Train");
            ds.Tables.Add("Type");
            ds.Tables.Add("Cost");
            ds.Tables.Add("Schedule");
            ds.Tables.Add("Schedule_info");
            ds.Tables.Add("Passenger");
            ds.Tables.Add("Stations");
            ds.Tables.Add("Users");
            ds.Tables.Add("Schedule_Trains");
        }

        //Привязка данных к гридам
        private void LoadGrids()
        {
            LoadPlanesToGrid();
            LoadTypesToGrid();
            LoadCostsToGrid();
            LoadScheduleToGrid();
            LoadScheduleInfoToGrid();
            LoadPassengersToGrid();
            LoadStationsToGrid();
            LoadUsersToGrid();
            LoadScheduleTrainsToGrid();
        }

        //Привязка данных к полям
        private void LoadForms()
        {
            LoadPlaneBindings();
            LoadTypesBindings();
            LoadCostsBindings();
            LoadScheduleBindings();
            LoadScheduleInfoBindings();
            LoadPassengersBindings();
            LoadStationsBindings();
            LoadUsersBindings();
            LoadScheduleTrainsBindings();
            LoadDataForTicket();
        }

        //При изменении выбора расписания
        private void comboBox_sch_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadStationsToGrid();
        }

        #region Привязка полей форрм
        private void LoadPlaneBindings()
        {
            bort_num.DataBindings.Add(new Binding("Text", trains, "number", false, DataSourceUpdateMode.Never));
            model.DataBindings.Add(new Binding("Text", trains, "name", false, DataSourceUpdateMode.Never));
            company.DataBindings.Add(new Binding("Text", trains, "company", false, DataSourceUpdateMode.Never));
            qty_places.DataBindings.Add(new Binding("Text", trains, "qty_places", false, DataSourceUpdateMode.Never));
        }
        private void LoadTypesBindings()
        {
            Type.DataBindings.Add(new Binding("Text", types, "name", false, DataSourceUpdateMode.Never));
        }
        private void LoadCostsBindings()
        {
            Price.DataBindings.Add(new Binding("Text", costs, "sum", false, DataSourceUpdateMode.Never));
            comboBox_train.DataSource = trains;
            comboBox_train.DisplayMember = "name";
            comboBox_train.ValueMember = "id";

            comboBox_type.DataSource = types;
            comboBox_type.DisplayMember = "name";
            comboBox_type.ValueMember = "id";
        }
        private void LoadScheduleBindings()
        {
            Schedule_name.DataBindings.Add(new Binding("Text", schedule, "name", false, DataSourceUpdateMode.Never));
        }
        private void LoadScheduleInfoBindings()
        {
            combobox_Schedule.DataSource = schedule;
            combobox_Schedule.DisplayMember = "name";
            combobox_Schedule.ValueMember = "id";

            date_start.DataBindings.Add(new Binding("Value", schedule_info, "date_start", false, DataSourceUpdateMode.Never));
            Periodicity.DataBindings.Add(new Binding("Text", schedule_info, "periodicity", false, DataSourceUpdateMode.Never));
        }
        private void LoadPassengersBindings()
        {
            DateOfBirth.DataBindings.Add(new Binding("Value", passengers, "date_of_birth", false, DataSourceUpdateMode.Never));
            FIO.DataBindings.Add(new Binding("Text", passengers, "fio", false, DataSourceUpdateMode.Never));
            Passport_num.DataBindings.Add(new Binding("Text", passengers, "passport_num", false, DataSourceUpdateMode.Never));
            Sex.DataBindings.Add(new Binding("Text", passengers, "sex", false, DataSourceUpdateMode.Never));
        }
        private void LoadStationsBindings()
        {
            Schedule_choice.DataSource = schedule;
            Schedule_choice.DisplayMember = "name";
            Schedule_choice.ValueMember = "id";
            Station.DataBindings.Add(new Binding("Text", stations, "station", false, DataSourceUpdateMode.Never));
            Arrival_time.DataBindings.Add(new Binding("Text", stations, "arrival_time", false, DataSourceUpdateMode.Never));
            Departure_time.DataBindings.Add(new Binding("Text", stations, "departure_time", false, DataSourceUpdateMode.Never));
            Parking_time.DataBindings.Add(new Binding("Text", stations, "parking_time", false, DataSourceUpdateMode.Never));
            Bias.DataBindings.Add(new Binding("Text", stations, "bias", false, DataSourceUpdateMode.Never));
        }
        private void LoadUsersBindings()
        {
            fio_user.DataBindings.Add(new Binding("Text", users, "fio", false, DataSourceUpdateMode.Never));
            login_user.DataBindings.Add(new Binding("Text", users, "login", false, DataSourceUpdateMode.Never));
            password_user.DataBindings.Add(new Binding("Text", users, "pass", false, DataSourceUpdateMode.Never));
            status_user.DataBindings.Add(new Binding("Text", users, "status", false, DataSourceUpdateMode.Never));
            priv_user.DataBindings.Add(new Binding("Text", users, "priv", false, DataSourceUpdateMode.Never));
            email_user.DataBindings.Add(new Binding("Text", users, "email", false, DataSourceUpdateMode.Never));
            tel_user.DataBindings.Add(new Binding("Text", users, "tel", false, DataSourceUpdateMode.Never));
        }
        private void LoadScheduleTrainsBindings()
        {
            comboBox_schedule_choice.DataSource = schedule;
            comboBox_schedule_choice.DisplayMember = "name";
            comboBox_schedule_choice.ValueMember = "id";

            comboBox_train_choice.DataSource = trains;
            comboBox_train_choice.DisplayMember = "name";
            comboBox_train_choice.ValueMember = "id";
        }
        private void LoadDataForTicket()
        {

        }
        #endregion

        #region Привязка к данным
        private void LoadPlanesToGrid()
        {
            using (MySqlConnection con = new MySqlConnection(connStr))
            {
                dataGridView1.DataSource = null;
                con.Open();
                ds.Tables["Train"].Clear();
                string sql = "Select id, number, name, company, qty_places From train Order By id";
                MySqlDataAdapter = new MySqlDataAdapter(sql, con);
                MySqlDataAdapter.Fill(ds, "Train");
                trains.DataSource = ds.Tables["Train"];
                dataGridView1.DataSource = trains;
                dataGridView1.Columns["id"].HeaderText = "#";
                dataGridView1.Columns["number"].HeaderText = "Номер";
                dataGridView1.Columns["name"].HeaderText = "Наименование";
                dataGridView1.Columns["company"].HeaderText = "Компания";
                dataGridView1.Columns["qty_places"].HeaderText = "Количество мест";
            }
        }
        private void LoadTypesToGrid()
        {
            using (MySqlConnection con = new MySqlConnection(connStr))
            {
                dataGridView2.DataSource = null;
                con.Open();
                ds.Tables["Type"].Clear();
                string sql = "Select id, name From type Order By id";
                MySqlDataAdapter = new MySqlDataAdapter(sql, con);
                MySqlDataAdapter.Fill(ds, "Type");
                types.DataSource = ds.Tables["Type"];
                dataGridView2.DataSource = types;
                dataGridView2.Columns["id"].HeaderText = "#";
                dataGridView2.Columns["name"].HeaderText = "Наименование";
            }
        }
        private void LoadCostsToGrid()
        {
            using (MySqlConnection con = new MySqlConnection(connStr))
            {
                dataGridView3.DataSource = null;
                con.Open();
                ds.Tables["Cost"].Clear();
                string sql = "Select c.id, c.id_train, tr.name tr_name, c.id_type, t.name t_name, c.sum From cost c" +
                    " Inner Join train tr on tr.id = c.id_train " +
                    " Inner Join type t on t.id = c.id_type " +
                    " Order By c.id";
                MySqlDataAdapter = new MySqlDataAdapter(sql, con);
                MySqlDataAdapter.Fill(ds, "Cost");
                costs.DataSource = ds.Tables["Cost"];
                dataGridView3.DataSource = costs;
                dataGridView3.Columns["id"].HeaderText = "#";
                dataGridView3.Columns["id_train"].HeaderText = "№ Поезда";
                dataGridView3.Columns["tr_name"].HeaderText = "Поезд";
                dataGridView3.Columns["id_type"].HeaderText = "№ Типа";
                dataGridView3.Columns["t_name"].HeaderText = "Тип";
                dataGridView3.Columns["sum"].HeaderText = "Цена";
            }
        }
        private void LoadScheduleToGrid()
        {
            using (MySqlConnection con = new MySqlConnection(connStr))
            {
                dataGridView4.DataSource = null;
                con.Open();
                ds.Tables["Schedule"].Clear();
                string sql = "Select id, name, introduction_date From schedule_prime";
                MySqlDataAdapter = new MySqlDataAdapter(sql, con);
                MySqlDataAdapter.Fill(ds, "Schedule");
                schedule.DataSource = ds.Tables["Schedule"];
                dataGridView4.DataSource = schedule;
                dataGridView4.Columns["id"].HeaderText = "#";
                dataGridView4.Columns["name"].HeaderText = "Наименование";
                dataGridView4.Columns["introduction_date"].HeaderText = "Дата создания";
            }
        }
        private void LoadScheduleInfoToGrid()
        {
            using (MySqlConnection con = new MySqlConnection(connStr))
            {
                dataGridView5.DataSource = null;
                con.Open();
                ds.Tables["Schedule_info"].Clear();
                string sql = "Select id, id_schedule, date_start, periodicity From schedule_info";
                MySqlDataAdapter = new MySqlDataAdapter(sql, con);
                MySqlDataAdapter.Fill(ds, "Schedule_info");
                schedule_info.DataSource = ds.Tables["Schedule_info"];
                dataGridView5.DataSource = schedule_info;
                dataGridView5.Columns["id"].HeaderText = "#";
                dataGridView5.Columns["id_schedule"].HeaderText = "№ Расписания";
                dataGridView5.Columns["date_start"].HeaderText = "Дата начала";
                dataGridView5.Columns["periodicity"].HeaderText = "Периодичность";
            }
        }
        private void LoadPassengersToGrid()
        {
            using (MySqlConnection con = new MySqlConnection(connStr))
            {
                try
                {
                    con.Open();
                    dataGridView6.DataSource = null;
                    ds.Tables["Passenger"].Clear();
                    string sql = "Select id, fio, date_of_birth, passport_num, sex From passenger";
                    MySqlDataAdapter = new MySqlDataAdapter(sql, con);
                    MySqlDataAdapter.Fill(ds, "Passenger");
                    passengers.DataSource = ds.Tables["Passenger"];
                    dataGridView6.DataSource = passengers;
                    dataGridView6.Columns["id"].HeaderText = "#";
                    dataGridView6.Columns["fio"].HeaderText = "ФИО";
                    dataGridView6.Columns["date_of_birth"].HeaderText = "Дата рождения";
                    dataGridView6.Columns["passport_num"].HeaderText = "Номер паспорта";
                    dataGridView6.Columns["sex"].HeaderText = "Пол";
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (DataException dex)
                {
                    MessageBox.Show(dex.Message);
                }
                catch (Exception ex2)
                {
                    MessageBox.Show(ex2.Message);
                }
            }
        }
        private void LoadStationsToGrid()
        {
            comboBox_sch.DataSource = schedule;
            comboBox_sch.DisplayMember = "name";
            comboBox_sch.ValueMember = "id";
            using (MySqlConnection con = new MySqlConnection(connStr))
            {
                dataGridView7.DataSource = null;
                con.Open();
                ds.Tables["Stations"].Clear();
                string sql = "Select * From schedule Where id_schedule = @id";
                MySqlDataAdapter = new MySqlDataAdapter(sql, con);
                MySqlDataAdapter.SelectCommand.Parameters.AddWithValue("@id", Convert.ToInt32(comboBox_sch.SelectedValue));
                MySqlDataAdapter.Fill(ds, "Stations");
                stations.DataSource = ds.Tables["Stations"];
                dataGridView7.DataSource = stations;
                dataGridView7.Columns["id"].HeaderText = "#";
                dataGridView7.Columns["id_schedule"].HeaderText = "№ Расписания";
                dataGridView7.Columns["station"].HeaderText = "Станция";
                dataGridView7.Columns["arrival_time"].HeaderText = "Время прибытия";
                dataGridView7.Columns["departure_time"].HeaderText = "Время отправления";
                dataGridView7.Columns["parking_time"].HeaderText = "Время стоянки";
                dataGridView7.Columns["bias"].HeaderText = "Смещение";
            }
        }
        private void LoadUsersToGrid()
        {
            using (MySqlConnection con = new MySqlConnection(connStr))
            {
                dataGridView8.DataSource = null;
                con.Open();
                ds.Tables["Users"].Clear();
                string sql = "Select * From users";
                MySqlDataAdapter = new MySqlDataAdapter(sql, con);
                MySqlDataAdapter.Fill(ds, "Users");
                users.DataSource = ds.Tables["Users"];
                dataGridView8.DataSource = users;
                dataGridView8.Columns["id"].HeaderText = "#";
                dataGridView8.Columns["fio"].HeaderText = "ФИО";
                dataGridView8.Columns["login"].HeaderText = "Логин";
                dataGridView8.Columns["pass"].HeaderText = "Пароль";
                dataGridView8.Columns["status"].HeaderText = "Статус";
                dataGridView8.Columns["priv"].HeaderText = "Привелегии";
                dataGridView8.Columns["email"].HeaderText = "Почта";
                dataGridView8.Columns["tel"].HeaderText = "Телефон";
            }
        }
        private void LoadScheduleTrainsToGrid()
        {
            using (MySqlConnection con = new MySqlConnection(connStr))
            {
                dataGridView9.DataSource = null;
                con.Open();
                ds.Tables["Schedule_Trains"].Clear();
                string sql = "Select ts.id, ts.id_schedule, sp.name sc_name, ts.id_train, t.name t_name From train_schedule ts " +
                    " Inner Join schedule_prime sp on sp.id = ts.id_schedule " +
                    " Inner Join train t on t.id = ts.id_train Order By ts.id";
                MySqlDataAdapter = new MySqlDataAdapter(sql, con);
                MySqlDataAdapter.Fill(ds, "Schedule_Trains");
                schedule_trains.DataSource = ds.Tables["Schedule_Trains"];
                dataGridView9.DataSource = schedule_trains;
                dataGridView9.Columns["id"].HeaderText = "#";
                dataGridView9.Columns["id_schedule"].HeaderText = "№ Расписания";
                dataGridView9.Columns["sc_name"].HeaderText = "Расписание";
                dataGridView9.Columns["id_train"].HeaderText = "№ Поезда";
                dataGridView9.Columns["t_name"].HeaderText = "Поезд";
            }
        }
        #endregion

        #region Добавление данных
        private void plane_add_Click(object sender, EventArgs e)
        {
            if (bort_num.Text == string.Empty ||
                model.Text == string.Empty ||
                company.Text == string.Empty ||
                qty_places.Text == string.Empty)
            {
                MessageBox.Show("Заполните все поля!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (MySqlConnection con = new MySqlConnection(connStr))
            {
                try
                {
                    con.Open();
                    //запрос на добавление
                    string sql = "INSERT INTO train (number, name, company, qty_places) " +
                        " VALUES (@bort_num, @model, @company, @colvo_mest)";
                    MySqlDataAdapter.InsertCommand = new MySqlCommand(sql, con);  // новая команда создана
                    // определим параметры и зададим им значения
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@bort_num", MySqlDbType.VarChar).Value = bort_num.Text;
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@model", MySqlDbType.VarChar).Value = model.Text;
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@company", MySqlDbType.VarChar).Value = company.Text;
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@colvo_mest", MySqlDbType.Int32).Value = qty_places.Text;
                    // выполним запрос
                    MySqlDataAdapter.InsertCommand.ExecuteNonQuery();
                    // обновим таблицу
                    LoadPlanesToGrid();
                    MessageBox.Show("Успешно добавлено!", "Добавление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void type_add_Click(object sender, EventArgs e)
        {
            if (Type.Text == string.Empty)
            {
                MessageBox.Show("Заполните все поля!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (MySqlConnection con = new MySqlConnection(connStr))
            {
                try
                {
                    con.Open();
                    //запрос на добавление
                    string sql = "INSERT INTO type (name) " +
                        " VALUES (@name)";
                    MySqlDataAdapter.InsertCommand = new MySqlCommand(sql, con);  // новая команда создана
                    // определим параметры и зададим им значения
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@name", MySqlDbType.VarChar).Value = Type.Text;
                    // выполним запрос
                    MySqlDataAdapter.InsertCommand.ExecuteNonQuery();
                    // обновим таблицу
                    LoadTypesToGrid();
                    MessageBox.Show("Успешно добавлено!", "Добавление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void cost_add_Click(object sender, EventArgs e)
        {
            if (comboBox_train.Text == string.Empty ||
                comboBox_type.Text == string.Empty ||
                Price.Text == string.Empty)

            {
                MessageBox.Show("Заполните все поля!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (MySqlConnection con = new MySqlConnection(connStr))
            {
                try
                {
                    con.Open();
                    //запрос на добавление
                    string sql = "INSERT INTO cost (id_train, id_type, sum) " +
                        " VALUES (@id_train, @id_type, @sum)";
                    MySqlDataAdapter.InsertCommand = new MySqlCommand(sql, con);  // новая команда создана
                    // определим параметры и зададим им значения
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@id_train", MySqlDbType.Int32).Value = comboBox_train.SelectedValue;
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@id_type", MySqlDbType.Int32).Value = comboBox_type.SelectedValue;
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@sum", MySqlDbType.Decimal).Value = Convert.ToDecimal(Price.Text);
                    // выполним запрос
                    MySqlDataAdapter.InsertCommand.ExecuteNonQuery();
                    // обновим таблицу
                    LoadCostsToGrid();
                    MessageBox.Show("Успешно добавлено!", "Добавление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void schedule_add_Click(object sender, EventArgs e)
        {
            if (Schedule_name.Text == string.Empty)
            {
                MessageBox.Show("Заполните все поля!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (MySqlConnection con = new MySqlConnection(connStr))
            {
                try
                {
                    con.Open();
                    //запрос на добавление
                    string sql = "INSERT INTO schedule_prime (name, introduction_date) " +
                        " VALUES (@name, now())";
                    MySqlDataAdapter.InsertCommand = new MySqlCommand(sql, con);  // новая команда создана
                    // определим параметры и зададим им значения
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@name", MySqlDbType.VarChar).Value = Schedule_name.Text;
                    // выполним запрос
                    MySqlDataAdapter.InsertCommand.ExecuteNonQuery();
                    // обновим таблицу
                    LoadScheduleToGrid();
                    MessageBox.Show("Успешно добавлено!", "Добавление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void schedule_info_add_Click(object sender, EventArgs e)
        {
            if (combobox_Schedule.Text == string.Empty ||
                date_start.Value == null ||
                Periodicity.Text == string.Empty)
            {
                MessageBox.Show("Заполните все поля!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (MySqlConnection con = new MySqlConnection(connStr))
            {
                try
                {
                    con.Open();
                    //запрос на добавление
                    string sql = "INSERT INTO schedule_info (id_schedule, date_start, periodicity) " +
                        " VALUES (@id_schedule, @date_start, @periodicity)";
                    MySqlDataAdapter.InsertCommand = new MySqlCommand(sql, con);  // новая команда создана
                    // определим параметры и зададим им значения
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@id_schedule", MySqlDbType.Int32).Value = combobox_Schedule.SelectedValue;
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@date_start", MySqlDbType.Date).Value = date_start.Value;
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@periodicity", MySqlDbType.Int32).Value = Periodicity.Text;
                    // выполним запрос
                    MySqlDataAdapter.InsertCommand.ExecuteNonQuery();
                    // обновим таблицу
                    LoadScheduleInfoToGrid();
                    MessageBox.Show("Успешно добавлено!", "Добавление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void passenger_add_Click(object sender, EventArgs e)
        {
            if (FIO.Text == string.Empty ||
                DateOfBirth.Value == null ||
                Passport_num.Text == string.Empty ||
                Sex.Text == string.Empty)
            {
                MessageBox.Show("Заполните все поля!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (MySqlConnection con = new MySqlConnection(connStr))
            {
                try
                {
                    con.Open();
                    //запрос на добавление
                    string sql = "INSERT INTO passenger (fio, date_of_birth, passport_num, sex) " +
                        " VALUES (@fio, @date_of_birth, @passport_num, @sex)";
                    MySqlDataAdapter.InsertCommand = new MySqlCommand(sql, con);  // новая команда создана
                    // определим параметры и зададим им значения
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@fio", MySqlDbType.VarChar).Value = FIO.Text;
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@date_of_birth", MySqlDbType.Date).Value = DateOfBirth.Value;
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@passport_num", MySqlDbType.VarChar).Value = Passport_num.Text;
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@sex", MySqlDbType.VarChar).Value = Sex.Text;
                    // выполним запрос
                    MySqlDataAdapter.InsertCommand.ExecuteNonQuery();
                    // обновим таблицу
                    LoadPassengersToGrid();
                    MessageBox.Show("Успешно добавлено!", "Добавление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void station_add_Click(object sender, EventArgs e)
        {
            if (Station.Text == string.Empty ||
                Schedule_choice.Text == string.Empty ||
                Arrival_time.Text == string.Empty ||
                Departure_time.Text == string.Empty ||
                Parking_time.Text == string.Empty ||
                Bias.Text == string.Empty)
            {
                MessageBox.Show("Заполните все поля!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (MySqlConnection con = new MySqlConnection(connStr))
            {
                try
                {
                    con.Open();
                    //запрос на добавление
                    string sql = "INSERT INTO schedule (id_schedule, station, arrival_time, departure_time, parking_time, bias) " +
                        " VALUES (@id_schedule,@station,@arrival_time,@departure_time,@parking_time,@bias)";
                    MySqlDataAdapter.InsertCommand = new MySqlCommand(sql, con);  // новая команда создана
                    // определим параметры и зададим им значения
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@id_schedule", MySqlDbType.Int32).Value = Schedule_choice.SelectedValue;
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@station", MySqlDbType.VarChar).Value = Station.Text;
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@arrival_time", MySqlDbType.VarChar).Value = Arrival_time.Text;
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@departure_time", MySqlDbType.VarChar).Value = Departure_time.Text;
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@parking_time", MySqlDbType.VarChar).Value = Parking_time.Text;
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@bias", MySqlDbType.VarChar).Value = Bias.Text;
                    // выполним запрос
                    MySqlDataAdapter.InsertCommand.ExecuteNonQuery();
                    // обновим таблицу
                    LoadStationsToGrid();
                    MessageBox.Show("Успешно добавлено!", "Добавление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void user_add_Click(object sender, EventArgs e)
        {
            if (fio_user.Text == string.Empty ||
                login_user.Text == string.Empty ||
                password_user.Text == string.Empty ||
                status_user.Text == string.Empty ||
                priv_user.Text == string.Empty ||
                email_user.Text == string.Empty ||
                tel_user.Text == string.Empty)
            {
                MessageBox.Show("Заполните все поля!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (MySqlConnection con = new MySqlConnection(connStr))
            {
                try
                {
                    con.Open();
                    //запрос на добавление
                    string sql = "INSERT INTO users (fio, login, pass, status, priv, email, tel) " +
                        " VALUES (@fio,@login,@pass,@status,@priv,@email,@tel)";
                    MySqlDataAdapter.InsertCommand = new MySqlCommand(sql, con);  // новая команда создана
                    // определим параметры и зададим им значения
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@fio", MySqlDbType.VarChar).Value = fio_user.Text;
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@login", MySqlDbType.VarChar).Value = login_user.Text;
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@pass", MySqlDbType.VarChar).Value = password_user.Text;
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@status", MySqlDbType.Int32).Value = Convert.ToInt32(status_user.Text);
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@priv", MySqlDbType.Int32).Value = Convert.ToInt32(priv_user.Text);
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@email", MySqlDbType.VarChar).Value = email_user.Text;
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@tel", MySqlDbType.VarChar).Value = tel_user.Text;
                    // выполним запрос
                    MySqlDataAdapter.InsertCommand.ExecuteNonQuery();
                    // обновим таблицу
                    LoadUsersToGrid();
                    MessageBox.Show("Успешно добавлено!", "Добавление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void schedule_train_add_Click(object sender, EventArgs e)
        {
            if (comboBox_schedule_choice.Text == string.Empty ||
                comboBox_train_choice.Text == string.Empty)
            {
                MessageBox.Show("Заполните все поля!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (MySqlConnection con = new MySqlConnection(connStr))
            {
                try
                {
                    con.Open();
                    //запрос на добавление
                    string sql = "INSERT INTO train_schedule (id_schedule, id_train) " +
                        " VALUES (@id_schedule,@id_train)";
                    MySqlDataAdapter.InsertCommand = new MySqlCommand(sql, con);  // новая команда создана
                    // определим параметры и зададим им значения
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@id_schedule", MySqlDbType.Int32).Value = comboBox_schedule_choice.SelectedValue;
                    MySqlDataAdapter.InsertCommand.Parameters.Add("@id_train", MySqlDbType.Int32).Value = comboBox_train_choice.SelectedValue;
                    // выполним запрос
                    MySqlDataAdapter.InsertCommand.ExecuteNonQuery();
                    // обновим таблицу
                    LoadScheduleTrainsToGrid();
                    MessageBox.Show("Успешно добавлено!", "Добавление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        #endregion

        #region Удаление данных
        private void plane_del_Click(object sender, EventArgs e)
        {
            if (ds.Tables["Train"].Rows.Count > 0)
            {
                string sql = "DELETE FROM train WHERE id = @ID";
                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    try
                    {
                        con.Open();
                        MySqlDataAdapter.DeleteCommand = new MySqlCommand(sql, con);
                        // Если нажата кномка да, удаления не избежать.
                        if (DialogResult.Yes == MessageBox.Show("Вы уверены в удалении? \nЗаписей:  "
                            + dataGridView1.SelectedRows.Count.ToString(), "Удаление", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                        {
                            foreach (DataGridViewRow drv in dataGridView1.SelectedRows)
                            {
                                MySqlDataAdapter.DeleteCommand.Parameters.Add("@ID", MySqlDbType.Int32).Value =
                                    Convert.ToInt32(ds.Tables["Train"].Rows[drv.Index][0]);
                                MySqlDataAdapter.DeleteCommand.ExecuteNonQuery();
                                MySqlDataAdapter.DeleteCommand.Parameters.Clear();
                            }
                            // обновим таблицу
                            LoadPlanesToGrid();
                            MessageBox.Show("Успешно удалено!", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Таблица пуста", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void type_del_Click(object sender, EventArgs e)
        {
            if (ds.Tables["Type"].Rows.Count > 0)
            {
                string sql = "DELETE FROM type WHERE id = @ID";
                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    try
                    {
                        con.Open();
                        MySqlDataAdapter.DeleteCommand = new MySqlCommand(sql, con);
                        // Если нажата кномка да, удаления не избежать.
                        if (DialogResult.Yes == MessageBox.Show("Вы уверены в удалении? \nЗаписей:  "
                            + dataGridView2.SelectedRows.Count.ToString(), "Удаление", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                        {
                            foreach (DataGridViewRow drv in dataGridView2.SelectedRows)
                            {
                                MySqlDataAdapter.DeleteCommand.Parameters.Add("@ID", MySqlDbType.Int32).Value =
                                    Convert.ToInt32(ds.Tables["Type"].Rows[drv.Index][0]);
                                MySqlDataAdapter.DeleteCommand.ExecuteNonQuery();
                                MySqlDataAdapter.DeleteCommand.Parameters.Clear();
                            }
                            // обновим таблицу
                            LoadTypesToGrid();
                            MessageBox.Show("Успешно удалено!", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Таблица пуста", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void cost_del_Click(object sender, EventArgs e)
        {
            if (ds.Tables["Cost"].Rows.Count > 0)
            {
                string sql = "DELETE FROM cost WHERE id = @ID";
                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    try
                    {
                        con.Open();
                        MySqlDataAdapter.DeleteCommand = new MySqlCommand(sql, con);
                        // Если нажата кномка да, удаления не избежать.
                        if (DialogResult.Yes == MessageBox.Show("Вы уверены в удалении? \nЗаписей:  "
                            + dataGridView3.SelectedRows.Count.ToString(), "Удаление", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                        {
                            foreach (DataGridViewRow drv in dataGridView3.SelectedRows)
                            {
                                MySqlDataAdapter.DeleteCommand.Parameters.Add("@ID", MySqlDbType.Int32).Value =
                                    Convert.ToInt32(ds.Tables["Cost"].Rows[drv.Index][0]);
                                MySqlDataAdapter.DeleteCommand.ExecuteNonQuery();
                                MySqlDataAdapter.DeleteCommand.Parameters.Clear();
                            }
                            // обновим таблицу
                            LoadCostsToGrid();
                            MessageBox.Show("Успешно удалено!", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Таблица пуста", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void schedule_del_Click(object sender, EventArgs e)
        {
            if (ds.Tables["Schedule"].Rows.Count > 0)
            {
                string sql = "DELETE FROM schedule_prime WHERE id = @ID";
                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    try
                    {
                        con.Open();
                        MySqlDataAdapter.DeleteCommand = new MySqlCommand(sql, con);
                        // Если нажата кномка да, удаления не избежать.
                        if (DialogResult.Yes == MessageBox.Show("Вы уверены в удалении? \nЗаписей:  "
                            + dataGridView4.SelectedRows.Count.ToString(), "Удаление", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                        {
                            foreach (DataGridViewRow drv in dataGridView4.SelectedRows)
                            {
                                MySqlDataAdapter.DeleteCommand.Parameters.Add("@ID", MySqlDbType.Int32).Value =
                                    Convert.ToInt32(ds.Tables["Schedule"].Rows[drv.Index][0]);
                                MySqlDataAdapter.DeleteCommand.ExecuteNonQuery();
                                MySqlDataAdapter.DeleteCommand.Parameters.Clear();
                            }
                            // обновим таблицу
                            LoadScheduleToGrid();
                            MessageBox.Show("Успешно удалено!", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Таблица пуста", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void schedule_info_del_Click(object sender, EventArgs e)
        {
            if (ds.Tables["Schedule_info"].Rows.Count > 0)
            {
                string sql = "DELETE FROM schedule_info WHERE id = @ID";
                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    try
                    {
                        con.Open();
                        MySqlDataAdapter.DeleteCommand = new MySqlCommand(sql, con);
                        // Если нажата кномка да, удаления не избежать.
                        if (DialogResult.Yes == MessageBox.Show("Вы уверены в удалении? \nЗаписей:  "
                            + dataGridView5.SelectedRows.Count.ToString(), "Удаление", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                        {
                            foreach (DataGridViewRow drv in dataGridView5.SelectedRows)
                            {
                                MySqlDataAdapter.DeleteCommand.Parameters.Add("@ID", MySqlDbType.Int32).Value =
                                    Convert.ToInt32(ds.Tables["Schedule_info"].Rows[drv.Index][0]);
                                MySqlDataAdapter.DeleteCommand.ExecuteNonQuery();
                                MySqlDataAdapter.DeleteCommand.Parameters.Clear();
                            }
                            // обновим таблицу
                            LoadScheduleInfoToGrid();
                            MessageBox.Show("Успешно удалено!", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Таблица пуста", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void passenger_del_Click(object sender, EventArgs e)
        {
            if (ds.Tables["Passenger"].Rows.Count > 0)
            {
                string sql = "DELETE FROM passenger WHERE id = @ID";
                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    try
                    {
                        con.Open();
                        MySqlDataAdapter.DeleteCommand = new MySqlCommand(sql, con);
                        // Если нажата кномка да, удаления не избежать.
                        if (DialogResult.Yes == MessageBox.Show("Вы уверены в удалении? \nЗаписей:  "
                            + dataGridView6.SelectedRows.Count.ToString(), "Удаление", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                        {
                            foreach (DataGridViewRow drv in dataGridView6.SelectedRows)
                            {
                                MySqlDataAdapter.DeleteCommand.Parameters.Add("@ID", MySqlDbType.Int32).Value =
                                    Convert.ToInt32(ds.Tables["Passenger"].Rows[drv.Index][0]);
                                MySqlDataAdapter.DeleteCommand.ExecuteNonQuery();
                                MySqlDataAdapter.DeleteCommand.Parameters.Clear();
                            }
                            // обновим таблицу
                            LoadPassengersToGrid();
                            MessageBox.Show("Успешно удалено!", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    catch (DataException dex)
                    {
                        MessageBox.Show(dex.Message);
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Таблица пуста", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void station_del_Click(object sender, EventArgs e)
        {
            if (ds.Tables["Stations"].Rows.Count > 0)
            {
                string sql = "DELETE FROM schedule WHERE id = @ID";
                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    try
                    {
                        con.Open();
                        MySqlDataAdapter.DeleteCommand = new MySqlCommand(sql, con);
                        // Если нажата кномка да, удаления не избежать.
                        if (DialogResult.Yes == MessageBox.Show("Вы уверены в удалении? \nЗаписей:  "
                            + dataGridView7.SelectedRows.Count.ToString(), "Удаление", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                        {
                            foreach (DataGridViewRow drv in dataGridView7.SelectedRows)
                            {
                                MySqlDataAdapter.DeleteCommand.Parameters.Add("@ID", MySqlDbType.Int32).Value =
                                    Convert.ToInt32(ds.Tables["Stations"].Rows[drv.Index][0]);
                                MySqlDataAdapter.DeleteCommand.ExecuteNonQuery();
                                MySqlDataAdapter.DeleteCommand.Parameters.Clear();
                            }
                            // обновим таблицу
                            LoadStationsToGrid();
                            MessageBox.Show("Успешно удалено!", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    catch (DataException dex)
                    {
                        MessageBox.Show(dex.Message);
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Таблица пуста", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void users_del_Click(object sender, EventArgs e)
        {
            if (ds.Tables["Users"].Rows.Count > 0)
            {
                string sql = "DELETE FROM users WHERE id = @ID";
                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    try
                    {
                        con.Open();
                        MySqlDataAdapter.DeleteCommand = new MySqlCommand(sql, con);
                        // Если нажата кномка да, удаления не избежать.
                        if (DialogResult.Yes == MessageBox.Show("Вы уверены в удалении? \nЗаписей:  "
                            + dataGridView8.SelectedRows.Count.ToString(), "Удаление", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                        {
                            foreach (DataGridViewRow drv in dataGridView8.SelectedRows)
                            {
                                MySqlDataAdapter.DeleteCommand.Parameters.Add("@ID", MySqlDbType.Int32).Value =
                                    Convert.ToInt32(ds.Tables["Users"].Rows[drv.Index][0]);
                                MySqlDataAdapter.DeleteCommand.ExecuteNonQuery();
                                MySqlDataAdapter.DeleteCommand.Parameters.Clear();
                            }
                            // обновим таблицу
                            LoadUsersToGrid();
                            MessageBox.Show("Успешно удалено!", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    catch (DataException dex)
                    {
                        MessageBox.Show(dex.Message);
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Таблица пуста", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void schedule_trains_del_Click(object sender, EventArgs e)
        {
            if (ds.Tables["Schedule_Trains"].Rows.Count > 0)
            {
                string sql = "DELETE FROM train_schedule WHERE id = @ID";
                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    try
                    {
                        con.Open();
                        MySqlDataAdapter.DeleteCommand = new MySqlCommand(sql, con);
                        // Если нажата кномка да, удаления не избежать.
                        if (DialogResult.Yes == MessageBox.Show("Вы уверены в удалении? \nЗаписей:  "
                            + dataGridView9.SelectedRows.Count.ToString(), "Удаление", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                        {
                            foreach (DataGridViewRow drv in dataGridView9.SelectedRows)
                            {
                                MySqlDataAdapter.DeleteCommand.Parameters.Add("@ID", MySqlDbType.Int32).Value =
                                    Convert.ToInt32(ds.Tables["Schedule_Trains"].Rows[drv.Index][0]);
                                MySqlDataAdapter.DeleteCommand.ExecuteNonQuery();
                                MySqlDataAdapter.DeleteCommand.Parameters.Clear();
                            }
                            // обновим таблицу
                            LoadScheduleTrainsToGrid();
                            MessageBox.Show("Успешно удалено!", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    catch (DataException dex)
                    {
                        MessageBox.Show(dex.Message);
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Таблица пуста", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #region Обновление данных
        private void plane_update_Click(object sender, EventArgs e)
        {
            if (ds.Tables["Train"].Rows.Count > 0)
            {
                if (bort_num.Text == string.Empty ||
                model.Text == string.Empty ||
                company.Text == string.Empty ||
                qty_places.Text == string.Empty)
                {
                    MessageBox.Show("Заполните все поля!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    try
                    {
                        con.Open();
                        // запрос на обновление
                        string sql = " UPDATE train SET number = @bort_num, name = @model, company = @company, " +
                                 " qty_places = @colvo_mest " +
                                 " WHERE id = @ID";

                        // команда для обноления создана
                        MySqlDataAdapter.UpdateCommand = new MySqlCommand(sql, con);
                        // зададим значения параметрам 
                        // определим параметры и зададим им значения
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@bort_num", MySqlDbType.VarChar).Value = bort_num.Text;
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@model", MySqlDbType.VarChar).Value = model.Text;
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@company", MySqlDbType.VarChar).Value = company.Text;
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@colvo_mest", MySqlDbType.Int32).Value = qty_places.Text;
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@ID", MySqlDbType.Int32).Value =
                            Convert.ToInt32(ds.Tables["Train"].Rows[dataGridView1.CurrentRow.Index][0]);
                        //выполним запрос
                        MySqlDataAdapter.UpdateCommand.ExecuteNonQuery();
                        //Обновим таблицу
                        LoadPlanesToGrid();
                        MessageBox.Show("Запись успешно обновлена!", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Таблица пуста", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void type_update_Click(object sender, EventArgs e)
        {
            if (ds.Tables["Type"].Rows.Count > 0)
            {
                if (Type.Text == string.Empty)
                {
                    MessageBox.Show("Заполните все поля!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    try
                    {
                        con.Open();
                        // запрос на обновление
                        string sql = " UPDATE type SET name = @name" +
                                 " WHERE id = @ID";

                        // команда для обноления создана
                        MySqlDataAdapter.UpdateCommand = new MySqlCommand(sql, con);
                        // зададим значения параметрам 
                        // определим параметры и зададим им значения
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@name", MySqlDbType.VarChar).Value = Type.Text;
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@ID", MySqlDbType.Int32).Value =
                            Convert.ToInt32(ds.Tables["Type"].Rows[dataGridView2.CurrentRow.Index][0]);
                        //выполним запрос
                        MySqlDataAdapter.UpdateCommand.ExecuteNonQuery();
                        //Обновим таблицу
                        LoadTypesToGrid();
                        MessageBox.Show("Запись успешно обновлена!", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Таблица пуста", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void cost_update_Click(object sender, EventArgs e)
        {
            if (ds.Tables["Cost"].Rows.Count > 0)
            {
                if (comboBox_train.Text == string.Empty ||
                    comboBox_type.Text == string.Empty ||
                    Price.Text == string.Empty)
                {
                    MessageBox.Show("Заполните все поля!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    try
                    {
                        con.Open();
                        // запрос на обновление
                        string sql = " UPDATE cost SET id_train = @id_train, id_type = @id_type, sum = @sum" +
                                 " WHERE id = @ID";

                        // команда для обноления создана
                        MySqlDataAdapter.UpdateCommand = new MySqlCommand(sql, con);
                        // зададим значения параметрам 
                        // определим параметры и зададим им значения
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@id_train", MySqlDbType.Int32).Value = comboBox_train.SelectedValue;
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@id_type", MySqlDbType.Int32).Value = comboBox_type.SelectedValue;
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@sum", MySqlDbType.Decimal).Value = Convert.ToDecimal(Price.Text);
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@ID", MySqlDbType.Int32).Value =
                            Convert.ToInt32(ds.Tables["Cost"].Rows[dataGridView3.CurrentRow.Index][0]);
                        //выполним запрос
                        MySqlDataAdapter.UpdateCommand.ExecuteNonQuery();
                        //Обновим таблицу
                        LoadCostsToGrid();
                        MessageBox.Show("Запись успешно обновлена!", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show(ex2.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Таблица пуста", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void schedule_update_Click(object sender, EventArgs e)
        {
            if (ds.Tables["Schedule"].Rows.Count > 0)
            {
                if (Schedule_name.Text == string.Empty)
                {
                    MessageBox.Show("Заполните все поля!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    try
                    {
                        con.Open();
                        // запрос на обновление
                        string sql = " UPDATE schedule_prime SET name = @name " +
                                 " WHERE id = @ID";

                        // команда для обноления создана
                        MySqlDataAdapter.UpdateCommand = new MySqlCommand(sql, con);
                        // зададим значения параметрам 
                        // определим параметры и зададим им значения
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@name", MySqlDbType.VarChar).Value = Schedule_name.Text;
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@ID", MySqlDbType.Int32).Value =
                            Convert.ToInt32(ds.Tables["Schedule"].Rows[dataGridView4.CurrentRow.Index][0]);
                        //выполним запрос
                        MySqlDataAdapter.UpdateCommand.ExecuteNonQuery();
                        //Обновим таблицу
                        LoadScheduleToGrid();
                        MessageBox.Show("Запись успешно обновлена!", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show(ex2.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Таблица пуста", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void schedule_info_update_Click(object sender, EventArgs e)
        {
            if (ds.Tables["Schedule_info"].Rows.Count > 0)
            {
                if (combobox_Schedule.Text == string.Empty ||
                    date_start.Value == null ||
                    Periodicity.Text == string.Empty)
                {
                    MessageBox.Show("Заполните все поля!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    try
                    {
                        con.Open();
                        // запрос на обновление
                        string sql = " UPDATE schedule_info SET id_schedule = @id_schedule, date_start = @date_start, periodicity = @periodicity " +
                                 " WHERE id = @ID";

                        // команда для обноления создана
                        MySqlDataAdapter.UpdateCommand = new MySqlCommand(sql, con);
                        // зададим значения параметрам 
                        // определим параметры и зададим им значения
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@id_schedule", MySqlDbType.Int32).Value = combobox_Schedule.SelectedValue;
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@date_start", MySqlDbType.Date).Value = date_start.Value;
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@periodicity", MySqlDbType.Int32).Value = Periodicity.Text;
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@ID", MySqlDbType.Int32).Value =
                            Convert.ToInt32(ds.Tables["Schedule_info"].Rows[dataGridView5.CurrentRow.Index][0]);
                        //выполним запрос
                        MySqlDataAdapter.UpdateCommand.ExecuteNonQuery();
                        //Обновим таблицу
                        LoadScheduleInfoToGrid();
                        MessageBox.Show("Запись успешно обновлена!", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show(ex2.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Таблица пуста", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void passenger_update_Click(object sender, EventArgs e)
        {
            if (ds.Tables["Passenger"].Rows.Count > 0)
            {
                if (FIO.Text == string.Empty ||
                    DateOfBirth.Value == null ||
                    Passport_num.Text == string.Empty ||
                    Sex.Text == string.Empty)
                {
                    MessageBox.Show("Заполните все поля!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    try
                    {
                        con.Open();
                        // запрос на обновление
                        string sql = " UPDATE passenger SET fio = @fio, date_of_birth = @date_of_birth, passport_num = @passport_num, sex = @sex " +
                                 " WHERE id = @ID";

                        // команда для обноления создана
                        MySqlDataAdapter.UpdateCommand = new MySqlCommand(sql, con);
                        // зададим значения параметрам 
                        // определим параметры и зададим им значения
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@fio", MySqlDbType.VarChar).Value = FIO.Text;
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@date_of_birth", MySqlDbType.Date).Value = DateOfBirth.Value;
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@passport_num", MySqlDbType.VarChar).Value = Passport_num.Text;
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@sex", MySqlDbType.VarChar).Value = Sex.Text;
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@ID", MySqlDbType.Int32).Value =
                            Convert.ToInt32(ds.Tables["Passenger"].Rows[dataGridView6.CurrentRow.Index][0]);
                        //выполним запрос
                        MySqlDataAdapter.UpdateCommand.ExecuteNonQuery();
                        //Обновим таблицу
                        LoadPassengersToGrid();
                        MessageBox.Show("Запись успешно обновлена!", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show(ex2.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Таблица пуста", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void station_update_Click(object sender, EventArgs e)
        {
            if (ds.Tables["Stations"].Rows.Count > 0)
            {
                if (Station.Text == string.Empty ||
                    Schedule_choice.Text == string.Empty ||
                    Arrival_time.Text == string.Empty ||
                    Departure_time.Text == string.Empty ||
                    Parking_time.Text == string.Empty ||
                    Bias.Text == string.Empty)
                {
                    MessageBox.Show("Заполните все поля!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    try
                    {
                        con.Open();
                        // запрос на обновление
                        string sql = " UPDATE schedule SET id_schedule = @id_schedule, station = @station, arrival_time = @arrival_time," +
                                 " departure_time = @departure_time, parking_time = @parking_time, bias = @bias" +
                                 " WHERE id = @ID";

                        // команда для обноления создана
                        MySqlDataAdapter.UpdateCommand = new MySqlCommand(sql, con);
                        // зададим значения параметрам 
                        // определим параметры и зададим им значения
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@id_schedule", MySqlDbType.Int32).Value = Schedule_choice.SelectedValue;
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@station", MySqlDbType.VarChar).Value = Station.Text;
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@arrival_time", MySqlDbType.VarChar).Value = Arrival_time.Text;
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@departure_time", MySqlDbType.VarChar).Value = Departure_time.Text;
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@parking_time", MySqlDbType.VarChar).Value = Parking_time.Text;
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@bias", MySqlDbType.VarChar).Value = Bias.Text;
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@ID", MySqlDbType.Int32).Value =
                            Convert.ToInt32(ds.Tables["Stations"].Rows[dataGridView7.CurrentRow.Index][0]);
                        //выполним запрос
                        MySqlDataAdapter.UpdateCommand.ExecuteNonQuery();
                        //Обновим таблицу
                        LoadStationsToGrid();
                        MessageBox.Show("Запись успешно обновлена!", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show(ex2.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Таблица пуста", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void users_update_Click(object sender, EventArgs e)
        {
            if (ds.Tables["Users"].Rows.Count > 0)
            {
                if (fio_user.Text == string.Empty ||
                    login_user.Text == string.Empty ||
                    password_user.Text == string.Empty ||
                    status_user.Text == string.Empty ||
                    priv_user.Text == string.Empty ||
                    email_user.Text == string.Empty ||
                    tel_user.Text == string.Empty)
                {
                    MessageBox.Show("Заполните все поля!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    try
                    {
                        con.Open();
                        // запрос на обновление
                        string sql = " UPDATE users SET fio = @fio, login = @login, pass = @pass," +
                                 " status = @status, priv = @priv, email = @email, tel = @tel" +
                                 " WHERE id = @ID";

                        // команда для обноления создана
                        MySqlDataAdapter.UpdateCommand = new MySqlCommand(sql, con);
                        // зададим значения параметрам 
                        // определим параметры и зададим им значения
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@fio", MySqlDbType.VarChar).Value = fio_user.Text;
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@login", MySqlDbType.VarChar).Value = login_user.Text;
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@pass", MySqlDbType.VarChar).Value = password_user.Text;
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@status", MySqlDbType.Int32).Value = Convert.ToInt32(status_user.Text);
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@priv", MySqlDbType.Int32).Value = Convert.ToInt32(priv_user.Text);
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@email", MySqlDbType.VarChar).Value = email_user.Text;
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@tel", MySqlDbType.VarChar).Value = tel_user.Text;
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@ID", MySqlDbType.Int32).Value =
                            Convert.ToInt32(ds.Tables["Users"].Rows[dataGridView8.CurrentRow.Index][0]);
                        //выполним запрос
                        MySqlDataAdapter.UpdateCommand.ExecuteNonQuery();
                        //Обновим таблицу
                        LoadUsersToGrid();
                        MessageBox.Show("Запись успешно обновлена!", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show(ex2.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Таблица пуста", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void schedule_trains_update_Click(object sender, EventArgs e)
        {
            if (ds.Tables["Schedule_Trains"].Rows.Count > 0)
            {
                if (comboBox_schedule_choice.Text == string.Empty ||
                    comboBox_train_choice.Text == string.Empty)
                {
                    MessageBox.Show("Заполните все поля!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    try
                    {
                        con.Open();
                        // запрос на обновление
                        string sql = " UPDATE train_schedule SET id_schedule = @id_schedule, id_train = @id_train " +
                                 " WHERE id = @ID";

                        // команда для обноления создана
                        MySqlDataAdapter.UpdateCommand = new MySqlCommand(sql, con);
                        // зададим значения параметрам 
                        // определим параметры и зададим им значения
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@id_schedule", MySqlDbType.Int32).Value = comboBox_schedule_choice.SelectedValue;
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@id_train", MySqlDbType.Int32).Value = comboBox_train_choice.SelectedValue;
                        MySqlDataAdapter.UpdateCommand.Parameters.Add("@ID", MySqlDbType.Int32).Value =
                            Convert.ToInt32(ds.Tables["Users"].Rows[dataGridView9.CurrentRow.Index][0]);
                        //выполним запрос
                        MySqlDataAdapter.UpdateCommand.ExecuteNonQuery();
                        //Обновим таблицу
                        LoadScheduleTrainsToGrid();
                        MessageBox.Show("Запись успешно обновлена!", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show(ex2.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Таблица пуста", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #region конпки управления
        //Открыть панель администрирования
        private void AdminPanel_Click(object sender, EventArgs e)
        {
            wizardPages1.SelectedTab = Administration;
        }
        //Вход
        private void button1_Click(object sender, EventArgs e)
        {
            string log = string.Empty;
            string pas = string.Empty;
            if (login.Text == string.Empty || psw.Text == string.Empty)
            {
                MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                log = login.Text;
                pas = psw.Text;
            }

            Authorization authorization = new Authorization(log, pas, connStr);
            authorization.Check();
            bool stat = authorization.GetStatus();

            int priv = authorization.GetRole();
            if (priv == 0)
                AdminPanel.Visible = true;
            else
                AdminPanel.Visible = false;

            if (stat) wizardPages1.SelectedTab = Prime;
            psw.Text = string.Empty;
        }

        //Выйти
        private void Logout_Click(object sender, EventArgs e)
        {
            wizardPages1.SelectedTab = Registration;
        }

        //Закрыть окно
        private void Exit_Click(object sender, EventArgs e)
        {
            Close();
        }
        //Выйти из админки
        private void ExitFromAdmin_Click(object sender, EventArgs e)
        {
            wizardPages1.SelectedTab = Prime;
        }
        #endregion
    }
}
