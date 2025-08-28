function calculateSipEndDateByFields(startDateField, installmentField, frequencyField, opt99Field, endDateField) {
    const startDateStr = $(startDateField).val();
    const installments = parseInt($(installmentField).val(), 10);
    const frequencyText = $(frequencyField).val();
    const opt99 = $(opt99Field).is(':checked');

    const dateFormat = 'DD/MM/YYYY';
    let endDateStr = '__/__/____';

    if (opt99 && startDateStr !== '__/__/____') {
        $(endDateField).val('01/12/2099');
        return;
    }

    if (frequencyText && !opt99 && startDateStr !== '__/__/____') {
        const freqParts = frequencyText.split('#');
        const freqCode = freqParts[1];
        const startDate = moment(startDateStr, dateFormat);

        let endDate;

        switch (freqCode) {
            case '208': // Daily (approximate)
                endDate = startDate.clone().add(Math.floor(installments / 30), 'months');
                break;
            case '173': // Weekly
                endDate = startDate.clone().add(installments, 'weeks');
                break;
            case '174': // Half-yearly
                endDate = startDate.clone().add(installments * 6, 'months');
                break;
            case '175': // Monthly
                const maxInstallments = moment('01/12/2099', dateFormat).diff(startDate, 'months');
                if (installments < maxInstallments) {
                    endDate = startDate.clone().add(installments, 'months');
                } else {
                    $(endDateField).val('01/12/2099');
                    return;
                }
                break;
            case '176': // Quarterly
                endDate = startDate.clone().add(installments * 3, 'months');
                break;
            case '301': // Yearly
                endDate = startDate.clone().add(installments * 12, 'months');
                break;
            default:
                endDateStr = '__/__/____';
        }

        if (endDate) {
            endDate.subtract(1, 'days'); // Match VB logic
            endDateStr = endDate.format(dateFormat);
        }

        $(endDateField).val(endDateStr);
    }
}


$('#CmbFrequency').on('change', function () {
    calculateSipEndDateByFields(
        '#ImSipStartDtA',
        '#TxtInstallmens',
        '#CmbFrequency',
        '#Opt99',
        '#ImSipEndDtA'
    );
});