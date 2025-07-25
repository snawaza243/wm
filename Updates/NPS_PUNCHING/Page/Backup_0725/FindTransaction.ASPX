<%@ Page Title="Transaction Search" Language="C#" MasterPageFile="~/vmSite.Master" AutoEventWireup="true" CodeBehind="FindTransaction.aspx.cs" Inherits="WM.Tree.FindTransaction" EnableViewState="true" MaintainScrollPositionOnPostback="true" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <link rel="shortcut icon" href="../assets/images/favicon.png" />
    <link rel="stylesheet" href="../assets/vendors/mdi/css/materialdesignicons.min.css">
    <link rel="stylesheet" href="../assets/vendors/ti-icons/css/themify-icons.css">
    <link rel="stylesheet" href="../assets/vendors/css/vendor.bundle.base.css">
    <link rel="stylesheet" href="../assets/vendors/font-awesome/css/font-awesome.min.css">
    <link rel="stylesheet" href="../assets/vendors/bootstrap-datepicker/bootstrap-datepicker.min.css">
    <link rel="stylesheet" href="../assets/css/style.css">


    <%-- CURSSOR WAIT AND FREE --%>
    <script type="text/javascript">
        function showLoadingCursor() {
            document.body.style.cursor = 'wait';
        }

        function resetCursor() {
            document.body.style.cursor = 'default';
        }
    </script>

    <%-- Number input validation --%>
    <script type="text/javascript">
        // Function to trim the input if it exceeds the max length
        function trimInput(inputElement, maxLength) {
            if (inputElement.value.length > maxLength) {
                inputElement.value = inputElement.value.substring(0, maxLength);
            }
        }

        // Function to ensure only numeric values are allowed while typing
        function validateNumericInput(inputElement) {
            // Replace all non-numeric characters with empty string
            inputElement.value = inputElement.value.replace(/\D/g, '');
        }

        // Function to prevent pasting of non-numeric characters
        function validatePaste(event, inputElement) {
            // Get pasted data
            var pastedData = (event.clipboardData || window.clipboardData).getData('text');

            // Only allow numbers in the pasted data
            if (/\D/.test(pastedData)) {
                event.preventDefault(); // Prevent paste if it's not a number
            }
        }
    </script>

    <!-- Loader HTML, CSS, JS -->
    <div id="serverLoader" class="loader-overlay" style="display: none;">
        <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Loading...</span>
        </div>
    </div>
    <style>
        .loader-overlay {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0, 0, 0, 0.5);
            display: flex;
            align-items: center;
            justify-content: center;
            z-index: 9999; /* Ensure it's above other elements */
        }
    </style>
    <script type="text/javascript">
        function showServerLoader() {
            document.getElementById('serverLoader').style.display = 'flex';
        }

        function hideServerLoader() {
            document.getElementById('serverLoader').style.display = 'none';
        }

        var lastFocusedElement;

        // Function to store focus before postback
        function storeFocus() {
            lastFocusedElement = document.activeElement;
            if (lastFocusedElement) {
                document.getElementById('<%= hfFocusedControl.ClientID %>').value = lastFocusedElement.id;
     }
 }

 // Function to restore focus after update
 function restoreFocus() {
            var focusedControlId = document.getElementById('<%= hfFocusedControl.ClientID %>').value;
            if (focusedControlId) {
                var element = document.getElementById(focusedControlId);
                if (element) {
                    element.focus();
                }
            }
        }


        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_beginRequest(function () {
            showServerLoader();
            storeFocus();

        });

        prm.add_endRequest(function () {
            hideServerLoader();
            restoreFocus();
        });
    </script>

   
    <%-- Default date setting --%>
    <script>
        $(document).ready(function () {
            $('.date').datepicker({
                format: 'dd/mm/yyyy',
                autoclose: true,
                todayHighlight: true
            });
        });
    </script>

    <%-- Date input validation dd/mm/yyyy --%>
    <script>
        function formatAndValidateDate(input) {
            // Remove any non-numeric or non-slash characters
            let value = input.value.replace(/[^0-9]/g, '');

            // Insert '/' automatically after DD and MM
            if (value.length >= 2 && value.charAt(2) !== '/') {
                value = value.slice(0, 2) + '/' + value.slice(2);
            }
            if (value.length >= 5 && value.charAt(5) !== '/') {
                value = value.slice(0, 5) + '/' + value.slice(5);
            }

            // Restrict maximum length to 10 characters (dd/mm/yyyy)
            input.value = value.substring(0, 10);
        }
    </script>


     <asp:HiddenField ID="hfFocusedControl" runat="server" />


    <div class="row">
        <div class="grid-margin stretch-card">
            <div class="card">
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-6">
                            <asp:Label ID="lblFormTitle" runat="server" CssClass="card-title text-primary" Text="AR Search" />

                        </div>
                        <div class="col-md-6 text-end">
                            <%--<asp:Button ID="associateMasterViewModel" Enabled="false" CssClass="btn btn-primary" data-toggle="modal" data-target="#associateListModal" runat="server" Text="View" OnClientClick="openAssociateListModal(); return false;" />--%>
                        </div>
                    </div>

                    <asp:UpdatePanel ID="updMain" runat="server">
                        <ContentTemplate>
                            <div class="row">
                                <div class="row g-3">

                                    <%-- ar no --%>
                                    <asp:Panel CssClass="col-md-2" ID="pnlARNo" runat="server" Visible="true">
                                        <asp:Label ID="lblARNo" runat="server" Text="AR No:"></asp:Label>
                                        <asp:TextBox ID="txtARNo" runat="server" CssClass="form-control" MaxLength="12" />

                                    </asp:Panel>

                                    <%-- ddl schemas --%>
                                    <asp:Panel CssClass="col-md-2" ID="pnlScheme" runat="server">
                                        <asp:Label ID="lblScheme" runat="server" Text="Scheme:"></asp:Label>
                                        <asp:DropDownList ID="ddlSchemas" runat="server" CssClass="form-select">
                                            <asp:ListItem Text="Scheme" Value="" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="New Pension Scheme Tier 1" Value="OP#09971"></asp:ListItem>
                                            <asp:ListItem Text="New Pension Scheme Tier 2" Value="OP#09972"></asp:ListItem>
                                            <asp:ListItem Text="New Pension Scheme Tier 1+2" Value="OP#09973"></asp:ListItem>
                                        </asp:DropDownList>
                                    </asp:Panel>

                                    <%-- cheque --%>
                                    <asp:Panel CssClass="col-md-2" ID="pnlChequeNo" runat="server">
                                        <asp:Label ID="lblChequeNo" runat="server" Text="Cheque No:"></asp:Label>
                                        <asp:TextBox ID="txtChequeNo" runat="server" CssClass="form-control" MaxLength="20" />
                                    </asp:Panel>

                                    <%-- app no --%>
                                    <asp:Panel CssClass="col-md-2" ID="pnlAppNo" runat="server">
                                        <asp:Label ID="lblAppNo" runat="server" Text="App No:"></asp:Label>
                                        <asp:TextBox ID="txtAppNo" runat="server" CssClass="form-control" MaxLength="50" />
                                    </asp:Panel>

                                    <%-- pran no --%>
                                    <asp:Panel CssClass="col-md-2" ID="pnlPranNo" runat="server">
                                        <asp:Label ID="lblPranNo" runat="server" Text="PRAN No:"></asp:Label>
                                        <asp:TextBox ID="txtPranNo" runat="server" CssClass="form-control"
                                            MaxLength="12"
                                            oninput="trimInput(this, 12); validateNumericInput(this);"
                                            onpaste="validatePaste(event, this);" />
                                    </asp:Panel>

                                    <%-- ana code --%>
                                    <asp:Panel CssClass="col-md-2" ID="pnlAnaExistCode" runat="server">
                                        <asp:Label ID="lblAnaExistCode" runat="server" Text="ANA Exist Code:"></asp:Label>
                                        <asp:TextBox ID="txtAnaExistCode" runat="server" CssClass="form-control" MaxLength="8" />
                                    </asp:Panel>

                                    <%-- name --%>
                                    <asp:Panel CssClass="col-md-4" ID="pnlInvName" runat="server">
                                        <asp:Label ID="lblInvName" runat="server" Text="Investor Name:"></asp:Label>
                                        <asp:TextBox ID="txtInvName" runat="server" CssClass="form-control" MaxLength="50" />

                                    </asp:Panel>
                                   
                                    <%-- from date --%>
                                    <asp:Panel CssClass="col-md-2" ID="pnlArFromDate" runat="server">
                                        <asp:Label ID="lblArFromDate" runat="server" Text="AR From Date:"></asp:Label>
                                        <div class="input-group date" id="arPopFromDatePicker" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                            <asp:TextBox ID="txtArFromDate" runat="server" CssClass="form-control" placeholder="dd/mm/yyyy" MaxLength="10" oninput="formatAndValidateDate(this);" />
                                            <div class="input-group-append">
                                                <span class="input-group-text">
                                                    <i class="glyphicon glyphicon-calendar"></i>
                                                </span>
                                            </div>
                                        </div>

                                    </asp:Panel>

                                    <%-- to date --%>
                                    <asp:Panel CssClass="col-md-2" ID="pnlArToDate" runat="server">
                                        <asp:Label ID="lblArToDate" runat="server" Text="AR To Date:"></asp:Label>
                                        <div class="input-group date" id="arPopToDatePicker" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                            <asp:TextBox ID="txtArToDate" runat="server" CssClass="form-control" placeholder="dd/mm/yyyy" MaxLength="10" oninput="formatAndValidateDate(this);" />
                                            <div class="input-group-append">
                                                <span class="input-group-text">
                                                    <i class="glyphicon glyphicon-calendar"></i>
                                                </span>
                                            </div>
                                        </div>

                                    </asp:Panel>

                                    <%-- deposit --%>
                                    <asp:Panel CssClass="col-md-2" ID="pnlDeposit" runat="server">
                                        <asp:Label ID="lblDeposit" runat="server" Text="Deposit:"></asp:Label>
                                        <div class="mt-3 d-flex justify-content-center gap-3">
                                            <asp:RadioButton ID="rbtDepositBefore" runat="server" GroupName="Deposit" Text="Before" />
                                            <asp:RadioButton ID="rbtDepositAfter" CssClass="ml-2" runat="server" GroupName="Deposit" Text="After" Checked="true" />
                                        </div>
                                    </asp:Panel>

                                    <%-- button --%>
                                    <asp:Panel CssClass="col-md-12" ID="pnlButtons" runat="server">
                                        <div class="text-end mt-1 gap-2 flex-wrap">

                                            <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-primary"
                                                Text="Search"
                                                OnClick="btnSearch_Click"
                                                OnClientClick="showServerLoader(); return true;" />

                                            <asp:Button ID="btbReset" runat="server" CssClass="btn btn-outline-primary"
                                                Text="Reset"
                                                OnClick="btnpReset_Click"
                                                OnClientClick="showServerLoader(); return true;" />

                                            <asp:Button ID="btnExit" runat="server" CssClass="btn btn-outline-primary"
                                                Text="Exit"
                                                OnClick="btnpExit_Click"
                                                OnClientClick="showServerLoader(); return true;" />
                                        </div>
                                    </asp:Panel>


                                </div>

                               
        
   
                                <script type="text/javascript">
                                    function onGridRowDoubleClick(row) {
                                        // Find the LinkButton inside the clicked row
                                        // The LinkButton is rendered as <a> or <input type="submit"> depending on setup
                                        // We can search for a control with CommandName='SelectRow' by looking for the text or id pattern

                                        // Option 1: If rendered as <a> with text "Select"
                                        var selectBtn = null;

                                        // Find all anchor tags in the row
                                        var anchors = row.getElementsByTagName("a");
                                        for (var i = 0; i < anchors.length; i++) {
                                            if (anchors[i].textContent.trim() === "Select") {
                                                selectBtn = anchors[i];
                                                break;
                                            }
                                        }

                                        if (selectBtn) {
                                            selectBtn.click(); // Programmatically click the LinkButton
                                            __doPostBack(selectBtn.id, ''); // Trigger postback
                                        } else {
                                            alert("Select button not found in this row.");
                                        }
                                    }
                                </script>

                          

                                <div class="row g-3 ">
                                    <h5 class="card-title m-0 p-0">AR Details</h5>
                                    <asp:Label ID="ARListlblMessage" runat="server" Text="" />
                                    <div class="table-responsive m-2" style="overflow-y: auto; overflow-x: auto; height: 350px;">
                                        <asp:GridView ID="ARListARGridView" runat="server" CssClass="table table-hover"
                                            AutoGenerateColumns="false"
                                            AllowPaging="True"
                                            PageSize="50"
                                            OnRowDataBound="gvResults_RowDataBound"

                                            OnPageIndexChanging="PaginationARListARGridView_PageIndexChanging"
                                            OnRowCommand="gvARSearch_RowCommand">
                                            <Columns>


                                                <asp:TemplateField HeaderText="Action">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="ARListlnkButton" runat="server" CommandName="SelectRow" CommandArgument='<%# Eval("TRAN_CODE") %>' Text="Select" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Transaction Code">
                                                    <ItemTemplate>
                                                        <asp:Label ID="arPopLblTransCode" runat="server" Text='<%# Eval("TRAN_CODE") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Transaction Date">
                                                    <ItemTemplate>
                                                        <asp:Label ID="arPopLblTransDate" runat="server" Text='<%# Eval("TR_DATE1") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Client Code">
                                                    <ItemTemplate>
                                                        <asp:Label ID="arPopLblClientCode" runat="server" Text='<%# Eval("CLIENT_CODE") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Client Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="arPopLblClientName" runat="server" Text='<%# Eval("CLIENT_NAME") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Receipt No">
                                                    <ItemTemplate>
                                                        <asp:Label ID="arPopLblReceiptNo" runat="server" Text='<%# Eval("UNIQUE_ID") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Mutual Fund">
                                                    <ItemTemplate>
                                                        <asp:Label ID="arPopLblMutCode" runat="server" Text='<%# Eval("MUT_CODE1") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Scheme">
                                                    <ItemTemplate>
                                                        <asp:Label ID="arPopLblSchCode" runat="server" Text='<%# Eval("SCH_CODE1") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Application Number">
                                                    <ItemTemplate>
                                                        <asp:Label ID="arPopLblAppNo" runat="server" Text='<%# Eval("APP_NO") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Folio Number">
                                                    <ItemTemplate>
                                                        <asp:Label ID="arPopLblFolioNo" runat="server" Text='<%# Eval("FOLIO_NO") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Transaction Type">
                                                    <ItemTemplate>
                                                        <asp:Label ID="arPopLblTransType" runat="server" Text='<%# Eval("TRAN_TYPE") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>

                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="btbReset" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="btnExit" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

