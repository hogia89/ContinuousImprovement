using ContinuousImprovement.Model;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ContinuousImprovement.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class DownloadExcel : ControllerBase
    {
        [HttpPost("Download")]
        public async Task<IActionResult> Download([FromForm] string json)
        {
            List<CipfSuggestion> suggestionList = JsonSerializer.Deserialize<List<CipfSuggestion>>(json);
            byte[] fileContents;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var Sheet = package.Workbook.Worksheets.Add("Simulation");

                #region Header row
                Sheet.Cells["A1"].Value = "No";
                Sheet.Cells["B1"].Value = "OwnerSuggestion";
                Sheet.Cells["C1"].Value = "OwnerCode";
                Sheet.Cells["D1"].Value = "OwnerDept";
                Sheet.Cells["E1"].Value = "SubmitDate";
                Sheet.Cells["F1"].Value = "IndicatorOfStatus";
                Sheet.Cells["G1"].Value = "Status";
                Sheet.Cells["H1"].Value = "CurrentStatus";
                Sheet.Cells["I1"].Value = "SuggestionAction";
                Sheet.Cells["J1"].Value = "ExpectedBenefit";
                Sheet.Cells["K1"].Value = "CostCenterRecSug";
                Sheet.Cells["L1"].Value = "DeptRecSug";
                Sheet.Cells["M1"].Value = "Email";
                Sheet.Cells["N1"].Value = "ApproveDt";
                Sheet.Cells["O1"].Value = "ApproveSuggBy";
                Sheet.Cells["P1"].Value = "OwnerAction";
                Sheet.Cells["Q1"].Value = "AssignDt";
                Sheet.Cells["R1"].Value = "PlanFinishActionDt";
                Sheet.Cells["S1"].Value = "ImageUriBefore";
                Sheet.Cells["T1"].Value = "ActionDesc";
                Sheet.Cells["U1"].Value = "Cifno";
                Sheet.Cells["V1"].Value = "SuggestionId";
                Sheet.Cells["W1"].Value = "ApproveActionBy";
                Sheet.Cells["X1"].Value = "ImageUriAfter";
                Sheet.Cells["Y1"].Value = "ActionEffectiveness";
                Sheet.Cells["Z1"].Value = "Remark";
                Sheet.Cells["AA1"].Value = "InputBy";
                Sheet.Cells["A1:AA1"].Style.Font.Bold = true;
                #endregion

                #region Body rows
                int row = 2;
                int id = 1;
                foreach (var item in suggestionList)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = id;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.OwnerSuggestion;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.OwnerCode;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.OwnerDept;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.SubmitDate.ToShortDateString();
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.IndicatorOfStatus;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Status;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.CurrentStatus;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.SuggestionAction;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.ExpectedBenefit;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.CostCenterRecSug;
                    Sheet.Cells[string.Format("L{0}", row)].Value = item.DeptRecSug;
                    Sheet.Cells[string.Format("M{0}", row)].Value = item.Email;
                    if(item.ApproveDt.Year == 1900)
                    {
                        Sheet.Cells[string.Format("N{0}", row)].Value = null;
                    }
                    else
                    {
                        Sheet.Cells[string.Format("N{0}", row)].Value = item.ApproveDt.ToShortDateString();
                    }
                    
                    Sheet.Cells[string.Format("O{0}", row)].Value = item.ApproveSuggBy;
                    Sheet.Cells[string.Format("P{0}", row)].Value = item.OwnerAction;
                    if (item.AssignDt.Year == 1900)
                    {
                        Sheet.Cells[string.Format("Q{0}", row)].Value = null;
                    }
                    else
                    {
                        Sheet.Cells[string.Format("Q{0}", row)].Value = item.AssignDt.ToShortDateString();
                    }
                    if (item.PlanFinishActionDt.Year == 1900)
                    {
                        Sheet.Cells[string.Format("R{0}", row)].Value = null;
                    }
                    else
                    {
                        Sheet.Cells[string.Format("R{0}", row)].Value = item.PlanFinishActionDt.ToShortDateString();
                    }
                    Sheet.Cells[string.Format("S{0}", row)].Value = item.ImageUriBefore;
                    Sheet.Cells[string.Format("T{0}", row)].Value = item.ActionDesc;
                    Sheet.Cells[string.Format("U{0}", row)].Value = item.Cifno;
                    Sheet.Cells[string.Format("V{0}", row)].Value = item.SuggestionId;
                    Sheet.Cells[string.Format("W{0}", row)].Value = item.ApproveActionBy;
                    Sheet.Cells[string.Format("X{0}", row)].Value = item.ImageUriAfter;
                    Sheet.Cells[string.Format("Y{0}", row)].Value = item.ActionEffectiveness;
                    Sheet.Cells[string.Format("Z{0}", row)].Value = item.Remark;
                    Sheet.Cells[string.Format("AA{0}", row)].Value = item.InputBy;
                    row++;
                    id++;
                }
                #endregion

                fileContents = package.GetAsByteArray();
            }
            var file = new FileContentResult(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            file.FileDownloadName = "SuggestionList_" + DateTime.Now.ToString().Replace("/", "").Replace("-", "").Replace(":", "").Replace(".", "").Replace(" ", "") + ".xlsx";
            return file;
        }

        //[HttpPost("DownloadSupplierList")]
        //public async Task<IActionResult> DownloadSupplierList([FromForm] string json)
        //{
        //    List<EmdbMposupplier> mposuppliers = JsonSerializer.Deserialize<List<EmdbMposupplier>>(json);
        //    byte[] fileContents;
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //    using (var package = new ExcelPackage())
        //    {
        //        var Sheet = package.Workbook.Worksheets.Add("Supplier List");
        //        #region Header row
        //        Sheet.Cells["A1"].Value = "Id";
        //        Sheet.Cells["B1"].Value = "Mbsuno";
        //        Sheet.Cells["C1"].Value = "Supplierdesc";
        //        Sheet.Cells["D1"].Value = "Countitno";
        //        Sheet.Cells["E1"].Value = "Stockeuro";
        //        Sheet.Cells["F1"].Value = "Anndmdeur";
        //        Sheet.Cells["G1"].Value = "Soldeur";
        //        Sheet.Cells["H1"].Value = "Transfereur";
        //        Sheet.Cells["I1"].Value = "LastUpdated";
        //        Sheet.Cells["A1:I1"].Style.Font.Bold = true;
        //        #endregion

        //        #region Body rows
        //        int row = 2;
        //        int id = 1;
        //        foreach (var item in mposuppliers)
        //        {
        //            Sheet.Cells[string.Format("A{0}", row)].Value = id;
        //            Sheet.Cells[string.Format("B{0}", row)].Value = item.Mbsuno;
        //            Sheet.Cells[string.Format("C{0}", row)].Value = item.SupplierDesc;
        //            Sheet.Cells[string.Format("D{0}", row)].Value = item.Countitno;
        //            Sheet.Cells[string.Format("E{0}", row)].Value = item.Stockeur;
        //            Sheet.Cells[string.Format("F{0}", row)].Value = item.Anndmdeur;
        //            Sheet.Cells[string.Format("G{0}", row)].Value = item.Soldeur;
        //            Sheet.Cells[string.Format("H{0}", row)].Value = item.Transfereur;
        //            Sheet.Cells[string.Format("I{0}", row)].Value = item.LastUpdated;
        //            row++;
        //            id++;
        //        }
        //        #endregion

        //        fileContents = package.GetAsByteArray();

        //    }
        //    var file = new FileContentResult(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        //    file.FileDownloadName = "SupplierList_" + DateTime.Now.ToString().Replace("/", "").Replace("-", "").Replace(":", "").Replace(".", "").Replace(" ", "") + ".xlsx";
        //    return file;
        //}
    }
}
