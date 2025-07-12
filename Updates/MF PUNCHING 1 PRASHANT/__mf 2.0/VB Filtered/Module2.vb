
Public Filename As String
Public MyAddress3 As String
Public UserPassword As String
Public UserNewPassword As String
Global emsg As String
Global wMsg As String
Dim rsBeforeSlab As New ADODB.Recordset
Dim rsAfterSlab As New ADODB.Recordset
Public MyOpt As Integer
Public BEFORE_PAID_CAL_ON As String
Public BEFORE_PER_RS As String
Public BEFORE_FIGURE As String
Public BEFORE_DUE_AFTER As String
Public BEFORE_VALID_TILL As String
Public BEFORE_CAL_ON As String
Public BEFORE_USER As String
Public BEFORE_LAST_USER As String
Public BEFORE_LAST_DATE As String

Public AFTER_PAID_CAL_ON As String
Public AFTER_PER_RS As String
Public AFTER_FIGURE As String
Public AFTER_DUE_AFTER As String
Public AFTER_VALID_TILL As String
Public AFTER_CAL_ON As String
Public AllIndiaSearchFlag As String
Public MyLoggedUserid As String
Public MyUpdProc As String
Public MyMainCode As String

Public Sub ExcelToDatabase(workbookPath As String, tabname As String, sheetno As Integer)
Dim XL As Object
Dim wbXL As Object
Dim wsXL As Object
Dim AddOfLastCell As String
Dim tempstr1 As String, tempstr2 As String, Fcolchar As String, Scolchar As String, Rowstr As String
Dim NoRow As Integer, NoCol As Integer
Dim counteri As Integer, counterj As Integer
Dim arrdata() As String
Set XL = CreateObject("excel.application", "")
Set wbXL = XL.Workbooks.open(workbookPath)
Set wsXL = wbXL.Worksheets.Item(sheetno)
wsXL.Activate
AddOfLastCell = XL.Range("a1:a1").SpecialCells(xlCellTypeLastCell).Address
tempstr1 = Mid(AddOfLastCell, 2, InStr(2, AddOfLastCell, "$") - 2)
If Len(tempstr1) > 1 Then
    Fcolchar = Mid(tempstr1, 1, 1)
    Scolchar = Mid(tempstr1, 2, 1)
    NoCol = (Asc(Fcolchar) + 1 - 65) * 26 + Asc(Scolchar) + 1 - 65
    NoCol = NoCol
Else
    NoCol = Asc(tempstr1) + 1 - 65
    NoCol = NoCol
End If
Rowstr = Mid(AddOfLastCell, InStr(2, AddOfLastCell, "$") + 1)
ReDim arrdata(1, NoCol)
If sheetno = 2 Then
For counteri = 2 To Rowstr
    insertstr = ""
    For counterj = 1 To NoCol
        If counterj >= 27 Then
             try = "A" & Chr(64 + counterj - 26)
            If counterj = NoCol Then
                insertstr = insertstr & "'" & wbXL.Worksheets(sheetno).Range(try & counteri).Value & "'"
            ElseIf counterj = 36 Then
                insertstr = insertstr & "'" & Format(wbXL.Worksheets(sheetno).Range(try & counteri).Value, "yyyy/mm/dd") & "',"
            Else
                insertstr = insertstr & "'" & wbXL.Worksheets(sheetno).Range(try & counteri).Value & "',"
            End If
        Else
            try = Chr(64 + counterj)
            If counterj = NoCol Then
                insertstr = insertstr & "'" & wbXL.Worksheets(sheetno).Range(try & counteri).Value & "')"
            ElseIf counterj = 13 Or counterj = 17 Then
                insertstr = insertstr & "'" & Format(wbXL.Worksheets(sheetno).Range(try & counteri).Value, "yyyy/mm/dd") & "',"
            Else
                insertstr = insertstr & "'" & wbXL.Worksheets(sheetno).Range(try & counteri).Value & "',"

            End If
        End If
    Next
    FINALSTR = "insert into " & tabname & " values(" & insertstr & ")"
    MyConn.Execute FINALSTR
Next
XL.Quit
Set wbXL = Nothing
Set XL = Nothing
End If
If sheetno = 1 Then
For counteri = 2 To Rowstr
    insertstr = ""
    For counterj = 1 To NoCol
        If counterj >= 27 Then
             try = "A" & Chr(64 + counterj - 26)
            If counterj = NoCol Then
                insertstr = insertstr & "'" & wbXL.Worksheets(sheetno).Range(try & counteri).Value & "'"
            Else
                insertstr = insertstr & "'" & wbXL.Worksheets(sheetno).Range(try & counteri).Value & "',"
            End If
        Else
            try = Chr(64 + counterj)
            If counterj = NoCol Then
                insertstr = insertstr & "'" & wbXL.Worksheets(sheetno).Range(try & counteri).Value & "')"
            ElseIf counterj = 16 Then
                insertstr = insertstr & "'" & Format(wbXL.Worksheets(sheetno).Range(try & counteri).Value, "yyyy/mm/dd") & "',"
            Else
                insertstr = insertstr & "'" & wbXL.Worksheets(sheetno).Range(try & counteri).Value & "',"
            End If
        End If
    Next
    FINALSTR = "insert into " & tabname & " values(" & insertstr & ")"
    MyConn.Execute FINALSTR
Next
XL.Quit
Set wbXL = Nothing
Set XL = Nothing
End If
End Sub
Public Sub DataInGrid(tblname As String, gridname As Control)
Dim CounA As Integer, counB As Integer
Dim Crow As Integer, Ccol As Integer
Dim rsS As New ADODB.Recordset
rsS.open tblname, MyConn, adOpenKeyset, adLockOptimistic
If rsS.EOF Then
    Exit Sub
End If
Crow = 0
rsS.MoveFirst
Do While Not rsS.EOF
    Crow = Crow + 1
    rsS.MoveNext
Loop
Crow = rsS.RecordCount
Ccol = rsS.Fields.count
gridname.Cols = Ccol
gridname.Rows = Crow + 1
gridname.AllowUserResizing = flexResizeColumns
rsS.MoveFirst
Do While Not rsS.EOF
    For CounA = 0 To Crow - 1
        For counB = 0 To Ccol - 1
            If IsNull(rsS.Fields(counB)) Then
                gridname.TextMatrix(CounA + 1, counB) = ""
            Else
                gridname.TextMatrix(CounA + 1, counB) = CStr(rsS.Fields(counB))
            End If
        Next
        rsS.MoveNext
    Next
Loop
Set rsS = Nothing
'Set connect = Nothing
End Sub
Public Sub exportGRIDToExcel(rpGrid_EXP1 As MSFlexGrid, rpGrid_EXP2 As MSFlexGrid, sheetno1 As Byte, sheetno2 As Byte)
Dim fso As New Scripting.filesystemobject
Dim rsS As New ADODB.Recordset
Dim XL As Object
Dim wbXL As Object
Dim wsXL As Object
Dim lRow As Integer, lRows As Integer
Dim lCol As Integer, lCols As Integer
Dim lCaption As String
Dim FL As String
Dim Z As Boolean
Static count As Integer
Dim i As Integer, j As Integer
Set XL = CreateObject("Excel.Application", "")
If Not IsObject(XL) Then
    MsgBox "You need Microsoft Excel to use this function", vbExclamation, "Print to Excel"
    Exit Sub
End If
Set rsS = MyConn.Execute("select agent_code from master where type='BR'")
If rsS.EOF Then
    code = "Invalid"
Else
    code = Right(rsS(0), 8)
End If
Set rsS = Nothing
If fso.FolderExists(App.Path & "\master_report") = True Then
        Filename = "mast-" & code & "-" & Replace(Date, "/", "-") & ".xls"
Else
        fso.CreateFolder (App.Path & "\master_report")
        Filename = "mast-" & code & "-" & Replace(Date, "/", "-") & ".xls"
End If
Set wbXL = XL.Workbooks.Add
wbXL.Worksheets(sheetno1).Activate
Set wsXL = XL.ActiveSheet
For i = 0 To rpGrid_EXP1.Rows - 1
    For j = 0 To rpGrid_EXP1.Cols - 1
            wsXL.Cells(i + 1, j + 1) = rpGrid_EXP1.TextMatrix(i, j)
    Next
