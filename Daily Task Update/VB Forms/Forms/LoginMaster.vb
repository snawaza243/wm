Dim RsRoleName As ADODB.Recordset
Dim RsFormPermission As ADODB.Recordset
Dim rs_pwd As New ADODB.Recordset
Dim br_code As String
Dim tt_num As String
Dim MyEncodeType    As ENCODE_METHOD
Dim etPriority      As MAIL_PRIORITY
Private WithEvents poSendMail As vbSendMail.clsSendMail
Private Declare Function GetKeyState Lib "user32" (ByVal nVirtKey As Long) As Integer
Public Sub cborolename_Click()
CmdOk.Enabled = True
'cmdOK.SetFocus
End Sub
Private Sub cboroleName_KeyPress(KeyAscii As Integer)
    If KeyAscii = 13 Then
        If CmdOk.Enabled = True Then CmdOk.SetFocus
    Else
        AutoMatch cboroleName, KeyAscii
        KeyAscii = 0
    End If
End Sub
Private Sub cmdCancel_Click()
 If MyConn.State = 1 Then MyConn.Close
 End
End Sub

Private Sub cmdforgot_Click()
frmforgot.Visible = True
End Sub


Private Sub cmdforgotclose_Click()
Me.Caption = "Wealthmaker Reporting - Login"
frmforgot.Visible = False
frmlog.Visible = True
End Sub

Private Sub cmdforgotok_Click()
On Error Resume Next
If Trim(Text3.Text) = "" Then
    MsgBox "Please Enter User Name", vbInformation
    Text3.Text = ""
    Text3.SetFocus
    Exit Sub
End If

If mskforgot.Text = "__/__/____" Then
    MsgBox "Please Enter Date of Birth", vbInformation
    mskforgot.SetFocus
    Exit Sub
End If

If Check_Date(mskforgot.Text) = False Then
    MsgBox "Please Enter Valid Date", vbInformation
    mskforgot.SetFocus
    Exit Sub
End If

If rs_pwd.State = 1 Then rs_pwd.Close
rs_pwd.open "select * from employee_master where payroll_id='" & Text3.Text & "' ", MyConn, adOpenDynamic, adLockOptimistic
If rs_pwd.EOF = False Then
    If rs_pwd.State = 1 Then rs_pwd.Close
    rs_pwd.open "select * from employee_master where payroll_id='" & Text3.Text & "' and dob='" & Format(mskforgot.Text, "dd-MMM-yyyy") & "' ", MyConn, adOpenDynamic, adLockOptimistic
    If rs_pwd.EOF = False Then
        If rs_pwd.State = 1 Then rs_pwd.Close
        rs_pwd.open "select user_unlog(login_pass) from user_master where login_id='" & Text3.Text & "'", MyConn, adOpenDynamic, adLockOptimistic
        If rs_pwd.EOF = False Then pwd = rs_pwd(0)
        
        If rs_pwd.State = 1 Then rs_pwd.Close
        rs_pwd.open "select rm_name,payroll_id,email from employee_master where payroll_id='" & Text3.Text & "'", MyConn, adOpenDynamic, adLockOptimistic
        If rs_pwd.EOF = False Then
            rmname = rs_pwd(0)
            UserID = rs_pwd(1)
            Email = rs_pwd(2)
        End If
        If rs_pwd.State = 1 Then rs_pwd.Close
        
        If pwd <> "" And Email <> "" Then
            strmsg = "Dear " & rmname & ",<br><br> This is response to your forgot password query. Your login details are provided below <br>Username : " & UserID & "<br>Password : " & pwd & "<br><br>For security purpose, you are advised to delete this email once you have read it. <br><br> Regards,<br>Wealthmaker<br>IT Team "
            Dim conn As ADODB.Connection, cmd As ADODB.Command, RS As ADODB.Recordset
            Dim SqlStr As String
            Set RS = New ADODB.Recordset
            Set cmd = New ADODB.Command
            Set conn = New ADODB.Connection
            Call cmdSend_Click(Email, strmsg)
            'Sqlstr = "Call send_mail ('" & email & "','wealthmaker@bajajcapital.com','" & strmsg & "','','','','Forgot Password Request');"
            Set RS = MyConn.Execute(SqlStr)
            
            MsgBox "Your username password has been send on your email", vbInformation
        Else
            MsgBox "Information Does't Exist", vbInformation
        End If
        frmforgot.Visible = False
        Me.Caption = "Wealthmaker- Login"
        frmlog.Visible = True
        text1.SetFocus
    Else
        MsgBox "Invalid Date of Birth", vbInformation
        mskforgot.SetFocus
    End If
Else
    MsgBox "Invalid User Name", vbInformation
    Text3.SetFocus
End If
End Sub

