<div class="col-md-12">
    <details>
        <summary>Grid 1: First Data Table</summary>
        <div class="table-responsive " style="overflow-y: auto; overflow-x: auto; max-height: 250px; min-height: 1px">
            <asp:GridView ID="GridView1" runat="server" CssClass="table table-bordered" AutoGenerateColumns="true"></asp:GridView>
        </div>
    </details>

    <details>
        <summary>Grid 2: Policy Data Table</summary>
        <div class="table-responsive " style="overflow-y: auto; overflow-x: auto; max-height: 250px; min-height: 1px">
            <asp:GridView ID="gridPolicyData" runat="server" AutoGenerateColumns="False" OnRowCommand="gvPolicyData_RowCommand" CssClass="table mt-4">
                <Columns>
                    <asp:BoundField DataField="POLICY_NO" HeaderText="Policy No" SortExpression="POLICY_NO" />
                    <asp:BoundField DataField="max_amt" HeaderText="Max Amount" SortExpression="max_amt" />
                    <asp:BoundField DataField="PREM_FREQ" HeaderText="Premium Frequency" SortExpression="PREM_FREQ" />
                    <asp:BoundField DataField="NEXT_DUE_DT" HeaderText="Next Due Date" SortExpression="NEXT_DUE_DT" />
                    <asp:BoundField DataField="COMPANY_CD" HeaderText="Company Code" SortExpression="COMPANY_CD" />
                    <asp:BoundField DataField="REGION_NAME" HeaderText="Region Name" SortExpression="REGION_NAME" />
                    <asp:BoundField DataField="ZONE_NAME" HeaderText="Zone Name" SortExpression="ZONE_NAME" />
                    <asp:BoundField DataField="RM_NAME" HeaderText="RM Name" SortExpression="RM_NAME" />
                    <asp:BoundField DataField="BRANCH_NAME" HeaderText="Branch Name" SortExpression="BRANCH_NAME" />
                    <asp:BoundField DataField="INVESTOR_NAME" HeaderText="Investor Name" SortExpression="INVESTOR_NAME" />
                    <asp:BoundField DataField="ADDRESS1" HeaderText="Address 1" SortExpression="ADDRESS1" />
                    <asp:BoundField DataField="ADDRESS2" HeaderText="Address 2" SortExpression="ADDRESS2" />
                    <asp:BoundField DataField="CITY_NAME" HeaderText="City" SortExpression="CITY_NAME" />
                    <asp:BoundField DataField="STATE_NAME" HeaderText="State" SortExpression="STATE_NAME" />
                    <asp:BoundField DataField="MOBILE" HeaderText="Mobile" SortExpression="MOBILE" />
                    <asp:BoundField DataField="PHONE" HeaderText="Phone" SortExpression="PHONE" />
                </Columns>
            </asp:GridView>
        </div>
    </details>
</div>