Next
For i = 0 To rpGrid_EXP1.Cols - 1
    wsXL.Cells(1, i + 1).Font.Bold = True
Next
wbXL.Worksheets(sheetno2).Activate
Set wsXL = XL.ActiveSheet
For i = 0 To rpGrid_EXP2.Rows - 1
    For j = 0 To rpGrid_EXP2.Cols - 1
        wsXL.Cells(i + 1, j + 1) = rpGrid_EXP2.TextMatrix(i, j)
    Next
Next
For i = 0 To rpGrid_EXP2.Cols - 1
    wsXL.Cells(1, i + 1).Font.Bold = True
Next
FL = App.Path & "\master_report\" & Filename
Z = fso.FileExists(FL)
If Z = True Then
ms = MsgBox("Today data has been exported do you want to export it again", vbYesNo)
flag = 1
If ms = 6 Then
XL.ActiveWorkbook.SaveAs FL, 0
flag = 1
End If
If ms = 7 Then
flag = 0
Exit Sub
End If
Else
XL.ActiveWorkbook.SaveAs FL
End If
Set wsXL = Nothing
Set wbXL = Nothing
XL.Quit
Set XL = Nothing
Set fso = Nothing
End Sub
'Its import new data from branch to ho
Public Sub InsertData(TableName As String, FieldCount As Integer, Data() As String, rsTab As Variant)
Dim strInsert As String
Dim i As Integer
    strInsert = "Insert into " & TableName & " Values("
    For i = 0 To FieldCount - 1
        If rsTab.Fields(i).Type = 131 Then
            strInsert = strInsert & Data(i) & ","
        ElseIf rsTab.Fields(i).Type = 135 Then
            If Data(i) <> "NULL" Then
                strInsert = strInsert & "to_date('" & Format(Data(i), "dd-mm-yyyy") & "','dd-mm-yyyy'),"
            Else
                strInsert = strInsert & Data(i) & ","
            End If
        Else
            strInsert = strInsert & "'" & Data(i) & "',"
        End If
    Next
    strInsert = Left(strInsert, Len(strInsert) - 1)
    strInsert = strInsert & ")"
    MyConn.Execute strInsert
End Sub
'Its update the existing data in ho data which comes from branch  to ho
Public Sub UpdateData(keyfield() As String, keydata() As String, datatypes() As String, TableName As String, FieldCount As Integer, Data() As String, rsTab As Variant)
Dim strInsert As String
Dim i As Integer
    strInsert = "Update " & TableName & " set "
    For i = 0 To FieldCount - 1
        If rsTab.Fields(i).Type = 131 Then
            strInsert = strInsert & rsTab.Fields(i).Name & "=" & Data(i) & ","
        ElseIf rsTab.Fields(i).Type = 135 Then
            If Data(i) <> "NULL" Then
                strInsert = strInsert & rsTab.Fields(i).Name & "=" & "to_date('" & Format(Data(i), "dd-mm-yyyy") & "','dd-mm-yyyy'),"
            Else
                strInsert = strInsert & rsTab.Fields(i).Name & "=" & Data(i) & ","
            End If
        Else
            strInsert = strInsert & rsTab.Fields(i).Name & "=" & "'" & Data(i) & "',"
        End If
    Next
    strInsert = Left(strInsert, Len(strInsert) - 1)
    strInsert = strInsert & " where 1=1"
    For i = LBound(keyfield) To UBound(keyfield)
        If UCase(datatypes(i)) = "NUMBER" Then
            strInsert = strInsert & " AND " & keyfield(i) & "=" & keydata(i)
        ElseIf UCase(datatypes(i)) = "DATE" Then
            If keydata(i) <> "NULL" Then
                strInsert = strInsert & " AND " & keyfield(i) & "=to_date('" & Format(keydata(i), "dd-mm-yyyy") & "','dd-mm-yyyy')"
            Else
                strInsert = strInsert & " AND " & keyfield(i) & " is " & keydata(i)
            End If
        ElseIf UCase(datatypes(i)) = "STRING" Then
            strInsert = strInsert & " AND " & keyfield(i) & "='" & keydata(i) & "'"
        End If
    Next
    MyConn.Execute strInsert
End Sub
''Its count No of lines of txt file
Public Function LinesInFile(ByVal file_name As String) As _
    Long
Dim fnum As Integer
Dim Lines As Long
Dim one_line As String
    fnum = FreeFile
    Open file_name For Input As #fnum
    Do While Not EOF(fnum)
        Line Input #fnum, one_line
        Lines = Lines + 1
    Loop
    Close fnum
    LinesInFile = Lines
End Function

Public Sub FlexGrid_To_Excel(TheFlexgrid As MSFlexGrid, _
  TheRows As Integer, TheCols As Integer, _
  Optional GridStyle As Integer = 1, Optional WorkSheetName _
  As String)
Dim objXL As New Excel.Application
Dim wbXL As New Excel.Workbook
Dim wsXL As New Excel.Worksheet
Dim intRow As Integer ' counter
Dim intCol As Integer ' counter
If Not IsObject(objXL) Then
    MsgBox "You need Microsoft Excel to use this function", _
       vbExclamation, "Print to Excel"
    Exit Sub
End If
On Error Resume Next
objXL.Visible = True
Set wbXL = objXL.Workbooks.Add
Set wsXL = objXL.ActiveSheet
With wsXL
    If Not WorkSheetName = "" Then
        .Name = WorkSheetName
    End If
End With
' fill worksheet
For intRow = 1 To TheRows
    For intCol = 1 To TheCols
        With TheFlexgrid
            wsXL.Cells(intRow, intCol).Value = _
               .TextMatrix(intRow - 1, intCol - 1) & " "
        End With
    Next
Next
' format the look
For intCol = 1 To TheCols
    wsXL.Columns(intCol).AutoFit
    wsXL.Range("a1", Right(wsXL.Columns(TheCols).AddressLocal, _
         1) & TheRows).AutoFormat GridStyle
Next
End Sub




Public Function CheckDate(fromDate As String, CurrentDate As String) As Boolean
Dim strFrom() As String
Dim strTo() As String
    ReDim strFrom(2)
    ReDim strTo(2)
    'if fromdate>Currentdate then return false else true
    strFrom = Split(fromDate, "/")
    strTo = Split(CurrentDate, "/")
    If strFrom(2) > strTo(2) Then
        CheckDate = False
    ElseIf strFrom(2) = strTo(2) Then
        If strFrom(1) > strTo(1) Then
            CheckDate = False
        ElseIf strFrom(1) = strTo(1) Then
            If strFrom(0) > strTo(0) Then
                CheckDate = False
            Else
                CheckDate = True
            End If
        Else
            CheckDate = True
        End If
    Else
        CheckDate = True
    End If
End Function
Public Function Check_Date(ByVal DateValue As String) As Boolean
        On Error GoTo err
        Dim arr() As String
        Dim Day As Integer, Month11 As Integer, Year1 As Integer
        Dim RetMonth1Days(12) As Integer
         'For i = 0 To UBound(recd_brok_type)
         
        arr = Split(DateValue, "/")
        Day = Val(arr(0)): Month1 = Val(arr(1)): Year1 = Val(arr(2))
        RetMonth1Days(0) = 31
        
        If year(DateValue) Mod 4 = 0 Then
            RetMonth1Days(1) = 29  '.IsLeapYear1(Year1) Then : RetMonth1Days(1) = 29
        Else
            RetMonth1Days(1) = 28
        End If
        RetMonth1Days(2) = 31: RetMonth1Days(3) = 30: RetMonth1Days(4) = 31: RetMonth1Days(5) = 30: RetMonth1Days(6) = 31: RetMonth1Days(7) = 31
        RetMonth1Days(8) = 30: RetMonth1Days(9) = 31: RetMonth1Days(10) = 30: RetMonth1Days(11) = 31
        If Month1 > 0 And Month1 < 13 And Year1 > 1900 And Year1 < 2051 And Day > 0 And Day <= RetMonth1Days(Month1 - 1) Then
            'Response.Write(Day & "/" & Month1 & "/" & Year1)
            Check_Date = True
            Exit Function
        End If
err:
    Check_Date = False 'Response.Writ
