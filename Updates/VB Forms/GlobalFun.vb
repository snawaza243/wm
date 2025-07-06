
Public Declare Function WritePrinter Lib "winspool.drv" _
    (ByVal hPrinter As Long, _
    pBuf As Any, _
    ByVal cdBuf As Long, _
    pcWritten As Long) As Long
Public Declare Function OpenPrinter Lib "winspool.drv" Alias "OpenPrinterA" (ByVal pPrinterName As String, phPrinter As Long, pDefault As Any) As Long

Public Declare Function ClosePrinter Lib "winspool.drv" _
    (ByVal hPrinter As Long) As Long
Public Declare Function StartDocPrinter Lib "winspool.drv" _
    Alias "StartDocPrinterA" _
    (ByVal hPrinter As Long, _
    ByVal Level As Long, _
    pDocInfo As Byte) As Long
Public Declare Function EndDocPrinter Lib "winspool.drv" _
    (ByVal hPrinter As Long) As Long

Dim rsRole As ADODB.Recordset
Dim RsRName As ADODB.Recordset
Dim RsRoleName As ADODB.Recordset
Dim RsFormPermission As ADODB.Recordset
Dim RsFormName As ADODB.Recordset
Dim RsData As ADODB.Recordset
Global Glbloginid As String
Global GlbPass As String
Global GlbroleId As String
Private GlbUpdatePre As String
Private Glbupdatenext As String
Private GlbINSPre As String
Private GlbINSnext As String


Private GlbUpdatePreType As String
Private GlbupdatenextType As String
Private GlbINSPreType As String
Private GlbINSnextType As String

Global Glbins_previousdate As Date
Global Glbins_nextdate As Date

Global GLBlocal_host As String
Global GLBglobal_host As String
Global GLBLOCAL_UID As String
Global GLBGLOBAL_UID As String
Global GLBLOCAL_PWD As String
Global GLBGLOBAL_PWD As String


Global Glbup_previousdate As Date
Global Glbup_nextdate As Date
Global GlbDataFilter As String
Global LockedUser As Boolean
Global LockedAllLI As Boolean

Public Enum enmDateFormat
    e1or2M_1or2D_YY = 1
    e1or2D_1or2M_2or4Y
    eMM_DD_YY
    eMM_DD_YYYY
    eYYYY_MM_DD
End Enum
Private Const CB_LIMITTEXT& = &H141
Private Declare Function SendMessage Lib "user32" Alias "SendMessageA" _
         (ByVal hwnd As Long, ByVal wMsg As Long, ByVal wParam As _
         Integer, ByVal lParam As Any) As Long
Public Sub SetComboBoxMaxLength(hwnd As Long, MaxLength As Long)
    SendMessage hwnd, CB_LIMITTEXT&, MaxLength, 0&
End Sub


Public Function RolePermission()
Dim i As Integer
frmLoginMaster.cboroleName.Clear
Set rsRole = New ADODB.Recordset
If rsRole.State = 1 Then rsRole.Close
rsRole.CursorLocation = adUseClient
Set rsRole = MyConn.Execute("select * from user_master where login_id='" & Trim(frmLoginMaster.text1.Text) & "' and login_pass='" & UserPassword & "' and status='1' AND LOGINVARIFY('" & Trim(frmLoginMaster.text1.Text) & "')=1")
'LogPrint "select * from user_master where login_id='" & Trim(frmLoginMaster.text1.Text) & "' and login_pass='" & UserPassword & "' and status='1' AND LOGINVARIFY('" & Trim(frmLoginMaster.text1.Text) & "')=1"
If rsRole.RecordCount > 0 Then
        LogPrint "Record Found"
        Glbloginid = rsRole!login_id
        GlbPass = Trim(frmLoginMaster.text2.Text) 'rsRole!login_pass
        With frmLoginMaster
        End With
        If InStr(1, rsRole!Role_id, ",") Then
            rolefind = Split(rsRole!Role_id, ",")
            For i = 0 To UBound(rolefind)
                Set RsRName = New ADODB.Recordset
                RsRName.CursorLocation = adUseClient
                RsRName.open "select role_name from role_master where role_id='" & Trim(rolefind(i)) & "'", MyConn
                If RsRName.RecordCount > 0 Then
                    If Not RsRName.EOF Then
                        frmLoginMaster.cboroleName.AddItem RsRName!role_name
                        frmLoginMaster.cboroleName.ListIndex = 0
                        frmLoginMaster.cborolename_Click
                    End If
                End If
            Next
        Else
                Set RsRName = New ADODB.Recordset
                RsRName.CursorLocation = adUseClient
                RsRName.open "select role_name from role_master where role_id='" & Trim(rsRole!Role_id) & "'", MyConn
                frmLoginMaster.cboroleName.AddItem RsRName!role_name
                frmLoginMaster.cboroleName.ListIndex = 0
                frmLoginMaster.cborolename_Click
        End If
Else
        'MsgBox "You Are not Authorized", vbOKOnly
End If
End Function
Public Function ButtonPermission(frm As Form)
Dim i As Integer
Dim X As Control
Set RsFormPermission = New ADODB.Recordset

