<div class="card" style="height: 480px;">
    <div class="card-body">
        <h4 class="card-title">Agents Searched Master:</h4>
        <asp:Label ID="lblAgentCodeSearchedMasterInfo" runat="server" Text='' Visible="true" />

        <div class="table-responsive " style="overflow-y: auto; overflow-x: auto; max-height: 300px;">
            <asp:GridView ID="agentsGridSearchedMaster" CssClass="table table-hover" runat="server">
                <Columns>

                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:CheckBox ID="chkHeaderSearchedMaster" runat="server" AutoPostBack="True" OnCheckedChanged="chkHeaderSearchedMaster_CheckedChanged" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="chkSelectSearchedMaster" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>


                    <asp:TemplateField HeaderText="Agent Code" ItemStyle-CssClass="column-width-100">
                        <ItemTemplate>
                            <asp:Label ID="lblAgentCodeSearchedMaster" runat="server" Text='<%# Eval("Agent_Code") %>' Visible="true" />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Exist Code" ItemStyle-CssClass="column-width-100">
                        <ItemTemplate>
                            <asp:Label ID="lblExistCodeSearchedMaster" runat="server" Text='<%# Eval("Exist_Code") %>' Visible="true" />
                        </ItemTemplate>
                    </asp:TemplateField>


                    <asp:TemplateField HeaderText="Agent Name" ItemStyle-CssClass="column-width-100">
                        <ItemTemplate>
                            <asp:Label ID="lblAgentNameSearchedMaster" runat="server" Text='<%# Eval("Agent_Name") %>' Visible="true" />
                        </ItemTemplate>
                    </asp:TemplateField>


                    <asp:TemplateField HeaderText="Address 1" ItemStyle-CssClass="column-width-100">
                        <ItemTemplate>
                            <asp:Label ID="lblAddress1SearchedMaster" runat="server" Text='<%# Eval("Address1") %>' Visible="true" />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Address 2" ItemStyle-CssClass="column-width-100">
                        <ItemTemplate>
                            <asp:Label ID="lblAddress2SearchedMaster" runat="server" Text='<%# Eval("Address2") %>' Visible="true" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Branch Code" ItemStyle-CssClass="column-width-100">
                        <ItemTemplate>
                            <asp:Label ID="lblSourceCodeSearchedMaster" runat="server" Text='<%# Eval("sourceid") %>' Visible="true" />
                        </ItemTemplate>
                    </asp:TemplateField>

                </Columns>
            </asp:GridView>
        </div>

        <p class="text-danger">Select client from this data sheet to include it into the data sheets on the right side.</p>
    </div>
</div>