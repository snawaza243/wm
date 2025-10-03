public DataTable PSMM_INV_SEARCH_TREE_FIND(
    string pxBranch, string pxCat, string pxCity, string pxCode, 
    string pxName, string pxAdd1, string pxAdd2, string pxPhone, 
    string pxPan, string pxMobile, string pxNewRm, string pxAhCode,
    string pxClientSubName, string pxClientBroker, string pxStrForm,
    string pxCurrentForm, string pxRm, string pxOldRm, string pxSort)
{
    try
    {
        string loginId = currentLoginID();
        string roleId = currentRoleID();
        var dt = new DataTable();

        using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
        {
            using (OracleCommand cmd = new OracleCommand("WEALTHMAKER.PSMM_INV_SEARCH_TREE_FIND", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300; // 5 minutes timeout

                // Add all input parameters
                AddParameter(cmd, "PX_BRANCH", pxBranch);
                AddParameter(cmd, "PX_CAT", pxCat);
                AddParameter(cmd, "PX_CITY", pxCity);
                AddParameter(cmd, "PX_CODE", pxCode);
                AddParameter(cmd, "PX_NAME", pxName);
                AddParameter(cmd, "PX_ADD1", pxAdd1);
                AddParameter(cmd, "PX_ADD2", pxAdd2);
                AddParameter(cmd, "PX_PHONE", pxPhone);
                AddParameter(cmd, "PX_PAN", pxPan);
                AddParameter(cmd, "PX_MOBILE", pxMobile);
                AddParameter(cmd, "PX_NEW_RM", pxNewRm);
                AddParameter(cmd, "PX_AH_CODE", pxAhCode);
                AddParameter(cmd, "PX_CLIENT_SUB_NAME", pxClientSubName);
                AddParameter(cmd, "PX_CLIENT_BROKER", pxClientBroker);
                AddParameter(cmd, "PX_LOG_ID", loginId);
                AddParameter(cmd, "PX_ROLE_ID", roleId);
                AddParameter(cmd, "PX_STR_FORM", pxStrForm);
                AddParameter(cmd, "PX_CURRENT_FORM", pxCurrentForm);
                AddParameter(cmd, "PX_RM", pxRm);
                AddParameter(cmd, "PX_OLD_RM", pxOldRm);
                AddParameter(cmd, "PX_SORT", pxSort);

                // Output cursor
                var outCursor = new OracleParameter("PX_CURSOR", OracleDbType.RefCursor)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outCursor);

                conn.Open();

                using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                return dt;
            }
        }
    }
    catch (OracleException ex)
    {
        // Log Oracle-specific errors with details
        Logger.Error($"Oracle error in PSMM_INV_SEARCH_TREE_FIND: {ex.Message}", ex);
        throw new ApplicationException("Database error occurred while searching records.", ex);
    }
    catch (Exception ex)
    {
        // Log general errors
        Logger.Error($"General error in PSMM_INV_SEARCH_TREE_FIND: {ex.Message}", ex);
        throw new ApplicationException("An error occurred while processing your request.", ex);
    }
}

// Helper method to add parameters
private void AddParameter(OracleCommand cmd, string parameterName, string value)
{
    cmd.Parameters.Add(parameterName, OracleDbType.Varchar2).Value = 
        string.IsNullOrEmpty(value) ? (object)DBNull.Value : value;
}