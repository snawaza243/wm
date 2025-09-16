<%@ Page Title="Print Renewal" Language="C#" MasterPageFile="~/vmSite.Master" AutoEventWireup="true" CodeBehind="print_renewal.aspx.cs" Inherits="WM.Masters.print_renewal" EnableViewState="true" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid py-4">
        <div class="page-header mb-4">
            <h3 class="page-title">MF Trans Punching 2</h3>
        </div>

        <div class="row">
            <div class="col-md-6 offset-md-3">
                <div class="card">
                    <div class="card-body">
                        <h4 class="mb-4">Renewal Printing</h4>
                        <div class="form-group row">
                            <label for="CmbLetterType" class="col-sm-4 col-form-label">Letter Type</label>
                            <div class="col-sm-8">
                                <asp:DropDownList ID="CmbLetterType" runat="server" CssClass="form-control">
                                    <asp:ListItem Text="A" Value="A" />
                                    <asp:ListItem Text="B" Value="B" />
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="form-group row">
                            <label for="txtMonth" class="col-sm-4 col-form-label">Month</label>
                            <div class="col-sm-8">
                                <asp:TextBox ID="txtMonth" runat="server" CssClass="form-control" Text="Mar-2021" />
                            </div>
                        </div>
                        <div class="form-group row mt-4">
                            <div class="col-sm-12 text-center">
                                <asp:Button ID="btnCalculate" runat="server" Text="Calculate" CssClass="btn btn-primary mx-1" />
                                <asp:Button ID="btnReport" runat="server" Text="Report" CssClass="btn btn-secondary mx-1" />
                                <asp:Button ID="btnExit" runat="server" Text="Exit" CssClass="btn btn-danger mx-1" OnClientClick="window.close(); return false;" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <asp:HiddenField ID="hdnLoginId" runat="server" />
        <asp:HiddenField ID="hdnRoleId" runat="server" />
    </div>

</asp:Content>