RsFormPermission.CursorLocation = adUseClient
RsFormPermission.open "select * from ROLE_FORM_PERMISSION_VB where role_id='" & Trim(GlbroleId) & "'", MyConn
If RsFormPermission.RecordCount > 0 Then
    For i = 0 To RsFormPermission.RecordCount - 1
        If Trim(RsFormPermission!view_Button) = 1 Or Trim(RsFormPermission!add_Button) = 1 Or Trim(RsFormPermission!update_Button) = 1 Or Trim(RsFormPermission!print_Button) = 1 Then
            Set RsFormName = New ADODB.Recordset
            RsFormName.CursorLocation = adUseClient
            RsFormName.open "select main from backoffice_forms where formname='" & Trim(RsFormPermission!Form_name) & "'", MyConn
            If RsFormName.RecordCount > 0 Then
                If frm.Name = Trim(RsFormName!main) Then
                    For Each X In frm.Controls
                        If TypeOf X Is CommandButton Then
                            If Trim(RsFormPermission!view_Button) = 0 And X.Caption = "&View" Then
                                X.Enabled = False
                            End If
                            If Trim(RsFormPermission!add_Button) = 0 And (X.Caption = "&Add" Or X.Caption = "&Save") Then
                                X.Enabled = False
                            End If
                            If Trim(RsFormPermission!update_Button) = 0 And (X.Caption = "&Update" Or X.Caption = "&Modify") Then
                                X.Enabled = False
                            End If
                            If Trim(RsFormPermission!print_Button) = 0 And X.Caption = "&Print" Then
                                X.Enabled = False
                            End If
                        End If
                        
                    Next
                End If
            End If
        End If
       RsFormPermission.MoveNext
    Next
End If
End Function

Public Sub Show_Tr_Branches_new(strBusiRM As String)
Dim BrCode() As String
Dim GlCode() As String
Dim GDF As String
Dim RsData As New ADODB.Recordset
Dim RsData_filter As New ADODB.Recordset
Dim j, K As Integer
    Tr_Branches = ""
    sql = ""
    sql = " select branch_id  from userdetails_ji where login_id='" & strBusiRM & "' and datafilter in( "
    sql = sql & " select data_filter datafilter from datafilter_master where login_id in(select payroll_id from employee_master where type='A' and category_id=2001) and  login_id='" & strBusiRM & "' AND ROLE_DEFAULT='1' )"
    
    RsData_filter.open sql, MyConn, adOpenForwardOnly
    While Not RsData_filter.EOF
            Set RsData = MyConn.Execute("SELECT getBranch_TR('" & RsData_filter.Fields("branch_id") & "') FROM DUAL")
            If IsNull(RsData(0)) = False Then
                BrCode = Split(Mid(RsData(0), 2), "#")
                For j = LBound(BrCode) To UBound(BrCode)
                    Tr_Branches = Tr_Branches & BrCode(j) & ","
                Next
            End If
            RsData_filter.MoveNext
    Wend
    If Tr_Branches <> "" Then
        Tr_Branches = Left(Tr_Branches, Len(Tr_Branches) - 1)
    End If
    RsData_filter.Close
    If RsData.State = 1 Then RsData.Close
    Set RsData_filter = Nothing
    Set RsData = Nothing
End Sub
Public Function DataPermission()
On Error Resume Next
       Set RsData = New ADODB.Recordset
       RsData.CursorLocation = adUseClient
       RsData.open "select * from role_master where role_id='" & Trim(GlbroleId) & "'", MyConn
       If RsData.RecordCount > 0 Then
            GlbUpdatePre = Trim(RsData!up_pre_dur)
            Glbupdatenext = Trim(RsData!up_next_dur)
            GlbINSPre = Trim(RsData!in_pre_dur)
            GlbINSnext = Trim(RsData!in_next_dur)
            
            GlbUpdatePreType = Trim(RsData!up_pre_dur_type)
            GlbupdatenextType = Trim(RsData!up_next_dur_type)
            GlbINSPreType = Trim(RsData!in_pre_dur_type)
            GlbINSnextType = Trim(RsData!in_next_dur_type)
            Call DataFilterWithDatesforINs(Val(GlbINSPre), Val(GlbINSPreType), Val(GlbINSnext), Val(GlbINSnextType))
            Call DataFilterWithDatesforUPd(Val(GlbUpdatePre), Val(GlbUpdatePreType), Val(Glbupdatenext), Val(GlbupdatenextType))
            Set rsRole = New ADODB.Recordset
            rsRole.CursorLocation = adUseClient
            rsRole.open "select data_filter from datafilter_master where role_id='" & Trim(GlbroleId) & "' and login_id='" & Trim(Glbloginid) & "'", MyConn
            If rsRole.RecordCount > 0 Then
                GlbDataFilter = Trim(rsRole!data_filter)
            End If
            rsRole.Close
            '------------------------------------To Lock The User For Transaction------------------------------
            Set rsRole = New ADODB.Recordset
            rsRole.CursorLocation = adUseClient
            rsRole.open "select * from USER_LOCKED where userid='" & Trim(Glbloginid) & "'", MyConn
            If Not rsRole.EOF Then
                LockedUser = True
            End If
            rsRole.Close

            Set rsRole = New ADODB.Recordset
            rsRole.CursorLocation = adUseClient
            rsRole.open "select BLOCKED from RDC_BLOCK WHERE MODULE='LI'", MyConn, adOpenForwardOnly
            If rsRole("blocked") = "Y" Then
                LockedAllLI = True
            Else
                LockedAllLI = False
            End If
            rsRole.Close
            
       End If
