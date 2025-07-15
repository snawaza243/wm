<%@ Page Title="Map Policy Number" Language="C#" MasterPageFile="~/vmSite.Master" AutoEventWireup="true" CodeBehind="map_policy_number.aspx.cs" Inherits="WM.Masters.map_policy_number" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Stylesheets -->
    <link rel="stylesheet" href="assets/vendors/mdi/css/materialdesignicons.min.css" />
    <link rel="stylesheet" href="assets/vendors/ti-icons/css/themify-icons.css" />
    <link rel="stylesheet" href="assets/vendors/css/vendor.bundle.base.css" />
    <link rel="stylesheet" href="assets/vendors/font-awesome/css/font-awesome.min.css" />
    <link rel="stylesheet" href="assets/vendors/bootstrap-datepicker/bootstrap-datepicker.min.css" />
    <link rel="stylesheet" href="assets/css/style.css" />
    <script src="https://cdnjs.cloudflare.com/ajax/libs/xlsx/0.16.6/xlsx.full.min.js"></script>

    <%-- Loader styles --%>
    <style>
        #loader {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(255, 255, 255, 0.8);
            display: flex;
            justify-content: center;
            align-items: center;
            z-index: 9999;
        }

        .spinner {
            border: 6px solid #f3f3f3;
            border-top: 6px solid #3498db;
            border-radius: 50%;
            width: 50px;
            height: 50px;
            animation: spin 1s linear infinite;
        }

        @keyframes spin {
            0% {
                transform: rotate(0deg);
            }

            100% {
                transform: rotate(360deg);
            }
        }
    </style>

    <!-- Loader -->
<!-- Server-Side Loader (For Full Page Requests Like Exporting Excel) -->
<div id="serverLoader" runat="server" style="display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; background-color: rgba(255, 255, 255, 0.7); z-index: 1000; text-align: center;">
    <div style="position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%);">
        <div class="spinner-border" role="status">
            <span class="sr-only">Processing...</span>
        </div>
    </div>
</div>

<!-- Client-Side Loader (For AJAX Postbacks) -->
<div id="clientLoader" style="display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; background-color: rgba(0, 0, 0, 0.5); z-index: 2000; text-align: center;">
    <div style="position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%);">
        <div class="spinner-border text-light" role="status">
            <span class="sr-only">Loading...</span>
        </div>
    </div>
</div>


<script type="text/javascript">
    // Show / Hide Server Loader (Full Page Requests)
    function showServerLoader() {
        document.getElementById('serverLoader').style.display = 'block';
    }

    function hideServerLoader() {
        document.getElementById('serverLoader').style.display = 'none';
    }

    // Show / Hide Client Loader (For AJAX Requests in UpdatePanel)
    function showClientLoader() {
        document.getElementById('clientLoader').style.display = 'block';
    }

    function hideClientLoader() {
        document.getElementById('clientLoader').style.display = 'none';
    }

    // Automatically hide client loader after AJAX postbacks
    Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function () {
        showClientLoader();
    });

    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
        hideClientLoader();
    });
