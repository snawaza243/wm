function handleTransactionTypeChange(tranTypeSelector, config) {
    const tranType = $(tranTypeSelector).val();

    // Reset common fields
    $(config.cmbSipStp).prop('selectedIndex', -1);
    $(config.sipStartDate).val(moment().format('DD/MM/YYYY'));
    $(config.sipEndDate).val('__/__/____');
    $(config.cmbSubSip).prop('selectedIndex', -1);
    $(config.cmbFrequency).prop('selectedIndex', -1);
    $(config.opt99).prop('checked', false);

    if (tranType === 'PURCHASE') {
        $(config.optChkSwitch).val(0);
        $(config.frame11).prop('disabled', true);
        $(config.label6_4).prop('disabled', true);
        $(config.label6_5).prop('disabled', true);

        $(config.cmbSipStp).prop('disabled', false);
        $(config.sipStartDate).prop('disabled', false);
        $(config.sipEndDate).prop('disabled', false);
        $(config.cmbSubSip).prop('disabled', false);
        $(config.cmbFrequency).prop('disabled', false);
        $(config.opt99).prop('disabled', false);

        $(config.switchFolio).val('');
        $(config.switchScheme).val('');
    } else if (tranType === 'SWITCH IN') {
        $(config.frame11).prop('disabled', false);
        $(config.optChkSwitch).hide();
        $(config.switchFolio).focus();
        $(config.cmbFrequency).prop('selectedIndex', -1);
        $(config.optOthers).prop('checked', true);

        $(config.label6_4).prop('disabled', false);
        $(config.label6_5).prop('disabled', false);

        $(config.cmbSipStp).prop('disabled', true);
        $(config.sipStartDate).prop('disabled', true);
        $(config.sipEndDate).prop('disabled', true);
        $(config.cmbSubSip).prop('disabled', true);
        $(config.cmbFrequency).prop('disabled', true);
        $(config.opt99).prop('disabled', true);
    }
}


$('#cmbTranTypeA').on('change', function () {
    handleTransactionTypeChange('#cmbTranTypeA', {
        cmbSipStp: '#CmbSipStpA',
        sipStartDate: '#ImSipStartDtA',
        sipEndDate: '#ImSipEndDtA',
        cmbSubSip: '#CmbSubSipA',
        cmbFrequency: '#CmbFrequency',
        opt99: '#Opt99',
        optChkSwitch: '#OptChkSwitch',
        frame11: '#Frame11',
        label6_4: '#Label6_4',
        label6_5: '#Label6_5',
        switchFolio: '#TxtSwitchFolioA',
        switchScheme: '#TxtSwitchSchemeA',
        optOthers: '#OptOthers'
    });
});