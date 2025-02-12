<!-- this is the script for opening nad closing hte model also handle the local storage that will helpt to keep open even on page loas once hte modle is open and close "     -->

<script>
    const PsmModalOneStateKey = 'modalState_psmModalOne';

    function openPsmModalOne() {
        const modal = document.getElementById("psmModalOne");
        if (modal) {
            modal.style.display = "block";
            // Store the modal's state in localStorage when it is opened
            localStorage.setItem(psmModalStateKey, 'open');
        }
    }

    function closePsmModalOne() {
        const modal = document.getElementById("psmModalOne");
        if (modal) {
            modal.style.display = "none";
            // Store the modal's state in localStorage when it is closed
            localStorage.setItem(psmModalStateKey, 'closed');
        }
    }

    // Automatically check and reopen the modal if it was open before
    document.addEventListener('DOMContentLoaded', function () {
        const modalState = localStorage.getItem(psmModalStateKey);
        if (modalState === 'open') {
            openPsmModalOne();
        }
    });



</script>
<!-- "This button to open the model "   -->
  <asp:Button ID="openPsmModalOne_btn" CssClass="btn btn-outline-primary" Text="Open Model" runat="server" OnClientClick="openPsmModalOne(); return false;" />
<!-- " this is model css "    -->

<style>
    /* Modal styles */
    .modal {
        display: none;
        position: fixed;
        z-index: 1;
        left: 0;
        top: 0;
        width: 100%;
        height: 100%;
        overflow: auto;
        background-color: rgb(0,0,0);
        background-color: rgba(0,0,0,0.4);
    }

    .modal-content {
        background-color: #fefefe;
        margin: 10% auto;
        padding: 20px;
        border: 1px solid #888;
        width: 80%;
    }

    .close {
        color: #aaa;
        float: right;
        font-size: 28px;
        font-weight: bold;
    }

    .close:hover, .close:focus {
        color: black;
        text-decoration: none;
        cursor: pointer;
    }
</style>

<!-- "  This is psmModalOne " -->
<div id="psmModalOne" class="modal" style="z-index: 999;">
    <div class="modal-content">
        <h2 class="page-title">Map Fields</h2>
        <div class="container mt-4">
            <%-- Dropdown row: psmModalOne_ddl1   --%>
            <div class="row">
                <div class="col-md-6">
                    <label for="psmModalOne_ddl1">Excel Fields:</label>
                    <asp:DropDownList ID="psmModalOne_ddl1" runat="server" CssClass="form-select">
                        <asp:ListItem Text="Select Excel Field" Value="" />
                    </asp:DropDownList>
                </div>

        
            </div>

            <%-- Buttons row row: psmModalOne_btn1   --%>
            <div class="row mt-3">
                <div class="col-md-12 d-flex justify-content-center gap-2">
                    <asp:Button ID="psmModalOne_btn1" runat="server" 
                    Text="Button 1" CssClass="btn btn-secondary" 
                    OnClick="psmModalOne_btn1_Click" />

                    <asp:Button 
                    ID="closePsmModalOne_btn" runat="server" 
                    Text="Close Model" CssClass="btn btn-outline-danger"
                    OnClientClick="closePsmModalOne(); return false;" />
                </div>
            </div>

            <!-- lblMappingMessage: Mapping model message label -->
            <div class="row mt-4">
                <div class="col-md-12 d-flex justify-content-start gap-3">
                    <asp:Label ID="psmModalOne_lbl1" runat="server" Text="Model Label 1"></asp:Label>
                </div>
            </div>
        </div>
    </div>
</div>

