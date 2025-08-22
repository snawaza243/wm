Private Sub cmdMapPMS_Click()
If MyTrCode = "" Then
    MsgBox "First Double Click The Record , You Want To Map", vbInformation
    Exit Sub
End If

'If Len(MasterId) = 0 Then
'    MsgBox "First Select The Record Of Sip MAster , To Which You Want To Map", vbInformation
'    Exit Sub
'End If
'
'-------------------------------------------------------------------------------------------------
If MyDispatch = "N" Then
    MyConn.Execute ("Update Transaction_mf_temp1 set  REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "' WHERE TRAN_CODE='" & MyTrCode & "'")
    MyConn.Execute ("Update Transaction_mf_temp1 set REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "' WHERE BASE_TRAN_CODE='" & MyTrCode & "'")
    'MyConn.Execute ("Update Transaction_st@MF.BAJAJCAPITAL set REC_FLAG='Y',HO_TRAN_CODE='" & MyTrCode & "' where tran_code in (" & MyRtaTrCode & ")")
Else
    MyConn.Execute ("Update Transaction_mf_temp1 set  REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "' WHERE TRAN_CODE='" & MyTrCode & "'")
    MyConn.Execute ("Update Transaction_mf_temp1 set  REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "' WHERE BASE_TRAN_CODE='" & MyTrCode & "'")
    'MyConn.Execute ("Update Transaction_st@MF.BAJAJCAPITAL set REC_FLAG='Y',HO_TRAN_CODE='" & MyTrCode & "' where tran_code in (" & MyRtaTrCode & ")")
End If
'--------------------------------------------------------------------------------------------------
'MyConn.Execute ("Update Transaction_mf_temp1@test.bajajcapital set  REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "' WHERE TRAN_CODE='" & MyTrCode & "'")
'MyConn.Execute ("Update  Transaction_mf_temp1@test.bajajcapital set  REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "' WHERE BASE_TRAN_CODE='" & MyTrCode & "'")
'MyConn.Execute ("Update TRAN_SIP_FEED@TEST.BAJAJCAPITAL SET REC_USER='" & Glbloginid & "' ,BASE_TRAN_CODE='" & MyTrCode & "',REC_DATE=TO_DATE(SYSDATE) WHERE SEQ_NO=" & MasterId & "")
MsgBox "Base SIP Registration Confirmed Sucessfully", vbInformation
End Sub


