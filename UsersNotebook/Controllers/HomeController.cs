using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UsersNotebook.Models;
using System.Data.SqlClient;
using PdfSharp.Pdf;
using System.Text;
using System.Collections;

namespace UsersNotebook.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        SqlConnection connection = new SqlConnection(); 
        SqlCommand command = new SqlCommand();
        SqlDataReader dataReader;
        List<UsersData> usersList = new List<UsersData>();
        UsersData userData = new UsersData();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Form()
        {
            return View(userData);
        }

        
        public ActionResult FormStart(int data)
        {
            if (data == 0)
            {
                userData = new UsersData();
                userData.Date = "01.01.2000 00:00:00";
            }
            else
            {
                FetchData();
                userData = usersList[data - 1];
                string sex = userData.Sex.Trim();
                userData.Sex= sex;
            }

            return View("Form", userData);
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
                    string text = "INSERT Users (ID, Name, Surname, Date, Sex, PhoneNumber, Job) VALUES ('"+ length + "', '" + userData.Name.Trim() + "', '" + userData.Surname.Trim() + "', '" + userData.Date.Trim() + "', '" + userData.Sex.Trim() + "', '" + userData.PhoneNumber.Trim() + "', '" + userData.Job.Trim() + "')" ;
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

        
        public ActionResult Update(SendData sendData)
        {
            FetchData();
            try
            {
                if (sendData != null)
                {
                    command.CommandType = System.Data.CommandType.Text;
                    string text = "UPDATE Users SET Name = '" + sendData.Name + "', Surname = '" + sendData.Surname + "', Date = '" + sendData.Date + "', Sex = '" + sendData.Sex + "', PhoneNumber = '" + sendData.PhoneNumber + "', Job = '" + sendData.Job + "' WHERE Name = '" + sendData.OldName + "' AND Surname = '" + sendData.OldSurname + "'";
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



        public ActionResult GenerateReport()
        {
            FetchData();
            
            string filename = DateTime.Now.ToString("G");
            StringBuilder sb = new StringBuilder();

            // Iterate through the usersList and append data to the StringBuilder
            foreach (UsersData userData in usersList)
            {
                var name = userData.Name;
                var trimmedName = name.Trim();
                var surname = userData.Surname;
                var trimmedSur = surname.Trim();
                var date = userData.Date;
                string datePart = date.Split(' ')[0];
                var trimmedDate = datePart.Trim();
                var sex = userData.Sex;
                var trimmedSex = sex.Trim();
                var phone = userData.PhoneNumber;
                var trimmedPhone = phone.Trim();
                var job = userData.Job;
                var trimmedJob = job.Trim();

                sb.AppendLine($"ID: {userData.ID}, Name: {trimmedName}, Surname: {trimmedSur}, Birthdate: {trimmedDate}, Sex: {trimmedSex}, Telephone: {trimmedPhone}, Job: {trimmedJob}");
                sb.AppendLine();
                // You can add more properties as needed
            }

            // Define the file path where you want to save the text file
            string filePath = filename +  ".txt";
            string content = sb.ToString();
            byte[] byteArray = Encoding.UTF8.GetBytes(content);
            // Save the content of the StringBuilder to a text file
            return File(byteArray, "text/plain", filePath);



        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}