

public void SetBranch(){

    string brachCode = txtBranchCode.Text;

    if(brachCode.length() == 5 or brachCode.length() == 6){
        ddlBranch.Item.Clear();

        string strbranch = txtstrbranch.Text;

        if(!string.IsNullOrEmpty(strbranch)){
            ddlBranch.AddItem = strbranch;
            ddlBranch.SelectIndex = 0;
        }
        else{
            DataTable dt1 = new DataTable();
            string sql1 = "";
            sql1 =  "Select source,branch_name from employee_master e,branch_master b where e.payroll_id='" & Trim(txtrmbusicode.Text) & "' and e.source=b.branch_code and (e.type='A' or e.type is null)";
            dt1 = pc.ExecuteCurrentQuery(sql1);

            if(dt1.Rows.Count>0){
                
            }

        }

    }
}