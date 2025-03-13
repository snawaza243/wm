Private Sub CmdNonEcsTran_Click()
frmnpsecsimp.Show
frmnpsecsimp.ZOrder
frmnpsecsimp.Label2 = "NPS Corporate NON ECS Transaction Importing"
Nps_Importing_flag = "NON ECS"

If ChkZeroCommission.Value = 1 Then
    VChkZeroCommission = "Y"
Else
    VChkZeroCommission = "N"
End If
End Sub

Private Sub CmdEcsTran_Click()
    ChkZeroCommission.Value = 0
    frmnpsecsimp.Show
    frmnpsecsimp.ZOrder
    frmnpsecsimp.Label2 = "NPS ECS Transaction Importing"
    Nps_Importing_flag = "ECS"
End Sub