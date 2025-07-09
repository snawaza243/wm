Dim RS As New ADODB.Recordset
Public ExlObj As Excel.Application 
Dim LI As ListItem
Dim li1 As ListItem
Dim j As Integer
Dim j1 As Integer
Dim X As Integer
Dim mnth As String
Dim Folio As String
Dim Mysql As String
 



Dim FlagSelect As Boolean
Dim Rs_Origin As ADODB.Recordset
Dim Br_origin As String
Dim XL As New Excel.Application
Dim XLW As Excel.Workbook
Dim cnt As Integer

Private Sub ClientDetails_DblClick()
    If ClientDetails.Col = 1 Then
        DeleteRow ClientDetails
    End If
End Sub
Private Sub ClientDetails_KeyDown(KeyCode As Integer, Shift As Integer)
    If KeyCode = 46 Then
        DeleteRow ClientDetails
    End If
End Sub



Private Sub cmdExcel_Click()
On Error GoTo errr
Dim i As Integer
Dim Sheet_Name As String
    ClientDetails.Clear
    ClientDetails.Rows = 2
    ClientDetails.FormatString = "S.No.|^Folio No."
    ClientDetails.ColWidth(0) = 550
    ClientDetails.ColWidth(1) = 1800
    cmdlgExcel.Filter = "Excel Files|*.xls"
    cmdlgExcel.ShowOpen
    file_name = ""
    If cmdlgExcel.CancelError = False Then
        file_name = cmdlgExcel.Filename
    End If
    If file_name <> "" Then
        Me.MousePointer = 11
        Set XLW = XL.Workbooks.open(file_name)
        Sheet_Name = XLW.Worksheets(1).Name
        i = 1
        While XLW.Sheets(Sheet_Name).Cells(i, 1) <> ""
            'Client_agent_check_add XLW.Sheets(Sheet_Name).Cells(i, 1)
             ClientDetails.Rows = ClientDetails.Rows + 1
             ClientDetails.TextMatrix(i, 1) = XLW.Sheets(Sheet_Name).Cells(i, 1)
             ClientDetails.TextMatrix(i, 0) = i
            i = i + 1
        Wend
        Me.MousePointer = 0
        XLW.Close
        XL.Quit
    End If
    ClientDetails.RemoveItem i
    Set XLW = Nothing
    Set XL = Nothing
Exit Sub
errr:
Me.MousePointer = 0
MsgBox err.Description
End Sub

Private Sub export()

Dim lc, NxtLine, K
Dim CSVData As String
Dim K1 As Long
Dim RetStr As String

dlgexp.Filter = "Excel Files|*.xls"
dlgexp.ShowSave
file_name = ""
If dlgexp.CancelError = False Then
    file_name = dlgexp.Filename
End If
        
If RS.State = 1 Then RS.Close

