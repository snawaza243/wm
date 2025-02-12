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
