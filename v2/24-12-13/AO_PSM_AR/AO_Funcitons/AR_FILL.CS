'-----------------------------fill advisory Details--------------------

sql = "select * from filladvisory where 1=1"
If txtclientcode.Text <> "" Then
    sql = sql & " and act_code='" & Trim(txtclientcode.Text) & "' "
End If
If txtpans.Text <> "" Then
    sql = sql & " and client_pan='" & Trim(txtpans.Text) & "' "
End If
RsAdvisory.open sql, MyConn, adforwardonly
If Not RsAdvisory.EOF Then
    '-------Fresh/Renewal----------
    OptRen.Value = True
    optFresh.Value = False
    For I = 0 To cmbplantype.ListCount - 1
        MyPlan = Split(cmbplantype.List(I), "=")
        If UCase(Trim(MyPlan(1))) = UCase(Trim(RsAdvisory!sch_code)) Then
            cmbplantype.ListIndex = I
            Exit For
        End If
    Next
    TxtAmount.Text = RsAdvisory.Fields("amount")
    For I = 0 To cmbBankName.ListCount - 1
        If UCase(Trim(cmbBankName.List(I))) = UCase(Trim(RsAdvisory!bank_name)) Then
            cmbBankName.ListIndex = I
            Exit For
        End If
    Next
    If RsAdvisory.Fields("payment_mode").Value = "C" Then
        optcheque.Value = True
    Else
        optdraft.Value = True
    End If
    TxtRemark.Text = RsAdvisory.Fields("bank_ac_no")
    txtChqNo.Text = RsAdvisory.Fields("cheque_no")
    dtChqDate.Text = RsAdvisory.Fields("cheque_date")
    MyTranCode = RsAdvisory.Fields("tran_code")
    MyPrintSourceId = RsAdvisory.Fields("source_code")
    MyTrDate = RsAdvisory.Fields("tr_date")
Else
    OptRen.Value = False
    optFresh.Value = True
    TxtAmount.Text = ""
    TxtRemark.Text = ""
    txtChqNo.Text = ""
    dtChqDate.Text = "__/__/____"
    MyTranCode = ""
    MyPrintSourceId = ""
    MyTrDate = "__/__/____"
End If
RsAdvisory.Close