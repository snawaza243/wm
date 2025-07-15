string printScript0 = $@"
var printWindow = window.open('', '', 'height=600,width=800');
printWindow.document.open();
printWindow.document.write(decodeURIComponent('{htmlContent}'));
printWindow.document.close();
printWindow.focus();

// Wait for the print window to load before triggering print
printWindow.onafterprint = function() {{
    // Close the print window after printing is finished
    //printWindow.close();
}};
printWindow.print();



";
