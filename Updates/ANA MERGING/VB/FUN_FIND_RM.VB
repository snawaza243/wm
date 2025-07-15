Private Sub Find_RM()
Dim rsTran As New ADODB.Recordset
Dim rsClient_CD As New ADODB.Recordset
Dim i As Integer
    CLIENT_CD = ""
    If msfgMain.TextMatrix(1, 0) <> "0" Then CLIENT_CD = msfgMain.TextMatrix(1, 2) & ","
    For i = 1 To msfgMergedInvestors.Rows - 1
        If msfgMergedInvestors.TextMatrix(i, 0) <> "0" Then CLIENT_CD = CLIENT_CD & msfgMergedInvestors.TextMatrix(i, 2) & ","
    Next i
    If CLIENT_CD <> "" Then
        CLIENT_CD = Left(CLIENT_CD, Len(CLIENT_CD) - 1)
    End If
    If CLIENT_CD <> "" Then
        rsTran.open "select max(tr_date) tr_date,source_code from( " _
                        & "select max(tr_date) tr_date,source_code from transaction_sttemp where source_code in(" & CLIENT_CD & ") " _
                           & "group by source_code " _
                        & " Union All " _
                        & "select max(tr_date) tr_date,source_code from transaction_st where  source_code in(" & CLIENT_CD & ") " _
                           & "group by source_code " _
                        & ") " _
                    & "group by source_code " _
                    & "order by tr_date desc ", MyConn, adOpenForwardOnly
        If Not (rsTran.EOF) Then rsTran.MoveFirst
        branch_cd = "10010226"
        While Not (rsTran.EOF) And branch_cd = "10010226"
            rsClient_CD.open "Select rm_code,sourceid from agent_master  where agent_code=" & rsTran("source_code"), MyConn, adOpenForwardOnly
            branch_cd = rsClient_CD("sourceid")
            Rm_cd = rsClient_CD("rm_code")
            rsClient_CD.Close
            rsTran.MoveNext
        Wend


    End If
    If branch_cd = "" Or Rm_cd = "" Or branch_cd = "10010226" Then
        rsClient_CD.open "Select sourceid,rm_code from agent_master where agent_code=" & msfgMain.TextMatrix(1, 2), MyConn, adOpenForwardOnly
        branch_cd = rsClient_CD("sourceid")
        Rm_cd = rsClient_CD("rm_code")
        rsClient_CD.Close
    End If
    
    If rsTran.State = 1 Then rsTran.Close
    Set rsTran = Nothing
    Set rsClient_CD = Nothing
    
End Sub


Private Sub Find_RM()
Dim rsTran As New ADODB.Recordset
Dim rsClient_CD As New ADODB.Recordset
Dim i As Integer
    CLIENT_CD = ""
    If msfgMain.TextMatrix(1, 0) <> "0" Then CLIENT_CD = msfgMain.TextMatrix(1, 2) & ","
    For i = 1 To msfgMergedInvestors.Rows - 1
        If msfgMergedInvestors.TextMatrix(i, 0) <> "0" Then CLIENT_CD = CLIENT_CD & msfgMergedInvestors.TextMatrix(i, 2) & ","
    Next i
    If CLIENT_CD <> "" Then
        CLIENT_CD = Left(CLIENT_CD, Len(CLIENT_CD) - 1)
    End If
    If CLIENT_CD <> "" Then
        rsTran.open "select max(tr_date) tr_date,source_code from( " _
                        & "select max(tr_date) tr_date,source_code from transaction_sttemp where source_code in(" & CLIENT_CD & ") " _
                           & "group by source_code " _
                        & " Union All " _
                        & "select max(tr_date) tr_date,source_code from transaction_st where  source_code in(" & CLIENT_CD & ") " _
                           & "group by source_code " _
                        & ") " _
                    & "group by source_code " _
                    & "order by tr_date desc ", MyConn, adOpenForwardOnly
                    
'        If Not (rsTran.EOF) Then
'            rsTran.MoveFirst
'            rsClient_CD.open "Select rm_code,sourceid from client_master where client_code=" & rsTran("source_code"), myconn, adOpenForwardOnly
'            Branch_cd = rsClient_CD("sourceid")
'            Rm_cd = rsClient_CD("rm_code")
'            rsClient_CD.Close
'        End If

        If Not (rsTran.EOF) Then rsTran.MoveFirst
        branch_cd = "10010226"
        While Not (rsTran.EOF) And branch_cd = "10010226"
            rsClient_CD.open "Select rm_code,sourceid from agent_master  where agent_code=" & rsTran("source_code"), MyConn, adOpenForwardOnly
            branch_cd = rsClient_CD("sourceid")
            Rm_cd = rsClient_CD("rm_code")
            rsClient_CD.Close
            rsTran.MoveNext
        Wend


    End If
    If branch_cd = "" Or Rm_cd = "" Or branch_cd = "10010226" Then
        rsClient_CD.open "Select sourceid,rm_code from agent_master where agent_code=" & msfgMain.TextMatrix(1, 2), MyConn, adOpenForwardOnly
        branch_cd = rsClient_CD("sourceid")
        Rm_cd = rsClient_CD("rm_code")
        rsClient_CD.Close
    End If
    
    If rsTran.State = 1 Then rsTran.Close
    Set rsTran = Nothing
    Set rsClient_CD = Nothing
    
End Sub


