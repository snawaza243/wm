Private Sub cmbTranTypeM_Change()
   ImSipStartDtA.Text = Format(Now, "DD/MM/YYYY")
   ImSipEndDtM.Text = "__/__/____"
   CmbFrequency1.ListIndex = -1
    Opt991.Value = False
If cmbTranTypeM.Text = "PURCHASE" Then
   Frame3.Visible = True
   Frame12.Visible = False
   OptChkSwitchM.Visible = False
   
   
   
ElseIf cmbTranTypeM.Text = "SWITCH IN" Then
   Frame3.Visible = False
   Frame12.Visible = True
   OptChkSwitchM.Visible = True
   TxtSwitchFolioM.SetFocus
   
End If
End Sub

