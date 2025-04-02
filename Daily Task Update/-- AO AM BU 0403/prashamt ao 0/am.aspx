<%@ Page Title="Associate Master" Language="C#" MasterPageFile="~/vmSite.Master" AutoEventWireup="true" CodeBehind="associate_master.aspx.cs" Inherits="WM.Masters.associate_master" EnableViewState="true" MaintainScrollPositionOnPostback="false"   %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">



      <!-- Loader HTML, CSS, JS -->

    <%-- NEW CHANGES --%>
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


    <style>
        /* Modal styles */

        .modal-backdrop.show {
    z-index: -1;
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
            background-color: rgb(0,0,0);
            background-color: rgba(0,0,0,0.4);
        }

        .modal-content {
            background-color: #fefefe;
            margin: 10% auto;
            margin-top:80px;
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

            .close:hover, .close:focus {
                color: black;
                text-decoration: none;
                cursor: pointer;
            }

        span[style="visibility:hidden;"],
        span[style="visibility: hidden;"] {
            display: none !important;
        }
    </style>
 
        

    <script>
        function validatePanInput(input) {
            var regex = /^[a-zA-Z0-9]*$/;
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
                alert("The mobile number cannot exceed " + maxLength + " digits.");
                input.value = input.value.slice(0, maxLength);  // Trim the input to max length
                return;
            }

            // Check if the last character is valid (numeric)
            var lastChar = input.value.slice(-1);
            if (!regex.test(lastChar)) {
                alert("The character '" + lastChar + "' is not allowed. Only numeric characters are permitted.");
                input.value = input.value.slice(0, -1); // Remove the last character
                input.focus();
            }
        }
        function validateAadharNumber(input, maxLength = 12) {
            var regex = /^[0-9]*$/;  // Regular expression to allow only numeric characters

            // Check if the length exceeds max length
            if (input.value.length > maxLength) {
                alert("The aadhar number cannot exceed " + maxLength + " digits.");
                input.value = input.value.slice(0, maxLength);  // Trim the input to max length
                return;
            }

            // Check if the last character is valid (numeric)
            var lastChar = input.value.slice(-1);
            if (!regex.test(lastChar)) {
                alert("The character '" + lastChar + "' is not allowed. Only numeric characters are permitted.");
                input.value = input.value.slice(0, -1); // Remove the last character
                input.focus();
            }
        }
        function validateFaxNumber(input, maxLength = 20) {
            var regex = /^[0-9\s\-]*$/;  // Regular expression to allow digits, spaces, and hyphens

            // Check if the length exceeds max length
            if (input.value.length > maxLength) {
                alert("The fax number cannot exceed " + maxLength + " characters.");
                input.value = input.value.slice(0, maxLength);  // Trim the input to max length
                return;
            }

            // Check if the last character is valid (numeric, space, or hyphen)
            var lastChar = input.value.slice(-1);
            if (!regex.test(lastChar)) {
                alert("The character '" + lastChar + "' is not allowed. Only numeric digits, spaces, and hyphens are permitted.");
                input.value = input.value.slice(0, -1); // Remove the last character
                input.focus();
            }
        }
        function validatePinCode(input, maxLength = 10) {
            var regex = /^[0-9]*$/;  // Regular expression to allow only numeric characters

            // Check if the length exceeds max length
            if (input.value.length > maxLength) {
                alert("The pin code cannot exceed " + maxLength + " characters.");
                input.value = input.value.slice(0, maxLength);  // Trim the input to max length
                return;
            }

            // Check if the last character is valid (only numeric)
            var lastChar = input.value.slice(-1);
            if (!regex.test(lastChar)) {
                alert("The character '" + lastChar + "' is not allowed. Only numeric digits are permitted.");
                input.value = input.value.slice(0, -1); // Remove the last character
                input.focus();
            }
        }
    </script>

    <div class="page-header mb-2">
        <h3 class="page-title">Associate Master</h3>
    </div>
    <p>Here you can create a new associate record or modify an existing one.</p>
    <div class="row">
        <!-- Associate Registration Form -->
        <div class="grid-margin stretch-card">
            <div class="card">
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-6">
                            <h2 class="card-title">Associate Registration</h2>
                        </div>
                        <div class="col-md-6 text-end">
                            <%--<asp:Button ID="associateMasterViewModel" CssClass="btn btn-primary" data-toggle="modal" data-target="#associateListModal" runat="server" Text="View" OnClientClick="openAssociateListModal(); return false;" />--%>
                            <asp:Button ID="associateMasterViewModel" CssClass="btn btn-primary"  runat="server" Text="View" OnClick="btnSearchAssociate" />


                        </div>
                    </div>
                    <div class="row g-3">
                        <!-- Form Fields -->
                        <div class="col-md-2">
                            <div class="form-group">
                                <label for="associate-code" class="form-label">Associate Code<span class="text-danger">*</span></label>
                                <asp:TextBox ID="associateCode" CssClass="form-control" runat="server" ReadOnly="True" MaxLength="8"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-md-4">
                            <div class="form-group">
                                <label for="sub-broker-exist-code" class="form-label">Exist Code<span class="text-danger">*</span> <span class="text-muted small">(Auto generated ANA code)</span></label>
                                <asp:TextBox ID="subBrokerExistCode" CssClass="form-control" runat="server" ReadOnly="True" MaxLength="20"></asp:TextBox>
                            </div>
                        </div>
                      
                        <div class="col-md-4">
                            <div class="form-group">
                                <label for="btnDTNumber" class="form-label">DT Number<span class="text-danger">*</span></label>

                                <asp:TextBox runat="server" ID="txtDTNumber" CssClass="form-control" MaxLength="20" />
                            </div>
                        </div>
               
                        <div class="col-md-2 text-end">
                            <div class="form-group mt-1">
                                <asp:Button ID="btnDTNumber" runat="server" CssClass="btn btn-outline-primary mt-3"  Text="Get by DT" OnClick="oneClientSearchByDT_Click" />

                            </div>

                        </div>

                        <div class="col-md-2">
                            <div class="form-group">
                                <label for="empanelment-type" class="form-label">Empanelment Type</label>
                                <asp:DropDownList ID="empanelmentType" CssClass="form-select" runat="server">
                                    <asp:ListItem Value="F">Free</asp:ListItem>
                                    <asp:ListItem Value="P">Paid</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-md-2">
                            <div class="form-group">

                            <label  CssClass="form-label">Title<span class='text-danger'>*</span></label>
                            <asp:DropDownList runat="server" ID="ddlTitle" CssClass="form-select">
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
                        </div>
                        
                        <div class="col-md-4">
                            <div class="form-group">
                                <label for="associate-name" class="form-label">Associate Name<span class="text-danger">*</span></label>
                                <asp:TextBox ID="associateName" CssClass="form-control" runat="server"
                                    MaxLength="150"></asp:TextBox>
                                
                                
                                <%--<asp:RequiredFieldValidator runat="server" ControlToValidate="associateName" ErrorMessage="Associate Name is required" CssClass="text-danger small d-block mt-1" SetFocusOnError="true"></asp:RequiredFieldValidator>--%>
                            </div>
                        </div>
                       
                        <script>
                            document.addEventListener("DOMContentLoaded", function () {
                                var source1 = document.getElementById('<%= associateName.ClientID %>');
                                var target2 = document.getElementById('<%= contactPerson.ClientID %>');


                                if (source1 && target2) {
                                    source1.addEventListener("input", function () {
                                        target2.value = source1.value;
                                    });
                                }


                               
                            });

                            document.addEventListener("DOMContentLoaded", function () {
                                var source2 = document.getElementById('<%= emailId.ClientID %>');
                                var target2 = document.getElementById('<%= contactPersionemailId.ClientID %>');


                                if (source2 && target2) {
                                    source2.addEventListener("input", function () {
                                        target2.value = source2.value;
                                    });
                                }
                               
                            });
