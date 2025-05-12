using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using WM.Models;
using System.Web.Configuration;
using System.Web.ModelBinding;
using Oracle.ManagedDataAccess.Types;
using System.Web.UI;
using System.Web.UI.WebControls; 
using System.IO;
using System.Data.OleDb;
using ClosedXML.Excel;


namespace WM.Controllers
{
    public class PsmController
    {
        OracleConnection connection = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString);


        #region DDL GET SET


        public List<string> DdlValueToList(DropDownList ddl)
        {
            /* How to use this function
             * 
             * List<string> ddlValues = DdlValueToList(myDropDownList);
             * 
             * This will return a list of values from the DropDownList.
             */


            List<string> result = new List<string>();

            foreach (ListItem item in ddl.Items)
            {
                if (!string.IsNullOrWhiteSpace(item.Value))
                {
                    result.Add(item.Value.Trim());
                }
            }

            return result;
        }


        public List<string> DdlTextToList(DropDownList ddl)
        {
            /* How to use this function
             * 
             * List<string> ddlTexts = DdlTextToList(myDropDownList);
             * 
             * This will return a list of texts from the DropDownList.
             */


            List<string> result = new List<string>();

            foreach (ListItem item in ddl.Items)
            {
                if (!string.IsNullOrWhiteSpace(item.Value))
                {
                    result.Add(item.Text.Trim());
                }
            }

            return result;
        }

        public void ListToDdl(DropDownList ddl, List<string> textList, List<string> valueList = null, bool checkMatch = false)
        {
            /* How to use this function 
             * 
             * List<string> textList = new List<string> { "Item 1", "Item 2", "Item 3" };
             * List<string> valueList = new List<string> { "Value1", "Value2", "Value3" };
             * ListToDdl(dbFieldFromDb1, textList, valueList, checkMatch: true);
             * 
             * List<string> textList = new List<string> { "Item 1", "Item 2", "Item 3" };
             * ListToDdl(dbFieldFromDb1, textList, checkMatch: false);
             */

            ddl.Items.Clear(); // Clear any existing items

            // If checkMatch is true, ensure the lists match in length and content
            if (checkMatch)
            {
                // Ensure both lists are of the same size or handle mismatch
                int maxLength = Math.Max(textList.Count, valueList?.Count ?? 0);

                for (int i = 0; i < maxLength; i++)
                {
                    string text = (i < textList.Count) ? textList[i].Trim() : null;
                    string value = (i < valueList.Count) ? valueList[i].Trim() : null;

                    // If either text or value is null or whitespace, skip this item
                    if (!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(value))
                    {
                        ListItem item = new ListItem(text, value);
                        ddl.Items.Add(item);
                    }
                    else
                    {
                        // Add a blank or placeholder item if mismatch is detected
                        ddl.Items.Add(new ListItem(text ?? "", value ?? ""));
                    }
                }
            }
            else
            {
                // Regular adding when not checking for match
                if (valueList == null)
                {
                    foreach (string text in textList)
                    {
                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            ListItem item = new ListItem(text.Trim(), text.Trim());
                            ddl.Items.Add(item); // Add Text and Value as same
                        }
                    }
                }
                else
                {
                    if (textList.Count == valueList.Count)
                    {
                        for (int i = 0; i < textList.Count; i++)
                        {
                            string text = textList[i].Trim();
                            string value = valueList[i].Trim();

                            if (!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(value))
                            {
                                ListItem item = new ListItem(text, value);
                                ddl.Items.Add(item);
                            }
                            else
                            {
                                // Add a blank item if text or value is empty
                                ddl.Items.Add(new ListItem("", ""));
                            }
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Text and Value lists must have the same number of items.");
                    }
                }
            }
        }

