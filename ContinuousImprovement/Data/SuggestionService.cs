using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using ContinuousImprovement.Model;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace ContinuousImprovement.Data
{
    public class SuggestionService
    {
        private readonly IConfiguration _configuration;
        private readonly MOMContext _context;
        public SuggestionService(IConfiguration configuration, MOMContext context)
        {

            _context = context;
            _configuration = configuration;
        }
        public string[] GetEmployeeInfo_Crs530(string employeeId)
        {
            string id = null;
            string fullName = null;
            string department = null;
            string costCenter = null;
            string connectionString = _configuration["ConnectionStrings:ODBCConnectionString"];
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                string sqlString = "" +
                 " SELECT PFODS.CEAEMP_143.EACANO AS id, PFODS.CEAEMP_143.EAEMNM AS FullName, PFODS.CEAEMP_143.EADEPT AS CostCenter" +
                 " FROM PFODS.CEAEMP_143" +
                 " WHERE PFODS.CEAEMP_143.EACANO = '" + employeeId + "'";
                OracleCommand command = new OracleCommand(sqlString, connection);
                using (OracleDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        id = dataReader.GetValue(0).ToString();
                        fullName = dataReader.GetString(1);
                        costCenter = dataReader.GetValue(2).ToString();
                        department = _context.ProductionDepartment.Where(x => x.CostCenter == costCenter).Select(x => x.Department).FirstOrDefault();
                    }
                    connection.Close();
                }
            }
            string[] info = new string[4] { id, fullName, department, costCenter };
            return info;
        }
        public string[] GetEmployeeInfo(string employeeId)
        {
            string id = "";
            string fullname = "";
            string department = "";
            string costCenter = "";
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sqlString = "" +
                    "Select EmployeeId,FullName,Department,CostCenter from SummarizeListOfOperatorLastestDate_HR where EmployeeId='" + employeeId + "'";
                SqlCommand command = new SqlCommand(sqlString, connection);
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        id = dataReader.GetValue(0).ToString();
                        fullname = dataReader.GetValue(1).ToString();
                        department = dataReader.GetValue(2).ToString();
                        costCenter = dataReader.GetValue(3).ToString();
                    }
                    connection.Close();
                }
            }
            string[] info = new string[4] { id, fullname, department, costCenter };
            return info;
        }

        public async Task<List<EmployeeInfoCrs530>> GetAllEmployeeInfo_Crs530()
        {
            DateTime td = DateTime.Today;
            return await _context.EmployeeInfoCrs530.Where(x => x.Updatetime.Year == td.Year && x.Updatetime.Month == td.Month && x.Updatetime.Day == td.Day).ToListAsync();
        }

        public async Task<List<HrweeklyReport>> GetHrweeklyReports(int filterYear, int filterQuater)
        {
            DateTime dt = DateTime.Today;
            try
            {
                switch (filterQuater)
                {
                    case 0:
                        dt = _context.HrweeklyReport.Where(x => x.WorkingDate.Year == filterYear).Select(x => x.WorkingDate).Max();
                        break;
                    case 1:
                        dt = _context.HrweeklyReport.Where(x => x.WorkingDate.Year == filterYear && x.WorkingDate.Month <= 3).Select(x => x.WorkingDate).Max();
                        break;
                    case 2:
                        dt = _context.HrweeklyReport.Where(x => x.WorkingDate.Year == filterYear && x.WorkingDate.Month <= 6).Select(x => x.WorkingDate).Max();
                        break;
                    case 3:
                        dt = _context.HrweeklyReport.Where(x => x.WorkingDate.Year == filterYear && x.WorkingDate.Month <= 9).Select(x => x.WorkingDate).Max();
                        break;
                    case 4:
                        dt = _context.HrweeklyReport.Where(x => x.WorkingDate.Year == filterYear).Select(x => x.WorkingDate).Max();
                        break;
                    default:
                        dt = _context.HrweeklyReport.Where(x => x.WorkingDate.Year == filterYear).Select(x => x.WorkingDate).Max();
                        break;
                }

            }
            catch
            {

            }
            //if (filterQuater == 0)
            //{
            //    dt = _context.HrweeklyReport.Where(x => x.WorkingDate.Year == filterYear).Select(x => x.WorkingDate).Max();
            //}
            //DateTime dt = new DateTime(2020, 4, 12);
            return await _context.HrweeklyReport.Where(x => x.WorkingDate.Year == dt.Year && x.WorkingDate.Month == dt.Month && x.WorkingDate.Day == dt.Day).ToListAsync();
        }
        //public string[] CheckName(string employeeId)
        //{
        //    string FullName = "";
        //    string CostCenter = "";
        //    OracleCommand comm = default(OracleCommand);
        //    OracleDataReader dr = default(OracleDataReader);
        //    string strConnectionString_ODBC = _configuration.GetSection("ConnectionStrings").GetSection("ODBCConnectionString").Value;
        //    OracleConnection cn = new OracleConnection(strConnectionString_ODBC);
        //    string sqlGetMONO = "" +
        //         " SELECT PFODS.CEAEMP_143.EACANO AS id, PFODS.CEAEMP_143.EAEMNM AS FullName, PFODS.CEAEMP_143.EADEPT AS CostCenter" +
        //         " FROM PFODS.CEAEMP_143" +
        //         " WHERE PFODS.CEAEMP_143.EACANO = '" + employeeId + "'";
        //    comm = new OracleCommand(sqlGetMONO, cn);
        //    cn.Open();
        //    dr = comm.ExecuteReader();
        //    if (dr.Read() == true)
        //    {
        //        FullName = dr.GetValue(1).ToString();
        //        CostCenter = dr.GetValue(2).ToString().Substring(2, 4);
        //    }
        //    cn.Close();
        //    dr.Close();
        //    cn.Dispose();
        //    comm.Dispose();
        //    cn.Dispose();
        //    string[] infor = new string[2] { FullName, CostCenter };
        //    return (infor);
        //}


        public async Task<List<CipfSuggestion>> GetSuggestions(string currentUser, int year, int quarter)
        {
            List<CipfSuggestion> suggestions = await _context.CipfSuggestion.Where(x => x.SubmitDate.Year == year).ToListAsync();
            //sorting quaterly
            if (quarter > 0)
            {
                suggestions = suggestions.Where(x => (x.SubmitDate.Month + 2) / 3 == quarter).ToList();
            }

            return suggestions.OrderByDescending(x => x.SubmitDate).ToList();
        }

        // Get Suggestions for edit
        public async Task<List<CipfSuggestion>> GetSuggestions(int id)
        {
            var ExistData = _context.CipfSuggestion.Where(x => x.Id == id).FirstOrDefault();
            if (ExistData != null)
            {
                return await _context.CipfSuggestion.Where(x => x.Id == id).ToListAsync();
            }
            else
            {
                return null;
            }
        }
        //Get suggestion base on suggestion id
        // Get Suggestions for edit
        public async Task<List<CipfSuggestion>> GetSuggestions(string id)
        {
            var ExistData = _context.CipfSuggestion.Where(x => x.SuggestionId == id).FirstOrDefault();
            if (ExistData != null)
            {
                return await _context.CipfSuggestion.Where(x => x.SuggestionId == id).ToListAsync();
            }
            else
            {
                return null;
            }
        }
        public async Task<List<CipfSuggestion>> GetMyApproval(string userId, int year)
        {
            return await _context.CipfSuggestion
                .Where(x => (x.ApproveActionBy.Contains(userId.Replace("AP\\", "")) && x.IndicatorOfStatus == 5) || (x.Email.Contains(userId.Replace("AP\\", "")) && x.IndicatorOfStatus == 7) || (x.Email.Contains(userId.Replace("AP\\", ""))) && x.SubmitDate.Year == year).ToListAsync();
        
        }
        public async Task<List<CipfSuggestion>> GetMyTask(string userId, int year)
        {
            return await _context.CipfSuggestion
                .Where(x => x.OwnerAction.Contains(userId.Replace("AP\\", "")) && x.SubmitDate.Year == year).ToListAsync();
        }
        public async Task<List<ProductionDepartment>> GetListOfDepartments()
        {
            return await _context.ProductionDepartment.ToListAsync();
        }

        public async Task<bool> UpdateData(string Id, string currentUser, string result, string remark)
        {
            var ExistingSuggestion = _context.CipfSuggestion.Where(x => x.SuggestionId == Id).FirstOrDefault();
            if (ExistingSuggestion != null)
            {
                ExistingSuggestion.ApproveDt = DateTime.Now;
                ExistingSuggestion.Status = result;
                ExistingSuggestion.Remark = remark;
                if (result == "Đã duyệt đề xuất")
                {
                    ExistingSuggestion.IndicatorOfStatus = 2;
                }
                else if (result == "Hoàn tất")
                {
                    ExistingSuggestion.IndicatorOfStatus = 6;
                }
                else
                {
                    ExistingSuggestion.IndicatorOfStatus = 7;
                }

                ExistingSuggestion.ApproveSuggBy = currentUser;
                await _context.SaveChangesAsync();
            }
            else
            {
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);

        }
        //Edit new topic
        public async Task<bool> UpdateData(CipfSuggestion cipf)
        {
            var ExistingSuggestion = _context.CipfSuggestion.Where(x => x.Id == cipf.Id).FirstOrDefault();
            if (ExistingSuggestion != null)
            {
                ExistingSuggestion.DeptRecSug = cipf.DeptRecSug;
                ExistingSuggestion.ExpectedBenefit = cipf.ExpectedBenefit;
                ExistingSuggestion.Email = cipf.Email;
                ExistingSuggestion.SuggestionAction = cipf.SuggestionAction;
                ExistingSuggestion.CurrentStatus = cipf.CurrentStatus;
                ExistingSuggestion.SubmitDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            else
            {
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);

        }
        public async Task<bool> UpdateAssignTask(string Id, string emailReceiver, string status)
        {
            var ExistingSuggestion = _context.CipfSuggestion.Where(x => x.SuggestionId == Id).FirstOrDefault();
            if (ExistingSuggestion != null)
            {
                ExistingSuggestion.AssignDt = DateTime.Now;
                ExistingSuggestion.Status = status;
                ExistingSuggestion.IndicatorOfStatus = 3;
                ExistingSuggestion.OwnerAction = emailReceiver;
                await _context.SaveChangesAsync();
                sendEmail(ExistingSuggestion.ApproveSuggBy.Replace("AP\\", "") + "@vn.pepperl-fuchs.com", emailReceiver, Id);
            }
            else
            {
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
        }
        #region Manage like post
        //Save new like
        public Task<CipfListOfLike> LikeSuggestion(string Id, string currentUser, string userBeLiked, DateTime submitDt)
        {
            CipfListOfLike cipf = new CipfListOfLike();
            cipf.SuggestionId = Id;
            cipf.SubmitSuggestionDt = submitDt;//_context.CipfSuggestion.Where(x => x.SuggestionId == Id).Select(x => x.SubmitDate).FirstOrDefault();
            cipf.UserName = currentUser.Replace(" ", "");
            cipf.UserBeLiked = userBeLiked.Replace(" ", "");
            _context.Add(cipf);
            _context.SaveChanges();
            return Task.FromResult(cipf);
        }

        //Unlike the post
        public Task<bool> UnlikeSuggestion(string Id, string currentUser)
        {
            var ExistLikeSugg = _context.CipfListOfLike.Where(x => x.SuggestionId == Id && x.UserName == currentUser).FirstOrDefault();
            if (ExistLikeSugg != null)
            {
                _context.Remove(ExistLikeSugg);
                _context.SaveChanges();
            }
            else
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }
        //Get and count of like
        public async Task<List<CipfListOfLike>> GetListOfLike(int yr, int quarter)
        {
            List<CipfListOfLike> cipfListOfLikes = await _context.CipfListOfLike.Where(x => x.SubmitSuggestionDt.Year == yr).ToListAsync();
            if (quarter > 0)
            {
                cipfListOfLikes = cipfListOfLikes.Where(x => (x.SubmitSuggestionDt.Month + 2) / 3 == quarter).ToList();
            }
            return cipfListOfLikes;
        }
        #endregion
        public async Task<CipfSuggestion> AddNewSuggestion(CipfSuggestion suggestion)
        {
            DateTime dtSubmit = DateTime.Now;
            //string Id = (dtSubmit.ToLongTimeString()+dtSubmit.ToShortDateString()+ suggestion.OwnerCode).Replace(" ", "").Replace("-", "").Replace("/", "").Replace(".", "").Replace(":","");
            suggestion.SuggestionId = suggestion.SuggestionId;
            suggestion.SubmitDate = dtSubmit;
            suggestion.ApproveDt = new DateTime(1900, 1, 1);
            suggestion.AssignDt = new DateTime(1900, 1, 1);
            suggestion.PlanFinishActionDt = new DateTime(1900, 1, 1);
            suggestion.Status = "Đăng ký mới";
            suggestion.IndicatorOfStatus = 1;
            _context.CipfSuggestion.Add(suggestion);
            await _context.SaveChangesAsync();
            return await Task.FromResult(suggestion);
        }

        //Delete new topic
        public async Task<bool> DeleteNewTopic(int id)
        {
            var ExistData = _context.CipfSuggestion.Where(x => x.Id == id).FirstOrDefault();
            if (ExistData != null)
            {
                _context.Remove(ExistData);
                await _context.SaveChangesAsync();
                return await Task.FromResult(true);
            }
            else
            {
                return await Task.FromResult(false);
            }
        }
        //Update picture before only
        public async Task<bool> UpdateBeforePicture(string Id)
        {
            var ExistSuggestion = _context.CipfSuggestion.Where(x => x.SuggestionId == Id).FirstOrDefault();
            if (ExistSuggestion != null)
            {
                ExistSuggestion.ImageUriBefore = Id + ".jpg";
                await _context.SaveChangesAsync();
                return await Task.FromResult(true);
            }
            else
            {
                return await Task.FromResult(false);
            }

        }
        public async Task<CipfUserProfiles> AddNewEmployee(CipfUserProfiles user)
        {
            _context.CipfUserProfiles.Add(user);
            await _context.SaveChangesAsync();
            return await Task.FromResult(user);
        }
        public async Task<List<CipfUserProfiles>> GetUserProfile(string emplId)
        {
            return await _context.CipfUserProfiles.Where(x => x.EmployeeId == emplId).ToListAsync();
        }

        #region send email
        //Send email
        private string message { get; set; } = "";
        private void sendEmail(string senderEmail, string recieverEmail, string id)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    //mail.From = new MailAddress("cipf.autoemail.noreply@gmail.com");
                    mail.From = new MailAddress("ContinuousImprovement.autoemail.noreply@vn.pepperl-fuchs.com");
                    mail.To.Add(recieverEmail);
                    mail.CC.Add(senderEmail);
                    //mail.CC.Add("hvho");
                    mail.Subject = "Continuous improvement _ New Suggestion _ " + id;
                    mail.Body = "<span style=\"font-size:13px;font-family:Helvetica,Arial,sans-serif;\">Dear Sir,</p></br>" +
                        "<span style=\"font-size:13px;font-family:Helvetica,Arial,sans-serif;\">There is a new suggestion to improve our company. You are right person to implement this idea. Here is link to access your task:</span></br>" +
                        "</br><a style=\"text-decoration:none;font-size:13px;font-family:Helvetica,Arial,sans-serif;color:#0275d8;border-radius:5px;padding:6px 12px 6px 12px;border:1px solid #0071c5;display:inline-block\"" +
                        " href=\"172.22.0.21:8384/mytask\">Your Task ID - " + id + "</a></br><hr>" +
                        "</br><span style=\"color:#868e96; font-size:10px\">This is an auto-generated message. Please DO NOT reply.</span></br>";
                    mail.IsBodyHtml = true;
                    //using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 25))
                    using (SmtpClient smtp = new SmtpClient("mailgw01", 25))
                    {
                        //smtp.Credentials = new System.Net.NetworkCredential("cipf.autoemail.noreply@gmail.com", "Pf1234567");
                        //smtp.Credentials = new System.Net.NetworkCredential("hvho@vn.pepperl-fuchs.com", "Pf1234567");
                        //smtp.EnableSsl = true;
                        //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        //smtp.UseDefaultCredentials = false;
                        smtp.Send(mail);
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        //Send email inform new email
        public void sendEmailNewSuggest(string recieverEmail, string cc,CipfSuggestion info)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    //mail.From = new MailAddress("cipf.autoemail.noreply@gmail.com");
                    mail.From = new MailAddress("ContinuousImprovement.autoemail.noreply@vn.pepperl-fuchs.com");
                    mail.To.Add(recieverEmail);
                    mail.CC.Add(cc);
                    //mail.CC.Add("hvho");
                    mail.Subject = "Continuous improvement _ New Suggestion";
                    mail.Body = "<span style=\"font-size:13px;font-family:Helvetica,Arial,sans-serif;\">Dear Sir,</p></br>" +
                        "<span style=\"font-size:13px;font-family:Helvetica,Arial,sans-serif;\">There is a new suggestion to improve our company. Here is link to access your task:</span></br>" +
                        "</br><a style=\"text-decoration:none;font-size:13px;font-family:Helvetica,Arial,sans-serif;color:#0275d8;border-radius:5px;padding:6px 12px 6px 12px;border:1px solid #0071c5;display:inline-block\"" +
                        " href=\"172.22.0.21:8384/myapproval\">Click Here</a></br><hr>" +
                         "<span class=\"small\" style=\"text-decoration-line:underline\"><b>Details:</b></span>" +
                        "<span style=\"font-size:13px;font-family:Helvetica,Arial,sans-serif;\">This is suggested by " + info.OwnerSuggestion + "_" + info.OwnerCode + ".</span></br>" +
                        "</br>" +
                        "<div class=\"row p-0 m - 0\">" +
                        "<span class=\"small\" style=\"text-decoration-line:underline\"><b>Current Status:</b></span>" +
                        "</div>" +
                        "<div class=\"row p-0 m-0\">" +
                        "<span class=\"small\" style=\"font-weight:bold\">" + info.CurrentStatus + "</span>" +
                        "</div>" +
                        "<br />" +
                        "<div class=\"row p-0 m-0\">" +
                        "<span class=\"small\" style=\"text-decoration-line:underline;font-style:oblique\"><b>Suggestion:</b></span>" +
                        "</div>" +
                        "<div class=\"row p-0 m-0\">" +
                        "<span class=\"small\" style=\"font-style:oblique\">" + info.SuggestionAction + "</span>" +
                        "</div>" +
                        "<br /><hr>" +

                        "</br><span style=\"color:#868e96; font-size:10px\">This is an auto-generated message. Please DO NOT reply.</span></br>";
                    mail.IsBodyHtml = true;
                    //using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 25))
                    using (SmtpClient smtp = new SmtpClient("mailgw01", 25))
                    {
                        //smtp.Credentials = new System.Net.NetworkCredential("cipf.autoemail.noreply@gmail.com", "Pf1234567");
                        //smtp.Credentials = new System.Net.NetworkCredential("hvho@vn.pepperl-fuchs.com", "Pf1234567");
                        //smtp.EnableSsl = true;
                        //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        //smtp.UseDefaultCredentials = false;
                        smtp.Send(mail);
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        //send email inform reject new suggestion
        public void sendEmailRejectSuggestion(string recieverEmail, string cc, string remark,string Id)
        {
            var info = _context.CipfSuggestion.Where(x => x.SuggestionId == Id).FirstOrDefault();
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    //mail.From = new MailAddress("cipf.autoemail.noreply@gmail.com");
                    mail.From = new MailAddress("ContinuousImprovement.autoemail.noreply@vn.pepperl-fuchs.com");
                    mail.To.Add(recieverEmail);
                    mail.CC.Add(cc);
                    //mail.CC.Add("hvho");
                    mail.Subject = "Continuous improvement _ Decline Suggestion";
                    mail.Body = "<span style=\"font-size:13px;font-family:Helvetica,Arial,sans-serif;\">Dear,</p></br>" +
                        "<span style=\"font-size:13px;font-family:Helvetica,Arial,sans-serif;\">Your suggestion have been declined. Please contact to your superior. Please see below for more details: </span></br>" +
                        "</br>" +
                        "<span style=\"font-size:13px;font-family:Helvetica,Arial,sans-serif;\">This is suggested by " + info.OwnerSuggestion + "_" + info.OwnerCode + ".</span></br>" +
                        "</br>" +
                        "<div class=\"row p-0 m - 0\">" +
                        "<span class=\"small\" style=\"text-decoration-line:underline\"><b>Current Status:</b></span>" +
                        "</div>" +
                        "<div class=\"row p-0 m-0\">" +
                        "<span class=\"small\" style=\"font-weight:bold\">" + info.CurrentStatus + "</span>" +
                        "</div>" +
                        "<br />" +
                        "<div class=\"row p-0 m-0\">" +
                        "<span class=\"small\" style=\"text-decoration-line:underline;font-style:oblique\"><b>Suggestion:</b></span>" +
                        "</div>" +
                        "<div class=\"row p-0 m-0\">" +
                        "<span class=\"small\" style=\"font-style:oblique\">" + info.SuggestionAction + "</span>" +
                        "</div>" +
                        "<br />" +
                         "<div class=\"row p-0 m-0\">" +
                        "<span class=\"small\" style=\"text-decoration-line:underline;font-style:oblique\"><b>Rejection Reason:</b></span>" +
                        "</div>" +
                        "<div class=\"row p-0 m-0\">" +
                        "<span class=\"small\" style=\"font-style:oblique;color:red\">" + info.Remark + "</span>" +
                        "</div>" +
                        "</br><hr><span style=\"color:#868e96; font-size:10px\">This is an auto-generated message. Please DO NOT reply.</span></br>";
                    mail.IsBodyHtml = true;
                    //using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 25))
                    using (SmtpClient smtp = new SmtpClient("mailgw01", 25))
                    {
                        //smtp.Credentials = new System.Net.NetworkCredential("cipf.autoemail.noreply@gmail.com", "Pf1234567");
                        //smtp.Credentials = new System.Net.NetworkCredential("hvho@vn.pepperl-fuchs.com", "Pf1234567");
                        //smtp.EnableSsl = true;
                        //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        //smtp.UseDefaultCredentials = false;
                        smtp.Send(mail);
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        //send email inform reject new suggestion
        public void sendEmailRejectProposal(string recieverEmail, string cc, string cc1, string remark,string Id)
        {
            var info = _context.CipfSuggestion.Where(x => x.SuggestionId == Id).FirstOrDefault();
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    //mail.From = new MailAddress("cipf.autoemail.noreply@gmail.com");
                    mail.From = new MailAddress("ContinuousImprovement.autoemail.noreply@vn.pepperl-fuchs.com");
                    mail.To.Add(recieverEmail);
                    mail.CC.Add(cc);
                    mail.CC.Add(cc1);
                    mail.Subject = "Continuous improvement _ Decline Suggestion";
                    mail.Body = "<span style=\"font-size:13px;font-family:Helvetica,Arial,sans-serif;\">Dear,</p></br>" +
                        "<span style=\"font-size:13px;font-family:Helvetica,Arial,sans-serif;\">Your proposal have been declined. Please contact to your superior. Please see below for more details: </span></br>" +
                         "</br>" +
                        "<span style=\"font-size:13px;font-family:Helvetica,Arial,sans-serif;\">This is suggested by " + info.OwnerAction + ".</span></br>" +
                        "</br>" +
                        "<div class=\"row p-0 m - 0\">" +
                        "<span class=\"small\" style=\"text-decoration-line:underline\"><b>Current Status:</b></span>" +
                        "</div>" +
                        "<div class=\"row p-0 m-0\">" +
                        "<span class=\"small\" style=\"font-weight:bold\">" + info.ActionDesc + "</span>" +
                        "</div>" +
                        "<br />" +
                        "<div class=\"row p-0 m-0\">" +
                        "<span class=\"small\" style=\"text-decoration-line:underline;font-style:oblique\"><b>Suggestion:</b></span>" +
                        "</div>" +
                        "<div class=\"row p-0 m-0\">" +
                        "<span class=\"small\" style=\"font-style:oblique\">" + info.ActionEffectiveness + "</span>" +
                        "</div>" +
                        "<br />" +
                         "<div class=\"row p-0 m-0\">" +
                        "<span class=\"small\" style=\"text-decoration-line:underline;font-style:oblique\"><b>Rejection Reason:</b></span>" +
                        "</div>" +
                        "<div class=\"row p-0 m-0\">" +
                        "<span class=\"small\" style=\"font-style:oblique;color:red\">" + info.Remark + "</span>" +
                        "</div>" +
                        "</br><hr><span style=\"color:#868e96; font-size:10px\">This is an auto-generated message. Please DO NOT reply.</span></br>";
                    mail.IsBodyHtml = true;
                    //using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 25))
                    using (SmtpClient smtp = new SmtpClient("mailgw01", 25))
                    {
                        //smtp.Credentials = new System.Net.NetworkCredential("cipf.autoemail.noreply@gmail.com", "Pf1234567");
                        //smtp.Credentials = new System.Net.NetworkCredential("hvho@vn.pepperl-fuchs.com", "Pf1234567");
                        //smtp.EnableSsl = true;
                        //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        //smtp.UseDefaultCredentials = false;
                        smtp.Send(mail);
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        //send email approve new suggest
        public void sendEmailApprovalSuggestion(string recieverEmail, string cc, CipfSuggestion info)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    //mail.From = new MailAddress("cipf.autoemail.noreply@gmail.com");
                    mail.From = new MailAddress("ContinuousImprovement.autoemail.noreply@vn.pepperl-fuchs.com");
                    mail.To.Add(recieverEmail);
                    mail.CC.Add(cc);
                    //mail.CC.Add("hvho");
                    mail.Subject = "Continuous improvement _ Suggestion are aprroved";
                    mail.Body = "<span style=\"font-size:13px;font-family:Helvetica,Arial,sans-serif;\">Dear,</p></br>" +
                        "<span style=\"font-size:13px;font-family:Helvetica,Arial,sans-serif;\">"+info.OwnerSuggestion+"_"+info.OwnerCode +"'s new suggestion have been approved.</span></br>" +
                        "</br><hr>" +
                         "<span class=\"small\" style=\"text-decoration-line:underline\"><b>Details:</b></span>" +
                        "<span style=\"font-size:13px;font-family:Helvetica,Arial,sans-serif;\">This is suggested by " + info.OwnerSuggestion + "_" + info.OwnerCode + ".</span></br>" +
                        "</br>" +
                        "<div class=\"row p-0 m - 0\">" +
                        "<span class=\"small\" style=\"text-decoration-line:underline\"><b>Current Status:</b></span>" +
                        "</div>" +
                        "<div class=\"row p-0 m-0\">" +
                        "<span class=\"small\" style=\"font-weight:bold\">" + info.CurrentStatus + "</span>" +
                        "</div>" +
                        "<br />" +
                        "<div class=\"row p-0 m-0\">" +
                        "<span class=\"small\" style=\"text-decoration-line:underline;font-style:oblique\"><b>Suggestion:</b></span>" +
                        "</div>" +
                        "<div class=\"row p-0 m-0\">" +
                        "<span class=\"small\" style=\"font-style:oblique\">" + info.SuggestionAction + "</span>" +
                        "</div>" +
                        "<hr>" +
                        "</br><span style=\"color:#868e96; font-size:10px\">This is an auto-generated message. Please DO NOT reply.</span></br>";
                    mail.IsBodyHtml = true;
                    //using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 25))
                    using (SmtpClient smtp = new SmtpClient("mailgw01", 25))
                    {
                        //smtp.Credentials = new System.Net.NetworkCredential("cipf.autoemail.noreply@gmail.com", "Pf1234567");
                        //smtp.Credentials = new System.Net.NetworkCredential("hvho@vn.pepperl-fuchs.com", "Pf1234567");
                        //smtp.EnableSsl = true;
                        //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        //smtp.UseDefaultCredentials = false;
                        smtp.Send(mail);
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        //send email approve new suggest
        public void sendEmailApprovalProposal(string recieverEmail, string cc, string cc1,string Id)
        {
            var info = _context.CipfSuggestion.Where(x => x.SuggestionId == Id).FirstOrDefault();
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    //mail.From = new MailAddress("cipf.autoemail.noreply@gmail.com");
                    mail.From = new MailAddress("ContinuousImprovement.autoemail.noreply@vn.pepperl-fuchs.com");
                    mail.To.Add(recieverEmail);
                    mail.CC.Add(cc);
                    mail.CC.Add(cc1);
                    //mail.CC.Add("hvho");
                    mail.Subject = "Continuous improvement _ The proposal are aprroved";
                    mail.Body = "<span style=\"font-size:13px;font-family:Helvetica,Arial,sans-serif;\">Dear,</p></br>" +
                        "<span style=\"font-size:13px;font-family:Helvetica,Arial,sans-serif;\">Your proposal have been approved.</span></br>" +
                        "</br><hr>" +
                         "<span class=\"small\" style=\"text-decoration-line:underline\"><b>Details:</b></span>" +
                        "<span style=\"font-size:13px;font-family:Helvetica,Arial,sans-serif;\">This is suggested by " + info.OwnerSuggestion + "_" + info.OwnerCode + ".</span></br>" +
                        "</br>" +
                        "<div class=\"row p-0 m - 0\">" +
                        "<span class=\"small\" style=\"text-decoration-line:underline\"><b>Current Status:</b></span>" +
                        "</div>" +
                        "<div class=\"row p-0 m-0\">" +
                        "<span class=\"small\" style=\"font-weight:bold\">" + info.CurrentStatus + "</span>" +
                        "</div>" +
                        "<br />" +
                        "<div class=\"row p-0 m-0\">" +
                        "<span class=\"small\" style=\"text-decoration-line:underline;font-style:oblique\"><b>Suggestion:</b></span>" +
                        "</div>" +
                        "<div class=\"row p-0 m-0\">" +
                        "<span class=\"small\" style=\"font-style:oblique\">" + info.SuggestionAction + "</span>" +
                        "</div>" +
                         "</br>" +
                        "<div class=\"row p-0 m - 0\">" +
                        "<span class=\"small\" style=\"text-decoration-line:underline\"><b>Action Description:</b></span>" +
                        "</div>" +
                        "<div class=\"row p-0 m-0\">" +
                        "<span class=\"small\" style=\"font-weight:bold\">" + info.ActionDesc + "</span>" +
                        "</div>" +
                        "<br />" +
                        "<div class=\"row p-0 m-0\">" +
                        "<span class=\"small\" style=\"text-decoration-line:underline;font-style:oblique\"><b>Action Effectiveness:</b></span>" +
                        "</div>" +
                        "<div class=\"row p-0 m-0\">" +
                        "<span class=\"small\" style=\"font-style:oblique\">" + info.ActionEffectiveness + "</span>" +
                        "</div></br>" +
                        "<hr>" +
                        "</br><span style=\"color:#868e96; font-size:10px\">This is an auto-generated message. Please DO NOT reply.</span></br>";
                    mail.IsBodyHtml = true;
                    //using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 25))
                    using (SmtpClient smtp = new SmtpClient("mailgw01", 25))
                    {
                        //smtp.Credentials = new System.Net.NetworkCredential("cipf.autoemail.noreply@gmail.com", "Pf1234567");
                        //smtp.Credentials = new System.Net.NetworkCredential("hvho@vn.pepperl-fuchs.com", "Pf1234567");
                        //smtp.EnableSsl = true;
                        //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        //smtp.UseDefaultCredentials = false;
                        smtp.Send(mail);
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Manage Commnet
        public async Task<List<CipfComment>> GetComment(int yr, int quarter)
        {
            List<CipfComment> cipfComments = await _context.CipfComment.Where(x => x.SubmitSuggestionDt.Year == yr).ToListAsync();
            if (quarter > 0)
            {
                cipfComments = cipfComments.Where(x => (x.SubmitSuggestionDt.Month + 2) / 3 == quarter).ToList();
            }
            return cipfComments;
        }

        //Add comment
        public async Task<CipfComment> AddComment(string id, DateTime submitSuggDt, string comment, string user)
        {
            CipfComment cipf = new CipfComment();
            cipf.SuggestionId = id;
            cipf.Comment = comment;
            cipf.SubmitSuggestionDt = submitSuggDt;
            cipf.DateComment = DateTime.Now;
            cipf.UserComment = user;
            _context.Add(cipf);
            await _context.SaveChangesAsync();
            return await Task.FromResult(cipf);
        }

        //Delete Comment
        public async Task<bool> DeleteComment(int id)
        {
            var ExistComment = _context.CipfComment.Where(x => x.Id == id).FirstOrDefault();
            if (ExistComment != null)
            {
                _context.Remove(ExistComment);
                await _context.SaveChangesAsync();
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }
        #endregion
    }
}
