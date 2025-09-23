//#region ONLOAD FUNCTIONS

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

$(document).ajaxStart(function () {
    checkUserSession();
    showLoader();
});

$(document).ajaxStop(function () {
    hideLoader();
});

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

preventDefaultActions(true, true);

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

// Auto-format enabled, default to today enabled
//validateDateField('#ImSipStartDtA', true, true);

// Only validation, no auto-format or default
//validateDateField('#ImSipEndDtA');


function handleTabsPersistence(tabContainerId, storageKey) {
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

function makeBootstrapModalsDraggable() {
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

function enableModalAutoReset(modal) {

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

function managePersistentModalSession() {
    const storageKey = "psmModelOpen1";

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

handleTabsPersistence("#wmTabs", "mf2_activeTab");
makeBootstrapModalsDraggable();
managePersistentModalSession();

function handleSchemeTranType(dropdownId) {
    var value = document.querySelector(dropdownId).value;

    if (!value || value.trim() === "") {
        alert("Please select a transaction type before searching.");
        return false;
    }

    return true;
}


function handlePaymentChange(ddlChe, lblCheNo, lblCheDt, txtChecValue, txtCheDt) {
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

handlePaymentChange("#ddlAddPaymentMode", "#lblAddChequeNo", "#lblAddChequeDate", "#txtAddChequeNo", "#txtAddChequeDate");

handlePaymentChange("#ddlViewPaymentMode", "#lblViewChequeNo", "#lblViewChequeDate", "#txtViewChequeNo", "#txtViewChequeDate");

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

$('#psmModalSchemeSearch_btnReset').on('click', function () {
    var $openModal = $('.modal.show');
    if ($openModal.length) {
        enableModalAutoReset($openModal);
    }
});

//#endregion ON LAOD FUNCTIONS
