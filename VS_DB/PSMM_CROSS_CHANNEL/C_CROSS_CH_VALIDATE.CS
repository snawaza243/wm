public (bool Success, int Count) PSMM_CROSS_CH_VALIDATE(string frmType, string commonId, string subClientCd)
{
    string loginId = currentLoginID();

    using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
    {
        using (OracleCommand cmd = new OracleCommand("WEALTHMAKER.PSMM_CROSS_CH_VALIDATE", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;

            // Input parameters
            cmd.Parameters.Add("P_FRM_TYPE", OracleDbType.Varchar2).Value = frmType ?? (object)DBNull.Value;
            cmd.Parameters.Add("P_COMMON_ID", OracleDbType.Varchar2).Value = commonId ?? (object)DBNull.Value;
            cmd.Parameters.Add("P_SUB_CLIENT_CD", OracleDbType.Varchar2).Value = subClientCd ?? (object)DBNull.Value;
            cmd.Parameters.Add("P_LOGIN_ID", OracleDbType.Varchar2).Value = loginId ?? (object)DBNull.Value;

            // Output parameter
            var outCnt = new OracleParameter("P_CNT", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outCnt);

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();

                int count = outCnt.Value != DBNull.Value ? Convert.ToInt32(outCnt.Value) : 0;
                bool success = count > 0;
                return (success, count);
            }
            catch (OracleException ex)
            {
                // Log or handle error as needed
                return (false, -1);
            }
            catch (Exception ex)
            {
                // Log or handle error as needed
                return (false, -1);
            }
        }
    }
}