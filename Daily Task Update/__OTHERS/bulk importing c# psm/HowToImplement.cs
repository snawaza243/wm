// Your large INSERT ALL statement as a string
string bulkInsertSql = @"INSERT ALL
    INTO BAJAJ_DUE_DATA (CL_ADD1,CL_ADD2,CL_ADD3,CL_ADD4,CL_ADD5,CL_CITY,CL_NAME,COMPANY_CD,PAY_MODE,POLICY_NO,PREM_FREQ, Mon_no, Year_No, UserId, Import_Dt, ImportDataType, NEWINSERT) 
    VALUES (NULL,NULL,NULL,NULL,NULL,NULL,NULL,'BAJAJ A','Non ECS','558452792','1',7, 2040, '121397', TO_DATE('20-Jun-2025','DD/MM/YYYY'), 'DUEDATA', 'BAL')
    -- ... all your other rows ...
    SELECT 1 FROM DUAL";

// Execute the bulk insert
var executor = new BulkInsertExecutor();
string errorMsg;
bool success = executor.ExecuteBulkInsert(bulkInsertSql, out errorMsg);

if (!success)
{
    // Handle error
    Console.WriteLine($"Bulk insert failed: {errorMsg}");
}
else
{
    Console.WriteLine("Bulk insert completed successfully");
}