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
