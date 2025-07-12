<script type="text/javascript">
    // Make panel draggable
    function makePanelDraggable() {
        var panel = $('.draggable');
        var header = $('.panel-heading', panel);

        var isDragging = false;
        var offsetX, offsetY;

        // Mouse down event on header
        header.on('mousedown', function (e) {
            isDragging = true;

            // Get current position
            var pos = panel.offset();

            // Calculate offset between mouse and panel position
            offsetX = e.clientX - pos.left;
            offsetY = e.clientY - pos.top;

            // Remove transform to allow manual positioning
            panel.css({
                'transform': 'none',
                'top': pos.top + 'px',
                'left': pos.left + 'px'
            });

            // Prevent text selection
            e.preventDefault();
        });

        // Mouse move event
        $(document).on('mousemove', function (e) {
            if (!isDragging) return;

            // Calculate new position
            var newX = e.clientX - offsetX;
            var newY = e.clientY - offsetY;

            // Get viewport dimensions
            var viewportWidth = $(window).width();
            var viewportHeight = $(window).height();

            // Get panel dimensions
            var panelWidth = panel.outerWidth();
            var panelHeight = panel.outerHeight();

            // Constrain to viewport boundaries
            newX = Math.max(0, Math.min(newX, viewportWidth - panelWidth));
            newY = Math.max(0, Math.min(newY, viewportHeight - panelHeight));

            // Apply new position
            panel.css({
                'left': newX + 'px',
                'top': newY + 'px'
            });
        });

        // Mouse up event
        $(document).on('mouseup', function () {
            isDragging = false;
        });
    }

    // Call this function when the panel is shown
    function showDuplicatePopup() {
        var panel = $('.draggable');
        panel.show();
        makePanelDraggable();
    }

    // Your existing cancel function
    function cancelPopup() {
        $('.draggable').hide();
    }

    // Initialize when document is ready
    $(document).ready(function () {
        makePanelDraggable();
    });
</script>
<asp:Panel ID="popupDuplicate" runat="server" CssClass="panel panel-default draggable"
    Style="display:none; width: 800px; min-height:350px; height: auto; top: 50%; left: 50%; transform: translate(-50%, -50%); background: #d3d3d3; border: 2px solid black; padding: 10px; box-shadow: 2px 2px 10px rgba(0, 0, 0, 0.5); z-index: 2000; border-radius: 5px; position: fixed;">

    <!-- Panel Header - Added unselectable="on" and id for easier selection -->
    <div class="panel-heading" style="background: navy; color: white; padding: 5px; font-weight: bold; cursor: move;"
        unselectable="on" onselectstart="return false;">
        Duplicate Check Popup
    </div>

    <!-- Scrollable GridView Container -->
    <div class="panel-body" style="max-height: 400px; overflow: auto; border: 1px solid #ccc; padding: 5px;">
        <asp:GridView ID="gvDuplicateTransactions" runat="server" CssClass="table table-bordered"
            AutoGenerateColumns="False" Width="100%" ShowHeader="True">
            <HeaderStyle CssClass="thead-dark" />
            <Columns>
                <asp:BoundField DataField="ClientCode" HeaderText="Client Code" />
                <asp:BoundField DataField="ClientName" HeaderText="Client Name" />
                <asp:BoundField DataField="Mobile" HeaderText="Mobile" />
                <asp:BoundField DataField="ARDate" HeaderText="AR Date" DataFormatString="{0:dd-MM-yyyy}" />
                <asp:BoundField DataField="SchemeName" HeaderText="Scheme Name" />
                <asp:BoundField DataField="SchemeCode" HeaderText="Scheme Code" />
                <asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="{0:N2}" />
                <asp:BoundField DataField="ARNumber" HeaderText="AR Number" />
                <asp:BoundField DataField="DTNumber" HeaderText="DT Number" />
                <asp:BoundField DataField="ChequeNumber" HeaderText="Cheque Number" />
            </Columns>
        </asp:GridView>
    </div>

    <!-- Buttons -->
    <div style="text-align: center; margin-top: 10px;">
        <asp:Button ID="btnContinue" runat="server" Text="Continue" OnClick="btnContinue_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="cancelPopup(); return false;" />
    </div>

    <!-- Hidden Field to Track Decision -->
    <asp:HiddenField ID="hfContinueTransaction" runat="server" Value="0" />
</asp:Panel>