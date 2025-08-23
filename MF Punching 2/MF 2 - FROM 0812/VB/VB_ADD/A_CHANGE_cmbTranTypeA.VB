Private Sub cmbTranTypeA_Click()
CmbSipStpA.ListIndex = -1
ImSipStartDtA.Text = Format(Now, "DD/MM/YYYY")
ImSipEndDtA.Text = "__/__/____"
CmbSubSipA.ListIndex = -1
CmbFrequency.ListIndex = -1
Opt99.Value = False
If cmbTranTypeA.Text = "PURCHASE" Then
   'Frame2.Visible = True
   OptChkSwitch.Value = 0
   Frame11.Enabled = False
   'Frame15.Enabled = False
   Frame11.Enabled = False
   Label6.Item(4).Enabled = False
   Label6.Item(5).Enabled = False
   
   CmbSipStpA.Enabled = True
   ImSipStartDtA.Enabled = True
   ImSipEndDtA.Enabled = True
   CmbSubSipA.Enabled = True
   CmbFrequency.Enabled = True
   Opt99.Enabled = True
   TxtSwitchFolioA.Text = ""
   TxtSwitchSchemeA.Text = ""
   'Frame11.BackColor = RGB(255, 255, 222)
   'Label6.Item(4).BackColor = RGB(255, 255, 222)
   'Label6.Item(5).BackColor = RGB(255, 255, 222)
ElseIf cmbTranTypeA.Text = "SWITCH IN" Then
   'Frame2.Visible = False
   Frame11.Enabled = True
   OptChkSwitch.Visible = False
   TxtSwitchFolioA.SetFocus
   CmbFrequency.ListIndex = -1
   OptOthers.Value = True
   'Frame15.Enabled = True
   Frame11.Enabled = True
   Label6.Item(4).Enabled = True
   Label6.Item(5).Enabled = True
   
   CmbSipStpA.Enabled = False
   ImSipStartDtA.Enabled = False
   ImSipEndDtA.Enabled = False
   CmbSubSipA.Enabled = False
   CmbFrequency.Enabled = False
   Opt99.Enabled = False
   'Frame11.BackColor = RGB(128, 128, 128)
   'Label6.Item(4).BackColor = RGB(128, 128, 128)
   'Label6.Item(5).BackColor = RGB(128, 128, 128)
End If
End Sub


