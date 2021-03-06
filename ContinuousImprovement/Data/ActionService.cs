﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ContinuousImprovement.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace ContinuousImprovement.Data
{
    [DisableRequestSizeLimit]
    public class ActionService
    {
        private readonly IConfiguration _configuration;
        private readonly MOMContext _context;
        

        public ActionService(IConfiguration configuration, MOMContext context)
        {

            _context = context;
            _configuration = configuration;
            
        }

        public async Task<List<CipfSuggestion>> GetMyTask(string userId,int year)
        {
            return await _context.CipfSuggestion.Where(x => x.OwnerAction.Contains(userId.Replace("AP\\", ""))&&x.SubmitDate.Year==year).ToListAsync();
        }
      
        public async Task<List<ProductionDepartment>> GetListOfDepartments()
        {
            return await _context.ProductionDepartment.ToListAsync();
        }

        #region Manage like post
        //Save new like
        public Task<CipfListOfLike> LikeSuggestion(string Id, string currentUser, string userBeLiked,DateTime submitDt)
        {
            CipfListOfLike cipf = new CipfListOfLike();
            cipf.SuggestionId = Id;
            cipf.SubmitSuggestionDt = submitDt;
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
        public async Task<List<CipfListOfLike>> GetListOfLike(int yr)
        {
            List<CipfListOfLike> cipfListOfLikes = await _context.CipfListOfLike.Where(x => x.SubmitSuggestionDt.Year == yr).ToListAsync();
            return cipfListOfLikes;
        }
        #endregion

        #region button fuction
        public async Task<bool> UpdatePlanDt(string suggestionId, DateTime planDt, string currentUser)
        {
            var existSuggestion = _context.CipfSuggestion.Where(x => x.SuggestionId == suggestionId).FirstOrDefault();
            if (existSuggestion != null)
            {
                existSuggestion.Status = "Đang thực hiện";
                existSuggestion.IndicatorOfStatus = 4;
                existSuggestion.ApproveDt = DateTime.Now;
                existSuggestion.ApproveSuggBy = currentUser;
                existSuggestion.PlanFinishActionDt = planDt;
                await _context.SaveChangesAsync();
                return await Task.FromResult(true);
            }
            else
            {
                return await Task.FromResult(false);
            }
        }
        public async Task<bool> RejectSuggestion(string suggestionId, string currentUser)
        {
            var existSuggestion = _context.CipfSuggestion.Where(x => x.SuggestionId == suggestionId).FirstOrDefault();
            if (existSuggestion != null)
            {
                existSuggestion.Status = "Không thực hiện";
                existSuggestion.IndicatorOfStatus = 7;
                existSuggestion.ApproveDt = DateTime.Now;
                existSuggestion.ApproveSuggBy = currentUser;
                await _context.SaveChangesAsync();
                return await Task.FromResult(true);
            }
            else
            {
                return await Task.FromResult(false);
            }
        }
        //Update action
        public async Task<bool> UpdateAction(string Id, string actionDesc, string actionEff, string cifNo, string img,string approvalBy)
        {
            var ExistSuggestion = _context.CipfSuggestion.Where(x => x.SuggestionId == Id).FirstOrDefault();
            if (ExistSuggestion != null)
            {
                ExistSuggestion.ActionDesc = actionDesc;
                ExistSuggestion.ActionEffectiveness = actionEff;
                ExistSuggestion.Cifno = cifNo;
                ExistSuggestion.ImageUriAfter = Id+".jpg";
                ExistSuggestion.ApproveActionBy = approvalBy;
                ExistSuggestion.Status = "Chờ duyệt hành động";
                ExistSuggestion.IndicatorOfStatus = 5;
                sendEmail(ExistSuggestion.OwnerAction, approvalBy, Id);
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

        #region send email
        //Send email
        private void sendEmail(string senderEmail, string recieverEmail, string id)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("ContinuousImprovement.autoemail.noreply@vn.pepperl-fuchs.com");
                    mail.To.Add(recieverEmail);
                    mail.CC.Add(senderEmail);
                    mail.Subject = "Continuous improvement _ New Suggestion _ " + id;
                    mail.Body = "<span style=\"font-size:13px;font-family:Helvetica,Arial,sans-serif;\">Dear Sir,</p></br>" +
                        "<span style=\"font-size:13px;font-family:Helvetica,Arial,sans-serif;\">Please take a look and approve this action. Here is the link to approve:</span></br>" +
                        "</br><a style=\"text-decoration:none;font-size:13px;font-family:Helvetica,Arial,sans-serif;color:#0275d8;border-radius:5px;padding:6px 12px 6px 12px;border:1px solid #0071c5;display:inline-block\"" +
                        " href=\"172.22.0.21:8384/myapproval\">Action ID - " + id + "</a></br><hr>" +
                        "</br><span style=\"color:#868e96; font-size:10px\">This is an auto-generated message. Please DO NOT reply.</span></br>";
                    mail.IsBodyHtml = true;
                    using (SmtpClient smtp = new SmtpClient("mailgw01", 25))
                    {
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
        #endregion
        #region Manage Commnet
        public async Task<List<CipfComment>> GetComment(int yr)
        {
            List<CipfComment> cipfComments = await _context.CipfComment.Where(x => x.SubmitSuggestionDt.Year == yr).ToListAsync();
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

