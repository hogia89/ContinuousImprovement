using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ContinuousImprovement.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace ContinuousImprovement.Data
{
    [DisableRequestSizeLimit]
    public class ApprovalService
    {
        private readonly IConfiguration _configuration;
        private readonly MOMContext _context;


        public ApprovalService(IConfiguration configuration, MOMContext context)
        {

            _context = context;
            _configuration = configuration;

        }

        public async Task<List<CipfSuggestion>> GetMyTask(string userId, int year)
        {
            return await _context.CipfSuggestion.Where(x => x.ApproveActionBy.Contains(userId.Replace("AP\\", "")) && x.SubmitDate.Year == year && x.IndicatorOfStatus>=5).ToListAsync();
        }
        public async Task<List<CipfSuggestion>> GetMyApproval(string userId)
        {
            return await _context.CipfSuggestion
                .Where(x => (x.ApproveActionBy.Contains(userId.Replace("AP\\", "")) && x.IndicatorOfStatus >= 5) ||(x.Email.Contains(userId.Replace("AP\\", "")) && x.IndicatorOfStatus == 1)).ToListAsync();
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
        public async Task<bool> RejectSuggestion(string suggestionId, string currentUser)
        {
            var existSuggestion = _context.CipfSuggestion.Where(x => x.SuggestionId == suggestionId).FirstOrDefault();
            if (existSuggestion != null)
            {
                existSuggestion.Status = "Hành động không hiệu quả";
                existSuggestion.IndicatorOfStatus = 8;
                existSuggestion.ApproveDt = DateTime.Now;
                //existSuggestion.ApproveSuggBy = currentUser;
                await _context.SaveChangesAsync();
                return await Task.FromResult(true);
            }
            else
            {
                return await Task.FromResult(false);
            }
        }
        //Approval action
        public async Task<bool> ApprovalSuggestion(string suggestionId, string currentUser)
        {
            var existSuggestion = _context.CipfSuggestion.Where(x => x.SuggestionId == suggestionId).FirstOrDefault();
            if (existSuggestion != null)
            {
                existSuggestion.Status = "Hoàn tất";
                existSuggestion.IndicatorOfStatus = 6;
                existSuggestion.ApproveDt = DateTime.Now;
                //existSuggestion.ApproveActionBy = currentUser;
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

