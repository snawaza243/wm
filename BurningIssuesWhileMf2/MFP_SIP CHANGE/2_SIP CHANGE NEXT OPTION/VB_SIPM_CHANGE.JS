function handleSipStpChange(elementId, options = {}) {
    // Default element IDs (can be overridden by options)
    const config = {
        cmbSipStpId: elementId,
        imSipStartDtId: 'ImSipStartDtM',
        imSipEndDtId: 'ImSipEndDtM',
        label8Id: 'Label2_8',
        label10Id: 'Label2_10',
        cmbFrequencyId: 'CmbFrequency1',
        opt991Id: 'Opt991',
        txtInstallmentsId: 'Txtinstallments1',
        frame3Id: 'Frame3',
        frame12Id: 'Frame12',
        optChkSwitchId: 'OptChkSwitchM',
        txtSwitchFolioId: 'TxtSwitchFolioM',
        optSIPRId: 'OptSIPRM',
        optSIPFId: 'OptSIPFM',
        chkCloseId: 'ChkCloseM'
    };
    
    // Merge with provided options
    Object.assign(config, options);
    
    // Get elements
    const cmbSipStp = document.getElementById(config.cmbSipStpId);
    const imSipStartDt = document.getElementById(config.imSipStartDtId);
    const imSipEndDt = document.getElementById(config.imSipEndDtId);
    const label8 = document.getElementById(config.label8Id);
    const label10 = document.getElementById(config.label10Id);
    const cmbFrequency = document.getElementById(config.cmbFrequencyId);
    const opt991 = document.getElementById(config.opt991Id);
    const txtInstallments = document.getElementById(config.txtInstallmentsId);
    const frame3 = document.getElementById(config.frame3Id);
    const frame12 = document.getElementById(config.frame12Id);
    const optChkSwitch = document.getElementById(config.optChkSwitchId);
    const txtSwitchFolio = document.getElementById(config.txtSwitchFolioId);
    const optSIPR = document.getElementById(config.optSIPRId);
    const optSIPF = document.getElementById(config.optSIPFId);
    const chkClose = document.getElementById(config.chkCloseId);
    
    if (!cmbSipStp) return; // Exit if main element not found
    
    const selectedValue = cmbSipStp.value || cmbSipStp.textContent || '';
    
    // Hide elements initially
    [imSipStartDt, imSipEndDt, label8, label10].forEach(el => {
        if (el) el.style.display = 'none';
    });
    
    // Handle SIP/STP selection
    if (selectedValue === "SIP" || selectedValue === "STP") {
        if (cmbFrequency) cmbFrequency.disabled = false;
        if (opt991) opt991.disabled = false;
        if (txtInstallments) txtInstallments.disabled = false;
        
        [imSipStartDt, imSipEndDt, label8, label10].forEach(el => {
            if (el) el.style.display = 'block';
        });
    } else {
        if (cmbFrequency) {
            cmbFrequency.selectedIndex = -1;
            cmbFrequency.disabled = true;
        }
        if (txtInstallments) {
            txtInstallments.value = "";
            txtInstallments.disabled = true;
        }
        if (opt991) opt991.disabled = true;
    }
    
    // Handle STP specific logic
    if (selectedValue === "STP") {
        if (frame3) frame3.style.display = 'none';
        if (frame12) frame12.style.display = 'block';
        if (optChkSwitch) optChkSwitch.style.display = 'block';
        if (txtSwitchFolio) txtSwitchFolio.focus();
    }
    
    // Handle SIP specific logic
    if (selectedValue === "SIP") {
        if (optSIPR) optSIPR.style.display = 'block';
        if (optSIPF) {
            optSIPF.style.display = 'block';
            optSIPF.checked = true;
        }
        [imSipStartDt, imSipEndDt, label8, label10].forEach(el => {
            if (el) el.style.display = 'block';
        });
    } else {
        if (optSIPF) optSIPF.checked = true;
        if (optSIPR) optSIPR.style.display = 'none';
        if (optSIPF) optSIPF.style.display = 'none';
    }
    
    // Trigger chkClose click if exists
    if (chkClose && typeof chkClose.click === 'function') {
        chkClose.click();
    }
}

// Usage example:
// Add event listener to your combobox:
// document.getElementById('CmbSipStpM').addEventListener('change', function() {
//     handleSipStpChange('CmbSipStpM');
// });

// Or with custom element IDs:
// handleSipStpChange('CmbSipStpM', {
//     imSipStartDtId: 'customStartDate',
//     imSipEndDtId: 'customEndDate'
// });