Private Sub CmbFrequency_Click()
'vinit 31-oct-2015
ImSipEndDtA.Text = "__/__/____"

MaxInstallmentNo = SqlRet("select round(months_between(to_date('01/12/2099','DD/MM/RRRR'),to_date('" & ImSipStartDtA.Text & "','DD/MM/RRRR'))) from dual")

If Opt99.Value = True And ImSipStartDtA.Text <> "__/__/____" Then
     ImSipEndDtA.Text = "01/12/2099"
End If
        
If CmbFrequency.ListIndex <> -1 And Opt99.Value = False Then
    MyFreq = Split(CmbFrequency.Text, "#")
    If MyFreq(1) = "208" Then
        If (TxtInstallmens.Text <> "" Or Opt99.Value = True) And ImSipStartDtA.Text <> "__/__/____" Then
            ImSipEndDtA.Text = SqlRet("select add_months(to_date('" & ImSipStartDtA.Text & "','DD/MM/RRRR'), to_number(" & TxtInstallmens.Text & "/30))-1 as enddt from dual ")
        End If
    ElseIf MyFreq(1) = "173" Then
        If (TxtInstallmens.Text <> "" Or Opt99.Value = True) And ImSipStartDtA.Text <> "__/__/____" Then
            ImSipEndDtA.Text = SqlRet("select WEALTHMAKER.add_weeks(to_date('" & ImSipStartDtA.Text & "','DD/MM/RRRR'), to_number(" & TxtInstallmens.Text & ")) as enddt  from dual ")
        End If
    ElseIf MyFreq(1) = "174" Then
        If (TxtInstallmens.Text <> "" Or Opt99.Value = True) And ImSipStartDtA.Text <> "__/__/____" Then
            ImSipEndDtA.Text = SqlRet("select add_months(to_date('" & ImSipStartDtA.Text & "','DD/MM/RRRR'),6* " & TxtInstallmens.Text & ")-1 as enddt from dual ")
        End If
    ElseIf MyFreq(1) = "175" Then
        If (TxtInstallmens.Text <> "" Or Opt99.Value = True) And ImSipStartDtA.Text <> "__/__/____" Then
            If Val(TxtInstallmens.Text) < MaxInstallmentNo Then
                ImSipEndDtA.Text = SqlRet("select add_months(to_date('" & ImSipStartDtA.Text & "','DD/MM/RRRR'),1* " & TxtInstallmens.Text & ")-1  as enddt from dual ")
            Else
                ImSipEndDtA.Text = "01/12/2099"
            End If
        End If
    ElseIf MyFreq(1) = "176" Then
        If (TxtInstallmens.Text <> "" Or Opt99.Value = True) And ImSipStartDtA.Text <> "__/__/____" Then
            ImSipEndDtA.Text = SqlRet("select add_months(to_date('" & ImSipStartDtA.Text & "','DD/MM/RRRR'),3* " & TxtInstallmens.Text & ")-1  as enddt from dual ")
        End If
    ElseIf MyFreq(1) = "301" Then
        If (TxtInstallmens.Text <> "" Or Opt99.Value = True) And ImSipStartDtA.Text <> "__/__/____" Then
            ImSipEndDtA.Text = SqlRet("select add_months(to_date('" & ImSipStartDtA.Text & "','DD/MM/RRRR'),12* " & TxtInstallmens.Text & ")-1  as enddt from dual ")
        End If
    End If
End If
End Sub

