<%@ Page Title="ANA Merging" Language="C#" MasterPageFile="~/vmSite.Master" AutoEventWireup="true" CodeBehind="ANA_Merging.aspx.cs" Inherits="WM.Masters.ANA_Merging" EnableViewState="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link rel="shortcut icon" href="../assets/images/favicon.png" />
    <link rel="stylesheet" href="../assets/vendors/mdi/css/materialdesignicons.min.css">
    <link rel="stylesheet" href="../assets/vendors/ti-icons/css/themify-icons.css">
    <link rel="stylesheet" href="../assets/vendors/css/vendor.bundle.base.css">
    <link rel="stylesheet" href="../assets/vendors/font-awesome/css/font-awesome.min.css">
    <link rel="stylesheet" href="../assets/vendors/bootstrap-datepicker/bootstrap-datepicker.min.css">
    <link rel="stylesheet" href="../assets/css/style.css">
    <div class="content-wrapper">
        <div class="page-header">
            <h3 class="page-title">ANA Merging
            </h3>
        </div>

        <div class="row">
            <div class="col-md-12 grid-margin stretch-card">
                <div class="card">
                    <div class="card-body">
                        <h4 class="card-title">Select client to merge</h4>
                        <div class="row g-3">

                            <div class="col-md-3">
                                <label class="form-label" for="sourceid">Branch</label>
                                <asp:DropDownList ID="ddlSourceID" CssClass="form-select" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSourceID_SelectedIndexChanged">
                                    <asp:ListItem Value="0">Select</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <script type="text/javascript">
                                function toggleAgentNameReadOnly() {
                                    var ddlRM = document.getElementById('<%= ddlRM.ClientID %>');
                                    var txtAgentName = document.getElementById('<%= txtAgentName.ClientID %>');

                                    if (ddlRM.value) {
                                        txtAgentName.readOnly = true; // Set read-only if RM Code is selected
                                    } else {
                                        txtAgentName.readOnly = false; // Make editable if no RM Code is selected
                                    }
                                }
                            </script>

                            <div class="col-md-3">
                                <label class="form-label" for="rmsbl">RM Code</label>
                                <%--<asp:DropDownList ID="DropDownList1" CssClass="form-select" runat="server" onchange="toggleAgentNameReadOnly()">--%>

                                <asp:DropDownList ID="ddlRM" CssClass="form-select" runat="server">
                                    <asp:ListItem Value="">Select</asp:ListItem>
                                    <asp:ListItem Value="rm1">RM 1</asp:ListItem>
                                    <asp:ListItem Value="rm2">RM 2</asp:ListItem>
                                </asp:DropDownList>
                            </div>

                            <div class="col-md-3">
                                <label class="form-label" for="agentname">Agent Name</label>
                                <asp:TextBox ID="txtAgentName" CssClass="form-control" runat="server" placeholder="Enter Agent Name" />
                            </div>

                            <div class="col-md-3">
                                <label class="form-label" for="existcode">Exist Code</label>
                                <asp:TextBox ID="txtExistCode" CssClass="form-control" runat="server" placeholder="Enter Exist Code"></asp:TextBox>
                            </div>
                            <div class="col-md-3">
                                <asp:Button ID="btnSearch" CssClass="w-100 btn btn-primary" Text="Search" runat="server" OnClick="btnSearch_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row g-3">
            <div class="col-md-5">
                <div class="card" style="height: 480px;">
                    <div class="card-body">
                        <h4 class="card-title">Agents Searched:</h4>
                        <div class="table-responsive " style="overflow-y: auto; overflow-x: auto; max-height: 300px;">
                            <asp:GridView ID="agentsGrid" CssClass="table table-hover" runat="server">
                                <Columns>

                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="chkHeader" runat="server" AutoPostBack="True" OnCheckedChanged="chkHeader_CheckedChanged" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkSelect" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>


                                    <asp:TemplateField HeaderText="Agent Code" ItemStyle-CssClass="column-width-100">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAgentCodeSearched" runat="server" Text='<%# Eval("Agent_Code") %>' Visible="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Exist Code" ItemStyle-CssClass="column-width-100">
                                        <ItemTemplate>
                                            <asp:Label ID="lblExistCodeSearched" runat="server" Text='<%# Eval("Exist_Code") %>' Visible="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>


                                    <asp:TemplateField HeaderText="Agent Name" ItemStyle-CssClass="column-width-100">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAgentNameSearched" runat="server" Text='<%# Eval("Agent_Name") %>' Visible="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>


                                    <asp:TemplateField HeaderText="Address 1" ItemStyle-CssClass="column-width-100">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAddress1Searched" runat="server" Text='<%# Eval("Address1") %>' Visible="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Address 2" ItemStyle-CssClass="column-width-100">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAddress2Searched" runat="server" Text='<%# Eval("Address2") %>' Visible="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                </Columns>
                            </asp:GridView>
                        </div>

                        <p class="text-danger">Select client from this data sheet to include it into the data sheets on the right side.</p>
                    </div>
                </div>
            </div>

            <div class="col-md-3">
                <div class="card" style="height: 480px;">
                    <div class="card-body">
                        <asp:Button ID="btnSetAsMainAgent" CssClass="btn mb-3 btn-primary w-100" Text="Set As Main Agent" runat="server" OnClick="btnSetAsMainAgent_Click" />

                        <asp:Button ID="btnSelectAgentToMerge" CssClass="btn mb-3 btn-primary w-100" Text="Select Agent To Merge" runat="server" OnClick="btnSelectAgentToMerge_Click" />

                        <div class="fs-2 text-danger my-2 text-center">
                            <i class="mdi mdi-chevron-right"></i>
                            <i class="mdi mdi-chevron-right"></i>
                            <i class="mdi mdi-chevron-right"></i>
                            <i class="mdi mdi-chevron-right"></i>
                        </div>
                        <asp:Button ID="btnMerge" CssClass="btn mb-3 btn-outline-primary w-100" Text="Merge" runat="server" OnClick="MergeAgents" />

                        <asp:Button ID="btnReset" CssClass="btn mb-3 btn-outline-primary w-100" Text="Reset" runat="server" OnClick="btnReset_Click" />
                        <asp:Button ID="btnExit" CssClass="btn btn-outline-primary w-100" Text="Exit" runat="server" OnClick="ExitButton_Click" />
                        <asp:Label ID="lblAgentCodes" runat="server" Text="" />

                    </div>
                </div>
            </div>

            <div class="col-md-4">
                <div class="card" style="height: 480px;">
                    <div class="card-body">
                        <h4 class="card-title">Selected List of Agent</h4>
                        <h5 class="mb-3">Main</h5>
                        <div class="table-responsive mb-5" style="overflow-y: auto; overflow-x: auto; max-height: 115px;">
                            <asp:GridView ID="mainAgentsGrid" CssClass="table table-hover" runat="server">
                                <Columns>
                                    <asp:TemplateField HeaderText="Agent Name" ItemStyle-CssClass="column-width-100">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAgentNameMain" runat="server" Text='<%# Eval("Agent_Name") %>' Visible="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Exist Code" ItemStyle-CssClass="column-width-100">
                                        <ItemTemplate>
                                            <asp:Label ID="lblExistCodeMain" runat="server" Text='<%# Eval("Exist_Code") %>' Visible="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Address 1" ItemStyle-CssClass="column-width-100">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAddress1Main" runat="server" Text='<%# Eval("Address1") %>' Visible="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Address 2" ItemStyle-CssClass="column-width-100">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAddress2Main" runat="server" Text='<%# Eval("Address2") %>' Visible="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                        <h5 class="mb-3">Agents To Merge</h5>
                        <div class="table-responsive" style="overflow-y: auto; overflow-x: auto; max-height: 180px;">
                            <asp:GridView ID="agentsToMergeGrid" CssClass="table table-hover" runat="server">
                                <Columns>

                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="chkHeaderRightDown" runat="server" AutoPostBack="True" OnCheckedChanged="chkHeaderRightDown_CheckedChanged" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkSelectRightDown" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Agent Name" ItemStyle-CssClass="column-width-100">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAgentNameToMerge" runat="server" Text='<%# Eval("Agent_Name") %>' Visible="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Exist Code" ItemStyle-CssClass="column-width-100">
                                        <ItemTemplate>
                                            <asp:Label ID="lblExistCodeToMerge" runat="server" Text='<%# Eval("Exist_Code") %>' Visible="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Address 1" ItemStyle-CssClass="column-width-100">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAddress1ToMerge" runat="server" Text='<%# Eval("Address1") %>' Visible="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Address 2" ItemStyle-CssClass="column-width-100">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAddress2ToMerge" runat="server" Text='<%# Eval("Address2") %>' Visible="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>


                        </div>

                    </div>
                        <asp:Button ID="btnRemoveSelected" runat="server" Text="Remove Selected" OnClick="btnRemoveSelected_Click" CssClass="btn btn-danger" />
                </div>
            </div>
        </div>
    </div>

</asp:Content>
