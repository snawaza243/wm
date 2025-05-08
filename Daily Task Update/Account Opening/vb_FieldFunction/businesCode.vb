If TguestCode.Text <> "" Then
  txtbusicode.Text = SqlRet("select nvl(emp_no,0) from bajaj_venue_booking where guest_cd='" & TguestCode & "'")
  Call txtbusicode_LostFocus
  TguestCode.Enabled = False
End If
End Sub