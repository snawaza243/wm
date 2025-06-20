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
            BulkLoadPaidData(conn, transaction, excelData, month, year, importDataType);

            // 2. Process due records updates
            ProcessDueRecords(conn, transaction, loginId, month, year, importDataType);

            // 3. Process policy master updates
            ProcessPolicyMasterUpdates(conn, transaction, loginId);

            // 4. Handle duplicate policies
            ProcessDuplicatePolicies(conn, transaction, month, year, importDataType);

            // 5. Update employee/investor codes
            UpdateEmployeeInvestorCodes(conn, transaction, month, year, importDataType);

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
                            DataTable excelData, int month, int year, string importDataType)
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
            PaymentDate = ParseDate(row[Excel_Payment_Date]),
            Amount = ParseDecimal(row[Excel_Amount]),
            ReceiptNo = row[Excel_Receipt_No]?.ToString().Trim(),
            MonNo = month,
            YearNo = year,
            ImportDataType = importDataType
        });
    }

    // Bulk insert using array binding
    using (var cmd = new OracleCommand(
        "INSERT INTO TEMP_PAID_IMPORT (policy_no, company_cd, status_cd, payment_date, " +
        "amount, receipt_no, mon_no, year_no, importdatatype) " +
        "VALUES (:1, :2, :3, :4, :5, :6, :7, :8, :9)", conn))
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
        cmd.Parameters.Add(":4", OracleDbType.Date, 
                         policies.Select(p => p.PaymentDate).ToArray(), 
                         ParameterDirection.Input);
        cmd.Parameters.Add(":5", OracleDbType.Decimal, 
                         policies.Select(p => p.Amount).ToArray(), 
                         ParameterDirection.Input);
        cmd.Parameters.Add(":6", OracleDbType.Varchar2, 
                         policies.Select(p => p.ReceiptNo).ToArray(), 
                         ParameterDirection.Input);
        cmd.Parameters.Add(":7", OracleDbType.Int32, 
                         policies.Select(p => p.MonNo).ToArray(), 
                         ParameterDirection.Input);
        cmd.Parameters.Add(":8", OracleDbType.Int32, 
                         policies.Select(p => p.YearNo).ToArray(), 
                         ParameterDirection.Input);
        cmd.Parameters.Add(":9", OracleDbType.Varchar2, 
                         policies.Select(p => p.ImportDataType).ToArray(), 
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
    (COMPANY_CD, DOC, FREQ_UPD, DUE_DATE, POLICY_NO, STATUS_CD, 
     Mon_no, Year_No, UserId, Import_Dt, ImportDataType, NEWINSERT, FORCE_FLAG)
    SELECT t.company_cd, t.payment_date, 'Manual', t.payment_date, t.policy_no, t.status_cd,
           t.mon_no, t.year_no, :loginId, SYSDATE, t.importdatatype, NULL, 'FORCE FULL'
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
        cmd.Parameters.Add("loginId", OracleDbType.Varchar2).Value = loginId;
        cmd.ExecuteNonQuery();
    }

    // 2. Update existing due records to PAID status
    string updateDueSql = @"
    UPDATE BAJAJ_DUE_DATA d
    SET status_Cd = t.status_cd,
        last_update_dt = SYSDATE,
        last_update = :loginId,
        payment_date = t.payment_date,
        receipt_no = t.receipt_no
    FROM TEMP_PAID_IMPORT t
    WHERE d.policy_no = t.policy_no
    AND d.company_cd = t.company_cd
    AND d.mon_no = t.mon_no
    AND d.year_no = t.year_no
    AND d.importdatatype = t.importdatatype";
    
    using (var cmd = new OracleCommand(updateDueSql, conn))
    {
        cmd.Transaction = transaction;
        cmd.Parameters.Add("loginId", OracleDbType.Varchar2).Value = loginId;
        cmd.ExecuteNonQuery();
    }
}

private void ProcessPolicyMasterUpdates(OracleConnection conn, OracleTransaction transaction,
                                      string loginId)
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

    // Update policy master records
    string updatePolicySql = @"
    UPDATE POLICY_DETAILS_MASTER p
    SET last_status = u.new_status,
        UPDATE_PROG = 'PAID_IMPORT',
        UPDATE_USER = :loginId,
        UPDATE_DATE = SYSDATE
    FROM TEMP_POLICY_UPDATES u
    WHERE p.policy_no = u.policy_no
    AND p.company_cd = u.company_cd";
    
    using (var cmd = new OracleCommand(updatePolicySql, conn))
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
            FROM bajaj_Ar_head a 
            JOIN bajaj_paid_Data b ON a.company_cd = b.company_cd AND a.Policy_No = b.Policy_No
            WHERE b.mon_no = :month AND b.year_no = :year AND b.importdatatype = :importType
            GROUP BY a.policy_no, a.company_cd
            HAVING COUNT(a.policy_no) > 1
        ) dup
        WHERE p.policy_no = dup.policy_no
        AND p.company_cd = dup.company_cd
        AND p.mon_no = :month
        AND p.year_no = :year
        AND p.importdatatype = :importType";
    
    using (var cmd = new OracleCommand(dupPolicySql, conn))
    {
        cmd.Transaction = transaction;
        cmd.Parameters.Add("month", OracleDbType.Int32).Value = month;
        cmd.Parameters.Add("year", OracleDbType.Int32).Value = year;
        cmd.Parameters.Add("importType", OracleDbType.Varchar2).Value = importDataType;
        cmd.ExecuteNonQuery();
    }
}

private void UpdateEmployeeInvestorCodes(OracleConnection conn, OracleTransaction transaction,
                                       int month, int year, string importDataType)
{
    string updateCodesSql = @"
    UPDATE bajaj_paid_data p
    SET emp_no = (
            SELECT MAX(h.emp_no) 
            FROM bajaj_ar_head h
            WHERE h.company_cd = p.company_cd 
            AND h.policy_no = p.policy_no 
            AND h.FRESH_RENEWAL = 1
        ),
        inv_cd = (
            SELECT MAX(h.client_Cd) 
            FROM bajaj_ar_head h
            WHERE h.company_cd = p.company_cd 
            AND h.policy_no = p.policy_no 
            AND h.FRESH_RENEWAL = 1
        )
    WHERE p.mon_no = :month 
    AND p.year_no = :year 
    AND p.dup_Rec IS NULL 
    AND p.importdatatype = :importType";
    
    using (var cmd = new OracleCommand(updateCodesSql, conn))
    {
        cmd.Transaction = transaction;
        cmd.Parameters.Add("month", OracleDbType.Int32).Value = month;
        cmd.Parameters.Add("year", OracleDbType.Int32).Value = year;
        cmd.Parameters.Add("importType", OracleDbType.Varchar2).Value = importDataType;
        cmd.ExecuteNonQuery();
    }
}




private class PaidPolicy
{
    public string PolicyNo { get; set; }
    public string CompanyCd { get; set; }
    public string StatusCd { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string ReceiptNo { get; set; }
    public int MonNo { get; set; }
    public int YearNo { get; set; }
    public string ImportDataType { get; set; }
}

private DateTime ParseDate(object value)
{
    if (value == null || value == DBNull.Value) return DateTime.MinValue;
    DateTime result;
    return DateTime.TryParse(value.ToString(), out result) ? result : DateTime.MinValue;
}

private decimal ParseDecimal(object value)
{
    if (value == null || value == DBNull.Value) return 0;
    decimal result;
    return decimal.TryParse(value.ToString(), out result) ? result : 0;
}

