<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MfManualReconciliation.aspx.cs" Inherits="WM.Masters.MfManualReconciliation" MasterPageFile="~/vmSite.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Stylesheets -->
    <link rel="stylesheet" href="assets/vendors/mdi/css/materialdesignicons.min.css" />
    <link rel="stylesheet" href="assets/vendors/ti-icons/css/themify-icons.css" />
    <link rel="stylesheet" href="assets/vendors/css/vendor.bundle.base.css" />
    <link rel="stylesheet" href="assets/vendors/font-awesome/css/font-awesome.min.css" />
    <link rel="stylesheet" href="assets/vendors/bootstrap-datepicker/bootstrap-datepicker.min.css" />
    <link rel="stylesheet" href="assets/css/style.css" />
    <style>
        /* .form-check-input {
            width: 5rem;
        }*/
    </style>

    <div class="page-header">
        <h3 class="page-title">MF AR Reconciliation </h3>
    </div>

    <asp:UpdatePanel ID="UpdatePanelFirst" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="row">
                <div class="grid-margin stretch-card">
                    <div class="card">
                        <div class="card-body">
                            <h4 class="card-title text-danger">WealthMaker Transactions
                            </h4>
                            <div class="row g-3">
                                <div class="col-md-4">
                                    <div class="row g-3">
                                        <div class="col-md-6">
                                            <asp:Label ID="LabelChannel" runat="server" Text="Channel" CssClass="form-label"></asp:Label>
                                            <asp:DropDownList ID="ddlChannel" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlBranchCategory_SelectedIndexChanged">
                                                <asp:ListItem>Channel 1</asp:ListItem>
                                                <asp:ListItem>Channel 2</asp:ListItem>
                                                <asp:ListItem>Channel 3</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-6">
                                            <asp:Label ID="LabelRegion" runat="server" Text="Region" CssClass="form-label"></asp:Label>
                                            <asp:DropDownList ID="ddlRegion" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlRegion_SelectedIndexChanged">
                                                <asp:ListItem>Region 1</asp:ListItem>
                                                <asp:ListItem>Region 2</asp:ListItem>
                                                <asp:ListItem>Region 3</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-6">
                                            <asp:Label ID="LabelZone" runat="server" Text="Zone" CssClass="form-label"></asp:Label>
                                            <asp:DropDownList ID="ddlZone" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlzone_SelectedIndexChanged">
                                                <asp:ListItem></asp:ListItem>
                                                <asp:ListItem></asp:ListItem>
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-6">
                                            <asp:Label ID="LabelBranch" runat="server" Text="Branch" CssClass="form-label"></asp:Label>
                                            <asp:DropDownList ID="ddlBranch" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlRmFill_SelectedIndexChanged">
                                                <asp:ListItem></asp:ListItem>
                                                <asp:ListItem></asp:ListItem>
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-12">
                                            <asp:Label ID="LabelRM" runat="server" Text="RM" CssClass="form-label"></asp:Label>
                                            <asp:DropDownList ID="ddlRM" runat="server" CssClass="form-select">
                                                <asp:ListItem></asp:ListItem>
                                                <asp:ListItem></asp:ListItem>
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-5">
                                    <div class="row g-3">
                                        <div class="col-md-6">
                                            <asp:Label ID="LabelDateFrom" runat="server" Text="Date From" CssClass="form-label"></asp:Label>
                                            <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                <asp:TextBox ID="txtDateFrom" runat="server" CssClass="form-control date-input" Placeholder="dd/mm/yyyy"></asp:TextBox>
                                                <div class="input-group-addon">
                                                    <span class="glyphicon glyphicon-th"></span>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <asp:Label ID="LabelDateTo" runat="server" Text="Date To" CssClass="form-label"></asp:Label>
                                            <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                <asp:TextBox ID="txtDateTo" runat="server" CssClass="form-control date-input" Placeholder="dd/mm/yyyy"></asp:TextBox>
                                                <div class="input-group-addon">
                                                    <span class="glyphicon glyphicon-th"></span>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <asp:Label ID="LabelAMC" runat="server" Text="AMC" CssClass="form-label"></asp:Label>
                                            <asp:DropDownList ID="ddlAMC" runat="server" CssClass="form-select">
                                                <asp:ListItem>AMC 1</asp:ListItem>
                                                <asp:ListItem>AMC 2</asp:ListItem>
                                                <asp:ListItem>AMC 3</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-6" style="display: none;">
                                            <asp:Label ID="LabelAR" runat="server" Text="AR" CssClass="form-label"></asp:Label>
                                            <asp:TextBox ID="txtAR" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                        <div>
                                            <div class="d-flex align-items-center gap-3">
                                                <asp:RadioButtonList ID="rblReconciliation" runat="server" RepeatDirection="Horizontal" CssClass="form-check">
                                                    <asp:ListItem Text="Reconciled" Value="Y" />
                                                    <asp:ListItem Text="Unreconciled" Value="N" Selected="True" />
                                                </asp:RadioButtonList>


                                                <div class="form-check">
                                                    <asp:CheckBox ID="cbCOB" runat="server" Text="COB" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-3">
                                    <asp:Label ID="LabelARNo" runat="server" Text="AR No." CssClass="form-label"></asp:Label>
                                    <asp:TextBox ID="txtARNo" runat="server" CssClass="form-control"></asp:TextBox>
                                    <div class="d-flex align-items-center flex-wrap gap-3 mt-3">
                                        <asp:Button ID="btnGo" runat="server" Text="Go" CssClass="btn btn-sm btn-primary" OnClick="btnGo_Click" />
                                        <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-sm btn-outline-primary" OnClick="btnReset_Click" />
                                        <asp:Button ID="btnExport" runat="server" Text="Export" CssClass="btn btn-sm btn-outline-primary" OnClick="btnExport_Click" />
                                    </div>
                                    <p class="text-danger fw-bold mt-4">
                                        Count:
                                <asp:Label ID="lblRowCount" runat="server" Text="0"></asp:Label>
                                    </p>
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Tran Type</label>
                                    <asp:RadioButtonList ID="rblTranType" runat="server" RepeatDirection="Horizontal" CssClass="form-check">
                                        <asp:ListItem Text="Regular" Value="regular" Selected="True" />
                                        <asp:ListItem Text="PMS" Value="pms" />
                                        <asp:ListItem Text="ATM" Value="atm" />
                                        <asp:ListItem Text="Trail Actual" Value="trailActual" />
                                        <asp:ListItem Text="NFO" Value="sip" />
                                    </asp:RadioButtonList>
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Registrar</label>
                                    <asp:RadioButtonList ID="rblRegistrar" runat="server" RepeatDirection="Horizontal" CssClass="form-check">
                                        <asp:ListItem Text="C" Value="c" />
                                        <asp:ListItem Text="K" Value="k" />
                                        <asp:ListItem Text="C COB" Value="ccob" />
                                        <asp:ListItem Text="K COB" Value="kcob" />
                                    </asp:RadioButtonList>
                                </div>

                                <div>
                                    <br />
                                    <h5 class="card-title">List</h5>

                                     <asp:HiddenField ID="hftran1stcode" runat="server" />
                               

                                    <div class="table-responsive" id="gridContainer1" style="max-height: 300px; overflow-y: auto;">
                                        <asp:GridView ID="GridTransaction" CssClass="table table-bordered" runat="server" AutoGenerateColumns="false">

                                            <HeaderStyle CssClass="thead-dark" />
                                            <Columns>



                                                <asp:TemplateField HeaderText="Action" ItemStyle-CssClass="column-width-50">
                                                    <ItemTemplate>

                                                        <asp:HiddenField ID="hfTranCode" runat="server" Value='<%# Eval("TRAN_CODE") %>' />

                                                        <%-- <asp:CheckBox ID="chkSelect" runat="server" AutoPostBack="true" OnCheckedChanged="chkSelect_CheckedChanged" OnClientClick="scrollToGridView(this); return false;" />
                                                        --%>
                                                        <asp:CheckBox ID="chkSelect" runat="server" CssClass="select-row-checkbox" AutoPostBack="false" OnClientClick="return onRowSelect(this);" />


                                                    </ItemTemplate>
                                                </asp:TemplateField>



                                                <asp:TemplateField HeaderText="Tran Code">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTranTyp" runat="server" Text='<%# Eval("TRAN_CODE") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="TranDate">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTrnDate" runat="server" Text='<%# Eval("TR_DATE"  ,  "{0:dd/MM/yyyy}") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Investor">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblInvestorNam" runat="server" Text='<%# Eval("INVESTOR_NAME") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Address">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAddress" runat="server" Text='<%# Eval("ADDRESS1") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="City Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCityName" runat="server" Text='<%# Eval("CITY_NAME") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>



                                                <asp:TemplateField HeaderText="AMC CODE" ItemStyle-CssClass="hide-column" HeaderStyle-CssClass="hide-column">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAC" runat="server" Text='<%# Eval("MUT_CODE") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="AMC NAME">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAMCNAM" runat="server" Text='<%# Eval("MUT_NAME") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Scheme Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblScemeName" runat="server" Text='<%# Eval("SCH_NAME") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Amount">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblmount" runat="server" Text='<%# Eval("AMOUNT") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Folio No">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFoloNo" runat="server" Text='<%# Eval("FOLIO_NO") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Chq No">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCqNo" runat="server" Text='<%# Eval("CHEQUE_NO") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="App No">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAppoModify" runat="server" Text='<%# Eval("APP_NO") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Pan No" ItemStyle-CssClass="hide-column" HeaderStyle-CssClass="hide-column">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblpanNo" runat="server" Text='<%# Eval("PANNO") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="RM" ItemStyle-CssClass="hide-column" HeaderStyle-CssClass="hide-column">
                                                    <ItemTemplate>
                                                        <asp:Label ID="llRM" runat="server" Text='<%# Eval("RMCODE") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="rm_name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblScemeCode" runat="server" Text='<%# Eval("rm_name") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Branch Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblbranhname" runat="server" Text='<%# Eval("branch_name") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>


                                                <asp:TemplateField HeaderText="TranType">
                                                    <ItemTemplate>
                                                        <asp:Label ID="llTrnType" runat="server" Text='<%# Eval("TRAN_TYPE") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>


                                                <asp:TemplateField HeaderText="SIP Type">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSipType" runat="server" Text='<%# Eval("SIP_TYPE") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Flag">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblfag" runat="server" Text='<%# Eval("FLAG") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>


                                                <asp:TemplateField HeaderText="Broker Code" ItemStyle-CssClass="hide-column" HeaderStyle-CssClass="hide-column">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblbrokerCode" runat="server" Text='<%# Eval("BROKER_ID") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>













                                                <asp:TemplateField HeaderText="Sip Amount" ItemStyle-CssClass="hide-column" HeaderStyle-CssClass="hide-column">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblsipamont" runat="server" Text='<%# Eval("Sip_Amount") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>



                                                <asp:TemplateField HeaderText="Branch Code" ItemStyle-CssClass="hide-column" HeaderStyle-CssClass="hide-column">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblbranhna" runat="server" Text='<%# Eval("branch_code") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>




                                                <asp:TemplateField HeaderText="Remark">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblremarkreco" runat="server" Text='<%# Eval("remark_reco") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>







                                                <asp:TemplateField HeaderText="registrar" ItemStyle-CssClass="hide-column" HeaderStyle-CssClass="hide-column">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblregis" runat="server" Text='<%# Eval("registrar") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="COB FLAG" ItemStyle-CssClass="hide-column" HeaderStyle-CssClass="hide-column">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCOBFL" runat="server" Text='<%# Eval("COB_FLAG") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                            </Columns>
                                        </asp:GridView>
                                    </div>

                                </div>


                            </div>
                        </div>
                    </div>
                </div>

                <style>
                    .hide-column {
                        display: none;
                    }
                </style>


                <script type="text/javascript">
                    function onRowSelect(checkbox) {
                        var row = checkbox.closest("tr");
                        var grid = document.getElementById("<%= GridTransaction.ClientID %>");
                        var rows = grid.getElementsByTagName("tr");

                        // Uncheck all other checkboxes (optional now)
                        for (var i = 1; i < rows.length; i++) {
                            rows[i].classList.remove("selected-row");
                        }

                        // Highlight the selected row only
                        row.classList.add("selected-row");

                        // Set selected row index
                        document.getElementById("hfSelectedRow").value = row.rowIndex - 1;

                        // ✅ Clear form (if you have a JS version of Clearfieldsofrta, call it here)
                        clearFieldsOfRta(); // You can define this separately

                        // ✅ Extract values from selected row
                        const getText = (id) => row.querySelector("[id*='" + id + "']")?.innerText.trim() || '';
                        const getValue = (id) => row.querySelector("[id*='" + id + "']")?.value.trim() || '';

                        const tranCode = getValue("hfTranCode");
                        const tranDateStr = getText("lblTrnDate");
                        const tranType = getText("llTrnType");
                        const investorName = getText("lblInvestorNam");
                        const amcCode = getText("lblAC");
                        const branch = getText("lblbranhna");
                        const amount = getText("lblmount");
                        const sipType = getText("lblSipType");
                        const chqno = getText("lblCqNo");
                        const registrar = getText("lblregis");
                        const cobfl = getText("lblCOBFL");

                        document.getElementById("<%= hftran1stcode.ClientID %>").value = tranCode;


                        // ✅ Fill form fields
                        document.getElementById("<%= txtInvestorName.ClientID %>").value = investorName;
                        document.getElementById("<%= txtAmount.ClientID %>").value = amount;
                        document.getElementById("<%= ddlChequeNo.ClientID %>").value = "001";
                        document.getElementById("<%= txtChequeSearch.ClientID %>").value = chqno;

                        // ✅ Set registrar radio buttons
                        const registrarVal = (registrar === "C" && cobfl === "0") ? "c" :
                            (registrar === "K" && cobfl === "0") ? "k" :
                                (registrar === "C" && cobfl === "1") ? "ccob" :
                                    (registrar === "K" && cobfl === "1") ? "kcob" : "";

                        const rblRegistrar = document.getElementById("<%= rblRegistrar.ClientID %>");
                        if (rblRegistrar && registrarVal) {
                            const rb = rblRegistrar.querySelector("input[value='" + registrarVal + "']");
                            if (rb) rb.checked = true;
                        }

                        // ✅ Set reconciliation type radio
                        const recoVal = tranType.toLowerCase() === "reconciled" ? "reconciled-rta" : "unreconciled-rta";
                        const rblReco = document.getElementById("<%= rblReconciliationType.ClientID %>");
                        if (rblReco) {
                            const rb = rblReco.querySelector("input[value='" + recoVal + "']");
                            if (rb) rb.checked = true;
                        }

                        // ✅ Set SIP type radio
                        const sipVal = sipType.toLowerCase() === "regular" ? "regular" : "sip";
                        const sipList = document.getElementById("<%= RadioButtonList1.ClientID %>");
                        if (sipList) {
                            const rb = sipList.querySelector("input[value='" + sipVal + "']");
                            if (rb) rb.checked = true;
                        }

                        // ✅ Set AMC dropdown
                        const ddlAmc = document.getElementById("<%= DropDownList1.ClientID %>");
                        if (ddlAmc && ddlAmc.querySelector("option[value='" + amcCode + "']")) {
                            ddlAmc.value = amcCode;
                        } else if (ddlAmc) {
                            ddlAmc.selectedIndex = 0;
                        }

                        // ✅ Set Branch dropdown
        <%--const ddlBranch = document.getElementById("<%= DropDownList2.ClientID %>");
        if (ddlBranch && ddlBranch.querySelector("option[value='" + branch + "']")) {
            ddlBranch.value = branch;
        } else if (ddlBranch) {
            ddlBranch.selectedIndex = 0;
        }--%>

                        // ✅ Set Date Range
                        const dateFrom = document.getElementById("<%= dateFromRta.ClientID %>");
                        const dateTo = document.getElementById("<%= dateToRta.ClientID %>");

                        if (/^\d{2}\/\d{2}\/\d{4}$/.test(tranDateStr)) {
                            const [dd, mm, yyyy] = tranDateStr.split('/');
                            const originalDate = new Date(`${yyyy}-${mm}-${dd}`);

                            const before = new Date(originalDate);
                            before.setMonth(before.getMonth() - 1);
                            const after = new Date(originalDate);
                            after.setMonth(after.getMonth() + 1);

                            const format = (d) => ("0" + d.getDate()).slice(-2) + "/" + ("0" + (d.getMonth() + 1)).slice(-2) + "/" + d.getFullYear();

                            dateFrom.value = format(before);
                            dateTo.value = format(after);
                        } else {
                            dateFrom.value = "";
                            dateTo.value = "";
                        }

                        return false; // prevent any postback
                    }

                    // Optional: Clear form logic (you can define your own)
                    function clearFieldsOfRta() {
                        document.getElementById("<%= txtInvestorName.ClientID %>").value = '';
        document.getElementById("<%= txtAmount.ClientID %>").value = '';
        document.getElementById("<%= ddlChequeNo.ClientID %>").selectedIndex = 0;
        document.getElementById("<%= txtChequeSearch.ClientID %>").value = '';
        document.getElementById("<%= dateFromRta.ClientID %>").value = '';
        document.getElementById("<%= dateToRta.ClientID %>").value = '';
                    }

                    // ✅ Rebind on every partial postback
                    Sys.Application.add_load(function () {
                        
                        const checkboxes = document.querySelectorAll(".select-row-checkbox");
                        checkboxes.forEach(cb => {
                            cb.onclick = function () {
                                return onRowSelect(this);
                            };
                        });
                        var selectedIndexch = document.getElementById("hfSelectedRow").value;
                        if (selectedIndexch) {
                            var gridch = document.getElementById("<%= GridTransaction.ClientID %>");
                            var rowsch = gridch.getElementsByTagName("tr");

                            // Highlight the previously selected row after the postback
                            selectedIndexch = parseInt(selectedIndexch);

                            // Ensure the selected row exists
                            if (rowsch[selectedIndexch + 1]) { // Ensure row exists
                                rowsch[selectedIndexch + 1].classList.add("selected-row");
                            }
                        }
                    });


                </script>

                <style>
                    .selected-row td {
                        background-color: #d0ebff !important; /* light blue */
                    }
                </style>

                <style>
                    .selected-row-rta td {
                        background-color: #d0ebff !important; /* light blue */
                    }
                </style>


                <style>
                    .draggable-container {
                        max-height: 450px; /* Set a max height */
                        overflow-y: auto; /* Enable vertical scrolling */
                        resize: vertical; /* Allow users to resize vertically */
                        border: 1px solid #ddd; /* Optional: Border for better visibility */
                        padding: 10px;
                        background: #fff; /* Optional: Background color */
                    }
                </style>

                <div class="grid-margin stretch-card draggable-container">
                    <div class="card">
                        <div class="card-body">
                            <div>
                                <h5 class="card-title text-danger">RTA Transactions</h5>
                                <div class="row g-3">
                                    <div class="col-md-4">
                                        <div class="row g-3">
                                            <div class="col-md-6">
                                                <label for="dateFrom-rta" class="form-label">Date From <span class="text-danger">*</span></label>
                                                <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                    <asp:TextBox ID="dateFromRta" runat="server" CssClass="form-control date-input" placeholder="dd/mm/yyyy"></asp:TextBox>
                                                    <div class="input-group-addon">
                                                        <span class="glyphicon glyphicon-th"></span>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-md-6">
                                                <label for="dateTo-rta" class="form-label">Date To <span class="text-danger">*</span></label>
                                                <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                    <asp:TextBox ID="dateToRta" runat="server" CssClass="form-control date-input" placeholder="dd/mm/yyyy"></asp:TextBox>
                                                    <div class="input-group-addon">
                                                        <span class="glyphicon glyphicon-th"></span>
                                                    </div>
                                                </div>
                                            </div>
                                            <div>
                                                <label for="reconciliationType-rta" class="form-label">Status</label>
                                                <asp:RadioButtonList ID="rblReconciliationType" runat="server" RepeatDirection="Horizontal" CssClass="form-check">
                                                    <asp:ListItem Text="Reconciled" Value="Y"></asp:ListItem>
                                                    <asp:ListItem Text="Unreconciled" Value="N" Selected="True"></asp:ListItem>
                                                </asp:RadioButtonList>
                                            </div>
                                            <div>
                                                <label for="amc" class="form-label">AMC</label>
                                                <asp:DropDownList ID="DropDownList1" runat="server" CssClass="form-select">
                                                    <asp:ListItem Text="AMC 1" Value=""></asp:ListItem>
                                                    <asp:ListItem Text="AMC 2" Value="2"></asp:ListItem>
                                                    <asp:ListItem Text="AMC 3" Value="3"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            <div>
                                                <label for="branch" class="form-label">Branch</label>
                                                <asp:DropDownList ID="DropDownList2" runat="server" CssClass="form-select">
                                                    <asp:ListItem Text="" Value=""></asp:ListItem>
                                                    <asp:ListItem Text="Branch 2" Value="2"></asp:ListItem>
                                                    <asp:ListItem Text="Branch 3" Value="3"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <h4 class="mb-3">Search</h4>
                                        <div class="row g-3">
                                            <div class="col-md-6">
                                                 <label for="chequeNo" class="form-label">Cheque No</label>
                                       
                                                <asp:DropDownList ID="ddlChequeNo" runat="server" CssClass="form-select" onchange="ddlChequeChanged()">
                                                    <asp:ListItem Text="" Value=""></asp:ListItem>
                                                    <asp:ListItem Text="CHEQUE_NO" Value="001"></asp:ListItem>
                                                    <asp:ListItem Text="FOLIO_NO" Value="002"></asp:ListItem>
                                                    <asp:ListItem Text="APP_NO" Value="003"></asp:ListItem>
                                                    <asp:ListItem Text="PANNO" Value="004"></asp:ListItem>
                                                    <asp:ListItem Text="BROKER_ID" Value="005"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>

                                            <asp:HiddenField ID="hfSelectedRow" runat="server" ClientIDMode="Static" Value="-1" />

                                            <asp:HiddenField ID="hfrtaselectedrow" runat="server" ClientIDMode="Static" Value="-1" />


                                            <script type="text/javascript">

                                                function ddlChequeChanged() {
                                                    var selectedField = document.getElementById("<%= ddlChequeNo.ClientID %>").value;
                                                    var selectedValue = document.getElementById("hfSelectedRow").value;
                                                    var selectedIndex = isNaN(parseInt(selectedValue)) ? -1 : parseInt(selectedValue);
                                                    var grid = document.getElementById("<%= GridTransaction.ClientID %>");

                                                    if (!grid || selectedIndex < 0) {
                                                        document.getElementById("<%= txtChequeSearch.ClientID %>").value = "Please select a row first.";
                                                        return;
                                                    }

                                                    var rows = grid.getElementsByTagName("tr");
                                                    if (selectedIndex + 1 >= rows.length) {
                                                        document.getElementById("<%= txtChequeSearch.ClientID %>").value = "Invalid row selected.";
                                                        return;
                                                    }

                                                    var row = rows[selectedIndex + 1];
                                                    var value = "";

                                                    switch (selectedField) {
                                                        case "001": value = row.querySelector("[id*='lblCqNo']").innerText; break;
                                                        case "002": value = row.querySelector("[id*='lblFoloNo']").innerText; break;
                                                        case "003": value = row.querySelector("[id*='lblAppoModify']").innerText; break;
                                                        case "004": value = row.querySelector("[id*='lblpanNo']").innerText; break;
                                                        case "005": value = row.querySelector("[id*='lblbrokerCode']").innerText; break;
                                                    }

                                                    document.getElementById("<%= txtChequeSearch.ClientID %>").value = value;
                                                }

                                                // Rebind dropdown after partial update
                                                Sys.Application.add_load(function () {
                                                    var ddl = document.getElementById("<%= ddlChequeNo.ClientID %>");
                                                    if (ddl) {
                                                        ddl.onchange = ddlChequeChanged;
                                                    }
                                                });
                                            </script>



                                            <div class="col-md-6">
                                                <asp:TextBox ID="txtChequeSearch" runat="server" CssClass="form-control mt-4" placeholder="Input type text"></asp:TextBox>
                                            </div>
                                            <div>
                                                <label for="investorName" class="form-label">Investor Name</label>
                                                <asp:TextBox ID="txtInvestorName" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div>
                                                <label for="amount" class="form-label">Amount</label>
                                                <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="col-md-6">
                                                <label class="form-label">Tran Type</label>
                                                <asp:RadioButtonList ID="RadioButtonList1" runat="server" RepeatDirection="Horizontal" CssClass="form-check">
                                                    <asp:ListItem Text="Regular" Value="REGULAR" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Text="Sip" Value="SIP"></asp:ListItem>
                                                </asp:RadioButtonList>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div>
                                            <label for="searching" class="form-label">Searching (Click Column You Want To Search)</label>
                                            <div class="input-group">
                                                <asp:TextBox ID="txtSearching" runat="server" CssClass="form-control"></asp:TextBox>
                                                <asp:Button ID="btnFind" runat="server" CssClass="btn btn-outline-primary" Text="Find" Enabled="false" />
                                            </div>
                                        </div>
                                        <div class="d-flex align-items-center flex-wrap gap-3 my-3">
                                            <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-sm btn-primary" Text="Search" OnClick="btnSearch_Click" />
                                            <asp:Button ID="Button1" runat="server" CssClass="btn btn-sm btn-outline-primary" Text="Reset" OnClick="Button1_Click" />
                                            <asp:Button ID="Button2" runat="server" CssClass="btn btn-sm btn-outline-primary" Text="Export" OnClick="btnExport1_Click" />
                                        </div>
                                        <div>
                                            <label for="remarks" class="form-label">Remarks</label>
                                            <div class="input-group">
                                                <asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control"></asp:TextBox>
                                                <asp:Button ID="btnSaveRemarks" runat="server" CssClass="btn btn-outline-primary" Text="Save" OnClick="CmdSaveRemark_Click" />
                                            </div>
                                        </div>
                                        <div class="d-flex align-items-center flex-wrap gap-3 my-3">
                                            <asp:Button ID="btnConfirmPMS" runat="server" CssClass="btn btn-sm btn-outline-primary" Text="Confirm PMS" OnClick="cmdConfirm_Click" />
                                            <asp:Button ID="btnUnconfirmPMS" runat="server" CssClass="btn btn-sm btn-outline-primary" Text="Unconfirm PMS" OnClick="cmdUnconfirm_Click" />
                                            <asp:Button ID="btnExit" runat="server" CssClass="btn btn-outline-primary" Text="Exit" OnClick="btnExit_Click" />
                                            <asp:Button ID="btnReconcile" runat="server" CssClass="btn btn-primary" Text="Reconcile" OnClick="btnReconcile_Click" />
                         
                                        </div>
                                    </div>
                                </div>
                                <br />
                                <h5 class="card-title">List</h5>

                                <asp:HiddenField ID="hfSelectedTranCode" runat="server" />
                                <asp:HiddenField ID="hfSelectedAmount" runat="server" />

                                  <script type="text/javascript">
                                      // Function to synchronize scrolling between the two grids
                                      function syncScroll() {
                                          var grid1 = document.getElementById('gridContainer1');
                                          var grid2 = document.getElementById('gridContainer2');

                                          // Check if both grids exist
                                          if (!grid1 || !grid2) return;

                                          // Synchronize horizontal scrolling (scrollLeft)
                                          grid1.addEventListener('scroll', function () {
                                              grid2.scrollLeft = grid1.scrollLeft;
                                          });

                                          grid2.addEventListener('scroll', function () {
                                              grid1.scrollLeft = grid2.scrollLeft;
                                          });

                                          // Synchronize vertical scrolling (scrollTop)
                                          //grid1.addEventListener('scroll', function () {
                                          //    grid2.scrollTop = grid1.scrollTop;
                                          //});

                                          //grid2.addEventListener('scroll', function () {
                                          //    grid1.scrollTop = grid2.scrollTop;
                                          //});
                                      }

                                      // Ensure the scroll synchronization is applied on initial page load and after postback
                                      document.addEventListener('DOMContentLoaded', function () {
                                          syncScroll();  // Sync scroll on initial page load

                                          // Rebind the scroll synchronization after partial postbacks
                                          if (typeof Sys !== "undefined" && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                                              Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                                                  syncScroll();  // Reapply scroll synchronization after postback
                                              });
                                          }
                                      });
                                  </script>


                                <div class="table-responsive" id="gridContainer2" style="max-height: 300px; overflow-y: auto;">
                                    <asp:GridView ID="GridView1" CssClass="table table-bordered" runat="server" AutoGenerateColumns="false">

                                        <HeaderStyle CssClass="thead-dark" />
                                        <Columns>

                                            <asp:TemplateField HeaderText="Action" ItemStyle-CssClass="column-width-50">
                                                <ItemTemplate>

                                                    <asp:HiddenField ID="hfTranCoderta" runat="server" Value='<%# Eval("TRAN_CODE") %>' />

                                                    <%--   <asp:CheckBox ID="chkSelect" runat="server" AutoPostBack="true" OnCheckedChanged="chkSelectrta_CheckedChanged" />
                                                    --%>
                                                    <asp:CheckBox ID="chkSelect" runat="server" CssClass="chk-rta" AutoPostBack="false" />

                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Tran Code">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblTranTeyp" runat="server" Text='<%# Eval("TRAN_CODE") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="TranDate">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbleTrnDate" runat="server" Text='<%# Eval("TR_DATE", "{0:dd/MM/yyyy}") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Investor">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblInvestorNeam" runat="server" Text='<%# Eval("INV_NAME") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Address">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblAdderess" runat="server" Text='<%# Eval("ADDRESS1") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="City Name">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCityeName" runat="server" Text='<%# Eval("CITY_NAME") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="AMC CODE" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblmAC" runat="server" Text='<%# Eval("MUT_CODE") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="AMC NAME">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbelAMCNAM" runat="server" Text='<%# Eval("MUT_NAME") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Scheme Name">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblScemeNaeme" runat="server" Text='<%# Eval("SCH_NAME") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Amount">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblemount" runat="server" Text='<%# Eval("AMOUNT", "{0:N2}") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Folio No">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblFeoloNo" runat="server" Text='<%# Eval("FOLIO_NO") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Chq No">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbleCqNo" runat="server" Text='<%# Eval("CHEQUE_NO") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="App No">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblApepoModify" runat="server" Text='<%# Eval("APP_NO") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="TranType" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="ellTrnType" runat="server" Text='<%# Eval("TRAN_TYPE") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>











                                            <asp:TemplateField HeaderText="RM" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="llReM" runat="server" Text='<%# Eval("RMCODE") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>







                                            <asp:TemplateField HeaderText="rm_name">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblScemeeCode" runat="server" Text='<%# Eval("rm_name") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <%-- <asp:TemplateField HeaderText="Sip Amount">
                                            <ItemTemplate>
                                                <asp:Label ID="lblsipameont" runat="server" Text='<%# Eval("Sip_Amount") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>--%>

                                            <asp:TemplateField HeaderText="Branch Name">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblbranhnae" runat="server" Text='<%# Eval("branch_name") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>


                                            <%-- <asp:TemplateField HeaderText="SIP Type">
                                            <ItemTemplate>
                                                <asp:Label ID="lbleSipType" runat="server" Text='<%# Eval("SIP_TYPE") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>--%>

                                            <asp:TemplateField HeaderText="Broker Code">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblbrokeercode" runat="server" Text='<%# Eval("BUSINESS_RMCODE") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>


                                            <asp:TemplateField HeaderText="Reg Tran Type">
                                        <ItemTemplate>
                                            <asp:Label ID="lbltrantype" runat="server" Text='<%# Eval("REG_TRANTYPE") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                     <asp:TemplateField HeaderText="Unique Key">
                                            <ItemTemplate>
                                                <asp:Label ID="lblekey" runat="server" Text='<%# Eval("UNQ_KEY") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>





                                    <asp:TemplateField HeaderText="Flag" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblfeag" runat="server" Text='<%# Eval("FLAG") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>

                            <script type="text/javascript">
                                let tranCoderta = "";
                                let amrtaparsed = 0;

                                function onRtaRowSelect(checkbox) {
                                    const row = checkbox.closest("tr");
                                    const grid = document.getElementById("<%= GridView1.ClientID %>");
                                    const rows = grid.getElementsByTagName("tr");

                                    for (let i = 1; i < rows.length; i++) {
                                        const chk = rows[i].querySelector("input[type='checkbox']");
                                        if (chk && chk !== checkbox) {
                                            chk.checked = false;
                                        }
                                    }

                                    // Uncheck all other checkboxes (optional now)
                                    for (var i = 1; i < rows.length; i++) {
                                        rows[i].classList.remove("selected-row-rta");
                                    }

                                    // Highlight the selected row only
                                    row.classList.add("selected-row-rta");

                                    // Set selected row index
                                    document.getElementById("hfrtaselectedrow").value = row.rowIndex - 1;

                                    // ✅ Extract values
                                    const getVal = (id) => row.querySelector("[id*='" + id + "']")?.value?.trim() || '';
                                    const getText = (id) => row.querySelector("[id*='" + id + "']")?.innerText?.trim() || '';

                                    tranCoderta = getVal("hfTranCoderta");
                                    const amountText = getText("lblemount");

                                    document.getElementById("<%= hfSelectedTranCode.ClientID %>").value = tranCoderta;
                                    document.getElementById("<%= hfSelectedAmount.ClientID %>").value = amountText.replace(/,/g, '');

                                    // ✅ Parse amount
                                    try {
                                        amrtaparsed = parseFloat(amountText.replace(/,/g, '')) || 0;
                                    } catch {
                                        amrtaparsed = 0;
                                    }

                                    console.log("Selected Tran Code:", tranCoderta);
                                    console.log("Parsed Amount:", amrtaparsed);
                                }

                                // ✅ Bind on each partial postback
                                Sys.Application.add_load(function () {
                                    // Rebind the click event for checkboxes after postback
                                    const checkboxes = document.querySelectorAll(".chk-rta");
                                    checkboxes.forEach(cb => {
                                        cb.onclick = function () {
                                            return onRtaRowSelect(this);
                                        };
                                    });

                                    // Set a delay to prevent blocking of page rendering
                                    setTimeout(function () {
                                        var selectedIndexchrta = document.getElementById("hfrtaselectedrow").value;
                                        if (selectedIndexchrta) {
                                            var gridchrta = document.getElementById("<%= GridView1.ClientID %>");
                                            var rowschrta = gridchrta.getElementsByTagName("tr");

                                            // Ensure the selected row exists and highlight it
                                            selectedIndexchrta = parseInt(selectedIndexchrta);

                                            // Ensure the selected row exists and is not out of bounds
                                            if (rowschrta[selectedIndexchrta + 1]) {
                                                rowschrta[selectedIndexchrta + 1].classList.add("selected-row-rta");
                                            }
                                        }
                                    }, 0);  // 0ms delay ensures non-blocking execution
                                });

                            </script>

                            
                        </div>
                    </div>
                </div>

            </div>

            <div class="col-md-6">
                <div class="card">
                    <div class="card-body">
                        <label class="form-label" for="tarn-code">
                            Tran Code
                        </label>
                        <div class="input-group">
                            <asp:TextBox ID="tarnCode" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:Button ID="findButton" runat="server" Text="Find" CssClass="btn btn-outline-primary" OnClick="btnSearchtrn_Click" />
                        </div>

                        <div class="table-responsive">
                            <asp:GridView ID="tranCodeGrid" CssClass="table table-bordered" runat="server" AutoGenerateColumns="false">
                                <HeaderStyle CssClass="thead-dark" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Tran Code">
                                        <ItemTemplate>
                                            <asp:Label ID="lblInvestorName" runat="server" Text='<%# Eval("HO_TRAN_CODE") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Branch Name">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAddress" runat="server" Text='<%# Eval("BRANCH_NAME") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Amount">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAmount" runat="server" Text='<%# Eval("AMOUNT") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>

            <div id="updateProgress" class="loading-overlay">
                <div class="spinner-container">
                    <div class="spinner-border text-dark" role="status">
                        <span class="rupee-sign">₹</span>
                    </div>
                </div>
            </div>

            <style>
                /* Loader Overlay with Relaxing Yellow Background */
                .loading-overlay {
                    position: fixed;
                    top: 0;
                    left: 0;
                    width: 100%;
                    height: 100%;
                    background: rgba(255, 223, 128, 0.6); /* Soft warm yellow */
                    display: flex;
                    justify-content: center;
                    align-items: center;
                    z-index: 1050;
                    display: none; /* Initially hidden */
                }

                /* Centered Spinner Container */
                .spinner-container {
                    position: relative;
                    width: 80px;
                    height: 80px;
                }

                /* Spinner with a Larger Size */
                .spinner-border {
                    width: 80px;
                    height: 80px;
                    border-width: 6px;
                    color: #FFC107; /* Bootstrap warning yellow */
                    animation: spin 1s linear infinite;
                }

                /* Rupee Sign Positioned in the Center */
                .rupee-sign {
                    position: absolute;
                    top: 50%;
                    left: 50%;
                    transform: translate(-50%, -50%);
                    font-size: 30px;
                    font-weight: bold;
                    color: #333; /* Dark color for contrast */
                    font-family: Arial, sans-serif;
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

            <script>
                // Get the PageRequestManager instance
                var prm = Sys.WebForms.PageRequestManager.getInstance();

                // Show loader when request starts
                prm.add_beginRequest(function () {
                    document.getElementById("updateProgress").style.display = "flex";
                });

                // Hide loader when request completes
                prm.add_endRequest(function () {
                    document.getElementById("updateProgress").style.display = "none";
                });

            </script>

        </ContentTemplate>
    </asp:UpdatePanel>




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

    <script type="text/javascript">
        function scrollToGridView() {
            var gridView1 = document.getElementById('GridTransaction');
            if (gridView1) {
                var middleOfGrid = Math.max(0, gridView1.offsetTop - 50); // Offset for better visibility
                window.scrollTo({
                    top: middleOfGrid,
                    behavior: 'smooth'
                });
            }
        }

        function scrollToGridView1() {
            var gridView = document.getElementById('GridView1');
            if (gridView) {
                var middleOfGrid = gridView.offsetTop + (gridView.offsetHeight / 2);
                window.scrollTo({
                    top: middleOfGrid,
                    behavior: 'smooth'
                });
            }
        }

        // Attach to endRequest when using UpdatePanel
        Sys.Application.add_load(function () {
            scrollToGridView();
            scrollToGridView1();
        });
    </script>

    <script>
        function formatDateInput(inputField) {
            inputField.addEventListener("input", function () {
                let input = this.value.replace(/\D/g, ''); // Remove non-numeric characters

                if (input.length > 2) input = input.substring(0, 2) + '/' + input.substring(2);
                if (input.length > 5) input = input.substring(0, 5) + '/' + input.substring(5, 10);

                this.value = input;
            });
        }

        function validateDateFormat(dateStr) {
            // Regex: dd/mm/yyyy
            const regex = /^(0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[0-2])\/\d{4}$/;
            return regex.test(dateStr);
        }

        function bindDateEvents(inputField) {
            inputField.addEventListener("focus", function () {
                formatDateInput(this);
            });

            inputField.addEventListener("blur", function () {
                const value = this.value.trim();
                if (value !== "" && !validateDateFormat(value)) {
                    alert("Invalid date format. Please use dd/mm/yyyy.");
                    setTimeout(() => this.focus(), 0); // bring focus back after alert closes
                }
            });

        }

        document.addEventListener("DOMContentLoaded", function () {
            document.querySelectorAll(".date-input").forEach(function (inputField) {
                bindDateEvents(inputField);
            });

            // ✅ Fix for UpdatePanel - Rebind after async postbacks
            if (typeof Sys !== "undefined" && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                    document.querySelectorAll(".date-input").forEach(function (inputField) {
                        bindDateEvents(inputField);
                    });
                });
            }
        });
    </script>

   
   
</asp:Content>