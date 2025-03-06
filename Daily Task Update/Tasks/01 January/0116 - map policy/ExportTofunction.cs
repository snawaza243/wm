DataTable exportDataFromTable = controller.GetPolicyDataForExport(currrentCompVlaue);
if (exportDataFromTable.Rows.Count > 0)
{
    string rowCount = exportDataFromTable.Rows.Count.ToString();
    lblMessage.Text = rowCount + " rows(s) found!";

    if (GridView1.Rows.Count > 0)
    {
        GridView1.DataSource = null;
        GridView1.DataBind();
    }

    // show exporting data on grid
    gridPolicyData.DataSource = exportDataFromTable;  // Bind the data to the GridView.
    gridPolicyData.DataBind();

    if (gridPolicyData.Rows.Count > 0)
    {
        try
        {
            ExportGridToExcel(gridPolicyData);
        }
        catch (Exception ex)
        {
            ShowAlert(ex.Message);
        }
    }
}

public string ExtractDatePart(string dateString)
{
    try
    {
        // Split the date string by '/'
        var parts = dateString.Split('/');
        if (parts.Length == 3)
        {
            // Return the date part in "dd/mm/yyyy" format
            return $"{parts[0]}/{parts[1]}/{parts[2]}";
        }
        else
        {
            throw new FormatException("Invalid date format.");
        }
    }
    catch (Exception ex)
    {
        // Handle exceptions (e.g., log the error)
        return $"Error: {ex.Message}";
    }
}
