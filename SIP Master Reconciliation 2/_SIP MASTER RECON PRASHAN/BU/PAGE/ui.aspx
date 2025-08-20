<%@ Page Title="SIP Master Reconciliation" Language="C#" MasterPageFile="~/vmSite.Master" AutoEventWireup="true" CodeBehind="sip_master_reconciliation.aspx.cs" Inherits="WM.Masters.sip_master_reconciliation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link rel="shortcut icon" href="../assets/images/favicon.png" />
    <link rel="stylesheet" href="../assets/vendors/mdi/css/materialdesignicons.min.css">
    <link rel="stylesheet" href="../assets/vendors/ti-icons/css/themify-icons.css">
    <link rel="stylesheet" href="../assets/vendors/css/vendor.bundle.base.css">
    <link rel="stylesheet" href="../assets/vendors/font-awesome/css/font-awesome.min.css">
    <link rel="stylesheet" href="../assets/vendors/bootstrap-datepicker/bootstrap-datepicker.min.css">
    <link rel="stylesheet" href="../assets/css/style.css">

    <div class="page-header">
        <h3 class="page-title">SIP Master Reconciliation</h3>
    </div>


    <asp:UpdatePanel ID="UpdatePanelFirst" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="card mb-3">
                <div class="card-body">
                    <h4 class="card-title">WealthMaker Transactions</h4>
                    <div class="row g-3">
                        <div class="col-md-4">
                            <label for="channelSelect" class="form-label">Channel</label>
                            <asp:DropDownList ID="channelSelect" CssClass="form-select" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlBranchCategory_SelectedIndexChanged">
                                <asp:ListItem Text="Select Channel" Value=""> </asp:ListItem>
                                <asp:ListItem>Channel A</asp:ListItem>
                                <asp:ListItem>Channel B</asp:ListItem>
                                <asp:ListItem>Channel C</asp:ListItem>
                            </asp:DropDownList>
                        </div>


                        <div class="col-md-4">
                            <label for="regionSelect" class="form-label">Region</label>
                            <asp:DropDownList ID="regionSelect" CssClass="form-select" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlRegion_SelectedIndexChanged">
                                <asp:ListItem Text="Select Region" Value=""></asp:ListItem>
                                <asp:ListItem>Region A</asp:ListItem>
                                <asp:ListItem>Region B</asp:ListItem>
                                <asp:ListItem>Region C</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="col-md-4">
                            <label for="zoneSelect" class="form-label">Zone</label>
                            <asp:DropDownList ID="zoneSelect" CssClass="form-select" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlzone_SelectedIndexChanged">
                                <asp:ListItem Text="Select Zone" Value=""></asp:ListItem>
                                <asp:ListItem>Zone A</asp:ListItem>
                                <asp:ListItem>Zone B</asp:ListItem>
                                <asp:ListItem>Zone C</asp:ListItem>
                            </asp:DropDownList>
                        </div>


                        <div class="col-md-4">
                            <label for="branchSelect" class="form-label">Branch</label>
                            <asp:DropDownList ID="branchSelect" CssClass="form-select" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlRmFill_SelectedIndexChanged">
                                <asp:ListItem Text="Select Branch" Value=""></asp:ListItem>
                                <asp:ListItem>Branch A</asp:ListItem>
                                <asp:ListItem>Branch B</asp:ListItem>
                                <asp:ListItem>Branch C</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="col-md-4">
                            <label for="rmSelect" class="form-label">RM</label>
                            <asp:DropDownList ID="rmSelect" CssClass="form-select" runat="server">
                                <asp:ListItem Text="Select RM" Value=""></asp:ListItem>
                                <asp:ListItem>RM A</asp:ListItem>
                                <asp:ListItem>RM B</asp:ListItem>
                                <asp:ListItem>RM C</asp:ListItem>
                            </asp:DropDownList>
                        </div>




                        <div class="col-md-4">
                            <label class="form-label">Tran Type</label>

                            <div class="d-flex align-items-center gap-3 my-cstm-radio-btn-cntnr-2">

                                <asp:RadioButtonList ID="RadioButtonList2" runat="server" RepeatDirection="Horizontal" CssClass="form-check">
                                    <asp:ListItem Text="Renewal" Value="renewal"></asp:ListItem>
                                    <asp:ListItem Text="Sip" Value="sip"></asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
                        </div>


                        <div class="col-md-4">
                            <div class="form-check-2">
                                <label class="form-check-label">
                                    <asp:CheckBox ID="pms" runat="server" Text="PMS" />
                                </label>
                            </div>
                        </div>

                        <div class="col-md-4">
                            <label for="arNumber" class="form-label">AR Number</label>
                            <asp:TextBox ID="arNumber" CssClass="form-control" runat="server" />
                        </div>
                        <div class="col-md-12">
                            <label class="form-label">Registrar</label>
                            <div class="my-cstm-radio-btn-cntnr-2" style="display: flex; gap: 1rem;">
                                <div class="form-check-2">
                                    <label class="form-check-label" for="c">
                                        <asp:RadioButton ID="c" GroupName="registrar" runat="server" Text="C" />
                                    </label>
                                </div>
                                <div class="form-check-2">
                                    <label class="form-check-label" for="k">
                                        <asp:RadioButton ID="k" GroupName="registrar" runat="server" Text="K" />
                                    </label>
                                </div>
                                <div class="form-check-2">
                                    <label class="form-check-label" for="cCob">
                                        <asp:RadioButton ID="cCob" GroupName="registrar" runat="server" Text="C COB" />
                                    </label>
                                </div>
                                <div class="form-check-2">
                                    <label class="form-check-label" for="kCob">
                                        <asp:RadioButton ID="kCob" GroupName="registrar" runat="server" Text="K COB" />
                                    </label>
                                </div>
                            </div>
                        </div>



                    </div>

                    <br />


                    <div class="row g-3">
                        <div class="col-md-4">
                            <label for="dateFrom" class="form-label">Date From</label>
                            <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                <asp:TextBox ID="dateFrom" runat="server" CssClass="form-control date-input"></asp:TextBox>
                                <div class="input-group-addon">
                                    <span class="glyphicon glyphicon-th"></span>
                                </div>
                            </div>
                        </div>

                        <div class="col-md-4">
                            <label for="dateTo" class="form-label">Date To </label>
                            <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                <asp:TextBox ID="dateTo" runat="server" CssClass="form-control date-input"></asp:TextBox>
                                <div class="input-group-addon">
                                    <span class="glyphicon glyphicon-th"></span>
                                </div>
                            </div>
                        </div>

                        <div class="col-md-4">
                            <label class="form-label">Status</label>
                            <div class="d-flex align-items-center gap-3">
                                <asp:RadioButtonList ID="rblReconciliationType" runat="server" RepeatDirection="Horizontal" CssClass="form-check">
                                    <asp:ListItem Text="Reconciled" Value="Y"></asp:ListItem>
                                    <asp:ListItem Text="Unreconciled" Value="N" Selected="True"></asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </div>




                    <div class="row g-3">
                        <div class="col-md-4">
                            <div class="form-check-2">
                                <label class="form-check-label">
                                    <asp:CheckBox ID="cob" runat="server" Text="COB" />
                                </label>
                            </div>
                        </div>

                        <div class="col-md-4">
                            <label for="amcSelect" class="form-label">AMC</label>
                            <asp:DropDownList ID="amcSelect" CssClass="form-select" runat="server">
                                <asp:ListItem Text="AMC 1" Value=""></asp:ListItem>
                                <asp:ListItem Text="AMC 2" Value="2"></asp:ListItem>
                                <asp:ListItem Text="AMC 3" Value="3"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>


                    <br />


                    <div class="row g-3">
                        <div class="d-flex align-items-center flex-md-row flex-column gap-3">
                            <asp:Button ID="btnSearch" CssClass="btn btn-primary w-100" runat="server" Text="Search" OnClick="btnSearch_Click" />
                            <asp:Button ID="btnReset" CssClass="btn btn-outline-primary w-100" runat="server" Text="Reset" OnClick="btnReset_Click" />
                            <asp:Button ID="btnExport" CssClass="btn btn-outline-primary w-100" runat="server" Text="Export" OnClick="btnExport1_Click" />
                        </div>
                        <div class="col-md-6">
                            <div class="my-cstm-radio-btn-cntnr-2" style="display: flex; align-items: center; gap: 1rem;">
                                <div class="form-check-2">
                                    <asp:RadioButton ID="op1" runat="server" GroupName="options" />
                                    <label class="form-check-label" for="op1">OP1<i class="input-helper"></i></label>
                                </div>
                                <div class="form-check-2">
                                    <asp:RadioButton ID="op2" runat="server" GroupName="options" Checked="true" />
                                    <label class="form-check-label" for="op2">OP2<i class="input-helper"></i></label>
                                </div>
                                <p class="mb-0 text-danger fw-bold">Count</p>
                                <asp:Label ID="lblRowCount" runat="server" Text="0"></asp:Label></p>
                      
                            </div>
                        </div>

                    </div>
                </div>

                <asp:HiddenField ID="hftran1stcode" runat="server" />

                <div class="table-responsive" id="gridContainer1" style="max-height: 300px; overflow-y: auto;">
                    <asp:GridView ID="tableSearchResults" CssClass="table table-bordered" runat="server" AutoGenerateColumns="false">

                        <HeaderStyle CssClass="thead-dark" />
                        <Columns>

                            <asp:TemplateField HeaderText="Action" ItemStyle-CssClass="column-width-50">
                                <ItemTemplate>

                                    <asp:HiddenField ID="hfTranCode" runat="server" Value='<%# Eval("TRAN_CODE") %>' />

                                      <asp:CheckBox ID="chkSelect" runat="server" AutoPostBack="true" OnCheckedChanged="chkSelect_CheckedChanged" />
                      
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="TranCode">
                                <ItemTemplate>
                                    <asp:Label ID="lblTranType" runat="server" Text='<%# Eval("TRAN_CODE") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="TranDate">
                                <ItemTemplate>
                                    <asp:Label ID="lblTranDate" runat="server" Text='<%# Eval("TR_DATE", "{0:dd/MM/yyyy}") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="InvestorName">
                                <ItemTemplate>
                                    <asp:Label ID="lblInvestorName" runat="server" Text='<%# Eval("INVESTOR_NAME") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="AMC CODE" ItemStyle-CssClass="hide-column" HeaderStyle-CssClass="hide-column">
                                <ItemTemplate>
                                    <asp:Label ID="lblAMC" runat="server" Text='<%# Eval("MUT_CODE") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Address">
                                <ItemTemplate>
                                    <asp:Label ID="lbladressfirst" runat="server" Text='<%# Eval("ADDRESS") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="City">
                                <ItemTemplate>
                                    <asp:Label ID="lblcityfirst" runat="server" Text='<%# Eval("CITY_NAME") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="AMC NAME">
                                <ItemTemplate>
                                    <asp:Label ID="lblAMCNAME" runat="server" Text='<%# Eval("MUT_NAME") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Scheme Name">
                                <ItemTemplate>
                                    <asp:Label ID="lblSchemeName" runat="server" Text='<%# Eval("SCH_NAME") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Amount">
                                <ItemTemplate>
                                    <asp:Label ID="lblAmount" runat="server" Text='<%# Eval("AMOUNT") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Folio No">
                                <ItemTemplate>
                                    <asp:Label ID="lblFolioNo" runat="server" Text='<%# Eval("FOLIO_NO") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Chq No">
                                <ItemTemplate>
                                    <asp:Label ID="lblChqNo" runat="server" Text='<%# Eval("CHEQUE_NO") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="TranType" ItemStyle-CssClass="hide-column" HeaderStyle-CssClass="hide-column">
                                <ItemTemplate>
                                    <asp:Label ID="lblTrnType" runat="server" Text='<%# Eval("TRAN_TYPE") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="App No">
                                <ItemTemplate>
                                    <asp:Label ID="lblAppNoModify" runat="server" Text='<%# Eval("APP_NO") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>




                            <asp:TemplateField HeaderText="Pan No" ItemStyle-CssClass="hide-column" HeaderStyle-CssClass="hide-column">
                                <ItemTemplate>
                                    <asp:Label ID="lblpanNo" runat="server" Text='<%# Eval("PANNO") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Broker Code" ItemStyle-CssClass="hide-column" HeaderStyle-CssClass="hide-column">
                                <ItemTemplate>
                                    <asp:Label ID="lblbrokerCode" runat="server" Text='<%# Eval("BROKER_ID") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>





                            <asp:TemplateField HeaderText="RM" ItemStyle-CssClass="hide-column" HeaderStyle-CssClass="hide-column">
                                <ItemTemplate>
                                    <asp:Label ID="lblRM" runat="server" Text='<%# Eval("RMCODE") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>







                            <asp:TemplateField HeaderText="rm_name">
                                <ItemTemplate>
                                    <asp:Label ID="lblSchemeCode" runat="server" Text='<%# Eval("rm_name") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Branch Name">
                                <ItemTemplate>
                                    <asp:Label ID="lblbranchna" runat="server" Text='<%# Eval("branch_name") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Sip Amount">
                                <ItemTemplate>
                                    <asp:Label ID="lblsipamount" runat="server" Text='<%# Eval("Sip_Amount") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>



                            <asp:TemplateField HeaderText="Branch Code" ItemStyle-CssClass="hide-column" HeaderStyle-CssClass="hide-column">
                                <ItemTemplate>
                                    <asp:Label ID="lblbranchco" runat="server" Text='<%# Eval("busi_branch_code") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Total SIP">
                                <ItemTemplate>
                                    <asp:Label ID="lblInstallNo" runat="server" Text='<%# Eval("INSTALLMENTS_NO") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="STATUS">
                                <ItemTemplate>
                                    <asp:Label ID="lblRECflag" runat="server"
                                        Text='<%# Eval("REC_FLAG") != null && Eval("REC_FLAG").ToString() == "Y" ? "RECONCILED" : "UNRECONCILED" %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Flag">
                                <ItemTemplate>
                                    <asp:Label ID="lblflag" runat="server" Text='<%# Eval("FLAG") %>' />
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

                <br />

                <style>
                    .hide-column {
                        display: none;
                    }
                </style>

                <style>
                    .draggable-container {
                        max-height: 500px; /* Set a max height */
                        overflow-y: auto; /* Enable vertical scrolling */
                        resize: vertical; /* Allow users to resize vertically */
                        border: 1px solid #ddd; /* Optional: Border for better visibility */
                        padding: 10px;
                        background: #fff; /* Optional: Background color */
                    }
                </style>

                <div class="grid-margin stretch-card draggable-container">
                    <div class="card mb-3">
                        <div class="card-body">
                            <h4 class="card-title">RTA Transactions</h4>
                            <div class="row g-3">
                                <div class="col-md-6">
                                    <div class="row g-3">
                                        <div>
                                            <label for="dateFrom-rta" class="form-label">Date From <span class="text-danger">*</span></label>
                                            <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                <asp:TextBox ID="dateFromRta" runat="server" CssClass="form-control date-input" placeholder="dd/mm/yyyy" />
                                                <div class="input-group-addon">
                                                    <span class="glyphicon glyphicon-th"></span>
                                                </div>
                                            </div>
                                        </div>

                                        <div>
                                            <label for="dateTo-rta" class="form-label">Date To <span class="text-danger">*</span></label>
                                            <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                <asp:TextBox ID="dateToRta" runat="server" CssClass="form-control date-input" placeholder="dd/mm/yyyy" />
                                                <div class="input-group-addon">
                                                    <span class="glyphicon glyphicon-th"></span>
                                                </div>
                                            </div>
                                        </div>

                                        <div>
                                            <label class="form-label"></label>
                                            <div class="d-flex align-items-center gap-3 my-cstm-radio-btn-cntnr-2">
                                                <div class="form-check-2">
                                                    <asp:RadioButton ID="RadioButton1" runat="server" GroupName="status" CssClass="form-check-input-2" Text="Reconciled" Value="Y" />
                                                </div>
                                                <div class="form-check-2">
                                                    <asp:RadioButton ID="RadioButton2" runat="server" GroupName="status" CssClass="form-check-input-2" Text="Unreconciled" value="N" Checked="true" />
                                                </div>
                                            </div>
                                        </div>



                                        <div>
                                            <label for="amcSelectrta" class="form-label">AMC</label>
                                            <asp:DropDownList ID="amcSelectrta" runat="server" CssClass="form-select">
                                                <asp:ListItem Text="Select AMC" Value="" />
                                                <asp:ListItem Text="AMC A" Value="A" />
                                                <asp:ListItem Text="AMC B" Value="B" />
                                                <asp:ListItem Text="AMC C" Value="C" />
                                            </asp:DropDownList>
                                        </div>
                                        <div>
                                            <label for="branchSelectrta" class="form-label">Branch</label>
                                            <asp:DropDownList ID="branchSelectrta" runat="server" CssClass="form-select">
                                                <asp:ListItem Text="Select Branch" Value="" />
                                                <asp:ListItem Text="A" Value="A" />
                                                <asp:ListItem Text="B" Value="B" />
                                                <asp:ListItem Text="C" Value="C" />
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-md-6">
                                    <h5 class="mb-3">Search</h5>
                                    <div class="row g-3 mb-3">
                                        <div>
                                            <label class="form-label">Search </label>
                                            <div class="d-flex align-items-center">
                                                <asp:DropDownList ID="chequeNoSelect" runat="server" CssClass="form-select me-2" onchange="ddlChequeChanged()">
                                                    <asp:ListItem Text="" Value="" />
                                                    <asp:ListItem Text="CHEQUE_NO" Value="001"></asp:ListItem>
                                                    <asp:ListItem Text="FOLIO_NO" Value="002"></asp:ListItem>
                                                    <asp:ListItem Text="APP_NO" Value="003"></asp:ListItem>
                                                    <asp:ListItem Text="PANNO" Value="004"></asp:ListItem>
                                                    <asp:ListItem Text="BROKER_ID" Value="005"></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:TextBox ID="chequeNo" runat="server" CssClass="form-control" />
                                            </div>
                                        </div>

                                        <div>
                                            <label for="investorName" class="form-label">Investor Name</label>
                                            <asp:TextBox ID="txtInvestorName" runat="server" CssClass="form-control" />
                                        </div>

                                        <div>
                                            <label for="amount" class="form-label">Amount</label>
                                            <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" />
                                        </div>

                                        <div>
                                            <label for="remark" class="form-label">Remark</label>
                                            <asp:TextBox ID="remark" runat="server" CssClass="form-control" />
                                        </div>

                                        <div>
                                            <label class="form-label">Tran Type</label>
                                            <div class="d-flex align-items-center gap-3 my-cstm-radio-btn-cntnr-2">

                                                <asp:RadioButtonList ID="RadioButtonList1" runat="server" RepeatDirection="Horizontal" CssClass="form-check">
                                                    <asp:ListItem Text="Renewal" Value="renewal"></asp:ListItem>
                                                    <asp:ListItem Text="Sip" Value="sip"></asp:ListItem>
                                                </asp:RadioButtonList>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="d-flex align-items-center gap-3 flex-wrap">
                                        <asp:Button ID="saveBtn" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="CmdSaveRemark_Click" />
                                        <asp:Button ID="reconcileBtn" runat="server" Text="Base SIP Reconcile" CssClass="btn btn-outline-primary" OnClick="btnsiprerta_Click" />
                                    </div>
                                </div>

                                <div class="d-flex align-items-center flex-md-row flex-column gap-3">
                                    <asp:Button ID="searchBtn" runat="server" Text="Search" CssClass="btn btn-primary w-100" OnClick="btnSearchrta_Click" />
                                    <asp:Button ID="resetBtn" runat="server" Text="Reset" CssClass="btn btn-outline-primary w-100" OnClick="btnReset2_Click" />
                                    <asp:Button ID="exportBtn" runat="server" Text="Export" CssClass="btn btn-outline-primary w-100" OnClick="btnExport_Click" />
                                </div>
                            </div>

                            <br />

                            <div class="table-responsive" id="gridContainer2" style="max-height: 300px; overflow-y: auto;">
                                <asp:GridView ID="GridRta" CssClass="table table-bordered" runat="server" AutoGenerateColumns="false">

                                    <HeaderStyle CssClass="thead-dark" />
                                    <Columns>

                                        <asp:TemplateField HeaderText="Action" ItemStyle-CssClass="column-width-50">
                                            <ItemTemplate>

                                                <asp:HiddenField ID="hfTranCoderta" runat="server" Value='<%# Eval("TRAN_CODE") %>' />

                                                <asp:CheckBox ID="chkSelect" runat="server" AutoPostBack="true" OnCheckedChanged="chkSelectrta_CheckedChanged" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="TranCode">
                                            <ItemTemplate>
                                                <asp:Label ID="lblTranTyp" runat="server" Text='<%# Eval("TRAN_CODE") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="TranDate">
                                            <ItemTemplate>
                                                <asp:Label ID="lblTrnDate" runat="server" Text='<%# Eval("TR_DATE", "{0:dd/MM/yyyy}") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Investor" ItemStyle-Width="150px" HeaderStyle-Width="150px">
                                            <ItemTemplate>
                                                <asp:Label ID="lblInvestorNam" runat="server" Text='<%# Eval("INV_NAME") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="AMC CODE" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lblAC" runat="server" Text='<%# Eval("MUT_CODE") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Address">
                                            <ItemTemplate>
                                                <asp:Label ID="lblAdderess" runat="server" Text='<%# Eval("ADDRESS") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="City Name">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCityeName" runat="server" Text='<%# Eval("CITY_NAME") %>' />
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
                                                <asp:Label ID="lblmount" runat="server" Text='<%# Eval("AMOUNT", "{0:N2}") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Folio No">
                                            <ItemTemplate>
                                                <asp:Label ID="lblFoloNo" runat="server" Text='<%# Eval("FOLIO_NO") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="TranType" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="llTrnType" runat="server" Text='<%# Eval("TRAN_TYPE") %>' />
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





                                        <asp:TemplateField HeaderText="RM" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="llRM" runat="server" Text='<%# Eval("RMCODE") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>




                                        <asp:TemplateField HeaderText="RM NAME">
                                            <ItemTemplate>
                                                <asp:Label ID="lblScemeCode" runat="server" Text='<%# Eval("rm_name") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <%--<asp:TemplateField HeaderText="Sip Amount">
                            <ItemTemplate>
                                <asp:Label ID="lblsipamont" runat="server" Text='<%# Eval("Sip_Amount") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>--%>

                                        <asp:TemplateField HeaderText="Branch Name">
                                            <ItemTemplate>
                                                <asp:Label ID="lblbranhna" runat="server" Text='<%# Eval("branch_name") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="BROKER CODE">
                                            <ItemTemplate>
                                                <asp:Label ID="LBLBROKERCODE" runat="server" Text='<%# Eval("BROKER_ID") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Reg Tran Type" ItemStyle-Width="50px" HeaderStyle-Width="50px">
                                            <ItemTemplate>
                                                <asp:Label ID="lblregfag" runat="server" Text='<%# Eval("REG_TRANTYPE") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Unique Key">
                                            <ItemTemplate>
                                                <asp:Label ID="lblekey" runat="server" Text='<%# Eval("UNQ_KEY") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Flag" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lblfag" runat="server" Text='<%# Eval("FLAG") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                    </Columns>
                                </asp:GridView>
                            </div>

                        </div>
                    </div>
                </div>

                <asp:HiddenField ID="hfSelectedRow" runat="server" ClientIDMode="Static" Value="-1" />

                <asp:HiddenField ID="hfrtaselectedrow" runat="server" ClientIDMode="Static" Value="-1" />


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

                <script type="text/javascript">

                    function ddlChequeChanged() {
                        var selectedField = document.getElementById("<%= chequeNoSelect.ClientID %>").value;
                        var selectedValue = document.getElementById("hfSelectedRow").value;
                        var selectedIndex = isNaN(parseInt(selectedValue)) ? -1 : parseInt(selectedValue);
                        var grid = document.getElementById("<%= tableSearchResults.ClientID %>");

                        if (!grid || selectedIndex < 0) {
                            document.getElementById("<%= chequeNo.ClientID %>").value = "Please select a row first.";
                            return;
                        }

                        var rows = grid.getElementsByTagName("tr");
                        if (selectedIndex + 1 >= rows.length) {
                            document.getElementById("<%= chequeNo.ClientID %>").value = "Invalid row selected.";
                            return;
                        }

                        var row = rows[selectedIndex + 1];
                        var value = "";

                        switch (selectedField) {
                            case "001": value = row.querySelector("[id*='lblChqNo']").innerText; break;
                            case "002": value = row.querySelector("[id*='lblFolioNo']").innerText; break;
                            case "003": value = row.querySelector("[id*='lblAppNoModify']").innerText; break;
                            case "004": value = row.querySelector("[id*='lblpanNo']").innerText; break;
                            case "005": value = row.querySelector("[id*='lblbrokerCode']").innerText; break;
                        }

                        document.getElementById("<%= chequeNo.ClientID %>").value = value;
                    }

                    // Rebind dropdown after partial update
                    Sys.Application.add_load(function () {
                        var ddl = document.getElementById("<%= chequeNoSelect.ClientID %>");
                        if (ddl) {
                            ddl.onchange = ddlChequeChanged;
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

                <div class="card mb-3">
                    <div class="card-body">
                        <h4 class="card-title">SIP Master</h4>

                        <div class="row g-3 mb-3">
                            <div class="col-md-4">
                                <label for="folioNo" class="form-label">Folio No</label>
                                <asp:TextBox ID="folioNo" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                            </div>

                            <div class="col-md-4">
                                <label for="amount" class="form-label">Amount</label>
                                <asp:TextBox ID="TextBox3" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                            </div>

                            <div class="col-md-4">
                                <label for="pan" class="form-label">PAN</label>
                                <asp:TextBox ID="pan" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                            </div>

                            <div class="col-md-4">
                                <label for="clientCode" class="form-label">Client Code</label>
                                <asp:TextBox ID="clientCode" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                            </div>



                            <div class="col-md-4">
                                <label for="sipStartDate" class="form-label">WW SIP Start Date</label>
                                <div class="date" data-provide="datepicker">
                                    <asp:TextBox ID="sipStartDate" runat="server" CssClass="form-control date-input" Enabled="false"></asp:TextBox>
                                    <div class="input-group-addon">
                                        <span class="glyphicon glyphicon-th"></span>
                                    </div>
                                </div>
                            </div>


                            <div class="d-flex align-items-center flex-md-row flex-column gap-3">
                                <asp:Button ID="searchButton" runat="server" CssClass="btn btn-primary w-100" Text="Search" OnClick="btnSearchsip_Click" Enabled="false" />
                                <asp:Button ID="reconcileButton" runat="server" CssClass="btn btn-outline-primary w-100" Text="SIP Reconcile" Enabled="false" />
                                <asp:Button ID="resetButton" runat="server" CssClass="btn btn-outline-primary w-100" Text="Reset" OnClick="btnReset_Click" Enabled="false" />
                            </div>

                            <br>

                            <div class="table-responsive">
                                <asp:GridView ID="GridSIPTransactions" CssClass="table table-bordered" runat="server" AutoGenerateColumns="false">
                                    <HeaderStyle CssClass="thead-dark" />
                                    <Columns>
                                        <asp:TemplateField HeaderText="Investor Name">
                                            <ItemTemplate>
                                                <asp:Label ID="lblInvestorName" runat="server" Text='<%# Eval("INVESTOR_NAME") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Address">
                                            <ItemTemplate>
                                                <asp:Label ID="lblAddress" runat="server" Text='<%# Eval("ADDRESS") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="City Name">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCityName" runat="server" Text='<%# Eval("CITY_NAME") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Bank Name">
                                            <ItemTemplate>
                                                <asp:Label ID="lblBankName" runat="server" Text='<%# Eval("BANK_NAME") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Client Code">
                                            <ItemTemplate>
                                                <asp:Label ID="lblClientCode" runat="server" Text='<%# Eval("CLIENT_CODE") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Scheme Code">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSchemeCode" runat="server" Text='<%# Eval("SCH_CODE") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Mut Code">
                                            <ItemTemplate>
                                                <asp:Label ID="lblMutCode" runat="server" Text='<%# Eval("MUT_CODE") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="RM Name">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRmName" runat="server" Text='<%# Eval("RM_NAME") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Branch Name">
                                            <ItemTemplate>
                                                <asp:Label ID="lblBranchName" runat="server" Text='<%# Eval("BRANCH_NAME") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="PAN No">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPanNo" runat="server" Text='<%# Eval("PANNO") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="AMC Name">
                                            <ItemTemplate>
                                                <asp:Label ID="lblAmcName" runat="server" Text='<%# Eval("MUT_NAME") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Scheme Name">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSchemeName" runat="server" Text='<%# Eval("SCH_NAME") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Transaction Date">
                                            <ItemTemplate>
                                                <asp:Label ID="lblTrDate" runat="server" Text='<%# Eval("TR_DATE", "{0:dd/MM/yyyy}") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Transaction Type">
                                            <ItemTemplate>
                                                <asp:Label ID="lblTranType" runat="server" Text='<%# Eval("TRAN_TYPE") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="App No">
                                            <ItemTemplate>
                                                <asp:Label ID="lblAppNo" runat="server" Text='<%# Eval("APP_NO") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Cheque No">
                                            <ItemTemplate>
                                                <asp:Label ID="lblChequeNo" runat="server" Text='<%# Eval("CHEQUE_NO") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Amount">
                                            <ItemTemplate>
                                                <asp:Label ID="lblAmount" runat="server" Text='<%# Eval("AMOUNT", "{0:N2}") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="SIP Amount">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSipAmount" runat="server" Text='<%# Eval("SIP_AMOUNT", "{0:N2}") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Folio No">
                                            <ItemTemplate>
                                                <asp:Label ID="lblFolioNo" runat="server" Text='<%# Eval("FOLIO_NO") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="SIP Type">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSipType" runat="server" Text='<%# Eval("SIP_TYPE") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Lead No">
                                            <ItemTemplate>
                                                <asp:Label ID="lblLeadNo" runat="server" Text='<%# Eval("LEAD_NO") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Lead Name">
                                            <ItemTemplate>
                                                <asp:Label ID="lblLeadName" runat="server" Text='<%# Eval("LEAD_NAME") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Logged User ID">
                                            <ItemTemplate>
                                                <asp:Label ID="lblLoggedUserId" runat="server" Text='<%# Eval("LOGGEDUSERID") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                    </Columns>
                                </asp:GridView>
                            </div>

                        </div>
                    </div>

                   

                    <div class="card">
                        <div class="card-body">
                            <div class="d-flex align-items-md-center justify-content-between gap-3 flex-md-row flex-column">
                                <div class="form-check">
                                    <label class="form-check-label">
                                        <span class="text-danger">Option1: WealthMaker step1 compare with RTA SIP</span>
                                        <asp:RadioButton ID="Option1Radio" runat="server" GroupName="options" />
                                    </label>
                                </div>

                                <asp:Button ID="ExitButton" runat="server" Text="Exit" CssClass="btn btn-outline-primary" OnClick="btnExit_Click" />
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
        </ContentTemplate>
    </asp:UpdatePanel>

    <script src="../assets/vendors/js/vendor.bundle.base.js"></script>
    <script src="../assets/vendors/chart.js/chart.umd.js"></script>
    <script src="../assets/vendors/bootstrap-datepicker/bootstrap-datepicker.min.js"></script>
    <script src="../assets/js/off-canvas.js"></script>
    <script src="../assets/js/misc.js"></script>
    <script src="../assets/js/settings.js"></script>
    <script src="../assets/js/todolist.js"></script>
    <script src="../assets/js/jquery.cookie.js"></script>
    <script src="../assets/js/dashboard.js"></script>
    <script src="../assets/js/hoverable-collapse.js"></script>
</asp:Content>
