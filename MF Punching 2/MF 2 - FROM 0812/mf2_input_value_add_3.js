$(document).ready(function() {
        // Variables to store field values
        let dtNumber, transactionDate, businessCode, rm, invCode, anaCode;
        let accountHolder, accountHolderCode, branch, amc, scheme, transactionType;
        let regularNfo, formSwitchFolio, formSwitchScheme, applicationNo, folioNo;
        let amount, paymentMode, chequeNo, chequeDate, bankName, expPercentage;
        let expRs, autoSwitchOnMaturity, formSwitchStpScheme, sipStp, installmentType;
        let sipType, sipAmount, frequency, installmentsNo, sipStartDate, sipEndDate;
        let freshRenewal, cobCase, swpCase, freedomSip, ninetyNineYears, panNo;

        // Store all field values in variables
        $("#btnStoreValues").on("click", function() {
            // Investor/Scheme Details
            dtNumber = $("#txtAddDtNumber").val();
            transactionDate = $("#txtAddTrDate").val();
            businessCode = $("#txtAddBssCode").val();
            rm = $("#txtAddRM").val();
            invCode = $("#txtAddInvCode").val();
            anaCode = $("#txtAddAnaCode").val();
            accountHolder = $("#txtAddAHName").val();
            accountHolderCode = $("#txtAddAHCode").val();
            branch = $("#ddlAddBranch").val();
            amc = $("#txtAddAMC").val();
            scheme = $("#txtAddScheme1").val();
            transactionType = $("#ddlAddTransactionType").val();
            regularNfo = $("input[name='rdbAddRN']:checked").val();
            formSwitchFolio = $("#txtAddformSwitchFolio").val();
            formSwitchScheme = $("#txtAddformSwitchScheme").val();

            // Application And Payment Details
            applicationNo = $("#txtAddAppNo").val();
            folioNo = $("#txtAddFolioNo").val();
            amount = $("#txtAddAmount").val();
            paymentMode = $("#txtAddPaymentMode").val();
            chequeNo = $("#txtAddChequeNo").val();
            chequeDate = $("#txtAddChequeDate").val();
            bankName = $("#ddlAddBankName").val();
            expPercentage = $("#txtAddExpPer").val();
            expRs = $("#txtAddExpRs").val();
            autoSwitchOnMaturity = $("#chkAddAutoSwitchOnMaturity").is(":checked");
            formSwitchStpScheme = $("#formSwitchScheme").val();

            // SIP Details
            sipStp = $("#ddlAddSipStp").val();
            installmentType = $("#ddlAddInstallmentType").val();
            sipType = $("#ddlAddSipType").val();
            sipAmount = $("#txtAddSipAmount").val();
            frequency = $("#ddlAddFrequency").val();
            installmentsNo = $("#txtAddInstallmentsNo").val();
            sipStartDate = $("#txtAddSIPStartDate").val();
            sipEndDate = $("#txtAddSIPEndDate").val();
            freshRenewal = $("input[name='rdbAddFreshRenewal']:checked").val();
            cobCase = $("#chkAddCOBCase").is(":checked");
            swpCase = $("#chkAddSWPCase").is(":checked");
            freedomSip = $("#chkAddFreedomSIP").is(":checked");
            ninetyNineYears = $("#chkAdd99Years").is(":checked");

            // PAN Details
            panNo = $("#txtAddPan2").val();

            // Log values to console for verification
            console.log("Stored field values:");
            console.log("DT Number:", dtNumber);
            console.log("Transaction Date:", transactionDate);
            console.log("Business Code:", businessCode);
            console.log("RM:", rm);
            console.log("Account Holder:", accountHolder);
            console.log("Amount:", amount);
            console.log("PAN No:", panNo);

            alert("All field values have been stored in variables!\nOpen browser console to see the values.");
        });

        // Display stored values
        $("#btnDisplayValues").on("click", function() {
            let html = `
                <div class="value-item"><span class="value-label">DT Number:</span> ${dtNumber || 'Not set'}</div>
                <div class="value-item"><span class="value-label">Transaction Date:</span> ${transactionDate || 'Not set'}</div>
                <div class="value-item"><span class="value-label">Business Code:</span> ${businessCode || 'Not set'}</div>
                <div class="value-item"><span class="value-label">RM:</span> ${rm || 'Not set'}</div>
                <div class="value-item"><span class="value-label">Inv Code:</span> ${invCode || 'Not set'}</div>
                <div class="value-item"><span class="value-label">Ana Code:</span> ${anaCode || 'Not set'}</div>
                <div class="value-item"><span class="value-label">Account Holder:</span> ${accountHolder || 'Not set'}</div>
                <div class="value-item"><span class="value-label">A/C Holder Code:</span> ${accountHolderCode || 'Not set'}</div>
                <div class="value-item"><span class="value-label">Branch:</span> ${branch || 'Not set'}</div>
                <div class="value-item"><span class="value-label">AMC:</span> ${amc || 'Not set'}</div>
                <div class="value-item"><span class="value-label">Scheme:</span> ${scheme || 'Not set'}</div>
                <div class="value-item"><span class="value-label">Transaction Type:</span> ${transactionType || 'Not set'}</div>
                <div class="value-item"><span class="value-label">Regular/NFO:</span> ${regularNfo || 'Not set'}</div>
                <div class="value-item"><span class="value-label">Form Switch Folio:</span> ${formSwitchFolio || 'Not set'}</div>
                <div class="value-item"><span class="value-label">Form Switch Scheme:</span> ${formSwitchScheme || 'Not set'}</div>
                <div class="value-item"><span class="value-label">Application No:</span> ${applicationNo || 'Not set'}</div>
                <div class="value-item"><span class="value-label">Folio No:</span> ${folioNo || 'Not set'}</div>
                <div class="value-item"><span class="value-label">Amount:</span> ${amount || 'Not set'}</div>
                <div class="value-item"><span class="value-label">Payment Mode:</span> ${paymentMode || 'Not set'}</div>
                <div class="value-item"><span class="value-label">Cheque No:</span> ${chequeNo || 'Not set'}</div>
                <div class="value-item"><span class="value-label">Cheque Date:</span> ${chequeDate || 'Not set'}</div>
                <div class="value-item"><span class="value-label">Bank Name:</span> ${bankName || 'Not set'}</div>
                <div class="value-item"><span class="value-label">Expenses %:</span> ${expPercentage || 'Not set'}</div>
                <div class="value-item"><span class="value-label">Expenses (Rs):</span> ${expRs || 'Not set'}</div>
                <div class="value-item"><span class="value-label">Auto Switch On Maturity:</span> ${autoSwitchOnMaturity ? 'Yes' : 'No'}</div>
                <div class="value-item"><span class="value-label">Form Switch/STP Scheme:</span> ${formSwitchStpScheme || 'Not set'}</div>
                <div class="value-item"><span class="value-label">SIP/STP:</span> ${sipStp || 'Not set'}</div>
                <div class="value-item"><span class="value-label">Installment Type:</span> ${installmentType || 'Not set'}</div>
                <div class="value-item"><span class="value-label">SIP Type:</span> ${sipType || 'Not set'}</div>
                <div class="value-item"><span class="value-label">SIP Amount:</span> ${sipAmount || 'Not set'}</div>
                <div class="value-item"><span class="value-label">Frequency:</span> ${frequency || 'Not set'}</div>
                <div class="value-item"><span class="value-label">Installments No:</span> ${installmentsNo || 'Not set'}</div>
                <div class="value-item"><span class="value-label">SIP Start Date:</span> ${sipStartDate || 'Not set'}</div>
                <div class="value-item"><span class="value-label">SIP End Date:</span> ${sipEndDate || 'Not set'}</div>
                <div class="value-item"><span class="value-label">Fresh/Renewal:</span> ${freshRenewal || 'Not set'}</div>
                <div class="value-item"><span class="value-label">COB Case:</span> ${cobCase ? 'Yes' : 'No'}</div>
                <div class="value-item"><span class="value-label">SWP Case:</span> ${swpCase ? 'Yes' : 'No'}</div>
                <div class="value-item"><span class="value-label">Freedom SIP:</span> ${freedomSip ? 'Yes' : 'No'}</div>
                <div class="value-item"><span class="value-label">99 Years:</span> ${ninetyNineYears ? 'Yes' : 'No'}</div>
                <div class="value-item"><span class="value-label">PAN No:</span> ${panNo || 'Not set'}</div>
            `;

            $("#valuesContainer").html(html);
        });

        // Clear values display
        $("#btnClearValues").on("click", function() {
            $("#valuesContainer").empty();
            $("#valuesContainer").html("<p class='text-muted'>No values displayed. Click 'Display Stored Values' to show them.</p>");
        });
    });