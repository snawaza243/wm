<%@ Page Title="MF Punching Interface" Language="C#" MasterPageFile="~/vmSite.Master" AutoEventWireup="true" CodeBehind="Mf_Punching.aspx.cs" Inherits="WM.Masters.Mf_Punching" EnableViewState="true" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link rel="shortcut icon" href="../assets/images/favicon.png" />
    <link rel="stylesheet" href="../assets/vendors/mdi/css/materialdesignicons.min.css">
    <link rel="stylesheet" href="../assets/vendors/ti-icons/css/themify-icons.css">
    <link rel="stylesheet" href="../assets/vendors/css/vendor.bundle.base.css">
    <link rel="stylesheet" href="../assets/vendors/font-awesome/css/font-awesome.min.css">
    <link rel="stylesheet" href="../assets/vendors/bootstrap-datepicker/bootstrap-datepicker.min.css">
    <link rel="stylesheet" href="../assets/css/style.css">


    <%-- RUPEES SYMBOLE LOADER : Loader Overlay --%>
    <div id="updateProgress" class="loading-overlay">
        <div class="spinner-container">
            <div class="spinner-border text-dark" role="status">
                <span class="rupee-sign">₹</span>
            </div>
        </div>
    </div>
    <style>
        /* Loader Overlay with Relaxing Yellow Background */
        .loading-overlay {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(255, 223, 128, 0.4); /* Soft warm yellow */
            display: flex;
            justify-content: center;
            align-items: center;
            z-index: 1050;
            display: none; /* Initially hidden */
        }

        /* Centered Spinner Container */
        .spinner-container {
            position: relative;
            width: 80px;
            height: 80px;
        }

        /* Spinner with a Larger Size */
        .spinner-border {
            width: 80px;
            height: 80px;
            border-width: 5px;
            color: #FFC107; /* Bootstrap warning yellow */
            animation: spin 1s linear infinite;
        }

        /* Rupee Sign Positioned in the Center */
        .rupee-sign {
            position: absolute;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            font-size: 16px;
            font-weight: bold;
            color: #333; /* Dark color for contrast */
            font-family: Arial, sans-serif;
        }

        @keyframes spin {
            0% {
                transform: rotate(0deg);
            }

            100% {
                transform: rotate(360deg);
            }
        }
    </style>
    <%-- CLOSE RUPEES SYMBOLE LOADER --%>
    
    
    <div class="page-header">
        <h3 class="page-title">MF Trans Punching
        </h3>
    </div>

    <div class="row">
        <div class="col-md-12 grid-margin stretch-card">
            <div class="card">
                <div class="card-body">
                    <!-- Tab Navigation -->
                    <ul class="nav nav-tabs wmTabs" id="wmTabs" role="tablist">
                        <li class="nav-item" role="presentation">
                            <a id="add_tab" class="nav-link active" href="#add_item"
                                role="tab" data-bs-toggle="tab" aria-selected="true"
                                aria-controls="add_item">Add</a>
                        </li>
                        <li class="nav-item" role="presentation">
                            <a id="view_modify_tab" class="nav-link" href="#view_modify"
                                role="tab" data-bs-toggle="tab" aria-selected="false"
                                aria-controls="view_modify">View/Modify</a>
                        </li>
                    </ul>
                   
                 

                    <!-- Tab Content -->
                    <div class="tab-content wmTabsContent" id="wmTabsContent">
                        <div class="tab-pane fade show active" id="add_item"
                            role="tabpanel" aria-labelledby="add_tab">

                            <%-- TAB 1: ADD --%>
                            <asp:UpdatePanel ID="UpdatePanelFillByDT" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="row g-3">
                                        <div class="col-md-6">
                                            <h4 class="card-title">Investor/Scheme Details</h4>
                                            <div> 
                                                <%-- DT, TRD, INV, BSS, INV SRC MDL, AH, MDL ADD, AR, RM, BR, REG, AMC, SCH,  --%>
                                                <div class="row g-3">                                                
                                                    <%-- UP: DT BOX & BTN --%>
                                                    <asp:UpdatePanel ID="UpdatePanelDTNumber" runat="server" UpdateMode="Conditional">
                                                        <ContentTemplate>
                                                            <div>
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

                                                    <%-- TRAN DATE --%>
                                                    <div class="col-md-6">
                                                        <label class="form-label">
                                                            Transaction Date
                                                        </label>
                                                        <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                            <asp:TextBox ID="transactionDate" runat="server" CssClass="form-control date-input"
                                                                placeholder="dd/mm/yyyy" ReadOnly="true" />
                                                            <div class="input-group-addon">
                                                                <span class="glyphicon glyphicon-th"></span>
                                                            </div>

                                                        </div>
                                                    </div>

                                                    <%-- INV CODE --%>
                                                    <div class="col-md-6">
                                                        <label class="form-label">Inv Code</label>
                                                        <asp:TextBox ID="invcode" runat="server" CssClass="form-control bg-light" />
                                                    </div>

                                                    <%-- BUSIESS CODE --%>
                                                    <div class="col-md-6">
                                                        <label for="businessCode" class="form-label">Business Code</label>
                                                        <asp:TextBox ID="businessCode" runat="server" CssClass="form-control" />
                                                    </div>

                                                    <%-- BTN: INVESTOR SEARCH BUTTON --%>
                                                    <div class="col-md-4">
                                                        <button type="button" style="margin-top:30px"  id="inv_find1" class="btn btn-outline-primary btn-sm" data-bs-toggle="modal" data-bs-target="#investorSearchModal">Search Investor</button>
                                                    </div>

                                                    
                                                    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>

                                                  
                                                    
                                                    <%-- MODEL: ADDRESS UPDATE --%>
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
                                                                    <asp:Button ID="btnAddressUpdate" runat="server" CssClass="btn btn-primary" Text="Save changes" OnClick="btnPanelAddressUpdate_Click" data-bs-dismiss="modal" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
                                                    <script src="https://stackpath.bootstrapcdn.com/bootstrap/5.1.3/js/bootstrap.bundle.min.js"></script>
                                                    <script type="text/javascript">
                                                        function openAddressModal() {
                                                            var myModal = new bootstrap.Modal(document.getElementById('addresModal'));
                                                            myModal.show();
                                                        }
                                                    </script>

                                                    <%-- TB: ACCOUNT HOLDER --%>
                                                    <div class="col-md-6">
                                                        <label for="accountHolder" class="form-label">Account Holder</label>
                                                        <asp:TextBox ID="accountHolder" runat="server" CssClass="form-control" Style="font-weight:bold;" />
                                                    </div>

                                                    <%-- TB: ACCOUNT HOLDER CODE --%>
                                                    <div class="col-md-6">
                                                        <label for="holderCode" class="form-label">A/C Holder Code</label>
                                                        <asp:TextBox ID="holderCode" runat="server" CssClass="form-control" Enabled="false" />
                                                    </div>

                                                    <%-- TB:RM --%>
                                                    <div class="col-md-6">
                                                        <label for="RMNAMEP" class="form-label">RM</label>
                                                        <asp:TextBox ID="RMNAMEP" runat="server" CssClass="form-control" Enabled="false" />
                                                    </div>

                                                    <%-- TB: DDL BRANCH --%>
                                                    <div class="col-md-6">
                                                        <label for="branch" class="form-label">Branch</label>
                                                        <asp:DropDownList ID="branch" runat="server" CssClass="form-select">
                                                            <asp:ListItem Text="--Select Branch--" Value="" />
                                                        </asp:DropDownList>
                                                    </div>

                                                    <%-- RDB: REGULAR/NFO --%>
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

                                                    <%-- DDL: AMC --%>
                                                    <div class="col-md-6">
                                                        <label for="amc" class="form-label">AMC</label>
                                                        <asp:DropDownList ID="amc" runat="server" CssClass="form-select">
                                                            <asp:ListItem Text="--Select AMC--" Value="" />
                                                            <asp:ListItem Text="AMC 1" Value="amc1" />
                                                            <asp:ListItem Text="AMC 2" Value="amc2" />
                                                        </asp:DropDownList>
                                                    </div>

                                                    <%-- BTN: SHEME SEARCH --%>
                                                    <div class="col-md-12">
                                                        <label class="form-label">Scheme</label>
                                                        <div class="input-group">
                                                            <asp:TextBox ID="scheme" runat="server" CssClass="form-control" placeholder="Scheme" />
                                                            <asp:Button ID="btnSearchScheme" runat="server" CssClass="btn btn-outline-primary" Text="Search" OnClick="btnSearchSchemeADD" Enabled="true" />
                                                            <asp:HiddenField ID="hfSearchClicked" runat="server" Value="0" />
                                                        </div>
                                                    </div>

                                                    <%-- JS, JQUERY: DRAGABLER PANNEL --%>
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

                                                    <%-- PNL: SEARCH SCHEME --%>
                                                    <asp:Panel ID="SchemeSearchPanel" runat="server" CssClass="panel panel-default draggable"
                                                        Style="display: none; position: fixed; top: 20%; left: 35%; width: 60%; max-height: 400px; overflow: auto; background-color: white; border: 1px solid #ccc; z-index: 5000;">

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

                                                <%-- JS: showError, clearError, allowNumber, validPan, validPin, validMobile,   --%>
                                                <%-- JS: validAadhar, validAmount, validChequeInput,  handleBlur, valiExpenPer, calcExpAmt,  --%>
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

                                                <%-- STYLE: ERROR MSG --%>
                                                <style>
                                                    .error-message {
                                                        display: block;
                                                        margin-top: 2px;
                                                        font-size: 12px; /* Small and compact */
                                                        color: red;
                                                    }
                                                </style>

                                                <%-- TB: Switch/STP --%>
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
                                                <%-- APP NO, FOLIO, AMT, AMT CLN JS, PMT METH AND DIFF FIELDS, DDL BANK, EXP PER, EXP RUP, MATURITY CHK,  --%>
                                                <%-- BTN SCHM SRCH, TRAN TYPE, SWITH/STP , SRCH SHM, SRCH SCHM PNL, --%>
                                                <div class="row g-3 ">
                                                     <%-- TB: APPLICATION NO --%>
                                                    <div class="col-md-4">
                                                        <label for="applicationNo" class="form-label">Application No</label>
                                                        <asp:TextBox ID="applicationNo" runat="server" oninput="validateChequeInput(this)" onblur="handleBlur(this)" CssClass="form-control"></asp:TextBox>
                                                    </div>

                                                    <%-- TB: FOLIO NO  --%>
                                                    <div class="col-md-4">
                                                        <label for="folioNo" class="form-label">Folio No</label>
                                                        <asp:TextBox ID="folioNoo" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>

                                                    <%-- TB: AMOUNT --%>
                                                    <div class="col-md-4">
                                                        <label for="amount" class="form-label">Amount</label>
                                                        <asp:TextBox ID="amountt" runat="server" oninput="validateAmountInput(this)" onblur="handleBlur(this)" CssClass="form-control"></asp:TextBox>
                                                    </div>

                                                    <%-- JS: AMOUNT CALCULATION  --%>
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

                                                    <%-- RB: PAYMENT METHOD --%>
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

                                                    <!-- TB: CHEQUE NO. AND CHEQUE DATE -->
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

                                                    <!-- Draft Details & Date -->
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

                                                    <!-- RTGS Details & Date -->
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

                                                    <!-- NEFT/Fund Transfer Details & Date -->
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

                                                    <!-- ECS Details & Date -->
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

                                                    <!-- Cash Payment Details & Date  -->
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

                                                    <!-- Others Payment Details & Date -->
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

                                                    <%-- DDL: BANK NAME --%>
                                                    <div>
                                                        <asp:Label ID="lblBankName" runat="server" CssClass="form-label" AssociatedControlID="ddlBankName" Text="Bank Name" />
                                                        <asp:DropDownList ID="ddlBankName" runat="server" CssClass="form-select">
                                                            <asp:ListItem Value="" Text="--Select Bank--" />
                                                            <asp:ListItem Value="bank1" Text="Bank 1" />
                                                            <asp:ListItem Value="bank2" Text="Bank 2" />
                                                        </asp:DropDownList>
                                                    </div>

                                                    <%-- TB: EXPRENCE PER --%>
                                                    <div class="col-md-6">
                                                        <asp:Label ID="Label2" runat="server" CssClass="form-label" AssociatedControlID="txtExpensesPercent" Text="Expenses%" />
                                                        <asp:TextBox ID="txtExpensesPercent" oninput="validateExpensesPercent(this)" runat="server" CssClass="form-control" Enabled="false" />
                                                    </div>

                                                    <%-- TB: EXPENSES RB --%>
                                                    <div class="col-md-6">
                                                        <asp:Label ID="Label3" runat="server" CssClass="form-label" AssociatedControlID="txtExpensesRs" Text="Expenses (Rs.)" />
                                                        <asp:TextBox ID="txtExpensesRs" runat="server" CssClass="form-control" oninput="allowOnlyNumeric(this)" Enabled="false" />
                                                    </div>

                                                    <%-- CHK: MATURITY --%>
                                                    <div>
                                                        <div class="form-label">
                                                            <asp:CheckBox ID="chkAutoSwitchOnMaturity" runat="server" />
                                                            <asp:Label ID="Label4" runat="server" CssClass="form-check-label" AssociatedControlID="chkAutoSwitchOnMaturity" Text="Auto Switch On Maturity" />
                                                        </div>
                                                    </div>

                                                    <%-- BTN: SEARCH SCHEME --%>
                                                    <div>
                                                        <div class="input-group">
                                                            <asp:TextBox ID="txtSearchSchemeDetails" runat="server" CssClass="form-control" placeholder="Enter details" />
                                                            <asp:Button ID="btnSearchSchemeDetails" runat="server" CssClass="btn btn-outline-primary" Text="Search" OnClick="btnSearchSchemeDetails_Click" />
                                                        </div>
                                                    </div>

                                                    <%-- ADD DDL: TRAN TYPE --%>
                                                    <div class="col-md-6">
                                                        <label for="transactionType" class="form-label">Transaction Type</label>
                                                        <asp:DropDownList ID="transactionType" runat="server" CssClass="form-select" Required="True">
                                                            <asp:ListItem Text="PURCHASE" Value="PURCHASE" />
                                                            <asp:ListItem Text="SWITCH IN" Value="SWITCH IN" />
                                                        </asp:DropDownList>
                                                    </div>

                                                    <br />
                                                    <br />

                                                    <%-- TB: orm Switch / STP Scheme, HIDDEN FIELDS,  BTN: SEARCH SCHEME, --%>
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

                                                    <%-- PNL: SEARCH SCHEME --%>
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

                                                    <%-- JS: SHOW/HIDE SCHEME PANEL: SchemeDetailsPanel --%>
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

                                            <%-- DDL: SIP,STP,REGULAR  & DDL: INSTALLMENT TYPE--%>
                                            <div class="row align-items-center">

                                                <%-- DDL: SIP/STP OR REGULAR --%>
                                                <div class="col-md-4">
                                                    <asp:Label ID="lblSIPSTP" runat="server" CssClass="form-label" AssociatedControlID="ddlSipStp" Text="SIP/STP" />
                                                    <asp:DropDownList ID="ddlSipStp" runat="server" CssClass="form-select">
                                                        <asp:ListItem Value="REGULAR" Text="REGULAR" />
                                                        <asp:ListItem Value="SIP" Text="SIP" />
                                                        <asp:ListItem Value="STP" Text="STP" />
                                                    </asp:DropDownList>
                                                </div>
                                                <asp:HiddenField ID="hdnSipStpValue" runat="server" />

                                                <%-- DDL: Installment Type --%>
                                                <div class="col-md-4" id="installmentTypeContainer" style="display: none;">
                                                    <asp:Label ID="iNSTYPEL" runat="server" CssClass="form-label" AssociatedControlID="iNSTYPE" Text="Installment Type" />
                                                    <asp:DropDownList ID="iNSTYPE" runat="server" CssClass="form-select">
                                                        <asp:ListItem Value="NORMAL" Text="Normal" />
                                                        <asp:ListItem Value="MICRO" Text="Micro" />
                                                    </asp:DropDownList>
                                                </div>
                                            </div>

                                            <%-- RDB: FRESH, RENEWAL & DDL: SIP TYPE & TB: SIP AMOUNT --%>
                                            <div class="row align-items-center">
                                                <%-- RDB: FRESH/RENEWAL --%>
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

                                                <%-- DDL: SIP TYPE --%>
                                                <div class="col-md-4" id="sipTypeContainer" style="display: none;">
                                                    <asp:Label ID="siptypeL" runat="server" CssClass="form-label" AssociatedControlID="siptype" Text="SIP TYPE" />
                                                    <asp:DropDownList ID="siptype" runat="server" CssClass="form-select">
                                                        <asp:ListItem Value="N" Text="NORMAL" />
                                                        <asp:ListItem Value="Y" Text="MICROSIP" />
                                                    </asp:DropDownList>
                                                </div>

                                                <%-- TB: SIP AMOUNT --%>
                                                <div class="col-md-4" id="sipAmountContainer" style="display: none;">
                                                    <asp:Label ID="sipamountL" runat="server" CssClass="form-label" AssociatedControlID="sipamount" Text="Sip Amount" />
                                                    <asp:TextBox ID="sipamount" runat="server" CssClass="form-control" oninput="validateAmountInput(this)" onblur="handleBlur(this)" />
                                                </div>
                                            </div>
                                        </div>

                                        <%-- DDL: FREQUENCY --%>
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

                                        <%-- TB: INSTALLMENTS NOS --%>
                                        <div class="col-md-4">
                                            <asp:Label ID="lblInstallmentsNos" runat="server" CssClass="form-label" AssociatedControlID="txtInstallmentsNos" Text="Installments Nos" />
                                            <asp:TextBox ID="txtInstallmentsNos" runat="server" CssClass="form-control" oninput="calculateSIPEndDate()" onkeypress="allowOnlyNumeric(this)" MaxLength="4" />
                                        </div>

                                        <%-- TB: SIP START DATE --%>
                                        <div class="col-md-6">
                                            <asp:Label ID="lblSIPStartDate" runat="server" CssClass="form-label" AssociatedControlID="txtSIPStartDate" Text="SIP Start Date" />
                                            <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                <asp:TextBox ID="txtSIPStartDate" runat="server" CssClass="form-control date-input" placeholder="dd/mm/yyyy" onblur="calculateSIPEndDate()" />
                                                <div class="input-group-addon">
                                                    <span class="glyphicon glyphicon-th"></span>
                                                </div>
                                            </div>
                                        </div>

                                        <%-- JS: calculateSIPEndDate --%>
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

                                        <%-- TB: SIP End Date --%>
                                        <div class="col-md-6">
                                            <asp:Label ID="lblSIPEndDate" runat="server" CssClass="form-label" AssociatedControlID="txtSIPEndDate" Text="SIP End Date" />
                                            <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                <asp:TextBox ID="txtSIPEndDate" runat="server" CssClass="form-control date-input" placeholder="dd/mm/yyyy" />
                                                <div class="input-group-addon">
                                                    <span class="glyphicon glyphicon-th"></span>
                                                </div>
                                            </div>
                                        </div>

                                        <%-- CHK: COB, SWP, FREEDOM SIP, AND 99 YEARS OR MORE CASE--%>
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

                                        <h4 class="card-title">PAN details</h4>

                                        <%-- TB: PAN No --%>
                                        <div class="col-md-4">
                                            <label for="pan" class="form-label">
                                                PAN No
                                            </label>
                                            <asp:TextBox ID="pann" runat="server" oninput="validatePanInput(this)" onblur="handleBlur(this)" MaxLength="10" CssClass="form-control"></asp:TextBox>
                                        </div>

                                        <%-- BTN: ADD, RESET, LEAR SRCH, BTN CHNG INV, EXIT --%>
                                        <%-- ADD FOLIO VALIDATION JS --%>
                                        <div class="col-md-8">
                                            <div class="d-flex align-items-md-center flex-wrap gap-3">
                                                <asp:Button ID="btnAdd" runat="server" CssClass="btn btn-primary" Text="Add" 
                                                    OnClick="btnAddClick" 
                                                    OnClientClick="return validateFolioOnAdd();"/>
                                                <%-- JS: validateFolioOnAdd --%>
                                                <script>
                                                     function validateFolioOnAdd() {
                                                        const transactionType = document.getElementById('<%= transactionType.ClientID %>').value;
                                                        const sipStpType = document.getElementById('<%= ddlSipStp.ClientID %>').value;
                                                        const folio1 = document.getElementById('<%= folioNoo.ClientID %>').value.trim();
                                                        const folio3 = document.getElementById('<%= formSwitchFolio.ClientID %>').value.trim();
                                                        // Case I: SWITCH
                                                        if (transactionType === "SWITCH IN" && sipStpType === "REGULAR") {
                                                            if (!folio1 || !folio3) {
                                                                alert("Folio 1 and Folio 2 are required for SWITCH case.");
                                                                return false;
                                                            }
                                                            if (folio1 !== folio3) {
                                                                alert("Folio 1 and Folio 2 must be the same for SWITCH.");
                                                                return false;
                                                            }
                                                        }
                                                        // Case II: STP
                                                        else if (transactionType === "PURCHASE" && sipStpType === "STP") {
                                                            if (!folio1 || !folio3) {
                                                                alert("Folio 1, and 2 are required for STP.");
                                                                return false;
                                                            }
                                                            if (folio1 !== folio3) {
                                                                alert("Folio 1 and Folio 2 must be the same for STP.");
                                                                return false;
                                                            }
                                                        }
                                                        // Case III: SIP
                                                        else if (transactionType === "PURCHASE" && sipStpType === "SIP") {
                                                            // Folios are optional, but if filled must be valid (can optionally validate format)
                                                            // So nothing to do here unless specific format checks needed
                                                        }
                                                        // Case IV: Additional
                                                        else if (transactionType === "PURCHASE" && sipStpType === "REGULAR") {
                                                            
                                                        }
                                                        // If all validations pass
                                                        //return confirm("Folio. Are you sure!");
                                                        return true;
                                                    }
                                                </script>

                                                <asp:Button ID="btnPrintAR" runat="server" CssClass="btn btn-outline-primary" Text="Print AR" OnClick="btn_PrintAR" Enabled="false" />
                                                <asp:Button ID="btnReset" runat="server" CssClass="btn btn-outline-primary" Text="Reset" OnClick="btnResetClick" />
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

                                        <%-- JS: REMOVING COMMAS FROM ANY ALERTBOX --%>
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
                                    <asp:HiddenField ID="hdnPopupVisible1" runat="server" />

                                    <asp:Panel ID="Panel2" runat="server" CssClass="panel panel-default draggable"
                                        Style="display: none; position: fixed; top: 20%; left: 35%; width: 30%; max-height: 400px; overflow: auto; background-color: white; border: 1px solid #ccc; z-index: 1000; padding: 15px;">
                                        <div class="panel-heading" style="cursor: move; display: flex; justify-content: space-between; align-items: center;">
                                            <h4 class="panel-title">Investor Address Updation</h4>
                                            <button type="button" class="close" onclick="hidePopup3()" style="font-size: 18px;">&times;</button>
                                        </div>
                                        <div class="panel-body">
                                            <h5>Investor Details 1</h5>

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

                                    <%-- PNL: Duplicate Check Popup --%>
                                    <asp:Panel ID="popupDuplicate" runat="server" CssClass="panel panel-default draggable"
                                        Style="display:none; width: 700px; height: auto; top: 50%; left: 50%; transform: translate(-50%, -50%); background: #d3d3d3; border: 2px solid black; padding: 10px; box-shadow: 2px 2px 10px rgba(0, 0, 0, 0.5); z-index: 5000; border-radius: 5px; position: fixed;">

                                        <!-- Panel Header -->
                                        <div class="panel-heading" style="background: navy; color: white; padding: 5px; font-weight: bold; cursor: move;">
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
                                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="cancelPopup(); return false;" />
                                        </div>

                                        <!-- Hidden Field to Track Decision -->
                                        <asp:HiddenField ID="hfContinueTransaction" runat="server" Value="0" />
                                    </asp:Panel>

                                    <%-- JS: CANCEL POPUP popupDuplicate AND SET hfContinueTransaction = 0 --%>
                                    <script>
                                        function cancelPopup() {
                                            // Set the hidden field value to '0'
                                            document.getElementById('<%= hfContinueTransaction.ClientID %>').value = '0';
    
                                            // Hide the popup
                                            document.getElementById('<%= popupDuplicate.ClientID %>').style.display = 'none';
                                        }
                                    </script>

                                    <%-- JS: SHOW/HIDE POPUP popupDuplicate --%>
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

                            <%-- TAB 2: VIEW/MODIFY --%>
                            <div class="tab-pane fade" id="view_modify"
                                role="tabpanel" aria-labelledby="view_modify_tab">
                                <div class="row g-3">
                                    <div>
                                        <asp:UpdatePanel ID="UpdatePanelSearchAR" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <h4 class="card-title">Search AR</h4>

                                                <div class="row g-3 mb-3">

                                                    <%-- TB: FROM DATE --%>
                                                    <div class="col-md-2">
                                                        <asp:Label ID="lblFromDate" runat="server" CssClass="form-label" AssociatedControlID="fromDate" Text="From Date" />
                                                        <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                            <asp:TextBox ID="fromDate" runat="server" CssClass="form-control date-input" Placeholder="dd/mm/yyyy" />
                                                            <div class="input-group-addon">
                                                                <span class="glyphicon glyphicon-th"></span>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <%-- TB: TO TR DATE --%>
                                                    <div class="col-md-2">
                                                        <asp:Label ID="lblToDate" runat="server" CssClass="form-label" AssociatedControlID="toDate" Text="To Date" />
                                                        <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                            <asp:TextBox ID="toDate" runat="server" CssClass="form-control date-input" Placeholder="dd/mm/yyyy" />
                                                            <div class="input-group-addon">
                                                                <span class="glyphicon glyphicon-th"></span>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <%-- DDL: ORDERED BY --%>
                                                    <div class="col-md-2">
                                                        <asp:Label ID="lblOrderBy" runat="server" CssClass="form-label" AssociatedControlID="orderBy" Text="Order By" />
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

                                                    <%-- RDB: ORD BY --%>
                                                    <div class="col-md-3">
                                                        <asp:Label ID="lblAscendingDescending" runat="server" CssClass="form-label" AssociatedControlID="ascending" Text="Order" />
                                                        <div class="d-flex align-items-center gap-3">
                                                            <div class="form-label">
                                                                <asp:RadioButton ID="ascending" runat="server" GroupName="order" Text="Ascending" />
                                                            </div>
                                                            <div class="form-label">
                                                                <asp:RadioButton ID="descending" runat="server" GroupName="order" Text="Descending" />
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <%-- TB: PAN NO --%>
                                                    <div class="col-md-3">
                                                        <asp:Label ID="lblPanNo" runat="server" CssClass="form-label" AssociatedControlID="panNo" Text="PAN No." />
                                                        <asp:TextBox ID="TextBox15" runat="server" CssClass="form-control" Placeholder="PAN No." onblur="handleBlur(this)" oninput="validatePanInput(this)" MaxLength="10" />
                                                    </div>

                                                    <%-- TB: TR NO --%>
                                                    <div class="col-md-2">
                                                        <asp:Label ID="lblTrNo" runat="server" CssClass="form-label" AssociatedControlID="trNo" Text="TR No." />
                                                        <asp:TextBox ID="trNo" runat="server" CssClass="form-control" Placeholder="TR No." />
                                                    </div>

                                                    <div class="col-md-2">
                                                        <asp:Label ID="lblUniqueClientCode" runat="server" CssClass="form-label" AssociatedControlID="uniqueClientCode" Text="Unique Client Code" />
                                                        <asp:TextBox ID="uniqueClientCode" runat="server" CssClass="form-control" Placeholder="Unique Client Code" />
                                                    </div>

                                                    <div class="col-md-2">
                                                        <asp:Label ID="lblAnaCode" runat="server" CssClass="form-label" AssociatedControlID="anaCode" Text="ANA Code" />
                                                        <asp:TextBox ID="anaCode" runat="server" CssClass="form-control" Placeholder="ANA Code" />
                                                    </div>

                                                    <div class="col-md-3">
                                                        <asp:Label ID="lblChequeNo" runat="server" CssClass="form-label" AssociatedControlID="chequeNo" Text="Cheque No" />
                                                        <asp:TextBox ID="chequeNo" runat="server" CssClass="form-control" Placeholder="Cheque No" />
                                                    </div>

                                                    <div class="col-md-3">
                                                        <asp:Label ID="lblAppNo" runat="server" CssClass="form-label" AssociatedControlID="appNo" Text="App No" />
                                                        <asp:TextBox ID="appNo" runat="server" CssClass="form-control" Placeholder="App No" />
                                                    </div>

                                                    <asp:UpdatePanel ID="UpdatePanelButton" runat="server" UpdateMode="Conditional">
                                                        <ContentTemplate>
                                                            <asp:Button ID="btnSearchAR" runat="server" CssClass="btn btn-primary"
                                                                Text="View" OnClick="btnSearch_Click" />

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
                                        <asp:Label ID="lblSearchInfo" runat="server" CssClass="form-label" Text="Searching by Investor Name/PAN/AR DATE etc" />
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
                                                            <asp:Button ID="Button1" runat="server"
                                                                Text="Select"
                                                                CommandName="SelectRow"
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
                                                            <asp:HiddenField ID="HiddenField2" runat="server" Value='<%# Eval("country_id") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Investor">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblInvestorName" runat="server" Text='<%# Eval("INVESTOR_NAME") %>' />
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
                                                            <asp:Label ID="lblDOB" runat="server" Text='<%# Eval("DOB" , "{0:dd/MM/yyyy}") %>' />
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
                                                            <asp:Label ID="lblTranDate" runat="server" Text='<%# Eval("TR_DATE", "{0:dd/MM/yyyy}") %>' />
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
                                                            <asp:Label ID="lblChqDt" runat="server" Text='<%# Eval("CHEQUE_DATE", "{0:dd/MM/yyyy}") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="drpdt">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lbldrpDt" runat="server" Text='<%# Eval("CHEQUE_DATE", "{0:dd/MM/yyyy}") %>' />
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
                                                            <asp:Label ID="lblInvestorType" runat="server" Text='<%# Eval("INVESTOR_TYPE") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>


                                                    <asp:TemplateField HeaderText="Target">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblTargetSwitchScheme" runat="server" Text='<%# Eval("TARGET_SWITCH_SCHEME") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>


                                                    <asp:TemplateField HeaderText="Target Name">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblTargetName" runat="server" Text='<%# Eval("GOAL") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>


                                                    <asp:TemplateField HeaderText="Switch">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSwitchScheme" runat="server" Text='<%# Eval("SWITCH_SCHEME") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    

                                                    <asp:TemplateField HeaderText="FREQUENCY">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblFREQUENCY" runat="server" Text='<%# Eval("FREQUENCY") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="NoofInstallment">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblinstallments_no" runat="server" Text='<%# Eval("installments_no") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="SIP_Start_Date">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSIP_Start_Date" runat="server" Text='<%# Eval("sip_start_date") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="SIP_End_Date">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSIP_End_Date" runat="server" Text='<%# Eval("sip_end_date") %>' />
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
                                                            <asp:Label ID="lblSwitchSchemeName" runat="server" Text='<%# Eval("sw_sch_name") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Scheme Code">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSchemeCode" runat="server" Text='<%# Eval("SCH_CODE") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Base Tran Code" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:HiddenField ID="hfBaseTranCode" runat="server" Value='<%# Eval("BASE_TRAN_CODE") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>



                                                    <asp:TemplateField HeaderText="Base Tran Code" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:HiddenField ID="HiddenField1" runat="server" Value='<%# Eval("BASE_TRAN_CODE") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="INV CODE MODIFY" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:HiddenField ID="hfINVCODEMOD" runat="server" Value='<%# Eval("CLIENT_CODE") %>' />
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
                                        <div class="d-flex justify-content-between align-items-center mb-3 mt-3">
                                            <h4 class="card-title mb-0">Tran Details</h4>
                                            <asp:Label ID="lblMfViewLogStatus" runat="server" CssClass="text-danger mt-2 mb-0 border-2" Text="" />
                                        </div>


                                        <div class="row g-3 mb-3">
                                            <div class="col-md-2">
                                                <label for="businessCode" class="form-label">Business Code</label>
                                                <asp:TextBox ID="businessCodeV" runat="server" CssClass="form-control" Placeholder="Business Code" onkeypress="return handleKeyPress(event)"
                                                    onchange="handleChange(event)"
                                                    OnBlur="TxtBusiCodeM_LostFocus"></asp:TextBox>
                                            </div>

                                            <div class="col-md-3">
                                                <label for="TXTbranchName" class="form-label">Branch Name.</label>
                                                <asp:TextBox ID="BranchNameView" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                            </div>

                                            <div class="col-md-2">
                                                <label for="txtClientCode" class="form-label">Client Code</label>
                                                <asp:TextBox ID="txtClientCode" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                            </div>



                                            <div class="col-md-3">
                                                <label for="accountHolder" class="form-label">Account Holder Name</label>
                                                <asp:TextBox ID="accountHolderV" runat="server" CssClass="form-control" Style="font-weight:bold;" Placeholder="Account Holder"></asp:TextBox>
                                            </div>

                                            <div class="col-md-2">
                                                <label for="holderCodeVL" class="form-label">A/C Holder Code</label>
                                                <asp:TextBox ID="holderCodeV" runat="server" CssClass="form-control" ReadOnly="true" />
                                            </div>

                                            <div class="col-md-2 " style="margin-top:30px">

                                                <asp:Button
                                                    ID="btnInvSearch2ForTr"
                                                    runat="server"
                                                    CssClass="d-none"
                                                    OnClick="btnInvSearch2ForTr_Click" />
                                                <asp:HiddenField ID="hdnInvCodeInModify" runat="server" />
                                                <%--<button type="button" style="margin-top:30px" ID="inv_find2" class="btn btn-outline-primary btn-sm " data-bs-toggle="modal" data-bs-target="#investorSearchModal">Search Investor</button>--%>
                                                <button type="button" style="margin-top:30px" ID="inv_find2new" class="btn btn-outline-primary btn-sm d-none"  >Search Investor 2</button>

                                                <asp:Button ID="btnStoreData" runat="server"  Text="Investor Search 2" class="btn btn-outline-primary btn-sm" OnClick="btnStoreData_Click" />


                                                <asp:LinkButton ID="btnRetriveInvSearchData" runat="server" OnClick="btnRetriveInvSearchData_Click" CssClass="reload-button" ToolTip="Reload Data">
                                                    <i class="fa fa-refresh"></i> 
                                                </asp:LinkButton>
                                            
                                                <script type="text/javascript">
                                                    document.getElementById("inv_find2new").addEventListener("click", function () {
                                                        window.open(
                                                            "../Tree/frm_tree_mf?SEARCH_TYPE=mf_view_inv",
                                                            "InvestorSearchPopup",
                                                            "width=1000,height=600,scrollbars=yes,resizable=yes"
                                                        );
                                                    });
                                                </script>

                                            </div>

                                            <div class="col-md-2">
                                                <label for="panNo" class="form-label">PAN NO</label>
                                                <asp:TextBox ID="panNo" runat="server" CssClass="form-control" onblur="handleBlur(this)" oninput="validatePanInput(this)" MaxLength="10" Placeholder="PAN NO"></asp:TextBox>
                                            </div>

                                            <div class="col-md-3">
                                                <label for="amcSelect" class="form-label">AMC</label>
                                                <asp:DropDownList ID="amcSelect" runat="server" CssClass="form-select">
                                                    <asp:ListItem Text="Select" Value=""></asp:ListItem>
                                                    <asp:ListItem Text="AMC 1" Value="amc1"></asp:ListItem>
                                                    <asp:ListItem Text="AMC 2" Value="amc2"></asp:ListItem>

                                                </asp:DropDownList>
                                            </div>

                                            <div class="col-md-3">
                                                <label for="SchemeName" class="form-label">SCHEME</label>
                                                <asp:TextBox ID="SchemeName" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>


                                            <div class="col-md-2">
                                                <label for="transactionDate" class="form-label">Transaction Date</label>
                                                <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                    <asp:TextBox ID="TextBox11" runat="server" CssClass="form-control date-input" Placeholder="dd/mm/yyyy"></asp:TextBox>
                                                    <div class="input-group-addon">
                                                        <span class="glyphicon glyphicon-th"></span>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-md-2">
                                                <label for="transactionType" class="form-label">Transaction Type</label>
                                                <asp:DropDownList ID="DropDownList4" runat="server" CssClass="form-select" OnSelectedIndexChanged="trantypechangeINupdate" AutoPostBack="true">
                                                    <asp:ListItem Text="Select" Value=""></asp:ListItem>
                                                    <asp:ListItem Text="PURCHASE" Value="PURCHASE"></asp:ListItem>
                                                    <asp:ListItem Text="SWITCH IN" Value="SWITCH IN"></asp:ListItem>

                                                </asp:DropDownList>
                                            </div>

                                            <div class="col-md-2">
                                                <label for="bankName" class="form-label">Bank Name</label>
                                                <asp:DropDownList ID="bankName" runat="server" CssClass="form-select">
                                                    <asp:ListItem Text="Select" Value=""></asp:ListItem>
                                                    <asp:ListItem Text="Bank 1" Value="bank1"></asp:ListItem>
                                                    <asp:ListItem Text="Bank 2" Value="bank2"></asp:ListItem>

                                                </asp:DropDownList>
                                            </div>

                                            <div class="col-md-2">
                                                <label for="folio-no" class="form-label">From Switch Folio</label>
                                                <asp:TextBox ID="TextBox12" runat="server" CssClass="form-control" Placeholder="Folio No" Enabled="false"></asp:TextBox>
                                            </div>

                                            <div class="col-md-2">
                                                <label for="folio-no" class="form-label">From Switch Scheme</label>
                                                <asp:TextBox ID="switchSchemeName" runat="server" CssClass="form-control" Placeholder="Folio No" Enabled="false"></asp:TextBox>
                                            </div>

                                            <div class="col-md-2">
                                                <label for="applicationNo" class="form-label">Application No</label>
                                                <asp:TextBox ID="TextBox13" runat="server" CssClass="form-control" Placeholder="Application No"></asp:TextBox>
                                            </div>

                                            <div class="col-md-2">
                                                <label for="amount" class="form-label">Amount</label>
                                                <asp:TextBox ID="TextBox14" runat="server" CssClass="form-control" Placeholder="Amount" oninput="validateAmountInput(this)" onblur="handleBlur(this)"></asp:TextBox>
                                            </div>

                                            <div class="col-md-2">
                                                <label for="sipStp" class="form-label">SIP/STP</label>
                                                <asp:DropDownList ID="sipStp" runat="server" CssClass="form-select" OnSelectedIndexChanged="ddlsipstpInupdate" AutoPostBack="true">
                                                    <asp:ListItem Text="" Value=""></asp:ListItem>
                                                    <asp:ListItem Text="REGULAR" Value="REGULAR"></asp:ListItem>
                                                    <asp:ListItem Text="SIP" Value="SIP"></asp:ListItem>
                                                    <asp:ListItem Text="STP" Value="STP"></asp:ListItem>

                                                </asp:DropDownList>
                                            </div>



                                            <div class="col-md-2">
                                                <label for="dropDate" class="form-label">Drop Date</label>
                                                <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                    <asp:TextBox ID="dropDat" runat="server" CssClass="form-control date-input" Placeholder="dd/mm/yyyy"></asp:TextBox>
                                                    <div class="input-group-addon">
                                                        <span class="glyphicon glyphicon-th"></span>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-md-2">
                                                <label for="frequency" class="form-label">Frequency</label>
                                                <asp:DropDownList ID="frequency" runat="server" CssClass="form-select" oninput="calculateSIPEndDateForUpdate()">
                                                    <asp:ListItem Value="301" Text="Select Frequency" />
                                                    <asp:ListItem Value="208" Text="Daily" />
                                                    <asp:ListItem Value="173" Text="Weekly" />
                                                    <asp:ListItem Value="174" Text="Fortnightly" />
                                                    <asp:ListItem Value="175" Text="Monthly" />
                                                    <asp:ListItem Value="176" Text="Quarterly" />
                                                    <asp:ListItem Value="301" Text="Yearly" />
                                                </asp:DropDownList>
                                            </div>

                                            <div class="col-md-3">
                                                <label for="installmentsNos" class="form-label">No of Installments</label>
                                                <asp:TextBox ID="installmentsNos" runat="server" CssClass="form-control" Placeholder="No of Installments" onblur="handleBlur(this)" oninput="calculateSIPEndDateForUpdate()" onkeypress="allowOnlyNumeric(this)"></asp:TextBox>
                                            </div>

                                            <div class="col-md-2">
                                                <div class="form-label">
                                                    <asp:CheckBox ID="cobCase" runat="server" />
                                                    <label class="form-check-label" for="cobCase">COB Case</label>
                                                </div>
                                            </div>



                                            <div class="col-md-2">
                                                <div class="form-label">
                                                    <label class="form-check-label" for="swpCase">
                                                        <asp:CheckBox ID="swpCase" runat="server" />
                                                        SWP Case
                                                    </label>
                                                </div>
                                            </div>

                                            <div class="col-md-2">
                                                <div class="form-label">
                                                    <label class="form-check-label" for="freedomSip">
                                                        <asp:CheckBox ID="freedomSip" runat="server" />
                                                        Freedom SIP
                                                    </label>
                                                </div>
                                            </div>


                                            <div class="col-md-2">
                                                <div class="form-label">
                                                    <label class="form-check-label" for="or99Years">
                                                        <asp:RadioButton ID="or99Years" runat="server" GroupName="duration" Value="99years" oninput="calculateSIPEndDateForUpdate()" />
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
                                                                    <asp:Label ID="lblSIPStartDateupdate" runat="server" CssClass="form-label" AssociatedControlID="txtSIPStartDateupdate" Text="SIP Start Date" Visible="false" />
                                                                    <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                                        <asp:TextBox ID="txtSIPStartDateupdate" runat="server" CssClass="form-control date-input" placeholder="dd/mm/yyyy" onblur="calculateSIPEndDateForUpdate()" Visible="false" />
                                                                        <div class="input-group-addon">
                                                                            <span class="glyphicon glyphicon-th"></span>
                                                                        </div>
                                                                    </div>
                                                                </div>


                                                            </div>

                                                            <div class="row mb-3">
                                                                <!-- Switch Scheme Checkbox -->
                                                                <div class="col-md-3">
                                                                    <asp:Label class="form-check-label" for="SwitchScheme" Visible="false" ID="lblswisch" runat="server">
                                                                        <asp:CheckBox ID="SwitchChebox" runat="server" Visible="false" />
                                                                        Switch Scheme
                                                                    </asp:Label>
                                                                </div>

                                                                <!-- SIP End Date -->
                                                                <div class="col-md-6">
                                                                    <asp:Label ID="lblSIPEndDateupdate" runat="server" CssClass="form-label" AssociatedControlID="txtSIPEndDateupdate" Text="SIP End Date" Visible="false" />
                                                                    <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                                        <asp:TextBox ID="txtSIPEndDateupdate" runat="server" CssClass="form-control date-input" placeholder="dd/mm/yyyy" Visible="false" />
                                                                        <div class="input-group-addon">
                                                                            <span class="glyphicon glyphicon-th"></span>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                                <div class="form-label">
                                                                    <asp:RadioButton ID="freshINupdate" runat="server" GroupName="sIp" Text="Fresh" Checked="true" Visible="false" />
                                                                </div>
                                                                <div class="form-label">
                                                                    <asp:RadioButton ID="renewalInUpdate" runat="server" GroupName="sIp" Text="Renewal" Visible="false" />
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

                                                <div class="panel-heading" style="cursor: move; display: flex; justify-content: space-between; align-items: center;">
                                                    <h3 class="panel-title">Scheme Search</h3>
                                                    <button type="button" class="close" onclick="closeSchemeSearchPanel2()" style="font-size: 18px;">&times;</button>
                                                </div>

                                                <div class="panel panel-primary">
                                                    <div class="panel-body">

                                                        <!-- UpdatePanel starts here -->
                                                        <asp:UpdatePanel ID="UpdatePanelSchemeSearch" runat="server" UpdateMode="Conditional">
                                                            <ContentTemplate>

                                                                <asp:Label ID="lblSearchScheme" runat="server" Text="Find:" CssClass="control-label" />
                                                                <asp:TextBox ID="txtSearchScheme" runat="server" CssClass="form-control" />

                                                                <div style="text-align: center; margin-top: 10px;">
                                                                    <!-- Search Button inside UpdatePanel -->
                                                                    <asp:Button ID="Button9" runat="server" Text="Search" CssClass="btn btn-primary btn-sm"
                                                                        OnClick="btnSearchScheme_Click" Style="margin-top: 5px;" />
                                                                    <button type="button" class="btn btn-secondary btn-sm" onclick="closeSchemeSearchPanel2()" style="margin-left: 5px;">Close</button>
                                                                </div>

                                                                <div class="table-responsive">
                                                                    <!-- GridView inside UpdatePanel -->
                                                                    <asp:GridView ID="SchemeGrid2" CssClass="table table-bordered" runat="server" AutoGenerateColumns="false"
                                                                        OnRowCommand="SchemeGrid2_RowCommand">
                                                                        <HeaderStyle CssClass="thead-dark" />
                                                                        <Columns>
                                                                            <asp:TemplateField HeaderText="Action" ItemStyle-HorizontalAlign="Center">
                                                                                <ItemTemplate>
                                                                                    <!-- Select Button inside UpdatePanel -->
                                                                                    <asp:Button ID="btnSelectScheme2" runat="server" Text="Select"
                                                                                        CommandName="SelectScheme" CommandArgument='<%# Eval("sch_code") %>' />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Scheme Code">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblSchemeCode2" runat="server" Text='<%# Eval("sch_code") %>' />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Scheme Name">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblSchemeName2" runat="server" Text='<%# Eval("sch_name") %>' />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>

                                                                            <asp:TemplateField HeaderText="Short Name">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblShortName2" runat="server" Text='<%# Eval("SHORT_NAME") %>' />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>

                                                                            <asp:TemplateField HeaderText="AMC Name">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblAMCName2" runat="server" Text='<%# Eval("mut_name") %>' />
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
                                                            <asp:RadioButton ID="cheque_view" runat="server" GroupName="paymentMethod_view" Text="Cheque" InputAttributes-Value="cheque_view" />
                                                        </div>
                                                        <div class="form-label">
                                                            <asp:RadioButton ID="draft_view" runat="server" GroupName="paymentMethod_view" Text="Draft" InputAttributes-Value="draft_view" />
                                                        </div>
                                                        <div class="form-label">
                                                            <asp:RadioButton ID="ecs_view" runat="server" GroupName="paymentMethod_view" Text="ECS" InputAttributes-Value="ecs_view" />
                                                        </div>
                                                        <div class="form-label">
                                                            <asp:RadioButton ID="cash_view" runat="server" GroupName="paymentMethod_view" Text="Cash" InputAttributes-Value="cash_view" />
                                                        </div>
                                                        <div class="form-label">
                                                            <asp:RadioButton ID="others_view" runat="server" GroupName="paymentMethod_view" Text="Others" InputAttributes-Value="others_view" />
                                                        </div>
                                                        <div class="form-label">
                                                            <asp:RadioButton ID="rtgs_view" runat="server" GroupName="paymentMethod_view" Text="RTGS" InputAttributes-Value="rtgs_view" />
                                                        </div>
                                                        <div class="form-label">
                                                            <asp:RadioButton ID="fund_view" runat="server" GroupName="paymentMethod_view" Text="Fund Transfer" InputAttributes-Value="neft_view" />
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
                                                            <asp:TextBox ID="TextBox3" runat="server" CssClass="form-control date-input" placeholder="dd/mm/yyyy" />
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
                                                            <asp:TextBox ID="TextBox5" runat="server" CssClass="form-control date-input" placeholder="dd/mm/yyyy" />
                                                            <div class="input-group-addon">
                                                                <span class="glyphicon glyphicon-th"></span>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <!-- RTGS Details -->
                                                    <div id="rtgsDetails_view" class="col-md-4 payment-detail-view" style="display: none;">
                                                        <asp:Label ID="Label10" runat="server" CssClass="form-label" Text="RTGS Transaction No" />
                                                        <asp:TextBox ID="TextBox6" runat="server" CssClass="form-control" />
                                                        <asp:Label ID="Label11" runat="server" CssClass="form-label" Text="RTGS Date" />
                                                        <span class="text-danger">*</span>
                                                        <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                            <asp:TextBox ID="TextBox7" runat="server" CssClass="form-control date-input" placeholder="dd/mm/yyyy" />
                                                            <div class="input-group-addon">
                                                                <span class="glyphicon glyphicon-th"></span>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <!-- NEFT/Fund Transfer Details -->
                                                    <div id="neftDetails_view" class="col-md-4 payment-detail-view" style="display: none;">
                                                        <asp:Label ID="Label12" runat="server" CssClass="form-label" Text="Fund Transfer No" />
                                                        <asp:TextBox ID="TextBox8" runat="server" CssClass="form-control" />
                                                        <asp:Label ID="Label13" runat="server" CssClass="form-label" Text="Fund Transfer Date" />
                                                        <span class="text-danger">*</span>
                                                        <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                            <asp:TextBox ID="TextBox9" runat="server" CssClass="form-control date-input" placeholder="dd/mm/yyyy" />
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
                                                            <asp:TextBox ID="TextBox17" runat="server" CssClass="form-control date-input" placeholder="dd/mm/yyyy" />
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
                                                            <asp:TextBox ID="TextBox19" runat="server" CssClass="form-control date-input" placeholder="dd/mm/yyyy" />
                                                            <div class="input-group-addon">
                                                                <span class="glyphicon glyphicon-th"></span>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <!-- Others Payment Details -->
                                                    <div id="othersDetails_view" class="col-md-4 payment-detail-view" style="display: none;">
                                                        <asp:Label ID="Label18" runat="server" CssClass="form-label" Text="Others Reference No" />
                                                        <asp:TextBox ID="TextBox20" runat="server" CssClass="form-control" />
                                                        <asp:Label ID="Label19" runat="server" CssClass="form-label" Text="Payment Date" />
                                                        <span class="text-danger">*</span>
                                                        <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                            <asp:TextBox ID="TextBox21" runat="server" CssClass="form-control date-input" placeholder="dd/mm/yyyy" />
                                                            <div class="input-group-addon">
                                                                <span class="glyphicon glyphicon-th"></span>
                                                            </div>
                                                        </div>
                                                    </div>

                                                </div>
                                            </div>



                                            <asp:Panel ID="popupPanel" runat="server" CssClass="panel panel-default draggable"
                                                Style="display: none; position: fixed; top: 20%; left: 35%; width: 30%; max-height: 400px; overflow: auto; background-color: white; border: 1px solid #ccc; z-index: 5000;">
                                                <div class="panel-heading" style="cursor: move; display: flex; justify-content: space-between; align-items: center;">
                                                    <h4 class="panel-title">Investor Address Updation</h4>
                                                    <button type="button" class="close" onclick="hidePopup()" style="font-size: 18px;">&times;</button>
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
                                                            <asp:TextBox ID="txtDOB" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                                        </div>

                                                        <div style="flex: 1;">
                                                            <label for="ddlCity">City</label>
                                                            <asp:DropDownList ID="ddlCity" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="ddlCity_SelectedIndexChanged">
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
                                                        <asp:Button ID="Button2" runat="server" CssClass="btn btn-secondary" Text="Exit" OnClientClick="hidePopup(); return false;" />
                                                    </div>
                                                </div>
                                            </asp:Panel>

                                            
                                             <asp:HiddenField ID="hdnPopupVisible2" runat="server" />

                                            <asp:Panel ID="Panel3" runat="server" CssClass="panel panel-default draggable"
                                                Style="display: none; position: fixed; top: 20%; left: 35%; width: 30%; max-height: 400px; overflow: auto; background-color: white; border: 1px solid #ccc; z-index: 5000; padding: 15px;">
                                                <div class="panel-heading" style="cursor: move; display: flex; justify-content: space-between; align-items: center;">
                                                    <h4 class="panel-title">Investor Address Updation 2</h4>
                                                    <button type="button" class="close" onclick="hidePopup4()" style="font-size: 18px;">&times;</button>
                                                </div>

                                                <div class="panel-body">
                                                    <h5>Investor Details</h5>

                                                    <!-- Address Fields -->
                                                    <div class="form-group" style="display: flex; flex-wrap: wrap; gap: 15px;">
                                                        <div style="flex: 1;">
                                                            <label for="txtAddressADDd3">Address 1</label>
                                                            <asp:TextBox ID="txtAddressADD3" runat="server" CssClass="form-control" Style="margin-bottom: 10px;"></asp:TextBox>
                                                        </div>

                                                        <div style="flex: 1;">
                                                            <label for="txtPinADDd3">PIN</label>
                                                            <asp:TextBox ID="txtPinADD3" runat="server" onblur="handleBlur(this)" oninput="validatePinInput(this)" CssClass="form-control" Style="margin-bottom: 10px;" MaxLength="10"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <!-- Mobile and PAN -->
                                                    <div class="form-group" style="display: flex; flex-wrap: wrap; gap: 15px;">
                                                        <div style="flex: 1;">
                                                            <label for="txtAddressADDd23">Address 2</label>
                                                            <asp:TextBox ID="txtAddressADD23" runat="server" CssClass="form-control" Style="margin-bottom: 10px;"></asp:TextBox>
                                                        </div>
                                                        <div style="flex: 1;">
                                                            <label for="txtMobileADDd3">Mobile</label>
                                                            <asp:TextBox ID="txtMobileADD3" runat="server" onblur="handleBlur(this)" oninput="validateMobileInput(this)" CssClass="form-control" Style="margin-bottom: 10px;"></asp:TextBox>
                                                        </div>

                                                    </div>

                                                    <!-- Email and Aadhaar -->
                                                    <div class="form-group" style="display: flex; flex-wrap: wrap; gap: 15px;">
                                                        <div style="flex: 1;">
                                                            <label for="txtPanADDd3">PAN</label>
                                                            <asp:TextBox ID="txtPanADD3" runat="server" onblur="handleBlur(this)" oninput="validatePanInput(this)" CssClass="form-control" Style="margin-bottom: 10px;" MaxLength="10"></asp:TextBox>
                                                        </div>
                                                        <div style="flex: 1;">
                                                            <label for="txtEmailADDd3">Email</label>
                                                            <asp:TextBox ID="txtEmailADD3" runat="server" onblur="handleBlur(this)" oninput="validateEmailInput(this)" CssClass="form-control" Style="margin-bottom: 10px;"></asp:TextBox>
                                                        </div>

                                                    </div>

                                                    <!-- Additional Fields -->
                                                    <div class="form-group" style="display: flex; flex-wrap: wrap; gap: 15px;">
                                                        <div style="flex: 1;">
                                                            <label for="txtAadharADDd3">Aadhar</label>
                                                            <asp:TextBox ID="txtAadharADD3" runat="server" onblur="handleBlur(this)" oninput="validateAadhaarInput(this)" CssClass="form-control" Style="margin-bottom: 10px;" MaxLength="12"></asp:TextBox>
                                                        </div>
                                                        <div style="flex: 1;">
                                                            <label class="form-label">
                                                                Date of Birth (DOB)
                                                            </label>
                                                            <div class="date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                                <asp:TextBox ID="txtDOBADD3" runat="server" CssClass="form-control date-input" placeholder="dd/mm/yyyy" />
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
                                                            <asp:DropDownList ID="ddlCityADD3" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="ddlCity3_SelectedIndexChanged" Style="margin-bottom: 10px;">
                                                                <asp:ListItem Text="--Select city--" Value="" />
                                                            </asp:DropDownList>
                                                        </div>
                                                        <div style="flex: 1;">
                                                            <label for="ddlStateADDd3">State</label>
                                                            <asp:DropDownList ID="ddlStateADD3" runat="server" CssClass="form-control" Style="margin-bottom: 10px;">
                                                                <asp:ListItem Text="--Select state--" Value="" />
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>

                                                    <div class="form-group" style="text-align: center;">
                                                        <asp:Button ID="Button33" runat="server" CssClass="btn btn-primary" Text="Update" OnClick="btnPanel3Update_Click" />
                                                        <asp:Button ID="Button73" runat="server" CssClass="btn btn-secondary" Text="Exit" OnClientClick="hidePopup4(); return false;" />
                                                    </div>
                                                </div>
                                            </asp:Panel>

                                            <div class="col-md-4">
                                                <label for="remarks" class="form-label">Remarks</label>
                                                <asp:TextBox ID="remarks" runat="server" CssClass="form-control" Placeholder="Remarks"></asp:TextBox>
                                            </div>


                                            <div class="col-md-4">
                                                <asp:Label ID="lblExpensesPercent" runat="server" CssClass="form-label" AssociatedControlID="expensesPercent" Text="Expenses %" />
                                                <asp:TextBox ID="expensesPercent" runat="server" CssClass="form-control" Placeholder="Expenses %" oninput="validateExpensesPercent(this)" Enabled="false" />
                                            </div>

                                            <div class="col-md-4">
                                                <asp:Label ID="lblExpensesRs" runat="server" CssClass="form-label" AssociatedControlID="expensesRs" Text="Expenses (Rs)" />
                                                <asp:TextBox ID="expensesRs" runat="server" CssClass="form-control" Placeholder="Expenses (Rs)" oninput="allowOnlyNumeric(this)" Enabled="false" />
                                            </div>

                                            <div class="col-md-12 mt-3">
                                                <h4 class="mb-3 text-danger">Reco Status</h4>
                                                <div class="row g-3">
                                                    <div>
                                                        <div class="form-label">
                                                            <asp:CheckBox ID="autoSwitchOnMaturity" runat="server" />
                                                            <asp:Label ID="lblAutoSwitchOnMaturity" runat="server" CssClass="form-check-label" AssociatedControlID="autoSwitchOnMaturity" Text="Auto Switch On Maturity" />
                                                        </div>
                                                    </div>

                                                    <div class="col-md-4">
                                                        <asp:Label ID="lblRecoStatus" runat="server" CssClass="form-label" AssociatedControlID="recoStatus" Text="Reco Status" />
                                                        <asp:TextBox ID="recoStatus" runat="server" CssClass="form-control" Placeholder="Reco Status" />
                                                    </div>

                                                    <div class="col-md-4">
                                                        <div class="d-flex align-items-center gap-3 mt-4">
                                                            <div class="form-label">
                                                                <asp:RadioButton ID="RadioButton9" runat="server" GroupName="regularNfo" Text="Regular" />
                                                            </div>
                                                            <div class="form-label">
                                                                <asp:RadioButton ID="RadioButton10" runat="server" GroupName="regularNfo" Text="NFO" />
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-md-4">
                                                        <asp:Label ID="lblSearchBox" runat="server" CssClass="form-label" AssociatedControlID="searchBox" Text="Search" />
                                                        <div class="input-group">
                                                            <asp:TextBox ID="searchBox" runat="server" CssClass="form-control" Placeholder="Search" ReadOnly="true" />
                                                            <asp:Button ID="Button5" runat="server" CssClass="btn btn-outline-primary" Text="Search" Enabled="false" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>


                                            <div class="col-md-12 mt-4">
                                                <div class="d-flex align-items-md-center flex-wrap flex-md-row flex-column gap-3">

                                                    <asp:Button ID="btnModify" runat="server" CssClass="btn btn-primary" Text="Modify" OnClick="btnModClick" />

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
                                                        Text="SearchTRPrint" OnClientClick="return confirmMultipleTransaction();" OnClick="btn_PrintModify" />

                                                    <asp:HiddenField ID="hdnPrintOption" runat="server" />
                                                    <asp:Button ID="Button6" runat="server" CssClass="btn btn-outline-primary" Text="Reset" OnClick="btnReset_Click" OnClientClick="ResetPayview(); return true;" />
                                                    <asp:Button ID="btnTranCancel" runat="server" CssClass="btn btn-outline-primary" Text="Tran Cancel" OnClientClick="showCancelPopup(); return false;" />
                                                    <asp:Button ID="btnExitView" runat="server" CssClass="btn btn-outline-primary" Text="Exit" OnClick="btnExit_Click" />
                                                    <asp:Button ID="btnOpenPanel" runat="server" Text="View Log" CssClass="btn btn-outline-primary"
                                                        OnClick="CmdViewLog_Click" OnClientClick="openPanel(); return false;" />

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


                                                    <asp:Panel ID="FrmTransactionLog" runat="server" Visible="false"
                                                        CssClass="floating-panel"
                                                        Style="position: fixed; top: 50%; left: 50%; transform: translate(-50%, -50%); z-index: 1050; width: 90%; max-width: 1000px; max-height: 90%; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1); border-radius: 8px; padding: 0; border: 1px solid #ddd;">
                                                        <!-- Heading Section -->
                                                        <div style="border-bottom: 1px solid #ddd; padding: 20px; display: flex; justify-content: space-between; align-items: center;">
                                                            <h5 class="text-primary fw-bold" style="margin: 0;">Transaction Log</h5>
                                                            <button type="button" class="btn-close" onclick="closePanel()" aria-label="Close"></button>

                                                        </div>
                                                        <!-- Content Section with Scrollable Grid -->
                                                        <div style="padding: 20px; overflow-y: auto; max-height: calc(90vh - 120px);">
                                                            <asp:GridView ID="gvTransactionLog" runat="server" AutoGenerateColumns="True"
                                                                CssClass="table table-striped table-bordered"
                                                                HeaderStyle-CssClass="thead-light"
                                                                RowStyle-CssClass="table-light"
                                                                AlternatingRowStyle-CssClass="table-secondary">
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

                                            <asp:TextBox ID="lblWaning2" runat="server" CssClass="form-control" ReadOnly="true" BackColor="#ffdddd" Visible="false" />

                                            <%--<h5 class="card-title">Print Preview</h5>--%>
                                            <div style="overflow-y: auto; overflow-x: auto; max-height: 450px;">


                                                <div class="table-responsive" style="display: none">
                                                    <asp:GridView ID="gridview1" CssClass="table table-bordered" runat="server"
                                                        AutoGenerateColumns="False" OnRowCommand="tableSearchResultsTran_RowCommand">
                                                        <HeaderStyle CssClass="thead-dark" />
                                                        <Columns>

                                                            <asp:TemplateField HeaderText="AR NO">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="Label23" runat="server" Text='<%# Eval("TRAN_CODE") %>' />
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
                                                                    <asp:Label ID="Label38" runat="server" Text='<%# Eval("BANK_NAME") %>' />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Cheque No">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="Label39" runat="server" Text='<%# Eval("CHEQUE_NO") %>' />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Cheque Date">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="Label40" runat="server" Text='<%# Eval("CHEQUE_DATE", "{0:dd/MM/yyyy}") %>' />
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
                                                                    <asp:Label ID="Label47" runat="server" Text='<%# Eval("PAYMENT_MODE") %>' />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Paid Amount">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="Label48" runat="server" Text='<%# Eval("PAIDAMT") %>' />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Upfront Paid">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="Label49" runat="server" Text='<%# Eval("PAID_BROK") %>' />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Logged User ID">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="Label50" runat="server" Text='<%# Eval("LOGGEDUSERID") %>' />
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
                                                        <p style="margin: 0; font-size: 12px; color: #555;">* For Investment in mutual funds, this receipt should be considered as provisional / temporary receipt</p>
                                                    </div>
                                                </div>
                                            </asp:Panel>

                                            <asp:Panel ID="CancelPopup" runat="server" CssClass="modal-popup" Style="display: none; position: fixed; top: 5%; left: 50%; transform: translateX(-50%); width: 40%; background-color: white; border: 1px solid #ccc; padding: 20px; z-index: 9999; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.2);">

                                                <div class="modal-header">
                                                    <h4 class="panel-title">Cancel Transaction</h4>
                                                    <button type="button" class="close" onclick="hideCancelPopup();">&times;</button>
                                                </div>

                                                <div class="modal-body">

                                                    <div class="form-group">
                                                        <label for="txtCanTrcode">Transaction Code:</label>
                                                        <asp:TextBox ID="txtCanTrcode" runat="server" CssClass="form-control" Enabled="false" />
                                                    </div>


                                                    <div class="form-group">
                                                        <label for="mdatecancel">Cancel Date:</label>
                                                        <asp:TextBox ID="mdatecancel" runat="server" CssClass="form-control" Text="__/__/____" />
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

                                                    <asp:Button ID="Button4" runat="server" CssClass="btn btn-primary" Text="OK" OnClick="btnOk_Click" />
                                                    <asp:Button ID="btnClosePopup" runat="server" CssClass="btn btn-secondary" Text="Cancel" OnClientClick="hideCancelPopup(); return false;" />
                                                </div>
                                            </asp:Panel>


                                            <asp:Panel ID="AutoSwitchPanel" runat="server" CssClass="modal-popup"
                                                Style="display: none; position: fixed; top: 20%; left: 30%; width: 40%; background-color: white; border: 1px solid #ccc; padding: 20px; z-index: 1000; box-shadow: 0px 4px 6px rgba(0, 0, 0, 0.1);">

                                                <div class="modal-header" style="display: flex; justify-content: space-between; align-items: center; border-bottom: 1px solid #eaeaea; margin-bottom: 15px;">
                                                    <h4 class="panel-title" style="margin: 0; font-size: 18px; font-weight: bold;">WealthMaker</h4>
                                                    <button type="button" class="btn-close" aria-label="Close" onclick="hideAutoSwitchPanel();" style="font-size: 16px; background: none; border: none; cursor: pointer;">&times;</button>
                                                </div>


                                                <div class="modal-body" style="text-align: center; margin-bottom: 20px;">
                                                    <p style="font-size: 14px; color: #555;">Auto switch on maturity can be selected on close-ended and non-SIP transactions.</p>
                                                </div>


                                                <div class="modal-footer" style="text-align: center;">
                                                    <asp:Button ID="Button8" runat="server" Text="OK" CssClass="btn btn-primary" OnClientClick="hideAutoSwitchPanel(); return false;" />
                                                </div>
                                            </asp:Panel>


                                            <asp:Panel ID="Panel1" runat="server" CssClass="panel panel-default" Style="display: none; position: fixed; top: 30%; left: 35%; width: 30%; background-color: white; border: 1px solid #ccc; z-index: 1000; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);">
                                                <div class="panel-heading" style="display: flex; justify-content: space-between; align-items: center;">
                                                    <h4 class="panel-title">Wealth Maker</h4>
                                                    <button type="button" class="close" onclick="hidePopup2()">&times;</button>
                                                </div>
                                                <div class="panel-body">
                                                    <p>Security restriction for data range.</p>
                                                    <asp:Button ID="btnOk" runat="server" CssClass="btn btn-primary" Text="OK" OnClientClick="hidePopup2(); return false;" />
                                                </div>
                                            </asp:Panel>
                                        </div>
                                    </div>





                                </ContentTemplate>




                            </asp:UpdatePanel>

                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    
      <!-- MODEL: INVESTOR SEARCH model investor search inv search model-->
  <div class="modal fade" id="investorSearchModal" style="z-index:5000;" tabindex="-1" aria-labelledby="investorSearchModalLabel" aria-hidden="true">
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
                              <input class="form-check-input m-3 d-none" type="checkbox" id="chkInvSearch" />
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
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script type="text/javascript">
$(document).ready(function () {
    $("#inv_find1").click(function () {
            $("#chkInvSearch").prop("checked", false); // uncheck
    });

    $("#inv_find2").click(function () {
        $("#chkInvSearch").prop("checked", true); // check
    });

    


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



        if ($("#chkInvSearch").is(":checked")) {

            const delimiter = '##@@##';

            let combinedData = [
                investorName,
                invCode,
                mobile,
                address1,
                address2,
                cityId,
                stateId,
                pincode,
                pan,
                aadharNo,
                dob,
                ahClientCode,
                BusinessCodeInMod,
                RmNameInMod,
                BRANCHCODE
            ].join(delimiter);

            $('#<%= hdnInvCodeInModify.ClientID %>').val(combinedData);
           $(this).closest('.modal').find("button[data-bs-dismiss='modal']").trigger('click');
          __doPostBack('<%= btnInvSearch2ForTr.UniqueID %>', '');
          event.stopImmediatePropagation();
        }else{

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
           }
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


    <script>
        document.getElementById("inv_find2").addEventListener("click", function() {
          sessionStorage.setItem("invSearch2", "true");
        });

        document.getElementById("inv_find1").addEventListener("click", function() {
          sessionStorage.removeItem("invSearch2");
        });

    </script>

     

    <script src="assets/vendors/js/vendor.bundle.base.js"></script>
    <script src="assets/vendors/chart.js/chart.umd.js"></script>
    <script src="assets/vendors/bootstrap-datepicker/bootstrap-datepicker.min.js"></script>
    <script src="assets/js/off-canvas.js"></script>
    <script src="assets/js/misc.js"></script>
    <script src="assets/js/settings.js"></script>
    <script src="assets/js/todolist.js"></script>
    <script src="assets/js/jquery.cookie.js"></script>
    <script src="assets/js/dashboard.js"></script>
    <script src="assets/js/folder-handler.js"></script>


    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/toastify-js/src/toastify.min.css">
    <script src="https://cdn.jsdelivr.net/npm/toastify-js"></script>
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://code.jquery.com/ui/1.12.1/jquery-ui.min.js"></script>
    <link rel="stylesheet" href="https://code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css">

    <%-- JQUERY: COMMON DRAGGABLE CALL, SHOW/HIDE POPUP: popupPanel --%>
    <script type="text/javascript">
        $(document).ready(function () {
            // Make the panel draggable
            $(".draggable").draggable();
        });

        function showPopup() {
            document.getElementById('<%= popupPanel.ClientID %>').style.display = 'block';

        }

        function hidePopup() {
            document.getElementById('<%= popupPanel.ClientID %>').style.display = 'none';
        }
    </script>

    <%-- JQUERY: COMMON DRAGGABLE CALL, SHOW/HIDE POPUP: Panel2  --%>
    <script type="text/javascript">
        $(document).ready(function () {
            // Make the panel draggable
            $(".draggable").draggable();
        });





        function showPopup3() {
            document.getElementById('<%= Panel2.ClientID %>').style.display = 'block';
            localStorage.setItem("popupVisiblePanel2", "true");
            //document.getElementById('<%= hdnPopupVisible1.ClientID %>').value = "true";
        }

        function hidePopup3() {
            document.getElementById('<%= Panel2.ClientID %>').style.display = 'none';
        localStorage.removeItem("popupVisiblePanel2");
        document.getElementById('<%= hdnPopupVisible1.ClientID %>').value = "false";
    }

    window.onload = function () {
        var shouldShow = localStorage.getItem("popupVisiblePanel2");
        if (shouldShow && shouldShow.toLowerCase().includes("true")) {
            showPopup3();
            //document.getElementById('<%= hdnPopupVisible1.ClientID %>').value = "true";
        }
    };


    </script>

    <%-- JQUERY: inv focous reaonly blur, SHOW/HIDE POPUP: Panel3  --%>
    <script type="text/javascript">
        $(document).ready(function () {
            
            $("[id$='invcode']").on("focus",function(){
                $(this).prop("readonly",true);
            }).on("blur",function(){
                $(this).prop("readonly",false);
            });
        });

        function showPopup4() {
            document.getElementById('<%= Panel3.ClientID %>').style.display = 'block';
            localStorage.setItem("popupVisiblePanel3", "true");

        
        }

        function hidePopup4() {
            document.getElementById('<%= Panel3.ClientID %>').style.display = 'none';
            localStorage.removeItem("popupVisiblePanel3");
            document.getElementById('<%= hdnPopupVisible2.ClientID %>').value = "false";
        }
        window.onload = function () {
            var shouldShow = localStorage.getItem("popupVisiblePanel3");
            if (shouldShow && shouldShow.toLowerCase().includes("true")) {
                showPopup4();
             //document.getElementById('<%= hdnPopupVisible1.ClientID %>').value = "true";
         }
        };



    </script>

    <%-- JS: SHOW/HIDE: Panel1 --%>
    <script type="text/javascript">
        function showPopup2() {
            document.getElementById('<%= Panel1.ClientID %>').style.display = 'block';
        }

        function hidePopup2() {
            document.getElementById('<%= Panel1.ClientID %>').style.display = 'none';
        }
    </script>

    <%-- JS: SHOW/HIDE CANCEL POPUP : CancelPopup --%>
    <script type="text/javascript">
        function showCancelPopup() {
            document.getElementById('<%= CancelPopup.ClientID %>').style.display = 'block';

        }

        function hideCancelPopup() {
            document.getElementById('<%= CancelPopup.ClientID %>').style.display = 'none';

        }
    </script>

    <%-- JS: PRINT HTML --%>
    <script type="text/javascript">
        function printDiv() {
            var printWindow = window.open('', '', 'height=600,width=800');
            var content = document.getElementById('printableArea').innerHTML;
            printWindow.document.write('<html><head><title>Print</title>');
            printWindow.document.write('</head><body >');
            printWindow.document.write(content);
            printWindow.document.write('</body></html>');
            printWindow.document.close();
            printWindow.focus();
            printWindow.print();
        }
    </script>

    <%-- STYLE: PRINT HTML --%>
    <style>
        .print-table {
            width: 100%;
            border-collapse: collapse;
            border: 1px solid #000;
        }

            .print-table th, .print-table td {
                border: 1px solid #000;
                padding: 8px;
            }

            .print-table th {
                background-color: #f2f2f2;
            }
    </style>

    <%-- JS: SHOW/HIDE: SchemeSearchPanel --%>
    <script type="text/javascript">
        function showSchemeSearchPanel() {
            document.getElementById('<%= SchemeSearchPanel.ClientID %>').style.display = 'block';
        }

        function closeSchemeSearchPanel() {
            document.getElementById('<%= SchemeSearchPanel.ClientID %>').style.display = 'none';
        }
    </script>

    <asp:HiddenField ID="hdnSelectedPaymentMethod" runat="server" />
    
    <%-- JS: INIT PAYMENT METHOD MULTIP FIELD SHOW HIDE --%>
    <script>
        function initializePaymentMethod() {
            const paymentMethodContainer = document.getElementById("paymentMethodContainer");
            const paymentDetails = document.querySelectorAll(".payment-detail");
            const dateInputs = document.querySelectorAll(".date-picker");
            const selectedMethodHiddenField = document.getElementById('<%= hdnSelectedPaymentMethod.ClientID %>');



            function initializeDatePickers() {
                dateInputs.forEach((input) => {
                    new Pikaday({
                        field: input,
                        format: "DD/MM/YYYY",
                    });
                });
            }

            function hideAllDetails() {
                paymentDetails.forEach((detail) => (detail.style.display = "none"));
            }

            function showSelectedPaymentMethod(selectedValue) {
                hideAllDetails();
                switch (selectedValue) {
                    case "cheque":
                        document.getElementById("chequeDetails1").style.display = "block";
                        break;
                    case "draft":
                        document.getElementById("draftDetails").style.display = "block";
                        break;
                    case "rtgs":
                        document.getElementById("rtgsDetails").style.display = "block";
                        break;
                    case "neft":
                        document.getElementById("neftDetails").style.display = "block";
                        break;
                    case "ecs":
                        document.getElementById("ecsDetails").style.display = "block";
                        break;
                    case "cash":
                        document.getElementById("cashDetails").style.display = "block";
                        break;
                    case "others":
                        document.getElementById("othersDetails").style.display = "block";
                        break;
                }
            }

            // Set initial state based on hidden field value
            if (selectedMethodHiddenField) {
                const initialSelectedMethod = selectedMethodHiddenField.value;
                if (initialSelectedMethod) {
                    showSelectedPaymentMethod(initialSelectedMethod);
                } else {
                    hideAllDetails();
                    document.getElementById("chequeDetails1").style.display = "block";
                }
            }

            if (paymentMethodContainer) {
                paymentMethodContainer.addEventListener("change", function (e) {
                    const selectedValue = e.target.value;
                    if (selectedMethodHiddenField) {
                        selectedMethodHiddenField.value = selectedValue; // Save to hidden field
                        showSelectedPaymentMethod(selectedValue);
                    }
                });
            }

            function showChequeDetailsOnly() {
                if (selectedMethodHiddenField.value === 'cheque') {
                    var paymentDetails = document.querySelectorAll(".payment-detail");
                    paymentDetails.forEach(function (detail) {
                        detail.style.display = "none";
                    });

                    document.getElementById("chequeDetails1").style.display = "block";
                    console.log('okk ho gya');
                    var hiddenField = document.getElementById("hdnSelectedPaymentMethod");
                    if (hiddenField) {
                        hiddenField.value = "cheque";
                    }
                }
            }

            initializeDatePickers();
        }

       

        // Ensure the function runs on page load
        document.addEventListener("DOMContentLoaded", function () {
            initializePaymentMethod();
        });

        // Ensure function runs after each UpdatePanel partial postback
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            initializePaymentMethod();
        });

        //document.addEventListener("DOMContentLoaded", function () {
        //    if (document.getElementById("hdnSelectedPaymentMethod").value === "cheque") {
        //        showChequeDetailsOnly();
        //    }
        //});

        //Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
        //    if (document.getElementById("hdnSelectedPaymentMethod").value === "cheque") {
        //        showChequeDetailsOnly();
        //    }
            
        //});
    </script>

    <%-- JS: INIT PAYMENT METHOD MULTIP FIELD SHOW HIDE --%>

    <script>

        document.addEventListener("DOMContentLoaded", function () {
            const paymentMethodContainer = document.getElementById("paymentMethodContainer");
            const paymentDetails = document.querySelectorAll(".payment-detail");
            const dateInputs = document.querySelectorAll(".date-picker");
            const selectedMethodHiddenField = document.getElementById('<%= hdnSelectedPaymentMethod.ClientID %>');

            function initializeDatePickers() {
                dateInputs.forEach((input) => {
                    new Pikaday({
                        field: input,
                        format: "DD/MM/YYYY",
                    });
                });
            }

            function hideAllDetails() {
                paymentDetails.forEach((detail) => (detail.style.display = "none"));
            }

            function showSelectedPaymentMethod(selectedValue) {
                hideAllDetails();
                switch (selectedValue) {
                    case "cheque":
                        document.getElementById("chequeDetails1").style.display = "block";
                        break;
                    case "draft":
                        document.getElementById("draftDetails").style.display = "block";
                        break;
                    case "rtgs":
                        document.getElementById("rtgsDetails").style.display = "block";
                        break;
                    case "neft":
                        document.getElementById("neftDetails").style.display = "block";
                        break;
                    case "ecs":
                        document.getElementById("ecsDetails").style.display = "block";
                        break;
                    case "cash":
                        document.getElementById("cashDetails").style.display = "block";
                        break;
                    case "others":
                        document.getElementById("othersDetails").style.display = "block";
                        break;
                }
            }

            // Set initial state based on hidden field value
            const initialSelectedMethod = selectedMethodHiddenField.value;
            if (initialSelectedMethod) {
                showSelectedPaymentMethod(initialSelectedMethod);
            } else {
                hideAllDetails();
                document.getElementById("chequeDetails1").style.display = "block";
            }

            paymentMethodContainer.addEventListener("change", function (e) {
                const selectedValue = e.target.value;
                selectedMethodHiddenField.value = selectedValue; // Save to hidden field
                showSelectedPaymentMethod(selectedValue);
            });

            initializeDatePickers();
        });
    </script>

   
    <asp:HiddenField ID="hdnSelectedPaymentMethod_view" runat="server" />
    
    <%-- JS: INIT PAYMENT METHOD MULTIP FIELD SHOW HIDE --%>

    <script>
        document.addEventListener("DOMContentLoaded", function () {
            const paymentMethodContainer = document.getElementById("paymentMethodContainer_view");
            const paymentDetails = document.querySelectorAll(".payment-detail-view");
            const dateInputs = document.querySelectorAll(".date-picker");
            const selectedMethodHiddenField = document.getElementById('<%= hdnSelectedPaymentMethod_view.ClientID %>');

            // Initialize date pickers for all date inputs
            function initializeDatePickers() {
                dateInputs.forEach((input) => {
                    new Pikaday({
                        field: input,
                        format: "DD/MM/YYYY",
                        onSelect: function () {
                            console.log("Selected date: " + input.value);
                        },
                    });
                });
            }

            // Hide all payment detail sections initially
            function hideAllDetails() {
                paymentDetails.forEach((detail) => (detail.style.display = "none"));
            }

            // Show the relevant payment detail based on the selected radio button
            function showSelectedPaymentMethod(selectedValue) {
                hideAllDetails();

                switch (selectedValue) {
                    case "cheque_view":
                        document.getElementById("chequeDetails_view").style.display = "block";
                        break;
                    case "draft_view":
                        document.getElementById("draftDetails_view").style.display = "block";
                        break;
                    case "rtgs_view":
                        document.getElementById("rtgsDetails_view").style.display = "block";
                        break;
                    case "fund_view":
                        document.getElementById("neftDetails_view").style.display = "block";
                        break;
                    case "ecs_view":
                        document.getElementById("ecsDetails_view").style.display = "block";
                        break;
                    case "cash_view":
                        document.getElementById("cashDetails_view").style.display = "block";
                        break;
                    case "others_view":
                        document.getElementById("othersDetails_view").style.display = "block";
                        break;
                }
            }

            // Set initial state based on hidden field value
            const initialSelectedMethod = selectedMethodHiddenField.value;
            if (initialSelectedMethod) {
                showSelectedPaymentMethod(initialSelectedMethod);
            } else {
                hideAllDetails();
                document.getElementById("chequeDetails_view").style.display = "block"; // Default to cheque details
            }

            // Event listener to save selected payment method to hidden field
            paymentMethodContainer.addEventListener("change", function (e) {
                const selectedValue = e.target.value;
                selectedMethodHiddenField.value = selectedValue; // Save to hidden field
                showSelectedPaymentMethod(selectedValue);
            });

            // Initialize everything on load

            initializeDatePickers();
        });
    </script>


    
    <script>
        document.addEventListener("DOMContentLoaded", function () {
            initializePaymentView(); // Run on initial page load
        });

        // Ensure JavaScript re-executes after UpdatePanel updates
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            initializePaymentView(); // Re-run when UpdatePanel refreshes
        });

        // Your existing function remains unchanged, but now it runs after postback
        function initializePaymentView() {
            const paymentMethodContainer = document.getElementById("paymentMethodContainer_view");
            const paymentDetails = document.querySelectorAll(".payment-detail-view");
            const dateInputs = document.querySelectorAll(".date-picker");
            const selectedMethodHiddenField = document.getElementById('<%= hdnSelectedPaymentMethod_view.ClientID %>');

            // Initialize date pickers
            function initializeDatePickers() {
                dateInputs.forEach((input) => {
                    new Pikaday({
                        field: input,
                        format: "DD/MM/YYYY",
                        onSelect: function () {
                            console.log("Selected date: " + input.value);
                        },
                    });
                });
            }

            // Hide all payment detail sections
            function hideAllDetails() {
                paymentDetails.forEach((detail) => (detail.style.display = "none"));
            }

            // Show the relevant payment detail based on the selected radio button
            function showSelectedPaymentMethod(selectedValue) {
                hideAllDetails();

                switch (selectedValue) {
                    case "cheque_view":
                        document.getElementById("chequeDetails_view").style.display = "block";
                        break;
                    case "draft_view":
                        document.getElementById("draftDetails_view").style.display = "block";
                        break;
                    case "rtgs_view":
                        document.getElementById("rtgsDetails_view").style.display = "block";
                        break;
                    case "fund_view":
                        document.getElementById("neftDetails_view").style.display = "block";
                        break;
                    case "ecs_view":
                        document.getElementById("ecsDetails_view").style.display = "block";
                        break;
                    case "cash_view":
                        document.getElementById("cashDetails_view").style.display = "block";
                        break;
                    case "others_view":
                        document.getElementById("othersDetails_view").style.display = "block";
                        break;
                }
            }

            // Set initial state based on hidden field value
            const initialSelectedMethod = selectedMethodHiddenField.value;
            if (initialSelectedMethod) {
                showSelectedPaymentMethod(initialSelectedMethod);
            } else {
                hideAllDetails();
                document.getElementById("chequeDetails_view").style.display = "block"; // Default to cheque details
            }

            // Event listener to save selected payment method to hidden field
            if (paymentMethodContainer) {
                paymentMethodContainer.addEventListener("change", function (e) {
                    const selectedValue = e.target.value;
                    selectedMethodHiddenField.value = selectedValue; // Save to hidden field
                    showSelectedPaymentMethod(selectedValue);
                });
            }

            initializeDatePickers(); // Ensure date pickers reinitialize
        }

        // Function to show cheque details only
        function showChequeDetailsOnlyView() {
            // Hide all payment detail sections
            var paymentDetails = document.querySelectorAll(".payment-detail-view");
            paymentDetails.forEach(function (detail) {
                detail.style.display = "none";
            });

            // Show the cheque details section
            document.getElementById("chequeDetails_view").style.display = "block";

            // Set the value for the hidden field to indicate "cheque" as the selected payment method
            document.getElementById("hdnSelectedPaymentMethod_view").value = "cheque_view";
        }

        //document.addEventListener("DOMContentLoaded", function () {
        //    showChequeDetailsOnlyView();
        //});

        //Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
        //    // Check if the value of the hidden field is "cheque_view"
        //    if (document.getElementById("hdnSelectedPaymentMethod_view").value === "cheque_view") {
        //        showChequeDetailsOnlyView();
        //    }
        //});


    </script>

    <%--Payment script ends--%>

    <%-- JS: STRANSACTION CHAGNE  --%>
    <script>
        function initializeScripts() {
            initializeFieldsAfterPostback();
            restoreSwitchFields();
            restorePaymentSelection();
            checkTransactionTypeOnLoad();

            // Re-attach event listener to transaction type dropdown after every postback
            var transactionTypeDropdown = document.getElementById('<%= transactionType.ClientID %>');
            if (transactionTypeDropdown) {
                transactionTypeDropdown.addEventListener('change', function () {
                   
                    toggleSwitchSTPFields(this.value);
                    saveSwitchFields();
                    checkTransactionType(this.value, document.getElementById('<%= ddlSipStp.ClientID %>').value);
                });
            }

            // Re-attach event listener to ddlSipStp dropdown
            var ddlSipStpDropdown = document.getElementById('<%= ddlSipStp.ClientID %>');
            if (ddlSipStpDropdown) {
                ddlSipStpDropdown.addEventListener('change', function () {
                    checkTransactionType(transactionTypeDropdown.value, this.value);
                });
            }
        }

        // Ensure scripts run **after an UpdatePanel postback**
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            initializeScripts();
        });

        // Ensure scripts run **on page load**
        document.addEventListener("DOMContentLoaded", function () {
            initializeScripts();
        });

        function initializeFieldsAfterPostback() {
            const transactionTypeValue = document.getElementById('<%= transactionType.ClientID %>').value;
            toggleSwitchSTPFields(transactionTypeValue);
        }

        function toggleSwitchSTPFields(transactionValue) {
            const ddlISSTPDropdown = document.getElementById('<%= ddlSipStp.ClientID %>');

            const isSTPSelected = ddlISSTPDropdown.value === "STP";




            const isSwitchIn = transactionValue === "SWITCH IN";

            const fieldsToDisable = [
        '<%= rdo99Years.ClientID %>',
        '<%= ddlSipStp.ClientID %>',
        '<%= iNSTYPE.ClientID %>',
        '<%= siptype.ClientID %>',
        '<%= sipamount.ClientID %>',
        '<%= ddlFrequency.ClientID %>',
        '<%= txtInstallmentsNos.ClientID %>',
        '<%= txtSIPEndDate.ClientID %>',
        '<%= ddlBankName.ClientID %>',
        '<%= txtChequeNo.ClientID %>',
        '<%= txtChequeDated.ClientID %>',
        '<%= txtDraftNo.ClientID %>',
        '<%= txtDraftDate.ClientID %>',
        '<%= txtRtgsNo.ClientID %>',
        '<%= txtRtgsDate.ClientID %>',
        '<%= txtNeftNo.ClientID %>',
        '<%= txtNeftDate.ClientID %>',
        '<%= txtEcsReference.ClientID %>',
        '<%= txtEcsDate.ClientID %>',
        '<%= txtCashAmount.ClientID %>',
        '<%= txtCashDate.ClientID %>',
        '<%= txtOthersReference.ClientID %>',
        '<%= txtOthersDate.ClientID %>'
            ];

            fieldsToDisable.forEach(id => {
                let field = document.getElementById(id);
                if (field) {
                    field.disabled = isSwitchIn;
                    if (isSwitchIn) {
                        field.value = '';
                    }
                }
            });

            if (isSwitchIn) {
                let sipStartDateField = document.getElementById('<%= txtSIPStartDate.ClientID %>');
                let instypepra = document.getElementById('<%= iNSTYPE.ClientID %>');
                const radioButtonsContainer = document.getElementById("radioButtonsContainer");
                const siptypefordis = document.getElementById("sipTypeContainer");
                const sipamountfordis = document.getElementById("sipAmountContainer");

                ddlISSTPDropdown.value = "REGULAR";

                instypepra.value = "NORMAL";
                
                if (siptypefordis) {
                    siptypefordis.style.display = isSwitchIn ? "none" : "block"; 
                }
                if (sipamountfordis) {
                    sipamountfordis.style.display = isSwitchIn ? "none" : "block";
                }
                if (radioButtonsContainer) {
                    radioButtonsContainer.style.display = isSwitchIn ? "none" : "block";
                }
                if (sipStartDateField) {
                    sipStartDateField.disabled = isSwitchIn; // Disable the field
                }
            }

            // Disable Payment Method Radio Buttons
            const paymentMethods = [
        '<%= cheque.ClientID %>',
        '<%= draft.ClientID %>',
        '<%= ecs.ClientID %>',
        '<%= cash.ClientID %>',
        '<%= others.ClientID %>',
        '<%= rtgs.ClientID %>',
        '<%= neft.ClientID %>'
            ];

            paymentMethods.forEach(id => {
                let radioBtn = document.getElementById(id);
                if (radioBtn) {
                    radioBtn.disabled = isSwitchIn;
                }
            });

            const formSwitchFolio = document.getElementById('<%= formSwitchFolio.ClientID %>');
            const formSwitchScheme = document.getElementById('<%= formSwitchScheme.ClientID %>');
            const btnSearchFormSwitchScheme = document.getElementById('<%= btnSearchFormSwitchScheme.ClientID %>');

            

            formSwitchFolio.readOnly = !isSwitchIn;
            formSwitchScheme.readOnly = !isSwitchIn;
            btnSearchFormSwitchScheme.disabled = !isSwitchIn;

            if (isSwitchIn) {
                ddlISSTPDropdown.value = "REGULAR";
               
                formSwitchFolio.style.filter = "none";
                formSwitchFolio.style.pointerEvents = "auto";

                formSwitchScheme.style.filter = "none";
                formSwitchScheme.style.pointerEvents = "auto";

            }

           
            const ddlSipStp = document.getElementById('<%= ddlSipStp.ClientID %>');
            ddlSipStp.value = 'REGULAR';

            if (isSwitchIn) {
                sessionStorage.setItem("formSwitchFolio", formSwitchFolio.value);
                sessionStorage.setItem("formSwitchScheme", formSwitchScheme.value);

                
            }

           


            if (isSTPSelected) {
                sessionStorage.setItem("formSwitchScheme", formSwitchScheme.value);
            }
        }

        function saveSwitchFields() {
            const formSwitchFolio = document.getElementById('<%= formSwitchFolio.ClientID %>').value;
            const formSwitchScheme = document.getElementById('<%= formSwitchScheme.ClientID %>').value;

            sessionStorage.setItem("formSwitchFolio", formSwitchFolio);
            sessionStorage.setItem("formSwitchScheme", formSwitchScheme);
        }

        function restoreSwitchFields() {
            const formSwitchFolio = document.getElementById('<%= formSwitchFolio.ClientID %>');
            const formSwitchScheme = document.getElementById('<%= formSwitchScheme.ClientID %>');

            formSwitchFolio.value = sessionStorage.getItem("formSwitchFolio") || formSwitchFolio.value;
            formSwitchScheme.value = sessionStorage.getItem("formSwitchScheme") || formSwitchScheme.value;

        }

        function savePaymentSelection() {
            let selectedPayment = "";
            const paymentMethods = [
        '<%= cheque.ClientID %>',
        '<%= draft.ClientID %>',
        '<%= ecs.ClientID %>',
        '<%= cash.ClientID %>',
        '<%= others.ClientID %>',
        '<%= rtgs.ClientID %>',
        '<%= neft.ClientID %>'
            ];

            paymentMethods.forEach(id => {
                let radioBtn = document.getElementById(id);
                if (radioBtn && radioBtn.checked) {
                    selectedPayment = id;
                }
            });

            sessionStorage.setItem("selectedPaymentMethod", selectedPayment);
        }

        function restorePaymentSelection() {
            let selectedPayment = sessionStorage.getItem("selectedPaymentMethod");
            if (selectedPayment) {
                let radioBtn = document.getElementById(selectedPayment);
                if (radioBtn) {
                    radioBtn.checked = true;
                }
            }
        }

        function checkTransactionType(transactionValue, sipTypeValue) {
            const isPurchase = transactionValue === "PURCHASE";
            const isSTP = sipTypeValue === "STP"; // Check for STP
            const formSwitchFolio = document.getElementById('<%= formSwitchFolio.ClientID %>');
            const formSwitchScheme = document.getElementById('<%= formSwitchScheme.ClientID %>');
            const switchSchemeValueByAmc = document.getElementById('<%= txtSearchSchemeDetails.ClientID %>');

            if (isPurchase ) {
                switchSchemeValueByAmc.value = '';
            }
         
            // If transaction type is PURCHASE and sipType is NOT STP, clear values
            if (isPurchase && !isSTP) {
                formSwitchFolio.value = '';
                formSwitchScheme.value = '';
            }
            if (isPurchase && isSTP) {
                sessionStorage.setItem("formSwitchFolio", formSwitchFolio.value);
                sessionStorage.setItem("formSwitchScheme", formSwitchScheme.value);
               
            }

            if (isPurchase) {
                var installmentTypeContainer = document.getElementById('installmentTypeContainer');
                var frequencyDropdown = document.getElementById('<%= ddlFrequency.ClientID %>');
                var installmentsTextBox = document.getElementById('<%= txtInstallmentsNos.ClientID %>');
                var rdo99 = document.getElementById('<%= rdo99Years.ClientID %>');
                var sipTypeContainerd = document.getElementById('<%= siptype.ClientID %>');
                var sipstartdate = document.getElementById('<%= txtSIPStartDate.ClientID %>');
                var sipenddatehere = document.getElementById('<%= txtSIPEndDate.ClientID %>');
                var installTypeRegu = document.getElementById('installmentTypeContainer');
                var installtextregu = document.getElementById('<%= iNSTYPE.ClientID %>');
                var freshrkho = document.getElementById('<%= fresh.ClientID %>');

                if (sipTypeValue === "REGULAR") {
                    frequencyDropdown.disabled = true;
                    frequencyDropdown.value = "301";
                    installmentsTextBox.disabled = true;
                    rdo99.disabled = true;
                    sipstartdate.disabled = true;
                    sipenddatehere.disabled = true;

                    installTypeRegu.style.display = 'block';
                   // installtextregu.value = "NORMAL";
                }
                if (sipTypeValue === "SIP") {
                    frequencyDropdown.value = "301";
                    sipTypeContainerd.value = "N";
                    freshrkho.checked = true;
                }
                if (sipTypeValue === "STP") {
                    frequencyDropdown.value = "301";
                }
                // Clear values
              //  frequencyDropdown.selectedIndex = 0; // Reset dropdown
              //  installmentsTextBox.value = "";      // Clear input
              //  rdo99.checked = false;
            }
        }

        function checkTransactionTypeOnLoad() {
            const transactionTypeValue = document.getElementById('<%= transactionType.ClientID %>').value;
            const sipTypeValue = document.getElementById('<%= ddlSipStp.ClientID %>').value;

            checkTransactionType(transactionTypeValue, sipTypeValue);
        }
    </script>

    <%-- JS: SHOW/HIDE AutoSwitchPanel --%>
    <script>
        function showAutoSwitchPanel() {
            document.getElementById('<%= AutoSwitchPanel.ClientID %>').style.display = 'block';
        }

        function hideAutoSwitchPanel() {
            document.getElementById('<%= AutoSwitchPanel.ClientID %>').style.display = 'none';
        }

    </script>


    <%-- JS: Focus on Search Results Grid --%>
    <script>
        document.addEventListener('DOMContentLoaded', function () {
            // Check if the grid exists and should be focused On it
            if (document.getElementById('tableSearchResults')) {
                document.getElementById('tableSearchResults').scrollIntoView({ behavior: 'smooth', block: 'center' });
                document.getElementById('tableSearchResults').focus();
            }
        });
    </script>

    <%-- JS: STORE ACTIVE TABLE IN LOCAL --%>
    <script>
        // Function to store the active tab in localStorage
        function storeActiveTab(tabId) {
            localStorage.setItem('activeTab', tabId);
        }

        // Retrieve the active tab from localStorage and activate it
        document.addEventListener('DOMContentLoaded', function () {
            var activeTab = localStorage.getItem('activeTab');

            if (activeTab) {
                // Find the tab link and trigger a click to activate it
                var tabToActivate = document.querySelector(`a[href='${activeTab}']`);
                if (tabToActivate) {
                    var tab = new bootstrap.Tab(tabToActivate);
                    tab.show();
                }
            } else {
                // If no active tab is found in localStorage, fallback to default behavior
                var defaultTab = document.querySelector('a.nav-link.active');
                if (defaultTab) {
                    var tab = new bootstrap.Tab(defaultTab);
                    tab.show();
                }
            }

            // Add event listeners to each tab to store the active tab on click
            var tabs = document.querySelectorAll('.nav-link');
            tabs.forEach(function (tab) {
                tab.addEventListener('shown.bs.tab', function (event) {
                    storeActiveTab(event.target.getAttribute('href'));
                });
            });
        });
    </script>


    <!-- JavaScript for Auto Formatting Date on Focus -->
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script>
        function formatDateInput(inputField) {
            inputField.addEventListener("input", function () {
                let input = this.value.replace(/\D/g, ''); // Remove non-numeric characters

                if (input.length > 2) input = input.substring(0, 2) + '/' + input.substring(2);
                if (input.length > 5) input = input.substring(0, 5) + '/' + input.substring(5, 10);

                this.value = input;
            });
        }

        function validateDateFormat(dateStr) {
            // Regex: dd/mm/yyyy
            const regex = /^(0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[0-2])\/\d{4}$/;
            return regex.test(dateStr);
        }

        function bindDateEvents(inputField) {
            inputField.addEventListener("focus", function () {
                formatDateInput(this);
            });

            inputField.addEventListener("blur", function () {
                const value = this.value.trim();
                if (value !== "" && !validateDateFormat(value)) {
                    alert("Invalid date format. Please use dd/mm/yyyy.");
                    setTimeout(() => this.focus(), 0); // bring focus back after alert closes
                }
            });

        }

        document.addEventListener("DOMContentLoaded", function () {
            document.querySelectorAll(".date-input").forEach(function (inputField) {
                bindDateEvents(inputField);
            });

            // ✅ Fix for UpdatePanel - Rebind after async postbacks
            if (typeof Sys !== "undefined" && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                    document.querySelectorAll(".date-input").forEach(function (inputField) {
                        bindDateEvents(inputField);
                    });
                });
            }
        });
    </script>

    <%-- JS: PRINT GRIDVIEW --%>
    <script>
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            if (window.shouldPrintGrid) {
                window.shouldPrintGrid = false;  // Reset the flag
                printGrid();
            }
        });

        // Function to explicitly trigger printing when called from server-side
        function triggerPrintGrid() {
            window.shouldPrintGrid = true; // Set flag to indicate that printing should occur
        }


        function printGrid() {
            setTimeout(function () {
                var grid = document.getElementById("<%= gridview1.ClientID %>");
                if (!grid) {
                    alert("Data is not present.");
                    return;
                }

                // Function to fetch values from GridView header row
                var getFieldValue = (headerText) => {
                    var headers = grid.getElementsByTagName("th");
                    for (var i = 0; i < headers.length; i++) {
                        if (headers[i].innerText.trim() === headerText) {
                            var cell = grid.rows[1]?.cells[i]; // Get first data row
                            return cell ? cell.innerText.trim() : 'N/A';
                        }
                    }
                    return 'N/A';
                };

                var printWindow = window.open('', '', 'height=800,width=1000');

                // Construct the print layout
                printWindow.document.write(`
<html>
<head>
<style>
body {
font-family: Arial, sans-serif;
padding: 20px;
font-size: 12px;
}
.header-table {
width: 100%;
border-collapse: collapse;
}
.header-table td {
font-size: 14px;
padding: 4px;
}
.transaction-table {
width: 100%;
border-collapse: collapse;
margin-top: 10px;
}
.transaction-table th, .transaction-table td {
border: 1px solid black;
padding: 8px;
text-align: left;
font-size: 12px;
}
.transaction-table th {
background-color: #f2f2f2;
font-weight: bold;
}
.total-row td {
font-weight: bold;
text-align: right;
}
.title {
text-align: center;
font-weight: bold;
font-size: 16px;
margin-bottom: 10px;
}
</style>
</head>
<body>
<table class="header-table">
<tr>
<td><strong>Name:</strong></td>
<td>${getFieldValue('Investor Name')}</td>
<td><strong>Tran. Date:</strong></td>
<td>${getFieldValue('Cheque Date')}</td>
</tr>
<tr>
<td><strong>Address:</strong></td>
<td colspan="3">${getFieldValue('Address1')}, ${getFieldValue('Address2')}</td>
</tr>
<tr>
<td><strong>Phone:</strong></td>
<td>${getFieldValue('Mobile')}</td>
</tr>
<tr>
<td><strong>Email:</strong></td>
<td>${getFieldValue('Email')}</td>
</tr>
</table>

<div class="title">ALL TRANSACTIONS FOR DATE: ${getFieldValue('Cheque Date')}</div>

<table class="transaction-table">
<thead>
<tr>
<th>Sr. No.</th>
<th>Tran No.</th>
<th>Investor Name</th>
<th>Scheme Name</th>
<th>App. No.</th>
<th>Pay Mode</th>
<th>Cheque/DD No. Date</th>
<th>Bank Name</th>
<th>Amount (Rs.)</th>
</tr>
</thead>
<tbody>`);

                // Fetch GridView data
                var rows = grid.getElementsByTagName("tr");
                var totalAmount = 0;
                for (var i = 1; i < rows.length; i++) { // Start from 1 to skip the header row
                    var cells = rows[i].getElementsByTagName("td");
                    if (cells.length === 0) continue;

                    var tranNo = cells[0].innerText.trim();        // TRAN_CODE
                    var investorName = cells[1].innerText.trim();  // INV
                    var schemeName = cells[18].innerText.trim();   // SCHMF (Scheme Name)
                    var appNo = cells[9].innerText.trim();         // APP_NO
                    var payMode = cells[20].innerText.trim();      // PAYMENT_MODE
                    var chequeNo = cells[14].innerText.trim();     // CHEQUE_NO
                    var chequeDate = cells[15].innerText.trim();   // CHEQUE_DATE
                    var bankName = cells[12].innerText.trim();     // BANK_NAME
                    var amount = parseFloat(cells[16].innerText.trim()) || 0; // AMOUNT
                    totalAmount += amount;

                    printWindow.document.write(`
<tr>
<td>${i}</td>
<td>${tranNo}</td>
<td>${investorName}</td>
<td>${schemeName}</td>
<td>${appNo}</td>
<td>${payMode}</td>
<td>${chequeNo} / ${chequeDate}</td>
<td>${bankName}</td>
<td>${amount.toFixed(2)}</td>
</tr>`);
                }

                // Add the total row
                printWindow.document.write(`
</tbody>
<tfoot>
<tr class="total-row">
<td colspan="8">Total:</td>
<td>${totalAmount.toFixed(2)}</td>
</tr>
</tfoot>
</table>
</body>
</html>`);

                printWindow.document.close();
                setTimeout(() => {
                    printWindow.print();
                }, 500);
            }, 1000); // Delay ensures gridview1 is fully loaded
        }
    </script>

    <%-- JS: UPDATE PANEL LOADER --%>
    <script>
        // Get the PageRequestManager instance
        var prm = Sys.WebForms.PageRequestManager.getInstance();

        // Show loader when request starts
        prm.add_beginRequest(function () {
            document.getElementById("updateProgress").style.display = "flex";
        });

        // Hide loader when request completes
        prm.add_endRequest(function () {
            document.getElementById("updateProgress").style.display = "none";
        });

    </script>

    <%-- JS: SIP STP SCRIPT --%>
    <script type="text/javascript">
        function initializeSipStpScript() {
            const ddlSipStp = document.getElementById('<%= ddlSipStp.ClientID %>');
            const transactionType = document.getElementById('<%= transactionType.ClientID %>');
            const transactionTypeValue = transactionType ? transactionType.value : '';

            const installmentTypeContainer = document.getElementById('installmentTypeContainer');
            const radioButtonsContainer = document.getElementById('radioButtonsContainer');
            const sipTypeContainer = document.getElementById('sipTypeContainer');
            const sipTypeContainerdropdown = document.getElementById('<%= siptype.ClientID %>');
            const freshRadioButton = document.getElementById('<%= fresh.ClientID %>');
            const sipAmountContainer = document.getElementById('sipAmountContainer');
            const sipamounttextbox = document.getElementById('<%= sipamount.ClientID %>');
            const hdnSipStpValue = document.getElementById('<%= hdnSipStpValue.ClientID %>'); // Hidden field to store value
            var frequencyDropdown = document.getElementById('<%= ddlFrequency.ClientID %>');
            var installmentsTextBox = document.getElementById('<%= txtInstallmentsNos.ClientID %>');
            var rdo99 = document.getElementById('<%= rdo99Years.ClientID %>');
            var installTypeRegular = document.getElementById('installmentTypeContainer');
            var sipstartdate = document.getElementById('<%= txtSIPStartDate.ClientID %>');
            var sipenddate = document.getElementById('<%= txtSIPEndDate.ClientID %>');
            // Switch fields  
            const formSwitchFolio = document.getElementById('<%= formSwitchFolio.ClientID %>');
            const formSwitchScheme = document.getElementById('<%= formSwitchScheme.ClientID %>');
            const btnSearchFormSwitchScheme = document.getElementById('<%= btnSearchFormSwitchScheme.ClientID %>');

            // Restore dropdown selection and switch fields from sessionStorage
            if (transactionType && transactionType.value === "PURCHASE") {
                if (sessionStorage.getItem("selectedSipStp")) {
                    ddlSipStp.value = sessionStorage.getItem("selectedSipStp");
                }
            }

            restoreSwitchFields(); // Restore switch fields after postback

            // Apply correct UI state based on saved selection
            toggleFields(ddlSipStp.value);

            // Prevent duplicate event listeners
            ddlSipStp.removeEventListener('change', handleDropdownChange);
            ddlSipStp.addEventListener('change', handleDropdownChange);

            function handleDropdownChange() {
                const selectedValue = ddlSipStp.value;
                hdnSipStpValue.value = selectedValue; // Store in hidden field
                sessionStorage.setItem("selectedSipStp", selectedValue); // Store in sessionStorage
                toggleFields(selectedValue);
            }

            // Function to toggle fields based on selection
            function toggleFields(selectedValue) {

                hideAll(); // Hide all sections initially

                if (transactionTypeValue === 'SWITCH IN') {
                    enableSwitchFields(true);
                }
                else if (selectedValue === 'REGULAR') {
                    frequencyDropdown.disabled = true;
                    installmentsTextBox.disabled = true;
                    rdo99.disabled = true;
                    sipstartdate.disabled = true;
                    sipenddate.disabled = true;
                    installTypeRegular.style.display = 'block';
                    // Clear values
                    freshRadioButton.checked = true;
                    sipenddate.value = "";
                    sipTypeContainerdropdown.selectedIndex = 0;
                    sipamounttextbox.value = ""; 
                    frequencyDropdown.selectedIndex = 0; // Reset dropdown
                    installmentsTextBox.value = "";      // Clear input
                    rdo99.checked = false;

                    enableSwitchFields(false);
                } else if (selectedValue === 'SIP') {
                    frequencyDropdown.disabled = false;
                    installmentsTextBox.disabled = false;
                   // sipenddate.disabled = false;
                    sipstartdate.disabled = false;
                    radioButtonsContainer.style.display = 'block';
                    sipTypeContainer.style.display = 'block';
                    sipAmountContainer.style.display = 'block';
                    rdo99.disabled = false;
                    enableSwitchFields(false);
                } else if (selectedValue === 'STP') {
                    frequencyDropdown.disabled = false;
                    installmentsTextBox.disabled = false;
                   // sipenddate.disabled = false;
                    sipstartdate.disabled = false;
                    sessionStorage.setItem("formSwitchFolio", formSwitchFolio.value);
                    sessionStorage.setItem("formSwitchScheme2", formSwitchScheme.value);
                    // alert(formSwitchScheme.value);
                    sipTypeContainerdropdown.selectedIndex = 0;
                    rdo99.disabled = false;
                    sipamounttextbox.value = ""; 
                    freshRadioButton.checked = true;
                    enableSwitchFields(true); // Enable Switch fields when "STP" is selected
                }
            }

            // Function to hide all sections
            function hideAll() {
                installmentTypeContainer.style.display = 'none';
                radioButtonsContainer.style.display = 'none';
                sipTypeContainer.style.display = 'none';
                sipAmountContainer.style.display = 'none';
            }

            // Function to enable/disable switch folio and scheme fields
            function enableSwitchFields(isSwitchIn) {
                formSwitchFolio.readOnly = !isSwitchIn;
                formSwitchScheme.readOnly = !isSwitchIn;
                btnSearchFormSwitchScheme.disabled = !isSwitchIn;

                if (isSwitchIn) {
                    sessionStorage.setItem("formSwitchFolio", formSwitchFolio.value);
                    sessionStorage.setItem("formSwitchScheme", formSwitchScheme.value);
                } else {
                    sessionStorage.setItem("formSwitchFolio", '');
                    sessionStorage.setItem("formSwitchScheme", '');
                    formSwitchFolio.value = '';  // Clear values when disabled
                    formSwitchScheme.value = '';
                }

                // Update UI appearance based on enabled/disabled state
                formSwitchFolio.style.filter = isSwitchIn ? "none" : "blur(3px)";
                formSwitchFolio.style.pointerEvents = isSwitchIn ? "auto" : "none";

                formSwitchScheme.style.filter = isSwitchIn ? "none" : "blur(3px)";
                formSwitchScheme.style.pointerEvents = isSwitchIn ? "auto" : "none";
            }

            // Function to save switch field values in sessionStorage
            function saveSwitchFields() {
                sessionStorage.setItem("formSwitchFolio", formSwitchFolio.value);
                sessionStorage.setItem("formSwitchScheme2", formSwitchScheme.value);
            }

            // Function to restore switch field values after postback
            function restoreSwitchFieldsss() {
                const formSwitchFolio = document.getElementById('<%= formSwitchFolio.ClientID %>');
            const formSwitchScheme = document.getElementById('<%= formSwitchScheme.ClientID %>');

                formSwitchFolio.value = sessionStorage.getItem("formSwitchFolio") || formSwitchFolio.value;
                formSwitchScheme.value = sessionStorage.getItem("formSwitchScheme2") || formSwitchScheme.value;

            }

            // Save switch field values when changed
            formSwitchFolio.addEventListener("input", saveSwitchFields);
            formSwitchScheme.addEventListener("input", saveSwitchFields);

        }


        // Run the script on page load
        document.addEventListener("DOMContentLoaded", function () {
            initializeSipStpScript();

        });

        // Re-run the script after every UpdatePanel update
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            initializeSipStpScript();

        });
    </script>

    <%-- JQUERY: ELEMENT PREFERENCE  --%>
    <script>
        $('body').on('keydown', 'input, select, button', function (e) {
            if (e.key === "Enter") {
                var self = $(this), form = self.parents('form:eq(0)'), focusable, next;
                focusable = form.find('input,a,select,button,textarea').filter(':visible');
                next = focusable.eq(focusable.index(this) + 1);

                // Check if the focused element is a button (including asp:Button rendered as <input type="submit">)
                if (self.is('button') || self.is('input[type="submit"]')) {
                    // Trigger the button's click event (either <button> or <input type="submit">)
                    self.click();
                } else if (next.length) {
                    // Move focus to the next element (like pressing Tab)
                    next.focus();
                } else {
                    // If no next element, submit the form
                    form.submit();
                }

                return false; // Prevent the default Enter key action (form submission)
            }
        });
    </script>




</asp:Content>
