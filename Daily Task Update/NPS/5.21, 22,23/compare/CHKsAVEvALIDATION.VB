


Private Function chkSaveValidation(FrSave As Boolean, FrMody As Boolean) As Boolean
Dim rs_get_chk_folio As ADODB.Recordset, rs_get_first_date As ADODB.Recordset, rs_get_facevalue As ADODB.Recordset
Dim tempFol As String
Dim pre As String
Dim dtf, DtL As String
Dim MyRs_validate As ADODB.Recordset
Dim MyRs_validate1 As ADODB.Recordset


If cmbProd.Text = "" Then
     MsgBox "Please select a Product Class.", vbInformation
      chkSaveValidation = False
      Exit Function
End If
If MutCode = "" Then
      MsgBox "Please select a Product.", vbInformation
      chkSaveValidation = False
      Exit Function
End If
If SCHCODE = "" Then
      MsgBox "Please select a Scheme.", vbInformation
      chkSaveValidation = False
      Exit Function
      
End If
If txtINV_CD.Text = "" Then
    MsgBox "Please select a Investor.", vbInformation
    chkSaveValidation = False
    Exit Function
End If
If ReqCode = "" Then
    MsgBox "Please Select a Request Id.", vbInformation
    chkSaveValidation = False
    Exit Function
End If

If Not IsDate(DtDate) Then
     MsgBox "Please enter a correct Transaction Date.", vbInformation
     chkSaveValidation = False
     Exit Function
End If
If Format(CDate(DtDate), "dd/mm/yyyy") < "01/01/1980" Then
    MsgBox "Please enter a correct transaction date.", vbInformation
    chkSaveValidation = False
    Exit Function
End If
If txtAmountInvest.Text = "" Then
    MsgBox "Please enter amount.", vbInformation
    chkSaveValidation = False
    Exit Function
End If
If Not IsNumeric(txtAmountInvest.Text) Then
    MsgBox "Please enter Correct Amount.", vbInformation
    chkSaveValidation = False
    Exit Function
End If
If txtrmbusicode.Text = "" Or Len(txtrmbusicode) < 5 Then
    MsgBox "Not a Valid RM Business Code", vbCritical, "BACKOFFICE"
    txtrmbusicode.SetFocus
    chkSaveValidation = False
    Exit Function
End If
Dim GEt_busiRMCode As New ADODB.Recordset
Set GEt_busiRMCode = MyConn.Execute("select rm_code from employee_master where payroll_id='" & Trim(txtrmbusicode.Text) & "'")
If GEt_busiRMCode.EOF Then
    MsgBox "Not a Valid RM Business Code", vbCritical, "BACKOFFICE"
    txtrmbusicode.SetFocus
    chkSaveValidation = False
    Exit Function
End If
If FrSave = True And FrMody = False Then
    If (CDate(Format(DtDate, "dd-mm-yyyy")) < CDate(Format(  , "DD-MM-YYYY"))) Or
     (CDate(Format(DtDate, "dd-mm-yyyy")) > CDate(Format(Glbins_nextdate, "DD-MM-YYYY"))) Then
        MsgBox "Security restrictions for date range"
        chkSaveValidation = False
        Exit Function
    End If
End If
If FrSave = False And FrMody = True Then
    If (CDate(Format(DtDate, "dd-mm-yyyy")) < CDate(Format(Glbup_previousdate, "DD-MM-YYYY"))) Or
     (CDate(Format(DtDate, "dd-mm-yyyy")) > CDate(Format(Glbup_nextdate, "DD-MM-YYYY"))) Then
        MsgBox "Security restrictions for date range"
        chkSaveValidation = False
        Exit Function
    End If
End If
If CheckDate(DtDate, Format(ServerDateTime, "dd/mm/yyyy")) = False Then
    MsgBox "Transaction Date Cannot Be Greater than Current Date ", vbInformation
    chkSaveValidation = False
    Exit Function
End If

If OptCorNECS.Value = False And optcheque.Value = False And optdraft.Value = False And OptEcs.Value = False And optcash.Value = False And OptOthers.Value = False Then
    MsgBox "Please Select a Payment Mode.", vbExclamation
    chkSaveValidation = False
    Exit Function
