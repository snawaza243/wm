<%@ Page Title="Renewal Letter Printed" Language="C#" MasterPageFile="~/vmSite.Master" AutoEventWireup="true" CodeBehind="insurance_renewal_letter_printed.aspx.cs" Inherits="WM.Masters.insurance_renewal_letter_printed" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Stylesheets -->
    <link rel="stylesheet" href="assets/vendors/mdi/css/materialdesignicons.min.css" />
    <link rel="stylesheet" href="assets/vendors/ti-icons/css/themify-icons.css" />
    <link rel="stylesheet" href="assets/vendors/css/vendor.bundle.base.css" />
    <link rel="stylesheet" href="assets/vendors/font-awesome/css/font-awesome.min.css" />
    <link rel="stylesheet" href="assets/vendors/bootstrap-datepicker/bootstrap-datepicker.min.css" />
    <link rel="stylesheet" href="assets/css/style.css" />
    <script type="text/javascript">
        function hideMessage() {
            setTimeout(function () {
                var messageLabel = document.getElementById('<%= lblMessage.ClientID %>');
            if (messageLabel) {
                messageLabel.style.visibility = 'hidden';
            }
        }, 5000); // The message will be hidden after 5000 milliseconds (5 seconds)
        }
    </script>

    <!-- Page Header -->
    <div class="page-header">
        <h3 class="page-title">Renewal Letter Printed</h3>
    </div>

    <!-- Form -->
    <!-- Form content -->
    <div class="row">
        <div class="grid-margin stretch-card">
            <div class="card">
                <div class="card-body">
                    <h4 class="text-center">IMIS-30</h4>
                    <h5 class="card-title">Select Excel File and Company</h5>

                    <div class="row g-3 mb-4">
                        <div class="col-md-4">
                            <label for="selectCompany" class="form-label">Select Company</label>
                            <asp:DropDownList ID="selectCompany" runat="server" CssClass="form-select">
                                <asp:ListItem Text="Select Company" Value="" />
                                <asp:ListItem Text="Company A" Value="Company A" />
                                <asp:ListItem Text="Company B" Value="Company B" />
                                <asp:ListItem Text="Company C" Value="Company C" />
                            </asp:DropDownList>
                        </div>

                        <div class="col-md-2">
                            <label for="month" class="form-label">Month</label>
                            <asp:DropDownList ID="ddlMonths" runat="server" CssClass="form-select">

                                <asp:ListItem Text="Select Month" Value="" />
                                <asp:ListItem Text="January" Value="01" />
                                <asp:ListItem Text="February" Value="02" />
                                <asp:ListItem Text="March" Value="03" />
                                <asp:ListItem Text="April" Value="04" />
                                <asp:ListItem Text="May" Value="05" />
                                <asp:ListItem Text="June" Value="06" />
                                <asp:ListItem Text="July" Value="07" />
                                <asp:ListItem Text="August" Value="08" />
                                <asp:ListItem Text="September" Value="09" />
                                <asp:ListItem Text="October" Value="10" />
                                <asp:ListItem Text="November" Value="11" />
                                <asp:ListItem Text="December" Value="12" />



                            </asp:DropDownList>

                        </div>

                        <div class="col-md-2">
                            <label for="year" class="form-label">Year</label>
                            <asp:DropDownList ID="ddlYears" runat="server" CssClass="form-select">
                                <asp:ListItem Text="Select Year" Value="" />
                                <asp:ListItem Text="2022" Value="2022" />
                                <asp:ListItem Text="2023" Value="2023" />
                                <asp:ListItem Text="2024" Value="2024" />

                            </asp:DropDownList>
                        </div>

                        <div class="col-md-4">
                            <label for="PolicyNumber" class="form-label">Policy Number</label>
                            <asp:TextBox ID="txtPolicyNumber" runat="server" CssClass="form-control" oninput="validateNumber(this, 20)" PlaceHolder="Enter Policy Number" MaxLength="20" />
                        <script>
                            function validateNumber(input, maxLength = 10) {
                                var regex = /^[0-9]*$/;  // Regular expression to allow only numeric characters

                                // Check if the length exceeds max length
                                if (input.value.length > maxLength) {
                                    //  alert("This number cannot exceed " + maxLength + " digits.");
                                    input.value = input.value.slice(0, maxLength);  // Trim the input to max length
                                    return;
                                }

                                // Check if the last character is valid (numeric)
                                var lastChar = input.value.slice(-1);
                                if (!regex.test(lastChar)) {
                                    //  alert("The character '" + lastChar + "' is not allowed. Only numeric characters are permitted.");
                                    input.value = input.value.slice(0, -1); // Remove the last character
                                    input.focus();
                                }
                            }

                        </script>
                        </div>


                         <div class="form-check m-3">


                             <label class="form-check-label">
                                 Marked 
        
         <asp:CheckBox
             ID="chckRemFlag"
             runat="server" />

                             </label>
                         </div>


                    </div>

                    <div class="d-flex align-items-center flex-md-row flex-column gap-3">
                        <asp:Button ID="btnGenerate" runat="server" CssClass="btn btn-primary" Text="Generate" OnClick="btnGenerate_Click" />
                        <asp:Button ID="btnMark" runat="server" CssClass="btn btn-outline-primary" Text="Mark" OnClick="btnMark_Click" />
                        <%--<asp:Button ID="btnPrint" runat="server" Text="Print" CssClass="btn btn-primary" OnClientClick="printGrid()" />--%>
                        <asp:Button ID="btnPrint" runat="server" Text="Print" CssClass="btn btn-primary" OnClick="btnGenerateAll_Click" />

                        <asp:Button ID="btnReset" runat="server" CssClass="btn btn-outline-primary" Text="Reset" OnClick="btnReset_Click" />
                        <asp:Button ID="btnExit" runat="server" CssClass="btn btn-outline-primary" Text="Exit" OnClick="btnExit_Click" />

                    </div>

                    <div class="row mt-3">
                        <div class="col-md-12">
                            <asp:Label ID="lblMessage" runat="server" CssClass="text-danger"></asp:Label>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
        <h5 class="card-title">Company Data</h5>
    <div style="overflow-y: auto; overflow-x: auto; max-height: 450px;">

 <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <div class="table-responsive" >
            <div class="scrollable-container" >
                <asp:GridView ID="companyGridView" runat="server" CssClass="table table-hover" AutoGenerateColumns="false" OnRowCommand="companyGridView_RowCommand">
                   <Columns>
    <asp:TemplateField HeaderText="#" ItemStyle-CssClass="column-width-50">
        <ItemTemplate>
            <asp:Label ID="lblSr" runat="server" Text='<%# Container.DisplayIndex + 1 %>' />
        </ItemTemplate>
    </asp:TemplateField>
 <asp:TemplateField HeaderText="Select" ItemStyle-CssClass="column-width-100">
    
      <ItemTemplate>
                <asp:Button ID="btnSelect" runat="server" Text="Select" CommandName="SelectPolicy" CommandArgument='<%# Container.DisplayIndex %>' />
            </ItemTemplate>
 </asp:TemplateField>