RS.open "select * from mftrail_exp", MyConn
If Not RS.EOF Then
    Me.MousePointer = 11
    RetStr = "sep=|" & vbNewLine & ""
    RetStr = RetStr & "FEED_SEQ|BC_NO|AMC_CODE|Folio|INVESTOR_NAME|PRODUCT_CODE|Units|amount|SUBBROKER_CODE|TRADE_DT|TRAIL_AMT|TRAIL_PAID_FROM_DT|TRAIL_PAID_TO_DT|DAYS_FEE_PAID_FROM_DT|DAYS_FEE_PAID_TO_DT|TRAIL_RATE|TRAIL_PAYABLE_AMT|flag|PS_SEQ|TRXN_ID|TRAIL_TYPE|DAYS_FEE_PAID|inv_Code|sch_code|FLAG_CAL|SUB_CD|FLAG_RATE|temp|SUB_CODE_LAST|CLIENT_RMCODE|BROK_CODE" & vbNewLine & ""
    If ShowColumnNames Then
        For K1 = 0 To RS.Fields.count - 1
            RetStr = RetStr & "|""" & RS.Fields(K1).Name & """"
        Next K1
        RetStr = Mid(RetStr, 2) & vbNewLine
    End If
        
        RetStr = RetStr & """" & RS.GetString(adClipString, -1, """|""", """" & vbNewLine & """", NULLStr)
        RetStr = Left(RetStr, Len(RetStr) - 3)
        
        CSVData = RetStr
    
    If file_name <> "" Then
        Open file_name For Binary Access Write As #1
            Put #1, , CSVData
        Close #1
    Else
        Exit Sub
    End If
End If

RS.Close
    
Me.MousePointer = 0




End Sub

Private Sub export_old()
Dim lc, NxtLine, K

dlgexp.Filter = "Excel Files|*.xls"
dlgexp.ShowSave
file_name = ""
If dlgexp.CancelError = False Then
        file_name = dlgexp.Filename
End If



        
    If RS.State = 1 Then RS.Close
            Set ExlObj = CreateObject("excel.application")      ' Initialize the excel object
            ExlObj.Workbooks.Add                                ' Add an excel workbook
            ' Get the required data from the database
            ExlObj.Application.DisplayAlerts = False
            RS.open "select * from mftrail_exp", MyConn
            If Not RS.EOF Then
              ExlObj.Visible = True
              With ExlObj.ActiveSheet
                ' Print the heading and columns
                .Cells(1, 3).Value = "MF Trail " & Format(Date, "dd-mmm-yyyy")
                
             
                   .Cells(1, 3).Font.Name = "Verdana"
                   .Cells(1, 3).Font.Bold = True:
                   
                   
                   .Cells(4, 1).Value = "FEED_SEQ":          .Cells(4, 2).Value = "BC_NO"
                   .Cells(4, 3).Value = "AMC_CODE":          .Cells(4, 4).Value = "Folio"
                   .Cells(4, 5).Value = "INVESTOR_NAME":     .Cells(4, 6).Value = "PRODUCT_CODE"
                   .Cells(4, 7).Value = "Units":             .Cells(4, 8).Value = "amount"
                   .Cells(4, 9).Value = "SUBBROKER_CODE":    .Cells(4, 10).Value = "TRADE_DT"
                   .Cells(4, 11).Value = "TRAIL_AMT":        .Cells(4, 12).Value = "TRAIL_PAID_FROM_DT"
                   .Cells(4, 13).Value = "TRAIL_PAID_TO_DT": .Cells(4, 14).Value = "TRAIL_RATE"
                   .Cells(4, 15).Value = "TRAIL_PAYABLE_AMT": .Cells(4, 16).Value = "flag"
                   .Cells(4, 17).Value = "PS_SEQ":           .Cells(4, 18).Value = "TRXN_ID"
                   .Cells(4, 19).Value = "TRAIL_TYPE":        .Cells(4, 20).Value = "DAYS_FEE_PAID"
                   .Cells(4, 21).Value = "inv_Code":       .Cells(4, 22).Value = "sch_code"
                   .Cells(4, 23).Value = "FLAG_CAL":           .Cells(4, 24).Value = "SUB_CD"
                   .Cells(4, 25).Value = "FLAG_RATE":          .Cells(4, 26).Value = "temp"
                   .Cells(4, 27).Value = "SUB_CODE_LAST":          .Cells(4, 28).Value = "CLIENT_RMCODE"
                   .Cells(4, 29).Value = "BROK_CODE":
                   
               End With
            End If
            For K = 1 To RS.Fields.count
                 ExlObj.ActiveSheet.Cells(4, K).Font.Bold = True
            Next
            Set K = Nothing
            NxtLine = 5
            Do Until RS.EOF
                For lc = 0 To RS.Fields.count - 1
                    ' Populate data into the sheet
                    ExlObj.ActiveSheet.Cells(NxtLine, lc + 1).Value = RS.Fields(lc)
                    If RS.Fields.Item(lc).Name <> "DATE" Then
                       ExlObj.ActiveSheet.Cells(NxtLine, lc + 1).Value = RS.Fields(lc)
                    Else
                       ExlObj.ActiveSheet.Cells(NxtLine, lc + 1).Value = Format(RS.Fields(lc), "dd/mm/yy")
                    End If
                    ' Autoformat the sheet
                   ' ExlObj.ActiveCell.Worksheet.Cells(NxtLine, lc + 1).AutoFormat _
                    '       xlRangeAutoFormatList2, 0, regular, 3, 1, 1
                Next
                RS.MoveNext
                NxtLine = NxtLine + 1
            Loop
            ExlObj.Columns(10).NumberFormat = "dd/mm/yyyy"
            ExlObj.Columns(12).NumberFormat = "dd/mm/yyyy"
            ExlObj.Columns(13).NumberFormat = "dd/mm/yyyy"
            
            If file_name <> "" Then
                ExlObj.ActiveWorkbook.SaveAs (file_name)
            Else
                Exit Sub
            End If
            
            ExlObj.Application.DisplayAlerts = False
            ExlObj.ActiveWorkbook.Close
            ExlObj.Application.Quit
            ExlObj.Quit
            Set ExlObj = Nothing
            'ExlObj.SaveWorkspace ("VINIT") 'VSaveWorkspace ("C:\VINITAUTO.XLS")
            'ExlObj.Quit
            ' Calculate the total
           ' ExlObj.ActiveCell.Worksheet.Cells(4, 1).Subtotal 4, xlSum, (6), 0, 0, xlSummaryBelow
           
            Screen.MousePointer = vbDefault
End Sub
Private Sub cmdExit_Click()
    Unload Me
End Sub
Private Sub CMDRESET_Click()
ClientDetails.Clear
ClientDetails.Rows = 2
chksel1_Click
CHKSEL_Click
End Sub
Private Sub CmdSave_Click()
If IsDate(mskfromdate) = False Or IsDate(msktodate) = False Then
    MsgBox "Please Enter Valid Dates", vbExclamation, strBajaj
    mskfromdate.SetFocus
    Exit Sub
End If

mnth = ""
Folio = ""




For X = 1 To ClientDetails.Rows - 1
    If ClientDetails.TextMatrix(X, 1) <> "" Then
        Folio = Folio & "'" & ClientDetails.TextMatrix(X, 1) & "',"
    End If
Next

If Len(Folio) <= 0 Then
        MsgBox "Please insert folio number", vbOKOnly
        Exit Sub
    Else
        Folio = Mid(Folio, 1, Len(Folio) - 1)
        'MsgBox Folio
End If





Mysql = "create or replace view mftrail_exp"
Mysql = Mysql & " as"
Mysql = Mysql & " SELECT * FROM( "
Mysql = Mysql & "SELECT FEED_SEQ, "
Mysql = Mysql & "       BC_NO, "
Mysql = Mysql & "       AMC_CODE, "
Mysql = Mysql & "       FOLIO, "
Mysql = Mysql & "       INVESTOR_NAME, "
Mysql = Mysql & "       PRODUCT_CODE, "
Mysql = Mysql & "       UNITS, "
Mysql = Mysql & "       AMOUNT, "
Mysql = Mysql & "       SUBBROKER_CODE, "
Mysql = Mysql & "       TRADE_DT, "
Mysql = Mysql & "       TRAIL_AMT, "
Mysql = Mysql & "       TRAIL_PAID_FROM_DT, "
Mysql = Mysql & "       TRAIL_PAID_TO_DT,NULL DAYS_FEE_PAID_FROM_DT,NULL DAYS_FEE_PAID_TO_DT, "
Mysql = Mysql & "       TRAIL_RATE, "
Mysql = Mysql & "       TRAIL_PAYABLE_AMT, "
Mysql = Mysql & "       FLAG, "
Mysql = Mysql & "       PS_SEQ, "
Mysql = Mysql & "       TRXN_ID, "
Mysql = Mysql & "       TRAIL_TYPE, "
Mysql = Mysql & "       DAYS_FEE_PAID, "
Mysql = Mysql & "       INV_CODE, "
Mysql = Mysql & "       SCH_CODE, "
Mysql = Mysql & "       FLAG_CAL, "
Mysql = Mysql & "       SUB_CD, "
Mysql = Mysql & "       FLAG_RATE, "
Mysql = Mysql & "       TEMP, "
Mysql = Mysql & "       SUB_CODE_LAST, "
Mysql = Mysql & "       CLIENT_RMCODE, "
Mysql = Mysql & "       BROK_CODE, "
Mysql = Mysql & "       TER_LOCATION, "
Mysql = Mysql & "       TR_LOCATION_UPD, "
Mysql = Mysql & "       IMPORT_COMPANY, "
Mysql = Mysql & "       TRAN_TYPE, "
Mysql = Mysql & "       NET_COMMISSION_AMOUNT, "
Mysql = Mysql & "       SOURCE_GST_STATE_CODE, "
Mysql = Mysql & "       COMPANY_GST_STATE_CODE, "
Mysql = Mysql & "       CGST_RATE, "
Mysql = Mysql & "       CGST_AMOUNT, "
Mysql = Mysql & "       SGST_RATE, "
Mysql = Mysql & "       SGST_AMOUNT, "
Mysql = Mysql & "       UTGST_RATE, "
Mysql = Mysql & "       UTGST_AMOUNT, "
Mysql = Mysql & "       IGST_RATE, "
Mysql = Mysql & "       IGST_AMOUNT, "
Mysql = Mysql & "       GST_CALCULATION_FLAG, "
Mysql = Mysql & "       MUT_CODE, "
Mysql = Mysql & "       MF_HO_GST_STATE_CODE "
Mysql = Mysql & "  FROM MFTRAIL.TRAIL_FEED@TEST_NEW.BAJAJCAPITAL "
Mysql = Mysql & "UNION ALL "
Mysql = Mysql & "SELECT FEED_SEQ, "
Mysql = Mysql & "       BC_NO, "
Mysql = Mysql & "       AMC_CODE, "
Mysql = Mysql & "       FOLIO, "
Mysql = Mysql & "       INVESTOR_NAME, "
Mysql = Mysql & "       PRODUCT_CODE, "
Mysql = Mysql & "       UNITS, "
Mysql = Mysql & "       AMOUNT, "
Mysql = Mysql & "       SUBBROKER_CODE, "
Mysql = Mysql & "       TRADE_DT, "
Mysql = Mysql & "       TRAIL_AMT, "
Mysql = Mysql & "       TRAIL_PAID_FROM_DT, "
Mysql = Mysql & "       TRAIL_PAID_TO_DT,DAYS_FEE_PAID_FROM_DT, DAYS_FEE_PAID_TO_DT, "
Mysql = Mysql & "       TRAIL_RATE, "
Mysql = Mysql & "       TRAIL_PAYABLE_AMT, "
Mysql = Mysql & "       FLAG, "
Mysql = Mysql & "       PS_SEQ, "
Mysql = Mysql & "       TRXN_ID, "
Mysql = Mysql & "       TRAIL_TYPE, "
Mysql = Mysql & "       DAYS_FEE_PAID, "
Mysql = Mysql & "       INV_CODE, "
Mysql = Mysql & "       SCH_CODE, "
Mysql = Mysql & "       FLAG_CAL, "
Mysql = Mysql & "       SUB_CD, "
Mysql = Mysql & "       FLAG_RATE, "
Mysql = Mysql & "       TEMP, "
Mysql = Mysql & "       SUB_CODE_LAST, "
Mysql = Mysql & "       CLIENT_RMCODE, "
Mysql = Mysql & "       BROK_CODE, "
Mysql = Mysql & "       TER_LOCATION, "
Mysql = Mysql & "       TR_LOCATION_UPD, "
Mysql = Mysql & "       IMPORT_COMPANY, "
Mysql = Mysql & "       TRAN_TYPE, "
Mysql = Mysql & "       NET_COMMISSION_AMOUNT, "
Mysql = Mysql & "       SOURCE_GST_STATE_CODE, "
Mysql = Mysql & "       COMPANY_GST_STATE_CODE, "
Mysql = Mysql & "       CGST_RATE, "
Mysql = Mysql & "       CGST_AMOUNT, "
Mysql = Mysql & "       SGST_RATE, "
Mysql = Mysql & "       SGST_AMOUNT, "
Mysql = Mysql & "       UTGST_RATE, "
Mysql = Mysql & "       UTGST_AMOUNT, "
Mysql = Mysql & "       IGST_RATE, "
Mysql = Mysql & "       IGST_AMOUNT, "
Mysql = Mysql & "       GST_CALCULATION_FLAG, "
Mysql = Mysql & "       MUT_CODE, "
Mysql = Mysql & "       MF_HO_GST_STATE_CODE "
Mysql = Mysql & "  FROM (SELECT * FROM MFTRAIL.TRAIL_FEED_NEW@TEST_NEW.BAJAJCAPITAL UNION ALL SELECT * FROM WEALTHMAKER.TRAIL_FEED_JAN18_DEC19@ARCHIVEDB_ORCL "
Mysql = Mysql & " ) "
Mysql = Mysql & "  ) "
Mysql = Mysql & " where FOLIO IN (" & Folio & ")"
Mysql = Mysql & " AND TRAIL_PAID_FROM_DT >= to_date('" & mskfromdate & "','DD-MM-YYYY') and  TRAIL_PAID_TO_DT <= to_date('" & msktodate & "','DD-MM-YYYY')"


MyConn.Execute Mysql

 'MyConn.Execute (Mysql)

export

End Sub



Private Sub Form_Load()
    Me.Top = (main.Height - Me.Height) / 2 - 600
    Me.Left = (main.width - Me.width) / 2
    Me.Icon = LoadPicture(App.Path & "\W.ICO")
    ClientDetails.FormatString = "S.No.|^Folio No."
    ClientDetails.ColWidth(0) = 550
    ClientDetails.ColWidth(1) = 1800
    Set LI = lstComp.ListItems.Add(, , "1") ' to use 01 lstComp.ListItems(j).Text
    LI.SubItems(1) = "JAN"
    Set LI = lstComp.ListItems.Add(, , "2")
    LI.SubItems(1) = "FEB"
    Set LI = lstComp.ListItems.Add(, , "3")
    LI.SubItems(1) = "MAR"
    Set LI = lstComp.ListItems.Add(, , "4")
    LI.SubItems(1) = "APR"
    Set LI = lstComp.ListItems.Add(, , "5")
    LI.SubItems(1) = "MAY"
    Set LI = lstComp.ListItems.Add(, , "6")
    LI.SubItems(1) = "JUN"
    Set LI = lstComp.ListItems.Add(, , "7")
    LI.SubItems(1) = "JUL"
    Set LI = lstComp.ListItems.Add(, , "8")
    LI.SubItems(1) = "AUG"
    Set LI = lstComp.ListItems.Add(, , "9")
    LI.SubItems(1) = "SEP"
    Set LI = lstComp.ListItems.Add(, , "10")
    LI.SubItems(1) = "OCT"
    Set LI = lstComp.ListItems.Add(, , "11")
    LI.SubItems(1) = "NOV"
    Set LI = lstComp.ListItems.Add(, , "12")
    LI.SubItems(1) = "DEC"

    Set li1 = lstPlans.ListItems.Add(, , "2010")
    li1.SubItems(1) = "2010"
    Set li1 = lstPlans.ListItems.Add(, , "2011")
    li1.SubItems(1) = "2011"
    Set li1 = lstPlans.ListItems.Add(, , "2012")
    li1.SubItems(1) = "2012"
    Set li1 = lstPlans.ListItems.Add(, , "2013")
    li1.SubItems(1) = "2013"
    Set li1 = lstPlans.ListItems.Add(, , "2014")
    li1.SubItems(1) = "2014"
    Set li1 = lstPlans.ListItems.Add(, , "2015")
    li1.SubItems(1) = "2015"
    Set li1 = lstPlans.ListItems.Add(, , "2016")
    li1.SubItems(1) = "2016"
    Set li1 = lstPlans.ListItems.Add(, , "2017")
    li1.SubItems(1) = "2017"
    Set li1 = lstPlans.ListItems.Add(, , "2018")
    li1.SubItems(1) = "2018"
    Set li1 = lstPlans.ListItems.Add(, , "2019")
    li1.SubItems(1) = "2019"
    Set li1 = lstPlans.ListItems.Add(, , "2020")
    li1.SubItems(1) = "2020"
    Set li1 = lstPlans.ListItems.Add(, , "2021")
    li1.SubItems(1) = "2021"
    Set li1 = lstPlans.ListItems.Add(, , "2022")
    li1.SubItems(1) = "2022"



End Sub


Private Sub chksel1_Click()
'lstPlans.ListItems.Clear
Call SelectUnSelectListView(lstPlans, chksel1.Value)
End Sub

Private Sub CHKSEL_Click()
'lstComp.ListItems.Clear
Call SelectUnSelectListView(lstComp, CHKSEL.Value)
End Sub
Private Sub SelectUnSelectListView(lst As ListView, Chk_Value As Integer)
If Chk_Value = 0 Then
    For i = 1 To lst.ListItems.count
        lst.ListItems(i).Checked = False
    Next
Else
    For i = 1 To lst.ListItems.count
        lst.ListItems(i).Checked = True
    Next
End If
End Sub






