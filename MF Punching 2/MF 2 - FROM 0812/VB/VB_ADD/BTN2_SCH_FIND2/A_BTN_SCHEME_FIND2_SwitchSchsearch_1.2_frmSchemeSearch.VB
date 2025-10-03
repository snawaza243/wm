Public searchschflag As Integer
Private Sub cmdExit_Click()
    Unload Me
End Sub
Private Sub cmdGo_Click()
Dim rsFind As New ADODB.Recordset
Dim strMF As String
Dim CodeMF As String
Dim strSearch As String
Dim i As Integer
        msfgSchemes.Rows = 1
        msfgSchemes.Rows = 2
        If TxtFind.Text <> "" Then
            strSearch = ConvertString(UCase(Trim(TxtFind.Text)))
            rsFind.open "Select name,PROD_CODE from product_master where nature_code='NT001'", MyConn, adOpenForwardOnly
            strMF = rsFind("Name")
            CodeMF = rsFind("prod_code")
            rsFind.Close
            If Trim(GlbroleId) = 1 Then
                rsFind.open "select sch_code,sch_name,Short_name,mut_name from scheme_info s,mut_fund m where UPPER(MUT_NAME)||upper(sch_name)||UPPER(MUT_NAME) LIKE '%" & strSearch & "%' and s.mut_code=m.mut_code and m.mut_code not in ('MF001','MF036','MF002','MF013','MF039','MF029','MF027','MF035','MF020') ORDER BY sch_name,Short_name", MyConn, adOpenForwardOnly
            Else
                rsFind.open "select sch_code,sch_name,Short_name,mut_name from scheme_info s,mut_fund m where UPPER(MUT_NAME)||upper(sch_name)||UPPER(MUT_NAME) LIKE '%" & strSearch & "%' and s.mut_code=m.mut_code and m.mut_code not in ('MF001','MF036','MF002','MF013','MF039','MF029','MF027','MF035','MF020') and exists (SELECT sch_code FROM SCH_CITY_MAPPING WHERE sch_code=s.sch_code and (city_id IN ((SELECT DISTINCT city_id FROM BRANCH_MASTER WHERE branch_code IN (" & Branches & "))) OR CITY_ID='ALL') and iss_code like 'MF%' ) ORDER BY sch_name,Short_name", MyConn, adOpenForwardOnly
            End If
            i = 1
            While Not rsFind.EOF
                msfgSchemes.TextMatrix(i, 1) = strMF
                msfgSchemes.TextMatrix(i, 2) = rsFind("Short_name")
                msfgSchemes.TextMatrix(i, 3) = rsFind("sch_name")
                msfgSchemes.TextMatrix(i, 4) = rsFind("mut_name")
                msfgSchemes.TextMatrix(i, 5) = CodeMF
                msfgSchemes.TextMatrix(i, 6) = rsFind("sch_code")
                i = i + 1
                msfgSchemes.Rows = msfgSchemes.Rows + 1
                rsFind.MoveNext
            Wend
            rsFind.Close
            If Trim(GlbroleId) = 1 Then
                rsFind.open "select osch_code,osch_name,longname,name,iss_name from other_product o, product_master p,iss_master i where UPPER(ISS_NAME)||upper(longname)||UPPER(ISS_NAME) LIKE '%" & strSearch & "%' and o.prod_class_code=p.prod_code and o.iss_code=i.iss_code and (i.branch_code<>'N' or i.branch_code is null) and (o.flag<>'N' or o.flag is null) ORDER BY longname,osch_name", MyConn, adOpenForwardOnly
            Else
                 rsFind.open "select osch_code,osch_name,longname,name,iss_name from other_product o, product_master p,iss_master i where UPPER(ISS_NAME)||upper(longname)||UPPER(ISS_NAME) LIKE '%" & strSearch & "%' and o.prod_class_code=p.prod_code and o.iss_code=i.iss_code and (i.branch_code<>'N' or i.branch_code is null) and (o.flag<>'N' or o.flag is null) and o.osch_code in (SELECT sch_code FROM SCH_CITY_MAPPING WHERE (city_id IN ((SELECT DISTINCT city_id FROM BRANCH_MASTER WHERE branch_code IN (" & Branches & "))) OR CITY_ID='ALL') and (iss_code like 'IS%' or iss_code like 'MFIS%')) ORDER BY longname,osch_name", MyConn, adOpenForwardOnly
            End If
            While Not rsFind.EOF
                msfgSchemes.TextMatrix(i, 1) = rsFind("name")
                msfgSchemes.TextMatrix(i, 2) = rsFind("osch_name")
                msfgSchemes.TextMatrix(i, 3) = rsFind("longname")
                msfgSchemes.TextMatrix(i, 4) = rsFind("iss_name")
                msfgSchemes.TextMatrix(i, 6) = rsFind("osch_code")
                i = i + 1
                msfgSchemes.Rows = msfgSchemes.Rows + 1
                rsFind.MoveNext
            Wend
            rsFind.Close
            If msfgSchemes.Rows > 2 Then
                msfgSchemes.Rows = msfgSchemes.Rows - 1
                msfgSchemes.SetFocus
            End If
            AlignCols
        Else
            TxtFind.SetFocus
        End If
        Set rsFind = Nothing