End Function
Public Function ConvertString(FindText As String) As String
Dim i As Integer
Dim strConverted As String
    strConverted = FindText
    For i = 1 To Len(FindText)
        If Mid(FindText, i, 1) = " " Then
            Mid(strConverted, i, 1) = "%"
        End If
    Next
    ConvertString = strConverted
End Function
Public Function ValidateEmail(ByVal strEmail As String) As Boolean
Dim strTmp As String, n As Long, sEXT As String
emsg = "" 'reset on open for good form
ValidateEmail = True 'Assume true on init
sEXT = strEmail
Do While InStr(1, sEXT, ".") <> 0
   sEXT = Right(sEXT, Len(sEXT) - InStr(1, sEXT, "."))
Loop
If InStr(1, strEmail, ".") = 0 Then
   ValidateEmail = False
   emsg = emsg & "Your email address does not contain . sign."
ElseIf InStr(1, strEmail, ".") = 1 Then
   ValidateEmail = False
   emsg = emsg & "Your . sign can not be the first character in your email address!"
ElseIf InStr(1, strEmail, ".") = Len(strEmail) Then
   ValidateEmail = False
   emsg = emsg & "Your  . sign can not be the last character in your email address!"
End If
If strEmail = "" Then
   ValidateEmail = False
   emsg = emsg & "You did not enter an email address!"
ElseIf InStr(1, strEmail, "@") = 0 Then
   ValidateEmail = False
   emsg = emsg & "Your email address does not contain an @ sign."
ElseIf InStr(1, strEmail, "@") = 1 Then
   ValidateEmail = False
   emsg = emsg & "Your @ sign can not be the first character in your email address!"
ElseIf InStr(1, strEmail, "@") = Len(strEmail) Then
   ValidateEmail = False
   emsg = emsg & "Your @sign can not be the last character in your email address!"
ElseIf Len(strEmail) < 6 Then
   ValidateEmail = False
   emsg = emsg & "Your email address is shorter than 6 characters which is impossible."
'add by pankaj pundir on dated 21032016 anup ji
ElseIf InStr(1, UCase(strEmail), "GMAIL") >= 1 Then
   If Len(strEmail) < 16 Then
        ValidateEmail = False
        emsg = emsg & "Your Gmail address should be between 6 and 30 characters.."
   End If
ElseIf InStr(1, UCase(strEmail), "YAHOO.IN") >= 1 Then
   If Len(strEmail) < 13 Then
        ValidateEmail = False
        emsg = emsg & "Your Yahoo.in username must be at least 4 characters long."
   End If
ElseIf InStr(1, UCase(strEmail), "YAHOO.COM") >= 1 Then
   If Len(strEmail) < 14 Then
        ValidateEmail = False
        emsg = emsg & "Your Yahoo.com username must be at least 4 characters long."
   End If
End If
strTmp = strEmail
Do While InStr(1, strTmp, "@") <> 0
   n = 1
   strTmp = Right(strTmp, Len(strTmp) - InStr(1, strTmp, "@"))
Loop
If n > 1 Then
   ValidateEmail = False 'found more than one @ sign
   emsg = emsg & "You have more than 1 @ sign in your email address"
End If
End Function

Public Function ValidateUploadedClient(ByVal PSys_ar_no As String, ByVal PClient_cd As String) As Boolean
ValidateUploadedClient = True
Dim cmd As New ADODB.Command
Dim vResult As String
Dim vErrMsg As String

Set cmd.ActiveConnection = MyConn
cmd.CommandType = adCmdStoredProc
cmd.CommandText = "WEALTHMAKER.FN_VALIDATE_CLIENT"
cmd.Parameters.Append cmd.CreateParameter("PSYS_AR_NO", adVarChar, adParamInput, 15, PSys_ar_no)
cmd.Parameters.Append cmd.CreateParameter("PCLIENT_CODE", adVarChar, adParamInput, 15, PClient_cd)
cmd.Parameters.Append cmd.CreateParameter("RESULT", adDouble, adParamOutput)
cmd.Parameters.Append cmd.CreateParameter("MESSAGE", adVarChar, adParamOutput, 200)
cmd.Execute
vResult = IIf(IsNull(cmd("RESULT")), "", cmd("RESULT"))
vErrMsg = IIf(IsNull(cmd("MESSAGE")), "", cmd("MESSAGE"))
Set cmd = Nothing
If vResult = 0 Then
    ValidateUploadedClient = False
    MsgBox vErrMsg, vbInformation
End If
End Function

Public Function ValidateUploadedClientDT(ByVal PPDT_NO As String, ByVal PClient_cd As String) As Boolean
ValidateUploadedClientDT = True
Dim cmd As New ADODB.Command
Dim vResult As String
Dim vErrMsg As String

Set cmd.ActiveConnection = MyConn
cmd.CommandType = adCmdStoredProc
cmd.CommandText = "WEALTHMAKER.FN_VALIDATE_CLIENT_DT"
cmd.Parameters.Append cmd.CreateParameter("PDT_NO", adVarChar, adParamInput, 15, PPDT_NO)
cmd.Parameters.Append cmd.CreateParameter("PCLIENT_CODE", adVarChar, adParamInput, 15, PClient_cd)
cmd.Parameters.Append cmd.CreateParameter("RESULT", adDouble, adParamOutput)
cmd.Parameters.Append cmd.CreateParameter("MESSAGE", adVarChar, adParamOutput, 200)
cmd.Execute
vResult = IIf(IsNull(cmd("RESULT")), "", cmd("RESULT"))
vErrMsg = IIf(IsNull(cmd("MESSAGE")), "", cmd("MESSAGE"))
Set cmd = Nothing
If vResult = 0 Then
    ValidateUploadedClientDT = False
    MsgBox vErrMsg, vbInformation
End If
End Function

'-----------------------------------------------validate uploaded branch---------------------------------------------------
Public Function ValidateUploadedBranch(ByVal PSys_ar_no As String, ByVal PBranch_cd As String, ByVal PPayroll_id As String) As Boolean
ValidateUploadedBranch = True
Dim cmd As New ADODB.Command
Dim vResult As String
Dim vErrMsg As String

Set cmd.ActiveConnection = MyConn
cmd.CommandType = adCmdStoredProc
cmd.CommandText = "WEALTHMAKER.FN_VALIDATE_BRANCH_RM"
cmd.Parameters.Append cmd.CreateParameter("PSYS_AR_NO", adVarChar, adParamInput, 15, PSys_ar_no)
cmd.Parameters.Append cmd.CreateParameter("PBRANCH_CODE", adVarChar, adParamInput, 15, PBranch_cd)
cmd.Parameters.Append cmd.CreateParameter("PPAYROLL_ID", adVarChar, adParamInput, 15, PPayroll_id)
cmd.Parameters.Append cmd.CreateParameter("RESULT", adDouble, adParamOutput)
cmd.Parameters.Append cmd.CreateParameter("MESSAGE", adVarChar, adParamOutput, 200)
cmd.Execute
vResult = IIf(IsNull(cmd("RESULT")), "", cmd("RESULT"))
vErrMsg = IIf(IsNull(cmd("MESSAGE")), "", cmd("MESSAGE"))
Set cmd = Nothing
If vResult = 0 Then
    ValidateUploadedBranch = False
    MsgBox vErrMsg, vbInformation
End If
End Function

Public Function ValidateUploadedBranchDT(ByVal PPDT_NO As String, ByVal PBranch_cd As String, ByVal PPayroll_id As String) As Boolean
ValidateUploadedBranchDT = True
Dim cmd As New ADODB.Command
Dim vResult As String
Dim vErrMsg As String

Set cmd.ActiveConnection = MyConn
cmd.CommandType = adCmdStoredProc
cmd.CommandText = "WEALTHMAKER.FN_VALIDATE_BRANCH_RM_DT"
cmd.Parameters.Append cmd.CreateParameter("PDT_NO", adVarChar, adParamInput, 15, PPDT_NO)
cmd.Parameters.Append cmd.CreateParameter("PBRANCH_CODE", adVarChar, adParamInput, 15, PBranch_cd)
cmd.Parameters.Append cmd.CreateParameter("PPAYROLL_ID", adVarChar, adParamInput, 15, PPayroll_id)
cmd.Parameters.Append cmd.CreateParameter("RESULT", adDouble, adParamOutput)
cmd.Parameters.Append cmd.CreateParameter("MESSAGE", adVarChar, adParamOutput, 200)
cmd.Execute
vResult = IIf(IsNull(cmd("RESULT")), "", cmd("RESULT"))
vErrMsg = IIf(IsNull(cmd("MESSAGE")), "", cmd("MESSAGE"))
Set cmd = Nothing
If vResult = 0 Then
    ValidateUploadedBranchDT = False
    MsgBox vErrMsg, vbInformation
