<%@ Page Title=" Account Opening" Language="C#" AutoEventWireup="true" CodeBehind="AccountOpening.aspx.cs" Inherits="WM.Masters.AccountOpening" MasterPageFile="~/vmSite.Master" EnableEventValidation="false" MaintainScrollPositionOnPostback="true"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link rel="shortcut icon" href="../assets/images/favicon.png" />
    <link rel="stylesheet" href="../assets/vendors/mdi/css/materialdesignicons.min.css">
    <link rel="stylesheet" href="../assets/vendors/ti-icons/css/themify-icons.css">
    <link rel="stylesheet" href="../assets/vendors/css/vendor.bundle.base.css">
    <link rel="stylesheet" href="../assets/vendors/font-awesome/css/font-awesome.min.css">
    <link rel="stylesheet" href="../assets/vendors/bootstrap-datepicker/bootstrap-datepicker.min.css">
    <link rel="stylesheet" href="../assets/css/style.css">
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>

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
   <%-- END OF LODER CONTENT --%>

   

  <%-- Model css and form controll css --%>
    <style>
     

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

    <%-- UI BASED VALIDATION --%>
    <script type="text/javascript">
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



    <%-- Loader styles --%>
    
  





    <%-- PREVEN ENTER PRESS INVOKING FIRST BUTTON  --%>
    <script type="text/javascript">
        document.addEventListener("DOMContentLoaded", function () {
            document.addEventListener("keypress", function (event) {
                if (event.key === "Enter") {
                    event.preventDefault(); // Prevent any button click on Enter
                }
            });
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
                            <asp:Button ID="ClientListClientMasterViewModel" CssClass="btn btn-primary" runat="server" Text="View Client" OnClick="btnSearchClient" />
                            <asp:Button ID="InvestorListViewButton" CssClass="btn btn-primary" runat="server" Text="View Investor" OnClick="btnSearchInvestor" />
                        </div>
                    </div>

                    <!-- Tabs Navigation basic_details, other_details_tab   -->
                    <ul class="nav nav-tabs wmTabs" id="wmTabs" role="tablist">
                        <li class="nav-item" role="presentation">
                            <a class="nav-link" id="basic_details_tab" data-bs-toggle="tab" href="#basic_details" role="tab" aria-controls="basic_details">Basic Details</a>
                        </li>
                        <li class="nav-item" role="presentation">
                            <a class="nav-link" id="other_details_tab" data-bs-toggle="tab" href="#other_details" role="tab" aria-controls="other_details">Other Details</a>
                        </li>
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
                    </ul>

                    <!-- basic_details tab section   -->

                    <div class="tab-content wmTabsContent" id="wmTabsContent">
                        <div class="tab-pane fade show active" id="basic_details" role="tabpanel" aria-labelledby="basic_details_tab">
                            <div class="row g-3 mb-3">
                                <%-- Get by DT --%>
                                <div class="col-md-3">
                                    <asp:Label runat="server" CssClass="form-label" Text="DT number <span class='text-danger'>*</span>" />
                                    <%--<asp:TextBox runat="server" ID="txtDTNumber" CssClass="form-control border-dark" MaxLength="15" OnTextChanged="oneClientSearchByDT_Click" AutoPostBack="true"   />--%>
                                    <asp:TextBox runat="server" ID="txtDTNumber" CssClass="form-control border-dark"
    MaxLength="15" OnTextChanged="oneClientSearchByDT_Click" AutoPostBack="true"
    oninput="showServerLoader();" />


                                </div>
                              
                                <%-- Get By Guest --%>
                                <div class="col-md-3">
                                    <asp:Label runat="server" CssClass="form-label" Text="Enter Guest Code" />
                                    <asp:TextBox runat="server" ID="txtGuestCode" CssClass="form-control" MaxLength="12" />
                                </div>
                                <div class="col-md-3 ">
                                    <asp:Button ID="btnGuestCode" runat="server" CssClass="btn btn-outline-primary mt-3" Style="width: 150px;" Text="Go" OnClick="oneGuestSearch_Click" OnClientClick="showServerLoader(); return true;" />
                                </div>

                                <%-- Approve status --%>
                                <div class="col-md-3 text-end">
                                    <div>
                                        <asp:Label runat="server" ID="Label1" CssClass="form-label" Text="Approved Status"></asp:Label>
                                    </div>

                                    <div>
                                        <asp:Label runat="server" ID="ApprovalStatus" CssClass="form-label" Text=""></asp:Label>
                                    </div>
                                </div>
                                
                                <%-- txtBusinessCode --%>
                                <div class="col-md-3">
                                    <asp:Label runat="server" CssClass="form-label" Text="Business Code <span class='text-danger'>*</span>" />
                                    <asp:TextBox runat="server" ID="txtBusinessCode" CssClass="form-control" ReadOnly="true" />
                                </div>

                                <%-- txtBusinessCodeName --%>
                                <div class="col-md-3">
                                    <asp:Label runat="server" CssClass="form-label" Text="RM Name <span class='text-danger'>*</span>" />
                                    <asp:TextBox runat="server" ID="txtBusinessCodeName" CssClass="form-control" ReadOnly="true" />
                                </div>

                                <%-- txtBusinessCodeBranch --%>
                                <div class="col-md-3">
                                    <asp:Label runat="server" CssClass="form-label" Text="RM Branch <span class='text-danger'>*</span>" />
                                    <asp:TextBox runat="server" ID="txtBusinessCodeBranch" CssClass="form-control" ReadOnly="true" />
                                </div>

                                <%-- ExistingClientCodeInv --%>
                                <div class="col-md-3">
                                    <asp:Label runat="server" CssClass="form-label"
                                        ID="ExistingClientCodeInvLabel" Text="Existing Client Code" />
                                    <asp:TextBox runat="server" ID="ExistingClientCodeInv" CssClass="form-control" ReadOnly="true"  />
                                </div>

                                <%-- Search Account by AH, PAN, INV --%>
                                <div>
                                    <%-- Search Account Holder title --%>
                                    <div class="row g-3">
                                        <div class="col-md-3">
                                            <h3 class="card-title">Search Account Holder </h3>
                                        </div>
                                        <div class="col-md-9">
                                            <span class="text-danger">** Before initiating new account, first search it on PAN basis </span>
                                        </div>                                        
                                    </div>

                                    <%-- Search Account by AH, PAN, INV --%>
                                    <div class="row g-3"> 

                                        <%-- Search Account by AH --%>
                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="Enter AH Code" />
                                            <asp:TextBox runat="server" ID="txtSearchClientCode" CssClass="form-control" MaxLength="10" Text="AH" />
                                        </div>
                                     

                                        <%-- Search Account by PAN --%>
                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label" Text="PAN No" />
                                            <asp:TextBox runat="server" ID="txtSearchPan" MaxLength="10" 
                                                placeholder="Enter PAN (ABCDE1234F)"
                                                 pattern="[A-Z]{5}[0-9]{4}[A-Z]{1}"
                                                 title="Enter PAN in format: ABCDE1234F"
                                                 oninput="validatePanInput2(this)"
                                                 CssClass="form-control"                                                
                                                />
                                        </div>

                                        <%-- Search Account by INV --%>
                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label"
                                                ID="txtClientCodeLabel" Text="Client Code" />
                                            <asp:TextBox runat="server" ID="txtClientCode" CssClass="form-control" ReadOnly="False" oninput="validateMobileNumber(this, 15)"/>
                                        </div>

                                        <div class="col-md-2 text-end"">
                                            <asp:Button ID="oneClientSearch" runat="server" CssClass="btn btn-outline-primary mt-3"  style="width:150px;" Text="Search" OnClick="oneClientSearch_Click" OnClientClick="showServerLoader(); return true;" />
                                        </div>
                                    </div>

                                    <div class="row g-3">
                                        <div class="col-md-4 " style="display: none">
                                            <asp:Label runat="server" ID="lblHolderMessage" CssClass="text-danger" Text="" />
                                        </div>
                                    </div>
                                </div>

                                <%-- Loaded Head Source --%>
                                <div class="row g-3">
                                    <div class="col-md-4 d-none">
                                        <asp:Label runat="server" CssClass="form-label" Text="Source Code" />
                                        <asp:TextBox runat="server" ID="txtHeadSourceCode" CssClass="form-control" />
                                    </div>
                                </div>

                                <%-- ddlTaxStatus, ddlOccupation --%>
                                <div>
                                    <h3 class="card-title">1. Occupation (of Account Holder)</h3>
                                    <div class="row g-3 mt-2">

                                        <%-- ddlTaxStatus --%>
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
                                        
                                        <%-- ddlOccupation --%>
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

                                <%-- ddlAOCategoryStatus, ddlAOClientCategory, ddlAOACCategory --%>
                                <div>
                                    <h4 class="mb-3">Category</h4>
                                    <div class="row g-3">

                                        <%-- ddlAOCategoryStatus --%>
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

                                        <%-- ddlAOClientCategory --%>
                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="Client Category <span class='text-danger'>*</span>" />
                                            <asp:DropDownList runat="server" ID="ddlAOClientCategory" CssClass="form-select">
                                             
                                            </asp:DropDownList>
                                        </div>
                                        
                                        <%-- ddlAOACCategory --%>
                                        <div class="col-md-3">
                                            <asp:Label runat="server" CssClass="form-label"
                                                Text="A/C Category" />
                                            <asp:DropDownList runat="server" ID="ddlAOACCategory" CssClass="form-select">
                                                <asp:ListItem Text="Select A/C Category" Value="" />
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>

                                <%-- ddlSalutation1, txtAccountName, ddlSalutation2, txtFatherSpouse, txtOther1 --%>
                                <%-- txtOther1, ddlAOGender, ddlAOMaritalStatus, ddlAONationality, ddlAOResidentNRI --%>
                                <%-- DOB, ddlAOAnnualIncome, itfAOClientPan, ddlLeadSource, itfAOGuardianPerson, itfAOGuardianNationality, itfAOGuardianPANNO --%>
                                <div>
                                    <%-- ddlSalutation1, txtAccountName, ddlSalutation2, txtFatherSpouse --%>
                                    <%-- txtOther1, ddlAOGender, ddlAOMaritalStatus, ddlAONationality, ddlAOResidentNRI --%>
                                     <asp:UpdatePanel ID="upnlNationality" runat="server" UpdateMode="Conditional" EnableViewState="true">
                                        <ContentTemplate>
                                    <h3 class="card-title">2. Account Holder Information</h3>
                                            <div class="row g-3">
                                                <%-- ddlSalutation1, txtAccountName, ddlSalutation2, txtFatherSpouse --%>
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
                                                    <asp:DropDownList runat="server" ID="ddlAONationality" CssClass="form-select"
                                                        AutoPostBack="true"
                                                        OnSelectedIndexChanged="ddlAONationality_SelectedIndexChanged"
                                                        onchange="showServerLoader();">

                                                        <asp:ListItem Text="Select" Value="" />
                                                        <asp:ListItem Text="Resident" Value="RESIDENT" />
                                                        <asp:ListItem Text="Indian" Value="INDIAN" Selected="True" />
                                                        <asp:ListItem Text="NRI" Value="NRI" />
                                                        <asp:ListItem Text="Non NRI" Value="NON NRI" />
                                                    </asp:DropDownList>
                                                </div>

                                                <div class="col-md-2">
                                                    <asp:Label runat="server" CssClass="form-label" Text="Resident/NRI" />
                                                    <asp:DropDownList runat="server" ID="ddlAOResidentNRI" CssClass="form-select">
                                                        <asp:ListItem Text="Select" Value="" />
                                                        <asp:ListItem Text="Resident" Value="Resident" Selected="True" />
                                                        <asp:ListItem Text="NRI" Value="NRI" />
                                                    </asp:DropDownList>
                                                </div>
                                            </div>

                                        </ContentTemplate>

                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="ddlAONationality" EventName="SelectedIndexChanged" />
                                        </Triggers>

                                    </asp:UpdatePanel>
                                    <%-- DOB, ddlAOAnnualIncome, itfAOClientPan, ddlLeadSource, itfAOGuardianPerson, itfAOGuardianNationality, itfAOGuardianPANNO --%>
                                    <asp:UpdatePanel runat="server" UpdateMode="Conditional" EnableViewState="true">
                                        <ContentTemplate>
                                            <%-- DOB, ddlAOAnnualIncome, itfAOClientPan, ddlLeadSource --%>
                                            <div class="row g-3">
                                                <div class="col-md-3">
                                                    <asp:Label runat="server" CssClass="form-label" Text="DOB <span class='text-danger'>*</span>" />
                                                    <div class="input-group date" data-provide="datepicker">
                                                        <asp:TextBox ID="cldDOB" runat="server" CssClass="form-control" oninput="formatDOBInput(this)"
                                                            OnTextChanged="cldDOB_TextChanged" AutoPostBack="true" />
                                                        <div class="input-group-append">
                                                            <span class="input-group-text"><i class="glyphicon glyphicon-th"></i></span>
                                                        </div>
                                                    </div>
                                                    <script type="text/javascript">
                                                        // Ensure jQuery runs after every UpdatePanel refresh
                                                        var prm = Sys.WebForms.PageRequestManager.getInstance();
                                                        prm.add_endRequest(function () {
                                                            $(".date").datepicker({
                                                                autoclose: true, // Auto-close the datepicker
                                                                format: 'dd/mm/yyyy', // Set desired format
                                                                todayHighlight: true
                                                            });
                                                        });
                                                    </script>
                                                    <script type="text/javascript">
                                                        $(document).ready(function () {
                                                            $(".date").datepicker({
                                                                autoclose: true,
                                                                format: 'dd/mm/yyyy',
                                                                todayHighlight: true
                                                            });
                                                        });
                                                    </script>

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

                                            <div class="row g-3">
                                                <h4 class="mb-3">Guardian Details</h4>
                                            </div>

                                            <%-- itfAOGuardianPerson, itfAOGuardianNationality, itfAOGuardianPANNO --%>
                                            <div class="row g-3">
                                                <div class="col-md-3">
                                                    <asp:Label runat="server"
                                                        CssClass="form-label" Text="Guardian/Contact Person" />
                                                    <asp:TextBox runat="server" ID="itfAOGuardianPerson" CssClass="form-control guardianFieldHead" MaxLength="100" />
                                                </div>
                                                <div class="col-md-3">
                                                    <asp:Label runat="server" CssClass="form-label"
                                                        Text="Nationality" />
                                                    <asp:TextBox runat="server" ID="itfAOGuardianNationality" CssClass="form-control guardianFieldHead" MaxLength="20" />
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

                                                </div>
                                            </div>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="cldDOB" EventName="TextChanged" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                     
                                </div>

                                <%-- itfAOMAddress1, itfAOMAddress2, ddlMailingCountryList, ddlMailingStateList, ddlMailingCityList, txtMailingPin  --%>
                                <%-- chkSameAsMailing --%>
                                <%-- txtPAddress1, txtPAddress2, ddlPCountryList, ddlPStateList, ddlPCityList, txtPPin --%>

                                <%-- txtOverseasAdd --%>
                                <!-- Fax, txtAadhar, txtEmail, txtOfficialEmail -->
                                <div>   
                                    <%-- Mailing, Permanent address and check box --%>
                                    <asp:UpdatePanel id="upnl_mailing" runat="server" UpdateMode="Conditional" EnableViewState="true">
                                        <ContentTemplate>
                                            <!-- Mailing Address -->
                                            <div class="row g-3 mb-3">
                                                <div class="col-md-6">
                                                    <asp:Label runat="server" CssClass="form-label"
                                                        Text="Mailing Address 1" />
                                                    <asp:TextBox runat="server" ID="itfAOMAddress1" CssClass="form-control" TextMode="MultiLine"
                                                        MaxLength="100"
                                                        Rows="3" Columns="10" />
                                                </div>
                                                <div class="col-md-6">
                                                    <asp:Label runat="server" CssClass="form-label"
                                                        Text="Mailing Address 2" />
                                                    <asp:TextBox runat="server" ID="itfAOMAddress2" CssClass="form-control" TextMode="MultiLine"
                                                        MaxLength="100"
                                                        Rows="3" Columns="10" />
                                                </div>
                                            </div>

                                            <!-- Mailing country, state, city and pin -->
                                            <div class="row g-3 mb-3">
                                                <!-- Country Dropdown -->
                                                <div class="col-md-3">
                                                    <asp:Label runat="server" CssClass="form-label" Text="Country <span class='text-danger'>*</span>" />
                                                    <asp:DropDownList
                                                        runat="server"
                                                        ID="ddlMailingCountryList"
                                                        CssClass="form-select"
                                                        AutoPostBack="true"
                                                        OnSelectedIndexChanged="ddlMailingCountryList_SelectedIndexChanged">
                                                        <asp:ListItem Value="0">Select Country</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>

                                                <!-- State Dropdown -->
                                                <div class="col-md-3">
                                                    <asp:Label runat="server" CssClass="form-label" Text="State <span class='text-danger'>*</span>" />
                                                    <asp:DropDownList
                                                        runat="server"
                                                        ID="ddlMailingStateList"
                                                        CssClass="form-select"
                                                        AutoPostBack="true"
                                                        OnSelectedIndexChanged="ddlMailingStateList_SelectedIndexChanged">
                                                        <asp:ListItem Value="0">Select State</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>

                                                <!-- City Dropdown -->
                                                <div class="col-md-3">
                                                    <asp:Label runat="server" CssClass="form-label" Text="City <span class='text-danger'>*</span>" />
                                                    <asp:DropDownList
                                                        runat="server"
                                                        ID="ddlMailingCityList"
                                                        CssClass="form-select">
                                                        <asp:ListItem Value="0">Select City</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>

                                                <!-- Mailing PIN Code -->
                                                <div class="col-md-3">
                                                    <asp:Label runat="server" CssClass="form-label" Text="Pin" />
                                                    <asp:TextBox
                                                        runat="server"
                                                        ID="txtMailingPin"
                                                        CssClass="form-control"
                                                        MaxLength="20" />
                                                    
                                                </div>
                                            </div>

                                            <!-- Checkbox to Copy Address -->
                                            <div class="form-input mt-2 mb-2">
                                                <label class="form-check-label">
                                                        <asp:CheckBox
                                                            ID="chkSameAsMailing"
                                                            runat="server"
                                                            AutoPostBack="true"
                                                            OnCheckedChanged="chkSameAsMailing_CheckedChanged" />
                                                    Permanent Address same as Mailing Address
                                                </label>
                                            </div>

                                            <!-- Permanent Address -->
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

                                            <!-- Permanent country, state, city and pin -->
                                            <div class="row g-3">
                                                <!-- Permanent Address - Country -->
                                                <div class="col-md-3">
                                                    <asp:Label runat="server" CssClass="form-label" Text="Country" />
                                                    <asp:DropDownList
                                                        runat="server"
                                                        ID="ddlPCountryList"
                                                        CssClass="form-select"
                                                        AutoPostBack="true"
                                                        OnSelectedIndexChanged="ddlPCountryList_SelectedIndexChanged">
                                                        <asp:ListItem Value="">Select Country</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>

                                                <!-- Permanent Address - State -->
                                                <div class="col-md-3">
                                                    <asp:Label runat="server" CssClass="form-label" Text="State" />
                                                    <asp:DropDownList
                                                        runat="server"
                                                        ID="ddlPStateList"
                                                        CssClass="form-select"
                                                        AutoPostBack="true"
                                                        OnSelectedIndexChanged="ddlPStateList_SelectedIndexChanged">
                                                        <asp:ListItem Value="">Select State</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>

                                                <!-- Permanent Address - City -->
                                                <div class="col-md-3">
                                                    <asp:Label runat="server" CssClass="form-label" Text="City" />
                                                    <asp:DropDownList
                                                        runat="server"
                                                        ID="ddlPCityList"
                                                        CssClass="form-select">
                                                        <asp:ListItem Value="">Select City</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>

                                                <!-- Permanent Address - PIN -->
                                                <div class="col-md-3">
                                                    <asp:Label runat="server" CssClass="form-label" Text="Pin" />
                                                    <asp:TextBox
                                                        runat="server"
                                                        ID="txtPPin"
                                                        MaxLength="20"
                                                        CssClass="form-control" />
                                                </div>
                                            </div>

                                                                                    <script>
                                                                                        document.addEventListener("DOMContentLoaded", function () {
                                                                                            var ddlNationality = document.getElementById("<%= ddlAONationality.ClientID %>");
                                                var txtMailingPin = document.getElementById("<%= txtMailingPin.ClientID %>");
    var txtPPin = document.getElementById("<%= txtPPin.ClientID %>");
    var txtMobileNo = document.getElementById("<%= MobileNo.ClientID %>");

                                                function setMaxLength() {
                                                    if (ddlNationality.value === "NRI") {
                                                        txtMailingPin.maxLength = 20;
                                                        txtPPin.maxLength = 20;
                                                        txtMailingPin.removeEventListener("input", enforceNumericInput);
                                                        txtPPin.removeEventListener("input", enforceNumericInput);
                                                    } else {
                                                        txtMailingPin.maxLength = 6;
                                                        txtPPin.maxLength = 6;
                                                        txtMailingPin.addEventListener("input", enforceNumericInput);
                                                        txtPPin.addEventListener("input", enforceNumericInput);
                                                    }
                                                    txtMobileNo.maxLength = ddlNationality.value === "NRI" ? 20 : 10;
                                                }

                                                function enforceNumericInput(event) {
                                                    this.value = this.value.replace(/\D/g, '').substring(0, this.maxLength); // Allow only digits up to max length
                                                }

                                                function checkNationalityBeforeInput(event) {
                                                    if (!ddlNationality.value) {
                                                        alert("Please select Nationality first.");
                                                        event.preventDefault();
                                                        this.value = "";
                                                    }
                                                }

                                                ddlNationality.addEventListener("change", setMaxLength);
                                                txtMobileNo.addEventListener("input", enforceNumericInput);
                                                txtMailingPin.addEventListener("input", checkNationalityBeforeInput);
                                                txtPPin.addEventListener("input", checkNationalityBeforeInput);
                                                txtMobileNo.addEventListener("input", checkNationalityBeforeInput);

                                                // Initial call to set max length on page load
                                                setMaxLength();
                                            });

                                                                                    </script>
                                        </ContentTemplate>
                                        <Triggers>

                                              
                                            <asp:AsyncPostBackTrigger ControlID="ddlMailingCountryList" EventName="SelectedIndexChanged" />
                                            <asp:AsyncPostBackTrigger ControlID="ddlMailingStateList" EventName="SelectedIndexChanged" />
                                            <asp:AsyncPostBackTrigger ControlID="chkSameAsMailing" EventName="CheckedChanged" />
                                            <asp:AsyncPostBackTrigger ControlID="ddlPCountryList" EventName="SelectedIndexChanged" />
                                            <asp:AsyncPostBackTrigger ControlID="ddlPStateList" EventName="SelectedIndexChanged" />
                                        </Triggers>
                                    </asp:UpdatePanel>

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


                                    <!-- Fax, txtAadhar, txtEmail, txtOfficialEmail -->
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

                                <%-- PhoneOfficeSTD, PhoneOfficeNumber, PhoneResSTD, PhoneResNumber, MobileNo, Ref name ref mobile --%>
                                <div class="row g-3">
                                    <%-- STD, phone, mobile --%>
                                    <div class="col-md-6">
                                        <h3 class="card-title">Enter Any One of Three Numbers</h3>
                                        <%-- PhoneOfficeSTD, PhoneOfficeNumber, PhoneResSTD, PhoneResNumber, MobileNo --%>

                                        <div class="row g-3">
                                            <!-- Phone STD (Off) -->
                                            <div class="col-md-6">
                                                <asp:Label runat="server" CssClass="form-label"
                                                    Text="Phone (Off) - STD" />
                                                <asp:TextBox runat="server" ID="PhoneOfficeSTD"
                                                    MaxLength="5"
                                                    CssClass="form-control" />
                                            </div>

                                            <!-- Phone (Off) -->
                                            <div class="col-md-6">
                                                <asp:Label runat="server" CssClass="form-label"
                                                    Text="Phone (Off) - Number" />
                                                <asp:TextBox runat="server" ID="PhoneOfficeNumber"
                                                    MaxLength="10"
                                                    oninput="validateMobileNumber(this, 10)" CssClass="form-control" />
                                            </div>

                                            <!-- Phone STD (Res) -->
                                            <div class="col-md-6">
                                                <asp:Label runat="server" CssClass="form-label"
                                                    Text="Phone (Res) - STD" />
                                                <asp:TextBox runat="server" ID="PhoneResSTD"
                                                    MaxLength="5"
                                                    CssClass="form-control" />
                                            </div>

                                            <!-- Phone (Res) -->
                                            <div class="col-md-6">
                                                <asp:Label runat="server" CssClass="form-label"
                                                    Text="Phone (Res) - Number" />
                                                <asp:TextBox runat="server" ID="PhoneResNumber"
                                                    MaxLength="10"
                                                    oninput="validateMobileNumber(this, 10)" CssClass="form-control" />
                                            </div>

                                            <!-- Mobile + NRI case -->
                                             
                                            <asp:UpdatePanel ID ="upnlMobile" runat="server" UpdateMode="Conditional" EnableViewState="true" >
                                                <ContentTemplate>
                                                     
                                            <div class="col-md-12">
                                                <asp:Label runat="server" CssClass="form-label"
                                                    Text="Mobile <span class='text-danger'>*</span>" />
                                                <asp:TextBox runat="server" ID="MobileNo" MaxLength="18" CssClass="form-control" />
                                                <script>
                                                    document.addEventListener("DOMContentLoaded", function () {
                                                        var ddlNationality = document.getElementById("<%= ddlAONationality.ClientID %>");
                                                        var txtMailingPin = document.getElementById("<%= txtMailingPin.ClientID %>");
                                                        var txtPPin = document.getElementById("<%= txtPPin.ClientID %>");
                                                        var txtMobileNo = document.getElementById("<%= MobileNo.ClientID %>");

                                                        function setMaxLength() {
                                                            if (ddlNationality.value === "NRI") {
                                                                txtMailingPin.maxLength = 20;
                                                                txtPPin.maxLength = 20;
                                                                txtMailingPin.removeEventListener("input", enforceNumericInput);
                                                                txtPPin.removeEventListener("input", enforceNumericInput);
                                                            } else {
                                                                txtMailingPin.maxLength = 6;
                                                                txtPPin.maxLength = 6;
                                                                txtMailingPin.addEventListener("input", enforceNumericInput);
                                                                txtPPin.addEventListener("input", enforceNumericInput);
                                                            }
                                                            txtMobileNo.maxLength = ddlNationality.value === "NRI" ? 15 : 10;
                                                        }

                                                        function enforceNumericInput(event) {
                                                            this.value = this.value.replace(/\D/g, '').substring(0, this.maxLength); // Allow only digits up to max length
                                                        }

                                                        function checkNationalityBeforeInput(event) {
                                                            if (!ddlNationality.value) {
                                                                alert("Please select Nationality first.");
                                                                event.preventDefault();
                                                                this.value = "";
                                                            }
                                                        }

                                                        ddlNationality.addEventListener("change", setMaxLength);
                                                        txtMobileNo.addEventListener("input", enforceNumericInput);
                                                        txtMailingPin.addEventListener("input", checkNationalityBeforeInput);
                                                        txtPPin.addEventListener("input", checkNationalityBeforeInput);
                                                        txtMobileNo.addEventListener("input", checkNationalityBeforeInput);

                                                        // Initial call to set max length on page load
                                                        setMaxLength();
                                                    });




                                                </script>

                                            </div>
                                                    </ContentTemplate>
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="ddlAONationality" EventName="SelectedIndexChanged" />
                                                    </Triggers>
                                            </asp:UpdatePanel>
                                        </div>
                                    </div>

                                    <%-- Ref name ref mobile --%>
                                    <div class="col-md-6">
                                        <h3 class="card-title">Reference Details</h3>
                                        <div class="row g-3">

                                            <%-- Reference name --%>
                                            <div class="col-md-6">
                                                <asp:Label runat="server" CssClass="form-label" Text="Reference Name" />
                                                <asp:TextBox runat="server" ID="ReferenceName1" MaxLenght="100" oninput="validateVarchar(this,100)" CssClass="form-control mb-3" Placeholder="Enter Ref. Name 1" />
                                                <asp:TextBox runat="server" ID="ReferenceName2" MaxLenght="100" oninput="validateVarchar(this,100)" CssClass="form-control mb-3" Placeholder="Enter Ref. Name 2" />
                                                <asp:TextBox runat="server" ID="ReferenceName3" MaxLenght="100" oninput="validateVarchar(this,100)" CssClass="form-control mb-3" Placeholder="Enter Ref. Name 3" />
                                                <asp:TextBox runat="server" ID="ReferenceName4" MaxLenght="100" oninput="validateVarchar(this,100)" CssClass="form-control" Placeholder="Enter Ref. Name 4" />
                                            </div>

                                            <%-- Reference mobile  --%>
                                            <div class="col-md-6">
                                                <asp:Label runat="server" CssClass="form-label" Text="Mobile No" />
                                                <asp:TextBox runat="server" ID="ReferenceMobileNo1" MaxLength="10" oninput="validateMobileNumber(this, 10)" CssClass="form-control mb-3" Placeholder="Enter Ref. Mobile 1" />
                                                <asp:TextBox runat="server" ID="ReferenceMobileNo2" MaxLength="10" oninput="validateMobileNumber(this, 10)" CssClass="form-control mb-3" Placeholder="Enter Ref. Mobile 2" />
                                                <asp:TextBox runat="server" ID="ReferenceMobileNo3" MaxLength="10" oninput="validateMobileNumber(this, 10)" CssClass="form-control mb-3" Placeholder="Enter Ref. Mobile 3" />
                                                <asp:TextBox runat="server" ID="ReferenceMobileNo4" MaxLength="10" oninput="validateMobileNumber(this, 10)" CssClass="form-control" Placeholder="Enter Ref. Mobile 4" />
                                            </div>
                                        </div>
                                    </div>

                                </div>
                                    
                                
                            </div>

                            <%-- Btn Next, Reset, Exit --%>
                            <div class="d-flex align-items-md-center flex-wrap flex-md-row flex-column gap-3">
                                <asp:Button ID="btnHiddenPostbackResetOnCreated" runat="server" OnClick="ResetFormFields1OnCreation" style="display:none;" />
                                <asp:Button runat="server" ID="btnNext" CssClass="btn btn-primary" Text="Next" OnClientClick="document.getElementById('other_details_tab').click(); return false;" AccessKey="N" />
                                <asp:Button runat="server" ID="btnReset" CssClass="btn btn-outline-primary" Text="Reset" OnClick="ResetFields" OnClientClick="showServerLoader(); return true;"/>
                                <asp:Button runat="server" ID="btnExit" CssClass="btn btn-outline-primary" Text="Exit" OnClick="ExitButton_Click" OnClientClick="showServerLoader(); return true;" />
                            </div>

                        </div>

                        <%-- Tab section other details --%>
                        <div class="tab-pane fade pt-0" id="other_details" role="tabpanel" aria-labelledby="other_details_tab">
                            <div class="container mt-0 ">
                                <h3 class="card-title">Add Family Member Details</h3>
                                <asp:UpdatePanel runat="server" ID="ngfd_UpdatePanels">
                                    <ContentTemplate>
                                        <div style="width: 100%; overflow-x: auto; white-space: nowrap; height: 200px" class="mt-6">

                                            <asp:GridView
                                                ID="ngfd_gvClients"
                                                runat="server"
                                                AutoGenerateColumns="False"
                                                OnRowDataBound="ngfd_gvClients_RowDataBound">
                                                <Columns>

                                                    <%-- ddl ngfd_ddlExistingInvestor --%>
                                                    <asp:TemplateField HeaderText="Existing Investor">
                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="ngfd_ddlExistingInvestor" runat="server"
                                                                AutoPostBack="true" OnSelectedIndexChanged="ngfd_ddladdfamExistingInvestor_SelectedIndexChanged"
                                                                Style="width: 200px; padding: 5px; border: 1px solid #ccc;"
                                                                >
                                                                <asp:ListItem Text="Select" Value="" />
                                                            </asp:DropDownList>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <%-- txt ngfd_txtInvestorCode --%>
                                                    <asp:TemplateField HeaderText="Investor Code"  >
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="ngfd_txtInvestorCode" runat="server" Text='<%# Eval("InvestorCode") %>' ReadOnly="true"
                                                                Style="width: 120px; padding: 5px; border: 1px solid #ccc;"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <%-- ddl ngfd_ddlSalutation--%>
                                                    <asp:TemplateField HeaderText="Title<span class='text-danger'>*</span>">
                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="ngfd_ddlSalutation" runat="server"
                                                                Style="width: 120px; padding: 5px; border: 1px solid #ccc;" 
                                                                SelectedValue='<%# DataBinder.Eval(Container.DataItem, "Salutation") %>'>
                                                                <asp:ListItem Text="Select Title" Value="" />
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
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <%-- txt ngfd_txtInvestorName--%>
                                                    <asp:TemplateField HeaderText="Investor Name<span class='text-danger'>*</span>">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="ngfd_txtInvestorName" runat="server" Text='<%# Eval("InvestorName") %>'
                                                                Style="width: 180px; padding: 5px; border: 1px solid #ccc;"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <%-- txt ngfd_txtMobile --%>
                                                    <asp:TemplateField HeaderText="Mobile<span class='text-danger'>*</span>">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="ngfd_txtMobile" runat="server" oninput="validateMobileNumber(this, 10)"
                                                                Text='<%# Eval("Mobile") %>'
                                                                Style="width: 150px; padding: 5px; border: 1px solid #ccc;"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <%-- txt ngfd_txtEmail --%>
                                                    <asp:TemplateField HeaderText="Email">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="ngfd_txtEmail" runat="server" Text='<%# Eval("Email") %>'
                                                                Style="width: 200px; padding: 5px; border: 1px solid #ccc;"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <%-- txt ngfd_txtPAN --%>
                                                    <asp:TemplateField HeaderText="PAN">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="ngfd_txtPAN" runat="server"
                                                                MaxLength="10" placeholder="Enter PAN (ABCDE1234F)"
                                                                pattern="[A-Z]{5}[0-9]{4}[A-Z]{1}"
                                                                title="Enter PAN in format: ABCDE1234F"
                                                                oninput="validatePanInput2(this)"
                                                                Text='<%# Eval("PAN") %>'
                                                                Style="width: 150px; padding: 5px; border: 1px solid #ccc;"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <%-- txt ngfd_txtAadhar --%>
                                                    <asp:TemplateField HeaderText="Aadhar">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="ngfd_txtAadhar" runat="server" MaxLength="12" Text='<%# Eval("Aadhar") %>'
                                                                Style="width: 180px; padding: 5px; border: 1px solid #ccc;"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <%-- ddl ngfd_ddlGender --%>
                                                    <asp:TemplateField HeaderText="Gender<span class='text-danger'>*</span>">
                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="ngfd_ddlGender" runat="server"
                                                                Style="width: 120px; padding: 5px; border: 1px solid #ccc;">
                                                                <asp:ListItem Text="Select" Value="" />
                                                                <asp:ListItem Text="Male" Value="MALE" />
                                                                <asp:ListItem Text="Female" Value="FEMALE" />
                                                                <asp:ListItem Text="Other" Value="OTHER" />
                                                            </asp:DropDownList>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <%-- txt ngfd_txtDOB --%>
                                                    <asp:TemplateField HeaderText="DOB<span class='text-danger'>*</span>">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="ngfd_txtDOB" runat="server" CssClass="datepicker"
                                                                MaxLength="10" Text='<%# Eval("DOB", "{0:dd-MM-yyyy}") %>'
                                                                Style="width: 140px; padding: 5px; border: 1px solid #ccc;"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <%-- txt ngfd_txtgPAN --%>
                                                    <asp:TemplateField HeaderText="Guardian PAN">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="ngfd_txtgPAN" runat="server"
                                                                MaxLength="10" Enabled="true" Text='<%# Eval("GuardianPAN") %>'
                                                                Style="width: 150px; padding: 5px; border: 1px solid #ccc;"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <%-- txt ngfd_txtgName --%>
                                                    <asp:TemplateField HeaderText="Guardian Name">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="ngfd_txtgName" runat="server"
                                                                MaxLength="100" Enabled="true" Text='<%# Eval("GuardianName") %>'
                                                                Style="width: 200px; padding: 5px; border: 1px solid #ccc;"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <%-- ddl ngfd_ddlOccupation--%>
                                                    <asp:TemplateField HeaderText="Occupation<span class='text-danger'>*</span>">
                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="ngfd_ddlOccupation" runat="server" 
                                                                 SelectedValue='<%# DataBinder.Eval(Container.DataItem, "Occupation") %>'
                                                                Style="width: 180px; padding: 5px; border: 1px solid #ccc;">
                                                                <asp:ListItem Text="Select Occupation" Value="" />
                                                                <asp:ListItem Text="ADVOCATES/ LAWYER" Value="29" />
                                                                <asp:ListItem Text="AGRICULTURIST" Value="38" />
                                                                <asp:ListItem Text="BCL GROUP EMPLOYEES" Value="27" />
                                                                <asp:ListItem Text="BUSINESS" Value="1" />
                                                                <asp:ListItem Text="CHARTERED ACCOUNTANTS" Value="30" />
                                                                <asp:ListItem Text="DEFENCE SERVICES" Value="4" />
                                                                <asp:ListItem Text="DOCTOR" Value="2" />
                                                                <asp:ListItem Text="FOREX DEALER" Value="37" />
                                                                <asp:ListItem Text="GOVERNMENT SERVICE" Value="40" />
                                                                <asp:ListItem Text="GOVT/PSU SERVICES" Value="9" />
                                                                <asp:ListItem Text="HOUSEWIFE" Value="8" />
                                                                <asp:ListItem Text="HUF" Value="33" />
                                                                <asp:ListItem Text="MINOR" Value="35" />
                                                                <asp:ListItem Text="NON INDIVIDUAL" Value="31" />
                                                                <asp:ListItem Text="NRI" Value="34" />
                                                                <asp:ListItem Text="OTHERS" Value="12" />
                                                                <asp:ListItem Text="PRIVATE SERVICE" Value="5" />
                                                                <asp:ListItem Text="PROFESSIONAL" Value="39" />
                                                                <asp:ListItem Text="RETIRED" Value="11" />
                                                                <asp:ListItem Text="SELF-EMPLOYED" Value="7" />
                                                                <asp:ListItem Text="STUDENT" Value="3" />
                                                                <asp:ListItem Text="TRUST" Value="32" />
                                                                <asp:ListItem Text="UNKNOWN/NOT APPLICABLE" Value="36" />

                                                            </asp:DropDownList>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <%-- ddl ngfd_ddlRelation --%>
                                                    <asp:TemplateField HeaderText="Relation<span class='text-danger'>*</span>">
                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="ngfd_ddlRelation" runat="server" 
                                                                 SelectedValue='<%# DataBinder.Eval(Container.DataItem, "Relation") %>'
                                                                
                                                                Style="width: 180px; padding: 5px; border: 1px solid #ccc;">
                                                                <asp:ListItem Text="Select Relation" Value="" />
                                                                <asp:ListItem Text="AUNT" Value="15" />
                                                                <asp:ListItem Text="BROTHER" Value="27" />
                                                                <asp:ListItem Text="BROTHER-IN-LAW" Value="13" />
                                                                <asp:ListItem Text="COUSIN" Value="16" />
                                                                <asp:ListItem Text="DAUGHTER" Value="3" />
                                                                <asp:ListItem Text="DAUGHTER-IN-LAW" Value="10" />
                                                                <asp:ListItem Text="FATHER" Value="4" />
                                                                <asp:ListItem Text="FATHER-IN-LAW" Value="9" />
                                                                <asp:ListItem Text="FRIEND" Value="2" />
                                                                <asp:ListItem Text="GRAND-DAUGHTER" Value="22" />
                                                                <asp:ListItem Text="GRAND-FATHER" Value="19" />
                                                                <asp:ListItem Text="GRAND-MOTHER" Value="20" />
                                                                <asp:ListItem Text="GRAND-SON" Value="21" />
                                                                <asp:ListItem Text="HUF" Value="32" />
                                                                <asp:ListItem Text="HUSBAND" Value="24" />
                                                                <asp:ListItem Text="KNOWN" Value="26" />
                                                                <asp:ListItem Text="MOTHER" Value="5" />
                                                                <asp:ListItem Text="MOTHER-IN-LAW" Value="8" />
                                                                <asp:ListItem Text="NEPHEW" Value="17" />
                                                                <asp:ListItem Text="NIECE" Value="25" />
                                                                <asp:ListItem Text="OTHERS" Value="29" />
                                                                <asp:ListItem Text="PROPRIETOR" Value="30" />
                                                                <asp:ListItem Text="RELATIVE" Value="18" />
                                                                <asp:ListItem Text="SELF" Value="11" />
                                                                <asp:ListItem Text="SISTER" Value="1" />
                                                                <asp:ListItem Text="SISTER-IN-LAW" Value="23" />
                                                                <asp:ListItem Text="SON" Value="7" />
                                                                <asp:ListItem Text="SON-IN-LAW" Value="12" />
                                                                <asp:ListItem Text="SPOUSE" Value="28" />
                                                                <asp:ListItem Text="TRUSTEE" Value="31" />
                                                                <asp:ListItem Text="UNCLE" Value="14" />
                                                                <asp:ListItem Text="WIFE" Value="6" />

                                                            </asp:DropDownList>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <%-- ddl ngfd_ddlKyc --%>
                                                    <asp:TemplateField HeaderText="KYC">
                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="ngfd_ddlKyc" runat="server" SelectedValue='<%# Eval("KYC") %>'
                                                                Style="width: 120px; padding: 5px; border: 1px solid #ccc;"
                                                                
                                                                >
                                                                <asp:ListItem Text="Select" Value="" />
                                                                <asp:ListItem Text="Yes" Value="YES" />
                                                                <asp:ListItem Text="No" Value="NO" />
                                                            </asp:DropDownList>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <%-- ddl ngfd_ddlApproved --%>
                                                    <asp:TemplateField HeaderText="Approved">
                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="ngfd_ddlApproved" runat="server" SelectedValue='<%# Eval("Approved") %>'
                                                                Style="width: 120px; padding: 5px; border: 1px solid #ccc;">
                                                                <asp:ListItem Text="Select" Value="" />
                                                                <asp:ListItem Text="Yes" Value="YES" />
                                                                <asp:ListItem Text="No" Value="NO" />
                                                            </asp:DropDownList>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <%-- ddl ngfd_ddlIsNominee --%>
                                                    <asp:TemplateField HeaderText="Is Nominee">
                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="ngfd_ddlIsNominee" runat="server" SelectedValue='<%# DataBinder.Eval(Container.DataItem, "IsNominee") %>'
                                                                Style="width: 120px; padding: 5px; border: 1px solid #ccc;">
                                                                <asp:ListItem Text="Select" Value="" />
                                                                <asp:ListItem Text="Yes" Value="YES" />
                                                                <asp:ListItem Text="No" Value="NO" />
                                                            </asp:DropDownList>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <%-- ngfd_txtAllocation --%>
                                                    <asp:TemplateField HeaderText="Allocation">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="ngfd_txtAllocation" runat="server" MaxLength="100"
                                                                Text='<%# Eval("Allocation") %>'
                                                                Style="width: 180px; padding: 5px; border: 1px solid #ccc;"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                        <br />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <%-- Validate memebr dob for gname  --%>
                            <script>
                                function validateGuardianRequirementMember() {
                                    var dobInput = document.getElementById('ngfd_txtDOB'); // Change to your actual DOB field ID
                                    var guardianNameInput = document.getElementById('ngfd_txtgName');

                                    if (!dobInput || !guardianNameInput) return; // Ensure elements exist

                                    var dobValue = dobInput.value.trim();
                                    if (dobValue === "") return; // Exit if DOB is empty

                                    // Parse DOB
                                    var parts = dobValue.split('/');
                                    if (parts.length !== 3) return; // Invalid format

                                    var dob = new Date(parts[2], parts[1] - 1, parts[0]); // Convert dd/MM/yyyy to Date object
                                    var today = new Date();
                                    var age = today.getFullYear() - dob.getFullYear();

                                    // Adjust age if the birthday hasn't occurred this year yet
                                    var monthDiff = today.getMonth() - dob.getMonth();
                                    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < dob.getDate())) {
                                        age--;
                                    }

                                    if (age < 18) {
                                        // Add required attribute
                                        guardianNameInput.setAttribute("required", "required");

                                        // Show alert if field is empty
                                        if (guardianNameInput.value.trim() === "") {
                                            alert("Guardian Name is required for applicants under 18.");
                                        }
                                    } else {
                                        // Remove required attribute if 18 or older
                                        guardianNameInput.removeAttribute("required");
                                    }
                                }

                            </script>

                         

                            <asp:Button runat="server" ID="btnSave" CssClass="btn btn-primary" Text="Save A/C" OnClick="btnInsert_Click" AccessKey="S" OnClientClick="showServerLoader(); return true;"/>
                            <asp:Button runat="server" ID="btnUpdate" CssClass="btn btn-primary" Text="Modify A/C" OnClick="btnUpdate_Click" AccessKey="M" OnClientClick="showServerLoader(); return true;" />
                            <asp:Button ID="btnApprove" runat="server" CssClass="btn btn-outline-primary" Text="Approve" OnClick="ApproveButton_Click" OnClientClick="showServerLoader(); return true;" />
                            <asp:Button runat="server" CssClass="btn btn-outline-primary" Text="Back" OnClientClick="document.getElementById('basic_details_tab').click(); return false;" AccessKey="B" />
                            <asp:Button runat="server" CssClass="btn btn-outline-primary" Text="Exit" OnClick="ExitButton_Click" OnClientClick="showServerLoader(); return true;"   />


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
                                    <asp:Label runat="server" CssClass="form-label mt-1 font-weight-bold" ID="lblClientAdvisoryInfo" Text="" />
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
                                                    <asp:UpdatePanel runat="server">
                                                        <ContentTemplate>


                                                            <div class="row g-3">
                                                                <%-- Cheque, draft, cash --%>
                                                                <div class="col-md-12">
                                                                    <div class="col-md-4 d-flex align-items-center gap-3 mt-3">
                                                                        <%--OnCheckedChanged="PaymentTypeChanged" AutoPostBack="True" --%>
                                                                        <asp:RadioButton runat="server" ID="cheque" GroupName="payment-type" Text="Cheque"
                                                                            OnCheckedChanged="PaymentTypeChanged" AutoPostBack="True" />
                                                                        <asp:RadioButton runat="server" ID="draft" GroupName="payment-type" Text="Draft"
                                                                            OnCheckedChanged="PaymentTypeChanged" AutoPostBack="True" />
                                                                        <asp:RadioButton runat="server" ID="optCash" GroupName="payment-type" Text="Cash"
                                                                            OnCheckedChanged="PaymentTypeChanged" AutoPostBack="True" CssClass="d-none" />

                                                                    </div>
                                                                </div>

                                                                <%-- Cheque no --%>
                                                                <div class="col-md-4">
                                                                    <asp:Label runat="server" CssClass="form-label" ID="ChequeLabel"
                                                                        Text="Cheque No <span class='text-danger'>*</span>" />
                                                                    <asp:TextBox runat="server" ID="txtChequeNo" CssClass="form-control" MaxLength="20" />
                                                                </div>

                                                                <%-- Cheque date --%>
                                                                <div class="col-md-4">
                                                                    <asp:Label runat="server" CssClass="form-label" ID="ChequeDatedLabel"
                                                                        Text="Cheque Dated <span class='text-danger'>*</span>" />
                                                                    <div class="date input-group" data-provide="datepicker">
                                                                        <asp:TextBox runat="server" ID="txtChequeDate" CssClass="form-control" />
                                                                        <div class="input-group-addon">
                                                                            <span class="glyphicon glyphicon-th"></span>
                                                                        </div>
                                                                    </div>

                                                                    <script>
                                                                        $(document).ready(function () {
                                                                            $('.date').datepicker({
                                                                                format: 'dd-mm-yyyy', // Sets date format to DD-MM-YYYY
                                                                                autoclose: true,      // Closes the picker after selection
                                                                                todayHighlight: true  // Highlights today's date
                                                                            });
                                                                        });
                                                                    </script>

                                                                </div>

                                                                <%-- Amount --%>
                                                                <div class="col-md-4">
                                                                    <asp:Label runat="server" CssClass="form-label" Text="Amount" />
                                                                    <asp:TextBox runat="server" ID="txtAmount" CssClass="form-control"
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

                                                            </div>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                    <div class="col-md-12">
                                                        <div class="mt-2 gap-3">
                                                            <asp:Button runat="server" Enabled="false" ID="btnAdvGenerate"
                                                                CssClass="btn btn-primary" Text="Generate AR"
                                                                OnClick="btnInsertAdvisoryTransaction_Click"
                                                                OnClientClick="showServerLoader(); return true;" />
                                                            <asp:Button runat="server" Enabled="false" ID="btnAdvPrint"
                                                                CssClass="btn btn-primary" Text="Print AR"
                                                                OnClick="btnPrintARReport_Click" />
                                                            <asp:Button runat="server" Enabled="false" ID="btnAdvUpdate"
                                                                CssClass="btn btn-outline-primary" Text="Modify AR"
                                                                OnClick="btnUpdateAdvisoryTransaction_Click"
                                                                OnClientClick="showServerLoader(); return true;" />

                                                        </div>
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



        <asp:Label runat="server" ID="lblMessage" CssClass="text-danger" Text ="" />

     


</asp:Content>
