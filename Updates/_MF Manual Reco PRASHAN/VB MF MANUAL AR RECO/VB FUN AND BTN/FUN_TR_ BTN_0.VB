Private Sub CmdGo1_Click()
cmbbranchAr.ListIndex = cmbbranch.ListIndex
MyBranchCode = ""
If optPMS.Value = True Then
    cmdConfirm.Caption = "Confirm PMS"
ElseIf optATM.Value = True Then
    cmdConfirm.Caption = "Confirm ATM"
End If

If optPMS.Value = True Then
    cmdUnconfirm.Caption = "Unconfirm PMS"
ElseIf optATM.Value = True Then
    cmdUnconfirm.Caption = "Unconfirm ATM"
End If

If cmbbranch.Text <> "" And UCase(cmbbranch.Text) <> "ALL" Then
    strbranch = Split(cmbbranch.Text, "#")
    MyBranchCode = strbranch(1)
End If

If Trim(cmbcategory.Text) <> "" And Trim(cmbcategory.Text) <> "-Select Branch-" And UCase(Trim(cmbcategory.Text)) <> "ALL" Then
    strbranch = Split(Trim(cmbcategory.Text), "#")
    MyBranchCat = Trim(strbranch(1))
End If

MyRegionCode = ""
If CmbRegion.Text <> "" And UCase(CmbRegion.Text) <> "ALL" Then
    strbranch = Split(CmbRegion.Text, "#")
    MyRegionCode = strbranch(1)
End If

MyZoneCode = ""
If CmbZone.Text <> "" And UCase(CmbZone.Text) <> "ALL" Then
    strbranch = Split(CmbZone.Text, "#")
    MyZoneCode = strbranch(1)
End If

MyRmCode = ""
If CmbRM.Text <> "" And UCase(CmbRM.Text) <> "ALL" Then
    strbranch = Split(CmbRM.Text, "#")
    MyRmCode = strbranch(1)
End If

MyMutCode = ""
If CmbAmcTR.Text <> "" And UCase(CmbAmcTR.Text) <> "ALL" Then
    strbranch = Split(CmbAmcTR.Text, "#")
    MyMutCode = strbranch(1)
End If


CmdGo1.Enabled = False
MyTotalAmount2 = 0
MyTotalmargin2 = 0
MyTotalTrans2 = 0
Call gridfill(2)
CmdGo1.Enabled = True
End Sub