Public Sub cmdOk_Click()
Dim i As Long
Dim j As Integer
Dim rsTran As ADODB.Recordset
Dim rsPass As ADODB.Recordset 'For expire Password
Branches = ""
SRmCode = ""
If MyConn.State = 0 Then
    With MyConn
            .ConnectionTimeout = 999999999
            .ConnectionString = "DSN=test;UID=" & DataBaseUser & "; Pwd=" & DataBasePassword & ""
            .open
    End With
End If
role_name = Trim(UCase(cboroleName.Text))
Set rsPass = New ADODB.Recordset
rsPass.CursorLocation = adUseClient
rsPass.open "select expire_date from user_master where login_id='" & Trim(Glbloginid) & "' and login_pass='" & UserPassword & "'", MyConn
If Not rsPass.EOF Then
    If Trim(rsPass(0)) = "" Or IsNull(Trim(rsPass(0))) Then
        MyConn.Execute ("update user_master set expire_date = to_date('" & Format(DateAdd("d", 15, ServerDateTime), "dd/mm/yyyy") & "','DD-MM-YYYY') where login_id='" & Trim(Glbloginid) & "' and login_pass='" & UserPassword & "'")
    Else
       If CheckDate(Format(ServerDateTime, "dd/mm/yyyy"), Format(rsPass(0), "dd/mm/yyyy")) = False Then
           MsgBox "Please Change Your Password First Using Wealthmaker.in", vbCritical, "BackOffice"
           Exit Sub
        End If
    End If
End If

