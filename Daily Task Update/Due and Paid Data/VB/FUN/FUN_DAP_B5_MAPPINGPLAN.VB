Private Sub CmdMappingPlan_Click()
'Shell (App.Path & "\Project1.exe")
frmmappingplan.Show
frmmappingplan.MonNo = CmbMonth.ListIndex + 1
frmmappingplan.ZOrder
End Sub