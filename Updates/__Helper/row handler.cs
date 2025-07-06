#region All print letter fields value
string arBranchCode   = GetFieldValue("ar_branch_cd", "N/A");
string companyCode    = GetFieldValue("company_cd", "N/A");
string clientName     = GetFieldValue("client_name", "N/A");
string address1       = GetFieldValue("address1", "N/A");
string address2       = GetFieldValue("address2", "N/A");
string clAdd3         = GetFieldValue("cl_add3", "N/A");
string clAdd4         = GetFieldValue("cl_add4", "N/A");
string clAdd5         = GetFieldValue("cl_add5", "N/A");
string phone1         = GetFieldValue("cl_phone1", "N/A");
string phone2         = GetFieldValue("cl_phone2", "N/A");
string cityName       = GetFieldValue("city_name", "N/A");
string dueDateBase    = GetFieldValue("due_date", "01/01/1900");
string remFlag        = GetFieldValue("rem_flage", "0");
string stateName      = GetFieldValue("State_Name", "N/A");
string companyName    = GetFieldValue("Company_Name", "N/A");
string favourName     = GetFieldValue("Favour_Name", "N/A");
string branchName     = GetFieldValue("Branch_Name", "N/A");
string branchAdd1     = GetFieldValue("Branch_Add1", "N/A");
string branchAdd2     = GetFieldValue("Branch_Add2", "N/A");
string planName1      = GetFieldValue("Plan_Name1", "N/A");
string payMode        = GetFieldValue("Pay_Mode", "N/A");
string policyNo       = GetFieldValue("Policy_No", "N/A");
string pName          = GetFieldValue("P_Name", "N/A");
string iName          = GetFieldValue("I_Name", "N/A");
string premFreq       = GetFieldValue("prem_freq", "N/A");
string bPremFreq      = GetFieldValue("bprem_freq", "N/A");
string planName       = GetFieldValue("plan_name", "N/A");
string sa             = GetFieldValue("sa", "0");
string premAmt        = GetFieldValue("prem_amt", "0");
string monNo          = GetFieldValue("mon_no", "0");
string yearNo         = GetFieldValue("year_no", "0");
string clPin          = GetFieldValue("cl_pin", "N/A");
string pin1           = GetFieldValue("pin1", "N/A");
string invCode        = GetFieldValue("inv_code", "N/A");
string invCode1       = GetFieldValue("inv_code1", "N/A");
string importDataType = GetFieldValue("importdatatype", "N/A");
string dueDate        = GetDateFromDateTimeString(dueDateBase);
#endregion

// Helper Method to Simplify Value Retrieval
private string GetFieldValue(string fieldName, string defaultValue)
{
    return row[fieldName]?.ToString().Trim() ?? defaultValue;
}
