<%@ Page Title="NPS Transaction Punching" Language="C#" AutoEventWireup="true" CodeBehind="NpsTransactionPunching.aspx.cs" Inherits="WM.Masters.NpsTransactionPunching" MasterPageFile="~/vmSite.Master" %>



<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link rel="shortcut icon" href="../assets/images/favicon.png" />
    <link rel="stylesheet" href="../assets/vendors/mdi/css/materialdesignicons.min.css">
    <link rel="stylesheet" href="../assets/vendors/ti-icons/css/themify-icons.css">
    <link rel="stylesheet" href="../assets/vendors/css/vendor.bundle.base.css">
    <link rel="stylesheet" href="../assets/vendors/font-awesome/css/font-awesome.min.css">
    <link rel="stylesheet" href="../assets/vendors/bootstrap-datepicker/bootstrap-datepicker.min.css">
    <link rel="stylesheet" href="../assets/css/style.css"> 






    <script>


        // Function to open the modal and keep it open even after page reload
        function openModalPrintSaveRecietDataModel() {
            document.getElementById("printSaveRecietDataModel").style.display = "block";
            localStorage.setItem("printSaveRecietDataModelOpen", "true"); // Store state in localStorage
        }

        // Function to explicitly close the modal and update localStorage
        function closeModalPrintSaveRecietDataModel() {
            document.getElementById("printSaveRecietDataModel").style.display = "none";
            localStorage.setItem("printSaveRecietDataModelOpen", "false"); // Update state in localStorage
        }

        // Function to open another modal for Search AR model and store its state
        function openModalSearchArModel() {
            document.getElementById("SearchArModel").style.display = "block";
            localStorage.setItem("SearchArModelOpen", "true"); // Store state in localStorage
        }

        // Function to explicitly close the Search AR modal and update localStorage
        function closeModalSearchArModel() {
            document.getElementById("SearchArModel").style.display = "none";
            localStorage.setItem("SearchArModelOpen", "false"); // Update state in localStorage
        }

        // Check localStorage on page load to keep modal open if it was open before page reload
        window.onload = function () {
            if (localStorage.getItem("printSaveRecietDataModelOpen") === "true") {
                document.getElementById("printSaveRecietDataModel").style.display = "block";
            }
            if (localStorage.getItem("SearchArModelOpen") === "true") {
                document.getElementById("SearchArModel").style.display = "block";
            }
        };






    </script>

    <script type="text/javascript">
        $(document).ready(function () {
            // Initialize datepicker for AR From Date
            $('#arPopFromDatePicker').datepicker({
                format: 'dd/mm/yyyy',
                autoclose: true,
                todayHighlight: true
            });

            // Initialize datepicker for AR To Date
            $('#arPopToDatePicker').datepicker({
                format: 'dd/mm/yyyy',
                autoclose: true,
                todayHighlight: true
            });
        });

        $(document).ready(function () {
            $('.date').datepicker({
                format: 'dd/mm/yyyy',
                autoclose: true, // Ensures the datepicker closes automatically after selection
                todayHighlight: true // Highlights today's date
            });
        });

    </script>


    <!-- JS -->
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.9.0/js/bootstrap-datepicker.min.js"></script>

    <style>
        /* Modal styles */
        .modal {
            display: none;
            position: fixed;
            z-index: 99;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            overflow: auto;
            background-color: rgb(0,0,0);
            background-color: rgba(0,0,0,0.4);
        }

        .modal-content {
            background-color: #fefefe;
            margin: 15% auto;
            padding: 20px;
            border: 1px solid #888;
            width: 80%;
        }

        .close {
            color: #aaa;
            float: right;
            font-size: 28px;
            font-weight: bold;
        }
    </style>



    <script type="text/javascript">
        function togglePaymentFields() {
            var paymentMethod = document.getElementById('<%= ddlPaymentMethod.ClientID %>').value;
            var chequeNoField = document.getElementById('<%= txtChequeNo.ClientID %>');
            var chequeDateField = document.getElementById('<%= txtChequeDated.ClientID %>');
            var bankNameField = document.getElementById('<%= ddlBankName.ClientID %>');

            var lblNumber = document.getElementById("lblNumber");
            var lblDate = document.getElementById("lblDate");

            // Reset field states and labels
            chequeNoField.disabled = true;
            chequeDateField.disabled = true;
            bankNameField.disabled = true;

            lblNumber.textContent = "Number";
            lblDate.textContent = "Date";

            // Update field availability and labels based on payment method
            if (paymentMethod === "C") {  // Cheque
                chequeNoField.disabled = false;
                chequeDateField.disabled = false;
                bankNameField.disabled = false;
                lblNumber.textContent = "Cheque No";
                lblDate.textContent = "Cheque Dated";
            } else if (paymentMethod === "D") {  // Draft
                chequeNoField.disabled = false;
                chequeDateField.disabled = false;
                bankNameField.disabled = false;
                lblNumber.textContent = "Draft No";
                lblDate.textContent = "Draft Dated";
            } else if (paymentMethod === "E" || paymentMethod === "H") {  // ECS or Corporate NON ECS
                chequeNoField.disabled = true;
                chequeDateField.disabled = true;
                bankNameField.disabled = true;
                lblNumber.textContent = "Number";
                lblDate.textContent = "Date";
            }
        }
        document.addEventListener("DOMContentLoaded", togglePaymentFields);

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
                        <div class="row g-3">
                            <div class="col-md-3">
                                <label for="ddlProductClass" class="form-label">Product Class <span class="text-danger">*</span></label>
                                <asp:DropDownList ID="ddlProductClass" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="New Pension Scheme" Value="IS02520" />

                                </asp:DropDownList>
                            </div>
                            <div class="col-md-2">
                                <label for="ddlType" class="form-label">Type</label>
                                <asp:DropDownList ID="ddlType" runat="server" CssClass="form-select" onchange="toggleCorporateName()">
                                    <asp:ListItem Text="Select Type" Value="" />
                                    <asp:ListItem Text="Individual" Value="0" />
                                    <asp:ListItem Text="Corporate" Value="1" />
                                </asp:DropDownList>
                            </div>

                            <div class="col-md-3">
                                <div id="corporateNameDiv">
                                    <label for="corporateName" class="form-label">Corporate Name</label>
                                    <asp:TextBox ID="corporateName" runat="server" CssClass="form-control" Placeholder="Enter Corporate Name" MaxLength="50"></asp:TextBox>
                                </div>
                            </div>

                            <script type="text/javascript">
                                function toggleCorporateName() {
                                    // Get the dropdown and corporate name textbox elements by their IDs
                                    var ddlType = document.getElementById('<%= ddlType.ClientID %>');
                                    var corporateName = document.getElementById('<%= corporateName.ClientID %>');

                                    // Check the selected value of ddlType
                                    var selectedValue = ddlType.value;

                                    // Enable or disable the Corporate Name textbox based on the selected value
                                    if (selectedValue === '1') {
                                        corporateName.disabled = false; // Enable the textbox
                                    } else {
                                        corporateName.disabled = true; // Disable the textbox
                                    }
                                }

                                // Call toggleCorporateName on page load to set the initial state
                                document.addEventListener("DOMContentLoaded", function () {
                                    toggleCorporateName();
                                });

                                // Add an event listener to call toggleCorporateName when the dropdown value changes
                                document.getElementById('<%= ddlType.ClientID %>').addEventListener('change', toggleCorporateName);
                            </script>





                            <div class="col-md-4">
                                <label for="txtDtNumber" class="form-label">DT Number</label>
                                <div class="input-group">
                                    <asp:TextBox ID="txtDtNumber" runat="server" CssClass="form-control" Placeholder="DT Number" oninput="validateMobileNumber(this, 15)"></asp:TextBox>
                                    <asp:Button ID="btnShow" runat="server" Text="Show" CssClass="btn btn-outline-primary" OnClick="btnCmdShow" />
                                    <%--<asp:Button ID="btnSearch" runat="server" Text="&#128269;" CssClass="btn btn-outline-primary" />--%>
                                </div>
                            </div>

                            <%--  <div class="col-md-5" style="display: none">
                                <label class="form-label">Search Investor</label>
                                <br>
                                <asp:Button ID="btnSearchInvestor" runat="server" Text="Search" CssClass="btn btn-outline-primary" />
                            </div>--%>

                            <div class="col-md-6">
                                <label for="txtInvestorCode" class="form-label">Investor Code</label>
                                <div class="input-group">
                                    <asp:TextBox ID="txtInvestorCode" runat="server" CssClass="form-control" oninput="validateMobileNumber(this, 20)"></asp:TextBox>
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
                                    <%--<asp:Button ID="btnSearchInverstor" runat="server" Text="Search" CssClass="btn btn-primary" OnClientClick="openModalSearchArModel(); return false;" />--%>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <label for="txtSearchAr" class="form-label">Search AR</label>
                                <div class="input-group">
                                    <asp:TextBox ID="lblArNo" runat="server" CssClass="form-control text-danger" Text="0" ReadOnly="true"></asp:TextBox>
                                    <asp:Button ID="btnSearchAr" runat="server" Text="Search" CssClass="btn btn-primary" OnClientClick="openModalSearchArModel(); return false;" />
                                </div>
                            </div>

                            <br />
                            <div class="col-md-4">
                                <label class="form-label">Select Scheme <span class="text-danger">*</span></label>
                                <asp:DropDownList ID="ddlScheme" runat="server" CssClass="form-select" onchange="toggleFields()">
                                    <asp:ListItem Text="Select Pension Scheme" Value="" Selected="True"></asp:ListItem>

                                    <asp:ListItem Text="New Pension Scheme Tier 1"   Value="OP#09971"></asp:ListItem>
                                    <asp:ListItem Text="New Pension Scheme Tier 2"   Value="OP#09972"></asp:ListItem>
                                    <asp:ListItem Text="New Pension Scheme Tier 1+2" Value="OP#09973"></asp:ListItem>
                                </asp:DropDownList>
                            </div>

                            <script type="text/javascript">
                                function toggleSchemaFields() {
                                    // Get the dropdown and textboxes by their IDs
                                    var ddlScheme = document.getElementById('<%= ddlScheme.ClientID %>');
                                   var txtAmountReceivedTire1 = document.getElementById('<%= txtAmountReceivedTire1.ClientID %>');
                                   var txtAmountReceivedTire2 = document.getElementById('<%= txtAmountReceivedTire2.ClientID %>');

                                    // Get the selected value of the dropdown
                                    var selectedValue = ddlScheme.value;

                                    // Enable or disable textboxes based on the selected value
                                    if (selectedValue === 'OP#09971') {
                                        txtAmountReceivedTire1.disabled = false; // Enable Tier 1 field
                                        txtAmountReceivedTire2.disabled = true;  // Disable Tier 2 field
                                    } else if (selectedValue === 'OP#09972') {
                                        txtAmountReceivedTire1.disabled = true;  // Disable Tier 1 field
                                        txtAmountReceivedTire2.disabled = false; // Enable Tier 2 field
                                    } else if (selectedValue === 'OP#09973') {
                                        txtAmountReceivedTire1.disabled = false; // Enable Tier 1 field
                                        txtAmountReceivedTire2.disabled = false; // Enable Tier 2 field
                                    } else {
                                        // Disable both fields for any other or default selection
                                        txtAmountReceivedTire1.disabled = true;
                                        txtAmountReceivedTire2.disabled = true;
                                    }
                                }

                                // Call toggleFields on page load to set the initial state
                                document.addEventListener("DOMContentLoaded", function () {
                                    toggleSchemaFields();
                                });

                                // Add an event listener to call toggleFields when the dropdown value changes
                                document.getElementById('<%= ddlScheme.ClientID %>').addEventListener('change', toggleSchemaFields);
                            </script>


                            <div class="col-md-2">
                                <label for="ddlCra" class="form-label">CRA</label>
                                <asp:DropDownList ID="ddlCra" runat="server" CssClass="form-select" AutoPostBack="True" OnSelectedIndexChanged="cboKRA_SelectedIndexChanged_SelectedIndexChanged">
                                    <asp:ListItem Text="NSDL" Value="0" Selected="True" />
                                    <asp:ListItem Text="Karvy" Value="1" />
                                </asp:DropDownList>
                            </div>

                            <div class="col-md-3">
                                <label for="ddlCraBranch" class="form-label">CRA Branch</label>
                                <asp:DropDownList ID="ddlCraBranch" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="NEHRU PLACE" Value="10010041" />
                                </asp:DropDownList>
                            </div>

                            <div class="col-md-3">
                                <label for="txtPopSpRegNo" class="form-label">POP-SP Registration Number</label>
                                <asp:TextBox ID="txtPopSpRegNo" runat="server" CssClass="form-control" oninput="validateMobileNumber(this, 20)"></asp:TextBox>
                            </div>

                            <script type="text/javascript">
                                function updateRegFolioNo() {
                                    var ddlCra = document.getElementById('<%= ddlCra.ClientID %>');
                                    var hfFolioNo = document.getElementById('<%= txtPopSpRegNo.ClientID %>');

                                    // Check the selected value and set folio number
                                    if (ddlCra.value === "0") {
                                        hfFolioNo.value = '6036914';
                                    } else if (ddlCra.value === "1") {
                                        hfFolioNo.value = '1171966';
                                    }
                                }



                                // Call toggleFields on page load to set the initial state
                                document.addEventListener("DOMContentLoaded", updateRegFolioNo);
                            </script>

                            <div class="col-md-4">
                                <label for="txtBusinessRm" class="form-label">Business RM</label>
                                <asp:TextBox ID="txtBusinessRm" runat="server" CssClass="form-control" oninput="validateMobileNumber(this, 10)"></asp:TextBox>
                            </div>

                            <div class="col-md-4">
                                <label for="ddlBusinessBranch" class="form-label text-danger">Business Branch</label>
                                <asp:DropDownList ID="ddlBusinessBranch" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="Select" Value="" />
                                    <asp:ListItem Text="Branch 1" Value="branch1" />
                                    <asp:ListItem Text="Branch 2" Value="branch2" />
                                </asp:DropDownList>
                            </div>

                            <div class="col-md-4">
                                <label for="txtReceiptNo" class="form-label">Receipt No</label>
                                <asp:TextBox ID="txtReceiptNo" runat="server" CssClass="form-control" oninput="validateMobileNumber(this, 20)"></asp:TextBox>
                            </div>

                            <div class="col-md-12">
                                <div class="border rounded-2 p-3">
                                    <div class="row g-3">
                                        <div class="col-md-4">
                                            <label class="form-label">Payment Method</label>
                                            <asp:DropDownList ID="ddlPaymentMethod" runat="server" CssClass="form-select" onchange="togglePaymentFields()">
                                                <asp:ListItem Text="Select Payment Method" Value="" />
                                                <asp:ListItem Text="Cheque" Value="C" />
                                                <asp:ListItem Text="Draft" Value="D" />
                                                <asp:ListItem Text="ECS" Value="E" />
                                                <asp:ListItem Text="Corporate NON ECS" Value="H" />
                                                <asp:ListItem Text="Others" Value="R" />

                                            </asp:DropDownList>

                                        </div>

                                        <div class="col-md-4">
                                            <label for="ddlBankName" class="form-label">Bank Name</label>
                                            <asp:DropDownList ID="ddlBankName" runat="server" CssClass="form-select">
                                                <asp:ListItem Text="Select" Value="" />
                                                <asp:ListItem Text="Bank 1" Value="bank1" />
                                                <asp:ListItem Text="Bank 2" Value="bank2" />
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-md-4">
                                            <div id="paymentDetails" class="row">
                                                <div class="col-md-6">
                                                    <label id="lblNumber" class="form-label" for="txtChequeNo">Cheque No</label>
                                                    <asp:TextBox ID="txtChequeNo" runat="server" CssClass="form-control mb-3" Placeholder="Enter Number" oninput="validateMobileNumber(this, 10)"></asp:TextBox>
                                                </div>
                                                <div class="col-md-6">
                                                    <label id="lblDate" class="form-label" for="txtChequeDated">Cheque Dated</label>

                                                    <div class="input-group date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                        <asp:TextBox ID="txtChequeDated" CssClass="form-control" runat="server"></asp:TextBox>
                                                        <div class="input-group-addon">
                                                            <span class="glyphicon glyphicon-th"></span>
                                                        </div>
                                                    </div>

                                                </div>
                                            </div>
                                        </div>



                                        <div class="col-md-6" style="display: none">
                                            <label class="form-label">For New Bank</label>
                                            <div class="input-group">
                                                <asp:TextBox ID="newBank" runat="server" CssClass="form-control" ReadOnly="True" Text="New Bank" />
                                                <asp:Button ID="btnAddNewBank" runat="server" CssClass="btn btn-outline-primary" Text="+" OnClick="btnAddNewBank_Click" />
                                            </div>
                                        </div>



                                    </div>
                                </div>
                            </div>



                            <div class="col-md-6" style="display: none">
                                <div class="border rounded-2 p-3">
                                    <div class="row g-3">
                                        <!-- CRA DropDownList -->
                                        <div class="col-md-6">
                                            <label for="cra" class="form-label">CRA</label>
                                            <asp:DropDownList ID="cra" runat="server" CssClass="form-select" Enabled="false">
                                                <asp:ListItem Text="All" Value="0" Selected="True" />
                                                <asp:ListItem Text="NSDL" Value="1" />
                                                <asp:ListItem Text="Karvy" Value="2" />
                                            </asp:DropDownList>
                                        </div>

                                        <!-- Transaction Type RadioButtonList -->
                                        <div class="gap-3 col-md-6">
                                            <label for="ecs-nonecs" class="form-label">ECS/Non ECS</label>

                                            <asp:DropDownList ID="rblTransactionType" runat="server" CssClass="form-select" Enabled="false">
                                                <asp:ListItem Text="ALL" Value="all" />
                                                <asp:ListItem Text="ECS" Value="ecs" />
                                                <asp:ListItem Text="NON ECS" Value="nonEcs" />
                                            </asp:DropDownList>
                                        </div>

                                        <!-- Date Pickers in Single Line -->
                                        <div class="d-flex gap-3">
                                            <div class="flex-fill">
                                                <label for="fromDate" class="form-label">From Date</label>
                                                <div class="input-group date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                    <asp:TextBox ID="fromDate" CssClass="form-control" runat="server" Enabled="false"></asp:TextBox>
                                                    <div class="input-group-addon">
                                                        <span class="glyphicon glyphicon-th"></span>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="flex-fill">
                                                <label for="toDate" class="form-label">To Date</label>
                                                <div class="input-group date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                    <asp:TextBox ID="toDate" CssClass="form-control" runat="server" Enabled="false"></asp:TextBox>
                                                    <div class="input-group-addon">
                                                        <span class="glyphicon glyphicon-th"></span>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.9.0/css/bootstrap-datepicker.min.css">
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


                                        <!-- Export Button -->
                                        <div class="col-md-12">
                                            <asp:Button ID="btnExportToExcel" runat="server" CssClass="btn btn-primary" Text="Export to Excel" OnClick="btnExportToExcel_Click" Enabled="false" />


                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-md-4">
                                <label for="requestType" class="form-label">Request Type</label>
                                <asp:DropDownList ID="ddlRequestType" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="Select" Value="" />
                                    <asp:ListItem Text="Type 1" Value="type1" />
                                    <asp:ListItem Text="Type 2" Value="type2" />
                                </asp:DropDownList>
                            </div>

                            <div class="col-md-2">
                                <label for="date" class="form-label">Date</label>
                                <div class="input-group date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                    <asp:TextBox ID="txtDate" CssClass="form-control" runat="server" MaxLength="10"></asp:TextBox>
                                    <div class="input-group-addon">
                                        <span class="glyphicon glyphicon-th"></span>
                                    </div>

                                    <script>
                                        document.addEventListener('DOMContentLoaded', function () {
                                            var dateInput = document.getElementById('<%= txtDate.ClientID %>');
                                            var gstInput = document.getElementById('<%= txtGst.ClientID %>');

                                            function setServiceTax() {
                                                var inputDate = new Date(dateInput.value.split('/').reverse().join('-')); // Parse dd/mm/yyyy to Date
                                                if (isNaN(inputDate)) {
                                                    gstInput.value = 'Invalid Date';
                                                    return;
                                                }

                                                // Service tax logic
                                                var serviceTax;
                                                var date1 = new Date('2015-06-01');
                                                var date2 = new Date('2015-11-15');
                                                var date3 = new Date('2016-06-01');
                                                var date4 = new Date('2017-06-01');
                                                var date5 = new Date('2017-07-01');

                                                if (inputDate < date1) {
                                                    serviceTax = 0.1236;
                                                } else if (inputDate >= date1 && inputDate < date2) {
                                                    serviceTax = 0.14;
                                                } else if (inputDate >= date2 && inputDate < date3) {
                                                    serviceTax = 0.145;
                                                } else if (inputDate >= date4 && inputDate < date5) {
                                                    serviceTax = 0.15;
                                                } else if (inputDate >= date5) {
                                                    serviceTax = 0.18;
                                                }

                                                gstInput.value = serviceTax ? serviceTax.toFixed(4) : '0';
                                            }

                                            // Check on load
                                            setServiceTax();

                                            // Check on change
                                            dateInput.addEventListener('change', setServiceTax);
                                        });

                                    </script>
                                </div>
                            </div>

                            <div class="col-md-2">
                                <label for="time" class="form-label">Time</label>
                                <div class="input-group time" data-provide="timepicker" data-minute-step="5" data-show-meridian="true">
                                    <asp:TextBox ID="txtTime" runat="server" CssClass="form-control" TextMode="SingleLine" placeholder="HH:mm" MaxLength="5"></asp:TextBox>
                                    <div class="input-group-addon">
                                        <span class="glyphicon glyphicon-time"></span>
                                    </div>
                                </div>
                            </div>


                            <div class="col-md-4">
                                <label for="nameOfSubscriber" class="form-label">Name of the Subscriber</label>
                                <asp:TextBox ID="txtNameOfSubscriber" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                            </div>

                            <div class="col-md-4">
                                <label for="pran" class="form-label">PRAN</label>
                                <asp:TextBox ID="txtPran" runat="server" CssClass="form-control" oninput="validateMobileNumber(this, 20)"></asp:TextBox>
                            </div>
                            <div class="col-md-2 d-flex justify-content-center align-items-center">
                                <div class="form-check">

                                    <label class="form-check-label" for="chkUnfreez">
                                        Unfreez
                                        <asp:CheckBox ID="CheckBox1" runat="server" />
                                    </label>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <label for="amountReceivedTire1" class="form-label">1. Amount Received Tire 1</label>
                                <asp:TextBox ID="txtAmountReceivedTire1" runat="server" CssClass="form-control" oninput="validateMobileNumber(this, 10)"></asp:TextBox>
                            </div>

                            <div class="col-md-3">
                                <label for="amountReceivedTire2" class="form-label">2. Amount Received Tire 2</label>
                                <asp:TextBox ID="txtAmountReceivedTire2" runat="server" CssClass="form-control" oninput="validateMobileNumber(this, 10)"></asp:TextBox>
                            </div>


                            <div class="col-md-4">
                                <label for="popRegistrationChargesOneTime" class="form-label">3. POP Registration Charges (One time)</label>
                                <asp:TextBox ID="txtPopRegistrationChargesOneTime" runat="server" CssClass="form-control" oninput="validateMobileNumber(this, 10)"></asp:TextBox>
                            </div>

                            <div class="col-md-4">
                                <label for="popRegistrationCharges" class="form-label">4. POP Registration Charges</label>
                                <asp:TextBox ID="txtPopRegistrationCharges" runat="server" CssClass="form-control" oninput="validateMobileNumber(this, 10)">0</asp:TextBox>
                            </div>

                            <div class="col-md-4">
                                <label for="gst" class="form-label">GST as applicable</label>
                                <asp:TextBox ID="txtGst" runat="server" CssClass="form-control" >0</asp:TextBox>
                            </div>

                            <div class="col-md-4">
                                <label for="collectionAmount" class="form-label" oninput="validateMobileNumber(this, 10)">5. Collection Amount</label>0
                                <asp:TextBox ID="txtCollectionAmount" runat="server" CssClass="form-control" oninput="validateMobileNumber(this, 10)">0</asp:TextBox>
                            </div>

                            <div class="col-md-4">
                                <label for="amountInvested" class="form-label amountIvestedlbl">Amount Invested</label>
                                <div class="d-flex">
                                    <asp:TextBox ID="txtAmountInvested" runat="server" CssClass="form-control me-2" oninput="validateMobileNumber(this, 10)">0</asp:TextBox>
                                    <asp:TextBox ID="txtAmountInvestedAdditional" runat="server" CssClass="form-control" oninput="validateMobileNumber(this, 10)"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-md-4">
                                <label for="remark" class="form-label">Remark</label>
                                <asp:TextBox ID="txtRemark" runat="server" CssClass="form-control" MaxLength ="50"></asp:TextBox>
                            </div>

                            <div class="col-md-4">


                                <div class="form-check mt-4">
                                    <label class="form-check-label" for="chkZeroCommission">
                                        Import with Zero Commission
                                        <asp:CheckBox ID="chkZeroCommission" runat="server" />
                                    </label>
                                </div>
                            </div>




                            <div class="col-md-12">
                                <asp:Button ID="btnImportCorporateNonEsc" runat="server" Text="Import Corporate Non-ESC Transaction" CssClass="btn btn-outline-primary me-2" Enabled="false" />
                                <asp:Button ID="btnImportEcs" runat="server" Text="Import ECS Transaction" CssClass="btn btn-outline-primary" Enable="false"
                                    
                                    OnClientClick="openModalNpsEcsModel(); return false;" 

                                    />
                            </div>

                            <div class="mt-5">
                                <div class="d-flex align-items-center flex-md-row flex-column gap-3">
                                    <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="SaveButton_Click" />
                                    <asp:Button ID="btnModify" runat="server" Text="Modify" CssClass="btn btn-outline-primary" OnClientClick="return showConfirmation();" OnClick="ModifyButton_Click" />
                                    <asp:HiddenField ID="hdnUserResponse" runat="server" />

                                    <script type="text/javascript">
                                        function showConfirmation() {
                                            var userResponse = confirm("Are you sure you want to proceed?");
                                            document.getElementById('<%= hdnUserResponse.ClientID %>').value = userResponse; // Store the response
                                            return userResponse; // Return the response to determine if the postback should proceed
                                        }
                                    </script>

                                    <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-outline-primary" OnClick="ResetButton_Click" />

                                    <asp:Button ID="btnExit" runat="server" Text="Exit" CssClass="btn btn-outline-primary" OnClick="ExitButton_Click" />

                                    <asp:Button ID="btnPrint" runat="server" Text="Print View" CssClass="btn btn-outline-primary" OnClick="PrintButton_Click" OnClientClick="openModalPrintSaveRecietDataModel();" />

                                </div>
                                <!-- Message label for displaying status messages -->
                            </div>
                            <asp:Label ID="lblMessage" runat="server" CssClass="text-danger mt-3" Visible="true"></asp:Label>
                        </div>
                    </div>
                </div>
            </div>
        </div>



        <div id="SearchArModel" class="modal" style="padding: 0 0 0 20%; " z-index: 5">
            <div class="modal-content" style="width:100%">
                <h2 class="page-title">Search AR</h2>

                <div class="row g-3">
                    <!-- AR No -->
                    <div class="col-md-2">
                        <label for="arPopArNo" class="form-label">AR No</label>
                        <asp:TextBox ID="arPopArNo" runat="server" CssClass="form-control" />
                    </div>
                    <!-- Scheme -->
                    <div class="col-md-3">
                        <label for="arPopScheme" class="form-label">Scheme</label>
                        <%--<asp:TextBox ID="arPopScheme" runat="server" CssClass="form-control" />--%>
                        <asp:DropDownList ID="arPopDdlSchemas" runat="server" CssClass="form-select" >
                            <asp:ListItem Text="Scheme" Value="" Selected="True"></asp:ListItem>

                            <asp:ListItem Text="New Pension Scheme Tier 1" Value="OP#09971"></asp:ListItem>
                            <asp:ListItem Text="New Pension Scheme Tier 2" Value="OP#09972"></asp:ListItem>
                            <asp:ListItem Text="New Pension Scheme Tier 1+2" Value="OP#09973"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                   
                    <!-- Cheque No -->
                    <div class="col-md-2">
                        <label for="arPopChequeNo" class="form-label">Cheque No</label>
                        <asp:TextBox ID="arPopChequeNo" runat="server" CssClass="form-control" />
                    </div>

                    <!-- Investor Name -->
                    <div class="col-md-4">
                        <label for="arPopInvName" class="form-label">Investor Name</label>
                        <asp:TextBox ID="arPopInvName" runat="server" CssClass="form-control" />
                    </div>
                    <!-- App No -->
                    <div class="col-md-1">
                        <label for="arPopAppNo" class="form-label">App No</label>
                        <asp:TextBox ID="arPopAppNo" runat="server" CssClass="form-control" />
                    </div>
                    
                    <!-- PRAN No -->
                    <div class="col-md-2">
                        <label for="arPopPranNo" class="form-label">PRAN No</label>
                        <asp:TextBox ID="arPopPranNo" runat="server" CssClass="form-control" />
                    </div>
                    <!-- ANA Exist Code -->
                    <div class="col-md-3">
                        <label for="arPopAnaExistCode" class="form-label">ANA Exist Code / Client Code</label>
                        <asp:TextBox ID="arPopAnaExistCode" runat="server" CssClass="form-control" />
                    </div>


                    <!-- AR From Date -->
                    <div class="col-md-2">
                        <label for="arPopArFromDate" class="form-label">AR From Date</label>
                        <div class="input-group date" id="arPopFromDatePicker" data-date-format="dd/mm/yyyy">
                            <asp:TextBox ID="arPopArFromDate" runat="server" CssClass="form-control" placeholder="dd/mm/yyyy" />
                            <span class="input-group-addon">
                                <span class="glyphicon glyphicon-calendar"></span>
                            </span>
                        </div>
                    </div>

                    <!-- AR To Date -->
                    <div class="col-md-2">
                        <label for="arPopArToDate" class="form-label">AR To Date</label>
                        <div class="input-group date" id="arPopToDatePicker" data-date-format="dd/mm/yyyy">
                            <asp:TextBox ID="arPopArToDate" runat="server" CssClass="form-control" placeholder="dd/mm/yyyy" />
                            <span class="input-group-addon">
                                <span class="glyphicon glyphicon-calendar"></span>
                            </span>
                        </div>
                    </div>


                    <!-- Before/After radio buttons -->
                    <div class="col-md-2">
                        <label class="form-label">Deposit</label>
                        <div>
                            <asp:RadioButton ID="arPopRadioBefore" runat="server" GroupName="BeforeAfter" Text="Before" />
                            <asp:RadioButton ID="arPopRadioAfter" CssClass="ml-2" runat="server" GroupName="BeforeAfter" Text="After" />
                        </div>
                    </div>
                    <div class="col-md-12">
                        <div class="text-end mt-1 gap-2 flex-wrap">
                            <asp:Button ID="arPopBtnSearch" runat="server" CssClass="btn btn-primary" Text="Search" OnClick="arPopSearch_Click" />
                            <asp:Button ID="arPopResetID" runat="server" CssClass="btn btn-outline-primary" Text="Reset" OnClick="arPopReset_Click" />
                            <asp:Button ID="Button2" runat="server" CssClass="btn btn-outline-primary" Text="Exit" OnClientClick="closeModalSearchArModel(); return false;" />

                        </div>
                    </div>

                </div>
                <br />
                <br />

                <div>
                </div>

                <div class="row g-3">
                    <h5 class="card-title">AR Details</h5>
                    <asp:Label ID="ARListlblMessage" runat="server" Text="" />
                    <div class="table-responsive" style="overflow-y: auto; overflow-x: auto; height: 250px;">
                        <asp:GridView ID="ARListARGridView" runat="server" CssClass="table table-hover" AutoGenerateColumns="false" OnRowCommand="gvARSearch_RowCommand">
                            <Columns>
                                <asp:TemplateField HeaderText="#" ItemStyle-CssClass="column-width-50">
                                    <ItemTemplate>
                                        <asp:Label ID="ARListlblSr" runat="server" Text='<%# Container.DisplayIndex + 1 %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:LinkButton ID="ARListlnkButton" runat="server" CommandName="SelectRow" CommandArgument='<%# Eval("TRAN_CODE") %>' Text="Select" />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Transaction Code">
                                    <ItemTemplate>
                                        <asp:Label ID="arPopLblTransCode" runat="server" Text='<%# Eval("TRAN_CODE") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Transaction Date">
                                    <ItemTemplate>
                                        <asp:Label ID="arPopLblTransDate" runat="server" Text='<%# Eval("TR_DATE1") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Client Code">
                                    <ItemTemplate>
                                        <asp:Label ID="arPopLblClientCode" runat="server" Text='<%# Eval("CLIENT_CODE") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Mutual Fund Code">
                                    <ItemTemplate>
                                        <asp:Label ID="arPopLblMutCode" runat="server" Text='<%# Eval("MUT_CODE") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Scheme Code">
                                    <ItemTemplate>
                                        <asp:Label ID="arPopLblSchCode" runat="server" Text='<%# Eval("SCH_CODE") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Application Number">
                                    <ItemTemplate>
                                        <asp:Label ID="arPopLblAppNo" runat="server" Text='<%# Eval("APP_NO") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Folio Number">
                                    <ItemTemplate>
                                        <asp:Label ID="arPopLblFolioNo" runat="server" Text='<%# Eval("FOLIO_NO") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Transaction Type">
                                    <ItemTemplate>
                                        <asp:Label ID="arPopLblTransType" runat="server" Text='<%# Eval("TRAN_TYPE") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>

        </div>



        <script>
            // Function to open the NPS ECS modal and store its state
            function openModalNpsEcsModel() {
                document.getElementById("NpsEcsModel").style.display = "block";
                localStorage.setItem("NpsEcsModelOpen", "true");
            }

            // Function to close the NPS ECS modal and update its state
            function closeModalNpsEcsModel() {
                document.getElementById("NpsEcsModel").style.display = "none";
                localStorage.setItem("NpsEcsModelOpen", "false");
            }

            // Check localStorage on page load to keep NPS ECS modal open if it was open before page reload
            window.onload = function () {
                if (localStorage.getItem("NpsEcsModelOpen") === "true") {
                    document.getElementById("NpsEcsModel").style.display = "block";
                }
            };
        </script>


        <div id="NpsEcsModel" class="modal">
            <div class="modal-content">
                <h2 class="page-title">Import ECS Transaction </h2>

                <div class="row g-3 mt-2">

                    <div class="col-md-6">

                        <asp:Label ID="npsEcsLblFileName" runat="server" CssClass="form-label" Text="File Name: "></asp:Label>
                        <div class=" d-flex">
                            <asp:FileUpload ID="NpsEcsFileInput" runat="server" CssClass="form-control mr-4" OnChanged="FileInput_Changed" />
                            <asp:Button ID="uploadButton" runat="server" Text="Upload" OnClick="UploadButton_Click" CssClass="btn btn-primary" />
                        </div>
                    </div>


                    <!-- Dropdown 2 -->
                    <div class="col-md-3">
                        <label for="npsEcsDdlSheetlist" class="form-label">Select Sheet</label>
                        <asp:DropDownList ID="npsEcsDdlSheetlist" runat="server" CssClass="form-select" OnSelectedIndexChanged="NpsEcsExcelSheetSelect_SelectedIndexChanged"