End If
End Function

'------------------------------------------------------------------------------------------------------------------------


Public Function webaddvaliadete(ByVal strwadd As String) As Boolean
Dim webaddress As String
Dim strTmp As String
wMsg = ""
webaddvaliadete = True 'Assume true on init
webaddress = strwadd
If Len(webaddress) < 8 Then
 wMsg = wMsg & "Your website address is shorter than 8 characters which is impossible."
webaddvaliadete = False
ElseIf InStr(1, webaddress, ".") = 0 Then
   webaddvaliadete = False
   wMsg = wMsg & "Your website address does not contain an . sign."
ElseIf InStr(1, webaddress, ".") = 1 Then
   webaddvaliadete = False
   wMsg = wMsg & "Your . sign can not be the first character in your website address!"
ElseIf InStr(1, webaddress, ".") = Len(strwadd) Then
   webaddvaliadete = False
   wMsg = wMsg & "Your .sign can not be the last character in your website address!"
End If
strTmp = strwadd
n = 0
Do While InStr(1, strTmp, ".") <> 0
   n = n + 1
   strTmp = Right(strTmp, Len(strTmp) - InStr(1, strTmp, "."))
Loop
If n < 2 Then
   webaddvaliadete = False 'found less than two . sign
   wMsg = wMsg & "You have less than two . sign in your website address"
End If
End Function
Public Function DATELIMIT(date1 As String) As Boolean
If Right(date1, 4) < 1900 Then
MsgBox "Year of date Can not be less then 1900", vbInformation
DATELIMIT = False
Else
DATELIMIT = True
End If
End Function

Public Sub Deal_Before_Update(SL_ID As String)
On Error Resume Next
    BEFORE_PAID_CAL_ON = ""
    BEFORE_PER_RS = ""
    BEFORE_FIGURE = ""
    BEFORE_DUE_AFTER = ""
    BEFORE_VALID_TILL = ""
    BEFORE_CAL_ON = ""
    BEFORE_USER = ""
    BEFORE_DATE = ""
    
    rsBeforeSlab.open "select PAID_CAL_ON,PER_RS,Figure,DUE_AFTER,VALID_TILL,CAL_ON,loggeduSerid,timest from dealspecific_paid where slab_id='" & SL_ID & "'", MyConn, adOpenForwardOnly
    If Not rsBeforeSlab.EOF Then
    
        BEFORE_PAID_CAL_ON = rsBeforeSlab(0)
        BEFORE_PER_RS = rsBeforeSlab(1)
        BEFORE_FIGURE = rsBeforeSlab(2)
        BEFORE_DUE_AFTER = rsBeforeSlab(3)
        BEFORE_VALID_TILL = rsBeforeSlab(4)
        BEFORE_CAL_ON = rsBeforeSlab(5)
        BEFORE_LAST_USER = rsBeforeSlab(6)
        BEFORE_LAST_DATE = rsBeforeSlab(7)

    End If
    rsBeforeSlab.Close
    Set rsBeforeSlab = Nothing
End Sub

Public Sub Deal_After_Update(SL_ID As String, tr_code As String)
On Error Resume Next
    AFTER_PAID_CAL_ON = ""
    AFTER_PER_RS = ""
    AFTER_FIGURE = ""
    AFTER_DUE_AFTER = ""
    AFTER_VALID_TILL = ""
    AFTER_CAL_ON = ""
    
    rsAfterSlab.open "select PAID_CAL_ON,PER_RS,Figure,DUE_AFTER,VALID_TILL,CAL_ON from dealspecific_paid where slab_id='" & SL_ID & "'", MyConn, adOpenForwardOnly
    If Not rsAfterSlab.EOF Then
    
        AFTER_PAID_CAL_ON = rsAfterSlab(0)
        AFTER_PER_RS = rsAfterSlab(1)
        AFTER_FIGURE = rsAfterSlab(2)
        AFTER_DUE_AFTER = rsAfterSlab(3)
        AFTER_VALID_TILL = rsAfterSlab(4)
        AFTER_CAL_ON = rsAfterSlab(5)

    End If
    rsAfterSlab.Close
    Set rsAfterSlab = Nothing
    
    If BEFORE_PAID_CAL_ON <> AFTER_PAID_CAL_ON Then
        MyConn.Execute "insert into Dealspecific_UPD_HIST(slab_id,tran_code,changed_field,PREV_VALUE,changed_value,Last_updated_user,LAST_UPDATE_DATE) values ('" & SL_ID & "','" & tr_code & "','PAID_CAL_ON','" & BEFORE_PAID_CAL_ON & "','" & AFTER_PAID_CAL_ON & "','" & BEFORE_LAST_USER & "',TO_DATE('" & Format(BEFORE_LAST_DATE, "dd/mm/yyyy") & "','dd/mm/yyyy'))"
    End If
    
    If BEFORE_PER_RS <> AFTER_PER_RS Then
        MyConn.Execute "insert into Dealspecific_UPD_HIST(slab_id,tran_code,changed_field,PREV_VALUE,changed_value,Last_updated_user,LAST_UPDATE_DATE) values ('" & SL_ID & "','" & tr_code & "','PER_RS','" & BEFORE_PER_RS & "','" & AFTER_PER_RS & "','" & BEFORE_LAST_USER & "',TO_DATE('" & Format(BEFORE_LAST_DATE, "dd/mm/yyyy") & "','dd/mm/yyyy'))"
    End If
    
    If BEFORE_FIGURE <> AFTER_FIGURE Then
        MyConn.Execute "insert into Dealspecific_UPD_HIST(slab_id,tran_code,changed_field,PREV_VALUE,changed_value,Last_updated_user,LAST_UPDATE_DATE) values ('" & SL_ID & "','" & tr_code & "','Figure','" & BEFORE_FIGURE & "','" & AFTER_FIGURE & "','" & BEFORE_LAST_USER & "',TO_DATE('" & Format(BEFORE_LAST_DATE, "dd/mm/yyyy") & "','dd/mm/yyyy'))"
    End If
    
    If BEFORE_DUE_AFTER <> AFTER_DUE_AFTER Then
        MyConn.Execute "insert into Dealspecific_UPD_HIST(slab_id,tran_code,changed_field,PREV_VALUE,changed_value,Last_updated_user,LAST_UPDATE_DATE) values ('" & SL_ID & "','" & tr_code & "','DUE_AFTER','" & BEFORE_DUE_AFTER & "','" & AFTER_DUE_AFTER & "','" & BEFORE_LAST_USER & "',TO_DATE('" & Format(BEFORE_LAST_DATE, "dd/mm/yyyy") & "','dd/mm/yyyy'))"
    End If
    
    If BEFORE_VALID_TILL <> AFTER_VALID_TILL Then
        MyConn.Execute "insert into Dealspecific_UPD_HIST(slab_id,tran_code,changed_field,PREV_VALUE,changed_value,Last_updated_user,LAST_UPDATE_DATE) values ('" & SL_ID & "','" & tr_code & "','VALID_TILL','" & BEFORE_VALID_TILL & "','" & AFTER_VALID_TILL & "','" & BEFORE_LAST_USER & "',TO_DATE('" & Format(BEFORE_LAST_DATE, "dd/mm/yyyy") & "','dd/mm/yyyy'))"
    End If
    
    If BEFORE_CAL_ON <> AFTER_CAL_ON Then
        MyConn.Execute "insert into Dealspecific_UPD_HIST(slab_id,tran_code,changed_field,PREV_VALUE,changed_value,Last_updated_user,LAST_UPDATE_DATE) values ('" & SL_ID & "','" & tr_code & "','CAL_ON','" & BEFORE_CAL_ON & "','" & AFTER_CAL_ON & "','" & BEFORE_LAST_USER & "',TO_DATE('" & Format(BEFORE_LAST_DATE, "dd/mm/yyyy") & "','dd/mm/yyyy'))"
    End If

