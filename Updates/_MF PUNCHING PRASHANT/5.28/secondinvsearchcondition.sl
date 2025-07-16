 if (sessionStorage.getItem("invSearch2") === "true") {
     $("#btnInvSearch2ForTr").trigger("click"); // server-side button click
     sessionStorage.removeItem("invSearch2");
     return;
 }
 else {
     // fill address details
     $("[id*='txtAddressADD']").val(address1);
     $("[id*='txtAddressADD2']").val(address2);
     $("[id*='invcode']").val(invCode); // invcode
     $("[id*='ddlCityADD']").val(cityId);
     $("[id*='ddlStateADD']").val(stateId);
     $("[id*='txtPinADD']").val(pincode);
     $("[id*='txtMobileADD']").val(mobile);
     $("[id*='txtPanADD']").val(pan);
     $("[id*='TextAadhar']").val(aadharNo);
     $("[id*='txtDOBADD']").val(dob);
     //$("[id*='invcode']").val(invCode);
     var cleanedName = investorName.replace(/\s*\(Client\)$/, ""); // Removes " (Client)" from the end
     $("[id*='accountHolder']").val(cleanedName);
     $("[id*='holderCode']").val(ahClientCode);

     $("[id*='businessCode']").val(BusinessCodeInMod);
     $("[id*='RMNAMEP']").val(RmNameInMod);
     $("[id*='branch']").val(BRANCHCODE);

     $(this).closest('.modal').find("button[data-bs-dismiss='modal']").trigger('click');

     // Open the Address Updation modal
     //$("#addresModal").modal("show");
    showPopup3();
}