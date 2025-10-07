Public Function DataPermission()
On Error Resume Next
       Set RsData = New ADODB.Recordset
       RsData.CursorLocation = adUseClient
       RsData.open "select * from role_master where role_id='" & Trim(GlbroleId) & "'", MyConn
       If RsData.RecordCount > 0 Then
            GlbUpdatePre = Trim(RsData!up_pre_dur)
            Glbupdatenext = Trim(RsData!up_next_dur)
            GlbINSPre = Trim(RsData!in_pre_dur)
            GlbINSnext = Trim(RsData!in_next_dur)
            
            GlbUpdatePreType = Trim(RsData!up_pre_dur_type)
            GlbupdatenextType = Trim(RsData!up_next_dur_type)
            GlbINSPreType = Trim(RsData!in_pre_dur_type)
            GlbINSnextType = Trim(RsData!in_next_dur_type)
            Call DataFilterWithDatesforINs(Val(GlbINSPre), Val(GlbINSPreType), Val(GlbINSnext), Val(GlbINSnextType))
            Call DataFilterWithDatesforUPd(Val(GlbUpdatePre), Val(GlbUpdatePreType), Val(Glbupdatenext), Val(GlbupdatenextType))
            Set rsRole = New ADODB.Recordset
            rsRole.CursorLocation = adUseClient
            rsRole.open "select data_filter from datafilter_master where role_id='" & Trim(GlbroleId) & "' and login_id='" & Trim(Glbloginid) & "'", MyConn
            If rsRole.RecordCount > 0 Then
                GlbDataFilter = Trim(rsRole!data_filter)
            End If
            rsRole.Close
            '------------------------------------To Lock The User For Transaction------------------------------
            Set rsRole = New ADODB.Recordset
            rsRole.CursorLocation = adUseClient
            rsRole.open "select * from USER_LOCKED where userid='" & Trim(Glbloginid) & "'", MyConn
            If Not rsRole.EOF Then
                LockedUser = True
            End If
            rsRole.Close

            Set rsRole = New ADODB.Recordset
            rsRole.CursorLocation = adUseClient
            rsRole.open "select BLOCKED from RDC_BLOCK WHERE MODULE='LI'", MyConn, adOpenForwardOnly
            If rsRole("blocked") = "Y" Then
                LockedAllLI = True
            Else
                LockedAllLI = False
            End If
            rsRole.Close
            
       End If
End Function