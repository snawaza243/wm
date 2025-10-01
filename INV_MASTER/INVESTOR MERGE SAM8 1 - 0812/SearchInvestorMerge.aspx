<%@ Page Title="Investor Merge Search" MasterPageFile="~/vmSite.Master" Language="C#" AutoEventWireup="true" CodeBehind="SearchInvestorMerge.aspx.cs" Inherits="WM.Masters.SearchInvestorMerge" %>



<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        th {
            position: sticky;
            top: 0;
            z-index: 1;
        }
    </style>

    <div class="page-header">
        <h3 class="page-title">Investor Merge Search </h3>
    </div>

    <div class="row">
        <div class="col-md-12 grid-margin stretch-card">
            <div class="card">
                <div class="card-body">
                    <div>
                        <h4 class="card-title">Search</h4>
                        <div class="forms-sample mb-5">
                            <div class="form-group row g-3">
                                <div class="col-md-3">
                                    <label>Category</label>
                                    <asp:DropDownList runat="server" ID="ddlCategory" CssClass="form-select">
                                        <asp:ListItem Selected="True">Client</asp:ListItem>
                                        <asp:ListItem>Subbroker</asp:ListItem>

                                    </asp:DropDownList>
                                </div>

                                <div class="col-md-3">
                                    <label>Branch Name</label>
                                    <asp:DropDownList runat="server" ID="ddlBranchName" CssClass="form-select">
                                        <asp:ListItem Text="Select Branch" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <div class="col-md-3">
                                    <label>City</label>
                                    <asp:DropDownList runat="server" ID="ddlCity" CssClass="form-select">
                                        <asp:ListItem Text="Select City" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <div class="col-md-3">
                                    <label>RM</label>
                                    <asp:DropDownList runat="server" ID="ddlRM" CssClass="form-select">
                                        <asp:ListItem Text="Select RM" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                 <div class="col-md-3">
                                    <label>Client/Subbroker Code</label>
                                    <asp:TextBox runat="server" ID="txtClientSubbrokerCode" CssClass="form-control" MaxLength="15" >                                          
                                    </asp:TextBox>
                                </div>

                                <div class="col-md-3">
                                    <label>Name</label>
                                    <asp:TextBox runat="server" ID="txtName" CssClass="form-control" MaxLength="100" >                                      
                                    </asp:TextBox>
                                </div>

                                <div class="col-md-3">
                                    <label>Address 1</label>
                                    <asp:TextBox runat="server" ID="txtAdd1" CssClass="form-control"  MaxLength="100">                                      
                                    </asp:TextBox>
                                </div>

                                <div class="col-md-3">
                                    <label>Address 2</label>
                                    <asp:TextBox runat="server" ID="txtAdd2" CssClass="form-control" MaxLength="100">                                      
                                    </asp:TextBox>
                                </div>
                               

                                <div class="col-md-3">
                                    <label>PAN</label>
                                    <asp:TextBox runat="server" ID="txtPAN" oninput="toUpperCase(this);" MaxLength="10" CssClass="form-control" >                                      
                                    </asp:TextBox>
                                    <asp:RegularExpressionValidator
                                        ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtPAN" ErrorMessage="Invalid PAN Format"
                                        CssClass="text-danger"
                                        ValidationExpression="^[A-Z]{5}[0-9]{4}[A-Z]{1}$"
                                        Display="Dynamic" />
                                </div>

                                <div class="col-md-3">
                                    <label>Mobile</label>
                                    <asp:TextBox runat="server" ID="txtMobile" MaxLength="15" CssClass="form-control"  >                          
                                    </asp:TextBox>
                                   <%-- <asp:RegularExpressionValidator
                                        ID="revPhone"
                                        runat="server"
                                        ControlToValidate="txtMobile"
                                        ErrorMessage="Invalid phone number format. (Min. 10 Digits)"
                                        CssClass="text-danger"
                                        ValidationExpression="^\d{10}$"
                                        Display="Dynamic" />--%>
                                </div>

                                <div class="col-md-3">
                                    <label>Phone</label>
                                    <asp:TextBox runat="server" ID="txtPhone" MaxLength="15" CssClass="form-control">                          
                                    </asp:TextBox>
                                </div>

                                <div class="col-md-3">
                                    <label>Sort By</label>
                                    <asp:DropDownList runat="server" ID="ddlSort" CssClass="form-select">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                        <asp:ListItem>Name</asp:ListItem>
                                        <asp:ListItem>Address1</asp:ListItem>
                                        <asp:ListItem>Address2</asp:ListItem>
                                        <asp:ListItem>City</asp:ListItem>
                                        <asp:ListItem>Phone</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <asp:Button ID="btnFind" runat="server" OnClick="btnFind_Click" CssClass="btn btn-gradient-primary" Text="Find" />
                            <asp:Button ID="btnReset" runat="server" OnClick="btnReset_Click" CssClass="btn btn-light" Text="Reset" />
                            <asp:Button ID="btnBack" runat="server" OnClick="btnBack_Click" CssClass="btn btn-light" Text="Back" />
                            <asp:Label ID="lblMessage" runat="server" Text="" CssClass="text-success mb-0" />
                        </div>
                        <div class="table-responsive" style="max-height: 350px">
                            <asp:GridView CssClass="table table-hover" runat="server" ID="gvInvMergeFind" OnRowCommand="gvInvMergeFind_RowCommand" AutoGenerateColumns="false">
                                <Columns>
                                    <asp:TemplateField HeaderText="#">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSr" runat="server" Text='<%# Container.DisplayIndex+1 %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkButton" runat="server" CommandName="SelectRow" CommandArgument='<%# Eval("CLIENT_CODE") + "," + Eval("CLIENT_NAME")  %>' Text="Select" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Client Name">
                                        <ItemTemplate>
                                            <asp:Label ID="lblClientName" runat="server" Text='<%# Eval("CLIENT_NAME")  %>' Visible="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Client Code">
                                        <ItemTemplate>
                                            <asp:Label ID="lblClientCode" runat="server" Text='<%# Eval("CLIENT_CODE")%> ' Visible="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Address 1">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAddress1" runat="server" Text='<%# Eval("ADDRESS1")  %>' Visible="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Address 2">
                                        <ItemTemplate>
                                            <asp:Label ID="lbladdress2" runat="server" Text='<%# Eval("ADDRESS2")  %>' Visible="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="City">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCity" runat="server" Text='<%# Eval("CITY_NAME")  %>' Visible="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Branch Name">
                                        <ItemTemplate>
                                            <asp:Label ID="lblBranch" runat="server" Text='<%# Eval("BRANCH_NAME")  %>' Visible="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Phone">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPhone" runat="server" Text='<%# Eval("PHONE")  %>' Visible="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="RM Name">
                                        <ItemTemplate>
                                            <asp:Label ID="lblRMName" runat="server" Text='<%# Eval("RM_NAME")  %>' Visible="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="KYC">
                                        <ItemTemplate>
                                            <asp:Label ID="lblKYC" runat="server" Text='<%# Eval("KYC")  %>' Visible="true" />
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
</asp:Content>