End Function
Public Sub Show_Tr_Branches(strBusiRM As String)
Dim BrCode() As String
Dim GlCode() As String
Dim GDF As String
Dim RsData As New ADODB.Recordset
Dim RsData_filter As New ADODB.Recordset
Dim j, K As Integer
    Tr_Branches = ""
    RsData_filter.open "select data_filter datafilter from datafilter_master where login_id in(select payroll_id from employee_master where type='A' and category_id=2018) and  login_id='" & strBusiRM & "' AND ROLE_DEFAULT='1' ", MyConn, adOpenForwardOnly 'and role_id in ('5','37')
    If Not RsData_filter.EOF Then
        If Left(RsData_filter("datafilter"), 1) <> "2" And Left(RsData_filter("datafilter"), 1) <> "7" Then
            Tr_Branches = ""
            GlCode = Split(Mid(RsData_filter("datafilter"), 1), "#")
            For K = LBound(GlCode) To UBound(GlCode)
                Set RsData = MyConn.Execute("SELECT getBranch_TR('" & GlCode(K) & "') FROM DUAL")
                If IsNull(RsData(0)) = False Then
                    BrCode = Split(Mid(RsData(0), 2), "#")
                    For j = LBound(BrCode) To UBound(BrCode)
                        Tr_Branches = Tr_Branches & BrCode(j) & ","
                    Next
                End If
             Next
            If Tr_Branches <> "" Then
                Tr_Branches = Left(Tr_Branches, Len(Tr_Branches) - 1)
            End If
        End If
            '        If Left(RsData_filter("datafilter"), 1) = "2" Then
            '            BrCode = Split(Mid(RsData(0), 2), "#")
            '            Tr_Branches = ""
            '            For J = LBound(BrCode) To UBound(BrCode)
            '                Tr_Branches = Tr_Branches & BrCode(J) & ","
            '            Next
            '            Tr_Branches = Left(Tr_Branches, Len(Tr_Branches) - 1)
            '        End If
    End If
    RsData_filter.Close
    If RsData.State = 1 Then RsData.Close
    Set RsData_filter = Nothing
    Set RsData = Nothing
End Sub

