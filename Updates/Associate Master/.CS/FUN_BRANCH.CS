    protected void FillValidBranchList()
    {
        DataTable dt = GetValidBranched();
        int dtCount = dt.Rows.Count;

        if (dtCount>0)
        {

            ddlBranchAM.DataSource = dt;
            ddlBranchAM.DataTextField = "branch_name"; 
            ddlBranchAM.DataValueField = "branch_code";  
            ddlBranchAM.DataBind();
            ddlBranchAM.Items.Insert(0, new ListItem("Select", ""));
            ddlBranchAM.Enabled = false;
        }
    }


    public void FillChannelBranches(string currentLoginId = null, string currentRoleId = null)
    {
        // Retrieve session values if parameters are not provided
        if (string.IsNullOrEmpty(currentLoginId))
            currentLoginId = HttpContext.Current.Session["LoginId"] as string;

        if (string.IsNullOrEmpty(currentRoleId))
            currentRoleId = HttpContext.Current.Session["RoleId"] as string;

        // Initialize an empty array to avoid returning null
        DataTable dt = new DataTable();

        // Ensure session values are not null before proceeding
        if (!string.IsNullOrEmpty(currentLoginId) && !string.IsNullOrEmpty(currentRoleId))
        {
            // Create an instance of AssociateController
            WM.Controllers.AssociateController controller = new WM.Controllers.AssociateController();

            // Get branch codes from the controller and assign to branchCodes
            dt = controller.PSMGetChannewlBranches(currentLoginId, currentRoleId);
            int rowCount = dt.Rows.Count;

            if (rowCount > 0)
            {

                branchsbl.DataSource = dt;
                branchsbl.DataTextField = "branch_name";
                branchsbl.DataValueField = "branch_code";
                branchsbl.DataBind();
                branchsbl.Items.Insert(0, new ListItem("Select Branch", ""));

                // super ana search 
                ddlSourceID.DataSource = dt;
                ddlSourceID.DataTextField = "branch_name";
                ddlSourceID.DataValueField = "branch_code";
                ddlSourceID.DataBind();
                ddlSourceID.Items.Insert(0, new ListItem("Select Branch", ""));
            }
        }

        ;
    }
