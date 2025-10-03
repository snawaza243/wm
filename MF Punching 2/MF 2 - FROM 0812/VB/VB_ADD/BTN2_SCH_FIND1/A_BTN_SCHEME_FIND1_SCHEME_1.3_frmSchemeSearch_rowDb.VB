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
