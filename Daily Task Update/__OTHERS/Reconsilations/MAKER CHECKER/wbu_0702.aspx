<%@ Page Title="" Language="C#" MasterPageFile="~/vmSite.Master" AutoEventWireup="true" CodeBehind="MakerChecker.aspx.cs" Inherits="WM.Masters.MakerChecker" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style type="text/css">
        .gridViewContainer, .gridViewContainerRTA {
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

    <div class="row">
        <div class="grid-margin stretch-card">
            <div class="card">
                <div class="card-body">

                    <h4 class="card-title">WealthMaker Transactions</h4>

                    <div class=" row g-3">
                        <div class="col-md-10">
                            <div class="row g-3">

                                <%-- REGION --%>
                                <div class="col-md-3">
                                    <label for="region" class="form-label">Region</label>
                                    <asp:DropDownList AutoPostBack="true" ID="regionIDC" OnSelectedIndexChanged="regionIDC_SelectedIndexChanged" CssClass="form-select" runat="server">
                                    </asp:DropDownList>
                                </div>

                                <%-- ZONE --%>
                             <div class="col-md-3">
                                    <label for="zone" class="form-label">Zone</label>
                                    <asp:DropDownList ID="zone" AutoPostBack="true" OnSelectedIndexChanged="zone_SelectedIndexChanged" runat="server" CssClass="form-select"></asp:DropDownList>

                                </div>

                                <%-- BRANCH --%>
                          <div class="col-md-3">
                                    <label for="branch" class="form-label">Branch</label>
                                    <asp:DropDownList ID="branch" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="branch_SelectedIndexChanged" runat="server"></asp:DropDownList>

                                </div>

                                <%-- RM --%>
                              <div class="col-md-3">
                                    <label for="rm" class="form-label">RM</label>
                                    <asp:DropDownList ID="rm" CssClass="form-select" runat="server"></asp:DropDownList>

                                </div>
              
                            <%--<h5 class=" mb-4">Reconcilation</h5>--%>

                                <%-- DATE FROM --%>
                                <div class="col-md-2">
                                    <label for="dateFrom" class="form-label">Date From <span class="text-danger">*</span></label>
                                    <div>
                                        <asp:TextBox ID="dtFrom" class="form-control"
                                            oninput="formatDateInput(this)" placeholder="dd/mm/yyyy" MaxLength="10" runat="server" />
                                    </div>
                                </div>

                                <%-- DATE TO --%>
                                <div class="col-md-2">
                                    <label for="dateTo" class="form-label">  Date To <span class="text-danger">*</span></label>
                                    <div >
                                        <asp:TextBox ID="dtTo" class="form-control" oninput="formatDateInput(this)" placeholder="dd/mm/yyyy" MaxLength="10" runat="server" />
                                    </div>
                                </div>

                                <%-- AMC --%>
                                <div class="col-md-2">
                                    <label for="amc" class="form-label">AMC</label>
                                    <asp:DropDownList ID="amc" runat="server" CssClass="form-select"></asp:DropDownList>
                                </div>

                                <%-- AR --%>
                                <div class="col-md-3">
                                    <label for="ar" class="form-label">AR</label>
                                    <asp:TextBox ID="txtAR" runat="server" class="form-control" />
                                </div>

                                <%-- AUTO/MANUAL RECO. --%>
                                <div class="col-md-3">
                                    <div class="d-flex align-items-center gap-3 mt-4">
                                        <div class="">

                                            <label class="form-check-label"
                                                for="autoReconciled">
                                                Auto Reco.                                                                   
                                                    <asp:RadioButton runat="server" class="form-check-input"
                                                        ID="autoReconciled" Checked="true" AutoPostBack="true" OnCheckedChanged="autoReconciled_CheckedChanged" name="reconciliationType" value="auto" />
                                            </label>
                                        </div>
                                        <div class="">

                                            <label class="form-check-label"
                                                for="manualReconciled">
                                                Manual Reco.                                                                   
                                                    <asp:RadioButton runat="server" class="form-check-input"
                                                        ID="manualReconciled" AutoPostBack="true" OnCheckedChanged="manualReconciled_CheckedChanged" name="reconciliationType"
                                                        value="manual" />
                                            </label>
                                        </div>
                                    </div>
                                </div>
                            </div>


                        </div>
                        <div class=" col-md-2">
                            <div class=" col-md-12">
                                <label for="remarks" class=" form-label">Remarks</label>
                                <input class="form-control" id="remarks" />
                            </div>

                            <div class=" col-md-12">
                                <div class="d-flex align-items-center gap-3 mt-3">
                                    <asp:Button ID="go" runat="server" OnClick="go_Click" CssClass=" btn btn-sm btn-primary" Text="Go" />
                                    <asp:Button runat="server" ID="reset" OnClick="reset_Click" Text="Reset" class=" btn btn-sm btn-outline-primary"></asp:Button>
                                </div>
                            </div>
                        </div>

                        <div>
                            <br>
                            <h5 class="card-title">List</h5>

                            <div class="table-responsive gridViewContainer" >
                                <asp:GridView ID="GridView1" OnRowCommand="GridView1_RowCommand" OnPageIndexChanging="GridView1_PageIndexChanging" CssClass="grid-view table table-hover" runat="server">
                                    <Columns>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkButton" runat="server" CommandName="SelectRow" CommandArgument='<%# Eval("TRAN_CODE") %>' Text="Select" />
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

        <div class=" grid-margin stretch-card">
            <div class="card">
                <div class="card-body">
                    <div>
                        <h5 class=" card-title">RTA Transactions</h5>

                        <div class="table-responsive gridViewContainerRTA">
                            <asp:GridView ID="GridView2" Height="100px" runat="server" CssClass="grid-view table table-hover"></asp:GridView>

                        </div>


                    </div>

                    <div class=" mt-5 d-flex align-items-center justify-content-center flex-md-row flex-column   gap-3">
                        <%--<button class=" btn btn-primary">Audit AR</button>--%>
                        <asp:Button ID="AuditAR" runat="server" OnClick="AuditAR_Click" CssClass="btn btn-outline-primary" Text="Audit AR" />

                        <asp:Button ID="unreconcile" runat="server" OnClick="unreconcile_Click" CssClass="btn btn-outline-primary" Text="Unreconcile" />
                        <%--<button class=" btn btn-outline-primary  ">Unreconcile</button>--%>
                        <%--<button class=" btn btn-outline-primary ">
                            Exit</button>--%>
                    </div>
                    <asp:Label ID="lblMessage" runat="server" />
                </div>
            </div>
        </div>


    </div>


 






</asp:Content>