AutoPostBack="true">
                            <asp:ListItem Text="Select" Value="Sheet 1" />
                        </asp:DropDownList>
                    </div>

                    <!-- Dropdown 1 -->
                    <div class="col-md-3">
                        <label for="npsEcsDdlCompany" class="form-label">Select Company <span class="text-danger">*</span></label>
                        <asp:DropDownList ID="npsEcsDdlCompany" runat="server" CssClass="form-select">
                            <asp:ListItem Text="Select" Value="1" />
                        </asp:DropDownList>
                    </div>
                    <!-- Input Field -->
                    <div class="col-md-4">
                        <label for="npsEcsInputField" class="form-label">Input Field Name</label>
                        <asp:TextBox ID="npsEcsInputField" runat="server" CssClass="form-control" />
                    </div>
                  

                    <!-- Success Message Label -->
                    <div class="col-md-12">
                        <asp:Label ID="npsEcsSuccessMessage" runat="server" CssClass="text-success" Text="Operation completed successfully!" Visible="false" />
                    </div>

                    <!-- Buttons -->
                    <div class="col-md-12 d-flex justify-content-end gap-2">
                        <asp:Button ID="npsEcsSubmitButton" runat="server" CssClass="btn btn-primary" Text="Import" OnClick="NpsEcsSubmit_Click" />
                        <asp:Button ID="npsEcsResetButton" runat="server" CssClass="btn btn-outline-primary" Text="Reset" OnClick="NpsEcsReset_Click" />
                        <asp:Button ID="npsEcsExitButton" runat="server" CssClass="btn btn-outline-primary" Text="Exit" OnClientClick="closeModalNpsEcsModel(); return false;" />
                    </div>
                </div>


                <div class="row mt-3">
                    <div class="col-md-12 grid-margin stretch-card">
                        <div class="card">

                            <div class="card-body" style="overflow-y: auto;">
                                <div class="table-responsive " style="overflow-y: auto; overflow-x: auto; max-height: 300px; min-height: 1px">

                                    <asp:GridView ID="GridView1" runat="server" CssClass="table table-bordered" AutoGenerateColumns="true"></asp:GridView>



                                </div>
                            </div>
                        </div>
                    </div>
    </div>
            
            </div>
        </div>




        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div id="printSaveRecietDataModel" class="modal" style="padding: 0 0 0 20%; z-index: 99">
                    <div class="modal-content">
                        <h2 class="page-title">Print Data</h2>
                        <div class="container mt-4">
                            <div class="row" style="overflow: auto">
                                <div class="col-md-6">
                                    <!-- Only keeping fields that are used in print function -->
                                    <div class="form-check mt-4">
                                        <label for="popSaveBUSINESS_RMCODE" class="form-check-label">Business RM Code:</label>
                                        <asp:TextBox ID="popSaveBUSINESS_RMCODE" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                    </div>
                                    <div class="form-check mt-4">
                                        <label for="popSaveTRAN_ID" class="form-check-label">Transaction ID:</label>
                                        <asp:TextBox ID="popSaveTRAN_ID" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                    </div>
                                    <div class="form-check mt-4">
                                        <label for="popSaveAMOUNT1" class="form-check-label">Amount 1:</label>
                                        <asp:TextBox ID="popSaveAMOUNT1" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                    </div>
                                    <div class="form-check mt-4">
                                        <label for="popSaveAMOUNT2" class="form-check-label">Amount 2:</label>
                                        <asp:TextBox ID="popSaveAMOUNT2" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                    </div>
                                    <div class="form-check mt-4">
                                        <label for="popSaveTRANCODE" class="form-check-label">Mode Of Payment:</label>
                                        <asp:TextBox ID="popSaveTRANCODE" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                    </div>
                                    <div class="form-check mt-4">
                                        <label for="popTRDATEl" class="form-check-label">Tr Date:</label>
                                        <asp:TextBox ID="popTRDATE" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                    </div>
                                    <div class="form-check mt-4">
                                        <label for="popSaveLOGGEDUSERID" class="form-check-label">Logged User ID:</label>
                                        <asp:TextBox ID="popSaveLOGGEDUSERID" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="form-check mt-4">
                                        <label for="popSaveTRAN_SRC" class="form-check-label">Transaction Source:</label>
                                        <asp:TextBox ID="popSaveTRAN_SRC" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                    </div>
                                    <div class="form-check mt-4">
                                        <label for="popSaveREG_TRANTYPE" class="form-check-label">Registration Transaction Type:</label>
                                        <asp:TextBox ID="popSaveREG_TRANTYPE" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                    </div>
                                    <div class="form-check mt-4">
                                        <label for="popSaveREG_recieptno" class="form-check-label">Receipt Number:</label>
                                        <asp:TextBox ID="popSaveREG_recieptnoT" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                    </div>


                                    <asp:Button ID="printDataButton" runat="server" Text="Print Receipt"
                                        CssClass="btn btn-outline-primary"
                                        OnClientClick="printGrid(); return false;" />
                                    <%--<asp:Button ID="printDataButtonSave" runat="server" Text="Save" CssClass="btn btn-outline-primary" OnClick="PrintReceiptSave_Click" />--%>

                                    <asp:Button ID="npsPrintModelClose" runat="server" CssClass="btn btn-outline-primary" Text="Exit" OnClientClick="closeModalPrintSaveRecietDataModel(); return false;" />

                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>

        <script>
            function printGrid() {
                // Function to fetch the value of a control rendered by ASP.NET
                const getFieldValue = (aspControlID) => {
                    const element = document.getElementById(aspControlID);
                    return element ? element.value || element.innerText || '' : '';
                };

                // Open a new window for printing
                const printWindow = window.open('', '', 'height=600,width=800');

                // Write the HTML content dynamically into the print window
                printWindow.document.write(`
            <html>
            <head>
                <style>
                    body {
                        font-family: Arial, sans-serif;
                        padding: 20px;
                        font-size: 12px;
                    }
                    .header {
                        display: flex;
                        justify-content: space-between;
                        align-items: flex-start;
                        margin-bottom: 30px;
                    }
                    .logo-section {
                        display: flex;
                        align-items: center;
                    }
                    .logo {
                        width: 50px;
                        height: 50px;
                        margin-right: 10px;
                    }
                    .title-section {
                        text-align: left;
                    }
                    .user-info {
                        text-align: right;
                    }
                    .main-content {
                        border: 1px solid black;
                        padding: 20px;
                        margin-top: 20px;
                    }
                    .field-row {
                        margin: 10px 0;
                        display: flex;
                    }
                    .field-label {
                        width: 200px;
                        font-weight: normal;
                    }
                    .field-value {
                        flex: 1;
                        border-bottom: 1px solid black;
                        min-height: 18px;
                    }
                    .title {
                        font-size: 14px;
                        font-weight: bold;
                    }
                    @media print {
                        body {
                            print-color-adjust: exact;
                            -webkit-print-color-adjust: exact;
                        }
                    }
                </style>
            </head>
            <body>
                <div class="header">
                    <div class="logo-section">
                        <img src="/path-to-your-logo.png" alt="WealthMaker" class="logo">
                        <div class="title-section">
                            <div class="title">NPS Transaction Receipt</div>
                        </div>
                    </div>
                    <div class="user-info">
                        User Id: ${getFieldValue('<%= popSaveLOGGEDUSERID.ClientID %>')}<br>
                        Run Date: ${new Date().toLocaleDateString()}
                    </div>
                </div>

                <div class="main-content">
                    <div class="field-row">
                        <span class="field-label">SOF_BO No :</span>
                        <span class="field-value">${getFieldValue('<%= popSaveTRAN_ID.ClientID %>')}</span>
                    </div>
                    
                    <div class="field-row">
                        <span class="field-label">Request Type :</span>
                        <span class="field-value">${getFieldValue('<%= popSaveREG_TRANTYPE.ClientID %>')}</span>
                    </div>
                    
                    <div class="field-row">
                        <span class="field-label">Type of BR Subscriber :</span>
                        <span class="field-value">${getFieldValue('<%= popSaveTRAN_SRC.ClientID %>')}</span>
                    </div>
                    
                    <div class="field-row">
                        <span class="field-label">Individual/Non-BR/KYC :</span>
                        <span class="field-value">${getFieldValue('<%= popSaveBUSINESS_RMCODE.ClientID %>')}</span>
                    </div>
                    
                    <div class="field-row">
                        <span class="field-label">Mode of Payment :</span>
                        <span class="field-value">${getFieldValue('<%= popSaveTRANCODE.ClientID %>')}</span>
                    </div>

                     <div class="field-row">
     <span class="field-label">Receipt Number :</span>
     <span class="field-value">${getFieldValue('<%= popSaveREG_recieptnoT.ClientID %>')}</span>
 </div>
                    
                    <div class="field-row">
                        <span class="field-label">Cheque/DD No :</span>
                        <span class="field-value"></span>
                                           <div class="field-row">
    <span class="field-label">Date :</span>
    <span class="field-value">${getFieldValue('<%= popTRDATE.ClientID %>')}</span>
</div> <span class="field-value"></span>
                    </div>
                    
                    <div class="field-row">
                        <span class="field-label">Amount Received Tier 1 :</span>
                        <span class="field-value">${getFieldValue('<%= popSaveAMOUNT1.ClientID %>')}</span>
                    </div>
                    
                    <div class="field-row">
                        <span class="field-label">Amount Received Tier 2 :</span>
                        <span class="field-value">${getFieldValue('<%= popSaveAMOUNT2.ClientID %>')}</span>
                    </div>
                </div>
            </body>
            </html>
        `);

                // Close the document to signal content loading
                printWindow.document.close();

                // Delay for printing to ensure all resources are loaded
                setTimeout(() => {
                    printWindow.print();
                }, 500);
            }

            // Add an event listener to the print button after DOM is loaded
            document.addEventListener('DOMContentLoaded', function () {
                const printButton = document.getElementById('<%= printDataButton.ClientID %>');
                if (printButton) {
                    printButton.addEventListener('click', function (e) {
                        e.preventDefault();
                        printGrid();
                    });
                }
            });
        </script>

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
