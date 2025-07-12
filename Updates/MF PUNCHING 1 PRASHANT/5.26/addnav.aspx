<asp:UpdatePanel ID="UpdatePanelFillByDT" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="row g-3">
                                        <div class="col-md-6">
                                            <h4 class="card-title">Investor/Scheme Details</h4>
                                            <div>
                                                <div class="row g-3">
                                                    <div class="col-md-9">
                                                    </div>

                                                    <asp:UpdatePanel ID="UpdatePanelDTNumber" runat="server" UpdateMode="Conditional">
                                                        <ContentTemplate>
                                                            <div class="mt-4">
                                                                <label for="dtNumberA" class="form-label">DT Number</label>
                                                                <div class="d-flex">
                                                                    <asp:TextBox ID="dtNumberA" runat="server" CssClass="form-control" placeholder="DT Number"
                                                                        oninput="allowOnlyNumeric(this)" onblur="handleBlur(this)" />
                                                                    <asp:Button ID="btnShowDT" runat="server" CssClass="btn btn-outline-primary ms-2"
                                                                        Text="Show" OnClick="btnShowA_Click" />
                                                                </div>
                                                            </div>
                                                        </ContentTemplate>
                                                        <Triggers>
                                                            <asp:AsyncPostBackTrigger ControlID="btnShowDT" EventName="Click" />
                                                        </Triggers>
                                                    </asp:UpdatePanel>


                                                    <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" />


                                                    <div class="col-md-6">
                                                        <label class="form-label">
                                                            Transaction Date
                                                        </label>
                                                        <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                            <asp:TextBox ID="transactionDate" runat="server" CssClass="form-control date-input"
                                                                placeholder="dd/mm/yyyy" ReadOnly="true"/>
                                                            <div class="input-group-addon">
                                                                <span class="glyphicon glyphicon-th"></span>
                                                            </div>

                                                        </div>
                                                    </div>

                                                    <div class="col-md-6">
                                                        <label class="form-label">Inv Code</label>
                                                        <asp:TextBox ID="invcode" runat="server" CssClass="form-control" Enabled="false" />
                                                    </div>


                                                    <div class="col-md-6">
                                                        <label for="businessCode" class="form-label">Business Code</label>
                                                        <asp:TextBox ID="businessCode" runat="server" CssClass="form-control" />
                                                    </div>
                                                   


                                                    <div class="col-md-4">
                                                        <button type="button" class="btn btn-outline-primary btn-sm" data-bs-toggle="modal" data-bs-target="#investorSearchModal">Search Investor</button>
                                                    </div>

                                                    <!-- Investor Modal -->
                                                    <div class="modal fade" id="investorSearchModal" tabindex="-1" aria-labelledby="investorSearchModalLabel" aria-hidden="true">
                                                        <div class="modal-dialog modal-lg">
                                                            <div class="modal-content bg-white">
                                                                <div class="modal-header">
                                                                    <h5 class="modal-title" id="investorSearchModalLabel">Investor Search</h5>
                                                                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                                                </div>
                                                                <div class="modal-body">
                                                                    <div class="row g-2 client-search-section">
                                                                        <div class="col-3">
                                                                            <label class="form-label small">Category</label>
                                                                            <select id="categorySearch" class="form-control form-control-sm">
                                                                                <option value="All">All</option>
                                                                                <option value="Client">Client</option>
                                                                                <option value="Sub Broker">Sub Broker</option>
                                                                            </select>
                                                                        </div>
                                                                        <div class="col-3">
                                                                            <label class="form-label small">Branch</label>
                                                                            <asp:DropDownList runat="server" ID="BranchSearch_Dropdown" CssClass="form-control form-control-sm branch-search">
                                                                                <asp:ListItem Text="Select Branch" Value=""></asp:ListItem>
                                                                            </asp:DropDownList>
                                                                        </div>
                                                                        <div class="col-3">
                                                                            <label class="form-label small">RM</label>
                                                                            <select id="RMSearch_Dropdown" class="form-control form-control-sm">
                                                                                <option value="">Select Branch First</option>
                                                                            </select>
                                                                        </div>
                                                                        <div class="col-3">
                                                                            <label class="form-label small">City</label>
                                                                            <asp:DropDownList runat="server" ID="CitySearch_Dropdown" CssClass="form-control form-control-sm city-search">
                                                                                <asp:ListItem Text="Select City" Value=""></asp:ListItem>
                                                                            </asp:DropDownList>
                                                                        </div>
                                                                        <div class="col-3">
                                                                            <label class="form-label small">Name</label>
                                                                            <input type="text" id="clientNameSearch" class="form-control form-control-sm" />
                                                                        </div>
                                                                        <div class="col-3">
                                                                            <label class="form-label small">Inv Code</label>
                                                                            <input type="text" id="invCodeSearch" class="form-control form-control-sm" />
                                                                        </div>
                                                                        <div class="col-3">
                                                                            <label class="form-label small">Pan</label>
                                                                            <input type="text" id="panNoSearch" class="form-control form-control-sm" oninput="validatePanInput(this)" maxlength="10" />
                                                                        </div>
                                                                        <div class="col-3">
                                                                            <label class="form-label small">Phone</label>
                                                                            <input type="text" id="phoneSearch" class="form-control form-control-sm" oninput="validateMobileInput(this)" />
                                                                        </div>
                                                                        <div class="col-3">
                                                                            <label class="form-label small">Mobile</label>
                                                                            <input type="text" id="mobileSearch" class="form-control form-control-sm" oninput="validateMobileInput(this)" />
                                                                        </div>
                                                                        <div class="col-3">
                                                                            <label class="form-label small">Address 1</label>
                                                                            <input type="text" id="address1Search" class="form-control form-control-sm" />
                                                                        </div>
                                                                        <div class="col-3">
                                                                            <label class="form-label small">Address 2</label>
                                                                            <input type="text" id="address2Search" class="form-control form-control-sm" />
                                                                        </div>
                                                                        <div class="col-3">
                                                                            <label class="form-label small">Sort by</label>
                                                                            <select id="sortBy2Search" class="form-control form-control-sm">
                                                                                <option value="im.INVESTOR_NAME">Select Sort</option>
                                                                                <option value="im.INVESTOR_NAME" selected>Name</option>
                                                                                <option value="im.ADDRESS1">Address 1</option>
                                                                                <option value="im.ADDRESS2">Address 2</option>
                                                                                <option value="cm.CITY_NAME">City</option>
                                                                                <option value="im.PHONE">Phone</option>
                                                                            </select>
                                                                        </div>
                                                                        <div class="col-3">
                                                                            <label class="form-label small">Email</label>
                                                                            <input type="text" id="emailSearch" class="form-control form-control-sm" oninput="validateEmailInput(this)" />
                                                                        </div>
                                                                        <div class="col-3">
                                                                            <label class="form-label small">DOB (dd/mm/yyyy)</label>
                                                                            <div class="input-group date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                                                <input type="text" id="dobSearch" class="form-control form-control-sm" placeholder="dd/mm/yyyy" />
                                                                                <div class="input-group-append">
                                                                                    <span class="input-group-text">
                                                                                        <i class="fa fa-calendar"></i>
                                                                                    </span>
                                                                                </div>
                                                                            </div>
                                                                        </div>

                                                                       <%-- <div class="col-3">
                                                                            <label class="form-label small">Account Code</label>
                                                                            <input type="text" id="accountCodeSearch" class="form-control form-control-sm" />
                                                                        </div>--%>
                                                                        <div class="col-3">
                                                                            <div class="mt-4 pt-1">
                                                                                <button id="btnInvestorSearch" type="button" class="btn btn-primary btn-sm"><i class="fa fa-search me-1"></i>Search</button>
                                                                                <button id="btnInvestorReset" type="button" class="btn btn-outline-primary btn-sm ms-2">Reset</button>
                                                                                <button type="button" class="btn btn-outline-primary btn-sm ms-2" data-bs-dismiss="modal">Exit</button>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <div class="row mt-3">
                                                                        <div class="col-md-12">
                                                                            <div class="table-responsive border">
                                                                                <table id="investorSearchResult" class="table table-hover">
                                                                                    <thead>
                                                                                        <tr>
                                                                                            
                                                                                            <th>Name</th>
                                                                                            <th>Address 1</th>
                                                                                            <th>Address 2</th>
                                                                                            <th>City</th>
                                                                                            <th>Branch</th>
                                                                                            <th>Phone</th>
                                                                                            <th>Mobile</th>
                                                                                            <th>RM</th>
                                                                                            <th>Account Status</th>
                                                                                            <th>Pan</th>
                                                                                            <th>Aadhar</th>
                                                                                            <th>DOB</th>
                                                                                            <th>AC. Hol.</th>
                                                                                            <th>Business Code</th>
                                                                                        </tr>
                                                                                    </thead>
                                                                                    <tbody>
                                                                                        <tr>
                                                                                            <td colspan="10">
                                                                                                <p class="text-primary mb-0">Search result display here</p>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </tbody>
                                                                                </table>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <!-- Modal -->
                                                    <div class="modal fade" id="addresModal" tabindex="-1" aria-labelledby="addresModalLabel" aria-hidden="true">
                                                        <div class="modal-dialog">
                                                            <div class="modal-content bg-white">
                                                                <div class="modal-header">
                                                                    <h1 class="modal-title fs-5" id="addresModalLabel">Address Updation</h1>
                                                                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                                                </div>
                                                                <div class="modal-body">
                                                                    <div class="row g-3">
                                                                        <div class="col-12">
                                                                            <label class="form-label">Address1</label>
                                                                            <asp:TextBox runat="server" ID="AddTextAddress1" CssClass="form-control" data-required="required"></asp:TextBox>
                                                                        </div>
                                                                        <div class="col-12">
                                                                            <label class="form-label">Address2</label>
                                                                            <asp:TextBox runat="server" ID="AddTextAddress2" CssClass="form-control" data-required="required"></asp:TextBox>
                                                                        </div>
                                                                        <div class="col-6">
                                                                            <label class="form-label">City</label>
                                                                            <asp:DropDownList runat="server" ID="DropDownCity" ClientIDMode="Static" CssClass="form-select" data-required="required">
                                                                                <asp:ListItem Text="--Select city--" Value="" />
                                                                                <asp:ListItem Text="" Value="" />
                                                                                <asp:ListItem Text="" Value="" />
                                                                            </asp:DropDownList>
                                                                        </div>
                                                                        <div class="col-6">
                                                                            <label class="form-label">State</label>
                                                                            <asp:DropDownList runat="server" ID="DropDownState" ClientIDMode="Static" CssClass="form-select" data-required="required">
                                                                                <asp:ListItem Text="--Select state--" Value="" />
                                                                                <asp:ListItem Text="" Value="" />
                                                                                <asp:ListItem Text="" Value="" />
                                                                            </asp:DropDownList>
                                                                        </div>
                                                                        <div class="col-6">
                                                                            <label class="form-label">Pin Code</label>
                                                                            <asp:TextBox runat="server" ID="TextPin" CssClass="form-control" oninput="validatePinInput(this)"></asp:TextBox>
                                                                        </div>
                                                                        <div class="col-6">
                                                                            <label class="form-label">Mobile</label>
                                                                            <asp:TextBox runat="server" ID="TextMobile" CssClass="form-control" data-required="required" oninput="validateMobileInput(this)"></asp:TextBox>
                                                                        </div>
                                                                        <div class="col-6">
                                                                            <label class="form-label">Pan No</label>
                                                                            <asp:TextBox runat="server" ID="TextPan" CssClass="form-control" oninput="validatePanInput(this)"></asp:TextBox>
                                                                        </div>
                                                                        <div class="col-6">
                                                                            <label class="form-label">Aadhar No</label>
                                                                            <asp:TextBox runat="server" ID="TextAadhar" CssClass="form-control" oninput="validateAadhaarInput(this)"></asp:TextBox>
                                                                        </div>
                                                                        <div class="col-6">
                                                                            <label class="form-label">DOB(dd/mm/yyyy)</label>
                                                                            <asp:TextBox runat="server" ID="TextDob" CssClass="form-control"></asp:TextBox>
                                                                        </div>
                                                                        <div class="col-6" style="display: none;">
                                                                            <label class="form-label">InvCode</label>
                                                                            <asp:TextBox runat="server" ID="Invcdtxt" CssClass="form-control" data-required="required"></asp:TextBox>
                                                                        </div>

                                                                    </div>
                                                                </div>
                                                                <div class="modal-footer">
                                                                    <button type="button" class="btn btn-secondary btn-sm" data-bs-dismiss="modal">Close</button>
                                                                    <asp:Button ID="btnAddressUpdate" runat="server" CssClass="btn btn-primary" Text="Save changes" OnClick="btnPanelAddressUpdate_Click" data-bs-dismiss="modal"/>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <!-- Include jQuery (Bootstrap requires jQuery to work with modals) -->
                                                    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>

                                                    <!-- Include Bootstrap JS -->
                                                    <script src="https://stackpath.bootstrapcdn.com/bootstrap/5.1.3/js/bootstrap.bundle.min.js"></script>


                                                    <script type="text/javascript">
                                                        function openAddressModal() {
                                                            var myModal = new bootstrap.Modal(document.getElementById('addresModal'));
                                                            myModal.show();
                                                        }
                                                    </script>



                                                    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>

                                                    <script type="text/javascript">
                                                        $(document).ready(function () {
                                                            // Reset modal when it's hidden
                                                            function bindEventHandlers() {
                                                            let investorModal = document.getElementById('investorSearchModal');
                                                            investorModal.addEventListener('hide.bs.modal', function () {
                                                                resetModal();
                                                            });

                                                            // Click event for reset button
                                                            $("#btnInvestorReset").click(function () {
                                                                resetModal();
                                                            });

                                                            // Reset modal function
                                                            function resetModal() {
                                                                $(".modal:not('#addresModal') input[type='text']:not(#categorySearch)").val('');
                                                                $(".modal:not('#addresModal') input[value='Before']").prop('checked', true);
                                                                $(".modal:not('#addresModal') select").each(function () {
                                                                    $(this).val($(this).find('option:first').val());
                                                                });
                                                                $(".modal:not('#addresModal') table tbody").html(`<tr class="empty"><td colspan='23'><p class="text-primary mb-0">Search result display here.</p></td></tr>`);

                                                                // Reset validation messages
                                                                $(".modal:not('#addresModal') .error-message").remove(); // Remove all error messages
                                                                $(".modal:not('#addresModal') input, .modal:not('#addresModal') select").css("borderColor", ""); // Reset border color
                                                            }

                                                            // Click event for the search button
                                                            $("#btnInvestorSearch").click(function () {
                                                                let $this = $(this);
                                                                let query = {
                                                                    category: $("#categorySearch").val(),
                                                                    branchCode: $(".branch-search").val(),
                                                                    cityCode: $(".city-search").val(),
                                                                    rmCode: $("#RMSearch_Dropdown").val(),
                                                                    ClientName: $("#clientNameSearch").val(),
                                                                    InvCode: $("#invCodeSearch").val(),
                                                                    panNo: $("#panNoSearch").val(),
                                                                    phone: $("#phoneSearch").val(),
                                                                    mobile: $("#mobileSearch").val(),
                                                                    address1: $("#address1Search").val(),
                                                                    address2: $("#address2Search").val(),
                                                                    sortBy: $("#sortBy2Search").val(),
                                                                    email: $("#emailSearch").val(),
                                                                    dob: $("#dobSearch").val(),
                                                                    accountCode: $("#accountCodeSearch").val()
                                                                };

                                                                $this.attr("disabled", "disabled").html("<i class='fa fa-spinner ms-0'></i> Wait..");

                                                                // AJAX call for searching investor data
                                                                $.ajax({
                                                                    url: "Mf_Punching.aspx/SearchInvestorData",
                                                                    method: "post",
                                                                    data: JSON.stringify({ query: query }),
                                                                    contentType: "application/json; charset=utf-8",
                                                                    dataType: "json",
                                                                    success: function (res) {
                                                                        let { data } = JSON.parse(res.d);
                                                                        let tableRow = createInvestorTable(data);
                                                                        $("#investorSearchModal tbody").html(tableRow);
                                                                        $this.removeAttr("disabled").html("<i class='fa fa-search'></i> Search");
                                                                    },
                                                                    error: function () {
                                                                        $this.removeAttr("disabled").html("<i class='fa fa-search'></i> Search");
                                                                    },
                                                                });
                                                            });

                                                            $(".branch-search").change(function () {
                                                                let branchCode = $(this).val();
                                                                if (branchCode) {
                                                                    $.ajax({
                                                                        url: "/masters/mf_punching.aspx/getrmlist",
                                                                        method: "post",
                                                                        data: JSON.stringify({ branchCode: branchCode }),
                                                                        contentType: "application/json; charset=utf-8",
                                                                        dataType: "json",
                                                                        success: function (res) {

                                                                            let { data } = JSON.parse(res.d);

                                                                            let text = data.length > 0 ? "Select RM" : "RM Not Found";
                                                                            data.unshift({ text: text, value: "" })

                                                                            let rmOptions = data.map(r => `<option value="${r.value}">${r.text}</option>`)
                                                                            $("#RMSearch_Dropdown").html(rmOptions.join(''));

                                                                        },
                                                                        error: function () {

                                                                        },
                                                                    })
                                                                }

                                                            })

                                                            $("#investorSearchModal").on("dblclick", "tr", function () {

                                                                let investorName = $(this).find('td:nth-child(1)').text();
                                                                let invCode = $(this).find('td:nth-child(1)').data("clientCode");
                                                                let mobile = $.trim($(this).find('td:nth-child(7)').text());
                                                                let address1 = $.trim($(this).find('td:nth-child(2)').text());
                                                                let address2 = $.trim($(this).find('td:nth-child(3)').text());
                                                                let cityId = $(this).find('td:nth-child(4)').data("cityId");
                                                                let stateId = $(this).find('td:nth-child(4)').data("stateId");
                                                                let pincode = $(this).find('td:nth-child(4)').data("pinCode");
                                                                let pan = $.trim($(this).find('td:nth-child(10)').text());
                                                                let aadharNo = $.trim($(this).find('td:nth-child(11)').text());
                                                                let dob = $.trim($(this).find('td:nth-child(12)').text());
                                                                let ahClientCode = $.trim($(this).find('td:nth-child(13)').text());

                                                                let BusinessCodeInMod = $.trim($(this).find('td:nth-child(14)').text());
                                                                let RmNameInMod = $.trim($(this).find('td:nth-child(8)').text());
                                                                let BRANCHCODE = $(this).find('td:nth-child(5)').data("branch-code");

                                                                // fill address details
                                                                $("[id*='AddTextAddress1']").val(address1);
                                                                $("[id*='AddTextAddress2']").val(address2);
                                                                $("[id*='Invcdtxt']").val(invCode);
                                                                $("[id*='DropDownCity']").val(cityId);
                                                                $("[id*='DropDownState']").val(stateId);
                                                                $("[id*='TextPin']").val(pincode);
                                                                $("[id*='TextMobile']").val(mobile);
                                                                $("[id*='TextPan']").val(pan);
                                                                $("[id*='TextAadhar']").val(aadharNo);
                                                                $("[id*='TextDob']").val(dob);
                                                                $("[id*='invcode']").val(invCode);
                                                                var cleanedName = investorName.replace(/\s*\(Client\)$/, ""); // Removes " (Client)" from the end
                                                                $("[id*='accountHolder']").val(cleanedName);
                                                                $("[id*='holderCode']").val(ahClientCode);

                                                                $("[id*='businessCode']").val(BusinessCodeInMod);
                                                                $("[id*='RMNAMEP']").val(RmNameInMod);
                                                                $("[id*='branch']").val(BRANCHCODE);

                                                                $(this).closest('.modal').find("button[data-bs-dismiss='modal']").trigger('click');

                                                                // Open the Address Updation modal
                                                                $("#addresModal").modal("show");

                                                            })

                                                                function createInvestorTable(data = []) {
                                                                    let tableRow = "";
                                                                    if (!data || data.length === 0) {
                                                                        tableRow = "<tr><td colspan='14'><p class='mb-0 text-danger'>Data not found</p></td></tr>";
                                                                        return tableRow;
                                                                    }

                                                                    tableRow = data.map(employee => {
                                                                        let dobString = '';
                                                                        if (employee.DOB) {
                                                                            try {
                                                                                let dobDate = new Date(employee.DOB);
                                                                                let day = dobDate.getDate();
                                                                                day = day < 10 ? `0${day}` : day;
                                                                                let month = (dobDate.getMonth() + 1);
                                                                                month = month < 10 ? `0${month}` : month;
                                                                                let year = dobDate.getFullYear();
                                                                                dobString = `${day}/${month}/${year}`;
                                                                            } catch (e) {
                                                                                dobString = '';
                                                                            }
                                                                        }

                                                                        return (`<tr>

                                                                                <td data-client-code="${employee.INV_CODE}" title='${employee.INVESTOR_NAME}'>${employee.INVESTOR_NAME} (${employee.CATEGORY})(${employee.INV_CODE})</td>
                                                                                <td title='${employee.ADDRESS1 || ""}'>${employee.ADDRESS1 || ""}</td>
                                                                                <td title='${employee.ADDRESS2 || ""}'>${employee.ADDRESS2 || ""}</td>
                                                                                <td data-city-id='${employee.CITY_ID}' data-state-id='${employee.STATE_ID}' data-pin-code='${employee.PINCODE}' title='${employee.CITY_NAME}'>${employee.CITY_NAME}</td>
                                                                                <td data-branch-code='${employee.BRANCH_CODE}' title='${employee.BRANCH_NAME}'>${employee.BRANCH_NAME}</td>
                                                                                <td title='${employee.PHONE || ""}'>${employee.PHONE || ""}</td>
                                                                                <td title='${employee.MOBILE || ""}'>${employee.MOBILE || ""}</td>
                                                                                <td data-rm-code='${employee.RM_CODE}' title='${employee.RM_NAME}'>${employee.RM_NAME}</td>
                                                                                <td title='${employee.KYC || ""}'>${employee.KYC || ""}</td>
                                                                                <td title='${employee.PAN || ""}'>${employee.PAN || ""}</td>
                                                                                <td title='${employee.AADHAR_CARD_NO || ""}'>${employee.AADHAR_CARD_NO || ""}</td>
                                                                                <td title='${dobString}'>${dobString}</td>
                                                                                <td title='${employee.AH_CLIENT_CODE || ""}'>${employee.AH_CLIENT_CODE || ""}</td>
                                                                                <td title='${employee.PAYROLL_ID || ""}'>${employee.PAYROLL_ID || ""}</td>
                                                                            </tr>`);
                                                                    }).join('');

                                                                    return tableRow;
                                                                }
                                                            }
                                                            Sys.Application.add_load(function () {
                                                                bindEventHandlers();
                                                            });

                                                            // Initial binding when the page first loads
                                                            bindEventHandlers();
                                                        });
                                                    </script>

                                                    

                                                    <div class="col-md-6">
                                                        <label for="accountHolder" class="form-label">Account Holder</label>
                                                        <asp:TextBox ID="accountHolder" runat="server" CssClass="form-control" />
                                                    </div>


                                                    <div class="col-md-6">
                                                        <label for="holderCode" class="form-label">A/C Holder Code</label>
                                                        <asp:TextBox ID="holderCode" runat="server" CssClass="form-control" Enabled="false" />
                                                    </div>

                                                    <div class="col-md-6">
                                                        <label for="RMNAMEP" class="form-label">RM</label>
                                                        <asp:TextBox ID="RMNAMEP" runat="server" CssClass="form-control" Enabled="false" />
                                                    </div>

                                                    <div class="col-md-6">
                                                        <label for="branch" class="form-label">Branch</label>
                                                        <asp:DropDownList ID="branch" runat="server" CssClass="form-select">
                                                            <asp:ListItem Text="--Select Branch--" Value="" />
                                                        </asp:DropDownList>
                                                    </div>

                                                    

                                                    <div class="col-md-6">
                                                        <label class="form-label">Regular/NFO</label>
                                                        <div class="d-flex align-items-center gap-3">
                                                            <div class="form-label">
                                                                <asp:RadioButton ID="regular" runat="server" GroupName="Dispatch" Text="Regular" />

                                                            </div>
                                                            <div class="form-label">
                                                                <asp:RadioButton ID="nfo" runat="server" GroupName="Dispatch" Text="NFO" />

                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-md-6">
                                                        <label for="amc" class="form-label">AMC</label>
                                                        <asp:DropDownList ID="amc" runat="server" CssClass="form-select">
                                                            <asp:ListItem Text="--Select AMC--" Value="" />
                                                            <asp:ListItem Text="AMC 1" Value="amc1" />
                                                            <asp:ListItem Text="AMC 2" Value="amc2" />

                                                        </asp:DropDownList>
                                                    </div>

                                                    <div class="col-md-12">
                                                        <label class="form-label">Scheme</label>
                                                        <div class="input-group">
                                                            <asp:TextBox ID="scheme" runat="server" CssClass="form-control" placeholder="Scheme" />
                                                            <asp:Button ID="btnSearchScheme" runat="server" CssClass="btn btn-outline-primary" Text="Search" OnClick="btnSearchSchemeADD" Enabled="true" />
                                                            <asp:HiddenField ID="hfSearchClicked" runat="server" Value="0" />
                                                        </div>
                                                    </div>



                                                    <asp:Panel ID="SchemeSearchPanel" runat="server" CssClass="panel panel-default draggable"
                                                        Style="display: none; position: fixed; top: 20%; left: 35%; width: 60%; max-height: 400px; overflow: auto; background-color: white; border: 1px solid #ccc; z-index: 1000;">

                                                        <div class="panel-heading" style="cursor: move; display: flex; justify-content: space-between; align-items: center;">
                                                            <h3 class="panel-title">Scheme Search</h3>
                                                            <button type="button" class="close" onclick="closeSchemeSearchPanel()" style="font-size: 18px;">&times;</button>
                                                        </div>

                                                        <div class="panel panel-primary">
                                                            <div class="panel-body">
                                                                <asp:Label ID="lblGo" runat="server" Text="Find:" CssClass="control-label" />
                                                                <asp:TextBox ID="txtGo" runat="server" CssClass="form-control" />

                                                                <div style="text-align: center; margin-top: 10px;">
                                                                    <asp:Button ID="btnGo" runat="server" Text="Go" CssClass="btn btn-primary btn-sm" OnClick="btnGo_Click" Style="margin-top: 5px;" />
                                                                    <button type="button" class="btn btn-secondary btn-sm" onclick="closeSchemeSearchPanel()" style="margin-left: 5px;">Close</button>
                                                                </div>

                                                                <div class="table-responsive">
                                                                    <asp:GridView ID="SchemeGrid" CssClass="table table-bordered" runat="server" AutoGenerateColumns="false" OnRowCommand="tableSearchResultsClient_RowCommand">
                                                                        <HeaderStyle CssClass="thead-dark" />
                                                                        <Columns>
                                                                            <asp:TemplateField HeaderText="Action" ItemStyle-HorizontalAlign="Center">
                                                                                <ItemTemplate>
                                                                                    <asp:Button ID="btnSelect" runat="server" Text="Select" CommandName="SelectRow" CommandArgument='<%# Eval("sch_code") %>' OnClick="btnSelectScheme_Click" />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Scheme Code" Visible="false">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblSchemeCode" runat="server" Text='<%# Eval("sch_code") %>' />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Scheme Name">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblSchemeName" runat="server" Text='<%# Eval("sch_name") %>' />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Short Name">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblShortName" runat="server" Text='<%# Eval("SHORT_NAME") %>' />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="AMC Name">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblAMCName" runat="server" Text='<%# Eval("mut_name") %>' />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            
                                                                        </Columns>
                                                                    </asp:GridView>
                                                                </div>



                                                            </div>
                                                        </div>
                                                    </asp:Panel>


                                                  

                                                    
                                                </div>


                                                <script>
                                                    function showError(input, message) {
                                                        let errorElement = input.parentNode.querySelector(".error-message");
                                                        if (!errorElement) {
                                                            errorElement = document.createElement("small");
                                                            errorElement.className = "error-message";
                                                            errorElement.style.color = "red";
                                                            errorElement.style.fontSize = "12px";
                                                            input.parentNode.appendChild(errorElement);
                                                        }
                                                        errorElement.textContent = message;
                                                        input.style.borderColor = "red";
                                                    }

                                                    function clearError(input) {
                                                        const errorElement = input.parentNode.querySelector(".error-message");
                                                        if (errorElement) {
                                                            errorElement.remove();
                                                        }
                                                        input.style.borderColor = "";
                                                    }

                                                    function allowOnlyNumeric(input) {
                                                        const regex = /^[0-9]*$/;
                                                        if (!regex.test(input.value)) {
                                                            input.value = input.value.replace(/[^0-9]/g, '');
                                                            showError(input, "Only numeric values are allowed.");
                                                        } else if (input.value === "") {
                                                            clearError(input);
                                                        } else {
                                                            clearError(input);
                                                        }
                                                        calculateExpenseAmount();
                                                    }

                                                    function validatePanInput(input) {
                                                        input.value = input.value.toUpperCase();
                                                        const regex = /^[A-Z]{5}[0-9]{4}[A-Z]{1}$/;
                                                        if (!regex.test(input.value) && input.value.length === 10) {
                                                            showError(input, "Invalid PAN format (e.g., 'AAAAA9999A').");
                                                        } else if (input.value === "") {
                                                            clearError(input);
                                                        } else {
                                                            clearError(input);
                                                        }
                                                    }

                                                    // Function to validate PIN code based on country
                                                    function validatePinInput(input) {
                                                        const countryId = document.getElementById('<%= hdncountryid.ClientID %>').value; // Get HiddenField value

                                                        if (countryId === "1") { // India
                                                            const regex = /^\d{6}$/;
                                                            if (!regex.test(input.value)) {
                                                                showError(input, "PIN must be exactly 6 numeric digits.");
                                                                input.value = input.value.replace(/\D/g, '').slice(0, 6);
                                                            } else {
                                                                clearError(input);
                                                            }
                                                        } else { // International
                                                            const regex = /^[a-zA-Z0-9]{1,10}$/; // Allows alphanumeric, up to 10 characters
                                                            if (!regex.test(input.value)) {
                                                                showError(input, "PIN must be alphanumeric and up to 10 characters.");
                                                                input.value = input.value.replace(/[^a-zA-Z0-9]/g, '').slice(0, 10);
                                                            } else {
                                                                clearError(input);
                                                            }
                                                        }
                                                    }

                                                    // Function to validate mobile number based on country
                                                    function validateMobileInput(input) {
                                                        const countryId = document.getElementById('<%= hdncountryid.ClientID %>').value; // Get HiddenField value

                                                        if (countryId === "1") { // India
                                                            const regex = /^[6-9]\d{9}$/; // Indian mobile numbers (10 digits, starts with 6-9)
                                                            if (!regex.test(input.value)) {
                                                                showError(input, "Mobile number must start with 6, 7, 8, or 9 and be exactly 10 digits.");
                                                                input.value = input.value.replace(/\D/g, '').slice(0, 10);
                                                            } else {
                                                                clearError(input);
                                                            }
                                                        } else { // International
                                                            const regex = /^\d+$/; // Accepts any number with 1 or more digits
                                                            if (!regex.test(input.value)) {
                                                                showError(input, "International mobile number can start with any digit and must have at least 10 digits.");
                                                                input.value = input.value.replace(/\D/g, '');
                                                            } else {
                                                                clearError(input);
                                                            }
                                                        }
                                                    }




                                                    function validateAadhaarInput(input) {
                                                        const regex = /^\d{12}$/;
                                                        if (!regex.test(input.value)) {
                                                            showError(input, "Aadhaar number must be exactly 12 numeric digits.");
                                                            input.value = input.value.replace(/\D/g, '').slice(0, 12);
                                                        } else if (input.value === "") {
                                                            clearError(input);
                                                        } else {
                                                            clearError(input);
                                                        }
                                                    }

                                                    function validateEmailInput(input) {
                                                        const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                                                        if (input.value && !regex.test(input.value)) {
                                                            showError(input, "Please enter a valid email address.");
                                                        } else if (input.value === "") {
                                                            clearError(input);
                                                        } else {
                                                            clearError(input);
                                                        }
                                                    }

                                                    function validateAmountInput(input) {
                                                        const regex = /^(\d+(\.\d{0,2})?)*$/; // This regex allows numbers with 1 or 2 decimal points.
                                                        if (!regex.test(input.value)) {
                                                            showError(input, "Please enter a valid amount with up to two decimal points.");
                                                            input.value = input.value.replace(/[^0-9.]/g, '').replace(/(\..*)\./g, '$1');  // Allow only one decimal point
                                                        } else {
                                                            clearError(input);
                                                        }
                                                    }

                                                    // Allow numeric input with '.' and '/' for cheque number
                                                    function validateChequeInput(input) {
                                                        const regex = /^[0-9\/.]*$/; // Allow numbers, slashes, and periods only
                                                        if (!regex.test(input.value)) {
                                                            showError(input, "Application number can only contain numbers, dots, and slashes.");
                                                            input.value = input.value.replace(/[^0-9/.]/g, ''); // Allow only numbers, '.' and '/'
                                                        } else {
                                                            clearError(input);
                                                        }
                                                    }

                                                    // Handle blur event to clear error messages when user moves to another field
                                                    function handleBlur(input) {
                                                        clearError(input);
                                                    }

                                                    function validateExpensesPercent(input) {
                                                        let sanitizedValue = input.value.replace(/[^0-9.]/g, '');

                                                        const dotCount = (sanitizedValue.match(/\./g) || []).length;
                                                        if (dotCount > 1) {
                                                            showError(input, "Only one decimal point is allowed.");
                                                            input.value = sanitizedValue = sanitizedValue.substring(0, sanitizedValue.lastIndexOf('.'));
                                                            return;
                                                        }

                                                        const value = parseFloat(sanitizedValue);

                                                        if (!isNaN(value) && value > 100) {
                                                            showError(input, "Expenses percentage cannot exceed 100.");
                                                            input.value = "100";
                                                        } else if (input.value === "") {
                                                            clearError(input);
                                                        } else if (isNaN(value)) {
                                                            showError(input, "Please enter a valid numeric value.");
                                                            input.value = "";
                                                        } else {
                                                            clearError(input);
                                                            input.value = sanitizedValue;
                                                        }
                                                        input.value = sanitizedValue;

                                                        calculateExpenseAmount();
                                                    }


                                                    function calculateExpenseAmount() {
                                                        const amountInput = document.getElementById('<%= amountt.ClientID %>');
                                                        const percentInput = document.getElementById('<%= txtExpensesPercent.ClientID %>');
                                                        const expenseRsInput = document.getElementById('<%= txtExpensesRs.ClientID %>');

                                                        const amountInput1 = document.getElementById('<%= TextBox14.ClientID %>');
                                                        const percentInput1 = document.getElementById('<%= expensesPercent.ClientID %>');
                                                        const expenseRsInput1 = document.getElementById('<%= expensesRs.ClientID %>');


                                                        const amount = parseFloat(amountInput.value) || 0;
                                                        const percent = parseFloat(percentInput.value) || 0;

                                                        const amount1 = parseFloat(amountInput1.value) || 0;
                                                        const percent1 = parseFloat(percentInput1.value) || 0;


                                                        const expenseAmount = (amount * percent) / 100;

                                                        const expenseAmount1 = (amount1 * percent1) / 100;


                                                        if (!isNaN(expenseAmount)) {
                                                            expenseRsInput.value = expenseAmount.toFixed(2);
                                                        } else {
                                                            expenseRsInput.value = ""; // Clear field if invalid
                                                        }

                                                        if (!isNaN(expenseAmount1)) {
                                                            expenseRsInput1.value = expenseAmount1.toFixed(2);
                                                        } else {
                                                            expenseRsInput1.value = ""; // Clear field if invalid
                                                        }
                                                    }
                                                </script>

                                                <style>
                                                    .error-message {
                                                        display: block;
                                                        margin-top: 2px;
                                                        font-size: 12px; /* Small and compact */
                                                        color: red;
                                                    }
                                                </style>

                                                <h4 class="card-title mt-3">Switch/STP</h4>

                                                <div class="row align-items-center">
                                                    <div class="col-md-12">
                                                        <label for="formSwitchFolio" class="form-label">Form Switch/STP Folio</label>
                                                        <asp:TextBox ID="formSwitchFolio" runat="server" CssClass="form-control" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-md-6">
                                            <h4 class="card-title">Application And Payment Details</h4>
                                            <div>
                                                <div class="row g-3 mt-5">
                                                    <div class="col-md-4">
                                                        <label for="applicationNo" class="form-label">Application No</label>
                                                        <asp:TextBox ID="applicationNo" runat="server" oninput="validateChequeInput(this)" onblur="handleBlur(this)" CssClass="form-control"></asp:TextBox>
                                                    </div>

                                                    <div class="col-md-4">
                                                        <label for="folioNo" class="form-label">Folio No</label>
                                                        <asp:TextBox ID="folioNoo" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <label for="amount" class="form-label">Amount</label>
                                                        <asp:TextBox ID="amountt" runat="server" oninput="validateAmountInput(this)" onblur="handleBlur(this)" CssClass="form-control"></asp:TextBox>
                                                    </div>

                                                    <script>
                                                        var a = ['', 'One ', 'Two ', 'Three ', 'Four ', 'Five ', 'Six ', 'Seven ', 'Eight ', 'Nine ', 'Ten ', 'Eleven ', 'Twelve ', 'Thirteen ', 'Fourteen ', 'Fifteen ', 'Sixteen ', 'Seventeen ', 'Eighteen ', 'Nineteen '];
                                                        var b = ['', '', 'Twenty', 'Thirty', 'Forty', 'Fifty', 'Sixty', 'Seventy', 'Eighty', 'Ninety'];

                                                        function inWords(num) {
                                                            if ((num = num.toString()).length > 9) return 'overflow';
                                                            n = ('000000000' + num).substr(-9).match(/^(\d{2})(\d{2})(\d{2})(\d{1})(\d{2})$/);
                                                            if (!n) return;
                                                            var str = '';
                                                            str += (n[1] != 0) ? (a[Number(n[1])] || b[n[1][0]] + ' ' + a[n[1][1]]) + 'Crore ' : '';
                                                            str += (n[2] != 0) ? (a[Number(n[2])] || b[n[2][0]] + ' ' + a[n[2][1]]) + 'Lakh ' : '';
                                                            str += (n[3] != 0) ? (a[Number(n[3])] || b[n[3][0]] + ' ' + a[n[3][1]]) + 'Thousand ' : '';
                                                            str += (n[4] != 0) ? (a[Number(n[4])] || b[n[4][0]] + ' ' + a[n[4][1]]) + 'Hundred ' : '';
                                                            str += (n[5] != 0) ? ((str != '') ? 'And ' : '') + (a[Number(n[5])] || b[n[5][0]] + ' ' + a[n[5][1]]) + ' ' : '';
                                                            return str;
                                                        }

                                                        // Function to handle the blur event
                                                        function handleAmountBlur(element) {
                                                            var amount = element.value;

                                                            if (!document.hasFocus()) {
                                                                return;
                                                            }

                                                            if (amount) {
                                                                var words = inWords(amount);
                                                                // Display confirmation dialog
                                                                var confirmMessage = words + 'Rupee Only';
                                                                var userConfirmed = window.confirm(confirmMessage);

                                                                // If the user clicks "Cancel", clear the textbox, else show the alert
                                                                if (!userConfirmed) {
                                                                    element.value = ''; // Clear the textbox
                                                                }
                                                            }
                                                        }

                                                        document.addEventListener('DOMContentLoaded', function () {
                                                            var amountField = document.getElementById('<%= amountt.ClientID %>');
                                                            if (amountField) {
                                                                amountField.addEventListener('blur', function () {
                                                                    handleAmountBlur(amountField);
                                                                });
                                                            }
                                                        });

                                                        // Rebind the blur event after partial postback using the Sys.Application.add_load
                                                        Sys.Application.add_load(function () {
                                                            var amountField = document.getElementById('<%= amountt.ClientID %>');
                                                            if (amountField) {
                                                                amountField.addEventListener('blur', function () {
                                                                    handleAmountBlur(amountField);
                                                                });
                                                            }
                                                        });
                                                    </script>

                                                   <%-- Payment Methods --%>
                                                    <div id="paymentMethodContainer" class="col-md-4 mt-4">
                                                        <div class="form-label">
                                                            <asp:RadioButton ID="cheque" runat="server" GroupName="paymentMethod" Text="Cheque" InputAttributes-Value="cheque" />
                                                        </div>
                                                        <div class="form-label">
                                                            <asp:RadioButton ID="draft" runat="server" GroupName="paymentMethod" Text="Draft" InputAttributes-Value="draft" />
                                                        </div>
                                                        <div class="form-label">
                                                            <asp:RadioButton ID="ecs" runat="server" GroupName="paymentMethod" Text="ECS" InputAttributes-Value="ecs" />
                                                        </div>
                                                        <div class="form-label">
                                                            <asp:RadioButton ID="cash" runat="server" GroupName="paymentMethod" Text="Cash" InputAttributes-Value="cash" />
                                                        </div>
                                                        <div class="form-label">
                                                            <asp:RadioButton ID="others" runat="server" GroupName="paymentMethod" Text="Others" InputAttributes-Value="others" />
                                                        </div>
                                                        <div class="form-label">
                                                            <asp:RadioButton ID="rtgs" runat="server" GroupName="paymentMethod" Text="RTGS" InputAttributes-Value="rtgs" />
                                                        </div>
                                                        <div class="form-label">
                                                            <asp:RadioButton ID="neft" runat="server" GroupName="paymentMethod" Text="Fund Transfer" InputAttributes-Value="neft" />
                                                        </div>
                                                    </div>

                                                    <!-- Cheque Details -->
                                                    <div class="col-md-4 payment-detail" id="chequeDetails1" style="display: none;">
                                                        <asp:Label ID="Label1" runat="server" CssClass="form-label" AssociatedControlID="txtChequeNo" Text="Cheque No" />
                                                        <span class="text-danger">*</span>
                                                        <asp:TextBox ID="txtChequeNo" runat="server" CssClass="form-control" />
                                                        <asp:Label ID="lblChequeDated" runat="server" CssClass="form-label" AssociatedControlID="txtChequeDated" Text="Cheque Dated" />
                                                        <span class="text-danger">*</span>
                                                        <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                            <asp:TextBox ID="txtChequeDated" runat="server" CssClass="form-control date-input" placeholder="dd/mm/yyyy" />
                                                            <div class="input-group-addon">
                                                                <span class="glyphicon glyphicon-th"></span>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <!-- Draft Details -->
                                                    <div id="draftDetails" class="col-md-4 payment-detail" style="display: none;">
                                                        <asp:Label ID="lblDraftNo" runat="server" CssClass="form-label" Text="Draft No" />
                                                        <asp:TextBox ID="txtDraftNo" runat="server" CssClass="form-control" />
                                                        <asp:Label ID="lblDraftDate" runat="server" CssClass="form-label" AssociatedControlID="txtDraftDate" Text="Draft Date" />
                                                        <span class="text-danger">*</span>
                                                        <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                            <asp:TextBox ID="txtDraftDate" runat="server" CssClass="form-control date-input" placeholder="dd/mm/yyyy" />
                                                            <div class="input-group-addon">
                                                                <span class="glyphicon glyphicon-th"></span>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <!-- RTGS Details -->
                                                    <div id="rtgsDetails" class="col-md-4 payment-detail" style="display: none;">
                                                        <asp:Label ID="lblRtgsNo" runat="server" CssClass="form-label" Text="RTGS Transaction No" />
                                                        <asp:TextBox ID="txtRtgsNo" runat="server" CssClass="form-control" />
                                                        <asp:Label ID="lblRtgsDate" runat="server" CssClass="form-label" AssociatedControlID="txtRtgsDate" Text="RTGS Date" />
                                                        <span class="text-danger">*</span>
                                                        <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                            <asp:TextBox ID="txtRtgsDate" runat="server" CssClass="form-control date-input" placeholder="dd/mm/yyyy" />
                                                            <div class="input-group-addon">
                                                                <span class="glyphicon glyphicon-th"></span>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <!-- NEFT/Fund Transfer Details -->
                                                    <div id="neftDetails" class="col-md-4 payment-detail" style="display: none;">
                                                        <asp:Label ID="lblNeftNo" runat="server" CssClass="form-label" Text="Fund Transfer No" />
                                                        <asp:TextBox ID="txtNeftNo" runat="server" CssClass="form-control" />
                                                        <asp:Label ID="lblNeftDate" runat="server" CssClass="form-label" AssociatedControlID="txtNeftDate" Text="Fund Transfer Date" />
                                                        <span class="text-danger">*</span>
                                                        <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                            <asp:TextBox ID="txtNeftDate" runat="server" CssClass="form-control date-input" placeholder="dd/mm/yyyy" />
                                                            <div class="input-group-addon">
                                                                <span class="glyphicon glyphicon-th"></span>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <!-- ECS Details -->
                                                    <div id="ecsDetails" class="col-md-4 payment-detail" style="display: none;">
                                                        <asp:Label ID="lblEcsReference" runat="server" CssClass="form-label" Text="ECS Reference No" />
                                                        <asp:TextBox ID="txtEcsReference" runat="server" CssClass="form-control" />
                                                        <asp:Label ID="lblEcsDate" runat="server" CssClass="form-label" AssociatedControlID="txtEcsDate" Text="ECS Date" />
                                                        <span class="text-danger">*</span>
                                                        <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                            <asp:TextBox ID="txtEcsDate" runat="server" CssClass="form-control date-input" placeholder="dd/mm/yyyy" />
                                                            <div class="input-group-addon">
                                                                <span class="glyphicon glyphicon-th"></span>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <!-- Cash Payment Details -->
                                                    <div id="cashDetails" class="col-md-4 payment-detail" style="display: none;">
                                                        <asp:Label ID="lblCashAmount" runat="server" CssClass="form-label" Text="Cash Amount" />
                                                        <asp:TextBox ID="txtCashAmount" runat="server" CssClass="form-control" />
                                                        <asp:Label ID="lblCashDate" runat="server" CssClass="form-label" Text="Cash Payment Date" />
                                                        <span class="text-danger">*</span>
                                                        <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                            <asp:TextBox ID="txtCashDate" runat="server" CssClass="form-control date-input" placeholder="dd/mm/yyyy" />
                                                            <div class="input-group-addon">
                                                                <span class="glyphicon glyphicon-th"></span>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <!-- Others Payment Details -->
                                                    <div id="othersDetails" class="col-md-4 payment-detail" style="display: none;">
                                                        <asp:Label ID="lblOthersReference" runat="server" CssClass="form-label" Text="Others Reference No" />
                                                        <asp:TextBox ID="txtOthersReference" runat="server" CssClass="form-control" />
                                                        <asp:Label ID="lblOthersDate" runat="server" CssClass="form-label" Text="Payment Date" />
                                                        <span class="text-danger">*</span>
                                                        <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                            <asp:TextBox ID="txtOthersDate" runat="server" CssClass="form-control date-input" placeholder="dd/mm/yyyy" />
                                                            <div class="input-group-addon">
                                                                <span class="glyphicon glyphicon-th"></span>
                                                            </div>
                                                        </div>
                                                    </div>


                                                    <%-- Bank Name --%>
                                                    <div>
                                                        <asp:Label ID="lblBankName" runat="server" CssClass="form-label" AssociatedControlID="ddlBankName" Text="Bank Name" />
                                                        <asp:DropDownList ID="ddlBankName" runat="server" CssClass="form-select">
                                                            <asp:ListItem Value="" Text="--Select Bank--" />
                                                            <asp:ListItem Value="bank1" Text="Bank 1" />
                                                            <asp:ListItem Value="bank2" Text="Bank 2" />

                                                        </asp:DropDownList>
                                                    </div>

                                                    <%-- Expenses Percent --%>
                                                    <div class="col-md-6">
                                                        <asp:Label ID="Label2" runat="server" CssClass="form-label" AssociatedControlID="txtExpensesPercent" Text="Expenses%" />
                                                        <asp:TextBox ID="txtExpensesPercent" oninput="validateExpensesPercent(this)" runat="server" CssClass="form-control" Enabled="false" />
                                                    </div>

                                                    <%-- Expenses (Rs.) --%>
                                                    <div class="col-md-6">
                                                        <asp:Label ID="Label3" runat="server" CssClass="form-label" AssociatedControlID="txtExpensesRs" Text="Expenses (Rs.)" />
                                                        <asp:TextBox ID="txtExpensesRs" runat="server" CssClass="form-control" oninput="allowOnlyNumeric(this)" Enabled="false" />
                                                    </div>

                                                    <%-- Auto Switch On Maturity --%>
                                                    <div>
                                                        <div class="form-label">
                                                            <asp:CheckBox ID="chkAutoSwitchOnMaturity" runat="server" />
                                                            <asp:Label ID="Label4" runat="server" CssClass="form-check-label" AssociatedControlID="chkAutoSwitchOnMaturity" Text="Auto Switch On Maturity" />
                                                        </div>
                                                    </div>

                                                   <%-- Button: Scheme Search  --%>
                                                    <div>
                                                        <div class="input-group">
                                                            <asp:TextBox ID="txtSearchSchemeDetails" runat="server" CssClass="form-control" placeholder="Enter details" />
                                                            <asp:Button ID="btnSearchSchemeDetails" runat="server" CssClass="btn btn-outline-primary" Text="Search" OnClick="btnSearchSchemeDetails_Click" />
                                                        </div>
                                                    </div>

                                                    <%-- Transaction Type --%>
                                                    <div class="col-md-6">
                                                        <label for="transactionType" class="form-label">Transaction Type</label>
                                                        <asp:DropDownList ID="transactionType" runat="server" CssClass="form-select" Required="True">

                                                            <asp:ListItem Text="PURCHASE" Value="PURCHASE" />
                                                            <asp:ListItem Text="SWITCH IN" Value="SWITCH IN" />

                                                        </asp:DropDownList>
                                                    </div>

                                                    <br />
                                                    <br />

                                                    <%-- Form Switch / STP Scheme --%>
                                                    <%-- Button: Search Form Switch / STP Scheme --%>
                                                    <div class="col-md-12 mt-4">
                                                        <label for="formSwitchScheme" class="form-label">Form Switch / STP Scheme</label>
                                                        <div class="input-group">

                                                            <asp:TextBox ID="formSwitchScheme" runat="server" CssClass="form-control" placeholder="Scheme" ReadOnly="true" />

                                                            <asp:HiddenField ID="selectedOption" runat="server" />
                                                            <asp:HiddenField ID="hiddenSchemeName" runat="server" />
                                                            <asp:HiddenField ID="hiddenCombinedText" runat="server" />
                                                            <asp:HiddenField ID="hiddenAMCName" runat="server" />

                                                            <asp:Button ID="btnSearchFormSwitchScheme" runat="server" CssClass="btn btn-outline-primary" Text="Search" OnClick="btn_SearchFormSwitchScheme" Enabled="false" />
                                                        </div>
                                                    </div>



                                                    <asp:Panel ID="SchemeDetailsPanel" runat="server" CssClass="panel panel-default draggable"
                                                        Style="display: none; position: fixed; top: 25%; left: 35%; width: 30%; max-height: 400px; overflow: auto; background-color: white; border: 1px solid #ccc; z-index: 1000;">

                                                        <div class="panel-heading" style="cursor: move; display: flex; justify-content: space-between; align-items: center;">
                                                            <h3 class="panel-title">Scheme Details Search</h3>
                                                            <button type="button" class="close" onclick="closeSchemeDetailsPanel()" style="font-size: 18px;">&times;</button>
                                                        </div>

                                                        <div class="panel panel-primary">
                                                            <div class="panel-body">
                                                                <asp:Label ID="lblSearchDetails" runat="server" Text="Find Details:" CssClass="control-label" />
                                                                <asp:TextBox ID="txtSearchDetails" runat="server" CssClass="form-control" ReadOnly="true" />

                                                                <div style="text-align: center; margin-top: 10px;">
                                                                    <asp:Button ID="btnSearchDetails" runat="server" Text="Search" CssClass="btn btn-primary btn-sm" OnClick="btnSearchDetails_Click" Style="margin-top: 5px;" />
                                                                    <button type="button" class="btn btn-secondary btn-sm" onclick="closeSchemeDetailsPanel()" style="margin-left: 5px;">Close</button>
                                                                </div>

                                                                <div class="table-responsive">
                                                                    <asp:GridView ID="DetailsGrid" CssClass="table table-bordered" runat="server" AutoGenerateColumns="false" OnRowCommand="detailsSearchResultsClient_RowCommand">
                                                                        <HeaderStyle CssClass="thead-dark" />
                                                                        <Columns>
                                                                            <asp:TemplateField HeaderText="Action" ItemStyle-HorizontalAlign="Center">
                                                                                <ItemTemplate>
                                                                                    <asp:Button ID="btnSelectDetail" runat="server" Text="Select" CommandName="SelectDetailRow" CommandArgument='<%# Eval("sch_code") %>' OnClick="btnSelectDetail_Click" />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Scheme Code">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblDetailCode" runat="server" Text='<%# Eval("sch_code") %>' />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Scheme Name">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblDetailName" runat="server" Text='<%# Eval("sch_name") %>' />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Short Name">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblDetailShortName" runat="server" Text='<%# Eval("short_name") %>' />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="AMC Name">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblDetailAMCName" runat="server" Text='<%# Eval("mut_name") %>' />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                    </asp:GridView>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </asp:Panel>

                                                    <script>
                                                        function showSchemeDetailsPanel() {
                                                            document.getElementById('<%= SchemeDetailsPanel.ClientID %>').style.display = 'block';
                                                        }

                                                        function closeSchemeDetailsPanel() {
                                                            document.getElementById('<%= SchemeDetailsPanel.ClientID %>').style.display = 'none';
                                                        }
                                                    </script>
                                                </div>
                                            </div>
                                        </div>

                                        <div>
                                            <h4 class="card-title">SIP details</h4>

                                            <div class="row align-items-center">

                                                <div class="col-md-4">
                                                    <asp:Label ID="lblSIPSTP" runat="server" CssClass="form-label" AssociatedControlID="ddlSipStp" Text="SIP/STP" />
                                                    <asp:DropDownList ID="ddlSipStp" runat="server" CssClass="form-select">

                                                        <asp:ListItem Value="REGULAR" Text="REGULAR" />
                                                        <asp:ListItem Value="SIP" Text="SIP" />
                                                        <asp:ListItem Value="STP" Text="STP" />
                                                    </asp:DropDownList>
                                                </div>

                                                <asp:HiddenField ID="hdnSipStpValue" runat="server" />

                                                <div class="col-md-4" id="installmentTypeContainer" style="display: none;">
                                                    <asp:Label ID="iNSTYPEL" runat="server" CssClass="form-label" AssociatedControlID="iNSTYPE" Text="Installment Type" />
                                                    <asp:DropDownList ID="iNSTYPE" runat="server" CssClass="form-select">
                                                        <asp:ListItem Value="NORMAL" Text="Normal" />
                                                        <asp:ListItem Value="MICRO" Text="Micro" />
                                                    </asp:DropDownList>
                                                </div>
                                            </div>


                                            <div class="row align-items-center">
                                                <div class="col-md-4" id="radioButtonsContainer" style="display: none;">
                                                    <div class="d-flex align-items-center gap-3">
                                                        <div class="form-label">
                                                            <asp:RadioButton ID="fresh" runat="server" GroupName="sIp" Text="Fresh" Checked="true" />
                                                        </div>
                                                        <div class="form-label">
                                                            <asp:RadioButton ID="renewal" runat="server" GroupName="sIp" Text="Renewal" />
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="col-md-4" id="sipTypeContainer" style="display: none;">
                                                    <asp:Label ID="siptypeL" runat="server" CssClass="form-label" AssociatedControlID="siptype" Text="SIP TYPE" />
                                                    <asp:DropDownList ID="siptype" runat="server" CssClass="form-select">
                                                        <asp:ListItem Value="N" Text="NORMAL" />
                                                        <asp:ListItem Value="Y" Text="MICROSIP" />
                                                    </asp:DropDownList>
                                                </div>

                                                <div class="col-md-4" id="sipAmountContainer" style="display: none;">
                                                    <asp:Label ID="sipamountL" runat="server" CssClass="form-label" AssociatedControlID="sipamount" Text="Sip Amount" />
                                                    <asp:TextBox ID="sipamount" runat="server" CssClass="form-control" oninput="validateAmountInput(this)" onblur="handleBlur(this)" />
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-md-4">
                                            <asp:Label ID="lblFrequency" runat="server" CssClass="form-label" AssociatedControlID="ddlFrequency" Text="Frequency" />
                                            <asp:DropDownList ID="ddlFrequency" runat="server" CssClass="form-select" oninput="calculateSIPEndDate()">
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
                                            <asp:Label ID="lblInstallmentsNos" runat="server" CssClass="form-label" AssociatedControlID="txtInstallmentsNos" Text="Installments Nos" />
                                            <asp:TextBox ID="txtInstallmentsNos" runat="server" CssClass="form-control" oninput="calculateSIPEndDate()" onkeypress="allowOnlyNumeric(this)" MaxLength="4"/>
                                        </div>

                                        <div class="col-md-6">
                                            <asp:Label ID="lblSIPStartDate" runat="server" CssClass="form-label" AssociatedControlID="txtSIPStartDate" Text="SIP Start Date" />
                                            <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                <asp:TextBox ID="txtSIPStartDate" runat="server" CssClass="form-control date-input" placeholder="dd/mm/yyyy" onblur="calculateSIPEndDate()" />
                                                <div class="input-group-addon">
                                                    <span class="glyphicon glyphicon-th"></span>
                                                </div>
                                            </div>
                                        </div>

                                        <script>
                                            function calculateSIPEndDate() {
                                                const startDateInput = document.getElementById("<%= txtSIPStartDate.ClientID %>");
                                                const endDateInput = document.getElementById("<%= txtSIPEndDate.ClientID %>");
                                                const installmentsInput = document.getElementById("<%= txtInstallmentsNos.ClientID %>");
    const frequencyInput = document.getElementById("<%= ddlFrequency.ClientID %>");
                                                const rdo99Years = document.getElementById("<%= rdo99Years.ClientID %>"); // "99 Years or more" radio button

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

                                                // Calculate the SIP End Date
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

                                        <div class="col-md-6">
                                            <asp:Label ID="lblSIPEndDate" runat="server" CssClass="form-label" AssociatedControlID="txtSIPEndDate" Text="SIP End Date" />
                                            <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                <asp:TextBox ID="txtSIPEndDate" runat="server" CssClass="form-control date-input" placeholder="dd/mm/yyyy" />
                                                <div class="input-group-addon">
                                                    <span class="glyphicon glyphicon-th"></span>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-md-3">
                                            <div class="form-label">
                                                <asp:CheckBox ID="chkCOBCase" runat="server" />
                                                <asp:Label ID="lblCOBCase" runat="server" CssClass="form-check-label" AssociatedControlID="chkCOBCase" Text="COB Case" />
                                            </div>
                                        </div>

                                        <div class="col-md-3">
                                            <div class="form-label">
                                                <asp:CheckBox ID="chkSWPCase" runat="server" />
                                                <asp:Label ID="lblSWPCase" runat="server" CssClass="form-check-label" AssociatedControlID="chkSWPCase" Text="SWP Case" />
                                            </div>
                                        </div>

                                        <div class="col-md-3">
                                            <div class="form-label">
                                                <asp:CheckBox ID="chkFreedomSIP" runat="server" />
                                                <asp:Label ID="lblFreedomSIP" runat="server" CssClass="form-check-label" AssociatedControlID="chkFreedomSIP" Text="Freedom SIP" />
                                            </div>
                                        </div>

                                        <div class="col-md-3">
                                            <div class="form-label">
                                                <asp:RadioButton ID="rdo99Years" runat="server" GroupName="duration" Text="OR 99 Years or more" onchange="calculateSIPEndDate()" />
                                            </div>
                                        </div>

                                        <h4 class="card-title">PAN details
                                        </h4>

                                        <div class="col-md-4">
                                            <label for="pan" class="form-label">
                                                PAN No
                                            </label>
                                            <asp:TextBox ID="pann" runat="server" oninput="validatePanInput(this)" onblur="handleBlur(this)" MaxLength="10" CssClass="form-control"></asp:TextBox>
                                        </div>

                                        <div class="col-md-8">
                                            <div class="d-flex align-items-md-center flex-wrap gap-3">
                                                <asp:Button ID="btnAdd" runat="server" CssClass="btn btn-primary" Text="Add" OnClick="btnAddClick" />
                                                <asp:Button ID="btnPrintAR" runat="server" CssClass="btn btn-outline-primary" Text="Print AR" OnClick="btn_PrintAR" Enabled="false" />
                                                <asp:Button ID="btnReset" runat="server" CssClass="btn btn-outline-primary" Text="Reset" OnClick="btnResetClick"  />
                                                <asp:Button ID="btnLeadSearch" runat="server" CssClass="btn btn-outline-primary" Text="Lead Search" Enabled="false" />
                                                <asp:Button ID="btnChangeInvestorDetails" runat="server" CssClass="btn btn-outline-primary" Text="Change Investor Details" OnClick="btnAdd_Change" />
                                                <asp:Button ID="btnExit" runat="server" CssClass="btn btn-outline-primary" Text="Exit" OnClick="btnExit_Click" />
                                            </div>
                                        </div>

                                        <script type="text/javascript">
                                            function ResetPaymentMethod() {
                                                var hiddenField = document.getElementById('<%= hdnSelectedPaymentMethod.ClientID %>');
                                                if (hiddenField) {
                                                    hiddenField.value = 'cheque';  // Set the hidden field value to 'cheque'
                                                }
                                                var paymentMethodContainer = document.getElementById("paymentMethodContainer");
                                                if (paymentMethodContainer) {
                                                    paymentMethodContainer.value = 'cheque';  // Set the value to 'cheque'
                                                    var event = new Event('change');
                                                    paymentMethodContainer.dispatchEvent(event);  // Trigger the change event
                                                }
                                            }
                                        </script>

                                        <asp:TextBox ID="lblWarning" runat="server" CssClass="form-control" ReadOnly="true" BackColor="#ffdddd" Visible="false" />


                                        <script>
                                            // Function to remove commas from input fields
                                            function removeCommasFromFields() {
                                                const fieldIds = [
            '<%= txtAddressADD.ClientID %>',
            '<%= txtAddressADD2.ClientID %>',
            '<%= txtPinADD.ClientID %>',
            '<%= txtMobileADD.ClientID %>',
            '<%= txtPanADD.ClientID %>',
            '<%= txtEmailADD.ClientID %>',
            '<%= txtAadharADD.ClientID %>',
            '<%= txtDOBADD.ClientID %>',
            '<%= hdncountryid.ClientID %>'

                                                ];

                                                fieldIds.forEach(id => {
                                                    const field = document.getElementById(id);
                                                    if (field && field.value.includes(',')) {
                                                        field.value = field.value.replace(/,/g, '').trim();
                                                    }
                                                });
                                            }

                                            // Attach event listener to detect UpdatePanel updates
                                            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                                                removeCommasFromFields(); // Remove commas after partial postback
                                            });

                                            // Ensure commas are removed on page load
                                            document.addEventListener("DOMContentLoaded", function () {
                                                removeCommasFromFields();
                                            });
                                        </script>



                                    </div>
                                    </div>


                        <asp:Panel ID="Panel2" runat="server" CssClass="panel panel-default draggable"
                            Style="display: none; position: fixed; top: 20%; left: 35%; width: 30%; max-height: 400px; overflow: auto; background-color: white; border: 1px solid #ccc; z-index: 1000; padding: 15px;">
                            <div class="panel-heading" style="cursor: move; display: flex; justify-content: space-between; align-items: center;">
                                <h4 class="panel-title">Investor Address Updation</h4>
                                <button type="button" class="close" onclick="hidePopup3()" style="font-size: 18px;">&times;</button>
                            </div>
                            <div class="panel-body">
                                <h5>Investor Details</h5>

                                <!-- Address Fields -->
                                <div class="form-group" style="display: flex; flex-wrap: wrap; gap: 15px;">
                                    <div style="flex: 1;">
                                        <label for="txtAddressADD">Address 1</label>
                                        <asp:TextBox ID="txtAddressADD" runat="server" CssClass="form-control" Style="margin-bottom: 10px;"></asp:TextBox>
                                    </div>
                                    <div style="flex: 1;">
                                        <label for="txtPinADD">PIN</label>
                                        <asp:TextBox ID="txtPinADD" runat="server" onblur="handleBlur(this)" oninput="validatePinInput(this)" CssClass="form-control" Style="margin-bottom: 10px;" MaxLength="10"></asp:TextBox>
                                    </div>

                                </div>

                                <!-- Mobile and PAN -->
                                <div class="form-group" style="display: flex; flex-wrap: wrap; gap: 15px;">
                                    <div style="flex: 1;">
                                        <label for="txtAddressADD2">Address 2</label>
                                        <asp:TextBox ID="txtAddressADD2" runat="server" CssClass="form-control" Style="margin-bottom: 10px;"></asp:TextBox>
                                    </div>
                                   
                                    <div style="flex: 1;">
                                        <label for="txtMobileADD">Mobile</label>
                                        <asp:TextBox ID="txtMobileADD" runat="server" onblur="handleBlur(this)" oninput="validateMobileInput(this)" CssClass="form-control" Style="margin-bottom: 10px;"></asp:TextBox>
                                    </div>
                                    
                                </div>

                                <!-- Email and Aadhaar -->
                                <div class="form-group" style="display: flex; flex-wrap: wrap; gap: 15px;">
                                    <div style="flex: 1;">
                                        <label for="txtPanADD">PAN</label>
                                        <asp:TextBox ID="txtPanADD" runat="server" onblur="handleBlur(this)" oninput="validatePanInput(this)" CssClass="form-control" Style="margin-bottom: 10px;" MaxLength="10"></asp:TextBox>
                                    </div>
                                    <div style="flex: 1;">
                                        <label for="txtEmailADD">Email</label>
                                        <asp:TextBox ID="txtEmailADD" runat="server" onblur="handleBlur(this)" oninput="validateEmailInput(this)" CssClass="form-control" Style="margin-bottom: 10px;"></asp:TextBox>
                                    </div>
                                   
                                </div>

                                <!-- Additional Fields -->
                                <div class="form-group" style="display: flex; flex-wrap: wrap; gap: 15px;">
                                    <div style="flex: 1;">
                                        <label for="txtAadharADD">Aadhar</label>
                                        <asp:TextBox ID="txtAadharADD" runat="server" onblur="handleBlur(this)" oninput="validateAadhaarInput(this)" CssClass="form-control" Style="margin-bottom: 10px;" MaxLength="12"></asp:TextBox>
                                    </div>
                                    <div style="flex: 1;">
                                        <label class="form-label">
                                            Date of Birth (DOB)
                                        </label>
                                        <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                            <asp:TextBox ID="txtDOBADD" runat="server" CssClass="form-control date-input" placeholder="dd/mm/yyyy" />
                                            <div class="input-group-addon">
                                                <span class="glyphicon glyphicon-th"></span>
                                            </div>
                                        </div>
                                    </div>

                                </div>

                            <div class="form-group" style="display: flex; flex-wrap: wrap; gap: 15px;">
                                <div style="flex: 1;">
                                    <label for="ddlCityADD">City</label>
                                    <asp:DropDownList ID="ddlCityADD" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="ddlCity_SelectedIndexChanged" Style="margin-bottom: 10px;">
                                        <asp:ListItem Text="--Select city--" Value="" />
                                    </asp:DropDownList>
                                </div>
                               
                                    <div style="flex: 1;">
                                        <label for="ddlStateADD">State</label>
                                        <asp:DropDownList ID="ddlStateADD" runat="server" CssClass="form-control" Style="margin-bottom: 10px;">
                                            <asp:ListItem Text="--Select state--" Value="" />
                                        </asp:DropDownList>
                                    </div>
                                
                            </div>

                                <div class="form-group" style="text-align: center;">
                                    <asp:Button ID="Button3" runat="server" CssClass="btn btn-primary" Text="Update" OnClick="btnPanel2Update_Click" />
                                    <asp:Button ID="Button7" runat="server" CssClass="btn btn-secondary" Text="Exit" OnClientClick="hidePopup3(); return false;" />
                                </div>
                            </div>
                        </asp:Panel>

                                    <asp:HiddenField ID="hdncountryid" runat="server" />




                                    <script type="text/javascript">
                                        // Make panel draggable
                                        function makePanelDraggable() {
                                            var panel = $('.draggable');
                                            var header = $('.panel-heading', panel);

                                            var isDragging = false;
                                            var offsetX, offsetY;

                                            // Mouse down event on header
                                            header.on('mousedown', function (e) {
                                                isDragging = true;

                                                // Get current position
                                                var pos = panel.offset();

                                                // Calculate offset between mouse and panel position
                                                offsetX = e.clientX - pos.left;
                                                offsetY = e.clientY - pos.top;

                                                // Remove transform to allow manual positioning
                                                panel.css({
                                                    'transform': 'none',
                                                    'top': pos.top + 'px',
                                                    'left': pos.left + 'px'
                                                });

                                                // Prevent text selection
                                                e.preventDefault();
                                            });

                                            // Mouse move event
                                            $(document).on('mousemove', function (e) {
                                                if (!isDragging) return;

                                                // Calculate new position
                                                var newX = e.clientX - offsetX;
                                                var newY = e.clientY - offsetY;

                                                // Get viewport dimensions
                                                var viewportWidth = $(window).width();
                                                var viewportHeight = $(window).height();

                                                // Get panel dimensions
                                                var panelWidth = panel.outerWidth();
                                                var panelHeight = panel.outerHeight();

                                                // Constrain to viewport boundaries
                                                newX = Math.max(0, Math.min(newX, viewportWidth - panelWidth));
                                                newY = Math.max(0, Math.min(newY, viewportHeight - panelHeight));

                                                // Apply new position
                                                panel.css({
                                                    'left': newX + 'px',
                                                    'top': newY + 'px'
                                                });
                                            });

                                            // Mouse up event
                                            $(document).on('mouseup', function () {
                                                isDragging = false;
                                            });
                                        }

                                        // Call this function when the panel is shown
                                        function showDuplicatePopup() {
                                            var panel = $('.draggable');
                                            panel.show();
                                            makePanelDraggable();
                                        }

                                        // Your existing cancel function
                                        function cancelPopup() {
                                            $('.draggable').hide();
                                        }

                                        // Initialize when document is ready
                                        $(document).ready(function () {
                                            makePanelDraggable();
                                        });
                                    </script>
                                    <asp:Panel ID="popupDuplicate" runat="server" CssClass="panel panel-default draggable"
                                        Style="display:none; width: 800px; min-height:350px; height: auto; top: 50%; left: 50%; transform: translate(-50%, -50%); background: #d3d3d3; border: 2px solid black; padding: 10px; box-shadow: 2px 2px 10px rgba(0, 0, 0, 0.5); z-index: 2000; border-radius: 5px; position: fixed;">

                                        <!-- Panel Header - Added unselectable="on" and id for easier selection -->
                                        <div class="panel-heading" style="background: navy; color: white; padding: 5px; font-weight: bold; cursor: move;"
                                            unselectable="on" onselectstart="return false;">
                                            Duplicate Check Popup
                                        </div>

                                        <!-- Scrollable GridView Container -->
                                        <div class="panel-body" style="max-height: 400px; overflow: auto; border: 1px solid #ccc; padding: 5px;">
                                            <asp:GridView ID="gvDuplicateTransactions" runat="server" CssClass="table table-bordered"
                                                AutoGenerateColumns="False" Width="100%" ShowHeader="True">
                                                <HeaderStyle CssClass="thead-dark" />
                                                <Columns>
                                                    <asp:BoundField DataField="ClientCode" HeaderText="Client Code" />
                                                    <asp:BoundField DataField="ClientName" HeaderText="Client Name" />
                                                    <asp:BoundField DataField="Mobile" HeaderText="Mobile" />
                                                    <asp:BoundField DataField="ARDate" HeaderText="AR Date" DataFormatString="{0:dd-MM-yyyy}" />
                                                    <asp:BoundField DataField="SchemeName" HeaderText="Scheme Name" />
                                                    <asp:BoundField DataField="SchemeCode" HeaderText="Scheme Code" />
                                                    <asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="{0:N2}" />
                                                    <asp:BoundField DataField="ARNumber" HeaderText="AR Number" />
                                                    <asp:BoundField DataField="DTNumber" HeaderText="DT Number" />
                                                    <asp:BoundField DataField="ChequeNumber" HeaderText="Cheque Number" />
                                                </Columns>
                                            </asp:GridView>
                                        </div>

                                        <!-- Buttons -->
                                        <div style="text-align: center; margin-top: 10px;">
                                            <asp:Button ID="btnContinue" runat="server" Text="Continue" OnClick="btnContinue_Click" />
                                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="cancelPopup(); return false;"/>
                                        </div>

                                        <!-- Hidden Field to Track Decision -->
                                        <asp:HiddenField ID="hfContinueTransaction" runat="server" Value="0" />
                                    </asp:Panel>

                                    <script>
                                        function cancelPopup() {
                                            // Set the hidden field value to '0'
                                            document.getElementById('<%= hfContinueTransaction.ClientID %>').value = '0';
    
    // Hide the popup
                                            document.getElementById('<%= popupDuplicate.ClientID %>').style.display = 'none';
                                        }

                                    </script>


                                    <script type="text/javascript">
                                        function showDPopup() {
                                            document.getElementById('<%= popupDuplicate.ClientID %>').style.display = 'block';
                                        }

                                        function hideDPopup() {
                                            document.getElementById('<%= popupDuplicate.ClientID %>').style.display = 'none';
                                        }
                                    </script>

                                </ContentTemplate>
                            </asp:UpdatePanel>
