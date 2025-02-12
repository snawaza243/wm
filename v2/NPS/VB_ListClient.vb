Private Sub cmdShow_Click()
'kyc
If Len(Branches) = 8 Then
If cmbbranch.ListIndex = -1 Then
        MsgBox "Please Select Atleast One Branch ", vbExclamation
        cmbbranch.SetFocus
        Exit Sub
    End If
End If
    If Cmbcat.Text <> "" Then
        If cmbbranch.ListIndex = -1 And cmbcity.ListIndex = -1 And txtcode.Text = "" And txtname.Text = "" And txtadd1.Text = "" And txtadd2.Text = "" And txtPhone.Text = "" And txtMobile.Text = "" And Trim(TxtPanNo.Text) = "" And Trim(txtAccountCode.Text) = "" And cmbNewRM.ListIndex = -1 Then
            If Cmbcat.Text = "INVESTOR" And txtCliSubName.Text = "" Then
                MsgBox "Please Enter Atleast One Searching Criteria !", vbExclamation
                txtname.SetFocus
                Exit Sub
            ElseIf Cmbcat.Text = "CLIENT" Or Cmbcat.Text = "AGENT" Then
                MsgBox "Please Enter Atleast One Searching Criteria !", vbExclamation
                txtname.SetFocus
                Exit Sub
            End If
        End If
        cmdShow.Enabled = False
        Me.MousePointer = 11
        msfgClients.Clear
        If txtname.Text = "" Then
            If (TxtPanNo.Text <> "" Or txtAccountCode.Text <> "") And (txtname.Text = "" And txtadd1.Text = "" And txtadd2.Text = "" And txtPhone.Text = "" And txtMobile.Text = "" And txtcode.Text = "") Then
                chkIndia.Value = 1
                AllIndiaSearchFlag = "ALL"
            Else
                AllIndiaSearchFlag = "SPECIFIC"
                chkIndia.Value = 0
            End If
        Else
            If (txtname.Text <> "" And ImEntryDt.Text <> "__/__/____") And (txtadd1.Text = "" And txtadd2.Text = "" And txtPhone.Text = "" And txtMobile.Text = "" And txtcode.Text = "" And TxtPanNo.Text = "" And txtAccountCode.Text = "") Then
                chkIndia.Value = 1
                AllIndiaSearchFlag = "ALL"
            Else
                AllIndiaSearchFlag = "SPECIFIC"
                chkIndia.Value = 0
            End If
        End If
        If chkIndia.Value = 1 Then
            AllIndiaSearch
        Else
            ShowFilterData
        End If
        set_grid
        Me.MousePointer = 0
        cmdShow.Enabled = True
    Else
        MsgBox "Please Select Category ", vbInformation
    End If
End Sub
