private void UpdateBusinessCodeDetails(string bc)
{
    if (!string.IsNullOrWhiteSpace(bc) && bc != "95829" && bc != "103914")
    {
        using (var con = new OracleConnection(pc.ConnectionString))
        using (var cmd = new OracleCommand("GET_BUSINESS_CODE_DETAILS", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("P_PAYROLL_ID", OracleDbType.Varchar2).Value = bc;
            cmd.Parameters.Add("P_BRANCH_LIST", OracleDbType.Varchar2).Value = pc.LogBranches();
            cmd.Parameters.Add("P_SOURCE", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("P_BRANCH_NAME", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("P_RM_NAME", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("P_RM_CODE", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("P_RESULT_FLAG", OracleDbType.Int32).Direction = ParameterDirection.Output;

            con.Open();
            cmd.ExecuteNonQuery();

            int result = Convert.ToInt32(cmd.Parameters["P_RESULT_FLAG"].Value.ToString());

            if (result == 1)
            {
                txtBusinessCodeBranch.Text = cmd.Parameters["P_BRANCH_NAME"].Value.ToString();
                txtBusinessCodeName.Text = cmd.Parameters["P_RM_NAME"].Value.ToString();
            }
            else if (result == 2)
            {
                pc.ShowAlert(this, "RM should be of Sale Support OR Sales");
                ClearFieldsExceptBusinessCode(bc);
            }
            else
            {
                pc.ShowAlert(this, "RM should be Active");
                ClearFieldsExceptBusinessCode(bc);
            }
        }
    }
    else
    {
        if (bc == "95829")
        {
            txtBusinessCode.Text = "";
        }
    }
}

private void ClearFieldsExceptBusinessCode(string bc)
{
    txtBusinessCodeBranch.Text = "";
    txtBusinessCodeName.Text = "";
    if (bc != "95829")
    {
        txtBusinessCode.Text = "";
    }
}
