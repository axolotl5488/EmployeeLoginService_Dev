using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BAModel;
using DataModel;
using Webportal.Models;
using BAModel.Common;
using System.Data.Entity;
using System.Net.Mail;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Webportal.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            DailyUserReport_Model model = new DailyUserReport_Model();

            EmployeeLoginEntities db = new EmployeeLoginEntities();
            List<User> users = db.Users.ToList();

            DateTime currentdate = DateTime.Now.AddDays(-1);
            List<PunchIn> records = db.PunchIns.Where(x=> DbFunctions.TruncateTime(x.PunchinTime) == DbFunctions.TruncateTime(currentdate)).ToList();

            List<AppUserLeave> leaves = new List<AppUserLeave>();
            try
            {
                 leaves = db.AppUserLeaves.Where(x => DbFunctions.TruncateTime(x.StartDate) >= DbFunctions.TruncateTime(currentdate)
                && DbFunctions.TruncateTime(x.EndDate) <= DbFunctions.TruncateTime(currentdate)).ToList();
            }
            catch
            {
                leaves = new List<AppUserLeave>();
            }

            foreach (User obj in users)
            {
                DailyUserReport_Detail_Model map = new DailyUserReport_Detail_Model();
                map.UserID = obj.UserId;
                map.UserName = obj.FirstName + " " + obj.LastName;
                PunchIn record = records.FirstOrDefault(x => x.UserId == obj.UserId);
                map.record = record;
                model.records.Add(map);
            }


            List<DailyUserReport_Detail_Model> notnullablerecords = model.records.Where(x => x.record != null).OrderBy(x => x.UserName).ToList();
            List<DailyUserReport_Detail_Model> nullablerecords = model.records.Where(x => x.record == null).OrderBy(x => x.UserName).ToList();
            model.records = notnullablerecords;
            model.records.AddRange(nullablerecords);

            model.TotalEarlyOut = model.records.Where(x => x.record != null && x.record.EarlyPunchout == true).Count();
            model.TotalIn = model.records.Where(x => x.record != null).Count();
            model.TotalInLocation = model.records.Where(x => x.record != null && x.record.PunchinType == true).Count();
            model.TotalInType = model.records.Where(x => x.record != null && x.record.PunchinType != null && x.record.PunchinType == true).Count();
            model.TotalLateIn = model.records.Where(x => x.record != null && x.record.LatePunchin == true).Count();
            model.TotalOut = model.records.Where(x => x.record != null && x.record.PunchoutTime != null && x.record.SystemPunchout == false).Count();
            model.TotalOutLocation = model.records.Where(x => x.record != null && x.record.PunchoutType == true).Count();

            model.TotalOutType = model.records.Where(x => x.record != null && x.record.PunchoutType == true).Count();
            model.TotalSystemOut = model.records.Where(x => x.record != null && x.record.SystemPunchout == true).Count();

            byte[] bytes = DailyReport(model, leaves);

            string name = "EmployeeDailyReport_" + DateTime.Now.ToString("MMddyyyhhmmss") + ".xlsx";
            return File(bytes, "application/excel", name);
        }


        public byte[] DailyReport(DailyUserReport_Model model, List<AppUserLeave> leaves)
        {

            try
            {


                string pageHeader = "Axolotl - Employee Daily Report";
                using (ExcelPackage pckExport = new ExcelPackage())
                {

                    #region Attractions List
                    ExcelWorksheet xlSheet = pckExport.Workbook.Worksheets.Add(pageHeader);
                    try
                    {
                        xlSheet.PrinterSettings.LeftMargin = 0.57M;
                        xlSheet.PrinterSettings.RightMargin = 0.41M;
                        xlSheet.PrinterSettings.HeaderMargin = 0.28M;
                        xlSheet.PrinterSettings.TopMargin = 1.28M;
                        xlSheet.PrinterSettings.BottomMargin = 0.7M;
                        xlSheet.PrinterSettings.FooterMargin = 0.3M;
                        xlSheet.PrinterSettings.Orientation = eOrientation.Portrait;
                        int padding = 1;
                        int miRow = 1 + padding;
                        int MinCol = 1 + padding;
                        int MaxCol = 13 + padding;
                        int TotalSearchColumn = 12;
                        int MaxColWithSearchCriteria = 11 + TotalSearchColumn + padding;

                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Value = "Daily REPORT";
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Merge = true;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0070C0"));
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 12;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = true;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));

                        miRow = miRow + 1;

                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Value = "[Name of Report Here]";
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Merge = true;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 24;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = true;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));

                        miRow = miRow + 1;

                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Value = "Report generated on " + (DateTime.Now).ToString("MM/dd/yyyy") + " at " + (DateTime.Now).ToString("HH:mm tt").ToLower();
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Merge = true;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 14;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = false;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#525252"));

                        miRow = miRow + 1;

                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Value = "Powered by Axolotl";
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Merge = true;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0070C0"));
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 14;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = true;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));

                        miRow = miRow + 1;



                        int k = 0;
                        // Add Time Range Search Filter - Tilte
                        for (int c = 0; c <= TotalSearchColumn;)
                        {


                            if (c == 0)
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Value = "Average Working Hours";
                            else if (c == 2)
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Value = "Total Employee";
                            else if (c == 4)
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Value = "Total Employee on Leave";
                            else if (c == 6)
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Value = "Total Employee Punche In";
                            else if (c == 8)
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Value = "Total Employee Punch Out";
                            else if (c == 10)
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Value = "Total Employee Late In";
                            else if (c == 12)
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Value = "Total Employee Ealry Out";

                            xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Merge = true;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#D3D3D3"));
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Style.Font.Name = "Sitka Text";
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Style.Font.Size = 12;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Style.Font.Bold = true;


                            c = c + 2;
                        }

                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#D3D3D3"));
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 12;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = true;

                        miRow = miRow + 1;

                        k = 0;

                        TimeSpan tp = TimeSpan.FromHours(model.records.Where(x => x.record != null && x.record.PunchoutTime != null).Average(x => x.record.PunchoutTime.Value.Subtract(x.record.PunchinTime).TotalHours));
                        // Add Time Range Search Filter - Data
                        for (int c = 0; c <= TotalSearchColumn;)
                        {

                            if (c == 0) // Time Range
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Value = tp.Hours + ":" + tp.Minutes + ":" + tp.Seconds;
                            else if (c == 2) // Type
                            {
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Value = model.records.Count;
                            }
                            else if (c == 4) // Operators
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Value = leaves.Count;
                            else if (c == 6) // Operators
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Value = model.TotalIn;
                            else if (c == 8) // Operators
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Value = model.TotalOut;
                            else if (c == 10) // Operators
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Value = model.TotalLateIn;
                            else if (c == 12) // Operators
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Value = model.TotalEarlyOut;

                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Merge = true;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#D3D3D3"));
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Style.Font.Name = "Sitka Text";
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Style.Font.Size = 12;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 1].Style.Font.Bold = false;


                            c = c + 2;
                        }

                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#D3D3D3"));
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 12;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = false;

                        #region Columns and Sheet Config

                        for (int i = 1 + padding; i <= MaxCol; i++)
                        {
                            xlSheet.Column(i).Width = 25.83;
                        }

                        miRow += 1;

                        //'header alignment
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        //'border
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                        //Font Bold
                        //xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = true;

                        //'Background Color
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0070C0"));


                        //Font Name
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";


                        //Font Size
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 14;

                        //Font Color
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));
                        xlSheet.Cells[miRow, 1 + padding].Value = "Employees";
                        xlSheet.Cells[miRow, 2 + padding].Value = "Puncn In";
                        xlSheet.Cells[miRow, 3 + padding].Value = "Puncn Out";
                        xlSheet.Cells[miRow, 4 + padding].Value = "In Type";
                        xlSheet.Cells[miRow, 5 + padding].Value = "In Location";
                        xlSheet.Cells[miRow, 6 + padding].Value = "Late In";
                        xlSheet.Cells[miRow, 7 + padding].Value = "Late In Reason";
                        xlSheet.Cells[miRow, 8 + padding].Value = "Out Type";
                        xlSheet.Cells[miRow, 9 + padding].Value = "Out Location";
                        xlSheet.Cells[miRow, 10 + padding].Value = "Early Out";
                        xlSheet.Cells[miRow, 11 + padding].Value = "Early Out Reason";
                        xlSheet.Cells[miRow, 12 + padding].Value = "System Punch Out";

                        miRow += 1;

                        #endregion

                        #region Bind Data

                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFF8E5"));
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 18;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = true;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#BD6427"));
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[miRow, 1 + padding].Style.Font.Size = 11;
                        xlSheet.Cells[miRow, 1 + padding].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlSheet.Cells[miRow, 1 + padding].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        xlSheet.Cells[miRow, 1 + padding].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                        xlSheet.Cells[miRow, 1 + padding].Value = "TOTALS";

                        xlSheet.Cells[miRow, 2 + padding].Value = model.TotalIn;
                        xlSheet.Cells[miRow, 3 + padding].Value = model.TotalOut+"/"+ model.TotalIn;
                        xlSheet.Cells[miRow, 4 + padding].Value = model.TotalInType+"/"+ model.TotalIn;
                        xlSheet.Cells[miRow, 5 + padding].Value = model.TotalInLocation + "/" + model.TotalIn;
                        xlSheet.Cells[miRow, 6 + padding].Value = model.TotalLateIn + "/" + model.TotalIn;
                        xlSheet.Cells[miRow, 7 + padding].Value = " - ";
                        xlSheet.Cells[miRow, 8 + padding].Value = model.TotalOutType + "/" + model.TotalIn; 
                        xlSheet.Cells[miRow, 9 + padding].Value = model.TotalOutLocation + "/" + model.TotalIn; 
                        xlSheet.Cells[miRow, 10 + padding].Value =model.TotalEarlyOut + "/" + model.TotalIn;
                        xlSheet.Cells[miRow, 11 + padding].Value =" - ";
                        xlSheet.Cells[miRow, 12 + padding].Value = model.TotalSystemOut + "/" + model.TotalIn;

                        miRow += 1;

                        foreach (DailyUserReport_Detail_Model obj in model.records)
                        {

                            xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                            xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                            xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            xlSheet.Cells[miRow, 1 + padding].Style.Font.Bold = true;
                            xlSheet.Cells[miRow, 1 + padding].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            xlSheet.Cells[miRow, 1 + padding].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            xlSheet.Cells[miRow, 1 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0070C0"));
                            xlSheet.Cells[miRow, 1 + padding].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, 1 + padding].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, 1 + padding].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, 1 + padding].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, 1 + padding].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));

                            xlSheet.Cells[miRow, 1 + padding].Value = obj.UserName;

                            if (obj.record != null)
                            {
                                AppUserLeave userleave = leaves.FirstOrDefault(x => x.UserID == obj.UserID);
                                if (userleave == null)
                                {
                                    xlSheet.Cells[miRow, 2 + padding].Value = obj.record.PunchinTime.ToString("hh:mm tt");
                                    xlSheet.Cells[miRow, 3 + padding].Value = obj.record.PunchoutTime == null ? " - " : obj.record.PunchoutTime.Value.ToString("hh:mm tt");
                                    xlSheet.Cells[miRow, 4 + padding].Value = Convert.ToBoolean(obj.record.PunchinType) ? "Inside" : "Outside";
                                    xlSheet.Cells[miRow, 5 + padding].Value = obj.record.PILocationId;
                                    xlSheet.Cells[miRow, 6 + padding].Value = obj.record.LatePunchin ? "Yes" : "No";

                                    xlSheet.Cells[miRow, 7 + padding].Style.WrapText = true;
                                    xlSheet.Cells[miRow, 7 + padding].Value = obj.record.LatePunchinReason;

                                    xlSheet.Cells[miRow, 8 + padding].Value = Convert.ToBoolean(obj.record.PunchoutType) ? "Inside" : "Outside";
                                    xlSheet.Cells[miRow, 9 + padding].Value = obj.record.POLocationId;
                                    xlSheet.Cells[miRow, 10 + padding].Value = obj.record.EarlyPunchout ? "Yes" : "No";
                                    xlSheet.Cells[miRow, 11 + padding].Value = obj.record.EarlyPunchoutReason;
                                    xlSheet.Cells[miRow, 11 + padding].Value = obj.record.SystemPunchout ? "Yes" : "No";
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, MinCol + 1, miRow, MaxCol].Merge = true;
                                    xlSheet.Cells[miRow, MinCol + 1, miRow, MaxCol].Value = "Employee on Leave";
                                }
                            }
                            else
                            {
                                xlSheet.Cells[miRow, MinCol+1, miRow, MaxCol].Merge = true;
                                xlSheet.Cells[miRow, MinCol+1, miRow, MaxCol].Value = "No Records";
                            }

                            miRow += 1;
                        }

                        xlSheet.Cells.AutoFitColumns();

                        byte[] bytes = pckExport.GetAsByteArray();

                        return bytes;
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                    #endregion
                }


            }
            catch (Exception ex)
            {
              
            }
            return null;

        }


        public ActionResult Index1()
        {

            DateTime MonthDate = DateTime.Now;
            DateTime startdateOfMonth = new DateTime(MonthDate.Year, MonthDate.Month, 1, 0, 0, 0);
            DateTime enddateOfMonth = startdateOfMonth.AddDays(7);
            //DateTime enddateOfMonth = startdateOfMonth.AddMonths(1).AddDays(-1);

            int TotalWorkingDaysOfmonth = enddateOfMonth.Subtract(startdateOfMonth).Days + 1;

            EmployeeLoginEntities db = new EmployeeLoginEntities();
            List<Location> location_list = db.Locations.ToList();
            List<Holiday> holiday_list = db.Holidays.ToList();

            List<AppUserLeave> AppUserLeave_list = new List<AppUserLeave>();
            try
            {
                AppUserLeave_list = db.AppUserLeaves.Where(x => DbFunctions.TruncateTime(x.StartDate) >= DbFunctions.TruncateTime(startdateOfMonth)
                && DbFunctions.TruncateTime(x.EndDate) <= DbFunctions.TruncateTime(enddateOfMonth)
                ).ToList();
            }
            catch
            {
                AppUserLeave_list = new List<AppUserLeave>();
            }

            User obj_user = db.Users.Find(160);
            Report_Model model = GetModel(MonthDate, startdateOfMonth, enddateOfMonth, 160);
            model.UserName = obj_user.FirstName + " " + obj_user.LastName;



            byte[] bytes = ExportExcel(model, MonthDate, startdateOfMonth, enddateOfMonth, TotalWorkingDaysOfmonth, AppUserLeave_list, holiday_list, location_list);

            string name = "EmployeeMonthlyReport_" + DateTime.Now.ToString("MMddyyyhhmmss") + ".xlsx";
            //SendEmail(bytes, name);
            return File(bytes, "application/excel", name);
        }

        public byte[] ExportExcel(Report_Model model, DateTime MonthDate, DateTime startdateOfMonth, DateTime enddateOfMonth, int TotalWorkingDaysOfmonth, List<AppUserLeave> AppUserLeave_list, List<Holiday> holiday_list, List<Location> location_list)
        {
            try
            {
                string pageHeader = "Data";
                using (OfficeOpenXml.ExcelPackage pckExport = new OfficeOpenXml.ExcelPackage())
                {
                    #region  List
                    #region  Headers
                    OfficeOpenXml.ExcelWorksheet xlSheet = pckExport.Workbook.Worksheets.Add(pageHeader);
                    xlSheet.Name = model.UserName;
                    xlSheet.PrinterSettings.LeftMargin = 0.57M;
                    xlSheet.PrinterSettings.RightMargin = 0.41M;
                    xlSheet.PrinterSettings.HeaderMargin = 0.28M;
                    xlSheet.PrinterSettings.TopMargin = 1.28M;
                    xlSheet.PrinterSettings.BottomMargin = 0.7M;
                    xlSheet.PrinterSettings.FooterMargin = 0.3M;
                    xlSheet.PrinterSettings.Orientation = OfficeOpenXml.eOrientation.Portrait;

                    int padding = 1;
                    int miRow = 1 + padding;
                    int MinCol = 1 + padding;
                    int MaxCol = TotalWorkingDaysOfmonth + 1 + padding;
                    int TotalSearchColumn = TotalWorkingDaysOfmonth + 1 + padding;

                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Value = "EMPLOYEE PUNCH IN-OUT MONTHLY REPORT";
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Merge = true;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0070C0"));
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 12;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = true;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));

                    miRow = miRow + 1;

                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Value = model.UserName + " - Work Report";
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Merge = true;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 24;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = true;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));

                    miRow = miRow + 1;

                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Value = "Report generated on " + (DateTime.Now).ToString("MM/dd/yyyy") + " at " + (DateTime.Now).ToString("HH:mm tt").ToLower();
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Merge = true;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 14;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = false;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#525252"));

                    miRow = miRow + 1;

                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Value = "Powered by NKTPL";
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Merge = true;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0070C0"));
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 14;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = true;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));

                    miRow = miRow + 1;

                    // Add Time Range Search Filter - Tilte
                    //for (int c = 0; c <= TotalSearchColumn;)
                    for (int c = 0; c <= 30;)
                    {
                        if (c == 0)
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = "Total Working Day";
                        else if (c == 3)
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = "Total Working Hours";
                        else if (c == 6)
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = "Average Workng Hours";
                        else if (c == 9)
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = "Total Leaves";
                        else if (c == 12)
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = "Total Late In";
                        else if (c == 15)
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = "Total Early Out";
                        else if (c == 18)
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = "Total System Out";
                        else if (c == 21)
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = "Total Outside Punch In";
                        else if (c == 24)
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = "Total Outside Punch Out";


                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Merge = true;
                        xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#D3D3D3"));
                        xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Font.Name = "Sitka Text";
                        xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Font.Size = 12;
                        xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                        xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Font.Bold = true;
                        c = c + 3;
                    }

                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#D3D3D3"));
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 12;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = true;

                    miRow = miRow + 1;

                    // Add Time Range Search Filter - Data
                    for (int c = 0; c <= 30;)
                    {
                        {
                            if (c == 0) // Payroll
                            {
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = model.TotalWorkingDays;
                            }
                            else if (c == 3) // Location
                            {
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = model.TotalWorkingHours;
                            }
                            else if (c == 6) // Jobclass
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = model.AverageWorkingHours;
                            else if (c == 9)
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = model.TotalLeaves;//"Total Leaves";
                            else if (c == 12)
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = model.TotalLatePunch_In;//"Total Late In";
                            else if (c == 15)
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = model.TotalEarlyPunch_Out;//"Total Early Out";
                            else if (c == 18)
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = model.TotalSystemPunchOut;//"Total System Out";
                            else if (c == 21)
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = model.TotalPunchIn_Outside;//"Total Outside Punch In";
                            else if (c == 24)
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = model.TotalPunchOut_Outside; //"Total Outside Punch Out";

                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Merge = true;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#D3D3D3"));
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Font.Name = "Sitka Text";
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Font.Size = 12;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Font.Bold = false;
                        }

                        c = c + 3;
                    }

                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#D3D3D3"));
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 12;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = false;
                    #endregion

                    #region Columns and Sheet Config

                    for (int i = 1 + padding; i <= MaxCol; i++)
                    {
                        xlSheet.Column(i).Width = 25.83;
                    }

                    miRow += 1;

                    //'header alignment
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    //'border
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;


                    //Font Bold
                    //xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = true;

                    //'Background Color
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0070C0"));


                    //Font Name
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";


                    //Font Size
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 12;


                    //Font Color
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));

                    int headercolnumber = 1;
                    xlSheet.Cells[miRow, headercolnumber + padding].Value = "Employee";
                    for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                    {
                        if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                        {
                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                        }
                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.WrapText = true;
                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.TextRotation = 90;
                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Value = startdateOfMonth.AddDays(i).ToString("dd-MM-yy") + Environment.NewLine + startdateOfMonth.AddDays(i).ToString("ddd");

                        // User Leave
                        AppUserLeave obj = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                        if (obj != null)
                        {
                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                        }
                        headercolnumber++;
                    }


                    miRow += 1;

                    #endregion

                    #region Bind Data

                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFF8E5"));
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 18;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = true;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#BD6427"));
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                    xlSheet.Cells[miRow, 1 + padding].Style.Font.Size = 11;
                    xlSheet.Cells[miRow, 1 + padding].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    xlSheet.Cells[miRow, 1 + padding].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    xlSheet.Cells[miRow, 1 + padding].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                    xlSheet.Cells[miRow, 1 + padding].Value = "Prashan Vyash";


                    headercolnumber = 1;
                    for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                    {
                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Font.Size = 11;
                        PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                        if (obj == null || obj.PunchoutTime == null)
                        {
                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Value = "0";
                        }
                        else
                        {
                            double hours = obj.PunchoutTime.Value.Subtract(obj.PunchinTime).TotalHours;
                            TimeSpan sp_hours = TimeSpan.FromHours(hours);
                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Value = sp_hours.Hours + ":" + sp_hours.Minutes + ":" + sp_hours.Seconds;
                        }

                        if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                        {
                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                        }
                        headercolnumber++;
                    }
                    miRow += 1;

                    for (int j = 0; j < 11; j++)
                    {

                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;


                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        xlSheet.Cells[miRow, 1 + padding].Style.Font.Bold = true;
                        xlSheet.Cells[miRow, 1 + padding].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        xlSheet.Cells[miRow, 1 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        xlSheet.Cells[miRow, 1 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0070C0"));
                        xlSheet.Cells[miRow, 1 + padding].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, 1 + padding].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, 1 + padding].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, 1 + padding].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, 1 + padding].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));

                        if (j == 0)
                        {
                            xlSheet.Cells[miRow, 1 + padding].Value = "Puncn In";

                            for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                            {
                                PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                if (obj == null)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = "-";
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = obj.PunchinTime.ToString("hh:mm tt");
                                }
                                if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                }

                                // User Leave
                                AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                if (objleave != null)
                                {
                                    if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                    }
                                    else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                    }
                                }
                            }
                        }

                        if (j == 1)
                        {
                            xlSheet.Cells[miRow, 1 + padding].Value = "Puncn Out";

                            for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                            {

                                PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                if (obj == null)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = "-";
                                    //xlSheet.Cells[miRow + 2, i + 2 + padding].Value = "-";
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = obj.PunchoutTime == null ? " - " : obj.PunchoutTime.Value.ToString("hh:mm tt");
                                }
                                if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                }

                                // User Leave
                                AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                if (objleave != null)
                                {
                                    if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                    }
                                    else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                    }
                                }
                            }
                        }

                        if (j == 2)
                        {
                            xlSheet.Cells[miRow, 1 + padding].Value = "In Type";

                            for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                            {
                                xlSheet.Cells[miRow, i + 2 + padding].Style.Font.Size = 8;
                                PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                if (obj == null)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = "-";
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = obj.PunchinType == null ? " - " : (obj.PunchinType.Value ? "Inside" : "Outside");
                                }
                                if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#BACFAB"));
                                }

                                // User Leave
                                AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                if (objleave != null)
                                {
                                    if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                    }
                                    else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                    }
                                }
                            }
                        }
                        if (j == 3)
                        {
                            xlSheet.Cells[miRow, 1 + padding].Value = "In Location";

                            for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                            {
                                xlSheet.Cells[miRow, i + 2 + padding].Style.Font.Size = 8;
                                PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                if (obj == null)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = "-";
                                }
                                else
                                {
                                    Location obj_location = location_list.FirstOrDefault(x => x.LocationId == obj.PILocationId);
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = obj_location == null ? " - " : obj_location.LocationName;
                                }
                                if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#BACFAB"));
                                }

                                // User Leave
                                AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                if (objleave != null)
                                {
                                    if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                    }
                                    else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                    }
                                }
                                xlSheet.Cells[miRow, i + 2 + padding].Style.WrapText = true;
                            }
                        }
                        if (j == 4)
                        {
                            xlSheet.Cells[miRow, 1 + padding].Value = "Late In";

                            for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                            {
                                xlSheet.Cells[miRow, i + 2 + padding].Style.Font.Size = 8;
                                PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                if (obj != null)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = obj.LatePunchin ? "Yes" : "No";
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = " - ";
                                }
                                if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#BACFAB"));
                                }

                                // User Leave
                                AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                if (objleave != null)
                                {
                                    if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                    }
                                    else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                    }
                                }
                                xlSheet.Cells[miRow, i + 2 + padding].Style.WrapText = true;
                            }
                        }
                        if (j == 5)
                        {
                            xlSheet.Cells[miRow, 1 + padding].Value = "Late In Reason";

                            for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                            {
                                xlSheet.Cells[miRow, i + 2 + padding].Style.Font.Size = 8;
                                PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                if (obj != null)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = obj.LatePunchinReason;
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = " - ";
                                }
                                if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#BACFAB"));
                                }
                                xlSheet.Cells[miRow, i + 2 + padding].Style.WrapText = true;
                                xlSheet.Cells[miRow, i + 2 + padding].Style.ShrinkToFit = true;
                            }
                        }

                        if (j == 6)
                        {
                            xlSheet.Cells[miRow, 1 + padding].Value = "Out Type";

                            for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                            {
                                xlSheet.Cells[miRow, i + 2 + padding].Style.Font.Size = 8;
                                PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                if (obj == null)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = "-";
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = obj.PunchoutType == null ? " - " : (obj.PunchoutType.Value ? "Inside" : "Outside");
                                }
                                if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#98ceb8"));
                                }

                                // User Leave
                                AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                if (objleave != null)
                                {
                                    if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                    }
                                    else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                    }
                                }
                            }
                        }
                        if (j == 7)
                        {
                            xlSheet.Cells[miRow, 1 + padding].Value = "Out Location";

                            for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                            {
                                xlSheet.Cells[miRow, i + 2 + padding].Style.Font.Size = 8;
                                PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                if (obj == null)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = "-";
                                }
                                else
                                {
                                    Location obj_location = location_list.FirstOrDefault(x => x.LocationId == obj.POLocationId);
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = obj_location == null ? " - " : obj_location.PlaceName;
                                }
                                if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#98ceb8"));
                                }

                                // User Leave
                                AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                if (objleave != null)
                                {
                                    if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                    }
                                    else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                    }
                                }
                                xlSheet.Cells[miRow, i + 2 + padding].Style.WrapText = true;
                            }
                        }
                        if (j == 8)
                        {
                            xlSheet.Cells[miRow, 1 + padding].Value = "Early Out";

                            for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                            {
                                xlSheet.Cells[miRow, i + 2 + padding].Style.Font.Size = 8;
                                PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                if (obj != null)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = obj.EarlyPunchout ? "Yes" : "No";
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = " - ";
                                }
                                if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#98ceb8"));
                                }

                                // User Leave
                                AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                if (objleave != null)
                                {
                                    if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                    }
                                    else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                    }
                                }
                                xlSheet.Cells[miRow, i + 2 + padding].Style.WrapText = true;
                            }
                        }
                        if (j == 9)
                        {
                            xlSheet.Cells[miRow, 1 + padding].Value = "Early Out Reason";

                            for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                            {
                                xlSheet.Cells[miRow, i + 2 + padding].Style.Font.Size = 8;
                                PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                if (obj != null)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = obj.EarlyPunchoutReason;
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = " - ";
                                }
                                if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#98ceb8"));
                                }

                                // User Leave
                                AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                if (objleave != null)
                                {
                                    if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                    }
                                    else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                    }
                                }
                                xlSheet.Cells[miRow, i + 2 + padding].Style.WrapText = true;
                                xlSheet.Cells[miRow, i + 2 + padding].Style.ShrinkToFit = true;
                            }
                        }

                        if (j == 10)
                        {
                            xlSheet.Cells[miRow, 1 + padding].Value = "System Punch Out";

                            for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                            {
                                xlSheet.Cells[miRow, i + 2 + padding].Style.Font.Size = 8;
                                PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                if (obj != null)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = obj.SystemPunchout ? "Yes" : "No";
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = " - ";
                                }
                                if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                }

                                // User Leave
                                AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                if (objleave != null)
                                {
                                    if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                    }
                                    else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                    }
                                }
                                xlSheet.Cells[miRow, i + 2 + padding].Style.WrapText = true;
                                xlSheet.Cells[miRow, i + 2 + padding].Style.ShrinkToFit = true;
                            }
                        }

                        miRow += 1;
                    }

                    ///Set Autofil columns



                    #endregion
                    xlSheet.Cells.AutoFitColumns();

                    byte[] bytes = pckExport.GetAsByteArray();

                    return bytes;
                    #endregion
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public byte[] ExportExcel1(Report_Model model, DateTime MonthDate, DateTime startdateOfMonth, DateTime enddateOfMonth, int TotalWorkingDaysOfmonth, List<AppUserLeave> AppUserLeave_list, List<Holiday> holiday_list, List<Location> location_list)
        {
            try
            {
                string pageHeader = "Data";
                using (OfficeOpenXml.ExcelPackage pckExport = new OfficeOpenXml.ExcelPackage())
                {
                    #region  List

                    #region  Headers
                    OfficeOpenXml.ExcelWorksheet xlSheet = pckExport.Workbook.Worksheets.Add(pageHeader);
                    xlSheet.Name = model.UserName;
                    xlSheet.PrinterSettings.LeftMargin = 0.57M;
                    xlSheet.PrinterSettings.RightMargin = 0.41M;
                    xlSheet.PrinterSettings.HeaderMargin = 0.28M;
                    xlSheet.PrinterSettings.TopMargin = 1.28M;
                    xlSheet.PrinterSettings.BottomMargin = 0.7M;
                    xlSheet.PrinterSettings.FooterMargin = 0.3M;
                    xlSheet.PrinterSettings.Orientation = OfficeOpenXml.eOrientation.Portrait;

                    int padding = 1;
                    int miRow = 1 + padding;
                    int MinCol = 1 + padding;
                    int MaxCol = TotalWorkingDaysOfmonth + 1 + padding;
                    int TotalSearchColumn = TotalWorkingDaysOfmonth + 1 + padding;

                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Value = "EMPLOYEE PUNCH IN-OUT MONTHLY REPORT";
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Merge = true;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0070C0"));
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 12;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = true;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));

                    miRow = miRow + 1;

                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Value = model.UserName + " - Work Report";
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Merge = true;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 24;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = true;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));

                    miRow = miRow + 1;

                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Value = "Report generated on " + (DateTime.Now).ToString("MM/dd/yyyy") + " at " + (DateTime.Now).ToString("HH:mm tt").ToLower();
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Merge = true;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 14;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = false;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#525252"));

                    miRow = miRow + 1;

                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Value = "Powered by NKTPL";
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Merge = true;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0070C0"));
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 14;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = true;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));

                    miRow = miRow + 1;

                    // Add Time Range Search Filter - Tilte
                    //for (int c = 0; c <= TotalSearchColumn;)
                    for (int c = 0; c <= 30;)
                    {
                        if (c == 0)
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = "Total Working Day";
                        else if (c == 3)
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = "Total Working Hours";
                        else if (c == 6)
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = "Average Workng Hours";
                        else if (c == 9)
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = "Total Leaves";
                        else if (c == 12)
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = "Total Late In";
                        else if (c == 15)
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = "Total Early Out";
                        else if (c == 18)
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = "Total System Out";
                        else if (c == 21)
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = "Total Outside Punch In";
                        else if (c == 24)
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = "Total Outside Punch Out";


                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Merge = true;
                        xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#D3D3D3"));
                        xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Font.Name = "Sitka Text";
                        xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Font.Size = 12;
                        xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                        xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Font.Bold = true;
                        c = c + 3;
                    }

                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#D3D3D3"));
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 12;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = true;

                    miRow = miRow + 1;

                    // Add Time Range Search Filter - Data
                    for (int c = 0; c <= 30;)
                    {
                        {
                            if (c == 0) // Payroll
                            {
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = model.TotalWorkingDays;
                            }
                            else if (c == 3) // Location
                            {
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = model.TotalWorkingHours;
                            }
                            else if (c == 6) // Jobclass
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = model.AverageWorkingHours;
                            else if (c == 9)
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = model.TotalLeaves;//"Total Leaves";
                            else if (c == 12)
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = model.TotalLatePunch_In;//"Total Late In";
                            else if (c == 15)
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = model.TotalEarlyPunch_Out;//"Total Early Out";
                            else if (c == 18)
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = model.TotalSystemPunchOut;//"Total System Out";
                            else if (c == 21)
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = model.TotalPunchIn_Outside;//"Total Outside Punch In";
                            else if (c == 24)
                                xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Value = model.TotalPunchOut_Outside; //"Total Outside Punch Out";

                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Merge = true;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#D3D3D3"));
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Font.Name = "Sitka Text";
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Font.Size = 12;
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                            xlSheet.Cells[miRow, MinCol + c, miRow, MinCol + c + 2].Style.Font.Bold = false;
                        }

                        c = c + 3;
                    }

                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#D3D3D3"));
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 12;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = false;
                    #endregion

                    #region Columns and Sheet Config

                    for (int i = 1 + padding; i <= MaxCol; i++)
                    {
                        xlSheet.Column(i).Width = 25.83;
                    }

                    miRow += 1;

                    //'header alignment
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    //'border
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;


                    //Font Bold
                    //xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = true;

                    //'Background Color
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0070C0"));


                    //Font Name
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";


                    //Font Size
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 12;


                    //Font Color
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));

                    int headercolnumber = 1;
                    xlSheet.Cells[miRow, headercolnumber + padding].Value = "Employee";
                    for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                    {
                        if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                        {
                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                        }
                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.WrapText = true;
                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.TextRotation = 90;
                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Value = startdateOfMonth.AddDays(i).ToString("dd-MM-yy") + Environment.NewLine + startdateOfMonth.AddDays(i).ToString("ddd");

                        // User Leave
                        AppUserLeave obj = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                        if (obj != null)
                        {
                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                        }
                        headercolnumber++;
                    }


                    miRow += 1;

                    #endregion

                    #region Bind Data

                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFF8E5"));
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Size = 18;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Bold = true;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#BD6427"));
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                    xlSheet.Cells[miRow, 1 + padding].Style.Font.Size = 11;
                    xlSheet.Cells[miRow, 1 + padding].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    xlSheet.Cells[miRow, 1 + padding].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    xlSheet.Cells[miRow, 1 + padding].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                    xlSheet.Cells[miRow, 1 + padding].Value = "Prashan Vyash";


                    headercolnumber = 1;
                    for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                    {
                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Font.Size = 11;
                        PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                        if (obj == null || obj.PunchoutTime == null)
                        {
                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Value = "0";
                        }
                        else
                        {
                            double hours = obj.PunchoutTime.Value.Subtract(obj.PunchinTime).TotalHours;
                            TimeSpan sp_hours = TimeSpan.FromHours(hours);
                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Value = sp_hours.Hours + ":" + sp_hours.Minutes + ":" + sp_hours.Seconds;
                        }

                        if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                        {
                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                        }
                        headercolnumber++;
                    }
                    miRow += 1;

                    for (int j = 0; j < 11; j++)
                    {

                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;


                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.Font.Name = "Sitka Text";
                        xlSheet.Cells[miRow, MinCol, miRow, MaxCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        xlSheet.Cells[miRow, 1 + padding].Style.Font.Bold = true;
                        xlSheet.Cells[miRow, 1 + padding].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        xlSheet.Cells[miRow, 1 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        xlSheet.Cells[miRow, 1 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0070C0"));
                        xlSheet.Cells[miRow, 1 + padding].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, 1 + padding].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, 1 + padding].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, 1 + padding].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        xlSheet.Cells[miRow, 1 + padding].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));

                        if (j == 0)
                        {
                            xlSheet.Cells[miRow, 1 + padding].Value = "Puncn In";

                            for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                            {
                                PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                if (obj == null)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = "-";
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = obj.PunchinTime.ToString("hh:mm tt");
                                }
                                if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                }

                                // User Leave
                                AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                if (objleave != null)
                                {
                                    if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                    }
                                    else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                    }
                                }
                            }
                        }

                        if (j == 1)
                        {
                            xlSheet.Cells[miRow, 1 + padding].Value = "Puncn Out";

                            for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                            {

                                PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                if (obj == null)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = "-";
                                    //xlSheet.Cells[miRow + 2, i + 2 + padding].Value = "-";
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = obj.PunchoutTime == null ? " - " : obj.PunchoutTime.Value.ToString("hh:mm tt");
                                }
                                if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                }

                                // User Leave
                                AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                if (objleave != null)
                                {
                                    if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                    }
                                    else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                    }
                                }
                            }
                        }

                        if (j == 2)
                        {
                            xlSheet.Cells[miRow, 1 + padding].Value = "In Type";
                            for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                            {
                                xlSheet.Cells[miRow, i + 2 + padding].Style.Font.Size = 8;
                                PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                if (obj == null)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = "-";
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = obj.PunchinType == null ? " - " : (obj.PunchinType.Value ? "Inside" : "Outside");
                                }
                                if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#BACFAB"));
                                }

                                // User Leave
                                AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                if (objleave != null)
                                {
                                    if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                    }
                                    else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                    }
                                }
                            }
                        }
                        if (j == 3)
                        {
                            xlSheet.Cells[miRow, 1 + padding].Value = "In Location";
                            for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                            {
                                xlSheet.Cells[miRow, i + 2 + padding].Style.Font.Size = 8;
                                PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                if (obj == null)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = "-";
                                }
                                else
                                {
                                    Location obj_location = location_list.FirstOrDefault(x => x.LocationId == obj.PILocationId);
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = obj_location == null ? " - " : obj_location.LocationName;
                                }
                                if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#BACFAB"));
                                }

                                // User Leave
                                AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                if (objleave != null)
                                {
                                    if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                    }
                                    else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                    }
                                }
                                xlSheet.Cells[miRow, i + 2 + padding].Style.WrapText = true;
                            }
                        }
                        if (j == 4)
                        {
                            xlSheet.Cells[miRow, 1 + padding].Value = "Late In";
                            for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                            {
                                xlSheet.Cells[miRow, i + 2 + padding].Style.Font.Size = 8;
                                PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                if (obj != null)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = obj.LatePunchin ? "Yes" : "No";
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = " - ";
                                }
                                if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#BACFAB"));
                                }

                                // User Leave
                                AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                if (objleave != null)
                                {
                                    if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                    }
                                    else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                    }
                                }
                                xlSheet.Cells[miRow, i + 2 + padding].Style.WrapText = true;
                            }
                        }
                        if (j == 5)
                        {
                            xlSheet.Cells[miRow, 1 + padding].Value = "Late In Reason";
                            for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                            {
                                xlSheet.Cells[miRow, i + 2 + padding].Style.Font.Size = 8;
                                PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                if (obj != null)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = obj.LatePunchinReason;
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = " - ";
                                }
                                if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#BACFAB"));
                                }
                                xlSheet.Cells[miRow, i + 2 + padding].Style.WrapText = true;
                                xlSheet.Cells[miRow, i + 2 + padding].Style.ShrinkToFit = true;
                            }
                        }

                        if (j == 6)
                        {
                            xlSheet.Cells[miRow, 1 + padding].Value = "Out Type";
                            for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                            {
                                xlSheet.Cells[miRow, i + 2 + padding].Style.Font.Size = 8;
                                PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                if (obj == null)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = "-";
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = obj.PunchoutType == null ? " - " : (obj.PunchoutType.Value ? "Inside" : "Outside");
                                }
                                if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#98ceb8"));
                                }

                                // User Leave
                                AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                if (objleave != null)
                                {
                                    if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                    }
                                    else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                    }
                                }
                            }
                        }
                        if (j == 7)
                        {
                            xlSheet.Cells[miRow, 1 + padding].Value = "Out Location";
                            for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                            {
                                xlSheet.Cells[miRow, i + 2 + padding].Style.Font.Size = 8;
                                PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                if (obj == null)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = "-";
                                }
                                else
                                {
                                    Location obj_location = location_list.FirstOrDefault(x => x.LocationId == obj.POLocationId);
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = obj_location == null ? " - " : obj_location.PlaceName;
                                }
                                if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#98ceb8"));
                                }

                                // User Leave
                                AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                if (objleave != null)
                                {
                                    if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                    }
                                    else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                    }
                                }
                                xlSheet.Cells[miRow, i + 2 + padding].Style.WrapText = true;
                            }
                        }
                        if (j == 8)
                        {
                            xlSheet.Cells[miRow, 1 + padding].Value = "Early Out";
                            for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                            {
                                xlSheet.Cells[miRow, i + 2 + padding].Style.Font.Size = 8;
                                PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                if (obj != null)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = obj.EarlyPunchout ? "Yes" : "No";
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = " - ";
                                }
                                if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#98ceb8"));
                                }

                                // User Leave
                                AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                if (objleave != null)
                                {
                                    if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                    }
                                    else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                    }
                                }
                                xlSheet.Cells[miRow, i + 2 + padding].Style.WrapText = true;
                            }
                        }
                        if (j == 9)
                        {
                            xlSheet.Cells[miRow, 1 + padding].Value = "Early Out Reason";

                            for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                            {
                                xlSheet.Cells[miRow, i + 2 + padding].Style.Font.Size = 8;
                                PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                if (obj != null)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = obj.EarlyPunchoutReason;
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = " - ";
                                }
                                if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#98ceb8"));
                                }

                                // User Leave
                                AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                if (objleave != null)
                                {
                                    if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                    }
                                    else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                    }
                                }
                                xlSheet.Cells[miRow, i + 2 + padding].Style.WrapText = true;
                                xlSheet.Cells[miRow, i + 2 + padding].Style.ShrinkToFit = true;
                            }
                        }

                        if (j == 10)
                        {
                            xlSheet.Cells[miRow, 1 + padding].Value = "System Punch Out";

                            for (int i = 0; i < TotalWorkingDaysOfmonth; i++)
                            {
                                xlSheet.Cells[miRow, i + 2 + padding].Style.Font.Size = 8;
                                PunchIn obj = model.records.FirstOrDefault(x => x.PunchinTime.Day == startdateOfMonth.AddDays(i).Day);
                                if (obj != null)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = obj.SystemPunchout ? "Yes" : "No";
                                }
                                else
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Value = " - ";
                                }
                                if (startdateOfMonth.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                                {
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    xlSheet.Cells[miRow, i + 2 + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFCCCB"));
                                }

                                // User Leave
                                AppUserLeave objleave = AppUserLeave_list.FirstOrDefault(x => x.StartDate.Date >= startdateOfMonth.AddDays(i).Date && x.EndDate.Date <= startdateOfMonth.AddDays(i).Date);
                                if (objleave != null)
                                {
                                    if (objleave.AppLeaveTypeId == (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#14b7b8"));
                                    }
                                    else if (objleave.AppLeaveTypeId != (int)APIEnum.Leavetype.FullLeave)
                                    {
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        xlSheet.Cells[miRow, (headercolnumber + 1) + padding].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#9f2d67"));
                                    }
                                }
                                xlSheet.Cells[miRow, i + 2 + padding].Style.WrapText = true;
                                xlSheet.Cells[miRow, i + 2 + padding].Style.ShrinkToFit = true;
                            }
                        }

                        miRow += 1;
                    }





                    ///Set Autofil columns
                    xlSheet.Cells.AutoFitColumns();

                    byte[] bytes = pckExport.GetAsByteArray();

                    return bytes;
                    #endregion

                    #endregion
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public Report_Model GetModel(DateTime MonthDate, DateTime startdateOfMonth, DateTime enddateOfMonth, int UserID)
        {


            EmployeeLoginEntities db = new EmployeeLoginEntities();

            List<Holiday> holidays_records = db.Holidays.Where(x => x.HolidayDate.Month == MonthDate.Month).ToList();
            List<PunchIn> records = db.PunchIns.Where(x => x.UserId == UserID && x.PunchinTime.Month == MonthDate.Month).OrderBy(x => x.PunchinTime).ToList();

            Report_Model model = new Report_Model();
            model.records = records;

            double AverageWorkingHours = records.Where(x => x.PunchoutTime != null).Average(x => x.PunchoutTime.Value.Subtract(x.PunchinTime).TotalHours);
            TimeSpan tp_AverageWorkingHours = TimeSpan.FromHours(AverageWorkingHours);
            model.AverageWorkingHours = tp_AverageWorkingHours.Hours + ":" + tp_AverageWorkingHours.Minutes + ":" + tp_AverageWorkingHours.Seconds;

            double TotalWorkingHours = records.Where(x => x.PunchoutTime != null).Sum(x => x.PunchoutTime.Value.Subtract(x.PunchinTime).TotalHours);
            TimeSpan tp_TotalWorkingHours = TimeSpan.FromHours(TotalWorkingHours);
            model.TotalWorkingHours = Convert.ToInt32(tp_TotalWorkingHours.TotalHours) + ":" + tp_TotalWorkingHours.Minutes + ":" + tp_TotalWorkingHours.Seconds;

            model.TotalLatePunch_In = records.Count;
            model.TotalEarlyPunch_Out = records.Where(x => x.PunchoutTime != null).Count();
            model.TotalPunchIn_Outside = records.Where(x => x.PunchinType == false).Count();
            model.TotalPunchOut_Outside = records.Where(x => x.PunchoutType == false).Count();
            model.TotalSystemPunchOut = records.Where(x => x.SystemPunchout == true).Count();

            model.TotalHolidays = holidays_records.Count;
            model.TotalWeekOffs = 0;
            model.TotalWorkingDays = 0;
            model.TotalWorkingDays = (enddateOfMonth.Subtract(startdateOfMonth).Days - (model.TotalHolidays + model.TotalWeekOffs));
            return model;
        }

        private void SendEmail(Byte[] bytes, string filename)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("milanttv13@gmail.com");
                mail.To.Add("milan.gorakhiait@gmail.com");
                mail.Subject = "Test Mail";
                mail.Body = "This is for testing SMTP mail from GMAIL";


                System.Net.Mail.Attachment attachment;
                Stream stream = new MemoryStream(bytes);
                attachment = new System.Net.Mail.Attachment(stream, filename);
                mail.Attachments.Add(attachment);
                SmtpServer.Port = 587;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential("milanttv13@gmail.com", "jaygopal");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
            }
        }
    }
}