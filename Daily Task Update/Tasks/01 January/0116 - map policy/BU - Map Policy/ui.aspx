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




    <div class="page-header">
        <h3 class="page-title">Map Policy Number</h3>
    </div>

    <!-- Form -->
    <div class="row">
        <div class="grid-margin stretch-card">
            <div class="card">
                <div class="card-body">
                    <div class="row g-3 mb-6">
                        <div class="col-md-4">
                            <label for="fileUpload1" class="form-label">Select File (.xls)</label>
                            <div class="input-group mb-3">
                                <asp:FileUpload ID="FileUpload1" runat="server" CssClass="form-control" />
                            </div>
                        </div>

                        <div class="col-md-2">
                            <label for="ddlSheetList" class="form-label">Select Sheet</label>
                            <asp:DropDownList ID="ddlSheetList" runat="server" CssClass="form-select">
                                <asp:ListItem Text="Select Sheet" Value=""></asp:ListItem>
                                <asp:ListItem Text="Sheet1" Value="Sheet 1"></asp:ListItem>
                                <asp:ListItem Text="Sheet2" Value="Sheet 2"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-6 text-end">
                            <div style="height: 20px;" class="form-label">&nbsp;</div>
                            <asp:Button ID="btnImport" runat="server" CssClass="btn btn-outline-primary" Text="Import/Export" OnClick="btnImport_Click" />
                            <%--<asp:Button ID="btnExport" runat="server" CssClass="btn btn-outline-primary" Text="Export Policy Report" OnClick="btnExport_Click" />--%>

                            <asp:Button ID="btnReset" runat="server" CssClass="btn btn-outline-primary" Text="Reset" OnClick="btnReset_Click" />
                            <asp:Button ID="btnExit" runat="server" CssClass="btn btn-outline-secondary" Text="Exit" OnClick="btnExit_Click" />

                        </div>

                        <div class="col-md-12">
                            <asp:Label ID="lblSheetMsg" runat="server" CssClass="ms-2 border rounded-3"
                                Style="display: block; height: 150px; overflow-y: auto; overflow-x: auto; border: 1px solid #ccc; padding: 5px; background-color: #f9f9f9;"
                                Text=""></asp:Label>
                        </div>


                        <div class="col-md-12 mt-3">
                            <asp:Literal ID="lblExportMsg" runat="server" />
                        </div>

                        <div class="col-md-12">

                            <div class="table-responsive "
                                style="overflow-y: auto; overflow-x: auto; display: block; height: 350px; padding: 5px; "
                                Cssclass="ms-2 border rounded-3">

                                <asp:GridView ID="gridPolicyData" runat="server" AutoGenerateColumns="False" OnRowCommand="gvPolicyData_RowCommand" CssClass="table mt-4">
                                    <Columns>
                                        <asp:BoundField DataField="POLICY_NO" HeaderText="Policy Number" />
                                        <asp:BoundField DataField="max_amt" HeaderText="Max Amount" />
                                        <asp:BoundField DataField="PREM_FREQ" HeaderText="Premium Frequency" />
                                        <asp:BoundField DataField="NEXT_DUE_DT" HeaderText="Next Due Date" />
                                        <asp:BoundField DataField="COMPANY_CD" HeaderText="Company Code" />
                                        <asp:BoundField DataField="REGION_NAME" HeaderText="Region Name" />
                                        <asp:BoundField DataField="ZONE_NAME" HeaderText="Zone Name" />
                                        <asp:BoundField DataField="RM_NAME" HeaderText="RM Name" />
                                        <asp:BoundField DataField="BRANCH_NAME" HeaderText="Branch Name" />
                                        <asp:BoundField DataField="INVESTOR_NAME" HeaderText="Investor Name" />

                                        <asp:BoundField DataField="BRANCH_NAME" HeaderText="Branch Name" />
                                        <asp:BoundField DataField="ADDRESS1" HeaderText="Address 1" />
                                        <asp:BoundField DataField="ADDRESS2" HeaderText="Address 2" />
                                        <asp:BoundField DataField="CITY_NAME" HeaderText="City" />
                                        <asp:BoundField DataField="STATE_NAME" HeaderText="State" />
                                        <asp:BoundField DataField="MOBILE" HeaderText="Mobile" />
                                        <asp:BoundField DataField="PHONE" HeaderText="Phone" />

                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:Button ID="btnSelect" runat="server" Text="Select" style="display:none" CommandName="SelectRow" CommandArgument='<%# Eval("POLICY_NO") %>' CssClass="btn btn-info" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>

                        </div>

                        <%--  <div class="col-md-12">
                            <asp:Button ID="btnReset" runat="server" CssClass="btn btn-outline-primary" Text="Reset" OnClick="btnReset_Click" />
                            <asp:Button ID="btnExit" runat="server" CssClass="btn btn-outline-secondary" Text="Exit" OnClick="btnExit_Click" />

                        </div>--%>

                        <%--<div class="col-md-12 d-flex gap-2">
                            <asp:Button ID="btnExport" runat="server" CssClass="btn btn-outline-primary" Text="Export Policy Report" OnClick="btnExport_Click" />
                            <asp:Literal ID="litMessage" runat="server" />
                        </div>--%>
                    </div>


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