        public DropDownList CreateDropDownFromList(List<string> sourceList)
        {
            /* How to use this function
             * 
             * List<string> sourceList = new List<string> { "Item 1", "Item 2", "Item 3" };
             * DropDownList ddl = CreateDropDownFromList(sourceList);
             * 
             * This will create a DropDownList with the items from the source list.
             */
            DropDownList ddl = new DropDownList();

            foreach (string item in sourceList)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    ddl.Items.Add(new ListItem(item.Trim(), item.Trim()));
                }
            }

            return ddl;
        }

        public DropDownList CreateDropDownFromMask(string csvItems)
        {
            /* How to use this function
             * 
             * string csvItems = "Item 1, Item 2, Item 3";
             * DropDownList ddl = CreateDropDownFromMask(csvItems);
             * 
             * This will create a DropDownList with the items from the CSV string.
             */
            DropDownList ddl = new DropDownList();

            if (!string.IsNullOrWhiteSpace(csvItems))
            {
                string[] items = csvItems.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string item in items)
                {
                    string trimmed = item.Trim();
                    if (!string.IsNullOrEmpty(trimmed))
                    {
                        ddl.Items.Add(new ListItem(trimmed, trimmed));
                    }
                }
            }

            return ddl;
        }


        public List<string> GetSplitedMaskList(List<string> inputList, int index)
        {
            /* How to use this function
             * 
             * List<string> inputList = new List<string> { "Field1 --> DBField1", "Field2 --> DBField2" };
             * int index = 0; // 0 for Excel field, 1 for DB field
             * List<string> result = GetSplitedMaskList(inputList, index);
             * 
             * This will return a list of either Excel or DB fields based on the index.
             */
            List<string> extractedParts = new List<string>();

            foreach (string item in inputList)
            {
                // Split the item based on '-->'
                string[] parts = item.Split(new string[] { "-->" }, StringSplitOptions.None);

                // If the split resulted in exactly two parts, process them
                if (parts.Length == 2)
                {
                    // If index is valid, add the corresponding part
                    if (index == 0) // Excel field (part before -->)
                    {
                        extractedParts.Add(parts[0].Trim());
                    }
                    else if (index == 1) // DB field (part after -->)
                    {
                        extractedParts.Add(parts[1].Trim());
                    }
                    else
                    {
                        // If an invalid index is passed, you can handle it or add an empty string
                        extractedParts.Add(string.Empty);
                    }
                }
            }

            return extractedParts;
        }


        public string[] GetMaskedSplitedString(string mask)
        {
            return mask.Split(new string[] { "-->" }, StringSplitOptions.None);
        }


        public List<string[]> GetListSplitWithCommaOfMaskedString(string maskedString)
        {
            List<string[]> result = new List<string[]>();

            if (!string.IsNullOrWhiteSpace(maskedString))
            {
                // Split by comma to get individual mask items
                string[] items = maskedString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string item in items)
                {
                    // Split each item by '-->' and add if valid
                    var parts = item.Split(new[] { "-->" }, StringSplitOptions.None);
                    if (parts.Length == 2)
                    {
                        result.Add(new string[] { parts[0].Trim(), parts[1].Trim() });
                    }
                }
            }

            return result;
        }


        public List<string> GetCommaSplittedListOfMask(string input)
        {
            /* How to use this function
             * 
             * string input = "Item 1, Item 2, Item 3";
             * List<string> result = GetCommaSplittedListOfMask(input);
             * 
             * This will return a list of items from the comma-separated string.
             */
            if (string.IsNullOrWhiteSpace(input))
                return new List<string>();

            return input
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Trim())
                .Where(item => !string.IsNullOrEmpty(item))
                .ToList();
        }



        public List<T> ListToList<T>(List<T> listA)
        {
            /* How to use this function
             * List<int> listA = new List<int> { 1, 2, 3, 4, 5 };
             * 
             * Call the function to copy listA to listB
             * List<int> listB = ListToList(listA);
             */

            List<T> listB = new List<T>();
            foreach (var item in listA)
            {
                listB.Add(item);  // Or apply a transformation here if needed
            }

            return listB;
        }







        public void SetDdlByText(string input, DropDownList ddl, bool doInsert = false)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                ListItem matchedItem = ddl.Items
                    .Cast<ListItem>()
                    .FirstOrDefault(item =>
                        string.Equals(item.Value.ToUpper(), input.ToUpper(), StringComparison.OrdinalIgnoreCase));

                if (matchedItem != null)
                {
                    ddl.ClearSelection();
                    matchedItem.Selected = true;
                }

                if (doInsert)
                {
                    ddl.Items.Add(new ListItem(input, input));
                    ddl.SelectedValue = input;
                }
            }
            else
            {
                if (ddl.Items.Count > 0)
                {
                    ddl.SelectedIndex = -1;
                }
            }
        }





        #endregion


        #region DB and UI NullHandler

        public string ToUpperTrimString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty; // Return empty string if input is null or empty
            }

            return input.ToUpper().Trim(); // Convert to uppercase and trim any whitespace
        }

        public bool isNullString(string value)
        {
            /* How to use this function
             * 
             * string value = "NULL";
             * bool isNull = isNullString(value);
             * 
             * This will return true if the value is null, "NULL", "N/A", or "NOT APPLICABLE".
             */
            return string.IsNullOrWhiteSpace(value) || value.ToUpper() == "NULL" || value.ToUpper() == "N/A" || value.ToUpper() == "NOT APPLICABLE";
        }

        public bool isNotNullString(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && value.ToUpper() != "NULL" && value.ToUpper() != "N/A" && value.ToUpper() != "NOT APPLICABLE";
        }



        public string ReplaceThis(string input, string replaceThis, string replaceWith = "")
        {

            /*
             * How to use this function
             * string cleaned1 = ReplaceSpecialCharacters("[Policy-No]", "[,],-", "");  // Output: PolicyNo
             * string cleaned2 = ReplaceSpecialCharacters("[Policy-No]", "[,],-", "_");  // Output: _Policy_No_
             */





            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(replaceThis))
                return input;

            // Split the replace list by comma
            string[] charsToReplace = replaceThis.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string item in charsToReplace)
            {
                string toReplace = item.Trim();
                if (!string.IsNullOrEmpty(toReplace))
                {
                    input = input.Replace(toReplace, replaceWith);
                }
            }

            return input;
        }



        public object NullHandlerStringUiToDb(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? DBNull.Value : (object)value;
        }

        public string SafeParseString(string input)
        {
            string value = "";

            if (!string.IsNullOrEmpty(input))
            {
                value = input;
            }

            return value;
        }

        public long SafeParseLong(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return 0;
            }

            if (long.TryParse(input, out long result))
            {
                return result;
            }

            return 0;
        }

        public double SafeParseDouble(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return 0;
            }

            if (double.TryParse(input, out double result))
            {
                return result;
            }

            return 0;
        }


        public decimal SafeParsedecimal(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return 0;
            }

            if (decimal.TryParse(input, out decimal result))
            {
                return result;
            }

            return 0;
        }

        public int SafeParseInd(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return 0;
            }

            if (int.TryParse(input, out int result))
            {
                return result;
            }

            return 0;
        }


        public string SafeParseCheckBoxZeroOne(bool isChecked)
        {
            return isChecked ? "1" : "0";
        }

        public string SafeParseCheckBoxYesNo(bool isChecked)
        {
            return isChecked ? "Y" : "N";
        }



        #endregion


        #region FILL FUNCTIONS

        // WEALTHMAKER.PSM_FILL_DDL_MASTER
        // A FUCIOTN TO RETRIEVE DATA FROM 'WEALTHMAKER.PSM_FILL_DDL_MASTER' PROCEDURE AND RETURN DATATABELR
        //public DataTable PSM_DDL_FILL_MASTER(string procedureName = "WEALTHMAKER.PSM_FILL_DDL_MASTER", string[] parameters, out int rowCount, out string exception)
        public DataTable PSM_DDL_FILL_MASTER(PsmModel pm, DropDownList ddl = null, bool isFirst = true)
        {
            pm.procedureName = "WEALTHMAKER.PSM_FILL_DDL_MASTER";
            //pm.parameters = new string[] { pm.LOGGEDUSERID, pm.ROLE_ID, pm.REGION_ID, pm.ZONE_ID, pm.BRANCH_CODE, pm.RM_CODE, pm.LIST_TYPE };
            

            DataTable dt = new DataTable();
            pm.rowCount = 0; // Default row count
            pm.exception = null; // Default exception as null

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand(pm.procedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure; // Set command type to stored procedure

                    // Add parameters if they exist
                    if (pm.parameters != null && pm.parameters.Length > 0)
                    {
                        for (int i = 0; i < pm.parameters.Length; i++)
                        {
                            cmd.Parameters.Add("P_VALUE" + (i + 1), OracleDbType.Varchar2).Value = pm.parameters[i];
                        }
                    }

                    // add two currors param
                    cmd.Parameters.Add("P_CURSOR1", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("P_CURSOR2", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {                      

                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {

                            adapter.Fill(dt); // Fill the DataTable with the result of the procedure
                            pm.rowCount = dt.Rows.Count; // Get the row count
                        }

                        string text = "FIELD_NAME";
                        string value = "FIELD_VALUE";
                        if (dt != null && dt.Rows.Count > 0 &&
                            dt.Columns.Contains(text) && dt.Columns.Contains(value))
                        {
                            pm.ddlText = dt.AsEnumerable()
                                           .Select(row => row[text].ToString())
                                           .ToArray();
                            pm.ddlValues = dt.AsEnumerable()
                                             .Select(row => row[value].ToString())
                                             .ToArray();

                            if (ddl != null)
                            {
                                ddl.DataSource = dt;
                                ddl.DataTextField = text;
                                ddl.DataValueField = value;
                                ddl.DataBind();
                                if (isFirst)
                                {
                                    ddl.Items.Insert(0, new ListItem("Select", ""));
                                }
                            }
                        }
                    }
                    catch (OracleException ex)
                    {
                        // Capture exception message
                        pm.exception = ex.Message;

                        // Create an empty DataTable with an Exception column
                        dt = new DataTable();
                        dt.Columns.Add("Exception", typeof(string));
                        dt.Rows.Add("Error: " + ex.Message);
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
            }

            return dt;
        }

        public static void FillDropDownWithLogBranch(string tableName, string textField, string valueField, string whereClause, DropDownList ddlToSet, Page page = null, Label messageLabel = null)
        {
            PsmController pc = new PsmController();

            try
            {
                // Build the SQL query
                string sql = $@"
            SELECT {textField} AS name, 
                   {valueField} AS value 
            FROM {tableName}";

                if (!string.IsNullOrWhiteSpace(whereClause))
                {
                    sql += $" {whereClause}";
                }

                // Add ORDER BY on the textField (display field)
                sql += $" ORDER BY {textField}";


                // make an object of PsmController

                DataTable dt = pc.ExecuteCurrentQueryMaster(sql, out int rowCount, out string isException);

                if (dt != null && dt.Rows.Count > 0 && string.IsNullOrEmpty(isException))
                {
                    ddlToSet.DataSource = dt;
                    ddlToSet.DataTextField = "name";
                    ddlToSet.DataValueField = "value";
                    ddlToSet.DataBind();
                    // Add default "Select" option at the top
                    ddlToSet.Items.Insert(0, new ListItem("Select", ""));

                }
                else
                {
                    ddlToSet.Items.Clear();
                    ddlToSet.Items.Add(new ListItem("-- No data found --", ""));

                    if (page != null)
                        pc.ShowAlert(page, "No data found for the selected input.");
                    if (messageLabel != null)
                        messageLabel.Text = "No data found.";
                }
            }
            catch (Exception ex)
            {
                ddlToSet.Items.Clear();

                if (page != null)
                    pc.ShowAlert(page, "Error: " + ex.Message);
                if (messageLabel != null)
                    messageLabel.Text = "Error: " + ex.Message;
            }
        }

        public static void FillDropDown(string tableName, string textField, string valueField, string whereClause, DropDownList ddlToSet, Page page = null, Label messageLabel = null)
        {
            PsmController pc = new PsmController();

            try
            {
                // Build the SQL query
                string sql = $@"
            SELECT {textField} AS name, 
                   {valueField} AS value 
            FROM {tableName}";

                if (!string.IsNullOrWhiteSpace(whereClause))
                {
                    sql += $" {whereClause}";
                }

                // Add ORDER BY on the textField (display field)
                sql += $" ORDER BY {textField}";


                // make an object of PsmController

                DataTable dt = pc.ExecuteCurrentQueryMaster(sql, out int rowCount, out string isException);

                if (dt != null && dt.Rows.Count > 0 && string.IsNullOrEmpty(isException))
                {
                    ddlToSet.DataSource = dt;
                    ddlToSet.DataTextField = "name";
                    ddlToSet.DataValueField = "value";
                    ddlToSet.DataBind();
                    // Add default "Select" option at the top
                    ddlToSet.Items.Insert(0, new ListItem("Select", ""));
                    ddlToSet.Enabled = true;

                }
                else
                {
                    ddlToSet.Items.Clear();

                    if (page != null)
                    {
                        ddlToSet.Items.Add(new ListItem("-- No data found --", ""));
                        pc.ShowAlert(page, "No data found for the selected input.");
                    }
                    if (messageLabel != null)
                    { messageLabel.Text = "No data found."; }

                    ddlToSet.Enabled = false;

                }
            }
            catch (Exception ex)
            {
                ddlToSet.Items.Clear();

                if (page != null)
                    pc.ShowAlert(page, "Error: " + ex.Message);
                if (messageLabel != null)
                    messageLabel.Text = "Error: " + ex.Message;
            }
        }


        #endregion


        #region Datatable Functions

        public string DtToUpperTrim(DataRow row, string columnName)
        {
            // Check if the column exists and has a value
            if (row != null && row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value)
            {
                // Retrieve the value, convert to string, trim, and convert to uppercase
                return row[columnName].ToString().ToUpper().Trim();
            }

            // Return empty string if column doesn't exist or value is DBNull
            return string.Empty;
        }

        public bool Dt_DoesColumnExistInRow(DataRow row, string columnName)
        {
            // Check if the row's table contains the column, ignoring case
            return row.Table.Columns.Cast<DataColumn>()
                                     .Any(col => string.Equals(col.ColumnName, columnName, StringComparison.OrdinalIgnoreCase));
        }


        public static List<string> GetDataTableHeaders(DataTable currentSheetData, object outputTarget = null)
        {
            /* How  TO Use 
            
            Get Header:
            var headers = PsmController.GetExcelHeadersFromDataTable(currentDataTable);

            Get and Set Headers to DDl
            var headers = PsmController.GetExcelHeadersFromDataTable(currentDataTable, myDropDownList);

            Get and Set Headers to Text fields of Label
            var headers = PsmController.GetExcelHeadersFromDataTable(currentDataTable, "out");
            lblHeaders.Text = string.Join(", ", headers);

             */
            
            
            List<string> headers = new List<string>();

            // Collect column names
            foreach (DataColumn column in currentSheetData.Columns)
            {
                headers.Add(column.ColumnName);
            }

            // If outputTarget is a DropDownList, populate it
            if (outputTarget is DropDownList ddl)
            {
                ddl.Items.Clear();
                foreach (string header in headers)
                {
                    ddl.Items.Add(new ListItem(header));
                }
            }
            // If outputTarget is the string "out", you might handle it elsewhere or return a string
            else if (outputTarget is string str && str.Equals("out", StringComparison.OrdinalIgnoreCase))
            {
                // You could optionally return a comma-separated version or store it elsewhere
                string headerString = string.Join(", ", headers); 
            }

            // Always return the list of headers
            return headers;
        }


        #endregion




        #region Global Mapping Functions

       
        public bool GetMappedFields(string mappingFor, string mappingType,out string getMask, out List<string> mappedExcelFields, out List<string> mappedDbFields)
        {

            /* --== How to use this function ==--
             * 
             * List<string> excelFieldFromDb = new List<string>();
             * List<string> dbFieldFromDb = new List<string>();
             * string mappingFor = "DUE_AND_DATA";
             * string mappingType = "DUE";
             * string isMaskFound = pc.GetMappedFields(mappingFor, mappingType, out excelFieldFromDb, out dbFieldFromDb);
             */

            getMask = string.Empty;
            mappedExcelFields = new List<string>();
            mappedDbFields = new List<string>();


            mappedExcelFields = new List<string>();
            mappedDbFields = new List<string>();

            string getOldMapped = $@"
        SELECT 
            FIELD_NAME AS TEXT, 
            FIELD_VALUE AS VALUE 
        FROM PSM_GLOBAL_WORKING_DATA 
        WHERE UPPER(DATA_TYPE_1) = 'MAPPED_MASK' 
            AND UPPER(DATA_TYPE_2) = UPPER('{mappingFor}')
            AND UPPER(DATA_TYPE_3) = UPPER('{mappingType}')
            AND ROWNUM = 1
            ";
            DataTable dt = ExecuteCurrentQueryMaster(getOldMapped, out int rc, out string ie);

            if (rc > 0 && string.IsNullOrEmpty(ie) && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                // You can choose either TEXT or VALUE, or merge both if needed
                string mappingsRaw = row["TEXT"]?.ToString(); // or row["VALUE"]
                getMask = row["TEXT"]?.ToString();
                if (!string.IsNullOrEmpty(mappingsRaw))
                {
                    string[] mappings = mappingsRaw.Split(',');
                    foreach (string pair in mappings)
                    {
                        string cleaned = pair.Trim();
                        if (cleaned.Contains("-->"))
                        {
                            int arrowIndex = cleaned.IndexOf("-->");
                            string excelField = cleaned.Substring(0, arrowIndex).Trim().TrimStart('[').TrimEnd(']');
                            string dbField = cleaned.Substring(arrowIndex + 3).Trim();

                            mappedExcelFields.Add(excelField);
                            mappedDbFields.Add(dbField);
                        }
                    }

                    return mappedExcelFields.Count > 0 && mappedDbFields.Count > 0;
                }
            }

            return false;
        }


        #endregion


        #region EXCEL HANDLER FUNCTIONS

        // 1. EXCEL CONNECTION
        public static string GetExcelConnectionString(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            string connectionString = string.Empty;

            // Ensure backslashes are escaped for use in connection strings
            string safePath = filePath.Replace(@"\", @"\\");

            if (extension == ".xls")
            {
                connectionString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={safePath};Extended Properties='Excel 8.0;HDR=Yes;IMEX=1';";
            }
            else if (extension == ".xlsx")
            {
                connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={safePath};Extended Properties='Excel 12.0 Xml;HDR=Yes;IMEX=1';";
            }

            return connectionString;
        }

        // 2. LOAD EXCEL SHEETS IN DDL
        public static List<ListItem> GetExcelSheetList(string filePath, DropDownList ddlToSet = null)
        {
            List<ListItem> items = new List<ListItem>
    {
        new ListItem("Select Sheet", "")
    };

            using (OleDbConnection conn = new OleDbConnection(GetExcelConnectionString(filePath)))
            {
                conn.Open();
                DataTable dtSheets = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                if (dtSheets != null && dtSheets.Rows.Count > 0)
                {
                    foreach (DataRow row in dtSheets.Rows)
                    {
                        string sheetName = row["TABLE_NAME"].ToString();
                        sheetName = sheetName.Replace("$", "").Replace("'", "");
                        items.Add(new ListItem(sheetName, sheetName));
                    }
                }
                else
                {
                    items.Add(new ListItem("No sheets found", ""));
                }
            }

            // Optionally bind to DropDownList
            if (ddlToSet != null)
            {
                ddlToSet.Items.Clear();
                ddlToSet.Items.AddRange(items.ToArray());
            }

            return items;
        }

        public static List<string> GetExcelHeaders(string filePath, string sheetName, DropDownList ddlToSet = null)
        {
            List<string> headers = new List<string>();

            using (OleDbConnection conn = new OleDbConnection(GetExcelConnectionString(filePath)))
            {
                conn.Open();
                string query = $"SELECT * FROM [{sheetName}$]";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                using (OleDbDataReader reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    DataTable schemaTable = reader.GetSchemaTable();

                    foreach (DataRow row in schemaTable.Rows)
                    {
                        string columnName = row["ColumnName"].ToString();
                        headers.Add(columnName);
                    }
                }
            }

            // Optional: Set to DropDownList
            if (ddlToSet != null)
            {
                ddlToSet.Items.Clear();
                ddlToSet.Items.Add(new ListItem("Select Column", ""));

                foreach (var header in headers)
                {
                    ddlToSet.Items.Add(new ListItem(header, header));
                }
            }

            return headers;
        }


        public static List<string> GetExcelHeadersFromDataTable(DataTable table, DropDownList ddlToSet = null)
        {
            List<string> headers = new List<string>();

            if (table == null || table.Columns.Count == 0)
                return headers;

            foreach (DataColumn col in table.Columns)
            {
                headers.Add(col.ColumnName);
            }
 

            headers.Sort();
            // Optionally bind to DropDownList
            if (ddlToSet != null)
            {
                ddlToSet.Enabled=true;
                ddlToSet.Items.Clear();
                ddlToSet.Items.Add(new ListItem("Select Column", ""));

                foreach (var header in headers)
                {
                    ddlToSet.Items.Add(new ListItem(header, header));
                }
            }else
            {
                ddlToSet.Enabled = false;
            }






           
            return headers;
        }


        // 3. LOAD EXCEL DATA
        public static DataTable LoadSheetData(string filePath, string sheetName, GridView gridToSet= null)
        {
            DataTable dtResult = new DataTable();

            using (OleDbConnection conn = new OleDbConnection(GetExcelConnectionString(filePath)))
            {
                conn.Open();
                string query = $"SELECT * FROM [{sheetName}$]";
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                    {
                        adapter.Fill(dtResult);

                        // Set the DataTable as the DataSource for the GridView
                        if (gridToSet != null)
                        {
                            gridToSet.DataSource = dtResult;
                            gridToSet.DataBind();
                        }
                    }
                }
            }

            return dtResult;
        }


        public static DataTable GetExcelSheetData(string filePath, string sheetName, out DataTable fetchData , GridView gridToSet = null, Label lblMesage = null)
        {
            DataTable dtResult = new DataTable();

            DataTable dt = new DataTable();
            using (OleDbConnection conn = new OleDbConnection(GetExcelConnectionString(filePath)))
            {
                conn.Open();
                string query = $"SELECT * FROM [{sheetName}$]";
                using (OleDbDataAdapter adapter = new OleDbDataAdapter(query, conn))
                {
                    adapter.Fill(dt);
                }
            }
            fetchData = dt;

            if (gridToSet != null)
            {
                if (dt.Rows.Count > 0)
                {
                    if (lblMesage != null)
                    {
                        lblMesage.Text = $"Selected sheet: {sheetName}, has {dt.Rows.Count} rows.";
                    } 

                    gridToSet.DataSource = dt;
                    gridToSet.DataBind();
                }
                else
                {
                    if (lblMesage != null)
                    {
                        lblMesage.Text = $"";
                    }
                    gridToSet.DataSource = null;
                    gridToSet.DataBind();
                }
            }
            
            return dt;
        }


        public void ExportToExcelByDT(DataTable dataToExport)
        {
            if (dataToExport != null && dataToExport.Rows.Count > 0)
            {
                try
                {
                    // Create a new workbook
                    using (var workbook = new XLWorkbook())
                    {
                        // Add a worksheet to the workbook
                        var worksheet = workbook.AddWorksheet("Policy Data");

                        // Add the header row (from DataTable columns) with borders
                        for (int col = 0; col < dataToExport.Columns.Count; col++)
                        {
                            var headerCell = worksheet.Cell(1, col + 1);
                            headerCell.Value = dataToExport.Columns[col].ColumnName;
                            headerCell.Style.Font.Bold = true;
                            headerCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            headerCell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        }

                        // Add the data rows (from DataTable rows) with borders
                        for (int row = 0; row < dataToExport.Rows.Count; row++)
                        {
                            for (int col = 0; col < dataToExport.Columns.Count; col++)
                            {
                                object cellValue = dataToExport.Rows[row][col];

                                // Replace NULL values with a space
                                string cellText = cellValue != DBNull.Value ? cellValue.ToString() : " ";

                                // Remove HTML encoded spaces if present
                                if (!string.IsNullOrEmpty(cellText) && cellText.Contains("&nbsp;"))
                                {
                                    cellText = cellText.Replace("&nbsp;", " ");
                                }

                                // Insert value into the worksheet
                                var cell = worksheet.Cell(row + 2, col + 1);
                                cell.Value = string.IsNullOrEmpty(cellText) ? " " : cellText;
                                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                            }
                        }

                        // Prepare response for Excel download
                        HttpContext.Current.Response.Clear();
                        HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=PolicyData.xlsx");

                        using (var memoryStream = new MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                            HttpContext.Current.Response.Buffer = false;
                            HttpContext.Current.Response.End();

                            /*
                            HttpContext.Current.Response.Flush();
                            HttpContext.Current.Response.SuppressContent = true;
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                             */
                        }
                        
                    }
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                HttpContext.Current.Response.Write("No data to export.");
            }
        }

        public void ExportToExcelByDT2(DataTable dt, string fileName)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "Sheet1");
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    byte[] data = stream.ToArray();

                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    HttpContext.Current.Response.AddHeader("content-disposition", $"attachment;filename={fileName}");
                    HttpContext.Current.Response.BinaryWrite(data);
                    HttpContext.Current.Response.End();
                }
            }
        }

        #endregion





        #region DDL DATA BINDING CONTROLS


        public DataTable get_BRANCH_RM(Models.PsmModel pm)
        {
            DataTable dtBrnachRM = new DataTable();

            using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                try
                {
                    con.Open();
                    using (OracleCommand cmd = new OracleCommand("PSM_BRANCH_RM_MASTER", con)) // Corrected procedure name
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Adding required parameters
                        cmd.Parameters.Add("P_REGIONID", OracleDbType.Varchar2).Value = pm.REGION_ID;
                        cmd.Parameters.Add("P_ZONE_ID", OracleDbType.Varchar2).Value = pm.ZONE_ID;
                        cmd.Parameters.Add("P_LOGINID", OracleDbType.Varchar2).Value = pm.LOGGEDUSERID;
                        cmd.Parameters.Add("P_ROLEID", OracleDbType.Varchar2).Value = pm.ROLE_ID;
                        cmd.Parameters.Add("P_BRANCH_CODE", OracleDbType.Varchar2).Value = pm.BRANCH_CODE;
                        cmd.Parameters.Add("P_RM_CODE", OracleDbType.Varchar2).Value = pm.RM_CODE;
                        cmd.Parameters.Add("P_LIST_TYPE", OracleDbType.Varchar2).Value = pm.LIST_TYPE;

                        // Output parameter for error message
                        cmd.Parameters.Add("P_ERRORMESSAGE", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;

                        // Output cursor
                        cmd.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dtBrnachRM);
                        }

                        // Fetch error message
                        string errorMessage = Convert.ToString(cmd.Parameters["P_ERRORMESSAGE"].Value);
                        //if (!string.IsNullOrEmpty(errorMessage) && errorMessage == "null")
                        //{
                        //    throw new ArgumentException(errorMessage);
                        //}
                    }
                }
                catch (Exception ex)
                {
                    // Log the error (optional)
                    Console.WriteLine("Error in PSM_BRANCH_RM: " + ex.Message);
                    throw;
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }

            return dtBrnachRM;
        }


        #endregion



        public bool ExecuteNonQueryMaster(string query, out string exception)
        {
            exception = null; // Default to no exception

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery(); // Execute COMMIT or other non-query commands
                        return true; // Return success
                    }
                    catch (OracleException ex)
                    {
                        exception = ex.Message; // Capture exception message
                        return false; // Return failure
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
            }
        }


        public string isException(DataTable dt)
        {
            return dt.Columns.Contains("Exception") ? dt.Rows[0]["Exception"]?.ToString() : null;

        }

        public DataTable ExecuteCurrentQuery(string query)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(dt); // Fill the DataTable with the result of the query
                        }
                    }
                    catch (OracleException ex)
                    {
                        // Handle exceptions and log them
                        Console.WriteLine("Error: " + ex.Message);
                        dt.Columns.Add("Exception", typeof(string));
                        dt.Rows.Add("Error: " + ex.Message);
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
            }

            return dt;
        }


        public DataTable ExecuteStoredProcedureMaster(string procedureName, OracleParameter[] parameters, out int rowCount, out string exception)
        {
            DataTable dt = new DataTable();
            rowCount = 0; // Default row count
            exception = null; // Default exception as null

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand(procedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure; // Set command type to stored procedure

                    // Add parameters if they exist
                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(dt); // Fill the DataTable with the result of the procedure
                            rowCount = dt.Rows.Count; // Get the row count
                        }
                    }
                    catch (OracleException ex)
                    {
                        // Capture exception message
                        exception = ex.Message;

                        // Create an empty DataTable with an Exception column
                        dt = new DataTable();
                        dt.Columns.Add("Exception", typeof(string));
                        dt.Rows.Add("Error: " + ex.Message);
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
            }

            return dt;
        }


        public DataTable ExecuteCurrentQueryMaster(string query, out int rowCount, out string exception)
        {
            DataTable dt = new DataTable();
            rowCount = 0; // Default row count
            exception = null; // Default exception as null

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(dt); // Fill the DataTable with the result of the query
                            rowCount = dt.Rows.Count; // Get the row count
                        }
                    }
                    catch (OracleException ex)
                    {
                        // Capture exception message
                        exception = ex.Message;

                        // Create an empty DataTable with an Exception column
                        dt = new DataTable();
                        dt.Columns.Add("Exception", typeof(string));
                        dt.Rows.Add("Error: " + ex.Message);
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
            }

            return dt;
        }

        public DataTable ExecuteCurrentQuery3(string query, out int rowCount, out string exception, out bool isExecuted)
        {
            DataTable dt = new DataTable();
            rowCount = 0;
            exception = null;
            isExecuted = false; // Default to false

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                            rowCount = dt.Rows.Count;
                            isExecuted = true; // Set to true only if fill succeeds
                        }
                    }
                    catch (OracleException ex)
                    {
                        exception = ex.Message;

                        dt = new DataTable();
                        dt.Columns.Add("Exception", typeof(string));
                        dt.Rows.Add("Error: " + ex.Message);

                        isExecuted = false; // Execution failed
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
            }

            return dt;
        }


        public DataTable ExecuteQuery(string query)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_SQL_RUN_QUERY", conn)) // Call the procedure
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        conn.Open();

                        // Pass the dynamic SQL query as input
                        cmd.Parameters.Add("p_query", OracleDbType.Clob).Value = query;

                        // Define the output cursor parameter
                        OracleParameter resultCursor = new OracleParameter("p_result", OracleDbType.RefCursor);
                        resultCursor.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(resultCursor);

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader); // Load result into DataTable
                        }
                    }
                    catch (OracleException ex)
                    {
                        // Handle exceptions and log them
                        Console.WriteLine("Error: " + ex.Message);
                        dt.Columns.Add("Exception", typeof(string));
                        dt.Rows.Add("Error: " + ex.Message);
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
            }

            return dt;
        }

        #region GetCityList
        public DataTable GetCityList()
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_GET_CITY_LIST", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "p_cursor",
                            OracleDbType = OracleDbType.RefCursor,
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(cursorParam);

                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine("Error: " + ex.Message);
            }

            return dt;
        }
        #endregion

        public DataTable LogBranchList(string loginId, string roleId)
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_GET_BRANCHES", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Input parameter for Login ID
                        command.Parameters.Add(new OracleParameter("P_LOGIN", OracleDbType.Varchar2)
                        {
                            Value = loginId,
                            Direction = ParameterDirection.Input
                        });

                        // Input parameter for Role ID
                        command.Parameters.Add(new OracleParameter("P_ROLEID", OracleDbType.Varchar2)
                        {
                            Value = roleId,
                            Direction = ParameterDirection.Input
                        });

                        // Output parameter for the cursor
                        command.Parameters.Add(new OracleParameter("P_BRANCHES", OracleDbType.RefCursor)
                        {
                            Direction = ParameterDirection.Output
                        });

                        conn.Open(); // Open the connection before execution

                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt); // Fill the DataTable with the results
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            // Convert DataTable to string array of branch codes
            return dt;
        }

        public string[] GetBranchCodes(string loginId, string roleId)
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_GET_BRANCHES", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Input parameter for Login ID
                        command.Parameters.Add(new OracleParameter("P_LOGIN", OracleDbType.Varchar2)
                        {
                            Value = loginId,
                            Direction = ParameterDirection.Input
                        });

                        // Input parameter for Role ID
                        command.Parameters.Add(new OracleParameter("P_ROLEID", OracleDbType.Varchar2)
                        {
                            Value = roleId,
                            Direction = ParameterDirection.Input
                        });

                        // Output parameter for the cursor
                        command.Parameters.Add(new OracleParameter("P_BRANCHES", OracleDbType.RefCursor)
                        {
                            Direction = ParameterDirection.Output
                        });

                        conn.Open(); // Open the connection before execution

                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt); // Fill the DataTable with the results
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine("Error: " + ex.Message);
                return new string[0]; // Return an empty array in case of an error
            }

            // Convert DataTable to string array of branch codes
            return dt.AsEnumerable()
                     .Select(row => row["BRANCH_CODE"].ToString())
                     .ToArray();
        }
        public string currentLoginID()
        {
            string currentLoginId = HttpContext.Current.Session["LoginId"] as string;

            return currentLoginId;
        }

        public string currentRoleID()
        {
            string currentRoleId = HttpContext.Current.Session["RoleId"] as string;

            return currentRoleId;
        }

        public string LogBranches(string currentLoginId = null, string currentRoleId = null)
        {
            currentLoginId = HttpContext.Current.Session["LoginId"] as string;
            currentRoleId = HttpContext.Current.Session["RoleId"] as string;

            string[] branchCodes = new string[0];
            if (!string.IsNullOrEmpty(currentLoginId) && !string.IsNullOrEmpty(currentRoleId))
            {
                branchCodes = GetBranchCodes(currentLoginId, currentRoleId);
            }

            return string.Join(",", branchCodes);
        }

        public string GlbloginidHardCoded()
        {
            // List to store the Glbloginid and GlbroleId values
            List<string> values = new List<string>();

            // Add hardcoded values for Glbloginid and GlbroleId
            values.Add("38387");
            values.Add("101877");
            values.Add("97120");
            values.Add("43452");
            values.Add("38423");
            values.Add("46183");
            values.Add("97117");
            values.Add("212");
            values.Add("242");

            // Join the list into a single string separated by commas and return it
            return string.Join(",", values);
        }



        public string psm_Branches(string currentLoginId = null, string currentRoleId = null)
        {
            currentLoginId = HttpContext.Current.Session["LoginId"] as string;
            currentRoleId = HttpContext.Current.Session["RoleId"] as string;

            if (string.IsNullOrEmpty(currentLoginId) || string.IsNullOrEmpty(currentRoleId))
            {
                return string.Empty;
            }

            string sql = "SELECT BM.BRANCH_CODE, BM.BRANCH_NAME FROM BRANCH_MASTER BR INNER JOIN USERDETAILS_JI UJ ON BR.BRANCH_CODE = UJ.BRANCH_ID WHERE UJ.LOGIN_ID = '" + currentLoginId + "' AND UJ.ROLE_ID = '" + currentRoleId + "' ORDER BY BRANCH_NAME";

            DataTable dtBranchList = ExecuteCurrentQuery(sql);

            if (dtBranchList != null && dtBranchList.Rows.Count > 0 &&
                dtBranchList.Columns.Contains("BRANCH_NAME") && dtBranchList.Columns.Contains("BRANCH_CODE"))
            {
                string[] branchCodes = dtBranchList.AsEnumerable()
                                                   .Select(row => row["BRANCH_CODE"].ToString())
                                                   .ToArray();
                return string.Join(",", branchCodes);
            }

            return string.Empty;
        }

        public string psm_RMs(Func<string> psm_Branches = null)
        {
            // Get branches from the provided function
            //string branches = psm_Branches.Invoke();
            string currentLoginId = HttpContext.Current.Session["LoginId"] as string;
            string currentRoleId = HttpContext.Current.Session["RoleId"] as string;

            if (string.IsNullOrEmpty(currentLoginId) || string.IsNullOrEmpty(currentRoleId))
            {
                return string.Empty;
            }

            string sql = "SELECT BM.BRANCH_CODE, BM.BRANCH_NAME FROM BRANCH_MASTER BM INNER JOIN USERDETAILS_JI UJ ON BM.BRANCH_CODE = UJ.BRANCH_ID WHERE UJ.LOGIN_ID = '" + currentLoginId + "' AND UJ.ROLE_ID = '" + currentRoleId + "' ORDER BY BRANCH_NAME";

            DataTable dtBranchList = ExecuteCurrentQuery(sql);

            string currentBranchCode = string.Empty;
            if (dtBranchList != null && dtBranchList.Rows.Count > 0 &&
                dtBranchList.Columns.Contains("BRANCH_NAME") && dtBranchList.Columns.Contains("BRANCH_CODE"))
            {
                string[] branchCodes = dtBranchList.AsEnumerable()
                                                   .Select(row => row["BRANCH_CODE"].ToString())
                                                   .ToArray();
                currentBranchCode = string.Join(",", branchCodes);
            }

 

            sql = "SELECT RM_CODE, RM_NAME FROM employee_master WHERE source IN (" + currentBranchCode + ")";

             dtBranchList = ExecuteCurrentQuery(sql);
            string currentRmCodes = string.Empty;
            if (dtBranchList != null && dtBranchList.Rows.Count > 0 &&
                dtBranchList.Columns.Contains("RM_CODE") && dtBranchList.Columns.Contains("RM_NAME"))
            {
                string[] RmCodes = dtBranchList.AsEnumerable()
                                               .Select(row => row["RM_CODE"].ToString())
                                               .ToArray();
                currentRmCodes = string.Join(",", RmCodes);
            }

            return currentRmCodes;
        }



        public DataTable GetLogBranchList(string currentLoginId = null, string currentRoleId = null)
        {
            currentLoginId = HttpContext.Current.Session["LoginId"] as string;
            currentRoleId = HttpContext.Current.Session["RoleId"] as string;
            DataTable dt = new DataTable();

            string[] branchCodes = new string[0];
            if (!string.IsNullOrEmpty(currentLoginId) && !string.IsNullOrEmpty(currentRoleId))
            {
                dt = LogBranchList(currentLoginId, currentRoleId);
            }

            return dt;
        }

        public void ShowAlert(Page page, string message)
        {
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page), "Page instance cannot be null.");
            }

            // Handle null message by using a default value
            message = message == null ? "" : message;

            // Sanitize the message to handle special characters and prevent JavaScript errors
            string sanitizedMessage = message.Replace("'", "\\'").Replace("\n", "\\n");

            // Check if the request is from an UpdatePanel (Partial PostBack)
            if (ScriptManager.GetCurrent(page)?.IsInAsyncPostBack == true)
            {
                // Use ScriptManager for UpdatePanel partial postbacks
                ScriptManager.RegisterStartupScript(
                    page,
                    page.GetType(),
                    "alert",
                    $"alert('{sanitizedMessage}');",
                    true
                );
            }
            else
            {
                // Use ClientScript for regular full postbacks
                page.ClientScript.RegisterStartupScript(
                    page.GetType(),
                    "alert",
                    $"alert('{sanitizedMessage}');",
                    true
                );
            }
        }


        private void ClearTextBoxes(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is TextBox)
                {
                    ((TextBox)c).Text = string.Empty;
                }
                else if (c.HasControls())
                {
                    ClearTextBoxes(c);
                }
            }
        }

        public string RowFoundMsg(DataTable dt)
        {
            string gettingRowsCount = dt.Rows.Count.ToString();
            string recordText = dt.Rows.Count == 1 ? "record" : "records";
            return gettingRowsCount + " " + recordText + " found!";
        }


        public string SafeHtml(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return HttpUtility.HtmlEncode(input);
        }

        public string SafeJavaScript(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return HttpUtility.JavaScriptStringEncode(input);
        }



        // redirect to welcome page with login id and roles roel or session
        public void RedirectToWelcomePage(string loginId = null, string roleId = null)
        {
            // Log the current session values for debugging
            string currentLoginId = HttpContext.Current.Session["LoginId"] as string;
            string currentRoleId = HttpContext.Current.Session["RoleId"] as string;
            Console.WriteLine($"Current Session LoginId: {currentLoginId}, RoleId: {currentRoleId}");

            // Update session values only if the parameters are not null
            if (loginId != null)
            {
                HttpContext.Current.Session["LoginId"] = loginId;
            }
            if (roleId != null)
            {
                HttpContext.Current.Session["RoleId"] = roleId;
            }

            // Log the updated session values for debugging
            currentLoginId = HttpContext.Current.Session["LoginId"] as string;
            currentRoleId = HttpContext.Current.Session["RoleId"] as string;
            Console.WriteLine($"Updated Session LoginId: {currentLoginId}, RoleId: {currentRoleId}");

            if (string.IsNullOrEmpty(currentLoginId) || string.IsNullOrEmpty(currentRoleId))
            {
                HttpContext.Current.Response.Redirect("https://wealthmaker.in/login_new.aspx");
            }
            else
            {
                string welcomeUrl = $"~/welcome?loginid={HttpUtility.UrlEncode(currentLoginId)}&roleid={HttpUtility.UrlEncode(currentRoleId)}";
                HttpContext.Current.Response.Redirect(welcomeUrl);
            }
        }


        // add Page page
        public void ReloadCurrentPage(Page page)
        {
            try
            {
                if (page != null)
                {
                    if (ScriptManager.GetCurrent(page) != null && ScriptManager.GetCurrent(page).IsInAsyncPostBack)
                    {
                        // If it's an async postback, use JavaScript to refresh the page
                        ScriptManager.RegisterStartupScript(page, page.GetType(), "ReloadPageScript", "window.location.reload();", true);
                    }
                    else
                    {
                        // Regular postback, use Response.Redirect
                        HttpContext.Current.Response.Redirect(HttpContext.Current.Request.RawUrl, false);
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                    }
                }
            }

            catch (Exception ex)
            {

            }
        }



        public List<string> GetTableColumns(string tableName)
        {
            List<string> columnNames = new List<string>();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand("PSM_GET_TABLE_COLUMN", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Input parameter (table name)
                        cmd.Parameters.Add("p_table_name", OracleDbType.Varchar2).Value = tableName.ToUpper();

                        // Output parameter (cursor)
                        cmd.Parameters.Add("v_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        conn.Open();
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                columnNames.Add(reader.GetString(0)); // Fetch column names
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception properly instead of just printing
                System.Diagnostics.Debug.WriteLine("Error fetching table columns: " + ex.Message);
            }

            return columnNames; // Return the list even if it's empty in case of failure
        }
    }
}