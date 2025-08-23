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


