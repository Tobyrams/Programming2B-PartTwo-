using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Security.Permissions;
using System.Threading;
using PARTONE_CLASSLIBRARY;


namespace Part_two_test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /* USERNAMES AND PASSWORDS👇
            
            Rams : hello
            Toby : Password
            Jane : flower
            Tim : hello
            Tebogo: rams123
           
         */

        //Connection to the database 
        private string connectionString = "Data Source=TOBYLAP\\SQLEXPRESS;Initial Catalog=ProgPart2_Test;Integrated Security=True";

       

        //This is the global userid and it will help with displaying the users 
        public int Holderid = 99;

        public MainWindow()
        {
            InitializeComponent();

            tabControl.SelectedIndex = 7;
            StartingApplicationLoader();


            Login_Page.Visibility = Visibility.Hidden;
            SignUp_Page.Visibility = Visibility.Hidden;
            addModule_Page.Visibility = Visibility.Hidden;
            ViewModels_Page.Visibility = Visibility.Hidden;
            LoadingModules_Page.Visibility = Visibility.Hidden;
            LogoutLoading_Page.Visibility = Visibility.Hidden;
            LogInLoading_Page.Visibility = Visibility.Hidden;
        }

        #region Methods  ===================================================================================

        private string ComputeHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Compute the hash of the input password
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();

                // Convert the hash bytes to a hexadecimal string
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }

                return builder.ToString();// Return the hashed password as a string
            }
        }

        private void DisaplayData() // Displayes all the data in the database
        {
            try
            {
                // Establish a connection to the database using the provided connection string.
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Open the database connection.
                    connection.Open();

                    // Define the SQL query to retrieve data from the database.
                    string query = "SELECT module_code as Code, module_name as Name, num_credits as Credits, class_hours_per_week as WeekHours, num_weeks_in_semester as SemesterWeeks, semester_start_date as StartDate, studied_on_certain_date as DateStudied, studied_hours_on_certin_date as HoursStudied, Self_study_hours, Remaning_study_hours " +
                                   "FROM Module " +
                                   "JOIN UserProfile ON Module.userID = UserProfile.user_id " +
                                   "WHERE userID = @userID;";

                    // Create a SqlCommand object with the query and the database connection.
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        // Add a parameter to the SqlCommand to prevent SQL injection and set its value.
                        cmd.Parameters.AddWithValue("@userID", Holderid);

                        // Creates a adapter to execute the query and retrieve data.
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            // Creates a DataTable to hold the retrieved data.
                            DataTable dataTable = new DataTable();

                            // Fill the DataTable with data from the database.
                            adapter.Fill(dataTable);

                            // Set the data source of ModuleDataGrid to the DataTable to display the data.
                            ModuleDataGrid.ItemsSource = dataTable.DefaultView;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public void resetDefaultText()
        {
            txtCode.Text = "Eg.Wede6212";
            txtCode.Foreground = Brushes.Gray;
            txtName.Text = "Eg.Web Development";
            txtName.Foreground = Brushes.Gray;
            txtCreditSlider.Text = "0";
            txtClassHoursSliderTextBox.Text = "0";
            txtNumWeeksSemesterSliderTextBox.Text = "0";
            SemesterStartDate.SelectedDate = null;
            CertainDatePicker.SelectedDate = null;
            CertainDateTextBox.Text = "Eg.2";
            CertainDateTextBox.Foreground = Brushes.Gray;
        }

        public async void StartingApplicationLoader()
        {
            await Task.Delay(6000);// Thread that delays a task for 5 seconds
            tabControl.SelectedIndex = 0;
            LodingApplication_Page.Visibility = Visibility.Hidden;
        }

        #endregion


        #region LoginPage  ===================================================================================

        private void Signup_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 1;
            Login_Page.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// The function when the user clicks the login button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loginClick(object sender, RoutedEventArgs e)
        {

            string usernameFromTextBox = loginUsernametxtbk.Text;
            string passwordFromTextBox = loginPasswordtxtbk.Password;


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT Password,user_id FROM UserProfile WHERE Username = @Username";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", usernameFromTextBox);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string passwordFromDatabase = reader["Password"].ToString();
                                //here we hash the password from the passwordBox then compare it to the already hashed password in the database
                                if (ComputeHash(passwordFromTextBox) == passwordFromDatabase)
                                {
                                    //if the password is correct we take them to the Add Modules page
                                    tabControl.SelectedIndex = 6;
                                    LogingInLoader();

                                    //Clear the textboxes
                                    loginUsernametxtbk.Clear();
                                    loginPasswordtxtbk.Clear();
                                    
                                    //Gets the userId
                                    Holderid = Convert.ToInt32(reader["user_id"]);

                                    Login_Page.Visibility = Visibility.Hidden;
                                }
                                else
                                {
                                    // Message they recieve when the password is incorrect
                                    MessageBox.Show("Password is incorrect.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            else
                            {
                                // Username does not exist in the database
                                MessageBox.Show("Username Does Not Exist In The Database", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
                // Handle exceptions (e.g., database connection error)
            }

        }

        public async void LogingInLoader()
        {
            await Task.Delay(3000);// Thread that delays a task for 3 seconds
            tabControl.SelectedIndex = 2;
        }

        #endregion


        #region SignUpPage ===================================================================================
        private void Go_To_Login_Page(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 0;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string username = signUpUsernametxtbk.Text;
            string password = signUpPasswordtxtbk.Text;

            string hashPassword = ComputeHash(password);

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {


                try
                {

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {

                        connection.Open();

                        string query = "INSERT INTO UserProfile (Username,Password) VALUES (@Username, @Password)";

                        SqlCommand cmd = new SqlCommand(query, connection);

                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", hashPassword);

                        cmd.ExecuteNonQuery();

                    }

                    MessageBox.Show("Account Has Been Registered", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                    tabControl.SelectedIndex = 0;
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627) // SQL Server error number for duplicate key violation

                    {
                        MessageBox.Show("This Username Aleready Exits", "Duplicate", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                        // Handle other types of SQL errors
                    }
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please Enter All Details!!", "ERROR!!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }


        }

        #endregion


        #region ModulePage  ===================================================================================

        private void txtCode_GotFocus(object sender, RoutedEventArgs e)
        {
            txtCode.Clear();
            txtCode.Foreground = Brushes.Black;
        }
        private void txtName_GotFocus(object sender, RoutedEventArgs e)
        {
            txtName.Clear();
            txtName.Foreground = Brushes.Black;
        }
        private void CertainDateTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            CertainDateTextBox.Clear();
            CertainDateTextBox.Foreground = Brushes.Black;
        }

        private void AddModule_ButtonClick(object sender, RoutedEventArgs e)
        {
            //Variables
            string code = txtCode.Text;
            string name = txtName.Text;
            int numOfCredits = int.Parse(txtCreditSlider.Text);
            int classHoursPerWeek = int.Parse(txtClassHoursSliderTextBox.Text);
            int weeksInSemester = int.Parse(txtNumWeeksSemesterSliderTextBox.Text);
            var semesterStart = SemesterStartDate.SelectedDate;
            var certainDate = CertainDatePicker.SelectedDate;
            int certainDateValue;

            //Test if user has NOT typed the incorrect detailes else(if they have) an error message will appear
            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(CertainDateTextBox.Text) && numOfCredits != 0 && classHoursPerWeek != 0 && weeksInSemester != 0 && semesterStart != null && certainDate != null)
            {
                //This test if the certain date value enterd is an int else it will send an error
                if (int.TryParse(CertainDateTextBox.Text, out certainDateValue)) { }
                else
                {
                    MessageBox.Show("Please Make Sure The Certin Date Is A Number", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    resetDefaultText();
                    return;
                }

                //If the certin value is less than 0 thows and error message.
                if (certainDateValue < 0)
                {
                    MessageBox.Show("Hours Studied cannot be less than 0!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    resetDefaultText();
                    return;
                }

                /*This is to check if a module has more than 30 creadits
                  and if it does we tell the user that, that cannot happen.
                  
                 */
                if (numOfCredits > 30)
                {
                    MessageBox.Show("Credits Cannot Be More Than 30 for One module!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    resetDefaultText();
                    return;
                }

                /*This if statement if so that the user cannot leave the textboxes with the example
                 text and pass it as a code or a name as by not entering details in it is the
                 same as leaving the textboxes as empty*/
                if (code.Equals("Eg.Wede6212") || name.Equals("Eg.Web Development"))
                {
                    MessageBox.Show("Please Make Sure All Details Are Entered!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    resetDefaultText();
                    return;
                }

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = "INSERT INTO Module (module_code, module_name, num_credits, class_hours_per_week, num_weeks_in_semester, semester_start_date, studied_on_certain_date, studied_hours_on_certin_date,Self_study_hours,Remaning_study_hours, userID) " +
                                        "VALUES (@module_code, @module_name, @num_credits, @class_hours_per_week, @num_weeks_in_semester, @semester_start_date, @studied_on_certain_date, @studied_hours_on_certin_date,@Self_study_hours,@Remaning_study_hours, @userID)";

                        SqlCommand cmd = new SqlCommand(query, connection);

                        cmd.Parameters.AddWithValue("@module_code", code);
                        cmd.Parameters.AddWithValue("@module_name", name);
                        cmd.Parameters.AddWithValue("@num_credits", numOfCredits);
                        cmd.Parameters.AddWithValue("@class_hours_per_week", classHoursPerWeek);
                        cmd.Parameters.AddWithValue("@num_weeks_in_semester", weeksInSemester);
                        cmd.Parameters.AddWithValue("@semester_start_date", (DateTime)semesterStart);
                        cmd.Parameters.AddWithValue("@studied_on_certain_date", (DateTime)certainDate);
                        cmd.Parameters.AddWithValue("@studied_hours_on_certin_date", certainDateValue);
                        cmd.Parameters.AddWithValue("@Self_study_hours", PARTONE_CLASSLIBRARY.Calculations.self_Study_Hours(numOfCredits, weeksInSemester, classHoursPerWeek));
                        cmd.Parameters.AddWithValue("@Remaning_study_hours", PARTONE_CLASSLIBRARY.Calculations.remainingStudyHours(PARTONE_CLASSLIBRARY.Calculations.self_Study_Hours(numOfCredits, weeksInSemester, classHoursPerWeek), certainDateValue));
                        cmd.Parameters.AddWithValue("@userID", Holderid);

                        //Executes the query
                        cmd.ExecuteNonQuery();
                    }

                    //Resets the fields back to their default state
                    resetDefaultText();

                    //Success Message
                    MessageBox.Show("Module Added!!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                catch (SqlException ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please Make Sure All Details Are Entered!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                resetDefaultText();
            }
        }



        private void logout_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 5;
            Holderid = 99;
            LogingOutLoader();
        }
        public async void LogingOutLoader()
        {
            await Task.Delay(3000);// Thread that delays a task for 3 seconds
            tabControl.SelectedIndex = 0;
        }

        private void ViewModules_ButtonClick(object sender, RoutedEventArgs e)
        {
            //Loads tha data
            DisaplayData();
            //Take user to the loading page
            tabControl.SelectedIndex = 4;

            //After the time is complete page redirects back to the viewMoudles page
            loadingpage();


        }

        public async void loadingpage()
        {
            await Task.Delay(3000);// Thread that delays a task for 3 seconds
            tabControl.SelectedIndex = 3;
        }

        #endregion


        #region ViewModulespage  ===================================================================================
        private void ListModuleBack_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 2;
        }

        #endregion


    }
}
