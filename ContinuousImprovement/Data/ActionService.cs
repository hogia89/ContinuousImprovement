using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace ContinuousImprovement.Data
{
    public class ActionService
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnviroment;


        private FirestoreDb db;
        private string projectId = "loopkup-476c7";
        //private string filepath = "C:\\Users\\hvho\\Downloads\\loopkup-476c7-6bdf61c5ce1f.json";
        private string filepath = "";
        public ActionService(IConfiguration configuration,IWebHostEnvironment webHostEnvironment)
        {
            
            
            _configuration = configuration;
            _webHostEnviroment = webHostEnvironment;
            filepath = _webHostEnviroment.ContentRootPath + "/loopkup-476c7-6bdf61c5ce1f.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            db = FirestoreDb.Create(projectId);
        }

        public string[] GetEmployeeInfo(string employeeId)
        {
            string id = "";
            string fullname = "";
            string department = "";
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sqlString = "" +
                    "Select EmployeeId,FullName,Department from SummarizeListOfOperatorLastestDate_CRS530 where EmployeeId='" + employeeId + "'";
                SqlCommand command = new SqlCommand(sqlString, connection);
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        id = dataReader.GetValue(0).ToString();
                        fullname = dataReader.GetValue(1).ToString();
                        department = dataReader.GetValue(2).ToString();
                    }
                    connection.Close();
                }

            }
            string[] info = new string[3] { id, fullname, department };
            return info;
        }
        public string[] CheckName(string employeeId)
        {
            string FullName = "";
            string CostCenter = "";
            OracleCommand comm = default(OracleCommand);
            OracleDataReader dr = default(OracleDataReader);
            string strConnectionString_ODBC = _configuration.GetSection("ConnectionStrings").GetSection("ODBCConnectionString").Value;
            OracleConnection cn = new OracleConnection(strConnectionString_ODBC);
            string sqlGetMONO = "" +
                 " SELECT PFODS.CEAEMP_143.EACANO AS id, PFODS.CEAEMP_143.EAEMNM AS FullName, PFODS.CEAEMP_143.EADEPT AS CostCenter" +
                 " FROM PFODS.CEAEMP_143" +
                 " WHERE PFODS.CEAEMP_143.EACANO = '" + employeeId + "'";
            comm = new OracleCommand(sqlGetMONO, cn);
            cn.Open();
            dr = comm.ExecuteReader();
            if (dr.Read() == true)
            {
                FullName = dr.GetValue(1).ToString();
                CostCenter = dr.GetValue(2).ToString().Substring(2, 4);
            }
            cn.Close();
            dr.Close();
            cn.Dispose();
            comm.Dispose();
            cn.Dispose();
            string[] infor = new string[2] { FullName, CostCenter };
            return (infor);
        }

        public async Task<List<Suggestion>> GetSuggestions(string currentUser)
        {
            //Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            //db = FirestoreDb.Create(projectId);
            List<Suggestion> suggestions = new List<Suggestion>();
            try
            {
                currentUser = currentUser.Replace("AP\\", "") + "@vn.pepperl-fuchs.com";
                Query query = db.Collection("Suggestion").WhereEqualTo("ownerAction",currentUser);
                QuerySnapshot suggestionSnapshots = await query.GetSnapshotAsync();

                foreach (DocumentSnapshot documentSnapshot in suggestionSnapshots.Documents)
                {
                    DocumentReference documentReference = db.Collection("Suggestion").Document(documentSnapshot.Id);
                    DocumentSnapshot snapshot = await documentReference.GetSnapshotAsync();
                    if (snapshot.Exists)
                    {
                        Suggestion suggestion = snapshot.ConvertTo<Suggestion>();
                        suggestion.id = documentSnapshot.Id;
                        suggestions.Add(suggestion);
                    }
                    else
                    {

                    }
                }

            }
            catch
            {
                throw;
            }
            return suggestions;
        }

        public async Task<List<Suggestion>> GetMyTask(string userId)
        {
            //Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            //db = FirestoreDb.Create(projectId);
            List<Suggestion> suggestions = new List<Suggestion>();
            try
            {
                Query query = db.Collection("Suggestion");
                QuerySnapshot suggestionSnapshots = await query.GetSnapshotAsync();

                foreach (DocumentSnapshot documentSnapshot in suggestionSnapshots.Documents)
                {
                    DocumentReference documentReference = db.Collection("Suggestion").Document(documentSnapshot.Id);
                    DocumentSnapshot snapshot = await documentReference.GetSnapshotAsync();
                    if (snapshot.Exists)
                    {
                        Suggestion suggestion = snapshot.ConvertTo<Suggestion>();
                        suggestion.id = documentSnapshot.Id;
                        suggestions.Add(suggestion);
                    }
                    else
                    {

                    }
                }

            }
            catch
            {
                throw;
            }
            return suggestions;
        }
        public async Task<List<ListOfDepartment>> GetListOfDepartments()
        {
            List<ListOfDepartment> listOfDepartments = new List<ListOfDepartment>();
            try
            {
                Query query = db.Collection("Department");
                QuerySnapshot deptSnapshots = await query.GetSnapshotAsync();
                foreach (DocumentSnapshot documentSnapshot in deptSnapshots.Documents)
                {
                    DocumentReference documentReference = db.Collection("Department").Document(documentSnapshot.Id);
                    DocumentSnapshot document = await documentReference.GetSnapshotAsync();
                    if (document.Exists)
                    {
                        ListOfDepartment listdept = document.ConvertTo<ListOfDepartment>();
                        listdept.Department = documentSnapshot.Id;
                        listOfDepartments.Add(listdept);

                    }
                }
            }
            catch
            {
                throw;
            }
            return listOfDepartments;
        }

        public async Task UpdateData(string Id, string result)
        {
            //Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            //FirestoreDb db = FirestoreDb.Create(projectId);
            DocumentReference document = db.Collection("Suggestion")
                .Document(Id);
            //DateTime td = DateTime.Now;
            Timestamp td = Timestamp.GetCurrentTimestamp();
            Dictionary<string, object> updateStatus = new Dictionary<string, object>
            {
                {"planFinishDt", td},
                {"status",result },
                //{"approveSuggBy",currentUser }
            };
            await document.UpdateAsync(updateStatus);
        }

        public async Task UpdateAssignTask(string Id, string emailReceiver, string status)
        {
            DocumentReference document = db.Collection("Suggestion")
                .Document(Id);
            //DateTime td = DateTime.Now;
            Timestamp td = Timestamp.GetCurrentTimestamp();
            Dictionary<string, object> updateStatus = new Dictionary<string, object>
            {
                {"assignDt", td},
                {"status",status },
                {"ownerAction",emailReceiver }
            };
            await document.UpdateAsync(updateStatus);
        }

        public async Task LikeSuggestion(string Id, string currentUser)
        {
            //Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            //FirestoreDb db = FirestoreDb.Create(projectId);
            DocumentReference document = db.Collection("Like").Document(Id);
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                {"userName",currentUser }
            };
            await document.SetAsync(updates);
        }
        public async Task UnlikeSuggestion(string Id, string currentUser)
        {
            //Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            //FirestoreDb db = FirestoreDb.Create(projectId);
            DocumentReference document = db.Collection("Like").Document(Id);
            //Dictionary<string, object> updates = new Dictionary<string, object>
            //{
            //    {"userName",FieldValue.Delete }
            //};
            await document.DeleteAsync();
        }
        public async Task<List<ListOfLike>> GetListOfLike()
        {
            //Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            //db = FirestoreDb.Create(projectId);
            List<ListOfLike> listOfLikes = new List<ListOfLike>();
            try
            {
                Query query = db.Collection("Like");
                QuerySnapshot documentSnapshots = await query.GetSnapshotAsync();
                foreach (DocumentSnapshot document in documentSnapshots.Documents)
                {
                    DocumentReference documentReference = db.Collection("Like").Document(document.Id);
                    DocumentSnapshot snapshot = await documentReference.GetSnapshotAsync();
                    if (snapshot.Exists)
                    {
                        ListOfLike list = snapshot.ConvertTo<ListOfLike>();
                        list.Id = document.Id;
                        listOfLikes.Add(list);
                    }
                    else
                    {

                    }
                }
            }
            catch
            {
                throw;
            }
            return listOfLikes;
        }

        public async Task AddNewSuggestion(Suggestion suggestion)
        {
            //Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            //FirestoreDb db = FirestoreDb.Create(projectId);
            DocumentReference document = db.Collection("Suggestion").Document();
            //DateTime td = DateTime.Now;
            suggestion.submitDate = Timestamp.GetCurrentTimestamp();
            suggestion.status = "Đăng ký mới";
            await document.SetAsync(suggestion);
        }
        public async Task AddNewEmployee(UserProfiles user)
        {
            //Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            //FirestoreDb db = FirestoreDb.Create(projectId);
            DocumentReference document = db.Collection("UserProfiles").Document(user.Id);
            await document.SetAsync(user);
        }
        public async Task<List<UserProfiles>> GetUserProfile(string emplId)
        {
            List<UserProfiles> userProfiles = new List<UserProfiles>();
            //String emplId = user.employeeId;
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            FirestoreDb db = FirestoreDb.Create(projectId);
            try
            {
                Query query = db.Collection("UserProfiles").WhereEqualTo("employeeId", emplId);
                QuerySnapshot suggestionSnapshots = await query.GetSnapshotAsync();

                foreach (DocumentSnapshot documentSnapshot in suggestionSnapshots.Documents)
                {
                    DocumentReference documentReference = db.Collection("UserProfiles").Document(documentSnapshot.Id);
                    DocumentSnapshot snapshot = await documentReference.GetSnapshotAsync();
                    if (snapshot.Exists)
                    {
                        UserProfiles userProfiles1 = snapshot.ConvertTo<UserProfiles>();
                        userProfiles1.Id = documentSnapshot.Id;
                        userProfiles.Add(userProfiles1);
                    }
                    else
                    {

                    }
                }

            }
            catch
            {
                throw;
            }
            return userProfiles;
        }

    }
}

