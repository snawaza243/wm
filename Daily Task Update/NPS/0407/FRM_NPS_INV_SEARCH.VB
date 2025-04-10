Private Sub Picture1_Click(Index As Integer)
    treeselected = "N"
    Set frmtree_searchnew.currentForm = Nothing
    Set frmtree_searchnew.currentForm = frmNPS
    frmtree_search.treeName = "Treeclient"
    frmtree_searchnew.Cmbcat.Text = "INVESTOR"   'BY HIMANSHU
    frmtree_searchnew.Cmbcat.Enabled = False
    
    frmtree_searchnew.lblOF.Visible = True
    frmtree_searchnew.CmbClientBroker.Visible = True
    frmtree_searchnew.CmbClientBroker.ListIndex = 0
    frmtree_searchnew.Label7.Enabled = True
    frmtree_searchnew.txtCliSubName.Enabled = True
    'frmtree_search.chkIndia.Visible = True
    frmtree_searchnew.Show
    frmtree_searchnew.cmbBranch.SetFocus
    frmtree_searchnew.ZOrder
End Sub