<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvestorAddressUpdate.aspx.cs" Inherits="WM.Tree.InvestorAddressUpdate" %>


<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Investor Address Updation</title>
     <link href="../favicon.ico" rel="shortcut icon" type="image/x-icon" />
    <style>
        .form-table {
            background-color: #f0e0cc;
            border: 1px solid #ccc;
            padding: 20px;
            width: 100%;
            max-width: 480px;
            margin: auto;
            border-collapse: collapse;
        }

            .form-table td {
                padding: 5px;
            }

            .form-table label {
                font-weight: bold;
            }

        .button-container {
            text-align: center;
            margin-top: 20px;
        }
    </style>

 <%--   <script type="text/javascript">
        window.onload = function () {
            window.open(
                'InvestorAddressUpdate.aspx',
                'InvestorPopup',
                'width=700,height=350,resizable=yes,scrollbars=yes'
            );
        };
    </script>--%>
 
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>

                <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
                <style>
                    .investor-form {
                        background-color: #f0e0cc;
                        border: 1px solid #ccc;
                        border-radius: 5px;
                        max-width: 800px;
                        margin: 20px auto;
                        padding: 25px;
                    }

                    .form-header {
                        text-align: center;
                        margin-bottom: 25px;
                        font-weight: bold;
                        font-size: 1.4rem;
                    }

                    .btn-custom {
                        min-width: 100px;
                        margin: 0 10px;
                    }
                </style>

                <div class="investor-form">
                    <div class="form-header">Investor Detail</div>
                    <div class="row mb-2">
                        <div class="col-md-5 mt-2">
                            <label for="txtAddress1" class="form-label">Address 1</label>
                            <asp:TextBox ID="txtAddress1" Placeholder="Enter Address 1" runat="server" CssClass="form-control" MaxLength="250" />
                        </div>

                        <div class="col-md-5 mt-2">
                            <label for="txtAddress2" class="form-label">Address 2</label>
                            <asp:TextBox ID="txtAddress2" Placeholder="Enter Address 2" runat="server" CssClass="form-control" MaxLength="250" />
                        </div>
                        <div class="col-md-2 mt-2">
                            <label for="txtPIN" class="form-label">PIN</label>
                            <asp:TextBox ID="txtPIN" Placeholder="Enter PIN" runat="server" CssClass="form-control" MaxLength="6" />
                        </div>
                        <div class="col-md-6 mt-2">
                            <label for="txtEmail" class="form-label">Email</label>
                            <asp:TextBox ID="txtEmail" Placeholder="Enter Email ID" runat="server" CssClass="form-control" MaxLength="150" />
                        </div>

                        <div class="col-md-3 mt-2">
                            <label for="txtPAN" class="form-label">PAN</label>
                            <asp:TextBox ID="txtPAN" Placeholder="Enter PAN " runat="server" CssClass="form-control" MaxLength="10" />
                        </div>

                        <div class="col-md-3 mt-2">
                            <label for="txtMobile" class="form-label">Mobile</label>
                            <asp:TextBox ID="txtMobile" Placeholder="Enter Mobile No." runat="server" CssClass="form-control" TextMode="Phone" MaxLength="15" />
                        </div>

                        <div class="col-md-3 mt-2">
                            <label for="ddlCity" class="form-label">City</label>
                            <asp:DropDownList ID="ddlCity" runat="server" CssClass="form-select "
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlCity_SelectedIndexChanged">
                                <asp:ListItem Text="-- Select City --" Value="" />
                            </asp:DropDownList>
                        </div>


                        <div class="col-md-3 mt-2">
                            <label for="ddlState" class="form-label">State</label>
                            <asp:DropDownList ID="ddlState" runat="server" CssClass="form-select" AutoPostBack="true" 
    OnSelectedIndexChanged="ddlState_SelectedIndexChanged">
                                <asp:ListItem Text="-- Select State --" Value="" />
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-3 mt-2">
                            <label for="txtDOB" class="form-label">DOB</label>
                            <asp:TextBox ID="txtDOB" Placeholder="dd/mm/yyy" runat="server" CssClass="form-control" MaxLength="10" />
                            <link rel="stylesheet" href="https://code.jquery.com/ui/1.13.2/themes/base/jquery-ui.css">
                            <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
                            <script src="https://code.jquery.com/ui/1.13.2/jquery-ui.min.js"></script>

                            <script>
                                $(function () {
                                    $("#<%= txtDOB.ClientID %>").datepicker({
                                        dateFormat: "dd/mm/yy",
                                        changeMonth: true,
                                        changeYear: true,
                                        yearRange: "1900:2050"
                                    });
                                });
                            </script>
                        </div>
                        <div class="col-md-3 mt-2">
                            <label for="txtAadhar" class="form-label">Aadhar</label>
                            <asp:TextBox ID="txtAadhar" Placeholder="Enter 12 Digits" runat="server" CssClass="form-control" MaxLength="12" />

                        </div>

                        <asp:HiddenField ID="hdnInvestorCode" runat="server" />
                        <asp:HiddenField ID="hdnInvestorName" runat="server" />

                    </div>

                    <div class="row mt-2">
                        <div class="col-md-12 text-center">
                            <asp:Button ID="btnUpdate" runat="server" Text="Update"
                                OnClick="btnUpdate_Click" CssClass="btn btn-primary btn-custom" />
                            <%-- OnClick="btnExit_Click"  --%>
                            <%-- OnClientClick="window.close(); return false;" --%>
                            <asp:Button ID="btnExit" runat="server" Text="Exit"
                                OnClick="btnExit_Click"
                                
                                CssClass="btn btn-secondary btn-custom" />

                        </div>
                    </div>
                </div>
                <script type="text/javascript">
                    const validationFields = [
                        { id: "<%= txtAadhar.ClientID %>", type: "aadhar" },
        { id: "<%= txtEmail.ClientID %>", type: "email" },
        { id: "<%= txtMobile.ClientID %>", type: "mobile" },
        { id: "<%= txtPAN.ClientID %>", type: "pan" },
        { id: "<%= txtPIN.ClientID %>", type: "pin" }
                    ];

                    function sanitizeAndLimit(input, maxLength) {
                        let value = input.value.replace(/\D/g, '').substring(0, maxLength);
                        input.value = value;
                        return value;
                    }

                    function validateField(input, type) {
                        if (!input) return false;

                        let value = input.value.trim();
                        let valid = true;

                        switch (type) {
                            case "aadhar":
                                if (value === "") {
                                    input.setCustomValidity("");
                                    break;
                                }
                                value = sanitizeAndLimit(input, 12);
                                valid = value.length === 12;
                                input.setCustomValidity(valid ? "" : "Aadhar must be exactly 12 digits.");
                                break;

                            case "mobile":
                                if (value === "") {
                                    input.setCustomValidity("");
                                    break;
                                }
                                value = sanitizeAndLimit(input, 10);
                                valid = /^[6-9]\d{9}$/.test(value);
                                input.setCustomValidity(valid ? "" : "Enter a valid 10-digit mobile number starting with 6-9.");
                                break;

                            case "email":
                                if (value === "") {
                                    input.setCustomValidity("");
                                    break;
                                }
                                input.value = value.toLowerCase();
                                const allowedSpecials = ["none", "not", "not given"];
                                const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                                valid = emailRegex.test(value) || allowedSpecials.includes(value.toLowerCase());
                                input.setCustomValidity(valid ? "" : "Enter a valid email address or type 'None', 'Not', or 'Not Given'.");
                                break;

                            case "pan":
                                if (value === "") {
                                    input.setCustomValidity("");
                                    break;
                                }
                                const panRegex = /^[A-Z]{5}[0-9]{4}[A-Z]$/i;
                                valid = panRegex.test(value);
                                input.setCustomValidity(valid ? "" : "Invalid PAN format (e.g. ABCDE1234F).");
                                break;

                            case "pin":
                                if (value === "") {
                                    input.setCustomValidity("");
                                    break;
                                }
                                value = sanitizeAndLimit(input, 6);
                                valid = value.length === 6;
                                input.setCustomValidity(valid ? "" : "PIN must be exactly 6 digits.");
                                break;

                            default:
                                input.setCustomValidity("");
                                break;
                        }

                        return valid;
                    }

                    function validateAllFields() {
                        let allValid = true;
                        for (const field of validationFields) {
                            const input = document.getElementById(field.id);
                            if (input && !validateField(input, field.type)) {
                                if (allValid) input.focus();
                                allValid = false;
                            }
                        }
                        return allValid;
                    }

                    function setupValidationEvents() {
                        for (const field of validationFields) {
                            const input = document.getElementById(field.id);
                            if (!input) continue;

                            input.removeEventListener("input", () => validateField(input, field.type));
                            input.addEventListener("input", () => validateField(input, field.type));

                            input.removeEventListener("blur", () => validateField(input, field.type));
                            input.addEventListener("blur", () => validateField(input, field.type));

                            input.removeEventListener("paste", () => setTimeout(() => validateField(input, field.type), 0));
                            input.addEventListener("paste", function () {
                                setTimeout(() => validateField(input, field.type), 0);
                            });
                        }

                        validateAllFields();
                    }

                    document.addEventListener("DOMContentLoaded", setupValidationEvents);

                    // Re-bind after partial postbacks
                    if (typeof Sys !== 'undefined' && Sys.WebForms) {
                        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(setupValidationEvents);
                    }
                </script>


                <!-- Bootstrap 5 JS Bundle with Popper -->
                <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>


            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ddlCity" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="ddlState" EventName="SelectedIndexChanged" />
                <%--<asp:AsyncPostBackTrigger ControlID="btnExit" EventName="Click" />--%>
            </Triggers>
        </asp:UpdatePanel>
    </form>
            <script type="text/javascript">
                const validationFields = [
                    { id: "<%= txtAadhar.ClientID %>", type: "aadhar" },
                { id: "<%= txtEmail.ClientID %>", type: "email" },
                { id: "<%= txtMobile.ClientID %>", type: "mobile" },
{ id: "<%= txtPAN.ClientID %>", type: "pan" },
{ id: "<%= txtPIN.ClientID %>", type: "pin" }
                ];

                function sanitizeAndLimit(input, maxLength) {
                    let value = input.value.replace(/\D/g, '').substring(0, maxLength);
                    input.value = value;
                    return value;
                }

                function validateField(input, type) {
                    if (!input) return false;

                    let value = input.value.trim();
                    let valid = true;

                    switch (type) {
                        case "aadhar":
                            if (value === "") {
                                input.setCustomValidity("");
                                break;
                            }
                            value = sanitizeAndLimit(input, 12);
                            valid = value.length === 12;
                            input.setCustomValidity(valid ? "" : "Aadhar must be exactly 12 digits.");
                            break;

                        case "mobile":
                            if (value === "") {
                                input.setCustomValidity("");
                                break;
                            }
                            value = sanitizeAndLimit(input, 10);
                            valid = /^[6-9]\d{9}$/.test(value);
                            input.setCustomValidity(valid ? "" : "Enter a valid 10-digit mobile number starting with 6-9.");
                            break;

                        case "email":
                            if (value === "") {
                                input.setCustomValidity("");
                                break;
                            }
                            input.value = value.toLowerCase();
                            const allowedSpecials = ["none", "not", "not given"];
                            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                            valid = emailRegex.test(value) || allowedSpecials.includes(value.toLowerCase());
                            input.setCustomValidity(valid ? "" : "Enter a valid email address or type 'None', 'Not', or 'Not Given'.");
                            break;

                        case "pan":
                            if (value === "") {
                                input.setCustomValidity("");
                                break;
                            }
                            const panRegex = /^[A-Z]{5}[0-9]{4}[A-Z]$/i;
                            valid = panRegex.test(value);
                            input.setCustomValidity(valid ? "" : "Invalid PAN format (e.g. ABCDE1234F).");
                            break;

                        case "pin":
                            if (value === "") {
                                input.setCustomValidity("");
                                break;
                            }
                            value = sanitizeAndLimit(input, 6);
                            valid = value.length === 6;
                            input.setCustomValidity(valid ? "" : "PIN must be exactly 6 digits.");
                            break;

                        default:
                            input.setCustomValidity("");
                            break;
                    }

                    return valid;
                }

                function validateAllFields() {
                    let allValid = true;
                    for (const field of validationFields) {
                        const input = document.getElementById(field.id);
                        if (input && !validateField(input, field.type)) {
                            if (allValid) input.focus();
                            allValid = false;
                        }
                    }
                    return allValid;
                }

                function setupValidationEvents() {
                    for (const field of validationFields) {
                        const input = document.getElementById(field.id);
                        if (!input) continue;

                        input.removeEventListener("input", () => validateField(input, field.type));
                        input.addEventListener("input", () => validateField(input, field.type));

                        input.removeEventListener("blur", () => validateField(input, field.type));
                        input.addEventListener("blur", () => validateField(input, field.type));

                        input.removeEventListener("paste", () => setTimeout(() => validateField(input, field.type), 0));
                        input.addEventListener("paste", function () {
                            setTimeout(() => validateField(input, field.type), 0);
                        });
                    }

                    validateAllFields();
                }

                document.addEventListener("DOMContentLoaded", setupValidationEvents);

                // Re-bind after partial postbacks
                if (typeof Sys !== 'undefined' && Sys.WebForms) {
                    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(setupValidationEvents);
                }
            </script>

 <script type="text/javascript">
     var lastFocusedId = null;

     // Track the last focused element
     document.addEventListener('focusin', function (e) {
         if (e.target.id) {
             lastFocusedId = e.target.id;
         }
     });

     // Restore focus after postback
     if (typeof Sys !== 'undefined' && Sys.WebForms) {
         var prm = Sys.WebForms.PageRequestManager.getInstance();

         prm.add_endRequest(function () {
             if (lastFocusedId) {
                 var el = document.getElementById(lastFocusedId);
                 if (el) {
                     el.focus();
                 }
             }
         });
     }
 </script>



</body>
</html>
