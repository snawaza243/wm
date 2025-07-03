<%@ Page Title="" Language="C#" MasterPageFile="~/vmSite.Master" AutoEventWireup="true"
    CodeBehind="MakerChecker.aspx.cs" Inherits="WM.Masters.MakerChecker" MaintainScrollPositionOnPostback="true" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

        <style type="text/css">
            .gridViewContainer,
            .gridViewContainerRTA {
                max-block-size: 250px;
                overflow: auto;
                border: 1px solid #ccc;
            }
        </style>



        <script>
            function formatDateInput(inputField) {
                // oninput="formatDateInput(this)" placeholder="dd/mm/yyyy" MaxLength="10"
                inputField.addEventListener("input", function () {
                    let input = this.value.replace(/\D/g, ''); // Remove non-numeric characters

                    if (input.length > 2) input = input.substring(0, 2) + '/' + input.substring(2);
                    if (input.length > 5) input = input.substring(0, 5) + '/' + input.substring(5, 10);

                    this.value = input;
                });
            }
        </script>
        <div class="page-header">
            <h3 class="page-title">Maker Checker
            </h3>
        </div>


        <div id="updateProgress" class="loading-overlay">
            <div class="spinner-container">
                <div class="spinner-border text-dark" role="status">
                    <span class="rupee-sign"></span>
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
                background: rgba(255, 223, 128, 0.2);
                /* Soft warm yellow */
                display: flex;
                justify-content: center;
                align-items: center;
                z-index: 1050;
                display: none;
                /* Initially hidden */
            }

            /* Centered Spinner Container */
            .spinner-container {
                position: relative;
                width: 40px;
                height: 40px;
            }

            /* Spinner with a Larger Size */
            .spinner-border {
                width: 40px;
                height: 40px;
                border-width: 6px;
                color: #FFC107;
                /* Bootstrap warning yellow */
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
                color: #333;
                /* Dark color for contrast */
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


        <div class="row">
            <%--<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>--%>

                <div class="grid-margin stretch-card">
                    <div class="card">
                        <div class="card-body">
                            <h4 class="card-title">WealthMaker Transactions</h4>
                            <asp:UpdatePanel ID="UpdatePanelFilters" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="row g-3">
                                        <div class="col-md-10">
                                            <div class="row g-3">

                                                <%-- REGION --%>
                                                    <div class="col-md-3">
                                                        <label for="region" class="form-label">Region</label>
                                                        <asp:DropDownList AutoPostBack="true" ID="regionIDC"
                                                            OnSelectedIndexChanged="regionIDC_SelectedIndexChanged"
                                                            CssClass="form-select" runat="server">
                                                        </asp:DropDownList>
                                                    </div>

                                                    <%-- ZONE --%>
                                                        <div class="col-md-3">
                                                            <label for="zone" class="form-label">Zone</label>
                                                            <asp:DropDownList ID="zone" AutoPostBack="true"
                                                                OnSelectedIndexChanged="zone_SelectedIndexChanged"
                                                                runat="server" CssClass="form-select">
                                                            </asp:DropDownList>
                                                        </div>

                                                        <%-- BRANCH --%>
                                                            <div class="col-md-3">
                                                                <label for="branch" class="form-label">Branch</label>
                                                                <asp:DropDownList ID="branch" CssClass="form-select"
                                                                    AutoPostBack="true"
                                                                    OnSelectedIndexChanged="branch_SelectedIndexChanged"
                                                                    runat="server"></asp:DropDownList>
                                                            </div>

                                                            <%-- RM --%>
                                                                <div class="col-md-3">
                                                                    <label for="rm" class="form-label">RM</label>
                                                                    <asp:DropDownList ID="rm" CssClass="form-select"
                                                                        runat="server"></asp:DropDownList>
                                                                </div>

                                                                <%-- DATE FROM --%>
                                                                    <div class="col-md-2">
                                                                        <label for="dateFrom" class="form-label">Date
                                                                            From <span
                                                                                class="text-danger">*</span></label>
                                                                        <div>
                                                                            <asp:TextBox ID="dtFrom"
                                                                                class="form-control"
                                                                                oninput="formatDateInput(this)"
                                                                                placeholder="dd/mm/yyyy" MaxLength="10"
                                                                                runat="server" />
                                                                        </div>
                                                                    </div>

                                                                    <%-- DATE TO --%>
                                                                        <div class="col-md-2">
                                                                            <label for="dateTo" class="form-label">Date
                                                                                To <span
                                                                                    class="text-danger">*</span></label>
                                                                            <div>
                                                                                <asp:TextBox ID="dtTo"
                                                                                    class="form-control"
                                                                                    oninput="formatDateInput(this)"
                                                                                    placeholder="dd/mm/yyyy"
                                                                                    MaxLength="10" runat="server" />
                                                                            </div>
                                                                        </div>

                                                                        <%-- AMC --%>
                                                                            <div class="col-md-2">
                                                                                <label for="amc"
                                                                                    class="form-label">AMC</label>
                                                                                <asp:DropDownList ID="amc"
                                                                                    runat="server"
                                                                                    CssClass="form-select">
                                                                                </asp:DropDownList>
                                                                            </div>

                                                                            <%-- AR --%>
                                                                                <div class="col-md-3">
                                                                                    <label for="ar"
                                                                                        class="form-label">AR</label>
                                                                                    <asp:TextBox ID="txtAR"
                                                                                        runat="server"
                                                                                        class="form-control" />
                                                                                </div>

                                                                                <%-- AUTO/MANUAL RECO. --%>
                                                                                    <div class="col-md-3">
                                                                                        <div
                                                                                            class="d-flex align-items-center gap-3 mt-4">
                                                                                            <div class="">
                                                                                                <label
                                                                                                    class="form-check-label"
                                                                                                    for="autoReconciled">
                                                                                                    Auto Reco.
                                                                                                    <asp:RadioButton
                                                                                                        runat="server"
                                                                                                        class="form-check-input"
                                                                                                        ID="autoReconciled"
                                                                                                        Checked="true"
                                                                                                        AutoPostBack="true"
                                                                                                        OnCheckedChanged="autoReconciled_CheckedChanged"
                                                                                                        name="reconciliationType"
                                                                                                        value="auto" />
                                                                                                </label>
                                                                                            </div>
                                                                                            <div class="">
                                                                                                <label
                                                                                                    class="form-check-label"
                                                                                                    for="manualReconciled">
                                                                                                    Manual Reco.
                                                                                                    <asp:RadioButton
                                                                                                        runat="server"
                                                                                                        class="form-check-input"
                                                                                                        ID="manualReconciled"
                                                                                                        AutoPostBack="true"
                                                                                                        OnCheckedChanged="manualReconciled_CheckedChanged"
                                                                                                        name="reconciliationType"
                                                                                                        value="manual" />
                                                                                                </label>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>
                                            </div>
                                        </div>
                                        <div class="col-md-2">
                                            <div class="col-md-12">

                                                <label for="remarks" class="form-label">Remarks</label>
                                                <input class="form-control" id="remarks" />
                                            </div>

                                            <div class="col-md-12">
                                                <div class="d-flex align-items-center gap-3 mt-3">
                                                    <asp:Button ID="go" runat="server" OnClick="go_Click"
                                                        CssClass="btn btn-sm btn-primary" Text="Go" />
                                                    <asp:Button runat="server" ID="reset" OnClick="reset_Click"
                                                        Text="Reset" class="btn btn-sm btn-outline-primary">
                                                    </asp:Button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>

                            <asp:UpdatePanel ID="UpdatePanelGridView1" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div>
                                        <br>
                                        <h5 class="card-title">List</h5>

                                        <div class="table-responsive gridViewContainer">
                                            <asp:GridView ID="GridView1" OnRowCommand="GridView1_RowCommand"
                                                OnPageIndexChanging="GridView1_PageIndexChanging"
                                                CssClass="grid-view table table-hover" runat="server">
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkButton" runat="server"
                                                                CommandName="SelectRow"
                                                                CommandArgument='<%# Eval("TRAN_CODE") %>'
                                                                Text="Select" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="go" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="reset" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="regionIDC" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="zone" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="branch" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="autoReconciled" EventName="CheckedChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="manualReconciled" EventName="CheckedChanged" />
                                </Triggers>
                            </asp:UpdatePanel>

                            <br />
                            <br />
                            <asp:UpdatePanel ID="UpdatePanelRTA" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div>
                                        <h5 class="card-title">RTA Transactions</h5>
                                        <div class="table-responsive gridViewContainerRTA">
                                            <asp:GridView ID="GridView2" runat="server"
                                                CssClass="grid-view table table-hover"></asp:GridView>
                                        </div>
                                    </div>

                                    <div
                                        class="mt-5 d-flex align-items-center justify-content-center flex-md-row flex-column gap-3">
                                        <asp:Button ID="AuditAR" runat="server" OnClick="AuditAR_Click"
                                            CssClass="btn btn-outline-primary" Text="Audit AR" />
                                        <asp:Button ID="unreconcile" runat="server" OnClick="unreconcile_Click"
                                            CssClass="btn btn-outline-primary" Text="Unreconcile" />
                                    </div>
                                    <asp:Label ID="lblMessage" runat="server" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="GridView1" EventName="RowCommand" />
                                    <asp:AsyncPostBackTrigger ControlID="AuditAR" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="unreconcile" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
        </div>









    </asp:Content>