</script>
                        <script>
                            function syncInput(source, targetId) {
                                var target = document.getElementById(targetId);
                                if (target) {
                                    target.value = source.value;
                                }
                            }

                        </script>

                        <div class="col-md-2">
                            <div class="form-group">
                                <label class="form-label">Associate Type</label>
                                <div class="form-check">
                                    <label class="form-check-label">
                                        Associate
                                        <input type="radio" id="radioAssociateType" name="AssociateType" checked />
                                    </label>
                                    <script>
                                        document.getElementById('radioAssociateType').addEventListener('click', function (event) {
                                            if (!this.checked) {
                                                this.checked = true; // Force it to always stay checked
                                            }
                                        });
                                    </script>

                                </div>
                            </div>
                        </div>

                        <div class="col-md-2">
                            <div class="form-group">
                                <label for="empanelment-date" class="form-label">Empanelment Date </label>
                                <div>
                                    <asp:TextBox ID="empanelmentDate" CssClass="form-control" runat="server" ReadOnly="True" PLACEHOLDER="(DD-MM-YYYY)" MaxLength="10"></asp:TextBox>
                                </div>
                            </div>
                        </div>


                        <div class="col-12">
                            <h4 class="card-title">Associate Source</h4>
                            <div class="row g-3">
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <label for="select-branch" class="form-label">Select Branch <span class="text-danger">*</span></label>
                                        <asp:DropDownList Enable="false" ID="ddlBranchAM" CssClass="form-select" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSourceID_SelectedIndexChanged">
                                            <asp:ListItem Text="Select Branch" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                        <%--<asp:RequiredFieldValidator runat="server" ControlToValidate="ddlBranchAM" ErrorMessage="Select Branch Name" CssClass="text-danger small d-block mt-1" SetFocusOnError="true"></asp:RequiredFieldValidator>--%>
                                    </div>
                                </div>

                                <div class="col-md-4">
                                    <div class="form-group">
                                        <label for="select-employee" class="form-label">Select Employee</label>
                                        <asp:DropDownList Enable="false" ID="selectEmployee" CssClass="form-select" runat="server" Enabled="false">
                                            <asp:ListItem Text="Select Branch First" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                            </div>
                        </div>

                        <div class="col-12">
                            <h4 class="card-title">Office</h4>
                            <style>
                                .update-panel {
    display: contents; /* Ensures UpdatePanel behaves like a natural container */
}

                            </style>
                  
                            <asp:UpdatePanel ID="upPnlMailinAddress" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false" Style="display: contents;" CssClass="update-panel">
                                <ContentTemplate>
                                    <div class="row g-3">
                                        <div class="col-md-4">
                                            <%-- address1 --%>
                                            <div class="form-group">
                                                <label for="address1" class="form-label">Address 1</label>
                                                <asp:TextBox ID="address1" CssClass="form-control" TextMode="MultiLine"
                                                    MaxLength="200"
                                                    Rows="3" Columns="10" runat="server"
                                                    placeholder="Address 1"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-md-4">

                                            <%-- address2 --%>
                                            <div class="form-group">
                                                <label for="address2" class="form-label">Address 2</label>
                                                <asp:TextBox ID="address2" CssClass="form-control" TextMode="MultiLine"
                                                    MaxLength="200"
                                                    Rows="3" Columns="10" runat="server"
                                                    placeholder="Address 2"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-md-4">

                                            <%-- addres 3 --%>
                                            <div class="form-group">
                                                <label for="address2" class="form-label">Address 3</label>
                                                <asp:TextBox ID="address3" CssClass="form-control" TextMode="MultiLine"
                                                    MaxLength="200"
                                                    Rows="3" Columns="10" runat="server"
                                                    placeholder="Address 3"></asp:TextBox>
                                            </div>
                                        </div>

                                        <%-- mailingcountry --%>
                                        <div class="col-md-2 mb-1">

                                            <asp:Label runat="server" CssClass="form-label" Text="Country <span class='text-danger'>*</span>" />
                                            <asp:DropDownList runat="server" ID="ddlMailingCountryList" CssClass="form-select" AutoPostBack="true"
                                                OnSelectedIndexChanged="ddlMailingCountryList_SelectedIndexChanged">
                                                <asp:ListItem Value="0">Select Country</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>

                                        <%-- mailing state --%>
                                        <div class="col-md-2">
                                            <asp:Label runat="server" CssClass="form-label" Text="State <span class='text-danger'>*</span>" />
                                            <asp:DropDownList runat="server" ID="ddlMailingStateList" CssClass="form-select" AutoPostBack="true"
                                                OnSelectedIndexChanged="ddlMailingStateList_SelectedIndexChanged">
                                                <asp:ListItem Value="0">Select State</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>

                                        <%-- mailing city --%>
                                        <div class="col-md-4">
                                            <asp:Label runat="server" CssClass="form-label" Text="City <span class='text-danger'>*</span>" />
                                            <div class="input-group">
                                                <asp:DropDownList runat="server" ID="ddlMailingCityList" AutoPostBack="true"
                                                    OnSelectedIndexChanged="ddlMailingCityList_SelectedIndexChanged"
                                                    CssClass="form-select">
                                                    <asp:ListItem Value="0">Select City</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:Button ID="newCityButton" CssClass="btn btn-outline-primary ms-2" runat="server" Text="New" OnClick="btnNewCityClick" CausesValidation="false" />
                                            </div>
                                        </div>

                                        <%-- mailing location --%>
                                        <div class="col-md-4">
                                            <div class="form-group">
                                                <label for="area" class="form-label">Location</label>
                                                <asp:DropDownList ID="ddlLocationAM" CssClass="form-select" AutoPostBack="true"
                                                    OnSelectedIndexChanged="ddlMailingLocationList_SelectedIndexChanged"
                                                    runat="server">
                                                    <asp:ListItem Value="" Text="Select"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>

                                        <%-- mailing pin --%>
                                        <div class="col-md-4">
                                            <asp:Label runat="server" CssClass="form-label" Text="Pin" />
                                            <div class="input-group">

                                                <asp:TextBox
                                                    runat="server"
                                                    ID="txtMailingPin"
                                                    CssClass="form-control"
                                                    MaxLength="8"
                                                    oninput="trimInputNumber(this, 8); validateNumericInput(this);"
                                                    onpaste="validatePasteNumber(event, this);" />

                                                <button type="button" class="btn btn-primary" onclick="openResAddModal()" title="Residence Address">Residence</button>

                                            </div>
                                        </div>

                                        <div class="col-md-2">
                                            <div class="form-group">
                                                <label for="mobile" class="form-label">Mobile <span class="text-danger">*</span></label>
                                                <asp:TextBox ID="mobile" CssClass="form-control" runat="server"
                                                    onkeypress="return isMobileNumberKey(event)" MaxLength="10"></asp:TextBox>

                                            </div>
                                        </div>
                                        <div class="col-md-3">
                                            <div class="form-group">
                                                <label for="phone" class="form-label">Phone</label>
                                                <asp:TextBox ID="phone" CssClass="form-control" runat="server" onkeypress="return isMobileNumberKey(event)" MaxLength="20"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-3">
                                            <div class="form-group">
                                                <label for="fax" class="form-label">Fax No.</label>
                                                <asp:TextBox ID="fax" CssClass="form-control" runat="server"
                                                    onkeypress="return isMobileNumberKey(event)" MaxLength="10" onpaste="validatePasteNumber(event, this);"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-1">
                                            <div class="form-group">
                                                <label for="tds" class="form-label">TDS %</label>
                                                <asp:TextBox ID="tds" CssClass="form-control" runat="server" onkeypress="return isTDSCOMKey(event)"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-3">
                                            <div class="form-group">
                                                <label for="emailId" class="form-label">Email Id <span class="text-danger">*</span></label>
                                                <asp:TextBox ID="emailId" CssClass="form-control" runat="server" MaxLength="50"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-2">
                                            <label for="associate-type" class="form-label">Associate Type <span class="text-danger">*</span></label>
                                            <asp:DropDownList ID="ddlAssociateType" CssClass="form-select" runat="server">
                                                <asp:ListItem Value="" Text="Select Associate Type"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-2">
                                            <label for="associate-type-category" class="form-label">Associate Category </label>
                                            <asp:DropDownList ID="ddlAssociateTypeCategory" CssClass="form-select" runat="server">
                                                <asp:ListItem Value="" Text="Select Associate Category"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-4">
                                            <div class="form-group">
                                                <label for="contact-person" class="form-label">Contact Person <span class="text-danger">*</span></label>
                                                <asp:TextBox ID="contactPerson" CssClass="form-control" runat="server" MaxLength="100"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-4">
                                            <div class="form-group">
                                                <label for="contactPersionemailId" class="form-label">Contact Person Email Id </label>
                                                <asp:TextBox ID="contactPersionemailId" CssClass="form-control" runat="server" MaxLength="50"></asp:TextBox>

                                            </div>
                                        </div>
                                    </div>

                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ddlMailingStateList" EventName="SelectedIndexChanged" />

                                    <asp:AsyncPostBackTrigger ControlID="ddlMailingCountryList" EventName="SelectedIndexChanged" />

                                    <asp:AsyncPostBackTrigger ControlID="ddlMailingCityList" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="ddlLocationAM" EventName="SelectedIndexChanged" />

                                </Triggers>
                            </asp:UpdatePanel>

                            <h2 class="card-title">Other Details</h2>
                            <div class="row g-3 mb-3">
                                <div class="col-md-1">
                                    <label class="form-label">SMS</label>
                                    <div class="form-check">
                                        <label class="form-check-label">
                                            Y/N
                                                             
                                  <asp:CheckBox ID="sms" runat="server" />
                                        </label>
                                    </div>
                                </div>

                                <div class="col-md-2">
                                    <label for="gstin" class="form-label">GSTIN No</label>
                                    <asp:TextBox ID="gstin" CssClass="form-control" runat="server" MaxLength="15"></asp:TextBox>
                                    <script>
                                        function validateGSTNumber() {
                                            var gstInput = document.getElementById('<%= gstin.ClientID %>').value;

                                            // Regular expression to match the GST number format
                                            var gstRegex = /^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[A-Z0-9]{1}$/;

                                            // Check if the input matches the GST number format
                                            if (gstRegex.test(gstInput)) {
                                                return true;  // Valid GST number, allow form submission
                                            } else {
                                                alert("Invalid GST number. Please enter a valid GST number.");
                                                return false;  // Invalid GST number, block form submission
                                            }
                                        }

                                    </script>
                                </div>


                                <div class="col-md-3">
                                    <label for="dob" class="form-label">DOB/Incorporation <span class="text-danger">*</span></label>
                                    <div class="input-group date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                        <asp:TextBox ID="dob" CssClass="form-control" runat="server" PLACEHOLDER="DD-MM-YY" MaxLength="10"
                                            ToolTip="Enter Date of Birth" aria-label="Date of Birth"
                                            oninput="formatDOBInput(this)"></asp:TextBox>
                                        <div class="input-group-addon">
                                            <span class="glyphicon glyphicon-th"></span>
                                        </div>
                                    </div>
                                    <script>

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
                                                formattedValue += value.substring(4, 8);
                                            }

                                            input.value = formattedValue;
                                        }
                                    </script>
                                </div>

                                <div class="col-md-2">
                                    <label for="ddlGender" class="form-label">Gender <span class='text-danger'>*</span></label>
                                    <asp:DropDownList runat="server" ID="ddlGender" CssClass="form-select">
                                        <asp:ListItem Text="Select" Value="" />
                                        <asp:ListItem Text="Male" Value="MALE" />
                                        <asp:ListItem Text="Female" Value="FEMALE" />
                                        <asp:ListItem Text="Non-Ind" Value="NON-IND" />
                                    </asp:DropDownList>
                                </div>

                                <div class="col-md-2">
                                    <label for="type" class="form-label">Type</label>
                                    <asp:DropDownList ID="type" CssClass="form-select" runat="server">
                                        <asp:ListItem Value="">Select Type</asp:ListItem>
                                        <asp:ListItem Value="1">1</asp:ListItem>
                                        <asp:ListItem Value="2">2</asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <div class="col-md-2">
                                    <label for="pan-gir" class="form-label">PAN/GIR No <span class="text-danger">*</span></label>
                                    <asp:TextBox ID="panGir" CssClass="form-control" runat="server"
                                        MaxLength="10"
                                        placeholder="Enter PAN (ABCDE1234F)"
                                        pattern="[A-Z]{5}[0-9]{4}[A-Z]{1}"
                                        title="Enter PAN in format: ABCDE1234F"
                                        oninput="validatePanInput2(this)"></asp:TextBox>

                                    <%--  OnClientClick="return validatePAN();" --%>
                                    <script>

                                        function validatePAN() {
                                            var panInput = document.getElementById('<%= panGir.ClientID %>').value;

                                            // Regular expression to match the PAN format
                                            var panRegex = /^[A-Z]{5}[0-9]{4}[A-Z]{1}$/;

                                            // Check if the input matches the PAN format
                                            if (panRegex.test(panInput)) {
                                                return true;  // Valid PAN, allow form submission
                                            } else {
                                                alert("Invalid PAN number. PAN should be in the format AAAAA9999A.");
                                                return false;  // Invalid PAN, block form submission
                                            }
                                        }

                                    </script>
                                </div>

                                <div class="col-md-3">
                                    <label for="aadhar-card" class="form-label">Aadhar Card</label>
                                    <asp:TextBox ID="aadharCard" CssClass="form-control" runat="server"
                                        MaxLength="12"
                                        oninput="trimInputNumber(this, 12); validateNumericInput(this);"
                                        onpaste="validatePasteNumber(event, this);"></asp:TextBox>
                                </div>

                                <div class="col-md-3">
                                    <label for="circle-ward-dist" class="form-label">Circle/Ward/Dist</label>
                                    <asp:TextBox ID="circleWardDist" CssClass="form-control" runat="server" MaxLength="100"></asp:TextBox>
                                </div>
                            </div>


                            <h4 class="card-title">Remarks/Source</h4>
                            <div class="row g-3">

                            <asp:UpdatePanel ID="uppSuperANADD" runat="server" UpdateMode="Conditional" EnableViewState="true">
                                <ContentTemplate>
                            <div class="row">

                               

                                <div class="col-md-3">
                                    <div class="form-group">
                                        <label for="remarks" class="form-label">Remarks</label>
                                        <asp:TextBox ID="remarks" CssClass="form-control" runat="server" MaxLength="100"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-md-3">
                                    <div class="form-group">
                                        <label for="source" class="form-label">Source</label>
                                        <div class="input-group">
                                            <asp:TextBox ID="source" CssClass="form-control" runat="server" ReadOnly="true" MaxLength="50"></asp:TextBox>
                                            <asp:Button ID="sourceButton" CssClass="btn btn-outline-primary ms-2 d-none" runat="server" Text="Find" Enabled="false" />
                                        </div>
                                    </div>
                                </div>

                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="super-ana" class="form-label">Super ANA </label>
                                        <div class="input-group">
                                            <asp:DropDownList ID="superAna" CssClass="form-select" runat="server">
                                                <asp:ListItem Value="0" Text="Selct Super ANA"></asp:ListItem>
                                                <asp:ListItem Value="1">One</asp:ListItem>
                                                <asp:ListItem Value="2">Two</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:Button ID="selectSuperANA" CssClass="btn btn-outline-primary ms-2" runat="server" Text="Find" OnClientClick="openSuperAnaFindModal(); return false;" />
                                        </div>
                                    </div>

                            </div>

                                    </ContentTemplate>