</script>

    <div class="page-header">
        <h3 class="page-title">Map Policy Number</h3>
    </div>

    <!-- Form -->
    <div class="row">
        <div class="grid-margin stretch-card">
            <div class="card">
                <div class="card-body">
                    <div class="row g-3 mb-6">
                        <div class="col-md-6">
                            <asp:Label ID="lblFIleName" runat="server" CssClass="form-label" Text="File Name: <span class='text-danger'>*</span>"></asp:Label>
                            <div class=" d-flex">
                                <asp:FileUpload ID="fileInput" runat="server" CssClass="form-control mr-4"  AutoPostBack="true" OnChanged="FileInput_Changed" />                                
                                <asp:Button ID="uploadButton" runat="server" Text="Upload" OnClick="UploadButton_Click" CssClass="btn btn-primary" />
                                <asp:Label ID="Label1" runat="server"></asp:Label>
                            </div>
                        </div>


                        <div class="col-md-6">
                            <asp:Label ID="companySelectLable" runat="server" CssClass="form-label" Text="Company"></asp:Label>

                            <asp:DropDownList ID="companySelect" CssClass="form-select" runat="server">
                                <asp:ListItem Text="Select Company" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        
                       
                        <div class="col-md-4">
                            <asp:Label ID="ddlSheetListLable" runat="server" CssClass="form-label" Text="Select Sheet <span class='text-danger'>*</span>"></asp:Label>

                            <asp:DropDownList ID="ddlSheetList" runat="server" CssClass="form-select" OnSelectedIndexChanged="ExcelSheetSelect_SelectedIndexChanged"
                                AutoPostBack="true" Enabled ="false">
                                <asp:ListItem Text="Select Sheet" Value=""></asp:ListItem>
                                <asp:ListItem Text="Sheet1" Value="Sheet 1"></asp:ListItem>
                                <asp:ListItem Text="Sheet2" Value="Sheet 2"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-8 text-end pt-4"">
                            <asp:Button ID="btnImport" runat="server" CssClass="btn btn-outline-primary" Text="Import" 
                                OnClientClick="return confirmImport();"
                                OnClick="btnImport_Click" />
                            <script type="text/javascript">
                                function confirmImport() {
                                    var ddl = document.getElementById('<%= companySelect.ClientID %>');
                                    var selectedText = ddl.options[ddl.selectedIndex].text;

                                    var message = (selectedText === "Select Company")
                                        ? "You are importing data for all companies from sheet. Do you want to proceed?"
                                        : "You are importing data for " + selectedText + " company from sheet. Do you want to proceed?";

                                    if (confirm(message)) {
                                        showClientLoader(); 
                                        return true; 
                                    } else {
                                        return false; // Cancel action
                                    }
                                }
                            </script>

                            <asp:Button ID="btnExport" runat="server" CssClass="btn btn-outline-primary" Text="Export" 
                                 OnClientClick="showServerLoader(); return true;"
                                OnClick="btnExport_Click" />


                            <asp:Button ID="btnReset" runat="server" CssClass="btn btn-outline-primary" Text="Reset" OnClick="btnReset_Click" />
                            <asp:Button ID="btnExit" runat="server" CssClass="btn btn-outline-secondary" Text="Exit" OnClick="btnExit_Click" />
                        </div>

                        <div class="col-md-12">
                            <asp:Label ID="lblMessage" runat="server"></asp:Label>

                        </div>

                        <div class="col-md-12 border-end">
                            <%--<asp:Label ID="lblPolicyDataGrid" runat="server" CssClass="form-label" Text="Policy Mapping Data to Import"></asp:Label>--%>

                            <div class="table-responsive " style="overflow-y: auto; overflow-x: auto; max-height: 250px; min-height: 1px">
                                <asp:GridView ID="GridView1" runat="server" CssClass="table table-bordered" AutoGenerateColumns="true"></asp:GridView>
                            </div>

                        </div>
                        

                        <div class="col-md-6 border-start d-none" style="min-height: 250px;">
                            <asp:Label ID="lblPolicExportData" runat="server" CssClass="form-label" Text="Mapped Data for Exporitng Report"></asp:Label>
                            
                            <div class="table-responsive " style="overflow-y: auto; overflow-x: auto; max-height: 250px; min-height: 1px">
                                <asp:GridView ID="gridPolicyData" runat="server" AutoGenerateColumns="False" OnRowCommand="gvPolicyData_RowCommand"
                                    CssClass="table mt-4">
                                    <Columns>
                                        <asp:BoundField DataField="POLICY_NO" HeaderText="Policy No" SortExpression="POLICY_NO" />
                                        <asp:BoundField DataField="max_amt" HeaderText="Max Amount" SortExpression="max_amt" />
                                        <asp:BoundField DataField="PREM_FREQ" HeaderText="Premium Frequency" SortExpression="PREM_FREQ" />
                                        <asp:BoundField DataField="NEXT_DUE_DT" HeaderText="Next Due Date" SortExpression="NEXT_DUE_DT" />
                                        <asp:BoundField DataField="COMPANY_CD" HeaderText="Company Code" SortExpression="COMPANY_CD" />
                                        <asp:BoundField DataField="REGION_NAME" HeaderText="Region Name" SortExpression="REGION_NAME" />
                                        <asp:BoundField DataField="ZONE_NAME" HeaderText="Zone Name" SortExpression="ZONE_NAME" />
                                        <asp:BoundField DataField="RM_NAME" HeaderText="RM Name" SortExpression="RM_NAME" />
                                        <asp:BoundField DataField="BRANCH_NAME" HeaderText="Branch Name" SortExpression="BRANCH_NAME" />
                                        <asp:BoundField DataField="INVESTOR_NAME" HeaderText="Investor Name" SortExpression="INVESTOR_NAME" />
                                        <asp:BoundField DataField="ADDRESS1" HeaderText="Address 1" SortExpression="ADDRESS1" />
                                        <asp:BoundField DataField="ADDRESS2" HeaderText="Address 2" SortExpression="ADDRESS2" />
                                        <asp:BoundField DataField="CITY_NAME" HeaderText="City" SortExpression="CITY_NAME" />
                                        <asp:BoundField DataField="STATE_NAME" HeaderText="State" SortExpression="STATE_NAME" />
                                        <asp:BoundField DataField="MOBILE" HeaderText="Mobile" SortExpression="MOBILE" />
                                        <asp:BoundField DataField="PHONE" HeaderText="Phone" SortExpression="PHONE" />
                                    </Columns>
                                </asp:GridView>

                            </div>
                        </div>
                    </div>

                    <asp:Label ID="lblInfo" runat="server" CssClass="form-label">
                        <span class='text-danger'>*</span> Importing sheet header name should be like this: 
                        <strong>Policy_No</strong> and <strong>Company_Code</strong>
                    </asp:Label>



                </div>


            </div>

        </div>
    </div>


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
