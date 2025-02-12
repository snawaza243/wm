Public Sub cmbProd_Change()
On Error Resume Next
Dim rs_get_nature As ADODB.Recordset, rs_get_product As ADODB.Recordset, rs_get_trantype As ADODB.Recordset
If cmbProd.Text <> "" Then
      cmbProduct.Clear
      lstSch.Clear
        lstlongname.ListItems.Clear
      Set rs_get_nature = MyConn.Execute("Select nature_code,prod_code from product_master where name like '" & Trim(cmbProd.Text) & "%'")
      If rs_get_nature.EOF Then Exit Sub
      NatureCode = rs_get_nature(0)
      Prodcode = rs_get_nature(1)
      If NatureCode = "NT001" Then
          Set rs_get_product = MyConn.Execute("Select mut_name,mut_code from mut_fund order by mut_name")
          cmbStru.Enabled = True
          CmbType.Enabled = True
          cmbNature.Enabled = True
          cmbStru.Visible = True
          cmbStru.BackColor = vbWhite
          CmbType.BackColor = vbWhite
          cmbNature.BackColor = vbWhite
          lblStru.Visible = True
          lblStru.Caption = "Structure"
          lblType.Caption = "Plan"
          lblNature.Caption = "Asset Class"
          cmbFolio.BackColor = vbWhite
          cmbFolio.Enabled = True
          Frame3.Enabled = True
          frameOP.Visible = False
          Frame4.Visible = True
          dttranMF.Text = Format(Date, "dd/mm/yyyy")
      Else
          If Trim(GlbroleId) = 1 Then
            Set rs_get_product = MyConn.Execute("Select iss_name from iss_master where iss_code in (Select distinct issuermf_code from product_class_issuer_mf where prod_code='" & rs_get_nature(1) & "') order by iss_name")
          Else
            Set rs_get_product = MyConn.Execute("Select iss_name from iss_master where iss_code in (Select distinct issuermf_code from product_class_issuer_mf where prod_code='" & rs_get_nature(1) & "') and iss_code in (SELECT iss_code FROM SCH_CITY_MAPPING WHERE (city_id IN ((SELECT DISTINCT city_id FROM BRANCH_MASTER WHERE branch_code IN (" & Branches & "))) or city_id='ALL')) order by iss_name")
          End If
          
          Frame3.Enabled = False
          Frame4.Visible = False
          frameOP.Visible = True
            txtOpUnits.Enabled = False   'vinod 11/05/2005
            txtOpPrice.Enabled = False  'vinod 11/05/2005
            dtOpMat.Enabled = False
            dtOpTran.Text = Format(Date, "dd/mm/yyyy")
            txtOpUnits.Text = ""
            txtOpPrice.Text = ""
            dtOpMat.Text = "__/__/____"
            Label9.Visible = True
            dtOpMat.Visible = True
            txtOpAmount.Text = ""
          If NatureCode <> "NT005" And NatureCode <> "NT002" Then
              lblOPUnits.Caption = "Months"
              lblOPRate.Caption = "Interest Rate"
              lbfaceval.Visible = False
              txtfaceval.Visible = False
              txtfaceval.Enabled = False
              lbmatperiod.Visible = False
              txtmatperiod.Visible = False
              txtmatperiod.Enabled = False
              Label9.Visible = True
              dtOpMat.Visible = True
          Else
              lblOPUnits.Caption = "Units/Bond"
              lbfaceval.Visible = True
              txtfaceval.Visible = True
              txtfaceval.Enabled = False
              lbmatperiod.Visible = True
              txtmatperiod.Visible = True
              txtmatperiod.Enabled = False
              Label9.Visible = False
              dtOpMat.Visible = False
          End If
      End If
      While Not rs_get_product.EOF
          cmbProduct.AddItem rs_get_product(0)
          rs_get_product.MoveNext
      Wend
      cmbProduct.ListIndex = 0
      Set rs_get_trantype = MyConn.Execute("Select tran_type_desc from tran_type_master a,nature_tran_master b where a.tran_type_code=b.tran_type_code and nature_code='" & NatureCode & "' ")
      While Not rs_get_trantype.EOF
          cmbTranType.AddItem rs_get_trantype(0)
          rs_get_trantype.MoveNext
      Wend
      cmbTranType.Text = "PURCHASE"
End If
If UCase(cmbProd.Text) = "MF" Then
  OptSip.Visible = True
  optStp.Visible = True
Else
  OptSip.Visible = False
  optStp.Visible = False
End If
If NatureCode = "NT001" Then
CmdStatus.Enabled = False
Else
CmdStatus.Enabled = True
End If
If NatureCode = "NT003" Or NatureCode = "NT004" Then
    OptOthers.Enabled = True
Else
    OptOthers.Enabled = False
End If
End Sub