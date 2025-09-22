
function handleData(type, data) {

    data.forEach(function (row) {
        var sourceCodes = (row.SOURCECODE1 || '');
        var clientNames = (row.CLIENT_NAME1 || '');
        var add1List = (row.ADD1 || '');
        var add2List = (row.ADD2 || '');
        var cityCodes = (row.CITY_CD || '');
        var mutNames = (row.MUTNAME || '');
        var arDates = (formatDateToDMY(row.ARDATE) || '');
        var cityNames = (row.CITY_NAME || '');
        var pinCodes = (row.PINCODE || '');

        const newData = {
            sourceCodes,
            clientNames,
            add1List,
            add2List,
            cityCodes,
            mutNames,
            arDates,
            cityNames,
            pinCodes
        };

        dataToDesign(type, newData);

        //alert(`Source Code: ${sourceCodes}\nClient Name: ${clientNames}\nAddress 1: ${add1List}\nAddress 2: ${add2List}\nCity Code: ${cityCodes}\nMutual Fund Name: ${mutNames}\nAR Date: ${arDates}\nCity Name: ${cityNames}\nPin Code: ${pinCodes}`);
    });
}


function dataToDesign(type, { data }) {
    var { sourceCodes } = data;
    var { clientNames } = data;
    var { add1List } = data;
    var { add2List } = data;
    var { cityCodes } = data;
    var { mutNames } = data;
    var { arDates } = data;
    var { cityNames } = data;
    var { pinCodes } = data;

    var monthText = document.getElementById("ddlMonth");
    var yearText = document.getElementById("ddlYear");
    var monthYearText = monthText.options[monthText.selectedIndex].text + '-' + yearText.options[yearText.selectedIndex].text;



    // current date 12-Sep-2025 in a variable
    var currentDate = new Date();
    var day = String(currentDate.getDate()).padStart(2, '0');
    var monthCurrent = String(currentDate.getMonth() + 1).padStart(2, '0');
    var yearCurrent = currentDate.getFullYear();
    var formattedDate = day + '-' + monthCurrent + '-' + yearCurrent;

    var head_start = `<!DOCTYPE html>
<html lang="en">

<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<title>Document</title>
</head>`;

    var letter_style = `
    
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
</style>`;

    var leter_body_start = `<body>`;
    var letter_start = `
<div class="letter-page">
<h1 style="text-align:center;">Maturity / Renewal Reminder</h1>
<div class="letter-meta" style="margin-bottom: 16px;">
<div>Ref No: <strong>${sourceCodes}</strong></div>
<div><strong>${formattedDate}</strong></div>

</div>
<div class="letter-address" style="margin-bottom: 16px;">
${clientNames || ''}<br>
${add1List || ''}<br>
${add2List || ''}<br>
${cityCodes || ''}
</div>
<div class="letter-subject" style="font-weight: bold; margin-bottom: 16px;">Sub: Maturity of your Fixed Deposits /
Bonds</div>
<div class="letter-body">
<p>Dear Sir / Madam,</p>
<p>We would like to thank you for patronizing our services. We are India's Premier Investment Services Company
with more than <strong>60 years</strong> of experience in helping people protect and grow their wealth. Over 10 Lakh Investor
rely on our Investment Services</p>
<p>Going forward, we wish to inform you that your following Fixed Deposits/Bonds are maturing in the month of
<strong>${monthYearText}</strong>
</p>
<table class="fd-table" border="1" cellpadding="5" cellspacing="0"
style="border-collapse:collapse; width:100%; margin-bottom: 16px;">
<thead style="background:#f2f2f2;">
<tr>
<th style="width: 25px;">S.N.</th>
<th>Company Name</th>
<th>Amount</th>
<th>Investor Name</th>
<th>Period</th>
<th>Date</th>
</tr>
</thead>
<tbody>
<div class="record-row">

<tr>
<td rowspan="2">1</td>
<td>MUT_NAME</td>
<td>1000</td>
<td>Sam Altmen</td>
<td>36 M</td>
<td>25-02-2025</td>
</tr>

<tr class="deposit-details">
<td>Details of Deposit:</td>
<td>Cheque NO: 1234</td>
<td>Cheque Date: 25-01-2025</td>
<td colspan="2">Bank: ICICI Bank</td>
</tr>

</div>

<tr>
<td rowspan="2">2</td>
<td>MUT_NAME</td>
<td>1000</td>
<td>Sam Altmen</td>
<td>36 M</td>
<td>25-02-2025</td>
</tr>
<tr class="deposit-details">
<td>Details of Deposit:</td>
<td>Cheque NO: 5678</td>
<td>Cheque Date: 20-01-2025</td>
<td colspan="2">Bank: HDFC Bank</td>
</tr>

<tr>
<td rowspan="2">3</td>
<td>MUT_NAME</td>
<td>1000</td>
<td>Sam Altmen</td>
<td>36 M</td>
<td>25-02-2025</td>
</tr>
<tr class="deposit-details">
<td>Details of Deposit:</td>
<td>Cheque NO: 1234</td>
<td>Cheque Date: 25-01-2025</td>
<td colspan="2">Bank: ICICI Bank</td>
</tr>           

</tbody>
</table>

<p>We request you to visit or call your nearest Bajaj Capital Investment Center TM. We have some investment
opportunities waiting for you that can help give boost to your investment further. Expect this meeting to add a
fresh perspective to your investments that would really be insightful and worthy of pondering over.</p>
<p>Looking forward to meet you and discuss newer opportunities to multiply your wealth.</p>
</div>
<div class="letter-sign" style="margin-top: 32px;"><br>
Warm Regards,<br><br>
<span class="name">Aparna Razdan</span><br>
Manager - Fixed Income Group <br><br>
PS: Your Client Reference Number is: <strong>${monthYearText}</strong>
</div>

</div>`;

    var letter_end = `
<script>
window.onload = function() {
window.print();
};
window.onafterprint = function() {
window.close();
};
<\/script>
</body>
</html>`;

    var popupContent = head_start + letter_style + leter_body_start + letter_start + letter_end;

    let popup = window.open("", "_blank");

    popup.document.open();
    popup.document.write(popupContent);
    popup.document.close();
}