End If

If optcheque.Value = True Then
    If Trim(cmbBankName.Text) = "" Then
        MsgBox "Please Select a Bank Name", vbExclamation, "Alert"
        cmbBankName.SetFocus
        chkSaveValidation = False
        Exit Function
    End If
    If Trim(txtChqNo.Text) = "" Then
        MsgBox "Please Insert a Cheque Number", vbExclamation, "Alert"
        txtChqNo.SetFocus
        chkSaveValidation = False
        Exit Function
    End If
    If Trim(dtChqDate.Text) = "" Or Trim(dtChqDate.Text) = "__/__/____" Then
        MsgBox "Please Insert a Cheque Date", vbExclamation, "Alert"
        dtChqDate.SetFocus
        chkSaveValidation = False
        Exit Function
    End If
End If
If optdraft.Value = True Then
    If Trim(txtChqNo.Text) = "" Then
        MsgBox "Please Insert a Draft Number", vbExclamation, "Alert"
        txtChqNo.SetFocus
        chkSaveValidation = False
        Exit Function
    End If
    If Trim(dtChqDate.Text) = "" Or Trim(dtChqDate.Text) = "__/__/____" Then
        MsgBox "Please Insert a Draft Date", vbExclamation, "Alert"
        dtChqDate.SetFocus
        chkSaveValidation = False
        Exit Function
    End If
End If
If OptEcs.Value = True Then
    If Trim(txtChqNo.Text) = "" Then
        MsgBox "Please Insert a MCR Number", vbExclamation, "Alert"
        txtChqNo.SetFocus
        chkSaveValidation = False
        Exit Function
    End If
    If Trim(dtChqDate.Text) = "" Or Trim(dtChqDate.Text) = "__/__/____" Then
        MsgBox "Please Insert a Date", vbExclamation, "Alert"
        dtChqDate.SetFocus
        chkSaveValidation = False
        Exit Function
    End If
End If
If OptOthers.Value = True Then
    If Trim(txtChqNo.Text) = "" Then
        MsgBox "Please Insert a FDR Number", vbExclamation, "Alert"
        txtChqNo.SetFocus
        chkSaveValidation = False
        Exit Function
    End If
    If Trim(dtChqDate.Text) = "" Or Trim(dtChqDate.Text) = "__/__/____" Then
        MsgBox "Please Insert a Renewal Date", vbExclamation, "Alert"
        dtChqDate.SetFocus
        chkSaveValidation = False
        Exit Function
    End If
    
    If Trim(dtChqDate.Text) <> "" And dtChqDate.Text <> "__/__/____" Then
        dtf = "01/" & Format(CDate(Date), "mm/yyyy")
        DtL = DateAdd("M", 1, Format(CDate(dtf), "dd/MM/yyyy")) - 1
        If CDate(dtChqDate.Text) < CDate(dtf) Or CDate(dtChqDate.Text) > CDate(DtL) Then
            MsgBox "Cheque/Draft Date Should be in Current Month", vbExclamation, "Alert"
            dtChqDate.Text = "__/__/____"
            dtChqDate.SetFocus
            chkSaveValidation = False
            Exit Function
        End If
    End If
End If
'End If
If cmbBusiBranch.Text = "" Then
    MsgBox "Please Select Business Branch ", vbInformation
    cmbBusiBranch.SetFocus
    chkSaveValidation = False
    Exit Function
End If
'-------------------My Validations--------------------------------
If CmdSave.Item(0) = True Then
        Set MyRs_validate = New ADODB.Recordset
        Mysql = "select * from transaction_st where client_code='" & txtINV_CD.Text & "' and sch_code in ('OP#09971') and tran_type in ('PURCHASE','REINVESTMENT','SWITCH IN')"
        MyRs_validate.open Mysql, MyConn, adOpenForwardOnly
        If Not MyRs_validate.EOF Then
            If MyRs_validate.Fields("sch_code") = "OP#09971" Then
            End If
        End If
        MyRs_validate.Close
        Set MyRs_validate = Nothing
End If

