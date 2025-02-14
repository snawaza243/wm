
<div style="width: 100%; overflow-x: auto; white-space: nowrap;" class="mt-6">
    <asp:GridView ID="new_mml_familyGridView" runat="server" CssClass="table table-hover" AutoGenerateColumns="false" OnRowCommand="gvFamily_RowCommand">
        <Columns>
            <asp:TemplateField HeaderText="#" ItemStyle-CssClass="column-width-50">
                <ItemTemplate>
                    <asp:Label ID="new_mml_lblSr" runat="server" Text='<%# Container.DisplayIndex + 1 %>' />
                </ItemTemplate>
            </asp:TemplateField>

           

            <asp:TemplateField HeaderText="Title">
                <ItemTemplate>
                    <asp:Label ID="new_mml_lblTitle" runat="server" Text='<%# Eval("investor_title") %>' Visible="true" />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Investor Name">
                <ItemTemplate>
                    <asp:Label ID="new_mml_lblInvestorName" runat="server" Text='<%# Eval("investor_name") %>' Visible="true" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Gender">
                <ItemTemplate>
                    <asp:Label ID="new_mml_lblGender" runat="server" Text='<%# Eval("gender") %>' Visible="true" />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Mobile">
                <ItemTemplate>
                    <asp:Label ID="new_mml_lblMobile" runat="server" Text='<%# Eval("mobile") %>' Visible="true" />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Email">
                <ItemTemplate>
                    <asp:Label ID="new_mml_lblEmail" runat="server" Text='<%# Eval("email") %>' Visible="true" />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="PAN">
                <ItemTemplate>
                    <asp:Label ID="new_mml_lblPAN" runat="server" Text='<%# Eval("pan") %>' Visible="true" />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Relation">
                <ItemTemplate>
                    <asp:Label ID="new_mml_lblRelation" runat="server" Text='<%# Eval("OUR_RELATIONSHIP") %>' Visible="true" />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="KYC">
                <ItemTemplate>
                    <asp:Label ID="new_mml_lblKYC" runat="server" Text='<%# Eval("KYC") %>' Visible="true" />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Guardian Name">
                <ItemTemplate>
                    <asp:Label ID="new_mml_lblGuardianName" runat="server" Text='<%# Eval("g_name") %>' Visible="true" />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Guardian PAN">
                <ItemTemplate>
                    <asp:Label ID="new_mml_lblGuardianPAN" runat="server" Text='<%# Eval("g_pan") %>' Visible="true" />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Approved">
                <ItemTemplate>
                    <asp:Label ID="new_mml_lblApproved" runat="server" Text='<%# Eval("approved") %>' Visible="true" />
                </ItemTemplate>
            </asp:TemplateField>


            <asp:TemplateField HeaderText="Aadhaar Card Number">
                <ItemTemplate>
                    <asp:Label ID="new_mml_lblAadhaarCardNumber" runat="server" Text='<%# Eval("AADHAR_CARD_NO") %>' Visible="true" />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Nominee">
                <ItemTemplate>
                <asp:Label ID="new_mml_lblIsNoinee" runat="server" Text='<%# Eval("is_nominee") %>' Visible="true" />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Allocation">
                <ItemTemplate>
                <asp:Label ID="new_mml_lblNomineePer" runat="server" Text='<%# Eval("nominee_per") %>' Visible="true" />
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>
    </asp:GridView>
</div>
