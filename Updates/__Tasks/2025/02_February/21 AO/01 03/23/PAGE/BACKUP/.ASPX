<%@ Page Title=" Account Opening" Language="C#" AutoEventWireup="true" CodeBehind="AccountOpening.aspx.cs" Inherits="WM.Masters.AccountOpening" MasterPageFile="~/vmSite.Master" EnableEventValidation="false" MaintainScrollPositionOnPostback="true"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link rel="shortcut icon" href="../assets/images/favicon.png" />
    <link rel="stylesheet" href="../assets/vendors/mdi/css/materialdesignicons.min.css">
    <link rel="stylesheet" href="../assets/vendors/ti-icons/css/themify-icons.css">
    <link rel="stylesheet" href="../assets/vendors/css/vendor.bundle.base.css">
    <link rel="stylesheet" href="../assets/vendors/font-awesome/css/font-awesome.min.css">
    <link rel="stylesheet" href="../assets/vendors/bootstrap-datepicker/bootstrap-datepicker.min.css">
    <link rel="stylesheet" href="../assets/css/style.css">


    <script>
        document.addEventListener("keydown", function (event) {
            if (event.key === "Enter") {
                //event.preventDefault(); // Prevent form submission

                // Get all focusable elements on the page
                let focusableElements = Array.from(document.querySelectorAll("input, select, textarea, button, a"))
                    .filter(el => !el.disabled && el.tabIndex !== -1); // Ignore disabled elements

                let activeElement = document.activeElement; // Get the currently focused element
                let index = focusableElements.indexOf(activeElement);

                if (index > -1 && index < focusableElements.length - 1) {
                    if ((focusableElements[index].tagName) != "TEXTAREA") {
                        event.preventDefault(); // Prevent form submission

                        focusableElements[index + 1].focus(); // Move to the next element
                    }
                    else {

                    }
                }
            }
        });
    </script>

    <%-- UI BASED VALIDATION --%>
    <script>
        document.addEventListener("DOMContentLoaded", function () {
            // Select all elements with the 'form-control' class
            var textInputs = document.querySelectorAll('.form-control');

            textInputs.forEach(function (input) {
                // Convert text to uppercase while typing
                input.addEventListener("input", function () {
                    this.value = this.value.toUpperCase();
                });


            });
        });


        function validateAadharNumberLast(inputElement, maxLength) {
            let value = inputElement.value.trim(); // Remove leading/trailing spaces

            // Allow empty field (no validation required)
            if (value === '') {
                inputElement.setCustomValidity('');
                return;
            }

            // Remove any non-numeric characters
            value = value.replace(/\D/g, '');

            // Limit to the specified length (12 digits)
            if (value.length > maxLength) {
                value = value.slice(0, maxLength);
            }

            // Set the sanitized value back
            inputElement.value = value;

            // Validate: If filled, ensure exactly 12 digits and not all zeros
            if (value.length === maxLength && value !== '000000000000') {
                inputElement.setCustomValidity(''); // Valid
            } else {
                inputElement.setCustomValidity('Aadhaar must be a 12-digit number and not all zeros.');
            }
        }

    </script>

    <style>
        .form-control {
            text-transform: uppercase;
        }


        html {
            scroll-behavior: smooth;
        }

        .modal {
            display: none;
            position: fixed;
            z-index: 1;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            overflow: auto;
            background-color: rgba(0,0,0,0.4)
            ;
        }

        .modal-content {
            background-color: #fefefe;
            margin:10% 5%;

            padding: 20px;
            border: 1px solid #888;
            width: 90%;
        }

        .close {
            color: #aaa;
            float: right;
            font-size: 28px;
            font-weight: bold;
        }

            .close:hover, .close:focus {
                color: black;
                text-decoration: none;
                cursor: pointer;
            }
    </style>

    <script type="text/javascript">


        function validatePanInput(input) {
            var regex = /^[a-zA-Z0-9]*$/;

            //var regex = /^[A-Z]{5}[0-9]{0,4}[A-Z]?$/;
            var lastChar = input.value.slice(-1);

            // Check if the last character is valid
            if (!regex.test(lastChar)) {
                alert("The character '" + lastChar + "' is not allowed. Only alphabetic and numeric characters are permitted.");
                input.value = input.value.slice(0, -1); // Remove the last character
                input.focus();
            }
        }


        function validateMobileNumber(input, maxLength = 10) {
            var regex = /^[0-9]*$/;  // Regular expression to allow only numeric characters

            // Check if the length exceeds max length
            if (input.value.length > maxLength) {
                //  alert("This number cannot exceed " + maxLength + " digits.");
                input.value = input.value.slice(0, maxLength);  // Trim the input to max length
                return;
            }

            // Check if the last character is valid (numeric)
            var lastChar = input.value.slice(-1);
            if (!regex.test(lastChar)) {
                //  alert("The character '" + lastChar + "' is not allowed. Only numeric characters are permitted.");
                input.value = input.value.slice(0, -1); // Remove the last character
                input.focus();
            }
        }


        function validateVarchar(input, maxLength = 255) {
            var regex = /^[A-Za-z\s]*$/;  // Regular expression to allow only alphabetic characters and spaces

            // Check if the length exceeds max length
            if (input.value.length > maxLength) {
                // Trim the input to max length
                input.value = input.value.slice(0, maxLength);
                return;
            }

            // Check if the last character is valid (alphabetic or space)
            var lastChar = input.value.slice(-1);
            if (!regex.test(lastChar)) {
                // Remove the last character if it's invalid
                input.value = input.value.slice(0, -1);
                input.focus();
            }
        }

        function validateAadharNumber(input, maxLength = 10) {
            var regex = /^[0-9]*$/;  // Regular expression to allow only numeric characters

            // Check if the length exceeds max length
            if (input.value.length > maxLength) {
                //   alert("The aadhar number cannot exceed " + maxLength + " digits.");
                input.value = input.value.slice(0, maxLength);  // Trim the input to max length
                return;
            }

            // Check if the last character is valid (numeric)
            var lastChar = input.value.slice(-1);
            if (!regex.test(lastChar)) {
                //   alert("The character '" + lastChar + "' is not allowed. Only numeric characters are permitted.");
                input.value = input.value.slice(0, -1); // Remove the last character
                input.focus();
            }
        }

        function validateFaxNumber(input, maxLength = 20) {
            var regex = /^[0-9\s\-]*$/;  // Regular expression to allow digits, spaces, and hyphens

            // Check if the length exceeds max length
            if (input.value.length > maxLength) {
                //   alert("The fax number cannot exceed " + maxLength + " characters.");
                input.value = input.value.slice(0, maxLength);  // Trim the input to max length
                return;
            }

            // Check if the last character is valid (numeric, space, or hyphen)
            var lastChar = input.value.slice(-1);
            if (!regex.test(lastChar)) {
                //   alert("The character '" + lastChar + "' is not allowed. Only numeric digits, spaces, and hyphens are permitted.");
                input.value = input.value.slice(0, -1); // Remove the last character
                input.focus();
            }
        }


        function formatDOBInput(input) {
            let value = input.value.replace(/\D/g, ""); // Remove non-numeric characters

            if (value.length > 8) value = value.slice(0, 8); // Limit to 8 characters (ddMMyyyy)

            let formattedValue = "";
            if (value.length >= 2) {
                formattedValue += value.substring(0, 2) + "/"; // Day
            } else {
                formattedValue += value;
            }
            if (value.length >= 4) {
                formattedValue += value.substring(2, 4) + "/"; // Month
            } else if (value.length > 2) {
                formattedValue += value.substring(2);
            }
            if (value.length > 4) {
                formattedValue += value.substring(4, 8); // Year
            }

            input.value = formattedValue;
        }


        function onChangeValidateFamActive(input) {
            let dobString = input.value; // Get the entered date (dd-MM-yyyy)

            if (dobString) {
                // Convert "dd-MM-yyyy" to Date Object
                let parts = dobString.split("/");
                let dob = new Date(parts[2], parts[1] - 1, parts[0]); // yyyy, mm-1, dd

                if (!isNaN(dob.getTime())) {
                    let ageInMonths = calculateAgeInMonths(dob);

                    // Enable/Disable Guardian Fields
                    enableGuardianFields(ageInMonths < 216); // Less than 18 years (216 months)
                } else {
                    alert("Invalid Date. Please select a valid date." + dob);
                    input.value = ""; // Clear invalid input
                }
            }
        }

        function onChangeValidateFamActiveHead(input) {
            let dobString = input.value; // Get the entered date (dd-MM-yyyy)

            if (dobString) {
                // Convert "dd-MM-yyyy" to Date Object
                let parts = dobString.split("/");
                let dob = new Date(parts[2], parts[1] - 1, parts[0]); // yyyy, mm-1, dd

                if (!isNaN(dob.getTime())) {
                    let ageInMonths = calculateAgeInMonths(dob);

                    // Enable/Disable Guardian Fields
                    enableGuardianFieldsHead(ageInMonths < 216); // Less than 18 years (216 months)
                } else {
                    alert("Invalid Date. Please select a valid date." + dob);
                    input.value = ""; // Clear invalid input
                }
            }
        }

        function calculateAgeInMonths(dob) {
            let today = new Date();
            let ageInMonths = (today.getFullYear() - dob.getFullYear()) * 12 + (today.getMonth() - dob.getMonth());
            return ageInMonths;
        }

        function enableGuardianFields(enable) {
            document.querySelectorAll(".guardianField").forEach(field => {
                field.disabled = !enable;
                if (!enable) {
                    field.value = "";
                }
            });
        }

        function enableGuardianFieldsHead(enable) {
            document.querySelectorAll(".guardianFieldHead").forEach(field => {
                field.disabled = !enable;

                if (!enable) {
                    field.value = "";
                }
            });
        }
    </script>

    <script>
        document.addEventListener("DOMContentLoaded", function () {
            // Select all text input fields
            var textInputs = document.querySelectorAll('input[type="text"], textarea');

            textInputs.forEach(function (input) {
                // Convert text to uppercase while typing
                input.addEventListener("input", function () {
                    this.value = this.value.toUpperCase();
                });


            });
        });
    </script>

    <%-- Loader styles --%>
    <style>
        #loader {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(255, 255, 255, 0.8);
            display: flex;
            justify-content: center;
            align-items: center;
            z-index: 9999;
        }

        .spinner {
            border: 6px solid #f3f3f3;
            border-top: 6px solid #3498db;
            border-radius: 50%;
            width: 50px;
            height: 50px;
            animation: spin 1s linear infinite;
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

    <!-- Loader -->
    <!-- Server-Side Loader (For Full Page Requests Like Exporting Excel) -->
    <div id="serverLoader" runat="server" style="display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; background-color: rgba(255, 255, 255, 0.7); z-index: 1000; text-align: center;">
        <div style="position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%);">
            <div class="spinner-border" role="status">
                <span class="sr-only">Processing...</span>
            </div>
        </div>
    </div>

    <!-- Client-Side Loader (For AJAX Postbacks) -->
    <div id="clientLoader" style="display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; background-color: rgba(0, 0, 0, 0.5); z-index: 2000; text-align: center;">
        <div style="position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%);">
            <div class="spinner-border text-light" role="status">
                <span class="sr-only">Loading...</span>
            </div>
        </div>
    </div>


    <script type="text/javascript">
        // Show / Hide Server Loader (Full Page Requests)
        var postBackInProgress = false;
        function showServerLoader() {
            if (!postBackInProgress) {
                postBackInProgress = true;
                document.getElementById('serverLoader').style.display = 'block';
            }
        }


        function hideServerLoader() {
            document.getElementById('serverLoader').style.display = 'none';
        }

        // Show / Hide Client Loader (For AJAX Requests in UpdatePanel)
        function showClientLoader() {
            document.getElementById('clientLoader').style.display = 'block';
        }

        function hideClientLoader() {
            document.getElementById('clientLoader').style.display = 'none';
        }

        window.onload = function () {
            postBackInProgress = false;
            document.getElementById('serverLoader').style.display = 'none';
        };


        // Automatically hide client loader after AJAX postbacks
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function () {
            showClientLoader();
        });

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            hideClientLoader();
        });
    </script>

    <div class="page-header">
        <h3 class="page-title">Bajaj Capital Account Opening Form</h3>
    </div>

    <div class="row">
        <div class="col-md-12 grid-margin stretch-card">
            <div class="card">
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-6">
                            <h2 class="card-title">Client Registration</h2>
                        </div>
                        <div class="col-md-6 text-end">
                            <asp:Button ID="ClientListClientMasterViewModel" CssClass="btn btn-primary" runat="server" Text="View Client" OnClientClick="openClientListModal(); return false;" />
                            <%--<asp:Button ID="InvestorListViewButton" CssClass="btn btn-primary" runat="server" Text="View Investor" OnClientClick="openInvestorListModal(); return false;" />--%>
                            <asp:Button ID="InvestorListViewButton" CssClass="btn btn-primary" 
                                runat="server" Text="View Investor" 
                                OnClientClick="sessionStorage.setItem('AC_INV_FIND', 'true'); location.href='InvestorMasterSearch.aspx'; return false;"

 />

                        </div>


                        <%-- modalState_ClientListModal --%>
                        <script>
                            const ClientModalStateKey = 'modalState_ClientListModal';

                            function openClientListModal() {
                                const modal = document.getElementById("ClientListClientListModal");
                                if (modal) {
                                    modal.style.display = "block";
                                    localStorage.setItem(ClientModalStateKey, 'open');
                                }
                            }

                            function closeClientListModal() {
                                const modal = document.getElementById("ClientListClientListModal");
                                if (modal) {
                                    modal.style.display = "none";
                                    localStorage.setItem(ClientModalStateKey, 'closed');
                                }
                            }

                            function autoCloseClientListModal() {
                                setTimeout(function () {
                                    closeClientListModal();
                                }, 1);
                            }

                            document.addEventListener('DOMContentLoaded', function () {
                                const modalState = localStorage.getItem(ClientModalStateKey);
                                if (modalState === 'open') {
                                    openClientListModal();
                                }

                                const closeButton = document.getElementById("ClientListbtnExit");
                                if (closeButton) {
                                    closeButton.addEventListener('click', closeClientListModal);
                                }
                            });
                        </script>
                        
                        <%-- ClientListClientListModal --%>
                        <div id="ClientListClientListModal" class="modal" role="dialog" style="z-index: 99">
                            <div class="modal-content">
                                <h2 class="page-title">Client List</h2>
                                <div class="container mt-1">
                                    <div class="row">
                                            <div class="col-md-2">
                                                <label for="ClientListbranch" class="form-label">Branch</label>
                                                <asp:DropDownList ID="ClientListbranch" runat="server" CssClass="form-select"></asp:DropDownList>
                                            </div>
                                            <div class="col-md-2">
                                                <label for="ClientListmobile" class="form-label">Mobile</label>
                                                <asp:TextBox ID="ClientListmobile" oninput="validateMobileNumber(this, 10)" runat="server" CssClass="form-control phone-input" />
                                            </div>
                                            <div class="col-md-2">
                                                <label for="ClientListcity" class="form-label">City</label>
                                                <asp:DropDownList ID="ClientListddlMailingCityListV" runat="server" CssClass="form-select"></asp:DropDownList>
                                            </div>
                                            <div class="col-md-2">
                                                <label for="ClientListphone" class="form-label">Phone</label>
                                                <asp:TextBox ID="ClientListphone" runat="server" CssClass="form-control phone-input" MaxLength="15" />
                                            </div>
                                            <div class="col-md-2">
                                                <label for="ClientListclientCode" class="form-label">Client Code - AH</label>
                                                <asp:TextBox ID="ClientListclientCode" runat="server" MaxLength="10" CssClass="form-control" />
                                            </div>
                                            <div class="col-md-2">
                                                <label for="ClientListbusinessCd" class="form-label">Pan Card</label>
                                                <asp:TextBox ID="ClientListbusinessCd" runat="server"
                                                    MaxLength="10"
                                                    placeholder="Enter PAN (ABCDE1234F)"
                                                    pattern="[A-Z]{5}[0-9]{4}[A-Z]{1}"
                                                    title="Enter PAN in format: ABCDE1234F"
                                                    oninput="validatePanInput2(this)"
                                                    CssClass="form-control" />
                                            </div>
                                            <div class="col-md-4">
                                                <label for="ClientListclientName" class="form-label">Client Name</label>
                                                <asp:TextBox ID="ClientListclientName" runat="server" MaxLength="100" CssClass="form-control" />


                                            </div>
                                            <div class="col-md-8">
                                               <%-- <div class="d-flex align-items-center mt-4 gap-2 flex-wrap end-0" style="margin-left: 90px">--%>
                    <div class="col-md-12 d-flex justify-content-end align-items-end mt-4">

                                                    <asp:Button ID="ClientListbtnSearch" CausesValidation="false" runat="server" CssClass="btn btn-primary me-2" Text="Search" OnClick="ClientListSearch_Click" />
                                                    <asp:Button ID="ClientListbtnReset" CausesValidation="false" runat="server" CssClass="btn btn-outline-primary me-2" Text="Reset" OnClick="ClientListReset_Click" />
                                                    <asp:Button ID="ClientListbtnExit" CausesValidation="false" runat="server" CssClass="btn btn-outline-primary" Text="Exit" OnClientClick="closeClientListModal(); return false;" />
                                                </div>
                                            </div>
                                        <br />
                                        <br />

                                        <div class="row g-3">
                                            <h5 class="card-title">Client Details</h5>
                                            <asp:Label ID="ClientListlblMessage" runat="server" Text="" />
                                            <div class="table-responsive" style="overflow-y: auto; overflow-x: auto; height: 350px;">
                                                <asp:GridView ID="ClientListclientGridView" runat="server" CssClass="table table-hover" AutoGenerateColumns="false" OnRowCommand="gvClientSearch_RowCommand">
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
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <%-- modalState_InvestorListModal --%>
                        <script>
                            const InvestorModalStateKey = 'modalState_InvestorListModal';

                            function openInvestorListModal() {
                                const modal = document.getElementById("InvestorListModal");
                                if (modal) {
                                    modal.style.display = "block";
                                    localStorage.setItem(InvestorModalStateKey, 'open');
                                }
                            }

                            function closeInvestorListModal() {
                                const modal = document.getElementById("InvestorListModal");
                                if (modal) {
                                    modal.style.display = "none";
                                    localStorage.setItem(InvestorModalStateKey, 'closed');
                                }
                            }

                            function autoCloseInvestorListModal() {
                                setTimeout(function () {
                                    closeInvestorListModal();
                                }, 1);
                            }

                            document.addEventListener('DOMContentLoaded', function () {
                                const modalState = localStorage.getItem(InvestorModalStateKey);
                                if (modalState === 'open') {
                                    openInvestorListModal();
                                }

                                const closeButton = document.getElementById("InvestorListbtnExit");
                                if (closeButton) {
                                    closeButton.addEventListener('click', closeInvestorListModal);
                                }
                            });
                        </script>

                        <%-- InvestorListModal --%>
                        <div id="InvestorListModal" class="modal" style="z-index: 99">
                            <div class="modal-content">
                                <h2 class="page-title">Investor List</h2>
                                <div class="container mt-1">
                                    <div class="row">
                                        <div class="row g-3">
                                            <!-- Branch Dropdown -->
                                            <div class="col-md-2">
                                                <label for="InvestorBranchDropDown" class="form-label">Branch</label>
                                                <asp:DropDownList ID="InvestorBranchDropDown" runat="server" ClientIDMode="Static"
                                                    CssClass="form-select" OnSelectedIndexChanged="InvestorBranchDropDown_SelectedIndexChanged"
                                                    AutoPostBack="true" ToolTip="Select a Branch">
                                                </asp:DropDownList>

                                                <%-- <asp:DropDownList runat="server" ID="InvestorBranchDropDown" ToolTip="Select a Branch" CssClass="form-select" >
    <asp:ListItem Value="0">Select Investor</asp:ListItem>
