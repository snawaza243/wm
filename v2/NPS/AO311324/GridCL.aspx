<Columns>
    <asp:TemplateField HeaderText="#" ItemStyle-CssClass="column-width-50">
        <ItemTemplate>
            <asp:Label ID="ClientListlblSr" runat="server" Text='<%# Container.DisplayIndex + 1 %>' />
        </ItemTemplate>
    </asp:TemplateField>

    <asp:TemplateField>
        <ItemTemplate>
            <asp:LinkButton ID="ClientListlnkButton" runat="server" CommandName="SelectRow" CommandArgument='<%# Eval("CLIENT_CODE") %>' Text="Select" />
        </ItemTemplate>
    </asp:TemplateField>

    <asp:TemplateField HeaderText="Client ID" ItemStyle-CssClass="column-width-90">
        <ItemTemplate>
            <asp:Label ID="ClientListlblClientId" runat="server" Text='<%# Eval("CLIENT_CODE") %>' />
        </ItemTemplate>
    </asp:TemplateField>

    <asp:TemplateField HeaderText="Client Name" ItemStyle-CssClass="column-width-90">
        <ItemTemplate>
            <asp:Label ID="ClientListlblClientName" runat="server" Text='<%# Eval("CLIENT_NAME") %>' />
        </ItemTemplate>
    </asp:TemplateField>

    <asp:TemplateField HeaderText="Business Code" ItemStyle-CssClass="column-width-90">
        <ItemTemplate>
            <asp:Label ID="ClientListlblClientCode" runat="server" Text='<%# Eval("BUSINESS_CODE") %>' />
        </ItemTemplate>
    </asp:TemplateField>

    <asp:TemplateField HeaderText="First Address" ItemStyle-CssClass="column-width-100">
        <ItemTemplate>
            <asp:Label ID="ClientListlblAddress1" runat="server" Text='<%# Eval("ADD1") %>' />
        </ItemTemplate>
    </asp:TemplateField>

</Columns>