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
                                    <asp:ListItem Value="">Select branch first</asp:ListItem>
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
                                <asp:Button ID="btnSearch" CssClass="w-100 btn btn-primary" Text="Search" runat="server" OnClientClick="openModalAnameringModel(); return false;" />
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
                        <div class="table-responsive" style="overflow-y: auto; overflow-x: auto; max-height: 300px;">
                            <asp:GridView ID="agentsGrid" CssClass="table table-hover" runat="server" AutoGenerateColumns="false">
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

                                    <asp:TemplateField HeaderText="Branch Code" ItemStyle-CssClass="column-width-100">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSourceCodeSearched" runat="server" Text='<%# Eval("SourceID") %>' Visible="true" />
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
                            <asp:GridView ID="mainAgentsGrid" CssClass="table table-hover" runat="server" AutoGenerateColumns="false">
                                <Columns>
                                    <asp:TemplateField HeaderText="Agent Name" ItemStyle-CssClass="column-width-100">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAgentNameMain" runat="server" Text='<%# Eval("Agent_Name") %>' Visible="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Agent Code" ItemStyle-CssClass="column-width-100">
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

                                    <asp:TemplateField HeaderText="Branch Code" ItemStyle-CssClass="column-width-100">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSourceCodeMain" runat="server" Text='<%# Eval("sourceid") %>' Visible="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                       
                            
                        </div>
                        <h5 class="mb-3">Agents To Merge</h5>
                        <div class="table-responsive" style="overflow-y: auto; overflow-x: auto; max-height: 180px;">
                            <asp:GridView ID="agentsToMergeGrid" CssClass="table table-hover" runat="server" AutoGenerateColumns="false">
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

                                    <asp:TemplateField HeaderText="Agent Code" ItemStyle-CssClass="column-width-100">
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

                                    <asp:TemplateField HeaderText="Branch Code" ItemStyle-CssClass="column-width-100">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSourceCodeToMerge" runat="server" Text='<%# Eval("sourceid") %>' Visible="true" />
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


    <script>
        // Function to open the Anamering modal and store its state
        function openModalAnameringModel() {
            document.getElementById("AnameringModel").style.display = "block";
            localStorage.setItem("AnameringModelOpen", "true");
        }

        // Function to close the Anamering modal and update its state
        function closeModalAnameringModel() {
            document.getElementById("AnameringModel").style.display = "none";
            localStorage.setItem("AnameringModelOpen", "false");
        }

        // Check localStorage on page load to keep the Anamering modal open if it was open before page reload
        window.onload = function () {
            if (localStorage.getItem("AnameringModelOpen") === "true") {
                document.getElementById("AnameringModel").style.display = "block";
            }
        };
    </script>

    <style>
        /* Modal styles */
        .modal {
            display: none;
            position: fixed;
            z-index: 99;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            overflow: auto;
            background-color: rgb(0,0,0);
            background-color: rgba(0,0,0,0.4);
        }

        .modal-content {
            background-color: #fefefe;
            margin: 8% auto;
            padding: 20px;
            border: 1px solid #888;
            width: 80%;
        }

        .close {
            color: #aaa;
            float: right;
            font-size: 28px;
            font-weight: bold;
        }
    </style>


    <div id="AnameringModel" class="modal">
        <div class="modal-content">
            <h2 class="page-title">ANA Merging Search</h2>

            <div class="row g-3 mt-2">
                <!-- Category Dropdown -->
                <div class="col-md-3">
                    <label for="anameringCategory" class="form-label">Category</label>
                    <asp:DropDownList ID="anameringCategory" runat="server" CssClass="form-select" Enabled="false">
                        <asp:ListItem Text="Select" Value="" />
                        <asp:ListItem Text="Investor" Value="investor" />
                        <asp:ListItem Text="Client" Value="client" />
                        <asp:ListItem Text="Agent" Value="agent" Selected="True" />

                    </asp:DropDownList>
                </div>

                <!-- Branch Dropdown -->
                <div class="col-md-3">
                    <label for="anameringBranch" class="form-label">Branch</label>
                    <asp:DropDownList ID="anameringBranch" runat="server" CssClass="form-select" AutoPostBack="True" OnSelectedIndexChanged="ddlSourceIDAnameringBranch_SelectedIndexChanged">
                        <asp:ListItem Text="Select" Value="" />
                    </asp:DropDownList>
                </div>

                <!-- City Dropdown -->
                <div class="col-md-3">
                    <label for="anameringCity" class="form-label">City</label>
                    <asp:DropDownList ID="anameringCity" runat="server" CssClass="form-select">
                        <asp:ListItem Text="Select" Value="" />
                    </asp:DropDownList>
                </div>

                <!-- RB Dropdown -->
                <div class="col-md-3">
                    <label for="anameringRm" class="form-label">RM</label>
                    <asp:DropDownList ID="anameringRm" runat="server" CssClass="form-select">
                        <asp:ListItem Text="Select branch first" Value="" />
                    </asp:DropDownList>
                </div>

                <!-- Old RM Dropdown -->
                <div class="col-md-3">
                    <label for="anameringOldRm" class="form-label">Old RM</label>
                    <asp:DropDownList ID="anameringOldRm" runat="server" CssClass="form-select" Enabled="false">
                        <asp:ListItem Text="Select" Value="" />
                    </asp:DropDownList>
                </div>



                <!-- Textboxes -->
                <div class="col-md-5">
                    <label for="anameringName" class="form-label">Name</label>
                    <asp:TextBox ID="anameringName" runat="server" CssClass="form-control" MaxLength="50" />
                </div>

                <div class="col-md-4">
                    <label for="anameringClientCode" class="form-label">Exist Code</label>
                    <asp:TextBox ID="anameringClientCode" runat="server" CssClass="form-control" MaxLength="20" />
                </div>

                <div class="col-md-3">
                    <label for="anameringOldCode" class="form-label">Agent Code</label>
                    <asp:TextBox ID="anameringOldCode" runat="server" CssClass="form-control" MaxLength="20" />
                </div>

                <div class="col-md-5">
                    <label for="anameringAddress1" class="form-label">Address 1</label>
                    <asp:TextBox ID="anameringAddress1" runat="server" CssClass="form-control" MaxLength="150" />
                </div>

                <div class="col-md-4">
                    <label for="anameringAddress2" class="form-label">Address 2</label>
                    <asp:TextBox ID="anameringAddress2" runat="server" CssClass="form-control" MaxLength="150" />
                </div>

                <div class="col-md-3">
                    <label for="anameringPan" class="form-label">PAN</label>
                    <asp:TextBox ID="anameringPan" runat="server" CssClass="form-control" MaxLength="10" />
                </div>

                <div class="col-md-3">
                    <label for="anameringPhone" class="form-label">Phone</label>
                    <asp:TextBox ID="anameringPhone" runat="server" CssClass="form-control"
                        MaxLength="12"
                        oninput="validatePhoneNumber(this);" />
                </div>

                <script type="text/javascript">
                    function validatePhoneNumber(input) {
                        // Allow only numbers and hyphens
                        input.value = input.value.replace(/[^0-9\-]/g, '');

                        // Optional: Limit length to 12 characters (example for phone numbers like "123-456-7890")
                        if (input.value.length > 12) {
                            input.value = input.value.substring(0, 12);
                        }
                    }

                    function validateMobileNumber(input) {
                        // Allow only numbers
                        input.value = input.value.replace(/[^0-9]/g, '');

                        // Optional: Limit length to 12 characters (example for phone numbers like "123-456-7890")
                        if (input.value.length > 10) {
                            input.value = input.value.substring(0, 10);
                        }
                    }
                </script>


                <div class="col-md-2">
                    <label for="anameringMobile" class="form-label">Mobile</label>
                    <asp:TextBox ID="anameringMobile" runat="server" CssClass="form-control" MaxLength="10" oninput="validateMobileNumber(this);" />
                </div>
                <!-- Sort By Dropdown -->
                <div class="col-md-4">
                    <label for="anameringSortBy" class="form-label">Sort By</label>
                    <asp:DropDownList ID="anameringSortBy" runat="server" CssClass="form-select">
                        <asp:ListItem Text="Select" Value="" />
                        <asp:ListItem Text="Name" Value="name" />
                        <asp:ListItem Text="Address 1" Value="address1" />
                        <asp:ListItem Text="Address 2" Value="address2" />
                        <asp:ListItem Text="City" Value="city" />
                        <asp:ListItem Text="Phone" Value="phone" />
                    </asp:DropDownList>
                </div>

                <!-- Buttons -->
                <div class="col-md-12 d-flex justify-content-between">
                    <!-- Add button on the left side -->
                    <asp:Button ID="btnAddSelectedRows" runat="server" CssClass="btn btn-primary" Text="Add Selected Rows" OnClick="btnAddSelectedRows_Click" />

                    <!-- Other buttons (Show, Reset, Exit) aligned to the right side -->
                    <div class="d-flex gap-2">
                        <asp:Button ID="anameringShowButton" runat="server" CssClass="btn btn-primary" Text="Show" OnClick="btnSearch_Click" />
                        <asp:Button ID="anameringResetButton" runat="server" CssClass="btn btn-secondary" Text="Reset" OnClick="btnModelReset_Click" />
                        <asp:Button ID="anameringExitButton" runat="server" CssClass="btn btn-outline-primary" Text="Exit" OnClientClick="closeModalAnameringModel(); return false;" />
                    </div>
                </div>

            </div>

            <div class="card-body">
                <asp:Label ID="lblAgentCodeSearchedMasterInfo" CssClass="mt-3 mb-2" runat="server" Text='' Visible="true" />

                <div class="table-responsive " style="overflow-y: auto; overflow-x: auto; max-height: 300px;">
                    <asp:GridView ID="agentsGridSearchedMaster" AutoGenerateColumns="false" CssClass="table table-hover" runat="server">
                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:CheckBox ID="chkHeaderSearchedMaster" runat="server" AutoPostBack="True" OnCheckedChanged="chkHeaderSearchedMaster_CheckedChanged" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkSelectSearchedMaster" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Agent Code" ItemStyle-CssClass="column-width-100">
                                <ItemTemplate>
                                    <asp:Label ID="lblAgentCodeSearchedMaster" runat="server" Text='<%# Eval("AGENT_CODE") %>' Visible="true" />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Exist Code" ItemStyle-CssClass="column-width-100">
                                <ItemTemplate>
                                    <asp:Label ID="lblExistCodeSearchedMaster" runat="server" Text='<%# Eval("EXIST_CODE") %>' Visible="true" />
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="Agent Name" ItemStyle-CssClass="column-width-100">
                                <ItemTemplate>
                                    <asp:Label ID="lblAgentNameSearchedMaster" runat="server" Text='<%# Eval("AGENT_NAME") %>' Visible="true" />
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="Address 1" ItemStyle-CssClass="column-width-100">
                                <ItemTemplate>
                                    <asp:Label ID="lblAddress1SearchedMaster" runat="server" Text='<%# Eval("ADDRESS1") %>' Visible="true" />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Address 2" ItemStyle-CssClass="column-width-100">
                                <ItemTemplate>
                                    <asp:Label ID="lblAddress2SearchedMaster" runat="server" Text='<%# Eval("ADDRESS2") %>' Visible="true" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Branch Code" ItemStyle-CssClass="column-width-100">
                                <ItemTemplate>
                                    <asp:Label ID="lblSourceCodeSearchedMaster" runat="server" Text='<%# Eval("SOURCEID") %>' Visible="true" />
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>
                    </asp:GridView>
                </div>

                <script type="text/javascript">
                    document.addEventListener("DOMContentLoaded", function () {
                        let grid = document.getElementById("<%= agentsGridSearchedMaster.ClientID %>");

                                if (grid) {
                                    let checkboxes = grid.querySelectorAll("[id*=chkSelectSearchedMaster]");
                                    let headerCheckbox = document.querySelector("[id*=chkHeaderSearchedMaster]");

                                    // Add event listener for each row checkbox
                                    checkboxes.forEach(chk => {
                                        chk.addEventListener("change", function () {
                                            validateSelectedAgents();
                                        });
                                    });

                                    // Add event listener for the header (Select All) checkbox
                                    if (headerCheckbox) {
                                        headerCheckbox.addEventListener("change", function () {
                                            checkboxes.forEach(chk => chk.checked = this.checked);
                                            validateSelectedAgents();
                                        });
                                    }
                                }
                            });

                    function validateSelectedAgents() {
                        let grid = document.getElementById("<%= agentsGridSearchedMaster.ClientID %>");
                        let checkboxes = grid.querySelectorAll("[id*=chkSelectSearchedMaster]");
                        let firstSourceID = null;
                        let isDifferentSourceID = false;

                        checkboxes.forEach(chk => {
                            if (chk.checked) {
                                let row = chk.closest("tr");
                                let sourceID = row.querySelector("[id*=lblSourceCodeSearchedMaster]").innerText.trim();

                                if (firstSourceID === null) {
                                    firstSourceID = sourceID; // Store the first selected SourceID
                                } else if (firstSourceID !== sourceID) {
                                    isDifferentSourceID = true;
                                }
                            }
                        });

                        if (isDifferentSourceID) {
                            alert("All the selected agents should be of the same branch");

                            // If the Select All checkbox was clicked, uncheck all checkboxes
                            let headerCheckbox = document.querySelector("[id*=chkHeaderSearchedMaster]");
                            if (headerCheckbox && headerCheckbox.checked) {
                                checkboxes.forEach(chk => chk.checked = false);
                                headerCheckbox.checked = false; // Also uncheck the Select All checkbox
                            } else {
                                // If a single row was selected wrongly, uncheck only that checkbox
                                checkboxes.forEach(chk => {
                                    let row = chk.closest("tr");
                                    let sourceID = row.querySelector("[id*=lblSourceCodeSearchedMaster]").innerText.trim();
                                    if (firstSourceID !== null && sourceID !== firstSourceID) {
                                        chk.checked = false;
                                    }
                                });
                            }
                        }
                    }
                </script>
                <p class="text-danger">Select client from this data sheet to include it into the data sheets on the left side.</p>
            </div>
        </div>
    </div>



</asp:Content>