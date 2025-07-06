Option Explicit
Dim importExcelcon As New ADODB.Connection
Dim RsHeader As New ADODB.Recordset
Dim a As Variant
Dim Count_Loop As Integer
Dim rslap As New ADODB.Recordset
Dim Chk_Lst_Sel As Boolean
Dim XL As New Excel.Application
Dim XLW As Excel.Workbook
Dim RsDueDate As New ADODB.Recordset
Dim XL1 As New Excel.Application
Dim XLW1 As Excel.Workbook
Dim sql As String
Dim Sheet_Name As String, File_content As String
Dim fso As New Scripting.filesystemobject
Dim delComma() As String
Dim dataBaseField As String
Dim MyImportDataType As String
Dim MyImport As String
Dim selectedFileField As String
Dim delHash() As String
Dim rsExcel As New ADODB.Recordset
Dim SqlStr As String
Dim Field_value As String, Excel_policy_no As String, Excel_Status As String
Dim Rs_Find_due As ADODB.Recordset, Excel_Comp As String
Dim Excel_Due_Date As String
Dim Excel_Payment_Mode As String
Dim Excel_Prem_Freq As String
Dim doc As String
Dim excel_mobile As String
Dim excel_pol_term As String
Dim Excel_Prem_Amt As String
Dim excel_sa As String
Dim MyPremFreq As String
Dim Rs_Chk_Excel As New ADODB.Recordset
Dim Rec_Count As Long
Dim Rec_Count_exl As Long
Dim rec1 As Long
Dim Rs_Dup_policy As New ADODB.Recordset
Dim Dup_Policy_Str As String
Dim Rec_Del As Integer
Dim UpMon As Integer
Dim UpYear As Integer
Dim MyLapsedDate As Date

Dim KOUNT_Loop As Integer
Dim sheetName As String
Dim filepath As String
Dim RsImport As New ADODB.Recordset
Dim TxtFile_Name As String

Private Sub CmbDataType_Click()
MyImportDataType = ""
MyImport = ""
'Due    0
'Lapsed 1
'Paid   2
'Reins  3
If CmbDataType.ListIndex = 0 Then 'DUE
    MyImportDataType = "DUEDATA"
    MyImport = "D"
    OptDue.Value = True
    OptPaid.Caption = "Paid Data"
    OptDue.Caption = "Due Data"
ElseIf CmbDataType.ListIndex = 1 Then 'LAPSED
    MyImportDataType = "LAPSEDDATA"
    MyImport = "L"
    OptDue.Value = True
    OptPaid.Caption = "Reinstate Data"
    OptDue.Caption = "Lappsed Data"
ElseIf CmbDataType.ListIndex = 2 Then 'PAID
    MyImportDataType = "DUEDATA"
    MyImport = "D"
    OptPaid.Value = True
    OptPaid.Caption = "Paid Data"
    OptDue.Caption = "Due Data"
ElseIf CmbDataType.ListIndex = 3 Then 'REINS
    MyImportDataType = "LAPSEDDATA"
    MyImport = "L"
    OptPaid.Value = True
    OptPaid.Caption = "Reinstate Data"
    OptDue.Caption = "Lappsed Data"
End If
End Sub
Private Sub CmdBrowse_Click()
TxtFileName.Text = ""
Cdilog.Filename = ""
Cdilog.Filter = "Excel Files|*.xls"
Cdilog.ShowOpen
TxtFileName.Text = Cdilog.Filename
lstsheet.Clear
If TxtFileName.Text <> "" Then
    Set XLW = XL.Workbooks.open(TxtFileName.Text)
    For Count_Loop = 1 To XLW.Worksheets.count
        lstsheet.AddItem XLW.Worksheets(Count_Loop).Name
    Next
    'XLW.Close
    'XL.Quit
End If
'Set XLW = Nothing
'Set XL = Nothing
End Sub

Private Sub CmdDone_Click()
Dim sql As String
Dim RS As New ADODB.Recordset
Dim MyReportCaption As String
MyReportCaption = ""
If CmbDataType.Text = "DUE DATA" Then 'due
   MyReportCaption = "Due Data Imported Policy"
   sql = " SELECT POLICY_NO, COMPANY_CD,IMPORTDATATYPE "
   sql = sql & "  FROM BAJAJ_DUE_DATA "
   sql = sql & " WHERE MON_NO = " & CmbMonth.ListIndex + 1 & " AND YEAR_NO = " & TxtYear.Text & " AND IMPORTDATATYPE = 'DUEDATA' "
ElseIf CmbDataType.Text = "LAPSED DATA" Then 'LAPSED
   MyReportCaption = "Lapsed Data Imported Policy"
   sql = " SELECT POLICY_NO, COMPANY_CD,IMPORTDATATYPE "
   sql = sql & "  FROM BAJAJ_DUE_DATA "
   sql = sql & " WHERE MON_NO = " & CmbMonth.ListIndex + 1 & " AND YEAR_NO = " & TxtYear.Text & " AND IMPORTDATATYPE = 'LAPSEDDATA' "
ElseIf CmbDataType.Text = "PAID DATA" Then 'PAID
   MyReportCaption = "Paid Data Imported Policy"
   sql = " SELECT POLICY_NO, COMPANY_CD,IMPORTDATATYPE "
   sql = sql & "  FROM BAJAJ_PAID_DATA "
   sql = sql & " WHERE MON_NO = " & CmbMonth.ListIndex + 1 & " AND YEAR_NO = " & TxtYear.Text & " AND IMPORTDATATYPE = 'DUEDATA' "
ElseIf CmbDataType.Text = "REINSTATE DATA" Then 'REINS
   MyReportCaption = "Reinstate Data Imported Policy"
   sql = " SELECT POLICY_NO, COMPANY_CD,IMPORTDATATYPE "
   sql = sql & "  FROM BAJAJ_PAID_DATA "
   sql = sql & " WHERE MON_NO = " & CmbMonth.ListIndex + 1 & " AND YEAR_NO = " & TxtYear.Text & " AND IMPORTDATATYPE = 'LAPSEDDATA' "
