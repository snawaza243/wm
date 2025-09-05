Private Sub cmdShow_Click()
If Len(Branches) = 8 Then
If cmbBranch.ListIndex = -1 Then
        MsgBox "Please Select Atleast One Branch ", vbExclamation
        cmbBranch.SetFocus
        Exit Sub
    End If
End If
    If Cmbcat.Text <> "" Then
        If cmbBranch.ListIndex = -1 And cmbCity.ListIndex = -1 And Txtcode.Text = "" And Txtname.Text = "" And txtAdd1.Text = "" And txtAdd2.Text = "" And txtPhone.Text = "" And Trim(txtPan.Text) = "" And txtMobile.Text = "" And cmbNewRM.ListIndex = -1 And txtahcode = "" Then
            If Cmbcat.Text = "INVESTOR" And txtCliSubName.Text = "" Then
                MsgBox "Please Enter Atleast One Searching Criteria !", vbExclamation
                Txtname.SetFocus
                Exit Sub
            ElseIf Cmbcat.Text = "CLIENT" Or Cmbcat.Text = "AGENT" Then
                MsgBox "Please Enter Atleast One Searching Criteria !", vbExclamation
                Txtname.SetFocus
                Exit Sub
            End If
        End If
        cmdShow.Enabled = False
        Me.MousePointer = 11
        msfgClients.Clear
'        If txtPan.Text <> "" Then  'Or txtAccountCode.Text <> ""
'            chkIndia.Value = 1
'            AllIndiaSearchFlag = "ALL"
'        Else
'            AllIndiaSearchFlag = "SPECIFIC"
'            chkIndia.Value = 0
'        End If
        If Txtname.Text = "" Then
            If (txtPan.Text <> "" Or txtahcode.Text <> "") And (Txtname.Text = "" And txtAdd1.Text = "" And txtAdd2.Text = "" And txtPhone.Text = "" And txtMobile.Text = "" And Txtcode.Text = "") Then
                chkIndia.Value = 1
                AllIndiaSearchFlag = "ALL"
                AllIndia = True
            Else
                AllIndiaSearchFlag = "SPECIFIC"
                chkIndia.Value = 0
                AllIndia = False
            End If
        Else
            If (txtPan.Text = "" And txtahcode.Text = "") Then
                AllIndiaSearchFlag = "SPECIFIC"
                chkIndia.Value = 0
                AllIndia = False
            Else
                chkIndia.Value = 1
                AllIndiaSearchFlag = "ALL"
                AllIndia = True
            End If
        End If
        If chkIndia.Value = 1 Then
            AllIndiaSearch
        Else
            ShowFilterData
        End If
        set_grid
        'MsgBox msfgClients.Rows - 1 & " Records Shown ! ", vbInformation
        Me.MousePointer = 0
        cmdShow.Enabled = True
    Else
        MsgBox "Please Select Category ", vbInformation
        Cmbcat.SetFocus
    End If
End Sub

