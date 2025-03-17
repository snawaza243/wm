Private Sub cmdShow_Click()
Dim rstb As New ADODB.Recordset
Dim clinfo As New ADODB.Recordset
Dim i As Integer
Dim reqq As Variant
Dim reqcod As String
rstb.open "select nvl(ar_code,'0'),nvl(INV_CODE,0),nvl(BUSI_BRANCH_CODE,0),nvl(BUSI_RM_CODE,0) from  tb_doc_upload where tran_type='FI' and common_id ='" & txtdocID.Text & "'", MyConn

If Not rstb.EOF And Not rstb.BOF Then
If rstb(1) <> 0 Then
clinfo.open "select inv_code,investor_name from investor_master where inv_code='" & rstb(1) & "'", MyConn
txtINV_CD = clinfo(0)
txtName = clinfo(1)
clinfo.Close
End If
txtrmbusicode = rstb(3)
cboBranch.ListIndex = 0

branch_fill
rstb.Close
Else
rstb.Close
End If
End Sub