Set MyRs_validate = New ADODB.Recordset
Mysql = "select * from transaction_st where client_code='" & txtINV_CD.Text & "' and sch_code in ('OP#09971','OP#09973') and tran_type in ('PURCHASE','REINVESTMENT','SWITCH IN')"
MyRs_validate.open Mysql, MyConn, adOpenForwardOnly
If Not MyRs_validate.EOF Then
Else
    If SCHCODE = "OP#09972" Then
        MsgBox "Not Allowed in this Scheme Please Select Scheme Tier1 or Tier1+2"
        lstlongname.SetFocus
        chkSaveValidation = False
        Exit Function
    End If
    
End If
MyRs_validate.Close
Set MyRs_validate = Nothing

If Not (OptCorporate.Value = True And ReqCode = 11) Then
    Set MyRs_validate = New ADODB.Recordset
    
    If lbtrancode.Caption = "0" Then
        Mysql = "select * from transaction_st where client_code='" & txtINV_CD.Text & "' and sch_code in ('OP#09971','OP#09972','OP#09973') and tran_type in ('PURCHASE','REINVESTMENT','SWITCH IN')"
    Else
        Mysql = "select * from transaction_st where tran_code<>" & lbtrancode.Caption & " and client_code='" & txtINV_CD.Text & "' and sch_code in ('OP#09971','OP#09972','OP#09973') and tran_type in ('PURCHASE','REINVESTMENT','SWITCH IN')"
    End If
    
    MyRs_validate.open Mysql, MyConn, adOpenForwardOnly
    If Not MyRs_validate.EOF Then
        If Trim(MyRs_validate.Fields("sch_code")) = "OP#09971" And (SCHCODE = "OP#09971" Or SCHCODE = "OP#09972") Then
            If Trim(MyRs_validate.Fields("manual_arno")) <> Trim(txtPRAN.Text) Then
                MsgBox "Please Enter Same PRAN No. "
                txtPRAN.SetFocus
                chkSaveValidation = False
                Exit Function
            End If
        ElseIf Trim(MyRs_validate.Fields("sch_code")) = "OP#09973" And SCHCODE = "OP#09973" Then
            If Trim(MyRs_validate.Fields("manual_arno")) <> Trim(txtPRAN.Text) Then
                MsgBox "Please Enter Same PRAN No. "
                txtPRAN.SetFocus
                chkSaveValidation = False
                Exit Function
            End If
        End If
    Else
        Set MyRs_validate1 = New ADODB.Recordset
        If lbtrancode.Caption = "0" Then
            Mysql = "select * from transaction_st where manual_arno='" & txtPRAN.Text & "' and tran_type in ('PURCHASE','REINVESTMENT','SWITCH IN')"
        Else
            Mysql = "select * from transaction_st where tran_code<>" & lbtrancode.Caption & " and manual_arno='" & txtPRAN.Text & "' and tran_type in ('PURCHASE','REINVESTMENT','SWITCH IN')"
        End If
        MyRs_validate1.open Mysql, MyConn, adOpenForwardOnly
        If Not MyRs_validate.EOF Then
            MsgBox "Please Enter Different PRAN No. "
            txtPRAN.SetFocus
            chkSaveValidation = False
            Exit Function
        End If
        MyRs_validate1.Close
        Set MyRs_validate1 = Nothing
    End If
    MyRs_validate.Close
    Set MyRs_validate = Nothing
End If


If SCHCODE = "OP#09973" And ReqCode = "11" Then
    If (Trim(TxtTire1.Text) = 0 Or Trim(TxtTire1.Text) = "") Or (Trim(TxtTire2.Text) = 0 Or Trim(TxtTire2.Text) = "") Then
        MsgBox "Please Enter Tier1 and Tier2 Amount in this Scheme"
        TxtTire1.SetFocus
        lstlongname.SetFocus
        chkSaveValidation = False
        Exit Function
    End If
End If

If ReqCode = "11" And SCHCODE = "OP#09973" Then
    If (Val(Trim(TxtTire1.Text)) < 500) Or (Val(Trim(TxtTire2.Text)) < 1000) Then
        MsgBox "Please Enter Correct Amount in this Scheme"
        TxtTire1.SetFocus
        lstlongname.SetFocus
        chkSaveValidation = False
        Exit Function
    End If
End If
'vinod  11/05/2005
chkSaveValidation = True
End Function