Private Sub cmdfindtran_Click()
Dim findsql As String
Dim X As Integer
VSFlexGrid1_reset
findsql = ""
findsql = "select nvl(HO_TRAN_CODE,0)HO_TRAN_CODE,branch_name from TRANSACTION_ST@mf.bajajcapital a,branch_master b where a.branch_code=b.branch_code and TRAN_CODE='" & txtfindtran.Text & "'"
rsfindtr.open findsql, MyConn, adOpenForwardOnly, adLockOptimistic
X = 1
While Not rsfindtr.EOF   'Or rsfindtr.BOF
        VSFlexGrid1.TextMatrix(X, 0) = IIf(IsNull(rsfindtr("HO_TRAN_CODE")), "", rsfindtr("HO_TRAN_CODE"))
        VSFlexGrid1.TextMatrix(X, 1) = IIf(IsNull(rsfindtr("branch_name")), "", rsfindtr("branch_name"))
        X = X + 1
        rsfindtr.MoveNext
Wend
rsfindtr.Close
End Sub