End If
Populate_Data sql
Call exportGRIDAll(msfgClients, MyReportCaption, "Policy Imported", "Sheet1")
If RS.State = 1 Then RS.Close
MsgBox "All The Policy Has Been Exported Sucessfully", vbInformation
End Sub



Private Sub cmdImport_Click()
'On Error Resume Next
'On Error GoTo err
If TxtFileName.Text = "" Then
    MsgBox "Select Valid Excel File", vbInformation
    CmdBrowse.SetFocus
    Exit Sub
End If
If lstsheet.ListCount = 0 Then
    MsgBox "No Excel Sheet Is Available For Importing Data "
    CmdBrowse.SetFocus
    Exit Sub
End If
Chk_Lst_Sel = False
For Count_Loop = 0 To lstsheet.ListCount - 1
    If lstsheet.Selected(Count_Loop) = True Then
        Sheet_Name = lstsheet.List(Count_Loop)
        Chk_Lst_Sel = True
        Exit For
    End If
Next
If Chk_Lst_Sel = False Then MsgBox "Select Excel Sheet For Importing": Exit Sub
If Val(TxtYear.Text) <= 1900 And Val(TxtYear.Text) > 2100 Then
    MsgBox "Enter Valid Year", vbInformation
    TxtYear.SetFocus
    Exit Sub
End If
'Code For Saving The Excel Heading'
'Set XLW = XL.Workbooks.open(TxtFileName.Text)
Count_Loop = 1
While XLW.Worksheets(Sheet_Name).Cells(1, Count_Loop) <> ""
    XLW.Worksheets(Sheet_Name).Cells(1, Count_Loop) = Replace(XLW.Worksheets(Sheet_Name).Cells(1, Count_Loop), ".", "")
    XLW.Worksheets(Sheet_Name).Cells(1, Count_Loop) = Replace(XLW.Worksheets(Sheet_Name).Cells(1, Count_Loop), "/", "")
    XLW.Worksheets(Sheet_Name).Cells(1, Count_Loop) = Replace(XLW.Worksheets(Sheet_Name).Cells(1, Count_Loop), "-", "")
    Count_Loop = Count_Loop + 1
