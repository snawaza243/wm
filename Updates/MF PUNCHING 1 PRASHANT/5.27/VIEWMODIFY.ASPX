<%-- TAB 2: VIEW/MODIFY --%>
    <div class="tab-pane fade" id="view_modify" role="tabpanel" aria-labelledby="view_modify_tab">
        <div class="row g-3">
            <div>
                <asp:UpdatePanel ID="UpdatePanelSearchAR" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <h4 class="card-title">Search AR</h4>

                        <div class="row g-3 mb-3">

                            <div class="col-md-4">
                                <asp:Label ID="lblFromDate" runat="server" CssClass="form-label"
                                    AssociatedControlID="fromDate" Text="From Date" />
                                <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                    <asp:TextBox ID="fromDate" runat="server" CssClass="form-control date-input"
                                        Placeholder="dd/mm/yyyy" />
                                    <div class="input-group-addon">
                                        <span class="glyphicon glyphicon-th"></span>
                                    </div>
                                </div>
                            </div>

                            <div class="col-md-4">
                                <asp:Label ID="lblToDate" runat="server" CssClass="form-label"
                                    AssociatedControlID="toDate" Text="To Date" />
                                <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                    <asp:TextBox ID="toDate" runat="server" CssClass="form-control date-input"
                                        Placeholder="dd/mm/yyyy" />
                                    <div class="input-group-addon">
                                        <span class="glyphicon glyphicon-th"></span>
                                    </div>
                                </div>
                            </div>



                            <div class="col-md-4">
                                <asp:Label ID="lblOrderBy" runat="server" CssClass="form-label"
                                    AssociatedControlID="orderBy" Text="Order By" />
                                <asp:DropDownList ID="orderBy" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="" Text="Select" />
                                    <asp:ListItem Value="panNo" Text="PAN No." />
                                    <asp:ListItem Value="trNo" Text="TR No." />
                                    <asp:ListItem Value="uniqueClientCode" Text="Unique Client Code" />
                                    <asp:ListItem Value="anaCode" Text="ANA Code" />
                                    <asp:ListItem Value="chequeNo" Text="Cheque No" />
                                    <asp:ListItem Value="appNo" Text="App No" />
                                </asp:DropDownList>
                            </div>

                            <div class="col-md-4">
                                <asp:Label ID="lblAscendingDescending" runat="server" CssClass="form-label"
                                    AssociatedControlID="ascending" Text="Order" />
                                <div class="d-flex align-items-center gap-3">
                                    <div class="form-label">
                                        <asp:RadioButton ID="ascending" runat="server" GroupName="order"
                                            Text="Ascending" />
                                    </div>
                                    <div class="form-label">
                                        <asp:RadioButton ID="descending" runat="server" GroupName="order"
                                            Text="Descending" />
                                    </div>
                                </div>
                            </div>

                            <div class="col-md-4">
                                <asp:Label ID="lblPanNo" runat="server" CssClass="form-label"
                                    AssociatedControlID="panNo" Text="PAN No." />
                                <asp:TextBox ID="TextBox15" runat="server" CssClass="form-control" Placeholder="PAN No."
                                    onblur="handleBlur(this)" oninput="validatePanInput(this)" MaxLength="10" />
                            </div>

                            <div class="col-md-4">
                                <asp:Label ID="lblTrNo" runat="server" CssClass="form-label" AssociatedControlID="trNo"
                                    Text="TR No." />
                                <asp:TextBox ID="trNo" runat="server" CssClass="form-control" Placeholder="TR No." />
                            </div>

                            <div class="col-md-4">
                                <asp:Label ID="lblUniqueClientCode" runat="server" CssClass="form-label"
                                    AssociatedControlID="uniqueClientCode" Text="Unique Client Code" />
                                <asp:TextBox ID="uniqueClientCode" runat="server" CssClass="form-control"
                                    Placeholder="Unique Client Code" />
                            </div>

                            <div class="col-md-4">
                                <asp:Label ID="lblAnaCode" runat="server" CssClass="form-label"
                                    AssociatedControlID="anaCode" Text="ANA Code" />
                                <asp:TextBox ID="anaCode" runat="server" CssClass="form-control"
                                    Placeholder="ANA Code" />
                            </div>

                            <div class="col-md-4">
                                <asp:Label ID="lblChequeNo" runat="server" CssClass="form-label"
                                    AssociatedControlID="chequeNo" Text="Cheque No" />
                                <asp:TextBox ID="chequeNo" runat="server" CssClass="form-control"
                                    Placeholder="Cheque No" />
                            </div>

                            <div class="col-md-4">
                                <asp:Label ID="lblAppNo" runat="server" CssClass="form-label"
                                    AssociatedControlID="appNo" Text="App No" />
                                <asp:TextBox ID="appNo" runat="server" CssClass="form-control" Placeholder="App No" />
                            </div>

                            <asp:UpdatePanel ID="UpdatePanelButton" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Button ID="btnSearchAR" runat="server" CssClass="btn btn-primary" Text="View"
                                        OnClick="btnSearch_Click" />

                                </ContentTemplate>

                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btnSearchAR" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
        </div>

        <asp:Label ID="Label20" runat="server" CssClass="text-danger"></asp:Label>



        <div class="row g-3 mb-3" id="additionalInfo" runat="server">
            <div class="col-md-4">
                <asp:Label ID="lblSearchInfo" runat="server" CssClass="form-label"
                    Text="Searching by Investor Name/PAN/AR DATE etc" />
                <div class="input-group">
                    <%-- <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" />
                    <asp:Button ID="btnFind" runat="server" CssClass="btn btn-outline-primary" Text="Find" />
                    --%>
                </div>
            </div>
            <div class="col-md-4">
                <span class="text-danger">*</span> To search any data click on the column to be searched and press find
            </div>
            <div class="col-md-4">
                <h4>Searched by -> Target scheme name</h4>
            </div>
        </div>



        <asp:UpdatePanel ID="UpdatePanelgrid" runat="server" UpdateMode="Conditional">
            <ContentTemplate>

                <div class="table-responsive">
                    <asp:GridView ID="tableSearchResults" CssClass="table table-bordered" runat="server"
                        AutoGenerateColumns="False" OnRowCommand="tableSearchResultsTran_RowCommand">
                        <HeaderStyle CssClass="thead-dark" />
                        <Columns>

                            <asp:TemplateField HeaderText="Action" ItemStyle-CssClass="column-width-50">
                                <ItemTemplate>
                                    <asp:Button ID="Button1" runat="server" Text="Select" CommandName="SelectRow"
                                        CommandArgument='<%# Eval("TRAN_CODE") %>' />

                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="AR NO">
                                <ItemTemplate>
                                    <asp:Label ID="lblARNO" runat="server" Text='<%# Eval("TRAN_CODE") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Country" Visible="false">
                                <ItemTemplate>
                                    <asp:HiddenField ID="HiddenField2" runat="server"
                                        Value='<%# Eval("country_id") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Investor">
                                <ItemTemplate>
                                    <asp:Label ID="lblInvestorName" runat="server"
                                        Text='<%# Eval("INVESTOR_NAME") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="PAN No">
                                <ItemTemplate>
                                    <asp:Label ID="lblPAN" runat="server" Text='<%# Eval("PAN") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="City Name">
                                <ItemTemplate>
                                    <asp:Label ID="lblCity" runat="server" Text='<%# Eval("CITY_NAME") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Address1">
                                <ItemTemplate>
                                    <asp:Label ID="lblAddress1" runat="server" Text='<%# Eval("ADDRESS1") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Address2">
                                <ItemTemplate>
                                    <asp:Label ID="lblAddress2" runat="server" Text='<%# Eval("ADDRESS2") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="PIN">
                                <ItemTemplate>
                                    <asp:Label ID="lblPIN" runat="server" Text='<%# Eval("PINCODE") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Mobile">
                                <ItemTemplate>
                                    <asp:Label ID="lblMobile" runat="server" Text='<%# Eval("MOBILE") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Aadhar no.">
                                <ItemTemplate>
                                    <asp:Label ID="lblAdhno" runat="server" Text='<%# Eval("AADHAR_CARD_NO") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="City">
                                <ItemTemplate>
                                    <asp:Label ID="lblcityname" runat="server" Text='<%# Eval("CITY_NAME") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Email">
                                <ItemTemplate>
                                    <asp:Label ID="lblEmail" runat="server" Text='<%# Eval("EMAIL") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="DOB">
                                <ItemTemplate>
                                    <asp:Label ID="lblDOB" runat="server"
                                        Text='<%# Eval("DOB" , "{0:dd/MM/yyyy}") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>



                            <asp:TemplateField HeaderText="CLIENT CODE">
                                <ItemTemplate>
                                    <asp:Label ID="lblCLIENT" runat="server" Text='<%# Eval("CLIENT_CODE") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="BUSINESS CODE">
                                <ItemTemplate>
                                    <asp:Label ID="lblbusicodem" runat="server" Text='<%# Eval("BUSINESS_RMCODE") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="ACOUNT HOLDER CODE">
                                <ItemTemplate>
                                    <asp:Label ID="lblHOLDERCODE" runat="server" Text='<%# Eval("AC_HOLDER_CODE") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="AMC CODE">
                                <ItemTemplate>
                                    <asp:Label ID="lblAMCview" runat="server" Text='<%# Eval("MUT_CODE") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="AMC NAME">
                                <ItemTemplate>
                                    <asp:Label ID="Label5" runat="server" Text='<%# Eval("MUT_NAME") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="TranDate">
                                <ItemTemplate>
                                    <asp:Label ID="lblTranDate" runat="server"
                                        Text='<%# Eval("TR_DATE", "{0:dd/MM/yyyy}") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="TranType">
                                <ItemTemplate>
                                    <asp:Label ID="lblTranType" runat="server" Text='<%# Eval("TRAN_TYPE") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="App No">
                                <ItemTemplate>
                                    <asp:Label ID="lblAppNoModify" runat="server" Text='<%# Eval("APP_NO") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="Mode">
                                <ItemTemplate>
                                    <asp:Label ID="lblMode" runat="server" Text='<%# Eval("PAYMENT_MODE") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="Chq No">
                                <ItemTemplate>
                                    <asp:Label ID="lblChqNo" runat="server" Text='<%# Eval("CHEQUE_NO") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="ChqDt">
                                <ItemTemplate>
                                    <asp:Label ID="lblChqDt" runat="server"
                                        Text='<%# Eval("CHEQUE_DATE", "{0:dd/MM/yyyy}") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="drpdt">
                                <ItemTemplate>
                                    <asp:Label ID="lbldrpDt" runat="server"
                                        Text='<%# Eval("CHEQUE_DATE", "{0:dd/MM/yyyy}") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Amount">
                                <ItemTemplate>
                                    <asp:Label ID="lblAmount" runat="server" Text='<%# Eval("AMOUNT") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="SIP/STP">
                                <ItemTemplate>
                                    <asp:Label ID="lblSIPSTPview" runat="server" Text='<%# Eval("SIP_TYPE") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="LeadNo">
                                <ItemTemplate>
                                    <asp:Label ID="lblLeadNo" runat="server" Text='<%# Eval("LEAD_NO") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="Lead Name">
                                <ItemTemplate>
                                    <asp:Label ID="lblLeadName" runat="server" Text='<%# Eval("LEAD_NAME") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="Bank Name">
                                <ItemTemplate>
                                    <asp:Label ID="lblBankName" runat="server" Text='<%# Eval("BANK_NAME") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="RM">
                                <ItemTemplate>
                                    <asp:Label ID="lblRM" runat="server" Text='<%# Eval("RMCODE") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>



                            <asp:TemplateField HeaderText="Folio No">
                                <ItemTemplate>
                                    <asp:Label ID="lblFolioNo" runat="server" Text='<%# Eval("FOLIO_NO") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="DocId">
                                <ItemTemplate>
                                    <asp:Label ID="lblDocId" runat="server" Text='<%# Eval("DOC_ID") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="Inves">
                                <ItemTemplate>
                                    <asp:Label ID="lblInvestorType" runat="server"
                                        Text='<%# Eval("INVESTOR_TYPE") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="Target">
                                <ItemTemplate>
                                    <asp:Label ID="lblTargetSwitchScheme" runat="server"
                                        Text='<%# Eval("TARGET_SWITCH_SCHEME") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="Target Name">
                                <ItemTemplate>
                                    <asp:Label ID="lblTargetName" runat="server" Text='<%# Eval("GOAL") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="Switch">
                                <ItemTemplate>
                                    <asp:Label ID="lblSwitchScheme" runat="server"
                                        Text='<%# Eval("SWITCH_SCHEME") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="Remarks">
                                <ItemTemplate>
                                    <asp:Label ID="lblRemark" runat="server" Text='<%# Eval("REMARK") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="Scheme Name">
                                <ItemTemplate>
                                    <asp:Label ID="lblSchemeName" runat="server" Text='<%# Eval("SCH_NAME") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Switch Scheme Name">
                                <ItemTemplate>
                                    <asp:Label ID="lblSwitchSchemeName" runat="server"
                                        Text='<%# Eval("sw_sch_name") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Scheme Code">
                                <ItemTemplate>
                                    <asp:Label ID="lblSchemeCode" runat="server" Text='<%# Eval("SCH_CODE") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Base Tran Code" Visible="false">
                                <ItemTemplate>
                                    <asp:HiddenField ID="hfBaseTranCode" runat="server"
                                        Value='<%# Eval("BASE_TRAN_CODE") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>



                            <asp:TemplateField HeaderText="Base Tran Code" Visible="false">
                                <ItemTemplate>
                                    <asp:HiddenField ID="HiddenField1" runat="server"
                                        Value='<%# Eval("BASE_TRAN_CODE") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="INV CODE MODIFY" Visible="false">
                                <ItemTemplate>
                                    <asp:HiddenField ID="hfINVCODEMOD" runat="server"
                                        Value='<%# Eval("CLIENT_CODE") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="PAN No TEMP">
                                <ItemTemplate>
                                    <asp:Label ID="lblPANTEMP" runat="server" Text='<%# Eval("PANNO") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Branch Name">
                                <ItemTemplate>
                                    <asp:Label ID="lblBranchName" runat="server" Text='<%# Eval("BRANCH_NAME") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>
                    </asp:GridView>


                </div>

            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="tableSearchResults" EventName="RowCommand" />
            </Triggers>
        </asp:UpdatePanel>


    </div>

    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div id="tranDetails" runat="server">
                <h4 class="card-title">Tran Details
                </h4>

                <div class="row g-3 mb-3">
                    <div class="col-md-3">
                        <label for="businessCode" class="form-label">Business Code</label>
                        <asp:TextBox ID="businessCodeV" runat="server" CssClass="form-control"
                            Placeholder="Business Code" onkeypress="return handleKeyPress(event)"
                            onchange="handleChange(event)" OnBlur="TxtBusiCodeM_LostFocus"></asp:TextBox>
                    </div>

                    <div class="col-md-3">
                        <label for="TXTbranchName" class="form-label">Branch Name</label>
                        <asp:TextBox ID="BranchNameView" runat="server" CssClass="form-control" ReadOnly="true">
                        </asp:TextBox>
                    </div>

                    <div class="col-md-3">
                        <label for="txtClientCode" class="form-label">Client Code</label>
                        <asp:TextBox ID="txtClientCode" runat="server" CssClass="form-control" ReadOnly="true">
                        </asp:TextBox>
                    </div>



                    <div class="col-md-3">
                        <label for="accountHolder" class="form-label">Account Holder</label>
                        <asp:TextBox ID="accountHolderV" runat="server" CssClass="form-control"
                            Placeholder="Account Holder"></asp:TextBox>
                    </div>

                    <div class="col-md-4">
                        <label for="holderCodeVL" class="form-label">A/C Holder Code</label>
                        <asp:TextBox ID="holderCodeV" runat="server" CssClass="form-control" ReadOnly="true" />
                    </div>

                    <div class="col-md-4">
                        <label for="panNo" class="form-label">PAN NO</label>
                        <asp:TextBox ID="panNo" runat="server" CssClass="form-control" onblur="handleBlur(this)"
                            oninput="validatePanInput(this)" MaxLength="10" Placeholder="PAN NO"></asp:TextBox>
                    </div>

                    <div class="col-md-4">
                        <label for="amcSelect" class="form-label">AMC</label>
                        <asp:DropDownList ID="amcSelect" runat="server" CssClass="form-select">
                            <asp:ListItem Text="Select" Value=""></asp:ListItem>
                            <asp:ListItem Text="AMC 1" Value="amc1"></asp:ListItem>
                            <asp:ListItem Text="AMC 2" Value="amc2"></asp:ListItem>

                        </asp:DropDownList>
                    </div>

                    <div class="col-md-4">
                        <label for="SchemeName" class="form-label">SCHEME</label>
                        <asp:TextBox ID="SchemeName" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>


                    <div class="col-md-4">
                        <label for="transactionDate" class="form-label">Transaction Date</label>
                        <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                            <asp:TextBox ID="TextBox11" runat="server" CssClass="form-control date-input"
                                Placeholder="dd/mm/yyyy"></asp:TextBox>
                            <div class="input-group-addon">
                                <span class="glyphicon glyphicon-th"></span>
                            </div>
                        </div>
                    </div>

                    <div class="col-md-4">
                        <label for="transactionType" class="form-label">Transaction Type</label>
                        <asp:DropDownList ID="DropDownList4" runat="server" CssClass="form-select"
                            OnSelectedIndexChanged="trantypechangeINupdate" AutoPostBack="true">
                            <asp:ListItem Text="Select" Value=""></asp:ListItem>
                            <asp:ListItem Text="PURCHASE" Value="PURCHASE"></asp:ListItem>
                            <asp:ListItem Text="SWITCH IN" Value="SWITCH IN"></asp:ListItem>

                        </asp:DropDownList>
                    </div>

                    <div class="col-md-4">
                        <label for="bankName" class="form-label">Bank Name</label>
                        <asp:DropDownList ID="bankName" runat="server" CssClass="form-select">
                            <asp:ListItem Text="Select" Value=""></asp:ListItem>
                            <asp:ListItem Text="Bank 1" Value="bank1"></asp:ListItem>
                            <asp:ListItem Text="Bank 2" Value="bank2"></asp:ListItem>

                        </asp:DropDownList>
                    </div>

                    <div class="col-md-4">
                        <label for="folio-no" class="form-label">From Switch Folio</label>
                        <asp:TextBox ID="TextBox12" runat="server" CssClass="form-control" Placeholder="Folio No"
                            Enabled="false"></asp:TextBox>
                    </div>

                    <div class="col-md-4">
                        <label for="folio-no" class="form-label">From Switch Scheme</label>
                        <asp:TextBox ID="switchSchemeName" runat="server" CssClass="form-control" Placeholder="Folio No"
                            Enabled="false"></asp:TextBox>
                    </div>

                    <div class="col-md-4">
                        <label for="applicationNo" class="form-label">Application No</label>
                        <asp:TextBox ID="TextBox13" runat="server" CssClass="form-control" Placeholder="Application No">
                        </asp:TextBox>
                    </div>

                    <div class="col-md-4">
                        <label for="amount" class="form-label">Amount</label>
                        <asp:TextBox ID="TextBox14" runat="server" CssClass="form-control" Placeholder="Amount"
                            oninput="validateAmountInput(this)" onblur="handleBlur(this)"></asp:TextBox>
                    </div>

                    <div class="col-md-4">
                        <label for="sipStp" class="form-label">SIP/STP</label>
                        <asp:DropDownList ID="sipStp" runat="server" CssClass="form-select"
                            OnSelectedIndexChanged="ddlsipstpInupdate" AutoPostBack="true">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                            <asp:ListItem Text="REGULAR" Value="REGULAR"></asp:ListItem>
                            <asp:ListItem Text="SIP" Value="SIP"></asp:ListItem>
                            <asp:ListItem Text="STP" Value="STP"></asp:ListItem>

                        </asp:DropDownList>
                    </div>



                    <div class="col-md-4">
                        <label for="dropDate" class="form-label">Drop Date</label>
                        <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                            <asp:TextBox ID="dropDat" runat="server" CssClass="form-control date-input"
                                Placeholder="dd/mm/yyyy"></asp:TextBox>
                            <div class="input-group-addon">
                                <span class="glyphicon glyphicon-th"></span>
                            </div>
                        </div>
                    </div>

                    <div class="col-md-4">
                        <label for="frequency" class="form-label">Frequency</label>
                        <asp:DropDownList ID="frequency" runat="server" CssClass="form-select"
                            oninput="calculateSIPEndDateForUpdate()">
                            <asp:ListItem Value="301" Text="Select Frequency" />
                            <asp:ListItem Value="208" Text="Daily" />
                            <asp:ListItem Value="173" Text="Weekly" />
                            <asp:ListItem Value="174" Text="Fortnightly" />
                            <asp:ListItem Value="175" Text="Monthly" />
                            <asp:ListItem Value="176" Text="Quarterly" />
                            <asp:ListItem Value="301" Text="Yearly" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-4">
                        <label for="installmentsNos" class="form-label">No of Installments</label>
                        <asp:TextBox ID="installmentsNos" runat="server" CssClass="form-control"
                            Placeholder="No of Installments" onblur="handleBlur(this)"
                            oninput="calculateSIPEndDateForUpdate()" onkeypress="allowOnlyNumeric(this)"></asp:TextBox>
                    </div>

                    <div class="col-md-3">
                        <div class="form-label">
                            <asp:CheckBox ID="cobCase" runat="server" />
                            <label class="form-check-label" for="cobCase">COB Case</label>
                        </div>
                    </div>



                    <div class="col-md-3">
                        <div class="form-label">
                            <label class="form-check-label" for="swpCase">
                                <asp:CheckBox ID="swpCase" runat="server" />
                                SWP Case
                            </label>
                        </div>
                    </div>

                    <div class="col-md-3">
                        <div class="form-label">
                            <label class="form-check-label" for="freedomSip">
                                <asp:CheckBox ID="freedomSip" runat="server" />
                                Freedom SIP
                            </label>
                        </div>
                    </div>

                    <div class="col-md-3">
                        <div class="form-label">
                            <label class="form-check-label" for="or99Years">
                                <asp:RadioButton ID="or99Years" runat="server" GroupName="duration" Value="99years"
                                    oninput="calculateSIPEndDateForUpdate()" />
                                OR 99 Years or more
                            </label>
                        </div>
                    </div>


                    <div class="col-md-6">
                        <h5 class="mb-3">Select Scheme</h5>

                        <!-- UpdatePanel for Button and Checkbox -->
                        <asp:UpdatePanel ID="UpdatePanelScheme" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>

                                <!-- Find Scheme Button -->
                                <div class="container">
                                    <div class="row mb-3">
                                        <!-- Find Scheme Button -->
                                        <div class="col-auto">
                                            <asp:Button ID="findSchemeButton" runat="server" CssClass="btn btn-primary"
                                                Text="Find Scheme" OnClick="btnfindSchemeButton_Click" />
                                        </div>

                                        <!-- SIP Start Date -->
                                        <div class="col-md-6">
                                            <asp:Label ID="lblSIPStartDateupdate" runat="server" CssClass="form-label"
                                                AssociatedControlID="txtSIPStartDateupdate" Text="SIP Start Date"
                                                Visible="false" />
                                            <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                <asp:TextBox ID="txtSIPStartDateupdate" runat="server"
                                                    CssClass="form-control date-input" placeholder="dd/mm/yyyy"
                                                    onblur="calculateSIPEndDateForUpdate()" Visible="false" />
                                                <div class="input-group-addon">
                                                    <span class="glyphicon glyphicon-th"></span>
                                                </div>
                                            </div>
                                        </div>


                                    </div>

                                    <div class="row mb-3">
                                        <!-- Switch Scheme Checkbox -->
                                        <div class="col-md-3">
                                            <asp:Label class="form-check-label" for="SwitchScheme" Visible="false"
                                                ID="lblswisch" runat="server">
                                                <asp:CheckBox ID="SwitchChebox" runat="server" Visible="false" />
                                                Switch Scheme
                                            </asp:Label>
                                        </div>

                                        <!-- SIP End Date -->
                                        <div class="col-md-6">
                                            <asp:Label ID="lblSIPEndDateupdate" runat="server" CssClass="form-label"
                                                AssociatedControlID="txtSIPEndDateupdate" Text="SIP End Date"
                                                Visible="false" />
                                            <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                <asp:TextBox ID="txtSIPEndDateupdate" runat="server"
                                                    CssClass="form-control date-input" placeholder="dd/mm/yyyy"
                                                    Visible="false" />
                                                <div class="input-group-addon">
                                                    <span class="glyphicon glyphicon-th"></span>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="form-label">
                                            <asp:RadioButton ID="freshINupdate" runat="server" GroupName="sIp"
                                                Text="Fresh" Checked="true" Visible="false" />
                                        </div>
                                        <div class="form-label">
                                            <asp:RadioButton ID="renewalInUpdate" runat="server" GroupName="sIp"
                                                Text="Renewal" Visible="false" />
                                        </div>



                                    </div>
                                </div>



                            </ContentTemplate>

                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="findSchemeButton" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="SwitchChebox" EventName="CheckedChanged" />
                            </Triggers>

                        </asp:UpdatePanel>
                    </div>


                    <script>
                        function calculateSIPEndDateForUpdate() {
                            const startDateInput = document.getElementById("<%= txtSIPStartDateupdate.ClientID %>");
                            const endDateInput = document.getElementById("<%= txtSIPEndDateupdate.ClientID %>");
                            const installmentsInput = document.getElementById("<%= installmentsNos.ClientID %>");
                            const frequencyInput = document.getElementById("<%= frequency.ClientID %>");
                            const rdo99Years = document.getElementById("<%= or99Years.ClientID %>"); // "99 Years or more" radio button

                            // Get values from controls
                            const startDateValue = startDateInput.value;
                            const installmentsValue = parseInt(installmentsInput.value, 10);
                            const frequencyValue = parseInt(frequencyInput.value, 10);

                            // If "99 Years or more" is selected, set End Date to 01/12/2099
                            if (rdo99Years.checked && startDateValue) {
                                endDateInput.value = "01/12/2099";
                                return;
                            }

                            // Validate inputs
                            if (!startDateValue || isNaN(installmentsValue) || isNaN(frequencyValue) || frequencyValue === 0) {
                                endDateInput.value = '';
                                return;
                            }

                            // Parse SIP Start Date (dd/mm/yyyy)
                            const [startDay, startMonth, startYear] = startDateValue.split('/').map(Number);
                            const startDate = new Date(startYear, startMonth - 1, startDay);

                            if (isNaN(startDate)) {
                                alert("Invalid SIP Start Date.");
                                endDateInput.value = '';
                                return;
                            }

                            // Calculate SIP End Date based on installments and frequency
                            let endDate = new Date(startDate);

                            switch (frequencyValue) {
                                case 208: // Daily
                                    endDate.setDate(endDate.getDate() + (installmentsValue - 1));
                                    break;
                                case 173: // Weekly
                                    endDate.setDate(endDate.getDate() + (installmentsValue - 1) * 7);
                                    break;
                                case 174: // Fortnightly
                                    endDate.setDate(endDate.getDate() + (installmentsValue - 1) * 14);
                                    break;
                                case 175: // Monthly
                                    endDate.setMonth(endDate.getMonth() + installmentsValue);
                                    endDate.setDate(0); // Month-end handling
                                    break;
                                case 176: // Quarterly
                                    endDate.setMonth(endDate.getMonth() + installmentsValue * 3);
                                    endDate.setDate(0);
                                    break;
                                case 301: // Yearly
                                    endDate.setFullYear(endDate.getFullYear() + installmentsValue);
                                    break;
                                default:
                                    alert("Invalid frequency selected.");
                                    return;
                            }

                            // Format SIP End Date (dd/mm/yyyy)
                            const formattedDate = [
                                String(endDate.getDate()).padStart(2, '0'),
                                String(endDate.getMonth() + 1).padStart(2, '0'),
                                endDate.getFullYear()
                            ].join('/');

                            // Set SIP End Date
                            endDateInput.value = formattedDate;
                        }

                    </script>


                    <asp:Panel ID="SchemeSearchPanel2" runat="server" CssClass="panel panel-default draggable"
                        Style="display: none; position: fixed; top: 20%; left: 35%; width: 30%; max-height: 400px; overflow: auto; background-color: white; border: 1px solid #ccc; z-index: 5000;">

                        <div class="panel-heading"
                            style="cursor: move; display: flex; justify-content: space-between; align-items: center;">
                            <h3 class="panel-title">Scheme Search</h3>
                            <button type="button" class="close" onclick="closeSchemeSearchPanel2()"
                                style="font-size: 18px;">&times;</button>
                        </div>

                        <div class="panel panel-primary">
                            <div class="panel-body">

                                <!-- UpdatePanel starts here -->
                                <asp:UpdatePanel ID="UpdatePanelSchemeSearch" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>

                                        <asp:Label ID="lblSearchScheme" runat="server" Text="Find:"
                                            CssClass="control-label" />
                                        <asp:TextBox ID="txtSearchScheme" runat="server" CssClass="form-control" />

                                        <div style="text-align: center; margin-top: 10px;">
                                            <!-- Search Button inside UpdatePanel -->
                                            <asp:Button ID="Button9" runat="server" Text="Search"
                                                CssClass="btn btn-primary btn-sm" OnClick="btnSearchScheme_Click"
                                                Style="margin-top: 5px;" />
                                            <button type="button" class="btn btn-secondary btn-sm"
                                                onclick="closeSchemeSearchPanel2()"
                                                style="margin-left: 5px;">Close</button>
                                        </div>

                                        <div class="table-responsive">
                                            <!-- GridView inside UpdatePanel -->
                                            <asp:GridView ID="SchemeGrid2" CssClass="table table-bordered"
                                                runat="server" AutoGenerateColumns="false"
                                                OnRowCommand="SchemeGrid2_RowCommand">
                                                <HeaderStyle CssClass="thead-dark" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Action"
                                                        ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <!-- Select Button inside UpdatePanel -->
                                                            <asp:Button ID="btnSelectScheme2" runat="server"
                                                                Text="Select" CommandName="SelectScheme"
                                                                CommandArgument='<%# Eval("sch_code") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Scheme Code">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSchemeCode2" runat="server"
                                                                Text='<%# Eval("sch_code") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Scheme Name">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSchemeName2" runat="server"
                                                                Text='<%# Eval("sch_name") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Short Name">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblShortName2" runat="server"
                                                                Text='<%# Eval("SHORT_NAME") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="AMC Name">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblAMCName2" runat="server"
                                                                Text='<%# Eval("mut_name") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>

                                    </ContentTemplate>

                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="Button9" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="SchemeGrid2" EventName="RowCommand" />
                                    </Triggers>

                                </asp:UpdatePanel>


                            </div>
                        </div>
                    </asp:Panel>

                    <script type="text/javascript">
                        function closeSchemeSearchPanel2() {
                            document.getElementById('<%= SchemeSearchPanel2.ClientID %>').style.display = 'none';
                        }
                    </script>



                    <div class="col-md-6">
                        <div class="row g-3">
                            <div id="paymentMethodContainer_view" class="col-md-4">
                                <div class="form-label">
                                    <asp:RadioButton ID="cheque_view" runat="server" GroupName="paymentMethod_view"
                                        Text="Cheque" InputAttributes-Value="cheque_view" />
                                </div>
                                <div class="form-label">
                                    <asp:RadioButton ID="draft_view" runat="server" GroupName="paymentMethod_view"
                                        Text="Draft" InputAttributes-Value="draft_view" />
                                </div>
                                <div class="form-label">
                                    <asp:RadioButton ID="ecs_view" runat="server" GroupName="paymentMethod_view"
                                        Text="ECS" InputAttributes-Value="ecs_view" />
                                </div>
                                <div class="form-label">
                                    <asp:RadioButton ID="cash_view" runat="server" GroupName="paymentMethod_view"
                                        Text="Cash" InputAttributes-Value="cash_view" />
                                </div>
                                <div class="form-label">
                                    <asp:RadioButton ID="others_view" runat="server" GroupName="paymentMethod_view"
                                        Text="Others" InputAttributes-Value="others_view" />
                                </div>
                                <div class="form-label">
                                    <asp:RadioButton ID="rtgs_view" runat="server" GroupName="paymentMethod_view"
                                        Text="RTGS" InputAttributes-Value="rtgs_view" />
                                </div>
                                <div class="form-label">
                                    <asp:RadioButton ID="fund_view" runat="server" GroupName="paymentMethod_view"
                                        Text="Fund Transfer" InputAttributes-Value="neft_view" />
                                </div>
                            </div>

                            <!-- Cheque Details -->
                            <div class="col-md-4 payment-detail-view" id="chequeDetails_view" style="display: none;">
                                <asp:Label ID="Label6" runat="server" CssClass="form-label" Text="Cheque No" />
                                <span class="text-danger">*</span>
                                <asp:TextBox ID="TextBox2" runat="server" CssClass="form-control" />
                                <asp:Label ID="Label7" runat="server" CssClass="form-label" Text="Cheque Dated" />
                                <span class="text-danger">*</span>
                                <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                    <asp:TextBox ID="TextBox3" runat="server" CssClass="form-control date-input"
                                        placeholder="dd/mm/yyyy" />
                                    <div class="input-group-addon">
                                        <span class="glyphicon glyphicon-th"></span>
                                    </div>
                                </div>
                            </div>

                            <!-- Draft Details -->
                            <div id="draftDetails_view" class="col-md-4 payment-detail-view" style="display: none;">
                                <asp:Label ID="Label8" runat="server" CssClass="form-label" Text="Draft No" />
                                <asp:TextBox ID="TextBox4" runat="server" CssClass="form-control" />
                                <asp:Label ID="Label9" runat="server" CssClass="form-label" Text="Draft Date" />
                                <span class="text-danger">*</span>
                                <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                    <asp:TextBox ID="TextBox5" runat="server" CssClass="form-control date-input"
                                        placeholder="dd/mm/yyyy" />
                                    <div class="input-group-addon">
                                        <span class="glyphicon glyphicon-th"></span>
                                    </div>
                                </div>
                            </div>

                            <!-- RTGS Details -->
                            <div id="rtgsDetails_view" class="col-md-4 payment-detail-view" style="display: none;">
                                <asp:Label ID="Label10" runat="server" CssClass="form-label"
                                    Text="RTGS Transaction No" />
                                <asp:TextBox ID="TextBox6" runat="server" CssClass="form-control" />
                                <asp:Label ID="Label11" runat="server" CssClass="form-label" Text="RTGS Date" />
                                <span class="text-danger">*</span>
                                <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                    <asp:TextBox ID="TextBox7" runat="server" CssClass="form-control date-input"
                                        placeholder="dd/mm/yyyy" />
                                    <div class="input-group-addon">
                                        <span class="glyphicon glyphicon-th"></span>
                                    </div>
                                </div>
                            </div>

                            <!-- NEFT/Fund Transfer Details -->
                            <div id="neftDetails_view" class="col-md-4 payment-detail-view" style="display: none;">
                                <asp:Label ID="Label12" runat="server" CssClass="form-label" Text="Fund Transfer No" />
                                <asp:TextBox ID="TextBox8" runat="server" CssClass="form-control" />
                                <asp:Label ID="Label13" runat="server" CssClass="form-label"
                                    Text="Fund Transfer Date" />
                                <span class="text-danger">*</span>
                                <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                    <asp:TextBox ID="TextBox9" runat="server" CssClass="form-control date-input"
                                        placeholder="dd/mm/yyyy" />
                                    <div class="input-group-addon">
                                        <span class="glyphicon glyphicon-th"></span>
                                    </div>
                                </div>
                            </div>

                            <!-- ECS Details -->
                            <div id="ecsDetails_view" class="col-md-4 payment-detail-view" style="display: none;">
                                <asp:Label ID="Label14" runat="server" CssClass="form-label" Text="ECS Reference No" />
                                <asp:TextBox ID="TextBox10" runat="server" CssClass="form-control" />
                                <asp:Label ID="Label15" runat="server" CssClass="form-label" Text="ECS Date" />
                                <span class="text-danger">*</span>
                                <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                    <asp:TextBox ID="TextBox17" runat="server" CssClass="form-control date-input"
                                        placeholder="dd/mm/yyyy" />
                                    <div class="input-group-addon">
                                        <span class="glyphicon glyphicon-th"></span>
                                    </div>
                                </div>
                            </div>

                            <!-- Cash Payment Details -->
                            <div id="cashDetails_view" class="col-md-4 payment-detail-view" style="display: none;">
                                <asp:Label ID="Label16" runat="server" CssClass="form-label" Text="Cash Amount" />
                                <asp:TextBox ID="TextBox18" runat="server" CssClass="form-control" />
                                <asp:Label ID="Label17" runat="server" CssClass="form-label" Text="Cash Payment Date" />
                                <span class="text-danger">*</span>
                                <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                    <asp:TextBox ID="TextBox19" runat="server" CssClass="form-control date-input"
                                        placeholder="dd/mm/yyyy" />
                                    <div class="input-group-addon">
                                        <span class="glyphicon glyphicon-th"></span>
                                    </div>
                                </div>
                            </div>

                            <!-- Others Payment Details -->
                            <div id="othersDetails_view" class="col-md-4 payment-detail-view" style="display: none;">
                                <asp:Label ID="Label18" runat="server" CssClass="form-label"
                                    Text="Others Reference No" />
                                <asp:TextBox ID="TextBox20" runat="server" CssClass="form-control" />
                                <asp:Label ID="Label19" runat="server" CssClass="form-label" Text="Payment Date" />
                                <span class="text-danger">*</span>
                                <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                    <asp:TextBox ID="TextBox21" runat="server" CssClass="form-control date-input"
                                        placeholder="dd/mm/yyyy" />
                                    <div class="input-group-addon">
                                        <span class="glyphicon glyphicon-th"></span>
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>



                    <asp:Panel ID="popupPanel" runat="server" CssClass="panel panel-default draggable"
                        Style="display: none; position: fixed; top: 20%; left: 35%; width: 30%; max-height: 400px; overflow: auto; background-color: white; border: 1px solid #ccc; z-index: 5000;">
                        <div class="panel-heading"
                            style="cursor: move; display: flex; justify-content: space-between; align-items: center;">
                            <h4 class="panel-title">Investor Address Updation</h4>
                            <button type="button" class="close" onclick="hidePopup()"
                                style="font-size: 18px;">&times;</button>
                        </div>
                        <div class="panel-body">
                            <h5>Investor Details</h5>
                            <div class="form-group" style="display: flex; flex-wrap: wrap; gap: 10px;">
                                <div style="flex: 1;">
                                    <label for="txtAddress">Address</label>
                                    <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div style="flex: 1;">
                                    <label for="txtAddress2">Address</label>
                                    <asp:TextBox ID="txtAddress2" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div style="flex: 1;">
                                    <label for="txtPin">PIN</label>
                                    <asp:TextBox ID="txtPin" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group" style="display: flex; flex-wrap: wrap; gap: 10px;">
                                <div style="flex: 1;">
                                    <label for="txtMobile">Mobile</label>
                                    <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div style="flex: 1;">
                                    <label for="txtPan">PAN</label>
                                    <asp:TextBox ID="txtPan" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group" style="display: flex; flex-wrap: wrap; gap: 10px;">
                                <div style="flex: 1;">
                                    <label for="txtEmail">Email</label>
                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div style="flex: 1;">
                                    <label for="txtAadhar">Aadhar</label>
                                    <asp:TextBox ID="txtAadhar" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group" style="display: flex; flex-wrap: wrap; gap: 10px;">
                                <div style="flex: 1;">
                                    <label for="txtDOB">DOB</label>
                                    <asp:TextBox ID="txtDOB" runat="server" CssClass="form-control" TextMode="Date">
                                    </asp:TextBox>
                                </div>

                                <div style="flex: 1;">
                                    <label for="ddlCity">City</label>
                                    <asp:DropDownList ID="ddlCity" runat="server" CssClass="form-control"
                                        AutoPostBack="True" OnSelectedIndexChanged="ddlCity_SelectedIndexChanged">
                                        <asp:ListItem Text="--Select city--" Value="" />
                                        <asp:ListItem Text="" Value="" />
                                        <asp:ListItem Text="" Value="" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="form-group" style="display: flex; flex-wrap: wrap; gap: 10px;">
                                <div style="flex: 1;">
                                    <label for="ddlState">State</label>
                                    <asp:DropDownList ID="ddlState" runat="server" CssClass="form-control">
                                        <asp:ListItem Text="--Select state--" Value="" />
                                        <asp:ListItem Text="" Value="" />
                                        <asp:ListItem Text="" Value="" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="form-group" style="text-align: center;">
                                <asp:Button ID="btnUpdate" runat="server" CssClass="btn btn-primary" Text="Update" />
                                <asp:Button ID="Button2" runat="server" CssClass="btn btn-secondary" Text="Exit"
                                    OnClientClick="hidePopup(); return false;" />
                            </div>
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="Panel3" runat="server" CssClass="panel panel-default draggable"
                        Style="display: none; position: fixed; top: 20%; left: 35%; width: 30%; max-height: 400px; overflow: auto; background-color: white; border: 1px solid #ccc; z-index: 5000; padding: 15px;">
                        <div class="panel-heading"
                            style="cursor: move; display: flex; justify-content: space-between; align-items: center;">
                            <h4 class="panel-title">Investor Address Updation (Panel 3)</h4>
                            <button type="button" class="close" onclick="hidePopup4()"
                                style="font-size: 18px;">&times;</button>
                        </div>
                        <div class="panel-body">
                            <h5>Investor Details</h5>

                            <!-- Address Fields -->
                            <div class="form-group" style="display: flex; flex-wrap: wrap; gap: 15px;">
                                <div style="flex: 1;">
                                    <label for="txtAddressADDd3">Address 1</label>
                                    <asp:TextBox ID="txtAddressADD3" runat="server" CssClass="form-control"
                                        Style="margin-bottom: 10px;"></asp:TextBox>
                                </div>

                                <div style="flex: 1;">
                                    <label for="txtPinADDd3">PIN</label>
                                    <asp:TextBox ID="txtPinADD3" runat="server" onblur="handleBlur(this)"
                                        oninput="validatePinInput(this)" CssClass="form-control"
                                        Style="margin-bottom: 10px;" MaxLength="10"></asp:TextBox>
                                </div>
                            </div>

                            <!-- Mobile and PAN -->
                            <div class="form-group" style="display: flex; flex-wrap: wrap; gap: 15px;">
                                <div style="flex: 1;">
                                    <label for="txtAddressADDd23">Address 2</label>
                                    <asp:TextBox ID="txtAddressADD23" runat="server" CssClass="form-control"
                                        Style="margin-bottom: 10px;"></asp:TextBox>
                                </div>
                                <div style="flex: 1;">
                                    <label for="txtMobileADDd3">Mobile</label>
                                    <asp:TextBox ID="txtMobileADD3" runat="server" onblur="handleBlur(this)"
                                        oninput="validateMobileInput(this)" CssClass="form-control"
                                        Style="margin-bottom: 10px;"></asp:TextBox>
                                </div>

                            </div>

                            <!-- Email and Aadhaar -->
                            <div class="form-group" style="display: flex; flex-wrap: wrap; gap: 15px;">
                                <div style="flex: 1;">
                                    <label for="txtPanADDd3">PAN</label>
                                    <asp:TextBox ID="txtPanADD3" runat="server" onblur="handleBlur(this)"
                                        oninput="validatePanInput(this)" CssClass="form-control"
                                        Style="margin-bottom: 10px;" MaxLength="10"></asp:TextBox>
                                </div>
                                <div style="flex: 1;">
                                    <label for="txtEmailADDd3">Email</label>
                                    <asp:TextBox ID="txtEmailADD3" runat="server" onblur="handleBlur(this)"
                                        oninput="validateEmailInput(this)" CssClass="form-control"
                                        Style="margin-bottom: 10px;"></asp:TextBox>
                                </div>

                            </div>

                            <!-- Additional Fields -->
                            <div class="form-group" style="display: flex; flex-wrap: wrap; gap: 15px;">
                                <div style="flex: 1;">
                                    <label for="txtAadharADDd3">Aadhar</label>
                                    <asp:TextBox ID="txtAadharADD3" runat="server" onblur="handleBlur(this)"
                                        oninput="validateAadhaarInput(this)" CssClass="form-control"
                                        Style="margin-bottom: 10px;" MaxLength="12"></asp:TextBox>
                                </div>
                                <div style="flex: 1;">
                                    <label class="form-label">
                                        Date of Birth (DOB)
                                    </label>
                                    <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                        <asp:TextBox ID="txtDOBADD3" runat="server" CssClass="form-control date-input"
                                            placeholder="dd/mm/yyyy" />
                                        <div class="input-group-addon">
                                            <span class="glyphicon glyphicon-th"></span>
                                        </div>
                                    </div>
                                </div>

                            </div>

                            <!-- State Dropdown -->
                            <div class="form-group" style="display: flex; flex-wrap: wrap; gap: 15px;">
                                <div style="flex: 1;">
                                    <label for="ddlCityADDd3">City</label>
                                    <asp:DropDownList ID="ddlCityADD3" runat="server" CssClass="form-control"
                                        AutoPostBack="True" OnSelectedIndexChanged="ddlCity3_SelectedIndexChanged"
                                        Style="margin-bottom: 10px;">
                                        <asp:ListItem Text="--Select city--" Value="" />
                                    </asp:DropDownList>
                                </div>
                                <div style="flex: 1;">
                                    <label for="ddlStateADDd3">State</label>
                                    <asp:DropDownList ID="ddlStateADD3" runat="server" CssClass="form-control"
                                        Style="margin-bottom: 10px;">
                                        <asp:ListItem Text="--Select state--" Value="" />
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="form-group" style="text-align: center;">
                                <asp:Button ID="Button33" runat="server" CssClass="btn btn-primary" Text="Update"
                                    OnClick="btnPanel3Update_Click" />
                                <asp:Button ID="Button73" runat="server" CssClass="btn btn-secondary" Text="Exit"
                                    OnClientClick="hidePopup4(); return false;" />
                            </div>
                        </div>
                    </asp:Panel>

                    <div class="col-md-4">
                        <label for="remarks" class="form-label">Remarks</label>
                        <asp:TextBox ID="remarks" runat="server" CssClass="form-control" Placeholder="Remarks">
                        </asp:TextBox>
                    </div>


                    <div class="col-md-4">
                        <asp:Label ID="lblExpensesPercent" runat="server" CssClass="form-label"
                            AssociatedControlID="expensesPercent" Text="Expenses %" />
                        <asp:TextBox ID="expensesPercent" runat="server" CssClass="form-control"
                            Placeholder="Expenses %" oninput="validateExpensesPercent(this)" Enabled="false" />
                    </div>

                    <div class="col-md-4">
                        <asp:Label ID="lblExpensesRs" runat="server" CssClass="form-label"
                            AssociatedControlID="expensesRs" Text="Expenses (Rs)" />
                        <asp:TextBox ID="expensesRs" runat="server" CssClass="form-control" Placeholder="Expenses (Rs)"
                            oninput="allowOnlyNumeric(this)" Enabled="false" />
                    </div>

                    <div class="col-md-12 mt-3">
                        <h4 class="mb-3 text-danger">Reco Status</h4>
                        <div class="row g-3">
                            <div>
                                <div class="form-label">
                                    <asp:CheckBox ID="autoSwitchOnMaturity" runat="server" />
                                    <asp:Label ID="lblAutoSwitchOnMaturity" runat="server" CssClass="form-check-label"
                                        AssociatedControlID="autoSwitchOnMaturity" Text="Auto Switch On Maturity" />
                                </div>
                            </div>

                            <div class="col-md-4">
                                <asp:Label ID="lblRecoStatus" runat="server" CssClass="form-label"
                                    AssociatedControlID="recoStatus" Text="Reco Status" />
                                <asp:TextBox ID="recoStatus" runat="server" CssClass="form-control"
                                    Placeholder="Reco Status" />
                            </div>

                            <div class="col-md-4">
                                <div class="d-flex align-items-center gap-3 mt-4">
                                    <div class="form-label">
                                        <asp:RadioButton ID="RadioButton9" runat="server" GroupName="regularNfo"
                                            Text="Regular" />
                                    </div>
                                    <div class="form-label">
                                        <asp:RadioButton ID="RadioButton10" runat="server" GroupName="regularNfo"
                                            Text="NFO" />
                                    </div>
                                </div>
                            </div>

                            <div class="col-md-4">
                                <asp:Label ID="lblSearchBox" runat="server" CssClass="form-label"
                                    AssociatedControlID="searchBox" Text="Search" />
                                <div class="input-group">
                                    <asp:TextBox ID="searchBox" runat="server" CssClass="form-control"
                                        Placeholder="Search" ReadOnly="true" />
                                    <asp:Button ID="Button5" runat="server" CssClass="btn btn-outline-primary"
                                        Text="Search" Enabled="false" />
                                </div>
                            </div>
                        </div>
                    </div>


                    <div class="col-md-12 mt-4">
                        <div class="d-flex align-items-md-center flex-wrap flex-md-row flex-column gap-3">

                            <asp:Button ID="btnModify" runat="server" CssClass="btn btn-primary" Text="Modify"
                                OnClick="btnModClick" />

                            <script type="text/javascript">
                                function confirmMultipleTransaction() {
                                    var hdnField = document.getElementById('<%= hdnPrintOption.ClientID %>'); // ✅ Ensure correct ID
                                    var result = confirm("Multiple Transactions Print?");

                                    // ✅ Always update the hidden field before returning
                                    hdnField.value = result ? "Yes" : "No";

                                    return true; // ✅ If "Cancel" is clicked, prevents form submission
                                }
                            </script>


                            <asp:Button ID="btnSearchTRPrint" runat="server" CssClass="btn btn-outline-primary"
                                Text="SearchTRPrint" OnClientClick="return confirmMultipleTransaction();"
                                OnClick="btn_PrintModify" />

                            <asp:HiddenField ID="hdnPrintOption" runat="server" />
                            <asp:Button ID="Button6" runat="server" CssClass="btn btn-outline-primary" Text="Reset"
                                OnClick="btnReset_Click" OnClientClick="ResetPayview(); return true;" />
                            <asp:Button ID="btnTranCancel" runat="server" CssClass="btn btn-outline-primary"
                                Text="Tran Cancel" OnClientClick="showCancelPopup(); return false;" />
                            <asp:Button ID="btnExitView" runat="server" CssClass="btn btn-outline-primary" Text="Exit"
                                OnClick="btnExit_Click" />
                            <asp:Button ID="btnOpenPanel" runat="server" Text="View Log"
                                CssClass="btn btn-outline-primary" OnClick="CmdViewLog_Click"
                                OnClientClick="openPanel(); return false;" />

                            <script type="text/javascript">
                                function ResetPayview() {
                                    // Set the hidden field value to 'cheque'
                                    var hiddenField = document.getElementById('<%= hdnSelectedPaymentMethod_view.ClientID %>');
                                    if (hiddenField) {
                                        hiddenField.value = 'cheque_view';
                                    }

                                    // Set the payment method container value to 'cheque' and trigger change event
                                    var paymentMethodContainer = document.getElementById("paymentMethodContainer_view");
                                    if (paymentMethodContainer) {
                                        paymentMethodContainer.value = 'cheque_view';  // Set the value to 'cheque'
                                        var event = new Event('change');
                                        paymentMethodContainer.dispatchEvent(event);  // Trigger the change event
                                    }
                                }
                            </script>


                            <asp:Panel ID="FrmTransactionLog" runat="server" Visible="false" CssClass="floating-panel"
                                Style="position: fixed; top: 50%; left: 50%; transform: translate(-50%, -50%); z-index: 1050; width: 90%; max-width: 1000px; max-height: 90%; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1); border-radius: 8px; padding: 0; border: 1px solid #ddd;">
                                <!-- Heading Section -->
                                <div
                                    style="border-bottom: 1px solid #ddd; padding: 20px; display: flex; justify-content: space-between; align-items: center;">
                                    <h5 class="text-primary fw-bold" style="margin: 0;">Transaction Log</h5>
                                    <button type="button" class="btn-close" onclick="closePanel()"
                                        aria-label="Close"></button>

                                </div>
                                <!-- Content Section with Scrollable Grid -->
                                <div style="padding: 20px; overflow-y: auto; max-height: calc(90vh - 120px);">
                                    <asp:GridView ID="gvTransactionLog" runat="server" AutoGenerateColumns="True"
                                        CssClass="table table-striped table-bordered" HeaderStyle-CssClass="thead-light"
                                        RowStyle-CssClass="table-light" AlternatingRowStyle-CssClass="table-secondary">
                                    </asp:GridView>
                                </div>
                            </asp:Panel>


                            <script type="text/javascript">
                                // Function to open the panel
                                function openPanel() {
                                    document.getElementById('<%= FrmTransactionLog.ClientID %>').style.display = 'block';
                                    localStorage.setItem('panelOpen', 'true');  // Store the state in localStorage
                                }

                                // Function to close the panel
                                function closePanel() {
                                    document.getElementById('<%= FrmTransactionLog.ClientID %>').style.display = 'none';
                                    localStorage.removeItem('panelOpen');  // Remove the state from localStorage when closed
                                }

                                // Check if the panel should be open on page load
                                window.onload = function () {
                                    if (localStorage.getItem('panelOpen') === 'true') {
                                        document.getElementById('<%= FrmTransactionLog.ClientID %>').style.display = 'block';
                                    }
                                }
                            </script>


                        </div>
                    </div>

                    <asp:TextBox ID="lblWaning2" runat="server" CssClass="form-control" ReadOnly="true"
                        BackColor="#ffdddd" Visible="false" />

                    <%--<h5 class="card-title">Print Preview</h5>--%>
                        <div style="overflow-y: auto; overflow-x: auto; max-height: 450px;">


                            <div class="table-responsive" style="display: none">
                                <asp:GridView ID="gridview1" CssClass="table table-bordered" runat="server"
                                    AutoGenerateColumns="False" OnRowCommand="tableSearchResultsTran_RowCommand">
                                    <HeaderStyle CssClass="thead-dark" />
                                    <Columns>

                                        <asp:TemplateField HeaderText="AR NO">
                                            <ItemTemplate>
                                                <asp:Label ID="Label23" runat="server"
                                                    Text='<%# Eval("TRAN_CODE") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Investor Name">
                                            <ItemTemplate>
                                                <asp:Label ID="Label24" runat="server" Text='<%# Eval("INV") %>' />
                                                <%-- ✅ Corrected from "INVESTOR_NAME" to "INV" --%>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Client">
                                            <ItemTemplate>
                                                <asp:Label ID="Label25" runat="server" Text='<%# Eval("CLIENT") %>' />
                                                <%-- ✅ Mapped to correct alias --%>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="City Name">
                                            <ItemTemplate>
                                                <asp:Label ID="Label26" runat="server" Text='<%# Eval("CCITY") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Address1">
                                            <ItemTemplate>
                                                <asp:Label ID="Label27" runat="server" Text='<%# Eval("ADD1") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Address2">
                                            <ItemTemplate>
                                                <asp:Label ID="Label28" runat="server" Text='<%# Eval("ADD2") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="PIN">
                                            <ItemTemplate>
                                                <asp:Label ID="Label29" runat="server" Text='<%# Eval("PIN") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Mobile">
                                            <ItemTemplate>
                                                <asp:Label ID="Label30" runat="server" Text='<%# Eval("PH") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Email">
                                            <ItemTemplate>
                                                <asp:Label ID="Label31" runat="server" Text='<%# Eval("EMAIL") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>



                                        <asp:TemplateField HeaderText="Branch Name">
                                            <ItemTemplate>
                                                <asp:Label ID="Label33" runat="server" Text='<%# Eval("BNAME") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Branch Address">
                                            <ItemTemplate>
                                                <asp:Label ID="Label34" runat="server" Text='<%# Eval("BADD1") %>' />
                                                <asp:Label ID="Label35" runat="server" Text='<%# Eval("BADD2") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Branch Phone">
                                            <ItemTemplate>
                                                <asp:Label ID="Label36" runat="server" Text='<%# Eval("BPHONE") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Location">
                                            <ItemTemplate>
                                                <asp:Label ID="Label37" runat="server" Text='<%# Eval("BLOC") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Bank Name">
                                            <ItemTemplate>
                                                <asp:Label ID="Label38" runat="server"
                                                    Text='<%# Eval("BANK_NAME") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Cheque No">
                                            <ItemTemplate>
                                                <asp:Label ID="Label39" runat="server"
                                                    Text='<%# Eval("CHEQUE_NO") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Cheque Date">
                                            <ItemTemplate>
                                                <asp:Label ID="Label40" runat="server"
                                                    Text='<%# Eval("CHEQUE_DATE", "{0:dd/MM/yyyy}") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Amount">
                                            <ItemTemplate>
                                                <asp:Label ID="Label41" runat="server" Text='<%# Eval("AMOUNT") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Scheme Name">
                                            <ItemTemplate>
                                                <asp:Label ID="Label42" runat="server" Text='<%# Eval("SCHMF") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Scheme Short Name">
                                            <ItemTemplate>
                                                <asp:Label ID="Label43" runat="server" Text='<%# Eval("SSCHMF") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="AMC Name">
                                            <ItemTemplate>
                                                <asp:Label ID="Label44" runat="server" Text='<%# Eval("COMPMF") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Remarks">
                                            <ItemTemplate>
                                                <asp:Label ID="Label45" runat="server" Text='<%# Eval("ASR") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="RM Name">
                                            <ItemTemplate>
                                                <asp:Label ID="Label46" runat="server" Text='<%# Eval("RNAME") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Payment Mode">
                                            <ItemTemplate>
                                                <asp:Label ID="Label47" runat="server"
                                                    Text='<%# Eval("PAYMENT_MODE") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Paid Amount">
                                            <ItemTemplate>
                                                <asp:Label ID="Label48" runat="server" Text='<%# Eval("PAIDAMT") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Upfront Paid">
                                            <ItemTemplate>
                                                <asp:Label ID="Label49" runat="server"
                                                    Text='<%# Eval("PAID_BROK") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Logged User ID">
                                            <ItemTemplate>
                                                <asp:Label ID="Label50" runat="server"
                                                    Text='<%# Eval("LOGGEDUSERID") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                    </Columns>
                                </asp:GridView>
                            </div>






                        </div>


                        <asp:Panel ID="printableArea" runat="server" Style="display: none; padding: 20px;">
                            <div style="text-align: center; margin-bottom: 20px;">
                                <h2 style="margin-bottom: 20px;">All TRANSACTIONS DONE ON</h2>
                                <div style="margin: 20px 0;">
                                    <table class="print-table">
                                        <thead>
                                            <tr>
                                                <th>Sr. No.</th>
                                                <th>Tran No.</th>
                                                <th>Investor Name</th>
                                                <th>Scheme Name</th>
                                                <th>App No.</th>
                                                <th>Pay Mode</th>
                                                <th>Cheque No.</th>
                                                <th>Bank Name</th>
                                                <th>Amount</th>
                                            </tr>
                                        </thead>
                                        <tbody id="transactionTableBody">
                                        </tbody>
                                    </table>
                                </div>
                                <div style="text-align: center; margin-top: 20px;">
                                    <p style="margin: 0; font-size: 12px; color: #555;">* For Investment in mutual
                                        funds, this receipt should be considered as provisional / temporary receipt</p>
                                </div>
                            </div>
                        </asp:Panel>

                        <asp:Panel ID="CancelPopup" runat="server" CssClass="modal-popup"
                            Style="display: none; position: fixed; top: 5%; left: 50%; transform: translateX(-50%); width: 40%; background-color: white; border: 1px solid #ccc; padding: 20px; z-index: 9999; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.2);">

                            <div class="modal-header">
                                <h4 class="panel-title">Cancel Transaction</h4>
                                <button type="button" class="close" onclick="hideCancelPopup();">&times;</button>
                            </div>

                            <div class="modal-body">

                                <div class="form-group">
                                    <label for="txtCanTrcode">Transaction Code:</label>
                                    <asp:TextBox ID="txtCanTrcode" runat="server" CssClass="form-control"
                                        Enabled="false" />
                                </div>


                                <div class="form-group">
                                    <label for="mdatecancel">Cancel Date:</label>
                                    <asp:TextBox ID="mdatecancel" runat="server" CssClass="form-control"
                                        Text="__/__/____" />
                                </div>




                                <div class="form-group">
                                    <label for="CmbReson">Select Reason:</label>
                                    <asp:DropDownList ID="CmbReson" runat="server" CssClass="form-control">

                                        <asp:ListItem Value="1" Text="Reason 1"></asp:ListItem>
                                        <asp:ListItem Value="2" Text="Reason 2"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="txtremark">Remark:</label>
                                <asp:TextBox ID="txtremark" runat="server" CssClass="form-control" />
                            </div>

                            <div class="modal-footer">

                                <asp:Button ID="Button4" runat="server" CssClass="btn btn-primary" Text="OK"
                                    OnClick="btnOk_Click" />
                                <asp:Button ID="btnClosePopup" runat="server" CssClass="btn btn-secondary" Text="Cancel"
                                    OnClientClick="hideCancelPopup(); return false;" />
                            </div>
                        </asp:Panel>


                        <asp:Panel ID="AutoSwitchPanel" runat="server" CssClass="modal-popup"
                            Style="display: none; position: fixed; top: 20%; left: 30%; width: 40%; background-color: white; border: 1px solid #ccc; padding: 20px; z-index: 1000; box-shadow: 0px 4px 6px rgba(0, 0, 0, 0.1);">

                            <div class="modal-header"
                                style="display: flex; justify-content: space-between; align-items: center; border-bottom: 1px solid #eaeaea; margin-bottom: 15px;">
                                <h4 class="panel-title" style="margin: 0; font-size: 18px; font-weight: bold;">
                                    WealthMaker</h4>
                                <button type="button" class="btn-close" aria-label="Close"
                                    onclick="hideAutoSwitchPanel();"
                                    style="font-size: 16px; background: none; border: none; cursor: pointer;">&times;</button>
                            </div>


                            <div class="modal-body" style="text-align: center; margin-bottom: 20px;">
                                <p style="font-size: 14px; color: #555;">Auto switch on maturity can be selected on
                                    close-ended and non-SIP transactions.</p>
                            </div>


                            <div class="modal-footer" style="text-align: center;">
                                <asp:Button ID="Button8" runat="server" Text="OK" CssClass="btn btn-primary"
                                    OnClientClick="hideAutoSwitchPanel(); return false;" />
                            </div>
                        </asp:Panel>


                        <asp:Panel ID="Panel1" runat="server" CssClass="panel panel-default"
                            Style="display: none; position: fixed; top: 30%; left: 35%; width: 30%; background-color: white; border: 1px solid #ccc; z-index: 1000; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);">
                            <div class="panel-heading"
                                style="display: flex; justify-content: space-between; align-items: center;">
                                <h4 class="panel-title">Wealth Maker</h4>
                                <button type="button" class="close" onclick="hidePopup2()">&times;</button>
                            </div>
                            <div class="panel-body">
                                <p>Security restriction for data range.</p>
                                <asp:Button ID="btnOk" runat="server" CssClass="btn btn-primary" Text="OK"
                                    OnClientClick="hidePopup2(); return false;" />
                            </div>
                        </asp:Panel>
                </div>
            </div>





        </ContentTemplate>




    </asp:UpdatePanel>