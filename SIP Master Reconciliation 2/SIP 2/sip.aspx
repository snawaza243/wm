
<%@ Page Title="SIP Master 2" Language="C#" MasterPageFile="~/vmSite.Master" AutoEventWireup="true" CodeBehind="sip_master1.aspx.cs" Inherits="WM.Masters.sip_master1" MaintainScrollPositionOnPostback="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link href="../assets/css/mf_ar_recon.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/xlsx/0.18.5/xlsx.full.min.js"></script>

    <!-- Add these to your head section -->
    <link href="https://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/css/toastr.min.css" rel="stylesheet" />
    <script src="https://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/js/toastr.min.js"></script>

    <script>
        // Initialize Toastr with your preferred settings
        toastr.options = {
            "closeButton": true,
            "debug": false,
            "newestOnTop": true,
            "progressBar": true,
            "positionClass": "toast-top-right",
            "preventDuplicates": false,
            "onclick": null,
            "showDuration": "300",
            "hideDuration": "1000",
            "timeOut": "5000",
            "extendedTimeOut": "1000",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        };

    </script>


    <style>
        .invalid-date {
            border: 1px solid red !important;
        }

        .form-check-label {
            margin-left: 2px !important;
        }

        td {
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
            max-width: 150px;
        }

        #transactionTable tbody tr.row-selected > td, #rtaGrid tbody tr.row-selected > td {
            background-color: lightblue !important;
        }

        #transactionTable thead tr th, #rtaGrid thead tr th {
            position: sticky !important;
            top: 0 !important;
            background-color: #B78939 !important;
            color: white !important;
            z-index: 1;
        }

        /* #transactionTable thead tr th, #transactionTable tbody tr td , #rtaGrid thead tr th, #rtaGrid tbody tr td{
            width:300px !important;
        }*/


        .ctm_hidden {
            display: none;
        }
    </style>

    <script>
        $(document).ready(function () {

            //#region ---------- HELPING FUNCTION: VALIDATION ONLOAD, AND LOADER ----------

            function checkUserSession() {
                const loginId = $('#' + '<%= hdnLoginId.ClientID %>').val();
                const roleId = $('#' + '<%= hdnRoleId.ClientID %>').val();

                if (!loginId || !roleId) {
                    alert("Session expired or invalid. Please Login");
                    window.location.href = 'https://www.wealthmaker.in/login_new.aspx';
                }
            }

            function showLoader() {
                if ($('#customLoader').length) {
                    $('#customLoader').show();
                    return;
                }

                const loaderHtml = `
    <div id="customLoader" style="
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background-color: rgba(0,0,0,0.2);
        display: flex;
        justify-content: center;
        align-items: center;
        z-index: 9999;">
        <div style="
            background-color: white;
            color: black;
            padding: 20px 30px;
            width: 250px;
            text-align: center;
            border-radius: 5px;
            font-weight: bold;
            font-size: 16px;">
            Please wait...
        </div>
    </div>
`;
                $('body').append(loaderHtml);
            }

            function hideLoader() {
                $('#customLoader').fadeOut(300, function () {
                    $(this).remove(); // Optional: remove from DOM
                });
            }

            function syncScroll() {
                var grid1 = document.getElementById('transactionContainer1');
                var grid2 = document.getElementById('transactionContainer2');

                if (!grid1 || !grid2) return;

                grid1.addEventListener('scroll', function () {
                    grid2.scrollLeft = grid1.scrollLeft;
                });

                grid2.addEventListener('scroll', function () {
                    grid1.scrollLeft = grid2.scrollLeft;
                });


            }

            function setCurrentDateToInput(inputId) {
                const now = new Date();
                const day = String(now.getDate()).padStart(2, '0');
                const month = String(now.getMonth() + 1).padStart(2, '0'); // Months are 0-based
                const year = now.getFullYear();

                const formattedDate = `${day}/${month}/${year}`;
                $('#' + inputId).val(formattedDate);
            }


            function formatDateToDMY(dateStr) {
                if (!dateStr) return '';

                const date = new Date(dateStr);

                if (isNaN(date.getTime())) return '';

                const day = ('0' + date.getDate()).slice(-2);
                const month = ('0' + (date.getMonth() + 1)).slice(-2);
                const year = date.getFullYear();

                return `${day}/${month}/${year}`;
            }

            function enableAutoDateFormat(inputId) {
                const $input = $("#" + inputId);


                // Auto-format on input
                $input.on("input", function () {
                    let val = $(this).val().replace(/\D/g, ""); // Only digits
                    if (val.length > 8) val = val.slice(0, 8);

                    let formatted = val;
                    if (val.length > 4) {
                        formatted = val.slice(0, 2) + '/' + val.slice(2, 4) + '/' + val.slice(4);
                    } else if (val.length > 2) {
                        formatted = val.slice(0, 2) + '/' + val.slice(2);
                    }

                    $(this).val(formatted);
                });

                // Validate on blur
                $input.on("blur", function () {
                    let val = $(this).val();

                    if (val === "") {
                        // Empty is allowed → reset any invalid styling
                        $(this).removeClass('invalid-date');
                        return;
                    }

                    if (!isValidDate(val)) {
                        //$(this).val(''); // Clear invalid input
                        $(this).addClass('invalid-date'); // Add red border via CSS class
                    } else {
                        $(this).removeClass('invalid-date'); // Remove red border if valid
                    }
                });

                // Helper function: validate dd/mm/yyyy format and actual date
                function isValidDate(dateStr) {
                    const regex = /^(\d{2})\/(\d{2})\/(\d{4})$/;
                    const match = dateStr.match(regex);
                    if (!match) return false;

                    const day = parseInt(match[1], 10);
                    const month = parseInt(match[2], 10) - 1; // JS months: 0–11
                    const year = parseInt(match[3], 10);

                    const date = new Date(year, month, day);
                    return (
                        date.getFullYear() === year &&
                        date.getMonth() === month &&
                        date.getDate() === day
                    );
                }
            }

            function clearDropdown(dropdownId) {
                $("#" + dropdownId).empty();
            }

            function makeTableColumnsResizable(tableId) {
                // Check if CSS for this tableId already exists
                if (!document.getElementById("resizableTableStyles_" + tableId)) {
                    const css = `
          #${tableId} {
              table-layout: fixed;
              width: 100%;
          }

          #${tableId} th {
              width: 150px;
              position: relative;
              overflow: hidden;
          }

          #${tableId} .resizer {
              position: absolute;
              right: 0;
              top: 0;
              width: 5px;
              height: 100%;
              cursor: col-resize;
              user-select: none;
              z-index: 1;
          }

          #${tableId} th:hover .resizer {
              background-color: rgba(0, 0, 0, 0.1);
          }
      `;

                    const style = document.createElement("style");
                    style.id = "resizableTableStyles_" + tableId;
                    style.innerHTML = css;
                    document.head.appendChild(style);
                }

                const table = document.getElementById(tableId);
                const ths = table.querySelectorAll("th");

                ths.forEach(th => {
                    if (!th.style.width) {
                        th.style.width = th.offsetWidth + "px";
                    }

                    const resizer = document.createElement("div");
                    resizer.classList.add("resizer");
                    th.appendChild(resizer);

                    let startX, startWidth;

                    resizer.addEventListener("mousedown", function (e) {
                        startX = e.pageX;
                        startWidth = th.offsetWidth;

                        function onMouseMove(e) {
                            const newWidth = startWidth + (e.pageX - startX);
                            th.style.width = newWidth + "px";
                        }

                        function onMouseUp() {
                            document.removeEventListener("mousemove", onMouseMove);
                            document.removeEventListener("mouseup", onMouseUp);
                        }

                        document.addEventListener("mousemove", onMouseMove);
                        document.addEventListener("mouseup", onMouseUp);

                        e.preventDefault();
                    });
                });
            }

            function RedirectToWelcomePage() {
                const loginId = $("#<%= hdnLoginId.ClientID %>").val();
                const roleId = $("#<%= hdnRoleId.ClientID %>").val();
                const encodedLoginId = encodeURIComponent(loginId);
                const encodedRoleId = encodeURIComponent(roleId);
                const url = `/welcome?loginid=${encodedLoginId}&roleid=${encodedRoleId}`;
                window.location.href = url; // Uncomment to redirect
            }

            function ExportTableToExcel_1(tableId, filePrefix) {
                try {
                    const $table = $(`#${tableId}`);
                    const rowCount = $table.find('tbody tr').length;

                    if (rowCount === 0 || $table.find('tbody tr:visible').length === 0) {
                        alert(`⚠️ No data available in the table to export.`);
                        return;
                    }
                    const tableElem = document.getElementById(tableId);
                    const wb = XLSX.utils.table_to_book(tableElem, { sheet: "Transactions" });
                    const fileName = `${filePrefix}_Transaction_List.xlsx`;
                    XLSX.writeFile(wb, fileName);
                    alert(`✅ Excel file "${fileName}" has been downloaded successfully.`);
                } catch (error) {
                    alert(`❌ Error during export: ${error.message}`);
                }
            }


            //#endregion ---------- HELPING: ONLOAD ----------

            //#region ---------- HELPING FUNCTION: DDL DATA LOAD AJAX ----------

            function loadChannelList() {
                //alert('channel list');

                $.ajax({
                    type: "POST",
                    url: "/masters/sip_master1.aspx/GetChannelList",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    beforeSend: function () {
                        //showLoader();
                    },

                    success: function (res) {
                        let { data } = JSON.parse(res.d);
                        data.unshift({ text: "All", value: "" });
                        let optionsHtml = data.map(r => `<option value="${r.value}">${r.text}</option>`);
                        $("#ddlChannel").html(optionsHtml.join(''));
                    },
                    error: function (xhr, status, error) {
                        alert("Failed to load channel list: " + error);
                    },
                    complete: function () {
                        //hideLoader();
                    }

                });
            }

            function loadRegionList() {
                //alert(' region list');

                $.ajax({
                    type: "POST",
                    url: "/masters/sip_master1.aspx/GetRegionList",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSON.stringify({}), // Required for WebMethod, even if no params
                    success: function (res) {
                        let { data } = JSON.parse(res.d);
                        data.unshift({ text: "All", value: "" });
                        let optionsHtml = data.map(r => `<option value="${r.value}">${r.text}</option>`);
                        $("#ddlRegion").html(optionsHtml.join(''));
                    },
                    error: function (xhr, status, error) {
                        alert("❌ Error loading region list: " + xhr.responseText);
                    }
                });
            }

            function loadBranchList() {
                //alert(' branch list');

                $.ajax({
                    type: "POST",
                    url: "/masters/sip_master1.aspx/GetBranchList", // Replace with actual ASPX page name
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (res) {
                        let { data } = JSON.parse(res.d);
                        data.unshift({ text: "All", value: "" });
                        let optionsHtml = data.map(r => `<option value="${r.value}">${r.text}</option>`);
                        $("#ddlBranch").html(optionsHtml.join(''));
                        $("#ddlRtaBranch").html(optionsHtml.join(''));
                    },
                    error: function (xhr, status, error) {
                        alert("❌ Error loading branch list: " + xhr.responseText);
                    }
                });
            }

            function loadZoneList() {
                //alert(' zone list');

                $.ajax({
                    type: "POST",
                    url: "/masters/sip_master1.aspx/GetZoneList", // Replace with actual ASPX page name
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (res) {
                        let { data } = JSON.parse(res.d);
                        data.unshift({ text: "All", value: "" });
                        let optionsHtml = data.map(r => `<option value="${r.value}">${r.text}</option>`);
                        $("#ddlZone").html(optionsHtml.join(''));
                    },
                    error: function (xhr, status, error) {
                        alert("❌ Error loading zone list: " + xhr.responseText);
                    }
                });
            }

            function loadAMCList() {
                //alert(' amc list');

                $.ajax({
                    type: "POST",
                    url: "/masters/sip_master1.aspx/GetAMCList", // Replace with actual ASPX page name
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (res) {
                        let { data } = JSON.parse(res.d);
                        data.unshift({ text: "All", value: "" });
                        let optionsHtml = data.map(r => `<option value="${r.value}">${r.text}</option>`);
                        $("#ddlAMC").html(optionsHtml.join(''));
                        $("#ddlRtaAMC").html(optionsHtml.join(''));
                    },
                    error: function (xhr, status, error) {
                        alert("❌ Error loading AMC list: " + xhr.responseText);
                    }
                });
            }

            function loadRMList() {
                //alert(' rm list');

                $.ajax({
                    type: "POST",
                    url: "/masters/sip_master1.aspx/GetRmList", // Replace with actual ASPX page name
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (res) {
                        let { data } = JSON.parse(res.d);
                        data.unshift({ text: "All", value: "" });
                        let optionsHtml = data.map(r => `<option value="${r.value}">${r.text}</option>`);
                        $("#ddlRM").html(optionsHtml.join(''));
                    },
                    error: function (xhr, status, error) {
                        alert("❌ Error loading RM list: " + xhr.responseText);
                    }
                });
            }

            function loadBranchByChannel(channelCode) {
                //alert(' branch by channel');

                $.ajax({
                    url: "/masters/sip_master1.aspx/GetBranchListByChannel",
                    method: "POST",
                    data: JSON.stringify({ channel: channelCode }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (res) {
                        let { data } = JSON.parse(res.d);
                        data.unshift({ text: "All", value: "" });
                        let options = data.map(x => `<option value="${x.value}">${x.text}</option>`);
                        $("#ddlBranch").html(options.join(''));
                    },
                    error: function (xhr, status, error) {
                        alert("❌ Error loading branch by channel: " + xhr.responseText);
                    }
                });
            }

            function loadBranchByZone(zoneCode) {
                //alert(' branch by zone');

                $.ajax({
                    url: "/masters/sip_master1.aspx/GetBranchListByZone",
                    method: "POST",
                    data: JSON.stringify({ zone: zoneCode }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (res) {
                        let { data } = JSON.parse(res.d);
                        data.unshift({ text: "All", value: "" });
                        let options = data.map(x => `<option value="${x.value}">${x.text}</option>`);
                        $("#ddlBranch").html(options.join(''));
                    },
                    error: function (xhr, status, error) {
                        alert("❌ Error loading branch by zone: " + xhr.responseText);
                    }
                });
            }

            function loadRegionByChannel(channelCode) {
                //alert(' region by channel ');

                $.ajax({
                    url: "/masters/sip_master1.aspx/GetRegionListByChannel",
                    method: "POST",
                    data: JSON.stringify({ channel: channelCode }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (res) {
                        let { data } = JSON.parse(res.d);
                        data.unshift({ text: "All", value: "" });
                        let options = data.map(x => `<option value="${x.value}">${x.text}</option>`);
                        $("#ddlRegion").html(options.join(''));
                    },
                    error: function (xhr, status, error) {
                        alert("❌ Error loading region by channel: " + xhr.responseText);
                    }
                });
            }

            function loadZoneByChannel(channelCode) {
                //alert(' zone by channel ');

                $.ajax({
                    url: "/masters/sip_master1.aspx/GetZoneListByChannel",
                    method: "POST",
                    data: JSON.stringify({ channel: channelCode }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (res) {
                        let { data } = JSON.parse(res.d);
                        data.unshift({ text: "All", value: "" });
                        let options = data.map(x => `<option value="${x.value}">${x.text}</option>`);
                        $("#ddlZone").html(options.join(''));
                    },
                    error: function (xhr, status, error) {
                        alert("❌ Error loading zone by channel: " + xhr.responseText);
                    }
                });
            }

            function loadZoneByRegion(regionCode) {
                //alert(' zone by region ');

                $.ajax({
                    url: "/masters/sip_master1.aspx/GetZoneListByRegion",
                    method: "POST",
                    data: JSON.stringify({ region: regionCode }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (res) {
                        let { data } = JSON.parse(res.d);
                        data.unshift({ text: "All", value: "" });
                        let options = data.map(x => `<option value="${x.value}">${x.text}</option>`);
                        $("#ddlZone").html(options.join(''));
                    },
                    error: function (xhr, status, error) {
                        alert("❌ Error loading zone by region: " + xhr.responseText);
                    }
                });
            }


            function loadRMListByBrnach(value) {
                //alert(' rm by branch ');

                $.ajax({
                    url: "/masters/sip_master1.aspx/GetRmListByBranch",
                    method: "POST",
                    data: JSON.stringify({ branchCode: value }), // 🔥 CORRECT HERE
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (res) {
                        let { data } = JSON.parse(res.d);
                        data.unshift({ text: "All", value: "" });
                        let options = data.map(x => `<option value="${x.value}">${x.text}</option>`);
                        $("#ddlRM").html(options.join(''));
                        //alert(data.length);
                    },
                    error: function (xhr, status, error) {
                        alert("❌ Error loading RM list: " + xhr.responseText);
                    }
                });

            }
            //#endregion ---------- HELPING FUNCTION: DDL DATA LOAD AJAX ----------

            //#region ---------- PAGE LOAD FUNCTIONS CALL ----------
            checkUserSession();
            const $menuIcon = $('.mdi-menu');       // The <span> you use as the button
            const $sidebar = $('#sidebar');         // The <nav> sidebar element
            const toggleClass = 'collapsed-sidebar'; // This is a custom class we’ll toggle

            // 1. Restore sidebar state from localStorage
            if (localStorage.getItem('sidebarState') === 'collapsed') {
                $sidebar.addClass(toggleClass);
                $menuIcon.addClass('active'); // Optional styling
            }

            // 2. On click of the <span class="mdi mdi-menu">
            $menuIcon.on('click', function () {
                $sidebar.toggleClass(toggleClass);
                $menuIcon.toggleClass('active'); // Optional styling

                // 3. Save state to localStorage
                if ($sidebar.hasClass(toggleClass)) {
                    localStorage.setItem('sidebarState', 'collapsed');
                } else {
                    localStorage.setItem('sidebarState', 'expanded');
                }
            });


            $(document).ajaxStart(function () {
                checkUserSession();
                showLoader();
            });


            $(document).ajaxStop(function () {
                hideLoader();
            });

            setCurrentDateToInput('txtDateFrom');
            setCurrentDateToInput('txtDateTo');

            loadChannelList();
            loadRegionList();
            loadBranchList();
            loadZoneList();
            loadAMCList();
            loadRMList();
            $('#tranRegular').prop('checked', true);
            enableAutoDateFormat("txtDateFrom");
            enableAutoDateFormat("txtDateTo");
            enableAutoDateFormat("txtRtaDateFrom");
            enableAutoDateFormat("txtRtaDateTo");
            makeTableColumnsResizable("transactionTable");
            makeTableColumnsResizable("rtaGrid");
            makeTableColumnsResizable("tranCodeGrid");
            syncScroll();
            //#endregion ---------- PAGE LOAD FUNCTIONS CALL ----------

            //#region ---------- SECTION 0.0: ONCHANGE TR ACTION BUTTON ----------

            $("#ddlChannel").change(function () {
                let channelCode = $(this).val();
                if (!channelCode || channelCode == null) {
                    loadRegionList();
                    loadZoneList();
                    loadBranchList();
                } else {
                    loadRegionByChannel(channelCode);
                    loadZoneByChannel(channelCode);
                    loadBranchByChannel(channelCode);
                }
                loadRMList();
            });

            $("#ddlRegion").change(function () {
                let value = $(this).val();

                if (!value || value === null) {
                    loadZoneList();
                    loadBranchList();
                } else {
                    loadZoneByRegion(value);
                }
                loadRMList();
            });

            $("#ddlZone").change(function () {
                let value = $(this).val();
                if (!value || value === null) {
                    loadBranchList();
                } else {
                    loadBranchByZone(value);
                }
                loadRMList();
            });

            $("#ddlBranch").change(function () {
                let value = $(this).val();

                if (!value || value === null) {
                    loadRMList();
                } else {
                    loadRMListByBrnach(value);
                }
            });

            //#endregion ---------- SECTON 0.0: ONCHANGE TR ACTION BUTTON ----------

            //#region ---------- SECTION 1.1: TR FIND HELPER FUNCTIONS ----------

            function loadTRList() {
                const channel = $('#ddlChannel').val() || '';
                const region = $('#ddlRegion').val() || '';
                const zone = $('#ddlZone').val() || '';
                const branch = $('#ddlBranch').val() || '';
                const rm = $('#ddlRM').val() || '';
                const amc = $('#ddlAMC').val() || '';
                const reconciliationStatus = $('input[name="reco"]:checked').val() || '';
                const cobFlag = $('#chkTrCOB').is(':checked') ? 'Y' : 'N';
                const tranType = $('input[name="tranType"]:checked').val() || '';
                const registrar = $('input[name="registrar"]:checked').val() || '';
                const arNo = ($('#txtARNo').val() || '').trim();
                const dateFrom = !arNo ? $('#txtDateFrom').val() : '';
                const dateTo = !arNo ? $('#txtDateTo').val() : '';
                const isPms = $('#chkPMS').is(':checked') ? 'Y' : 'N';
                const isOp = $('input[name="rdbOp"]:checked').val() || '';



                // '01/06/2025' '05/06/2025'
                alert(`Transaction Filter Values:\n\n` + `Channel: ${channel}\n` + `Region: ${region}\n` +
                    `Zone: ${zone}\n` + `Branch: ${branch}\n` + `RM: ${rm}\n` + `Date From: ${dateFrom}\n` +
                    `Date To: ${dateTo}\n` + `AMC: ${amc}\n` + `Reconciliation Status: ${reconciliationStatus}\n` +
                    `COB Flag: ${cobFlag}\n` + `Tran Type: ${tranType}\n` + `Registrar: ${registrar}\n` + `AR No: ${arNo}\n` + 
                    `PMS Flag: ${isPms}\n` + `OP Flag : ${isOp}\n`

                
                );
                //return;
                if (!arNo && (!dateFrom || !dateTo)) {
                    alert("⚠️ Please enter AR No or both From Date and To Date.");
                    return; // stop execution
                }

                $.ajax({
                    url: "/masters/sip_master1.aspx/GetTRList",
                    method: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSON.stringify({
                        channel, region, zone, branch, rm,
                        dateFrom, dateTo, amc, arNo,
                        reconciliationStatus, cobFlag,
                        tranType, registrar, isPms, isOp
                    }),
                    success: function (res) {
                        let { data } = JSON.parse(res.d);

                        if (!data || data.length === 0) {
                            $("#transactionTable tbody").html(`
        <tr><td colspan="38" class="text-center text-danger">No records found</td></tr>
    `);
                            return;
                        }

                        let rows = data.map(tr => `
    <tr>
        <td>${tr.TRAN_CODE || ''}</td>
        <td>${formatDateToDMY(tr.TR_DATE)}</td>
        <td>${tr.INVESTOR_NAME || ''}</td>
        <td>${tr.ADDRESS1 || ''}</td>
        <td>${tr.CITY_NAME || ''}</td>
        <td>${tr.MUT_NAME || ''}</td>
        <td style="width:400px;">${tr.SCH_NAME || ''}</td>
        <td>${tr.AMOUNT || ''}</td>
        <td>${tr.FOLIO_NO || ''}</td>
        <td>${tr.CHEQUE_NO || ''}</td>
        <td>${tr.APP_NO || ''}</td>
        <td>${tr.RM_NAME || ''}</td>
        <td>${tr.BRANCH_NAME || ''}</td>
        <td>${tr.TRAN_TYPE || ''}</td>
        <td>${tr.SIP_TYPE || ''}</td>
        <td>${tr.LOGGEDUSER || ''}</td>
        <td>${tr.REMARK || ''}</td>
        <td style="display:none;" >${tr.BANK_NAME || ''}</td>
        <td style="display:none;" >${tr.RM_CODE || ''}</td>
        <td style="display:none;" >${tr.PANNO || ''}</td>
        <td style="display:none;" >${tr.CHEQUE_DATE || ''}</td>
        <td style="display:none;" >${tr.BROKER_ID || ''}</td>
        <td style="display:none;" >${tr.SIP_AMOUNT || ''}</td>
        <td style="display:none;" >${tr.REGISTRAR || ''}</td>
        <td style="display:none;" >${tr.SOURCE_CODE || ''}</td>
        <td style="display:none;" >${tr.DISPATCH || ''}</td>
        <td style="display:none;" >${tr.COB_FLAG || ''}</td>
        <td style="display:none;" >${tr.CLIENT_CODE || ''}</td>
        <td style="display:none;" >${tr.SCH_CODE || ''}</td>
        <td style="display:none;" >${tr.MUT_CODE || ''}</td>
        <td style="display:none;" >${tr.PAYMENT_MODE || ''}</td>
        <td style="display:none;" >${tr.LEAD_NO || ''}</td>
        <td style="display:none;" >${tr.LEAD_NAME || ''}</td>
        <td style="display:none;" >${tr.BRANCH_CODE || ''}</td>
        <td style="display:none;" >${tr.BUSINESS_RMCODE || ''}</td>
    </tr>
`).join('');

                        $("#transactionTable tbody").html(rows);
                        $("#lblTrCount").text(`${data.length}`);
                    },
                    error: function (xhr, status, error) {
                        $("#transactionTable tbody").html(`
                    <tr><td colspan="38" class="text-center text-danger">Error loading data</td></tr>
                `);

                        $("#lblTrCount").text(`0`);

                        alert("Error: " + xhr.responseText);
                    }
                });
            }


            function loadTRListX(x) {
                const branchCat = $('#ddlChannel').val() || '';
                const region = $('#ddlRegion').val() || '';
                const zone = $('#ddlZone').val() || '';
                const branch = $('#ddlBranch').val() || '';
                const rm = $('#ddlRM').val() || '';
                const amc = $('#ddlAMC').val() || '';
                const tranType = $('input[name="tranType"]:checked').val() || '';
                const registrar = $('input[name="registrar"]:checked').val() || '';
                const statusType = $('input[name="reco"]:checked').val() || '';
                const pms = $('#chkPMS').is(':checked') ? 'Y' : 'N';
                const cob = $('#chkTrCOB').is(':checked') ? 'Y' : 'N';
                const dateFrom = $('#txtDateFrom').val() || '';
                const dateTo = $('#txtDateTo').val() || '';
                const arNum = ($('#txtARNo').val() || '').trim();
                const chequeType = $('#ddlChequeType').val() || '';
                const chequeSearch = ($('#txtChequeSearch').val() || '').trim();
                const investorName = ($('#txtInvestorName').val() || '').trim();
                const amount = ($('#txtAmount').val() || '').trim();
                const sipFolioNo = ($('#txtSIPFolioNo').val() || '').trim();
                const sipAmount = ($('#txtSIPAmount').val() || '').trim();
                const sipPan = ($('#txtSIPPan').val() || '').trim();
                const sipClientCode = ($('#txtSIPClientCode').val() || '').trim();
                const sipDate = $('#txtSIPDate').val() || '';

                // Validation - either AR Number or Date range should be provided
                if (!arNum && (!dateFrom || !dateTo)) {
                    alert("⚠️ Please enter AR No or both From Date and To Date.");
                    return;
                }

                // Debug alert (optional)
                alert(`Transaction Filter Values:\n\n` +
                    `X: ${x}\n` + `Branch Category: ${branchCat}\n` + `Region: ${region}\n` +
                    `Zone: ${zone}\n` + `Branch: ${branch}\n` + `RM: ${rm}\n` +
                    `AMC: ${amc}\n` + `Transaction Type: ${tranType}\n` + `Registrar: ${registrar}\n` +
                    `Status Type: ${statusType}\n` + `PMS: ${pms}\n` + `COB: ${cob}\n` +
                    `Date From: ${dateFrom}\n` + `Date To: ${dateTo}\n` + `AR Number: ${arNum}\n` +
                    `Cheque Type: ${chequeType}\n` + `Cheque Search: ${chequeSearch}\n` +
                    `Investor Name: ${investorName}\n` + `Amount: ${amount}\n` +
                    `SIP Folio No: ${sipFolioNo}\n` + `SIP Amount: ${sipAmount}\n` +
                    `SIP PAN: ${sipPan}\n` + `SIP Client Code: ${sipClientCode}\n` +
                    `SIP Date: ${sipDate}\n` + `Log ID: ${logId}\n` + `Role ID: ${roleId}`
                );

                $.ajax({
                    url: "/masters/sip_master1.aspx/GetTRListX",
                    method: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSON.stringify({
                        x: x,
                        branchCat: branchCat,
                        region: region,
                        zone: zone,
                        branch: branch,
                        rm: rm,
                        amc: amc,
                        tranType: tranType,
                        registrar: registrar,
                        statusType: statusType,
                        pms: pms,
                        cob: cob,
                        dateFrom: dateFrom,
                        dateTo: dateTo,
                        arNum: arNum,
                        chequeType: chequeType,
                        chequeSearch: chequeSearch,
                        investorName: investorName,
                        amount: amount,
                        sipFolioNo: sipFolioNo,
                        sipAmount: sipAmount,
                        sipPan: sipPan,
                        sipClientCode: sipClientCode,
                        sipDate: sipDate,
                        logId: logId,
                        roleId: roleId
                    }),
                    success: function (res) {
                        let { data } = JSON.parse(res.d);

                        if (!data || data.length === 0) {
                            alert('No records found');
                            return;
                        } else {
                            alert(data.length +' Record(s) found, for x = ' + x);
                            return;
                        }

                        
                    },
                    error: function (xhr, status, error) {                        
                        alert("Error: " + xhr.responseText);
                    }
                });
            }


            function setRtaTranTypeByTrSipType(sipVal) {
                const normalizedSip = (sipVal || "").toLowerCase() === "regular" ? "REGULAR" : "SIP";

                alert('TR VALUE ' + normalizedSip);
                // Find radio input by name and value (case-sensitive matching)
                const $radioButton = $(`input[name='rtaTranType'][value='${normalizedSip}']`);

                alert('RTA VALUE ' + $radioButton);

                if ($radioButton.length) {
                    $radioButton.prop("checked", true);
                } else {
                }
            }

            function setRtaRecoStatus(tranType) {
                const recoVal = (tranType || "").toLowerCase() === "reconciled" ? "Y" : "N";

                const $radio = $(`input[name='rtaRecoStatus'][value='${recoVal}']`);

                if ($radio.length) {
                    $radio.prop("checked", true);
                } else {
                }
            }

            function setTrRegistrarByTr(registrar, cobfl) {
                let registrarVal = "";

                if (registrar === "C" && cobfl === "0") {
                    registrarVal = "c";
                } else if (registrar === "K" && cobfl === "0") {
                    registrarVal = "k";
                } else if (registrar === "C" && cobfl === "1") {
                    registrarVal = "ccob";
                } else if (registrar === "K" && cobfl === "1") {
                    registrarVal = "kcob";
                }

                if (registrarVal) {
                    $('input[name="registrar"][value="' + registrarVal + '"]').prop('checked', true);
                }
            }

            function setTranAmtCheqFolAppPanBrkByTr(trCode, trAmount, chequeNo, folioNo, appNo, panNo, brokerId, dispatch) {
                $('#hdnTrCode').text(trCode);
                $('#hdnTrAmount').text(trAmount);
                $('#hdnTrChe').text(chequeNo);
                $('#hdnTrFolio').text(folioNo);
                $('#hdnTrApp').text(appNo);
                $('#hdnTrPan').text(panNo);
                $('#hdnTrBroker').text(brokerId);
                $('#hdnTrDispatch').text(dispatch);

            }

            function setHiddenRtaValues(rtaCode, rtaAmount, rtaDate, rtaFolio) {
                $('#hdnRtaCode').text(rtaCode.trim());
                $('#hdnRtaAmount').text(rtaAmount.trim());
                $('#hdnRtaDate').text(rtaDate.trim());
                $('#hdnRtaFolio').text(rtaFolio.trim());
            }

            function setRtaFromToDatesByTrDate(tranDateStr) {
                if (/^\d{2}\/\d{2}\/\d{4}$/.test(tranDateStr)) {
                    const [dd, mm, yyyy] = tranDateStr.split('/');
                    const originalDate = new Date(`${yyyy}-${mm}-${dd}`);

                    if (!isNaN(originalDate)) {
                        const before = new Date(originalDate);
                        before.setMonth(before.getMonth() - 1);

                        const after = new Date(originalDate);
                        after.setMonth(after.getMonth() + 1);

                        const format = (d) => ("0" + d.getDate()).slice(-2) + "/" + ("0" + (d.getMonth() + 1)).slice(-2) + "/" + d.getFullYear();

                        $('#txtRtaDateFrom').val(format(before));
                        $('#txtRtaDateTo').val(format(after));
                    } else {
                        $('#txtRtaDateFrom').val('');
                        $('#txtRtaDateTo').val('');
                    }
                } else {
                    $('#txtRtaDateFrom').val('');
                    $('#txtRtaDateTo').val('');
                }
            }

            function setRtaAmcBrStsTranInvAmtRemByTr({
                amc,
                branch,
                status,
                tranType,
                investorName,
                amount,
                remarks
            }) {

                $('#ddlRtaAMC').val(amc || '');
                $('#ddlRtaBranch').val(branch || '');

                if (status) {
                    $(`input[name="status"][value="${status}"]`).prop('checked', true);
                }

                if (tranType) {
                    $(`input[name="tranType"][value="${tranType}"]`).prop('checked', true);
                }

                $('#txtRtaInvestorName').val(investorName || '');
                $('#txtRtaAmount').val((amount || '').replace(/[^0-9.]/g, ''));

                $('#txtRtaRemarks').val(remarks || '');
            }


            function clearRtaFilterForm() {

                $('#rtaGrid tbody').empty();

                // Clear date inputs
                $('#txtRtaDateFrom').val('');
                $('#txtRtaDateTo').val('');
                $('#txtRtaInvestorName').val('');
                $('#txtRtaAmount').val('');
                $('#txtRtaRemarks').val('');


                // Reset dropdowns to no selection (index -1)
                $('#ddlRtaAMC').prop('selectedIndex', -1);
                $('#ddlRtaBranch').prop('selectedIndex', -1);
                $('#ddlRtaCheque').prop('selectedIndex', -1);
                // Reset and set default radio selections
                $('input[name="rtaRecoStatus"]').prop('checked', false);
                $('input[name="rtaRecoStatus"][value="N"]').prop('checked', true); // Set "Unreconciled" checked

                $('input[name="rtaTranType"]').prop('checked', false);
                $('input[name="rtaTranType"][value="REGULAR"]').prop('checked', true); // Set "Regular" checked

                // Clear hidden span values
                $('#hdnTrCode, #hdnTrAmount, #hdnTrChe, #hdnTrFolio, #hdnTrApp, #hdnTrPan, #hdnTrBroker, #hdnRtaCode, #hdnRtaAmount, #hdnRtaFolio, #hdnRtaDate, #hdnTrDispatch ').text('');
                $('#txtRtaChequeNo').val('');
            }
            //#endregion ---------- SECTION 1.1: TR FIND HELPER FUNCTIONS ----------

            //#region ---------- SECTION 1.2: TR FIND ACTION BUTTONS ----------


            $("#btnTrFind").click(function () {
                clearRtaFilterForm();
                loadTRList();
            });

            $('#transactionTable tbody').on('click', 'tr', function () {
                $('#transactionTable tbody tr').removeClass('row-selected');
                $(this).addClass('row-selected');

                const headers = [];
                $('#transactionTable thead th').each(function () {
                    headers.push($(this).text().trim());
                });

                let rowObj = {};
                $(this).find('td').each(function (index) {
                    const key = headers[index];
                    const value = $(this).text().trim();
                    rowObj[key] = value;

                    const spanId = `rowData_${key.replace(/\s+/g, '_')}`;
                    const $existing = $(`#${spanId}`);
                    if ($existing.length) {
                        $existing.text(value);
                    } else {
                        $('<span>', {
                            id: spanId,
                            class: 'hidden-row-data',
                            text: value,
                            style: 'display:none;'
                        }).appendTo('body'); // Or any container you prefer
                    }
                });

                //#region Now you can access named variables like:
                const TRAN_CODE = rowObj["TRAN_CODE"];
                const TR_DATE = rowObj["TR_DATE"];
                const INVESTOR_NAME = rowObj["INVESTOR_NAME"];
                const ADDRESS1 = rowObj["ADDRESS"];
                const CITY_NAME = rowObj["CITY_NAME"];
                const MUT_NAME = rowObj["AMC_NAME"];
                const SCH_NAME = rowObj["SCH_NAME"];
                const AMOUNT = rowObj["AMOUNT"];
                const BANK_NAME = rowObj["BANK_NAME"];
                const RM_NAME = rowObj["RM_NAME"];
                const BRANCH_NAME = rowObj["BRANCH_NAME"];
                const RM_CODE = rowObj["RM_CODE"];
                const PANNO = rowObj["PANNO"];
                const CHEQUE_NO = rowObj["CHEQUE_NO"];
                const CHEQUE_DATE = rowObj["CHEQUE_DATE"];
                const FOLIO_NO = rowObj["FOLIO_NO"];
                const BROKER_ID = rowObj["BROKER_ID"];
                const SIP_AMOUNT = rowObj["SIP_AMOUNT"];
                const REGISTRAR = rowObj["REGISTRAR"];
                const SOURCE_CODE = rowObj["SOURCE_CODE"];
                const DISPATCH = rowObj["DISPATCH"];
                const COB_FLAG = rowObj["COB_FLAG"];
                const CLIENT_CODE = rowObj["CLIENT_CODE"];
                const SCH_CODE = rowObj["SCH_CODE"];
                const MUT_CODE = rowObj["MUT_CODE"];
                const TRAN_TYPE = rowObj["TRAN_TYPE"];
                const APP_NO = rowObj["APP_NO"];
                const PAYMENT_MODE = rowObj["PAYMENT_MODE"];
                const SIP_TYPE = rowObj["SIP_TYPE"];
                const LEAD_NO = rowObj["LEAD_NO"];
                const LEAD_NAME = rowObj["LEAD_NAME"];
                const BRANCH_CODE = rowObj["BRANCH_CODE"];
                const BUSINESS_RMCODE = rowObj["BUSINESS_RMCODE"];
                const REMARK = rowObj["REMARK"];
                const LOGGEDUSER = rowObj["LOGGEDUSER"];
                //#endregion Now you can access named variables like:


                clearRtaFilterForm();

                setTrRegistrarByTr(REGISTRAR, COB_FLAG);

                setTranAmtCheqFolAppPanBrkByTr(
                    rowObj.TRAN_CODE,
                    rowObj.AMOUNT,
                    rowObj.CHEQUE_NO,
                    rowObj.FOLIO_NO,
                    rowObj.APP_NO,
                    rowObj.PANNO,
                    rowObj.BROKER_ID,
                    rowObj.DISPATCH
                );

                setRtaFromToDatesByTrDate(rowObj.TR_DATE);

                setRtaAmcBrStsTranInvAmtRemByTr({
                    amc: rowObj.MUT_CODE,
                    branch: rowObj.BRANCH_CODE,
                    status: 'N',
                    tranType: 'REGULAR',
                    investorName: rowObj.INVESTOR_NAME,
                    amount: rowObj.AMOUNT,
                    remarks: rowObj.REMARK
                });

                setRtaTranTypeByTrSipType(rowObj.SIP_TYPE);

                setRtaRecoStatus(rowObj.TRAN_TYPE);

                $('#ddlRtaCheque').prop('selectedIndex', 4);
                $('#txtRtaChequeNo').val(PANNO);

                let rowDetails = Object.entries(rowObj)
                    .map(([key, value]) => `${key}: ${value}`)
                    .join('\n');

                //alert("Clicked Row Data:\n" + rowDetails);
            });

            $("#btnTrReset").click(function () {
                window.location.href = '/Masters/sip_master1.aspx';
            });

            $("#btnTrExport").click(function () {
                ExportTableToExcel_1("transactionTable", "TR");
            });

            $("#btnTrExit").click(function () {
                RedirectToWelcomePage();
            });

            //#endregion ---------- SECTION 1.2: TR FIND ACTION BUTTONS ----------

            //#region ---------- SECTION 2.1: RTA DATA FIND HELPER ----------

            function highlightMatchingRTAColumns() {
                const columnMap = {
                    "TRAN_CODE": "TRAN_CODE",
                    "TR_DATE": "TRAN_DATE",
                    "INVESTOR_NAME": "INVESTOR_NAME",
                    "ADDRESS": "ADDRESS",
                    "CITY_NAME": "CITY_NAME",
                    "AMC_NAME": "AMC_NAME",
                    "SCH_NAME": "SCHEME_NAME",
                    "AMOUNT": "AMOUNT",
                    "FOLIO_NO": "FOLIO_NO",
                    "CHEQUE_NO": "CHEQUE_NO",
                    "APP_NO": "APP_NO",
                    "RM_NAME": "RM_NAME",
                    "BRANCH_NAME": "BRANCH_NAME"
                };

                // Get headers from transactionTable
                const tranHeaders = [];
                $('#transactionTable thead th').each(function () {
                    tranHeaders.push($(this).text().trim().toUpperCase());
                });

                const $selectedRow = $('#transactionTable tbody tr.row-selected');
                if ($selectedRow.length === 0) {
                    alert('No selected row found in transactionTable!');
                    return;
                }

                // Extract selected row values
                const selectedData = {};
                $selectedRow.find('td').each(function (index) {
                    const key = tranHeaders[index];
                    const value = $(this).text().trim().toLowerCase();
                    selectedData[key] = value;
                });

                // RTA grid headers
                const rtaHeaders = [];
                $('#rtaGrid thead th').each(function () {
                    rtaHeaders.push($(this).text().trim().toUpperCase());
                });

                // Compare and highlight
                $('#rtaGrid tbody tr').each(function () {
                    $(this).find('td').each((index, cell) => {
                        const rtaKey = rtaHeaders[index];
                        const tranKey = Object.keys(columnMap).find(k => columnMap[k] === rtaKey);

                        if (tranKey && selectedData[tranKey] !== undefined) {
                            const rtaValue = $(cell).text().trim().toLowerCase();
                            const tranValue = selectedData[tranKey];

                            if (rtaValue === tranValue) {
                                // Exact match – Strong green
                                $(cell).css('background-color', '#28a745').css('color', '');
                            } else if (isPartialMatch(tranValue, rtaValue)) {
                                // Partial match – Light green
                                //$(cell).css('background-color', '#a9dfbf');
                            } else {
                                // No match – Clear background
                                $(cell).css('background-color', '').css('color', '');
                            }
                        }
                    });
                });

                function isPartialMatch(text1, text2) {
                    if (!text1 || !text2) return false;

                    const words1 = text1.split(/\s+/);
                    const words2 = text2.split(/\s+/);

                    let matchCount = 0;

                    for (const word1 of words1) {
                        if (words2.includes(word1)) {
                            matchCount++;
                        }
                    }

                    const matchRatio = matchCount / Math.max(words1.length, words2.length);
                    return matchRatio >= 0.5;
                }
            }

            function loadRTAList() {

                const rtaDtFrom = $('#txtRtaDateFrom').val() || '';
                const rtaDtTo = $('#txtRtaDateTo').val() || '';
                const rtaRecoStatus = $('input[name="rtaRecoStatus"]:checked').val() || '';
                const rtaAmc = $('#ddlRtaAMC').val() || '';
                const rtaBranch = $('#ddlRtaBranch').val() || '';
                const rtaCheqType = $('#ddlRtaCheque').val() || '';
                const rtaCheqValue = ($('#txtRtaChequeNo').val() || '').trim();
                const rtaInvName = ($('#txtRtaInvestorName').val() || '').trim();
                const rtaAmount = ($('#txtRtaAmount').val() || '').trim();
                const rtaTranType = $('input[name="rtaTranType"]:checked').val() || '';
                const rtaFindText = ($('#txtSearching').val() || '').trim();
                const trTranType = $('input[name="tranType"]:checked').val() || '';
                const trRegistrar = $('input[name="registrar"]:checked').val() || '';


                $.ajax({
                    url: "/masters/sip_master1.aspx/GetRTAList",
                    method: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSON.stringify({
                        rtaDtFrom, rtaDtTo, rtaRecoStatus, rtaAmc, rtaBranch,
                        rtaCheqType, rtaCheqValue, rtaInvName, rtaAmount,
                        rtaTranType, rtaFindText, trTranType, trRegistrar
                    }),
                    success: function (res) {
                        let { data } = JSON.parse(res.d);

                        if (!data || data.length === 0) {
                            $("#rtaGrid tbody").html(`
         <tr><td colspan="16" class="text-center text-danger">No records found</td></tr>
     `);
                            return;
                        }

                        let rows = data.map(rta => `
     <tr>
        <td>${rta.UNIQUE_TRAN || ''}</td>
         <td>${formatDateToDMY(rta.TR_DATE)}</td>
         <td>${rta.INVESTOR_NAME || ''}</td>
         <td>${rta.ADDRESS || ''}</td>
         <td>${rta.CITY_NAME || ''}</td>
         <td>${rta.MUT_NAME || ''}</td>
         <td style="width:400px;">${rta.SCH_NAME || ''}</td>
         <td>${rta.AMOUNT || ''}</td>
         <td>${rta.FOLIO_NO || ''}</td>
         <td>${rta.CHEQUE_NO || ''}</td>
         <td>${rta.APP_NO || ''}</td>
         <td>${rta.RM_NAME || ''}</td>
         <td>${rta.BRANCH_NAME || ''}</td>
         <td>${rta.BROKER_CODE || ''}</td>
         <td>${rta.REG_TRANTYPE || ''}</td>
         <td>${rta.UNQ_KEY || ''}</td>
     </tr>
 `).join('');

                        $("#rtaGrid tbody").html(rows);
                        highlightMatchingRTAColumns();

                    },
                    error: function (xhr, status, error) {
                        $("#rtaGrid tbody").html(`
     <tr><td colspan="16" class="text-center text-danger">Error loading data</td></tr>
 `);
                        $("#lblRTACount").text(`0`);
                        alert("Error: " + xhr.responseText);
                    }
                });
            }

            function onRtaSaveRemark(trCodeValue, txtRemarkValue) {
                $.ajax({
                    url: "/masters/sip_master1.aspx/SaveRemark",
                    method: "POST",
                    data: JSON.stringify({
                        trCode: trCodeValue,
                        txtRemark: txtRemarkValue
                    }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (res) {
                        const result = JSON.parse(res.d);
                        alert(result.success ? `✅ ${result.message}` : `✅ ${result.message}`);

                    },
                    error: function (xhr, status, error) {
                        alert(`❌ Error saving remark: ${error}`);
                    }
                });
            }

            function onConfirmPMSATM(trCode, remark, pmsStatus, atmStatus) {
                //alert('Initiating PMS/ATM confirmation for:', trCode);
                $.ajax({
                    type: "POST",
                    url: "/masters/sip_master1.aspx/RtaConfirmPMS",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSON.stringify({
                        trCode: trCode,
                        remark: remark,
                        pmsStatus: pmsStatus,
                        atmStatus: atmStatus
                    }),
                    success: function (res) {
                        const result = JSON.parse(res.d);
                        alert(result.success ? `✅ ${result.message}` : `✅ ${result.message}`);
                    },
                    error: function (xhr) {
                        let errorMsg = "Server error occurred";
                        try {
                            const errResponse = JSON.parse(xhr.responseText);
                            if (errResponse.Message) {
                                errorMsg = errResponse.Message;
                            }
                        } catch (e) {
                            errorMsg = xhr.statusText || "Unknown error";
                        }

                        alert(`❌ Error: ${errorMsg}`);

                    }
                });
            }

            function onUnconfirmPMSATM(trCode, remark, pmsStatus, atmStatus) {
                $.ajax({
                    type: "POST",
                    url: "/masters/sip_master1.aspx/RtaUnConfirmPMS",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSON.stringify({
                        trCode: trCode,
                        remark: remark,
                        pmsStatus: pmsStatus,
                        atmStatus: atmStatus
                    }),
                    success: function (res) {
                        const result = JSON.parse(res.d);
                        if (result.success) {
                            alert(`✅ ${result.message}`);
                        } else {
                            alert(`✅ ${result.message}`);
                        }
                    },
                    error: function (xhr) {
                        let errorMsg = "Unconfirmation failed - server error";
                        try {
                            const errResponse = JSON.parse(xhr.responseText);
                            errorMsg = errResponse.Message ||
                                errResponse.message ||
                                "Unconfirmation could not be completed";
                        } catch (e) {
                            errorMsg = xhr.statusText || "Unknown unconfirmation error";
                        }

                        alert(`❌ Error: ${errorMsg}`);
                    }
                });
            }

            function onReconcileTransaction(trCode, rtaAmount, rtaCode, rtaDate, rtaFolio, dispatch, trType) {
                $.ajax({
                    type: "POST",
                    url: "/masters/sip_master1.aspx/ReconcileTransactions",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSON.stringify({
                        trTranCodeValue: trCode,
                        trTranTypeValue: trType,
                        rtaTranCodeValue: rtaCode,
                        rtaTranDateValue: rtaDate,
                        rtaFolioValue: rtaFolio,
                        rtaAmountValue: rtaAmount,
                        rtaDispatchValue: dispatch
                    }),
                    success: function (res) {
                        const result = JSON.parse(res.d);
                        alert(result.success ? `✅ ${result.message}` : `❌ ${result.message}`);
                        if (result.success) {
                            // Optional: Refresh the grid or perform other success actions
                            //location.reload();
                        }
                    },
                    error: function (xhr) {
                        alert("❌ Error: " + (xhr.responseJSON && xhr.responseJSON.Message || xhr.responseText));
                    }
                });
            }

            //#endregion ---------- SECTION 2.1: RTA DATA FIND HELPER  ----------

            //#region ---------- SECTION 2.2: RTA DATA FIND ACTION ----------

            $('#ddlRtaCheque').on('change', function () {
                const selectedCode = $(this).val();
                let valueToSet = '';

                switch (selectedCode) {
                    case '001':
                        valueToSet = $('#hdnTrChe').text();
                        break;
                    case '002':
                        valueToSet = $('#hdnTrFolio').text();
                        break;
                    case '003':
                        valueToSet = $('#hdnTrApp').text();
                        break;
                    case '004':
                        valueToSet = $('#hdnTrPan').text();
                        break;
                    case '005':
                        valueToSet = $('#hdnTrBroker').text();
                        break;
                    default:
                        valueToSet = '';
                }

                $('#txtRtaChequeNo').val(valueToSet);
            });


            $('#btnRtaFind').click(function () {
                const rtaDtFrom = $('#txtRtaDateFrom').val() || '';
                const rtaDtTo = $('#txtRtaDateTo').val() || '';
                const rtaRecoStatus = $('input[name="rtaRecoStatus"]:checked').val() || '';
                const rtaAmc = $('#ddlRtaAMC').val() || '';
                const rtaBranch = $('#ddlRtaBranch').val() || '';
                const rtaCheqType = $('#ddlRtaCheque').val() || '';
                const rtaCheqValue = ($('#txtRtaChequeNo').val() || '').trim();
                const rtaInvName = ($('#txtRtaInvestorName').val() || '').trim();
                const rtaAmount = ($('#txtRtaAmount').val() || '').trim();
                const rtaTranType = $('input[name="rtaTranType"]:checked').val() || '';
                const rtaFindText = ($('#txtSearching').val() || '').trim();
                const trTranType = $('input[name="tranType"]:checked').val() || '';
                const trRegistrar = $('input[name="registrar"]:checked').val() || '';

                if (!rtaDtFrom && !rtaDtTo && !rtaCheqValue) {
                    alert("⚠️ Please fill at least one filter to proceed RTA find.");
                    return false;
                } else {
                    loadRTAList();
                }
            });

            $('#rtaGrid tbody').on('click', 'tr', function () {

                $('#rtaGrid tbody tr').removeClass('row-selected');
                $(this).addClass('row-selected');

                const headers = [];
                $('#rtaGrid thead th').each(function () {
                    headers.push($(this).text().trim());
                });


                let rowObj = {};
                $(this).find('td').each(function (index) {
                    const key = headers[index];
                    const value = $(this).text().trim().replace(/'/g, ''); // Remove single quotes
                    rowObj[key] = value;

                    const spanId = `rta_rowData_${key.replace(/\s+/g, '_')}`;
                    const $existing = $(`#${spanId}`);
                    if ($existing.length) {
                        $existing.text(value);
                    } else {
                        $('<span>', {
                            id: spanId,
                            class: 'hidden-row-data',
                            text: value,
                            style: 'display:none;'
                        }).appendTo('body');
                    }
                });

                const TRAN_CODE = rowObj["TRAN_CODE"];
                const TRAN_DATE = rowObj["TRAN_DATE"];
                const INVESTOR_NAME = rowObj["INVESTOR_NAME"];
                const AMOUNT = rowObj["AMOUNT"];
                const AMX_NAME = rowObj["AMC_NAME"];
                const SCHEME_NAME = rowObj["SCHEME_NAME"];
                const ADDRESS = rowObj["ADDRESS"];
                const CITY_NAME = rowObj["CITY_NAME"];
                const FOLIO_NO = rowObj["FOLIO_NO"];
                const CHEQUE_NO = rowObj["CHEQUE_NO"];
                const APP_NO = rowObj["APP_NO"];
                const RM_NAME = rowObj["RM_NAME"];
                const BRANCH_NAME = rowObj["BRANCH_NAME"];
                const BROKER_CODE = rowObj["BROKER_CODE"];
                const REG_TRAN_TYPE = rowObj["REG_TRAN_TYPE"];
                const UNIQUE_KEY = rowObj["UNIQUE_KEY"];

                setHiddenRtaValues(TRAN_CODE, AMOUNT, TRAN_DATE, FOLIO_NO);

                const hiddenCode = $('#hdnRtaCode').text().trim();
                const hiddenAmount = $('#hdnRtaAmount').text().trim();

                //alert(`Hidden RTA Code: ${hiddenCode}\nHidden RTA Amount: ${hiddenAmount}`);
                let rowDetails = Object.entries(rowObj)
                    .map(([key, value]) => `${key}: ${value}`)
                    .join('\n');

                //alert("Clicked Row Data:\n" + rowDetails);
            });

            $('#btnRtaReset').click(function () {
                clearRtaFilterForm();
            });

            $("#btnRtaExport").click(function () {
                ExportTableToExcel_1("rtaGrid", "RTA");
            });

            $('#btnRtaSaveRemark').click(function () {
                const trCode = $('#hdnTrCode').text().trim();
                const txtRemark = $('#txtRtaRemarks').val().trim();
                if (!trCode || !txtRemark) {
                    alert('⚠️ Transaction Code and Remark are required to save the remark.');
                    return;
                }
                onRtaSaveRemark(trCode, txtRemark);
            });

            $('#btnRtaConfPMS').click(function () {
                const trCode = $('#hdnTrCode').text().trim();
                const txtRemark = $('#txtRtaRemarks').val().trim();
                const selectedTranType = $('input[name="tranType"]:checked').val();

                // Determine confirmation type
                const isPMS = (selectedTranType === 'pms');
                const isATM = (selectedTranType === 'atm');

                // Validation
                if (!trCode) {
                    alert('⚠️ Please select a transaction record first');
                    return;
                }

                if (isPMS && !txtRemark) {
                    alert('⚠️ Please enter remarks for PMS confirmation');
                    $('#txtRtaRemarks').focus();
                    return;
                }

                if (!isPMS && !isATM) {
                    alert('⚠️ Please select either PMS or ATM transaction type');
                    return;
                }

                // Confirmation dialog
                if (confirm(`Confirm this transaction as ${isPMS ? 'PMS' : 'ATM'} type?`)) {
                    onConfirmPMSATM(trCode, txtRemark, isPMS, isATM);
                }
            });

            $('#btnRtaUncomfPMS').click(function () {
                const trCode = $('#hdnTrCode').text().trim();
                const txtRemark = $('#txtRtaRemarks').val().trim();
                const selectedTranType = $('input[name="tranType"]:checked').val();

                const isPMS = (selectedTranType === 'pms');
                const isATM = (selectedTranType === 'atm');

                if (!trCode) {
                    alert('⚠️ Please select a transaction record first');
                    return;
                }

                if (isPMS && !txtRemark) {
                    alert('⚠️ Please enter remarks for PMS confirmation');
                    $('#txtRtaRemarks').focus();
                    return;
                }

                if (!isPMS && !isATM) {
                    alert('⚠️ Please select either PMS or ATM transaction type');
                    return;
                }
                //alert(`ALL DATA TR: ${trCode}, REMARK: ${txtRemark}, PSM: ${isPMS}, ATM: ${isATM}`)


                // Confirmation dialog
                if (confirm(`Are you sure you want to unconfirm this ${isPMS ? 'PMS' : 'ATM'} transaction?`)) {
                    onUnconfirmPMSATM(trCode, txtRemark, isPMS, isATM);
                }
            });

            $('#btnRtaReconsile').click(function () {
                const trCode = $('#hdnTrCode').text().trim();
                const trTranType = $('input[name="tranType"]:checked').val() || '';
                const trDispatch = $('#hdnTrDispatch').text().trim();
                const rtaCode = $('#hdnRtaCode').text().trim();
                const rtaDate = $('#hdnRtaDate').text().trim();
                const rtaFolio = $('#hdnRtaFolio').text().trim();
                const rtaAmount = $('#hdnRtaAmount').text().trim();

                let alertMsg = '';
                alertMsg += `trCode       : ${trCode}\n`;
                alertMsg += `trTranType   : ${trTranType}\n`;
                alertMsg += `trDispatch   : ${trDispatch}\n`;
                alertMsg += `rtaCode      : ${rtaCode}\n`;
                alertMsg += `rtaDate      : ${rtaDate}\n`;
                alertMsg += `rtaFolio     : ${rtaFolio}\n`;
                alertMsg += `rtaAmount    : ${rtaAmount}\n`;
                //alert(alertMsg);
                //return;

                if (!trCode || !rtaCode || !rtaAmount) {
                    alert("All fields (Transaction TR Code, RTA Code, and RTA Amount) are required for reconciliation.");
                    return;
                } else {
                    onReconcileTransaction(
                        trCode,      // trCode
                        rtaAmount,   // rtaAmount
                        rtaCode,     // rtaCode
                        rtaDate,     // rtaDate
                        rtaFolio,    // rtaFolio
                        trDispatch,  // dispatch
                        trTranType   // trType
                    );
                }
            });

            // desable not in use
            $('#btnRtaFindByTr').click(function () {
                alert('btnRtaFind By TR clicked (demo)');
            });
            //#endregion ---------- SECTION 2.2: RTA DATA FIND ACTION ----------

            //#region ---------- SECTION 3: RECONCILED TRAN DATA FIND HELPER & ACTION ----------
            function loadTranList(tranCode) {
                $.ajax({
                    url: "/masters/sip_master1.aspx/GetTranList",
                    method: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSON.stringify({ tranCode }),
                    success: function (res) {
                        let { data } = JSON.parse(res.d);

                        if (!data || data.length === 0) {
                            $("#tranCodeGrid tbody").html(`
        <tr><td colspan="3" class="text-center text-danger">No records found</td></tr>
    `);
                            return;
                        }

                        let rows = data.map(rta => `
                                                <tr>
                                                   <td>${rta.HO_TRAN_CODE || ''}</td>        
                                                    <td>${rta.BRANCH_NAME || ''}</td>
                                                    <td>${rta.AMOUNT || ''}</td>
                                                </tr>
                                            `).join('');

                        $("#tranCodeGrid tbody").html(rows);
                    },
                    error: function (xhr, status, error) {
                        $("#tranCodeGrid tbody").html(`
    <tr><td colspan="3" class="text-center text-danger">Error loading data</td></tr>
`);
                        alert(`❌ Error: ${xhr.responseText}`);
                    }
                });
            }

            $('#btnTranFind').click(function () {
                const tranValue = $('#txtTranTranCode').val().trim();
                if (!tranValue) {
                    alert('Enter Tran Code!');
                    return;
                } else {
                    loadTranList(tranValue);
                }
            })
            //#endregion ---------- SECTION 3: RECONCILED TRAN DATA FIND ----------
        });
    </script>

    <div class="page-header">
        <h3 class="page-title">SIP Master Reconciliation 2 </h3>
    </div>

    <div class="row">
        <div class="grid-margin stretch-card draggable-container">
            <div class="card">
                <div class="card-body">
                    <%-- TR SERACH --%>
                    <div>
                        <%-- TR SEARCH INPUT --%>
                        <div>
                            <h4 class="card-title text-danger">WealthMaker Transactions</h4>
                            <div class="row g-3">
                                <%-- 1:  side col: ch, reg, zn, br, rm, amc --%>
                                <div class="col-md-4">
                                    <div class="row g-3">

                                        <!-- CHANNEL -->
                                        <div class="col-md-6">
                                            <label for="ddlChannel" class="form-label">Channel</label>
                                            <select id="ddlChannel" class="form-select">
                                            </select>
                                        </div>

                                        <!-- REGION -->
                                        <div class="col-md-6">
                                            <label for="ddlRegion" class="form-label">Region</label>
                                            <select id="ddlRegion" class="form-select">
                                            </select>
                                        </div>

                                        <!-- ZONE -->
                                        <div class="col-md-6">
                                            <label for="ddlZone" class="form-label">Zone</label>
                                            <select id="ddlZone" class="form-select">
                                            </select>
                                        </div>

                                        <!-- BRANCH -->
                                        <div class="col-md-6">
                                            <label for="ddlBranch" class="form-label">Branch</label>
                                            <select id="ddlBranch" class="form-select">
                                            </select>
                                        </div>

                                        <!-- RM -->
                                        <div class="col-md-6">
                                            <label for="ddlRM" class="form-label">RM</label>
                                            <select id="ddlRM" class="form-select">
                                            </select>
                                        </div>

                                        <!-- AMC -->
                                        <div class="col-md-6">
                                            <label for="ddlAMC" class="form-label">AMC</label>
                                            <select id="ddlAMC" class="form-select">
                                            </select>
                                        </div>

                                    </div>
                                </div>

                                <%-- 2: DTF, DTT, REGI, TRAN TYPE --%>
                                <div class="col-md-4">
                                    <div class="row g-3">
                                        <!-- DATE FROM -->
                                        <div class="col-md-6">
                                            <label for="txtDateFrom" class="form-label">Date From</label>
                                            <input type="text" id="txtDateFrom" class="form-control" placeholder="dd/mm/yyyy" maxlength="10" />
                                        </div>

                                        <!-- DATE TO -->
                                        <div class="col-md-6">
                                            <label for="txtDateTo" class="form-label">Date To</label>
                                            <input type="text" id="txtDateTo" class="form-control" placeholder="dd/mm/yyyy" maxlength="10" />
                                        </div>

                                        <!-- REGISTRAR -->
                                        <div class="col-md-12">
                                            <label class="form-label">Registrar</label>
                                            <div class="d-flex align-items-center flex-wrap gap-3 ms-4">
                                                <div class="form-check me-3">
                                                    <input class="form-check-input" id="regC" type="radio" name="registrar" value="c">
                                                    <label class="form-check-label" for="regC">C</label>
                                                </div>
                                                <div class="form-check me-3">
                                                    <input class="form-check-input" id="regK" type="radio" name="registrar" value="k">
                                                    <label class="form-check-label" for="regK">K</label>

                                                </div>
                                                <div class="form-check me-3">
                                                    <input class="form-check-input" id="regCCOB" type="radio" name="registrar" value="ccob">
                                                    <label class="form-check-label" for="regCCOB">C COB</label>

                                                </div>
                                                <div class="form-check me-3">
                                                    <input class="form-check-input" id="regKCOB" type="radio" name="registrar" value="kcob">
                                                    <label class="form-check-label" for="regKCOB">K COB</label>

                                                </div>
                                            </div>
                                        </div>

                                        <!-- TRAN TYPE -->
                                        <div class="col-md-6">
                                            <label class="form-label">Tran Type</label>
                                            <div class="d-flex align-items-start flex-wrap gap-2 ms-4">
                                                <div class="form-check me-4">
                                                    <input class="form-check-input" type="radio" name="tranType" id="tranRegular" value="regular" checked>
                                                    <label class="form-check-label" for="tranRegular">Regular</label>
                                                </div>

                                                <div class="form-check me-4">
                                                    <input class="form-check-input" type="radio" name="tranType" id="tranSIP" value="sip">
                                                    <label class="form-check-label" for="tranSIP">SIP</label>
                                                </div>

                                            </div>
                                        </div>
                                        <%-- PMS --%>
                                        <div class="col-md-3">

                                            <div class="form-check mt-5">
                                                <input type="checkbox" id="chkPMS" class="form-check-input" />
                                                <label for="chkPMS" class="form-check-label">PMS</label>
                                            </div>
                                        </div>

                                        <%-- COB --%>
                                        <div class="col-md-3">
                                            <div class="form-check mt-5">
                                                <input type="checkbox" id="chkTrCOB" class="form-check-input" />
                                                <label for="chkTrCOB" class="form-check-label">COB</label>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <%-- 3: RECOSTATUS, COB, COUNT   --%>
                                <div class="col-md-2 ps-2">
                                    <div class="row g-3 ms-2">
                                        <!-- RECONCILIATION -->
                                        <div class="col-md-12">
                                            <div class="">
                                                <div class="form-check me-3">
                                                    <input class="form-check-input" type="radio" id="rdbTrReconcile" name="reco" value="Y">
                                                    <label class="form-check-label" for="rdbTrReconcile">Reconcile</label>
                                                </div>
                                                <div class="form-check me-3">
                                                    <input class="form-check-input" type="radio" id="rdbTrUnreconcile" name="reco" value="N" checked>
                                                    <label class="form-check-label" for="rdbTrUnreconcile">Unreconcile</label>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- OPT 1 & OPT 2 -->
                                        <div class="col-md-12">
                                            <div class="">
                                                <div class="form-check me-3">
                                                    <input class="form-check-input" type="radio" id="rdbOp1" name="rdbOp" value="1">
                                                    <label class="form-check-label" for="rdbOp1">OP1</label>
                                                </div>
                                                <div class="form-check me-3">
                                                    <input class="form-check-input" type="radio" id="rdbOp2" name="rdbOp" value="2" checked>
                                                    <label class="form-check-label" for="rdbOp1">OP2</label>
                                                </div>
                                            </div>
                                        </div>


                                        <%-- COUNT --%>
                                        <div class="col-md-12 ms-0">
                                            <p class="text-danger fw-bold mt-2">Count: <span id="lblTrCount">0</span></p>
                                        </div>
                                    </div>
                                </div>


                                <!-- 4: AR INPUT, BTN:GO, RESET, EXPORT, EXIT-->
                                <div class="col-md-2">
                                    <!-- AR NO -->
                                    <div class="col-md-12">
                                        <label for="txtARNo" class="form-label">AR No.</label>
                                        <input type="text" id="txtARNo" class="form-control" />
                                    </div>

                                    <!-- Buttons:FIND TR, RESET -->
                                    <div class="col-md-12 mt-2">
                                        <div class="d-flex align-items-center justify-content-end flex-wrap gap-3 mt-4">
                                            <button type="button" id="btnTrFind" style="width:70px;" class="btn btn-sm btn-primary">Go</button>
                                            <button type="button" id="btnTrReset" style="width:70px;" class="btn btn-sm btn-outline-primary">Reset</button>
                                            <button type="button" id="btnTrExport" style="width:70px;" class="btn btn-sm btn-outline-primary">Export</button>
                                            <button type="button" id="btnTrExit" style="width:70px;" class="btn btn-sm btn-outline-primary">Exit</button>

                                        </div>
                                    </div>

            
                                </div>
                            </div>
                        </div>


                        <br />
                        <%-- TR SEARCHED GRID --%>
                        <div>


                            <div id="transactionContainer1" class="table table-bordered table-responsive" style="max-height: 250px; overflow-y: auto;">
                                <table id="transactionTable" class="table table-bordered">
                                    <thead>
                                        <tr>
                                            <th>TRAN_CODE</th>
                                            <th>TR_DATE</th>
                                            <th>INVESTOR_NAME</th>
                                            <th>ADDRESS</th>
                                            <th>CITY_NAME</th>
                                            <th>AMC_NAME</th>
                                            <th style="width:400px;">SCH_NAME</th>
                                            <th>AMOUNT</th>
                                            <th>FOLIO_NO</th>
                                            <th>CHEQUE_NO</th>
                                            <th>APP_NO</th>
                                            <th>RM_NAME</th>
                                            <th>BRANCH_NAME</th>
                                            <th>TRAN_TYPE</th>
                                            <th>SIP_TYPE</th>
                                            <th>LOGGEDUSER</th>
                                            <th>REMARK</th>
                                            <th style="display: none;">BANK_NAME</th>
                                            <th style="display: none;">RM_CODE</th>
                                            <th style="display: none;">PANNO</th>
                                            <th style="display: none;">CHEQUE_DATE</th>
                                            <th style="display: none;">BROKER_ID</th>
                                            <th style="display: none;">SIP_AMOUNT</th>
                                            <th style="display: none;">REGISTRAR</th>
                                            <th style="display: none;">SOURCE_CODE</th>
                                            <th style="display: none;">DISPATCH</th>
                                            <th style="display: none;">COB_FLAG</th>
                                            <th style="display: none;">CLIENT_CODE</th>
                                            <th style="display: none;">SCH_CODE</th>
                                            <th style="display: none;">MUT_CODE</th>
                                            <th style="display: none;">PAYMENT_MODE</th>
                                            <th style="display: none;">LEAD_NO</th>
                                            <th style="display: none;">LEAD_NAME</th>
                                            <th style="display: none;">BRANCH_CODE</th>
                                            <th style="display: none;">BUSINESS_RMCODE</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                    </tbody>
                                </table>

                            </div>

                        </div>
                    </div>

                    <br />
                    <hr />
                    <%-- RTA  SEARCH--%>
                    <div>
                        <%-- RTA SEARCH INPUT  --%>
                        <div>
                            <div class="row g-3">

                                <!-- Left Section -->
                                <div class="col-md-4">
                                    <div class="row g-3">

                                        <!-- FROM DATE -->
                                        <div class="col-md-6">
                                            <label for="txtRtaDateFrom" class="form-label">Date From <span class="text-danger">*</span></label>
                                            <input type="text" id="txtRtaDateFrom" class="form-control date-input" placeholder="dd/mm/yyyy" />
                                        </div>

                                        <!-- TO DATE -->
                                        <div class="col-md-6">
                                            <label for="txtRtaDateTo" class="form-label">Date To <span class="text-danger">*</span></label>
                                            <input type="text" id="txtRtaDateTo" class="form-control date-input" placeholder="dd/mm/yyyy" />
                                        </div>

                                        <!-- AMC -->
                                        <div class="col-md-6">
                                            <label for="ddlRtaAMC" class="form-label">AMC</label>
                                            <select id="ddlRtaAMC" class="form-select">
                                            </select>
                                        </div>

                                        <!-- BRANCH -->
                                        <div class="col-md-6">
                                            <label for="ddlRtaBranch" class="form-label">Branch</label>
                                            <select id="ddlRtaBranch" class="form-select">
                                              
                                            </select>
                                        </div>

                                        <!-- STATUS -->
                                        <div class="col-md-6">
                                            <%--<label class="form-label">Status</label>--%>
                                            <div class="d-flex align-items-center flex-wrap gap-3 ms-4">

                                                <div class="form-check form-check-inline">
                                                    <input class="form-check-input" type="radio" name="rtaRecoStatus" id="rtaReconcile" value="Y">
                                                    <label class="form-check-label" for="rtaReconcile">Reconciled</label>
                                                </div>
                                                <div class="form-check form-check-inline">
                                                    <input class="form-check-input" type="radio" name="rtaRecoStatus" id="rtaUnreconcile" value="N" checked>
                                                    <label class="form-check-label" for="rtaUnreconcile">Unreconciled</label>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- TRAN TYPE -->
                                        <div class="col-md-6">
                                            <%--<label class="form-label">Tran Type</label>--%>
                                            <div class="d-flex align-items-center flex-wrap gap-3 ms-4">

                                                <div class="form-check form-check-inline">
                                                    <input class="form-check-input" type="radio" name="rtaTranType" id="rtaTranTypeRegular" value="REGULAR" checked>
                                                    <label class="form-check-label" for="rtaTranTypeRegular">Regular</label>
                                                </div>
                                                <div class="form-check form-check-inline">
                                                    <input class="form-check-input" type="radio" name="rtaTranType" id="rtaTranTYpeSip" value="SIP">
                                                    <label class="form-check-label" for="rtaTranTYpeSip">SIP</label>
                                                </div>
                                            </div>
                                        </div>

                                    </div>
                                </div>

                                <!-- Middle Section -->
                                <div class="col-md-4">
                                    <div class="row g-3">

                                        <!-- SEARCH DROPDOWN -->
                                        <div class="col-md-6">
                                            <label for="ddlRtaCheque" class="form-label">Search</label>

                                            <span class="ctm_hidden" id="hdnTrCode"></span>
                                            <span class="ctm_hidden" id="hdnTrAmount"></span>
                                            <span class="ctm_hidden" id="hdnTrChe"></span>
                                            <span class="ctm_hidden" id="hdnTrFolio"></span>
                                            <span class="ctm_hidden" id="hdnTrApp"></span>
                                            <span class="ctm_hidden" id="hdnTrPan"></span>
                                            <span class="ctm_hidden" id="hdnTrBroker"></span>
                                            <span class="ctm_hidden" id="hdnTrDispatch"></span>
                                            <span class="ctm_hidden" id="hdnRtaCode"></span>
                                            <span class="ctm_hidden" id="hdnRtaDate"></span>
                                            <span class="ctm_hidden" id="hdnRtaFolio"></span>
                                            <span class="ctm_hidden" id="hdnRtaAmount"></span>
                                            <span class="ctm_hidden" id="hdnRtaRemark"></span>
                                            <select id="ddlRtaCheque" class="form-select" onchange="ddlChequeChanged()">
                                                <option value=""></option>
                                                <option value="001">CHEQUE_NO</option>
                                                <option value="002">FOLIO_NO</option>
                                                <option value="003">APP_NO</option>
                                                <option value="004">PANNO</option>
                                                <option value="005">BROKER_ID</option>
                                            </select>
                                        </div>

                                        <div class="col-md-6">
                                            <label for="txtRtaChequeNo" class="form-label">&nbsp;</label>
                                            <input type="text" id="txtRtaChequeNo" class="form-control" placeholder="Input type text">
                                        </div>

                                        <!-- INVESTOR NAME -->
                                        <div class="col-md-7">
                                            <label for="txtRtaInvestorName" class="form-label">Investor Name</label>
                                            <input type="text" id="txtRtaInvestorName" class="form-control" />
                                        </div>

                                        <!-- AMOUNT -->
                                        <div class="col-md-5">
                                            <label for="txtRtaAmount" class="form-label">Amount</label>
                                            <input type="text" id="txtRtaAmount" class="form-control" />
                                        </div>

                                        <!-- ACTION BUTTONS: Rta search, reset export -->
                                        <div class="d-flex align-items-center justify-content-end flex-wrap gap-3 my-3">
                                            <button type="button" id="btnRtaFind" class="btn btn-sm btn-primary">Search</button>
                                            <button type="button" id="btnRtaReset" class="btn btn-sm btn-outline-primary">Reset</button>
                                        </div>

                                    </div>
                                </div>

                                <!-- Right Section -->
                                <div class="col-md-4">

                                    <div class="row g-3">

                                        <!-- REMARKS -->
                                        <div class="col-md-12">
                                            <label for="txtRtaRemarks" class="form-label">Remarks</label>
                                            <div class="input-group">
                                                <input type="text" id="txtRtaRemarks" class="form-control" />
                                                <button type="button" id="btnRtaSaveRemark" class="btn btn-outline-primary">Save</button>
                                            </div>
                                        </div>

                                        <div class="col-md-12">

                                            <!-- CONFIRMATION BUTTONS -->
                                            <div class="d-flex align-items-center justify-content-end flex-wrap gap-3">
                                                <button type="button" id="btnRtaExport" class="btn btn-sm btn-outline-primary">Export</button>
                                                <button type="button" id="btnRtaReconsile" class="btn btn-sm btn-primary">Base SIP Reconcile</button>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </div>

                        <br />
                        <%-- RTA GRID TABLE --%>
                        <div>

                            <div class="table-responsive" id="transactionContainer2" style="max-height: 250px; overflow-y: auto;">
                                <table id="rtaGrid" class="table table-bordered">
                                    <thead class="thead-dark">
                                        <tr>
                                            <th>TRAN_CODE</th>
                                            <th>TRAN_DATE</th>
                                            <th>INVESTOR_NAME</th>
                                            <th>ADDRESS</th>
                                            <th>CITY_NAME</th>
                                            <th>AMC_NAME</th>
                                            <th style="width:400px;">SCHEME_NAME</th>
                                            <th>AMOUNT</th>
                                            <th>FOLIO_NO</th>
                                            <th>CHEQUE_NO</th>
                                            <th>APP_NO</th>
                                            <th>RM_NAME</th>
                                            <th>BRANCH_NAME</th>
                                            <th>BROKER_CODE</th>
                                            <th>REG_TRAN_TYPE</th>
                                            <th>UNIQUE_KEY</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                    </tbody>
                                </table>
                            </div>

                        </div>
                    </div>

                    <hr />
                    <br />
                    <!-- TRAN CODE -->
                    <div>
                        <%--  TRAN SERACH INPUT --%>
                        <div>
                            <h5 class="card-title text-danger">Tran Code</h5>
                            <div class="row g-3">
                                <div class="col-md-4">
                                    <div class="input-group">
                                        <input type="text" id="txtTranTranCode" class="form-control" placeholder="Enter Tran Code" />
                                        <button type="button" id="btnTranFind" class="btn btn-outline-primary">Find</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <br />
                        <%--  TRAN SERACH TABLE --%>
                        <div>
                            <div class="table-responsive">
                                <table id="tranCodeGrid" class="table table-bordered table-striped">
                                    <thead class="thead-dark">
                                        <tr>
                                            <th>TRAN_CODE</th>
                                            <th>BRANCH_NAME</th>
                                            <th>AMOUNT</th>
                                        </tr>
                                    </thead>
                                    <tbody></tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>
        <asp:HiddenField ID="hdnLoginId" runat="server" />
        <asp:HiddenField ID="hdnRoleId" runat="server"/>
    </div>



</asp:Content>