Call MainMenuFalse
Set RsRoleName = New ADODB.Recordset
RsRoleName.CursorLocation = adUseClient
RsRoleName.open "select * from role_master where role_name='" & Trim(cboroleName.List(cboroleName.ListIndex)) & "'", MyConn
If RsRoleName.RecordCount > 0 Then
    GlbroleId = Trim(RsRoleName!Role_id)
    Set RsFormPermission = New ADODB.Recordset
    RsFormPermission.CursorLocation = adUseClient
    If RsFormPermission.State = 1 Then RsFormPermission.Close
    RsFormPermission.open "select * from ROLE_FORM_PERMISSION_VB where role_id='" & Trim(RsRoleName!Role_id) & "'", MyConn
    If RsFormPermission.RecordCount > 0 Then
        For i = 0 To RsFormPermission.RecordCount - 1
            If RsFormPermission!view_Button = 1 Or RsFormPermission!add_Button = 1 Or RsFormPermission!update_Button = 1 Or RsFormPermission!print_Button Then
                For j = 1 To main.MASTER.count
                    If "master(" & j & ")" = RsFormPermission!Form_name Then
                        main.MASTER(j).Visible = True
                    End If
                Next
                
                For j = 1 To main.admin.count
                    If "admin(" & j & ")" = RsFormPermission!Form_name Then
                            main.admin(j).Visible = True
                    End If
                Next
               
                For j = 1 To main.InsuranceMastersubsub.count
                    If "InsuranceMastersubsub(" & j & ")" = RsFormPermission!Form_name Then
                        main.InsuranceMastersubsub(j).Visible = True
                    End If
                Next
                
               
                
                For j = 1 To main.branch.count
                    If "branch(" & j & ")" = RsFormPermission!Form_name Then
                        main.branch(j).Visible = True
                    End If
                Next
                
                For j = 1 To main.mnuPanRequest.count
                    If "mnuPanRequest(" & j & ")" = RsFormPermission!Form_name Then
                        main.mnuPanRequest(j).Visible = True
                    End If
                Next
                
                For j = 1 To main.cl.count
                    If "cl(" & j & ")" = RsFormPermission!Form_name Then
                        main.cl(j).Visible = True
                    End If
                Next
                
                
                For j = 1 To main.issop.count
                    If "issop(" & j & ")" = RsFormPermission!Form_name Then
                        main.issop(j).Visible = True
                    End If
                Next
                
                For j = 1 To main.investor.count
                    If "investor(" & j & ")" = RsFormPermission!Form_name Then
                        main.investor(j).Visible = True
                    End If
                Next
                
                For j = 1 To main.transaction.count
                    If "transaction(" & j & ")" = RsFormPermission!Form_name Then
                        main.transaction(j).Visible = True
                    End If
                Next
                
                For j = 1 To main.brokerage.count
                    If "brokerage(" & j & ")" = RsFormPermission!Form_name Then
                        main.brokerage(j).Visible = True
                    End If
                Next
                
                For j = 1 To main.rslab.count
                    If "rslab(" & j & ")" = RsFormPermission!Form_name Then
                        main.rslab(j).Visible = True
                    End If
                Next

                For j = 1 To main.transactionsub.count
                        If "transactionsub(" & j & ")" = RsFormPermission!Form_name Then
                                main.transactionsub(j).Visible = True
                        End If
                Next
                
                
                If Glbloginid = 42723 Or Glbloginid = 118739 Or Glbloginid = 38387 Then
                    main.mnuirdarep.Visible = True
                End If
                
                
                For j = 1 To main.transactionsub_new.count
                        If "transactionsub_new(" & j & ")" = RsFormPermission!Form_name Then
                                main.transactionsub_new(j).Visible = True
                        End If
                Next
                
                For j = 1 To main.Insurancereportsub.count
                    If "Insurancereportsub(" & j & ")" = RsFormPermission!Form_name Then
                            main.Insurancereportsub(j).Visible = True
                    End If
                Next
                
                main.InsuranceMastersub(32).Visible = True
                
                For j = 1 To main.InsuranceMastersub.count
                    If "InsuranceMastersub(" & j & ")" = RsFormPermission!Form_name Then
                            main.InsuranceMastersub(j).Visible = True
                    End If
                Next
              
                
                For j = 1 To main.Favorite.count
                    If "Favorite(" & j & ")" = RsFormPermission!Form_name Then
                        main.Favorite(j).Visible = True
                    End If
                Next
                
                For j = 3 To 8
                    If "FA(" & j & ")" = RsFormPermission!Form_name Then
                        main.fa(j).Visible = True
                    End If
                Next
                For j = 10 To 13
                    If "FA(" & j & ")" = RsFormPermission!Form_name Then
                        main.fa(j).Visible = True
                    End If
                Next

                For j = 1 To main.fas.count
                    If "FAS(" & j & ")" = RsFormPermission!Form_name Then
                        main.fas(j).Visible = True
                    End If
                Next
              
            
                For j = 1 To main.logout.count
                    If "logout(" & j & ")" = RsFormPermission!Form_name Then
                        main.logout(j).Visible = True
                    End If
                Next
             
                For j = 1 To main.renewal.count
                    If "Renewal(" & j & ")" = RsFormPermission!Form_name Then
                        main.renewal(j).Visible = True
                    End If
                Next
                For j = 1 To main.mnuinstool.count
                    If "mnuinstool(" & j & ")" = RsFormPermission!Form_name Then
                        main.mnuinstool(j).Visible = True
                    End If
                Next
                
                For j = 1 To main.mnuimportingSub.count
                    If "mnuimportingSub(" & j & ")" = RsFormPermission!Form_name Then
                        main.mnuimportingSub(j).Visible = True
                    End If
                Next
                
                         
                For j = 1 To main.ipoBilling.count
                    If "ipoBilling(" & j & ")" = RsFormPermission!Form_name Then
                        main.ipoBilling(j).Visible = True
                    End If
                Next
                
             
           
                
                If "mftrail" = RsFormPermission!Form_name Then
                   main.mftrail.Visible = True
                End If
                
             
                
           
                
                For j = 0 To main.venuerep.count - 1
                        If "venuerep(" & j & ")" = RsFormPermission!Form_name Then
                                main.venuerep(j).Visible = True
                        End If
                Next
               
                For j = 0 To main.renrep.count - 1
                        If "renrep(" & j & ")" = RsFormPermission!Form_name Then
                                main.renrep(j).Visible = True
                        End If
                Next
                
                If main.renrep(0).Visible = False Then
                    If Glbloginid = 95584 Then
                         main.renrep(0).Visible = True
                    End If
                End If
                
                
                For j = 0 To main.mnuadvSub.count
                        If "mnuadvSub(" & j & ")" = RsFormPermission!Form_name Then
                             main.mnuadvSub(j).Visible = True
                        End If
                Next
                
              
            End If
            RsFormPermission.MoveNext
        Next
    End If
    
End If



If Glbloginid = "39000" Then
    main.mnuadvSub(2).Visible = True
    main.mnuadvSub(3).Visible = True
End If
If Glbloginid = "93961" Then    'as per sanjiv dheer
    main.fa(2).Visible = False
End If

Call DataPermission
If LockedUser = True Then
   MsgBox "Please See Reports in Wealthmaker.in Business Module", vbInformation
   Unload Me
   Exit Sub
End If


    Unload Me
    
    frmSplash.Show
    For i = 1 To 7000
        DoEvents
    Next
    Unload frmSplash
    PicForm.Show
    
    Call ShowData
    
    DT = ServerDateTime
    mon = DatePart("m", DT)
    yr = DatePart("yyyy", DT)
    'Added for Role Default
    Set rsTran = New ADODB.Recordset
    If rsTran.State = 1 Then rsTran.Close
    rsTran.CursorLocation = adUseClient
    rsTran.open "Select role_default from datafilter_master where login_id='" & Glbloginid & "' and role_id in ('54','6','5','1','42','29') and role_default='1'", MyConn, adOpenForwardOnly, adLockReadOnly
    If Not rsTran.EOF Then
        role_default = "1"
    Else
        role_default = "0"
    End If
    'Added for Role Default
    main.Show
    
