public void ExportDataToExcel(string companyCd)
{
    DataTable dt = GetPolicyDataForExport(companyCd);

    if (dt.Rows.Count > 0)
    {
        int batchSize = 5000;  // Process in batches of 5000 rows
        int totalRows = dt.Rows.Count;
        int processedRows = 0;

        using (var workbook = new XLWorkbook())
        {
            int sheetCount = 1;

            while (processedRows < totalRows)
            {
                var worksheet = workbook.AddWorksheet("Sheet" + sheetCount);

                // Write Header
                for (int col = 0; col < dt.Columns.Count; col++)
                {
                    worksheet.Cell(1, col + 1).Value = dt.Columns[col].ColumnName;
                    worksheet.Cell(1, col + 1).Style.Font.Bold = true;
                }

                // Write Data in Chunks
                for (int row = 0; row < batchSize && processedRows < totalRows; row++, processedRows++)
                {
                    for (int col = 0; col < dt.Columns.Count; col++)
                    {
                        worksheet.Cell(row + 2, col + 1).Value = dt.Rows[processedRows][col].ToString();
                    }
                }

                sheetCount++;
            }

            // Export to Excel
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=PolicyData.xlsx");

            using (var ms = new MemoryStream())
            {
                workbook.SaveAs(ms);
                ms.WriteTo(HttpContext.Current.Response.OutputStream);
                ms.Close();
            }
        }
    }
}




public DataTable GetPolicyDataForExport(string companyCd)
{
    DataTable dtPolicyData = new DataTable();

    using (OracleConnection con = new OracleConnection(connectionString))
    {
        try
        {
            con.Open();

            using (OracleCommand cmd = new OracleCommand("psm_mpn_export_data", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Adding the input parameter for company_cd
                cmd.Parameters.Add("p_company_cd", OracleDbType.Varchar2).Value = companyCd;

                // Adding the OUT parameter for the cursor
                cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                // *** Use OracleDataReader for better performance ***
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    dtPolicyData.Load(reader);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    return dtPolicyData;
}

