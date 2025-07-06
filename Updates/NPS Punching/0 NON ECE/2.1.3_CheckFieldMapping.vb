Private Function CheckFieldMapping(data_Base_Fld As String, statusCode As String) As Boolean
CheckFieldMapping = False
If Comp_Cd = "D" Or Comp_Cd = "A" Or Comp_Cd = "B" Then
    If InStr(1, UCase(data_Base_Fld), "STATUS") <= 0 Then
        MsgBox "Status is Not Mapped", vbCritical
        Exit Function
    End If
    If InStr(1, UCase(data_Base_Fld), "REASON") <= 0 Then
        MsgBox "Reason is Not Mapped", vbCritical
        Exit Function
    End If
    If Comp_Cd = "A" Then
        If InStr(1, UCase(data_Base_Fld), "POLICY_ISSUE_DT") <= 0 Then
            MsgBox "POLICY ISSUE DATE is Not Mapped", vbCritical
            Exit Function
        End If
        If InStr(1, UCase(data_Base_Fld), "DOC") Then
            MsgBox "DOC is Not Mapped", vbCritical
            Exit Function
        End If
    ElseIf Comp_Cd = "D" Then
        If InStr(1, UCase(data_Base_Fld), "Login_DT") <= 0 Then
            MsgBox "LoginDate is Not Mapped", vbCritical
            Exit Function
        End If
    End If
End If
If Comp_Cd = "OM KOTAK" Or Comp_Cd = "SBI LIFE" Then
    If InStr(1, UCase(data_Base_Fld), "POLICY_NO") <= 0 Then
        MsgBox "POLICY_NO is Not Mapped", vbCritical
        Exit Function
    End If
    If InStr(1, UCase(data_Base_Fld), "CHEQUE_NO") <= 0 Then
        'MsgBox "Cheque_No is Not Mapped", vbCritical
       ' Exit Function
    End If
    If InStr(1, UCase(data_Base_Fld), "PREMIUM") <= 0 Then
        MsgBox "PREMIUM is Not Mapped", vbCritical
        Exit Function
    End If
ElseIf Comp_Cd = "TATAAIG" Then
    If InStr(1, UCase(data_Base_Fld), "POLICY_NO") <= 0 Then
        MsgBox "POLICY_NO is Not Mapped", vbCritical
        Exit Function
    End If
ElseIf Comp_Cd = "RELLIFE" Or Comp_Cd = "ICICI" Or Comp_Cd = "AVIVA" Or Comp_Cd = "BIRLA SUN" Or Comp_Cd = "BHT A" Then
    If InStr(1, UCase(data_Base_Fld), "POLICY_NO") <= 0 Then
        MsgBox "POLICY_NO is Not Mapped", vbCritical
        Exit Function
    End If
    If InStr(1, UCase(data_Base_Fld), "APP_NO") <= 0 Then
        MsgBox "Application No is Not Mapped", vbCritical
        Exit Function
    End If
ElseIf Comp_Cd = "BAJAJ A" Then
    If InStr(1, UCase(data_Base_Fld), "POLICY_NO") <= 0 Then
        MsgBox "POLICY_NO is Not Mapped", vbCritical
        Exit Function
    End If
    If InStr(1, UCase(data_Base_Fld), "CHEQUE_NO") <= 0 Then
       ' MsgBox "Cheque_No is Not Mapped", vbCritical
        'Exit Function
    End If
    If InStr(1, UCase(data_Base_Fld), "APP_NO") <= 0 Then
        MsgBox "Application No is Not Mapped", vbCritical
        Exit Function
    End If
    '------------------mayank gi status update--------------------
ElseIf OptGeneral.Value = True Then
    If InStr(1, UCase(data_Base_Fld), "POLICY_NO") <= 0 Then
        MsgBox "POLICY_NO is Not Mapped", vbCritical
        Exit Function
    End If
    If InStr(1, UCase(data_Base_Fld), "LOGIN_DT") <= 0 Then
        MsgBox "Policy Issue Dt is Not Mapped", vbCritical
        Exit Function
    End If
    If InStr(1, UCase(data_Base_Fld), "APP_NO") <= 0 Then
        MsgBox "Application No is Not Mapped", vbCritical
        Exit Function
    End If
ElseIf OptNPS.Value = True Then
    If InStr(1, UCase(data_Base_Fld), "REF_TRAN_CODE") <= 0 Then
        MsgBox "Consumer Code is Not Mapped", vbCritical
        Exit Function
    End If
    If InStr(1, UCase(data_Base_Fld), "TR_DATE") <= 0 Then
        MsgBox "Transaction Date is Not Mapped", vbCritical
        Exit Function
    End If
    If InStr(1, UCase(data_Base_Fld), "ECS_AMT") <= 0 Then
        MsgBox "Amount is Not Mapped", vbCritical
        Exit Function
    End If
End If
CheckFieldMapping = True
End Function


