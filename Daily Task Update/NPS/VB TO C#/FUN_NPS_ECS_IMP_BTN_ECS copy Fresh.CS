   #region NEW CODE

   public void NewCOdeImp()
   {
       TransactionImporter importer = new TransactionImporter();

       // Initialize sample values for testing
       importer.filepath = "sample.xls";
       importer.sheetName = "Sheet1";
       importer.Glbloginid = "User1";
       importer.ServerDateTime = DateTime.Now;
       importer.TxtFile_Name = "mapping.txt";

       // Fix DropDownList issue
       importer.cmbCompany.Items.Add(new ListItem("COMPANY#001", "COMPANY#001"));
       importer.cmbCompany.SelectedValue = "COMPANY#001";

       importer.CmbStatus.Items.Add(new ListItem("ACTIVE#A", "ACTIVE#A"));
       importer.CmbStatus.SelectedIndex = 0;

       // Fix RadioButton selection
       importer.OptLife.Checked = true;
       importer.OptGeneral.Checked = false;
       importer.OptNPS.Checked = false;

       // Ensure ImportTransaction() exists
       importer.ImportTransaction();
   }


   public DropDownList CmbStatus;
   public DropDownList cmbCompany;
   public RadioButton OptLife;
   public RadioButton OptGeneral;
   public RadioButton OptNPS;
   public Label Label5;
   public string TxtFile_Name;
   public string filepath;
   public string sheetName;
   public string Glbloginid;
   public DateTime ServerDateTime;

   // Global variables used in the code
   public string Status_Chk;
   public string Comp_Cd;
   public string importFile;
   public int Total_records = 0;
   public int Already_Exist = 0;
   public int countNull = 0;


   public class TransactionImporter
   {
       public DropDownList CmbStatus { get; set; } = new DropDownList();
       public DropDownList cmbCompany { get; set; } = new DropDownList();
       public RadioButton OptLife { get; set; } = new RadioButton();
       public RadioButton OptGeneral { get; set; } = new RadioButton();
       public RadioButton OptNPS { get; set; } = new RadioButton();
       public Label Label5 { get; set; } = new Label();

       public string TxtFile_Name;
       public string filepath;
       public string sheetName;
       public string Glbloginid;
       public DateTime ServerDateTime;

       // Global variables used in the code
       public string Status_Chk;
       public string Comp_Cd;
       public string importFile;
       public int Total_records = 0;
       public int Already_Exist = 0;
       public int countNull = 0;

       public void ImportTransaction()
       {
           // Dummy method to avoid errors
           Console.WriteLine("Import Transaction called.");
       }
   }


   public void ImportTransaction()
   {
       try
       {
           string Sql = "";
           string FileFields = "";
           string dataDaseField = "";
           string selectedFileField = "";
           string str1;

           string cmbStatusItem = Convert.ToString(CmbStatus.Items[CmbStatus.SelectedIndex]);
           int posHash = cmbStatusItem.IndexOf("#");
           Status_Chk = (posHash >= 0 && posHash < cmbStatusItem.Length - 1) ?
               cmbStatusItem.Substring(posHash + 1) :
               "";


           string[] Split_Company = cmbCompany.Text.Split('#');
           if (Split_Company.Length > 1)
           {
               Comp_Cd = Split_Company[1].Trim();
           }
           else
           {
               Comp_Cd = "";
           }


           importFile = filepath;

            
           DataTable dtExcel = GetExcelDataTable(importFile, sheetName);

           for (int i = 0; i < dtExcel.Columns.Count; i++)
           {
               string colName = dtExcel.Columns[i].ColumnName;
               if (!string.IsNullOrEmpty(colName))
               {
                   colName = colName.Replace(".", "").Replace("/", "").Replace("-", "");
                   dtExcel.Columns[i].ColumnName = colName;
               }
           }

           if (dtExcel.Columns.Count > 0)
           {
               for (int i = 0; i < dtExcel.Columns.Count; i++)
               {
                   FileFields += dtExcel.Columns[i].ColumnName + ",";
               }
               if (FileFields.Length > 0)
               {
                   FileFields = FileFields.Substring(0, FileFields.Length - 1);
               }
           }

           Get_Comp_TextFilePath(Split_Company[0].Trim().ToUpper());

           str1 = FieldsParametersName(TxtFile_Name);
           if (str1.Trim() == "")
           {

               //pc.ShowAlert(this, "File Format Is Wrong");

               return;
           }

           // delComma = Split(str1, ","): selectedFileField = "": dataDaseField = ""
           string[] delComma = str1.Split(',');
           selectedFileField = "";
           dataDaseField = "";
           for (int i = 0; i <= delComma.GetUpperBound(0); i++)
           {
               // delHash = Split(delComma(i), "#")
               string[] delHash = delComma[i].Split('#');
               if (delHash.Length >= 2)
               {
                   selectedFileField += delHash[1] + ",";
                   dataDaseField += delHash[0] + ",";
               }
           }
           if (selectedFileField.Length > 0)
           {
               selectedFileField = selectedFileField.Substring(0, selectedFileField.Length - 1);
           }
           if (dataDaseField.Length > 0)
           {
               dataDaseField = dataDaseField.Substring(0, dataDaseField.Length - 1);
           }

           // 'RsImport.Close
           // 'If CheckFieldsVis(FileFields, selectedFileField) = False Then Exit Sub
           if (!CheckFieldMapping(dataDaseField.ToUpper(), Status_Chk.ToUpper()))
           {
               return;
           }

           if (OptLife.Checked)
           {
               // If OptLife.Value = True Then
               // MyConn.Execute " delete from  Bajaj_PolicyInfo_Data "
               ExecuteSQL("delete from Bajaj_PolicyInfo_Data");

               // Set RsImport = New ADODB.Recordset
               // Set RsImport = importExcelcon.Execute("Select " & selectedFileField & " from [" & sheetName & "$] ")
               DataTable RsImport = GetExcelDataTable(importFile, sheetName, selectedFileField);

               Total_records = 0;
               // Do While Not RsImport.EOF
               foreach (DataRow row in RsImport.Rows)
               {
                   Sql = "";
                   string SqlChk = "";
                   string Xls_Fld = "";
                   SqlChk = " select * from Bajaj_PolicyInfo_Data where ";
                   // For i = 0 To UBound(delComma)
                   for (int i = 0; i <= delComma.GetUpperBound(0); i++)
                   {
                       string[] delHash = delComma[i].Split('#');
                       // Xls_Fld = Replace(Replace(Replace(delHash(1), "[", ""), "]", ""), "'", "")
                       Xls_Fld = delHash[1].Replace("[", "").Replace("]", "").Replace("'", "");
                       if (Xls_Fld.IndexOf("&") >= 0)
                       {
                           Xls_Fld = Exc_Clent_FldName();
                       }
                       // Check the type of field from the Excel DataTable
                       if (RsImport.Columns.Contains(Xls_Fld))
                       {
                           object fieldValue = row[Xls_Fld];
                           // Check if field is Date type (adDate = 7)
                           if (RsImport.Columns[Xls_Fld].DataType == typeof(DateTime))
                           {
                               // Format as dd-mmm-yyyy
                               Xls_Fld = Convert.ToDateTime(fieldValue).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
                               Xls_Fld = Xls_Fld.Replace("'", "");
                               SqlChk += delHash[0] + "='" + Xls_Fld + "' ";
                           }
                           // Else if the field appears to be a date in string format and meets specific length/position checks
                           else if (IsStringDate(row[Xls_Fld]))
                           {
                               if (IsNull(row[Xls_Fld]))
                               {
                                   Xls_Fld = "";
                                   SqlChk += "(" + delHash[0] + " is null  )  ";
                               }
                               else
                               {
                                   Xls_Fld = row[Xls_Fld].ToString();
                                   Xls_Fld = Xls_Fld.Replace("'", "");
                                   DateTime dt;
                                   if (DateTime.TryParse(Xls_Fld, out dt))
                                   {
                                       SqlChk += delHash[0] + "='" + dt.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) + "' ";
                                   }
                                   else
                                   {
                                       SqlChk += delHash[0] + "='" + Xls_Fld + "' ";
                                   }
                               }
                           }
                           // Else if field is assumed to be string (adVarChar = 202)
                           else if (RsImport.Columns[Xls_Fld].DataType == typeof(string))
                           {
                               if (IsNull(row[Xls_Fld]))
                               {
                                   Xls_Fld = "";
                                   SqlChk += " (" + delHash[0] + "='" + Xls_Fld.ToUpper().Trim() + "' or " + delHash[0] + " is null  )  ";
                               }
                               else
                               {
                                   Xls_Fld = row[Xls_Fld].ToString().Replace("'", "");
                                   SqlChk += delHash[0] + "='" + Xls_Fld + "' ";
                               }
                           }
                           else
                           {
                               // Default numeric case
                               if (IsNull(row[Xls_Fld]))
                               {
                                   Xls_Fld = "0";
                                   SqlChk += "(" + delHash[0] + "=" + Xls_Fld + " or " + delHash[0] + " is null  )  ";
                               }
                               else
                               {
                                   Xls_Fld = row[Xls_Fld].ToString().Replace("'", "");
                                   // Format numeric value using "######0"
                                   decimal decValue;
                                   if (Decimal.TryParse(Xls_Fld, out decValue))
                                   {
                                       SqlChk += delHash[0] + "=" + decValue.ToString("######0") + " ";
                                   }
                                   else
                                   {
                                       SqlChk += delHash[0] + "=" + Xls_Fld + " ";
                                   }
                               }
                           }
                       }
                       SqlChk += " and ";
                   }
                   SqlChk += " Company_cd='" + Comp_Cd + "' ";
                   SqlChk = newString(SqlChk);
                   DataTable rschk = ExecuteSQL(SqlChk);
                   // 'JIN FIELD SE MAPPING KI HAI UNHI FIELD SE CHECK KARTE HAI KI VO RECORD DATABASE KE ANDAR EXIST HAI YA NAHI
                   // 'AGAR NAHI HAI TO  Bajaj_PolicyInfo_Data KE ANDAR INSERT KARA DO
                   if (rschk.Rows.Count == 0)
                   {
                       countNull = 0;
                       Already_Exist = 0;
                       Sql = "Insert into Bajaj_PolicyInfo_Data (" + dataDaseField + ",Company_cd,Import_dt,UserID)  Values(";
                       for (int i = 0; i < RsImport.Columns.Count; i++)
                       {
                           string fieldName = RsImport.Columns[i].ColumnName;
                           object fieldVal = row[fieldName];
                           if (fieldVal != null && fieldVal.ToString().IndexOf("'") >= 0)
                           {
                               string Value1 = fieldVal.ToString().Replace("'", "");
                               Sql += "'" + Value1 + "',";
                           }
                           else
                           {
                               // Check if field is date type or string date format based on length checks
                               if (RsImport.Columns[fieldName].DataType == typeof(DateTime) || IsStringDate(fieldVal))
                               {
                                   string formattedDate = "";
                                   if (!IsNull(fieldVal))
                                   {
                                       DateTime dt;
                                       if (DateTime.TryParse(fieldVal.ToString(), out dt))
                                       {
                                           formattedDate = dt.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture).Trim();
                                       }
                                   }
                                   Sql += "'" + formattedDate + "',";
                               }
                               else
                               {
                                   Sql += "'" + fieldVal.ToString().Trim() + "',";
                               }
                           }
                       }
                       Sql += "'" + Comp_Cd + "','" + ServerDateTime.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) + "','" + Glbloginid + "'";
                       Sql += ")";
                       Sql = Sql.Replace("''", "Null");
                       Sql = newString(Sql);
                       INSERT_RECORD(Sql);
                       if (countNull >= 50)
                           break;
                   }
                   else
                   {
                       Already_Exist = Already_Exist + 1;
                   }
                   Total_records = Total_records + 1;
                   Label5.Text = Total_records.ToString();
               }
           }
           else if (OptGeneral.Checked)
           {
               // ElseIf OptGeneral.Value = True Then
               ExecuteSQL("delete from Bajaj_PolicyInfo_Data_gen");
               DataTable RsImport = GetExcelDataTable(importFile, sheetName, selectedFileField);
               Total_records = 0;
               foreach (DataRow row in RsImport.Rows)
               {
                   Sql = "";
                   string SqlChk = "";
                   string Xls_Fld = "";
                   SqlChk = " select * from Bajaj_PolicyInfo_Data_gen where ";
                   for (int i = 0; i <= delComma.GetUpperBound(0); i++)
                   {
                       string[] delHash = delComma[i].Split('#');
                       Xls_Fld = delHash[1].Replace("[", "").Replace("]", "").Replace("'", "");
                       if (Xls_Fld.IndexOf("&") >= 0)
                       {
                           Xls_Fld = Exc_Clent_FldName();
                       }
                       if (RsImport.Columns.Contains(Xls_Fld))
                       {
                           object fieldValue = row[Xls_Fld];
                           if (RsImport.Columns[Xls_Fld].DataType == typeof(DateTime))
                           {
                               Xls_Fld = Convert.ToDateTime(fieldValue).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
                               Xls_Fld = Xls_Fld.Replace("'", "");
                               SqlChk += delHash[0] + "='" + Xls_Fld + "' ";
                           }
                           else if (IsStringDate(fieldValue))
                           {
                               if (IsNull(fieldValue))
                               {
                                   Xls_Fld = "";
                                   SqlChk += "(" + delHash[0] + " is null  )  ";
                               }
                               else
                               {
                                   Xls_Fld = fieldValue.ToString();
                                   Xls_Fld = Xls_Fld.Replace("'", "");
                                   DateTime dt;
                                   if (DateTime.TryParse(Xls_Fld, out dt))
                                   {
                                       SqlChk += delHash[0] + "='" + dt.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) + "' ";
                                   }
                                   else
                                   {
                                       SqlChk += delHash[0] + "='" + Xls_Fld + "' ";
                                   }
                               }
                           }
                           else if (RsImport.Columns[Xls_Fld].DataType == typeof(string))
                           {
                               if (IsNull(fieldValue))
                               {
                                   Xls_Fld = "";
                                   SqlChk += " (" + delHash[0] + "='" + Xls_Fld.ToUpper().Trim() + "' or " + delHash[0] + " is null  )  ";
                               }
                               else
                               {
                                   Xls_Fld = fieldValue.ToString().Replace("'", "");
                                   SqlChk += delHash[0] + "='" + Xls_Fld + "' ";
                               }
                           }
                           else
                           {
                               if (IsNull(fieldValue))
                               {
                                   Xls_Fld = "0";
                                   SqlChk += "(" + delHash[0] + "=" + Xls_Fld + " or " + delHash[0] + " is null  )  ";
                               }
                               else
                               {
                                   Xls_Fld = fieldValue.ToString().Replace("'", "");
                                   decimal decValue;
                                   if (Decimal.TryParse(Xls_Fld, out decValue))
                                   {
                                       SqlChk += delHash[0] + "=" + decValue.ToString("######0") + " ";
                                   }
                                   else
                                   {
                                       SqlChk += delHash[0] + "=" + Xls_Fld + " ";
                                   }
                               }
                           }
                       }
                       SqlChk += " and ";
                   }
                   SqlChk += " Company_cd='" + Comp_Cd + "' ";
                   SqlChk = newString(SqlChk);
                   DataTable rschk = ExecuteSQL(SqlChk);
                   if (rschk.Rows.Count == 0)
                   {
                       countNull = 0;
                       Already_Exist = 0;
                       Sql = "Insert into Bajaj_PolicyInfo_Data_gen (" + dataDaseField + ",Company_cd,Import_dt,UserID)  Values(";
                       for (int i = 0; i < RsImport.Columns.Count; i++)
                       {
                           string fieldName = RsImport.Columns[i].ColumnName;
                           object fieldVal = row[fieldName];
                           if (fieldVal != null && fieldVal.ToString().IndexOf("'") >= 0)
                           {
                               string Value1 = fieldVal.ToString().Replace("'", "");
                               Sql += "'" + Value1 + "',";
                           }
                           else
                           {
                               if (RsImport.Columns[fieldName].DataType == typeof(DateTime) || IsStringDate(fieldVal))
                               {
                                   string formattedDate = "";
                                   if (!IsNull(fieldVal))
                                   {
                                       DateTime dt;
                                       if (DateTime.TryParse(fieldVal.ToString(), out dt))
                                       {
                                           formattedDate = dt.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture).Trim();
                                       }
                                   }
                                   Sql += "'" + formattedDate + "',";
                               }
                               else
                               {
                                   Sql += "'" + fieldVal.ToString().Trim() + "',";
                               }
                           }
                       }
                       Sql += "'" + Comp_Cd + "','" + ServerDateTime.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) + "','" + Glbloginid + "'";
                       Sql += ")";
                       Sql = Sql.Replace("''", "Null");
                       Sql = newString(Sql);
                       INSERT_RECORD(Sql);
                       if (countNull >= 50)
                           break;
                   }
                   else
                   {
                       Already_Exist = Already_Exist + 1;
                   }
                   Total_records = Total_records + 1;
                   Label5.Text = Total_records.ToString();
               }
           }
           else if (OptNPS.Checked)
           {
               // ElseIf OptNPS.Value = True Then
               ExecuteSQL("insert into nps_ecs_tbl_imp_bk select * from nps_ecs_tbl_imp");
               ExecuteSQL("delete from nps_ecs_tbl_imp");

               DataTable RsImport = GetExcelDataTable(importFile, sheetName, selectedFileField);
               Total_records = 0;
               foreach (DataRow row in RsImport.Rows)
               {
                   Sql = "";
                   string SqlChk = "";
                   string Xls_Fld = "";
                   SqlChk = " select * from nps_ecs_tbl_imp where ";
                   for (int i = 0; i <= delComma.GetUpperBound(0); i++)
                   {
                       string[] delHash = delComma[i].Split('#');
                       Xls_Fld = delHash[1].Replace("[", "").Replace("]", "").Replace("'", "");
                       if (Xls_Fld.IndexOf("&") >= 0)
                       {
                           Xls_Fld = Exc_Clent_FldName();
                       }
                       if (RsImport.Columns.Contains(Xls_Fld))
                       {
                           object fieldValue = row[Xls_Fld];
                           if (RsImport.Columns[Xls_Fld].DataType == typeof(DateTime))
                           {
                               Xls_Fld = Convert.ToDateTime(fieldValue).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
                               Xls_Fld = Xls_Fld.Replace("'", "");
                               SqlChk += delHash[0] + "='" + Xls_Fld + "' ";
                           }
                           else if (IsStringDate(fieldValue))
                           {
                               if (IsNull(fieldValue))
                               {
                                   Xls_Fld = "";
                                   SqlChk += "(" + delHash[0] + " is null  )  ";
                               }
                               else
                               {
                                   Xls_Fld = fieldValue.ToString();
                                   Xls_Fld = Xls_Fld.Replace("'", "");
                                   DateTime dt;
                                   if (DateTime.TryParse(Xls_Fld, out dt))
                                   {
                                       SqlChk += delHash[0] + "='" + dt.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) + "' ";
                                   }
                                   else
                                   {
                                       SqlChk += delHash[0] + "='" + Xls_Fld + "' ";
                                   }
                               }
                           }
                           else if (RsImport.Columns[Xls_Fld].DataType == typeof(string))
                           {
                               if (IsNull(fieldValue))
                               {
                                   Xls_Fld = "";
                                   SqlChk += " (" + delHash[0] + "='" + Xls_Fld.ToUpper().Trim() + "' or " + delHash[0] + " is null  )  ";
                               }
                               else
                               {
                                   Xls_Fld = fieldValue.ToString().Replace("'", "");
                                   SqlChk += delHash[0] + "='" + Xls_Fld + "' ";
                               }
                           }
                           else
                           {
                               if (IsNull(fieldValue))
                               {
                                   Xls_Fld = "0";
                                   SqlChk += "(" + delHash[0] + "=" + Xls_Fld + " or " + delHash[0] + " is null  )  ";
                               }
                               else
                               {
                                   Xls_Fld = fieldValue.ToString().Replace("'", "");
                                   decimal decValue;
                                   if (Decimal.TryParse(Xls_Fld, out decValue))
                                   {
                                       SqlChk += delHash[0] + "=" + decValue.ToString("######0") + " ";
                                   }
                                   else
                                   {
                                       SqlChk += delHash[0] + "=" + Xls_Fld + " ";
                                   }
                               }
                           }
                       }
                       SqlChk += " and ";
                   }
                   // SqlChk = SqlChk & " Company_cd='" & Comp_Cd & "' "
                   SqlChk += " 1=1 ";
                   SqlChk = newString(SqlChk);
                   DataTable rschk = ExecuteSQL(SqlChk);
                   if (rschk.Rows.Count == 0)
                   {
                       countNull = 0;
                       Already_Exist = 0;
                       Sql = "Insert into nps_ecs_tbl_imp (" + dataDaseField + ",Import_dt,LOGGEDUserID)  Values(";
                       for (int i = 0; i < RsImport.Columns.Count; i++)
                       {
                           string fieldName = RsImport.Columns[i].ColumnName;
                           object fieldVal = row[fieldName];
                           if (fieldVal != null && fieldVal.ToString().IndexOf("'") >= 0)
                           {
                               string Value1 = fieldVal.ToString().Replace("'", "");
                               Sql += "'" + Value1 + "',";
                           }
                           else
                           {
                               if (RsImport.Columns[fieldName].DataType == typeof(DateTime) || IsStringDate(fieldVal))
                               {
                                   string formattedDate = "";
                                   if (!IsNull(fieldVal))
                                   {
                                       DateTime dt;
                                       if (DateTime.TryParse(fieldVal.ToString(), out dt))
                                       {
                                           formattedDate = dt.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture).Trim();
                                       }
                                   }
                                   Sql += "'" + formattedDate + "',";
                               }
                               else
                               {
                                   Sql += "'" + fieldVal.ToString().Trim() + "',";
                               }
                           }
                       }
                       Sql += "'" + ServerDateTime.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) + "','" + Glbloginid + "'";
                       Sql += ")";
                       Sql = Sql.Replace("''", "Null");
                       Sql = newString(Sql);
                       INSERT_RECORD(Sql);
                       if (countNull >= 50)
                           break;
                   }
                   else
                   {
                       Already_Exist = Already_Exist + 1;
                   }
                   Total_records = Total_records + 1;
                   Label5.Text = Total_records.ToString();
               }
           }

           Sql = "delete nps_ecs_tbl_imp where ref_tran_code is null";
           INSERT_RECORD(Sql);
           // Label5.Caption = Label5.Caption - 1
           int lblVal;
           if (int.TryParse(Label5.Text, out lblVal))
           {
               Label5.Text = (lblVal - 1).ToString();
           }
           // MsgBox " Out OF " & Str(Total_records - 1) & " Records " & Str(Already_Exist) & " Are Already Exist "
           pc.ShowAlert(this, " Out OF " + (Total_records - 1).ToString() + " Records " + Already_Exist.ToString() + " Are Already Exist ");
       
           return;
       }
       catch (Exception ex)
       {
           // 'err1:
           // '    Chk_OPENFORM = False
           // '    MsgBox err.Description
           // '    Set RsImport = Nothing
           // '    Set importExcelcon = Nothing
           // '    Set XLW = Nothing
           // '    Set XL = Nothing

           pc.ShowAlert(this, ex.Message);
           return;
       }
   }

   // Helper function that simulates executing an SQL statement and returns a DataTable.
   // In a real implementation this would connect to a database.
   private DataTable ExecuteSQL(string sql)
   {
       // Complete implementation using available functions.
       // For now, we return an empty DataTable as a stub.
       // Replace this with actual database call.
       DataTable dt = new DataTable();
       // ... actual SQL execution code ...
       return dt;
   }

   // Helper function that simulates inserting a record by executing an SQL command.
   private void INSERT_RECORD(string sql)
   {
       // Complete implementation using available functions.
       // For now, just call ExecuteSQL (which in a real scenario will execute non-query commands).
       ExecuteSQL(sql);
   }

   // Helper function that simulates cleaning up a SQL string by removing the trailing " and " if present.
   private string newString(string sql)
   {
       sql = sql.Trim();
       if (sql.EndsWith("and", StringComparison.InvariantCultureIgnoreCase))
       {
           sql = sql.Substring(0, sql.Length - 3).Trim();
       }
       else if (sql.EndsWith("and ", StringComparison.InvariantCultureIgnoreCase))
       {
           sql = sql.Substring(0, sql.Length - 4).Trim();
       }
       return sql;
   }

   // Helper function that simulates getting Excel data from a file as a DataTable.
   // Optionally, you can pass a list of columns (selectedFileField) to return subset columns.
   private DataTable GetExcelDataTable(string filePath, string sheetName, string selectedFileField = "*")
   {
       // Complete implementation that reads Excel file.
       // For now, we return a new DataTable with dummy columns.
       DataTable dt = new DataTable();
       // For simulation purposes, assume selectedFileField is comma separated list of column names.
       if (selectedFileField != "*" && !string.IsNullOrEmpty(selectedFileField))
       {
           string[] cols = selectedFileField.Split(',');
           foreach (string col in cols)
           {
               dt.Columns.Add(col.Trim());
           }
       }
       else
       {
           // Dummy columns
           dt.Columns.Add("Column1");
           dt.Columns.Add("Column2");
       }
       // Add dummy data rows if needed.
       return dt;
   }

   // Helper function that simulates checking if a field mapping is valid.
   private bool CheckFieldMapping(string databaseFields, string statusChk)
   {
       // Complete implementation
       // For simulation, return true.
       return true;
   }

   // Helper function that simulates the Get_Comp_TextFilePath call.
   private void Get_Comp_TextFilePath(string comp)
   {
       // Complete implementation as needed.
       // For simulation, do nothing.
   }

   // Helper function that simulates FieldsParametersName.
   private string FieldsParametersName(string fileName)
   {
       // Complete implementation that returns mapping fields string.
       // For simulation, return a dummy mapping if fileName is not empty.
       if (string.IsNullOrEmpty(fileName))
           return "";
       // For example, return "dbField1#ExcelField1,dbField2#ExcelField2"
       return "dbField1#ExcelField1,dbField2#ExcelField2";
   }

   // Helper function that simulates Exc_Clent_FldName.
   private string Exc_Clent_FldName()
   {
       // Complete implementation that returns a field name.
       // For simulation, return a dummy field name.
       return "ClientField";
   }

   // Helper function to simulate IsNull check.
   private bool IsNull(object value)
   {
       return (value == null || Convert.IsDBNull(value));
   }

   // Helper function to check if an object (expected a string) is a date string in the specified formats.
   private bool IsStringDate(object value)
   {
       if (IsNull(value))
           return false;
       string s = value.ToString();
       // Check conditions:
       // (Len(s)=10 and InStr(s, "/") = 3 and InStr(s, "/") = 6) or (Len(s)=9 and ... )
       if (s.Length == 10)
       {
           // In VB, InStr returns position starting at 1.
           // So we expect character at index 2 (position 3) and index 5 (position 6) to be '/'
           if (s.Length > 5 && s[2] == '/' && s[5] == '/')
               return true;
       }
       else if (s.Length == 9)
       {
           if (s.Length > 4 && s[2] == '/' && s[4] == '/')
               return true;
       }
       return false;
   }






   #endregion
    