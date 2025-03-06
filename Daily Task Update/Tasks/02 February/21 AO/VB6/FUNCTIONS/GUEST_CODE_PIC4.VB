Private Sub picture4_fun()
On Error Resume Next
selected_client_key = treeClient.SelectedItem.Key
Guest_AR = True
Guest_ARGen = False
Load frm_guestsearch
frm_guestsearch.client_master_flg = "Client" 'gaurav
frm_guestsearch.MyGuestCode = TguestCode.Text
'If lostguest = 1 Then
        frm_guestsearch.TguestCode.Text = TguestCode.Text
        frm_guestsearch.tempval.Text = 2
        frm_guestsearch.Command3_Click
        frm_guestsearch.Guestgrid_DblClick
        Unload frm_guestsearch
'Else
'frm_guestsearch.Show
'frm_guestsearch.ZOrder
'End If
'If txtbusicode <> "" Then
'txtbusicode_LostFocus
'End If
End Sub