Wend
XLW.save: XLW.Close: XL.Quit
'Set XLW = Nothing: Set XL = Nothing
'End Code For Saving The Excel Files
If fso.FileExists(App.Path & "\Life\insufld\" & "Field_Parameter_Paid.txt") = False Then
    MsgBox "Field Parameter File Not Found", vbInformation
    Exit Sub
End If
If OptDue.Value = True Then
    File_content = FieldsParametersName(App.Path & "\Life\insufld\" & "Field_Parameter_due.txt")
Else
    File_content = FieldsParametersName(App.Path & "\Life\insufld\" & "Field_Parameter_paid.txt")
End If
If File_content = "" Then
    MsgBox "File Format Is Wrong ", vbInformation
    Exit Sub
End If
delComma = Split(File_content, ","): selectedFileField = "": dataBaseField = ""
Excel_policy_no = "": Excel_Status = "": Excel_Comp = ""
For Count_Loop = 0 To UBound(delComma)
    delHash = Split(delComma(Count_Loop), "#")
    selectedFileField = selectedFileField & delHash(1) & ","
    dataBaseField = dataBaseField & delHash(0) & ","
    If UCase(delHash(0)) = "POLICY_NO" Then
        Excel_policy_no = Replace(Replace(Replace(delHash(1), "[", ""), "]", ""), "-", "")
    End If
    If UCase(delHash(0)) = "COMPANY_CD" Then
        Excel_Comp = Replace(Replace(delHash(1), "[", ""), "]", "")
    End If
    If UCase(delHash(0)) = "PAY_MODE" Then
        Excel_Payment_Mode = Replace(Replace(Replace(delHash(1), "[", ""), "]", ""), "-", "")
    End If
    If UCase(delHash(0)) = "PREM_FREQ" Then
        Excel_Prem_Freq = Replace(Replace(delHash(1), "[", ""), "]", "")
    End If
    If UCase(delHash(0)) = "DUE_DATE" Then
        Excel_Due_Date = Replace(Replace(delHash(1), "[", ""), "]", "")
    End If
    If OptPaid.Value = True Then
        If UCase(delHash(0)) = "STATUS_CD" Then
            Excel_Status = Replace(Replace(delHash(1), "[", ""), "]", "")
        End If
    End If
    If UCase(delHash(0)) = "DOC" Then
        doc = Replace(Replace(delHash(1), "[", ""), "]", "")
    End If
    If UCase(delHash(0)) = "PREM_AMT" Then
        Excel_Prem_Amt = Replace(Replace(delHash(1), "[", ""), "]", "")
    End If
    If UCase(delHash(0)) = "CL_MOBILE" Then
        excel_mobile = Replace(Replace(delHash(1), "[", ""), "]", "")
    End If
    If UCase(delHash(0)) = "PLY_TERM" Then
        excel_pol_term = Replace(Replace(delHash(1), "[", ""), "]", "")
    End If
    If UCase(delHash(0)) = "SA" Then
        excel_sa = Replace(Replace(delHash(1), "[", ""), "]", "")
    End If
Next



selectedFileField = Left(selectedFileField, Len(selectedFileField) - 1)
dataBaseField = Left(dataBaseField, Len(dataBaseField) - 1)
If importExcelcon.State = adStateOpen Then importExcelcon.Close:   Set importExcelcon = Nothing
Set importExcelcon = New ADODB.Connection
importExcelcon.open "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" & TxtFileName.Text & ";Extended Properties=""Excel 8.0;HDR=Yes;"";"
Set rsExcel = New ADODB.Recordset
Set rsExcel = importExcelcon.Execute("Select " & selectedFileField & " from [" & Sheet_Name & "$] ")
Rec_Count = 0
Rec_Count_exl = 0
Label9 = Rec_Count_exl
rec1 = 0

If OptDue.Value = True Then
   Do While Not rsExcel.EOF
         If rsExcel("" & Excel_Comp & "").Value & "" <> "" Then
            SqlStr = " select * from bajaj_due_Data WHERE upper(trim(POLICY_no))=upper(trim('" & UCase(Trim(rsExcel("" & Trim(Excel_policy_no) & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "' and mon_no = " & CmbMonth.ListIndex + 1 & " and year_no=" & TxtYear.Text & " and importdatatype='" & MyImportDataType & "' "
            Set Rs_Chk_Excel = New ADODB.Recordset
            Rs_Chk_Excel.open SqlStr, MyConn, adOpenDynamic, adLockOptimistic
            If Rs_Chk_Excel.EOF = True Then
            ' commented by pankaj according to parvesh
            '  If UCase(Trim(rsExcel("" & Trim(Excel_Comp) & "").Value)) <> "BIRLA SUN" Then
                    MyPremFreq = ""
                    If UCase(Trim(rsExcel("" & Trim(Excel_Prem_Freq) & "").Value)) = UCase(Trim("1")) Or UCase(Trim(rsExcel("" & Trim(Excel_Prem_Freq) & "").Value)) = UCase(Trim("01")) Or UCase(Trim(rsExcel("" & Trim(Excel_Prem_Freq) & "").Value)) = UCase(Trim("ANNUALLY")) Or UCase(Trim(rsExcel("" & Trim(Excel_Prem_Freq) & "").Value)) = UCase(Trim("ANNUAL")) Or UCase(Trim(rsExcel("" & Trim(Excel_Prem_Freq) & "").Value)) = UCase(Trim("YEARLY")) Then
                        MyPremFreq = 1
                    ElseIf UCase(Trim(rsExcel("" & Trim(Excel_Prem_Freq) & "").Value)) = UCase(Trim("12")) Or UCase(Trim(rsExcel("" & Trim(Excel_Prem_Freq) & "").Value)) = UCase(Trim("MONTHLY")) Then
                        MyPremFreq = 12
                    ElseIf UCase(Trim(rsExcel("" & Trim(Excel_Prem_Freq) & "").Value)) = UCase(Trim("0")) Then
                        MyPremFreq = 0
                    ElseIf UCase(Trim(rsExcel("" & Trim(Excel_Prem_Freq) & "").Value)) = UCase(Trim("Quarterly")) Then
                        MyPremFreq = 4
                    ElseIf UCase(Trim(rsExcel("" & Trim(Excel_Prem_Freq) & "").Value)) = UCase(Trim("2")) Or UCase(Trim(rsExcel("" & Trim(Excel_Prem_Freq) & "").Value)) = UCase(Trim("SEMI ANNUALLY")) Or UCase(Trim(rsExcel("" & Trim(Excel_Prem_Freq) & "").Value)) = UCase(Trim("SEMI ANNUAL")) Or UCase(Trim(rsExcel("" & Trim(Excel_Prem_Freq) & "").Value)) = UCase(Trim("SEMI-ANNUALLY")) Or UCase(Trim(rsExcel("" & Trim(Excel_Prem_Freq) & "").Value)) = UCase(Trim("SEMI-ANNUAL")) Or UCase(Trim(rsExcel("" & Trim(Excel_Prem_Freq) & "").Value)) = UCase(Trim("HALF YEARLY")) Then
                        MyPremFreq = 2
                    ElseIf UCase(Trim(rsExcel("" & Trim(Excel_Prem_Freq) & "").Value)) = UCase(Trim("4")) Then
                        MyPremFreq = 4
                    End If
            '    End If
                SqlStr = "Insert into BAJAJ_due_DATA (" & dataBaseField & ",Mon_no,Year_No,UserId,Import_Dt,ImportDataType,NEWINSERT)  Values("
                For Count_Loop = 0 To rsExcel.Fields.count - 1
                    If InStr(1, rsExcel(Count_Loop), "'") > 0 Then
                        Field_value = Replace(rsExcel(Count_Loop), "'", "")
                        SqlStr = SqlStr & "'" & Trim(Field_value) & "',"
                    Else
                        If rsExcel(Count_Loop).Type = 7 Then
                            SqlStr = SqlStr & "'" & Format(rsExcel(Count_Loop), "dd-mmm-yyyy") & "',"
                            If UCase(Trim(rsExcel(Count_Loop).Name)) = UCase(Excel_Due_Date) Then
                                 If Format(rsExcel(Count_Loop), "dd-mmm-yyyy") <> "" Then MyLapsedDate = Format(rsExcel(Count_Loop), "dd-mmm-yyyy")
                            End If
                        Else
                            If InStr(Trim(rsExcel(Count_Loop)), ",") > 0 Then
                                SqlStr = SqlStr & "'" & Replace(Trim(rsExcel(Count_Loop)), ",", "") & "',"
                            Else
                                SqlStr = SqlStr & "'" & Trim(rsExcel(Count_Loop)) & "',"
                            End If
                        End If
                    End If
                Next
                SqlStr = SqlStr & "" & CmbMonth.ListIndex + 1 & "," & TxtYear.Text & ",'" & Glbloginid & "','" & Format(ServerDateTime, "dd-mmm-yyyy") & "','" & MyImportDataType & "','BAL' "
                SqlStr = SqlStr & ")"
                SqlStr = Replace(SqlStr, "''", "Null")
                Rec_Count = Rec_Count + 1
                If MyImportDataType = "LAPSEDDATA" Then
                       MyLapsedDate = MyLapsedDate
                       UpMon = Format(MyLapsedDate, "mm")
                       UpYear = Format(MyLapsedDate, "yyyy")
                        sql = " SELECT   policy_no, company_cd, MAX (due_date) due_date,max(mon_no),max(year_no), "
                        sql = sql & "         (SELECT MAX (status_cd) "
                        sql = sql & "            FROM bajaj_due_data "
                        sql = sql & "           WHERE UPPER (TRIM (policy_no)) = "
                        sql = sql & "                                         UPPER (TRIM ('" & UCase(Trim(rsExcel("" & Trim(Excel_policy_no) & "").Value)) & "')) "
                        sql = sql & "             AND UPPER (TRIM (company_cd)) = '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "'"
                        sql = sql & "             AND importdatatype = 'DUEDATA' "
                        sql = sql & "             AND due_date = "
                        sql = sql & "                    (SELECT MAX (due_date) "
                        sql = sql & "                       FROM bajaj_due_data "
                        sql = sql & "                      WHERE UPPER (TRIM (policy_no)) = "
                        sql = sql & "                                                    UPPER (TRIM ('" & UCase(Trim(rsExcel("" & Trim(Excel_policy_no) & "").Value)) & "')) "
                        sql = sql & "                        AND UPPER (TRIM (company_cd)) = "
                        sql = sql & "                                                   '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "' "
                        sql = sql & "                        AND due_date IS NOT NULL AND IMPORTDATATYPE='DUEDATA' "
                        sql = sql & "                        )) status_cd "
                        sql = sql & "    FROM bajaj_due_data a "
                        sql = sql & "   WHERE   importdatatype = 'DUEDATA' "
                        sql = sql & "                      AND UPPER (TRIM (policy_no)) = "
                        sql = sql & "                                                    UPPER (TRIM ('" & UCase(Trim(rsExcel("" & Trim(Excel_policy_no) & "").Value)) & "')) "
                        sql = sql & "                        AND UPPER (TRIM (company_cd)) = "
                        sql = sql & "                                                   '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "' "
                        sql = sql & "GROUP BY policy_no, company_cd "
                        RsDueDate.open sql, MyConn, adOpenForwardOnly, adLockOptimistic
                        If RsDueDate.EOF = False Then
                            If MyLapsedDate >= RsDueDate.Fields("due_date") Then
                                MyConn.Execute "update bajaj_due_Data set status_Cd='LAPSED',last_update_dt='" & Format(Now, "dd-mmm-yyyy") & "',last_update='" & Glbloginid & "' WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "' and due_date='" & Format(RsDueDate.Fields("due_date"), "dd-mmm-yyyy") & "' and importdatatype='DUEDATA' "
                                MyConn.Execute "update policy_details_master set last_status='L',UPDATE_PROG='" & CmbDataType.Text & "',UPDATE_USER='" & Glbloginid & "',UPDATE_DATE=TO_DATE('" & Format(ServerDateTime, "DD/MM/YYYY") & "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "'"
                                Call update_bajajar_status(UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)), UCase(Trim(rsExcel("" & Excel_Comp & "").Value)), "L", "LAPSED DATA")
                            End If
                        End If
                        RsDueDate.Close
                End If
                MyConn.Execute SqlStr
                
                If MyImportDataType = "DUEDATA" Then
                    MyConn.Execute "update policy_details_master set FILE_NAME='" & TxtFileName.Text & "', PAYMENT_MODE='" & UCase(Trim(rsExcel("" & Excel_Payment_Mode & "").Value)) & "',UPDATE_PROG='" & CmbDataType.Text & "',UPDATE_USER='" & Glbloginid & "',UPDATE_DATE=TO_DATE('" & Format(ServerDateTime, "DD/MM/YYYY") & "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "'"
                    If MyPremFreq <> "" Then
                        MyConn.Execute "update policy_details_master set FILE_NAME='" & TxtFileName.Text & "', PREM_FREQ='" & MyPremFreq & "',UPDATE_PROG='" & CmbDataType.Text & "',UPDATE_USER='" & Glbloginid & "',UPDATE_DATE=TO_DATE('" & Format(ServerDateTime, "DD/MM/YYYY") & "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "'"
                    End If
                    
                    
                    If UCase(Trim(rsExcel("" & doc & "").Value)) <> "" Then
                        MyConn.Execute "update policy_details_master set doc=to_date('" & UCase(Trim(rsExcel("" & doc & "").Value)) & "','dd/mm/rrrr'),UPDATE_USER='" & Glbloginid & "',UPDATE_DATE=TO_DATE('" & Format(ServerDateTime, "DD/MM/YYYY") & "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "'"
                    End If
                    
                    If UCase(Trim(rsExcel("" & excel_pol_term & "").Value)) <> "" Then
                        MyConn.Execute "update policy_details_master set ply_term='" & UCase(Trim(rsExcel("" & excel_pol_term & "").Value)) & "',UPDATE_USER='" & Glbloginid & "',UPDATE_DATE=TO_DATE('" & Format(ServerDateTime, "DD/MM/YYYY") & "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "'"
                    End If
                    If UCase(Trim(rsExcel("" & excel_mobile & "").Value)) <> "" Then
                        MyConn.Execute "update policy_details_master set mobile='" & UCase(Trim(rsExcel("" & excel_mobile & "").Value)) & "',UPDATE_USER='" & Glbloginid & "',UPDATE_DATE=TO_DATE('" & Format(ServerDateTime, "DD/MM/YYYY") & "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "'"
                    End If
                    If UCase(Trim(rsExcel("" & excel_sa & "").Value)) <> "" Then
                        MyConn.Execute "update policy_details_master set sa='" & UCase(Trim(rsExcel("" & excel_sa & "").Value)) & "',UPDATE_USER='" & Glbloginid & "',UPDATE_DATE=TO_DATE('" & Format(ServerDateTime, "DD/MM/YYYY") & "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "'"
                    End If
                    If UCase(Trim(rsExcel("" & Excel_Prem_Amt & "").Value)) <> "" Then
                        MyConn.Execute "update policy_details_master set prem_amt='" & UCase(Trim(rsExcel("" & Excel_Prem_Amt & "").Value)) & "',UPDATE_USER='" & Glbloginid & "',UPDATE_DATE=TO_DATE('" & Format(ServerDateTime, "DD/MM/YYYY") & "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "'"
                    End If
                    
                    
                End If
                Label5.Caption = Rec_Count
                DoEvents
            End If
            DoEvents
            Rs_Chk_Excel.Close
         End If
         Label9 = Rec_Count_exl
         Rec_Count_exl = Rec_Count_exl + 1
         If Rec_Count_exl = 5501 Then
            'MsgBox UCase(Trim(rsExcel("" & Trim(Excel_policy_no) & "").Value))
         End If
         rsExcel.MoveNext
    Loop
    SqlStr = " select  distinct policy_no from (select a.policy_no,a.sys_ar_Dt,a.company_Cd from bajaj_Ar_head a ,bajaj_due_Data b where"
    SqlStr = SqlStr & " upper(trim(a.COMPANY_CD)) = upper(trim(B.COMPANY_CD)) And a.Policy_No = B.Policy_No and mon_no = " & CmbMonth.ListIndex + 1 & " and year_no=" & TxtYear.Text & " and importdatatype='" & MyImportDataType & "'"
    SqlStr = SqlStr & " group by a.policy_no,a.sys_ar_dt,a.company_Cd having count(a.policy_no)>1 and count(a.sys_ar_dt)>1 and count(a.company_Cd)>1)"
    Set Rs_Dup_policy = New ADODB.Recordset
    Rs_Dup_policy.open SqlStr, MyConn, adOpenDynamic, adLockOptimistic
    Dup_Policy_Str = ""
    MyConn.Execute "DELETE FROM DUP_POLICY"
    Do While Not Rs_Dup_policy.EOF
        MyConn.Execute "INSERT INTO DUP_POLICY VALUES('" & Rs_Dup_policy!policy_no & "')"
        Dup_Policy_Str = Dup_Policy_Str & "'" & Rs_Dup_policy!policy_no & "'" & ","
        Rs_Dup_policy.MoveNext
    Loop
    If Dup_Policy_Str <> "" Then Dup_Policy_Str = Left(Dup_Policy_Str, Len(Dup_Policy_Str) - 1)
    Rec_Del = 0
    If Dup_Policy_Str <> "" Then
        MyConn.Execute " update bajaj_due_Data set dup_rec='Y' where policy_no in (SELECT POLICY_NO FROM DUP_POLICY) and mon_no =" & CmbMonth.ListIndex + 1 & " and year_no=" & TxtYear.Text & " and importdatatype='" & MyImportDataType & "' ", Rec_Del
    End If
    Rec_Count = Rec_Count - Rec_Del
ElseIf OptPaid.Value = True Then
    Do While Not rsExcel.EOF
        If rsExcel("" & Excel_Comp & "").Value & "" <> "" Then
            If (CmbDataType.ListIndex = 2 Or CmbDataType.ListIndex = 3) Then 'paid and reinstate
                If rslap.State = 1 Then rslap.Close
                rslap.open "select POLICY_NO  from bajaj_due_data where UPPER(TRIM(POLICY_no))=UPPER(TRIM('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "' and mon_no =" & CmbMonth.ListIndex + 1 & " and year_no=" & TxtYear.Text & " AND IMPORTDATATYPE='" & MyImportDataType & "'", MyConn
                If rslap.EOF Then
                    SqlStr = "Insert into BAJAJ_DUE_DATA (" & dataBaseField & ",Mon_no,Year_No,UserId,Import_Dt,ImportDataType,NEWINSERT,FORCE_FLAG)  Values("
                    For Count_Loop = 0 To rsExcel.Fields.count - 1
                       If InStr(1, rsExcel(Count_Loop), "'") > 0 Then
                           Field_value = Replace(rsExcel(Count_Loop), "'", "")
                           SqlStr = SqlStr & "'" & Field_value & "',"
                       Else
                           If rsExcel(Count_Loop).Type = 7 Then
                               SqlStr = SqlStr & "'" & Format(rsExcel(Count_Loop), "dd-mmm-yyyy") & "',"
                           Else
                               SqlStr = SqlStr & "'" & Trim(rsExcel(Count_Loop)) & "',"
                           End If
                       End If
                    Next
                    SqlStr = SqlStr & " " & CmbMonth.ListIndex + 1 & "," & TxtYear.Text & ",'" & Glbloginid & "','" & Format(ServerDateTime, "dd-mmm-yyyy") & "','" & MyImportDataType & "' "
                    SqlStr = SqlStr & ",NULL,'FORCE FULL')"
                    SqlStr = Replace(SqlStr, "''", "Null")
                    MyConn.Execute SqlStr
                    DoEvents
                 End If
            End If
            SqlStr = " select * from bajaj_paid_Data WHERE upper(trim(POLICY_no))=upper(trim('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "'  and mon_no=" & CmbMonth.ListIndex + 1 & " and year_no=" & TxtYear.Text & " and importdatatype='" & MyImportDataType & "' "
            Set Rs_Chk_Excel = New ADODB.Recordset
            Rs_Chk_Excel.open SqlStr, MyConn, adOpenDynamic, adLockOptimistic
            
            If Rs_Chk_Excel.EOF = True Then
                Dim xy As Integer
                xy = 0
                If RsHeader.State = 1 Then RsHeader.Close
                RsHeader.open "Select policy_no,company_cd,last_status from policy_details_master WHERE UPPER(TRIM(POLICY_no))=UPPER(TRIM('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "' ", MyConn, adOpenStatic, adLockOptimistic
                If RsHeader.EOF = False Then
                    If UCase(RsHeader.Fields("last_status")) = "L" Then
                          If UCase(rsExcel("" & Excel_Status & "")) = "PAID" Then
                               SqlStr = " update policy_details_master set last_status='R',UPDATE_PROG='" & CmbDataType.Text & "',UPDATE_USER='" & Glbloginid & "',UPDATE_DATE=to_date('" & Format(ServerDateTime, "DD/MM/YYYY") & "','dd/mm/yyyy') WHERE UPPER(TRIM(POLICY_no))=UPPER(TRIM('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "' "
                               MyConn.Execute SqlStr
                          End If
                    End If
                Else
                    MyConn.Execute ("insert into PolicyNotInHeader(policy_no,company_Cd) values(('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "'),'" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "')")
                End If
           
                If rslap.State = 1 Then rslap.Close
                rslap.open "select max(to_date('01/'||mon_no||'/'||year_no,'dd/mm/yyyy')) from_dt from bajaj_due_data where UPPER(TRIM(POLICY_no))=UPPER(TRIM('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "' and status_cd='LAPSED'", MyConn
                If Not IsNull(rslap(0)) Then
                    SqlStr = "update bajaj_due_Data set status_Cd='" & UCase(Trim(rsExcel("" & Excel_Status & ""))) & "',last_update_dt='" & Format(ServerDateTime, "dd-mmm-yyyy") & "',last_update='" & Glbloginid & "' WHERE UPPER(TRIM(POLICY_no))=upper(trim('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "' and importdatatype='" & MyImportDataType & "' AND to_date('01/'||mon_no||'/'||year_no,'dd/mm/yyyy')>'" & Format(rslap(0), "dd-MMM-yyyy") & "' and to_date('01/'||mon_no||'/'||year_no,'dd/mm/yyyy')<=last_day(to_date('01-" & CmbMonth.Text & "-" & TxtYear.Text & "','dd-mm-yyyy')) "
                Else
                    SqlStr = "update bajaj_due_Data set status_Cd='" & UCase(Trim(rsExcel("" & Excel_Status & ""))) & "',last_update_dt='" & Format(ServerDateTime, "dd-mmm-yyyy") & "',last_update='" & Glbloginid & "' WHERE UPPER(TRIM(POLICY_no))=upper(trim('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "' and importdatatype='" & MyImportDataType & "' AND mon_no =" & CmbMonth.ListIndex + 1 & " and year_no=" & TxtYear.Text & ""
                End If
                MyConn.Execute SqlStr, xy
                If UCase(Trim(rsExcel("" & Excel_Status & ""))) = "PAID" And xy > 0 Then  'Jis Month Ka Paid Data Hai Agara Us Month Kak Due Data Update Ho gaya
                    SqlStr = "Insert into BAJAJ_PAID_DATA (" & dataBaseField & ",Mon_no,Year_No,UserId,Import_Dt,importdatatype)  Values("
                    For Count_Loop = 0 To rsExcel.Fields.count - 1
                        If InStr(1, rsExcel(Count_Loop), "'") > 0 Then
                            Field_value = Replace(rsExcel(Count_Loop), "'", "")
                            SqlStr = SqlStr & "'" & Field_value & "',"
                        Else
                            If rsExcel(Count_Loop).Type = 7 Then
                                SqlStr = SqlStr & "'" & Format(rsExcel(Count_Loop), "dd-mmm-yyyy") & "',"
                            Else
                                SqlStr = SqlStr & "'" & Trim(rsExcel(Count_Loop)) & "',"
                            End If
                        End If
                    Next
                    SqlStr = SqlStr & "" & CmbMonth.ListIndex + 1 & "," & TxtYear.Text & ",'" & Glbloginid & "','" & Format(ServerDateTime, "dd-mmm-yyyy") & "','" & MyImportDataType & "' "
                    SqlStr = SqlStr & ")"
                    SqlStr = Replace(SqlStr, "''", "Null")
                    Rec_Count = Rec_Count + 1
                    MyConn.Execute SqlStr
                    MyConn.Execute "update policy_details_master set last_status='A',UPDATE_PROG='" & CmbDataType.Text & "',UPDATE_USER='" & Glbloginid & "',UPDATE_DATE=to_date('" & Format(ServerDateTime, "DD/MM/YYYY") & "','dd/mm/yyyy') WHERE UPPER(TRIM(POLICY_no))=UPPER(TRIM('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "'"
                    Call update_bajajar_status(UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)), UCase(Trim(rsExcel("" & Excel_Comp & "").Value)), "F", "PAID DATA")
                    Label5.Caption = Rec_Count
                    DoEvents

                End If
            Else
                MyConn.Execute SqlStr
                SqlStr = " update bajaj_paid_Data set status_Cd='" & UCase(Trim(rsExcel("" & Excel_Status & ""))) & "',last_update_dt='" & Format(Now, "dd-mmm-yyyy") & "',last_update='" & Glbloginid & "' WHERE UPPER(TRIM(POLICY_no))=UPPER(TRIM('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "' and mon_no=" & CmbMonth.ListIndex + 1 & " and year_no=" & TxtYear.Text & " and importdatatype='" & MyImportDataType & "' "
                MyConn.Execute SqlStr
                rec1 = rec1 + 1
                Label7 = rec1
                DoEvents
            End If
            Rs_Chk_Excel.Close
            DoEvents
        End If
        Label9 = Rec_Count_exl
        Rec_Count_exl = Rec_Count_exl + 1
        rsExcel.MoveNext
    Loop
    SqlStr = " select  policy_no from ( select a.policy_no,a.sys_ar_Dt,a.company_Cd from bajaj_Ar_head a ,bajaj_paid_Data b where"
    SqlStr = SqlStr & " upper(trim(a.COMPANY_CD)) = upper(trim(B.COMPANY_CD)) And a.Policy_No = B.Policy_No and mon_no = " & CmbMonth.ListIndex + 1 & " and year_no=" & TxtYear.Text & " and importdatatype='" & MyImportDataType & "'"
    SqlStr = SqlStr & " group by a.policy_no,a.sys_ar_dt,a.company_Cd having count(a.policy_no)>1 and count(a.sys_ar_dt)>1 and count(A.company_cd)>1)"
    Set Rs_Dup_policy = New ADODB.Recordset
    Rs_Dup_policy.open SqlStr, MyConn, adOpenDynamic, adLockOptimistic
    Dup_Policy_Str = ""
    MyConn.Execute "DELETE FROM DUP_POLICY"
    Do While Not Rs_Dup_policy.EOF
        MyConn.Execute "INSERT INTO DUP_POLICY VALUES('" & Rs_Dup_policy!policy_no & "')"
        Dup_Policy_Str = Dup_Policy_Str & "'" & Rs_Dup_policy!policy_no & "'" & ","
        Rs_Dup_policy.MoveNext
    Loop
    If Dup_Policy_Str <> "" Then Dup_Policy_Str = Left(Dup_Policy_Str, Len(Dup_Policy_Str) - 1)
    Rec_Del = 0
    If Dup_Policy_Str <> "" Then
        MyConn.Execute " update bajaj_paid_Data set dup_rec='Y' where policy_no in (SELECT POLICY_NO FROM DUP_POLICY) and mon_no =" & CmbMonth.ListIndex + 1 & " and year_no=" & TxtYear.Text & " and importdatatype='" & MyImportDataType & "' ", Rec_Del
    End If
    Rec_Count = Rec_Count - Rec_Del
    MyConn.Execute " update bajaj_paid_data A set A.emp_no " & _
    " =(select MAX(B.emp_no) from bajaj_ar_head B where upper(trim(B.Company_cd))=upper(trim(A.Company_cd)) and B.policy_no=A.policy_no AND B.FRESH_RENEWAL=1), " & _
    " inv_cd=(select MAX(B.client_Cd) from bajaj_ar_head B where upper(trim(B.Company_cd))=upper(trim(A.Company_cd)) and B.policy_no=A.policy_no  AND B.FRESH_RENEWAL=1) " & _
    "where mon_no= " & CmbMonth.ListIndex + 1 & " and year_no= " & TxtYear.Text & " and dup_Rec is null and importdatatype='" & MyImportDataType & "'"
End If 'End Of Main If
rsExcel.Close
Set rsExcel = Nothing
importExcelcon.Close
TxtFileName.Text = ""
lstsheet.Clear
MsgBox "Records Imported Successfully", vbInformation
Exit Sub
err:
Resume
MsgBox err.Description
    If OptDue.Value = True And CmbDataType.Text = "DUE DATA" Then
        'myconn.Execute ("insert into bajaj_due_Data_missing(policy_no,company_cd,loggeduserid,errordesc) values('" & UCase(Trim(rsExcel("" & Trim(Excel_policy_no) & "").Value)) & "' ,'" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "','" & Glbloginid & "','" & Replace(err.Description, "`", "") & "')")
    End If
End Sub

Private Sub CmdMap_Click()
Dim RS_Temp As New ADODB.Recordset
Dim exeCutionTime As String
Dim currentTime As String
Dim flag As Integer
If MyImport = "" Then
   MsgBox "Please Select The DataType To Which You Want To Map", vbInformation
   Exit Sub
End If
If TxtYear = "" Then
   MsgBox "Please Select The Year For Which You Want To Map", vbInformation
   Exit Sub
End If
If MsgBox("Are You Sure You Want To Map The Recods Now") = vbNo Then
   Exit Sub
End If
If RS_Temp.State = 1 Then RS_Temp.Close
RS_Temp.open "select SYSDATE ,TO_CHAR(SYSDATE + (1/1440),'DD/MM/YYYY HH12:MI:SS AM') DATE1 FROM DUAL", MyConn, adOpenForwardOnly, adLockReadOnly
If Not RS_Temp.EOF Then
    exeCutionTime = RS_Temp(1)
End If
If RS_Temp.State = 1 Then RS_Temp.Close
'MyConn.Execute "call RunDueProcedure(" & val(cmbMonth.ListIndex + 1) & "," & val(TxtYear) & ",'" & MyImport & "')"
flag = SqlRet("select count(*) from MAPPED_SUCCESSFULLY where mon_no=" & Val(CmbMonth.ListIndex + 1) & " and year_no=" & Val(TxtYear) & " and IMPORTDATATYPE='" & MyImport & "' and LOGGEDUSERID='" & Glbloginid & "' and TIMEST='" & Format(ServerDateTime, "dd-mmm-yyyy") & "'")
If flag = 0 Then
    sql = ""
    sql = " DECLARE "
    sql = sql & "  X NUMBER; "
    sql = sql & "BEGIN "
    sql = sql & "  SYS.DBMS_JOB.SUBMIT "
    sql = sql & "    ( "
    sql = sql & "      job        => X "
    sql = sql & "     ,what       => 'RUNDUEPROCEDURE(''" & Val(CmbMonth.ListIndex + 1) & "'',''" & Val(TxtYear) & "'',''" & MyImport & "'');' "
    sql = sql & "     ,next_date  => to_date('" & exeCutionTime & "','dd/mm/yyyy hh12:mi:ss am') "
    sql = sql & "     ,no_parse   => FALSE "
    sql = sql & "    ); "
    sql = sql & "  "
    sql = sql & "END; "
    MyConn.Execute (sql)
    sql = " INSERT INTO  MAPPED_SUCCESSFULLY(MON_NO,YEAR_NO,IMPORTDATATYPE,TIMEST,LOGGEDUSERID) "
    sql = sql & " VALUES(" & Val(CmbMonth.ListIndex + 1) & "," & Val(TxtYear) & ",'" & MyImport & "','" & Format(ServerDateTime, "dd-mmm-yyyy") & "','" & Glbloginid & "')"
    MyConn.Execute sql
End If
MsgBox " Job For To Mapp The Data Is  Successfully Submitted For The Month " & Val(CmbMonth.ListIndex + 1) & " and year " & TxtYear.Text & " ", vbInformation
End Sub


Private Sub CmdMappingPlan_Click()
'Shell (App.Path & "\Project1.exe")
frmmappingplan.Show
frmmappingplan.MonNo = CmbMonth.ListIndex + 1
frmmappingplan.ZOrder
End Sub

Private Sub Form_Load()
CmbMonth.Clear
For Count_Loop = 1 To 12
    CmbMonth.AddItem MonthName(Count_Loop, True), Count_Loop - 1
Next
CmbMonth.ListIndex = 0
TxtYear.Text = Format(Now, "YYYY")
CmbDataType.AddItem ("DUE DATA")
CmbDataType.AddItem ("LAPSED DATA")
CmbDataType.AddItem ("PAID DATA")
CmbDataType.AddItem ("REINSTATE DATA")
CmbDataType.ListIndex = 0
End Sub

Private Sub Form_QueryUnload(Cancel As Integer, UnloadMode As Integer)
On Error Resume Next
'RsImport.Close
'Set RsImport = Nothing
'rsChk.Close
'Set rsChk = Nothing
End Sub

Private Sub Form_Unload(Cancel As Integer)
On Error Resume Next
'RsImport.Close
'Set RsImport = Nothing
'rsChk.Close
'Set rsChk = Nothing
'Set ImportExcelcon = Nothing'
End Sub
Private Sub cmdExit_Click()
Unload Me
End Sub
Public Function FieldsParametersName(txtFile As String) As String
Dim FileObj As filesystemobject, FS As Object
Set FileObj = CreateObject("Scripting.Filesystemobject")
Set fso = New filesystemobject
Set FS = FileObj.OpenTextFile(txtFile)
FieldsParametersName = FS.ReadLine
Set FileObj = Nothing
End Function




Private Sub FrmMap_DragDrop(Source As Control, X As Single, Y As Single)

End Sub

Private Sub Picture1_Click()
If lstsheet.ListCount = 0 Then
   MsgBox "Please First Select Sheet", vbInformation, "Main Form"
   Exit Sub
End If
If lstsheet.Text <> "" Then
    For KOUNT_Loop = 0 To lstsheet.ListCount - 1
        If lstsheet.Selected(KOUNT_Loop) = True Then
           sheetName = lstsheet.List(KOUNT_Loop)
           Exit For
        End If
    Next
Else
   MsgBox "Please First Select Sheet", vbInformation, "Main Form"
   Exit Sub
End If
Call Get_Comp_TextFilePath
If TxtFileName.Text = "" Then MsgBox "Select Valid File", vbInformation: Exit Sub
Set importExcelcon = New ADODB.Connection
If importExcelcon.State = adStateOpen Then Set importExcelcon = Nothing
importExcelcon.open "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" & TxtFileName.Text & " ;Extended Properties=""Excel 8.0;HDR=Yes;"";"
Set RsImport = importExcelcon.Execute("Select * from [" & sheetName & "$] ")
Set Rs_Map_Exl_Fld = RsImport
If InStr(1, TxtFile_Name, ":") > 0 Then Map_Exl_Path = TxtFile_Name Else Map_Exl_Path = App.Path & "\Life\insufld\" & TxtFile_Name 'App.Path & "\Life\insufld\" & "Field_Parameter_due.txt"
Set fso = New Scripting.filesystemobject
If fso.FileExists(Map_Exl_Path) = False Then fso.CreateTextFile (Map_Exl_Path)
Map_DataBAse_FldName = " "
Map_DataBAse_FldName = "Company_Cd/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Status_cd/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Location/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Policy_No/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "CL_Name/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Prem_Amt/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Pay_Mode/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Due_date/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "SA/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Cl_Add1/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Cl_add2/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Cl_add3/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Cl_add4/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Cl_add5/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Cl_City/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Cl_pin/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Cl_Phone1/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Cl_phone2/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Cl_mobile/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "plan_name/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "prem_freq/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Doc/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "ply_term/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "ecs_date/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "next_due_date/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "paid_date/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "net_amount/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "fup_date/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "ppt/"
Load FrmMapField_JPM
FrmMapField_JPM.lblmap = "** Status,Scheme,Frequency Field Are Mandatory "
FrmMapField_JPM.Frame3.Caption = "Excel File Fields"
FrmMapField_JPM.Show vbModal
RsImport.Close
Set RsImport = Nothing
Set Rs_Map_Exl_Fld = Nothing
End Sub
Private Sub Get_Comp_TextFilePath()
If OptDue.Value = True Then
    TxtFile_Name = "Field_Parameter_due.txt"
ElseIf OptPaid.Value = True Then
    TxtFile_Name = "Field_Parameter_paid.txt"
End If
End Sub

Private Sub update_bajajar_status(ply_no As String, Comp_Cd As String, status_cd As String, import_type As String)
On Error GoTo err
Dim RS As New ADODB.Recordset
Dim rschk As New ADODB.Recordset
RS.open "SELECT * FROM BAJAJ_AR_HEAD WHERE POLICY_NO='" & ply_no & "' AND COMPANY_CD='" & Comp_Cd & "' AND TO_CHAR(SYS_AR_DT,'MON-YYYY')= '" & UCase(CmbMonth.Text) & "-" & TxtYear.Text & "' and status_cd='" & status_cd & "'", MyConn
If RS.EOF = False Then
    rschk.open "SELECT * FROM BAJAJ_AR_DETAILS WHERE SYS_AR_NO='" & RS("sys_aR_no") & "' and status_dt=last_day(to_date('" & RS("sys_aR_dt") & "','dd/mm/yyyy'))", MyConn
    If rschk.EOF = True Then
         MyConn.Execute ("update bajaj_AR_head set status_cd='" & status_cd & "' where sys_aR_no='" & RS("sys_aR_no") & "'")
         MyConn.Execute "INSERT INTO BAJAJ_AR_DETAILS (SYS_AR_NO,STATUS_DT,STATUS_CD,REMARKS,SYS_AR_DT,STATUS_UPDATE_ON)  VALUES('" & RS("sys_aR_no") & "',last_day('" & Format(RS("sys_aR_dt"), "DD-MMM-YYYY") & "'),'" & status_cd & "','" & import_type & "'||' '||sysdate ,'" & Format(RS("sys_aR_dt"), "DD-MMM-YYYY") & "' ,SYSDATE)"
    End If
End If
Set RS = Nothing
Set rschk = Nothing
Exit Sub
err:
    MsgBox "error in while status updating", vbInformation
End Sub

Private Sub Populate_Data(strQuery As String)
On Error GoTo err1
    MSRClient.sql = ""
    MSRClient.sql = strQuery
    MSRClient.Refresh
    Exit Sub
err1:
    'MsgBox Err.Description
End Sub

