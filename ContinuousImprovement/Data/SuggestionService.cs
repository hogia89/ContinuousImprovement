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
    public class SuggestionService
    {
        private readonly IConfiguration _configuration;
        //private readonly IHostingEnvironment _hostingEnviroment;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private FirestoreDb db;
        private string projectId = "loopkup-476c7";
        //private string filepath ="\\\\pfvn-netapp1\\files\\20-Manufacturing\\11-TPU-EMS\\_Public\\Vu\\loopkup-476c7-6bdf61c5ce1f.json";
        private string filepath = "";//"~/fonts/loopkup-476c7-6bdf61c5ce1f.json";//"C:\\inetpub\\wwwroot\\hvho\\MOnitoring\\loopkup-476c7-6bdf61c5ce1f.json";//"C:\\Users\\hvho\\Downloads\\loopkup-476c7-6bdf61c5ce1f.json";// "\\\\pfvn-netapp1\\files\\20-Manufacturing\\11-TPU-EMS\\_Public\\Vu\\loopkup-476c7-6bdf61c5ce1f.json";
        public SuggestionService(IConfiguration configuration,IWebHostEnvironment webHostEnvironment)
        {
            
           
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            string contentpath = _webHostEnvironment.ContentRootPath;
            filepath = contentpath + "/loopkup-476c7-6bdf61c5ce1f.json";
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

        public List<ListOfEmployee_CRS530> GetAllEmployeeInfo_Crs530()
        {
            List<ListOfEmployee_CRS530> lists = new List<ListOfEmployee_CRS530>();
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sqlString = "" +
                    "Select * from SummarizeListOfOperatorLastestDate_CRS530";
                SqlCommand command = new SqlCommand(sqlString, connection);
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        ListOfEmployee_CRS530 list = new ListOfEmployee_CRS530();
                        //                        list.EmployeeId = dataReader.GetInt32(1);
                        list.FullName = dataReader.GetValue(1).ToString();
                        list.CostCenter = dataReader.GetValue(2).ToString();
                        list.TPU = dataReader.GetValue(4).ToString();
                        list.Department = dataReader.GetValue(5).ToString();
                        list.Expr1 = dataReader.GetDateTime(6);
                        list.MonthSupport = dataReader.GetString(7);

                        lists.Add(list);
                    }
                    connection.Close();
                }
            }
            return lists;
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

        public async Task<List<Suggestion>> GetSuggestions(string currentUser, int year, int quater)
        {
            List<Suggestion> suggestions = new List<Suggestion>();
            try
            {

                Query query = db.Collection("Suggestion")
                    .WhereGreaterThanOrEqualTo("submitDate", Timestamp.FromDateTime(new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc)))
                    .WhereLessThan("submitDate", Timestamp.FromDateTime(new DateTime(year + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc)));
                //sorting quaterly
                switch (quater)
                {
                    case 0:
                        break;
                    case 1:
                        query = query.WhereGreaterThanOrEqualTo("submitDate", Timestamp.FromDateTime(new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc)))
                            .WhereLessThan("submitDate", Timestamp.FromDateTime(new DateTime(year, 4, 1, 0, 0, 0, DateTimeKind.Utc)));
                        break;
                    case 2:
                        query = query.WhereGreaterThanOrEqualTo("submitDate", Timestamp.FromDateTime(new DateTime(year, 4, 1, 0, 0, 0, DateTimeKind.Utc)))
                             .WhereLessThan("submitDate", Timestamp.FromDateTime(new DateTime(year, 7, 1, 0, 0, 0, DateTimeKind.Utc)));
                        break;
                    case 3:
                        query = query.WhereGreaterThanOrEqualTo("submitDate", Timestamp.FromDateTime(new DateTime(year, 7, 1, 0, 0, 0, DateTimeKind.Utc)))
                              .WhereLessThan("submitDate", Timestamp.FromDateTime(new DateTime(year, 10, 1, 0, 0, 0, DateTimeKind.Utc)));
                        break;
                    case 4:
                        query = query.WhereGreaterThanOrEqualTo("submitDate", Timestamp.FromDateTime(new DateTime(year, 10, 1, 0, 0, 0, DateTimeKind.Utc)))
                               .WhereLessThan("submitDate", Timestamp.FromDateTime(new DateTime(year + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc)));
                        break;
                    default:
                        break;
                }
                //// sorting department
                //if (dept != null || dept !="All")
                //{
                //    query = query.WhereEqualTo("deptRecSug", dept);
                //}
                ////sorting status
                //if (status != null)
                //{
                //    query = query.WhereEqualTo("status", status);
                //}
                QuerySnapshot suggestionSnapshots = await query.GetSnapshotAsync();

                foreach (DocumentSnapshot documentSnapshot in suggestionSnapshots.Documents)
                {
                    DocumentReference documentReference = db.Collection("Suggestion").Document(documentSnapshot.Id);
                    Query query1 = db.Collection("Like").WhereEqualTo(documentSnapshot.Id.ToString(), true);
                    QuerySnapshot documentSnapshots = await query1.GetSnapshotAsync();
                    int countLike = 0;
                    bool like = false;
                    foreach (DocumentSnapshot document in documentSnapshots.Documents)
                    {
                        if (document.Id == currentUser)
                        {
                            like = true;
                        }
                        countLike++;
                    }
                    DocumentSnapshot snapshot = await documentReference.GetSnapshotAsync();
                    if (snapshot.Exists)
                    {
                        Suggestion suggestion = snapshot.ConvertTo<Suggestion>();
                        suggestion.id = documentSnapshot.Id;
                        suggestion.CountLike = countLike;
                        suggestion.like = like;
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

        public async Task UpdateData(string Id, string currentUser, string result)
        {
            DocumentReference document = db.Collection("Suggestion")
                .Document(Id);
            Timestamp td = Timestamp.GetCurrentTimestamp();
            Dictionary<string, object> updateStatus = new Dictionary<string, object>
            {
                {"approveDt", td},
                {"status",result },
                {"approveSuggBy",currentUser }
            };
            await document.UpdateAsync(updateStatus);
        }

        public async Task UpdateAssignTask(string Id, string emailReceiver, string status)
        {
            DocumentReference document = db.Collection("Suggestion")
                .Document(Id);
            Timestamp td = Timestamp.GetCurrentTimestamp();
            Dictionary<string, object> updateStatus = new Dictionary<string, object>
            {
                {"assignDt", td},
                {"status",status },
                {"ownerAction",emailReceiver }
            };
            await document.UpdateAsync(updateStatus);
        }
        #region Manage like post
        //Save new like
        public async Task LikeSuggestion(string Id, string currentUser)
        {
            DocumentReference document = db.Collection("Like").Document(currentUser);
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                {Id,true}
            };
            await document.SetAsync(updates, SetOptions.MergeAll);
        }

        //Unlike the post
        public async Task UnlikeSuggestion(string Id, string currentUser)
        {
            DocumentReference document = db.Collection("Like").Document(currentUser);
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                {Id,false }
            };
            await document.SetAsync(updates, SetOptions.MergeAll);
        }
        //Get and count of like
        public async Task<List<ListOfLike>> GetListOfLike()
        {
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
                        list.userName = document.Id;
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
        #endregion
        public async Task AddNewSuggestion(Suggestion suggestion)
        {
            Timestamp dtSubmit = Timestamp.GetCurrentTimestamp();
            string Id = (dtSubmit.ToString() + suggestion.ownerCode).Replace(" ", "").Replace("-", "").Replace("Timestamp:", "").Replace(".", "");
            DocumentReference document = db.Collection("Suggestion").Document(Id);
            // DateTime td = DateTime.Now;
            suggestion.id = Id;
            suggestion.submitDate = dtSubmit;
            suggestion.status = "Đăng ký mới";
            await document.SetAsync(suggestion);
        }
        public async Task AddNewEmployee(UserProfiles user)
        {
            DocumentReference document = db.Collection("UserProfiles").Document(user.Id);
            await document.SetAsync(user);
        }
        public async Task<List<UserProfiles>> GetUserProfile(string emplId)
        {
            List<UserProfiles> userProfiles = new List<UserProfiles>();
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
