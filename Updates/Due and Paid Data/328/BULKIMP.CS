  public void ImportDatainDB()
  {
      string monthValue = monthSelect.SelectedValue ?? string.Empty;
      string yearValue = yearSelect.SelectedValue ?? string.Empty;
      string ddlImportDataTypeValue = ddlImportDataType.SelectedValue ?? string.Empty;
      string logginId = Session["LoginId"].ToString() ?? string.Empty;
      string filePath = Session["CurrentDAPExcelFile"]?.ToString();
      string selectedSheet = excelSheetSelect.SelectedValue;

      string selectedText = GetCheckedRadioButtonText();
      string chkDataTypeValue = selectedText.Substring(0, 1).ToUpper();

      string fileName = lblFIleName.Text;

      // CHECK MAPPED DDL HAVE DATA OR NOT
      if (ddlMappedFieldList.Items.Count == 0)
      {
          ShowAlert("Mapping is required");
          return;
      }


      // Step 1: Collect database fields and dropdown values
      List<string> allDBFieldList = new List<string>();
      foreach (ListItem item in ddlDbField.Items)
      {
          allDBFieldList.Add(item.Text); // Collect all dropdown field names
      }

      List<string> dropdownValues = new List<string>();
      foreach (ListItem item in ddlMappedFieldList.Items)
      {
          dropdownValues.Add(item.Text); // Collect mapped fields (Excel Field#DB Field)
      }

      // Step 2: Destructure dropdown values to get Excel fields and database fields
      List<string> excelFields = GetExcelFieldList(dropdownValues); // Extract Excel field names
      List<string> databaseFields = GetDatabaseFieldList(dropdownValues); // Extract database field names


      DataTable excelData = LoadSheetData(filePath, selectedSheet); // Load data from the selected Excel sheet

      // Step 4: Create a new DataTable with fields matching ddlDbField
      DataTable mappedDataTable = new DataTable();
      foreach (string dbField in allDBFieldList)
      {
          mappedDataTable.Columns.Add(dbField, typeof(string)); // Add columns for all database fields
      }

      // Step 5: Map data from Excel to the new DataTable based on the mapping
      foreach (DataRow excelRow in excelData.Rows)
      {
          DataRow newRow = mappedDataTable.NewRow();

          for (int i = 0; i < databaseFields.Count; i++)
          {
              string dbField = databaseFields[i]; // Database field name
              string excelField = excelFields[i]; // Corresponding Excel field name

              if (allDBFieldList.Contains(dbField) && excelData.Columns.Contains(excelField))
              {
                  newRow[dbField] = excelRow[excelField]; // Map data from Excel to the new DataTable
              }
          }
          mappedDataTable.Rows.Add(newRow); // Add the populated row to the DataTable
      }

      string MyImportDataType = "";
      string MyImport = "";


      if (!string.IsNullOrEmpty(ddlImportDataTypeValue))
      {
          if (ddlImportDataTypeValue == "DUE")
          {
              MyImport = "D";
              MyImportDataType = "DUEDATA";
          }
          if (ddlImportDataTypeValue == "LAPSED")
          {
              MyImport = "L";
              MyImportDataType = "LAPSEDDATA";
          }
          if (ddlImportDataTypeValue == "PAID")
          {
              MyImport = "D";
              MyImportDataType = "DUEDATA";
          }
          if (ddlImportDataTypeValue == "REINS")
          {
              MyImport = "L";
              MyImportDataType = "LAPSEDDATA";
          }
      }



      string currentImpDataName = ddlImportDataType.SelectedValue.ToString();
      string fieldSavedPathX = Server.MapPath("~/SampleFile/DuePaidMappedField" + currentImpDataName + ".txt");
      string fieldSavedPath = "";

      if (System.IO.File.Exists(fieldSavedPathX))
      {
          fieldSavedPath = System.IO.File.ReadAllText(fieldSavedPathX);
      }



      //string[] delComma = fieldSavedPath.Split(',');
      DropDownList DDLx = new DropDownList(); // Create a new DropDownList

      // Assuming delComma is an array of strings
      string[] delComma = fieldSavedPath.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

      // Populate the DropDownList
      foreach (string item in delComma)
      {
          DDLx.Items.Add(new ListItem(item));
      }
      string selectedFileField = "";
      string dataBaseField = "";
      string Excel_policy_no = "";
      string Excel_Status = ""; //-
      string Excel_Comp = "";
      string Excel_Payment_Mode = "";
      string Excel_Prem_Freq = "";
      string Excel_Due_Date = "";
      string doc = "";
      string Excel_Prem_Amt = "";  //-
      string excel_mobile = "";  //-
      string excel_pol_term = "";  //-
      string excel_sa = "";  //-

     
      int upb = delComma.GetUpperBound(0);
      for (int Count_Loop = 0; Count_Loop < DDLx.Items.Count; Count_Loop++)
      {
          string[] delHash = delComma[Count_Loop].Split('#');
          selectedFileField = selectedFileField + delHash[0] + ",";
          dataBaseField = dataBaseField + delHash[1] + ",";
          if (delHash[1].ToUpper().Trim() == "POLICY_NO")
          {
              //Excel_policy_no = delHash[1].Replace("[", "").Replace("]", "").Replace("-", "");
              Excel_policy_no = delHash[0];

          }
          if (delHash[1].ToUpper().Trim() == "COMPANY_CD")
          {
              Excel_Comp = delHash[0] ;
          }
          if (delHash[1].ToUpper().Trim() == "PAY_MODE")
          {
              Excel_Payment_Mode = delHash[1].Replace("[", "").Replace("]", "").Replace("-", "");
          }
          if (delHash[1].ToUpper().Trim() == "PREM_FREQ")
          {
              Excel_Prem_Freq = delHash[0];
          }
          if (delHash[1].ToUpper().Trim() == "DUE_DATE")
          {
              Excel_Due_Date = delHash[0];
          }
          if (chkOptPaid.Checked == true)
          {
              if (delHash[1].ToUpper().Trim() == "STATUS_CD") 
              {
                  Excel_Status = delHash[0];
              }
          }
          if (delHash[1].ToUpper().Trim() == "DOC")
          {
              doc = delHash[0];
          }
          if (delHash[1].ToUpper().Trim() == "PREM_AMT")
          {
              Excel_Prem_Amt = delHash[0];
          }
          if (delHash[1].ToUpper().Trim() == "CL_MOBILE")
          {
              excel_mobile = delHash[0];
          }
          if (delHash[1].ToUpper().Trim() == "PLY_TERM")
          {
              excel_pol_term = delHash[0];
          }
          if (delHash[1].ToUpper().Trim() == "SA")
          {
              excel_sa = delHash[0];
          }
      }

      if (selectedFileField.Length > 0)
          selectedFileField = selectedFileField.Substring(0, selectedFileField.Length - 1);
      if (dataBaseField.Length > 0)
          dataBaseField = dataBaseField.Substring(0, dataBaseField.Length - 1);

      selectedFileField.TrimEnd(',');
      dataBaseField.TrimEnd(',');




    if(chkOptDue.Checked){
        DataTable dt_e1 = new DataTable();
        int dt_e1_rc = dt_e1.Rows.Count;

        for(DataRow row in dt_e1.Rows ){

            if(!string.IsNullOrEmpty(Excel_Comp )){
            SqlStr = " select * from bajaj_due_Data WHERE upper(trim(POLICY_no))=upper(trim('" & UCase(Trim(rsExcel("" & Trim(Excel_policy_no) & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "' and mon_no = " & CmbMonth.ListIndex + 1 & " and year_no=" & TxtYear.Text & " and importdatatype='" & MyImportDataType & "' "

            }
        }

        for(int i = 0; i< dt_e1_rc; i;){

        }
    }






      return;














      string insertionResult = "";
      string isDupUpdated = "";
      string insertionBDDForBPDResult = "";
      if (ddlImportDataTypeValue == "DUE" || ddlImportDataTypeValue == "LAPSED")
      {
          insertionResult = new DueAndPaidDataImportingController().InsertDueData_N(mappedDataTable, Convert.ToInt32(monthValue), Convert.ToInt32(yearValue), ddlImportDataTypeValue, MyImportDataType, MyImport, chkDataTypeValue, logginId, filePath);
      }
      else if (ddlImportDataTypeValue == "PAID" || ddlImportDataTypeValue == "REINS")
      {
          insertionResult = new DueAndPaidDataImportingController().InsertPaidData_N(mappedDataTable, Convert.ToInt32(monthValue), Convert.ToInt32(yearValue), ddlImportDataTypeValue, MyImportDataType, MyImport, chkDataTypeValue, logginId, filePath);
      }


      Label1.Text = insertionResult;
      if (insertionResult.ToUpper().Contains("success".ToUpper()))
      {

          isDupUpdated = new DueAndPaidDataImportingController().BDD_UpdateDuplicatePolicies(Convert.ToInt32(monthValue), Convert.ToInt32(yearValue), MyImportDataType);
          string dupPolMsg = isDupUpdated.Contains("success") ? " and " + isDupUpdated : null;




          ShowAlert("Data Imported Status: " + insertionResult + dupPolMsg);
      }


      if (insertionResult.Contains("Not found due data"))
      {
          if (ddlImportDataTypeValue == "DUE" || ddlImportDataTypeValue == "LAPSED")
          {
              insertionBDDForBPDResult = new DueAndPaidDataImportingController().InsertDueData_N_For_Paid(mappedDataTable, Convert.ToInt32(monthValue), Convert.ToInt32(yearValue), ddlImportDataTypeValue, MyImportDataType, MyImport, chkDataTypeValue, logginId, filePath);
          }
          insertionResult = new DueAndPaidDataImportingController().InsertPaidData_N(mappedDataTable, Convert.ToInt32(monthValue), Convert.ToInt32(yearValue), ddlImportDataTypeValue, MyImportDataType, MyImport, chkDataTypeValue, logginId, filePath);
          ShowAlert(insertionResult);
      }
      else
      {
          ShowAlert("Data Imported Status: " + insertionResult);
      }
      ShowAlert(insertionResult);
  }