If GlbroleId = "47" Or GlbroleId = "32" Or GlbroleId = "120" Then
    main.fas(19).Visible = True
    For i = 1 To 1000
        DoEvents
    Next
End If

End Sub

Private Sub DTPicker1_KeyPress(KeyAscii As Integer)
'KeyAscii = 0
End Sub

Private Sub Form_Activate()
Clipboard.Clear
End Sub

Private Sub update_venue_nap_test()
On Error GoTo err
Dim i As Integer
i = 1
Dim rsloop As New ADODB.Recordset
rsloop.open "select distinct plan_no from bajaj_COMM_RECEIVABLE where plan_no='617' order by plan_no ", MyConn, adOpenForwardOnly
'rsloop.open "select distinct plan_no from bajaj_COMM_RECEIVABLE order by plan_no ", MyConn, adOpenForwardOnly
While rsloop.EOF = False
    Dim RS As New ADODB.Recordset
    Dim rs2 As New ADODB.Recordset
    Dim rs3 As New ADODB.Recordset
    MyConn.Execute "delete from test_venue_COMM_RECEIVABLE WHERE PLAN_NO='" & rsloop(0) & "'  "
    If RS.State = 1 Then RS.Close
    RS.open "select * from bajaj_COMM_RECEIVABLE a WHERE PLAN_NO='" & rsloop(0) & "' and comm_id='1' ORDER BY PREM_TERM_FR,PREM_AMT_TO", MyConn
    'RS.open "select * from bajaj_COMM_RECEIVABLE a WHERE PLAN_NO='" & rsloop(0) & "' and comm_id='1' and PREM_TERM_FR=5 and PREM_YR_FR=1 ORDER BY PREM_TERM_FR,PREM_AMT_TO", MyConn
    While RS.EOF = False
        If rs2.State = 1 Then rs2.Close
        If IsNull(RS("to_dt")) Then
            rs2.open "SELECT * FROM BAJAJ_COMM_RECEIVABLE WHERE PLAN_NO='" & rsloop(0) & "' AND COMM_ID<>'1' AND PREM_TERM_FR>=" & RS("PREM_TERM_FR") & " AND  PREM_TERM_TO<=" & RS("PREM_TERM_TO") & " AND PREM_AMT_FR=" & RS("PREM_AMT_FR") & " AND PREM_AMT_TO=" & RS("PREM_AMT_TO") & "  AND PREM_YR_FR=" & RS("PREM_YR_FR") & " AND PREM_YR_TO=" & RS("PREM_YR_TO") & " AND FROM_DT>='" & Format(RS("FROM_DT"), "dd-mmm-yyyy") & "' AND  to_dt is null ", MyConn, adOpenForwardOnly
            If rs2.EOF = True Then
                If rs2.State = 1 Then rs2.Close
                rs2.open "SELECT * FROM BAJAJ_COMM_RECEIVABLE WHERE PLAN_NO='" & rsloop(0) & "' AND COMM_ID<>'1' AND PREM_TERM_FR=" & RS("PREM_TERM_FR") & " AND  PREM_TERM_TO=" & RS("PREM_TERM_TO") & " AND PREM_AMT_FR=" & RS("PREM_AMT_FR") & " AND PREM_AMT_TO=" & RS("PREM_AMT_TO") & " AND PREM_YR_FR=" & RS("PREM_YR_FR") & " AND PREM_YR_TO=" & RS("PREM_YR_TO") & " AND FROM_DT<='" & Format(RS("FROM_DT"), "dd-mmm-yyyy") & "' AND TO_DT is null ", MyConn, adOpenForwardOnly
            End If
        Else
            rs2.open "SELECT * FROM BAJAJ_COMM_RECEIVABLE WHERE PLAN_NO='" & rsloop(0) & "' AND COMM_ID<>'1' AND PREM_TERM_FR>=" & RS("PREM_TERM_FR") & " AND  PREM_TERM_TO<=" & RS("PREM_TERM_TO") & " AND PREM_AMT_FR=" & RS("PREM_AMT_FR") & " AND PREM_AMT_TO=" & RS("PREM_AMT_TO") & " AND PREM_YR_FR=" & RS("PREM_YR_FR") & " AND PREM_YR_TO=" & RS("PREM_YR_TO") & " AND FROM_DT>='" & Format(RS("FROM_DT"), "dd-mmm-yyyy") & "' AND TO_DT<='" & Format(RS("TO_DT"), "dd-mmm-yyyy") & "' ", MyConn, adOpenForwardOnly
            'rs2.open "SELECT * FROM BAJAJ_COMM_RECEIVABLE WHERE PLAN_NO='" & rsloop(0) & "' AND COMM_ID<>'1' AND  " & RS("PREM_TERM_TO") & " between PREM_TERM_FR AND  PREM_TERM_TO AND PREM_AMT_FR=" & RS("PREM_AMT_FR") & " AND PREM_AMT_TO=" & RS("PREM_AMT_TO") & " AND PREM_YR_FR=" & RS("PREM_YR_FR") & " AND PREM_YR_TO=" & RS("PREM_YR_TO") & " AND FROM_DT>='" & Format(RS("FROM_DT"), "dd-mmm-yyyy") & "' AND TO_DT<='" & Format(RS("TO_DT"), "dd-mmm-yyyy") & "' ", MyConn, adOpenForwardOnly
            If rs2.EOF = True Then
                If rs2.State = 1 Then rs2.Close
                rs2.open "SELECT * FROM BAJAJ_COMM_RECEIVABLE WHERE PLAN_NO='" & rsloop(0) & "' AND COMM_ID<>'1' AND PREM_TERM_FR>=" & RS("PREM_TERM_FR") & " AND  PREM_TERM_TO<=" & RS("PREM_TERM_TO") & " AND PREM_AMT_FR=" & RS("PREM_AMT_FR") & " AND PREM_AMT_TO=" & RS("PREM_AMT_TO") & " AND PREM_YR_FR=" & RS("PREM_YR_FR") & " AND PREM_YR_TO=" & RS("PREM_YR_TO") & " AND '" & Format(RS("TO_DT"), "dd-mmm-yyyy") & "' BETWEEN FROM_DT AND TO_DT ", MyConn, adOpenForwardOnly
                'rs2.open "SELECT * FROM BAJAJ_COMM_RECEIVABLE WHERE PLAN_NO='" & rsloop(0) & "' AND COMM_ID<>'1' AND " & RS("PREM_TERM_TO") & " between PREM_TERM_FR AND  PREM_TERM_TO AND PREM_AMT_FR=" & RS("PREM_AMT_FR") & " AND PREM_AMT_TO=" & RS("PREM_AMT_TO") & " AND PREM_YR_FR=" & RS("PREM_YR_FR") & " AND PREM_YR_TO=" & RS("PREM_YR_TO") & " AND '" & Format(RS("TO_DT"), "dd-mmm-yyyy") & "' BETWEEN FROM_DT AND TO_DT ", MyConn, adOpenForwardOnly
            End If
        End If
        If rs2.RecordCount = 1 And rs2.EOF = False Then
            MyConn.Execute "insert into test_VENUE_COMM_RECEIVABLE(select * from BAJAJ_COMM_RECEIVABLE where seq_no ='" & RS("seq_no") & "') "
            MyConn.Execute "update test_VENUE_COMM_RECEIVABLE set comm_id=1,net_recieved= " & rs2("net_recieved") & " + " & RS("net_recieved") & " where seq_no ='" & RS("seq_no") & "'"
            ' code for bonus
            If rs2("PREM_TERM_TO") > 0 And rs2("comm_id") <> 2 Then
                If rs3.State = 1 Then rs3.Close
                    rs3.open "SELECT * FROM BAJAJ_COMM_RECEIVABLE WHERE PLAN_NO='" & rsloop(0) & "' AND COMM_ID='2' AND  " & RS("PREM_TERM_TO") & " between PREM_TERM_FR AND  PREM_TERM_TO AND PREM_AMT_FR=" & RS("PREM_AMT_FR") & " AND PREM_AMT_TO=" & RS("PREM_AMT_TO") & " AND PREM_YR_FR=" & RS("PREM_YR_FR") & " AND PREM_YR_TO=" & RS("PREM_YR_TO") & " AND FROM_DT>='" & Format(RS("FROM_DT"), "dd-mmm-yyyy") & "' AND TO_DT<='" & Format(RS("TO_DT"), "dd-mmm-yyyy") & "' ", MyConn, adOpenForwardOnly
                If rs3.EOF = True Then
                    If rs3.State = 1 Then rs3.Close
                    rs3.open "SELECT * FROM BAJAJ_COMM_RECEIVABLE WHERE PLAN_NO='" & rsloop(0) & "' AND COMM_ID='2' AND " & RS("PREM_TERM_TO") & " between PREM_TERM_FR AND  PREM_TERM_TO AND PREM_AMT_FR=" & RS("PREM_AMT_FR") & " AND PREM_AMT_TO=" & RS("PREM_AMT_TO") & " AND PREM_YR_FR=" & RS("PREM_YR_FR") & " AND PREM_YR_TO=" & RS("PREM_YR_TO") & " AND '" & Format(RS("TO_DT"), "dd-mmm-yyyy") & "' BETWEEN FROM_DT AND TO_DT ", MyConn, adOpenForwardOnly
                End If

                While rs3.EOF = False
                    MyConn.Execute "insert into test_VENUE_COMM_RECEIVABLE(select * from BAJAJ_COMM_RECEIVABLE where seq_no ='" & rs3("seq_no") & "') "
                    MyConn.Execute "update test_VENUE_COMM_RECEIVABLE set PREM_TERM_FR=" & rs2("PREM_TERM_FR") & ",PREM_TERM_TO=" & rs2("PREM_TERM_TO") & ",comm_id=1,FROM_DT='" & Format(rs2("FROM_DT"), "dd-mmm-yyyy") & "',TO_DT='" & Format(rs2("TO_DT"), "dd-mmm-yyyy") & "' where comm_id=2 and seq_no ='" & rs3("seq_no") & "'"
                    rs3.MoveNext
                Wend
            End If
        ElseIf rs2.RecordCount > 1 Then
            While rs2.EOF = False
            If rs2("comm_id") = 3 Then
                MyConn.Execute "insert into test_VENUE_COMM_RECEIVABLE(select * from BAJAJ_COMM_RECEIVABLE where seq_no ='" & rs2("seq_no") & "') "
                MyConn.Execute "update test_VENUE_COMM_RECEIVABLE set comm_id=1,net_recieved= " & rs2("net_recieved") & " + " & RS("net_recieved") & " where seq_no ='" & rs2("seq_no") & "'"
            ElseIf rs2("comm_id") = 4 Then
                MyConn.Execute "insert into test_VENUE_COMM_RECEIVABLE(select * from BAJAJ_COMM_RECEIVABLE where seq_no ='" & rs2("seq_no") & "') "
                MyConn.Execute "update test_VENUE_COMM_RECEIVABLE set comm_id=1 where comm_id=4 and seq_no ='" & rs2("seq_no") & "'"
            ElseIf rs2("comm_id") = 2 Then
                MyConn.Execute "insert into test_VENUE_COMM_RECEIVABLE(select * from BAJAJ_COMM_RECEIVABLE where seq_no ='" & rs2("seq_no") & "') "
                MyConn.Execute "update test_VENUE_COMM_RECEIVABLE set comm_id=1 where comm_id=2 and seq_no ='" & rs2("seq_no") & "'"
            End If
            rs2.MoveNext
            Wend
        Else
            MyConn.Execute "insert into test_VENUE_COMM_RECEIVABLE(select * from BAJAJ_COMM_RECEIVABLE where seq_no ='" & RS("seq_no") & "') "
        End If
        RS.MoveNext
    Wend
    i = i + 1
    Me.Caption = i & " / " & rsloop.RecordCount
    DoEvents
    rsloop.MoveNext
