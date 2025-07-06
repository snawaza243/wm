        #region PROCESS BULK IMPORTING DATA FOR DUE DATA
        public void ProcessBulkDueData(DataTable excelData, int month, int year, string MyImportDataType, string logginId, string curDate, string ddlImportDataType,  DateTime ServerDateTime, CheckBox chkOptDue)
        {
            if (!chkOptDue.Checked) return;

            // Create batches for processing
            const int batchSize = 1000;
            int totalRecords = excelData.Rows.Count;
            int processedRecords = 0;
            int insertCount = 0;
            int updateCount = 0;

            // Create lists for batch processing
            var insertRecords = new List<DataRow>();
            var updateRecords = new List<DataRow>();
            var lapsedPolicies = new List<Tuple<string, string>>();

            // Pre-calculate values that don't change
            string fileNameNe = "DuePaidMappedField" + ddlImportDataType + ".txt";
            string currentDateFormatted = ServerDateTime.ToString("dd-MMM-yyyy");

            // Process records in batches
            for (int i = 0; i < totalRecords; i += batchSize)
            {
                int endIndex = Math.Min(i + batchSize, totalRecords);
                var batch = excelData.AsEnumerable().Skip(i).Take(endIndex - i);

                // Clear batch lists
                insertRecords.Clear();
                updateRecords.Clear();
                lapsedPolicies.Clear();

                // Categorize records
                foreach (DataRow row in batch)
                {
                    if (!PsmController.IsContainAndNotNull(row, Excel_Comp)) continue;

                    string policyNo = row[Excel_policy_no]?.ToString().Trim().ToUpper();
                    string companyCd = row[Excel_Comp]?.ToString().Trim().ToUpper();

                    // Check if record exists
                    string checkSql = $@"SELECT COUNT(1) as count_value FROM BAJAJ_DUE_DATA 
                              WHERE UPPER(TRIM(POLICY_NO)) = :policyNo
                              AND UPPER(TRIM(COMPANY_CD)) = :companyCd
                              AND MON_NO = :month 
                              AND YEAR_NO = :year
                              AND IMPORTDATATYPE = :importType";

                    var parameters = new Dictionary<string, object>
                                        {
                                            { "policyNo", policyNo },
                                            { "companyCd", companyCd },
                                            { "month", month },
                                            { "year", year },
                                            { "importType", MyImportDataType }
                                        };

                    DataTable dt = pc.ExecuteParameterizedQuery(checkSql, parameters, out int rowCount, out string error);

                    // count_value rown valeu <= 0 means no record found
                    if (dt.Rows.Count > 0 && dt.Rows[0]["count_value"] != DBNull.Value)
                    {
                        rowCount = Convert.ToInt32(dt.Rows[0]["count_value"]);
                    }
                    else
                    {
                        rowCount = 0;
                    }
                    if (rowCount <= 0 && error == null)
                    {
                        insertRecords.Add(row);
                        if (MyImportDataType == "LAPSEDDATA")
                        {
                            lapsedPolicies.Add(Tuple.Create(policyNo, companyCd));
                        }
                    }
                    else
                    {
                        updateRecords.Add(row);
                    }
                }

                // Process inserts in bulk
                if (insertRecords.Count > 0)
                {
                    insertCount += BulkInsertDueData(insertRecords, month, year, MyImportDataType,
                                                  logginId, curDate, dataBaseField, mskDbFld, mskExFld);
                }

                // Process lapsed policies in bulk
                if (lapsedPolicies.Count > 0)
                {
                    ProcessLapsedPolicies(lapsedPolicies, logginId, month, year, currentDateFormatted,
                                        ddlImportDataType, ServerDateTime);
                }

                // Process updates in bulk
                if (updateRecords.Count > 0)
                {
                    updateCount += BulkUpdateDueData(updateRecords, month, year, MyImportDataType,
                                                   logginId, fileNameNe, ddlImportDataType,
                                                   ServerDateTime, currentDateFormatted);
                }

                processedRecords += endIndex - i;
                //UpdateProgress(processedRecords, totalRecords); // Optional progress reporting
                UpdateProgress1.Visible = true;  // Assuming you have an UpdateProgress control with ID="UpdateProgress1"
                //ShowProgress(processedRecords, totalRecords);
            }

            string returnMsg = $"Total {totalRecords} record(s) processed: {insertCount} inserted, {updateCount} updated.";
            pc.ShowAlert(this, returnMsg);
        }

        

        #region New DUE DATA BULK INSERTION
        private int BulkInsertDueData(List<DataRow> rows, int month, int year, string importDataType,
                                    string loginId, string currentDate, string dataBaseField,
                                    List<string> dbFields, List<string> excelFields)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.AppendLine("INSERT ALL");

            foreach (DataRow row in rows)
            {
                sqlBuilder.Append("INTO BAJAJ_DUE_DATA (");
                sqlBuilder.Append(dataBaseField);
                sqlBuilder.Append(", Mon_no, Year_No, UserId, Import_Dt, ImportDataType, NEWINSERT) VALUES (");

                for (int i = 0; i < dbFields.Count; i++)
                {
                    string fieldName = excelFields[i];
                    string fieldValue = row[fieldName]?.ToString().Trim();

                    if (string.IsNullOrEmpty(fieldValue))
                    {
                        sqlBuilder.Append("NULL,");
                        continue;
                    }

                    if (fieldValue.Contains("'"))
                    {
                        fieldValue = fieldValue.Replace("'", "''");
                    }

                    if (row.Table.Columns[fieldName].DataType == typeof(DateTime))
                    {
                        if (DateTime.TryParse(fieldValue, out DateTime dtValue))
                        {
                            sqlBuilder.Append($"TO_DATE('{dtValue.ToString("dd/MMM/yyyy")}','DD/MON/YYYY'),");
                        }
                        else
                        {
                            sqlBuilder.Append("NULL,");
                        }
                    }
                    else
                    {
                        if (fieldValue.Contains(","))
                        {
                            fieldValue = fieldValue.Replace(",", "");
                        }
                        sqlBuilder.Append($"'{fieldValue}',");
                    }
                }

                sqlBuilder.Append($"{month}, {year}, '{loginId}', TO_DATE('{currentDate}','DD/MM/YYYY'), ");
                sqlBuilder.Append($"'{importDataType}', 'BAL') ");
            }

            sqlBuilder.AppendLine("SELECT 1 FROM DUAL");
            string sql = sqlBuilder.ToString();

            //pc.ExecuteCurrentQueryMaster(sql, out int rowsAffected, out string error);
            //return rowsAffected;

            var executor = new BulkInsertExecutor();
            string errorMsg;
            bool success = executor.ExecuteBulkInsert(sql, out errorMsg);
            string returnMsg = $"Total {rows.Count} record(s) inserted.";
            if (!success)
            {
                // Handle error
                returnMsg = ($"Bulk insert failed: {errorMsg}");
            }
            else
            {
                returnMsg +=("Bulk insert completed successfully");
            }

            return rows.Count; // Return the number of inserted records
        }
        
        #endregion
         
        #region New DUE DATA UPDATE BULK

        private int BulkUpdateDueData(List<DataRow> rows, int month, int year, string importDataType, string loginId, string fileName, string importType,  DateTime serverDateTime, string currentDateFormatted)
        {
            if (importDataType != "DUEDATA") return 0;

            // Group updates by type
            var policyUpdates = new Dictionary<string, List<Tuple<string, string>>>
            {
                ["BASE"] = new List<Tuple<string, string>>(),
                ["PREM_FREQ"] = new List<Tuple<string, string>>(),
                // Other update types...
            };

            // Populate the lists (same as your current code)
            foreach (DataRow row in rows)
            {
                string policyNo = row[Excel_policy_no]?.ToString().Trim().ToUpper();
                string companyCd = row[Excel_Comp]?.ToString().Trim().ToUpper();

                policyUpdates["BASE"].Add(Tuple.Create(policyNo, companyCd));

                if (PsmController.IsContainAndNotNull(row, Excel_Payment_Mode))
                {
                    policyUpdates["PREM_FREQ"].Add(Tuple.Create(policyNo, companyCd));
                }
                // Other conditions...
            }

            int totalUpdates = 0;

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                OracleTransaction transaction = conn.BeginTransaction();

                try
                {
                    // 1. Process BASE updates
                    if (policyUpdates["BASE"].Count > 0)
                    {
                        // Load data to temp table
                        BulkInsertToTempTable(conn, transaction, "TEMP_POLICY_UPDATES_BASE", policyUpdates["BASE"]);

                        // Execute update
                        string sql = @"UPDATE POLICY_DETAILS_MASTER p
                              SET FILE_NAME = :fileName,
                                  PAYMENT_MODE = :paymentMode,
                                  UPDATE_PROG = :updateProg,
                                  UPDATE_USER = :updateUser,
                                  UPDATE_DATE = TO_DATE(:updateDate, 'DD-MON-YYYY')
                              WHERE EXISTS (
                                  SELECT 1 FROM TEMP_POLICY_UPDATES_BASE t
                                  WHERE p.POLICY_no = t.policy_no
                                  AND p.COMPANY_CD = t.company_cd
                              )";

                        totalUpdates += ExecuteUpdateWithParams(conn, transaction, sql,
                            new
                            {
                                fileName,
                                paymentMode = Excel_Payment_Mode,
                                updateProg = importType,
                                updateUser = loginId,
                                updateDate = currentDateFormatted
                            });

                        // Clear temp table
                        ClearTempTable(conn, transaction, "TEMP_POLICY_UPDATES_BASE");
                    }

                    // 2. Process PREM_FREQ updates (and other types similarly)
                    if (policyUpdates["PREM_FREQ"].Count > 0)
                    {
                        // Similar pattern as above
                    }

                    transaction.Commit();
                    return totalUpdates;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Log error
                    return 0;
                }
            }
        }

        // Helper methods
        private void BulkInsertToTempTable(OracleConnection conn, OracleTransaction transaction, string tableName, List<Tuple<string, string>> data)
        {
            // Clear table first
            ClearTempTable(conn, transaction, tableName);

            // Use array binding for efficient bulk insert
            using (var cmd = new OracleCommand(
                $"INSERT INTO {tableName} (policy_no, company_cd) VALUES (:1, :2)", conn))
            {
                cmd.Transaction = transaction;
                cmd.ArrayBindCount = data.Count;

                var policyNos = data.Select(p => p.Item1).ToArray();
                var companyCds = data.Select(p => p.Item2).ToArray();

                cmd.Parameters.Add(":1", OracleDbType.Varchar2, policyNos, ParameterDirection.Input);
                cmd.Parameters.Add(":2", OracleDbType.Varchar2, companyCds, ParameterDirection.Input);

                cmd.ExecuteNonQuery();
            }
        }

        private void ClearTempTable(OracleConnection conn, OracleTransaction transaction, string tableName)
        {
            using (var cmd = new OracleCommand($"DELETE FROM {tableName}", conn))
            {
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
            }
        }


        // showing error
        private int ExecuteUpdateWithParams(OracleConnection conn, OracleTransaction transaction,  string sql, dynamic parameters)
        {
            using (var cmd = new OracleCommand(sql, conn))
            {
                cmd.Transaction = transaction;

                // Add parameters dynamically
                var props = ((object)parameters).GetType().GetProperties();
                foreach (var prop in props)
                {
                    cmd.Parameters.Add(prop.Name, GetOracleDbType(prop.PropertyType)).Value = prop.GetValue(parameters);
                }

                return cmd.ExecuteNonQuery();
            }
        }

        private OracleDbType GetOracleDbType(Type type)
        {
            if (type == typeof(string)) return OracleDbType.Varchar2;
            // Add other type mappings as needed
            return OracleDbType.Varchar2;
        }


        #endregion

        #region NEW DUE DATA FOR LAPSED 

        private void ProcessLapsedPolicies(List<Tuple<string, string>> policies, string loginId, int month, int year, string currentDateFormatted,  string importType, DateTime serverDateTime)
        {
            if (policies.Count == 0) return;

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                OracleTransaction transaction = conn.BeginTransaction();

                try
                {
                    // 1. Load policies into temp table
                    BulkInsertToTempTable(conn, transaction, "TEMP_LAPSED_POLICIES", policies);

                    // 2. Get max due dates in single query
                    DataTable lapsedPolicies = GetLapsedPoliciesData(conn, transaction);

                    if (lapsedPolicies.Rows.Count == 0)
                    {
                        transaction.Commit();
                        return;
                    }

                    // 3. Process lapsed policies
                    ProcessLapsedUpdates(conn, transaction, lapsedPolicies, loginId,
                                        currentDateFormatted, importType, serverDateTime);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Log error
                    throw;
                }
            }
        }

        private DataTable GetLapsedPoliciesData(OracleConnection conn, OracleTransaction transaction)
        {
            string sql = @"SELECT b.policy_no, b.company_cd, b.due_date, b.status_cd
                   FROM (
                       SELECT policy_no, company_cd, MAX(due_date) as max_due_date
                       FROM bajaj_due_data 
                       WHERE importdatatype = 'DUEDATA'
                       AND (policy_no, company_cd) IN (
                           SELECT policy_no, company_cd FROM TEMP_LAPSED_POLICIES
                       )
                       GROUP BY policy_no, company_cd
                   ) max_dates
                   JOIN bajaj_due_data b ON b.policy_no = max_dates.policy_no 
                                       AND b.company_cd = max_dates.company_cd
                                       AND b.due_date = max_dates.max_due_date";

            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                cmd.Transaction = transaction;
                using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        private void ProcessLapsedUpdates(OracleConnection conn, OracleTransaction transaction,  DataTable lapsedPolicies, string loginId,  string currentDateFormatted, string importType, DateTime serverDateTime)
        {
            // 1. Prepare lapsed policy data
            var lapsedUpdates = new List<Tuple<string, string, DateTime>>();
            DateTime currentDate = DateTime.Now;

            foreach (DataRow dr in lapsedPolicies.Rows)
            {
                string policyNo = dr["policy_no"].ToString();
                string companyCd = dr["company_cd"].ToString();
                DateTime dueDate = Convert.ToDateTime(dr["due_date"]);

                if (currentDate >= dueDate)
                {
                    lapsedUpdates.Add(Tuple.Create(policyNo, companyCd, dueDate));
                }
            }

            if (lapsedUpdates.Count == 0) return;

            // 2. Update bajaj_due_data (status to LAPSED)
            string updateDueDataSql = @"UPDATE bajaj_due_data SET 
                              status_Cd = 'LAPSED',
                              last_update_dt = TO_DATE(:currentDate, 'DD/MM/YYYY'),
                              last_update = :loginId
                              WHERE (policy_no, company_cd, due_date) IN (
                                  SELECT policy_no, company_cd, due_date 
                                  FROM TEMP_LAPSED_UPDATES
                              ) AND importdatatype = 'DUEDATA'";

            // 3. Update policy_details_master
            string updatePolicyMasterSql = @"UPDATE policy_details_master SET 
                                   last_status = 'L',
                                   UPDATE_PROG = :importType,
                                   UPDATE_USER = :loginId,
                                   UPDATE_DATE = TO_DATE(:serverDate, 'DD-MON-YYYY')
                                   WHERE (policy_no, company_cd) IN (
                                       SELECT policy_no, company_cd 
                                       FROM TEMP_LAPSED_UPDATES
                                   )";

            // Load updates to temp table
            BulkInsertLapsedUpdates(conn, transaction, lapsedUpdates);

            // Execute updates with parameters
            ExecuteUpdateWithParams(conn, transaction, updateDueDataSql, new
            {
                currentDate = currentDateFormatted,
                loginId
            });

            ExecuteUpdateWithParams(conn, transaction, updatePolicyMasterSql, new
            {
                importType,
                loginId,
                serverDate = serverDateTime.ToString("dd-MMM-yyyy")
            });
        }

        private void BulkInsertLapsedUpdates(OracleConnection conn, OracleTransaction transaction,  List<Tuple<string, string, DateTime>> updates)
        {
            // Clear temp table first
            using (var cmd = new OracleCommand("TRUNCATE TABLE TEMP_LAPSED_UPDATES", conn))
            {
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
            }

            // Use array binding for bulk insert
            using (var cmd = new OracleCommand(
                "INSERT INTO TEMP_LAPSED_UPDATES (policy_no, company_cd, due_date) VALUES (:1, :2, :3)", conn))
            {
                cmd.Transaction = transaction;
                cmd.ArrayBindCount = updates.Count;

                var policyNos = updates.Select(u => u.Item1).ToArray();
                var companyCds = updates.Select(u => u.Item2).ToArray();
                var dueDates = updates.Select(u => u.Item3).ToArray();

                cmd.Parameters.Add(":1", OracleDbType.Varchar2, policyNos, ParameterDirection.Input);
                cmd.Parameters.Add(":2", OracleDbType.Varchar2, companyCds, ParameterDirection.Input);
                cmd.Parameters.Add(":3", OracleDbType.Date, dueDates, ParameterDirection.Input);

                cmd.ExecuteNonQuery();
            }
        }

        #endregion

        #endregion

        #region PROCCESS BULK UPDATING DATA FOR PAID DATA


        // PART 2:

        public void BulkPaidDataImport(DataTable excelData, string importDataType,
                             string loginId, int month, int year)
        {
            if (!chkOptPaid.Checked) return;

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                OracleTransaction transaction = conn.BeginTransaction();

                try
                {
                    // 1. Load all paid data into temp table
                    BulkLoadPaidData(conn, transaction, excelData, month, year, importDataType, loginId);

                    // 2. Process due records updates
                    ProcessDueRecords(conn, transaction, loginId, month, year, importDataType);

                    // 3. Process policy master updates
                    ProcessPolicyMasterUpdates(conn, transaction, loginId, importDataType);

                    // 4. Insert paid records
                    InsertPaidRecords(conn, transaction, month, year, importDataType, loginId);

                    // 5. Handle duplicate policies
                    ProcessDuplicatePolicies(conn, transaction, month, year, importDataType);

                    transaction.Commit();
                    pc.ShowAlert(this, "Records Imported Successfully");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    pc.ShowAlert(this, $"Error during bulk import: {ex.Message}");
                }
            }
        }

        private void BulkLoadPaidData(OracleConnection conn, OracleTransaction transaction,
                                    DataTable excelData, int month, int year,
                                    string importDataType, string loginId)
        {
            // Clear temp table first
            using (var cmd = new OracleCommand("TRUNCATE TABLE TEMP_PAID_IMPORT", conn))
            {
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
            }

            // Prepare data for bulk insert
            var policies = new List<PaidPolicy>();
            foreach (DataRow row in excelData.Rows)
            {
                if (!PsmController.IsContainAndNotNull(row, Excel_Comp)) continue;

                policies.Add(new PaidPolicy
                {
                    PolicyNo = row[Excel_policy_no]?.ToString().Trim().ToUpper(),
                    CompanyCd = row[Excel_Comp]?.ToString().Trim().ToUpper(),
                    StatusCd = row[Excel_Status]?.ToString().Trim().ToUpper(),
                    MonNo = month,
                    YearNo = year,
                    ImportDataType = importDataType,
                    UserId = loginId,
                    ImportDt = DateTime.Now
                });
            }

            // Bulk insert using array binding
            using (var cmd = new OracleCommand(
                "INSERT INTO TEMP_PAID_IMPORT (policy_no, company_cd, status_cd, " +
                "mon_no, year_no, importdatatype, user_id, import_dt) " +
                "VALUES (:1, :2, :3, :4, :5, :6, :7, :8)", conn))
            {
                cmd.Transaction = transaction;
                cmd.ArrayBindCount = policies.Count;

                cmd.Parameters.Add(":1", OracleDbType.Varchar2,
                                 policies.Select(p => p.PolicyNo).ToArray(),
                                 ParameterDirection.Input);
                cmd.Parameters.Add(":2", OracleDbType.Varchar2,
                                 policies.Select(p => p.CompanyCd).ToArray(),
                                 ParameterDirection.Input);
                cmd.Parameters.Add(":3", OracleDbType.Varchar2,
                                 policies.Select(p => p.StatusCd).ToArray(),
                                 ParameterDirection.Input);
                cmd.Parameters.Add(":4", OracleDbType.Int32,
                                 policies.Select(p => p.MonNo).ToArray(),
                                 ParameterDirection.Input);
                cmd.Parameters.Add(":5", OracleDbType.Int32,
                                 policies.Select(p => p.YearNo).ToArray(),
                                 ParameterDirection.Input);
                cmd.Parameters.Add(":6", OracleDbType.Varchar2,
                                 policies.Select(p => p.ImportDataType).ToArray(),
                                 ParameterDirection.Input);
                cmd.Parameters.Add(":7", OracleDbType.Varchar2,
                                 policies.Select(p => p.UserId).ToArray(),
                                 ParameterDirection.Input);
                cmd.Parameters.Add(":8", OracleDbType.Date,
                                 policies.Select(p => p.ImportDt).ToArray(),
                                 ParameterDirection.Input);

                cmd.ExecuteNonQuery();
            }
        }

        private void ProcessDueRecords(OracleConnection conn, OracleTransaction transaction,
                                     string loginId, int month, int year, string importDataType)
        {
            // 1. Insert new due records for missing policies
            string insertDueSql = @"
    INSERT INTO BAJAJ_DUE_DATA 
    (COMPANY_CD, POLICY_NO, STATUS_CD, 
     Mon_no, Year_No, UserId, Import_Dt, ImportDataType, NEWINSERT, FORCE_FLAG)
    SELECT t.company_cd, t.policy_no, t.status_cd,
           t.mon_no, t.year_no, t.user_id, t.import_dt, t.importdatatype, NULL, 'FORCE FULL'
    FROM TEMP_PAID_IMPORT t
    WHERE NOT EXISTS (
        SELECT 1 FROM BAJAJ_DUE_DATA d
        WHERE d.policy_no = t.policy_no
        AND d.company_cd = t.company_cd
        AND d.mon_no = t.mon_no
        AND d.year_no = t.year_no
        AND d.importdatatype = t.importdatatype
    )";

            using (var cmd = new OracleCommand(insertDueSql, conn))
            {
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
            }

            // 2. Update existing due records status
            string updateDueSql = @"
UPDATE BAJAJ_DUE_DATA d
SET (status_Cd, last_update_dt, last_update) = 
    (SELECT t.status_cd, SYSDATE, :loginId
     FROM TEMP_PAID_IMPORT t
     WHERE d.policy_no = t.policy_no
     AND d.company_cd = t.company_cd
     AND d.mon_no = t.mon_no
     AND d.year_no = t.year_no
     AND d.importdatatype = t.importdatatype)
WHERE EXISTS (
    SELECT 1
    FROM TEMP_PAID_IMPORT t
    WHERE d.policy_no = t.policy_no
    AND d.company_cd = t.company_cd
    AND d.mon_no = t.mon_no
    AND d.year_no = t.year_no
    AND d.importdatatype = t.importdatatype)";

            using (var cmd = new OracleCommand(updateDueSql, conn))
            {
                cmd.Transaction = transaction;
                cmd.Parameters.Add("loginId", OracleDbType.Varchar2).Value = loginId;
                cmd.ExecuteNonQuery();
            }
        }

        private void ProcessPolicyMasterUpdates(OracleConnection conn, OracleTransaction transaction,
                                              string loginId, string importType)
        {
            // Clear temp table
            using (var cmd = new OracleCommand("TRUNCATE TABLE TEMP_POLICY_UPDATES", conn))
            {
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
            }

            // Identify policies that need status updates
            string identifyUpdatesSql = @"
    INSERT INTO TEMP_POLICY_UPDATES (policy_no, company_cd, new_status)
    SELECT DISTINCT t.policy_no, t.company_cd, 
           CASE WHEN h.last_status = 'L' AND t.status_cd = 'PAID' THEN 'R' ELSE 'A' END
    FROM TEMP_PAID_IMPORT t
    LEFT JOIN POLICY_DETAILS_MASTER h ON h.policy_no = t.policy_no AND h.company_cd = t.company_cd
    WHERE t.status_cd = 'PAID'";

            using (var cmd = new OracleCommand(identifyUpdatesSql, conn))
            {
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
            }

            string mergeSql = @"
MERGE INTO POLICY_DETAILS_MASTER p
USING TEMP_POLICY_UPDATES u
ON (p.policy_no = u.policy_no AND p.company_cd = u.company_cd)
WHEN MATCHED THEN
    UPDATE SET 
        p.last_status = u.new_status,
        p.UPDATE_PROG = :importType,
        p.UPDATE_USER = :loginId,
        p.UPDATE_DATE = SYSDATE";

            using (var cmd = new OracleCommand(mergeSql, conn))
            {
                cmd.Transaction = transaction;
                cmd.Parameters.Add("importType", OracleDbType.Varchar2).Value = importType;
                cmd.Parameters.Add("loginId", OracleDbType.Varchar2).Value = loginId;
                cmd.ExecuteNonQuery();
            }
        }

        private void InsertPaidRecords(OracleConnection conn, OracleTransaction transaction,
                                     int month, int year, string importDataType, string loginId)
        {
            // Insert new paid records
            string insertPaidSql = @"
    INSERT INTO BAJAJ_PAID_DATA 
    (COMPANY_CD, POLICY_NO, STATUS_CD, 
     Mon_no, Year_No, UserId, Import_Dt, ImportDataType)
    SELECT t.company_cd, t.policy_no, t.status_cd,
           t.mon_no, t.year_no, t.user_id, t.import_dt, t.importdatatype
    FROM TEMP_PAID_IMPORT t
    WHERE NOT EXISTS (
        SELECT 1 FROM BAJAJ_PAID_DATA p
        WHERE p.policy_no = t.policy_no
        AND p.company_cd = t.company_cd
        AND p.mon_no = t.mon_no
        AND p.year_no = t.year_no
        AND p.importdatatype = t.importdatatype
    )";

            using (var cmd = new OracleCommand(insertPaidSql, conn))
            {
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
            }

            // Update existing paid records
            string mergeSql = @"
MERGE INTO BAJAJ_PAID_DATA p
USING TEMP_PAID_IMPORT t
ON (
    p.policy_no = t.policy_no
    AND p.company_cd = t.company_cd
    AND p.mon_no = t.mon_no
    AND p.year_no = t.year_no
    AND p.importdatatype = t.importdatatype
)
WHEN MATCHED THEN
    UPDATE SET
        p.STATUS_CD = t.status_cd,
        p.LAST_UPDATE_DT = SYSDATE,
        p.LAST_UPDATE = :loginId";

            using (var cmd = new OracleCommand(mergeSql, conn))
            {
                cmd.Transaction = transaction;
                cmd.Parameters.Add("loginId", OracleDbType.Varchar2).Value = loginId;
                cmd.ExecuteNonQuery();
            }
        }

        private void ProcessDuplicatePolicies(OracleConnection conn, OracleTransaction transaction,
                                            int month, int year, string importDataType)
        {
            // Mark duplicate policies
            string dupPolicySql = @"
UPDATE BAJAJ_PAID_DATA p
SET dup_rec = 'Y'
WHERE EXISTS (
    SELECT 1 FROM (
        SELECT a.policy_no, a.company_cd 
        FROM BAJAJ_AR_HEAD a 
        JOIN BAJAJ_PAID_DATA b ON a.company_cd = b.company_cd AND a.POLICY_NO = b.POLICY_NO
        WHERE b.mon_no = :monthInner AND b.year_no = :yearInner AND b.importdatatype = :importTypeInner
        GROUP BY a.policy_no, a.company_cd
        HAVING COUNT(a.policy_no) > 1
    ) dup
    WHERE p.policy_no = dup.policy_no
    AND p.company_cd = dup.company_cd
    AND p.mon_no = :monthOuter
    AND p.year_no = :yearOuter
    AND p.importdatatype = :importTypeOuter
)";

            using (var cmd = new OracleCommand(dupPolicySql, conn))
            {
                cmd.Transaction = transaction;
                cmd.Parameters.Add("monthInner", OracleDbType.Int32).Value = month;
                cmd.Parameters.Add("yearInner", OracleDbType.Int32).Value = year;
                cmd.Parameters.Add("importTypeInner", OracleDbType.Varchar2).Value = importDataType;
                cmd.Parameters.Add("monthOuter", OracleDbType.Int32).Value = month;
                cmd.Parameters.Add("yearOuter", OracleDbType.Int32).Value = year;
                cmd.Parameters.Add("importTypeOuter", OracleDbType.Varchar2).Value = importDataType;
                cmd.ExecuteNonQuery();
            }
        }




        // PART 3: HELPR FUNCTIONS AND CLASSES
        private class PaidPolicy
        {
            public string PolicyNo { get; set; }
            public string CompanyCd { get; set; }
            public string StatusCd { get; set; }
            public int MonNo { get; set; }
            public int YearNo { get; set; }
            public string ImportDataType { get; set; }
            public string UserId { get; set; }
            public DateTime ImportDt { get; set; }
        }




        #endregion

