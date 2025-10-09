<%@ Page Title="NPS Transaction Punching" Language="C#" AutoEventWireup="true" CodeBehind="NpsTransactionPunching.aspx.cs" Inherits="WM.Masters.NpsTransactionPunching" MasterPageFile="~/vmSite.Master" %>



<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link rel="shortcut icon" href="../assets/images/favicon.png" />
    <link rel="stylesheet" href="../assets/vendors/mdi/css/materialdesignicons.min.css">
    <link rel="stylesheet" href="../assets/vendors/ti-icons/css/themify-icons.css">
    <link rel="stylesheet" href="../assets/vendors/css/vendor.bundle.base.css">
    <link rel="stylesheet" href="../assets/vendors/font-awesome/css/font-awesome.min.css">
    <link rel="stylesheet" href="../assets/vendors/bootstrap-datepicker/bootstrap-datepicker.min.css">
    <link rel="stylesheet" href="../assets/css/style.css">


    <!-- Loader HTML, CSS, JS -->
    <div id="serverLoader" class="loader-overlay" style="display: none;">
        <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Loading...</span>
        </div>
    </div>
    <style>
        .loader-overlay {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0, 0, 0, 0.5);
            display: flex;
            align-items: center;
            justify-content: center;
            z-index: 9999;
        }
    </style>

    <asp:HiddenField ID="hfFocusedControl_new" runat="server" />


    <script type="text/javascript">
        var lastFocusedElement = null;

        function storeFocus() {
            lastFocusedElement = document.activeElement;
            if (lastFocusedElement) {
                var hiddenField = document.getElementById('<%= hfFocusedControl_new.ClientID %>');
                if (hiddenField) {
                    hiddenField.value = lastFocusedElement.id;
                }
            }
        }

        function restoreFocus() {
            var hiddenField = document.getElementById('<%= hfFocusedControl_new.ClientID %>');
            if (hiddenField && hiddenField.value) {
                var elementToFocus = document.getElementById(hiddenField.value);
                if (elementToFocus) {
                    elementToFocus.focus();
                }
            }
        }

        function showServerLoader() {
            var loader = document.getElementById('serverLoader');
            if (loader) {
                loader.style.display = 'flex'; // or 'block' depending on your CSS
            }
        }

        function hideServerLoader() {
            var loader = document.getElementById('serverLoader');
            if (loader) {
                loader.style.display = 'none';
            }
        }

        // Hook into ASP.NET PageRequestManager events
        var prm = Sys.WebForms.PageRequestManager.getInstance();

        if (prm) {
            prm.add_beginRequest(function () {
                storeFocus();
                showServerLoader();
            });

            prm.add_endRequest(function () {
                hideServerLoader();
                restoreFocus();
            });
        }
    </script>



    <script type="text/javascript">
        // Function to trim the input if it exceeds the max length
        function trimInput(inputElement, maxLength) {
            if (inputElement.value.length > maxLength) {
                inputElement.value = inputElement.value.substring(0, maxLength);
            }
        }

        // Function to ensure only numeric values are allowed while typing
        function validateNumericInput(inputElement) {
            // Replace all non-numeric characters with empty string
            inputElement.value = inputElement.value.replace(/\D/g, '');
        }

        // Function to prevent pasting of non-numeric characters
        function validatePaste(event, inputElement) {
            // Get pasted data
            var pastedData = (event.clipboardData || window.clipboardData).getData('text');

            // Only allow numbers in the pasted data
            if (/\D/.test(pastedData)) {
                event.preventDefault(); // Prevent paste if it's not a number
            }
        }
    </script>


    <script>
        function validateNumericInput(input) {
            input.value = input.value.replace(/\D/g, ''); // Remove all non-digits
        }

        function trimInput(input, maxLength) {
            if (input.value.length > maxLength) {
                input.value = input.value.substring(0, maxLength);
            }
        }

        function validatePaste(event, input) {
            let paste = (event.clipboardData || window.clipboardData).getData('text');
            if (!/^\d{0,12}$/.test(paste)) {
                event.preventDefault(); // Only allow 0-12 digits
            }
        }

        function validatePRAN(input) {
            validateNumericInput(input);
            trimInput(input, 12);
        }

        // Called by ASP.NET CustomValidator
        function validatePranLength(sender, args) {
            const val = args.Value.trim();
            if (val === "") {
                args.IsValid = true; // empty is OK
            } else {
                args.IsValid = /^\d{12}$/.test(val); // must be exactly 12 digits
            }
        }
    </script>




    <div class="content-wrapper">
        <div class="page-header">
            <h3 class="page-title">Preformation - NCIS Acknowledgement to the subscriber (To be filled by POP/POP-SP)
            </h3>
        </div>

        <div class="row">
            <div class="grid-margin stretch-card">
                <div class="card">
                    <div class="card-body">
                        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
                            <ContentTemplate>
                                <script>
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
                                </script>

                                <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
                                <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.9.0/js/bootstrap-datepicker.min.js"></script>
                                <script>$(document).ready(function () {
                                        $('.input-group.date').datepicker({
                                            format: 'dd/mm/yyyy',
                                            autoclose: true, // Ensures the datepicker closes after a date is selected
                                            todayHighlight: true // Highlights today's date for better user experience
                                        });
                                    });
                                </script>

                                <%-- NPS FORM AND BUTTONS --%>
                                <div class="row g-3">
                                    <%-- NEW PENSION SCHEME --%>
                                    <div class="col-md-3">
                                        <label for="ddlProductClass" class="form-label">Product Class <span class="text-danger">*</span></label>
                                        <asp:DropDownList ID="ddlProductClass" runat="server" CssClass="form-select">
                                            <asp:ListItem Text="New Pension Scheme" Value="IS02520" />
                                        </asp:DropDownList>
                                    </div>

                                    <%-- CORPORATE DDL --%>
                                    <div class="col-md-2">
                                        <label for="ddlType" class="form-label">Type</label>
                                        <asp:DropDownList ID="ddlType" runat="server" CssClass="form-select" AutoPostBack="true"
                                            OnSelectedIndexChanged="ddlType_SelectedIndexChanged">
                                            <asp:ListItem Text="Individual" Value="0" />
                                            <asp:ListItem Text="Corporate" Value="1" />
                                        </asp:DropDownList>
                                    </div>

                                    <%-- CORPORATE NAME --%>
                                    <div class="col-md-3">
                                        <div id="corporateNameDiv">
                                            <label for="corporateName" class="form-label">Corporate Name</label>
                                            <asp:TextBox ID="corporateName" runat="server" CssClass="form-control" Enabled="false"
                                                Placeholder="Enter Corporate Name" MaxLength="50"></asp:TextBox>
                                        </div>
                                    </div>

                                    <%-- DT Number --%>
                                    <div class="col-md-4">
                                        <label for="txtDtNumber" class="form-label">DT Number</label>
                                        <div class="input-group">
                                            <asp:TextBox ID="txtDtNumber" runat="server" CssClass="form-control" Placeholder="DT Number" oninput="validateMobileNumber(this, 15)"></asp:TextBox>
                                            <asp:Button ID="btnShow" runat="server" Text="Show" CssClass="btn btn-outline-primary" OnClick="btnCmdShow" OnClientClick="showServerLoader(); return true;" />
                                        </div>
                                    </div>

                                    <%-- SEARCH INVESTOR --%>
                                    <div class="col-md-4">
                                        <label for="txtInvestorCode" class="form-label">Investor Code</label>
                                        <div class="input-group">
                                            <asp:TextBox ID="txtInvestorCode" runat="server" CssClass="form-control" oninput="validateMobileNumber(this, 20)" ReadOnly="true"></asp:TextBox>
                                            <%--<asp:Button ID="btnSearchInverstor" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearchInvestor_Click" />--%>
                                            <asp:LinkButton ID="LbtnSearchInverstor" runat="server" CssClass="btn btn-primary" ToolTip="Investor Search" OnClick="btnSearchInvestor_Click">