<asp:TemplateField HeaderText="Policy No" ItemStyle-CssClass="column-width-150">
    <ItemTemplate>
        <asp:Label ID="lblPolicyNo" runat="server" Text='<%# Eval("POLICY_NO") %>' />
    </ItemTemplate>
</asp:TemplateField>

    <asp:TemplateField HeaderText="AR Branch Code" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblARBranchCode" runat="server" Text='<%# Eval("AR_BRANCH_CD") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Company Code" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblCompanyCode" runat="server" Text='<%# Eval("COMPANY_CD") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Client Name" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblClientName" runat="server" Text='<%# Eval("CLIENT_NAME") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Address 1" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblAddress1" runat="server" Text='<%# Eval("ADDRESS1") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Address 2" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblAddress2" runat="server" Text='<%# Eval("ADDRESS2") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="CL_ADD3" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblCLAdd3" runat="server" Text='<%# Eval("CL_ADD3") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="CL_ADD4" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblCLAdd4" runat="server" Text='<%# Eval("CL_ADD4") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="CL_ADD5" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblCLAdd5" runat="server" Text='<%# Eval("CL_ADD5") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Phone 1" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblPhone1" runat="server" Text='<%# Eval("CL_PHONE1") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Phone 2" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblPhone2" runat="server" Text='<%# Eval("CL_PHONE2") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="City Name" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblCityName" runat="server" Text='<%# Eval("CITY_NAME") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Due Date" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblDueDate" runat="server" Text='<%# Eval("DUE_DATE") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Reminder Flag" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblReminderFlag" runat="server" Text='<%# Eval("REM_FLAGE") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="State Name" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblStateName" runat="server" Text='<%# Eval("STATE_NAME") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Company Name" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblCompanyName" runat="server" Text='<%# Eval("COMPANY_NAME") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Favour Name" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblFavourName" runat="server" Text='<%# Eval("FAVOUR_NAME") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Branch Name" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblBranchName" runat="server" Text='<%# Eval("BRANCH_NAME") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Branch Address 1" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblBranchAdd1" runat="server" Text='<%# Eval("BRANCH_ADD1") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Branch Address 2" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblBranchAdd2" runat="server" Text='<%# Eval("BRANCH_ADD2") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Plan Name 1" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblPlanName1" runat="server" Text='<%# Eval("PLAN_NAME1") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Payment Mode" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblPayMode" runat="server" Text='<%# Eval("PAY_MODE") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    
    <asp:TemplateField HeaderText="P Name" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblPName" runat="server" Text='<%# Eval("P_NAME") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="I Name" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblIName" runat="server" Text='<%# Eval("I_NAME") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Premium Frequency" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblPremFreq" runat="server" Text='<%# Eval("PREM_FREQ") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Base Premium Frequency" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblBasePremFreq" runat="server" Text='<%# Eval("BPREM_FREQ") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Plan Name" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblPlanName" runat="server" Text='<%# Eval("PLAN_NAME") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Sum Assured" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblSumAssured" runat="server" Text='<%# Eval("SA") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Premium Amount" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblPremAmount" runat="server" Text='<%# Eval("PREM_AMT") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Month Number" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblMonthNo" runat="server" Text='<%# Eval("MON_NO") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Year Number" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblYearNo" runat="server" Text='<%# Eval("YEAR_NO") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Client PIN" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblClientPin" runat="server" Text='<%# Eval("CL_PIN") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="PIN1" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblPin1" runat="server" Text='<%# Eval("PIN1") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="INV Code" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblINVCode" runat="server" Text='<%# Eval("INV_CODE") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="INV Code 1" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblINVCode1" runat="server" Text='<%# Eval("INV_CODE1") %>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Import Data Type" ItemStyle-CssClass="column-width-150">
        <ItemTemplate>
            <asp:Label ID="lblImportDataType" runat="server" Text='<%# Eval("IMPORTDATATYPE") %>' />
        </ItemTemplate>
    </asp:TemplateField>

   