</asp:DropDownList>--%>
                                            </div>

                                            <!-- RM Dropdown -->
                                            <div class="col-md-2">
                                                <label for="InvestorListBranchRM" class="form-label">RM</label>
                                                <asp:DropDownList ID="InvestorListBranchRM" runat="server" CssClass="form-select"
                                                    ToolTip="Select a Relationship Manager">
                                                </asp:DropDownList>
                                            </div>

                                            <!-- DOB Input -->
                                            <div class="col-md-2">
                                                <label for="InvestorListDOB">Date of Birth</label>
                                                <div class="input-group date">
                                                    <asp:TextBox ID="InvestorListDOB" runat="server" CssClass="form-control datepicker"
                                                        ToolTip="Enter Date of Birth" aria-label="Date of Birth" />
                                                    <div class="input-group-append">
                                                        <span class="input-group-text">
                                                            <i class="glyphicon glyphicon-th"></i>
                                                        </span>
                                                    </div>
                                                </div>



                                            </div>

                                             <script>
                                                 $(document).ready(function () {
                                                     // Initialize Date Picker inside the modal
                                                     $('.datepicker').datepicker({
                                                         format: 'dd/mm/yyyy',  // Set date format to DD/MM/YYYY
                                                         autoclose: true,       // Close picker after selecting a date
                                                         todayHighlight: true,  // Highlight today's date
                                                         endDate: new Date(),   // Disable future dates
                                                         container: '#InvestorListModal' // Ensure datepicker stays inside modal
                                                     });

                                                     // Close Date Picker on Modal Scroll
                                                     $('#InvestorListModal').on('scroll', function () {
                                                         $('.datepicker').datepicker('hide');
                                                     });

                                                     // Ensure modal opens properly
                                                     $('#InvestorListModal').modal({
                                                         backdrop: 'static',
                                                         keyboard: false
                                                     });
                                                 });

                                             </script> 

                                            
                                            <!-- Client Code -->
                                            <div class="col-md-2">
                                                <label for="InvestorListClientCode" class="form-label">Client Code</label>
                                                <asp:TextBox ID="InvestorListClientCode" runat="server" CssClass="form-control"
                                                    placeholder="e.g: 400000000" oninput="validateMobileNumber(this, 8)"
                                                    ToolTip="Enter Client Code" aria-label="Client Code" MaxLength="8" />
                                            </div>

                                            <!-- Client Name -->
                                            <div class="col-md-2">
                                                <label for="InvestorListClientName" class="form-label">Client Name</label>
                                                <asp:TextBox ID="InvestorListClientName" runat="server" CssClass="form-control"
                                                    ToolTip="Enter Client Name" aria-label="Client Name" MaxLength="100" />
                                            </div>

                                            <!-- Mobile -->
                                            <div class="col-md-2">
                                                <label for="InvestorMobileTextBox" class="form-label">Mobile</label>

                                                <asp:TextBox ID="InvestorMobileTextBox" runat="server"
                                                    CssClass="form-control phone-input"
                                                    ToolTip="Enter Mobile Number"
                                                    MaxLength="10"
                                                    oninput="validateMobileNumber(this, 10)"
                                                    title="10 digit mobile number"
                                                    aria-label="Mobile" />
                                            </div>

                                            <!-- City Dropdown -->
                                            <div class="col-md-2">
                                                <label for="InvestorCityDropDown" class="form-label">City</label>
                                                <asp:DropDownList ID="InvestorCityDropDown" runat="server" CssClass="form-select"
                                                    ToolTip="Select a City">
                                                </asp:DropDownList>
                                            </div>

                                            <!-- Phone -->
                                            <div class="col-md-2">
                                                <label for="InvestorPhoneTextBox" class="form-label">Phone</label>
                                                <asp:TextBox ID="InvestorPhoneTextBox" runat="server" CssClass="form-control phone-input"
                                                    oninput="validateMobileNumber(this, 15)"
                                                    ToolTip="Enter Phone Number" aria-label="Phone" />
                                            </div>

                                            <!-- Account Code -->
                                            <div class="col-md-2">
                                                <label for="InvestorListAccountCode" class="form-label">Account Code - Inv</label>
                                                <asp:TextBox ID="InvestorListAccountCode" runat="server" CssClass="form-control"
                                                    MaxLength="13"
                                                    oninput="validateMobileNumber(this, 15)"
                                                    ToolTip="Enter Account Code" aria-label="Account Code" />
                                            </div>

                                            <!-- PAN -->
                                            <div class="col-md-2">
                                                <label for="InvestorListPan" class="form-label">PAN</label>
                                                <asp:TextBox ID="InvestorListPan" CssClass="form-control"
                                                    runat="server"
                                                    MaxLength="10"
                                                    placeholder="Enter PAN (ABCDE1234F)"
                                                    pattern="[A-Z]{5}[0-9]{4}[A-Z]{1}"
                                                    title="Enter PAN in format: ABCDE1234F"
                                                    oninput="validatePanInput2(this)" aria-label="PAN" />
                                            </div>

                                            <!-- Investor Name -->
                                            <div class="col-md-4">
                                                <label for="InvestorListName" class="form-label">Investor Name</label>
                                                <asp:TextBox ID="InvestorListName" runat="server" CssClass="form-control"
                                                    MaxLength="100"
                                                    ToolTip="Enter Investor Name" aria-label="Investor Name" />
                                            </div>

                                            <!-- Address 1 -->
                                            <div class="col-md-4">
                                                <label for="InvestorListAdd1" class="form-label">Address 1</label>
                                                <asp:TextBox ID="InvestorListAdd1" runat="server" CssClass="form-control"
                                                    MaxLength="100"
                                                    ToolTip="Enter Address Line 1" aria-label="Address 1" />
                                            </div>

                                            <!-- Address 2 -->
                                            <div class="col-md-3">
                                                <label for="InvestorListAdd2" class="form-label">Address 2</label>
                                                <asp:TextBox ID="InvestorListAdd2" runat="server" CssClass="form-control"
                                                    MaxLength="100"
                                                    ToolTip="Enter Address Line 2" aria-label="Address 2" />
                                            </div>

                                            <!-- Buttons -->
                                            <div class="col-md-5 text-end">
                                                <div class="d-flex align-items-center mt-4 gap-2 flex-wrap end-0" style="margin-left: 90px">
                                                    <asp:Button ID="InvestorSearchButton" runat="server" CssClass="btn btn-primary" Text="Search"
                                                        OnClick="InvestorListSearch_Click" ToolTip="Search Investors" />
                                                    <asp:Button ID="InvestorResetButton" runat="server" CssClass="btn btn-outline-primary" Text="Reset"
                                                        OnClick="InvestorListReset_Click" ToolTip="Reset Fields" />
                                                    <asp:Button ID="InvestorExitButton" runat="server" CssClass="btn btn-outline-primary" Text="Exit"
                                                        OnClientClick="closeInvestorListModal(); return false;" ToolTip="Exit" />
                                                </div>
                                            </div>
                                        </div>


                                        <br />
                                        <br />
                                        <div class="row g-3">
                                            <h5 class="card-title">Investor Details</h5>
                                            <asp:Label ID="InvestorDetailsLabel" runat="server" Text="" />
                                            <div class="table-responsive" style="overflow-y: auto; overflow-x: auto; height: 350px;">


                                                <asp:GridView ID="InvestorDetailsGridView" runat="server"
                                                    CssClass="table table-hover table-striped"
                                                    AutoGenerateColumns="false"
                                                    EmptyDataText="No investors found."
                                                    OnRowCommand="gvInvestorSearch_RowCommand"
                                                    aria-label="Investor Search Results">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="#" ItemStyle-CssClass="column-width-50">
                                                            <ItemTemplate>
                                                                <asp:Label ID="InvestorListSrLabel" runat="server" Text='<%# Container.DisplayIndex + 1 %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField>
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="InvestorListSelectLinkButton" runat="server"
                                                                    CommandName="SelectRow"
                                                                    CommandArgument='<%# Eval("INV_CODE") %>'
                                                                    Text="Select" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>


                                                        <asp:TemplateField HeaderText="Investor Name" ItemStyle-CssClass="column-width-150">
                                                            <ItemTemplate>
                                                                <asp:Label ID="InvestorListNameLabel" runat="server" Text='<%# Eval("INVESTOR_NAME") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Address 1" ItemStyle-CssClass="column-width-150">
                                                            <ItemTemplate>
                                                                <asp:Label ID="InvestorListAddress1Label" runat="server" Text='<%# Eval("ADDRESS1") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Address 2" ItemStyle-CssClass="column-width-150">
                                                            <ItemTemplate>
                                                                <asp:Label ID="InvestorListAddress2Label" runat="server" Text='<%# Eval("ADDRESS2") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="City" ItemStyle-CssClass="column-width-100">
                                                            <ItemTemplate>
                                                                <asp:Label ID="InvestorListCityLabel" runat="server" Text='<%# Eval("CITY_ID") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Phone" ItemStyle-CssClass="column-width-100">
                                                            <ItemTemplate>
                                                                <asp:Label ID="InvestorListPhoneLabel" runat="server" Text='<%# Eval("PHONE") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Branch" ItemStyle-CssClass="column-width-100">
                                                            <ItemTemplate>
                                                                <asp:Label ID="InvestorListBranchCodeLabel" runat="server" Text='<%# Eval("BRANCH_CODE") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="RM " ItemStyle-CssClass="column-width-100">
                                                            <ItemTemplate>
                                                                <asp:Label ID="InvestorListRmCodeLabel" runat="server" Text='<%# Eval("RM_CODE") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Source ID" ItemStyle-CssClass="column-width-100">
                                                            <ItemTemplate>
                                                                <asp:Label ID="InvestorListSourceIdLabel" runat="server" Text='<%# Eval("SOURCE_ID") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Mobile" ItemStyle-CssClass="column-width-100">
                                                            <ItemTemplate>
                                                                <asp:Label ID="InvestorListMobileLabel" runat="server" Text='<%# Eval("MOBILE") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="PAN" ItemStyle-CssClass="column-width-100">
                                                            <ItemTemplate>
                                                                <asp:Label ID="InvestorListPanLabel" runat="server" Text='<%# Eval("PAN") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="DOB" ItemStyle-CssClass="column-width-100">
                                                            <ItemTemplate>
                                                                <asp:Label ID="InvestorListDobLabel" runat="server" Text='<%# Eval("DOB", "{0:dd-MMM-yyyy}") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>


                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>


                    </div>
                    <!-- Tabs Navigation -->
                    <ul class="nav nav-tabs wmTabs" id="wmTabs" role="tablist">
                        <li class="nav-item" role="presentation">
                            <a class="nav-link" id="basic_details_tab" data-bs-toggle="tab" href="#basic_details" role="tab" aria-controls="basic_details">Basic Details</a>
                        </li>
                        <li class="nav-item" role="presentation">
                            <a class="nav-link" id="other_details_tab" data-bs-toggle="tab" href="#other_details" role="tab" aria-controls="other_details">Other Details</a>
                        </li>
                    </ul>
                    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
                    <script>
                        $(document).ready(function () {
                            var activeTab = sessionStorage.getItem('activeTab');

                            if (!activeTab) {
                                // Default to Basic Details if no tab is stored
                                activeTab = "basic_details_tab";
                            }

                            // Remove existing active classes
                            $('.nav-link, .tab-pane').removeClass('active show');

                            // Add active classes based on sessionStorage
                            $('#' + activeTab).addClass('active');
                            $('#' + activeTab.replace('_tab', '')).addClass('show active');

                            // Store the active tab when clicked
                            $('.nav-link').on('click', function () {
                                var tabId = $(this).attr('id');
                                sessionStorage.setItem('activeTab', tabId);
                            });
                        });

                        // Clear sessionStorage when the page is closed
                        //$(window).on("unload", function () {
                        //    sessionStorage.removeItem("activeTab");
                        //});
