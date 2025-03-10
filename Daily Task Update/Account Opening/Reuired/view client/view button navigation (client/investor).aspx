<%--<asp:Button ID="ClientListClientMasterViewModel" CssClass="btn btn-primary" runat="server" Text="View Client" OnClientClick="openClientListModal(); return false;" />--%>
<%--<asp:Button ID="InvestorListViewButton" CssClass="btn btn-primary" runat="server" Text="View Investor" OnClientClick="openInvestorListModal(); return false;" />--%>

 <asp:Button ID="ClientListClientMasterViewModel" CssClass="btn btn-primary" runat="server" Text="View Client" OnClick="btnSearchClient" /> 
    <asp:Button ID="InvestorListViewButton" CssClass="btn btn-primary" runat="server" Text="View Investor" OnClick="btnSearchInvestor" /> 

<%--    <asp:Button ID="ClientListClientMasterViewModel" CssClass="btn btn-primary" runat="server"
    Text="View Client" OnClientClick="sessionStorage.setItem('AC_CL_FIND', 'true'); location.href='../Tree/frm_tree_mf.aspx'; return false;" />

<asp:Button ID="InvestorListViewButton" CssClass="btn btn-primary" runat="server"
    Text="View Investor" OnClientClick="sessionStorage.setItem('AC_INV_FIND', 'true'); location.href='../Tree/frm_tree_mf.aspx'; return false;" />--%>


<%--<asp:Button ID="InvestorListViewButton" CssClass="btn btn-primary" 
    runat="server" Text="View Investor" 
    OnClientClick="sessionStorage.setItem('AC_INV_FIND', 'true'); location.href='InvestorMasterSearch.aspx'; return false;"
    />--%>
