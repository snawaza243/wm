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
            z-index: 9999; /* Ensure it's above other elements */
        }
    </style>
    <script type="text/javascript">
        function showServerLoader() {
            document.getElementById('serverLoader').style.display = 'flex';
        }

        function hideServerLoader() {
            document.getElementById('serverLoader').style.display = 'none';
        }

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            hideServerLoader();
        });
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
            margin: 6% auto;
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
    </style>





    <div class="content-wrapper">
        <div class="page-header">
            <h3 class="page-title">Preformation - NCIS Acknowledgement to the subscriber (To be filled by POP/POP-SP)
            </h3>
        </div>

        <div class="row">
            <div class="grid-margin stretch-card">
                <div class="card">
                    <div class="card-body">
                        <asp:UpdatePanel runat="server">
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
                                            <asp:TextBox ID="txtInvestorCode" runat="server" CssClass="form-control" oninput="validateMobileNumber(this, 20)"></asp:TextBox>
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
                                                    <asp:TextBox Enabled="false" ID="txtChequeNo" runat="server" CssClass="form-control mb-3" Placeholder="Enter Number"  MaxLength="50"></asp:TextBox>
                                                </div>

                                                <%--  AND DATE --%>
                                                <div class="col-md-2">
                                                    <asp:Label ID="lblDate" runat="server" Text="Date" CssClass="form-label"></asp:Label>
                                                    <div class="input-group date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                                        <asp:TextBox Enabled="false" ID="txtChequeDated" CssClass="form-control" runat="server" MaxLength="10" oninput="formatDateInput(this)"></asp:TextBox>
                                                        <div class="input-group-addon">
                                                            <span class="glyphicon glyphicon-th"></span>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>


                                    <%-- No need to show as per anup sir --%>
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



                                                <!-- Export Button -->
                                                <div class="col-md-12">
                                                    <asp:Button ID="btnExportToExcel" runat="server" CssClass="btn btn-primary" Text="Export to Excel" OnClick="btnExportToExcel_Click" Enabled="false" />


                                                </div>
                                            </div>
                                        </div>
                                    </div>

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
                              <asp:TextBox ID="txtDate" CssClass="form-control" runat="server" MaxLength="10" oninput="formatDateInput(this)" onchange="setServiceTax()" Enabled="false"></asp:TextBox>
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
                                            OnTextChanged="ddlRequestType_SelectedIndexChanged" AutoPostBack="true"
                                            ></asp:TextBox>
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

                                    <%-- REMARKS --%>
                                    <div class="col-md-3">
                                        <label for="remark" class="form-label">Remark</label>
                                        <asp:TextBox ID="txtRemark" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                                    </div>



                                    <%-- CHK ZERO COMMISSION --%>
                                    <div class="col-md-4">
                                        <div class="form-check mt-4">
                                            <label class="form-label" for="chkZeroCommission">
                                                Import with Zero Com.
                                            </label>
                                            <div>

                                        <asp:CheckBox ID="chkZeroCommission" runat="server" ToolTip="Import with Zero Commission" />
                                            </div>
                                             
                                        </div>
                                    </div>

                                    <%-- NON ESC AND ECS BUTTON --%>
                                    <div class="col-md-12">
                                        <asp:Button ID="btnImportCorporateNonEsc" OnClick="btnImportCorporateNonEsc_Click" runat="server" Text="Import Corporate Non-ESC Transaction" CssClass="btn btn-outline-primary me-2" Enabled="true" />
                                        <asp:Button ID="btnImportEcs" OnClick="btnImportEcs_Click" runat="server" Text="Import ECS Transaction" CssClass="btn btn-outline-primary" Enable="true" Enabled="true" />
                                    </div>

                                    <%-- NPS BUTTONS AND HIDDEN FIELDS --%>
                                    <div class="mt-5">
                                        <div class="d-flex align-items-center flex-md-row flex-column gap-3">
                                            <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="SaveButton_Click" />
                                            <asp:Button ID="btnModify" runat="server" Enabled="false" Text="Modify" CssClass="btn btn-outline-primary" OnClientClick="return showConfirmation();" OnClick="ModifyButton_Click" />
                                            <asp:HiddenField ID="hdnUserResponse" runat="server" />
                                            <asp:Button ID="btnReset" runat="server" OnClientClick="showServerLoader(); return true;" Text="Reset" CssClass="btn btn-outline-primary" OnClick="ResetButton_Click" />
                                            <asp:Button ID="btnExit" runat="server" OnClientClick="showServerLoader(); return true;" Text="Exit" CssClass="btn btn-outline-primary" OnClick="ExitButton_Click" />
                                            <asp:Button ID="btnPrint" runat="server" Text="Print View" CssClass="btn btn-outline-primary" OnClick="PrintButton_Click" OnClientClick="openModalPrintSaveRecietDataModel();" />

                                            <script type="text/javascript">
                                                function showConfirmation() {
                                                    var userResponse = confirm("Are you sure you want to proceed?");
                                                    document.getElementById('<%= hdnUserResponse.ClientID %>').value = userResponse; // Store the response
                                                    return userResponse; // Return the response to determine if the postback should proceed
                                                }
                                            </script>
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
                                <asp:AsyncPostBackTrigger ControlID="btnExportToExcel" EventName="Click" />
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