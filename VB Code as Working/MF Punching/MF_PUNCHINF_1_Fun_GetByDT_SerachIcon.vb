Private Sub Picture7_Click(Index As Integer)
On Error GoTo err1
Dim openfile As String, ARL As String, FL As String
Dim strFile As String
Dim strPath As String
Dim strFiletype() As String
Dim l_file As String
Dim rsGetDocPath As New ADODB.Recordset

    '    If lbtrancode.Caption = "lbtrancode" Then
    '        MsgBox "Please Select a transaction to view documents ", vbInformation
    '        Exit Sub
    '    End If
        
    'ARL = "C:\Program Files\Adobe\Reader 9.0\Reader\AcroRd32.exe" ', vbNormalFocus
    
    Me.MousePointer = 11

    rsGetDocPath.open "select doc_filename,doc_path from tb_doc_upload where common_id='" & Trim(txtdocID.Text) & "' and tran_type='MF'", MyConn, adOpenForwardOnly
    If Not rsGetDocPath.EOF Then
        strFile = rsGetDocPath("doc_filename")
        strPath = UCase(rsGetDocPath("doc_path"))
    End If
    rsGetDocPath.Close
    Set rsGetDocPath = Nothing
    
    ReDim strFiletype(1)
    strFiletype = Split(strFile, ".")
    l_file = "c:\doc\doc" & Glbloginid & "." & LCase(strFiletype(1))
    
    'FL = "c:\doc\doc1.pdf"
    '
    'openfile = FL & "/" & ARL
    
    'Shell openfile, vbNormalFocus
    'App_doc_path & "/branch.pdf"
    'App_doc_path & "/branch.pdf"


    Generate_File strPath, strFile, l_file
    
    ShellExecute Me.hwnd, vbNullString, "c:\doc\ftp1.bat", vbNullString, "c:", SW_SHOWNORMAL
    For i = 1 To 1000
        DoEvents
    Next i
    ShellExecute Me.hwnd, vbNullString, l_file, vbNullString, "c:", SW_SHOWNORMAL
    'ShellExecute Me.hwnd, vbNullString, "d:\doc\abc.jpg", vbNullString, "c:", SW_SHOWNORMAL
    
    Me.MousePointer = 0
    Exit Sub
err1:
    Me.MousePointer = 0
    MsgBox err.Description
    
End Sub