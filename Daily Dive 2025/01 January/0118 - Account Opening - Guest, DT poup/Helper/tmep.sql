SELECT GUEST_NAME,MOBILE,EMP_NO,NVL(TELEPHONE,0)TELEPHONE FROM BAJAJ_VENUE_BOOKING WHERE GUEST_CD='1001077072' ;

frmactopen.txtClientName.Text = GUEST_NAME
frmactopen.txtMobile.Text = MOBILE
frmactopen.txtbusicode.Text = EMP_NO
frmactopen.TxtTel2.Text = TELEPHONE


/* Client Information*/
SELECT B.SEX,PITCH_BOOK_NO,RESIADD1,RESIADD2,CITY,STATE,RESIPINCODE,B.DOB,CORESSEMAIL
FROM CLIENTBASICDET       A,CLIENTPERSDET        B,CLIENTWORKDET        C,BAJAJ_VENUE_BOOKING  D
WHERE     D.GUEST_CD = '1001077072'
AND TO_CHAR (A.ACCOUNTNO) = D.PITCH_BOOK_NO
AND A.ACCOUNTNO = B.ACCOUNTNO
AND A.ACCOUNTNO = C.ACCOUNTNO(+);

If RS("sex") = "Male" Then
    .cmbtitle.Text = "Mr."
ElseIf RS("sex") = "Female" And RS("matrialst") = "Single" Then
    .cmbtitle.Text = "Ms."
Else
    .cmbtitle.Text = "Mrs."
End If