</script>


                    <div class="tab-content wmTabsContent" id="wmTabsContent">
                        <div class="tab-pane fade show active" id="basic_details" role="tabpanel" aria-labelledby="basic_details_tab">
                            <div class="row g-3 mb-3">


                                <div class="col-md-3">
                                    <asp:Label runat="server" CssClass="form-label" Text="DT number <span class='text-danger'>*</span>" />
                                    <asp:TextBox runat="server" ID="txtDTNumber" CssClass="form-control border-dark" MaxLength="15" OnTextChanged="oneClientSearchByDT_Click" AutoPostBack="true" />
                                </div>
                                <div class="col-md-3 d-none">
                                    <asp:Button ID="btnDTNumber" runat="server" CssClass="btn btn-outline-primary mt-3" Style="width: 150px;" Text="Get by DT" OnClick="oneClientSearchByDT_Click" />
                                </div>
                                <div class="col-md-3">
                                    <asp:Label runat="server" CssClass="form-label" Text="Enter Guest Code" />
                                    <asp:TextBox runat="server" ID="txtGuestCode" CssClass="form-control" MaxLength="12" />
                                </div>
                                <div class="col-md-3 ">
                                    <asp:Button ID="btnGuestCode" runat="server" CssClass="btn btn-outline-primary mt-3" Style="width: 150px;" Text="Go" OnClick="oneGuestSearch_Click" />

                                </div>

                                <div class="col-md-3 text-end">
    <div>
        <asp:Label runat="server" ID="Label1" CssClass="form-label" Text="Approved Status"></asp:Label>
    </div>

    <div>
        <asp:Label runat="server" ID="ApprovalStatus" CssClass="form-label" Text="--"></asp:Label>
    </div>
                                </div>
                                <div class="col-md-3">
                                    <asp:Label runat="server" CssClass="form-label" Text="Business Code <span class='text-danger'>*</span>" />
                                    <asp:TextBox runat="server" ID="txtBusinessCode" CssClass="form-control" ReadOnly="true" AutoPostBack="true" OnTextChanged="txtBusinessCode_TextChanged" />
                                </div>
                                <div class="col-md-3">
                                    <asp:Label runat="server" CssClass="form-label" Text="RM Name <span class='text-danger'>*</span>" />
                                    <asp:TextBox runat="server" ID="txtBusinessCodeName" CssClass="form-control" ReadOnly="true" />
                                </div>
                                <div class="col-md-3">
                                    <asp:Label runat="server" CssClass="form-label" Text="RM Branch <span class='text-danger'>*</span>" />
                                    <asp:TextBox runat="server" ID="txtBusinessCodeBranch" CssClass="form-control" ReadOnly="true" />
                                </div>

                                <div class="col-md-3">
                                    <asp:Label runat="server" CssClass="form-label"
                                        ID="ExistingClientCodeInvLabel" Text="Existing Client Code" />
                                    <asp:TextBox runat="server" ID="ExistingClientCodeInv" CssClass="form-control" ReadOnly="true"  />
                                </div>






                                <div>
                                    <div class="row g-3">
                                        <div class="col-md-3">
                                            <h3 class="card-title">Search Account Holder </h3>
                                        </div>
                                        <div class="col-md-9">
                                            <span class="text-danger">** Before initiating new account, first search it on PAN basis </span>
                                        </div>

                                        <%--<div class="col-md-4 text-end">
                                            <asp:Label runat="server" ID="ApprovalStatus" CssClass="form-label" Text="Approved Status: " />

                                        </div>--%>
                                    </div>

                                    <div class="row g-3">
                                        <div class="col-md-4 " style="display:none">
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="Enter AH Number" />
                                            <asp:TextBox runat="server" ID="txtSearchAHNum" CssClass="form-control" />
                                        </div>
                                      
                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="Enter AH Code" />
                                            <asp:TextBox runat="server" ID="txtSearchClientCode" CssClass="form-control" MaxLength="10" />
                                        </div>
                                     
                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label" Text="PAN No" />

                                            <asp:TextBox runat="server" ID="txtSearchPan" CssClass="form-control" oninput="validatePanInput(this)"
                                                            MaxLength="10" />
                                        </div>

                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label"
                                                ID="txtClientCodeLabel" Text="Client Code" />
                                            <asp:TextBox runat="server" ID="txtClientCode" CssClass="form-control" ReadOnly="False" oninput="validateMobileNumber(this, 15)"/>
                                        </div>

                                        <div class="col-md-2 text-end"">

                                            <asp:Button ID="oneClientSearch" runat="server" CssClass="btn btn-outline-primary mt-3"  style="width:150px;" Text="Search" OnClick="oneClientSearch_Click" />

                                        </div>
                                    </div>

                                    <div class="row g-3">
                                        <div class="col-md-4 " style="display: none">
                                            <asp:Label runat="server" ID="lblHolderMessage" CssClass="text-danger" Text="" />
                                        </div>
                                    </div>
                                </div>

                                <div class="row g-3">

                                    <div class="col-md-4 d-none">
                                        <asp:Label runat="server" CssClass="form-label" Text="Source Code" />
                                        <asp:TextBox runat="server" ID="txtHeadSourceCode" CssClass="form-control" />
                                    </div>
                                </div>

                                <div>
                                    <h3 class="card-title">1. Occupation (of Account Holder)</h3>
                                    

                                    <div class="row g-3 mt-2">

                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="Tax Status" />
                                            <asp:DropDownList runat="server" ID="ddlTaxStatus" CssClass="form-select">
                                                <asp:ListItem Text="Select Status" Value="" />
                                                <asp:ListItem Text="Resident Individual" Value="Resident Individual" />
                                                <asp:ListItem Text="Resident Minor" Value="Resident Minor" />
                                                <asp:ListItem Text="NRI (Repatriable)" Value="NRI (Repatriable)" />
                                                <asp:ListItem Text="NRI (Non-Repatriable)" Value="NRI (Non-Repatriable)" />
                                                <asp:ListItem Text="NRI Minor (Repatriable)" Value="NRI Minor (Repatriable)" />
                                                <asp:ListItem Text="NRI Minor (Non-Repatriable)" Value="NRI Minor (Non-Repatriable)" />
                                                <asp:ListItem Text="Sole-Proprietor" Value="Sole-Proprietor" />
                                                <asp:ListItem Text="Others" Value="Others" Selected="True" />

                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="Occupation <span class='text-danger'>*</span>" />
                                            <asp:DropDownList runat="server" ID="ddlOccupation" CssClass="form-select">
                                                <asp:ListItem Text="One--" Value="1" />
                                                <asp:ListItem Text="Two--" Value="2" />
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>

                                <div>
                                    <h4 class="mb-3">Category</h4>
                                    <div class="row g-3">
                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="Status <span class='text-danger'>*</span>" />
                                            <asp:DropDownList runat="server" ID="ddlAOCategoryStatus" CssClass="form-select">
                                                <asp:ListItem Text="Select Status" Value="" />
                                                <asp:ListItem Text="Individual" Value="1" />
                                                <asp:ListItem Text="HUF" Value="2" />
                                                <asp:ListItem Text="Company" Value="3" />
                                                <asp:ListItem Text="Trust" Value="4" />
                                                <asp:ListItem Text="Society" Value="5" />
                                                <asp:ListItem Text="Bajaj Capital Staff" Value="6" />
                                            </asp:DropDownList>

                                        </div>
                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="Client Category <span class='text-danger'>*</span>" />
                                            <asp:DropDownList runat="server" ID="ddlAOClientCategory" CssClass="form-select">
                                             
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="A/C Category" />
                                            <asp:DropDownList runat="server" ID="ddlAOACCategory" CssClass="form-select">
                                                <asp:ListItem Text="Select A/C Category" Value="" />
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>





                                <div>
                                    <h3 class="card-title">2. Account Holder Information</h3>
                                    <div class="row g-3">
                                        <div class="col-md-2">
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="Title<span class='text-danger'>*</span>" />
                                            <asp:DropDownList runat="server" ID="ddlSalutation1" CssClass="form-select">
                                                <asp:ListItem Text="Select Salutation" Value="" />
                                                <asp:ListItem Text="Mr." Value="Mr." />
                                                <asp:ListItem Text="Mrs." Value="Mrs." />
                                                <asp:ListItem Text="Ms." Value="Ms." />
                                                <asp:ListItem Text="M/S." Value="M/S." />
                                                <asp:ListItem Text="Dr." Value="Dr." />
                                                <asp:ListItem Text="Adv." Value="Adv." />
                                                <asp:ListItem Text="Col." Value="Col." />
                                                <asp:ListItem Text="Maj." Value="Maj." />
                                                <asp:ListItem Text="Gen." Value="Gen." />
                                                <asp:ListItem Text="Brig." Value="Brig." />
                                                <asp:ListItem Text="CDR." Value="CDR." />
                                                <asp:ListItem Text="SQN LDR" Value="SQN LDR" />
                                                <asp:ListItem Text="CAPT." Value="CAPT." />
                                                <asp:ListItem Text="LT. COL." Value="LT. COL." />
                                                <asp:ListItem Text="LT." Value="LT." />
                                                <asp:ListItem Text="Wg.Cdr." Value="Wg.Cdr." />
                                                <asp:ListItem Text="Maj. Gen." Value="Maj. Gen." />
                                                <asp:ListItem Text="Master" Value="Master" />
                                                <asp:ListItem Text="Others" Value="Others" />

                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-4">
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="Account Name <span class='text-danger'>*</span>" />
                                            <asp:TextBox runat="server" ID="txtAccountName" CssClass="form-control" />
                                        </div>
                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="Title (Father/Spouse)" />
                                            <asp:DropDownList runat="server" ID="ddlSalutation2" CssClass="form-select">
                                                <asp:ListItem Text="Select Salutation" Value="" />
                                                <asp:ListItem Text="Mr." Value="Mr." />
                                                <asp:ListItem Text="Mrs." Value="Mrs." />
                                                <asp:ListItem Text="Ms." Value="Ms." />
                                                <asp:ListItem Text="M/S." Value="M/S." />
                                                <asp:ListItem Text="Dr." Value="Dr." />
                                                <asp:ListItem Text="Adv." Value="Adv." />
                                                <asp:ListItem Text="Col." Value="Col." />
                                                <asp:ListItem Text="Maj." Value="Maj." />
                                                <asp:ListItem Text="Gen." Value="Gen." />
                                                <asp:ListItem Text="Brig." Value="Brig." />
                                                <asp:ListItem Text="CDR." Value="CDR." />
                                                <asp:ListItem Text="SQN LDR" Value="SQN LDR" />
                                                <asp:ListItem Text="CAPT." Value="CAPT." />
                                                <asp:ListItem Text="LT. COL." Value="LT. COL." />
                                                <asp:ListItem Text="LT." Value="LT." />
                                                <asp:ListItem Text="Wg.Cdr." Value="Wg.Cdr." />
                                                <asp:ListItem Text="Maj. Gen." Value="Maj. Gen." />
                                                <asp:ListItem Text="Master" Value="Master" />
                                                <asp:ListItem Text="Others" Value="Others" />
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="Father/Spouse Name" />
                                            <asp:TextBox runat="server" ID="txtFatherSpouse" CssClass="form-control" MaxLength="30" />
                                        </div>
                                        <div class="col-md-4">
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="Other (Please Specify)" />
                                            <asp:TextBox runat="server" ID="txtOther1" CssClass="form-control" MaxLength="30" />
                                        </div>
                                        <div class="col-md-2">
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="Gender <span class='text-danger'>*</span>" />
                                            <asp:DropDownList runat="server" ID="ddlAOGender" CssClass="form-select">
                                                <asp:ListItem Text="Select" Value="" />
                                                <asp:ListItem Text="Male" Value="Male" />
                                                <asp:ListItem Text="Female" Value="Female" />
                                                <asp:ListItem Text="Other" Value="Other" />
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-2">
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="Marital Status" />
                                            <asp:DropDownList runat="server" ID="ddlAOMaritalStatus" CssClass="form-select">
                                                <asp:ListItem Text="Select" Value="" />
                                                <asp:ListItem Text="Single" Value="Single" />
                                                <asp:ListItem Text="Married" Value="Married" />
                                                <asp:ListItem Text="Divorced" Value="Divorced" />
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-2">
                                            <asp:Label runat="server" CssClass="form-label" Text="Nationality" />
                                            <asp:DropDownList runat="server" ID="ddlAONationality" CssClass="form-select">
                                                <asp:ListItem Text="Select" Value="" />
                                                <asp:ListItem Text="Resident" Value="RESIDENT" />
                                                <asp:ListItem Text="Indian" Value="INDIAN" Selected="True" />
                                                <asp:ListItem Text="NRI" Value="NRI" />
                                                <asp:ListItem Text="Non NRI" Value="NON NRI" />

                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-2">
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="Resident/NRI" />
                                            <asp:DropDownList runat="server" ID="ddlAOResidentNRI" CssClass="form-select">
                                                <asp:ListItem Text="Select" Value="" />
                                                <asp:ListItem Text="Resident" Value="Resident" Selected="True" />
                                                <asp:ListItem Text="NRI" Value="NRI" />
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-md-3">
                                            <label for="dob" class="form-label">Date of Birth <span class="text-danger">*</span></label>
                                            <%--AutoPostBack="true" OnTextChanged="cldDOB_TextChanged"--%>
                                                    <%--onchange="return onChangeValidateFamActiveHead(this)" oninput="formatDOBInput(this)"--%> 

                                            <div class="input-group date" data-provide="datepicker">
                                                <%--<asp:TextBox ID="cldDOB" runat="server" CssClass="form-control" onchange="validateDOB(this)" />--%>
                                                <asp:TextBox ID="cldDOB" runat="server" CssClass="form-control" oninput="formatDOBInput(this)" OnTextChanged="cldDOB_TextChanged" AutoPostBack="true" />

                                               
                                               

                                                <div class="input-group-append">
                                                    <span class="input-group-text"><i class="glyphicon glyphicon-th"></i></span>
                                                </div>
                                            </div>
                                        </div>



                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="Annual Income" />
                                            <asp:DropDownList runat="server" ID="ddlAOAnnualIncome" CssClass="form-select">
                                                <asp:ListItem Text="Select Annual Income" Value="" />
                                                <asp:ListItem Text="1-5 Lac" Value="1-5 Lac" />
                                                <asp:ListItem Text="5-10 Lac" Value="5-10 Lac" />
                                                <asp:ListItem Text="10-25 Lac" Value="10-25 Lac" />
                                                <asp:ListItem Text="25 Lac and Above" Value="25 Lac and Above" />
                                                <asp:ListItem Text="Other" Value="Other" />
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label" Text="PAN" />
                                            <asp:TextBox runat="server" ID="itfAOClientPan"
                                                MaxLength="10"
                                                placeholder="Enter PAN (ABCDE1234F)"
                                                pattern="[A-Z]{5}[0-9]{4}[A-Z]{1}"
                                                title="Enter PAN in format: ABCDE1234F"
                                                oninput="validatePanInput2(this)"
                                                CssClass="form-control" />
                                        </div>
                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="Lead Source" />
                                            <asp:DropDownList runat="server" ID="ddlLeadSource" CssClass="form-select">
                                                <asp:ListItem Text="Select Lead Source" Value="" />
                                                <asp:ListItem Text="Online" Value="Online" />
                                                <asp:ListItem Text="Referral" Value="Referral" />
                                                <asp:ListItem Text="Advertisement" Value="Advertisement" />
                                                <asp:ListItem Text="Other" Value="Other" />

                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>



                                <div>
                                    <h4 class="mb-3">Guardian Details</h4>
                                    <div class="row g-3">
                                        <div class="col-md-3">
                                            <asp:Label runat="server"
                                                CssClass="form-label" Text="Guardian/Contact Person" />
                                            <asp:TextBox runat="server" ID="itfAOGuardianPerson" CssClass="form-control guardianFieldHead" MaxLength="100" />
                                        </div>
                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="Nationality" />
                                            <asp:TextBox runat="server" ID="itfAOGuardianNationality" CssClass="form-control guardianFieldHead" MaxLength="20"/>
                                        </div>
                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="PAN " />
                                            <asp:TextBox runat="server" ID="itfAOGuardianPANNO"
                                                MaxLength="10"
                                                placeholder="Enter PAN (ABCDE1234F)"
                                                pattern="[A-Z]{5}[0-9]{4}[A-Z]{1}"
                                                title="Enter PAN in format: ABCDE1234F"
                                                oninput="validatePanInput2(this)"
                                                CssClass="form-control guardianFieldHead" />
                                            <script>
                                                function validatePanInput2(input) {
                                                    let panPattern = /^[A-Z]{5}[0-9]{4}[A-Z]{1}$/; // PAN format

                                                    // Convert to uppercase and remove invalid characters
                                                    input.value = input.value.toUpperCase().replace(/[^A-Z0-9]/g, '');

                                                    // Show error message as a tooltip if invalid
                                                    if (input.value.length > 0 && !panPattern.test(input.value)) {
                                                        input.setAttribute("title", "Invalid PAN format (ABCDE1234F)");
                                                        input.classList.add("invalid-input"); // Add red border
                                                    } else {
                                                        input.setAttribute("title", "Enter PAN in format: ABCDE1234F");
                                                        input.classList.remove("invalid-input"); // Remove red border if valid
                                                    }
                                                }
                                            </script>
                                             <style>
                                                 /* Optional: Add red border for invalid input */
                                                 .invalid-input {
                                                     border: 2px solid red;
                                                 }
                                             </style>
                                            <%-- 
                                                
                                                 <asp:TextBox ID="panNosbl"
     CssClass="form-control"
     runat="server"
     MaxLength="10"
     placeholder="Enter PAN (ABCDE1234F)"
     pattern="[A-Z]{5}[0-9]{4}[A-Z]{1}"
     title="Enter PAN in format: ABCDE1234F"
     oninput="validatePanInput2(this)">
 </asp:TextBox>
 
                  
                                                
                                                --%>
                                        </div>
                                    </div>
                                </div>


                                <div>


                                    <!-- Mailing Address -->
                                    <div class="row g-3 mb-3">
                                        <div class="col-md-6">
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="Mailing Address 1" />
                                            <asp:TextBox runat="server" ID="itfAOMAddress1" CssClass="form-control" TextMode="MultiLine"
                                                MaxLength="100"
                                                Rows="3" Columns="10" />
                                        </div>
                                        <div class="col-md-6" >
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="Mailing Address 2" />
                                            <asp:TextBox runat="server" ID="itfAOMAddress2" CssClass="form-control" TextMode="MultiLine"
                                                MaxLength="100"
                                                Rows="3" Columns="10" />
                                        </div>
                                    </div>

                                    <div class="row g-3 mb-3">
                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label" Text="Country <span class='text-danger'>*</span>" />
                                            <asp:DropDownList runat="server" ID="ddlMailingCountryList" CssClass="form-select" AutoPostBack="true"
                                                OnSelectedIndexChanged="ddlMailingCountryList_SelectedIndexChanged">
                                                <asp:ListItem Value="0">Select Country</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label" Text="State <span class='text-danger'>*</span>" />
                                            <asp:DropDownList runat="server" ID="ddlMailingStateList" CssClass="form-select" AutoPostBack="true"
                                                OnSelectedIndexChanged="ddlMailingStateList_SelectedIndexChanged">
                                                <asp:ListItem Value="0">Select State</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label" Text="City <span class='text-danger'>*</span>" />
                                            <asp:DropDownList runat="server" ID="ddlMailingCityList" CssClass="form-select">
                                                <asp:ListItem Value="0">Select City</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label" Text="Pin" />
                                            <asp:TextBox
                                                runat="server"
                                                ID="txtMailingPin"
                                                CssClass="form-control"
                                                MaxLength="20"
                                                />
                                        </div>                                    
                                    </div>

                                    <div class="row g-3 mb-3">
                                        <div class="col-md-6">
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="Permanent Address 1" />
                                            <asp:TextBox runat="server" ID="txtPAddress1" CssClass="form-control"
                                                MaxLength="100"
                                                TextMode="MultiLine" Rows="3" Columns="10" />
                                        </div>
                                        <div class="col-md-6">
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="Permanent Address 2" />
                                            <asp:TextBox runat="server" ID="txtPAddress2" CssClass="form-control"
                                                MaxLength="100"
                                                TextMode="MultiLine" Rows="3" Columns="10" />
                                        </div>
                                    </div>

                                    <div class="row g-3">
                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label" Text="Country" />
                                            <asp:DropDownList runat="server" ID="ddlPCountryList" CssClass="form-select" AutoPostBack="true"
                                                OnSelectedIndexChanged="ddlPCountryList_SelectedIndexChanged">
                                                <asp:ListItem Value="">Select Country</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label" Text="State" />
                                            <asp:DropDownList runat="server" ID="ddlPStateList" CssClass="form-select" AutoPostBack="true"
                                                OnSelectedIndexChanged="ddlPStateList_SelectedIndexChanged">
                                                <asp:ListItem Value="">Select State</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label" Text="City" />
                                            <asp:DropDownList runat="server" ID="ddlPCityList" CssClass="form-select">
                                                <asp:ListItem Value="">Select City</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label" Text="Pin" />
                                            <asp:TextBox runat="server" ID="txtPPin" MaxLength="20"
                                                CssClass="form-control" />
                                        </div>
                                    </div>

                                    <div class="form-check">


                                        <label class="form-check-label">
                                            Permanent Address same as Mailing Address 
                                           
                                            <asp:CheckBox
                                                ID="CheckBox1"
                                                runat="server"
                                                AutoPostBack="true"
                                                OnCheckedChanged="CheckBox1_CheckedChanged" />

                                        </label>
                                    </div>



                                    <!-- Overseas Address for NRIs/FIIs -->
                                    <div class="row g-3 mb-3">
                                        <div class="col-md-12">
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="Overseas Address (if applicable)" />
                                            <asp:TextBox runat="server" ID="txtOverseasAdd" CssClass="form-control"
                                                TextMode="MultiLine" Rows="3" Columns="10" 
                                                MaxLength="100"
                                                />
                                        </div>
                                    </div>


                                    <!-- Additional Details -->
                                    <div class="row g-3 mb-3">
                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label" Text="Fax" />
                                            <asp:TextBox runat="server" ID="txtFax"
                                                MaxLength="15"
                                                 oninput="validateMobileNumber(this, 12)"
                                                CssClass="form-control" />


                                        </div>
                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="Aadhaar" />
                                            <asp:TextBox runat="server" ID="txtAadhar"
                                                MaxLength="12"
                                                oninput="validateAadharNumber(this, 12)"
                                                CssClass="form-control"  />
                                        </div>
                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="Email <span class='text-danger'>*</span>" />
                                            <asp:TextBox runat="server" ID="txtEmail" CssClass="form-control" 
                                                MaxLength="50" 
                                                />
                                        </div>
                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label"
                                                
                                                Text="Email Official" />
                                            <asp:TextBox runat="server" ID="txtOfficialEmail"
                                                MaxLength="50"
                                                CssClass="form-control"  />
                                        </div>
                                    </div>
                                </div>

                                <div class="row g-3">
                                    <div class="col-md-6">
                                        <h3 class="card-title">Enter Any One of Three Numbers</h3>
                                        <div class="row g-3">
                                            <!-- Phone (Off) -->
                                            <div class="col-md-6">
                                                <asp:Label runat="server" CssClass="form-label"
                                                    Text="Phone (Off) - STD" />
                                                <asp:TextBox runat="server" ID="PhoneOfficeSTD"
                                                    MaxLength="5" 
                                                    CssClass="form-control" />
                                            </div>
                                            <div class="col-md-6">
                                                <asp:Label runat="server" CssClass="form-label"
                                                    Text="Phone (Off) - Number" />
                                                <asp:TextBox runat="server" ID="PhoneOfficeNumber" 
                                                    MaxLength="15"
                                                    
                                                    oninput="validateMobileNumber(this, 15)" CssClass="form-control" />
                                            </div>

                                            <!-- Phone (Res) -->
                                            <div class="col-md-6">
                                                <asp:Label runat="server" CssClass="form-label"
                                                    Text="Phone (Res) - STD" />
                                                <asp:TextBox runat="server" ID="PhoneResSTD" 
                                                    MaxLength="5"
                                                    CssClass="form-control" />
                                            </div>
                                            <div class="col-md-6">
                                                <asp:Label runat="server" CssClass="form-label"
                                                    Text="Phone (Res) - Number" />
                                                <asp:TextBox runat="server" ID="PhoneResNumber"
                                                    MaxLength="15"
                                                    
                                                    oninput="validateMobileNumber(this, 15)" CssClass="form-control" />
                                            </div>

                                            <!-- Mobile -->
                                            <div class="col-md-12">
                                                <asp:Label runat="server" CssClass="form-label"
                                                    Text="Mobile <span class='text-danger'>*</span>" />
                                                <asp:TextBox runat="server" ID="MobileNo"
                                                    MaxLength="10"
                                                    oninput="validateMobileNumber(this, 10)" CssClass="form-control" />
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-md-6">
                                        <h3 class="card-title">Reference Details</h3>
                                        <div class="row g-3">
                                            <div class="col-md-6">
                                                <asp:Label runat="server" CssClass="form-label" Text="Reference Name" />
                                                <asp:TextBox runat="server" ID="ReferenceName1" MaxLenght="100" oninput ="validateVarchar(this,100)" CssClass="form-control mb-3" Placeholder="Enter Ref. Name 1" />
                                                <asp:TextBox runat="server" ID="ReferenceName2" MaxLenght="100" oninput ="validateVarchar(this,100)" CssClass="form-control mb-3" Placeholder="Enter Ref. Name 2"/>
                                                <asp:TextBox runat="server" ID="ReferenceName3" MaxLenght="100" oninput ="validateVarchar(this,100)" CssClass="form-control mb-3" Placeholder="Enter Ref. Name 3"/>
                                                <asp:TextBox runat="server" ID="ReferenceName4" MaxLenght="100" oninput ="validateVarchar(this,100)" CssClass="form-control"      Placeholder="Enter Ref. Name 4" />
                                            </div>

                                            <div class="col-md-6">
                                                <asp:Label runat="server" CssClass="form-label" Text="Mobile No" />
                                                <asp:TextBox runat="server" ID="ReferenceMobileNo1" MaxLength="10"  oninput="validateMobileNumber(this, 10)" CssClass="form-control mb-3" Placeholder="Enter Ref. Mobile 1" />
                                                <asp:TextBox runat="server" ID="ReferenceMobileNo2" MaxLength="10"  oninput="validateMobileNumber(this, 10)" CssClass="form-control mb-3" Placeholder="Enter Ref. Mobile 2" />
                                                <asp:TextBox runat="server" ID="ReferenceMobileNo3" MaxLength="10"  oninput="validateMobileNumber(this, 10)" CssClass="form-control mb-3" Placeholder="Enter Ref. Mobile 3" />
                                                <asp:TextBox runat="server" ID="ReferenceMobileNo4" MaxLength="10"  oninput="validateMobileNumber(this, 10)" CssClass="form-control"      Placeholder="Enter Ref. Mobile 4" />
                                            </div>
                                        </div>
                                    </div>

                                </div>
                                    
                                
                            </div>
                            <div class="d-flex align-items-md-center flex-wrap flex-md-row flex-column gap-3">
                                <asp:Button ID="btnHiddenPostbackResetOnCreated" runat="server" OnClick="ResetFormFields1OnCreation" style="display:none;" />


                                <asp:Button runat="server" ID="btnNext" CssClass="btn btn-primary" Text="Next" OnClientClick="document.getElementById('other_details_tab').click(); return false;" AccessKey="N" />
                                <asp:Button runat="server" ID="btnReset" CssClass="btn btn-outline-primary" Text="Reset" OnClick="ResetFields" />
                                <asp:Button runat="server" ID="btnExit" CssClass="btn btn-outline-primary" Text="Exit" OnClick="ExitButton_Click" />
                            </div>

                        </div>

                        <div class="tab-pane fade pt-0" id="other_details" role="tabpanel" aria-labelledby="other_details_tab">
                            <div class="container mt-0 d-none" >
                                <h3 class="card-title">Add Family Member Details</h3>
                                <asp:UpdatePanel runat="server" ID="ngfd_UpdatePanels">


                                    <ContentTemplate>
                                        <div style="width: 100%; overflow-x: auto; white-space: nowrap; height:200px" class="mt-6">

                                            <asp:GridView
                                                ID="ngfd_gvClients"
                                                runat="server"
                                                AutoGenerateColumns="False"
                                                OnRowDataBound="ngfd_gvClients_RowDataBound">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Existing Investor">
                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="ngfd_ddlExistingInvestor" runat="server"
                                                                AutoPostBack="true" OnSelectedIndexChanged="ddladdfamExistingInvestor_SelectedIndexChanged"
                                                                >
                                                                <asp:ListItem Text="Select" Value="" />
                                                            </asp:DropDownList>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Title">
                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="ngfd_ddlSalutation" runat="server" >
                                                                <asp:ListItem Text="Select" Value="" />
                                                                <asp:ListItem Text="Mr." Value="Mr." />
                                                                <asp:ListItem Text="Mrs." Value="Mrs." />
                                                            </asp:DropDownList>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Investor Name">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="ngfd_txtInvestorName" runat="server"  Text='<%# Eval("InvestorName") %>'></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Mobile">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="ngfd_txtMobile" runat="server" oninput="validateMobileNumber(this, 10)" Text='<%# Eval("Mobile") %>'></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Email">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="ngfd_txtEmail" runat="server" Text='<%# Eval("Email") %>'></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="PAN">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="ngfd_txtPAN" runat="server" oninput="validatePanInput(this)" Text='<%# Eval("PAN") %>'></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Aadhar">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="ngfd_txtAadhar" runat="server" MaxLength="12" Text='<%# Eval("Aadhar") %>'></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="DOB">
                                                        <ItemTemplate>
                                                            <%--<asp:TextBox ID="ngfd_txtDOB" runat="server" Text='<%# Eval("DOB") %>'></asp:TextBox>--%>
                                                            <asp:TextBox ID="ngfd_txtDOB" runat="server" CssClass="datepicker" MaxLength="10" 
                                                                oninput="formatDOBInput(this)"
                                                                onchange="return onChangeValidateFamActive(this)" 
                                                                Text='<%# Eval("DOB", "{0:dd-MM-yyyy}") %>' ></asp:TextBox>

                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Gender">
                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="ngfd_ddlGender" runat="server"  >
                                                                <asp:ListItem Text="Select" Value="" />
                                                                <asp:ListItem Text="Male" Value="M" />
                                                                <asp:ListItem Text="Female" Value="F" />
                                                            </asp:DropDownList>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Guardian PAN">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="ngfd_txtgPAN" runat="server" CssClass="guardianField" MaxLength="10" Text='<%# Eval("GuardianPAN") %>'></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Guardian Name">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="ngfd_txtgName" runat="server" CssClass="guardianField" MaxLength="100" Text='<%# Eval("GuardianName") %>'></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <%--SelectedValue='<%# Eval("Salutation") %>'--%>
                                                    <asp:TemplateField HeaderText="Occupation">
                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="ngfd_ddlOccupation" runat="server" >
                                                                <asp:ListItem Text="Select" Value="" />
                                                            </asp:DropDownList>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Relation">
                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="ngfd_ddlRelation" runat="server" >
                                                                <asp:ListItem Text="Select" Value="" />
                                                            </asp:DropDownList>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="KYC">
                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="ngfd_ddlKyc" runat="server" SelectedValue='<%# Eval("KYC") %>'>
                                                                <asp:ListItem Text="Select" Value="" />

                                                                <asp:ListItem Text="Yes" Value="Y" />
                                                                <asp:ListItem Text="No" Value="N" />
                                                            </asp:DropDownList>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Approved">
                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="ngfd_ddlApproved" runat="server" SelectedValue='<%# Eval("Approved") %>'>
                                                                <asp:ListItem Text="Select" Value="" />

                                                                <asp:ListItem Text="Yes" Value="Y" />
                                                                <asp:ListItem Text="No" Value="N" />
                                                            </asp:DropDownList>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Is Nominee">
                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="ngfd_ddlIsNominee" runat="server" >
                                                                <asp:ListItem Text="Select" Value="" />

                                                                <asp:ListItem Text="Yes" Value="Y" />
                                                                <asp:ListItem Text="No" Value="N" />
                                                            </asp:DropDownList>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Allocation">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="ngfd_txtAllocation" runat="server" MaxLength="100" Text='<%# Eval("Allocation") %>'></asp:TextBox>

                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                        <br />
                                        <asp:Button runat="server" ID="ngfd_btnAddRow" Text="Add New Row" OnClick="ngfd_btnAddRow_Click" CssClass="btn btn-primary" />
                                        <asp:Button runat="server" ID="ngfd_btnSave" Text="Save" OnClick="ngfd_btnSave_Click" CssClass="btn btn-success" />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>

     

                            <asp:FormView >

                            <div>
                                <h3 class="card-title">Add Family Member Details</h3>
                                <div class="row g-3">
                                    <div class="col-md-3">
                                        <asp:Label runat="server" CssClass="form-label" Text="Existing Investor" />

                                        <asp:DropDownList runat="server" ID="ddladdfamExistingInvestor" CssClass="form-select"
                                            AutoPostBack="true" OnSelectedIndexChanged="ddladdfamExistingInvestor_SelectedIndexChanged">
                                        </asp:DropDownList>




                                    </div>
                                    <div class="col-md-2">
                                        <asp:Label runat="server" CssClass="form-label" Text="Title <span class='text-danger'>*</span>" />
                                        <asp:DropDownList runat="server" ID="ddlSalutation3" CssClass="form-select">
                                            <asp:ListItem Text="Select" Value="" />
                                            <asp:ListItem Text="Mr." Value="Mr." />
                                            <asp:ListItem Text="Mrs." Value="Mrs." />
                                            <asp:ListItem Text="Ms." Value="Ms." />
                                            <asp:ListItem Text="M/S." Value="M/S." />
                                            <asp:ListItem Text="Dr." Value="Dr." />
                                            <asp:ListItem Text="Adv." Value="Adv." />
                                            <asp:ListItem Text="Col." Value="Col." />
                                            <asp:ListItem Text="Maj." Value="Maj." />
                                            <asp:ListItem Text="Gen." Value="Gen." />
                                            <asp:ListItem Text="Brig." Value="Brig." />
                                            <asp:ListItem Text="CDR." Value="CDR." />
                                            <asp:ListItem Text="SQN LDR" Value="SQN LDR" />
                                            <asp:ListItem Text="CAPT." Value="CAPT." />
                                            <asp:ListItem Text="LT. COL." Value="LT. COL." />
                                            <asp:ListItem Text="LT." Value="LT." />
                                            <asp:ListItem Text="Wg.Cdr." Value="Wg.Cdr." />
                                            <asp:ListItem Text="Maj. Gen." Value="Maj. Gen." />
                                            <asp:ListItem Text="Master" Value="Master" />
                                            <asp:ListItem Text="Others" Value="Others" />

                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-md-3">
                                        <asp:Label runat="server" CssClass="form-label" Text="Investor Name <span class='text-danger'>*</span>" />
                                        <asp:TextBox runat="server" ID="addfamInvestorName" CssClass="form-control" MaxLength="100" />
                                    </div>
                                    <div class="col-md-2">
                                        <asp:Label runat="server" CssClass="form-label" Text="Gender <span class='text-danger'>*</span>" />
                                        <asp:DropDownList runat="server" ID="addfamGender" CssClass="form-select">
                                            <asp:ListItem Text="Select" Value="" />
                                            <asp:ListItem Text="Male" Value="Male" />
                                            <asp:ListItem Text="Female" Value="Female" />
                                            <asp:ListItem Text="Non-IND" Value="Other" />
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-md-2">
                                        <asp:Label runat="server" CssClass="form-label" Text="Mobile <span class='text-danger'>*</span>" />
                                        <asp:TextBox runat="server" ID="addfamMobile" CssClass="form-control"  oninput="validateMobileNumber(this, 10)" />
                                    </div>
                                    
                                    <div class="col-md-3">
                                        <asp:Label runat="server" CssClass="form-label" Text="Email" />
                                        <asp:TextBox runat="server" ID="addfamEmail" CssClass="form-control" MaxLength="50"  />
                                    </div>


                                     <%--  Member DOB --%>
                                    <div class="col-md-2">
                                        <asp:Label runat="server" CssClass="form-label" Text="DOB <span class='text-danger'>*</span>" class="text-danger" />
                                            

                                        <%--AutoPostBack="true" OnTextChanged="addfamDOB_TextChanged  onchange="return onChangeValidateFamActive(this)""--%>
                                        <div class="input-group date" data-provide="datepicker">
                                            <asp:TextBox ID="addfamDOB" runat="server" CssClass="form-control"
                                                MaxLength="10"
                                                AutoPostBack="true" OnTextChanged="addfamDOB_TextChanged"
                                                
                                                oninput="formatDOBInput(this)" />
                                            <div class="input-group-append">
                                                <span class="input-group-text"><i class="glyphicon glyphicon-th"></i></span>
                                            </div>
                                        </div>
                                    </div>
                                    <script>
                                        $(document).ready(function () {
                                            $('.input-group.date').datepicker({
                                                endDate: new Date(), // Restrict selection to dates up to today
                                                autoclose: true,
                                                format: 'dd/mm/yyyy' // Customize the format as needed
                                            });
                                        });
                                    </script>

                                    <%--  Guuardian name --%>
                                    <div class="col-md-3">
                                        <asp:Label runat="server" CssClass="form-label" Text="Guardian Name" />
                                        <asp:TextBox runat="server" ID="addfamGuardianName" CssClass="form-control guardianField" MaxLength="50" />
                                    </div>

                                    <%--  Guuardian PAN --%>
                                    <div class="col-md-2">
                                        <asp:Label runat="server" CssClass="form-label" Text="Guardian PAN" />
                                        <asp:TextBox runat="server" ID="addfamGuardianPan" CssClass="form-control guardianField"
                                            MaxLength="10"
                                            placeholder="Enter PAN (ABCDE1234F)"
                                            pattern="[A-Z]{5}[0-9]{4}[A-Z]{1}"
                                            title="Enter PAN in format: ABCDE1234F"
                                            oninput="validatePanInput2(this)" />
                                    </div>

                                    <%--  Member PAN --%>
                                    <div class="col-md-2">
                                        <asp:Label runat="server" CssClass="form-label" Text="Member PAN" />
                                        <asp:TextBox runat="server" ID="addfamPan" CssClass="form-control"
                                            MaxLength="10"
                                            placeholder="Enter PAN (ABCDE1234F)"
                                            pattern="[A-Z]{5}[0-9]{4}[A-Z]{1}"
                                            title="Enter PAN in format: ABCDE1234F"
                                            oninput="validatePanInput2(this)" />
                                    </div>

                                    <%--  Guuardian AADHAR --%>
                                    <div class="col-md-2">
                                        <asp:Label runat="server" CssClass="form-label" Text="Aadhar Number" />
                                        <asp:TextBox runat="server" ID="addfamAadharNumber" CssClass="form-control" MaxLength="12" oninput="validateMobileNumber(this, 12)" />
                                    </div>
                                  
                                    <%--  Member OCC --%>
                                    <div class="col-md-2">
                                        <asp:Label runat="server" CssClass="form-label" Text="Occupation <span class='text-danger'>*</span>" />
                                        <asp:DropDownList runat="server" ID="addfamOccupation" CssClass="form-select">
                                            <asp:ListItem Text="Select" Value="" />
                                            <asp:ListItem Text="Employed" Value="Employed" />
                                            <asp:ListItem Text="Self-Employed" Value="Self-Employed" />
                                            <asp:ListItem Text="Unemployed" Value="Unemployed" />
                                        </asp:DropDownList>
                                    </div>

                                     <%--  Member relation --%>
                                    <div class="col-md-2">
                                        <asp:Label runat="server" CssClass="form-label" Text="Relation <span class='text-danger'>*</span>" />
                                        <asp:DropDownList runat="server" ID="addfamRelation" CssClass="form-select">
                                            <asp:ListItem Text="Select" Value="" />
                                            <asp:ListItem Text="Spouse" Value="Spouse" />
                                            <asp:ListItem Text="Child" Value="Child" />
                                            <asp:ListItem Text="Parent" Value="Parent" />
                                        </asp:DropDownList>
                                    </div>
                                  
                                     <%--  Member KYC --%>
                                    <div class="col-md-2">
                                        <asp:Label runat="server" CssClass="form-label" Text="KYC" />
                                        <asp:DropDownList runat="server" ID="addfamKYC" CssClass="form-select">
                                            <asp:ListItem Text="Select" Value="" />
                                            <asp:ListItem Text="Yes" Value="YES" />
                                            <asp:ListItem Text="No" Value="NO" />
                                        </asp:DropDownList>
                                    </div>

                                     <%--  Member approved --%>
                                    <div class="col-md-2">
                                        <asp:Label runat="server" CssClass="form-label" Text="Approved" />
                                        <asp:DropDownList runat="server" ID="addfamApproved" CssClass="form-select">
                                            <asp:ListItem Text="Select" Value="" />
                                            <asp:ListItem Text="Yes" Value="YES" />
                                            <asp:ListItem Text="No" Value="NO" />
                                        </asp:DropDownList>
                                    </div>


                                     <%--  Member nominee --%>
                                    <div class="col-md-2">
                                        <asp:Label runat="server" CssClass="form-label" Text="Is Nominee" />
                                        <asp:DropDownList runat="server" ID="addfamIsNominee" CssClass="form-select">
                                            <asp:ListItem Text="Select" Value="" />
                                            <asp:ListItem Text="Yes" Value="YES" />
                                            <asp:ListItem Text="No" Value="NO" />
                                        </asp:DropDownList>
                                    </div>

                                     <%--  Member allocation --%>
                                    <div class="col-md-2">
                                        <asp:Label runat="server" CssClass="form-label" Text="Allocation %" />
                                        <asp:TextBox
                                            runat="server"
                                            ID="addfamAllocationPercentage"
                                            CssClass="form-control"
                                            MaxLength="3"
                                            TextMode="Number"
                                            oninput="validatePercentage(this)" />
                                        <script>
                                            function validatePercentage(input) {
                                                // Remove non-numeric characters
                                                input.value = input.value.replace(/[^0-9]/g, '');

                                                // Check if the value exceeds 100
                                                if (input.value > 100) {
                                                    alert("Value cannot exceed 100.");
                                                    input.value = 100; // Reset to maximum allowed value
                                                }
                                            }
                                        </script>


                                    </div>

                                    <div class="col-md-7 mt-4">

                                        <asp:Button runat="server" CssClass="btn btn-success mt-2" Text="Add" ID="AddMemberButton"       OnClick="AddMemberButton_Click" AutoPostBack="true" />
                                        <asp:Button runat="server" CssClass="btn btn-warning mt-2" Text="Update" ID="UpdateMemberButton" OnClick="UpdateMemberButton_Click"  />
                                        <asp:Button runat="server" CssClass="btn btn-warning mt-2" Text="Reset" ID="ResetMemberButton"   OnClick="ResetMemberButton_Click"  />



                                <asp:Label runat="server" ID="lblMessageAddFam" CssClass="text-info" Text="" Visible="false" />

                                    </div>


                                </div>

                               

                            </div>

                            </asp:FormView>

                             <div class="mt-3">

                                 </div>

                 
                            <div style="width: 100%; overflow-x: auto; white-space: nowrap; margin-bottom:25px" class="mt-6" >
                                <asp:Label runat="server" ID="lblExistingMemList" CssClass="m-1" Text="Existing Member List:" Visible="false" />
                                <asp:GridView ID="familyGridView" runat="server" CssClass="table table-hover" AutoGenerateColumns="false" OnRowCommand="gvFamily_RowCommand">
                                    <Columns>
                                        <%--<asp:TemplateField HeaderText="#" ItemStyle-CssClass="column-width-50">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSr" runat="server" Text='<%# Container.DisplayIndex + 1 %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>--%>

                                        <%--                      <asp:TemplateField HeaderText="Inv Code">
                <ItemTemplate>
                    <asp:Label ID="lblInvCode" runat="server" Text='<%# Eval("inv_code") %>' Visible="true" />
                </ItemTemplate>
            </asp:TemplateField>--%>

                                        <asp:TemplateField HeaderText="Title">
                                            <HeaderStyle Width="100px" />
                                            <ItemStyle Width="100px" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblTitle" runat="server" Text='<%# Eval("investor_title") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Investor Name">
                                            <HeaderStyle Width="200px" />
                                            <ItemStyle Width="200px" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblInvestorName" runat="server" Text='<%# Eval("investor_name") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Gender">
                                            <ItemTemplate>
                                                <asp:Label ID="lblGender" runat="server" Text='<%# Eval("gender") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Mobile">
                                            <ItemTemplate>
                                                <asp:Label ID="lblMobile" runat="server" Text='<%# Eval("mobile") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Email">
                                            <HeaderStyle Width="150px" />
                                            <ItemStyle Width="150px" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblEmail" runat="server" Text='<%# Eval("email") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="PAN">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPAN" runat="server" Text='<%# Eval("pan") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Occupation">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOccupation" runat="server" Text='<%# Eval("OCC_NAME") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Occupation ID" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOccupationID" runat="server" Text='<%# Eval("OCC_ID") %>' Visible="false" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Relation">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRelation" runat="server" Text='<%# Eval("OUR_RELATIONSHIP") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Relation ID" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRelationID" runat="server" Text='<%# Eval("OUR_RELATIONSHIP_ID") %>' Visible="false" />
                                            </ItemTemplate>
                                        </asp:TemplateField>


                                        <asp:TemplateField HeaderText="KYC">
                                            <ItemTemplate>
                                                <asp:Label ID="lblKYC" runat="server" Text='<%# Eval("KYC") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Guardian Name">
                                            <ItemTemplate>
                                                <asp:Label ID="lblGuardianName" runat="server" Text='<%# Eval("g_name") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Guardian PAN">
                                            <ItemTemplate>
                                                <asp:Label ID="lblGuardianPAN" runat="server" Text='<%# Eval("g_pan") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Approved">
                                            <ItemTemplate>
                                                <asp:Label ID="lblApproved" runat="server" Text='<%# Eval("approved") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>


                                        <asp:TemplateField HeaderText="Aadhaar Card Number">
                                            <ItemTemplate>
                                                <asp:Label ID="lblAadhaarCardNumber" runat="server" Text='<%# Eval("AADHAR_CARD_NO") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Nominee">
                                            <ItemTemplate>
                                                <asp:Label ID="lblIsNoinee" runat="server" Text='<%# Eval("is_nominee") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Allocation">
                                            <ItemTemplate>
                                                <asp:Label ID="lblNomineePer" runat="server" Text='<%# Eval("nominee_per") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                    </Columns>
                                </asp:GridView>
                            </div>
                            
                            
                             <asp:Label runat="server" ID="lblAddingNewMemList" CssClass="mt-2 h3 card-title" Text="Adding New Member List:" Visible="false" />
                            <div style="width: 100%; overflow-x: auto; white-space: nowrap; margin-top:10px" class="">
                                <%--<asp:Label runat="server" ID="lblAddingNewMemList" CssClass="m-2" Text="Adding New Member List:" Visible="true" />--%>

                                <asp:GridView ID="newMemGrid_IL_familyGridView" runat="server"
                                    CssClass="table table-hover" AutoGenerateColumns="false"
                                    ShowHeader="TRUE"
                                    OnRowDeleting="gvFamilyMembers_RowDeleting"
                                    OnRowCommand="gvFamily_RowCommand">
                                    <Columns>
                                        <%--  <asp:TemplateField HeaderText="#" ItemStyle-CssClass="column-width-50">
                                <ItemTemplate>
                                    <asp:Label ID="newMemGrid_IL_lblSr" runat="server" Text='<%# Container.DisplayIndex + 1 %>' />
                                </ItemTemplate>
                            </asp:TemplateField>--%>

                                        <%--                        <asp:TemplateField HeaderText="Inv Code" HeaderStyle-Width="60px">
    <ItemTemplate>
        <asp:Label ID="newMemGrid_IL_lblInvCode" runat="server" Text='<%# Eval("inv_code") %>' Visible="true" />
    </ItemTemplate>
