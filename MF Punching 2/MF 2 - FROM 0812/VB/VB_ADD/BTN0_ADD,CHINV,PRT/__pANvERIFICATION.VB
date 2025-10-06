'''''''''''''''''''''''''''''Cross Checking Of Pan With Account Master''''''''''''''''''''''''''''
If (CmbSipStpA.Text = "SIP") And CmbSubSipA.Text = "MICROSIP" Then
Else
    
    '--------------micro pan validate vinit 20 dec 2014    
    If MySchCode <> "OP#10826" Then
        If micropanflag = 0 Then
            If TxtPanVarify.Text = "" Or ValidatePan(TxtPanA.Text) = False Then
                If ValidatePan(TxtPanVarify.Text) = False Then
                   MsgBox "Please Either Enter a Valid PAN Number", vbCritical, "Pan Entry"
                   TxtPanVarify.SetFocus
                   Exit Sub
                End If
            End If
        End If
    End If
    
    

    If MySchCode <> "OP#10826" Then    
        If Left(Label32.Caption, 1) = "3" Then
            '-----------------------------CHECK IT IS MINOR OR NOT------------------------------------
            If Label32.Caption <> "" Then
                dob = SqlRet("select NVL(dob,TO_DATE(SYSDATE)-10000) from INVESTOR_MASTER where INV_CODE=" & Label32.Caption & "")
                If Format(dob, "dd/mm/yyyy") <> "__/__/____" And IsDate(dob) = True Then
                        If DateDiff("YYYY", Format(dob, "DD/MM/YYYY"), ServerDateTime) <= 18 Then
                            IsMinor = True
                        Else
                            IsMinor = False
                        End If
                End If
            End If
            '------------------------------------------------------------------------------------------
            If MySchCode <> "OP#10826" Then                
                    If IsMinor = False Then
                    '--------------micro pan validate vinit 20 dec 2014            
                        If micropanflag = 0 Then
                            PAN1 = ""
                            PAN1 = SqlRet("select UPPER(PAN) from investor_master where INV_CODE='" & Label32.Caption & "'")
                            If PAN1 <> "" And PAN1 <> "0" Then
                                If PAN1 <> UCase(TxtPanVarify.Text) Then
                                    MsgBox "PAN Entered Did Not Match With The Selected Investor's PAN ", vbCritical
                                    Exit Sub
                                End If
                            End If
                        End If
                    Else
                    End If
            End If        
        End If
    End If
    
    If MySchCode <> "OP#10826" Then
        If Left(Label32.Caption, 1) = "4" Then
            Dim AccountPan As Integer
            'Dim inv_cd As String
            AccountPan = 0
            inv_cd = ""
            IsMinor = False
            Category = ""
            '-----------------------------CHECK IT IS MINOR OR NOT------------------------------------
            If Label32.Caption <> "" Then
                dob = SqlRet("select nvl(dob,to_date('01/01/1800','mm/dd/yyyy')) from client_test where client_codekyc=" & Label32.Caption & "")
                If dob = "01/01/1800" Then
                    MsgBox "Please fill Date of Birth for this investor"
                    Exit Sub
                End If
                Category = SqlRet("select nvl(investor_code,1) from client_master where client_code=" & Mid(Label32.Caption, 1, 8) & "")
                If Format(dob, "dd/mm/yyyy") <> "__/__/____" And IsDate(dob) = True Then
                        If DateDiff("YYYY", Format(dob, "DD/MM/YYYY"), ServerDateTime) <= 18 Then
                            If Category = 1 Then
                                IsMinor = True
                            Else
                                IsMinor = False
                            End If
                        Else
                            IsMinor = False
                        End If
                End If
            End If
            '------------------------------------------------------------------------------------------
            If IsMinor = False Then            
                '--------------micro pan validate vinit 20 dec 2014            
                If micropanflag = 0 Then            
                    inv_cd = SqlRet("select client_codekyc from client_test where upper(CLIENT_PAN)='" & UCase(TxtPanVarify.Text) & "'")
                    If inv_cd <> "" And inv_cd <> "0" Then
                        If inv_cd <> Label32.Caption Then
                            MsgBox "PAN Entered Did Not Match With The Selected Investor's PAN", vbCritical
                            Exit Sub
                        End If
                    End If                
                    AccountPan = SqlRet("select NVL(COUNT(*),0) from client_test where client_codekyc='" & Label32.Caption & "' AND (upper(CLIENT_PAN)='" & UCase(TxtPanVarify.Text) & "' OR CLIENT_PAN IS NULL)")
                    If AccountPan = 0 Then
                        MsgBox "PAN Entered Did Not Match With The Selected Investor's PAN", vbCritical
                        Exit Sub
                    End If
                End If
            Else     
                '--------------micro pan validate vinit 20 dec 2014            
                If micropanflag = 0 Then
                    PAN1 = ""
                    PAN1 = SqlRet("select NVL(upper(g_pan),0) from client_test where client_codekyc='" & Label32.Caption & "'")
                    If PAN1 <> "" And PAN1 <> "0" Then
                        If PAN1 <> UCase(TxtPanVarify.Text) Then
                            MsgBox "PAN Entered Did Not Match With The Selected Investor's PAN", vbCritical
                            Exit Sub
                        End If
                    End If
                    
                    AccountPan = SqlRet("select NVL(COUNT(*),0) from client_test where client_codekyc='" & Label32.Caption & "' AND (upper(G_PAN)='" & UCase(TxtPanVarify.Text) & "' OR G_PAN IS NULL)")
                    If AccountPan = 0 Then
                        MsgBox "PAN Entered Did Not Match With The Selected Investor's PAN ", vbCritical
                        Exit Sub
                    End If
                End If
            End If
        End If
        
        If TxtPanA.Text = "" Or ValidatePan(TxtPanA.Text) = False Then
           TxtPanA.Text = TxtPanVarify.Text
        End If
        
        End If
    End If
'''''''''''''''''''''''''''''end Cross Checking Of Pan With Account Master''''''''''''''''''''''''
