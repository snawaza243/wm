Private Sub cmdGo_Click()
MyBranchCode = ""
If cmbbranchAr.Text <> "" And UCase(cmbbranchAr.Text) <> "ALL" Then
    strbranch = Split(cmbbranchAr.Text, "#")
    MyBranchCode = strbranch(1)
End If

MyMutCodeAr = ""
If CmbAmcAR.Text <> "" And UCase(CmbAmcAR.Text) <> "ALL" Then
    strbranch = Split(CmbAmcAR.Text, "#")
    MyMutCodeAr = strbranch(1)
End If


CmdGo.Enabled = False
MyTotalAmount1 = 0
MyTotalMargin1 = 0
MyTotalTrans1 = 0
Call gridfill(1)
CmdGo.Enabled = True
End Sub