Sub ShowProducts()    ''' By Pravesh for Product wise Security Purpose on 25 aug 2006
Dim rsData1 As New ADODB.Recordset
    If rsData1.State = 1 Then rsData1.Close
    Set rsData1 = MyConn.Execute("SELECT prod_code from bajaj_product_permissions where role_id='" & GlbroleId & "'")
    If rsData1.EOF = False Then
    Products = Left(rsData1(0), Len(rsData1(0)) - 1)
    Products = "'" & Replace(Products, "#", "','") & "'"
    End If
    rsData1.Close
    Set rsData1 = Nothing
End Sub
Public Function ServerDateTime() As Date
Dim rsDate As ADODB.Recordset
Set rsDate = New ADODB.Recordset
rsDate.CursorLocation = adUseClient
rsDate.open "select sysdate from dual", MyConn, adOpenDynamic, adLockReadOnly
ServerDateTime = Trim(rsDate(0))
End Function

'start Date Format

Public Function IsValidDateString(ByVal p_sSource As String _
                , ByVal p_lMode As enmDateFormat _
                , Optional ByVal p_IsValidateSeperator = False _
                , Optional ByVal p_AllowInternalSpaces = False) _
                As Boolean
        Dim bReturn As Boolean
        Dim sTemp As String: sTemp = Replace(p_sSource, ".", "/")
        If IsDate(sTemp) Then
                    
            ' Validate string length
            If Not LengthCheck(p_lMode, p_sSource, p_AllowInternalSpaces) Then
                Exit Function
            Else
                p_sSource = Trim(p_sSource)
            End If
            
            ' Validate Year
            Dim lYear As Long  ' passed byref
            If Not YearCheck(p_lMode, p_sSource, lYear) Then Exit Function
            
            
            ' Validate month
            Dim lMonth As Long
            lMonth = MonthExtract(p_lMode, p_sSource)
            If lMonth = 0 Then Exit Function
            
            
            ' Validate day
            Dim lday As Long
            lday = DayCheck(p_lMode, p_sSource, lMonth, lYear)
            If lday = 0 Then Exit Function
        
        
            ' [Validate seperator]    Done inline so can Exit Function
            If p_IsValidateSeperator Then
                Dim vDelims_ As Variant
                vDelims_ = Array("/", "-", ".")   ' Hardcoded for specific uses.
                
                Select Case p_lMode
                Case enmDateFormat.eMM_DD_YY, enmDateFormat.eMM_DD_YYYY
                    If Not InList(Mid(p_sSource, 3, 1), vDelims_) Then Exit Function
                    If Not InList(Mid(p_sSource, 6, 1), vDelims_) Then Exit Function
                Case enmDateFormat.eYYYY_MM_DD
                    If Not InList(Mid(p_sSource, 5, 1), vDelims_) Then Exit Function
                    If Not InList(Mid(p_sSource, 8, 1), vDelims_) Then Exit Function
                Case enmDateFormat.e1or2M_1or2D_YY
                    If Len(p_sSource) = 8 Then   ' MM\DD\YY
                        If Not InList(Mid(p_sSource, 3, 1), vDelims_) Then Exit Function
                        If Not InList(Mid(p_sSource, 6, 1), vDelims_) Then Exit Function
                    ElseIf Len(p_sSource) = 6 Then    ' M\D\YY
                        If Not InList(Mid(p_sSource, 2, 1), vDelims_) Then Exit Function
                        If Not InList(Mid(p_sSource, 4, 1), vDelims_) Then Exit Function
                    Else
                        If IsDigitsOnly(Left(p_sSource, 2)) Then   ' MM\D\YY
                            If Not InList(Mid(p_sSource, 3, 1), vDelims_) Then Exit Function
                        Else  ' M\DD\YY
                            If Not InList(Mid(p_sSource, 2, 1), vDelims_) Then Exit Function
                        End If
                        If Not InList(Mid(p_sSource, 5, 1), vDelims_) Then Exit Function
                    End If
                End Select
            End If
            
            ' just to be safe ...
            If Not IsDate(CStr(lday) & "/" & CStr(lMonth) & "/" & CStr(lYear)) Then
                Exit Function
            Else
                bReturn = True
            End If
        
        End If
        
        IsValidDateString = bReturn


End Function
Private Function LengthCheck(ByVal p_lMode As enmDateFormat _
                    , ByVal p_sSource As String _
                    , ByVal p_AllowInternalSpaces As Boolean) _
                    As Boolean
                    
        Dim bReturn As Boolean
        
        If Not p_AllowInternalSpaces Then
            p_sSource = Replace(p_sSource, " ", "")
        End If
        
        Select Case p_lMode
        Case enmDateFormat.eMM_DD_YY
            If Len(Trim(p_sSource)) = 8 Then
                bReturn = True
            End If
        Case enmDateFormat.eMM_DD_YYYY, enmDateFormat.eYYYY_MM_DD
            If Len(Trim(p_sSource)) = 10 Then
                bReturn = True
            End If
        Case enmDateFormat.e1or2M_1or2D_YY
            If Len(Trim(p_sSource)) >= 6 And Len(Trim(p_sSource)) <= 8 Then
                bReturn = True
            End If
        Case enmDateFormat.e1or2D_1or2M_2or4Y
            If Len(Trim(p_sSource)) >= 6 And Len(Trim(p_sSource)) <= 10 Then
                bReturn = True
            End If

        End Select
        
        LengthCheck = bReturn
        
End Function
Private Function YearCheck(ByVal p_lMode As enmDateFormat _
                    , ByVal p_sSource As String _
                    , ByRef P_LYEAR As Long) _
                    As Boolean
                    
        Dim bReturn As Boolean
        
        Dim sYear As String
        Select Case p_lMode
        Case enmDateFormat.eMM_DD_YY, enmDateFormat.e1or2M_1or2D_YY
            sYear = Right(p_sSource, 2)
        Case enmDateFormat.eMM_DD_YYYY
            sYear = Right(p_sSource, 4)
        Case enmDateFormat.eYYYY_MM_DD
            sYear = Left(p_sSource, 4)
        Case enmDateFormat.e1or2D_1or2M_2or4Y
            If IsDigitsOnly(Right(p_sSource, 4)) Then
                sYear = Right(p_sSource, 4)
            Else
                sYear = Right(p_sSource, 2)
            End If
        End Select
        
        If Not IsDigitsOnly(sYear) Then
            Exit Function
        Else
            P_LYEAR = CLng(sYear)
        End If
        
        If P_LYEAR < 0 Then
            Exit Function
        Else
            bReturn = True
        End If
        
        YearCheck = bReturn
        
End Function
Private Function MonthExtract(ByVal p_lMode As enmDateFormat _
                    , ByVal p_sSource As String) _
                    As Long
                    
        ' Returns month if valid, zero otherwise
        Dim lReturn As Long
        Dim sMonth As String
        
        ' extract month
        Select Case p_lMode
        Case enmDateFormat.eMM_DD_YY, enmDateFormat.eMM_DD_YYYY
            sMonth = Left(p_sSource, 2)
        Case enmDateFormat.eYYYY_MM_DD
            sMonth = Mid(p_sSource, 6, 2)
        Case enmDateFormat.e1or2M_1or2D_YY
            Select Case Len(p_sSource)
            Case 6
                sMonth = Left(p_sSource, 1)
            Case 7
                If IsDigitsOnly(Left(p_sSource, 2)) Then
                    sMonth = Left(p_sSource, 2)
                Else
                    sMonth = Left(p_sSource, 1)
                End If
            Case 8
                sMonth = Left(p_sSource, 2)
            End Select
            
        Case enmDateFormat.e1or2D_1or2M_2or4Y
            Select Case Len(p_sSource)
            Case 6
                sMonth = Left(p_sSource, 1)
            Case 10
                sMonth = Mid(p_sSource, 4, 2)
            Case Else
                If IsDigitsOnly(Left(p_sSource, 2)) Then
                    sMonth = Left(p_sSource, 2)
                Else
                    sMonth = Left(p_sSource, 1)
                End If
            End Select
        
        End Select
        
        ' Validate
        If IsDigitsOnly(sMonth) Then
            If CLng(sMonth) >= 1 And CLng(sMonth) <= 12 Then
                lReturn = CLng(sMonth)
            End If
        End If
        
        MonthExtract = lReturn
        
End Function
Private Function DayCheck(ByVal p_lMode As enmDateFormat _
                    , ByVal p_sSource As String _
                    , ByVal p_lMonth As Long _
                    , ByVal P_LYEAR As Long) _
                    As Long
                    
        ' Returns day if valid, zero otherwise
                    
        Dim lReturn As Long
        
        ' extract day
        Dim sDay As String
        Select Case p_lMode
        Case enmDateFormat.eMM_DD_YY, enmDateFormat.eMM_DD_YYYY
            sDay = Mid(p_sSource, 4, 2)
        Case enmDateFormat.eYYYY_MM_DD
            sDay = Mid(p_sSource, 9, 2)
        Case enmDateFormat.e1or2M_1or2D_YY
            Select Case Len(p_sSource)
            Case 8
                sDay = Mid(p_sSource, 4, 2)
            Case 6
                sDay = Mid(p_sSource, 3, 1)
            Case Else
                If IsDigitsOnly(Mid(p_sSource, 3, 2)) Then
                    sDay = Mid(p_sSource, 3, 2)
                Else
                    sDay = Mid(p_sSource, 4, 1)
                End If
            End Select
        
        Case enmDateFormat.e1or2D_1or2M_2or4Y
            Select Case Len(p_sSource)
            Case 10
                sDay = Left(p_sSource, 2)
            Case 6
                sDay = Mid(p_sSource, 3, 1)
            Case 7
                If IsDigitsOnly(Mid(p_sSource, 3, 2)) Then
                    sDay = Mid(p_sSource, 3, 2)
                Else
                    sDay = Mid(p_sSource, 4, 1)
                End If
            Case 8
                If IsDigitsOnly(Mid(p_sSource, 4, 2)) Then
                    sDay = Mid(p_sSource, 4, 2)
                Else
                    sDay = Mid(p_sSource, 3, 1)
                End If
            Case 9
                If IsDigitsOnly(Mid(p_sSource, 3, 2)) Then
                    sDay = Mid(p_sSource, 3, 2)
                Else
                    sDay = Mid(p_sSource, 4, 1)
                End If
            End Select
            
        End Select
        
        
        ' ? positve number
        If Not IsDigitsOnly(sDay) Then
            Exit Function
        Else
            Dim lday As Long: lday = CLng(sDay)
            If lday < 0 Then Exit Function
        End If
        
        ' Get maximum days in the month
        Dim lMax As Long
        Select Case p_lMonth
        Case 4, 6, 9, 11
            lMax = 30
        Case 1, 3, 5, 7, 8, 10, 12
            lMax = 31
        Case 2
            If IsLeapYear(P_LYEAR) Then
                lMax = 29
            Else
                lMax = 28
            End If
        End Select
        
        ' showtime
        If lday > lMax Then Exit Function
        
        DayCheck = lday
        
End Function
Private Function IsLeapYear(P_LYEAR As Long)
        If (P_LYEAR Mod 4 = 0) And _
                ((P_LYEAR Mod 100 <> 0) Or (P_LYEAR Mod 400 = 0)) Then
        IsLeapYear = True
    Else
        IsLeapYear = False
    End If
End Function
Private Function InList(p_vCompare As Variant _
                , p_vItems_ As Variant) _
                As Boolean
    '' Returns True if 1st parameter is a member of second.

    Dim lMin As Long: lMin = LBound(p_vItems_)
    Dim lMax As Long: lMax = UBound(p_vItems_)
    Dim bReturn As Boolean: bReturn = False
    Dim l As Long
    
    For l = lMin To lMax
        If p_vCompare = p_vItems_(l) Then
            bReturn = True
            Exit For
        End If
    Next
    
    InList = bReturn

End Function
Public Function IsDigitsOnly(Value As String) As Boolean
    IsDigitsOnly = Not Value Like "*[!0-9]*"
End Function

'End Date Format

'vinod Print in Dos

Public Sub Print_File(Fname As String)
    Dim StrToPrint As String
    Dim LngPrinted As Long
    Dim lhPrinter As Long
    Dim PrntErr As Long
    
    If (Fname = "") Then
        MsgBox "Nothing to Print", vbCritical, "Error"
        Exit Sub
    End If
    
    PrntErr = OpenPrinter(Printer.DeviceName, lhPrinter, 0)
    
    'PrntErr = OpenPrinter(" ", lhPrinter, 0&)
    
    If (PrntErr = 0) Then
       MsgBox "DLL Error: " & err.LastDllError & " = " & Error(err.LastDllError)
        MsgBox "Error in Printing ", vbCritical, "Error"
        Exit Sub
    End If
        
    PrntErr = StartDocPrinter(lhPrinter, 1, 0&)
    
    If (PrntErr = 0) Then
        'Printer is Directly connected to the Machine
        Call PrintToPort(Fname)
        Exit Sub
    End If
        
    Open Fname For Input As #1
    
    StrToPrint = ""
    
    While Not EOF(1)
        Line Input #1, StrToPrint
        StrToPrint = StrToPrint + Chr(13) + Chr(10)
        WritePrinter lhPrinter, ByVal StrToPrint, _
            Len(StrToPrint), LngPrinted
    Wend
    
    EndDocPrinter lhPrinter
    ClosePrinter lhPrinter
    Close #1
End Sub

Private Sub PrintToPort(Fname As String)
    Open Fname For Input As #1
    Open "132.0.0.250" For Output As #2
    
    StrToPrint = ""
    
    While Not EOF(1)
        Line Input #1, StrToPrint
        StrToPrint = StrToPrint
        Print #2, StrToPrint
    Wend
    
    Close #2
    Close #1
End Sub
'end Dos Print

Public Function checkday(lday As Long, p_lMonth As Long, P_LYEAR As Long)
Dim yr As Long
Dim lMax As Long

        Select Case p_lMonth
        Case 4, 6, 9, 11
            lMax = 30
        Case 1, 3, 5, 7, 8, 10, 12
            lMax = 31
        Case 2
            If IsLeapYear(P_LYEAR) Then
                lMax = 29
            Else
                lMax = 28
            End If
        End Select
        
        If lday = lMax Then
            checkday = True
        Else
            checkday = False
        End If
End Function
'vinod security Purpose code 3 May
Public Sub ShowData()
'On Error resume Next
Dim BrCode() As String
Dim GlCode() As String
Dim GDF As String
Dim New_Branch As String
Dim RsData As New ADODB.Recordset
Dim rsData1 As New ADODB.Recordset
Dim rsRMDATA As New ADODB.Recordset
Dim j, K As Integer

    If Left(GlbDataFilter, 1) = "7" Then
        Set RsData = MyConn.Execute("select branch_code from branch_master where (BRANCH_TYPE <>'Inactive' OR BRANCH_TYPE IS NULL) and category_id not in (1006)")
        New_Branch = ""
        While Not RsData.EOF
            New_Branch = New_Branch & "#" & RsData(0)
            RsData.MoveNext
        Wend
        
    End If
    'InactiveBranches
    If Left(GlbDataFilter, 1) = "2" Then
        GlCode = Split(Mid(GlbDataFilter, 1), "#")
        Set RsData = MyConn.Execute("SELECT getBranchRM(" & GlCode(0) & ") FROM DUAL")
        SRmCode = GlCode(0)
        If SRmCode <> "" Then
            Set rsRMDATA = MyConn.Execute("SELECT PAYROLL_ID FROM EMPLOYEE_MASTER WHERE RM_CODE=" & SRmCode & " ")
        End If
        If rsRMDATA.EOF = False Then
            SPayrollId = rsRMDATA(0)
        End If
    ElseIf Left(GlbDataFilter, 1) <> "2" And Left(GlbDataFilter, 1) <> "7" Then
        Branches = ""
        GlCode = Split(Mid(GlbDataFilter, 1), "#")
        For K = LBound(GlCode) To UBound(GlCode)
            Set RsData = MyConn.Execute("SELECT getBranch('" & GlCode(K) & "') FROM DUAL")
            If IsNull(RsData(0)) = False Then
                BrCode = Split(Mid(RsData(0), 2), "#")
                For j = LBound(BrCode) To UBound(BrCode)
                    Branches = Branches & BrCode(j) & ","
                Next
            End If
         Next
        If Branches <> "" Then
            Branches = Left(Branches, Len(Branches) - 1)
        End If
    End If
    
    If Left(GlbDataFilter, 1) = "2" Then
        BrCode = Split(Mid(RsData(0), 2), "#")
        Branches = ""
        For j = LBound(BrCode) To UBound(BrCode)
            Branches = Branches & BrCode(j) & ","
        Next
        Branches = Left(Branches, Len(Branches) - 1)
    End If
    
    If Left(GlbDataFilter, 1) = "7" Then
        BrCode = Split(Mid(New_Branch, 2), "#")
        Branches = ""
        For j = LBound(BrCode) To UBound(BrCode)
            Branches = Branches & BrCode(j) & ","
        Next
        Branches = Left(Branches, Len(Branches) - 1)
    End If
    
    If Left(GlbDataFilter, 1) = "7" Then
        Set RsData = MyConn.Execute("select branch_code from branch_master where category_id in (1006)")
        InactiveBranches = ""
        While Not RsData.EOF
            InactiveBranches = InactiveBranches & RsData(0) & ","
            RsData.MoveNext
        Wend
        If InactiveBranches <> "" Then
            InactiveBranches = Mid(InactiveBranches, 1, Len(InactiveBranches) - 1)
        End If
    End If
    Call ShowProducts  ''By Pravesh on 25 Aug 2006
End Sub
'vinod security Purpose code 3 May end
' new addition by kalra 27-04-05
Public Sub DataFilterWithDatesforINs(ins_pre_day As Integer, ins_pre_type As Integer, ins_next_day As Integer, ins_next_type As Integer)
Dim GetCurYeardateFRTO As ADODB.Recordset
Dim FinYearStartDate As Date
Dim ActaulStartDate As Date

If ins_pre_type = 1 And ins_next_type = 1 Then
    Glbins_previousdate = DateAdd("d", "-" & ins_pre_day, Format(ServerDateTime, "dd-mm-yyyy"))
    Glbins_nextdate = DateAdd("d", ins_next_day - 1, Format(ServerDateTime, "dd-mm-yyyy"))
ElseIf ins_pre_type = 1 And ins_next_type = 2 Then
    Glbins_previousdate = DateAdd("d", "-" & ins_pre_day, Format(ServerDateTime, "dd-mm-yyyy"))
    Glbins_nextdate = DateAdd("m", ins_next_day - 1, Format(ServerDateTime, "dd-mm-yyyy"))
ElseIf ins_pre_type = 1 And ins_next_type = 3 Then
    Set GetCurYeardateFRTO = New ADODB.Recordset
    Set GetCurYeardateFRTO = MyConn.Execute("select date_fr,date_to from financial_year")
    If Not GetCurYeardateFRTO.EOF Then
        FinYearStartDate = Format(GetCurYeardateFRTO("date_fr"), "dd-mm-yyyy")
        ActaulStartDate = DateAdd("d", "-" & ins_pre_day, Format(ServerDateTime, "dd-mm-yyyy"))
        If Format(FinYearStartDate, "dd-mm-yyyy") >= Format(ActaulStartDate) Then
            Glbins_previousdate = Format(FinYearStartDate, "DD-MM-YYYY")
        Else
            Glbins_previousdate = Format(ActaulStartDate, "DD-MM-YYYY")
        End If
        Glbins_nextdate = DateAdd("YYYY", ins_next_day - 1, Format(GetCurYeardateFRTO("date_TO"), "dd-mm-yyyy"))
    End If
ElseIf ins_pre_type = 2 And ins_next_type = 1 Then
    Glbins_previousdate = DateAdd("m", "-" & ins_pre_day, Format(ServerDateTime, "dd-mm-yyyy"))
    Glbins_nextdate = DateAdd("d", ins_next_day - 1, Format(ServerDateTime, "dd-mm-yyyy"))
ElseIf ins_pre_type = 2 And ins_next_type = 2 Then
    Glbins_previousdate = Format(DateAdd("m", "-" & ins_pre_day, ServerDateTime), "dd/mm/yyyy")
    Glbins_nextdate = Format(DateAdd("m", ins_next_day - 1, ServerDateTime), "dd/mm/yyyy")
ElseIf ins_pre_type = 2 And ins_next_type = 3 Then
    Set GetCurYeardateFRTO = New ADODB.Recordset
    Set GetCurYeardateFRTO = MyConn.Execute("select date_fr,date_to from financial_year")
    If Not GetCurYeardateFRTO.EOF Then
        FinYearStartDate = Format(GetCurYeardateFRTO("date_fr"), "dd-mm-yyyy")
        ActaulStartDate = DateAdd("m", "-" & ins_pre_day, Format(ServerDateTime, "dd-mm-yyyy"))
        If Format(FinYearStartDate, "dd-mm-yyyy") >= Format(ActaulStartDate) Then
            Glbins_previousdate = Format(FinYearStartDate, "DD-MM-YYYY")
        Else
            Glbins_previousdate = Format(ActaulStartDate, "DD-MM-YYYY")
        End If
        Glbins_nextdate = DateAdd("YYYY", ins_next_day - 1, Format(GetCurYeardateFRTO("date_TO"), "dd-mm-yyyy"))
    End If
ElseIf ins_pre_type = 3 And ins_next_type = 1 Then
    Set GetCurYeardateFRTO = New ADODB.Recordset
    Set GetCurYeardateFRTO = MyConn.Execute("select date_fr,date_to from financial_year")
    If Not GetCurYeardateFRTO.EOF Then
        Glbins_previousdate = DateAdd("yyyy", "-" & ins_pre_day, Format(GetCurYeardateFRTO("date_fr"), "dd-mm-yyyy"))
    End If
    Glbins_nextdate = DateAdd("d", ins_next_day - 1, Format(ServerDateTime, "dd-mm-yyyy"))
ElseIf ins_pre_type = 3 And ins_next_type = 2 Then
    Set GetCurYeardateFRTO = New ADODB.Recordset
    Set GetCurYeardateFRTO = MyConn.Execute("select date_fr,date_to from financial_year")
    If Not GetCurYeardateFRTO.EOF Then
        Glbins_previousdate = DateAdd("yyyy", "-" & ins_pre_day, Format(GetCurYeardateFRTO("date_fr"), "dd-mm-yyyy"))
    End If
    Glbins_nextdate = DateAdd("m", ins_next_day - 1, Format(ServerDateTime, "dd-mm-yyyy"))
ElseIf ins_pre_type = 3 And ins_next_type = 3 Then
    Set GetCurYeardateFRTO = New ADODB.Recordset
    Set GetCurYeardateFRTO = MyConn.Execute("select date_fr,date_to from financial_year")
    'Format(ServerDateTime, "dd-mm-yyyy")
    If Not GetCurYeardateFRTO.EOF Then
        'Glbins_previousdate = DateAdd("yyyy", "-" & ins_pre_day, Format(GetCurYeardateFRTO("date_fr"), "dd-mm-yyyy"))
        Glbins_previousdate = DateAdd("yyyy", "-" & ins_pre_day, Format(ServerDateTime, "dd-mm-yyyy"))
    End If
    'Glbins_nextdate = DateAdd("yyyy", ins_next_day - 1, Format(GetCurYeardateFRTO("date_to"), "dd-mm-yyyy"))
    Glbins_nextdate = DateAdd("yyyy", ins_next_day - 1, Format(ServerDateTime, "dd-mm-yyyy"))
End If
End Sub
'' end here
Public Sub DataFilterWithDatesforUPd(up_pre_day As Integer, up_pre_type As Integer, up_next_day As Integer, up_next_type As Integer)
Dim GetCurYeardateFRTO As ADODB.Recordset
Dim FinYearStartDate As Date
Dim ActaulStartDate As Date
If up_pre_type = 1 And up_next_type = 1 Then
    Glbup_previousdate = DateAdd("d", "-" & up_pre_day, Format(ServerDateTime, "dd-mm-yyyy"))
    Glbup_nextdate = DateAdd("d", up_next_day - 1, Format(ServerDateTime, "dd-mm-yyyy"))
ElseIf up_pre_type = 1 And up_next_type = 2 Then
    Glbup_previousdate = DateAdd("d", "-" & up_pre_day, Format(ServerDateTime, "dd-mm-yyyy"))
    Glbup_nextdate = DateAdd("m", up_next_day - 1, Format(ServerDateTime, "dd-mm-yyyy"))
ElseIf up_pre_type = 1 And up_next_type = 3 Then
    Set GetCurYeardateFRTO = New ADODB.Recordset
    Set GetCurYeardateFRTO = MyConn.Execute("select date_fr,date_to from financial_year")
    If Not GetCurYeardateFRTO.EOF Then
        FinYearStartDate = Format(GetCurYeardateFRTO("date_fr"), "dd-mm-yyyy")
        ActaulStartDate = DateAdd("d", "-" & up_pre_day, Format(ServerDateTime, "dd-mm-yyyy"))
        If Format(FinYearStartDate, "dd-mm-yyyy") >= Format(ActaulStartDate) Then
            Glbup_previousdate = Format(FinYearStartDate, "DD-MM-YYYY")
        Else
            Glbup_previousdate = Format(ActaulStartDate, "DD-MM-YYYY")
        End If
        Glbup_nextdate = DateAdd("YYYY", up_next_day - 1, Format(GetCurYeardateFRTO("date_TO"), "dd-mm-yyyy"))
    End If
ElseIf up_pre_type = 2 And up_next_type = 1 Then
    Glbup_previousdate = DateAdd("m", "-" & up_pre_day, Format(ServerDateTime, "dd-mm-yyyy"))
    Glbup_nextdate = DateAdd("d", up_next_day - 1, Format(ServerDateTime, "dd-mm-yyyy"))
ElseIf up_pre_type = 2 And up_next_type = 2 Then
    Glbup_previousdate = Format(DateAdd("m", "-" & up_pre_day, ServerDateTime), "dd/mm/yyyy")
    'Glbup_previousdate = DateAdd("m", "-" & up_pre_day, Format(ServerDateTime, "dd-mm-yyyy"))
    'Glbins_nextdate = DateAdd("m", up_next_day - 1, Format(ServerDateTime, "dd-mm-yyyy"))' vinod due to error
    Glbup_nextdate = Format(DateAdd("m", up_next_day - 1, ServerDateTime), "dd-mm-yyyy")
    'Glbup_nextdate = DateAdd("m", up_next_day - 1, Format(ServerDateTime, "dd-mm-yyyy"))
ElseIf up_pre_type = 2 And up_next_type = 3 Then
    Set GetCurYeardateFRTO = New ADODB.Recordset
    Set GetCurYeardateFRTO = MyConn.Execute("select date_fr,date_to from financial_year")
    If Not GetCurYeardateFRTO.EOF Then
        FinYearStartDate = Format(GetCurYeardateFRTO("date_fr"), "dd-mm-yyyy")
        ActaulStartDate = DateAdd("m", "-" & ins_pre_day, Format(ServerDateTime, "dd-mm-yyyy"))
        If Format(FinYearStartDate, "dd-mm-yyyy") >= Format(ActaulStartDate) Then
            Glbup_previousdate = Format(FinYearStartDate, "DD-MM-YYYY")
        Else
            Glbup_previousdate = Format(ActaulStartDate, "DD-MM-YYYY")
        End If
        Glbup_nextdate = DateAdd("YYYY", up_next_day - 1, Format(GetCurYeardateFRTO("date_TO"), "dd-mm-yyyy"))
    End If
ElseIf up_pre_type = 3 And up_next_type = 1 Then
    Set GetCurYeardateFRTO = New ADODB.Recordset
    Set GetCurYeardateFRTO = MyConn.Execute("select date_fr,date_to from financial_year")
    If Not GetCurYeardateFRTO.EOF Then
        Glbup_previousdate = DateAdd("yyyy", "-" & up_pre_day, Format(GetCurYeardateFRTO("date_fr"), "dd-mm-yyyy"))
    End If
    Glbup_nextdate = DateAdd("d", up_next_day - 1, Format(ServerDateTime, "dd-mm-yyyy"))
ElseIf up_pre_type = 3 And up_next_type = 2 Then
    Set GetCurYeardateFRTO = New ADODB.Recordset
    Set GetCurYeardateFRTO = MyConn.Execute("select date_fr,date_to from financial_year")
    If Not GetCurYeardateFRTO.EOF Then
        Glbup_previousdate = DateAdd("yyyy", "-" & up_pre_day, Format(GetCurYeardateFRTO("date_fr"), "dd-mm-yyyy"))
    End If
    Glbup_nextdate = DateAdd("m", up_next_day - 1, Format(ServerDateTime, "dd-mm-yyyy"))
ElseIf up_pre_type = 3 And up_next_type = 3 Then
    Set GetCurYeardateFRTO = New ADODB.Recordset
    Set GetCurYeardateFRTO = MyConn.Execute("select date_fr,date_to from financial_year")
    'Format(ServerDateTime, "dd-mm-yyyy")
    If Not GetCurYeardateFRTO.EOF Then
        'Glbup_previousdate = DateAdd("yyyy", "-" & up_pre_day, Format(GetCurYeardateFRTO("date_fr"), "dd-mm-yyyy"))
        Glbup_previousdate = DateAdd("yyyy", "-" & up_pre_day, Format(ServerDateTime, "dd-mm-yyyy"))
    End If
    'Glbup_nextdate = DateAdd("yyyy", up_next_day - 1, Format(GetCurYeardateFRTO("date_to"), "dd-mm-yyyy"))
    Glbup_nextdate = DateAdd("yyyy", up_next_day - 1, Format(ServerDateTime, "dd-mm-yyyy"))
End If


End Sub
'' end here