End Sub

Public Function CheckDate_Equal(fromDate As String, CurrentDate As String) As Boolean
Dim strFrom() As String
Dim strTo() As String
    ReDim strFrom(2)
    ReDim strTo(2)
    strFrom = Split(fromDate, "/")
    strTo = Split(CurrentDate, "/")
    If strFrom(2) > strTo(2) Then
        CheckDate_Equal = False
    ElseIf strFrom(2) = strTo(2) Then
        If strFrom(1) > strTo(1) Then
            CheckDate_Equal = False
        ElseIf strFrom(1) = strTo(1) Then
            If strFrom(0) >= strTo(0) Then
                CheckDate_Equal = False
            Else
                CheckDate_Equal = True
            End If
        Else
            CheckDate_Equal = True
        End If
    Else
        CheckDate_Equal = True
    End If
End Function

Public Sub NEWtreeBranchFill_Tran(treeFill As Control, refcode As String, Optional AllBr As Boolean = False)
Dim rs_GET_REGION As ADODB.Recordset
Dim rs_get_zone As ADODB.Recordset
Dim rs_get_City As ADODB.Recordset
Dim rs_get_Loc As ADODB.Recordset
Dim rs_get_branch, rs_get_rm  As ADODB.Recordset
Dim brcnt As Integer
Dim BrCode, brname, RMcode, rmname As String
Dim SqlQry As String
If AllBr = True Then
    treeFill.Nodes.Clear
    Set node1 = treeFill.Nodes.Add(, tvwNext, "MFIBR", "Branch", "cl")
    node1.ExpandedImage = "op"
    brcnt = 0
    rmcnt = 0
    Set rs_get_City = New ADODB.Recordset
    Set rs_get_City = MyConn.Execute("select city_id from city_master")
    If Not rs_get_City.EOF Then
        Set rs_get_Loc = New ADODB.Recordset
        rs_get_City.MoveFirst
        Do While Not rs_get_City.EOF
            Set rs_get_Loc = MyConn.Execute("select location_id from location_master where city_id='" & rs_get_City(0) & "'")
            If Not rs_get_Loc.EOF Then  ''location start
                Set rs_get_Bran = New ADODB.Recordset
                rs_get_Loc.MoveFirst
                Do While Not rs_get_Loc.EOF ''loop location
                    Set rs_get_branch = MyConn.Execute("select branch_code,branch_name from branch_master where branch_code not in(10071019) and location_id='" & rs_get_Loc(0) & "' AND CATEGORY_ID<>1004 AND CATEGORY_ID<>1005 AND CATEGORY_ID<>1006 ORDER BY BRANCH_NAME")
                    If Not rs_get_branch.EOF Then ''start branch
                        Do While Not rs_get_branch.EOF ''loop branch
                            brcnt = brcnt + 1
                            BrCode = "B" & Trim(rs_get_branch(0))
                            brname = Trim(rs_get_branch(1))
                            Set node01 = treeFill.Nodes.Add("MFIBR", tvwChild, BrCode, brname, "cl")
                            Set node02 = treeFill.Nodes.Add(BrCode, tvwChild, "MFIRM" & brcnt, "RM", "cl")
                            Set rs_get_rm = MyConn.Execute("Select  rm_code,rm_name,source from employee_master where source=" & Mid(BrCode, 2) & " AND (TYPE='A' OR TYPE IS NULL) order by rm_name")
                            While Not rs_get_rm.EOF
                                rmcnt = rmcnt + 1
                                RMcode = "R" & Trim(rs_get_rm(0))
                                rmname = Trim(rs_get_rm(1))
                                Set node1 = treeFill.Nodes.Add("MFIRM" & brcnt, tvwChild, RMcode, rmname, "cl")
                                Set node2 = treeFill.Nodes.Add(RMcode, tvwChild, "MFICL" & rmcnt, "Client", "cl")
                                node2.ExpandedImage = "op"
                                Set node3 = treeFill.Nodes.Add(RMcode, tvwChild, "MFIAG" & rmcnt, "Agent", "cl")
                                node1.ExpandedImage = "op"
                                node3.ExpandedImage = "op"
                                rs_get_rm.MoveNext
                            Wend
                            node01.ExpandedImage = "op"
                            node02.ExpandedImage = "op"
                            rs_get_branch.MoveNext
                        Loop ''end loop branch
                    End If ''end branch
                    rs_get_Loc.MoveNext
                Loop ''end loop location
            End If ''end location
            rs_get_City.MoveNext
        Loop
    End If
    'COMMENT BY PANKAJ REVERT CHANGE TO FILL TREE ON SHOW BUTTON
