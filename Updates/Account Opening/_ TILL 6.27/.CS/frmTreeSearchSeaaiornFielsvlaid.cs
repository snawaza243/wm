 string currrentFormSession = Session["AC_FIND"] != null ? Session["AC_FIND"].ToString() : null;

 if (currrentFormSession == "CLIENT")
 {
     lblFormTitle.Text = "Client Search";
     ddlCategoryList.SelectedIndex = 0;
     ddlCategoryList.Enabled = false;
     ddlClientList.SelectedIndex = 0;
     ddlClientList.Enabled = false;
     pnlClientCode0.Visible = true;
     lblClientCode0.Text = "Client AH Code:";
     pnlClientCode00.Visible = false;
 }

 else if (currrentFormSession == "INVESTOR")
 {
     lblFormTitle.Text = "Investor Search";
     ddlCategoryList.SelectedIndex = 0;
     ddlCategoryList.Enabled = false;
     ddlClientList.SelectedIndex = 2;
     ddlClientList.Enabled = false;
     pnlClientCode0.Visible = true;
     lblClientCode0.Text = "Investor Code:";
     pnlClientCode00.Visible = true;
     lblClientCode00.Text = "Client Code:";
     txtClientCode00.MaxLength = 8;

     ddlBranchList.SelectedIndex = 0;
     ddlBranchList.Enabled = false;
 }

 else if (currrentFormSession == "AGENT")
 {
     lblFormTitle.Text = "Agent Search";
     ddlCategoryList.SelectedIndex = 0;
     ddlCategoryList.Enabled = false;
     ddlClientList.SelectedIndex = 1;
     ddlClientList.Enabled = false;

     lblClientCode0.Text = "Agent Code:";
     pnlClientCode0.Visible = true;
     txtClientCode0.MaxLength = 8;

     lblClientCode00.Text = "Exist Code:";
     pnlClientCode00.Visible = true;
     txtClientCode00.MaxLength = 20;

     ddlBranchList.SelectedIndex = 0;
     ddlBranchList.Enabled = true;

     pnlRMList.Visible = true;
 }

 else
 {
     lblFormTitle.Text = "Client Search";
     ddlCategoryList.SelectedIndex = 0;
     ddlCategoryList.Enabled = false;
     ddlClientList.SelectedIndex = 0;
     ddlClientList.Enabled = true;
     pnlClientCode0.Visible = true;
     lblClientCode0.Text = "Client AH Code:";
     pnlClientCode00.Visible = false;
     ddlBranchList.SelectedIndex = 0;
     pnlRMList.Visible = true;
     pnlRMList.Enabled = false;

 }