  public (bool Success, string Message, Dictionary<string, object> Data) PSMM_INV_ADD_UP_GET_DATA(string invCode)
  {
      string loginId = currentLoginID();
      string roleId = currentRoleID();
      var data = new Dictionary<string, object>();

      using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
      {
          using (OracleCommand cmd = new OracleCommand("WEALTHMAKER.PSMM_INV_ADD_UP_GET_DATA", conn))
          {
              cmd.CommandType = CommandType.StoredProcedure;

              // Input parameters
              cmd.Parameters.Add("P_INV_CODE", OracleDbType.Varchar2).Value = invCode ?? (object)DBNull.Value;
              cmd.Parameters.Add("P_LOGIN", OracleDbType.Varchar2).Value = loginId ?? (object)DBNull.Value;
              cmd.Parameters.Add("P_ROLE", OracleDbType.Varchar2).Value = roleId ?? (object)DBNull.Value;

              // Output cursor
              var outCursor = new OracleParameter("P_CURSOR", OracleDbType.RefCursor)
              {
                  Direction = ParameterDirection.Output
              };
              cmd.Parameters.Add(outCursor);

              try
              {
                  conn.Open();
                  cmd.ExecuteNonQuery();

                  using (var reader = ((OracleRefCursor)outCursor.Value).GetDataReader())
                  {
                      if (reader.Read())
                      {
                          // Always return the message
                          string message = reader["MSG"]?.ToString() ?? "No message returned";
                          bool isSuccess = message.StartsWith("SUCCESS:");

                          // Collect all returned fields dynamically
                          for (int i = 0; i < reader.FieldCount; i++)
                          {
                              string colName = reader.GetName(i);
                              object colValue = reader.IsDBNull(i) ? null : reader.GetValue(i);
                              data[colName] = colValue;
                          }

                          return (isSuccess, message, data);
                      }
                  }

                  return (false, "No response received from procedure", data);
              }
              catch (OracleException ex)
              {
                  string errorMessage = $"Database Error: {ex.Message}";
                  return (false, errorMessage, null);
              }
              catch (Exception ex)
              {
                  return (false, $"System Error: {ex.Message}", null);
              }
          }
      }
  }
