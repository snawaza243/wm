Private Sub cmdGo_Click()
If TguestCode <> "" Then
    lostguest = 1
    picture4_fun
    If TguestCode.Text = "" Then
        MsgBox "Guest code not Found", vbOKOnly, "Wealthmaker"
        TguestCode.Text = ""
        'TguestCode.SetFocus
        Exit Sub
    End If
    If txtPitchBook.Text = "" Then
        MsgBox "Pitch book number not Found", vbOKOnly, "Wealthmaker"
        TguestCode.Text = ""
        'TguestCode.SetFocus
        Exit Sub
    End If
Else
    lostguest = 0
End If
If TguestCode.Text <> "" Then
  txtbusicode.Text = SqlRet("select nvl(emp_no,0) from bajaj_venue_booking where guest_cd='" & TguestCode & "'")
  Call txtbusicode_LostFocus
  TguestCode.Enabled = False
End If
End Sub