Wend
Exit Sub
err:
    MsgBox err.Description & "NAP PROC", vbInformation
    Resume
End Sub

Private Sub Form_Load()
On Error GoTo BOError
Me.Icon = LoadPicture(App.Path & "\W.ICO")

'GLBlocal_host = "bclmail.bajajcapital.com"
'GLBLOCAL_UID = "troubleticket"
'GLBLOCAL_PWD = "bajaj@123"

GLBlocal_host = "mail.bajajcapital.com"
GLBLOCAL_UID = "troubleticket"
GLBLOCAL_PWD = "bajaj@1234"

GLBglobal_host = "mail.bajajcapital.com"
GLBGLOBAL_UID = "troubleticket"
GLBGLOBAL_PWD = "bajajcap"

'For connecting the database using the API
'Live
'Set Req = New WinHttp.WinHttpRequest
'DataBasePassword = GetUserPassword("WEALTHMAKER", "LIVE_ORCL")
'
'If DataBasePassword = "" Then
'    DataBasePassword = GetUserPassword_2("WEALTHMAKER", "LIVE_ORCL")
'End If


'UAT
Set Req = New WinHttp.WinHttpRequest
DataBasePassword = GetUserPassword_UAT("WEALTHMAKER", "UAT_ORCL")

If DataBasePassword = "" Then
    DataBasePassword = GetUserPassword_2_UAT("WEALTHMAKER", "UAT_ORCL")
