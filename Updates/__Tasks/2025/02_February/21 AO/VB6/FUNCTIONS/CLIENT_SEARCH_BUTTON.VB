Private Sub Picture2_Click()
treeselected = "N"
Set frmtreeMf.currentForm = Nothing
Set frmtreeMf.currentForm = frmactopen
frmtreeMf.treeName = "Treeclient"
Load frmtreeMf
frmtreeMf.Ar_type = ""
frmtreeMf.Cmbcat.ListIndex = 0
frmtreeMf.Combo2.ListIndex = 0
frmtreeMf.Cmbcat.Enabled = False
frmtreeMf.CmbClientBroker.Visible = True
frmtreeMf.CmbClientBroker.ListIndex = 0
frmtreeMf.lblOF.Visible = True
frmtreeMf.Show vbModal
End Sub