public DataTable PSMM_INV_SEARCH_TREE_FIND(
    string pxBranch, string pxCat, string pxCity, string pxCode, 
    string pxName, string pxAdd1, string pxAdd2, string pxPhone, 
    string pxPan, string pxMobile, string pxNewRm, string pxAhCode,
    string pxClientSubName, string pxClientBroker, string pxStrForm,
    string pxCurrentForm, string pxRm, string pxOldRm, string pxSort)
{
    string loginId = currentLoginID();
    string roleId = currentRoleID();
    var dt = new DataTable();

    using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
    {
        using (OracleCommand cmd = new OracleCommand("WEALTHMAKER.PSMM_INV_SEARCH_TREE_FIND", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;

            // Input parameters
            cmd.Parameters.Add("PX_BRANCH", OracleDbType.Varchar2).Value = pxBranch ?? (object)DBNull.Value;
            cmd.Parameters.Add("PX_CAT", OracleDbType.Varchar2).Value = pxCat ?? (object)DBNull.Value;
            cmd.Parameters.Add("PX_CITY", OracleDbType.Varchar2).Value = pxCity ?? (object)DBNull.Value;
            cmd.Parameters.Add("PX_CODE", OracleDbType.Varchar2).Value = pxCode ?? (object)DBNull.Value;
            cmd.Parameters.Add("PX_NAME", OracleDbType.Varchar2).Value = pxName ?? (object)DBNull.Value;
            cmd.Parameters.Add("PX_ADD1", OracleDbType.Varchar2).Value = pxAdd1 ?? (object)DBNull.Value;
            cmd.Parameters.Add("PX_ADD2", OracleDbType.Varchar2).Value = pxAdd2 ?? (object)DBNull.Value;
            cmd.Parameters.Add("PX_PHONE", OracleDbType.Varchar2).Value = pxPhone ?? (object)DBNull.Value;
            cmd.Parameters.Add("PX_PAN", OracleDbType.Varchar2).Value = pxPan ?? (object)DBNull.Value;
            cmd.Parameters.Add("PX_MOBILE", OracleDbType.Varchar2).Value = pxMobile ?? (object)DBNull.Value;
            cmd.Parameters.Add("PX_NEW_RM", OracleDbType.Varchar2).Value = pxNewRm ?? (object)DBNull.Value;
            cmd.Parameters.Add("PX_AH_CODE", OracleDbType.Varchar2).Value = pxAhCode ?? (object)DBNull.Value;
            cmd.Parameters.Add("PX_CLIENT_SUB_NAME", OracleDbType.Varchar2).Value = pxClientSubName ?? (object)DBNull.Value;
            cmd.Parameters.Add("PX_CLIENT_BROKER", OracleDbType.Varchar2).Value = pxClientBroker ?? (object)DBNull.Value;
            cmd.Parameters.Add("PX_LOG_ID", OracleDbType.Varchar2).Value = loginId ?? (object)DBNull.Value;
            cmd.Parameters.Add("PX_ROLE_ID", OracleDbType.Varchar2).Value = roleId ?? (object)DBNull.Value;
            cmd.Parameters.Add("PX_STR_FORM", OracleDbType.Varchar2).Value = pxStrForm ?? (object)DBNull.Value;
            cmd.Parameters.Add("PX_CURRENT_FORM", OracleDbType.Varchar2).Value = pxCurrentForm ?? (object)DBNull.Value;
            cmd.Parameters.Add("PX_RM", OracleDbType.Varchar2).Value = pxRm ?? (object)DBNull.Value;
            cmd.Parameters.Add("PX_OLD_RM", OracleDbType.Varchar2).Value = pxOldRm ?? (object)DBNull.Value;
            cmd.Parameters.Add("PX_SORT", OracleDbType.Varchar2).Value = pxSort ?? (object)DBNull.Value;

            // Output cursor
            var outCursor = new OracleParameter("PX_CURSOR", OracleDbType.RefCursor)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outCursor);

            try
            {
                conn.Open();

                using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                return dt;
            }
            catch (OracleException ex)
            {
                // Log Oracle-specific errors
                // Logger.Error("Oracle error in PSMM_INV_SEARCH_TREE_FIND", ex);
                return null;
            }
            catch (Exception ex)
            {
                // Log general errors
                // Logger.Error("General error in PSMM_INV_SEARCH_TREE_FIND", ex);
                return null;
            }
        }
    }
}