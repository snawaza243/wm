Private Sub cmdSearch_Click()
Dim StrSql As String
If cboCity.Text = "" And Txtcode.Text = "" And Txtname.Text = "" And txtMobile.Text = "" And txtPhone.Text = "" And txtPan.Text = "" And cmbBr.Text = "" Then
    MsgBox "Please Select/Enter Searching Parameter", vbInformation
    cboCity.SetFocus
    Exit Sub
End If
    StrSql = "select * from agent_master where 1=1"
    If cboCity.ListIndex <> -1 Then
        StrSql = StrSql & " and city_id='" & cboCity.List(cboCity.ListIndex, 1) & "'"
    End If
    If cmbBr.ListIndex <> -1 Then
        StrSql = StrSql & " and sourceid='" & cmbBr.List(cmbBr.ListIndex, 1) & "'"
    End If
    If Txtcode.Text <> "" Then
        StrSql = StrSql & " and EXIST_CODE like '%" & Trim(Txtcode.Text) & "%'"
    End If
    If Txtname.Text <> "" Then
        StrSql = StrSql & " and Upper(agent_name) like '%" & UCase(Trim(Txtname.Text)) & "%'"
    End If
    If txtMobile.Text <> "" Then
        StrSql = StrSql & " and mobile like '%" & Trim(txtMobile.Text) & "%'"
    End If
    If txtPhone.Text <> "" Then
        StrSql = StrSql & " and phone like '%" & Trim(txtPhone.Text) & "%'"
    End If
    If txtPan.Text <> "" Then
        StrSql = StrSql & " and Upper(pan) like '%" & UCase(Trim(txtPan.Text)) & "%'"
    End If
    'vinod security Purpose 3 may
      If Trim(SRmCode) <> "" Then
        StrSql = StrSql & " and sourceid in (" & Branches & ") and (rm_code=" & SRmCode & ")"
      Else
        StrSql = StrSql & " and sourceid in (" & Branches & ")"
      End If
    'vinod security Purpose 3 may
    StrSql = StrSql & " order by agent_name"
    Call Search(StrSql)
    Me.MousePointer = 0
    dataview.SetFocus
End Sub