             if (OptNPS)
             {
                 pc.ExecuteCurrentQueryMaster("insert into nps_nonecs_tbl_imp_bk select * from  nps_nonecs_tbl_imp", out _, out _);
                 DataTable dtDel = pc.ExecuteCurrentQueryMaster(" delete from  nps_nonecs_tbl_imp ", out _, out _);

                 Total_records = 0;
                 foreach (DataRow row in sheetDataTable.Rows)
                 {
                     sql = "";
                     string SqlChk = "";
                     string Xls_Fld = "";
                     SqlChk = " select * from  nps_nonecs_tbl_imp where ";
                     for (int i = 0; i <= delComma.GetUpperBound(0); i++)
                     {
                         string[] delHash = delComma[i].Split('#');
                         Xls_Fld = delHash[1].Replace("[", "").Replace("]", "").Replace("'", "");
                         if (Xls_Fld.IndexOf("&") >= 0)
                         {
                             Xls_Fld = Exc_Clent_FldName(); // need to check
                         }
                         if (sheetDataTable.Columns.Contains(Xls_Fld))
                         {
                             object fieldValue = row[Xls_Fld];
                             if (sheetDataTable.Columns[Xls_Fld].DataType == typeof(DateTime))
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
                             else if (sheetDataTable.Columns[Xls_Fld].DataType == typeof(string))
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
                             SqlChk += " and ";
                         }
                     }
                     // SqlChk = SqlChk & " Company_cd='" & Comp_Cd & "' "
                     SqlChk += " 1=1 ";
                     SqlChk = newString(SqlChk);
                     DataTable rschk = pc.ExecuteCurrentQueryMaster(SqlChk, out int rnRschk, out string ieRschk);
                     // JIN FIELD SE MAPPING KI HAI UNHI FIELD SE CHECK KARTE HAI KI VO RECORD DATABASE KE ANDAR EXIST HAI YA NAHI
                     // AGAR NAHI HAI TO  Bajaj_PolicyInfo_Data KE ANDAR INSERT KARA DO

                     if (rschk.Rows.Count == 0)
                     {
                         countNull = 0;
                         Already_Exist = 0;
                         sql = "Insert into  nps_nonecs_tbl_imp (" + dataDaseField + ",Import_dt,LOGGEDUserID)  Values(";
                         for (int i = 0; i < sheetDataTable.Columns.Count; i++)
                         {
                             string fieldName = sheetDataTable.Columns[i].ColumnName;

                             if (selectedFileField.Contains(fieldName))
                             {
                                 object fieldVal = row[fieldName];
                                 string Value1 = "";
                                 if (fieldVal != null && fieldVal.ToString().IndexOf("'") >= 0)
                                 {
                                      Value1 = fieldVal.ToString().Replace("'", "");
                                     sql += "'" + Value1 + "',";
                                 }
                                 else
                                 {
                                     if (sheetDataTable.Columns[fieldName].DataType == typeof(DateTime) || IsStringDate(fieldVal))
                                     {
                                         string formattedDate = fieldVal.ToString();
                                         if (!IsNull(fieldVal))
                                         {
                                             //TO_DATE('01/02/2021', 'DD/MM/YYYY')
                                             formattedDate = " TO_DATE('" + formattedDate + " ', 'DD/MM/YYYY') ";
                                         }
                                         sql += " " + formattedDate + " ,";
                                     }
                                     else
                                     {
                                         sql += "'" + fieldVal.ToString().Trim() + "',";
                                     }
                                 }
                             }
                         }
                         sql += "sysdate, '"  + Glbloginid + "'";
                         sql += ")";
                         //sql = sql.Replace("''", "Null");
                         sql = newString(sql);

                         
                         DataTable insertOptNPS = pc.ExecuteCurrentQueryMaster(sql, out int rninsertOptNPS, out string ieinsertOptNPS);

                         if (countNull >= 50)
                         {
                             break;
                         }
                         else
                         {
                             Already_Exist = Already_Exist + 1;
                         }
                         Total_records = Total_records + 1;
                         //Label5.Text = Total_records.ToString();
                     }
                 }
             }