</asp:TemplateField>--%>


                                        <asp:TemplateField HeaderText="Title">
                                            <ItemStyle Width="100px" />
                                            <ItemTemplate>
                                                <asp:Label ID="newMemGrid_IL_lblTitle" runat="server" Text='<%# Eval("investor_title") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Investor Name">
                                            <ItemStyle Width="200px" />
                                            <ItemTemplate>
                                                <asp:Label ID="newMemGrid_IL_lblInvestorName" runat="server" Text='<%# Eval("investor_name") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Gender">
                                            <ItemTemplate>
                                                <asp:Label ID="newMemGrid_IL_lblGender" runat="server" Text='<%# Eval("gender") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Mobile">
                                            <ItemTemplate>
                                                <asp:Label ID="newMemGrid_IL_lblMobile" runat="server" Text='<%# Eval("mobile") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Email">
                                            <ItemStyle Width="150px" />
                                            <ItemTemplate>
                                                <asp:Label ID="newMemGrid_IL_lblEmail" runat="server" Text='<%# Eval("email") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="PAN">
                                            <ItemTemplate>
                                                <asp:Label ID="newMemGrid_IL_lblPAN" runat="server" Text='<%# Eval("pan") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="DOB">
                                            <ItemTemplate>
                                                <asp:Label ID="newMemGrid_IL_lblDOB" runat="server" Text='<%# Eval("dob") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Occupation">
                                            <ItemTemplate>
                                                <asp:Label ID="newMemGrid_IL_lblOccupation" runat="server" Text='<%# Eval("OCC_NAME") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Occupation ID" Visible="FALSE">
                                            <ItemTemplate>
                                                <asp:Label ID="newMemGrid_IL_lblOccupationID" runat="server" Text='<%# Eval("OCC_ID") %>' Visible="FALSE" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Relation">
                                            <ItemTemplate>
                                                <asp:Label ID="newMemGrid_IL_lblRelation" runat="server" Text='<%# Eval("OUR_RELATIONSHIP") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Relation ID" Visible="FALSE">
                                            <ItemTemplate>
                                                <asp:Label ID="newMemGrid_IL_lblRelationID" runat="server" Text='<%# Eval("OUR_RELATIONSHIP_ID") %>' Visible="FALSE" />
                                            </ItemTemplate>
                                        </asp:TemplateField>


                                        <asp:TemplateField HeaderText="KYC">
                                            <ItemTemplate>
                                                <asp:Label ID="newMemGrid_IL_lblKYC" runat="server" Text='<%# Eval("KYC") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Guardian Name">
                                            <ItemTemplate>
                                                <asp:Label ID="newMemGrid_IL_lblGuardianName" runat="server" Text='<%# Eval("g_name") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Guardian PAN">
                                            <ItemTemplate>
                                                <asp:Label ID="newMemGrid_IL_lblGuardianPAN" runat="server" Text='<%# Eval("g_pan") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Approved">
                                            <ItemTemplate>
                                                <asp:Label ID="newMemGrid_IL_lblApproved" runat="server" Text='<%# Eval("approved") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>


                                        <asp:TemplateField HeaderText="Aadhaar Card Number">
                                            <ItemTemplate>
                                                <asp:Label ID="newMemGrid_IL_lblAadhaarCardNumber" runat="server" Text='<%# Eval("AADHAR_CARD_NO") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Nominee">
                                            <ItemTemplate>
                                                <asp:Label ID="newMemGrid_IL_lblIsNoinee" runat="server" Text='<%# Eval("is_nominee") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Allocation">
                                            <ItemTemplate>
                                                <asp:Label ID="newMemGrid_IL_lblNomineePer" runat="server" Text='<%# Eval("nominee_per") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                    </Columns>
                                </asp:GridView>
                            </div>

                            <div class="mt-3">
                                <div class="d-flex align-items-center flex-md-row flex-column gap-3">
                                    <%--<asp:Button runat="server" CssClass="btn btn-primary" Text="Save New Account" />
                                    <asp:Button runat="server" CssClass="btn btn-outline-primary" Text="Modify and save as A/C" OnClick="btnUpdate_Click" />
                                    <asp:Button runat="server" CssClass="btn btn-outline-primary" Text="Reset" OnClick="ResetFields"  />
                                    <asp:Button runat="server" CssClass="btn btn-outline-primary" Text="Approve"  OnClick="ApproveButton_Click"/>
                                    --%>
                                    
                                </div>
                            </div>

                            <hr />

                            <asp:Button runat="server" ID="btnSave" CssClass="btn btn-primary" Text="Save A/C" OnClick="btnInsert_Click" AccessKey="S" />
                            <asp:Button runat="server" ID="btnUpdate" CssClass="btn btn-primary" Text="Modify A/C" OnClick="btnUpdate_Click" AccessKey="M" />
                            <asp:Button ID="btnApprove" runat="server" CssClass="btn btn-outline-primary" Text="Approve" OnClick="ApproveButton_Click" />
                            <asp:Button runat="server" CssClass="btn btn-outline-primary" Text="Back" OnClientClick="document.getElementById('basic_details_tab').click(); return false;" AccessKey="B" />
                            <asp:Button runat="server" CssClass="btn btn-outline-primary" Text="Exit" OnClick="ExitButton_Click"   />


                            <hr />

                            <div>
                                <h2 class="card-title">Advisory Package (Option)</h2>
                                <p>
                                    * Please select the appropriate advisory package and details of payment, If client
    has opted for paid package and click on generate AR.
    <br />
                                    * AR will be generated automatically.
                                </p>

                                <div class="row g-3">
                                    <asp:Label runat="server" CssClass="form-label mt-1 font-weight-bold" ID="txtClientAdvisoryInfo" Text="" />
                                    <div class="col-md-5 grid-margin stretch-card">
                                        <div class="card">
                                            <div class="card-body">
                                                <div class="row g-3">
                                                    <h5 class="mb-3">Advisory Fees transaction entry system</h5>
                                                    <div class="col-md-12">
                                                        <asp:Label runat="server" CssClass="form-label"
                                                            Text="Plan Type <span class='text-danger'>*</span>" />
                                                        <asp:DropDownList runat="server" ID="ddlMutualPlanType" CssClass="form-select">
                                                            <asp:ListItem Text="Select Plan Type" Value="" />

                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-12">
                                                        <asp:Label runat="server" CssClass="form-label"
                                                            Text="Bank Name" />
                                                        <asp:DropDownList runat="server" ID="ddlMutualBank" CssClass="form-select">
                                                            <asp:ListItem Text="Select Bank Name" Value="" />

                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-6">
                                                        <asp:Label runat="server" CssClass="form-label"
                                                            Text="Remark" />
                                                        <asp:TextBox runat="server" ID="txtRemark" MaxLength="150" CssClass="form-control" />
                                                    </div>

                                                    <div class="col-md-6 d-flex align-items-md-center gap-3 ">

                                                        <asp:RadioButton runat="server" ID="fresh" GroupName="freshRen"
                                                            Text="Fresh" />

                                                        <asp:RadioButton runat="server" ID="renewal" GroupName="freshRen"
                                                            Text="Renewal" />

                                                    </div>

                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-md-7 grid-margin stretch-card">
                                        <div class="card">
                                            <div class="card-body">
                                                <div class="row g-3">
                                                    <h5 class="mb-3">Payment Details:</h5>

                                                    <%-- Cheque, draft, cash --%>
                                                    <div class="col-md-8">
                                                        <div class="col-md-4 d-flex align-items-center gap-3 mt-3">
                                                            <%--OnCheckedChanged="PaymentTypeChanged" AutoPostBack="True"--%>
                                                            <asp:RadioButton runat="server" ID="cheque" GroupName="payment-type" Text="Cheque" OnCheckedChanged="PaymentTypeChanged" AutoPostBack="True" />
                                                            <asp:RadioButton runat="server" ID="draft" GroupName="payment-type" Text="Draft" OnCheckedChanged="PaymentTypeChanged" AutoPostBack="True" />
                                                            <asp:RadioButton runat="server" ID="optCash" GroupName="payment-type" Text="Cash" OnCheckedChanged="PaymentTypeChanged" AutoPostBack="True" />

                                                        </div>
                                                    </div>

                                                    <%-- Amount --%>
                                                    <div class="col-md-4">
                                                        <asp:Label runat="server" CssClass="form-label"
                                                            Text="Amount" />
                                                        <asp:TextBox runat="server" ID="txtAmount"
                                                            CssClass="form-control"
                                                            placeholder="Enter amount (e.g., 123456789012.34)"
                                                            oninput="validateAmount(this)" />
                                                        <script>
                                                            function validateAmount(input) {
                                                                const errorMessage = document.getElementById("errorMessage");

                                                                // Allow only numbers, one decimal point, and up to 2 decimal places
                                                                const regex = /^\d{0,14}(\.\d{0,2})?$/;

                                                                // Get the current input value
                                                                let value = input.value;

                                                                if (!regex.test(value)) {
                                                                    // If the value doesn't match, trim the last character
                                                                    input.value = value.slice(0, -1);
                                                                    errorMessage.textContent = "Invalid input! Enter up to 14 digits before the decimal point and 2 after.";
                                                                } else {
                                                                    // Clear the error message if the value is valid
                                                                    errorMessage.textContent = "";
                                                                }

                                                                // Ensure the value is numeric and doesn't exceed 16 digits in total
                                                                if (value.replace('.', '').length > 16) {
                                                                    input.value = value.slice(0, 16);
                                                                }
                                                            }
                                                        </script>

                                                    </div>

                                                    <%-- Cheque no --%>
                                                    <div class="col-md-8">
                                                        <asp:Label runat="server" CssClass="form-label" ID="ChequeLabel" Text="Cheque No <span class='text-danger'>*</span>" />
                                                        <asp:TextBox runat="server" ID="txtChequeNo" CssClass="form-control" MaxLength="20" />
                                                    </div>

                                                    <%-- Cheque date --%>
                                                    <div class="col-md-4">
                                                        <asp:Label runat="server" CssClass="form-label" ID="ChequeDatedLabel" Text="Cheque Dated <span class='text-danger'>*</span>" />
                                                        <div class="date" data-provide="datepicker">
                                                            <asp:TextBox runat="server" ID="txtChequeDate" CssClass="form-control" />
                                                            <div class="input-group-addon">
                                                                <span class="glyphicon glyphicon-th"></span>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-md-12">
                                                        <div class="mt-5 gap-3">
                                                            <asp:Button runat="server" Enabled="true" CssClass="btn btn-primary" Text="Generate AR" OnClick="btnInsertAdvisoryTransaction_Click" />
                                                            <asp:Button runat="server" Enabled="true" CssClass="btn btn-primary" Text="Print AR" OnClick="btnPrintARReport_Click" />
                                                            <asp:Button runat="server" Enabled="true" CssClass="btn btn-outline-primary" Text="Modify AR" OnClick="btnUpdateAdvisoryTransaction_Click" />

                                                        </div>
                                                    </div>

                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                                <div>


                                    <div class="row g-3">

                                        <style>
                                            .rptsection {
                                                margin-bottom: 20px;
                                            }

                                            #transactionTable {
                                                width: 100%;
                                                border-collapse: collapse;
                                                margin-top: 10px;
                                            }

                                                #transactionTable, #transactionTable th, #transactionTable td {
                                                    border: 1px solid #000;
                                                }

                                                    #transactionTable th, #transactionTable td {
                                                        padding: 8px;
                                                        text-align: left;
                                                    }
                                        </style>
                           <script type="text/javascript">
                               function printDiv(divId) {
                                   var divContent = document.getElementById(divId).innerHTML;
                                   var printWindow = window.open('', '_blank', 'height=600,width=800');
                                   printWindow.document.write('<html><head><title>Print</title>');
                                   printWindow.document.write('<link rel="stylesheet" type="text/css" href="styles.css">');
                                   // Add inline styles for the print
                                   printWindow.document.write('<style>');
                                   printWindow.document.write('.rptsection { margin-bottom: 20px; }');
                                   printWindow.document.write('#transactionTable { width: 100%; border-collapse: collapse; margin-top: 10px; }');
                                   printWindow.document.write('#transactionTable, #transactionTable th, #transactionTable td { border: 1px solid #000; }');
                                   printWindow.document.write('#transactionTable th, #transactionTable td { padding: 8px; text-align: left; }');
                                   printWindow.document.write('</style>');
                                   printWindow.document.write('</head><body>');
                                   printWindow.document.write(divContent);
                                   printWindow.document.write('</body></html>');
                                   printWindow.document.close();
                                   printWindow.print();
                               }
                           </script>


 