</Columns>

                
                
                </asp:GridView>
            </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
    </div>



    <script>
        function printGrid() {
            // Get the HTML of the UpdatePanel containing the GridView
            const updatePanelHtml = document.getElementById("<%= UpdatePanel1.ClientID %>").innerHTML;

            // Open a new window
            const printWindow = window.open('', '', 'height=600,width=800');

            // Write the HTML to the new window
            printWindow.document.write(`
            <html>
                <head>
                   
                    <style>
                        body {
                            font-family: Arial, sans-serif;
                        }
                        table {
                            width: 100%;
                            border-collapse: collapse;
                        }
                        th, td {
                            padding: 8px;
                            text-align: left;
                            border: 1px solid #ddd;
                        }
                    </style>
                </head>
                <body>
                    <h5 class="card-title">Company Data</h5>
                    ${updatePanelHtml}
                </body>
            </html>
        `);
            printWindow.document.close(); // Close the document
            printWindow.print(); // Trigger print
        }
    </script>



    <!-- Scripts -->
    <script src="assets/vendors/js/vendor.bundle.base.js"></script>
    <script src="assets/vendors/chart.js/chart.umd.js"></script>
    <script src="assets/vendors/bootstrap-datepicker/bootstrap-datepicker.min.js"></script>
    <script src="assets/js/off-canvas.js"></script>
    <script src="assets/js/misc.js"></script>
    <script src="assets/js/settings.js"></script>
    <script src="assets/js/todolist.js"></script>
    <script src="assets/js/jquery.cookie.js"></script>
    <script src="assets/js/dashboard.js"></script>
</asp:Content>
