Public Function chkSaveValidation(FrSave As Boolean, FrMody As Boolean, tranCode As String) As Boolean
Dim rs_get_chk_folio As ADODB.Recordset, rs_get_first_date As ADODB.Recordset, rs_get_facevalue As ADODB.Recordset
Dim tempFol As String
Dim pre As String
Dim MyOriginalDate As Date
If tranCode <> "" Then
    MyOriginalDate = SqlRet("select tr_date from transaction_mf_temp1 where tran_code='" & tranCode & "'")
End If
If FrSave = True And FrMody = False Then
    If (CDate(Format(ImEntryDtA.Text, "dd-mm-yyyy")) < CDate(Format(Glbins_previousdate, "DD-MM-YYYY"))) Or (CDate(Format(ImEntryDtA.Text, "dd-mm-yyyy")) > CDate(Format(Glbins_nextdate, "DD-MM-YYYY"))) Then
        MsgBox "Security restrictions for date range"
        chkSaveValidation = False
        Exit Function
    End If
End If
If FrSave = False And FrMody = True Then
    If (CDate(Format(MyOriginalDate, "dd-mm-yyyy")) < CDate(Format(Glbup_previousdate, "DD-MM-YYYY"))) Or (CDate(Format(MyOriginalDate, "dd-mm-yyyy")) > CDate(Format(Glbup_nextdate, "DD-MM-YYYY"))) Then
        MsgBox "Security restrictions for date range"
        chkSaveValidation = False
        Exit Function
    End If
    
    If MyConn.Execute("select count(*) from WEALTHMAKER.ANATRANDETAILTABLE_NEW_ALL_VB  where tran_code='" & tranCode & "'")(0) > 0 Then
        MsgBox "ANG Bills generated for this transaction. You can not modify the payout of this AR. ", vbInformation
        chkSaveValidation = False
        Exit Function
    End If
End If
chkSaveValidation = True
End Function
