  public void ADD_TO_SEARCHED_LIST()
  {
      try
      {
          DataTable dt_toList;

          bool anyRowSelected = false;
          bool sourceIDExists = false;

          if (agentsGrid.DataSource == null && agentsGrid.Rows.Count == 0)
          {
              dt_toList = new DataTable();
              dt_toList.Columns.Add("AGENT_NAME");
              dt_toList.Columns.Add("agent_code");
              dt_toList.Columns.Add("EXIST_CODE");
              dt_toList.Columns.Add("ADDRESS1");
              dt_toList.Columns.Add("ADDRESS2");
              dt_toList.Columns.Add("BranchName");
              dt_toList.Columns.Add("BranchCode");
          }
          else
          {
              dt_toList = ((DataView)agentsGrid.DataSource).ToTable();
          }

          foreach (GridViewRow row in agentsGridSearchedMaster.Rows)
          {
              CheckBox chkSelectSearchedMaster = (CheckBox)row.FindControl("chkSelectSearchedMaster");

              if (chkSelectSearchedMaster != null && chkSelectSearchedMaster.Checked)
              {
                  string newBranchCode = ((Label)row.FindControl("lblBranchCodeSearchedMaster")).Text;

                  foreach (GridViewRow targetRow in agentsGrid.Rows)
                  {
                      // Get the SourceID from the target grid row
                      string existingBranchCode = ((Label)targetRow.FindControl("lblBranchCodeSearched")).Text;

                      // Compare the SourceID values, if found different branch agent then break and show alert that agent should be the same branch
                      if (newBranchCode != existingBranchCode)
                      {
                          sourceIDExists = true;
                          break;
                      }
                  }

                  if (sourceIDExists)
                  {
                      string msg = "All selected agents should belongs from the same branch!";
                      pc.ShowAlert(this, msg);
                      return;
                  }
                  else
                  {
                      DataRow newRow = dt_toList.NewRow();
                      newRow["Agent_Code"] = ((Label)row.FindControl("lblAgentCodeSearchedMaster")).Text;
                      newRow["Exist_Code"] = ((Label)row.FindControl("lblExistCodeSearchedMaster")).Text;
                      newRow["Agent_Name"] = ((Label)row.FindControl("lblAgentNameSearchedMaster")).Text;
                      newRow["Address1"] = ((Label)row.FindControl("lblAddress1SearchedMaster")).Text;
                      newRow["Address2"] = ((Label)row.FindControl("lblAddress2SearchedMaster")).Text;
                      newRow["BranchName"] = ((Label)row.FindControl("lblBranchNameSearchedMaster")).Text;
                      newRow["BranchCode"] = newBranchCode;
                      dt_toList.Rows.Add(newRow);
                  }
              }
          }

          agentsGrid.DataSource = dt_toList.DefaultView;
          agentsGrid.DataBind();
      }
      catch (Exception ex)
      {
          pc.ShowAlert(this, ex.Message);
          return;
      }

  }
