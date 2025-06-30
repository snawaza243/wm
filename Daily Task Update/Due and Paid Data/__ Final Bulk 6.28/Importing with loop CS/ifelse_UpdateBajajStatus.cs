public void UpdateBajajStatus(string plyNo, string compCd, string statusCd, string importType, string cmbMonth, string txtYear)
{
    try
    {
        string sysArNo = "";
        string sysArDt = "";

        // Query to check if the record exists in BAJAJ_AR_HEAD
        string queryHead = $@"
         SELECT sys_ar_no, TO_CHAR(sys_ar_dt, 'DD-MMM-YYYY') AS sys_ar_dt
         FROM BAJAJ_AR_HEAD 
         WHERE POLICY_NO = '{plyNo}' 
         AND COMPANY_CD = '{compCd}' 
         AND TO_CHAR(SYS_AR_DT, 'MON-YYYY') = '{cmbMonth.ToUpper()}-{txtYear}'
         AND STATUS_CD = '{statusCd}'";

        DataTable dtHead = pc.ExecuteCurrentQuery(queryHead);

        if (dtHead.Rows.Count > 0)
        {
            sysArNo = dtHead.Rows[0]["sys_ar_no"].ToString();
            sysArDt = dtHead.Rows[0]["sys_ar_dt"].ToString();
        }
        else
        {
            return; // No record found, exit function
        }

        // Check if record exists in BAJAJ_AR_DETAILS
        string queryDetails = $@"
         SELECT 1 FROM BAJAJ_AR_DETAILS 
         WHERE SYS_AR_NO = '{sysArNo}' 
         AND STATUS_DT = LAST_DAY(TO_DATE('{sysArDt}', 'DD-MMM-YYYY'))";

        DataTable dtDetails = pc.ExecuteCurrentQuery(queryDetails);
        if (dtDetails.Rows.Count > 0) return; // Record already exists, exit function

        // Update BAJAJ_AR_HEAD
        string updateQuery = $@"
         UPDATE BAJAJ_AR_HEAD 
         SET STATUS_CD = '{statusCd}' 
         WHERE SYS_AR_NO = '{sysArNo}'";

        pc.ExecuteCurrentQuery(updateQuery);

        // Insert into BAJAJ_AR_DETAILS
        string insertQuery = $@"
         INSERT INTO BAJAJ_AR_DETAILS 
         (SYS_AR_NO, STATUS_DT, STATUS_CD, REMARKS, SYS_AR_DT, STATUS_UPDATE_ON) 
         VALUES 
         ('{sysArNo}', LAST_DAY(TO_DATE('{sysArDt}', 'DD-MMM-YYYY')), '{statusCd}', '{importType} ' || SYSDATE, TO_DATE('{sysArDt}', 'DD-MMM-YYYY'), SYSDATE)";

        pc.ExecuteCurrentQuery(insertQuery);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error while updating status: " + ex.Message);
    }
}