'    Set rs_get_branch = MyConn.Execute("select branch_code,branch_name from branch_master where branch_code in (" & MyAllBranches & ")   AND CATEGORY_ID<>1004 AND CATEGORY_ID<>1005 AND CATEGORY_ID<>1006 ORDER BY BRANCH_NAME")
'    If Not rs_get_branch.EOF Then ''start branch
'        Do While Not rs_get_branch.EOF ''loop branch
'            brcnt = brcnt + 1
'            BrCode = "B" & Trim(rs_get_branch(0))
'            brname = Trim(rs_get_branch(1))
'            Set node01 = treeFill.Nodes.Add("MFIBR", tvwChild, BrCode, brname, "cl")
'            Set node02 = treeFill.Nodes.Add(BrCode, tvwChild, "MFIRM" & brcnt, "RM", "cl")
'            Set rs_get_rm = MyConn.Execute("Select  rm_code,rm_name,source from employee_master where source=" & Mid(BrCode, 2) & " AND (TYPE='A' OR TYPE IS NULL) order by rm_name")
'            While Not rs_get_rm.EOF
'                rmcnt = rmcnt + 1
'                RMcode = "R" & Trim(rs_get_rm(0))
'                rmname = Trim(rs_get_rm(1))
'                Set node1 = treeFill.Nodes.Add("MFIRM" & brcnt, tvwChild, RMcode, rmname, "cl")
'                Set node2 = treeFill.Nodes.Add(RMcode, tvwChild, "MFICL" & rmcnt, "Client", "cl")
'                node2.ExpandedImage = "op"
'                Set node3 = treeFill.Nodes.Add(RMcode, tvwChild, "MFIAG" & rmcnt, "Agent", "cl")
'                node1.ExpandedImage = "op"
'                node3.ExpandedImage = "op"
'                rs_get_rm.MoveNext
'            Wend
'            node01.ExpandedImage = "op"
'            node02.ExpandedImage = "op"
'            rs_get_branch.MoveNext
'        Loop ''end loop branch
'    End If
Else
    refcode = Replace(refcode, "#", "','")
    If Left(refcode, 1) = "R" Then
        treeFill.Nodes.Clear
        Set node1 = treeFill.Nodes.Add(, tvwNext, "MFIBR", "Branch", "cl")
        node1.ExpandedImage = "op"
        brcnt = 0
        rmcnt = 0
        Set rs_get_Bran = New ADODB.Recordset
            Set rs_get_branch = MyConn.Execute("select branch_code,branch_name from branch_master where REGION_ID in('" & refcode & "') AND CATEGORY_ID<>1004 AND CATEGORY_ID<>1005 AND CATEGORY_ID<>1006 ORDER BY BRANCH_NAME")
            If Not rs_get_branch.EOF Then ''start branch
                Do While Not rs_get_branch.EOF ''loop branch
                    brcnt = brcnt + 1
                    BrCode = "B" & Trim(rs_get_branch(0))
                    brname = Trim(rs_get_branch(1))
                    Set node01 = treeFill.Nodes.Add("MFIBR", tvwChild, BrCode, brname, "cl")
                    Set node02 = treeFill.Nodes.Add(BrCode, tvwChild, "MFIRM" & brcnt, "RM", "cl")
                    Set rs_get_rm = MyConn.Execute("Select  rm_code,rm_name,source from employee_master where source=" & Mid(BrCode, 2) & " AND (TYPE='A' OR TYPE IS NULL) order by rm_name")
                    While Not rs_get_rm.EOF
                        rmcnt = rmcnt + 1
                        RMcode = "R" & Trim(rs_get_rm(0))
                        rmname = Trim(rs_get_rm(1))
                        Set node1 = treeFill.Nodes.Add("MFIRM" & brcnt, tvwChild, RMcode, rmname, "cl")
                        Set node2 = treeFill.Nodes.Add(RMcode, tvwChild, "MFICL" & rmcnt, "Client", "cl")
                        node2.ExpandedImage = "op"
                        Set node3 = treeFill.Nodes.Add(RMcode, tvwChild, "MFIAG" & rmcnt, "Agent", "cl")
                        node1.ExpandedImage = "op"
                        node3.ExpandedImage = "op"
                        rs_get_rm.MoveNext
                    Wend
                    node01.ExpandedImage = "op"
                    node02.ExpandedImage = "op"
                    rs_get_branch.MoveNext
                Loop ''end loop branch
            End If ''end branch
        ElseIf Left(refcode, 1) = "Z" Then
            treeFill.Nodes.Clear
            Set node1 = treeFill.Nodes.Add(, tvwNext, "MFIBR", "Branch", "cl")
            node1.ExpandedImage = "op"
            brcnt = 0
            rmcnt = 0
            Set rs_get_branch = MyConn.Execute("select branch_code,branch_name from branch_master where ZONE_ID in('" & refcode & "') AND CATEGORY_ID<>1004 AND CATEGORY_ID<>1005 AND CATEGORY_ID<>1006 ORDER BY BRANCH_NAME")
            If Not rs_get_branch.EOF Then ''start branch
                Do While Not rs_get_branch.EOF ''loop branch
                    brcnt = brcnt + 1
                    BrCode = "B" & Trim(rs_get_branch(0))
                    brname = Trim(rs_get_branch(1))
                    Set node01 = treeFill.Nodes.Add("MFIBR", tvwChild, BrCode, brname, "cl")
                    Set node02 = treeFill.Nodes.Add(BrCode, tvwChild, "MFIRM" & brcnt, "RM", "cl")
                    Set rs_get_rm = MyConn.Execute("Select  rm_code,rm_name,source from employee_master where source=" & Mid(BrCode, 2) & " AND (TYPE='A' OR TYPE IS NULL) order by rm_name")
                    While Not rs_get_rm.EOF
                        rmcnt = rmcnt + 1
                        RMcode = "R" & Trim(rs_get_rm(0))
                        rmname = Trim(rs_get_rm(1))
                        Set node1 = treeFill.Nodes.Add("MFIRM" & brcnt, tvwChild, RMcode, rmname, "cl")
                        Set node2 = treeFill.Nodes.Add(RMcode, tvwChild, "MFICL" & rmcnt, "Client", "cl")
                        node2.ExpandedImage = "op"
                        Set node3 = treeFill.Nodes.Add(RMcode, tvwChild, "MFIAG" & rmcnt, "Agent", "cl")
                        node1.ExpandedImage = "op"
                        node3.ExpandedImage = "op"
                        rs_get_rm.MoveNext
                    Wend
                    node01.ExpandedImage = "op"
                    node02.ExpandedImage = "op"
                    rs_get_branch.MoveNext
                Loop ''end loop branch
            End If ''end branch
        ElseIf Left(refcode, 1) = "C" Then
            treeFill.Nodes.Clear
            Set node1 = treeFill.Nodes.Add(, tvwNext, "MFIBR", "Branch", "cl")
            node1.ExpandedImage = "op"
            brcnt = 0
            rmcnt = 0
            Set rs_get_Loc = New ADODB.Recordset
            Set rs_get_Loc = MyConn.Execute("select location_id from location_master where city_id in('" & refcode & "')")
            If Not rs_get_Loc.EOF Then  ''location start
                Set rs_get_Bran = New ADODB.Recordset
                rs_get_Loc.MoveFirst
                Do While Not rs_get_Loc.EOF ''loop location
                    Set rs_get_branch = MyConn.Execute("select branch_code,branch_name from branch_master where location_id='" & rs_get_Loc(0) & "' AND CATEGORY_ID<>1004 AND CATEGORY_ID<>1005 AND CATEGORY_ID<>1006 ORDER BY BRANCH_NAME")
                    If Not rs_get_branch.EOF Then ''start branch
                        Do While Not rs_get_branch.EOF ''loop branch
                            brcnt = brcnt + 1
                            BrCode = "B" & Trim(rs_get_branch(0))
                            brname = Trim(rs_get_branch(1))
                            Set node01 = treeFill.Nodes.Add("MFIBR", tvwChild, BrCode, brname, "cl")
                            Set node02 = treeFill.Nodes.Add(BrCode, tvwChild, "MFIRM" & brcnt, "RM", "cl")
                            Set rs_get_rm = MyConn.Execute("Select  rm_code,rm_name,source from employee_master where source=" & Mid(BrCode, 2) & " AND (TYPE='A' OR TYPE IS NULL) order by rm_name")
                            While Not rs_get_rm.EOF
                                rmcnt = rmcnt + 1
                                RMcode = "R" & Trim(rs_get_rm(0))
                                rmname = Trim(rs_get_rm(1))
                                Set node1 = treeFill.Nodes.Add("MFIRM" & brcnt, tvwChild, RMcode, rmname, "cl")
                                Set node2 = treeFill.Nodes.Add(RMcode, tvwChild, "MFICL" & rmcnt, "Client", "cl")
                                node2.ExpandedImage = "op"
                                Set node3 = treeFill.Nodes.Add(RMcode, tvwChild, "MFIAG" & rmcnt, "Agent", "cl")
                                node1.ExpandedImage = "op"
                                node3.ExpandedImage = "op"
                                rs_get_rm.MoveNext
                            Wend
                            node01.ExpandedImage = "op"
                            node02.ExpandedImage = "op"
                            rs_get_branch.MoveNext
                        Loop ''end loop branch
                    End If ''end branch
                    rs_get_Loc.MoveNext
                Loop ''end loop location
            End If ''end location
        ElseIf Left(refcode, 1) = "L" Then
            treeFill.Nodes.Clear
            Set node1 = treeFill.Nodes.Add(, tvwNext, "MFIBR", "Branch", "cl")
            node1.ExpandedImage = "op"
            brcnt = 0
            rmcnt = 0
            Set rs_get_branch = MyConn.Execute("select branch_code,branch_name from branch_master where location_id in('" & refcode & "') AND CATEGORY_ID<>1004 AND CATEGORY_ID<>1005 AND CATEGORY_ID<>1006 ORDER BY BRANCH_NAME")
            If Not rs_get_branch.EOF Then ''start branch
                Do While Not rs_get_branch.EOF ''loop branch
                    brcnt = brcnt + 1
                    BrCode = "B" & Trim(rs_get_branch(0))
                    brname = Trim(rs_get_branch(1))
                    Set node01 = treeFill.Nodes.Add("MFIBR", tvwChild, BrCode, brname, "cl")
                    Set node02 = treeFill.Nodes.Add(BrCode, tvwChild, "MFIRM" & brcnt, "RM", "cl")
                    Set rs_get_rm = MyConn.Execute("Select  rm_code,rm_name,source from employee_master where source=" & Mid(BrCode, 2) & " AND (TYPE='A' OR TYPE IS NULL) order by rm_name")
                    While Not rs_get_rm.EOF
                        rmcnt = rmcnt + 1
                        RMcode = "R" & Trim(rs_get_rm(0))
                        rmname = Trim(rs_get_rm(1))
                        Set node1 = treeFill.Nodes.Add("MFIRM" & brcnt, tvwChild, RMcode, rmname, "cl")
                        Set node2 = treeFill.Nodes.Add(RMcode, tvwChild, "MFICL" & rmcnt, "Client", "cl")
                        node2.ExpandedImage = "op"
                        Set node3 = treeFill.Nodes.Add(RMcode, tvwChild, "MFIAG" & rmcnt, "Agent", "cl")
                        node1.ExpandedImage = "op"
                        node3.ExpandedImage = "op"
                        rs_get_rm.MoveNext
                    Wend
                    node01.ExpandedImage = "op"
                    node02.ExpandedImage = "op"
                    rs_get_branch.MoveNext
                Loop ''end loop branch
            End If ''end branch
        ElseIf Left(refcode, 1) = "1" Then
            treeFill.Nodes.Clear
            Set node1 = treeFill.Nodes.Add(, tvwNext, "MFIBR", "Branch", "cl")
            node1.ExpandedImage = "op"
            brcnt = 0
            rmcnt = 0
            Set rs_get_branch = MyConn.Execute("select branch_code,branch_name from branch_master where branch_code in('" & refcode & "') AND CATEGORY_ID<>1004 AND CATEGORY_ID<>1005 AND CATEGORY_ID<>1006 ORDER BY BRANCH_NAME")
            If Not rs_get_branch.EOF Then ''start branch
                Do While Not rs_get_branch.EOF ''loop branch
                    brcnt = brcnt + 1
                    BrCode = "B" & Trim(rs_get_branch(0))
                    brname = Trim(rs_get_branch(1))
                    Set node01 = treeFill.Nodes.Add("MFIBR", tvwChild, BrCode, brname, "cl")
                    Set node02 = treeFill.Nodes.Add(BrCode, tvwChild, "MFIRM" & brcnt, "RM", "cl")
                    Set rs_get_rm = MyConn.Execute("Select  rm_code,rm_name,source from employee_master where source=" & Mid(BrCode, 2) & " AND (TYPE='A' OR TYPE IS NULL) order by rm_name")
                    While Not rs_get_rm.EOF
                        rmcnt = rmcnt + 1
                        RMcode = "R" & Trim(rs_get_rm(0))
                        rmname = Trim(rs_get_rm(1))
                        Set node1 = treeFill.Nodes.Add("MFIRM" & brcnt, tvwChild, RMcode, rmname, "cl")
                        Set node2 = treeFill.Nodes.Add(RMcode, tvwChild, "MFICL" & rmcnt, "Client", "cl")
                        node2.ExpandedImage = "op"
                        Set node3 = treeFill.Nodes.Add(RMcode, tvwChild, "MFIAG" & rmcnt, "Agent", "cl")
                        node1.ExpandedImage = "op"
                        node3.ExpandedImage = "op"
                        rs_get_rm.MoveNext
                    Wend
                    node01.ExpandedImage = "op"
                    node02.ExpandedImage = "op"
                    rs_get_branch.MoveNext
                Loop ''end loop branch
            End If ''end branch
        ElseIf Left(refcode, 1) = "@" Then
            treeFill.Nodes.Clear
            Set node1 = treeFill.Nodes.Add(, tvwNext, "MFIBR", "Branch", "cl")
            node1.ExpandedImage = "op"
            Set rs_get_branch = MyConn.Execute("select source from employee_master where rm_code in(" & refcode & ")")
            If Not rs_get_branch.EOF Then
                Dim rs_branch As New ADODB.Recordset
                Set rs_branch = MyConn.Execute("select branch_name from branch_master where branch_code=" & rs_get_branch(0))
                If Not rs_branch.EOF Then
                    Set node01 = treeFill.Nodes.Add("MFIBR", tvwChild, "B" & rs_get_branch(0), rs_branch(0), "cl")
                    Set node02 = treeFill.Nodes.Add("B" & rs_get_branch(0), tvwChild, "MFIRM", "RM", "cl")
                    Set rs_get_rm = MyConn.Execute("Select  rm_code,rm_name from employee_master where rm_code=" & refcode & " AND (TYPE='A' OR TYPE IS NULL)")
                    If Not rs_get_rm.EOF Then
                        Set node1 = treeFill.Nodes.Add("MFIRM", tvwChild, "R" & rs_get_rm(0), rs_get_rm(1), "cl")
                        Set node2 = treeFill.Nodes.Add("R" & rs_get_rm(0), tvwChild, "MFICL" & rmcnt, "Client", "cl")
                        node2.ExpandedImage = "op"
                        Set node3 = treeFill.Nodes.Add("R" & rs_get_rm(0), tvwChild, "MFIAG" & rmcnt, "Agent", "cl")
                        node1.ExpandedImage = "op"
                        node3.ExpandedImage = "op"
                    End If
                End If
            End If
        ElseIf Left(refcode, 1) = "2" Then
            treeFill.Nodes.Clear
            rmcnt = 0
            Set node1 = treeFill.Nodes.Add(, tvwNext, "MFIBR", "Branch", "cl")
            node1.ExpandedImage = "op"
            Set rs_get_branch = MyConn.Execute("select source from employee_master where rm_code in(" & refcode & ")")
            If Not rs_get_branch.EOF Then
                'Dim rs_branch As New ADODB.Recordset
                Set rs_branch = MyConn.Execute("select branch_name from branch_master where branch_code=" & rs_get_branch(0))
                If Not rs_branch.EOF Then
                    brcnt = brcnt + 1
                    Set node01 = treeFill.Nodes.Add("MFIBR", tvwChild, "B" & rs_get_branch(0), rs_branch(0), "cl")
                    Set node02 = treeFill.Nodes.Add("B" & rs_get_branch(0), tvwChild, "MFIRM" & brcnt, "RM", "cl")
                    'Set rs_get_rm = myconn.Execute("Select  rm_code,rm_name from employee_master where rm_code=" & refcode & " AND (TYPE='A' OR TYPE IS NULL)")
                    
                    'If Not rs_get_rm.EOF Then
                        'Set node1 = treeFill.Nodes.Add("MFIRM", tvwChild, "R" & rs_get_rm(0), rs_get_rm(1), "cl")
                        'Set node2 = treeFill.Nodes.Add("R" & rs_get_rm(0), tvwChild, "MFICL" & rmcnt, "Client", "cl")
                            Set rs_get_rm = New ADODB.Recordset
                            rs_get_rm.open "select NVL(branch_tar_cat,0) from branch_master where branch_code= " & rs_get_branch(0), MyConn, adOpenForwardOnly
                            If rs_get_rm.Fields(0) = 187 Or rs_get_rm.Fields(0) = 188 Then
                                rs_get_rm.Close
                                Set rs_get_rm = MyConn.Execute("Select  rm_code,rm_name from employee_master where rm_code=" & refcode & " AND (TYPE='A' OR TYPE IS NULL)")
                            Else
                                rs_get_rm.Close
                                Set rs_get_rm = MyConn.Execute("Select  rm_code,rm_name,source from employee_master where source=" & rs_get_branch(0) & " AND (TYPE='A' OR TYPE IS NULL) order by rm_name")
                            End If
                            
                            While Not rs_get_rm.EOF
                                rmcnt = rmcnt + 1
                                RMcode = "R" & Trim(rs_get_rm(0))
                                rmname = Trim(rs_get_rm(1))
                                Set node1 = treeFill.Nodes.Add("MFIRM" & brcnt, tvwChild, RMcode, rmname, "cl")
                                Set node2 = treeFill.Nodes.Add(RMcode, tvwChild, "MFICL" & rmcnt, "Client", "cl")
                                Set node3 = treeFill.Nodes.Add(RMcode, tvwChild, "MFIAG" & rmcnt, "Agent", "cl")
                                node2.ExpandedImage = "op"
                                node1.ExpandedImage = "op"
                                node3.ExpandedImage = "op"
                                rs_get_rm.MoveNext
                            Wend
                        node2.ExpandedImage = "op"
                        node1.ExpandedImage = "op"
                    'End If
                End If
            End If
            
        End If