<div id ="arPrintDataForPrintWindow" style ="display:none">
    <div class="rptsection" id="doctorInfo">
         
        <p><strong>Name:</strong> <asp:Literal ID="litDoctorName" runat="server"></asp:Literal></p>

        <p><strong>Address:</strong> <asp:Literal ID="litAddress" runat="server"></asp:Literal></p>
        <p><strong>Phone:</strong> <asp:Literal ID="litPhone" runat="server"></asp:Literal></p>
        <p><strong>Email:</strong> <asp:Literal ID="litEmail" runat="server"></asp:Literal></p>
        <p><strong>Client ID (New/Old):</strong> <asp:Literal ID="litClientNewOld" runat="server"></asp:Literal></p>

        <p><strong>Transaction Date:</strong> <asp:Literal ID="litTranDate" runat="server"></asp:Literal></p>
    </div>

    <div class="section" id="transactionDetails">
        <div id="header">Transaction Details</div>
        <table id="transactionTable">
            <thead>
                <tr>
                    <th>Sr. No.</th>
                    <th>Tran No.</th>
                    <th>Investor Name</th>
                    <th>Scheme Name</th>
                    <th>Cheque/DD No./Date</th>
                    <th>Bank Name</th>
                    <th>Amount (Rs.)</th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater ID="rptTransactions" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td><%# Container.ItemIndex + 1 %></td>
                            <td><%# Eval("tran_code") %></td>
                            <td><%# Eval("client") %></td>
                            <td><%# Eval("schmf") %></td>
                            <td><%# Eval("cheque_no") %></td>
                            <td><%# Eval("BANK_NAME") %></td>
                            <td><%# Eval("amount") %></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
        <p><strong>Total:</strong> <asp:Literal ID="litTotal" runat="server"></asp:Literal></p>
    </div>

    <div class="section" id="footerInfo">
        <p><strong>Printed By:</strong> <asp:Literal ID="litPrintedBy" runat="server"></asp:Literal></p>
        <p><strong>Branch:</strong> <asp:Literal ID="litBranch" runat="server"></asp:Literal></p>
        <p><strong>Company:</strong> <asp:Literal ID="litCompany" runat="server"></asp:Literal></p>
        <p><strong>Date:</strong> <asp:Literal ID="litPrintedDate" runat="server"></asp:Literal></p>
    </div>

    <footer id="footer">
        <p><strong>Disclaimer:</strong> For Investments In Mutual Funds, This Receipt Should Be Considered As Provisional / Temporary Receipt.</p>
    </footer>
</div>
                                    
                                    
                                    </div>

                                </div>
                            </div>
                        </div>
                    </div>
                    </div>
                </div>
            </div>
        </div>


        <asp:Label runat="server" ID="lblMessage" CssClass="text-danger" Text =" " />

     


</asp:Content>
