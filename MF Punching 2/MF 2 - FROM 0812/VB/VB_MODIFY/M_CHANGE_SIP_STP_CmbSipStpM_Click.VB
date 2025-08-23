Private Sub CmbSipStpM_Click()
ImSipStartDtM.Visible = False
   ImSipEndDtM.Visible = False
   
   Label2(8).Visible = False
   Label2(10).Visible = False

If CmbSipStpM.Text = "SIP" Or CmbSipStpM.Text = "STP" Then
    CmbFrequency1.Enabled = True
    Opt991.Enabled = True
    Txtinstallments1.Enabled = True
    ImSipStartDtM.Visible = True
   ImSipEndDtM.Visible = True
    Label2(8).Visible = True
   Label2(10).Visible = True
Else
    CmbFrequency1.ListIndex = -1
    Txtinstallments1.Text = ""
    CmbFrequency1.Enabled = False
    Opt991.Enabled = False
    Txtinstallments1.Enabled = False
    ImSipStartDtM.Visible = False
   ImSipEndDtM.Visible = False
   Label2(8).Visible = False
   Label2(10).Visible = False
End If
If CmbSipStpM.Text = "STP" Then
   Frame3.Visible = False
   Frame12.Visible = True
   OptChkSwitchM.Visible = True
   TxtSwitchFolioM.SetFocus
End If
If CmbSipStpM.Text = "SIP" Then
    OptSIPRM.Visible = True
    OptSIPFM.Visible = True
    OptSIPFM.Value = True
    ImSipStartDtM.Visible = True
   ImSipEndDtM.Visible = True
    Label2(8).Visible = True
   Label2(10).Visible = True
Else
    OptSIPFM.Value = True
    OptSIPRM.Visible = False
    OptSIPFM.Visible = False
    ImSipStartDtM.Visible = False
   ImSipEndDtM.Visible = False
   Label2(8).Visible = False
   Label2(10).Visible = False
End If
ChkCloseM_Click
End Sub




