string G_MyImportDataType;
string G_MyImport;
protected void ddlDataTypeSelect_SelectedIndexChanged(object sender, EventArgs e)
{
    /* this is olv vb code 
        'Due    0
'Lapsed 1
'Paid   2
'Reins  3
If CmbDataType.ListIndex = 0 Then 'DUE
MyImportDataType = "DUEDATA"
MyImport = "D"
OptDue.Value = True
OptPaid.Caption = "Paid Data"
OptDue.Caption = "Due Data"
ElseIf CmbDataType.ListIndex = 1 Then 'LAPSED
MyImportDataType = "LAPSEDDATA"
MyImport = "L"
OptDue.Value = True
OptPaid.Caption = "Reinstate Data"
OptDue.Caption = "Lappsed Data"
ElseIf CmbDataType.ListIndex = 2 Then 'PAID
MyImportDataType = "DUEDATA"
MyImport = "D"
OptPaid.Value = True
OptPaid.Caption = "Paid Data"
OptDue.Caption = "Due Data"
ElseIf CmbDataType.ListIndex = 3 Then 'REINS
MyImportDataType = "LAPSEDDATA"
MyImport = "L"
OptPaid.Value = True
OptPaid.Caption = "Reinstate Data"
OptDue.Caption = "Lappsed Data"
End If

        
        */

    string ddlValue = ddlImportDataType.SelectedValue.ToString();

    if (!string.IsNullOrEmpty(ddlValue))
    {
        #region CheckBox Validation
        if (ddlValue == "DUE" || ddlValue == "LAPSED")
        {
            chkOptDue.Checked = true;
            chkOptPaid.Checked = false;
            chkOptLap.Checked = false;
            chkOptRein.Checked = false;

            chkOptDue.Visible = true;
            chkOptPaid.Visible = true; 

            chkOptLap.Visible = false;
            chkOptRein.Visible = false;

        }
        
        else if (ddlValue == "PAID" || ddlValue == "REINS")
        {
            chkOptPaid.Checked = true;
            chkOptDue.Checked = false;
            chkOptLap.Checked = false;
            chkOptRein.Checked = false;

            chkOptDue.Visible = true;
            chkOptPaid.Visible = true;
            chkOptLap.Visible = false;
            chkOptRein.Visible = false;
        }
        
        #endregion

        if (!string.IsNullOrEmpty(ddlValue))
        {
            string filePath = Session["CurrentDAPExcelFile"]?.ToString();

            Fun_FetchMapped(filePath, excelSheetSelect.SelectedValue, "DUE_AND_PAID", ddlValue, ddlFillPrevious2);
        }

    }
}