End If


server = ""
exeserver = ""
otherpara = ""
Login_From = ""
file = App.Path & "\mfi_server.srv"
Open file For Input As #1
Line Input #1, server
Line Input #1, expserver
If Not EOF(1) Then
   Line Input #1, otherpara
End If
Close #1
para = Split(server, "#")
If para(5) = "ORA" Then
    sql_ora = "ORA"
Else
    sql_ora = "SQL"
End If
DataBaseUser = "wealthmaker"

If App.PrevInstance = True Then
    MsgBox "An Instance of this Application is Already Running ", vbInformation
    Unload Me
    Exit Sub
End If

Set MyConn = New ADODB.Connection
With MyConn
        .ConnectionTimeout = 999999999
        If MyConn.State = 1 Then MyConn.Close
        'LIVE
        .ConnectionString = "DSN=test;UID=" & DataBaseUser & "; Pwd=" & DataBasePassword & ""
        
        'UAT
        '.ConnectionString = "DSN=testdb;UID=" & DataBaseUser & "; Pwd=" & DataBasePassword & ""
        .open
End With
MyConn.CursorLocation = adUseClient
'update_venue_nap_test


CmdOk.Enabled = False
Exit Sub
BOError:
'PrintToTrace "Error : " & Err.Description
MsgBox err.Description, vbOKOnly, "Bajaj Capital"
'PrintToTrace "Error in Module : Main   Error in Procedure : MDIForm_Load"
'PrintToTrace "Error End"
MsgBox "Please Contact BackOffice Administrator.", vbCritical, "Error..."
End
End Sub







