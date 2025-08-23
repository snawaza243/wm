Private Sub CmbFrequency1_Click()

ImSipEndDtM.Text = "__/__/____"
'MyFreq1 = ""

MaxInstallmentNo = SqlRet("select round(months_between(to_date('01/12/2099','DD/MM/RRRR'),to_date('" & ImSipStartDtA.Text & "','DD/MM/RRRR'))) from dual")

If Opt991.Value = True And ImSipStartDtM.Text <> "__/__/____" Then
            ImSipEndDtM.Text = "01/12/2099"
End If


If CmbFrequency1.ListIndex <> -1 And Opt991.Value = False Then
    MyFreq1 = Split(CmbFrequency1.Text, "#")
    If MyFreq1(1) = "208" Then
        If (Txtinstallments1.Text <> "" Or Opt991.Value = True) And ImSipStartDtM.Text <> "__/__/____" Then
            ImSipEndDtM.Text = SqlRet("select add_months(to_date('" & ImSipStartDtM.Text & "','DD/MM/RRRR'), to_number(" & Txtinstallments1.Text & "/30))-1 as enddt from dual ")
        End If
    ElseIf MyFreq1(1) = "173" Then
        If (Txtinstallments1.Text <> "" Or Opt991.Value = True) And ImSipStartDtM.Text <> "__/__/____" Then
            ImSipEndDtM.Text = SqlRet("select to_date(add_months(to_date('" & ImSipStartDtM.Text & "','DD/MM/RRRR'), to_number(" & Txtinstallments1.Text & "/4))-1) as enddt  from dual ")
        End If
    ElseIf MyFreq1(1) = "174" Then
        If (Txtinstallments1.Text <> "" Or Opt991.Value = True) And ImSipStartDtM.Text <> "__/__/____" Then
            ImSipEndDtM.Text = SqlRet("select add_months(to_date('" & ImSipStartDtM.Text & "','DD/MM/RRRR'),6* " & Txtinstallments1.Text & ")-1 as enddt from dual ")
        End If
    ElseIf MyFreq1(1) = "175" Then
'        If (Txtinstallments1.Text <> "" Or Opt991.Value = True) And ImSipStartDtM.Text <> "__/__/____" Then
'            ImSipEndDtM.Text = SqlRet("select add_months(to_date('" & ImSipStartDtM.Text & "','DD/MM/RRRR'),1* " & Txtinstallments1.Text & ")-1  as enddt from dual ")
'        End If
        
        If (Txtinstallments1.Text <> "" Or Opt991.Value = True) And ImSipStartDtM.Text <> "__/__/____" Then
            If Val(Txtinstallments1.Text) < MaxInstallmentNo Then
                ImSipEndDtM.Text = SqlRet("select add_months(to_date('" & ImSipStartDtM.Text & "','DD/MM/RRRR'),1* " & Txtinstallments1.Text & ")-1  as enddt from dual ")
            Else
                ImSipEndDtM.Text = "01/12/2099"
            End If
        End If
    ElseIf MyFreq1(1) = "176" Then
        If (Txtinstallments1.Text <> "" Or Opt991.Value = True) And ImSipStartDtM.Text <> "__/__/____" Then
            ImSipEndDtM.Text = SqlRet("select add_months(to_date('" & ImSipStartDtM.Text & "','DD/MM/RRRR'),3* " & Txtinstallments1.Text & ")-1  as enddt from dual ")
        End If
    ElseIf MyFreq1(1) = "301" Then
        If (Txtinstallments1.Text <> "" Or Opt991.Value = True) And ImSipStartDtM.Text <> "__/__/____" Then
            ImSipEndDtM.Text = SqlRet("select add_months(to_date('" & ImSipStartDtM.Text & "','DD/MM/RRRR'),12* " & Txtinstallments1.Text & ")-1  as enddt from dual ")
        End If
    End If
End If

 
End Sub