</asp:UpdatePanel>
                                </div>



                                <div class="col-md-2">

                                    <div class="form-group">
                                        <div class="form-check">
                                            <label class="form-check-label">
                                                Apply 
                                                         <asp:CheckBox ID="onlineSubscription" runat="server" />

                                            </label>
                                        </div>
                                        <label for="onlinePlatformRemark" class="form-label" tooltip="Apply Online Subscription">Online Subscription</label>

                                    </div>
                                </div>

                                <div class="col-md-4">
                                    <div class="form-group">
                                        <div class="form-check">
                                            <label class="form-check-label">
                                                Online Platform Block    
                                                        <asp:CheckBox ID="chkbOnlinePlaformBlock" runat="server" onclick="toggleOnlineRemark(this)" />
                                            </label>
                                        </div>
                                        <label for="onlinePlatformRemark" class="form-label">Online Remark</label>
                                        <div class="input-group">
                                            <asp:TextBox ID="onlinePlatformRemark" CssClass="form-control" runat="server" Enabled="false" MaxLength="100"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-md-4">
                                    <div class="form-check">
                                        <label class="form-check-label">
                                            Offline Platform Block    
                                                    <asp:CheckBox ID="chkbOfflinePlaformBlock" runat="server" onclick="toggleOfflineRemark(this)" />
                                        </label>
                                    </div>

                                    <label for="offlinePlatformRemark" class="form-label">Offline Remark</label>
                                    <div class="input-group">
                                        <asp:TextBox ID="offlinePlatformRemark" CssClass="form-control" runat="server" Enabled="false" MaxLength="100"></asp:TextBox>
                                    </div>
                                </div>

                                <script type="text/javascript">
                                    function toggleOnlineRemark(checkbox) {
                                        var textBox = document.getElementById('<%= onlinePlatformRemark.ClientID %>');
                                        textBox.disabled = !checkbox.checked;

                                        if (!checkbox.checked) {
                                            textBox.value = ""; // Clear text when unchecked
                                        }
                                    }

                                    function toggleOfflineRemark(checkbox) {
                                        var textBox = document.getElementById('<%= offlinePlatformRemark.ClientID %>');
                                        textBox.disabled = !checkbox.checked;
                                        if (!checkbox.checked) {
                                            textBox.value = ""; // Clear text when unchecked
                                        }
                                    }

                                    function toggleAuditCheck(checkbox) {
                                        var textBox = document.getElementById('<%= auditDate.ClientID %>');
                                        textBox.disabled = !checkbox.checked;

                                        if (!checkbox.checked) {
                                            textBox.value = ""; // Clear text when unchecked
                                        }
                                    }

                                    window.onload = function () {
                                        var onlineCheckbox = document.getElementById('<%= chkbOnlinePlaformBlock.ClientID %>');
                                        if (onlineCheckbox) {
                                            toggleOnlineRemark(onlineCheckbox);
                                            onlineCheckbox.addEventListener("change", function () {
                                                toggleOnlineRemark(onauditheckbox);
                                            });
                                        }

                                        var offlineCheckbox = document.getElementById('<%= chkbOfflinePlaformBlock.ClientID %>');
                                        if (offlineCheckbox) {
                                            toggleOfflineRemark(offlineCheckbox);
                                            offlineCheckbox.addEventListener("change", function () {
                                                toggleOfflineRemark(offlineCheckbox);
                                            });
                                        }

                                        var auditCheckbox = document.getElementById('<%= audit.ClientID %>');
                                        if (auditCheckbox) {
                                            toggleAuditCheck(auditCheckbox);
                                            auditCheckbox.addEventListener("change", function () {
                                                toggleAuditCheck(auditCheckbox);
                                            });
                                        }
                                    };
                                </script>

                                <%-- Hide audit section on 212 --%>
                                <script>
                                    document.addEventListener("DOMContentLoaded", function () {
                                        // Get session value from backend
                                        var roleId = '<%= Session["RoleId"] %>'; // Assuming RoleId is stored in session

                                        // Check if session value is "212" and hide the div
                                        if (roleId === "261") {
                                            document.getElementById("audit-section").classList.add("d-none");
                                        }
                                    });

                                </script>
                                <div class="col-md-2" id="audit-section">
                                    <div class="form-check">
                                        <label class="form-check-label">
                                            Audit Check
            <asp:CheckBox ID="audit" runat="server" onclick="toggleAuditCheck(this)" />
                                        </label>
                                    </div>

                                    <label for="offlinePlatformRemark" class="form-label">Audit Date</label>
                                    <div class="input-group date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                        <asp:TextBox ID="auditDate" CssClass="form-control" runat="server" placeholder="DD-MM-YY" oninput="formatDOBInput(this)" Enabled="false" MaxLength="10"></asp:TextBox>
                                        <div class="input-group-addon">
                                            <span class="glyphicon glyphicon-th"></span>
                                        </div>
                                    </div>
                                </div>



                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </div>



        <div class="col-md-12 grid-margin stretch-card">
            <div class="card">
                <div class="card-body">
                    <h2 class="card-title">Bank Details</h2>
                    <div class="row g-3">
                        <div class="col-md-2">
                            <label for="payment-mode" class="form-label">Payment Mode</label>
                            <asp:DropDownList ID="paymentMode" CssClass="form-select" runat="server">
                                <asp:ListItem Value="">Select Payment Mode</asp:ListItem>
                                <asp:ListItem Value="cash">Cash</asp:ListItem>
                                <asp:ListItem Value="cheque">Cheque</asp:ListItem>
                                <asp:ListItem Value="transfer">Transfer</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="col-md-3">
                            <label for="account-type" class="form-label">Account Type</label>
                            <asp:DropDownList ID="accountType" CssClass="form-select" runat="server">
                                <asp:ListItem Value="">Select Account Type</asp:ListItem>
                                <asp:ListItem Value="savings">Savings</asp:ListItem>
                                <asp:ListItem Value="checking">Checking</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="col-md-5">
                            <label for="account-no" class="form-label">Account No</label>
                            <asp:TextBox ID="accountNo" CssClass="form-control" runat="server" MaxLength="30"></asp:TextBox>
                        </div>

                        <div class="col-md-2">
                            <label for="affected-from" class="form-label">Affected From </label>
                            <div class="input-group date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                <asp:TextBox ID="affectedFrom" CssClass="form-control" runat="server" placeholder="DD-MM-YY" oninput="formatDOBInput(this)" MaxLength="10"></asp:TextBox>
                                <div class="input-group-addon">
                                    <span class="glyphicon glyphicon-th"></span>
                                </div>
                            </div>
                        </div>

                        <div class="col-md-5">
                            <label for="bank-name" class="form-label">Bank Name</label>
                            <div class="input-group">
                                <asp:DropDownList ID="bankName" CssClass="form-select" runat="server">
                                    <asp:ListItem Value="">Select Bank</asp:ListItem>
                                    <asp:ListItem Value="bank1">Bank 1</asp:ListItem>
                                    <asp:ListItem Value="bank2">Bank 2</asp:ListItem>
                                </asp:DropDownList>
                                <asp:Button ID="newBankButton" CssClass="btn btn-outline-primary" runat="server" Text="New" OnClick="btnNewBankClick" />
                            </div>
                        </div>


                        <div class="col-md-5">
                            <label for="branch-name" class="form-label">Bank Branch Name</label>
                            <div class="input-group">
                                <asp:DropDownList ID="branchName" CssClass="form-select" runat="server">
                                    <asp:ListItem Value="">Select Branch</asp:ListItem>
                                    <asp:ListItem Value="branch1">Branch 1</asp:ListItem>
                                    <asp:ListItem Value="branch2">Branch 2</asp:ListItem>
                                </asp:DropDownList>
                                <asp:Button ID="newBranchButton" CssClass="btn btn-outline-primary" runat="server" Text="New" OnClick="btnNewBranchClick" />
                            </div>
                        </div>

                        <div class="col-md-2">
                            <label for="bank-city" class="form-label">City</label>
                            <asp:DropDownList ID="ddlBankCityAM" CssClass="form-select" runat="server">
                                <asp:ListItem Value="">Select City</asp:ListItem>
                                <asp:ListItem Value="1">1</asp:ListItem>
                                <asp:ListItem Value="2">2</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>

                <div class="card-body">
                    <h2 class="card-title">NEFT Details</h2>
                    <div class="row g-3">
                        <div class="col-md-4">
                            <label for="neftBankName" class="form-label">Bank Name</label>
                            <asp:TextBox ID="neftBankName" CssClass="form-control" runat="server" ReadOnly="True"></asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <label for="neftBranch" class="form-label">Branch</label>
                            <asp:TextBox ID="neftBranch" CssClass="form-control" runat="server" ReadOnly="True"></asp:TextBox>
                        </div>

                        <div class="col-md-4">
                            <label for="neftIFCSCode" class="form-label">IFSC Code</label>
                            <asp:TextBox ID="neftIFCSCode" CssClass="form-control" runat="server" ReadOnly="True"></asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <label for="neftAccountNo" class="form-label">Account No</label>
                            <asp:TextBox ID="neftAccountNo" CssClass="form-control" runat="server" ReadOnly="True"></asp:TextBox>
                        </div>

                        <div class="col-md-4">
                            <label for="neftName" class="form-label">Name as per Bank</label>
                            <asp:TextBox ID="neftName" CssClass="form-control" runat="server" ReadOnly="True"></asp:TextBox>
                        </div>
                    </div>
                </div>
           
              
            </div>
        </div>

        <%-- <div class="col-md-6 grid-margin stretch-card">
            <div class="card">
                <div class="card-body"></div>
            </div>
        </div>--%>

        <div class=" grid-margin stretch-card">
            <div class="card">
                <asp:UpdatePanel ID="upPnlPOSPDetails" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="card-body">
                            <h2 class="card-title">POSP Details</h2>
                            <div class="row g-3">

                                <div class="col-md-2">
                                    <label for="posp-marking" class="form-label">POSP Marking</label>
                                    <asp:DropDownList ID="pospMarking" CssClass="form-select" runat="server">
                                        <asp:ListItem Value="">POSP Marking</asp:ListItem>
                                        <asp:ListItem Value="Y">Yes</asp:ListItem>
                                        <asp:ListItem Value="N">No</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-3">
                                    <label for="posp-type" class="form-label">POSP Type</label>
                                    <asp:DropDownList ID="pospType" CssClass="form-select" runat="server" AutoPostBack="true" OnSelectedIndexChanged="pospType_SelectedIndexChanged">
                                        <asp:ListItem Value="">Select POSP Type</asp:ListItem>
                                        <asp:ListItem Text="Life (LI)" Value="Life" />
                                        <asp:ListItem Text="General (GI)" Value="General" />
                                        <asp:ListItem Text="Both" Value="Both" />
                                    </asp:DropDownList>
                                </div>

                                <div class="col-md-3">
                                    <label for="posp-no-li" class="form-label">POSP No LI</label>
                                    <asp:TextBox ID="pospNoLi" CssClass="form-control" runat="server" MaxLength="20"></asp:TextBox>
                                </div>


                                <div class="col-md-2">
                                    <label for="posp-certified-li-on" class="form-label">POSP Certified LI on</label>
                                    <div class="input-group date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                        <asp:TextBox ID="pospCertifiedLiOn" CssClass="form-control" runat="server" placeholder="DD-MM-YY" oninput="formatDOBInput(this)"></asp:TextBox>
                                        <div class="input-group-addon">
                                            <span class="glyphicon glyphicon-th"></span>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-md-2">
                                    <label for="posp-certified-li-on-valid-till" class="form-label">LI Valid Till </label>
                                    <div class="input-group date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                        <asp:TextBox ID="pospCertifiedLiOnValidTill" CssClass="form-control" runat="server" placeholder="DD-MM-YY" oninput="formatDOBInput(this)"></asp:TextBox>
                                        <div class="input-group-addon">
                                            <span class="glyphicon glyphicon-th"></span>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-md-2">
                                    <label class="form-check-label"></label>
                                    <div class="form-check">
                                        <label for="verified" class="form-check-label">
                                            Verified                                  
                                    <asp:CheckBox ID="verified" runat="server" />
                                        </label>
                                    </div>
                                </div>




                                <div class="col-md-3">
                                    <label for="posp-no-gi" class="form-label">POSP No GI</label>
                                    <asp:TextBox ID="pospNoGi" CssClass="form-control" runat="server" MaxLength="20"></asp:TextBox>
                                </div>

                                <div class="col-md-2">
                                    <label for="posp-certified-gi-on" class="form-label">POSP Certified GI on </label>
                                    <div class="input-group date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                        <asp:TextBox ID="pospCertifiedGiOn" CssClass="form-control" runat="server" placeholder="DD-MM-YY" oninput="formatDOBInput(this)"></asp:TextBox>
                                        <div class="input-group-addon">
                                            <span class="glyphicon glyphicon-th"></span>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-md-2">
                                    <label for="posp-certified-gi-on-valid-till" class="form-label">GI Valid Till </label>
                                    <div class="input-group date" data-provide="datepicker" data-date-format="dd/mm/yyyy">
                                        <asp:TextBox ID="pospCertifiedGiOnValidTill" CssClass="form-control" runat="server" placeholder="DD-MM-YY" oninput="formatDOBInput(this)"></asp:TextBox>
                                        <div class="input-group-addon">
                                            <span class="glyphicon glyphicon-th"></span>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-md-4 d-none">
                                    <label for="document" class="form-label">Document</label>
                                    <div class="input-group">
                                        <asp:DropDownList ID="document" CssClass="form-select" runat="server" Enabled="false">
                                            <asp:ListItem Value="">Select Document</asp:ListItem>
                                            <asp:ListItem Value="1">1</asp:ListItem>
                                            <asp:ListItem Value="2">2</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:Button ID="documentSearchButton" CssClass="btn btn-outline-primary" runat="server" Text="Search" Enabled="false" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="pospType" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
 
        <div class="col-md-12 grid-margin stretch-card">
            <div class="card">
                <div class="card-body">
                    <h2 class="card-title">Certification Details</h2>
                    <div class="row g-3">
                        <div class="col-md-3">
                            <label class="form-label"></label>
                            <div class="form-check">
                                <label class="form-check-label">
                                    Enrolled 
                                    <asp:CheckBox ID="certEnrolledCheck" runat="server" ClientIDMode="Static" />
                                </label>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <label class="form-label"></label>
                            <div class="form-check">
                                <label class="form-check-label">
                                    Passed
                                    <asp:CheckBox ID="certPassedCheck" runat="server" ClientIDMode="Static" />
                                </label>
                            </div>
                        </div>
                       
                        <div class="col-md-3">
                            <label for="ddlCertExam" class="form-label">Exams</label>
                            <asp:DropDownList ID="ddlCertExam" CssClass="form-select" runat="server" ClientIDMode="Static">
                                <asp:ListItem Value="">Select</asp:ListItem>
                            </asp:DropDownList>
                        </div>


                        <div class="col-md-3">
                            <label for="certRegNo" class="form-label">Reg No</label>
                            <asp:TextBox ID="certRegNo" CssClass="form-control" runat="server" ClientIDMode="Static"
                                                                                  onkeypress="return isMobileNumberKey(event)" MaxLength="20"
