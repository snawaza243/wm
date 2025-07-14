
<%@ Page Title="Experimental Page" Language="C#" AutoEventWireup="true" CodeBehind="Experimental.aspx.cs" 
    Inherits="WM.Tree.Experimental" EnableViewState="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Experimental Page</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style type="text/css">
        /* Custom styles */
        .persistent-modal {
            display: block !important;
            position: fixed;
            z-index: 1050;
        }
        
        .modal-backdrop.show {
            opacity: 0.5;
        }
        
        .section-card {
            margin-bottom: 20px;
            border: 1px solid #dee2e6;
            border-radius: 5px;
            padding: 15px;
        }
        
        .section-title {
            border-bottom: 1px solid #dee2e6;
            padding-bottom: 10px;
            margin-bottom: 15px;
        }
        
        .status-message {
            padding: 10px;
            margin: 10px 0;
            border-radius: 4px;
        }
        
        .success {
            background-color: #d4edda;
            color: #155724;
        }
        
        .error {
            background-color: #f8d7da;
            color: #721c24;
        }
        
        .loading-overlay {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0,0,0,0.5);
            z-index: 9999;
            display: flex;
            justify-content: center;
            align-items: center;
            color: white;
            font-size: 1.5rem;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true"></asp:ScriptManager>
        
        <!-- Loading overlay -->
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="upMain">
            <ProgressTemplate>
                <div class="loading-overlay">
                    <div class="spinner-border text-primary" role="status">
                        <span class="visually-hidden">Loading...</span>
                    </div>
                    <span class="ms-3">Processing...</span>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
        
        <div class="container mt-4">
            <h2>Experimental Page</h2>
            
            <!-- Main Update Panel -->
            <asp:UpdatePanel ID="upMain" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <!-- Status Messages -->
                    <asp:Panel ID="pnlStatus" runat="server" CssClass="status-message" Visible="false">
                        <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>
                    </asp:Panel>
                    
                    <!-- Section 1: Basic Input Controls -->
                    <div class="section-card">
                        <h4 class="section-title">Basic Input Controls</h4>
                        
                        <div class="row mb-3">
                            <div class="col-md-4">
                                <label for="txtName" class="form-label">Text Input</label>
                                <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Enter text"></asp:TextBox>
                            </div>
                            <div class="col-md-4">
                                <label for="ddlOptions" class="form-label">Dropdown</label>
                                <asp:DropDownList ID="ddlOptions" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="-- Select --" Value=""></asp:ListItem>
                                    <asp:ListItem Text="Option 1" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Option 2" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Option 3" Value="3"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-4">
                                <label class="form-label">Radio Buttons</label>
                                <div class="form-check">
                                    <asp:RadioButton ID="rbOption1" runat="server" GroupName="radioGroup" CssClass="form-check-input" />
                                    <label class="form-check-label" for="rbOption1">Option 1</label>
                                </div>
                                <div class="form-check">
                                    <asp:RadioButton ID="rbOption2" runat="server" GroupName="radioGroup" CssClass="form-check-input" />
                                    <label class="form-check-label" for="rbOption2">Option 2</label>
                                </div>
                            </div>
                        </div>
                        
                        <div class="row mb-3">
                            <div class="col-md-4">
                                <label for="txtDate" class="form-label">Date Input</label>
                                <asp:TextBox ID="txtDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            </div>
                            <div class="col-md-4">
                                <label for="txtNumber" class="form-label">Number Input</label>
                                <asp:TextBox ID="txtNumber" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                            </div>
                            <div class="col-md-4">
                                <label for="fileUpload" class="form-label">File Upload</label>
                                <asp:FileUpload ID="fileUpload" runat="server" CssClass="form-control" />
                            </div>
                        </div>
                        
                        <div class="row">
                            <div class="col-md-12">
                                <asp:Button ID="btnSubmitBasic" runat="server" Text="Submit Basic" 
                                    CssClass="btn btn-primary" OnClick="btnSubmitBasic_Click" />
                                <asp:Button ID="btnResetBasic" runat="server" Text="Reset Basic" 
                                    CssClass="btn btn-secondary" OnClick="btnResetBasic_Click" />
                                <asp:Button ID="btnOpenModal" runat="server" Text="Open Modal" 
                                    CssClass="btn btn-info" OnClientClick="openPersistentModal(); return false;" />
                            </div>
                        </div>
                    </div>
                    
                    <!-- Section 2: GridView with Update Panel -->
                    <div class="section-card">
                        <h4 class="section-title">Data Grid with CRUD Operations</h4>
                        
                        <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row mb-3">
                                    <div class="col-md-6">
                                        <label for="txtSearch" class="form-label">Search</label>
                                        <div class="input-group">
                                            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search..."></asp:TextBox>
                                            <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-outline-secondary" 
                                                OnClick="btnSearch_Click" />
                                        </div>
                                    </div>
                                    <div class="col-md-6 text-end">
                                        <asp:Button ID="btnAddNew" runat="server" Text="Add New" CssClass="btn btn-success" 
                                            OnClick="btnAddNew_Click" />
                                    </div>
                                </div>
                                
                                <div class="table-responsive">
                                    <asp:GridView ID="gvData" runat="server" CssClass="table table-striped table-bordered"
                                        AutoGenerateColumns="false" AllowPaging="true" PageSize="5"
                                        OnPageIndexChanging="gvData_PageIndexChanging" OnRowCommand="gvData_RowCommand"
                                        OnRowEditing="gvData_RowEditing" OnRowDeleting="gvData_RowDeleting"
                                        DataKeyNames="ID">
                                        <Columns>
                                            <asp:BoundField DataField="ID" HeaderText="ID" />
                                            <asp:BoundField DataField="Name" HeaderText="Name" />
                                            <asp:BoundField DataField="Description" HeaderText="Description" />
                                            <asp:BoundField DataField="CreatedDate" HeaderText="Date" DataFormatString="{0:d}" />
                                            <asp:TemplateField HeaderText="Actions">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" CssClass="btn btn-sm btn-warning"
                                                        CommandArgument='<%# Eval("ID") %>' Text="Edit" />
                                                    <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" CssClass="btn btn-sm btn-danger"
                                                        CommandArgument='<%# Eval("ID") %>' Text="Delete" 
                                                        OnClientClick="return confirm('Are you sure you want to delete this item?');" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <PagerSettings Mode="NumericFirstLast" />
                                        <PagerStyle CssClass="pagination" />
                                    </asp:GridView>
                                </div>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btnAddNew" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="gvData" EventName="PageIndexChanging" />
                                <asp:AsyncPostBackTrigger ControlID="gvData" EventName="RowCommand" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            
            <!-- Persistent Modal Dialog -->
            <div class="modal fade" id="persistentModal" tabindex="-1" aria-labelledby="modalLabel" aria-hidden="true">
                <div class="modal-dialog modal-lg">
                    <asp:UpdatePanel ID="upModal" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h5 class="modal-title" id="modalLabel">
                                        <asp:Label ID="lblModalTitle" runat="server" Text="Persistent Modal"></asp:Label>
                                    </h5>
                                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close" 
                                        onclick="closePersistentModal(); return false;"></button>
                                </div>
                                <div class="modal-body">
                                    <div class="row mb-3">
                                        <div class="col-md-6">
                                            <label for="txtModalName" class="form-label">Name</label>
                                            <asp:TextBox ID="txtModalName" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                        <div class="col-md-6">
                                            <label for="ddlModalType" class="form-label">Type</label>
                                            <asp:DropDownList ID="ddlModalType" runat="server" CssClass="form-select">
                                                <asp:ListItem Text="Type A" Value="A"></asp:ListItem>
                                                <asp:ListItem Text="Type B" Value="B"></asp:ListItem>
                                                <asp:ListItem Text="Type C" Value="C"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="row mb-3">
                                        <div class="col-md-12">
                                            <label for="txtModalDescription" class="form-label">Description</label>
                                            <asp:TextBox ID="txtModalDescription" runat="server" CssClass="form-control" 
                                                TextMode="MultiLine" Rows="3"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-6">
                                            <label for="chkModalActive" class="form-label">Active</label>
                                            <asp:CheckBox ID="chkModalActive" runat="server" CssClass="form-check-input" />
                                        </div>
                                        <div class="col-md-6">
                                            <label for="txtModalDate" class="form-label">Date</label>
                                            <asp:TextBox ID="txtModalDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <asp:Button ID="btnModalSave" runat="server" Text="Save" CssClass="btn btn-primary" 
                                        OnClick="btnModalSave_Click" />
                                    <asp:Button ID="btnModalCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary" 
                                        OnClientClick="closePersistentModal(); return false;" />
                                </div>
                            </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnModalSave" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
        
        <!-- Hidden field to track modal state -->
        <asp:HiddenField ID="hfModalState" runat="server" Value="closed" />
    </form>
    
    <!-- Bootstrap JS Bundle with Popper -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    
    <script type="text/javascript">
        // Function to open the persistent modal
        function openPersistentModal() {
            var modal = new bootstrap.Modal(document.getElementById('persistentModal'));
            modal.show();
            document.getElementById('<%= hfModalState.ClientID %>').value = "open";
            
            // Optional: Focus on first input field
            setTimeout(function() {
                var firstInput = document.querySelector('#persistentModal .form-control');
                if (firstInput) {
                    firstInput.focus();
                }
            }, 500);
        }
        
        // Function to close the persistent modal
        function closePersistentModal() {
            var modal = bootstrap.Modal.getInstance(document.getElementById('persistentModal'));
            if (modal) {
                modal.hide();
            }
            document.getElementById('<%= hfModalState.ClientID %>').value = "closed";
        }
        
        // Reopen modal if it was open before postback
        function pageLoad() {
            if (document.getElementById('<%= hfModalState.ClientID %>').value === "open") {
                openPersistentModal();
            }
        }
        
        // Initialize tooltips
        document.addEventListener("DOMContentLoaded", function() {
            var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
            var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
                return new bootstrap.Tooltip(tooltipTriggerEl);
            });
        });
    </script>
</body>
</html>