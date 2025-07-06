<div class="col-md-4">
    <button type="button" class="btn btn-outline-primary btn-sm" data-bs-toggle="modal"
        data-bs-target="#investorSearchModal">Search Investor</button>
</div>

<script>
    // Add a click event listener to the investorButton
    document.addEventListener('DOMContentLoaded', function () {
        const investorButton = document.getElementById('investorButton');

        if (investorButton) {
            investorButton.addEventListener('click', function () {
                // Set a session storage item to track the click
                sessionStorage.setItem('isViewInvSearchClicked', 'true');
                console.log('Investor button clicked and tracked.');
            });
        }
    });

    // Example: Check if it was clicked during this session
    const wasClicked = sessionStorage.getItem('isViewInvSearchClicked');
    console.log('Was Investor button clicked this session?', wasClicked);


    
</script>



<!-- MODEL: INVESTOR SEARCH-->
<div class="modal fade" id="investorSearchModal" tabindex="-1" aria-labelledby="investorSearchModalLabel"
    aria-hidden="true">
    <div class="modal-dialog modal-lg">
        <div class="modal-content bg-white">
            <div class="modal-header">
                <h5 class="modal-title" id="investorSearchModalLabel">Investor Search</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <!-- inputs -->
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
                        <asp:DropDownList runat="server" ID="BranchSearch_Dropdown"
                            CssClass="form-control form-control-sm branch-search">
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
                        <asp:DropDownList runat="server" ID="CitySearch_Dropdown"
                            CssClass="form-control form-control-sm city-search">
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
                        <input type="text" id="panNoSearch" class="form-control form-control-sm"
                            oninput="validatePanInput(this)" maxlength="10" />
                    </div>
                    <div class="col-3">
                        <label class="form-label small">Phone</label>
                        <input type="text" id="phoneSearch" class="form-control form-control-sm"
                            oninput="validateMobileInput(this)" />
                    </div>
                    <div class="col-3">
                        <label class="form-label small">Mobile</label>
                        <input type="text" id="mobileSearch" class="form-control form-control-sm"
                            oninput="validateMobileInput(this)" />
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
                        <input type="text" id="emailSearch" class="form-control form-control-sm"
                            oninput="validateEmailInput(this)" />
                    </div>
                    <div class="col-3">
                        <label class="form-label small">DOB (dd/mm/yyyy)</label>
                        <div class="input-group date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                            <input type="text" id="dobSearch" class="form-control form-control-sm"
                                placeholder="dd/mm/yyyy" />
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

                <!-- buttons -->
                <div class="col-3">
                    <div class="mt-4 pt-1">
                        <button id="btnInvestorSearch" type="button" class="btn btn-primary btn-sm"><i
                                class="fa fa-search me-1"></i>Search</button>
                        <button id="btnInvestorReset" type="button"
                            class="btn btn-outline-primary btn-sm ms-2">Reset</button>
                        <button type="button" class="btn btn-outline-primary btn-sm ms-2"
                            data-bs-dismiss="modal">Exit</button>
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

                let fullText = $(this).find('td:nth-child(1)').text().trim();
                let investorName = fullText.split('(')[0].trim();
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
                $("[id*='txtAddressADD']").val(address1);
                $("[id*='txtAddressADD2']").val(address2);
                $("[id*='invcode']").val(invCode); // invcode
                $("[id*='ddlCityADD']").val(cityId);
                $("[id*='ddlStateADD']").val(stateId);
                $("[id*='txtPinADD']").val(pincode);
                $("[id*='txtMobileADD']").val(mobile);
                $("[id*='txtPanADD']").val(pan);
                $("[id*='TextAadhar']").val(aadharNo);
                $("[id*='txtDOBADD']").val(dob);
                //$("[id*='invcode']").val(invCode);
                var cleanedName = investorName.replace(/\s*\(Client\)$/, ""); // Removes " (Client)" from the end
                $("[id*='accountHolder']").val(cleanedName);
                $("[id*='holderCode']").val(ahClientCode);

                $("[id*='businessCode']").val(BusinessCodeInMod);
                $("[id*='RMNAMEP']").val(RmNameInMod);
                $("[id*='branch']").val(BRANCHCODE);

                $(this).closest('.modal').find("button[data-bs-dismiss='modal']").trigger('click');

                // Open the Address Updation modal
                //$("#addresModal").modal("show");
                showPopup3();


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