onpaste="validatePasteNumber(event, this);"></asp:TextBox>

                                
                                <script type = "text/javascript" >
    // Function to trim the input if it exceeds the max length
    function trimInputNumber(inputElement, maxLength) {
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
    function validatePasteNumber(event, inputElement) {
        // Get pasted data
        var pastedData = (event.clipboardData || window.clipboardData).getData('text');
low numbers in the pasted data
        if (/\D/.test(pastedData)) {
            event.preventDefault(); // Prevent paste if it's not a number
        }
    }

    function validatePasteNumberForCountry(event, inputElement) {
        // Get pasted data
        var pastedData = (event.clipboardData || window.clipboardData).getData('text');

        // Get selected country ID correctly
        var countryDropdown = document.getElementById('<%= ddlMailingCountryList.ClientID %>');
        var currentCountry = countryDropdown ? countryDropdown.value : null;

        // If country ID is "1", allow only numbers
        if (currentCountry === "1" && /\D/.test(pastedData)) {
            event.preventDefault(); // Prevent paste if it contains non-numeric characters
        }
    }

</script>
                        </div>
                    </div>
                </div>

                <script>
                    // JavaScript to enable/disable Exams dropdown based on Enrolled checkbox
                    document.addEventListener("DOMContentLoaded", function () {
                        var enrolledCheck = document.getElementById("certEnrolledCheck");
                        var ddlCertExamDropdown = document.getElementById("ddlCertExam");
                        var certPassedCheck = document.getElementById("certPassedCheck");
                        var certRegNo = document.getElementById("certRegNo");

                        // Function to toggle the dropdown based on checkbox status
                        function toggleExamDropdown() {
                            if (enrolledCheck.checked) {
                                ddlCertExamDropdown.disabled = false;
                                certPassedCheck.disabled = false;
                            } else {
                                ddlCertExamDropdown.selectedIndex = 0; // Reset dropdown
                                certRegNo.value = ""; // Clear certRegNo input
                                ddlCertExamDropdown.disabled = true;
                                certPassedCheck.disabled = true;
                                certRegNo.disabled = true;
                                certPassedCheck.checked = false;
                            }

                            // Handle certRegNo enable/disable based on certPassedCheck
                            if (certPassedCheck.checked) {
                                certRegNo.disabled = false;
                            } else {
                                certRegNo.disabled = true;
                                certRegNo.value = ""; // Clear input if disabled
                            }
                        }

                        // Initially set the correct state
                        toggleExamDropdown();

                        // Add event listeners
                        enrolledCheck.addEventListener("change", toggleExamDropdown);
                        certPassedCheck.addEventListener("change", toggleExamDropdown);
                    });

                </script>

            </div>
        </div>
       
        <div class="col-12">
            <div class="row">
                <div class="col-md-3">
                    <asp:Button ID="saveButton" CssClass="btn btn-primary w-100" runat="server" Text="Add" OnClick="btnInsert_Click" />
                   
                </div>

                <div class="col-md-3">
                    <asp:Button ID="upateButton" CssClass="btn btn-primary w-100" runat="server" Text="Update" OnClick="btnUpdate_Click" />

                </div>

                <div class="col-md-3">
                    <asp:Button ID="resetButton" CssClass="btn btn-primary w-100" runat="server" Text="Reset" OnClick="btnReset_Click" />

                </div>

                <div class="col-md-3">
                    <asp:Button ID="btnExitAssociatePage" CssClass="btn btn-secondary w-100" runat="server" Text="Exit" OnClick="btnExitAssociatePage_Click" />

                </div>
            </div>
            <div class="mt-2">
                <asp:Label ID="lblMessage" runat="server" CssClass="message-label" Text=""></asp:Label>
            </div>
        </div>
    </div>

    <script>
        // Unique key for storing the modal's state
        const associateModalStateKey = 'modalState_associateListModal';  // Unique storage key for this modal

        // Function to open the modal and save state
        function openAssociateListModal() {
            const modal = document.getElementById("associateListModal");
            if (modal) {
                modal.style.display = "block"; // Open the modal
                localStorage.setItem(associateModalStateKey, 'open'); // Store state as 'open'
            }
        }

        // Function to close the modal
        function closeAssociateListModal() {
            const modal = document.getElementById("associateListModal");
            if (modal) {
                modal.style.display = "none"; // Close the modal
                localStorage.setItem(associateModalStateKey, 'closed'); // Store state as 'closed'
            }
        }

        // Close modal automatically after data is filled
        function autoCloseAssociateListModal() {
            setTimeout(function () {
                closeAssociateListModal();
            }, 1); // Delay before closing to simulate data filling
        }

        // Check modal state on page load
        document.addEventListener('DOMContentLoaded', function () {
            const modalState = localStorage.getItem(associateModalStateKey); // Get modal state
            if (modalState === 'open') {
                openAssociateListModal(); // Open modal if state is 'open'
            }

            // Add event listener for the close button
            const closeButton = document.getElementById("btnCloseAssociateListModal"); // Use the correct ID for the close button
            if (closeButton) {
                closeButton.addEventListener('click', closeAssociateListModal); // Close modal on close button click
            }
        });
    </script>


    <div id="associateListModal" class="modal" role="dialog" style="z-index: 99;">
        <div class="modal-content">
            <h2 class="page-title">Associate List</h2>
            <div class="row g-3">

                <%-- ASSOCIATE SEARCH BRANCH --%>
                <div class="col-md-2">
                    <label for="branchsbl" class="form-label">Branch</label>
                    <asp:DropDownList ID="branchsbl" runat="server" CssClass="form-select">
                        <asp:ListItem Value="">Select</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <%-- ASSOCIATE SEARCH CITY --%>
                <div class="col-md-2">
                    <label for="citysbl" class="form-label">City</label>
                    <asp:DropDownList ID="citysbl" CssClass="form-select" runat="server">
                        <asp:ListItem Value="">Select</asp:ListItem>
                        <asp:ListItem Value="city1">City 1</asp:ListItem>
                        <asp:ListItem Value="city2">City 2</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <%-- ASSOCIATE SEARCH MOBILE --%>

                <div class="col-md-2">
                    <label for="mobilesbl" class="form-label">Mobile No.</label>
                    <asp:TextBox ID="mobilesbl"
                        CssClass="form-control phone-input"
                        runat="server"
                        MaxLength="10"
                        placeholder="Enter 10-digit Mobile Number"
                        pattern="[6-9]\d{9}"
                        title="Enter a valid 10-digit mobile number (starting with 6-9)"
                        oninput="sanitizeMobileInput(this)"
                        onpaste="sanitizeMobileInput(this)">

                    </asp:TextBox>

                    <script>
                        function sanitizeMobileInput(input) {
                            // Remove all non-numeric characters
                            input.value = input.value.replace(/\D/g, '');

                            // Show validation tooltip
                            let mobilePattern = /^[6-9]\d{9}$/; // Must start with 6-9 and be exactly 10 digits
                            if (input.value.length > 0 && !mobilePattern.test(input.value)) {
                                input.setAttribute("title", "Invalid Mobile Number (must be 10 digits, start with 6-9)");
                                input.classList.add("invalid-input"); // Add red border
                            } else {
                                input.setAttribute("title", "Enter a valid 10-digit mobile number (starting with 6-9)");
                                input.classList.remove("invalid-input"); // Remove red border if valid
                            }
                        }

                    </script>

                </div>

                <%-- ASSOCIATE SEARCH PHONE --%>
                <div class="col-md-2">
                    <label for="phonesbl" class="form-label">Phone</label>
                    <asp:TextBox ID="phonesbl" CssClass="form-control phone-input" runat="server" MaxLength="50"
                        placeholder="Enter Phone Number"></asp:TextBox>
                </div>

                <%-- ASSOCIATE SEARCH EXIST --%>
                <div class="col-md-2">
                    <label for="codesbl" class="form-label">Exist Code</label>
                    <asp:TextBox ID="codesbl" CssClass="form-control" runat="server"
                        placeholder="Enter Exist Code"
                        MaxLength="20"></asp:TextBox>
                </div>


                <%-- ASSOCIATE SEARCH PAN --%>
                <div class="col-md-2">
                    <label for="panNosbl" class="form-label">PAN No</label>
                    <asp:TextBox ID="panNosbl"
                        CssClass="form-control"
                        runat="server"
                        MaxLength="10"
                        placeholder="Enter PAN (ABCDE1234F)"
                        pattern="[A-Z]{5}[0-9]{4}[A-Z]{1}"
                        title="Enter PAN in format: ABCDE1234F"
                        oninput="validatePanInput2(this)">
                    </asp:TextBox>
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

                <%-- ASSOCIATE SEARCH ASSOCIATE CODE --%>
                <div class="col-md-2">
                    <label for="agentCodesbl" class="form-label">Associate Code</label>
                    <asp:TextBox ID="agentCodesbl" CssClass="form-control" runat="server"
                        placeholder="Enter Associate Code"
                        MaxLength="100"></asp:TextBox>
                </div>

                <%-- ASSOCIATE SEARCH ASSOCIATE NAME --%>
                <div class="col-md-4">
                    <label for="namesbl" class="form-label">Associate Name</label>
                    <asp:TextBox ID="namesbl" CssClass="form-control" runat="server"
                        placeholder="Enter Associate Name"
                        MaxLength="100"></asp:TextBox>
                </div>


                <div class="col-md-6">
                    <%-- BUTTON SECTION (RIGHT-ALIGNED) --%>
                    <div class="col-md-12 d-flex justify-content-end align-items-end mt-4">
                        <asp:Button ID="btnSearchsbl" CssClass="btn btn-primary me-2" Text="Search" runat="server" OnClick="btnAssListSearchsbl_Click" CausesValidation="false" />
                        <asp:Button ID="btnResetsbl" CssClass="btn btn-outline-primary me-2" Text="Reset" runat="server" OnClick="btnAssListReset_Click" CausesValidation="false" />
                        <asp:Button ID="btnCloseAssociateListModal" CssClass="btn btn-outline-primary" Text="Exit" runat="server" OnClientClick="closeAssociateListModal(); return false;" CausesValidation="false" />
                    </div>
                </div>
          
                </div>
            <br />
            <asp:Label ID="lblAssociateListMessage" runat="server" CssClass="message-label m-2" style="font-size: 16px; font-weight: bold; color: blue; margin: 0;"></asp:Label>
            <div class="table-responsive" style="overflow-y: auto; overflow-x: auto; height: 300px;">
                    <asp:GridView
    ID="agentListDetailsGridsbl"
    CssClass="table table-hover"
    runat="server"
    AllowPaging="True"
                        AutoGenerateColumns="false"
    PageSize="20"
    OnPageIndexChanging="agentListDetailsGridsbl_PageIndexChanging"
    OnRowCommand="gvAgentSearch_RowCommand"
    DataKeyNames="AGENT_CODE">


                        <Columns>
                           <%-- <asp:TemplateField HeaderText="S.No">
                                <ItemTemplate>
                                    <%# Container.DataItemIndex + 1 + (agentListDetailsGridsbl.PageIndex * agentListDetailsGridsbl.PageSize) %>
                                </ItemTemplate>
                            </asp:TemplateField>--%>
                        <asp:TemplateField HeaderText="Select Agent">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnAgentAssociateList"  runat="server" CommandName="SelectRow" CommandArgument='<%# Eval("AGENT_CODE") %>' Text="Select" CausesValidation="false" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Associate Code" ItemStyle-CssClass="column-width-90">
                            <ItemTemplate>
                                <asp:Label ID="lblSubBrokerCode" runat="server" Text='<%# Eval("AGENT_CODE") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Associate Name" ItemStyle-CssClass="column-width-90">
                            <ItemTemplate>
                                <asp:Label ID="lblSubBrokerName" runat="server" Text='<%# Eval("AGENT_NAME") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="First Address" ItemStyle-CssClass="column-width-100">
                            <ItemTemplate>
                                <asp:Label ID="lblFirstAddress" runat="server" Text='<%# Eval("ADDRESS1") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Second Address" ItemStyle-CssClass="column-width-100">
                            <ItemTemplate>
                                <asp:Label ID="lblSecondAddress" runat="server" Text='<%# Eval("ADDRESS2") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="City Name" ItemStyle-CssClass="column-width-100">
                            <ItemTemplate>
                                <asp:Label ID="lblCityId" runat="server" Text='<%# Eval("cm_city_name") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Phone" ItemStyle-CssClass="column-width-100">
                            <ItemTemplate>
                                <asp:Label ID="lblPhone" runat="server" Text='<%# Eval("Phone") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Mobile" ItemStyle-CssClass="column-width-100">
                            <ItemTemplate>
                                <asp:Label ID="lblMobile" runat="server" Text='<%# Eval("Mobile") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Email" ItemStyle-CssClass="column-width-100">
                            <ItemTemplate>
                                <asp:Label ID="lblEmail" runat="server" Text='<%# Eval("Email") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                            <asp:TemplateField HeaderText="PAN" ItemStyle-CssClass="column-width-100">
    <ItemTemplate>
        <asp:Label ID="lblPan" runat="server" Text='<%# Eval("PAN") %>' />
    </ItemTemplate>
</asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>

    <div id="superAnaFindModal" class="modal" style="z-index: 99">
        <asp:UpdatePanel runat="server" UpdateMode="Conditional" EnableViewState="true">

            <ContentTemplate>
                <div class="modal-content">
                    <h2 class="page-title">Super ANA Agent Search</h2>
                    <div class="row g-3 mt-2">
                        <div class="row g-3">

                            <%-- SUPER ANA SEARCH BRANCH --%>
                            <div class="col-md-2">
                                <label class="form-label" for="sourceid">Branch</label>
                                <asp:DropDownList ID="ddlSourceID" CssClass="form-select" onchange="showServerLoader();" runat="server" AutoPostBack="True" OnSelectedIndexChanged="RM_ddlSourceID_SelectedIndexChanged">
                                    <asp:ListItem Value="0">Select</asp:ListItem>
                                </asp:DropDownList>
                            </div>

                            <%-- SUPER ANA SEARCH RM AGENT --%>
                            <div class="col-md-2">
                                <label class="form-label" for="rmsbl">Agent</label>

                                <asp:DropDownList ID="ddlRM" CssClass="form-select" runat="server" Enabled="false">
                                    <asp:ListItem Value="0">Select</asp:ListItem>
                                </asp:DropDownList>
                            </div>

                            <%-- SUPER ANA SEARCH MOBILE --%>
                            <div class="col-md-2">
                                <label class="form-label" for="agentname">Mobile</label>
                                <asp:TextBox ID="m_sas_txtMobile" CssClass="form-control" runat="server" placeholder="Enter Agent Mobile"  oninput="validateMobileNumber(this, 10)"/>
                            </div>

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
</script>
                            <%-- SUPER ANA SEARCH phone --%>
                            <div class="col-md-2">
                                <label class="form-label" for="existcode">Phone</label>
                                <asp:TextBox ID="m_sas_txtPhone" CssClass="form-control" runat="server" placeholder="Enter Exist Phone" MaxLength="30"></asp:TextBox>
                            </div>

                            <%-- SUPER ANA SEARCH EXIST CODE --%>
                            <div class="col-md-2">
                                <label class="form-label" for="existcode">Exist Code</label>
                                <asp:TextBox ID="txtExistCode" CssClass="form-control" runat="server" placeholder="Enter Exist Code" oninput="validateMobileNumber(this, 20)"></asp:TextBox>
                            </div>

                            <%-- SUPER ANA SEARCH pan --%>
                            <div class="col-md-2">
                                <label class="form-label" for="existcode">PAN</label>
                                <asp:TextBox ID="m_sas_txtPan" CssClass="form-control" runat="server"
                                    MaxLength="10"
                                    placeholder="Enter PAN (ABCDE1234F)"
                                    pattern="[A-Z]{5}[0-9]{4}[A-Z]{1}"
                                    title="Enter PAN in format: ABCDE1234F"
                                    oninput="validatePanInput2(this)"></asp:TextBox>
                            </div>

                            <%-- SUPER ANA SEARCH AGENT code --%>
                            <div class="col-md-2">
                                <label class="form-label" for="agentname">Agent Code</label>
                                <asp:TextBox ID="m_sas_txtAgentCode" CssClass="form-control" runat="server" placeholder="Enter Agent Code" oninput="validateMobileNumber(this, 8)"/>
                            </div>

                            <%-- SUPER ANA SEARCH AGENT NAME --%>
                            <div class="col-md-3 me-3">
                                <label class="form-label" for="agentname">Agent Name</label>
                                <asp:TextBox ID="txtAgentName" CssClass="form-control me-4" runat="server" placeholder="Enter Agent Name" MaxLength="50" />
                            </div>



                            <div class="col-md-6 ">

                                <div class="col-md-12 d-flex justify-content-end align-items-end mt-4 gap-1 ms-5">
                                    <asp:Button
                                        ID="btnSearch"
                                        CssClass="btn btn-primary ms-2"
                                        Text="Search"
                                        runat="server"
                                        OnClientClick="showServerLoader();"
                                        OnClick="btnSearch_Click_SuperAna"
                                        CausesValidation="false"
                                        UseSubmitBehavior="false" />

                                    

                                    <asp:Button
                                        ID="btnSetAsMainAgent"
                                        CssClass="btn btn-outline-success "
                                        Text="Add in List"
                                        runat="server"
                                        OnClick="btnSetSuperAgent_Click"
                                        CausesValidation="false"
                                        Enabled="false" />

                                    <asp:Button
                                        ID="btnSuperANAReset"
                                        CssClass="btn btn-outline-primary"
                                        Text="Reset"
                                        runat="server"
                                        OnClick="ResetSuperANAModelFields"
                                        CausesValidation="false" />

                                    <asp:Button
                                        ID="btnCloseSuperAnaModal"
                                        CssClass="btn btn-outline-secondary"
                                        runat="server"
                                        Text="Close"
                                        OnClick="ExitSuperANAModelFields" />
                                </div>
                            </div>
                        </div>


                        <div class="row mt-3">
                            <asp:Label ID="lblSuperAnamsg" runat="server" CssClass="message-label m-2" Style="font-size: 16px; font-weight: bold; color: blue; margin: 0;"></asp:Label>

                            <div class="table-responsive " style="overflow-y: auto; overflow-x: auto; height: 280px;">
                                <asp:GridView
                                    ID="agentsGrid"
                                    CssClass="table table-hover"
                                    AutoGenerateColumns="false"
                                    AllowPaging="True"
                                    PageSize="20"
                                    OnPageIndexChanging="superANAAgentSearchListDetailsGridsbl_PageIndexChanging"
                                    DataKeyNames="AGENT_CODE"
                                    runat="server">
                                    <Columns>

                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:CheckBox ID="chkHeader" runat="server" AutoPostBack="True" OnCheckedChanged="chkHeader_CheckedChanged" />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkSelect" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>


                                        <asp:TemplateField HeaderText="Agent Code" ItemStyle-CssClass="column-width-100">
                                            <ItemTemplate>
                                                <asp:Label ID="lblAgentCodeSearched" runat="server" Text='<%# Eval("Agent_Code") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Agent Name" ItemStyle-CssClass="column-width-100">
                                            <ItemTemplate>
                                                <asp:Label ID="lblAgentNameSearched" runat="server" Text='<%# Eval("Agent_Name") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Exist Code" ItemStyle-CssClass="column-width-100">
                                            <ItemTemplate>
                                                <asp:Label ID="lblExistCodeSearched" runat="server" Text='<%# Eval("Exist_Code") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>


                                        <asp:TemplateField HeaderText="Address 1" ItemStyle-CssClass="column-width-100">
                                            <ItemTemplate>
                                                <asp:Label ID="lblAddress1Searched" runat="server" Text='<%# Eval("Address1") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Address 2" ItemStyle-CssClass="column-width-100">
                                            <ItemTemplate>
                                                <asp:Label ID="lblAddress2Searched" runat="server" Text='<%# Eval("Address2") %>' Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                    </Columns>
                                </asp:GridView>
                            </div>


                        </div>
                    </div>
                </div>

            </ContentTemplate>
            <Triggers>
                   <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
                   <asp:AsyncPostBackTrigger ControlID="ddlSourceID" EventName="SelectedIndexChanged" />

        <asp:AsyncPostBackTrigger ControlID="btnSetAsMainAgent" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="btnSuperANAReset" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="btnCloseSuperAnaModal" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
    </div>

    <div id="resAddModal" class="modal" style="z-index: 99;">
        <!-- Modal Dialog -->

        <!-- Modal Content -->
        <div class="modal-content" style="width: 480PX; margin: auto auto; margin-top: 80px;">
            <!-- Modal Header -->
            <div class="modal-header">
                <h5 class="modal-title">Residence Address Details</h5>
            </div>
            <!-- Modal Body -->
            <div class="modal-body">
                <asp:UpdatePanel ID="updPanelResCityState" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="row g-3 m-1">

                            <div class="col-md-12">
                                <label for="resAddAddress1" class="form-label">Address 1</label>
                                <asp:TextBox ID="resAddAddress1" CssClass="form-control" runat="server" MaxLength="200"></asp:TextBox>
                            </div>

                            <div class="col-md-12">
                                <label for="resAddAddress2" class="form-label">Address 2</label>
                                <asp:TextBox ID="resAddAddress2" CssClass="form-control" runat="server" MaxLength="200"></asp:TextBox>
                            </div>


                            <div class="col-md-6">
                                <label for="resAddCity" class="form-label">City</label>


                                <asp:DropDownList runat="server" ID="ddlresAddCity" AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlresAddCity_SelectedIndexChanged"
                                    CssClass="form-select">
                                    <asp:ListItem Value="0">Select City</asp:ListItem>
                                </asp:DropDownList>
                                <div class="input-group">
                                    <%--<asp:Button ID="Button1" CssClass="btn btn-outline-primary ms-2" runat="server" Text="New" OnClick="btnNewCityClick" CausesValidation="false" />--%>
                                </div>
                            </div>

                            <div class="col-md-6">
                                <label for="resAddState" class="form-label">State</label>
                                <asp:TextBox ID="resAddState" CssClass="form-control" runat="server" Enabled="false" MaxLength="50"></asp:TextBox>
                            </div>


                            <div class="col-md-12">
                                <label for="resAddPIN" class="form-label">PIN</label>

                                <div class="d-flex">

                                    <div class="col-md-6">

                                        <asp:TextBox ID="resAddPIN" CssClass="form-control"
                                            onkeypress="return isMobileNumberKey(event)" MaxLength="8"
                                            onpaste="validatePasteNumber(event, this);"
                                            runat="server"></asp:TextBox>
                                    </div>

                                    <div class="col-md-6 d-flex justify-content-end">
                                        <asp:Button ID="btnCloseModal" runat="server" Text="Close" CssClass="btn btn-secondary" OnClick="closeResAddModal" CausesValidation="false" />
                                    </div>

                                </div>

                            </div>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlresAddCity" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>


            </div>

       </div>
    </div>

    


<%-- RESIDENCE ADDRESS MODEL LOCAL STORAGE --%>
<script>
    // Unique key for storing the modal's state
    const resAddModalStateKey = 'modalState_resAddModal'; // Unique storage key for this modal

    // Function to open the modal and save state
    function openResAddModal() {
        const modal = document.getElementById("resAddModal");
        if (modal) {
            modal.style.display = "block"; // Open the modal
            localStorage.setItem(resAddModalStateKey, 'open'); // Store state as 'open'
        }
    }

    // Function to close the modal and save state
    function closeResAddModal() {
        const modal = document.getElementById("resAddModal");
        if (modal) {
            modal.style.display = "none"; // Close the modal
            localStorage.setItem(resAddModalStateKey, 'closed'); // Store state as 'closed'
        }
    }

    // Check modal state on page load
    document.addEventListener('DOMContentLoaded', function () {
        const modalState = localStorage.getItem(resAddModalStateKey); // Get modal state
        if (modalState === 'open') {
            openResAddModal(); // Open modal if state is 'open'
        }

        // Add event listener for the close button
        const closeButton = document.getElementById("btnCloseResAddModal");
        if (closeButton) {
            closeButton.addEventListener('click', closeResAddModal); // Close modal on close button click
        }
    });
                               </script>

      

    
    <script type="text/javascript">
        function isValidChar(evt) {
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            // Allow: backspace, tab, escape, enter, and special characters
            if (charCode === 8 || charCode === 9 || charCode === 27 || charCode === 13 ||
                (charCode >= 65 && charCode <= 90) || // A-Z
                (charCode >= 97 && charCode <= 122) || // a-z

                charCode === 46 || // . (dot)
                charCode === 44 || // , (comma)
                charCode === 45 || // - (dash)
                charCode === 95 || // _ (underscore)
                charCode === 32) { // space
                return true;
            }
            // Block non-alphanumeric characters
            return false;
        }
    </script>

    <script type="text/javascript">
        function isMobileNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : evt.keyCode;

            // Allow: backspace (8), tab (9), arrow keys (37-40), delete (46), and numbers (48-57)
            if (charCode === 8 || charCode === 9 || (charCode >= 37 && charCode <= 40)) {
                return true;
            }

            // Prevent non-numeric input and dot (charCode for dot is 46, removing it)
            if (charCode >= 48 && charCode <= 57) { // 0-9 only
                return true;
            }

            // If charCode is anything else, return false to block input
            return false;
        }

        function isTDSCOMKey(evt) {
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            var inputField = evt.target;

            // Allow: backspace, tab, delete, arrow keys (left, up, right, down), and numbers (0-9)
            if (charCode === 8 || charCode === 9 || charCode === 46 ||
                (charCode >= 37 && charCode <= 40)) {
                return true;
            }

            // Only allow numbers (0-9)
            if (charCode >= 48 && charCode <= 57) {
                // Get the current value of the input field, including the new character being input
                var currentValue = inputField.value + String.fromCharCode(charCode);

                // Parse to an integer and check if the value is less than 100
                if (parseInt(currentValue) < 101) {
                    return true;
                } else {
                    return false; // Block input if the result is 100 or greater
                }
            }

            // Prevent non-numeric input
            return false;
        }


    </script>

 


    <%-- SUPER ANA MODEL LOCAL STORAGE  --%>
    <script>
        // Unique key for storing the modal's state
        const superAnaModalStateKey = 'modalState_superAnaFindModal';  // Unique storage key for this modal

        // Function to open the modal and save state
        function openSuperAnaFindModal() {
            const modal = document.getElementById("superAnaFindModal");
            if (modal) {
                modal.style.display = "block"; // Open the modal
                localStorage.setItem(superAnaModalStateKey, 'open'); // Store state as 'open'
            }
        }

        // Function to close the modal and save state
        function closeSuperAnaFindModal() {
            const modal = document.getElementById("superAnaFindModal");
            if (modal) {
                modal.style.display = "none"; // Close the modal
                localStorage.setItem(superAnaModalStateKey, 'closed'); // Store state as 'closed'
            }
        }

        // Check modal state on page load
        document.addEventListener('DOMContentLoaded', function () {
            const modalState = localStorage.getItem(superAnaModalStateKey); // Get modal state
            if (modalState === 'open') {
                openSuperAnaFindModal(); // Open modal if state is 'open'
            }

            // Add event listener for the close button
            const closeButton = document.getElementById("closeSuperAnaModal");
            if (closeButton) {
                closeButton.addEventListener('click', closeSuperAnaFindModal); // Close modal on close button click
            }
        });
    </script>

    <script type="text/javascript">
        function toggleAgentNameReadOnly() {
            var ddlRM = document.getElementById('<%= ddlRM.ClientID %>');
            var txtAgentName = document.getElementById('<%= txtAgentName.ClientID %>');

            if (ddlRM.value) {
                txtAgentName.readOnly = true; // Set read-only if RM Code is selected
            } else {
                txtAgentName.readOnly = false; // Make editable if no RM Code is selected
            }
        }
    </script>

</asp:Content>
