<%@ Page Title="Print Renewal" Language="C#" MasterPageFile="~/vmSite.Master" AutoEventWireup="true" CodeBehind="print_renewal.aspx.cs" Inherits="WM.Masters.print_renewal" EnableViewState="true" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845DCD8080CC91"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid py-4">
        <div class="page-header mb-4">
            <h3 class="page-title">Renewal Printing</h3>
        </div>
        <div class="row">
            <div class="col-md-12">
                <div class="card">
                    <div class="card-body">
                        <div class="row g-3">
                            <div class="col-md-2">
                                <div class="form-group row">
                                    <div class="col-sm-12   d-flex align-items-center gap-2 mt-0">
                                        <label for="ddlLetterType" class="col-sm-4 col-form-label" style="width: fit-content">Letter Type</label>
                                        <select id="ddlLetterType" class="form-control me-1" style="width: 100px">
                                            <option value="A">A</option>
                                            <option value="B">B</option>
                                        </select>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="form-group row">
                                    <div class="col-sm-12   d-flex align-items-center gap-2 mt-0">
                                        <label for="ddlMonth" class="col-sm-4 col-form-label" style="width: 100px">Month/Year</label>
                                        <select id="ddlMonth" class="form-control me-1" style="width: 100px"></select>
                                        <span>/</span>
                                        <select id="ddlYear" class="form-control ms-1" style="width: 100px"></select>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group row">
                                    <div class="col-sm-12  d-flex align-items-center gap-2 mt-2 ms-1">
                                        <button id="btnCalculate" class="btn btn-primary btn-sm mx-1" type="button">Calculate</button>
                                        <button id="btnReport" class="btn btn-secondary btn-sm mx-1" type="button">Report</button>
                                        <button id="btnReset" class="btn btn-secondary btn-sm mx-1" type="button" onclick="location.reload();">Reset</button>
                                        <button id="btnExit" class="btn btn-outline-secondary btn-sm mx-1" type="button" onclick="window.close();">Exit</button>
                                        <span id="lblResultCount" class="text-danger fw-bold px-3 mt-2">Count</span>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-3 d-none">
                                <div class="form-group row">
                                    <div class="col-sm-12   d-flex align-items-center gap-2 mt-4">
                                        <div class="progress" style="height: 20px; margin-left: 10px; width: 200px">
                                            <div id="saveProgressBar" class="progress-bar bg-success" role="progressbar" style="width: 0%;" aria-valuemin="0" aria-valuemax="100">
                                                0%
                                            </div>
                                        </div>
                                        <span id="lblSaveResultCount" class="text-success fw-bold px-3"></span>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row g-3">
                            <div class="form-group row mt-2">
                                <div class="table-responsive" style="height: 280px">
                                    <table id="tbView1" class="table table-bordered">
                                        <thead class="thead-dark">
                                        </thead>
                                        <tbody>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <asp:HiddenField ID="hdnLoginId" runat="server" />
        <asp:HiddenField ID="hdnRoleId" runat="server" />
    </div>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>

    <link href="https://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/toastr.min.css" rel="stylesheet" />
    <script src="https://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/toastr.min.js"></script>

    <script>

        //#region ONLOAD FUNCTIONS: SESSION, LOADER, DEFAULT BTN PREVENT, DYNAMIC TABLE,
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
                $(this).remove();
            });
        }

        function preventDefaultActions(preventClick = false, preventEnter = false) {
            if (preventClick) {
                $(document).on('click.preventBtn', 'button, input[type="submit"]', function (e) {
                    e.preventDefault();
                    console.log('Button click prevented');
                });
            } else {
                $(document).off('click.preventBtn');
            }

            if (preventEnter) {
                $(document).on('keydown.preventEnter', function (e) {
                    if (e.key === "Enter" || e.keyCode === 13) {
                        e.preventDefault();
                        console.log('Enter key prevented');
                        return false;
                    }
                });
            } else {
                $(document).off('keydown.preventEnter');
            }
        }


        function validateDateField(fieldSelector, autoFormat = false, defaultToToday = false) {
            const datePattern = /^(0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[0-2])\/\d{4}$/;

            // Set default date if enabled
            if (defaultToToday && $(fieldSelector).val().trim() === '') {
                const today = new Date();
                const formatted = ('0' + today.getDate()).slice(-2) + '/' +
                    ('0' + (today.getMonth() + 1)).slice(-2) + '/' +
                    today.getFullYear();
                $(fieldSelector).val(formatted);
            }

            // Validate on blur
            $(fieldSelector).on('blur', function () {
                const value = $(this).val().trim();
                if (!datePattern.test(value)) {
                    $(this).css('border', '2px solid red');
                    $(this).attr('title', 'Please enter date in DD/MM/YYYY format');
                } else {
                    $(this).css('border', '');
                    $(this).removeAttr('title');
                }
            });

            // Auto-format on input
            if (autoFormat) {
                $(fieldSelector).on('input', function () {
                    let val = $(this).val().replace(/[^\d]/g, '');
                    if (val.length >= 2 && val.length < 4) {
                        val = val.slice(0, 2) + '/' + val.slice(2);
                    } else if (val.length >= 4 && val.length < 8) {
                        val = val.slice(0, 2) + '/' + val.slice(2, 4) + '/' + val.slice(4);
                    } else if (val.length >= 8) {
                        val = val.slice(0, 2) + '/' + val.slice(2, 4) + '/' + val.slice(4, 8);
                    }
                    $(this).val(val);
                });
            }
        }



        function formatDateToDMY(dateStr) {
            if (!dateStr) return '';
            const d = new Date(dateStr);
            return `${d.getDate().toString().padStart(2, '0')}/${(d.getMonth() + 1).toString().padStart(2, '0')}/${d.getFullYear()}`;
        }


        //#endregion ONLOAD FUNCTIONS: SESSION, LOADER, DEFAULT BTN PREVENT, DYNAMIC TABLE,

        //#region DATE,MONTH,YEAR FUNCTIONS

        function getCurrentDtDDMonYYYY() {
            // console.log(getCurrentFormattedDate()); // e.g. "30-Sep-2025"
            const now = new Date();

            const day = String(now.getDate()).padStart(2, '0');
            const month = now.toLocaleString('default', { month: 'short' }); // e.g. Sep
            const year = now.getFullYear();

            return `${day}-${month}-${year}`;
        }


        function getCurrentMonthYearFormatted() {
            const now = new Date();

            // Get full month name from locale and abbreviate to 3 letters
            const monthAbbr = now.toLocaleString('default', { month: 'short' }).toUpperCase();

            // Get last two digits of the year
            const yearShort = now.getFullYear().toString().substring(2);

            return `${monthAbbr}-${yearShort}`;
        }


        function getFormattedMonthYear(ddlMonthId, ddlYearId) {

            // HOW TO USE? const monthYear = getFormattedMonthYear("ddlMonth", "ddlYear");
            // Get selected month text and year value
            const monthText = $(`#${ddlMonthId} option:selected`).text().trim();
            const yearVal = $(`#${ddlYearId} option:selected`).val().trim();

            if (!monthText || !yearVal) {
                return null; // Incomplete selection
            }

            // Convert month to 3-letter abbreviation (JAN, FEB, etc.)
            const monthAbbr = monthText.substring(0, 3).toUpperCase();

            // Get last two digits of year (2025 → 25)
            const yearShort = yearVal.length === 4 ? yearVal.substring(2) : yearVal;

            return `${monthAbbr}-${yearShort}`;
        }



        function fillMonthDropdown(targetId) {
            const months = [
                { text: "January", value: "01" },
                { text: "February", value: "02" },
                { text: "March", value: "03" },
                { text: "April", value: "04" },
                { text: "May", value: "05" },
                { text: "June", value: "06" },
                { text: "July", value: "07" },
                { text: "August", value: "08" },
                { text: "September", value: "09" },
                { text: "October", value: "10" },
                { text: "November", value: "11" },
                { text: "December", value: "12" }
            ];

            const $ddl = $(`#${targetId}`);
            $ddl.empty(); // Clear any existing items

            // Optional: Add default "Select Month"
            $ddl.append(`<option value="">Select Month</option>`);

            months.forEach(month => {
                $ddl.append(`<option value="${month.value}">${month.text}</option>`);
            });
        }


        function fillYearDropdown(targetId, startYear, endYear) {
            const $ddl = $(`#${targetId}`);
            $ddl.empty(); // Clear existing items

            // Optional: Add default "Select Year"
            $ddl.append(`<option value="">Select Year</option>`);

            for (let year = startYear; year <= endYear; year++) {
                $ddl.append(`<option value="${year}">${year}</option>`);
            }
        }


        function setCurrentMonthYear(monthDropdownId, yearDropdownId) {
            // user setCurrentMonthYear('ddlMonth', 'ddlYear');
            const now = new Date();

            const currentMonth = String(now.getMonth() + 1).padStart(2, '0'); // "01" to "12"
            const currentYear = now.getFullYear(); // e.g., 2025

            console.log('CURRENT MONTH' + currentMonth);
            console.log('CURRENT YEAR' + currentYear);


            $(`#${monthDropdownId}`).val(currentMonth);
            $(`#${yearDropdownId}`).val(currentYear);
        }

        function formatDateToDMY(dateStr) {
            if (!dateStr) return '';
            const d = new Date(dateStr);
            return `${d.getDate().toString().padStart(2, '0')}/${(d.getMonth() + 1).toString().padStart(2, '0')}/${d.getFullYear()}`;
        }


        function getCurrentMonthYear2kFormat() {
            const now = new Date();

            const month = now.getMonth() + 1; // getMonth is 0-based, so add 1
            const year = now.getFullYear();   // e.g., 2024

            const shortYear = year.toString().slice(-1); // Get last digit only (e.g., 2024 → "4")
            const twoKYear = "2k" + shortYear;

            return `${month}/${twoKYear}`;
        }


        //#endregion DATE, MONTH, YEAR HANDLER

        //#region Dynamic Table Functions


        function psmJs_DynamicTableGenerate(data, targetTable, options = {}) {
            const {
                countElement = null,
                defaultColumnWidth = null,
                adjustableColumns = false,
                tableClass = 'table-striped table-bordered',
                displayNoneToTheseColumns = ''

            } = options;

            // Get the target table element
            const table = $(targetTable);

            table.empty();

            if (data.data && data.data.length > 0) {
                if (countElement) {
                    $(countElement).text(data.totalCount || data.data.length);
                }

                // Create table structure
                let headerRow = $('<tr>');
                let tableBody = $('<tbody>');

                // Create table header
                if (data.schema && data.schema.length > 0) {
                    data.schema.forEach(function (column) {
                        let th = $('<th>').text(column.columnName);
                        if (defaultColumnWidth) {
                            th.addClass('width', defaultColumnWidth);
                        }
                        headerRow.append(th);
                    });
                } else if (data.data.length > 0) {
                    // Fallback: use keys from first data item
                    Object.keys(data.data[0]).forEach(function (key) {
                        let th = $('<th>').text(key);
                        if (defaultColumnWidth) {
                            th.addClass('width', defaultColumnWidth);
                        }
                        headerRow.append(th);
                    });
                }

                // Create table body rows
                data.data.forEach(function (row) {
                    let tr = $('<tr>');
                    Object.values(row).forEach(function (value) {
                        tr.append($('<td>').text(value !== null ? value : ''));
                    });
                    tableBody.append(tr);
                });

                // Add header to thead and append to table
                table.append($('<thead>').append(headerRow));
                table.append(tableBody);
                //table.addClass(tableClass);

                // Make columns adjustable if enabled
                if (adjustableColumns) {
                    var newName = targetTable;
                    makeTableColumnsResizable(targetTable);
                }
                applyDataTableStyles(targetTable);
                applyColumnVisibility(targetTable, options);

            } else {
                table.html('<tbody><tr><td colspan="100%" class="no-results">No records found matching your criteria</td></tr></tbody>');
                $(countElement).text('');

            }
        }


        function makeTableColumnsResizable(tableId) {

            if (tableId.startsWith('#')) {
                tableId = tableId.substring(1);
            }

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

        function applyDataTableStyles(targetId) {
            // targetId with hash like #psmModal_InvestorSearch

            const styleId = 'dynamic-custom-style';
            if (!document.getElementById(styleId)) {
                const styles = `
                        .error-message {
                            display: block;
                            margin-top: 2px;
                            font-size: 12px;
                            /* Small and compact */
                            color: red;
                        }


                        .draggable {
                            cursor: move;
                        }

                        .print-table {
                            width: 100%;
                            border-collapse: collapse;
                            border: 1px solid #000;
                        }

                            .print-table th,
                            .print-table td {
                                border: 1px solid #000;
                                padding: 8px;
                            }

                            .print-table th {
                                background-color: #f2f2f2;
                            }

                        .floating-panel {
                            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                            border-radius: 5px;
                        }

                        .client-search-section {
                            background-color: #f8f9fa;
                            padding: 15px;
                            border-radius: 5px;
                            margin-bottom: 15px;
                        }

                        th {
                            background-color: #B78939 !important;
                            color: #fff !important;
                            position: sticky;
                            top: 0;
                            z-index: 2;
                        }

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

                ${targetId} tbody tr.row-selected > td {
                    background-color: lightblue !important;
                }

                ${targetId} thead tr th, #rtaGrid thead tr th {
                    position: sticky !important;
                    top: 0 !important;
                    background-color: #B78939 !important;
                    color: white !important;
                    z-index: 1;
                }

                .ctm_hidden {
                    display: none;
                }
            `;

                // Inject into head
                $('<style>', {
                    id: styleId,
                    type: 'text/css',
                    html: styles
                }).appendTo('head');
            }
        }

        function applyColumnVisibility(targetTable, options = {}) {

            // var displayNoneToTheseColumns = '1,2,3,4'

            const { displayNoneToTheseColumns = '' } = options;

            // Parse column numbers into an array of integers
            const hiddenColumns = displayNoneToTheseColumns
                .split(',')
                .map(c => parseInt(c.trim(), 10))
                .filter(c => !isNaN(c));

            if (hiddenColumns.length === 0) return; // nothing to hide

            const table = $(targetTable);

            // Hide headers
            table.find('thead tr th').each(function (index) {
                if (hiddenColumns.includes(index + 1)) {
                    $(this).css('display', 'none');
                }
            });

            // Hide body cells
            table.find('tbody tr').each(function () {
                $(this).find('td').each(function (index) {
                    if (hiddenColumns.includes(index + 1)) {
                        $(this).css('display', 'none');
                    }
                });
            });
        }

        //#endregion Dynamic Table Functions


        //#region Toastify
        function showToast(message, type = "info", title = "", options = {}) {
            // How to use
            // showToast("Data saved successfully.", "success");
            // showToast("Something went wrong.", "error");
            // showToast("Password is weak.", "warning");
            // showToast("Reminder set for tomorrow.", "info");

            const icons = {
                success: '<i class="fa fa-check-circle me-2 text-success"></i>',
                error: '<i class="fa fa-times-circle me-2 text-danger"></i>',
                warning: '<i class="fa fa-exclamation-triangle me-2 text-warning"></i>',
                info: '<i class="fa fa-info-circle me-2 text-primary"></i>'
            };

            // Use icon based on type
            let icon_msg = icons[type] || '' + icon + message;

            // Default options
            const defaultOptions = {
                closeButton: true,
                progressBar: true,
                positionClass: "toast-top-right",
                timeOut: 4000,
                extendedTimeOut: 2000,
                escapeHtml: false // Important to allow HTML content
            };

            toastr.options = { ...defaultOptions, ...options };

            toastr[type](message, title);
        }

        //#endregion Toastify

        $(document).ready(function () {

            //#region Default ONLOAD FUNCTIONS

            $(document).ajaxStart(function () {
                checkUserSession();
                showLoader();
            });

            $(document).ajaxStop(function () {
                hideLoader();
            });

            // var curDt = new Date().getFullYear();

            fillMonthDropdown('ddlMonth');

            fillYearDropdown('ddlYear', 2020, 2030);

            setCurrentMonthYear('ddlMonth', 'ddlYear');

            //#endregion On load functions

            //#region Calculate and Report Functions


            function frmPrintRen_btnReport_ClickGenerateReportShow(allPagesHTML) {

                const typeValue = $('#ddlLetterType').val();

                const monthYear = getFormattedMonthYear("ddlMonth", "ddlYear");
                let popup = window.open("", "_blank");

                popup.document.open();
                popup.document.write(`
<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<title>Report of Type: ${typeValue || ''} - ${monthYear}</title>

<style>
body {
font-family: Arial, sans-serif;
background: #ffffff;
padding: 20px;
}

button {
padding: 10px 20px;
font-size: 16px;
}

.letter-page {
background: #ffffff;
color: #111;
line-height: 1.45;
page-break-after: always;
break-after: page;
font-family: Arial, sans-serif;
margin: 0 auto;
border: unset;
padding-top: 20%;
padding-bottom: 10%;
width: 190mm;
height: 277mm;
box-sizing: border-box;
background: #fff;
overflow: hidden;
}

.letter-page h1 {
text-align: center;
font-size: 14px;
text-decoration: underline;
margin-bottom: 5px;
}

.letter-meta {
display: flex;
justify-content: space-between;
font-size: 13px;
margin: 12px 0;
}

.letter-address {
font-size: 13px;
margin-bottom: 15px;
}

.letter-subject {
text-align: center;
font-weight: bold;
margin-bottom: 15px;
font-size: 14px;
}

.fd-table {
width: 100%;
border-collapse: collapse;
margin: 12px 0;

}

.fd-table th,
.fd-table td {
border: 1px solid #000;
padding: 5px 8px;
font-size: 13px;
}

.fd-table tr td {
border: none;
}

.deposit-details {
border-bottom: 1px solid #000;
}





.fd-table th {
background: #f5f5f5;
font-weight: bold;
}

.letter-body p {
margin-bottom: 10px;
font-size: 13px;
text-align: justify;
}

.letter-sign {
margin-top: 25px;
font-size: 13px;

margin-top: 25px;
font-size: 13px;
position: relative;
bottom: 40px;
left: 0;
width: 100%;
}

.letter-sign .name {
font-weight: bold;
margin-top: 20px;
}

.ps {
/* margin-top: 15px; */
font-size: 12px;
}

.fd-table th,
.fd-table td {
font-size: 10px;
}

@page {
margin: 0;
}
</style>
</head>
<body>
${allPagesHTML}
<script>
  window.onload = function(){ window.print(); }
  window.onafterprint = function(){ window.close(); }
<\/script>
</body>
</html>`);
                popup.document.close();
            }


            function frmPrintRen_btnReport_ClickBuildTranRow(sourceCode) {
                console.log("🔍 Filtering by SOURCECODE:", sourceCode);

                var storedData = sessionStorage.getItem('rpt_tr');
                let adh = '';

                if (storedData) {
                    var investmentArray = JSON.parse(storedData);
                    console.log("✅ Parsed session data:", investmentArray);

                    if (investmentArray.length > 0) {
                        console.log("🧪 Sample record SOURCECODE:", investmentArray[0].SOURCECODE);
                    }

                    // Better filtering with logging
                    var filteredRecords = investmentArray.filter(function (item, i) {
                        console.log(`[${i}] Comparing '${item.SOURCECODE}' to '${sourceCode}'`);
                        return item.SOURCECODE?.toString().trim().toLowerCase() === sourceCode?.toString().trim().toLowerCase();
                    });

                    if (!filteredRecords || filteredRecords.length === 0) {


                        adh += `
<tr>
    <td rowspan="2">1</td>
    <td colspan="5" style="text-align: center; color: gray;">No data available</td>
</tr>
<tr class="deposit-details">
    <td colspan="5" style="text-align: center; color: lightgray;">No deposit details</td>
</tr>
`;


                    } else {
                        $.each(filteredRecords, function (index, record) {
                            console.log("✅ Match:", record);
                            adh += `
<tr>
    <td rowspan="2">${index + 1}</td>
    <td>${record.MUT_NAME}</td>
    <td>${record.AMOUNT}</td>
    <td>${record.INVESTOR_NAME}</td>
    <td>${record.MAT_PERIOD}</td>
    <td>${formatDateToDMY(record.ARDATE||'')}</td>
</tr>
<tr class="deposit-details">
    <td>Details of Deposit:</td>
    <td>Cheque NO: ${record.CHEQUE_NO}</td>
    <td>Cheque Dt: ${formatDateToDMY(record.CHEQUE_DATE|| '')}</td>
    <td colspan="2">Bank: ${(record.BANK_NAME|| '')}</td>
</tr>
`;
                        });
                    }

                    return adh;
                } else {
                    alert("❌ No data found in sessionStorage.");
                    return '';
                }
            }


            function frmPrintRen_btnReport_ClickBuildLetterHTML(type, { ref, sourceCodes, clientNames, add1List, add2List, cityCodes, mutNames, arDates, cityNames, pinCodes }) {

                let crtDtDDMonYYYY = getCurrentDtDDMonYYYY();
                let currentMonthYear = getCurrentMonthYearFormatted()
                let choosedMonYear = getFormattedMonthYear("ddlMonth", "ddlYear") || '';
                let multiTranDataRow = frmPrintRen_btnReport_ClickBuildTranRow(sourceCodes)

                return `
<div class="letter-page">
  <h1 style="text-align:center;">Maturity / Renewal Reminder</h1>
  <div class="letter-meta" style="margin-bottom: 16px;">
    <div>Ref No: <strong>${ref}</strong></div>
    <div><strong>${crtDtDDMonYYYY}</strong></div>
  </div>
  <div class="letter-address" style="margin-bottom: 16px;">
    ${clientNames}<br>
    ${add1List}<br>
    ${add2List}<br>
    ${cityNames}-${pinCodes}
  </div>
  <div class="letter-subject" style="font-weight: bold; margin-bottom: 16px;">
    Sub: Maturity of your Fixed Deposits / Bonds
  </div>
  <div class="letter-body">
    <p>Dear Sir / Madam,</p>
    <p>We would like to thank you for patronizing our services. We are India's Premier Investment Services Company with more than <strong>60 years</strong> of experience in helping people protect and grow their wealth. Over 10 Lakh Investor rely on our Investment Services</p>
    <p>Going forward, we wish to inform you that your following Fixed Deposits/Bonds are maturing in the month of <strong>${choosedMonYear}</strong>.</p>
    <table class="fd-table" border="1" cellpadding="5" cellspacing="0">
      <thead style="background:#f2f2f2;">
        <tr>
          <th>S.N.</th><th>Company Name</th><th>Amount</th>
          <th>Investor Name</th><th>Period</th><th>Date</th>
        </tr>
      </thead>
      <tbody>
        ${multiTranDataRow}
      </tbody>
    </table>
   <p>We request you to visit or call your nearest Bajaj Capital Investment Center TM. We have some investment
        opportunities waiting for you that can help give boost to your investment further. Expect this meeting to add a
        fresh perspective to your investments that would really be insightful and worthy of pondering over.</p>
      <p>Looking forward to meet you and discuss newer opportunities to multiply your wealth.</p>
    </div>
    <div class="letter-sign" style="margin-top: 32px;"><br>
    Warm Regards,<br><br>
    <span class="name">Aparna Razdan</span><br>
    Manager - Fixed Income Group<br><br>
    PS: Your Client Reference Number is: <strong>${sourceCodes}</strong>
  </div>
</div>`;
            }

            function getCurrentYearShort2KFormat() {
                const fullYear = new Date().getFullYear(); // e.g. 2025
                return `2K${fullYear % 10}`; // "2K5"
            }


            function frmPrintRen_btnReport_ClickGenerateReport(type, data) {
                let allPagesHTML = "";
                let formRef3 = getCurrentMonthYear2kFormat(); //"9/2k5" for Sep 2025

                let formRef4 = type === 'A' ? 'REM-A':'REM-B';

                data.forEach(function (row, index) {
                    let newData = {
                        ref: `${row.SOURCECODE1 || ''} ${formRef3.toUpperCase()}/${formRef4}/${index + 1}`,
                        sourceCodes: row.SOURCECODE1 || '',
                        clientNames: row.CLIENT_NAME1 || '',
                        add1List: row.ADD1 || '',
                        add2List: row.ADD2 || '',
                        cityCodes: row.CITY_CD || '',
                        mutNames: row.MUTNAME || '',
                        arDates: formatDateToDMY(row.ARDATE) || '',
                        cityNames: row.CITY_NAME || '',
                        pinCodes: row.PINCODE || ''
                    };

                    // Instead of directly opening popup, we collect HTML for each record
                    allPagesHTML += frmPrintRen_btnReport_ClickBuildLetterHTML(type, newData);
                });

                frmPrintRen_btnReport_ClickGenerateReportShow(allPagesHTML);
            }

            function frmPrintRen_btnReport_Click(type, mon_year) {
                //alert("Type: " + type + ", Month-Year: " + mon_year);
                $.ajax({
                    url: "/masters/print_renewal.aspx/ProcessReport",
                    method: "POST",
                    data: JSON.stringify({ type: type, mon_year }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var result = JSON.parse(response.d);
                        if (result.success) {
                            if (
                                result.data &&                         // data exists
                                Array.isArray(result.data) &&          // data is an array
                                result.data.length > 0 &&              // has at least one row
                                Object.keys(result.data[0]).length > 1 // first row has more than one column
                            ) {
                                frmPrintRen_btnReport_ClickGenerateReport(type, result.data);
                                console.log(result.data2);

                            } else {
                                alert("No records found.");
                            }

                        } else {
                            alert("Error: " + result.message);
                        }
                    },
                    error: function (xhr, status, error) {
                        alert("AJAX Error: " + error);
                    }
                });
            }

            function setSessionDataAndValidateTrData(key, newData) {
                const existing = sessionStorage.getItem(key);

                if (existing) {
                    //showToast("Session data already exists. Overwriting existing data.", "info");
                }

                sessionStorage.setItem(key, JSON.stringify(newData));
                const stored = sessionStorage.getItem(key);
                if (stored) {
                    try {
                        const parsedData = JSON.parse(stored);

                        if (Array.isArray(parsedData) && parsedData.length > 0) {
                        } else {
                            //showToast("Session data exists but is empty or not in expected format.", "error");
                        }
                    } catch (err) {
                        showToast("Failed to parse session data: " + err.message, "error");
                    }
                } else {
                    showToast("Failed to fetch data from sessionStorage.", "error");
                }
            }

        

            function frmPrintRen_btnCalculate_Click(type, mon_year) {
                //alert("Type: " + type + ", Month-Year: " + mon_year);
                $.ajax({
                    url: "/masters/print_renewal.aspx/ProcessReport",
                    method: "POST",
                    data: JSON.stringify({ type: type, mon_year: mon_year }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var result = JSON.parse(response.d);
                        if (result.success) {
                            if (result.data && result.data.length > 0) {
                                setSessionDataAndValidateTrData('rpt_data', result.data);
                                setSessionDataAndValidateTrData('rpt_tr', result.data2);
                                psmJs_DynamicTableGenerate(result, '#tbView1', {
                                    countElement: '#lblResultCount',
                                    defaultColumnWidth: '150px',
                                    adjustableColumns: true,
                                    displayNoneToTheseColumns: ''
                                });
                                showToast("Data calculated.", "success");
                            } else {
                                showToast("No records found.","error");
                                $('#lblResultCount').text('');
                                psmJs_DynamicTableGenerate(result, '#tbView1', {
                                    countElement: 'lblResultCount',
                                    defaultColumnWidth: '150px',
                                    adjustableColumns: true,
                                    displayNoneToTheseColumns: ''
                                });
                            }
                        } else {
                            alert("Error: " + result.message);
                            $('#lblResultCount').text('');
                            psmJs_DynamicTableGenerate(result, '#tbView1', {
                                countElement: 'lblResultCount',
                                defaultColumnWidth: '150px',
                                adjustableColumns: true,
                                displayNoneToTheseColumns: ''
                            });
                        }
                    },
                    error: function (xhr, status, error) {
                        alert("AJAX Error: " + error);
                    }
                });
            }


            //#endregion Calculate and Report Functions

            $('#btnCalculate').click(function () {
                var type = $('#ddlLetterType').val();
                var mon = $('#ddlMonth').val();
                var year = $('#ddlYear').val();
                if (!mon) {
                    alert("Please select Month");
                    return;
                }
                if (!year) {
                    alert("Please select Year");
                    return;
                }

                frmPrintRen_btnCalculate_Click(type, mon + '-' + year)
            });

            $('#btnReport').click(function () {
                var type = $('#ddlLetterType').val();
                var mon = $('#ddlMonth').val();
                var year = $('#ddlYear').val();

                if (!mon) {
                    alert("Please select Month");
                    $('#ddlMonth').focus();
                    return;
                }
                if (!year) {
                    alert("Please select Year");
                    $('#ddlYear').focus();
                    return;
                }
 

                const rptData = sessionStorage.getItem('rpt_data');
                const rptTr = sessionStorage.getItem('rpt_tr');

                if (rptData && rptTr) {
                    const reportDataJson = JSON.parse(rptData); // or merge rptTr if needed
                    frmPrintRen_btnReport_ClickGenerateReport(type, reportDataJson);
                    sessionStorage.removeItem('rpt_data');
                    sessionStorage.removeItem('rpt_tr');
                    return;
                    //frmPrintRen_btnReport_Click(type, mon + '-' + year);
                } else {
                    showToast("Data not present for print. Please calculate.", "error");
                    return;

                }


            });

            function resetSessionAndReload(keys = []) {
                if (Array.isArray(keys) && keys.length > 0) {
                    keys.forEach(key => {
                        sessionStorage.removeItem(key);
                    });
                } else {
                    sessionStorage.clear();
                }

                location.reload(true);
            }

            $('#btnReset').on('click', function () {
                resetSessionAndReload(['rpt_data', 'rpt_tr']);

                

            });


        });
    </script>

</asp:Content>




