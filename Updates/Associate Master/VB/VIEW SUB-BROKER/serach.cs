<asp:GridView ID="GridAgentSearch" runat="server" AutoGenerateColumns="False" 
              OnPageIndexChanging="GridAgentSearch_PageIndexChanging" 
              AllowPaging="True" PageSize="10">
    <Columns>
        <!-- Select Column (LinkButton) -->
        <asp:TemplateField HeaderText="Select">
            <ItemTemplate>
                <asp:LinkButton ID="LinkButtonSelect" runat="server" Text="Select" CommandName="Select" CommandArgument='<%# Eval("ID") %>' OnClick="LinkButtonSelect_Click"></asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>

        <!-- Dynamic Columns Will Be Added Here -->
    </Columns>
    <PagerSettings Mode="NextPrevious" />
</asp:GridView>
