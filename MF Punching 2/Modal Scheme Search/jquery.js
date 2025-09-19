// jquery funciotns

// ...existing code...
// Hide browser default print header/footer (page number, date, URL)

function searchScheme1(flag, tranType){
    if (!tranType) {
        alert('Please Select Transaction Type First');
        return;
    }

    var schSearchFlag = '0'; // 1: first time, 2: next time

    var schString = $('#schString').val().trim();
    if (schString == '') {
        alert('Please Input Search String');
        $('#schString').focus();
        return;
    }else{
        ajaxCallForSchemeSearch(flag, tranType, schString, schSearchFlag);
    }


}

function ajaxCallForSchemeSearch(flag, tranType, schString, schSearchFlag){
    var url = 'MF_Punching_2Servlet';
    var params = {
        'action': 'SCHEME_SEARCH',
        'flag': flag,
        'tranType': tranType,
        'schString': schString,
        'schSearchFlag': schSearchFlag
    };