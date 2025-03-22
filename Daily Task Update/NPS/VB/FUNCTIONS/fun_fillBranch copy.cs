protected void BranchFill()
{
    string payrollId = txtrmbusicode.Text.Trim();
    if (string.IsNullOrEmpty(payrollId))
    {
        return;
    }

    else
    {
        try
        {

            string q1 = "SELECT category_id FROM employee_master WHERE payroll_id = '" +payrollId+"' AND type = 'A'";
            DataTable dt1 = pc.ExecuteCurrentQuery(q1);

            if (dt1.Rows.Count > 0)
            {
                string category_id = dt1.Rows[0]["category_id"].ToString();

                if (category_id != null && category_id != "" && category_id != "2001" && category_id != "2018")
                {
                    pc.ShowAlert(this, "Rm Should be Sales and Support Only");
                    txtrmbusicode.Text = "";
                    cmbBusiBranch.Items.Clear();
                    return;
                }

            }

            // If payrollId length is 5 or 6, fetch branch details
            if (payrollId.Length == 5 || payrollId.Length == 6)
            {
                cmbBusiBranch.Items.Clear();
                if (!string.IsNullOrEmpty(strbranch))
                {
                    cmbBusiBranch.Items.Add(new ListItem(strbranch));
                }

                string q2 = "Select source,branch_name from employee_master e,branch_master b where e.payroll_id='" +payrollId+ "' and e.source=b.branch_code and (e.type='A' or e.type is null)";
                DataTable dt2 = pc.ExecuteCurrentQuery(q2);

                if(dt2.Rows.Count>0){
                    DataRow dt2_row= dt2.Rows[0];
                    string branchCode = dt2_row["branch_code"].ToString();
                    string branchName = dt2_row["branch_name"].ToString();

                    if (!string.IsNullOrEmpty(strbranch))
                    {
                        string[] b_cd = strbranch.Split('#');
                        if (b_cd.Length > 1 && b_cd[1] != branchCode)
                        {
                            cmbBusiBranch.Items.Add(new ListItem(branchName + new string(' ', 100) + "#" + branchCode));
                        }
                    }
                    else
                    {
                        cmbBusiBranch.Items.Add(new ListItem(branchName + new string(' ', 100) + "#" + branchCode));
                    }
                    cmbBusiBranch.SelectedIndex = 0;
                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Error: " + ex.Message + "');", true);
        }
    }
}
