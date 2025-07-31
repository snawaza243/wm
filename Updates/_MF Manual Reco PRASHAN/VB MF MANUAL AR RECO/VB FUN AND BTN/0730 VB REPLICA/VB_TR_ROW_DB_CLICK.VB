Private Sub VSFCommGrdT_DblClick()
MyCurrRow1 = VSFCommGrdT.Row
'cmdConfirm.Visible = True
If VSFCommGrdT.TextMatrix(VSFCommGrdT.Row, 0) <> "" Then
      If MySelectedRow1 > 0 Then
          VSFCommGrdT.Row = MySelectedRow1
          For KCount_cOL = 0 To VSFCommGrdT.Cols - 1
               VSFCommGrdT.Col = KCount_cOL
                VSFCommGrdT.CellBackColor = vbWhite
                VSFCommGrdT.CellForeColor = vbBlack
                VSFCommGrdT.CellFontBold = False
          Next
      End If
      VSFCommGrdT.Row = MyCurrRow1
      For KCount_cOL = 0 To VSFCommGrdT.Cols - 1
        VSFCommGrdT.Col = KCount_cOL
        If VSFCommGrdT.CellBackColor = vbBlue Then
            VSFCommGrdT.CellBackColor = vbWhite
            VSFCommGrdT.CellForeColor = vbBlack
            VSFCommGrdT.CellFontBold = False
            If KCount_cOL = 18 Or KCount_cOL = 19 Then
                VSFCommGrdT.CellForeColor = vbWhite
            End If
        Else
            VSFCommGrdT.CellBackColor = vbBlue
            VSFCommGrdT.CellForeColor = vbWhite
            VSFCommGrdT.CellFontBold = True
            MySelectedRow1 = VSFCommGrdT.Row
            If KCount_cOL = 18 Or KCount_cOL = 19 Then
                VSFCommGrdT.CellForeColor = vbBlue
            End If
        End If
        If KCount_cOL = 18 Then
            If VSFCommGrdT.CellPicture = Image1.Picture Then
                Exit Sub
            End If
            If VSFCommGrdT.CellPicture = Image2.Picture Then
                Set VSFCommGrdT.CellPicture = Image22.Picture
                VSFCommGrdT.CellPictureAlignment = flexPicAlignCenterCenter
                VSFCommGrdT.Text = 1
            Else
                 Set VSFCommGrdT.CellPicture = Image2.Picture
                 VSFCommGrdT.CellPictureAlignment = flexPicAlignCenterCenter
                 VSFCommGrdT.Text = 0
            End If
        End If
    Next
    MyAppNo = ""
    MyChqNo = ""
    MyPan = ""
    MyFolio = ""
    MyAmount = 0
    MyTrCode = ""
    MyBroker = 0
    TxtInvestorName.Text = ""
    MyDispatch = ""
    MyTrCode = VSFCommGrdT.TextMatrix(VSFCommGrdT.Row, 0)
    TxtInvestorName.Text = VSFCommGrdT.TextMatrix(VSFCommGrdT.Row, 2)
    MyTrDate = CDate(Format(VSFCommGrdT.TextMatrix(VSFCommGrdT.Row, 1), "dd/mm/yyyy"))
    If IsNull(MyTrDate) Then
    Else
        mskfromdt.Text = MyTrDate - 30
        msktodt.Text = MyTrDate + 30
    End If
    TxtAmount.Text = VSFCommGrdT.TextMatrix(VSFCommGrdT.Row, 7)
    TxtSearch.Text = VSFCommGrdT.TextMatrix(VSFCommGrdT.Row, 9)
    If MyTrCode <> "" Then
        MyConn.Execute ("Update Transaction_mf_temp1 set PROCESSED=nvl(PROCESSED,0)+1 where tran_code='" & MyTrCode & "' ")
    End If
    Dim MyRs As New ADODB.Recordset
    cmdConfirm.Enabled = False
    MyRs.open "select pms_identify('" & MyTrCode & "') from dual", MyConn, adOpenForwardOnly
    If MyRs(0) = "1" Then
        cmdConfirm.Enabled = True
        cmdUnconfirm.Enabled = True
        If optATM.Value = False Then
            CmdMap.Enabled = False
        Else
            CmdMap.Enabled = True
        End If
    Else
        cmdConfirm.Enabled = False
        cmdUnconfirm.Enabled = False
        CmdMap.Enabled = True
    End If
    MyRs.Close
    
    If MyRs.State = 1 Then MyRs.Close
    MyRs.open "Select App_no,CHEQUE_NO,panno,folio_no,amount,remark_reco,source_code,DISPATCH,SCH_CODE,WEALTHMAKER.FN_IDENTIFY_REGISTRAR@MF.BAJAJCAPITAL(mut_code) registrar,NVL(cob_flag,0)cob_flag,MUT_CODE from transaction_mf_temp1 where tran_code='" & MyTrCode & "'", MyConn, adOpenForwardOnly, adLockReadOnly
    If Not MyRs.EOF Then
        MyAppNo = IIf(IsNull(MyRs.Fields("app_no")), "", MyRs.Fields("app_no"))
        MyChqNo = IIf(IsNull(MyRs.Fields("CHEQUE_NO")), "", MyRs.Fields("CHEQUE_NO"))
        MyPan = IIf(IsNull(MyRs.Fields("panno")), "", MyRs.Fields("panno"))
        MyFolio = IIf(IsNull(MyRs.Fields("folio_NO")), "", MyRs.Fields("folio_NO"))
        MyBroker = SqlRet("select exist_code from agent_master where agent_code='" & MyRs.Fields("source_code") & "'")
        MyAmount = IIf(IsNull(MyRs.Fields("amount")), 0, MyRs.Fields("amount"))
        MyDispatch = IIf(IsNull(MyRs.Fields("DISPATCH")), "", MyRs.Fields("DISPATCH"))
        MyProdCode = "" 'SqlRet("SELECT PROD_CLASS_CODE FROM OTHER_PRODUCT WHERE OSCH_CODE='" & MyRs.Fields("sch_code") & "'")
        TxtRemarks.Text = ""
        TxtRemarks.Text = IIf(IsNull(MyRs.Fields("REMARK_RECO")), "", MyRs.Fields("REMARK_RECO"))
        If MyRs.Fields("registrar") = "C" And MyRs.Fields("cob_flag") = "0" Then
            Optcams.Value = True
        ElseIf MyRs.Fields("registrar") = "K" And MyRs.Fields("cob_flag") = "0" Then
            OptKarvy.Value = True
        ElseIf MyRs.Fields("registrar") = "C" And MyRs.Fields("cob_flag") = "1" Then
            OptCamsCOB.Value = True
        ElseIf MyRs.Fields("registrar") = "K" And MyRs.Fields("cob_flag") = "1" Then
            OptKarvyCOB.Value = True
        End If
    Else
        TxtRemarks.Text = ""
        MyDispatch = ""
    End If
    
    
    CmbSearch.ListIndex = 0
    MyRs.Close
End If
End Sub

