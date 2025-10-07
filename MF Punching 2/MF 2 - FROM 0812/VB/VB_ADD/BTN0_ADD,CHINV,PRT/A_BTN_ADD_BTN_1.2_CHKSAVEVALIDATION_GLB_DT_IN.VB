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
