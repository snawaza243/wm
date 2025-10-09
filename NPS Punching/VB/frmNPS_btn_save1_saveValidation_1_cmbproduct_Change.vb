Public Sub cmbproduct_Change()
Dim rs_get_mutCode As ADODB.Recordset
SCHCODE = ""
If cmbProduct.Text <> "" Then
    If NatureCode = "NT001" Then
        Set rs_get_mutCode = MyConn.Execute("Select mut_code from mut_fund where mut_name='" & Trim(cmbProduct.Text) & "'")
        If Frame3.Visible = True Then
              'FillFolioNo
        End If
    Else
        Set rs_get_mutCode = MyConn.Execute("Select iss_code from iss_master a,product_class_issuer_mf b where trim(iss_name)='" & Trim(cmbProduct.Text) & "' and a.iss_code=b.issuermf_code and b.prod_code='" & Prodcode & "'")
    End If
    If Not rs_get_mutCode.EOF Then
          MutCode = rs_get_mutCode(0)
          mutschfill
     End If
End If
End Sub