<i class="fa fa-book"></i> Search
                                            </asp:LinkButton>
                                        </div>
                                    </div>

                                    <%-- SEARCH AR --%>
                                    <div class="col-md-6 offset-md-2">
                                        <label for="txtSearchAr" class="form-label">Search AR</label>
                                        <div class="input-group">
                                            <asp:TextBox ID="lblArNo" runat="server" CssClass="form-control text-danger" Text="" ReadOnly="true"></asp:TextBox>
                                            <%--<asp:Button ID="btnSearchAr" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearchAR_Click" />--%>
                                            <asp:LinkButton ID="LbtnSearchAr" runat="server" CssClass="btn btn-primary" ToolTip="AR Search" OnClick="btnSearchAR_Click">
        <i class="fa fa-search"></i> Search
                                            </asp:LinkButton>
                                        </div>
                                    </div>

                                    <%-- TIER PENSION SCHEME --%>
                                    <div class="col-md-4">
                                        <label class="form-label">Select Scheme<span class="text-danger">*</span></label>
                                        <asp:DropDownList ID="ddlScheme" runat="server" CssClass="form-select" AutoPostBack="true"
                                            OnSelectedIndexChanged="ddlScheme_SelectedIndexChanged">
                                            <asp:ListItem Text="New Pension Scheme Tier 1" Value="OP#09971"></asp:ListItem>
                                            <asp:ListItem Text="New Pension Scheme Tier 2" Value="OP#09972"></asp:ListItem>
                                            <asp:ListItem Text="New Pension Scheme Tier 1+2" Value="OP#09973"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>

                                    <%-- CRA (NSDL, KARVY - 0,1) --%>
                                    <div class="col-md-2">
                                        <label for="ddlCra" class="form-label">CRA</label>
                                        <asp:DropDownList ID="ddlCra" runat="server" CssClass="form-select" AutoPostBack="True" OnSelectedIndexChanged="cboKRA_SelectedIndexChanged_SelectedIndexChanged">
                                            <asp:ListItem Text="NSDL" Value="0" Selected="True" />
                                            <asp:ListItem Text="Karvy" Value="1" />
                                        </asp:DropDownList>
                                    </div>

                                    <%-- CRA BRANCH --%>
                                    <div class="col-md-2">
                                        <label for="ddlCraBranch" class="form-label">CRA Branch</label>
                                        <asp:DropDownList ID="ddlCraBranch" runat="server" CssClass="form-select">
                                            <asp:ListItem Text="NEHRU PLACE" Value="10010041" />
                                        </asp:DropDownList>
                                    </div>

                                    <%-- POP SP REGISTRATION NUMBER --%>
                                    <div class="col-md-4">
                                        <label for="txtPopSpRegNo" class="form-label">POP-SP Registration Number</label>
                                        <asp:TextBox ID="txtPopSpRegNo" runat="server" CssClass="form-control" oninput="validateMobileNumber(this, 20)"></asp:TextBox>
                                    </div>

                                    <%-- BUSINESS RM --%>
                                    <div class="col-md-4">
                                        <label for="txtBusinessRm" class="form-label">Business RM</label>
                                        <asp:TextBox ID="txtBusinessRm" Enabled="false" runat="server" CssClass="form-control" oninput="validateMobileNumber(this, 10)"></asp:TextBox>
                                    </div>

                                    <%-- BUSINESS BRANCH --%>
                                    <div class="col-md-4">
                                        <label for="ddlBusinessBranch" class="form-label text-danger">Business Branch</label>
                                        <asp:DropDownList ID="ddlBusinessBranch" runat="server" CssClass="form-select" Enabled="false">
                                            <asp:ListItem Text="Select" Value="" />
                                            <asp:ListItem Text="Branch 1" Value="branch1" />
                                            <asp:ListItem Text="Branch 2" Value="branch2" />
                                        </asp:DropDownList>
                                    </div>

                                    <%-- RECEIPT NUMBER --%>
                                    <div class="col-md-4">
                                        <label for="txtReceiptNo" class="form-label">Receipt No</label>
                                        <asp:TextBox ID="txtReceiptNo" runat="server" CssClass="form-control" oninput="validateMobileNumber(this, 20)"></asp:TextBox>
                                    </div>

                                    <%-- PAYMENT METHOD --%>
                                    <div class="col-md-12">
                                        <div class="border rounded-2 p-3">
                                            <div class="row g-3">
                                                <%-- PAYMENT METHOD --%>
                                                <div class="col-md-4">
                                                    <asp:Label ID="lblPaymentMethod" runat="server" Text="Payment Method" CssClass="form-label" />

                                                    <asp:DropDownList ID="ddlPaymentMethod" runat="server" CssClass="form-select" AutoPostBack="true"
                                                        OnSelectedIndexChanged="ddlPaymentMethod_SelectedIndexChanged">
                                                        <asp:ListItem Text="Select Payment Method" Value="" />
                                                        <asp:ListItem Text="Cheque" Value="C" />
                                                        <asp:ListItem Text="Draft" Value="D" />
                                                        <asp:ListItem Text="Cash" Value="H" />
                                                        <asp:ListItem Text="ECS" Value="E" />
                                                        <asp:ListItem Text="Corporate NECS" Value="M" />
                                                        <asp:ListItem Text="Others" Value="R" />
                                                    </asp:DropDownList>


                                                </div>

                                                <%-- BANK NAME --%>
                                                <div class="col-md-4">
                                                    <asp:Label ID="lblBankName" runat="server" Text="Bank Name" CssClass="form-label" />
                                                    <asp:DropDownList ID="ddlBankName" runat="server" CssClass="form-select">
                                                        <asp:ListItem Text="Select" Value="" />
                                                    </asp:DropDownList>
                                                </div>

                                                <%-- CHEQUE NO --%>
                                                <div class="col-md-2">
                                                    <asp:Label ID="lblNumber" runat="server" Text="Number" CssClass="form-label"></asp:Label>
                                                    <asp:TextBox Enabled="false" ID="txtChequeNo" runat="server" CssClass="form-control mb-3" Placeholder="Enter Number" MaxLength="50"></asp:TextBox>
                                                </div>

                                                <%--  AND DATE --%>
                                                <div class="col-md-2">
                                                    <asp:Label ID="lblDate" runat="server" Text="Date" CssClass="form-label"></asp:Label>
                                                    <div class="input-group">
                                                        <asp:TextBox Enabled="false" ID="txtChequeDated" CssClass="form-control" runat="server" MaxLength="10" oninput="formatDateInput(this)"></asp:TextBox>
                                                        <script>
                                                            // oninput="formatDateInput(this)"
                                                            function formatDateInput(input) {
                                                                // Remove all non-digit characters
                                                                let value = input.value.replace(/\D/g, '');

                                                                // Limit to max 8 digits (ddmmyyyy)
                                                                if (value.length > 8) {
                                                                    value = value.slice(0, 8);
                                                                }

                                                                // Add slashes after dd and mm
                                                                if (value.length > 4) {
                                                                    value = value.slice(0, 2) + '/' + value.slice(2, 4) + '/' + value.slice(4);
                                                                } else if (value.length > 2) {
                                                                    value = value.slice(0, 2) + '/' + value.slice(2);
                                                                }

                                                                input.value = value;
                                                            }
                                                        </script>


                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>


                                    <%-- No need to show as per anup sir --%>

                                    <%-- REQUEST TYPE  --%>
                                    <div class="col-md-2">
                                        <label for="requestType" class="form-label">Request Type</label>
                                        <asp:DropDownList ID="ddlRequestType" runat="server" CssClass="form-select"
                                            AutoPostBack="True" OnSelectedIndexChanged="ddlRequestType_SelectedIndexChanged">
                                            <asp:ListItem Text="Select" Value="" />
                                        </asp:DropDownList>
                                    </div>

                                    <%-- SUBSCRIBER NAME --%>
                                    <div class="col-md-4">
                                        <label for="nameOfSubscriber" class="form-label">Name of the Subscriber</label>
                                        <asp:TextBox ID="txtNameOfSubscriber" runat="server" CssClass="form-control" MaxLength="50" Enabled="false"></asp:TextBox>
                                    </div>

                                    <%-- PRAN --%>
                                    <div class="col-md-2">
                                        <label for="pran" class="form-label">PRAN</label>
                                        <asp:TextBox ID="txtPran" runat="server" CssClass="form-control"
                                            MaxLength="12"
                                            oninput="validatePRAN(this);"
                                            onpaste="validatePaste(event, this);">
                                        </asp:TextBox>

                                        <asp:CustomValidator ID="cvPran" runat="server"
                                            ControlToValidate="txtPran"
                                            ErrorMessage="PRAN must be exactly 12 digits if entered."
                                            ClientValidationFunction="validatePranLength"
                                            Display="Dynamic"
                                            ForeColor="Red">
                                        </asp:CustomValidator>

                                    </div>

                                    <%-- Date AND TAX CALCULATION --%>
                                    <div class="col-md-2">
                                        <label for="date" class="form-label">Date</label>
                                        <div class="input-group date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                            <asp:TextBox ID="txtDate" CssClass="form-control" runat="server" MaxLength="10" Enabled="false">z</asp:TextBox>
                                            <div class="input-group-addon">
                                                <span class="glyphicon glyphicon-th"></span>
                                            </div>
                                        </div>


                                    </div>

                                    <%-- TIME --%>
                                    <div class="col-md-2">
                                        <label for="time" class="form-label">Time</label>
                                        <div class="input-group time" data-provide="timepicker" data-minute-step="5" data-show-meridian="true">
                                            <asp:TextBox ID="txtTime" runat="server" CssClass="form-control timepicker" Enabled="false"
                                                TextMode="SingleLine" placeholder="HH:mm" MaxLength="5" oninput="formatTime(this)">
                                            </asp:TextBox>
                                            <div class="input-group-addon">
                                                <span class="glyphicon glyphicon-time"></span>
                                            </div>
                                        </div>
                                        <script>
                                            function formatTime(input) {
                                                let val = input.value.replace(/\D/g, ''); // Remove non-numeric characters

                                                if (val.length > 2) {
                                                    val = val.substring(0, 2) + ":" + val.substring(2, 4); // Insert `:` after 2 digits
                                                }

                                                if (val.length > 5) {
                                                    val = val.substring(0, 5); // Limit to HH:mm format
                                                }

                                                input.value = val;
                                            }

                                            document.addEventListener('DOMContentLoaded', function () {
                                                var txtTime = document.getElementById('<%= txtTime.ClientID %>');

                                                // Set current time on load in HH:mm format
                                                var now = new Date();
                                                var formattedTime = ('0' + now.getHours()).slice(-2) + ':' + ('0' + now.getMinutes()).slice(-2);
                                                txtTime.value = formattedTime;

                                                // Activate Bootstrap Timepicker
                                                $('.timepicker').timepicker({
                                                    minuteStep: 5,
                                                    showMeridian: true,
                                                    defaultTime: formattedTime
                                                });
                                            });
                                        </script>

                                    </div>

                                    <%-- AMT T1 --%>
                                    <div class="col-md-3">
                                        <label for="amountReceivedTire1" class="form-label">1. Amount Received Tire 1</label>
                                        <asp:TextBox ID="txtAmountReceivedTire1" runat="server" CssClass="form-control"
                                            oninput="validateMobileNumber(this, 10)"
                                            onkeydown="triggerPostBack(event)"
                                            OnTextChanged="ddlRequestType_SelectedIndexChanged" AutoPostBack="true"></asp:TextBox>
                                    </div>

                                    <script>

                                        function triggerPostBack(event) {
                                            if (event.key === "Tab") {
                                                showServerLoader();
                                            }
                                        }
                                    </script>

                                    <%-- REG CHARGES ONE TIME --%>
                                    <div class="col-md-3">
                                        <label for="popRegistrationChargesOneTime" class="form-label">3. POP Reg. Char: One time</label>
                                        <asp:TextBox ToolTip=" POP Registration Charges (One time)" ID="txtPopRegistrationChargesOneTime"
                                            Enabled="false"
                                            runat="server" CssClass="form-control" oninput="validateMobileNumber(this, 10)"></asp:TextBox>
                                    </div>

                                    <%-- GST --%>
                                    <div class="col-md-2">
                                        <label for="gst" class="form-label">GST as applicable</label>
                                        <asp:TextBox ID="txtGst" Enabled="false" runat="server" CssClass="form-control">0</asp:TextBox>
                                    </div>

                                    <%-- AMOUNT INVESTED --%>
                                    <div class="col-md-4">
                                        <asp:Label ID="lblInvestorAmountn" runat="server"
                                            CssClass="form-label" Text="Investor / Miscellaneous ">
                                        </asp:Label>
                                        <div class="d-flex mt-2">
                                            <asp:TextBox Enabled="false" ID="txtAmountInvested" runat="server" CssClass="form-control me-2" oninput="validateMobileNumber(this, 10)">0</asp:TextBox>
                                            <asp:TextBox Enabled="false" ID="txtAmountInvestedAdditional" runat="server" CssClass="form-control" oninput="validateMobileNumber(this, 10)"></asp:TextBox>
                                        </div>
                                    </div>

                                    <%-- AMT T2 --%>
                                    <div class="col-md-3">
                                        <label for="amountReceivedTire2" class="form-label">2. Amount Received Tire 2</label>
                                        <asp:TextBox ID="txtAmountReceivedTire2" runat="server" CssClass="form-control"
                                            oninput="validateMobileNumber(this, 10)"
                                            onkeydown="triggerPostBack(event)"
                                            OnTextChanged="ddlRequestType_SelectedIndexChanged" AutoPostBack="true"></asp:TextBox>
                                    </div>

                                    <%-- REG CHARGES --%>
                                    <div class="col-md-3">
                                        <label for="popRegistrationCharges" class="form-label">4. POP Reg. Charges</label>
                                        <asp:TextBox ToolTip="POP Registration Charges" ID="txtPopRegistrationCharges" runat="server"
                                            Enabled="false"
                                            CssClass="form-control" oninput="validateMobileNumber(this, 10)"></asp:TextBox>
                                    </div>

                                    <%-- COLLECTION AMOUNT --%>
                                    <div class="col-md-4">
                                        <label for="collectionAmount" class="form-label" oninput="validateMobileNumber(this, 10)">5. Collection Amount</label>
                                        <asp:TextBox Enabled="false" ID="txtCollectionAmount" runat="server" CssClass="form-control"
                                            oninput="validateMobileNumber(this, 10)"
                                            onkeydown="triggerPostBack(event)"
                                            OnTextChanged="ddlRequestType_SelectedIndexChanged" AutoPostBack="true"></asp:TextBox>
                                    </div>

                                    <%-- Unfreez checkbox --%>
                                    <div class="col-md-2 d-flex justify-content-center align-items-center">
                                        <div class="form-check">

                                            <label class="form-label" for="chkUnfreez">
                                                Unfreez
                                            </label>
                                            <div class="mt2">

                                                <asp:CheckBox ID="CheckBox1" CssClass="" runat="server" />
                                            </div>
                                        </div>
                                    </div>


                                    <div class="col-md-6">
                                        <div class="row ">
                                            <%-- REMARKS --%>
                                            <div class="col-md-12">
                                                <label for="txtRemark" class="form-label">Remark</label>
                                                <asp:TextBox ID="txtRemark" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                                            </div>


                                        </div>
                                    </div>
                                    <div class="col-md-6 mt-4">
                                        <div class="row mt-2">
                                            <%-- NPS BUTTONS AND HIDDEN FIELDS --%>
                                            <div class="col-md-3 pt-2 pe-2 pb-2">
                                                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="SaveButton_Click" />
                                            </div>
                                            <div class="col-md-3 p-2">
                                                <asp:Button ID="btnModify" runat="server" Enabled="false" Text="Modify" CssClass="btn btn-outline-primary" OnClientClick="return showConfirmation();" OnClick="ModifyButton_Click" />
                                            </div>

                                            <asp:HiddenField ID="hdnUserResponse" runat="server" />

                                            <div class="col-md-3 p-2">
                                                <asp:Button ID="btnReset" runat="server" OnClientClick="showServerLoader(); return true;" Text="Reset" CssClass="btn btn-outline-primary" OnClick="ResetButton_Click" />
                                            </div>
                                            <div class="col-md-3 p-2 pe-0">
                                                <asp:Button ID="btnExit" runat="server" OnClientClick="showServerLoader(); return true;" Text="Exit" CssClass="btn btn-outline-primary" OnClick="ExitButton_Click" />
                                            </div>
                                            <script type="text/javascript">
                                                function showConfirmation() {
                                                    var userResponse = confirm("Are you sure you want to proceed?");
                                                    document.getElementById('<%= hdnUserResponse.ClientID %>').value = userResponse; // Store the response
                                                    return userResponse; // Return the response to determine if the postback should proceed
                                                }
                                            </script>

                                        </div>
                                    </div>
                                    <div class="mt-2">

                                        <hr>
                                    </div>
                                    <div class="col-md-12 mt-4">
                                        <div class="row d-flex justify-content-between align-items-center">
                                            <%-- CHK ZERO COMMISSION --%>
                                            <div class="col-md-3">
                                                <div class="form-check d-flex align-items-center">
                                                    <label class="form-label mb-0" for="chkZeroCommission">
                                                        Import with Zero Commission
                                                    </label>
                                                    <div class="ms-2">
                                                        <asp:CheckBox ID="chkZeroCommission" runat="server" ToolTip="Import with Zero Commission" />
                                                    </div>
                                                </div>
                                            </div>


                                            <%-- ECS BUTTON --%>
                                            <div class="col-md-3 text-end">
                                                <asp:Button ID="btnImportEcs" OnClick="btnImportEcs_Click" runat="server"
                                                    Text="Import ECS Transaction" ToolTip="Import ECS Transaction"
                                                    CssClass="btn btn-outline-primary" Enabled="true" />
                                            </div>
                                            <%-- NON ESC BUTTON --%>
                                            <div class="col-md-3 text-start">
                                                <asp:Button ID="btnImportCorporateNonEsc" OnClick="btnImportCorporateNonEsc_Click" runat="server"
                                                    Text="Import Corporate Non-ESC Transaction" ToolTip="Import Corporate Non-ESC Transaction"
                                                    CssClass="btn btn-outline-primary" Enabled="true" />
                                            </div>

                                            <%-- PRINT BUTTON --%>
                                            <div class="col-md-3 text-end">
                                                <asp:Button ID="btnPrint" runat="server"
                                                    Text="Print"
                                                    CssClass="btn btn-outline-primary"
                                                    OnClick="PrintButton_Click" />
                                            </div>
                                        </div>

                                    </div>

                                    <asp:Label ID="lblMessage" runat="server" CssClass="text-danger mt-3" Visible="true"></asp:Label>
                                </div>
                            </ContentTemplate>
                            <Triggers>
                                <%-- Triggers for buttons controler --%>
                                <asp:AsyncPostBackTrigger ControlID="btnShow" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="LbtnSearchInverstor" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="LbtnSearchAr" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btnSave" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btnModify" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btnReset" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btnExit" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btnPrint" EventName="Click" />
                                <%--<asp:AsyncPostBackTrigger ControlID="btnExportToExcel" EventName="Click" />--%>
                                <%--<asp:PostBackTrigger ControlID="btnExportToExcel" />--%>

                                <asp:AsyncPostBackTrigger ControlID="btnImportEcs" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btnImportCorporateNonEsc" EventName="Click" />

                                <asp:AsyncPostBackTrigger ControlID="ddlPaymentMethod" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlScheme" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlCra" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlCraBranch" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlBusinessBranch" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlRequestType" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlBankName" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlProductClass" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlType" EventName="SelectedIndexChanged" />


                            </Triggers>
                        </asp:UpdatePanel>


                        <%-- EXPORT EXCLE DATA --%>
                        <div class="col-md-12 mt-2">
                            <%--<div class="border rounded-2 p-3">--%>
                            <div class="row g-3">
                                <!-- CRA DropDownList -->
                                <div class="col-md-2">
                                    <label for="cra" class="form-label">CRA</label>
                                    <asp:DropDownList ID="cra" runat="server" CssClass="form-select">
                                        <asp:ListItem Text="All" Value="0" Selected="True" />
                                        <asp:ListItem Text="NSDL" Value="1" />
                                        <asp:ListItem Text="Karvy" Value="2" />
                                    </asp:DropDownList>
                                </div>

                                <!-- Transaction Type RadioButtonList -->
                                <div class="gap-3 col-md-2">
                                    <label for="ecs-nonecs" class="form-label">ECS/Non ECS</label>

                                    <asp:DropDownList ID="rblTransactionType" runat="server" CssClass="form-select">
                                        <asp:ListItem Text="ALL" Value="ALL" />
                                        <asp:ListItem Text="ECS" Value="ECS" />
                                        <asp:ListItem Text="NON ECS" Value="NON_ECS" />
                                    </asp:DropDownList>
                                </div>

                                <!-- Date Pickers in Single Line -->
                                <div class="d-flex gap-3 col-md-4 ">
                                    <div class="flex-fill">
                                        <label for="fromDate" class="form-label">From Date</label>
                                        <div class="input-group date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                            <asp:TextBox ID="fromDate" placeholder="dd/mm/yyyy" oninput="onInputAddSlashInThisDate(this)" CssClass="form-control" runat="server"></asp:TextBox>
                                            <div class="input-group-addon">
                                                <span class="glyphicon glyphicon-th"></span>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="flex-fill">
                                        <label for="toDate" class="form-label">To Date</label>
                                        <div class="input-group date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                            <asp:TextBox ID="toDate" placeholder="dd/mm/yyyy" oninput="onInputAddSlashInThisDate(this)" CssClass="form-control" runat="server"></asp:TextBox>
                                            <div class="input-group-addon">
                                                <span class="glyphicon glyphicon-th"></span>
                                            </div>
                                        </div>
                                    </div>

                                    <script type="text/javascript">
                                        function onInputAddSlashInThisDate(input) {
                                            let value = input.value.replace(/\D/g, ''); // Remove non-digit characters

                                            // Insert slashes at correct positions
                                            if (value.length >= 3 && value[2] !== '/') {
                                                value = value.substring(0, 2) + '/' + value.substring(2);
                                            }
                                            if (value.length >= 6 && value[5] !== '/') {
                                                value = value.substring(0, 5) + '/' + value.substring(5);
                                            }

                                            // Limit to 10 characters (dd/mm/yyyy)
                                            if (value.length > 10) {
                                                value = value.substring(0, 10);
                                            }

                                            input.value = value;
                                        }
                                    </script>
                                </div>


                                <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.1/font/bootstrap-icons.css" rel="stylesheet">


                                <!-- Export Button -->
                                <div class="col-md-4 mt-5 d-flex justify-content-end">
                                    <asp:LinkButton ID="lnkResetExportButtons" runat="server" CssClass="reload-button me-4 mt-2" ToolTip="Reset Expoted Data"
                                        OnClick="lnkResetExportButtons_Click">
                                                    <i class="fa fa-refresh"></i> 
                                    </asp:LinkButton>
                                    <asp:Button ID="btnExportToExcel" runat="server" CssClass="btn btn-primary" Text="Export" OnClick="btnExportToExcel_Click" Enabled="TRUE" />


                                </div>
                            </div>
                            <%--</div>--%>
                            <div class="col-md-12">
                                <asp:PlaceHolder ID="pnlExportButtons" runat="server"></asp:PlaceHolder>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </div>

    <script src="../assets/vendors/js/vendor.bundle.base.js"></script>
    <script src="../assets/vendors/chart.js/chart.umd.js"></script>
    <script src="../assets/vendors/bootstrap-datepicker/bootstrap-datepicker.min.js"></script>
    <script src="../assets/js/off-canvas.js"></script>
    <script src="../assets/js/misc.js"></script>
    <script src="../assets/js/settings.js"></script>
    <script src="../assets/js/todolist.js"></script>
    <script src="../assets/js/jquery.cookie.js"></script>

    <script src="../assets/js/dashboard.js"></script>

</asp:Content>
