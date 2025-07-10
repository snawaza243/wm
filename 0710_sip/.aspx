<%@ Page Title="SIP Master Reconciliation" Language="C#" MasterPageFile="~/vmSite.Master" AutoEventWireup="true"
    CodeBehind="sip_master_reconciliation.aspx.cs" Inherits="WM.Masters.sip_master_reconciliation"
    MaintainScrollPositionOnPostback="false" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <link rel="shortcut icon" href="../assets/images/favicon.png" />
        <link rel="stylesheet" href="../assets/vendors/mdi/css/materialdesignicons.min.css">
        <link rel="stylesheet" href="../assets/vendors/ti-icons/css/themify-icons.css">
        <link rel="stylesheet" href="../assets/vendors/css/vendor.bundle.base.css">
        <link rel="stylesheet" href="../assets/vendors/font-awesome/css/font-awesome.min.css">
        <link rel="stylesheet" href="../assets/vendors/bootstrap-datepicker/bootstrap-datepicker.min.css">
        <link rel="stylesheet" href="../assets/css/style.css">

        <div class="page-header">
            <h3 class="page-title">SIP Master Reconciliation</h3>
        </div>

        
                        <div id="updateProgress" class="loading-overlay">
                            <div class="spinner-container">
                                <div class="spinner-border text-dark" role="status">
                                    <span class="rupee-sign"></span>
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
                                background: rgba(255, 223, 128, 0.2);
                                /* Soft warm yellow */
                                display: flex;
                                justify-content: center;
                                align-items: center;
                                z-index: 1050;
                                display: none;
                                /* Initially hidden */
                            }

                            /* Centered Spinner Container */
                            .spinner-container {
                                position: relative;
                                width: 40px;
                                height: 40px;
                            }

                            /* Spinner with a Larger Size */
                            .spinner-border {
                                width: 50px;
                                height: 50px;
                                border-width: 6px;
                                color: #FFC107;
                                /*animation: spin 1s linear infinite;*/
                            }
                        </style>

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



        <style>
            th {
                position: sticky;
                top: 0;
                background-color: black !important;
                color: white !important;
                z-index: 1;
                border: 2px solid black;
            }

            .highlight-row {
                background-color: #cce5ff !important;
                /* Light blue */
                cursor: pointer;
            }
        
                            .hide-column {
                                display: none;
                            }

                            .draggable-container {
                                max-height: 900px;
                                /* Set a max height */
                                overflow-y: auto;
                                /* Enable vertical scrolling */
                                resize: vertical;
                                /* Allow users to resize vertically */
                                border: 1px solid #ddd;
                                /* Optional: Border for better visibility */
                                padding: 10px;
                                background: #fff;
                                /* Optional: Background color */
                            }

                            .selected-row td {
                                background-color: #d0ebff !important;
                                /* light blue */
                            }

                            .selected-row-rta td {
                                background-color: #d0ebff !important;
                                /* light blue */
                            }
                        </style>

        <!-- formate input field by js -->
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

    // Function to synchronize scrolling between the two grids
        <script type="text/javascript">
            function syncScroll() {
                var grid1 = document.getElementById('gridContainer1');
                var grid2 = document.getElementById('gridContainer2');

                // Check if both grids exist
                if (!grid1 || !grid2) return;

                // Synchronize horizontal scrolling (scrollLeft)
                grid1.addEventListener('scroll', function () {
                    grid2.scrollLeft = grid1.scrollLeft;
                });

                grid2.addEventListener('scroll', function () {
                    grid1.scrollLeft = grid2.scrollLeft;
                });

                // Synchronize vertical scrolling (scrollTop)
                //grid1.addEventListener('scroll', function () {
                //    grid2.scrollTop = grid1.scrollTop;
                //});

                //grid2.addEventListener('scroll', function () {
                //    grid1.scrollTop = grid2.scrollTop;
                //});
            }

            // Ensure the scroll synchronization is applied on initial page load and after postback
            document.addEventListener('DOMContentLoaded', function () {
                syncScroll();  // Sync scroll on initial page load

                // Rebind the scroll synchronization after partial postbacks
                if (typeof Sys !== "undefined" && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                        syncScroll();  // Reapply scroll synchronization after postback
                    });
                }
            });
        </script>

        <script type="text/javascript">
            function onRowDoubleClick_ServerPush(row) {
                var checkbox = row.querySelector('input[type="checkbox"]');
                if (checkbox) {
                    checkbox.checked = !checkbox.checked;
                    __doPostBack(checkbox.name, '');
                }
            }

            function onGridRowClick(row) {
                const index = row.rowIndex;
                let table = row.closest("table");
                let rows = table.querySelectorAll("tr");

                const header = table.querySelector("thead");
                const dataRowIndex = header ? index - 1 : index;

                // Reset background for all rows
                rows.forEach(r => {
                    r.style.backgroundColor = "";
                    r.classList.remove("selected-row"); // Optional: remove any selection class
                });

                // Highlight clicked row
                row.style.backgroundColor = "#d9edf7"; // Light blue or use any color
                row.classList.add("selected-row");     // Optional: apply a CSS class


                rows.forEach(r => {
                    if (r !== row) {
                        let cb = r.querySelector("input[type='checkbox']");
                        if (cb) cb.checked = false;

                    }
                });

                // Check checkbox in clicked row and set background
                let checkbox = row.querySelector("input[type='checkbox']");
                if (checkbox) checkbox.checked = true;
                //row.style.backgroundColor = "#d1ecf1";


                // Collect data from row cells or hidden fields
                // Assuming your data is in cells after the checkbox column
                let cells = row.querySelectorAll("td");

                // Store each cell's text in variables (customize according to your columns)

                let tr_code = cells[1]?.innerText.trim() || "";
                let tr_date = cells[2]?.innerText.trim() || "";
                let inv_name = cells[3]?.innerText.trim() || "";
                let mut_code = cells[4]?.innerText.trim() || "";
                let address = cells[5]?.innerText.trim() || "";
                let city_name = cells[6]?.innerText.trim() || "";
                let mut_name = cells[7]?.innerText.trim() || "";
                let sch_name = cells[8]?.innerText.trim() || "";
                let amount = cells[9]?.innerText.trim() || "";
                let folio = cells[10]?.innerText.trim() || "";
                let cheque_no = cells[11]?.innerText.trim() || "";
                let tran_type = cells[12]?.innerText.trim() || "";
                let app_no = cells[13]?.innerText.trim() || "";
                let pann_no = cells[14]?.innerText.trim() || "";
                let broker_id = cells[15]?.innerText.trim() || "";
                let rm_code = cells[16]?.innerText.trim() || "";
                let rm_name = cells[17]?.innerText.trim() || "";
                let branch_name = cells[18]?.innerText.trim() || "";
                let sip_amount = cells[19]?.innerText.trim() || "";
                let busi_branch_code = cells[20]?.innerText.trim() || "";
                let installments_no = cells[21]?.innerText.trim() || "";
                let reco_unreco_status = cells[22]?.innerText.trim() || "";
                let falg = cells[23]?.innerText.trim() || "";
                let registrar = cells[24]?.innerText.trim() || "";
                let cob_flag = cells[25]?.innerText.trim() || "";
                let cob_remark = cells[26]?.innerText.trim() || "";



                // Store all in an object or array first
                let rowData = {
                    "tr_code value": tr_code,
                    "tr_date value": tr_date,
                    "inv_name value": inv_name,
                    "mut_code value": mut_code,
                    "address value": address,
                    "city_name value": city_name,
                    "mut_name value": mut_name,
                    "sch_name value": sch_name,
                    "amount value": amount,
                    "folio value": folio,
                    "cheque_no value": cheque_no,
                    "tran_type value": tran_type,
                    "app_no value": app_no,
                    "pann_no value": pann_no,
                    "broker_id value": broker_id,
                    "rm_code value": rm_code,
                    "rm_name value": rm_name,
                    "branch_name value": branch_name,
                    "sip_amount value": sip_amount,
                    "busi_branch_code value": busi_branch_code,
                    "installments_no value": installments_no,
                    "reco_unreco_status value": reco_unreco_status,
                    "falg value": falg,
                    "registrar value": registrar,
                    "cob_flag value": cob_flag,
                    "cob_remark value": cob_remark
                };

                // Build alert message string from the object
                let alertMsg = "Selected Row Details:\n\n";
                for (let key in rowData) {
                    alertMsg += key + ": " + rowData[key] + "\n";
                }

                document.getElementById("<%= hfSelectedRow.ClientID %>").value = index;
                alert("CURRENT ROW INDEX: " + index)
                // Now pass it to your function
                //setSelectedRowIndex(dataRowIndex);

                trSearch_setCheckboxBasedOnRegistrarAndCobfalg(registrar, cob_flag);
                trSearch_setFromAndToDates(tr_date);
                trSearch_setChequeFolioAppBroker(cheque_no, folio, app_no, pann_no, broker_id);
                trSearch_setFormValues(tran_type, mut_code, amount, inv_name, cheque_no, pann_no, cob_remark);
                // Show the alert with all data
                alert(alertMsg);
            }

            function trSearch_setCheckboxBasedOnRegistrarAndCobfalg(registrar, cobfl) {
                // Uncheck all radios first
                document.getElementById('<%= c.ClientID %>').checked = false;
                document.getElementById('<%= k.ClientID %>').checked = false;
                document.getElementById('<%= cCob.ClientID %>').checked = false;
                document.getElementById('<%= kCob.ClientID %>').checked = false;

                // Check the correct radio based on parameters
                if (registrar === "C" && cobfl === "0") {
                    document.getElementById('<%= c.ClientID %>').checked = true;
                } else if (registrar === "K" && cobfl === "0") {
                    document.getElementById('<%= k.ClientID %>').checked = true;
                } else if (registrar === "C" && cobfl === "1") {
                    document.getElementById('<%= cCob.ClientID %>').checked = true;
                } else if (registrar === "K" && cobfl === "1") {
                    document.getElementById('<%= kCob.ClientID %>').checked = true;
                }

                alert("Check func hit by regi adn cobflag");
            }

            function trSearch_setFromAndToDates(tranDateStr) {
                const fromInput = document.getElementById('<%= dateFromRta.ClientID %>');
                const toInput = document.getElementById('<%= dateToRta.ClientID %>');

                const parts = tranDateStr.split('/');
                if (parts.length !== 3) {
                    fromInput.value = '';
                    toInput.value = '';
                    return;
                }

                const day = parseInt(parts[0], 10);
                const month = parseInt(parts[1], 10) - 1; // JavaScript months are 0-based
                const year = parseInt(parts[2], 10);

                const tranDate = new Date(year, month, day);
                if (isNaN(tranDate.getTime())) {
                    fromInput.value = '';
                    toInput.value = '';
                    return;
                }

                // Calculate fromDate (1 month before)
                const fromMonth = tranDate.getMonth() - 1;
                const fromYear = fromMonth < 0 ? tranDate.getFullYear() - 1 : tranDate.getFullYear();
                const adjustedFromMonth = (fromMonth + 12) % 12;
                const maxFromDay = new Date(fromYear, adjustedFromMonth + 1, 0).getDate();
                const safeFromDay = Math.min(day, maxFromDay);
                const fromDate = new Date(fromYear, adjustedFromMonth, safeFromDay);

                // Calculate toDate (1 month after)
                const toMonth = tranDate.getMonth() + 1;
                const toYear = toMonth > 11 ? tranDate.getFullYear() + 1 : tranDate.getFullYear();
                const adjustedToMonth = toMonth % 12;
                const maxToDay = new Date(toYear, adjustedToMonth + 1, 0).getDate();
                const safeToDay = Math.min(day, maxToDay);
                const toDate = new Date(toYear, adjustedToMonth, safeToDay);

                const formatDate = (d) => {
                    const dd = String(d.getDate()).padStart(2, '0');
                    const mm = String(d.getMonth() + 1).padStart(2, '0'); // Month is 0-based
                    const yyyy = d.getFullYear();
                    return `${dd}/${mm}/${yyyy}`;
                };

                fromInput.value = formatDate(fromDate);
                toInput.value = formatDate(toDate);
            }

            function trSearch_setFormValues(tranType, amcCode, amount, investorName, chqno, PAN, remarkValue) {
                // Set Reconciliation Type
                const rblReconciliationTypeReconciled = document.querySelector('input[id$="rblReconciliationType_0"]');
                const rblReconciliationTypeUnreconciled = document.querySelector('input[id$="rblReconciliationType_1"]');

                if (tranType.toLowerCase() === "reconciled") {
                    if (rblReconciliationTypeReconciled) rblReconciliationTypeReconciled.checked = true;
                } else {
                    if (rblReconciliationTypeUnreconciled) rblReconciliationTypeUnreconciled.checked = true;
                }

                // Set AMC dropdown
                const amcSelect = document.getElementById('<%= amcSelectrta.ClientID %>');
                if (amcSelect) amcSelect.value = amcCode;

                // Set Amount
                const amountInput = document.getElementById('<%= txtAmount.ClientID %>');
                if (amountInput) amountInput.value = amount;

                // Set Investor Name
                const investorInput = document.getElementById('<%= txtInvestorName.ClientID %>');
                if (investorInput) investorInput.value = investorName;

                // Set Cheque No dropdown
                const chequeSelect = document.getElementById('<%= chequeNoSelect.ClientID %>');
                if (chequeSelect) chequeSelect.value = "004"; // hardcoded as in your server code

                // Set Cheque No text
                const chequeText = document.getElementById('<%= chequeNo.ClientID %>');
                if (chequeText) chequeText.value = chqno;

                // Set Remark
                const remarkInput = document.getElementById('<%= remark.ClientID %>');
                if (remarkInput) remarkInput.value = remarkValue;

                // Check Unreconciled Radio
                const radioUnreconciled = document.getElementById('<%= RadioButton2.ClientID %>');
                if (radioUnreconciled) radioUnreconciled.checked = true;
            }

            function trSearch_setSelectedRowIndex(rowIndex) {
                if (rowIndex >= 0) {
                    const hfSelectedRow = document.getElementById('<%= hfSelectedRow.ClientID %>');
                    if (hfSelectedRow) {
                        hfSelectedRow.value = rowIndex.toString();
                    }
                }
            }

            function trSearch_setChequeFolioAppBroker(cheque, folio, app, pan, broker) {
                // Clear existing values
                document.getElementById("<%= hdfSearchedCheque.ClientID %>").value = "";
                document.getElementById("<%= hdfSearchedFilio.ClientID %>").value = "";
                document.getElementById("<%= hdfSearchedApp.ClientID %>").value = "";
                document.getElementById("<%= hdfSearchedPan.ClientID %>").value = "";
                document.getElementById("<%= hdfSearchedBroker.ClientID %>").value = "";

                // Set new values
                document.getElementById("<%= hdfSearchedCheque.ClientID %>").value = cheque;
                document.getElementById("<%= hdfSearchedFilio.ClientID %>").value = folio;
                document.getElementById("<%= hdfSearchedApp.ClientID %>").value = app;
                document.getElementById("<%= hdfSearchedPan.ClientID %>").value = pan;
                document.getElementById("<%= hdfSearchedBroker.ClientID %>").value = broker;
            }

            function ddlChequeChanged_TR2() {
                const selectedField = document.getElementById("<%= chequeNoSelect.ClientID %>").value;
                const chequeInput = document.getElementById("<%= chequeNo.ClientID %>");

                let value = "";

                switch (selectedField) {
                    case "001":
                        value = document.getElementById("<%= hdfSearchedCheque.ClientID %>").value;
                        break;
                    case "002":
                        value = document.getElementById("<%= hdfSearchedFilio.ClientID %>").value;
                        break;
                    case "003":
                        value = document.getElementById("<%= hdfSearchedApp.ClientID %>").value;
                        break;
                    case "004":
                        value = document.getElementById("<%= hdfSearchedPan.ClientID %>").value;
                        break;
                    case "005":
                        value = document.getElementById("<%= hdfSearchedBroker.ClientID %>").value;
                        break;
                    default:
                        value = "N/A";
                }
                alert("SELECTED OPT " + selectedField + " AND VALUE" + value);
                chequeInput.value = value || "Value not found";
            }


            function ddlChequeChanged_tr1() {
                const selectedField = document.getElementById("<%= chequeNoSelect.ClientID %>").value;
                const chequeInput = document.getElementById("<%= chequeNo.ClientID %>");

                if (!rowData) {
                    chequeInput.value = "No row data available.";
                    return;
                }

                const fieldMap = {
                    "001": "cheque_no",
                    "002": "folio",
                    "003": "app_no",
                    "004": "pann_no",
                    "005": "broker_id"
                };

                const dataKey = fieldMap[selectedField];
                if (!dataKey || !rowData[dataKey + " value"]) {
                    chequeInput.value = "Invalid selection or missing data.";
                    return;
                }

                chequeInput.value = rowData[dataKey + " value"];
            }

            function ddlChequeChanged() {
                var selectedField = document.getElementById("<%= chequeNoSelect.ClientID %>").value;
                var selectedValue = document.getElementById("hfSelectedRow").value;
                var selectedIndex = isNaN(parseInt(selectedValue)) ? -1 : parseInt(selectedValue);
                var grid = document.getElementById("<%= tableSearchResults.ClientID %>");


                if (!grid || selectedIndex < 0) {
                    document.getElementById("<%= chequeNo.ClientID %>").value = "Please select a row first.";
                    return;
                }

                var rows = grid.getElementsByTagName("tr");
                if (selectedIndex + 1 >= rows.length) {
                    document.getElementById("<%= chequeNo.ClientID %>").value = "Invalid row selected.";
                    return;
                }

                var row = rows[selectedIndex + 1];
                var value = "";

                switch (selectedField) {
                    case "001": value = row.querySelector("[id*='lblChqNo']").innerText; break;
                    case "002": value = row.querySelector("[id*='lblFolioNo']").innerText; break;
                    case "003": value = row.querySelector("[id*='lblAppNoModify']").innerText; break;
                    case "004": value = row.querySelector("[id*='lblpanNo']").innerText; break;
                    case "005": value = row.querySelector("[id*='lblbrokerCode']").innerText; break;
                }

                document.getElementById("<%= chequeNo.ClientID %>").value = value;

                // Rebind dropdown after partial update
                Sys.Application.add_load(function () {
                    var ddl = document.getElementById("<%= chequeNoSelect.ClientID %>");
                    if (ddl) {
                        ddl.onchange = ddlChequeChanged;
                    }
                });
                alert("this is amy onchange alert")
            }

            // Rebind dropdown after partial update
            Sys.Application.add_load(function () {
                var ddl = document.getElementById("<%= chequeNoSelect.ClientID %>");
                if (ddl) {
                    ddl.onchange = ddlChequeChanged;
                }
            });
        </script>

     <script type="text/javascript">
                                            function onRowDoubleClick2(row) {
                                                // Find the checkbox in the row
                                                var checkbox = row.querySelector('input[type="checkbox"]');

                                                if (checkbox) {
                                                    // Toggle the checkbox state
                                                    checkbox.checked = !checkbox.checked;

                                                    // Trigger the checkbox's change event to cause postback
                                                    __doPostBack(checkbox.name, '');
                                                }
                                            }
                                        </script>

    

        <asp:UpdatePanel ID="UpdatePanelFirst" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="card mb-3">
                    <%-- AR SEARCH INPUT --%>
                        <div class="card-body">
                            <h4 class="card-title">WealthMaker Transactions</h4>
                            <div class="row g-3">
                                <%-- CHANNEL --%>
                                    <div class="col-md-1">
                                        <label for="channelSelect" class="form-label">Channel</label>
                                        <asp:DropDownList ID="channelSelect" CssClass="form-select" runat="server"
                                            AutoPostBack="true"
                                            OnSelectedIndexChanged="ddlBranchCategory_SelectedIndexChanged">
                                            <asp:ListItem Text="Select Channel" Value=""> </asp:ListItem>
                                        </asp:DropDownList>
                                    </div>

                                    <%-- REGION --%>
                                        <div class="col-md-1">
                                            <label for="regionSelect" class="form-label">Region</label>
                                            <asp:DropDownList ID="regionSelect" CssClass="form-select" runat="server"
                                                AutoPostBack="true"
                                                OnSelectedIndexChanged="ddlRegion_SelectedIndexChanged">
                                                <asp:ListItem Text="Select Region" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>

                                        <%-- ZONE --%>
                                            <div class="col-md-1">
                                                <label for="zoneSelect" class="form-label">Zone</label>
                                                <asp:DropDownList ID="zoneSelect" CssClass="form-select" runat="server"
                                                    AutoPostBack="true"
                                                    OnSelectedIndexChanged="ddlzone_SelectedIndexChanged">
                                                    <asp:ListItem Text="" Value=""></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>

                                            <%-- BRANCH LIST --%>
                                                <div class="col-md-1">
                                                    <label for="branchSelect" class="form-label">Branch</label>
                                                    <asp:DropDownList ID="branchSelect" CssClass="form-select"
                                                        runat="server" AutoPostBack="true"
                                                        OnSelectedIndexChanged="ddlRmFill_SelectedIndexChanged">
                                                        <asp:ListItem Text="Select Branch" Value=""></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>

                                                <%-- RM --%>
                                                    <div class="col-md-1">
                                                        <label for="rmSelect" class="form-label">RM</label>
                                                        <asp:DropDownList ID="rmSelect" CssClass="form-select"
                                                            runat="server">
                                                            <asp:ListItem Text="Select RM" Value=""></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>

                                                    <%-- DDL: AMC --%>
                                                        <div class="col-md-1">
                                                            <label for="amcSelect" class="form-label">AMC</label>
                                                            <asp:DropDownList ID="amcSelect" CssClass="form-select"
                                                                runat="server">
                                                                <asp:ListItem Text="AMC 1" Value=""></asp:ListItem>
                                                                <asp:ListItem Text="AMC 2" Value="2"></asp:ListItem>
                                                                <asp:ListItem Text="AMC 3" Value="3"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>

                                                        <%-- AR NUMBER --%>
                                                            <div class="col-md-2">
                                                                <label for="arNumber" class="form-label">AR
                                                                    Number</label>
                                                                <asp:TextBox ID="arNumber" CssClass="form-control"
                                                                    runat="server" />
                                                            </div>

                                                            <%-- DATE FROM --%>
                                                                <div class="col-md-2">
                                                                    <label for="dateFrom" class="form-label">Date
                                                                        From</label>
                                                                    <div class="date">
                                                                        <asp:TextBox ID="dateFrom" runat="server"
                                                                            CssClass="form-control"
                                                                            oninput="formatDateInput(this)"
                                                                            placeholder="dd/mm/yyyy" MaxLength="10">
                                                                        </asp:TextBox>
                                                                        <%-- DATE FORMAT SCRIPT --%>


                                                                    </div>
                                                                </div>

                                                                <%-- DATE TO --%>
                                                                    <div class="col-md-2">
                                                                        <label for="dateTo" class="form-label">Date To
                                                                        </label>
                                                                        <div class="date">
                                                                            <asp:TextBox ID="dateTo" runat="server"
                                                                                CssClass="form-control "
                                                                                oninput="formatDateInput(this)"
                                                                                placeholder="dd/mm/yyyy" MaxLength="10">
                                                                            </asp:TextBox>
                                                                            <div class="input-group-addon">
                                                                                <span
                                                                                    class="glyphicon glyphicon-th"></span>
                                                                            </div>
                                                                        </div>
                                                                    </div>

                                                                    <%-- TRAM TYPE REN/SIP --%>
                                                                        <div class="col-md-2">
                                                                            <label class="form-label">Tran Type</label>
                                                                            <div
                                                                                class="d-flex align-items-center gap-3 my-cstm-radio-btn-cntnr-2">
                                                                                <asp:RadioButtonList
                                                                                    ID="RadioButtonList2" runat="server"
                                                                                    RepeatDirection="Horizontal"
                                                                                    CssClass="form-check">
                                                                                    <asp:ListItem Text="Renewal"
                                                                                        Value="renewal"></asp:ListItem>
                                                                                    <asp:ListItem Text="Sip"
                                                                                        Value="sip"></asp:ListItem>
                                                                                </asp:RadioButtonList>
                                                                            </div>
                                                                        </div>

                                                                        <%-- RDB: REGISTRAR --%>
                                                                            <div class="col-md-3">
                                                                                <label
                                                                                    class="form-label mb-4">Registrar</label>
                                                                                <div
                                                                                    class="d-flex align-items-center gap-3 my-cstm-radio-btn-cntnr-2">
                                                                                    <div class="form-check-2">
                                                                                        <label class="form-check-label "
                                                                                            for="c">
                                                                                            <asp:RadioButton ID="c"
                                                                                                GroupName="registrar"
                                                                                                runat="server"
                                                                                                Text="C" />
                                                                                        </label>
                                                                                    </div>
                                                                                    <div class="form-check-2">
                                                                                        <label class="form-check-label"
                                                                                            for="k">
                                                                                            <asp:RadioButton ID="k"
                                                                                                GroupName="registrar"
                                                                                                runat="server"
                                                                                                Text="K" />
                                                                                        </label>
                                                                                    </div>
                                                                                    <div class="form-check-2">
                                                                                        <label class="form-check-label"
                                                                                            for="cCob">
                                                                                            <asp:RadioButton ID="cCob"
                                                                                                GroupName="registrar"
                                                                                                runat="server"
                                                                                                Text="C COB" />
                                                                                        </label>
                                                                                    </div>
                                                                                    <div class="form-check-2">
                                                                                        <label class="form-check-label"
                                                                                            for="kCob">
                                                                                            <asp:RadioButton ID="kCob"
                                                                                                GroupName="registrar"
                                                                                                runat="server"
                                                                                                Text="K COB" />
                                                                                        </label>
                                                                                    </div>
                                                                                </div>
                                                                            </div>

                                                                            <%-- PSM --%>
                                                                                <div class="col-md-1">
                                                                                    <label
                                                                                        class="form-label mb-4">PMS:</label>
                                                                                    <div
                                                                                        class="form-check-2 d-flex align-items-center gap-3 my-cstm-radio-btn-cntnr-2">
                                                                                        <asp:CheckBox ID="pms"
                                                                                            runat="server" Text="PMS" />
                                                                                    </div>
                                                                                </div>

                                                                                <%-- CHK: COB --%>
                                                                                    <div class="col-md-1">
                                                                                        <label
                                                                                            class="form-label mb-4">COB:</label>

                                                                                        <div
                                                                                            class="form-check-2 d-flex align-items-center gap-3 my-cstm-radio-btn-cntnr-2">
                                                                                            <asp:CheckBox ID="cob"
                                                                                                runat="server"
                                                                                                Text="COB" />
                                                                                        </div>
                                                                                    </div>

                                                                                    <%-- RDB: STATUS RECO STATUE Y/N
                                                                                        --%>
                                                                                        <div class="col-md-2">
                                                                                            <label
                                                                                                class="form-label">Status</label>
                                                                                            <div
                                                                                                class="d-flex align-items-center gap-3">
                                                                                                <asp:RadioButtonList
                                                                                                    ID="rblReconciliationType"
                                                                                                    runat="server"
                                                                                                    RepeatDirection="Horizontal"
                                                                                                    CssClass="form-check">
                                                                                                    <asp:ListItem
                                                                                                        Text="Reco"
                                                                                                        Value="Y">
                                                                                                    </asp:ListItem>
                                                                                                    <asp:ListItem
                                                                                                        Text="Unreco"
                                                                                                        Value="N"
                                                                                                        Selected="True">
                                                                                                    </asp:ListItem>
                                                                                                </asp:RadioButtonList>
                                                                                            </div>
                                                                                        </div>

                                                                                        <div class="col-md-2">
                                                                                            <div class="my-cstm-radio-btn-cntnr-2 mt-5"
                                                                                                style="display: flex; align-items: center; gap: 1rem;">
                                                                                                <div
                                                                                                    class="form-check-2">
                                                                                                    <asp:RadioButton
                                                                                                        ID="op1"
                                                                                                        runat="server"
                                                                                                        GroupName="options" />
                                                                                                    <label
                                                                                                        class="form-check-label"
                                                                                                        for="op1">OP1<i
                                                                                                            class="input-helper"></i></label>
                                                                                                </div>
                                                                                                <div
                                                                                                    class="form-check-2">
                                                                                                    <asp:RadioButton
                                                                                                        ID="op2"
                                                                                                        runat="server"
                                                                                                        GroupName="options"
                                                                                                        Checked="true" />
                                                                                                    <label
                                                                                                        class="form-check-label"
                                                                                                        for="op2">OP2<i
                                                                                                            class="input-helper"></i></label>
                                                                                                </div>
                                                                                            </div>
                                                                                        </div>

                                                                                        <div class="col-md-1">
                                                                                            <div class="my-cstm-radio-btn-cntnr-2 mt-5"
                                                                                                style="display: flex; align-items: center; gap: 1rem;">
                                                                                                <p
                                                                                                    class="mb-0 text-danger fw-bold">
                                                                                                    Count</p>
                                                                                                <asp:Label
                                                                                                    ID="lblRowCount"
                                                                                                    runat="server"
                                                                                                    Text="0">
                                                                                                </asp:Label>
                                                                                                </p>
                                                                                            </div>
                                                                                        </div>

                                                                                        <%-- BTN: SEARCH, RESET AND
                                                                                            EXPORT --%>

                                                                                            <div class="col-md-2">
                                                                                                <asp:Button
                                                                                                    ID="btnSearch"
                                                                                                    CssClass="btn btn-primary w-100"
                                                                                                    runat="server"
                                                                                                    Text="Search"
                                                                                                    OnClick="btnSearch_Click" />

                                                                                            </div>
                                                                                            <div class="col-md-2">
                                                                                                <asp:Button
                                                                                                    ID="btnReset"
                                                                                                    CssClass="btn btn-outline-primary w-100"
                                                                                                    runat="server"
                                                                                                    Text="Reset"
                                                                                                    OnClick="btnReset_Click" />

                                                                                            </div>
                                                                                            <div class="col-md-2">
                                                                                                <asp:Button
                                                                                                    ID="btnExport"
                                                                                                    CssClass="btn btn-outline-primary w-100"
                                                                                                    runat="server"
                                                                                                    Text="Export"
                                                                                                    OnClick="btnExport1_Click" />
                                                                                            </div>
                            </div>
                        </div>
                        <asp:HiddenField ID="hftran1stcode" runat="server" />

                        <div class="table-responsive m-4" id="gridContainer1"
                            style="max-height: 250px; overflow-y: auto;">


                            <asp:GridView ID="tableSearchResults" AutopushtBack="true" EnableViewState="true"
                                CssClass="table table-bordered" runat="server"
                                OnRowDataBound="tableSearchResults_RowDataBound" DataKeyNames="TRAN_CODE"
                                AutoGenerateColumns="false" OnRowCommand="tableSearchResults_RowCommand">

                                <HeaderStyle CssClass="thead-dark" />
                                <Columns>

                                    <asp:TemplateField HeaderText="Action" ItemStyle-CssClass="column-width-50">
                                        <ItemTemplate>

                                            <asp:HiddenField ID="hfTranCode" runat="server"
                                                Value='<%# Eval("TRAN_CODE") %>' />

                                            <asp:CheckBox ID="chkSelect" runat="server" AutoPostBack="true"
                                                OnCheckedChanged="chkSelect_CheckedChanged" />

                                        </ItemTemplate>
                                    </asp:TemplateField>



                                    <asp:TemplateField HeaderText="TranCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblTranType" runat="server"
                                                Text='<%# Eval("TRAN_CODE") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="TranDate">
                                        <ItemTemplate>
                                            <asp:Label ID="lblTranDate" runat="server"
                                                Text='<%# Eval("TR_DATE", "{0:dd/MM/yyyy}") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="InvestorName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblInvestorName" runat="server"
                                                Text='<%# Eval("INVESTOR_NAME") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="AMC CODE" ItemStyle-CssClass="hide-column"
                                        HeaderStyle-CssClass="hide-column">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAMC" runat="server" Text='<%# Eval("MUT_CODE") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Address" ItemStyle-Width="80px"
                                        HeaderStyle-Width="80px">
                                        <ItemTemplate>
                                            <asp:Label ID="lbladressfirst" runat="server" Style="width: 80px"
                                                Text='<%# Eval("ADDRESS") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="City">
                                        <ItemTemplate>
                                            <asp:Label ID="lblcityfirst" runat="server"
                                                Text='<%# Eval("CITY_NAME") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="AMC NAME">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAMCNAME" runat="server" Text='<%# Eval("MUT_NAME") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Scheme Name">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSchemeName" runat="server"
                                                Text='<%# Eval("SCH_NAME") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Amount">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAmount" runat="server" Text='<%# Eval("AMOUNT") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Folio No">
                                        <ItemTemplate>
                                            <asp:Label ID="lblFolioNo" runat="server" Text='<%# Eval("FOLIO_NO") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Chq No">
                                        <ItemTemplate>
                                            <asp:Label ID="lblChqNo" runat="server" Text='<%# Eval("CHEQUE_NO") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="TranType" ItemStyle-CssClass="hide-column"
                                        HeaderStyle-CssClass="hide-column">
                                        <ItemTemplate>
                                            <asp:Label ID="lblTrnType" runat="server" Text='<%# Eval("TRAN_TYPE") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>


                                    <asp:TemplateField HeaderText="App No" ItemStyle-CssClass="hide-column"
                                        HeaderStyle-CssClass="hide-column">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAppNoModify" runat="server"
                                                Text='<%# Eval("APP_NO") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>




                                    <asp:TemplateField HeaderText="Pan No" ItemStyle-CssClass="hide-column"
                                        HeaderStyle-CssClass="hide-column">
                                        <ItemTemplate>
                                            <asp:Label ID="lblpanNo" runat="server" Text='<%# Eval("PANNO") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Broker Code" ItemStyle-CssClass="hide-column"
                                        HeaderStyle-CssClass="hide-column">
                                        <ItemTemplate>
                                            <asp:Label ID="lblbrokerCode" runat="server"
                                                Text='<%# Eval("BROKER_ID") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>





                                    <asp:TemplateField HeaderText="RM" ItemStyle-CssClass="hide-column"
                                        HeaderStyle-CssClass="hide-column">
                                        <ItemTemplate>
                                            <asp:Label ID="lblRM" runat="server" Text='<%# Eval("RMCODE") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>







                                    <asp:TemplateField HeaderText="RM Name">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSchemeCode" runat="server"
                                                Text='<%# Eval("rm_name") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Branch Name">
                                        <ItemTemplate>
                                            <asp:Label ID="lblbranchna" runat="server"
                                                Text='<%# Eval("branch_name") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Sip Amount">
                                        <ItemTemplate>
                                            <asp:Label ID="lblsipamount" runat="server"
                                                Text='<%# Eval("Sip_Amount") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>



                                    <asp:TemplateField HeaderText="Branch Code" ItemStyle-CssClass="hide-column"
                                        HeaderStyle-CssClass="hide-column">
                                        <ItemTemplate>
                                            <asp:Label ID="lblbranchco" runat="server"
                                                Text='<%# Eval("busi_branch_code") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Total SIP" ItemStyle-CssClass="hide-column"
                                        HeaderStyle-CssClass="hide-column">
                                        <ItemTemplate>
                                            <asp:Label ID="lblInstallNo" runat="server"
                                                Text='<%# Eval("INSTALLMENTS_NO") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>


                                    <asp:TemplateField HeaderText="STATUS">
                                        <ItemTemplate>
                                            <asp:Label ID="lblRECflag" runat="server"
                                                Text='<%# Eval("REC_FLAG") != null && Eval("REC_FLAG").ToString() == "Y" ? "RECONCILED" : "UNRECONCILED" %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Flag" ItemStyle-CssClass="hide-column"
                                        HeaderStyle-CssClass="hide-column">
                                        <ItemTemplate>
                                            <asp:Label ID="lblflag" runat="server" Text='<%# Eval("FLAG") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="registrar" ItemStyle-CssClass="hide-column"
                                        HeaderStyle-CssClass="hide-column">
                                        <ItemTemplate>
                                            <asp:Label ID="lblregis" runat="server" Text='<%# Eval("registrar") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="COB FLAG" ItemStyle-CssClass="hide-column"
                                        HeaderStyle-CssClass="hide-column">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCOBFL" runat="server" Text='<%# Eval("COB_FLAG") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="REMARK">
                                        <ItemTemplate>
                                            <asp:Label ID="lblREMARK" runat="server" Text='<%# Eval("cob_remark") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                </Columns>
                            </asp:GridView>

                        </div>

                        <br />

                   
                        <div class="grid-margin stretch-card draggable-container">
                            <div class="card mb-1">
                                <div class="card-body ">
                                    <h4 class="card-title">RTA Transactions</h4>

                                    <div class="table-responsive" id="gridContainer2" style="max-height: 250px; overflow-y: auto;">

                                        <asp:GridView ID="GridRta" CssClass="table table-bordered" runat="server"
                                            AutopushtBack="true" EnableViewState="true"
                                            OnRowDataBound="GridRta_RowDataBound" DataKeyNames="TRAN_CODE"
                                            OnRowCommand="GridRta_RowCommand" AutoGenerateColumns="false">
                                            <RowStyle CssClass="grid-row" />
                                            <HeaderStyle CssClass="thead-dark" />
                                            <Columns>

                                                <asp:TemplateField HeaderText="Action"
                                                    ItemStyle-CssClass="column-width-50">
                                                    <ItemTemplate>

                                                        <asp:HiddenField ID="hfTranCoderta" runat="server"
                                                            Value='<%# Eval("TRAN_CODE") %>' />

                                                        <asp:CheckBox ID="chkSelect" runat="server" AutoPostBack="true"
                                                            OnCheckedChanged="chkSelectrta_CheckedChanged" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="TranCode">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTranTyp" runat="server"
                                                            Text='<%# Eval("TRAN_CODE") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="TranDate">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTrnDate" runat="server"
                                                            Text='<%# Eval("TR_DATE", "{0:dd/MM/yyyy}") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Investor" ItemStyle-Width="150px"
                                                    HeaderStyle-Width="150px">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblInvestorNam" runat="server"
                                                            Text='<%# Eval("INV_NAME") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="AMC CODE" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAC" runat="server"
                                                            Text='<%# Eval("MUT_CODE") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Address">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAdderess" runat="server"
                                                            Text='<%# Eval("ADDRESS") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="City Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCityeName" runat="server"
                                                            Text='<%# Eval("CITY_NAME") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="AMC NAME">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAMCNAM" runat="server"
                                                            Text='<%# Eval("MUT_NAME") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>


                                                <asp:TemplateField HeaderText="Scheme Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblScemeName" runat="server"
                                                            Text='<%# Eval("SCH_NAME") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Amount">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblmount" runat="server"
                                                            Text='<%# Eval("AMOUNT", "{0:N2}") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Folio No">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFoloNo" runat="server"
                                                            Text='<%# Eval("FOLIO_NO") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="TranType" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="llTrnType" runat="server"
                                                            Text='<%# Eval("TRAN_TYPE") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>



                                                <asp:TemplateField HeaderText="Chq No">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCqNo" runat="server"
                                                            Text='<%# Eval("CHEQUE_NO") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>


                                                <asp:TemplateField HeaderText="App No">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAppoModify" runat="server"
                                                            Text='<%# Eval("APP_NO") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>





                                                <asp:TemplateField HeaderText="RM" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="llRM" runat="server"
                                                            Text='<%# Eval("RMCODE") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>




                                                <asp:TemplateField HeaderText="RM NAME">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblScemeCode" runat="server"
                                                            Text='<%# Eval("rm_name") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <%--<asp:TemplateField HeaderText="Sip Amount">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblsipamont" runat="server"
                                                            Text='<%# Eval("Sip_Amount") %>' />
                                                    </ItemTemplate>
                                                    </asp:TemplateField>--%>

                                                    <asp:TemplateField HeaderText="Branch Name">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblbranhna" runat="server"
                                                                Text='<%# Eval("branch_name") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="BROKER CODE">
                                                        <ItemTemplate>
                                                            <asp:Label ID="LBLBROKERCODE" runat="server"
                                                                Text='<%# Eval("BROKER_ID") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Reg Tran Type" ItemStyle-Width="50px"
                                                        HeaderStyle-Width="50px">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblregfag" runat="server"
                                                                Text='<%# Eval("REG_TRANTYPE") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Unique Key">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblekey" runat="server"
                                                                Text='<%# Eval("UNQ_KEY") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Flag" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblfag" runat="server"
                                                                Text='<%# Eval("FLAG") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                            </Columns>
                                        </asp:GridView>
                                    </div>

                                    <br />

                                    <!-- RTA SEARCH INPUT FIELDS -->
                                    <div class="row g-3">
                                        <div class="col-md-4">
                                            <div class="row g-3">

                                                <%-- DATE FROM --%>
                                                    <div class="col-md-6">
                                                        <label for="dateFrom-rta" class="form-label">Date From <span
                                                                class="text-danger">*</span></label>
                                                        <div class="date">
                                                            <asp:TextBox ID="dateFromRta" runat="server"
                                                                CssClass="form-control " oninput="formatDateInput(this)"
                                                                placeholder="dd/mm/yyyy" MaxLength="10" />

                                                        </div>
                                                    </div>

                                                    <%-- DATE TO --%>
                                                        <div class="col-md-6">
                                                            <label for="dateTo-rta" class="form-label">Date To <span
                                                                    class="text-danger">*</span></label>
                                                            <div class="date">
                                                                <asp:TextBox ID="dateToRta" runat="server"
                                                                    CssClass="form-control"
                                                                    oninput="formatDateInput(this)"
                                                                    placeholder="dd/mm/yyyy" MaxLength="10" />

                                                            </div>
                                                        </div>

                                                        <%-- AMC --%>
                                                            <div class="col-md-6">
                                                                <label for="amcSelectrta" class="form-label">AMC</label>
                                                                <asp:DropDownList ID="amcSelectrta" runat="server"
                                                                    CssClass="form-select">
                                                                    <asp:ListItem Text="Select AMC" Value="" />
                                                                    <asp:ListItem Text="AMC A" Value="A" />
                                                                    <asp:ListItem Text="AMC B" Value="B" />
                                                                    <asp:ListItem Text="AMC C" Value="C" />
                                                                </asp:DropDownList>
                                                            </div>

                                                            <%-- BRANCH --%>
                                                                <div class="col-md-6">
                                                                    <label for="branchSelectrta"
                                                                        class="form-label">Branch</label>
                                                                    <asp:DropDownList ID="branchSelectrta"
                                                                        runat="server" CssClass="form-select">
                                                                        <asp:ListItem Text="Select Branch" Value="" />
                                                                        <asp:ListItem Text="A" Value="A" />
                                                                        <asp:ListItem Text="B" Value="B" />
                                                                        <asp:ListItem Text="C" Value="C" />
                                                                    </asp:DropDownList>
                                                                </div>

                                                                <%-- RECON OR UNRECON --%>
                                                                    <div class="col-md-6">
                                                                        <label class="form-label"></label>
                                                                        <div
                                                                            class="d-flex align-items-center gap-3 my-cstm-radio-btn-cntnr-2">
                                                                            <div class="form-check-2">
                                                                                <asp:RadioButton ID="RadioButton1"
                                                                                    runat="server" GroupName="status"
                                                                                    CssClass="form-check-input-2"
                                                                                    Text="Reconciled" Value="Y" />
                                                                            </div>
                                                                            <div class="form-check-2">
                                                                                <asp:RadioButton ID="RadioButton2"
                                                                                    runat="server" GroupName="status"
                                                                                    CssClass="form-check-input-2"
                                                                                    Text="Unreconciled" value="N"
                                                                                    Checked="true" />
                                                                            </div>
                                                                        </div>
                                                                    </div>

                                            </div>
                                        </div>

                                        <div class="col-md-8">
                                            <div class="row g-3 mb-2">

                                                <div class="col-md-2">
                                                    <label for="ddlSelectedKey" class="form-label">Search</label>

                                                    <asp:DropDownList ID="chequeNoSelect" runat="server"
                                                        CssClass="form-select me-2" onchange="ddlChequeChanged()">
                                                        <asp:ListItem Text="" Value="" />
                                                        <asp:ListItem Text="CHEQUE_NO" Value="001"></asp:ListItem>
                                                        <asp:ListItem Text="FOLIO_NO" Value="002"></asp:ListItem>
                                                        <asp:ListItem Text="APP_NO" Value="003"></asp:ListItem>
                                                        <asp:ListItem Text="PANNO" Value="004"></asp:ListItem>
                                                        <asp:ListItem Text="BROKER_ID" Value="005"></asp:ListItem>

                                                    </asp:DropDownList>
                                                </div>

                                                <div class="col-md-2">
                                                    <label for="ddlSelectedValue" class="form-label">Value</label>

                                                    <asp:TextBox ID="chequeNo" runat="server" CssClass="form-control" />

                                                    <asp:HiddenField ID="hdfSearchedCheque" runat="server"
                                                        ClientIDMode="Static" />
                                                    <asp:HiddenField ID="hdfSearchedFilio" runat="server"
                                                        ClientIDMode="Static" />
                                                    <asp:HiddenField ID="hdfSearchedApp" runat="server"
                                                        ClientIDMode="Static" />
                                                    <asp:HiddenField ID="hdfSearchedPan" runat="server"
                                                        ClientIDMode="Static" />

                                                    <asp:HiddenField ID="hdfSearchedBroker" runat="server"
                                                        ClientIDMode="Static" />


                                                </div>

                                                <div class="col-md-4">
                                                    <label for="investorName" class="form-label">Investor Name</label>
                                                    <asp:TextBox ID="txtInvestorName" runat="server"
                                                        CssClass="form-control" />
                                                </div>

                                                <div class="col-md-2">
                                                    <label for="amount" class="form-label">Amount</label>
                                                    <asp:TextBox ID="txtAmount" runat="server"
                                                        CssClass="form-control" />
                                                </div>

                                                <div class="col-md-2">
                                                    <label for="remark" class="form-label">Remark</label>
                                                    <asp:TextBox ID="remark" runat="server" CssClass="form-control" />
                                                </div>

                                                <div class="col-md-2">
                                                    <label class="form-label">Tran Type</label>
                                                    <div
                                                        class="d-flex align-items-center gap-3 my-cstm-radio-btn-cntnr-2">

                                                        <asp:RadioButtonList ID="RadioButtonList1" runat="server"
                                                            RepeatDirection="Horizontal" CssClass="form-check">
                                                            <asp:ListItem Text="Renewal" Value="renewal"></asp:ListItem>
                                                            <asp:ListItem Text="Sip" Value="sip"></asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </div>
                                                </div>

                                                <div class="d-flex justify-content-between">
                                                    <!-- Left-aligned buttons -->
                                                    <div>
                                                        <asp:Button ID="searchBtn" runat="server" Text="Search"
                                                            CssClass="btn btn-primary" OnClick="btnSearchrta_Click" />
                                                        <asp:Button ID="resetBtn" runat="server" Text="Reset"
                                                            CssClass="btn btn-outline-primary"
                                                            OnClick="btnReset2_Click" />
                                                        <asp:Button ID="exportBtn" runat="server" Text="Export"
                                                            CssClass="btn btn-outline-primary"
                                                            OnClick="btnExport_Click" />
                                                    </div>

                                                    <!-- Right-aligned buttons -->
                                                    <div>
                                                        <asp:Button ID="saveBtn" runat="server" Text="Save"
                                                            CssClass="btn btn-primary" OnClick="CmdSaveRemark_Click" />
                                                        <asp:Button ID="reconcileBtn" runat="server"
                                                            Text="Base SIP Reconcile" CssClass="btn btn-outline-primary"
                                                            OnClick="btnsiprerta_Click" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>



                                </div>
                            </div>
                        </div>

                        <asp:HiddenField ID="hfSelectedRow" runat="server" ClientIDMode="Static" Value="-1" />

                        <asp:HiddenField ID="hfrtaselectedrow" runat="server" ClientIDMode="Static" Value="-1" />







                        <div class="card mb-3">
                            <div class="card-body">
                                <h4 class="card-title">SIP Master</h4>

                                <div class="row g-3 mb-3">
                                    <div class="col-md-2">
                                        <label for="folioNo" class="form-label">Folio No</label>
                                        <asp:TextBox ID="folioNo" runat="server" CssClass="form-control"
                                            Enabled="false"></asp:TextBox>
                                    </div>

                                    <div class="col-md-2">
                                        <label for="amount" class="form-label">Amount</label>
                                        <asp:TextBox ID="TextBox3" runat="server" CssClass="form-control"
                                            Enabled="false"></asp:TextBox>
                                    </div>

                                    <div class="col-md-2">
                                        <label for="pan" class="form-label">PAN</label>
                                        <asp:TextBox ID="pan" runat="server" CssClass="form-control" Enabled="false">
                                        </asp:TextBox>
                                    </div>

                                    <div class="col-md-2">
                                        <label for="clientCode" class="form-label">Client Code</label>
                                        <asp:TextBox ID="clientCode" runat="server" CssClass="form-control"
                                            Enabled="false"></asp:TextBox>
                                    </div>



                                    <div class="col-md-2">
                                        <label for="sipStartDate" class="form-label">WW SIP Start Date</label>
                                        <div class="date" data-provide="datepicker">
                                            <asp:TextBox ID="sipStartDate" runat="server"
                                                CssClass="form-control date-input" Enabled="false"></asp:TextBox>
                                            <div class="input-group-addon">
                                                <span class="glyphicon glyphicon-th"></span>
                                            </div>
                                        </div>
                                    </div>


                                    <div class="d-flex align-items-center flex-md-row flex-column gap-3">
                                        <asp:Button ID="searchButton" runat="server" CssClass="btn btn-primary w-100"
                                            Text="Search" OnClick="btnSearchsip_Click" Enabled="false" />
                                        <asp:Button ID="reconcileButton" runat="server"
                                            CssClass="btn btn-outline-primary w-100" Text="SIP Reconcile"
                                            Enabled="false" />
                                        <asp:Button ID="resetButton" runat="server"
                                            CssClass="btn btn-outline-primary w-100" Text="Reset"
                                            OnClick="btnReset_Click" Enabled="false" />
                                    </div>

                                    <br>

                                    <div class="table-responsive">
                                        <asp:GridView ID="GridSIPTransactions" CssClass="table table-bordered"
                                            runat="server" AutoGenerateColumns="false">
                                            <HeaderStyle CssClass="thead-dark" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="Investor Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblInvestorName" runat="server"
                                                            Text='<%# Eval("INVESTOR_NAME") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Address">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAddress" runat="server"
                                                            Text='<%# Eval("ADDRESS") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="City Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCityName" runat="server"
                                                            Text='<%# Eval("CITY_NAME") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Bank Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblBankName" runat="server"
                                                            Text='<%# Eval("BANK_NAME") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Client Code">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblClientCode" runat="server"
                                                            Text='<%# Eval("CLIENT_CODE") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Scheme Code">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSchemeCode" runat="server"
                                                            Text='<%# Eval("SCH_CODE") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Mut Code">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblMutCode" runat="server"
                                                            Text='<%# Eval("MUT_CODE") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="RM Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRmName" runat="server"
                                                            Text='<%# Eval("RM_NAME") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Branch Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblBranchName" runat="server"
                                                            Text='<%# Eval("BRANCH_NAME") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="PAN No">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPanNo" runat="server"
                                                            Text='<%# Eval("PANNO") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="AMC Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAmcName" runat="server"
                                                            Text='<%# Eval("MUT_NAME") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Scheme Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSchemeName" runat="server"
                                                            Text='<%# Eval("SCH_NAME") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Transaction Date">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTrDate" runat="server"
                                                            Text='<%# Eval("TR_DATE", "{0:dd/MM/yyyy}") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Transaction Type">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTranType" runat="server"
                                                            Text='<%# Eval("TRAN_TYPE") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="App No">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAppNo" runat="server"
                                                            Text='<%# Eval("APP_NO") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Cheque No">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblChequeNo" runat="server"
                                                            Text='<%# Eval("CHEQUE_NO") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Amount">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAmount" runat="server"
                                                            Text='<%# Eval("AMOUNT", "{0:N2}") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="SIP Amount">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSipAmount" runat="server"
                                                            Text='<%# Eval("SIP_AMOUNT", "{0:N2}") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Folio No">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFolioNo" runat="server"
                                                            Text='<%# Eval("FOLIO_NO") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="SIP Type">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSipType" runat="server"
                                                            Text='<%# Eval("SIP_TYPE") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Lead No">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLeadNo" runat="server"
                                                            Text='<%# Eval("LEAD_NO") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Lead Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLeadName" runat="server"
                                                            Text='<%# Eval("LEAD_NAME") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Logged User ID">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLoggedUserId" runat="server"
                                                            Text='<%# Eval("LOGGEDUSERID") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                            </Columns>
                                        </asp:GridView>
                                    </div>

                                </div>
                            </div>



                            <div class="card">
                                <div class="card-body">
                                    <div
                                        class="d-flex align-items-md-center justify-content-between gap-3 flex-md-row flex-column">
                                        <div class="form-check">
                                            <label class="form-check-label">
                                                <span class="text-danger">Option1: WealthMaker step1 compare with RTA
                                                    SIP</span>
                                                <asp:RadioButton ID="Option1Radio" runat="server" GroupName="options" />
                                            </label>
                                        </div>

                                        <asp:Button ID="ExitButton" runat="server" Text="Exit"
                                            CssClass="btn btn-outline-primary" OnClick="btnExit_Click" />
                                    </div>
                                </div>
                            </div>

                        </div>

                        <div class="col-md-6">
                            <div class="card">
                                <div class="card-body">
                                    <label class="form-label" for="tarn-code">
                                        Tran Code
                                    </label>
                                    <div class="input-group">
                                        <asp:TextBox ID="tarnCode" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:Button ID="findButton" runat="server" Text="Find"
                                            CssClass="btn btn-outline-primary" OnClick="btnSearchtrn_Click" />
                                    </div>

                                    <div class="table-responsive">
                                        <asp:GridView ID="tranCodeGrid" CssClass="table table-bordered" runat="server"
                                            AutoGenerateColumns="false">
                                            <HeaderStyle CssClass="thead-dark" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="Tran Code">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblInvestorName" runat="server"
                                                            Text='<%# Eval("HO_TRAN_CODE") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Branch Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAddress" runat="server"
                                                            Text='<%# Eval("BRANCH_NAME") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Amount">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAmount" runat="server"
                                                            Text='<%# Eval("AMOUNT") %>' />
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
                <%--<asp:AsyncPostBackTrigger ControlID="chkSelect" EventName="CheckedChanged" />--%>
                
            </Triggers>

        </asp:UpdatePanel>

        <script src="../assets/vendors/js/vendor.bundle.base.js"></script>
        <script src="../assets/vendors/chart.js/chart.umd.js"></script>
        <script src="../assets/vendors/bootstrap-datepicker/bootstrap-datepicker.min.js"></script>
        <script src="../assets/js/off-canvas.js"></script>
        <script src="../assets/js/misc.js"></script>
        <script src="../assets/js/settings.js"></script>
        <script src="../assets/js/todolist.js"></script>
        <script src="../assets/js/jquery.cookie.js"></script>
        <script src="../assets/js/dashboard.js"></script>
        <script src="../assets/js/hoverable-collapse.js"></script>
    </asp:Content>