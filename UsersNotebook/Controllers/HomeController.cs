using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UsersNotebook.Models;
using System.Data.SqlClient;

namespace UsersNotebook.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        SqlConnection connection = new SqlConnection(); 
        SqlCommand command = new SqlCommand();
        SqlDataReader dataReader;
        List<UsersData> usersList = new List<UsersData>();
        SendData sendData = new SendData();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            connection.ConnectionString = UsersNotebook.Properties.Resources.ConnectionString;
            
        }

        public IActionResult Index()
        {
            FetchData();
            return View(usersList);
        }
        public IActionResult Form()
        {
            return View();
        }
        
        public void FetchData()
        {
            try
            {
                if (usersList.Count > 0)
                {
                    usersList.Clear();
                }
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT TOP (1000) [ID], [Name],[Surname], [Date], [Sex], [PhoneNumber], [Job]  FROM [Users].[dbo].[Users]";
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    usersList.Add(new UsersData() {
                        ID = dataReader["ID"].ToString(),
                        Name = dataReader["Name"].ToString(),
                        Surname = dataReader["Surname"].ToString(),
                        Date = dataReader["Date"].ToString(),
                        Sex = dataReader["Sex"].ToString(),
                        PhoneNumber = dataReader["PhoneNumber"].ToString(),
                        Job = dataReader["Job"].ToString()
                    });
                }
                connection.Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult Post(UsersData userData)
        {
            FetchData();
            try
            {
                if (userData != null)
                {
                    command.CommandType = System.Data.CommandType.Text;
                    int length = usersList.Count +1;
                    string text = "INSERT Users (ID, Name, Surname, Date, Sex, PhoneNumber, Job) VALUES ('"+ length + "', '" + userData.Name + "', '" + userData.Surname + "', '" + userData.Date + "', '" + userData.Sex + "', '" + userData.PhoneNumber + "', '" + userData.Job + "')" ;
                    command.CommandText = text;
                    command.Connection = connection;

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                }  
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return RedirectToAction("Index");
        }

        public ActionResult Post(SendData sendData)
        {
            FetchData();
            try
            {
                if (sendData != null)
                {
                    command.CommandType = System.Data.CommandType.Text;
                    int length = usersList.Count + 1;
                    string text = "UPDATE Users (Name, Surname, Date, Sex, PhoneNumber, Job) VALUES ('" + sendData.data.Name + "', '" + sendData.data.Surname + "', '" + sendData.data.Date + "', '" + sendData.data.Sex + "', '" + sendData.data.PhoneNumber + "', '" + sendData.data.Job + "') WHERE Name= " + sendData.Name + " Surname= " + sendData.Surname;
                    command.CommandText = text;
                    command.Connection = connection;

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}