End Sub
Private Sub cmdSelect_Click()
    msfgSchemes_DblClick
End Sub
Private Sub Form_Load()
    'Image1.Picture = LoadPicture(App.Path & "\logo1.JPG")
    Me.Icon = LoadPicture(App.Path & "\W.ICO")
    
    Me.Top = ((Screen.Height - Me.Height) / 2) - 800
    Me.Left = (Screen.width - Me.width) / 2
    msfgSchemes.ColWidth(0) = 0
    msfgSchemes.ColWidth(1) = 1000
    msfgSchemes.ColWidth(2) = 2400
    msfgSchemes.ColWidth(3) = 3500
    msfgSchemes.ColWidth(4) = 2400
    msfgSchemes.ColWidth(5) = 0
    msfgSchemes.ColWidth(6) = 1000
    msfgSchemes.ColWidth(7) = 0
    msfgSchemes.TextMatrix(0, 1) = "Prod. Class"
    msfgSchemes.TextMatrix(0, 2) = "Short Name"
    msfgSchemes.TextMatrix(0, 3) = "Long Name"
    msfgSchemes.TextMatrix(0, 4) = "Issuer/MUT Fund"
    msfgSchemes.TextMatrix(0, 6) = "Sch Code"
End Sub
Private Sub msfgSchemes_DblClick()
On Error Resume Next
Dim RowSel As Integer
Dim rsDates As ADODB.Recordset
Dim i As Integer
    RowSel = msfgSchemes.Row
    If msfgSchemes.TextMatrix(RowSel, 2) <> "" Then
        If strscheme = "frmTransactionNew" Then
            If msfgSchemes.TextMatrix(RowSel, 1) = "Shares" Then
                MsgBox "Entry of IPO Transaction cannot be Done Here", vbInformation
                Exit Sub
            End If
            FrmtransactionNew.cmbProd.Text = msfgSchemes.TextMatrix(RowSel, 1)
            FrmtransactionNew.cmbproduct.Text = msfgSchemes.TextMatrix(RowSel, 4)
            For i = 1 To FrmtransactionNew.lstlongname.ListItems.count
                If FrmtransactionNew.lstlongname.ListItems(i).Text = msfgSchemes.TextMatrix(RowSel, 3) Then
                    FrmtransactionNew.lstlongname.ListItems(i).Selected = True
                    Exit For
                End If
            Next
            Dim lst As ListItem
            FrmtransactionNew.lstlongname_ItemClick lst
            For i = 0 To FrmtransactionNew.lstSch.ListCount - 1
                If FrmtransactionNew.lstSch.List(i) = msfgSchemes.TextMatrix(RowSel, 2) Then
                    FrmtransactionNew.lstSch.Selected(i) = True
                    Exit For
                End If
            Next
            FrmtransactionNew.lstSch_Click
            FrmtransactionNew.cmbBankName.SetFocus
        ElseIf strscheme = "frmBrokRecdSlabs" Then
            frmBrokRecdSlabs.cmbProdClass.Text = msfgSchemes.TextMatrix(RowSel, 1)
            frmBrokRecdSlabs.cmbMutFund.Text = msfgSchemes.TextMatrix(RowSel, 4)
            For i = 0 To frmBrokRecdSlabs.lstlongname.ListCount - 1
                If frmBrokRecdSlabs.lstlongname.List(i) = msfgSchemes.TextMatrix(RowSel, 3) Then
                    frmBrokRecdSlabs.lstlongname.Selected(i) = True
                    Exit For
                End If
            Next
            frmBrokRecdSlabs.lstlongname_Click
            For i = 0 To frmBrokRecdSlabs.lstSch.ListCount - 1
                If frmBrokRecdSlabs.lstSch.List(i) = msfgSchemes.TextMatrix(RowSel, 2) Then
                    frmBrokRecdSlabs.lstSch.Selected(i) = True
                    Exit For
                End If
            Next
            frmBrokRecdSlabs.lstSch_Click
         ElseIf strscheme = "frmBrokRecdSlabsRIO" Then
            frmBrokRecdSlabsRIO.cmbProdClass.Text = msfgSchemes.TextMatrix(RowSel, 1)
            frmBrokRecdSlabsRIO.cmbMutFund.Text = msfgSchemes.TextMatrix(RowSel, 4)
            For i = 0 To frmBrokRecdSlabsRIO.lstlongname.ListCount - 1
                If frmBrokRecdSlabsRIO.lstlongname.List(i) = msfgSchemes.TextMatrix(RowSel, 3) Then
                    frmBrokRecdSlabsRIO.lstlongname.Selected(i) = True
                    Exit For
                End If
            Next
            frmBrokRecdSlabsRIO.lstlongname_Click
            For i = 0 To frmBrokRecdSlabsRIO.lstSch.ListCount - 1
                If frmBrokRecdSlabsRIO.lstSch.List(i) = msfgSchemes.TextMatrix(RowSel, 2) Then
                    frmBrokRecdSlabsRIO.lstSch.Selected(i) = True
                    Exit For
                End If
            Next
            frmBrokRecdSlabsRIO.lstSch_Click
            frmBrokRecdSlabsRIO.chkInvAll = 1
       ElseIf strscheme = "frmtransactionmf" Then
            frmtransactionmf.CmbAmcA.Text = msfgSchemes.TextMatrix(RowSel, 4)
            For i = 0 To frmtransactionmf.lstlongnameA.ListCount - 1
                If UCase(Trim(Mid(frmtransactionmf.lstlongnameA.List(i), 1, 99))) = UCase(Trim(msfgSchemes.TextMatrix(RowSel, 3))) Then
                    frmtransactionmf.lstlongnameA.Selected(i) = True
                    frmtransactionmf.TxtSchemeA.Text = frmtransactionmf.lstlongnameA.Text
                    Exit For
                End If
            Next
            frmtransactionmf.CmbAmcA.SetFocus
        ElseIf strscheme = "FrmNfoSchemes" Then
            Set rsDates = New ADODB.Recordset
            rsDates.open "SELECT ISS_DATE,CLOSE_DATE FROM SCHEME_INFO WHERE SCH_CODE='" & msfgSchemes.TextMatrix(RowSel, 6) & "'", MyConn, adOpenForwardOnly
            FrmNfoSchemes.TSch_code.Text = msfgSchemes.TextMatrix(RowSel, 6)
            FrmNfoSchemes.TName.Text = msfgSchemes.TextMatrix(RowSel, 3)
            FrmNfoSchemes.Tissue.Text = rsDates("ISS_DATE")
            FrmNfoSchemes.Tclose.Text = rsDates("CLOSE_DATE")
            rsDates.Close
            Set rsDates = Nothing
        End If
        strscheme = ""
        Unload Me
    End If
