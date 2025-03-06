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
'-----------------------------------------------------------------------
Else
    If txtpans.Text <> "" Then
        RsPan.open "select nvl(count(*),0) from client_test where client_pan=user_log('" & Trim(txtpans.Text) & "')", MyConn, adOpenForwardOnly
        If RsPan(0) = 0 Then
            MsgBox "The Requested Account Doesn't Exist", vbInformation
        Else
            MsgBox "The Requested Account is already mapped to some other RM", vbInformation
        End If
        RsPan.Close
    Else
        MsgBox "The Requested Account Doesn't Exist", vbInformation
    End If
    Set RsPan = Nothing
    
    Call CMDRESET_Click
End If
'CmdUpdate.Enabled = True
frameAdvisory.Enabled = True
If MyOldBusiCode <> "95829" Then
    If GlbroleId <> 218 And GlbroleId <> 124 And GlbroleId <> 1 And Glbloginid <> 97117 And GlbroleId <> 212 And GlbroleId <> 54 Then
        txtbusicode.Enabled = False
    End If
Else
    txtbusicode.Enabled = True
End If
Picture1.Enabled = False
End Sub
