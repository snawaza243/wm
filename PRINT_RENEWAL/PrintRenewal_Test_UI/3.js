function handleData(type, data) {
    let allPagesHTML = "";

    data.forEach(function (row) {
        let newData = {
            sourceCodes: row.SOURCECODE1 || '',
            clientNames: row.CLIENT_NAME1 || '',
            add1List: row.ADD1 || '',
            add2List: row.ADD2 || '',
            cityCodes: row.CITY_CD || '',
            mutNames: row.MUTNAME || '',
            arDates: formatDateToDMY(row.ARDATE) || '',
            cityNames: row.CITY_NAME || '',
            pinCodes: row.PINCODE || ''
        };

        // Instead of directly opening popup, we collect HTML for each record
        allPagesHTML += buildLetterHTML(type, newData);
    });

    // Once all records are built, open popup only once
    openPopupForAllPages(allPagesHTML);
}

function buildLetterHTML(type, {
    sourceCodes,
    clientNames,
    add1List,
    add2List,
    cityCodes,
    mutNames,
    arDates,
    cityNames,
    pinCodes
}) {
    let monthText = document.getElementById("ddlMonth");
    let yearText = document.getElementById("ddlYear");
    let monthYearText =
        (monthText?.options[monthText.selectedIndex]?.text || '') + '-' +
        (yearText?.options[yearText.selectedIndex]?.text || '');

    // Current date
    let currentDate = new Date();
    let day = String(currentDate.getDate()).padStart(2, '0');
    let monthCurrent = String(currentDate.getMonth() + 1).padStart(2, '0'); 
 // month name in short like JAN, FEB, etc.
    let monthNames = ["JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"];
        monthCurrent = monthNames[currentDate.getMonth()]; // Convert month number to month name

    let yearCurrent = currentDate.getFullYear();
    let formattedDate = `${day}-${monthCurrent}-${yearCurrent}`;

    return `
<div class="letter-page">
  <h1 style="text-align:center;">Maturity / Renewal Reminder</h1>
  <div class="letter-meta" style="margin-bottom: 16px;">
    <div>Ref No: <strong>${sourceCodes}</strong></div>
    <div><strong>${formattedDate}</strong></div>
  </div>
  <div class="letter-address" style="margin-bottom: 16px;">
    ${clientNames}<br>
    ${add1List}<br>
    ${add2List}<br>
    ${cityCodes}
  </div>
  <div class="letter-subject" style="font-weight: bold; margin-bottom: 16px;">
    Sub: Maturity of your Fixed Deposits / Bonds
  </div>
  <div class="letter-body">
    <p>Dear Sir / Madam,</p>
    <p>We would like to thank you for patronizing our services...</p>
    <p>Your deposits are maturing in <strong>${monthYearText}</strong></p>
    <table class="fd-table" border="1" cellpadding="5" cellspacing="0">
      <thead style="background:#f2f2f2;">
        <tr>
          <th>S.N.</th><th>Company Name</th><th>Amount</th>
          <th>Investor Name</th><th>Period</th><th>Date</th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td rowspan="2">1</td>
          <td>${mutNames}</td>
          <td>1000</td>
          <td>${clientNames}</td>
          <td>36 M</td>
          <td>${arDates}</td>
        </tr>
        <tr class="deposit-details">
          <td>Details of Deposit:</td>
          <td>Cheque NO: 1234</td>
          <td>Cheque Date: 25-01-2025</td>
          <td colspan="2">Bank: ICICI Bank</td>
        </tr>
      </tbody>
    </table>
  </div>
  <div class="letter-sign">
    Warm Regards,<br><br>
    <span class="name">Aparna Razdan</span><br>
    Manager - Fixed Income Group<br><br>
    PS: Your Client Reference Number is: <strong>${monthYearText}</strong>
  </div>
</div>`;
}

function openPopupForAllPages(allPagesHTML) {
    let popup = window.open("", "_blank");

    popup.document.open();
    popup.document.write(`
<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<title>All Letters</title>

<style>
body {
font-family: Arial, sans-serif;
background: #ffffff;
padding: 20px;
}

button {
padding: 10px 20px;
font-size: 16px;
}

.letter-page {
background: #ffffff;
color: #111;
line-height: 1.45;
page-break-after: always;
break-after: page;
font-family: Arial, sans-serif;
margin: 0 auto;
border: unset;
padding-top: 20%;
padding-bottom: 10%;
width: 190mm;
height: 277mm;
box-sizing: border-box;
background: #fff;
overflow: hidden;
}

.letter-page h1 {
text-align: center;
font-size: 14px;
text-decoration: underline;
margin-bottom: 5px;
}

.letter-meta {
display: flex;
justify-content: space-between;
font-size: 13px;
margin: 12px 0;
}

.letter-address {
font-size: 13px;
margin-bottom: 15px;
}

.letter-subject {
text-align: center;
font-weight: bold;
margin-bottom: 15px;
font-size: 14px;
}

.fd-table {
width: 100%;
border-collapse: collapse;
margin: 12px 0;

}

.fd-table th,
.fd-table td {
border: 1px solid #000;
padding: 5px 8px;
font-size: 13px;
}

.fd-table tr td {
border: none;
}

.deposit-details {
border-bottom: 1px solid #000;
}





.fd-table th {
background: #f5f5f5;
font-weight: bold;
}

.letter-body p {
margin-bottom: 10px;
font-size: 13px;
text-align: justify;
}

.letter-sign {
margin-top: 25px;
font-size: 13px;

margin-top: 25px;
font-size: 13px;
position: relative;
bottom: 40px;
left: 0;
width: 100%;
}

.letter-sign .name {
font-weight: bold;
margin-top: 20px;
}

.ps {
/* margin-top: 15px; */
font-size: 12px;
}

.fd-table th,
.fd-table td {
font-size: 10px;
}

@page {
margin: 0;
}
</style>
</head>
<body>
${allPagesHTML}
<script>
  window.onload = function(){ window.print(); }
  window.onafterprint = function(){ window.close(); }
<\/script>
</body>
</html>`);
    popup.document.close();
}