End Sub
Private Sub msfgSchemes_KeyPress(KeyAscii As Integer)
    If KeyAscii = 13 Then
        msfgSchemes_DblClick
    End If
End Sub
Private Sub txtFind_KeyPress(KeyAscii As Integer)
Dim rsFind As New ADODB.Recordset
Dim strMF As String
Dim CodeMF As String
Dim strSearch As String
Dim i As Integer
    If KeyAscii = 39 Then KeyAscii = 146
    If KeyAscii = 13 Then
        msfgSchemes.Rows = 1
        msfgSchemes.Rows = 2
        If TxtFind.Text <> "" Then
            strSearch = ConvertString(UCase(Trim(TxtFind.Text)))
            rsFind.open "Select name,PROD_CODE from product_master where nature_code='NT001'", MyConn, adOpenForwardOnly
            strMF = rsFind("Name")
            CodeMF = rsFind("prod_code")
            rsFind.Close
            If Trim(GlbroleId) = 1 Then
                rsFind.open "select sch_code,sch_name,Short_name,mut_name from scheme_info s,mut_fund m where UPPER(MUT_NAME)||upper(sch_name)||UPPER(MUT_NAME) LIKE '%" & strSearch & "%' and s.mut_code=m.mut_code and m.mut_code not in ('MF001','MF036','MF002','MF013','MF039','MF029','MF027','MF035','MF020') ORDER BY sch_name,Short_name", MyConn, adOpenForwardOnly
            Else
                'rsFind.open "select sch_name,Short_name,mut_name from scheme_info s,mut_fund m where UPPER(MUT_NAME)||upper(sch_name)||UPPER(MUT_NAME) LIKE '%" & strSearch & "%' and s.mut_code=m.mut_code and s.sch_code in (SELECT sch_code FROM SCH_CITY_MAPPING WHERE city_id IN ((SELECT DISTINCT city_id FROM BRANCH_MASTER WHERE branch_code IN (" & Branches & "))) and iss_code like 'MF%' OR CITY_ID='ALL') ORDER BY sch_name,Short_name", myconn, adOpenForwardOnly
                rsFind.open "select sch_code,sch_name,Short_name,mut_name from scheme_info s,mut_fund m where UPPER(MUT_NAME)||upper(sch_name)||UPPER(MUT_NAME) LIKE '%" & strSearch & "%' and s.mut_code=m.mut_code and m.mut_code not in ('MF001','MF036','MF002','MF013','MF039','MF029','MF027','MF035','MF020') and s.sch_code in (SELECT sch_code FROM SCH_CITY_MAPPING WHERE (city_id IN ((SELECT DISTINCT city_id FROM BRANCH_MASTER WHERE branch_code IN (" & Branches & "))) OR CITY_ID='ALL') and iss_code like 'MF%' ) ORDER BY sch_name,Short_name", MyConn, adOpenForwardOnly
                'rsFind.open "select sch_name,Short_name,mut_name from scheme_info s,mut_fund m where UPPER(MUT_NAME)||upper(sch_name)||UPPER(MUT_NAME) LIKE '%" & strSearch & "%' and s.mut_code=m.mut_code and s.sch_code in (SELECT sch_code FROM SCH_CITY_MAPPING WHERE (city_id IN ((SELECT DISTINCT city_id FROM BRANCH_MASTER WHERE branch_code IN (" & Branches & "))) OR CITY_ID='ALL')) ORDER BY sch_name,Short_name", myconn, adOpenForwardOnly
            End If
            i = 1
            While Not rsFind.EOF
                msfgSchemes.TextMatrix(i, 1) = strMF
                msfgSchemes.TextMatrix(i, 2) = rsFind("SHORT_NAME")
                msfgSchemes.TextMatrix(i, 3) = rsFind("SCH_NAME")
                msfgSchemes.TextMatrix(i, 4) = rsFind("mut_name")
                msfgSchemes.TextMatrix(i, 5) = CodeMF
                msfgSchemes.TextMatrix(i, 6) = rsFind("sch_code")
                
                i = i + 1
                msfgSchemes.Rows = msfgSchemes.Rows + 1
                rsFind.MoveNext
            Wend
            rsFind.Close
            If Trim(GlbroleId) = 1 Then
                rsFind.open "select osch_code,osch_name,longname,name,iss_name from other_product o, product_master p,iss_master i where UPPER(ISS_NAME)||upper(longname)||UPPER(ISS_NAME) LIKE '%" & strSearch & "%' and o.prod_class_code=p.prod_code and o.iss_code=i.iss_code and (i.branch_code<>'N' or i.branch_code is null) and (o.flag<>'N' or o.flag is null) ORDER BY longname,osch_name", MyConn, adOpenForwardOnly
            Else
                'rsFind.open "select osch_name,longname,name,iss_name from other_product o, product_master p,iss_master i where UPPER(ISS_NAME)||upper(longname)||UPPER(ISS_NAME) LIKE '%" & strSearch & "%' and o.prod_class_code=p.prod_code and o.iss_code=i.iss_code and o.osch_code in (SELECT sch_code FROM SCH_CITY_MAPPING WHERE city_id IN ((SELECT DISTINCT city_id FROM BRANCH_MASTER WHERE branch_code IN (" & Branches & "))) and iss_code like 'IS%' OR CITY_ID='ALL') ORDER BY longname,osch_name", myconn, adOpenForwardOnly
                rsFind.open "select osch_code,osch_name,longname,name,iss_name from other_product o, product_master p,iss_master i where UPPER(ISS_NAME)||upper(longname)||UPPER(ISS_NAME) LIKE '%" & strSearch & "%' and o.prod_class_code=p.prod_code and o.iss_code=i.iss_code and (i.branch_code<>'N' or i.branch_code is null) and (o.flag<>'N' or o.flag is null) and o.osch_code in (SELECT sch_code FROM SCH_CITY_MAPPING WHERE (city_id IN ((SELECT DISTINCT city_id FROM BRANCH_MASTER WHERE branch_code IN (" & Branches & "))) OR CITY_ID='ALL') and (iss_code like 'IS%' or iss_code like 'MFIS%')) ORDER BY longname,osch_name", MyConn, adOpenForwardOnly
            End If
            While Not rsFind.EOF
                msfgSchemes.TextMatrix(i, 1) = rsFind("name")
                msfgSchemes.TextMatrix(i, 2) = rsFind("osch_name")
                msfgSchemes.TextMatrix(i, 3) = rsFind("longname")
                msfgSchemes.TextMatrix(i, 4) = rsFind("iss_name")
                msfgSchemes.TextMatrix(i, 6) = rsFind("osch_code")
                i = i + 1
                msfgSchemes.Rows = msfgSchemes.Rows + 1
                rsFind.MoveNext
            Wend
            rsFind.Close
            If msfgSchemes.Rows > 2 Then
                msfgSchemes.Rows = msfgSchemes.Rows - 1
                msfgSchemes.SetFocus
            End If
            AlignCols
        Else
            TxtFind.SetFocus
        End If
    End If
    Set rsFind = Nothing
End Sub
Private Sub AlignCols()
    msfgSchemes.ColAlignment(0) = 1
    msfgSchemes.ColAlignment(1) = 1
    msfgSchemes.ColAlignment(2) = 1
    msfgSchemes.ColAlignment(3) = 1
End Sub