End If
End Sub

Public Function getPrepaidPoints(from_dt As String, to_dt As String, busi_cd As String)
'Dim rsGetData As New ADODB.Recordset
Dim Curr_mon As String
Dim Curr_year As String
Dim fr_dt As Date
Dim to_dt1 As Date
    PrepaidPoints = 0
    getPrepaidPoints = 0
    
    Curr_mon = Format(ServerDateTime, "mm")
    Curr_year = Format(ServerDateTime, "yyyy")
    
    If Format(month(CDate(from_dt)), "00") = Curr_mon And Format(year(CDate(from_dt)), "00") = Curr_year Then
        'curr
        getPrepaidPoints = getPrepaid_Curr(Format(from_dt, "dd-MMM-yyyy"), Format((to_dt), "dd-MMM-yyyy"), busi_cd)
    Else
        fr_dt = Format(from_dt, "dd-mmm-yyyy")
        to_dt1 = Format(to_dt, "dd-mmm-yyyy")
        If Format(month(CDate(to_dt)), "00") = Curr_mon And Format(year(CDate(to_dt)), "00") = Curr_year Then
            'All
            While fr_dt < to_dt1
                PrepaidPoints = PrepaidPoints + getPrepaid_Hist(Format(fr_dt, "mm"), Format(fr_dt, "yyyy"), busi_cd)
                fr_dt = DateAdd("M", 1, fr_dt)
            Wend
            getPrepaidPoints = PrepaidPoints + getPrepaid_Curr("01-" & Format(to_dt, "MMM-yyyy"), Format(to_dt, "dd-MMM-yyyy"), busi_cd)
        Else
            'history
            While fr_dt < to_dt
                PrepaidPoints = PrepaidPoints + getPrepaid_Hist(Format(fr_dt, "mm"), Format(fr_dt, "yyyy"), busi_cd)
                fr_dt = DateAdd("M", 1, fr_dt)
            Wend
            getPrepaidPoints = PrepaidPoints
        End If
    End If
    