Private Sub Label3_Click()
frmlog.Visible = False
frmforgot.Top = 0
frmforgot.Left = 75
frmforgot.Visible = True
Text3.SetFocus
Text3.Text = text1.Text
mskforgot.Text = "__/__/____"
Me.Caption = "Wealthmaker- Forgot Password"
End Sub

Private Sub Label6_Click()
frmforgot.Visible = False
frmlog.Visible = True
text1.SetFocus
End Sub

Private Sub mskforgot_GotFocus()
If Trim(Text3.Text) = "" Then
    MsgBox "Please Enter User Name", vbInformation
    Text3.SetFocus
End If
End Sub

Private Sub text1_GotFocus()
Me.Caption = "Wealthmaker- Login"
frmforgot.Visible = False
CmdOk.Enabled = False
If Len(Trim(text1.Text)) > 0 Then
    If Len(Trim(text2.Text)) > 0 Then
        lblstatus.Caption = ""
        Call RolePermission
    Else
        text2.SetFocus
        lblstatus.Caption = "Password enter"
    End If
Else
    text1.SetFocus
End If
End Sub
Private Sub Text1_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
    text2.SetFocus
End If
If KeyAscii = 39 Then KeyAscii = 146
End Sub

Private Sub text1_LostFocus()
On Error Resume Next
CmdOk.Enabled = False
If Len(Trim(text1.Text)) > 0 Then
    If Len(Trim(text2.Text)) > 0 Then
        lblstatus.Caption = ""
        Call RolePermission
    Else
        text2.SetFocus
        lblstatus.Caption = "Password enter"
    End If
End If
End Sub
Private Sub text2_GotFocus()
Clipboard.Clear
If GetKeyState(vbKeyCapital) = 1 Then
    MsgBox "Caps lock is on", vbInformation
End If
Me.Caption = "Wealthmaker- Login"
CmdOk.Enabled = False
If Len(Trim(text1.Text)) > 0 Then
    If Len(Trim(text2.Text)) > 0 Then
        lblstatus.Caption = ""
        Call RolePermission
    Else
        text2.SetFocus
        lblstatus.Caption = "Password enter"
    End If
Else
    MsgBox "Please Enter User Name", vbInformation
    text1.SetFocus
End If
End Sub

Private Sub text2_KeyDown(KeyCode As Integer, Shift As Integer)
Clipboard.Clear
End Sub

Private Sub Text2_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
    cboroleName.SetFocus
    SendKeys "{TAB}"
End If
If KeyAscii = 39 Then KeyAscii = 146
End Sub
Private Sub text2_LostFocus()
Set Req = New WinHttp.WinHttpRequest
If Len(Trim(text1.Text)) > 0 Then
    If Len(Trim(text2.Text)) > 0 Then
        lblstatus.Caption = ""
        UserPassword = GetRMPassword(text1.Text, text2.Text)
        Call RolePermission
    Else
        lblstatus.Caption = "Password enter"
    End If
Else
    text1.SetFocus
End If
End Sub
Function FormPermissionLevel(ByVal Fname As Object)
        Fname.Visible = True
