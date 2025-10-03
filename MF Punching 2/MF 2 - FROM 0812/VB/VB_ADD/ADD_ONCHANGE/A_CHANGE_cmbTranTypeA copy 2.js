 function frmMf2_TranTypeChange(index) {
     const prefix = index === 0 ? 'Add' : 'View';

     const today = new Date();
     const dd = String(today.getDate()).padStart(2, '0');
     const mm = String(today.getMonth() + 1).padStart(2, '0'); // Months start at 0
     const yyyy = today.getFullYear();

     const currentDate = `${dd}/${mm}/${yyyy}`;

     // Reset dropdowns and fields
     $(`#ddl${prefix}SipStp`).val('');
     $(`#txt${prefix}SIPStartDate`).val(currentDate || ''); // Set current date
     $(`#txt${prefix}SIPEndDate`).val('');
     $(`#ddl${prefix}InstallmentType`).val('');
     $(`#ddl${prefix}Frequency`).val('');
     $(`#chk${prefix}99Years`).prop('checked', false);

     const tranType = $(`#ddl${prefix}TransactionType`).val();

     if (tranType === 'PURCHASE') {
         // Disable switch section
         //$(`#frame${prefix}SwitchSection`).prop('disabled', true).hide();
         $(`#frame${prefix}SwitchSection`).prop('disabled', true);


         // Extra 
         //$(`#txt${prefix}formSwitchFolio, #txt${prefix}Scheme2_fromSwitch`).prop('disabled', true);
         $(`#txt${prefix}formSwitchFolio`).val('');
         $(`#txt${prefix}Scheme2_fromSwitch`).val('');
         $(`#hdn${prefix}Scheme2_fromSwitch`).val('');

         // Disable labels (if IDs exist)
         $(`#lbl${prefix}SwitchLabel1, #lbl${prefix}SwitchLabel2`).prop('disabled', true);

         // Enable SIP-related fields
         $(`#ddl${prefix}SipStp`).prop('disabled', false);
         $(`#txt${prefix}SIPStartDate`).prop('disabled', false);
         $(`#txt${prefix}SIPEndDate`).prop('disabled', false);
         $(`#ddl${prefix}InstallmentType`).prop('disabled', false);
         $(`#ddl${prefix}Frequency`).prop('disabled', false);
         $(`#chk${prefix}99Years`).prop('disabled', false);

         // Clear switch fields
         $(`#txt${prefix}formSwitchFolio`).val('');
         $(`#hdn${prefix}Scheme2_fromSwitch`).val('');

     } else if (tranType === 'SWITCH IN') {
         // Enable switch section
         //$(`#frame${prefix}SwitchSection`).prop('disabled', false).show();
         $(`#frame${prefix}SwitchSection`).prop('disabled', false);

         //$(`#txt${prefix}formSwitchFolio, #txt${prefix}Scheme2_fromSwitch`).prop('disabled', false);


         // Enable labels (if IDs exist)
         $(`#lbl${prefix}SwitchLabel1, #lbl${prefix}SwitchLabel2`).prop('disabled', true); // Match behavior from VB

         // Focus on Switch Folio input
         $(`#txt${prefix}formSwitchFolio`).focus();

         


         // Reset frequency
         $(`#ddl${prefix}Frequency`).val('');

         // Assume "Others" radio button
         $(`#rdb${prefix}Others`).prop('checked', true);

         // Disable SIP-related fields
         $(`#ddl${prefix}SipStp`).prop('disabled', true);
         $(`#txt${prefix}SIPStartDate`).prop('disabled', true);
         $(`#txt${prefix}SIPEndDate`).prop('disabled', true);
         $(`#ddl${prefix}InstallmentType`).prop('disabled', true);
         $(`#ddl${prefix}Frequency`).prop('disabled', true);
         $(`#chk${prefix}99Years`).prop('disabled', true);
     }
 }
