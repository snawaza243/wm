Private Sub CmdClientSearchM_Click()
TxtAcHolderA.Text = ""
 treeselected = "N"
 Set frmtree_searchnewMF.currentForm = Nothing
 Set frmtree_searchnewMF.currentForm = frmtransactionmf
 frmtree_searchnewMF.treeName = "Treeclient"
 If MyCurrentBranchCode <> "" Then
     BrCode = MyCurrentBranchCode
 End If
Load frmtree_searchnewMF
frmtree_searchnewMF.Ar_type = ""
frmtree_searchnewMF.Cmbcat.ListIndex = 1
frmtree_searchnewMF.Cmbcat.Enabled = False
frmtree_searchnewMF.CmbClientBroker.Visible = True
frmtree_searchnewMF.CmbClientBroker.ListIndex = 0
frmtree_searchnewMF.lblOF.Visible = True
frmtree_searchnewMF.Show vbModal
End SubMFV_BTN_SEARCH