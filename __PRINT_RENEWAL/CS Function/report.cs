// protected void ProcessReport(string type, string mon_year)
// {
//     // Set report file path based on type
//     string reportFile = Server.MapPath(type == "A" 
//         ? "~/reports/RenewalLetter_A11.RPT" 
//         : "~/reports/RenewalLetter_B1.RPT");

//     // Parse mon_year (format: MM-YYYY) and get month name in "MMM-yy"
//     DateTime dt;
//     if (!DateTime.TryParseExact("01-" + mon_year, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out dt))
//     {
//         // Handle invalid date
//         throw new ArgumentException("Invalid month-year format.");
//     }
//     string monname = dt.ToString("MMM-yy");

//     // Load and configure Crystal Report
//     ReportDocument rpt = new ReportDocument();
//     rpt.Load(reportFile);

//     // Set DB login info (adjust as needed)
//     rpt.SetDatabaseLogon("wealthmaker", "DataBasePassword", "test", "wealthmaker");

//     // Set parameter
//     rpt.SetParameterValue("mon_name", monname);

//     // Optional: set viewer properties if using CrystalReportViewer
//     CrystalReportViewer1.ReportSource = rpt;
//     CrystalReportViewer1.HasPrintButton = true;
//     CrystalReportViewer1.HasSearchButton = true;
//     CrystalReportViewer1.HasPrintSetupButton = true;
//     CrystalReportViewer1.HasExportButton = true;
//     CrystalReportViewer1.DisplayGroupTree = false;

//     // Optionally log the report generation
//     // SaveLogIn(Session["Glbloginid"].ToString(), "", "ProcessReport");
// }