End Function

Public Function getPrepaidRedemptions(from_dt As String, to_dt As String, busi_cd As String)
Dim rsGetData As New ADODB.Recordset

    getPrepaidRedemptions = 0
    rsGetData.open "select nvl(sum(points_redem),0) points_redem from jt_prepaid_REDEMPTION where busi_code='" & busi_cd & "' and status='A' and approve_date between to_date('" & Format(from_dt, "dd-MMM-yyyy") & "') and to_date('" & Format(to_dt, "dd-MMM-yyyy") & "')", MyConn, adOpenForwardOnly
    If Not rsGetData.EOF Then
        getPrepaidRedemptions = rsGetData("points_redem")
    End If
    rsGetData.Close
    Set rsGetData = Nothing
    
End Function

Public Function getPrepaid_Curr(from_dt As String, to_dt As String, busi_cd As String)
Dim rsGetData As New ADODB.Recordset

    getPrepaid_Curr = 0
    rsGetData.open "select jtpkj_vh.GET_PREACC_MAR('" & busi_cd & "', to_date('" & Format(from_dt, "dd-MMM-yyyy") & "'),to_date('" & Format(to_dt, "dd-MMM-yyyy") & "')) points from dual", MyConn, adOpenForwardOnly
    If Not rsGetData.EOF Then
        getPrepaid_Curr = Round(rsGetData("points") / 1000)
    End If
    rsGetData.Close
    Set rsGetData = Nothing
    
End Function

Public Function getPrepaid_Hist(hist_mon As String, hist_year As String, busi_cd As String)
Dim rsGetData As New ADODB.Recordset

    getPrepaid_Hist = 0
    rsGetData.open "select points from jt_prepaid_club where busi_code='" & busi_cd & "' and mmonth='" & hist_mon & "' and yyear='" & hist_year & "'", MyConn, adOpenForwardOnly
    If Not rsGetData.EOF Then
        getPrepaid_Hist = rsGetData("points")
    End If
    rsGetData.Close
    Set rsGetData = Nothing
    
End Function
Public Sub Update_Lead_AR(Lead_no As String, tr_cd As String)
Dim rsSQLtmp As New ADODB.Recordset
Dim rsSQLtmp1 As New ADODB.Recordset

    If rsSQLtmp.State = 1 Then rsSQLtmp.Close
    rsSQLtmp.open "select isnull(count(*),0) from tblObjectType10020_3 where nid='" & Lead_no & "'", msConn, adOpenForwardOnly
    
    If rsSQLtmp(0) > 0 Then
        'update
            '2nd field
            If rsSQLtmp1.State = 1 Then rsSQLtmp1.Close
            rsSQLtmp1.open "select isnull(count(*),0) from tblObjectType10020_3 where nid='" & Lead_no & "' and FldString22493 is null", msConn, adOpenForwardOnly
            If rsSQLtmp1(0) > 0 Then
                msConn.Execute "update tblObjectType10020_3 set [FldString22493]='" & tr_cd & "' where nid='" & Lead_no & "'"
            Else
                '3rd field
                If rsSQLtmp1.State = 1 Then rsSQLtmp1.Close
                rsSQLtmp1.open "select isnull(count(*),0) from tblObjectType10020_3 where nid='" & Lead_no & "' and FldString22494 is null", msConn, adOpenForwardOnly
                If rsSQLtmp1(0) > 0 Then
                    msConn.Execute "update tblObjectType10020_3 set [FldString22494]='" & tr_cd & "' where nid='" & Lead_no & "'"
                Else
                    '4th Field
                    If rsSQLtmp1.State = 1 Then rsSQLtmp1.Close
                    rsSQLtmp1.open "select isnull(count(*),0) from tblObjectType10020_3 where nid='" & Lead_no & "' and FldString22495 is null", msConn, adOpenForwardOnly
                    If rsSQLtmp1(0) > 0 Then
                        msConn.Execute "update tblObjectType10020_3 set [FldString22495]='" & tr_cd & "' where nid='" & Lead_no & "'"
                    Else
                        '5th Field
                        If rsSQLtmp1.State = 1 Then rsSQLtmp1.Close
                        rsSQLtmp1.open "select isnull(count(*),0) from tblObjectType10020_3 where nid='" & Lead_no & "' and FldString22496 is null", msConn, adOpenForwardOnly
                        If rsSQLtmp1(0) > 0 Then
                            msConn.Execute "update tblObjectType10020_3 set [FldString22496]='" & tr_cd & "' where nid='" & Lead_no & "'"
                        End If
                    End If
                End If
            End If
    Else
        'insert
        msConn.Execute "insert into tblObjectType10020_3(nid,FldString22493) values('" & Lead_no & "','" & tr_cd & "')"
    End If
End Sub
Public Sub Update_Lead_AR_old(Lead_no As String, tr_cd As String)
Dim rsSQLtmp As New ADODB.Recordset
Dim rsSQLtmp1 As New ADODB.Recordset

    If rsSQLtmp.State = 1 Then rsSQLtmp.Close
    rsSQLtmp.open "select FldString22493,FldString22494,FldString22495,FldString22496 from tblObjectType10020_3 where nid='" & Lead_no & "'", msConn, adOpenForwardOnly
    If rsSQLtmp.EOF = False Then
        If rsSQLtmp(0) = tr_cd Then
            msConn.Execute "update tblObjectType10020_3 set FldString22493=NULL where nid='" & Lead_no & "'"
        ElseIf rsSQLtmp(1) = tr_cd Then
            msConn.Execute "update tblObjectType10020_3 set FldString22494=NULL where nid='" & Lead_no & "'"
        ElseIf rsSQLtmp(1) = tr_cd Then
            msConn.Execute "update tblObjectType10020_3 set FldString22495=NULL where nid='" & Lead_no & "'"
        ElseIf rsSQLtmp(1) = tr_cd Then
            msConn.Execute "update tblObjectType10020_3 set FldString22496=NULL where nid='" & Lead_no & "'"
        End If
    End If
    
    rsSQLtmp.Close
    Set rsSQLtmp = Nothing
End Sub

Public Function get_ATM_scheme(sch_cd As String) As Boolean
Dim rsATM As New ADODB.Recordset

    rsATM.open "select nvl(count(*),0) from reliance_atm_master where  sch_code='" & sch_cd & "' and from_dt<=sysdate and (to_dt>=sysdate or to_dt is null)", MyConn, adOpenForwardOnly
    If rsATM(0) > 0 Then
        get_ATM_scheme = True
    Else
        get_ATM_scheme = False
    End If
    
    rsATM.Close
    Set rsATM = Nothing
    
End Function
Public Function get_ATM_scheme_amt_condition(sch_cd As String, AMT As Long) As Boolean
Dim rsATM As New ADODB.Recordset

    rsATM.open "select min_amount from reliance_atm_master where  sch_code='" & sch_cd & "' and from_dt<=syadate and (to_dt>=sysdate or to_dt is null) ", MyConn, adOpenForwardOnly
    If Not rsATM.EOF Then
        If AMT >= rsATM(0) Then
            get_ATM_scheme_amt_condition = True
        Else
            get_ATM_scheme_amt_condition = False
        End If
    Else
        get_ATM_scheme_amt_condition = False
    End If
    
    rsATM.Close
    Set rsATM = Nothing
    
End Function


