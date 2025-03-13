<div>
    <!-- Script for opening and closing the modal, handling local storage -->
    <script>
        const PsmModalOneStateKey = 'modalState_psmModalOne';

        function openPsmModalOne() {
            const modal = document.getElementById("psmModalOne");
            if (modal) {
                modal.style.display = "block";
                // Store the modal's state in localStorage when it is opened
                localStorage.setItem(PsmModalOneStateKey, 'open');
            }
        }

        function closePsmModalOne() {
            const modal = document.getElementById("psmModalOne");
            if (modal) {
                modal.style.display = "none";
                // Store the modal's state in localStorage when it is closed
                localStorage.setItem(PsmModalOneStateKey, 'closed');
            }
        }

        // Automatically check and reopen the modal if it was open before
        document.addEventListener('DOMContentLoaded', function () {
            const modalState = localStorage.getItem(PsmModalOneStateKey);
            if (modalState === 'open') {
                openPsmModalOne();
            }
        });
    </script>

    <!-- Button to open the modal -->
    <asp:Button ID="openPsmModalOne_btn" CssClass="d-none btn btn-outline-primary" Text="Open Modal" runat="server" OnClientClick="openPsmModalOne(); return false;" />

    <!-- Modal CSS -->
    <style>
        .modal {
            display: none;
            position: fixed;
            z-index: 1;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            overflow: auto;
            background-color: rgba(0, 0, 0, 0.4);
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
            cursor: pointer;
        }

            .close:hover, .close:focus {
                color: black;
                text-decoration: none;
            }
    </style>

    <!-- Modal Structure -->
    <div id="psmModalOne" class="modal" style="z-index: 999;">
        <div class="modal-content">
            <h2 class="page-title">Map Fields</h2>
            <div class="container mt-4">
                <div class="row">
                    <div class="col-md-6">
                        <label for="psmModalOne_ddl1">Excel Fields:</label>
                        <asp:DropDownList ID="psmModalOne_ddl1" runat="server" CssClass="form-select">
                            <asp:ListItem Text="Select Excel Field" Value="" />
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="row mt-3">
                    <div class="col-md-12 d-flex justify-content-center gap-2">
                        <asp:Button ID="psmModalOne_btn1" runat="server"
                            Text="Button 1" CssClass="btn btn-secondary" />

                        <asp:Button
                            ID="closePsmModalOne_btn" runat="server"
                            Text="Close Modal" CssClass="btn btn-outline-danger"
                            OnClientClick="closePsmModalOne(); return false;" />
                    </div>
                </div>

                <div class="row mt-4">
                    <div class="col-md-12 d-flex justify-content-start gap-3">
                        <asp:Label ID="psmModalOne_lbl1" runat="server" Text="Modal Label 1"></asp:Label>
                    </div>
                </div>
            </div>
        </div>
    </div>

</div>