            string htmlContent0 = $@"
<!DOCTYPE html>
<html>
<head>
    <title>Policy Letter</title>
    <style>
        body {{ font-family: Arial, sans-serif; }}
        .header, .footer {{ text-align: left; margin-bottom: 20px; }}
        .content {{ margin-bottom: 30px; }}
        table {{ width: 100%; border-collapse: collapse; margin-top: 20px; }}
        table, th, td {{ border: 1px solid black; }}
        th, td {{ padding: 8px; text-align: left; }}
    </style>
</head>
<body>
    <div class='header'>
        <p><b>CLIENT_NAME:</b> {clientName}</p>
        <p><b>CLIENT_ADD1:</b> {address1}</p>
        <p><b>CLIENT_ADD2, CLIENT_ADD3:</b> {address2}, {clAdd3}</p>
        <p><b>CLIENT_ADD4, CLIENT_ADD5:</b> {clAdd4}, {clAdd5}</p>
        <p><b>CITY_NAME:</b> {cityName}</p>
        <p><b>STATE_NAME:</b> {stateName}</p>
        <p><b>PHONE 1:</b> {phone1}</p>
        <p><b>PHONE 2:</b> {phone2}</p>
    </div>
    <div class='content'>
        <table>
            <tr><th>Proposer Name</th><td>{clientName}</td><th>Life Insured Name</th><td>{iName}</td></tr>
            <tr><th>Insurer</th><td>{companyName}</td><th>Plan Name</th><td>{planName1}</td></tr>
            <tr><th>Policy Number</th><td>{policyNo}</td><th>Payment Frequency</th><td>{premFreq}</td></tr>
            <tr><th>Payment Mode</th><td>{payMode}</td><th>Basic Sum Assured</th><td>{sa}</td></tr>
            <tr><th>Premium Amount</th><td>{premAmt}</td><th>Premium Due Date</th><td>{dueDate}</td></tr>
            <tr><th>Service Unit Address</th><td>{branchName}</td><th>Cheque in favor of</th><td>{favourName}</td></tr>
        </table>
    </div>
    <div class='footer'>
        <p>* Please ensure to verify the exact premium amount with the Insurance Company on account of change in GST rates on Insurance Premium w.e.f. 1st Jul 2017</p>
        <p><b>Ref No:</b> {invCode}</p>
        <p>PAGE NO: 1</p>
    </div>
</body>
</html>";
 