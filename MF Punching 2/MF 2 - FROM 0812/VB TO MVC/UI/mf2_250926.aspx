
<%@ Page Title="MF Punching Interface" Language="C#" MasterPageFile="~/vmSite.Master" AutoEventWireup="true" CodeBehind="mf_punching_interface.aspx.cs" Inherits="WM.Masters.mf_punching_interface" EnableViewState="true" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <%-- JQuery CSS/JS --%>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css">
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>

    <!-- Toastify CSS/JS-->
    <link rel="stylesheet" type="text/css" href="https://cdn.jsdelivr.net/npm/toastify-js/src/toastify.min.css">
    <script type="text/javascript" src="https://cdn.jsdelivr.net/npm/toastify-js"></script>
    <script>
        // Toastify Example

        //Toastify({
        //    text: "This is a toast notification!",
        //    duration: 3000, // 3 seconds
        //    close: true,    // show close button
        //    gravity: "top", // top or bottom
        //    position: "right", // left, center or right
        //    backgroundColor: "linear-gradient(to right, #00b09b, #96c93d)",
        //    stopOnFocus: true, // prevent dismiss on hover
        //}).showToast();

    </script>

    <script>

        //#region ONLOAD FUNCTIONS: LOGSESSION, LOADER, Date validator

        function psmJs_checkSessionAndLoader(loginFieldId = '<%= hdnLoginId.ClientID %>', roleFieldId = '<%= hdnRoleId.ClientID %>') {
            /* Old approach to show laoder
            $(document).ajaxStart(function () {
                psmJs_checkUserSession();
                psmJs_showLoader();
            });

            $(document).ajaxStop(function () {
                psmJs_hideLoader();
            });
            */

            // Unbind previous event handlers to avoid duplicates
            $(document).off('ajaxStart.ajaxSession').on('ajaxStart.ajaxSession', function () {
                psmJs_checkUserSession(loginFieldId, roleFieldId);
                psmJs_showLoader();
            });

            $(document).off('ajaxStop.ajaxSession').on('ajaxStop.ajaxSession', function () {
                psmJs_hideLoader();
            });


        }

        function psmJs_checkUserSession(loginFieldId = '<%= hdnLoginId.ClientID %>', roleFieldId = '<%= hdnRoleId.ClientID %>') {
            const loginId = $('#' + loginFieldId).val();
            const roleId = $('#' + roleFieldId).val();
            if (!loginId || !roleId) {
                alert("Session expired or invalid. Please Login");
                window.location.href = 'https://www.wealthmaker.in/login_new.aspx';
            }
        }


        function psmJs_showLoader() {
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
            color: #B78939;
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

        function psmJs_hideLoader() {
            $('#customLoader').fadeOut(300, function () {
                $(this).remove();
            });
        }


        function mf2_authPopup(popupType) {
            switch (popupType) {
                case 'INV_UPDATE':
                    $('#psmModalInvestorAddUpdate').modal('show');
                    break;
                case 'INV_SEARCH':
                    $('#psmModelInvestorSearch').modal('show');
                    break;
                case 'CROSS_CHANNEL':
                    $('#psmModalCrossChannel').modal('show');
                    break;

            }
        }

        function psmJs_validateDateField(fieldSelector, autoFormat = false, defaultToToday = false) {

            /* How to use?
            
            // Auto-format enabled, default to today enabled
            //validateDateField('#ImSipStartDtA', true, true);

            // Only validation, no auto-format or default
            //validateDateField('#ImSipEndDtA');                
            */
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

        class psmJs_DateFieldValidator {
            // How to use?
            // new psmJs_DateFieldValidator('psm-date-validate'); // where date-validate is input field calss name
            constructor(className = 'psm-date-validate') {
                this.className = className;
                this.init();
            }

            init() {
                const self = this;
                $(`input.${this.className}`).each(function () {
                    self.attachEvents($(this));
                });
            }

            attachEvents($input) {
                $input.on('input', (e) => this.onInput(e));
                $input.on('blur', (e) => this.onBlur(e));
            }

            onInput(event) {
                const input = event.target;
                let val = input.value;
                let cursorPosition = input.selectionStart;

                // Count digits before formatting
                let digitsBefore = val.slice(0, cursorPosition).replace(/[^\d]/g, '').length;

                // Remove all non-digit characters
                val = val.replace(/[^\d]/g, '');

                // Format as DD/MM/YYYY
                if (val.length > 2) {
                    val = val.slice(0, 2) + '/' + val.slice(2);
                }
                if (val.length > 5) {
                    val = val.slice(0, 5) + '/' + val.slice(5);
                }

                // Limit to 10 characters
                if (val.length > 10) {
                    val = val.slice(0, 10);
                }

                input.value = val;

                // Recalculate cursor position after formatting
                let newCursorPosition = 0;
                let digitCount = 0;

                // Place cursor after the same number of digits
                for (let i = 0; i < val.length; i++) {
                    if (/\d/.test(val[i])) {
                        digitCount++;
                    }
                    newCursorPosition++;
                    if (digitCount >= digitsBefore) {
                        break;
                    }
                }

                // Set the new cursor position
                input.setSelectionRange(newCursorPosition, newCursorPosition);
            }
            onBlur(event) {
                const input = event.target;
                const val = input.value.trim();

                if (!val) return; // allow empty

                if (!this.isValidDate(val)) {
                    alert(`Invalid date format or invalid date: "${val}". Please use DD/MM/YYYY.`);
                    $(input).val('');
                    input.focus();
                }
            }

            isValidDate(dateStr) {
                // Check format dd/mm/yyyy strictly
                if (!/^\d{2}\/\d{2}\/\d{4}$/.test(dateStr)) return false;

                // Parse day, month, year
                const [dd, mm, yyyy] = dateStr.split('/').map(Number);

                // Basic range check
                if (dd < 1 || dd > 31 || mm < 1 || mm > 12 || yyyy < 1000 || yyyy > 9999) return false;

                // Check actual date validity
                const dateObj = new Date(yyyy, mm - 1, dd);
                if (dateObj.getFullYear() !== yyyy || dateObj.getMonth() + 1 !== mm || dateObj.getDate() !== dd) {
                    return false;
                }
                return true;
            }
        }

        function psmJs_onDateInput(e) {
            /* How to use
                $('.psm-date-validate').on('input', psmJs_onDateInput);
                $('.psm-date-validate').on('blur', psmJs_onDateBlur);
            */
            const input = e.target;
            let val = input.value;

            // Remove all non-digit chars
            val = val.replace(/[^\d]/g, '');

            // Insert slashes after day and month
            if (val.length > 2) val = val.slice(0, 2) + '/' + val.slice(2);
            if (val.length > 5) val = val.slice(0, 5) + '/' + val.slice(5);

            // Limit length to 10
            if (val.length > 10) val = val.slice(0, 10);

            input.value = val;
        }

        function psmJs_onDateBlur(e) {
            /* How to use
                $('.psm-date-validate').on('input', psmJs_onDateInput);
                $('.psm-date-validate').on('blur', psmJs_onDateBlur);
            */
            const input = e.target;
            const val = input.value.trim();

            if (!val) return; // allow empty

            if (!psmJs_isValidDate(val)) {
                alert(`Invalid date: "${val}". Please enter date as DD/MM/YYYY.`);
                $(input).val('');
                input.focus();
            }
        }

        function psmJs_isValidDate(dateStr) {
            if (!/^\d{2}\/\d{2}\/\d{4}$/.test(dateStr)) return false;

            const [dd, mm, yyyy] = dateStr.split('/').map(Number);

            if (dd < 1 || dd > 31 || mm < 1 || mm > 12 || yyyy < 1000) return false;

            const dateObj = new Date(yyyy, mm - 1, dd);
            return dateObj.getFullYear() === yyyy &&
                dateObj.getMonth() + 1 === mm &&
                dateObj.getDate() === dd;
        }

        function psmJs_formatDateToDMY(dateStr) {
            if (!dateStr) return '';
            const d = new Date(dateStr);
            return `${d.getDate().toString().padStart(2, '0')}/${(d.getMonth() + 1).toString().padStart(2, '0')}/${d.getFullYear()}`;
        }


        //#endregion ONLOAD FUNCTIONS: LOGSESSION, LOADER, Date validator

        //#region ONLOAD MODAL FUNCTIONS: DRAGGABLE, AUTORESET, PERSIST TAB & MODAL , MODAL TOGGLE

        function psmJs_bsmTabOpenPersist(tabContainerId, storageKey) {
            // Restore last active tab from localStorage
            let savedTab = localStorage.getItem(storageKey);
            if (savedTab) {
                $(`${tabContainerId} a[href="${savedTab}"]`).tab('show');
            }

            // When a tab is clicked → store it in localStorage
            $(`${tabContainerId} a[data-bs-toggle="tab"]`).on('shown.bs.tab', function (e) {
                let currentTab = $(e.target).attr("href");
                localStorage.setItem(storageKey, currentTab);
                console.log("Saved active tab:", currentTab);
            });
        }

        function psmJs_bsmDraggable() {
            // make boostrap modal draggable, boostrapmodaldraggable
            $(document).on('shown.bs.modal', '.modal', function () {
                var $modal = $(this);
                var $dialog = $modal.find('.modal-dialog');
                var $header = $modal.find('.modal-header');

                if ($header.length === 0) return; // needs header to drag

                let isDragging = false, startX = 0, startY = 0, origX = 0, origY = 0;

                $header.css('cursor', 'move');

                $header.off('mousedown.modaldrag').on('mousedown.modaldrag', function (e) {
                    isDragging = true;
                    startX = e.clientX;
                    startY = e.clientY;

                    // current offset of modal-dialog
                    const rect = $dialog[0].getBoundingClientRect();
                    origX = rect.left;
                    origY = rect.top;

                    // disable Bootstrap centering transform
                    $dialog.css('transform', 'none');
                    e.preventDefault();
                });

                $(document).off('mousemove.modaldrag').on('mousemove.modaldrag', function (e) {
                    if (!isDragging) return;

                    let dx = e.clientX - startX;
                    let dy = e.clientY - startY;

                    $dialog.css({
                        position: 'absolute',
                        margin: 0,
                        left: origX + dx,
                        top: origY + dy
                    });
                });

                $(document).off('mouseup.modaldrag').on('mouseup.modaldrag', function () {
                    isDragging = false;
                });

                // reset position when modal closes
                $modal.off('hidden.bs.modal.dragreset').on('hidden.bs.modal.dragreset', function () {
                    $dialog.attr('style', ''); // reset all inline styles
                });
            });
        }

        function psmJs_bsmAutoReset(modal) {
            //
            // make boostrap modal auto reset, boostrapmodalautoreset


            // internal helper that clears modal contents
            function resetOpenedModal(modal) {
                var $m = $(modal);

                // inputs
                $m.find('input').each(function () {
                    var t = (this.type || '').toLowerCase();
                    if (t === 'checkbox' || t === 'radio') {
                        this.checked = false;
                    } else {
                        this.value = '';
                    }
                });

                // textareas
                $m.find('textarea').val('');

                // selects
                $m.find('select').each(function () {
                    this.selectedIndex = -1;
                });

                // tables → clear tbody
                $m.find('table tbody').empty();
            }

            // bind once
            $(document).off('hidden.bs.modal.autoreset')
                .on('hidden.bs.modal.autoreset', '.modal', function () {
                    resetOpenedModal(this);
                });
        }

        function psmJs_bsmModalPersistOpen(prefixKey = 'psmModal_') {

            const storageKey = prefixKey;

            // --- 1. On page load, restore modal if stored in sessionStorage ---
            $(function () {
                const lastModalId = sessionStorage.getItem(storageKey);
                if (lastModalId) {
                    const modalEl = document.getElementById(lastModalId);
                    if (modalEl) {
                        const bsModal = new bootstrap.Modal(modalEl);
                        bsModal.show();
                    }
                }
            });

            // --- 2. When any modal is shown, store its ID in sessionStorage ---
            $(document).off('show.bs.modal.persistentSession').on('show.bs.modal.persistentSession', '.modal', function () {
                sessionStorage.setItem(storageKey, this.id);
            });

            // --- 3. When modal is closed via close button, clear sessionStorage ---
            $(document).off('click.persistentCloseSession').on('click.persistentCloseSession', '.close-modal', function () {
                const modalId = $(this).data("modal-id");
                const modalEl = document.getElementById(modalId);
                if (!modalEl) return;

                const bsModal = bootstrap.Modal.getInstance(modalEl);
                if (bsModal) bsModal.hide();

                // Clear sessionStorage if it matches
                if (sessionStorage.getItem(storageKey) === modalId) {
                    sessionStorage.removeItem(storageKey);
                }
            });
        }

        function psmJs_bsmModalToggle(modalId, action) {
            if (!modalId) return;

            const $modal = $(modalId);

            if (action === 1) {
                $modal.modal('show');
            } else if (action === 0) {
                $modal.modal('hide');

                // Extra cleanup to prevent stuck backdrop or scroll lock
                setTimeout(function () {
                    $('.modal-backdrop').remove();
                    $('body').removeClass('modal-open');
                    $('body').css('padding-right', '');
                }, 300);
            }
        }

        function psmJs_bsmModalToggle2(modalId, action = 'show', resetContent = false) {
            const $modal = $('#' + modalId);

            if (!$modal.length) {
                console.warn(`Modal with ID "${modalId}" not found.`);
                return;
            }

            if (action === 'show') {
                $modal.modal('show');
            } else if (action === 'hide') {
                $modal.modal('hide');

                // Delay to match Bootstrap fade transition
                setTimeout(function () {
                    $('.modal-backdrop').remove();           // Remove leftover backdrops
                    $('body').removeClass('modal-open');     // Remove scroll-lock class
                    $('body').css('padding-right', '');      // Reset padding

                    // Optional reset modal form content
                    if (resetContent) {
                        $modal.find('input:not([type="hidden"])').val('');
                        $modal.find('select').prop('selectedIndex', 0);
                        $modal.find('textarea').val('');
                        $modal.find('input:checkbox, input:radio').prop('checked', false);
                    }
                }, 300); // Bootstrap default fade duration
            } else {
                console.warn(`Invalid action: "${action}". Use 'show' or 'hide'.`);
            }
        }

        function psmJs_bsmModalCleanUpModals() {
            // Check if any modal is still visible
            if ($('.modal.show').length === 0) {
                $('body').removeClass('modal-open');
                $('.modal-backdrop').remove();
            }
        }

        // Handle every time a modal is fully hidden
        $(document).on('hidden.bs.modal', '.modal', function () {
            setTimeout(function () {
                fullModalCleanup();
            }, 200); // small delay to allow any chained modals
        });

        // Optional: ensure modal-open added when any modal is shown
        $(document).on('shown.bs.modal', '.modal', function () {
            $('body').addClass('modal-open');
        });

        // Apply to all modals
        $('.modal').on('hidden.bs.modal', function () {
            // Delay cleanup slightly to handle modal transitions
            setTimeout(cleanUpModals, 200);
        });

        $('.modal').on('shown.bs.modal', function () {
            // Make sure 'modal-open' is added to <body>
            $('body').addClass('modal-open');
        });


        //#endregion ONLOAD MODAL FUNCTIONS: DRAGGABLE, AUTORESET, PERSIST TAB & MODAL

        //#region DYNAMIC TABLE AND ROW HILIGHT, PREVENT DEFAULT ENTER, BUTTON
        function psmJs_preventDefaultButtonEnterEvent(preventClick = false, preventEnter = false) {
            // How to use: 
            // psmJs_preventDefaultButtonEnterEvent(true, true);
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

        function psmJs_DynamicTableHighlightRow($table, $row, activeClass = 'selected-row', bgColor = 'skyblue') {
            $table.find('tbody tr').removeClass(activeClass).find('td').css('background-color', '');
            $row.addClass(activeClass).find('td').css('background-color', bgColor);
        }

        function psmJs_DynamicTableGenerateTable(data, targetTable, options = {}) {
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
                    psmJs_DynamicTableMakeResizable(targetTable);
                }
                psmJs_DynamicTableStyle(targetTable);
                psmJs_DynamicTableColVisible(targetTable, options);

            } else {
                table.html('<tbody><tr><td colspan="100%" class="no-results">No records found matching your criteria</td></tr></tbody>');
                $(countElement).text('');

            }

            function psmJs_DynamicTableMakeResizable(tableId) {

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

            function psmJs_DynamicTableStyle(targetId) {

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

            function psmJs_DynamicTableColVisible(targetTable, options = {}) {

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

        }

        //#endregion  DYNAMIC TABLE AND ROW HILIGHT, PREVENT DEFAULT ENTER, BUTTON

        //#region DDL ONLOAD/ONCHANGE: AMC, Branch, Country, State, City, commonDdlData

        function loadAMCMasterList() {
            //alert('channel list');

            $.ajax({
                type: "POST",
                url: "/masters/mf_punching_interface.aspx/GetAMCMasterList",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: function () {
                    //psmJs_showLoader();
                },

                success: function (res) {
                    let { data } = JSON.parse(res.d);
                    data.unshift({ text: "All", value: "" });
                    let optionsHtml = data.map(r => `<option value="${r.value}">${r.text}</option>`);
                    $("#ddlAddAMC").html(optionsHtml.join(''));
                },
                error: function (xhr, status, error) {
                    alert("Failed to load channel list: " + error);
                },
                complete: function () {
                    //psmJs_hideLoader();
                }

            });
        }

        function loadBranchList() {
            //alert('channel list');

            $.ajax({
                type: "POST",
                url: "/masters/mf_punching_interface.aspx/GetBranchList",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: function () {
                    //psmJs_showLoader();
                },

                success: function (res) {
                    let { data } = JSON.parse(res.d);
                    data.unshift({ text: "All", value: "" });
                    let optionsHtml = data.map(r => `<option value="${r.value}">${r.text}</option>`);
                    $("#ddlAddBranch").html(optionsHtml.join(''));
                },
                error: function (xhr, status, error) {
                    alert("Failed to load branch list: " + error);
                },
                complete: function () {
                    //psmJs_hideLoader();
                }

            });
        }

        function loadDropdownData(psm = null, for_x = null, by_y = null, y = null, get_name = null, get_code = null, targetElementId = null, includeAll = true) {
            $.ajax({
                url: "/masters/mf_punching_interface.aspx/GetDropdownData",
                method: "POST",
                data: JSON.stringify({
                    psm: psm,
                    for_x: for_x,
                    by_y: by_y,
                    y: y,
                    get_name: get_name,
                    get_code: get_code
                }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    let response = JSON.parse(res.d);
                    let data = response.data;

                    if (includeAll) {
                        data.unshift({ text: "All", value: "" });
                    }

                    let options = data.map(x => `<option value="${x.value}">${x.text}</option>`);
                    $(`#${targetElementId}`).html(options.join(''));
                },
                error: function (xhr, status, error) {
                    alert("❌ Error loading dropdown data: " + xhr.responseText);
                }
            });
        }

        function loadCountry(for_x = 'COUNTRY_LIST', by_y = null, y = null) {
            $.ajax({
                url: "/masters/mf_punching_interface.aspx/GetCountryStateCity",
                method: "POST",
                data: JSON.stringify({ for_x, by_y, y }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    let { data } = JSON.parse(res.d);
                    data.unshift({ text: "All", value: "" });
                    let options = data.map(x => `<option value="${x.value}">${x.text}</option>`);
                    $("#psmModalInvestorAddUpdate_ddlCountry").html(options.join(''));
                },
                error: function (xhr, status, error) {
                    alert("❌ Error loading country: " + xhr.responseText);
                }
            });
        }

        function loadState(for_x = 'STATE_LIST', by_y = null, y = null) {
            $.ajax({
                url: "/masters/mf_punching_interface.aspx/GetCountryStateCity",
                method: "POST",
                data: JSON.stringify({ for_x, by_y, y }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    let { data } = JSON.parse(res.d);
                    if (by_y != "BY_CITY") {
                        data.unshift({ text: "All", value: "" });
                    }
                    let options = data.map(x => `<option value="${x.value}">${x.text}</option>`);
                    $("#psmModalInvestorAddUpdate_ddlState").html(options.join(''));
                },
                error: function (xhr, status, error) {
                    alert("❌ Error loading state: " + xhr.responseText);
                }
            });
        }

        function loadCity(for_x = 'CITY_LIST', by_y = null, y = null) {
            $.ajax({
                url: "/masters/mf_punching_interface.aspx/GetCountryStateCity",
                method: "POST",
                data: JSON.stringify({ for_x, by_y, y }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    let { data } = JSON.parse(res.d);
                    data.unshift({ text: "All", value: "" });
                    let options = data.map(x => `<option value="${x.value}">${x.text}</option>`);
                    $("#psmModalInvestorAddUpdate_ddlCity").html(options.join(''));
                },
                error: function (xhr, status, error) {
                    alert("❌ Error loading city: " + xhr.responseText);
                }
            });
        }

        //#endregion DDL ONLOAD/ONCHANGE: AMC, Branch, Country, State, City, commonDdlData

        //#region FRM MF Functions: PaymentChagne,

        function frmMf2_handlePaymentChange(ddlChe, lblCheNo, lblCheDt, txtChecValue, txtCheDt) {
            $(ddlChe).on("change", function () {
                var selected = $(this).val();

                if (selected === "CHEQUE") {
                    $(lblCheNo).text("Cheque No");
                    $(lblCheDt).text("Cheque Dated");
                    $(txtChecValue).prop("disabled", false).val("");
                    $(txtCheDt).prop("disabled", false).val("");
                }

                else if (selected === "DRAFT") {
                    $(lblCheNo).text("Draft No");
                    $(lblCheDt).text("Draft Date");
                    $(txtChecValue).prop("disabled", false).val("");
                    $(txtCheDt).prop("disabled", false).val("");
                }
                else if (selected === "ECS") {
                    $(lblCheNo).text("ECS Reference No");
                    $(lblCheDt).text("ECS Date");
                    $(txtChecValue).prop("disabled", false).val("");
                    $(txtCheDt).prop("disabled", false).val("");
                }
                else if (selected === "CASH") {
                    $(lblCheNo).text("Cash Amount");
                    $(lblCheDt).text("Cash Payment Date");
                    $(txtChecValue).prop("disabled", false).val("");
                    $(txtCheDt).prop("disabled", false).val("");
                }
                else if (selected === "RTGS") {
                    $(lblCheNo).text("RTGS Transaction No");
                    $(lblCheDt).text("RTGS Date");
                    $(txtChecValue).prop("disabled", false).val("");
                    $(txtCheDt).prop("disabled", false).val("");
                }
                else if (selected === "FUND_TRANSFER") {
                    $(lblCheNo).text("Fund Transfer No");
                    $(lblCheDt).text("Fund Transfer Date");
                    $(txtChecValue).prop("disabled", false).val("");
                    $(txtCheDt).prop("disabled", false).val("");
                }
                else {
                    // Others
                    $(lblCheNo).text("Others Ref. No");
                    $(lblCheDt).text("Payment Date");
                    $(txtChecValue).prop("disabled", false).val("");
                    $(txtCheDt).prop("disabled", false).val("");
                }
            });
        }


        //#endregion

        //#region HOW TO CALL session loader, tab persist modal persist

        /*
        psmJs_checkSessionAndLoader();
        psmJs_preventDefaultButtonEnterEvent(true, true);
        psmJs_bsmTabOpenPersist("#wmTabs", "mf2_activeTab");
        psmJs_bsmDraggable();
        //psmJs_bsmModalPersistOpen('psmModal_Mf2');

        */
        //#endregion HOW TO CALL

    </script>

    <%-- MAIN CALL: ONLOAD SESSION, LOADER, TAB, MODEL, DRAGGABLE, COUNTRY, STATE, CITY, AMC, BRANCH, DATE FIELD VALIDATOR --%>
    <script>
        $(document).ready(function () {
            //#region ONLOAD FUNCTIONS CALL: SESSION, LOADER, TAB PERSIST, DRRAGABLE MODAL

            //psmJs_checkSessionAndLoader();
            psmJs_preventDefaultButtonEnterEvent(true, true);
            psmJs_bsmTabOpenPersist("#wmTabs", "mf2_activeTab");
            psmJs_bsmDraggable();
            //psmJs_bsmModalPersistOpen('psmModal_Mf2');
            //psmJs_bsmModalCleanUpModals();

            if ($('.modal.show').length === 0) {
                $('body').removeClass('modal-open');
                $('.modal-backdrop').remove();
            }
            loadCountry();
            loadState();
            loadCity();
            loadAMCMasterList();
            loadBranchList();
            new psmJs_DateFieldValidator('psm-date-validate');

            //#endregion ONLOAD FUNCTIONS CALL: SESSION, LOADER, TAB PERSIST, DRRAGABLE MODAL
        });


    </script>

    <%-- FRM ADD SECTION --%>
    <script>

        function frmMf2Add_txtAddDtNumber_Click(index = '0') {
            const commonId = $('#txtAddDtNumber').val() || '';
            const chkSwitch = $('#ddlChkSwitch').val() || '0';
            if (!commonId) {
                alert("⚠️ Please enter a DT number.");
                return;
            } else {
                frmMf2Add_txtAddDtNumber_ClickGetData(index, commonId, chkSwitch);
            }
        }

        function frmMf2Add_txtAddDtNumber_ClickGetData(index, commonId, chkSwitch) {
            $.ajax({
                url: "/masters/mf_punching_interface.aspx/ProcessMfAdd_GetByDT",
                method: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify({
                    index,
                    commonId,
                    chkSwitch
                }),
                success: function (res) {

                    let response = JSON.parse(res.d);
                    if (response.success && response.message.startsWith("SUCCESS:")) {
                        if (response.data) {
                            if (index === '0') {

                                frmMf2Add_txtAddDtNumber_ClickSetData(response.data);
                            } else if (index === '1') {
                                alert('Do something with index 1')
                            }
                        }
                    } else {
                        alert("❌ " + response.message);

                        if (index === '0') {
                            frmMf2Add_txtAddDtNumber_ClickSetData(null);
                        }

                    }
                },
                error: function (xhr, status, error) {
                    alert("Error: " + xhr.responseText);
                    frmMf2Add_txtAddDtNumber_ClickSetData(null);

                }
            });
        }

        function frmMf2Add_txtAddDtNumber_ClickSetData(data = null) {

            if (data) {

                $("#txtAddTrDate").val(psmJs_formatDateToDMY(data.VA_IM_ENTRY_DT) || '');
                $("#txtAddBssCode").val(data.VA_BUSI_CODE || '');
                $("#txtAddInvCode").val(data.VA_LABEL32 || '');
                $("#txtAddAHName").val(data.VA_INVESTOR_NAME || '');
                $("#txtAddAHCode").val(data.VA_AH_CODE || '');
                $("#lblAddExpPer").html(data.VA_EXPENSE_PER || '');
                $("#lblAddExpRs").html(data.VA_EXPENSE_RS || '');
                $("#txtAddExpPer").val(data.VA_IM_N_EXP_PER || '');
                $("#ddlAddBranch").val(data.VA_BUSI_BRANCH_CODE || '');
                $("#ddlAddAMC").val(data.VA_AMC_CODE || '');
                $("#txtAddScheme1").val((data.VA_SCH_NAME || '') + '#' + (data.VA_SCH_CODE || ''));
                $("#hdnAddScheme1").val(data.VA_SCH_CODE || '');
                $("#txtAddScheme2_fromSwitch").val(data.VA_SWITCH_SCH || '');
                $("#hdnAddScheme2_fromSwitch").val(data.VA_SWITCH_SCH_CODE || '');
                $("#hdnAddClientCode").val(data.VA_CLIENT_CODE || '');
                $("#hdnAddPan1").val(data.VA_PAN || '');
                $("#txtAddTrDate").prop('disabled', data.VA_IM_ENTRY_DT_ENABLED !== 'Y');
                $("#ddlAddAMC").prop('disabled', data.VA_AMC_ENABLED !== 'Y');
                $("#txtAddScheme1").prop('disabled', data.VA_SCHEM_ENABLED !== 'Y');

                var retMsg = (data.V_RETURN_MESSAGE || '');
                var curDT = $('#txtAddDtNumber').val();
                var curBsi = $("#txtAddBssCode").val();
                var curInv = $("#txtAddInvCode").val();

                if (retMsg.includes('CROSS_CHANNEL_VALIDATION')) {
                    psmModalCrossChannelJs_GetSetData(curDT, curInv);
                    $("#psmModalCrossChannel").modal("show");
                } else if (retMsg.includes('INVESTOR_ADDRESS_UPDATE0')) {
                    psmModalInvestorAddUpdateJs_GetSetData(curInv);
                    $("#psmModalInvestorAddUpdate").modal("show");
                }


            } else {
                $("#txtAddTrDate").val('');
                $("#txtAddBssCode").val('');
                $("#txtAddInvCode").val('');
                $("#txtAddAHName").val('');
                $("#txtAddAHCode").val('');
                $("#lblAddExpPer").html('');
                $("#lblAddExpRs").html('');
                $("#txtAddExpPer").val('');
                $("#ddlAddBranch").val('');
                $("#ddlAddAMC").val('');
                $("#txtAddScheme1").val('');
                $("#hdnAddScheme1").val('');
                $("#txtAddScheme2_fromSwitch").val('');
                $("#hdnAddScheme2_fromSwitch").val('');
                $("#hdnAddClientCode").val('');
                $("#hdnAddPan1").val('');
                $("#txtAddTrDate").prop('disabled', true);
                $("#ddlAddAMC").prop('disabled', true);
                $("#txtAddScheme1").prop('disabled', true);
                frmMf2Add_txtAddDtNumber_ClickSetBusiCodeData(null);

                psmModalCrossChannelJs_GetSetData(null);
                psmModalInvestorAddUpdateJs_GetSetData(null);
            }

            frmMf2Add_txtAddDtNumber_ClickGetSetBusiCodeData();
        }

        function frmMf2Add_txtAddDtNumber_ClickGetSetBusiCodeData(curBsi = null, curInv = null) {

            const busiCode = $("#txtAddBssCode").val() || curBsi;
            const invCode = $("#txtAddInvCode").val() || curInv;
            const allIndia = '';
            const allIndiaSearchLag = '';
            const branches = '';

            if (!busiCode || !invCode) {
                alert("⚠️ Business Code/ Ivestor Code is Invalid");
                frmMf2Add_txtAddDtNumber_ClickSetBusiCodeData(null);

            }
            else if (busiCode && invCode) {
                $.ajax({
                    url: "/masters/mf_punching_interface.aspx/ProcessBusiCodeLostFocus",
                    method: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",

                    data: JSON.stringify({
                        busiCode: busiCode,
                        invCode: invCode,
                        allIndia: allIndia,
                        allIndiaSearchLag: allIndiaSearchLag,
                        branches: branches,
                    }),
                    success: function (res) {
                        let response = JSON.parse(res.d);
                        if (response.success && response.message.startsWith("SUCCESS:")) {
                            if (response.data) {
                                frmMf2Add_txtAddDtNumber_ClickSetBusiCodeData(response.data);
                            }
                        } else {
                            alert("❌ " + response.message);
                            frmMf2Add_txtAddDtNumber_ClickSetBusiCodeData(null);
                        }
                    },
                    error: function (xhr, status, error) {
                        alert("Error: " + xhr.responseText);
                        frmMf2Add_txtAddDtNumber_ClickSetBusiCodeData(null);
                    }
                });
            }
        }

        function frmMf2Add_txtAddDtNumber_ClickSetBusiCodeData(data = null) {
            if (data) {
                const msg = data.V_MSG || '';
                const bsi_code = data.VA_BUSI_CODE || '';
                const br_name = data.VA_BRANCH_NAME || '';
                const br_code = data.VA_BRANCH_CODE || '';
                const rm_name = data.VA_RM_NAME || '';
                const rm_code = data.VA_RM_CODE || '';
                const inv_code = data.VA_INV_CODE || '';
                const inv_name = data.VA_INV_NAME || '';
                const cur_br_code = data.MYCURRENTBRANCHCODE || '';

                const alertMsg = `
Return Msg      : ${msg}
Business Code   : ${bsi_code}
Branch Name     : ${br_name}
Branch Code     : ${br_code}
RM Name         : ${rm_name}
RM Code         : ${rm_code}
Investor Code   : ${inv_code}
Investor Name   : ${inv_name}
Current Branch  : ${cur_br_code}
`;

                if (msg.startsWith("SUCCESS:")) {
                    $("#txtAddRM").val(rm_name);
                } else {
                    alert("❌ " + msg);
                    $("#txtAddRM").val('');
                    $("#txtAddInvCode").val('');
                }
            }
            else {
                $("#txtAddRM").val('');
                $("#txtAddInvCode").val('');
                return;
            }
        }

        function frmMf2AddJs_GetSetInvDataForDt_ChangeInv(data = null) {
            // pass data from inestor search model on row db click

            if (data) {
                //$("#txtAddTrDate").val(psmJs_formatDateToDMY(data.VA_IM_ENTRY_DT) || '');
                //$("#txtAddBssCode").val(data.VA_BUSI_CODE || '');
                $("#txtAddInvCode").val(data.VA_LABEL32 || '');
                $("#txtAddAHName").val(data.VA_INVESTOR_NAME || '');
                $("#txtAddAHCode").val(data.VA_AH_CODE || '');
                //$("#lblAddExpPer").html(data.VA_EXPENSE_PER || '');
                //$("#lblAddExpRs").html(data.VA_EXPENSE_RS || '');
                //$("#txtAddExpPer").val(data.VA_IM_N_EXP_PER || '');
                $("#ddlAddBranch").val(data.VA_BUSI_BRANCH_CODE || '');
                $("#ddlAddAMC").val(data.VA_AMC_CODE || '');
                $("#txtAddScheme1").val(data.VA_SCH_NAME || '');
                $("#hdnAddScheme1").val(data.VA_SCH_CODE || '');
                $("#txtAddScheme2_fromSwitch").val(data.VA_SWITCH_SCH || '');
                $("#hdnAddScheme2_fromSwitch").val(data.VA_SWITCH_SCH_CODE || '');
                $("#hdnAddClientCode").val(data.VA_CLIENT_CODE || '');
                $("#hdnAddPan1").val(data.VA_PAN || '');
                $("#txtAddTrDate").prop('disabled', data.VA_IM_ENTRY_DT_ENABLED !== 'Y');
                $("#ddlAddAMC").prop('disabled', data.VA_AMC_ENABLED !== 'Y');
                $("#txtAddScheme1").prop('disabled', data.VA_SCHEM_ENABLED !== 'Y');

                //alert(ReturnMessage);
                var retMsg = (data.V_RETURN_MESSAGE || '');
                var curDT = $('#txtAddDtNumber').val();
                var curBsi = $("#txtAddBssCode").val();
                var curInv = $("#txtAddInvCode").val();

                if (retMsg.includes('CROSS_CHANNEL_VALIDATION')) {
                    psmModalCrossChannelJs_GetSetData(curDT, curInv);
                    $("#psmModalCrossChannel").modal("show");
                } else if (retMsg.includes('SUCCESS:OPEN_POPUP:INVESTOR_ADDRESS_UPDATE0')) {
                    psmModalInvestorAddUpdateJs_GetSetData(curInv);
                    $("#psmModalInvestorAddUpdate").modal("show");
                }

            } else {
                $("#txtAddTrDate").val('');
                $("#txtAddBssCode").val('');
                $("#txtAddInvCode").val('');
                $("#txtAddAHName").val('');
                $("#txtAddAHCode").val('');
                $("#lblAddExpPer").html('');
                $("#lblAddExpRs").html('');
                $("#txtAddExpPer").val('');
                $("#ddlAddBranch").val('');
                $("#ddlAddAMC").val('');
                $("#txtAddScheme1").val('');
                $("#hdnAddScheme1").val('');
                $("#txtAddScheme2_fromSwitch").val('');
                $("#hdnAddScheme2_fromSwitch").val('');
                $("#hdnAddClientCode").val('');
                $("#hdnAddPan1").val('');
                $("#txtAddTrDate").prop('disabled', true);
                $("#ddlAddAMC").prop('disabled', true);
                $("#txtAddScheme1").prop('disabled', true);
            }
            frmMf2Add_txtAddDtNumber_ClickGetSetBusiCodeData();
        }


        $(document).ready(function () {

            //#region MF.ADD: ONLOAD FUNCTIONS CALL
            frmMf2_handlePaymentChange("#ddlAddPaymentMode", "#lblAddChequeNo", "#lblAddChequeDate", "#txtAddChequeNo", "#txtAddChequeDate");

            //#endregion MF.ADD: ONLOAD FUNCTIONS CALL



            //#region MF.ADD: ONCHANGE FUNCTIONS CALL

            //#endregion MF.ADD: ONCHANGE FUNCTIONS CALL



            //#region MF.ADD: ONCLICK FUNCTIONS CALL
            $('#btnAddDtNumber').on('click', function (e) {
                e.preventDefault();
                frmMf2Add_txtAddDtNumber_Click();
            });

            $("#btnAddTransaction").on("click", function () {
                let dtNumber = $("#txtAddDtNumber").val();
                let transactionDate = $("#txtAddTrDate").val();
                let businessCode = $("#txtAddBssCode").val();
                let rm = $("#txtAddRM").val();
                let invCode = $("#txtAddInvCode").val();
                let anaCode = $("#txtAddAnaCode").val();
                let accountHolder = $("#txtAddAHName").val();
                let accountHolderCode = $("#txtAddAHCode").val();
                let branch = $("#ddlAddBranch").val();
                let amc = $("#ddlAddAMC").val();
                let scheme = $("#hdnAddScheme1").val();
                let transactionType = $("#ddlAddTransactionType").val();
                let regularNfo = $("input[name='rdbAddRN']:checked").val();
                let formSwitchFolio = $("#txtAddformSwitchFolio").val();
                let formSwitchScheme = $("#hdnAddScheme2_fromSwitch").val();
                let schemeSearchState = $("#hdnSchemeSearchState").val();


                let applicationNo = $("#txtAddAppNo").val();
                let folioNo = $("#txtAddFolioNo").val();
                let amount = $("#txtAddAmount").val();
                let paymentMode = $("#txtAddPaymentMode").val();
                let chequeNo = $("#txtAddChequeNo").val();
                let chequeDate = $("#txtAddChequeDate").val();
                let bankName = $("#ddlAddBankName").val();
                let expPercentage = $("#txtAddExpPer").val();
                let expRs = $("#txtAddExpRs").val();
                let autoSwitchOnMaturity = $("#chkAddAutoSwitchOnMaturity").is(":checked") ? 'Y' : 'N';
                let closeScheme = $("#hdnAddScheme3_Close").val();


                let sipStp = $("#ddlAddSipStp").val();
                let installmentType = $("#ddlAddInstallmentType").val();
                let sipType = $("#ddlAddSipType").val();
                let sipAmount = $("#txtAddSipAmount").val();
                let frequency = $("#ddlAddFrequency").val();
                let installmentsNo = $("#txtAddInstallmentsNo").val();
                let sipStartDate = $("#txtAddSIPStartDate").val();
                let sipEndDate = $("#txtAddSIPEndDate").val();
                let freshRenewal = $("input[name='rdbAddFreshRenewal']:checked").val();
                let cobCase = $("#chkAddCOBCase").is(":checked") ? 'Y' : 'N';
                let swpCase = $("#chkAddSWPCase").is(":checked") ? 'Y' : 'N';
                let freedomSip = $("#chkAddFreedomSIP").is(":checked") ? 'Y' : 'N';
                let ninetyNineYears = $("#chkAdd99Years").is(":checked") ? 'Y' : 'N';

                let panNo = $("#txtAddPan2").val();

                let msg1 = "Stored field values:\n\n" +
                    "Investor / Scheme Details\n" +
                    "-----------------------------\n" +
                    "DT Number: " + dtNumber + "\n" +
                    "Transaction Date: " + transactionDate + "\n" +
                    "Business Code: " + businessCode + "\n" +
                    "RM: " + rm + "\n" +
                    "Investor Code: " + invCode + "\n" +
                    "ANA Code: " + anaCode + "\n" +
                    "Account Holder: " + accountHolder + "\n" +
                    "Account Holder Code: " + accountHolderCode + "\n" +
                    "Branch: " + branch + "\n" +
                    "AMC: " + amc + "\n" +
                    "Scheme: " + scheme + "\n" +
                    "Scheme Search State: " + schemeSearchState + "\n" +
                    "Transaction Type: " + transactionType + "\n" +
                    "Regular/NFO: " + regularNfo + "\n" +
                    "Form Switch Folio: " + formSwitchFolio + "\n" +
                    "Form Switch Scheme: " + formSwitchScheme + "\n\n"

                let msg2 = "Application & Payment Details\n" +
                    "-----------------------------\n" +
                    "Application No: " + applicationNo + "\n" +
                    "Folio No: " + folioNo + "\n" +
                    "Amount: " + amount + "\n" +
                    "Payment Mode: " + paymentMode + "\n" +
                    "Cheque No: " + chequeNo + "\n" +
                    "Cheque Date: " + chequeDate + "\n" +
                    "Bank Name: " + bankName + "\n" +
                    "Exp %: " + expPercentage + "\n" +
                    "Exp Rs: " + expRs + "\n" +
                    "Auto Switch On Maturity: " + autoSwitchOnMaturity + "\n" +
                    "Close Scheme: " + closeScheme + "\n\n"

                let msg3 = "SIP Details\n" +
                    "-----------------------------\n" +
                    "SIP/STP: " + sipStp + "\n" +
                    "Installment Type: " + installmentType + "\n" +
                    "SIP Type: " + sipType + "\n" +
                    "SIP Amount: " + sipAmount + "\n" +
                    "Frequency: " + frequency + "\n" +
                    "Installments No: " + installmentsNo + "\n" +
                    "SIP Start Date: " + sipStartDate + "\n" +
                    "SIP End Date: " + sipEndDate + "\n" +
                    "Fresh/Renewal: " + freshRenewal + "\n" +
                    "COB Case: " + cobCase + "\n" +
                    "SWP Case: " + swpCase + "\n" +
                    "Freedom SIP: " + freedomSip + "\n" +
                    "99 Years: " + ninetyNineYears + "\n\n" +

                    "PAN Details\n" +
                    "-----------------------------\n" +
                    "PAN No: " + panNo;

                alert(msg1);
                alert(msg2);
                alert(msg3);

                console.log(msg);
            });

            //#endregion MF.ADD: ONCLICK FUNCTIONS CALL

        });

    </script>

    <%-- FRM VIEW SECTION --%>
    <script>
        $(document).ready(function () {

            //#region FRM VIEW  SECTION FUNCTIONS


            frmMf2_handlePaymentChange("#ddlViewPaymentMode", "#lblViewChequeNo", "#lblViewChequeDate", "#txtViewChequeNo", "#txtViewChequeDate");




            //#endregion  FRM VIEW SECTION FUNCTIONS
        });
    </script>

    <%-- VIEW TR FIND --%>
    <script>

        $(document).ready(function () {

            //#region VIEW: TR FIND
            function loadGridFillList(x = '1') {

                const tabIndex = x;
                const businessRmCodeA = $('#txtAddBssCode').val() || '';
                const fromDate = '01/05/2025'; // $('#txtViewTrFromDate').val() || '';
                const toDate = '10/05/2025'; //$('#txtViewTrToDate').val() || '';
                const panS = $('#txtViewTrPan').val() || '';
                const clientS = $('#txtViewTrUniqueClientCode').val() || '';
                const tranNo = $('#txtViewTrTrNo').val() || '';
                const anaCode = $('#txtViewTrAnaCode').val() || '';
                const chequeNo = $('#txtViewTrChequeNo').val() || '';
                const appNo = $('#txtViewTrAppNo').val() || '';
                const installments = $('#txtViewInstallmentsNo').val() || '';
                const sipType = $('#ddlViewSipStp').val() || '';
                const clientCode = $('#txtViewInvCode').val() || '';
                const acHolderCode = $('#txtViewAHCode').val() || '';
                const businessRmCodeM = $('#txtViewBssCode').val() || '';
                const panM = $('#txtViewPan').val() || '';
                const frequencyCode = $('#ddlViewFrequency').val() || '';
                const orderBy = $('#ddlViewTrOrderBy').val() || '';
                const descending = $('#rdbViewTrDescending').is(':checked') ? '1' : '0';

                if (!fromDate || !toDate) {
                    alert("⚠️ Please enter both From Date and To Date.");
                    return;
                }
                $.ajax({
                    url: "/masters/mf_punching_interface.aspx/GetGridFillList",
                    method: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSON.stringify({
                        tabIndex,
                        businessRmCodeA,
                        fromDate,
                        toDate,
                        panS,
                        clientS,
                        tranNo,
                        anaCode,
                        chequeNo,
                        appNo,
                        installments,
                        sipType,
                        clientCode,
                        acHolderCode,
                        businessRmCodeM,
                        panM,
                        frequencyCode,
                        orderBy,
                        descending
                    }),
                    success: function (res) {

                        let { data } = JSON.parse(res.d);

                        if (x === '1') {
                            populateTempBssData(data, '#tbViewTrTrDetails');
                        } else {
                            console.warn('x is not 1, skipping row rendering');
                        }
                    }
                    ,

                    error: function (xhr, status, error) {
                        $("#gridTable tbody").html(`
                <tr><td colspan="30" class="text-center text-danger">Error loading data</td></tr>
            `);
                        $("#lblGridCount").text(`0`);
                        alert("Error: " + xhr.responseText);
                    }
                });
            }


            $('#btnViewTrSearchAR').on('click', function (e) {
                e.preventDefault(); // Stops default behavior like form submission
                loadGridFillList('1'); // Call your function
            });

            function populateTempBssData(data, table) {
                const $tbody = $(table).find('tbody');
                $tbody.empty();

                if (data == null || data.length === 0) {
                    alert('No Data Found! 🙁');
                    return;
                }

                $.each(data, function (index, row) {
                    console.log('Appending row', row);
                    const tr = $('<tr>');

                    // Appending each cell
                    tr.append($('<td>').text(row.INVESTOR_NAME || ''));
                    tr.append($('<td>').text(row.BRANCH_NAME || ''));
                    tr.append($('<td>').text(row.PANNO || ''));
                    tr.append($('<td>').text(row.AMC_NAME || ''));
                    tr.append($('<td>').text(row.SCHEME_NAME || ''));
                    tr.append($('<td>').text(psmJs_formatDateToDMY(row.TR_DATE) || ''));
                    tr.append($('<td>').text(row.TRAN_TYPE || ''));
                    tr.append($('<td>').text(row.APP_NO || ''));
                    tr.append($('<td>').text(row.PAYMENT_MODE || ''));
                    tr.append($('<td>').text(row.CHEQUE_NO || ''));
                    tr.append($('<td>').text(psmJs_formatDateToDMY(row.CHEQUE_DATE) || ''));
                    tr.append($('<td>').text(row.AMOUNT || ''));
                    tr.append($('<td>').text(row.SIP_TYPE || ''));
                    tr.append($('<td>').text(row.LEAD_NO || ''));
                    tr.append($('<td>').text(row.LEAD_NAME || ''));
                    tr.append($('<td>').text(row.TRAN_CODE || ''));
                    tr.append($('<td>').text(row.BRANCH_CODE || ''));
                    tr.append($('<td>').text(row.BUSINESS_RMCODE || ''));
                    tr.append($('<td>').text(row.SCH_CODE || ''));
                    tr.append($('<td>').text(row.SOURCE_CODE || ''));
                    tr.append($('<td>').text(row.BANK_NAME || ''));
                    tr.append($('<td>').text(row.RM_NAME || ''));
                    tr.append($('<td>').text(row.FOLIO_NO || ''));
                    tr.append($('<td>').text(row.DOC_ID || ''));
                    tr.append($('<td>').text(row.MICRO_INVESTMENT || ''));
                    tr.append($('<td>').text(row.TARGET_SWITCH_SCHEME || ''));
                    tr.append($('<td>').text(row.TARGET_SCHEME_NAME || ''));
                    tr.append($('<td>').text(row.SWITCH_SCHEME_NAME || ''));

                    $tbody.append(tr);
                });

            }

            function populateMfTransactionTable(data, tableElem) {
                const $tbody = $(tableElem).find('tbody');
                $tbody.empty();

                if (!Array.isArray(data) || data.length === 0) {
                    alert('No MF Transaction Data Found! 🙁');
                    return;
                }

                $.each(data, function (index, row) {
                    const tr = $('<tr>');

                    tr.append($('<td>').text(row.investor_name || ''));
                    tr.append($('<td>').text(row.bank_name || ''));
                    tr.append($('<td>').text(row.SCH_CODE || ''));
                    tr.append($('<td>').text(row.rm_name || ''));
                    tr.append($('<td>').text(row.sip_type || ''));
                    tr.append($('<td>').text(row.branch_name || ''));
                    tr.append($('<td>').text(row.PANNO || ''));
                    tr.append($('<td>').text(row.Amc_Name || ''));
                    tr.append($('<td>').text(row.Scheme_Name || ''));
                    tr.append($('<td>').text(psmJs_formatDateToDMY(row.TR_DATE) || ''));
                    tr.append($('<td>').text(row.TRAN_TYPE || ''));
                    tr.append($('<td>').text(row.App_No || ''));
                    tr.append($('<td>').text(row.PAYMENT_MODE || ''));
                    tr.append($('<td>').text(row.CHEQUE_NO || ''));
                    tr.append($('<td>').text(psmJs_formatDateToDMY(row.CHEQUE_DATE) || ''));
                    tr.append($('<td>').text(row.Amount || ''));
                    tr.append($('<td>').text(row.lEAD_nO || ''));
                    tr.append($('<td>').text(row.LEAD_NAME || ''));
                    tr.append($('<td>').text(row.TRAN_code || ''));
                    tr.append($('<td>').text(row.branch_code || ''));
                    tr.append($('<td>').text(row.BUSINESS_RMCODE || ''));
                    tr.append($('<td>').text(row.frequency || ''));
                    tr.append($('<td>').text(row.installments_no || ''));
                    tr.append($('<td>').text(row.micro_investment || ''));
                    tr.append($('<td>').text(row.target_switch_scheme || ''));

                    $tbody.append(tr);
                });
            }



            function psmJs_formatDateToDMYNew(dateStr) {
                if (!dateStr) return '';
                const monthMap = {
                    jan: '01', january: '01',
                    feb: '02', february: '02',
                    mar: '03', march: '03',
                    apr: '04', april: '04',
                    may: '05',
                    jun: '06', june: '06',
                    jul: '07', july: '07',
                    aug: '08', august: '08',
                    sep: '09', sept: '09', september: '09',
                    oct: '10', october: '10',
                    nov: '11', november: '11',
                    dec: '12', december: '12'
                };

                // Normalize separators to dashes for easy splitting
                const cleaned = dateStr.trim().replace(/[\s./]+/g, '-');
                const parts = cleaned.split('-');

                if (parts.length !== 3) return ''; // Not a valid format

                let [day, month, year] = parts;

                // Handle month in word form
                const monLower = month.toLowerCase();
                if (monthMap[monLower]) {
                    month = monthMap[monLower];
                } else if (!/^\d+$/.test(month)) {
                    // If it's not numeric and not in map, it's invalid
                    return '';
                } else {
                    // If numeric, pad it
                    month = month.padStart(2, '0');
                }

                day = day.padStart(2, '0');

                // Fix 2-digit years
                if (year.length === 2) {
                    const yr = parseInt(year);
                    year = yr < 50 ? '20' + year : '19' + year;
                }

                return `${day}/${month}/${year}`;
            }



            //#endregion VIEW: TR FIND
        });
    </script>

    <%-- M.INV ADD UPDATE --%>
    <script>
        function psmModalInvestorAddUpdateJs_GetSetData(investor_code) {
            if (!investor_code) {
                alert("⚠️ Investor Code not Present!");
                psmModalInvestorAddUpdateJs_SetData(null);
            } else {
                $.ajax({
                    url: "/masters/mf_punching_interface.aspx/ProcessInvestoUpdateGetData",
                    method: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSON.stringify({
                        inv_code: investor_code
                    }),
                    success: function (res) {
                        let response = JSON.parse(res.d);
                        if (response.success && response.message.startsWith("SUCCESS:")) {
                            if (response.data) {
                                psmModalInvestorAddUpdateJs_SetData(response.data);
                            }
                        } else {
                            alert("❌ " + response.message);
                        }
                    },
                    error: function (xhr, status, error) {
                        alert("Error: " + xhr.responseText);
                    }
                });
            }
        }

        function psmModalInvestorAddUpdateJs_SetData(data) {

            if (!data) {

                $("#psmModalInvestorAddUpdate_txtInvCode").val('');
                $("#psmModalInvestorAddUpdate_txtInvName").val('');
                $("#psmModalInvestorAddUpdate_txtAddress1").val('');
                $("#psmModalInvestorAddUpdate_txtAddress2").val('');
                $("#psmModalInvestorAddUpdate_txtPIN").val('');
                $("#psmModalInvestorAddUpdate_txtEmail").val('');
                $("#psmModalInvestorAddUpdate_txtMobile").val('');
                $("#psmModalInvestorAddUpdate_txtPAN").val('');
                $("#psmModalInvestorAddUpdate_txtAadhar").val('');
                $("#psmModalInvestorAddUpdate_txtDOB").val('');
                $("#psmModalInvestorAddUpdate_ddlCity").val('');
                $("#psmModalInvestorAddUpdate_ddlState").html('<option value="">All</option>');
            } else {
                const inv = {
                    msg: data.MSG || '',
                    code: data.V_INV_CODE || '',
                    name: data.V_INV_NAME || '',
                    address1: data.V_ADD1 || '',
                    address2: data.V_ADD2 || '',
                    pin: data.V_PIN || '',
                    dob: (data.V_DOB) || '',
                    pan: data.V_PAN ? data.V_PAN.toUpperCase() : '',
                    aadhar: data.V_AADHAR || '',
                    city: data.V_CITY_NAME || '',
                    state: data.V_STATE_NAME || '',
                    city_id: data.V_CITY_ID || '',
                    state_id: data.V_STATE_ID || '',
                    mobile: data.V_MOBILE || '',
                    email: data.V_EMAIL || ''
                };

                // Optional debug
                const prepareAlertMsg = `
Message       = ${inv.msg}
invCode  = ${inv.code}
invName  = ${inv.name}
Address1      = ${inv.address1}
Address2      = ${inv.address2}
City          = ${inv.city}
State         = ${inv.state}
City ID       = ${inv.city_id}
Mobile        = ${inv.mobile}
Email         = ${inv.email}`;
                //alert(prepareAlertMsg);
                $("#psmModalInvestorAddUpdate_txtInvCode").val(inv.code || '');
                $("#psmModalInvestorAddUpdate_txtInvName").val(inv.name || '');
                $("#psmModalInvestorAddUpdate_txtAddress1").val(inv.address1 || '');
                $("#psmModalInvestorAddUpdate_txtAddress2").val(inv.address2 || '');
                $("#psmModalInvestorAddUpdate_txtPIN").val(inv.pin || '');
                $("#psmModalInvestorAddUpdate_txtEmail").val(inv.email || '');
                $("#psmModalInvestorAddUpdate_txtMobile").val(inv.mobile || '');
                $("#psmModalInvestorAddUpdate_txtPAN").val(inv.pan || '');
                $("#psmModalInvestorAddUpdate_txtAadhar").val(inv.aadhar || '');
                $("#psmModalInvestorAddUpdate_txtDOB").val(inv.dob || '');
                $("#psmModalInvestorAddUpdate_ddlCity").val(inv.city_id || '');

                let selectedCityID = inv.city_id || $('#psmModalInvestorAddUpdate_ddlCity').val();
                if (!selectedCityID) {
                    loadCity();
                    loadState();
                } else {
                    loadState('STATE_LIST', 'BY_CITY', selectedCityID);
                    $("#psmModalInvestorAddUpdate_ddlState option:contains('All')").remove();
                }
            }
        }

        function psmModalInvestorAddUpdate_btnUpdate_Click() {

            const formData = {
                code: $("#psmModalInvestorAddUpdate_txtInvCode").val().trim(),
                name: $("#psmModalInvestorAddUpdate_txtInvName").val().trim(),
                address1: $("#psmModalInvestorAddUpdate_txtAddress1").val().trim(),
                address2: $("#psmModalInvestorAddUpdate_txtAddress2").val().trim(),
                pin: $("#psmModalInvestorAddUpdate_txtPIN").val().trim(),
                email: $("#psmModalInvestorAddUpdate_txtEmail").val().trim(),
                pan: $("#psmModalInvestorAddUpdate_txtPAN").val().trim(),
                mobile: $("#psmModalInvestorAddUpdate_txtMobile").val().trim(),
                aadhar: $("#psmModalInvestorAddUpdate_txtAadhar").val().trim(),
                dob: $("#psmModalInvestorAddUpdate_txtDOB").val().trim(),
                city_id: $("#psmModalInvestorAddUpdate_ddlCity").val(),
                state_id: $("#psmModalInvestorAddUpdate_ddlState").val()
            };

            $.ajax({
                url: "/masters/mf_punching_interface.aspx/ProcessInvAddUp_UpdateData",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify({ formData: formData }),
                success: function (response) {
                    const result = response.d;
                    if (result.Success) {
                        alert(result.Message || "Investor info saved successfully.");
                    } else {
                        alert("Update failed: " + (result.Message || "Unknown error."));
                    }
                },
                error: function (xhr, status, error) {
                    alert("AJAX error:", error);
                }
            });
        }


        $(document).ready(function () {

            //#region MODEL HANDLER: INVESTOR ADDRESS UPADATE
            $("#psmModalInvestorAddUpdate_ddlState").change(function () {
                let stateId = $(this).val();
                loadCity('CITY_LIST', 'BY_STATE', stateId);
            });

            $("#psmModalInvestorAddUpdate_ddlCity").change(function () {
                let cityID = $(this).val();
                if (!cityID) {
                    loadCity();
                    loadState();
                } else {
                    loadState('STATE_LIST', 'BY_CITY', cityID);
                    $("#psmModalInvestorAddUpdate_ddlState option:contains('All')").remove();
                }
            });

            $('#psmModalInvestorAddUpdate_btnUpdate').on('click', function (e) {
                e.preventDefault();
                psmModalInvestorAddUpdate_btnUpdate_Click();
            });



            //#endregion
        });
    </script>

    <%-- M.CROSS CHANNEL --%>
    <script>
        $(document).ready(function () {

            //#region MODEL HANDLER: CROSS CHANNEL VALIDATION
            function psmModalCrossChannelJs_GetSetData(dt = null, inv = null, type = 'MF') {
                alert(`DT: ${dt}, INV Code: ${inv}, Frm Type: ${type}`)
                if (!dt || !inv) {
                    psmModalCrossChannelJs_SetData(null);
                } else {

                    $.ajax({
                        url: "/masters/mf_punching_interface.aspx/ProcessCrossChannelGetData",
                        method: "POST",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: JSON.stringify({
                            connonID: dt,
                            inv_code: inv,
                            frm_type: type
                        }),
                        success: function (res) {
                            let response = JSON.parse(res.d);

                            if (response.success && response.message && response.message.startsWith("SUCCESS:")) {
                                if (response.data) {
                                    psmModalCrossChannelJs_SetData(response.data);
                                }
                            } else {
                                alert("❌ " + (response.message || "Unknown error"));
                            }
                        },
                        error: function (xhr, status, error) {
                            console.error("AJAX Error:", error, xhr.responseText);
                            alert("Error: " + xhr.responseText);
                        }
                    });
                }
            }

            function psmModalCrossChannelJs_SetData(data) {
                let $tbody = $("#psmCrossChannel_gv tbody");
                $tbody.empty();

                if (!data || data.length === 0) {
                    $tbody.html(`<tr><td colspan="12" class="text-center">No data found</td></tr>`);
                    return;
                }

                data.forEach(item => {
                    const {
                        INV_CODE,
                        CLIENT_NAME,
                        RM_NAME,
                        PAN,
                        EMAIL,
                        MOBILE,
                        BRANCH_NAME,
                        ZONE_NAME,
                        REGION_NAME,
                        CHANNEL,
                        REMARK,
                        APPROVED

                    } = item;

                    let rowHtml = `
            <tr>
                <td>${INV_CODE || ""}</td>
                <td>${CLIENT_NAME || ""}</td>
                <td>${RM_NAME || ""}</td>
                <td>${PAN || ""}</td>
                <td>${EMAIL || ""}</td>
                <td>${MOBILE || ""}</td>
                <td>${BRANCH_NAME || ""}</td>
                <td>${ZONE_NAME || ""}</td>
                <td>${REGION_NAME || ""}</td>
                <td>${CHANNEL || ""}</td>
                <td>${REMARK || ""}</td>
                <td>${APPROVED || ""}</td>

            </tr>
        `;
                    $tbody.append(rowHtml);
                });
            }


            //#endregion MODEL HANDLER: CROSS CHANNEL VALIDATION
        });
    </script>

    <%-- M.INV SEARCH --%>
    <script>
        //#region HELPING FUNCTIONS: MODAL INVESTOR SEARCH

        function psmModelInvestorSearch_btnSearch_click() {
            var params = {
                pxBranch: $('#psmModelInvestorSearch_ddlBranch').val() || '',
                pxCat: $('#psmModelInvestorSearch_ddlType').val() || '',
                pxCity: $('#psmModelInvestorSearch_ddlCity').val() || '',
                pxCode: $('#psmModelInvestorSearch_txtClientSubBrokerCode').val() || '',
                pxName: $('#psmModelInvestorSearch_txtInvestorName').val() || '',
                pxAdd1: $('#psmModelInvestorSearch_txtAddress1').val() || '',
                pxAdd2: $('#psmModelInvestorSearch_txtAddress2').val() || '',
                pxPhone: $('#psmModelInvestorSearch_txtPhone').val() || '',
                pxPan: $('#psmModelInvestorSearch_txtPanNo').val() || '',
                pxMobile: $('#psmModelInvestorSearch_txtMobile').val() || '',
                pxNewRm: $('#psmModelInvestorSearch_ddlRM').val() || '',
                pxAhCode: $('#psmModelInvestorSearch_txtAHCode').val() || '',
                pxClientSubName: $('#psmModelInvestorSearch_txtClientSubBrokerName').val() || '',
                pxClientBroker: $('#psmModelInvestorSearch_ddlBroker').val() || '',
                pxStrForm: 'frmtransactionmf',
                pxCurrentForm: 'frmtransactionmf',
                pxRm: '',//$('#ddlRM').val() || '',
                pxOldRm: '',// $('#ddlOldRM').val() || '',
                pxSort: $('#psmModelInvestorSearch_ddlSort').val() || 'NAME'
            };

            $.ajax({
                type: "POST",
                url: "/masters/mf_punching_interface.aspx/ProcessModalInvestorSearch_Find",
                data: JSON.stringify(params),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    let parsed = JSON.parse(response.d);
                    if (parsed.message && parsed.message.toUpperCase().startsWith("SUCCESS")) {
                        psmJs_DynamicTableGenerateTable(parsed, '#psmModelInvestorSearch_tbResult', {
                            countElement: '#psmModelInvestorSearch_lblResultCount',
                            defaultColumnWidth: '250px',
                            adjustableColumns: true,
                            displayNoneToTheseColumns: ''
                        });
                        return

                    } else {
                        alert(parsed.message || 'Unknown error occurred');
                        psmJs_DynamicTableGenerateTable(result, '#psmModelInvestorSearch_tbResult', {
                            countElement: '#psmModelInvestorSearch_lblResultCount',
                            defaultColumnWidth: '250px',
                            adjustableColumns: true,
                            displayNoneToTheseColumns: ''
                        });
                    }
                },
                error: function (xhr, status, error) {
                    alert('AJAX Error: ' + error);
                }
            });
        }

        function setupRowClick(tableId, activeClass = 'selected-row', bgColor = 'skyblue') {
            // Remove any old inline style or class
            $('#' + tableId + ' tbody').on('click', 'tr', function () {
                alert('Row clicked!');

                // Remove the active class and inline bg color from all rows
                $('#' + tableId + ' tbody tr')
                    .removeClass(activeClass)
                    .css('background-color', '');

                // Add the active class and inline bg color to the clicked row
                $(this)
                    .addClass(activeClass)
                    .css('background-color', bgColor);
            });
        }

        function setupRowDoubleClick(tableId) {
            $('#' + tableId + ' tbody').on('dblclick', 'tr', function () {
                let rowData = [];

                // Loop through each <td> in the clicked row
                $(this).find('td').each(function () {
                    rowData.push($(this).text().trim());
                });

                // Show the row data as comma-separated values
                alert('Row data: ' + rowData.join(', '));
            });
        }

        function psmModelInvestorSearch_tvRowDb(PX_INV_CODE, PX_INDEX, PX_CAT) {
            const params = {
                'PX_INV_CODE': PX_INV_CODE,
                'PX_INDEX': PX_INDEX,
                'PX_CAT': PX_CAT
            };

            for (let key in params) {
                if (params[key] === null || params[key] === '') {
                    alert(`Missing or empty parameter: ${key}`);
                    return;
                }
            }

            console.log('Calling with:', PX_INV_CODE, PX_INDEX, PX_CAT);



            $.ajax({
                type: "POST",
                url: "/masters/mf_punching_interface.aspx/ProcessInvSearchRowClick_MF",

                data: JSON.stringify({
                    PX_INV_CODE: PX_INV_CODE,
                    PX_INDEX: PX_INDEX,
                    PX_CAT: PX_CAT
                }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response && response.d) {
                        var result = JSON.parse(response.d);
                        if (result.success === true && result.data && result.data.length > 0) {
                            var row = result.data[0];

                            // Debugging Data //
                            /*
                            var msg = row.MSG || '';
                            var V_LOG_USER_ID = row.V_LOG_USER_ID || '';
                            var V_MAIN_CODE = row.V_MAIN_CODE || '';
                            var V_UPD_PROC = row.V_UPD_PROC || '';
                            var V_MFA_INV_CODE = row.V_MFA_INV_CODE || '';
                            var V_MFA_CLIENT_CODE = row.V_MFA_CLIENT_CODE || '';
                            var M_MFA_INVESTOR_NAME = row.M_MFA_INVESTOR_NAME || '';
                            var V_MFA_PAN = row.V_MFA_PAN || '';
                            var V_MFA_AH_CODE = row.V_MFA_AH_CODE || '';
                            var V_MFA_BUSI_CODE = row.V_MFA_BUSI_CODE || '';
                            var V_MFM_INV_CODE = row.V_MFM_INV_CODE || '';
                            var V_MFM_CLIENT_CODE = row.V_MFM_CLIENT_CODE || '';
                            var M_MFM_INVESTOR_NAME = row.M_MFM_INVESTOR_NAME || '';
                            var V_MFM_PAN = row.V_MFM_PAN || '';
                            var V_MFM_AH_CODE = row.V_MFM_AH_CODE || '';
                            var V_MFM_BUSI_CODE = row.V_MFM_BUSI_CODE || '';
                            var V_MF_LABEL42 = row.V_MF_LABEL42 || '';
                            var PX_INV_CODE = row.PX_INV_CODE || '';
                            var PX_INDEX = row.PX_INDEX || '';
                            var PX_CUR_FORM = row.PX_CUR_FORM || '';

                            console.log("Investor:", M_MFA_INVESTOR_NAME);
                            console.log("PAN:", V_MFA_PAN);

                            var message = "Row Data:\n\n";

                            for (var key in row) {
                                if (row.hasOwnProperty(key)) {
                                    message += `${key}: ${row[key]}\n`;
                                }
                            }

                            alert(message);

                            const inv_data = {
                                msg: row.MSG,
                                log_user: row.V_LOG_USER_ID,
                                main_code: row.V_MAIN_CODE,
                                upd_proc: row.V_UPD_PROC,
                                add_inv_code: row.V_MFA_INV_CODE,
                                add_client_code: row.V_MFA_CLIENT_CODE,
                                add_inv_name: row.V_MFA_INVESTOR_NAME,
                                add_pan: row.V_MFA_PAN,
                                add_ah_code: row.V_MFA_AH_CODE,
                                add_busi_code: row.V_MFA_BUSI_CODE,
                                mod_inv_code: row.V_MFM_INV_CODE,
                                mod_clinet_code: row.V_MFM_CLIENT_CODE,
                                mod_inv_name: row.V_MFM_INVESTOR_NAME,
                                mod_pan: row.V_MFM_PAN,
                                mod_ad_code: row.V_MFM_AH_CODE,
                                mod_busi_code: row.V_MFM_BUSI_CODE,
                                mod_inv_code: row.V_MF_LABEL42,
                                px_inv_code: row.PX_INV_CODE,
                                px_index: row.PX_INDEX,
                                px_form: row.PX_CUR_FORM

                            }
                            */

                            psmModelInvestorSearch_tvRowDbSetInvData(row);

                        } else {
                            alert(result.message || "No data found.");
                        }
                    } else {
                        alert("Invalid server response.");
                    }

                },
                error: function (xhr, status, error) {
                    // Hide loading indicator
                    hideLoading();

                    // Handle AJAX error
                    console.error("AJAX Error:", status, error);
                    showErrorMessage("Network error occurred. Please try again.");
                }
            });
        }

        function removeModalBackdropIfNoModals() {
            // Check if there are any open modals
            if ($('.modal.show').length === 0) {
                $('body').removeClass('modal-open'); // remove scrolling block
                $('.modal-backdrop').remove();       // remove the backdrop
            }
        }


        function psmModelInvestorSearch_tvRowDbSetInvData(data = null) {
            if (data) {

                var message = '';

                for (var key in data) {
                    if (data.hasOwnProperty(key)) {
                        message += `${key}: ${data[key]}\n`;
                    }
                }

                //alert('Before setting data: ' + message);


                //$("#txtAddTrDate").val(psmJs_formatDateToDMY(data.VA_IM_ENTRY_DT) || '');
                //$("#txtAddBssCode").val(data.V_MFA_BUSI_CODE || '');
                $("#txtAddInvCode").val(data.V_MFA_INV_CODE || '');
                $("#txtAddAHName").val(data.V_MFA_INVESTOR_NAME || '');
                $("#txtAddAHCode").val(data.V_MAIN_CODE || '');
                //$("#lblAddExpPer").html(data.VA_EXPENSE_PER || '');
                //$("#lblAddExpRs").html(data.VA_EXPENSE_RS || '');
                //$("#txtAddExpPer").val(data.VA_IM_N_EXP_PER || '');
                //$("#ddlAddBranch").val(data.VA_BUSI_BRANCH_CODE || '');
                //$("#ddlAddAMC").val(data.VA_AMC_CODE || '');
                //$("#txtAddScheme1").val(data.VA_SCH_NAME || '');
                //$("#hdnAddScheme1").val(data.VA_SCH_CODE || '');
                //$("#txtAddScheme2_fromSwitch").val(data.VA_SWITCH_SCH || '');
                //$("#hdnAddScheme2_fromSwitch").val(data.VA_SWITCH_SCH_CODE || '');
                $("#hdnAddClientCode").val(data.V_MFA_CLIENT_CODE || '');
                $("#hdnAddPan1").val(data.V_MFA_PAN || '');
                $("#txtAddTrDate").prop('disabled', false);
                $("#ddlAddAMC").prop('disabled', false);
                $("#txtAddScheme1").prop('disabled', false);


                for (var key in data) {
                    if (data.hasOwnProperty(key)) {
                        message += `${key}: ${data[key]}\n`;
                    }
                }

                //alert('After setting data: ' + message);

                var retMsg = (data.V_RETURN_MESSAGE || '');
                var curDT = $('#txtAddDtNumber').val();
                var curBsi = $("#txtAddBssCode").val();
                var curInv = $("#txtAddInvCode").val();

                //$('#psmModelInvestorSearch_btnExit').click();
                //$('#psmModelInvestorSearch').modal('hide');
                //$('#psmModelInvestorSearch').css('display', 'none');
                //psmJs_bsmModalToggle2('psmModalInvestorAddUpdate', 'hide');
                /*
                // Hide the modal the correct way
$('#psmModelInvestorSearch').modal('hide');

// Clean up if it's the last modal
$('#psmModelInvestorSearch').on('hidden.bs.modal', function () {
    if ($('.modal.show').length === 0) {
        $('body').removeClass('modal-open');
        $('.modal-backdrop').remove();
    }
});

                */

                //psmModalInvestorAddUpdateJs_GetSetData(curInv);
                //$("#psmModalInvestorAddUpdate").modal("show");
                //frmMf2Add_txtAddDtNumber_ClickGetSetBusiCodeData(curBsi, curInv);
                Toastify({
                    text: `Investor ${curInv} selected!`,
                    duration: 2500,
                    gravity: "top", // top or bottom
                    position: "right", // left, center or right
                    backgroundColor: "#B78939",
                }).showToast();


            } else {
                /*
                $("#txtAddTrDate").val('');
                $("#txtAddBssCode").val('');
                $("#txtAddInvCode").val('');
                $("#txtAddAHName").val('');
                $("#txtAddAHCode").val('');
                $("#lblAddExpPer").html('');
                $("#lblAddExpRs").html('');
                $("#txtAddExpPer").val('');
                $("#ddlAddBranch").val('');
                $("#ddlAddAMC").val('');
                $("#txtAddScheme1").val('');
                $("#hdnAddScheme1").val('');
                $("#txtAddScheme2_fromSwitch").val('');
                $("#hdnAddScheme2_fromSwitch").val('');
                $("#hdnAddClientCode").val('');
                $("#hdnAddPan1").val('');
                $("#txtAddTrDate").prop('disabled', true);
                $("#ddlAddAMC").prop('disabled', true);
                $("#txtAddScheme1").prop('disabled', true);
                frmMf2Add_txtAddDtNumber_ClickSetBusiCodeData(null);
                 */
                alert('No record found for this Investor');
            }
        }


        //#endregion HELPING FUNCTIONS: MODAL INVESTOR SEARCH
        function forceModalCleanup() {
            // Close all open modals forcibly
            $('.modal').removeClass('show').attr('aria-hidden', 'true').css('display', 'none');

            // Remove backdrop and modal-open class
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
        }


        $(document).ready(function () {

            // When #psmModelInvestorSearch finishes closing
            $('#psmModelInvestorSearch').on('hidden.bs.modal', function () {
                // Show the next modal
                $('#psmModalInvestorAddUpdate').modal('show');
            });

            //#region CALL ONLOAD: LOAD BRNACH, RM, COUNTRY, STATE, CITY ON MODEL OPEN
            $('#psmModelInvestorSearch').on('shown.bs.modal', function () {
                loadDropdownData('PSM_LOG_BRANCH_LIST', 'MF_PUNCHING', null, null, 'BRANCH_NAME', 'BRANCH_CODE', 'psmModelInvestorSearch_ddlBranch');
                loadDropdownData('PSM_LOG_RM_LIST', 'MF_PUNCHING', null, null, 'RM_NAME', 'RM_CODE', 'psmModelInvestorSearch_ddlRM');
                loadDropdownData('PSM_LOG_COUNTRY_STATE_CITY', 'CITY_LIST', null, null, 'CITY_NAME', 'CITY_ID', 'psmModelInvestorSearch_ddlCity');
            });
            //#endregion CALL ONLOAD

            //#region CALL ONCLICK: SEARCH INVESTOR

            $('#psmModelInvestorSearch_btnSearch').click(psmModelInvestorSearch_btnSearch_click);


            //#endregion CALL ONCLICK


            //#region CALL ONCHANGE
            $('#psmModelInvestorSearch_ddlBranch').on('change', function (e) {
                const crtBranchCode = $(this).val();
                if (crtBranchCode) {
                    loadDropdownData('PSM_LOG_RM_LIST', 'MF_PUNCHING', 'BRANCH', crtBranchCode, 'RM_NAME', 'RM_CODE', 'psmModelInvestorSearch_ddlRM');
                } else {
                    loadDropdownData('PSM_LOG_RM_LIST', 'MF_PUNCHING', null, null, 'RM_NAME', 'RM_CODE', 'psmModelInvestorSearch_ddlRM');
                }
            });

            //#endregion CALL ONCHANGE


            //#region CALL ONROWCLICK


            // SINGLE CLICK ON ROW
            $(document).on('click', '#psmModelInvestorSearch_tbResult tbody tr', function () {
                const $row = $(this);
                const $table = $('#psmModelInvestorSearch_tbResult');

                if ($row.data('doubleClickPending')) {
                    clearTimeout($row.data('doubleClickPending'));
                    $row.removeData('doubleClickPending');
                } else {
                    const timeout = setTimeout(function () {
                        psmJs_DynamicTableHighlightRow($table, $row);
                    }, 250);

                    $row.data('doubleClickPending', timeout);
                }
            });

            // DOUBLE CLICK ON ROW
            $(document).on('dblclick', '#psmModelInvestorSearch_tbResult tbody tr', function () {
                const $row = $(this);
                const $table = $('#psmModelInvestorSearch_tbResult');

                if ($row.data('doubleClickPending')) {
                    clearTimeout($row.data('doubleClickPending'));
                    $row.removeData('doubleClickPending');
                }

                psmJs_DynamicTableHighlightRow($table, $row);

                const rowData = [];
                $row.find('td').each(function () {
                    rowData.push($(this).text().trim());
                });

                //alert('Double Clicked Row data: ' + rowData.join(', '));

                var inv_code_cur = rowData[1] || '';

                var cur_cat = $("#psmModelInvestorSearch_ddlType").val() || '';

                psmModelInvestorSearch_tvRowDb(inv_code_cur, '0', cur_cat);

            });


            //#endregion CALL ONROWCLICK

        });
    </script>

    <%-- M.SCHEME SEARCH --%>
    <script>
        $(document).ready(function () {

            //#region MODEL HANDLER: SCHEME SEARCH

            $('#psmModalSchemeSearch').on('shown.bs.modal', function () {
                $('#psmModalSchemeSearch_txtSchemeName').trigger('focus');
            });

            function psmModalSchemeSearch_ajaxSeachScheme(params) {
                $.ajax({
                    type: 'POST',
                    url: "/masters/mf_punching_interface.aspx/ProcessSchemeSearch",
                    data: JSON.stringify({ pxSchStr: params }),
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    success: function (response) {
                        if (response && response.d) {
                            var result = JSON.parse(response.d);
                            if (result.success === true && result.data && result.data.length > 0) {
                                console.log("Data received:", result.data);

                                psmJs_DynamicTableGenerateTable(result, '#psmModalSchemeSearch_tbSchemeData', {
                                    countElement: '#psmModalSchemeSearch_lblResultCount',
                                    defaultColumnWidth: '250px',
                                    adjustableColumns: true,
                                    displayNoneToTheseColumns: '3,4'
                                });
                                return
                            } else {
                                alert(result.message || "No records found.");
                                psmJs_DynamicTableGenerateTable(result, '#psmModalSchemeSearch_tbSchemeData', {
                                    countElement: '#psmModalSchemeSearch_lblResultCount',
                                    defaultColumnWidth: '250px',
                                    adjustableColumns: true,
                                    displayNoneToTheseColumns: ''
                                });

                            }
                        } else {
                            alert("Invalid server response.");
                        }
                    },
                    error: function (xhr, status, error) {
                        alert('Error: ' + error);
                    }
                });
            }

            $('#psmModalSchemeSearch_btnSearch').on('click', function () {
                const schemeName = $('#psmModalSchemeSearch_txtSchemeName').val().trim();
                if (!schemeName) {
                    alert('Please enter a scheme name to search.');
                    return;
                }
                psmModalSchemeSearch_ajaxSeachScheme(schemeName);
            });


            function ajaxCallForSchemeSearchGetRow(params) {
                $.ajax({
                    type: 'POST',
                    url: "/masters/mf_punching_interface.aspx/ProcessSchemeSearchGetRow",
                    data: JSON.stringify({
                        pxRowData: params.pxRowData,
                        pxFrmStr: params.pxFrmStr,
                        pxAmcCol: params.pxAmcCol || null,
                        pxCol1: params.pxCol1 || null,
                        pxCol2: params.pxCol2 || null,
                        pxCol3: params.pxCol3 || null,
                        pxCol4: params.pxCol4 || null,
                        pxCol5: params.pxCol5 || null,
                        pxCol6: params.pxCol6 || null,
                        pxCol7: params.pxCol7 || null,
                        pxOther1: params.pxOther1 || null,
                        pxOther2: params.pxOther2 || null
                    }),
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    success: function (response) {
                        if (response && response.d) {
                            var result = JSON.parse(response.d);
                            if (result.success === true && result.data && result.data.length > 0) {
                                console.log("Data received:", result.data);
                                handleSchemeSearchDbClick(response, params.pxFrmStr);
                                return;
                            } else {

                            }
                        } else {
                            alert("Invalid server response.");
                        }
                    },
                    error: function (xhr, status, error) {
                        alert('Error: ' + error);
                    }
                });
            }

            function handleSchemeSearchDbClick(response, pxFrmStr) {
                if (response && response.d) {
                    var result = JSON.parse(response.d);

                    if (result.success === true && result.data && result.data.length > 0) {
                        var rowData = result.data[0]; // Get the first (and only) record

                        // Store data in separate variables based on form type
                        switch (pxFrmStr) {
                            case 'frmTransactionNew':
                                window.V_TR_NEW1_PROD = rowData.PROD_TYPE || '';
                                window.V_TR_NEW2_PRODUCT = rowData.PRODUCT || '';
                                window.V_TR_NEW3_LOG_NAME = rowData.LOG_NAME || '';
                                window.V_TR_NEW4_SCH = rowData.SCHEME_CODE || '';
                                console.log('Stored frmTransactionNew variables:',
                                    window.V_TR_NEW1_PROD, window.V_TR_NEW2_PRODUCT,
                                    window.V_TR_NEW3_LOG_NAME, window.V_TR_NEW4_SCH);
                                break;

                            case 'frmBrokRecdSlabs':
                                window.V_BROK_RECD1_PROD_CLASS = rowData.PROD_CLASS || '';
                                window.V_BROK_RECD2_MUT_FUND = rowData.MUT_FUND || '';
                                window.V_BROK_RECD3_LOG_NAME = rowData.LOG_NAME || '';
                                window.V_BROK_RECD4_SCH = rowData.SCHEME_CODE || '';
                                console.log('Stored frmBrokRecdSlabs variables:',
                                    window.V_BROK_RECD1_PROD_CLASS, window.V_BROK_RECD2_MUT_FUND,
                                    window.V_BROK_RECD3_LOG_NAME, window.V_BROK_RECD4_SCH);
                                break;

                            case 'frmBrokRecdSlabsRIO':
                                window.V_BROK_RECD_RIO1_PROD_CLASS = rowData.PROD_CLASS || '';
                                window.V_BROK_RECD_RIO2_MUT_FUND = rowData.MUT_FUND || '';
                                window.V_BROK_RECD_RIO3_LOG_NAME = rowData.LOG_NAME || '';
                                window.V_BROK_RECD_RIO4_SCH = rowData.SCHEME_CODE || '';
                                console.log('Stored frmBrokRecdSlabsRIO variables:',
                                    window.V_BROK_RECD_RIO1_PROD_CLASS, window.V_BROK_RECD_RIO2_MUT_FUND,
                                    window.V_BROK_RECD_RIO3_LOG_NAME, window.V_BROK_RECD_RIO4_SCH);
                                break;

                            case 'frmtransactionmf':
                                window.V_TR_MF1_A_AMC = rowData.AMC_CODE || '';
                                window.V_TR_MF3_SCH = rowData.SCHEME_NAME || '';
                                console.log('Stored frmtransactionmf variables:',
                                    window.V_TR_MF1_A_AMC, window.V_TR_MF3_SCH);
                                break;

                            case 'FrmNfoSchemes':
                                window.V_NFO_SCHEME1_ISSUE_DATE = rowData.ISSUE_DATE || '';
                                window.V_NFO_SCHEME2_CLOSE_DATE = rowData.CLOSE_DATE || '';
                                window.V_NFO_SCHEME3_T_SCHCD = rowData.SCHEME_CODE || '';
                                window.V_NFO_SCHEME3_T_SCHNM = rowData.SCHEME_NAME || '';
                                console.log('Stored FrmNfoSchemes variables:',
                                    window.V_NFO_SCHEME1_ISSUE_DATE, window.V_NFO_SCHEME2_CLOSE_DATE,
                                    window.V_NFO_SCHEME3_T_SCHCD, window.V_NFO_SCHEME3_T_SCHNM);
                                break;

                            default:
                                console.log('Unknown form type:', pxFrmStr);
                                return false;
                        }

                        return true; // Successfully stored data
                    } else {
                        console.log('No data found or error:', result.message);
                        return false;
                    }
                } else {
                    console.log('Invalid response format');
                    return false;
                }
            }

            // psmModalSchemeSearch_lblResultCount


            $('#psmModalSchemeSearch_tbSchemeData').on('dblclick', 'tr', function () {
                const $tds = $(this).children('td');
                const $row = $(this);
                const $table = $('#psmModalSchemeSearch_tbSchemeData');

                if ($tds.length < 7) {
                    console.warn("Row doesn't have enough columns");
                    return;
                }

                const rowData = $tds.map(function () {
                    return $(this).text().trim();
                }).get();

                psmJs_DynamicTableHighlightRow($table, $row);

                const params = {
                    pxRowData: rowData,
                    pxFrmStr: 'SchemeSearchForm', // Update as needed
                    pxAmcCol: null,               // Or extract if needed
                    pxCol1: rowData[0] || null,
                    pxCol2: rowData[1] || null,
                    pxCol3: rowData[2] || null,
                    pxCol4: rowData[3] || null,
                    pxCol5: rowData[4] || null,
                    pxCol6: rowData[5] || null,
                    pxCol7: rowData[6] || null,
                    pxOther1: null,
                    pxOther2: null
                };

                ajaxCallForSchemeSearchGetRow(params);
            });


            $('#psmModalSchemeSearch_btnReset').on('click', function () {
                var $openModal = $('.modal.show');
                if ($openModal.length) {
                    psmJs_bsmAutoReset($openModal);
                }
            });



            //#endregion MODEL HANDLER: SCHEME SEARCH

        });

    </script>


    <div class="container-fluid py-4">
        <div class="page-header mb-4">
            <h3 class="page-title">MF Trans Punching 2</h3>
        </div>

        <div class="row">
            <div class="col-md-12 grid-margin stretch-card">
                <div class="card">
                    <div class="card-body">
                        <!-- Tab Navigation -->
                        <ul class="nav nav-tabs wmTabs" id="wmTabs" role="tablist">
                            <li class="nav-item" role="presentation">
                                <a id="add_tab" class="nav-link active" href="#add_item" role="tab" data-bs-toggle="tab"
                                    aria-selected="true" aria-controls="add_item">Add</a>
                            </li>
                            <li class="nav-item" role="presentation">
                                <a id="view_modify_tab" class="nav-link" href="#view_modify" role="tab"
                                    data-bs-toggle="tab" aria-selected="false"
                                    aria-controls="view_modify">View/Modify</a>
                            </li>
                        </ul>

                        <!-- Tab Content -->
                        <div class="tab-content wmTabsContent mt-3" id="wmTabsContent">

                            <!-- TAB 1: ADD -->
                            <div class="tab-pane fade show active" id="add_item" role="tabpanel" aria-labelledby="add_tab">

                                <div class="row g-3">
                                    <!-- Investor/Scheme Details -->
                                    <div class="col-md-8">
                                        <fieldset class="border p-2">
                                            <legend class="card-title">Investor/Scheme Details</legend>
                                            <div class="row g-3">
                                                <%-- all opoup button for testing --%>
                                                <div class=" d-flex g-3 d-none">
                                                    <div class="col-md-3">
                                                        <button type="button" id="btnAddCrossChannelSearch"
                                                            data-bs-target="#psmModalCrossChannel"
                                                            class="btn btn-outline-primary btn-sm"
                                                            data-bs-toggle="modal">
                                                            Cross Search
                                                        </button>
                                                    </div>
                                                    <div class="col-md-3">
                                                        <button type="button" id="btnAddCrossInvestorAddUpdate"
                                                            data-bs-target="#psmModalInvestorAddUpdate"
                                                            class="btn btn-outline-primary btn-sm"
                                                            data-bs-toggle="modal">
                                                            Investor Add/Update
                                                        </button>
                                                    </div>

                                                    <div class="col-md-3">
                                                        <button type="button" id="btnAddCrossDuplicateCheck"
                                                            data-bs-target="#psmModalDuplicateCheck"
                                                            class="btn btn-outline-primary btn-sm"
                                                            data-bs-toggle="modal">
                                                            Duplicate Check
                                                        </button>
                                                    </div>

                                                    <div class="col-md-3">
                                                        <button type="button" id="btnAddSchemeFind"
                                                            data-bs-target="#psmModalSchemeSearch"
                                                            class="btn btn-outline-primary btn-sm"
                                                            data-bs-toggle="modal">
                                                            Scheme Search 1
                                                        </button>
                                                    </div>



                                                </div>

                                                <!-- DT Number Box & Button -->
                                                <div class="col-md-3">
                                                    <label for="txtAddDtNumber" class="form-label">DT Number</label>
                                                    <div class="input-group">
                                                        <input type="text" id="txtAddDtNumber" class="form-control" autocomplete="on" />
                                                        <button id="btnAddDtNumber"
                                                            class="btn btn-outline-primary p-0" style="width: 50px;">
                                                            <%--<i class="fas fa-search"></i>--%>
                                                            Get

                                                        </button>
                                                        <%-- containt fiod boostratp --%>
                                                    </div>
                                                </div>

                                                <!-- Transaction Date -->
                                                <div class="col-md-3">
                                                    <label class="form-label" for="txtAddTrDate">Transaction Date</label>
                                                    <input type="text" id="txtAddTrDate" class="form-control"
                                                        readonly />
                                                </div>

                                                <!-- Business Code -->
                                                <div class="col-md-3">
                                                    <label for="txtAddBssCode" class="form-label">Business Code</label>
                                                    <input type="text" id="txtAddBssCode" class="form-control" />
                                                </div>

                                                <!-- RM -->
                                                <div class="col-md-3">
                                                    <label for="txtAddRM" class="form-label">RM</label>
                                                    <input type="text" id="txtAddRM" class="form-control bg-light"
                                                        disabled />
                                                </div>

                                                <!-- Inv Code -->
                                                <div class="col-md-3">
                                                    <label for="txtAddInvCode" class="form-label">Inv Code</label>
                                                    <input type="text" id="txtAddInvCode" class="form-control bg-light" disabled />
                                                    <input type="hidden" id="hdnAddClientCode" class="form-control" />


                                                </div>

                                                <!-- Ana Code -->
                                                <div class="col-md-3 d-none">
                                                    <label class="form-label" for="txtAddAnaCode">Ana Code</label>
                                                    <input type="text" id="txtAddAnaCode" class="form-control bg-light"
                                                        disabled />
                                                </div>

                                                <!-- Account Holder -->
                                                <div class="col-md-6">
                                                    <label for="txtAddAHName" class="form-label">Account Holder</label>
                                                    <div class="input-group">
                                                        <input type="text" id="txtAddAHName"
                                                            class="form-control font-weight-bold"
                                                            value="INVESTOR NAME" />
                                                        <button type="button" id="btnAddInvestorSearch1"
                                                            class="btn btn-outline-primary p-0" style="width: 50px;"
                                                            data-bs-toggle="modal"
                                                            data-bs-target="#psmModelInvestorSearch">
                                                            <%--Search Investor--%>
                                                            <i class="fas fa-search"></i>


                                                        </button>
                                                    </div>
                                                </div>

                                                <!-- Account Holder Code -->
                                                <div class="col-md-3">
                                                    <label for="txtAddAHCode" class="form-label">A/C Holder Code</label>
                                                    <input type="text" id="txtAddAHCode" class="form-control"
                                                        disabled />
                                                </div>

                                                <!-- Branch Dropdown -->
                                                <div class="col-md-3">
                                                    <label for="ddlAddBranch" class="form-label">Branch</label>
                                                    <select id="ddlAddBranch" class="form-select">
                                                        <option value=""></option>
                                                    </select>
                                                </div>

                                                <!-- AMC Dropdown -->
                                                <div class="col-md-6">
                                                    <label for="ddlAddAMC" class="form-label">AMC</label>
                                                    <select id="ddlAddAMC" class="form-select">
                                                        <option value=""></option>
                                                        <option value="amc1">AMC 1</option>
                                                        <option value="amc2">AMC 2</option>
                                                    </select>
                                                </div>

                                                <!-- Transaction Type -->
                                                <div class="col-md-3 ">
                                                    <label for="ddlAddTransactionType" class="form-label">
                                                        Transaction
                                                        Type <span class="text-danger">*</span></label>
                                                    <select id="ddlAddTransactionType" class="form-select">
                                                        <option value=""></option>
                                                        <option value="PURCHASE">PURCHASE</option>
                                                        <option value="SWITCH IN">SWITCH IN</option>
                                                    </select>
                                                </div>

                                                <!-- Regular/NFO Radio Buttons -->
                                                <div class="col-md-3 ">
                                                    <label class="form-label">Regular/NFO</label>
                                                    <div class="d-flex align-items-center gap-2">
                                                        <div class="form-check ms-4">
                                                            <input type="radio" id="rdbAddRegular" name="rdbAddRN"
                                                                class="form-check-input" value="REGULAR" checked />
                                                            <label for="rdbAddRegular"
                                                                class="form-check-label ps-0 ms-0 me-4">
                                                                Regular</label>
                                                        </div>
                                                        <div class="form-check">
                                                            <input type="radio" id="rdbAddNFO" name="rdbAddRN"
                                                                class="form-check-input" value="NFO" />
                                                            <label for="rdbAddNFO" class="form-check-label ps-0 ms-0 me-4">NFO</label>
                                                        </div>
                                                    </div>
                                                </div>

                                                <!-- Scheme Search -->
                                                <div class="col-md-9">
                                                    <label for="txtAddScheme1" class="form-label">Scheme</label>
                                                    <div class="input-group">
                                                        <input type="text" id="txtAddScheme1" class="form-control" />
                                                        <input type="hidden" id="hdnAddScheme1" class="form-control" />
                                                        <button type="button" id="btnAddScheme1"
                                                            class="btn btn-outline-primary p-0" style="width: 50px;"
                                                            data-bs-toggle="modal"
                                                            data-bs-target="#psmModalSchemeSearch">
                                                            <i class="fas fa-search"></i>
                                                        </button>
                                                    </div>
                                                    <input type="hidden" id="hdnAddSchemeSearchState" value="0" />
                                                </div>


                                                <!-- From Switch/STP Folio -->
                                                <div class="col-md-12">
                                                    <fieldset class="border p-2">
                                                        <legend class="card-title">Switch/STP</legend>
                                                        <div class="row">
                                                            <div class="col-md-6">
                                                                <label for="txtAddformSwitchFolio"
                                                                    class="form-label">
                                                                    From Switch/STP Folio</label>
                                                                <input type="text" id="txtAddformSwitchFolio"
                                                                    class="form-control" />
                                                            </div>
                                                            <div class="col-md-6">
                                                                <label for="txtAddScheme2_fromSwitch"
                                                                    class="form-label">
                                                                    From Switch/STP Scheme</label>
                                                                <div class="input-group">
                                                                    <input type="text" id="txtAddScheme2_fromSwitch" class="form-control" />
                                                                    <input type="hidden" id="hdnAddScheme2_fromSwitch" class="form-control" />
                                                                    <button type="button" id="btnAddScheme2_fromSwitch"
                                                                        class="btn btn-outline-primary p-0" style="width: 50px;"
                                                                        data-bs-toggle="modal"
                                                                        data-bs-target="#psmModalSchemeSearch">

                                                                        <i class="fas fa-search"></i>

                                                                    </button>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </fieldset>
                                                </div>
                                            </div>
                                        </fieldset>
                                    </div>

                                    <!-- Application And Payment Details Details -->
                                    <div class="col-md-4">
                                        <fieldset class="border p-2">
                                            <legend class="card-title">Application And Payment Details</legend>
                                            <!-- <h4 class="card-title">Application And Payment Details</h4> -->
                                            <div class="row g-3">
                                                <!-- Application No -->
                                                <div class="col-md-4">
                                                    <label for="txtAddAppNo" class="form-label">Application No</label>
                                                    <input type="text" id="txtAddAppNo" class="form-control" />
                                                </div>

                                                <!-- Folio No -->
                                                <div class="col-md-4">
                                                    <label for="txtAddFolioNo" class="form-label">Folio No</label>
                                                    <input type="text" id="txtAddFolioNo" class="form-control" />
                                                </div>

                                                <!-- Amount -->
                                                <div class="col-md-4">
                                                    <label for="txtAddAmount" class="form-label">Amount</label>
                                                    <input type="text" id="txtAddAmount" class="form-control" />
                                                </div>


                                                <!-- Payment Method -->
                                                <div class="col-md-6">
                                                    <label for="ddlAddPaymentMode" class="form-label">
                                                        Payment
                                                        Method</label>
                                                    <select id="ddlAddPaymentMode" class="form-select">
                                                        <option value="CHEQUE" selected>Cheque</option>
                                                        <option value="DRAFT">Draft</option>
                                                        <option value="ECS">ECS</option>
                                                        <option value="CASH">Cash</option>
                                                        <option value="OTHERS">Others</option>
                                                        <option value="RTGS">RTGS</option>
                                                        <option value="FUND_TRANSFER">Fund Transfer</option>
                                                    </select>
                                                </div>


                                                <!-- Payment Details  -->
                                                <div class="col-md-6">
                                                    <label for="txtAddChequeNo" id="lblAddChequeNo" class="form-label">Cheque No</label>
                                                    <span class="text-danger">*</span>
                                                    <input type="text" id="txtAddChequeNo" class="form-control" />


                                                </div>

                                                <!-- Payment Details  -->
                                                <div class="col-md-6">
                                                    <label for="txtAddChequeDate" id="lblAddChequeDate" class="form-label">
                                                        Cheque
                                                        Dated</label>
                                                    <span class="text-danger">*</span>
                                                    <div class="date">
                                                        <input type="text" id="txtAddChequeDate" class="form-control" />
                                                    </div>
                                                </div>

                                                <!-- Bank Name -->
                                                <div class="col-md-6">
                                                    <label for="ddlAddBankName" class="form-label">Bank Name</label>
                                                    <select id="ddlAddBankName" class="form-select">
                                                        <option value=""></option>
                                                        <option value="bank1">Bank 1</option>
                                                        <option value="bank2">Bank 2</option>
                                                    </select>
                                                </div>

                                                <!-- Expenses Percentage -->
                                                <div class="col-md-6">
                                                    <label for="txtAddExpPer" id="lblAddExpPer" class="form-label">Expenses%</label>
                                                    <input type="text" id="txtAddExpPer" class="form-control" />
                                                </div>

                                                <!-- Expenses (Rs.) -->
                                                <div class="col-md-6">
                                                    <label for="txtAddExpRs" id="lblAddExpRs" class="form-label">Expenses (Rs.)</label>
                                                    <input type="text" id="txtAddExpRs" class="form-control" />
                                                </div>

                                                <!-- Auto Switch On Maturity -->
                                                <div class="col-md-12">
                                                    <div class="form-check mt-3 ms-4">
                                                        <input type="checkbox" id="chkAddAutoSwitchOnMaturity"
                                                            class="form-check-input" />
                                                        <label for="chkAddAutoSwitchOnMaturity"
                                                            class="form-check-label ps-0 ms-0">
                                                            Auto Switch On Maturity</label>
                                                    </div>
                                                </div>


                                                <!-- Auto Switch Scheme -->
                                                <div class="col-md-12 mt-4">
                                                    <label for="txtAddScheme3_Close" class="form-label"></label>
                                                    <div class="input-group">
                                                        <input type="text" id="txtAddScheme3_Close"
                                                            class="form-control" />
                                                        <input type="hidden" id="hdnAddScheme3_Close"
                                                            class="form-control" />
                                                        <button id="btnAddScheme3_Close"
                                                            class="btn btn-outline-primary p-0" style="width: 50px;"
                                                            data-bs-toggle="modal"
                                                            data-bs-target="#psmModalSchemeSearch">

                                                            <i class="fas fa-search"></i>
                                                        </button>
                                                    </div>
                                                </div>
                                            </div>
                                        </fieldset>

                                    </div>

                                    <!-- SIP Details -->
                                    <div class="col-md-12">
                                        <fieldset class="border p-2">
                                            <legend class="card-title">SIP Details</legend>
                                            <div class="row g-3">
                                                <!-- SIP/STP or Regular -->
                                                <div class="col-md-2">
                                                    <label for="ddlAddSipStp" class="form-label">SIP/STP</label>
                                                    <select id="ddlAddSipStp" class="form-select">
                                                        <option value=""></option>
                                                        <option value="REGULAR">REGULAR</option>
                                                        <option value="SIP">SIP</option>
                                                        <option value="STP">STP</option>
                                                    </select>
                                                </div>

                                                <!-- Installment Type -->
                                                <div class="col-md-2">
                                                    <label for="ddlAddInstallmentType" class="form-label">
                                                        Installment
                                                        Type</label>
                                                    <select id="ddlAddInstallmentType" class="form-select">
                                                        <option value=""></option>
                                                        <option value="NORMAL">NORMAL</option>
                                                        <option value="MICRO">MICRO</option>
                                                    </select>
                                                </div>

                                                <!-- SIP Type -->
                                                <div class="col-md-2">
                                                    <label for="ddlAddSipType" class="form-label">
                                                        SIP TYPE (Sub
                                                        SIP)</label>
                                                    <select id="ddlAddSipType" class="form-select">
                                                        <option value=""></option>
                                                        <option value="N">NORMAL</option>
                                                        <option value="Y">MICROSIP</option>
                                                    </select>
                                                </div>

                                                <!-- SIP Amount -->
                                                <div class="col-md-2">
                                                    <label for="txtAddSipAmount" class="form-label">SIP Amount</label>
                                                    <input type="text" id="txtAddSipAmount" class="form-control" />
                                                </div>

                                                <!-- Frequency-->
                                                <div class="col-md-2">
                                                    <label for="ddlAddFrequency" class="form-label">Frequency</label>
                                                    <select id="ddlAddFrequency" class="form-select">
                                                        <option value=""></option>
                                                        <option value="208">Daily</option>
                                                        <option value="173">Weekly</option>
                                                        <option value="174">Fortnightly</option>
                                                        <option value="175">Monthly</option>
                                                        <option value="176">Quarterly</option>
                                                        <option value="301">Yearly</option>
                                                    </select>
                                                </div>

                                                <!-- Installments Nos-->
                                                <div class="col-md-2">
                                                    <label for="txtAddInstallmentsNo" class="form-label">
                                                        Installments
                                                        Nos</label>
                                                    <input type="text" id="txtAddInstallmentsNo" class="form-control"
                                                        maxlength="4" />
                                                </div>

                                                <!-- SIP Start Date -->
                                                <div class="col-md-2">
                                                    <label for="txtAddSIPStartDate" class="form-label">SIP Start Date</label>
                                                    <div class="date">
                                                        <input type="text" id="txtAddSIPStartDate" class="form-control" />
                                                    </div>
                                                </div>

                                                <!-- SIP End Date -->
                                                <div class="col-md-2">
                                                    <label for="txtAddSIPEndDate" class="form-label">SIP End Date</label>
                                                    <div class="date">
                                                        <input type="text" id="txtAddSIPEndDate" class="form-control" />
                                                    </div>
                                                </div>

                                                <!-- Fresh/Renewal Radio Buttons  -->
                                                <div class="col-md-3">
                                                    <div class="d-flex align-items-between gap-3 mt-4">
                                                        <div class="form-check ms-4">
                                                            <input type="radio" id="rdbAddFresh"
                                                                name="rdbAddFreshRenewal" class="form-check-input "
                                                                value="FRESH" checked />
                                                            <label for="rdbAddFresh"
                                                                class="form-check-label ms-0">
                                                                Fresh</label>
                                                        </div>
                                                        <div class="form-check ms-4">
                                                            <input type="radio" id="rdbAddRenewal"
                                                                name="rdbAddFreshRenewal" class="form-check-input"
                                                                value="RENEWAL" />
                                                            <label for="rdbAddRenewal"
                                                                class="form-check-label ms-2">
                                                                Renewal</label>
                                                        </div>
                                                    </div>
                                                </div>

                                                <!-- COB, SWP, Freedom SIP, 99 Years -->
                                                <div class="col-md-4">
                                                    <div class="d-flex align-items-start gap-3 mt-4">
                                                        <div class="col-md-2">
                                                            <div class="form-check">
                                                                <input type="checkbox" id="chkAddCOBCase"
                                                                    name="chkAddCOB99Case" class="form-check-input" />
                                                                <label for="chkAddCOBCase" class="form-check-label ms-0">
                                                                    COB
                                                                    Case</label>
                                                            </div>
                                                        </div>
                                                        <div class="col-md-2">
                                                            <div class="form-check">
                                                                <input type="checkbox" id="chkAddSWPCase"
                                                                    name="chkAddCOB99Case" class="form-check-input" />
                                                                <label for="chkAddSWPCase" class="form-check-label ms-0">
                                                                    SWP
                                                                    Case</label>
                                                            </div>
                                                        </div>
                                                        <div class="col-md-3">
                                                            <div class="form-check">
                                                                <input type="checkbox" id="chkAddFreedomSIP"
                                                                    name="chkAddCOB99Case" class="form-check-input" />
                                                                <label for="chkAddFreedomSIP"
                                                                    class="form-check-label ms-0">
                                                                    Freedom SIP</label>
                                                            </div>
                                                        </div>
                                                        <div class="col-md-4">
                                                            <div class="form-check">
                                                                <input type="radio" id="chkAdd99Years"
                                                                    name="chkAddCOB99Case" class="form-check-input" />
                                                                <label for="chkAdd99Years" class="form-check-label ms-0">OR 99 Years + </label>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </fieldset>
                                    </div>

                                    <!-- PAN Details -->
                                    <div class="col-md-3">
                                        <fieldset class="border p-2">
                                            <legend class="card-title">PAN Details</legend>
                                            <div class="col-md-12">
                                                <div class="d-flex">
                                                    <label for="txtAddPan2" class="form-label" style="width: 120px;">PAN No</label>
                                                    <input type="text" id="txtAddPan2" class="form-control" maxlength="10" />
                                                    <input type="hidden" id="hdnAddPan1" class="form-control" maxlength="10" />

                                                </div>
                                            </div>
                                        </fieldset>
                                    </div>

                                    <!-- Action Buttons -->
                                    <div class="col-md-9">
                                        <fieldset class="border p-2">
                                            <legend class="card-title">Action </legend>
                                            <!-- Action Buttons -->
                                            <div class="col-md-12">
                                                <div class="d-flex align-items-md-center flex-wrap gap-3">
                                                    <button id="btnAddTransaction"
                                                        class="btn btn-outline-primary">
                                                        Add</button>
                                                    <button id="btnAddPrintAR" class="btn btn-outline-primary"
                                                        disabled>
                                                        Print AR</button>
                                                    <button id="btnAddReset"
                                                        class="btn btn-outline-primary">
                                                        Reset</button>
                                                    <button id="btnAddLeadSearch" class="btn btn-outline-primary"
                                                        disabled>
                                                        Lead Search</button>
                                                    <button id="btnAddChangeInvestorDetails"
                                                        data-bs-target="#psmModalInvestorAddUpdate"
                                                        data-bs-toggle="modal"
                                                        class="btn btn-outline-primary">
                                                        Change Investor Details</button>
                                                    <button id="btnAddExit"
                                                        class="btn btn-outline-primary">
                                                        Exit</button>
                                                </div>
                                            </div>
                                        </fieldset>
                                    </div>

                                </div>


                            </div>

                            <!-- TAB 2: VIEW/MODIFY -->
                            <div class="tab-pane fade" id="view_modify" role="tabpanel"
                                aria-labelledby="view_modify_tab">
                                <div class="row g-3">
                                    <div class="col-md-12">
                                        <fieldset class="border p-2">
                                            <legend class="card-title">Transaction Log</legend>
                                            <div class="col-md-12">
                                                <fieldset class="border p-2">
                                                    <legend class="card-title">Search AR</legend>
                                                    <div class="row g-3 mb-3">

                                                        <!-- From Date -->
                                                        <div class="col-md-2">
                                                            <label for="txtViewTrFromDate" class="form-label">From Date</label>
                                                            <div class="date">
                                                                <input type="text" id="txtViewTrFromDate"
                                                                    class="form-control " />
                                                            </div>
                                                        </div>

                                                        <!-- To Date -->
                                                        <div class="col-md-2">
                                                            <label for="txtViewTrToDate" class="form-label">To Date</label>
                                                            <div class="date">
                                                                <input type="text" id="txtViewTrToDate" class="form-control " />
                                                            </div>
                                                        </div>

                                                        <!-- PAN No -->
                                                        <div class="col-md-2">
                                                            <label for="txtViewTrPan" class="form-label">
                                                                PAN
                                                                No.</label>
                                                            <input type="text" id="txtViewTrPan"
                                                                class="form-control" maxlength="10" />
                                                        </div>

                                                        <!-- TR No -->
                                                        <div class="col-md-2">
                                                            <label for="txtViewTrTrNo" class="form-label">TR No.</label>
                                                            <input type="text" id="txtViewTrTrNo" class="form-control" />
                                                        </div>

                                                        <!-- Unique Client Code -->
                                                        <div class="col-md-2">
                                                            <label for="txtViewTrUniqueClientCode" class="form-label">
                                                                Unique
                                                                Client Code</label>
                                                            <input type="text" id="txtViewTrUniqueClientCode"
                                                                class="form-control" />
                                                        </div>

                                                        <!-- ANA Code -->
                                                        <div class="col-md-2">
                                                            <label for="txtViewTrAnaCode" class="form-label">ANA Code</label>
                                                            <input type="text" id="txtViewTrAnaCode" class="form-control" />
                                                        </div>

                                                        <!-- Cheque No -->
                                                        <div class="col-md-2">
                                                            <label for="txtViewTrChequeNo" class="form-label">Cheque No</label>
                                                            <input type="text" id="txtViewTrChequeNo" class="form-control" />
                                                        </div>

                                                        <!-- App No -->
                                                        <div class="col-md-2">
                                                            <label for="txtViewTrAppNo" class="form-label">App No</label>
                                                            <input type="text" id="txtViewTrAppNo" class="form-control" />
                                                        </div>


                                                        <!-- Order By -->
                                                        <div class="col-md-2">
                                                            <label for="ddlViewTrOrderBy" class="form-label">Order By</label>
                                                            <select id="ddlViewTrOrderBy" class="form-select">
                                                                <option value="">Select</option>
                                                                <option value="txtAddPan2o">PAN No.</option>
                                                                <option value="trNo">TR No.</option>
                                                                <option value="uniqueClientCode">Unique Client Code
                                                                </option>
                                                                <option value="anaCode">ANA Code</option>
                                                                <option value="chequeNo">Cheque No</option>
                                                                <option value="appNo">App No</option>
                                                            </select>
                                                        </div>

                                                        <!-- Ascending/Descending -->
                                                        <div class="col-md-4 mt-5">
                                                            <!-- <label class="form-label">Order</label> -->
                                                            <div class="d-flex align-items-center gap-3">
                                                                <div class="form-check ms-4">
                                                                    <input type="radio" id="txtViewTrAscending" name="txtViewTrOrder"
                                                                        class="form-check-input" value="ascending"
                                                                        checked />
                                                                    <label for="txtViewTrAscending"
                                                                        class="form-check-label ms-2">
                                                                        Ascending</label>
                                                                </div>
                                                                <div class="form-check ms-4">
                                                                    <input type="radio" id="rdbViewTrDescending" name="txtViewTrOrder"
                                                                        class="form-check-input" value="descending" />
                                                                    <label for="rdbViewTrDescending"
                                                                        class="form-check-label ms-2">
                                                                        Descending</label>
                                                                </div>
                                                            </div>
                                                        </div>


                                                        <!-- Search Button -->
                                                        <div class="col-md-2 mt-5">
                                                            <button id="btnViewTrSearchAR"
                                                                class="btn btn-primary w-100">
                                                                View</button>
                                                        </div>

                                                        <div class="col-md-4">
                                                            <label for="uniqueClientCode" class="form-label font-bold">
                                                                <b>Searching By Investor </b>
                                                            </label>
                                                            <div class="input-group">
                                                                <input type="text" id="txtViewSearAr2_uniqueClientCode"
                                                                    class="form-control" placeholder="Contact" />
                                                                <button class="btn btn-primary p-0" style="width: 50px;"
                                                                    type="button">
                                                                    Find</button>
                                                            </div>
                                                        </div>

                                                        <div class="col-md-6 mt-5">
                                                            <p><span class="text-danger">*</span> To Search any data click on the column to be searched and press Find</p>
                                                        </div>
                                                    </div>

                                                    <div id="Label20" class="text-danger"></div>
                                                    <!-- Search Results -->
                                                    <div class="table-responsive mt-3  " style="max-height: 250px">
                                                        <table id="tbViewTrTrDetails" class="table table-bordered">
                                                            <thead class="thead-dark">
                                                                <tr>
                                                                    <th>Investor Name</th>
                                                                    <th>Branch Name</th>
                                                                    <th>PAN No</th>
                                                                    <th>AMC Name</th>
                                                                    <th>Scheme Name</th>
                                                                    <th>Transaction Date</th>
                                                                    <th>Transaction Type</th>
                                                                    <th>Application No</th>
                                                                    <th>Payment Mode</th>
                                                                    <th>Cheque No</th>
                                                                    <th>Cheque Date</th>
                                                                    <th>Amount</th>
                                                                    <th>SIP/STP Type</th>
                                                                    <th>Lead No</th>
                                                                    <th>Lead Name</th>
                                                                    <th>Transaction Code</th>
                                                                    <th>Branch Code</th>
                                                                    <th>Business RM Code</th>
                                                                    <th>Scheme Code</th>
                                                                    <th>Source Code</th>
                                                                    <th>Bank Name</th>
                                                                    <th>RM Name</th>
                                                                    <th>Folio No</th>
                                                                    <th>Document ID</th>
                                                                    <th>Micro Investment</th>
                                                                    <th>Target Switch Scheme</th>
                                                                    <th>Target Scheme Name</th>
                                                                    <th>Switch Scheme Name</th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                <!-- Data will be populated dynamically -->
                                                            </tbody>
                                                        </table>
                                                    </div>

                                                </fieldset>
                                            </div>
                                        </fieldset>
                                    </div>
                                    <div class="col-md-12 mt-2">
                                        <fieldset class="border p-2">
                                            <legend class="card-title">Transaction Details</legend>
                                            <div class="col-md-12">
                                                <div class="row g-3">
                                                    <!-- Investor/Scheme Details -->
                                                    <div class="col-md-8">
                                                        <fieldset class="border p-2">
                                                            <legend class="card-title">Investor/Scheme Details</legend>
                                                            <div class="row g-3">

                                                                <!-- DT Number Box & Button -->
                                                                <%-- <div class="col-md-3">
                                                                    <label for="txtViewDtNumber" class="form-label">DT Number</label>
                                                                    <div class="input-group">
                                                                        <input type="text" id="txtViewDtNumber" class="form-control" />
                                                                        <button id="btnViewDtNumber"
                                                                            class="btn btn-outline-primary">Show</button>
                                                                    </div>
                                                                </div>--%>

                                                                <!-- Transaction Date -->
                                                                <div class="col-md-3">
                                                                    <label class="form-label" for="txtViewTrDate">Transaction Date</label>
                                                                    <input type="text" id="txtViewTrDate" class="form-control"
                                                                        readonly />
                                                                </div>

                                                                <!-- Business Code -->
                                                                <div class="col-md-3">
                                                                    <label for="txtViewBssCode" class="form-label">Business Code</label>
                                                                    <input type="text" id="txtViewBssCode" class="form-control" />
                                                                </div>

                                                                <!-- RM -->
                                                                <div class="col-md-3">
                                                                    <label for="txtViewRM" class="form-label">RM</label>
                                                                    <input type="text" id="txtViewRM" class="form-control bg-light"
                                                                        disabled />
                                                                </div>

                                                                <!-- Inv Code -->
                                                                <div class="col-md-3">
                                                                    <label for="txtViewInvCode" class="form-label">Inv Code</label>
                                                                    <input type="text" id="txtViewInvCode" class="form-control bg-light"
                                                                        disabled />
                                                                </div>

                                                                <!-- Ana Code -->
                                                                <div class="col-md-3 d-none">
                                                                    <label class="form-label" for="txtViewAnaCode">Ana Code</label>
                                                                    <input type="text" id="txtViewAnaCode" class="form-control bg-light"
                                                                        disabled />
                                                                </div>

                                                                <!-- Account Holder -->
                                                                <div class="col-md-6">
                                                                    <label for="txtViewAHName" class="form-label">Account Holder</label>
                                                                    <div class="input-group">
                                                                        <input type="text" id="txtViewAHName"
                                                                            class="form-control font-weight-bold"
                                                                            value="INVESTOR NAME" />
                                                                        <button type="button" id="btnViewInvestorSearch1"
                                                                            class="btn btn-outline-primary p-0" style="width: 50px;"
                                                                            data-bs-toggle="modal"
                                                                            data-bs-target="#psmModelInvestorSearch">
                                                                            <i class="fas fa-search"></i>

                                                                        </button>
                                                                    </div>
                                                                </div>

                                                                <!-- Account Holder Code -->
                                                                <div class="col-md-3">
                                                                    <label for="txtViewAHCode" class="form-label">A/C Holder Code</label>
                                                                    <input type="text" id="txtViewAHCode" class="form-control"
                                                                        disabled />
                                                                </div>

                                                                <!-- Branch Dropdown -->
                                                                <div class="col-md-3">
                                                                    <label for="ddlViewBranch" class="form-label">Branch</label>
                                                                    <select id="ddlViewBranch" class="form-select">
                                                                        <option value=""></option>
                                                                    </select>
                                                                </div>

                                                                <!-- AMC Dropdown -->
                                                                <div class="col-md-3">
                                                                    <label for="txtViewAMC" class="form-label">AMC</label>
                                                                    <select id="txtViewAMC" class="form-select">
                                                                        <option value=""></option>
                                                                        <option value="amc1">AMC 1</option>
                                                                        <option value="amc2">AMC 2</option>
                                                                    </select>
                                                                </div>

                                                                <!-- Transaction Type -->
                                                                <div class="col-md-3">
                                                                    <label for="ddlViewTransactionType" class="form-label">
                                                                        Transaction
                                                                        Type</label>
                                                                    <select id="ddlViewTransactionType" class="form-select">
                                                                        <option value=""></option>
                                                                        <option value="PURCHASE">PURCHASE</option>
                                                                        <option value="SWITCH IN">SWITCH IN</option>
                                                                    </select>
                                                                </div>

                                                                <!-- Regular/NFO Radio Buttons -->
                                                                <div class="col-md-3">
                                                                    <label class="form-label">Regular/NFO</label>
                                                                    <div class="d-flex align-items-center gap-3">
                                                                        <div class="form-check ms-4">
                                                                            <input type="radio" id="rdbViewRegular" name="rdbViewRN"
                                                                                class="form-check-input" value="REGULAR" checked />
                                                                            <label for="rdbViewRegular"
                                                                                class="form-check-label ms-2">
                                                                                Regular</label>
                                                                        </div>
                                                                        <div class="form-check ms-4">
                                                                            <input type="radio" id="rdbViewNFO" name="rdbViewRN"
                                                                                class="form-check-input" value="NFO" />
                                                                            <label for="rdbViewNFO" class="form-check-label ms-2">NFO</label>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                                <div class="col-md-6">
                                                                    <div class="">
                                                                        <label for="txtViewPan" class="form-label">PAN No</label>
                                                                        <input type="text" id="txtViewPan" class="form-control"
                                                                            maxlength="10" />
                                                                    </div>
                                                                </div>
                                                                <!-- Scheme Search -->
                                                                <div class="col-md-6">
                                                                    <label for="txtViewScheme1" class="form-label">Scheme</label>
                                                                    <div class="input-group">
                                                                        <input type="text" id="txtViewScheme1" class="form-control" />
                                                                        <input type="hidden" id="hdnViewScheme1" class="form-control" />
                                                                        <button type="button" id="btnViewScheme1"
                                                                            class="btn btn-outline-primary p-0" style="width: 50px;"
                                                                            data-bs-toggle="modal"
                                                                            data-bs-target="#psmModalSchemeSearch" disabled>
                                                                            <i class="fas fa-search"></i>
                                                                        </button>
                                                                    </div>
                                                                    <input type="hidden" id="hdnViewSchemeSearchState" value="0" />
                                                                </div>


                                                                <!-- From Switch/STP Folio -->
                                                                <div class="col-md-12">
                                                                    <fieldset class="border p-2">
                                                                        <legend class="card-title">Switch/STP</legend>
                                                                        <div class="row">
                                                                            <div class="col-md-6">
                                                                                <label for="txtViewformSwitchFolio"
                                                                                    class="form-label">
                                                                                    From Switch/STP Folio</label>
                                                                                <input type="text" id="txtViewformSwitchFolio"
                                                                                    class="form-control" />
                                                                            </div>
                                                                            <div class="col-md-6">
                                                                                <label for="txtViewScheme2_fromSwitch"
                                                                                    class="form-label">
                                                                                    From Switch/STP Scheme</label>
                                                                                <div class="input-group">
                                                                                    <input type="text" id="txtViewScheme2_fromSwitch" class="form-control" />
                                                                                    <input type="hidden" id="hdnViewScheme2_fromSwitch" class="form-control" />
                                                                                    <button type="button" id="btnViewScheme2_fromSwitch"
                                                                                        class="btn btn-outline-primary p-0" style="width: 50px;"
                                                                                        data-bs-toggle="modal"
                                                                                        data-bs-target="#psmModalSchemeSearch">
                                                                                        <i class="fas fa-search"></i>
                                                                                    </button>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </fieldset>
                                                                </div>
                                                            </div>
                                                        </fieldset>
                                                    </div>

                                                    <!-- Application And Payment Details Details -->
                                                    <div class="col-md-4">
                                                        <fieldset class="border p-2">
                                                            <legend class="card-title">Application And Payment Details</legend>
                                                            <!-- <h4 class="card-title">Application And Payment Details</h4> -->
                                                            <div class="row g-3">
                                                                <!-- Application No -->
                                                                <div class="col-md-4">
                                                                    <label for="txtViewAppNo" class="form-label">Application No</label>
                                                                    <input type="text" id="txtViewAppNo" class="form-control" />
                                                                </div>

                                                                <!-- Folio No -->
                                                                <div class="col-md-4">
                                                                    <label for="txtViewFolioNo" class="form-label">Folio No</label>
                                                                    <input type="text" id="txtViewFolioNo" class="form-control" />
                                                                </div>

                                                                <!-- Amount -->
                                                                <div class="col-md-4">
                                                                    <label for="txtViewAmount" class="form-label">Amount</label>
                                                                    <input type="text" id="txtViewAmount" class="form-control" />
                                                                </div>

                                                                <!-- Payment Method -->
                                                                <div class="col-md-4">
                                                                    <label for="ddlViewPaymentMode" class="form-label">
                                                                        Payment
                                                                        Method</label>
                                                                    <select id="ddlViewPaymentMode" class="form-select">
                                                                        <option value="CHEQUE" selected>Cheque</option>
                                                                        <option value="DRAFT">Draft</option>
                                                                        <option value="ECS">ECS</option>
                                                                        <option value="CASH">Cash</option>
                                                                        <option value="OTHERS">Others</option>
                                                                        <option value="RTGS">RTGS</option>
                                                                        <option value="FUND_TRANSFER">Fund Transfer</option>
                                                                    </select>
                                                                </div>


                                                                <!-- Payment Details  -->
                                                                <div class="col-md-4">
                                                                    <label for="txtViewChequeNo" id="lblViewChequeNo" class="form-label">Cheque No</label>
                                                                    <span class="text-danger">*</span>
                                                                    <input type="text" id="txtViewChequeNo" class="form-control" />


                                                                </div>

                                                                <!-- Payment Details  -->
                                                                <div class="col-md-4">
                                                                    <label for="txtViewChequeDate" id="lblViewChequeDate" class="form-label">
                                                                        Cheque
                                                                        Dated</label>
                                                                    <span class="text-danger">*</span>
                                                                    <div class="date">
                                                                        <input type="text" id="txtViewChequeDate" class="form-control" />
                                                                    </div>
                                                                </div>

                                                                <!-- Bank Name -->
                                                                <div class="col-md-6">
                                                                    <label for="ddlViewBankName" class="form-label">Bank Name</label>
                                                                    <select id="ddlViewBankName" class="form-select">
                                                                        <option value=""></option>
                                                                        <option value="bank1">Bank 1</option>
                                                                        <option value="bank2">Bank 2</option>
                                                                    </select>
                                                                </div>

                                                                <!-- Remark -->
                                                                <div class="col-md-6">
                                                                    <label for="txtViewRemarks" class="form-label">Remarks</label>
                                                                    <textarea id="txtViewRemarks" class="form-control" rows="1"></textarea>
                                                                </div>

                                                                <!-- Expenses Percentage -->
                                                                <div class="col-md-6">
                                                                    <label for="txtViewExpPer" class="form-label">Expenses%</label>
                                                                    <input type="text" id="txtViewExpPer" class="form-control"
                                                                        disabled />
                                                                </div>

                                                                <!-- Expenses (Rs.) -->
                                                                <div class="col-md-6">
                                                                    <label for="txtViewExpRs" class="form-label">Expenses (Rs.)</label>
                                                                    <input type="text" id="txtViewExpRs" class="form-control" disabled />
                                                                </div>


                                                                <!-- Auto Switch On Maturity -->
                                                                <div class="col-md-6 ">
                                                                    <div class="form-check mt-3 ms-4">
                                                                        <input type="checkbox" id="chkViewAutoSwitchOnMaturity" class="form-check-input" />
                                                                        <label for="chkViewAutoSwitchOnMaturity" class="form-check-label ms-2">Auto Switch On Maturity</label>
                                                                    </div>
                                                                </div>

                                                                <!-- Reco Status with danger asterisk -->
                                                                <div class="col-md-6">
                                                                    <div class="form-check mt-3">
                                                                        <label for="lblViewRecoStatus" id="lblViewRecoStatus" class="form-label text-uppercase text-danger">Reco Status</label>
                                                                    </div>
                                                                </div>

                                                                <!-- Auto Switch Scheme -->
                                                                <div class="col-md-12">
                                                                    <label for="txtViewScheme3_Close" class="form-label"></label>
                                                                    <div class="input-group">
                                                                        <input type="text" id="txtViewScheme3_Close"
                                                                            class="form-control" />
                                                                        <input type="hidden" id="hdnViewScheme3_Close"
                                                                            class="form-control" />
                                                                        <button id="btnViewScheme3_Close"
                                                                            class="btn btn-outline-primary p-0" style="width: 50px;"
                                                                            data-bs-toggle="modal"
                                                                            data-bs-target="#psmModalSchemeSearch">
                                                                            <i class="fas fa-search"></i>
                                                                        </button>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </fieldset>

                                                    </div>

                                                    <!-- SIP Details -->
                                                    <div class="col-md-12">
                                                        <fieldset class="border p-2">
                                                            <legend class="card-title">SIP Details</legend>
                                                            <div class="row g-3">

                                                                <!-- SIP/STP or Regular -->
                                                                <div class="col-md-2">
                                                                    <label for="ddlViewSipStp" class="form-label">SIP/STP</label>
                                                                    <select id="ddlViewSipStp" class="form-select">
                                                                        <option value=""></option>
                                                                        <option value="REGULAR">REGULAR</option>
                                                                        <option value="SIP">SIP</option>
                                                                        <option value="STP">STP</option>
                                                                    </select>
                                                                </div>

                                                                <!-- Installment Type -->
                                                                <div class="col-md-2">
                                                                    <label for="ddlViewInstallmentType" class="form-label">
                                                                        Installment
                                                                        Type</label>
                                                                    <select id="ddlViewInstallmentType" class="form-select">
                                                                        <option value=""></option>
                                                                        <option value="NORMAL">NORMAL</option>
                                                                        <option value="MICRO">MICRO</option>
                                                                    </select>
                                                                </div>

                                                                <!-- SIP Type -->
                                                                <div class="col-md-2">
                                                                    <label for="ddlViewSipType" class="form-label">
                                                                        SIP TYPE (Sub
                                                                        SIP)</label>
                                                                    <select id="ddlViewSipType" class="form-select">
                                                                        <option value=""></option>
                                                                        <option value="N">NORMAL</option>
                                                                        <option value="Y">MICROSIP</option>
                                                                    </select>
                                                                </div>

                                                                <!-- SIP Amount -->
                                                                <div class="col-md-2">
                                                                    <label for="txtViewSipAmount" class="form-label">SIP Amount</label>
                                                                    <input type="text" id="txtViewSipAmount" class="form-control" />
                                                                </div>

                                                                <!-- Drop Date -->
                                                                <div class="col-md-2">
                                                                    <label for="txtViewDropDate" class="form-label">Drop Date</label>
                                                                    <input type="text" id="txtViewDropDate" class="form-control" />
                                                                </div>

                                                                <!-- Frequency-->
                                                                <div class="col-md-2">
                                                                    <label for="ddlViewFrequency" class="form-label">Frequency</label>
                                                                    <select id="ddlViewFrequency" class="form-select">
                                                                        <option value=""></option>
                                                                        <option value="208">Daily</option>
                                                                        <option value="173">Weekly</option>
                                                                        <option value="174">Fortnightly</option>
                                                                        <option value="175">Monthly</option>
                                                                        <option value="176">Quarterly</option>
                                                                        <option value="301">Yearly</option>
                                                                    </select>
                                                                </div>

                                                                <!-- Installments Nos-->
                                                                <div class="col-md-2">
                                                                    <label for="txtViewInstallmentsNo" class="form-label">
                                                                        Installments
                                                                        Nos</label>
                                                                    <input type="text" id="txtViewInstallmentsNo" class="form-control"
                                                                        maxlength="4" />
                                                                </div>

                                                                <!-- SIP Start Date -->
                                                                <div class="col-md-2">
                                                                    <label for="txtViewSIPStartDate" class="form-label">SIP Start Date</label>
                                                                    <div class="date">
                                                                        <input type="text" id="txtViewSIPStartDate" class="form-control" />
                                                                    </div>
                                                                </div>

                                                                <!-- SIP End Date -->
                                                                <div class="col-md-2">
                                                                    <label for="txtViewSIPEndDate" class="form-label">SIP End Date</label>
                                                                    <div class="date">
                                                                        <input type="text" id="txtViewSIPEndDate" class="form-control" />
                                                                    </div>
                                                                </div>

                                                                <!-- Fresh/Renewal Radio Buttons  -->
                                                                <div class="col-md-2">
                                                                    <div class="d-flex align-items-between gap-3 mt-4  ">
                                                                        <div class="form-check ms-4">
                                                                            <input type="radio" id="rdbViewFresh"
                                                                                name="rdbViewFreshRenewal" class="form-check-input"
                                                                                value="FRESH" disabled />
                                                                            <label for="rdbViewFresh"
                                                                                class="form-check-label ms-2">
                                                                                Fresh</label>
                                                                        </div>
                                                                        <div class="form-check m-4">
                                                                            <input type="radio" id="rdbViewRenewal"
                                                                                name="rdbViewFreshRenewal" class="form-check-input"
                                                                                value="RENEWAL" disabled />
                                                                            <label for="rdbViewRenewal"
                                                                                class="form-check-label ms-2">
                                                                                Renewal</label>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                                <!-- COB, SWP, Freedom SIP, 99 Years -->
                                                                <div class="col-md-4">
                                                                    <div class="d-flex align-items-between gap-3 mt-3">
                                                                        <div class="col-md-2">
                                                                            <div class="form-check">
                                                                                <input type="checkbox" id="chkViewCOBCase"
                                                                                    name="chkViewCOB99Case" class="form-check-input" />
                                                                                <label for="chkViewCOBCase" class="form-check-label">
                                                                                    COB
                                                                                    Case</label>
                                                                            </div>
                                                                        </div>
                                                                        <div class="col-md-2">
                                                                            <div class="form-check">
                                                                                <input type="checkbox" id="chkViewSWPCase"
                                                                                    name="chkViewCOB99Case" class="form-check-input" />
                                                                                <label for="chkViewSWPCase" class="form-check-label">
                                                                                    SWP
                                                                                    Case</label>
                                                                            </div>
                                                                        </div>
                                                                        <div class="col-md-3">
                                                                            <div class="form-check">
                                                                                <input type="checkbox" id="chkViewFreedomSIP"
                                                                                    name="chkViewCOB99Case" class="form-check-input" />
                                                                                <label for="chkViewFreedomSIP"
                                                                                    class="form-check-label">
                                                                                    Freedom SIP</label>
                                                                            </div>
                                                                        </div>
                                                                        <div class="col-md-4">
                                                                            <div class="form-check">
                                                                                <input type="radio" id="chkView99Years"
                                                                                    name="chkViewCOB99Case" class="form-check-input" />
                                                                                <label for="chkView99Years" class="form-check-label">
                                                                                    OR
                                                                                    99 Years or more</label>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </fieldset>
                                                    </div>

                                                    <!-- PAN Details -->
                                                    <div class="col-md-4 d-none">
                                                        <fieldset class="border p-2">
                                                            <legend>PAN Details</legend>
                                                            <div class="col-md-12">
                                                                <div class="d-flex">
                                                                    <label for="txtViewPan2" class="form-label" style="width: 120px;">
                                                                        PAN
                                                                        No</label>
                                                                    <input type="text" id="txtViewPan2" class="form-control"
                                                                        maxlength="10" />
                                                                </div>
                                                            </div>
                                                        </fieldset>
                                                    </div>

                                                    <!-- Action Buttons -->
                                                    <div class="col-md-12">
                                                        <fieldset class="border p-2">
                                                            <legend class="card-title">Action </legend>
                                                            <!-- Action Buttons -->
                                                            <div class="col-md-12">
                                                                <div class="d-flex align-items-md-center flex-wrap gap-3">
                                                                    <button id="btnViewTransaction"
                                                                        class="btn btn-outline-primary">
                                                                        Modify</button>
                                                                    <button id="btnViewPrintAR" class="btn btn-outline-primary"
                                                                        disabled>
                                                                        Print Search AR</button>
                                                                    <button id="btnViewReset"
                                                                        class="btn btn-outline-primary">
                                                                        Reset</button>
                                                                    <button id="btnViewTranCancel" class="btn btn-outline-primary"
                                                                        disabled>
                                                                        Tran Cancel</button>
                                                                    <button id="btnViewViewLog"
                                                                        class="btn btn-outline-primary">
                                                                        View Log</button>
                                                                    <button id="btnViewExit"
                                                                        class="btn btn-outline-primary">
                                                                        Exit</button>
                                                                </div>
                                                            </div>
                                                        </fieldset>
                                                    </div>

                                                </div>
                                            </div>
                                        </fieldset>
                                    </div>
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



    <!-- Scheme Search Modal -->
    <div class="modal fade" id="psmModalSchemeSearch" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Search Scheme</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <hr />
                <div class="modal-body">
                    <div class="container-fluid common-scheme-form">

                        <!-- Search Row -->
                        <div class="row mb-3 d-flex justify-content-center">
                            <div class="col-md-12">
                                <div class="d-flex align-items-center gap-2" style="margin: 0 10%;">
                                    <label for="psmModalSchemeSearch_txtSchemeName" class="form-label mb-0">Find</label>

                                    <input type="text" id="psmModalSchemeSearch_txtSchemeName" class="form-control" maxlength="80" style="max-width: 300px; min-width: 150px">

                                    <button id="psmModalSchemeSearch_btnSearch" class="btn btn-primary">Go</button>
                                    <button id="psmModalSchemeSearch_btnReset" class="btn btn-secondary">Reset</button>
                                    <button id="psmModalSchemeSearch_btnExit" class="btn btn-secondary" data-bs-dismiss="modal">Exit</button>
                                    <label id="psmModalSchemeSearch_lblResultCount" class="text-danger p-3 font-weight-bold">Count</label>

                                </div>
                            </div>

                        </div>
                        <div class="col-md-12 mt-2">
                            <center>
                                <small class="text-muted">(Issuer/Mutual Fund OR Scheme Name OR Both)</small>
                            </center>
                        </div>
                    </div>

                    <!-- Table -->
                    <div class="card-body">
                        <div style="" class="mt-3 bg-white">
                            <table id="psmModalSchemeSearch_tbSchemeData" class="table table-bordered table-hover">
                            </table>
                        </div>
                    </div>

                </div>
            </div>


        </div>
    </div>

    <!-- Investor Master Search Modal -->
    <div class="modal fade" id="psmModelInvestorSearch" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">

                <!-- Header -->
                <div class="modal-header">
                    <h5 class="modal-title">Investor Search</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <hr />
                <!-- Body -->
                <div class="modal-body">
                    <!-- Search Form -->
                    <div class="row g-3 mb-3">
                        <div class="col-md-2">
                            <label class="form-label">Category</label>
                            <select id="psmModelInvestorSearch_ddlType" class="form-select">
                                <option value="CLIENT">Client</option>
                                <option value="INVESTOR" selected>Investor</option>
                                <option value="AGENT">Agent</option>
                            </select>
                        </div>

                        <div class="col-md-2">
                            <label class="form-label">Of</label>
                            <select id="psmModelInvestorSearch_ddlBroker" class="form-select">
                                <option value="" selected>All</option>
                                <option value="CLIENT" selected>Client</option>
                                <option value="SUB BROKER">Sub Broker</option>
                            </select>
                        </div>

                        <div class="col-md-2">
                            <label class="form-label">Branch</label>
                            <select id="psmModelInvestorSearch_ddlBranch" class="form-select">
                                <option value="">Select</option>
                            </select>
                        </div>

                        <div class="col-md-2">
                            <label class="form-label">RM</label>
                            <select id="psmModelInvestorSearch_ddlRM" class="form-select">
                                <option value="">Select</option>
                            </select>
                        </div>

                        <div class="col-md-2">
                            <label class="form-label">City</label>
                            <select id="psmModelInvestorSearch_ddlCity" class="form-select">
                            </select>
                        </div>

                        <div class="col-md-2">
                            <label class="form-label">PAN</label>
                            <input type="text" id="psmModelInvestorSearch_txtPanNo" maxlength="10" class="form-control text-uppercase search-input-on-enter" autocomplete="off">
                        </div>

                        <div class="col-md-4">
                            <label class="form-label">Name</label>
                            <input type="text" id="psmModelInvestorSearch_txtInvestorName" maxlength="100" autocomplete="off" class="form-control search-input-on-enter">
                        </div>


                        <div class="col-md-3">
                            <label class="form-label">Address 1</label>
                            <textarea id="psmModelInvestorSearch_txtAddress1" maxlength="250" class="form-control search-input-on-enter" rows="1"></textarea>
                        </div>

                        <div class="col-md-3">
                            <label class="form-label">Address 2</label>
                            <textarea id="psmModelInvestorSearch_txtAddress2" maxlength="250" class="form-control search-input-on-enter" rows="1"></textarea>
                        </div>

                        <div class="col-md-2">
                            <label class="form-label">Mobile</label>
                            <input type="text" id="psmModelInvestorSearch_txtMobile" maxlength="10" class="form-control search-input-on-enter">
                        </div>
                        <div class="col-md-2">
                            <label class="form-label">Phone</label>
                            <input type="text" id="psmModelInvestorSearch_txtPhone" maxlength="10" class="form-control search-input-on-enter">
                        </div>

                        <div class="col-md-2">
                            <label class="form-label">Client/SubBroker Code</label>
                            <input type="text" id="psmModelInvestorSearch_txtClientSubBrokerCode" class="form-control search-input-on-enter">
                        </div>

                        <div class="col-md-2">
                            <label class="form-label">Client/SubBroker Name</label>
                            <input type="text" id="psmModelInvestorSearch_txtClientSubBrokerName" class="form-control search-input-on-enter">
                        </div>




                        <div class="col-md-2">
                            <label class="form-label">AH Code</label>
                            <input type="text" id="psmModelInvestorSearch_txtAHCode" class="form-control search-input-on-enter">
                        </div>

                        <div class="col-md-2">
                            <label class="form-label">Sort By</label>
                            <select id="psmModelInvestorSearch_ddlSort" class="form-select">
                                <option value="NAME" selected>Name</option>
                                <option value="ADDRESS1">Address1</option>
                                <option value="ADDRESS2">Address2</option>
                                <option value="PHONE">PHONE</option>
                                <option value="CITY">City</option>
                            </select>
                        </div>

                        <div class="col-md-6">
                            <div class="mt-1">
                                <button id="psmModelInvestorSearch_btnSearch" class="btn btn-primary me-2">Search</button>
                                <button id="psmModelInvestorSearch_btnReset" class="btn btn-secondary me-2">Reset</button>
                                <button id="psmModelInvestorSearch_btnExit" class="btn btn-secondary" data-bs-dismiss="modal">Exit</button>
                                <label id="psmModelInvestorSearch_lblResultCount" class="text-danger p-3 font-weight-bold">Count</label>
                            </div>
                        </div>

                    </div>


                    <!-- Result Table -->
                    <div class="table-responsive " style="max-height: 320px;">
                        <table id="psmModelInvestorSearch_tbResult" class="table">
                        </table>
                    </div>

                    <!-- No Records -->
                    <div id="divNoRec" class="d-none">
                        <h5 class="text-danger">No Records Found</h5>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Cross Channel Alert Modal -->
    <div class="modal fade" id="psmModalCrossChannel" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">

                <!-- Header -->
                <div class="modal-header">
                    <h5 class="modal-title">Cross Channel Alert</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <hr />

                <!-- Body -->
                <div class="modal-body">
                    <!-- Message / Info -->
                    <div class="row mb-3">
                        <div class="col-12">
                            <label id="psmCrossChannel_lblMessage" class="text-primary d-block"></label>
                        </div>
                    </div>

                    <!-- Grid -->
                    <div class="table-responsive" style="max-height: 400px; overflow-y: auto; width: 100%;">
                        <table id="psmCrossChannel_gv" class="table table-hover">
                            <thead>
                                <tr>
                                    <th style="width: 250px;">CLIENT_CODE</th>
                                    <th style="width: 250px;">CLIENT_NAME</th>
                                    <th style="width: 250px;">RM_NAME</th>
                                    <th style="width: 250px;">PAN</th>
                                    <th style="width: 250px;">EMAIL</th>
                                    <th style="width: 250px;">MOBILE</th>
                                    <th style="width: 250px;">BRANCH</th>
                                    <th style="width: 250px;">ZONE</th>
                                    <th style="width: 250px;">REGION</th>
                                    <th style="width: 250px;">CHANNEL</th>
                                    <th style="width: 250px;">REMARK</th>
                                    <th style="width: 250px;">APPROVED</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td colspan="12" class="text-center">No data found</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>

                    <!-- No Records -->
                    <div id="psmCrossChannel_divNoRec" class="d-none">
                        <h5 class="text-danger">No Records Found</h5>
                    </div>
                </div>

                <!-- Footer -->
                <div class="modal-footer justify-content-center">
                    <button id="psmCrossChannel_btnContinue" class="btn btn-primary btn-sm me-2">Continue</button>
                    <button id="psmCrossChannel_btnApproval" class="btn btn-primary btn-sm">Send Approval</button>

                    <!-- Hidden fields -->
                    <input type="hidden" id="psmCrossChannel_Hidden1">
                    <input type="hidden" id="psmCrossChannel_IsContinue">
                    <input type="hidden" id="psmCrossChannel_VehSCE">
                    <input type="hidden" id="psmCrossChannel_ModelId">
                    <input type="hidden" id="psmCrossChannel_VariantId">
                </div>

            </div>
        </div>
    </div>

    <!-- Investor Address Updation Modal -->
    <div class="modal fade" id="psmModalInvestorAddUpdate" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">

                <!-- Header -->
                <div class="modal-header">
                    <h5 class="modal-title">Investor Address Updation</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <hr />

                <!-- Body -->
                <div class="modal-body">
                    <div class="row g-3">
                        <div class="col-md-6 d-none">
                            <label for="psmModalInvestorAddUpdate_txtInvCode" class="form-label">Investor Code</label>
                            <input type="text" id="psmModalInvestorAddUpdate_txtInvCode" maxlength="250"
                                class="form-control">
                        </div>

                        <div class="col-md-6 d-none">
                            <label for="psmModalInvestorAddUpdate_txtInvName" class="form-label">Investor Name</label>
                            <input type="text" id="psmModalInvestorAddUpdate_txtInvName" maxlength="250"
                                class="form-control">
                        </div>

                        <div class="col-md-5">
                            <label for="psmModalInvestorAddUpdate_txtAddress1" class="form-label">Address 1</label>
                            <input type="text" id="psmModalInvestorAddUpdate_txtAddress1" maxlength="250"
                                class="form-control">
                        </div>

                        <div class="col-md-5">
                            <label for="psmModalInvestorAddUpdate_txtAddress2" class="form-label">Address 2</label>
                            <input type="text" id="psmModalInvestorAddUpdate_txtAddress2" maxlength="250"
                                class="form-control">
                        </div>

                        <div class="col-md-2">
                            <label for="psmModalInvestorAddUpdate_txtPIN" class="form-label">PIN</label>
                            <input type="text" id="psmModalInvestorAddUpdate_txtPIN" maxlength="6" class="form-control">
                        </div>

                        <div class="col-md-6">
                            <label for="psmModalInvestorAddUpdate_txtEmail" class="form-label">Email</label>
                            <input type="text" id="psmModalInvestorAddUpdate_txtEmail" maxlength="150"
                                class="form-control">
                        </div>

                        <div class="col-md-3">
                            <label for="psmModalInvestorAddUpdate_txtPAN" class="form-label">PAN</label>
                            <input type="text" id="psmModalInvestorAddUpdate_txtPAN" maxlength="10"
                                class="form-control text-uppercase">
                        </div>

                        <div class="col-md-3">
                            <label for="psmModalInvestorAddUpdate_txtMobile" class="form-label">Mobile</label>
                            <input type="text" id="psmModalInvestorAddUpdate_txtMobile" maxlength="10"
                                class="form-control">
                        </div>

                        <div class="col-md-3">
                            <label for="psmModalInvestorAddUpdate_ddlCity" class="form-label">City</label>
                            <select id="psmModalInvestorAddUpdate_ddlCity" class="form-select">
                                <option value="">-- Select City --</option>
                            </select>
                        </div>

                        <div class="col-md-3">
                            <label for="psmModalInvestorAddUpdate_ddlState" class="form-label">State</label>
                            <select id="psmModalInvestorAddUpdate_ddlState" class="form-select">
                                <option value="">-- Select State --</option>
                            </select>
                        </div>

                        <div class="col-md-3">
                            <label for="psmModalInvestorAddUpdate_txtDOB" class="form-label">DOB</label>
                            <input type="text" id="psmModalInvestorAddUpdate_txtDOB" maxlength="10"
                                class="form-control psm-date-validate">
                        </div>

                        <div class="col-md-3">
                            <label for="psmModalInvestorAddUpdate_txtAadhar" class="form-label">Aadhar</label>
                            <input type="text" id="psmModalInvestorAddUpdate_txtAadhar" maxlength="12"
                                class="form-control">
                        </div>

                    </div>
                </div>

                <hr />

                <!-- Footer -->
                <div class="modal-footer d-flex justify-content-center">

                    <button id="psmModalInvestorAddUpdate_btnUpdate" class="btn btn-primary btn-sm">Update</button>


                    <button id="psmModalInvestorAddUpdate_btnExit" class="btn btn-secondary btn-sm" data-bs-dismiss="modal">Exit</button>
                </div>
            </div>
        </div>
    </div>

    <!-- Duplicate Check Alert Modal -->
    <div class="modal fade" id="psmModalDuplicateCheck" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">

                <!-- Header -->
                <div class="modal-header">
                    <h5 class="modal-title">Duplicate Check Alert</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <!-- Body -->
                <hr />
                <div class="modal-body">
                    <!-- Message / Info -->
                    <div class="row mb-3">
                        <div class="col-12">
                            <label id="psmDuplicateCheck_lblMessage" class="text-primary d-block"></label>
                        </div>
                    </div>

                    <!-- Grid -->
                    <div class="table-responsive" style="max-height: 400px; overflow-y: auto; width: 100%;">
                        <table id="psmDuplicateCheck_gv" class="table table-hover">
                            <thead>
                                <tr>
                                    <th style="width: 250px;">CLIENT_CODE</th>
                                    <th style="width: 250px;">CLIENT_NAME</th>
                                    <th style="width: 250px;">RM_NAME</th>
                                    <th style="width: 250px;">PAN</th>
                                    <th style="width: 250px;">EMAIL</th>
                                    <th style="width: 250px;">MOBILE</th>
                                    <th style="width: 250px;">BRANCH</th>
                                    <th style="width: 250px;">ZONE</th>
                                    <th style="width: 250px;">REGION</th>
                                    <th style="width: 250px;">CHANNEL</th>
                                    <th style="width: 250px;">REMARK</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td colspan="11" class="text-center">No data found</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>

                    <!-- No Records -->
                    <div id="psmDuplicateCheck_divNoRec" class="d-none">
                        <h5 class="text-danger">No Records Found</h5>
                    </div>
                </div>

                <!-- Footer -->
                <hr />
                <div class="modal-footer justify-content-center">
                    <button id="psmDuplicateCheck_btnContinue" class="btn btn-primary btn-sm me-2">Continue & Save</button>
                    <button id="psmDuplicateCheck_btnCancel" class="btn btn-primary btn-sm">Cancel</button>
                </div>

            </div>
        </div>
    </div>

    <!-- JavaScript Libraries -->
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://code.jquery.com/ui/1.13.0/jquery-ui.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/Pikaday/1.8.2/pikaday.min.js"></script>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/Pikaday/1.8.2/css/pikaday.min.css">
</asp:Content>