End Function
Sub MainMenuFalse()
On Error Resume Next
        main.MASTER(2).Visible = False
        main.MASTER(3).Visible = False
        main.MASTER(4).Visible = False
      
        main.MASTER(7).Visible = False
        main.MASTER(8).Visible = False
        main.MASTER(13).Visible = False
        main.MASTER(15).Visible = False
        main.MASTER(16).Visible = False
        main.MASTER(17).Visible = False
        main.MASTER(18).Visible = False

        
        For i = 1 To main.admin.count
            main.admin(i).Visible = False
        Next
        For i = 1 To main.branch.count
            main.branch(i).Visible = False
        Next

        
        For i = 1 To main.cl.count
            main.cl(i).Visible = False
        Next
        For i = 1 To main.issop.count
            main.issop(i).Visible = False
        Next
        For i = 1 To main.investor.count
            main.investor(i).Visible = False
        Next
        
        For i = 1 To main.rslab.count
            main.rslab(i).Visible = False
        Next
        
        main.brokerage(2).Visible = False
        main.brokerage(4).Visible = False
        main.brokerage(6).Visible = False
        main.brokerage(7).Visible = False
        main.brokerage(9).Visible = False
       
      
        For i = 1 To main.transaction.count
         If i = 1 Or i = 17 Or i = 18 Or i = 19 Or i = 3 Or i = 2 Then
            main.transaction(i).Visible = False
         End If
        Next
        
       
        For i = 1 To main.Favorite.count
            main.Favorite(i).Visible = False
        Next
        
        For i = 1 To main.transactionsub.count
            main.transactionsub(i).Visible = False
        Next
        For i = 1 To main.Insurancereportsub.count
            main.Insurancereportsub(i).Visible = False
        Next
        For i = 1 To main.InsuranceMastersub.count
            main.InsuranceMastersub(i).Visible = False
            Debug.Print i
        Next
        
        
        main.fa(3).Visible = False
        main.fa(4).Visible = False
        main.fa(5).Visible = False
        main.mnuadvSub(1).Visible = False
        
        For i = 1 To main.logout.count
            main.logout(i).Visible = False
        Next
End Sub


Private Sub cmdSend_Click(ByVal Email, ByVal strmsg)
On Error GoTo err
    Set poSendMail = New clsSendMail

    With poSendMail

        .SMTPHostValidation = VALIDATE_NONE         ' Optional, default = VALIDATE_HOST_DNS
        .EmailAddressValidation = VALIDATE_SYNTAX   ' Optional, default = VALIDATE_SYNTAX
        .Delimiter = ";"                            ' Optional, default = ";" (semicolon)
        ' **************************************************************************
        ' Basic properties for sending email
        ' **************************************************************************
        .SMTPHost = GLBlocal_host                 ' Required the fist time, optional thereafter
        .From = "wealthmakerIPC@bajajcapital.com"          ' Required the fist time, optional thereafter
        .FromDisplayName = "Wealthmaker"                 ' Optional, saved after first use
        .Recipient = Email     ' Required, separate multiple entries with delimiter character
        .RecipientDisplayName = Email    ' Optional, separate multiple entries with delimiter character
'        .BccRecipient = txtBcc                      ' Optional, separate multiple entries with delimiter character
'        .ReplyToAddress = txtfrom.Text              ' Optional, used when different than 'From' address
        .Subject = "Forgot Password Request"                          ' Optional
        
        .Message = strmsg                             ' Optional
'        .Attachment = Trim(txtAttach.Text)          ' Optional, separate multiple entries with delimiter character

        ' **************************************************************************
        ' Additional Optional properties, use as required by your application / environment
        ' **************************************************************************
        .AsHTML = True                             ' Optional, default = FALSE, send mail as html or plain text
        .ContentBase = ""                           ' Optional, default = Null String, reference base for embedded links
        .EncodeType = MyEncodeType                  ' Optional, default = MIME_ENCODE
        .Priority = etPriority                      ' Optional, default = PRIORITY_NORMAL
        .Receipt = False                         ' Optional, default = FALSE
        .UseAuthentication = True             ' Optional, default = FALSE
        .UsePopAuthentication = True           ' Optional, default = FALSE
        .UserName = GLBLOCAL_UID             ' Optional, default = Null String
        .Password = GLBLOCAL_PWD                     ' Optional, default = Null String, value is NOT saved
        .POP3Host = "mail.bajajcapital.com"
        .MaxRecipients = 100                        ' Optional, default = 100, recipient count before error is raised
        
        ' **************************************************************************
        ' Advanced Properties, change only if you have a good reason to do so.
        ' **************************************************************************
        ' .ConnectTimeout = 10                      ' Optional, default = 10
        ' .ConnectRetry = 5                         ' Optional, default = 5
        ' .MessageTimeout = 60                      ' Optional, default = 60
        ' .PersistentSettings = True                ' Optional, default = TRUE
         .SMTPPort = 25                            ' Optional, default = 25

        ' **************************************************************************
        ' OK, all of the properties are set, send the email...
        ' **************************************************************************
        ' .Connect                                  ' Optional, use when sending bulk mail
        .send                                       ' Required
        .Disconnect                               ' Optional, use when sending bulk mail
    End With
Exit Sub
err:
    MsgBox err.Description & err.Number
End Sub

Private Sub text2_MouseDown(Button As Integer, Shift As Integer, X As Single, Y As Single)
Clipboard.Clear
End Sub
