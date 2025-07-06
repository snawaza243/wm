<%@ Page Title="Playground" Language="C#" MasterPageFile="~/vmSite.Master" AutoEventWireup="true" CodeBehind="playground.aspx.cs" Inherits="WM.Tree.playground" EnableViewState="true" MaintainScrollPositionOnPostback="true" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <link rel="shortcut icon" href="../assets/images/favicon.png" />
    <link rel="stylesheet" href="../assets/vendors/mdi/css/materialdesignicons.min.css">
    <link rel="stylesheet" href="../assets/vendors/ti-icons/css/themify-icons.css">
    <link rel="stylesheet" href="../assets/vendors/css/vendor.bundle.base.css">
    <link rel="stylesheet" href="../assets/vendors/font-awesome/css/font-awesome.min.css">
    <link rel="stylesheet" href="../assets/vendors/bootstrap-datepicker/bootstrap-datepicker.min.css">
    <link rel="stylesheet" href="../assets/css/style.css">
    
    <!-- START NEW LOADER WITH LAST FOCOUS STORE -->
    <div id="serverLoader" class="loader-overlay" style="display: none;">
        <div class="spinner-border text-primary" role="status">
            <span class="sr-only">Loading...</span>
        </div>
        <div class="loading-text">Processing your request...</div>
    </div>

    <%-- LAST FOCOUS STORE ELEMENT --%>
    <asp:HiddenField ID="hfFocusedControl" runat="server" />

    <%-- LOADER CSS --%>
    <style>
        .loader-overlay {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0, 0, 0, 0.7);
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            z-index: 9999;
        }

        .loading-text {
            color: white;
            margin-top: 15px;
            font-size: 1.2rem;
        }

        .spinner-border {
            width: 3rem;
            height: 3rem;
        }
    </style>

    <%-- LOADER, CURSSOR AND LAST FOCOUS JS --%>
    <script type="text/javascript">
        (function () {

            // Track active element before postback
            var lastFocusedElement = null;

            // Show loading indicators (both cursor and overlay)
            function showLoading() {
                // Store the currently focused element
                lastFocusedElement = document.activeElement;
                document.body.style.cursor = 'wait';
                var loader = document.getElementById('serverLoader');
                if (loader) {
                    loader.style.display = 'flex';
                }
            }

            // Hide loading indicators
            function hideLoading() {
                document.body.style.cursor = 'default';
                var loader = document.getElementById('serverLoader');
                if (loader) {
                    loader.style.display = 'none';
                }

                // Restore focus to the last focused element if it exists
                if (lastFocusedElement && document.body.contains(lastFocusedElement)) {
                    try {
                        lastFocusedElement.focus();

                        // For text inputs, restore cursor position
                        if (lastFocusedElement.setSelectionRange &&
                            (lastFocusedElement.type === 'text' || lastFocusedElement.type === 'textarea' ||
                                lastFocusedElement.type === 'password' || lastFocusedElement.tagQuery === 'TEXTAREA')) {
                            var len = lastFocusedElement.value.length;
                            lastFocusedElement.setSelectionRange(len, len);
                        }
                    } catch (e) {
                        console.log("Could not restore focus: ", e);
                    }
                }
            }

            // Function to store focus before postback
            function storeFocus() {
                lastFocusedElement = document.activeElement;
                if (lastFocusedElement) {
                    document.getElementById('<%= hfFocusedControl.ClientID %>').value = lastFocusedElement.id;
                }
            }

            // Function to restore focus after update
            function restoreFocus() {
                var focusedControlId = document.getElementById('<%= hfFocusedControl.ClientID %>').value;
                if (focusedControlId) {
                    var element = document.getElementById(focusedControlId);
                    if (element) {
                        element.focus();
                    }
                }
            }



            // Global functions for manual control
            window.showServerLoader = showLoading;
            window.hideServerLoader = hideLoading;

            // Handle full page postbacks
            var prm = window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager.getInstance();
            if (prm) {
                // Show loader when async postback begins
                prm.add_beginRequest(function (sender, args) {
                    showLoading();
                    storeFocus();
                });

                // Hide loader when async postback completes
                prm.add_endRequest(function (sender, args) {
                    hideLoading();
                    restoreFocus();
                });
            }

            // Handle regular form submission (full postback)
            var originalFormSubmit = window.__doPostBack;
            if (originalFormSubmit) {
                window.__doPostBack = function (eventTarget, eventArgument) {
                    showLoading();
                    storeFocus();
                    return originalFormSubmit(eventTarget, eventArgument);
                };
            }

            // Fallback for manual form submissions
            document.addEventListener('submit', function (e) {
                var form = e.target;
                if (form && form.tagQuery === 'FORM' && !form.hasAttribute('data-ajax-form')) {
                    showLoading();
                    storeFocus();

                }
            }, true);

            // Also handle button clicks that cause postbacks
            document.addEventListener('click', function (e) {
                if (e.target && (e.target.tagQuery === 'INPUT' || e.target.tagQuery === 'BUTTON') &&
                    (e.target.type === 'submit' || e.target.type === 'button')) {
                    showLoading();
                    storeFocus();
                }
            }, true);
        })();
    </script>

    <script>
        Sys.Application.add_load(function () {
            //hideLoading(); // Always hide loader after postback completes
        });
    </script>

    <%-- CURSSOR WAIT AND FREE --%>
    <script type="text/javascript">
        function showLoadingCursor() {
            document.body.style.cursor = 'wait';
        }

        function resetCursor() {
            document.body.style.cursor = 'default';
        }
    </script>

    <%-- Key press evens and trigger buttons --%>
<script type="text/javascript">
    document.addEventListener("DOMContentLoaded", function () {
        document.addEventListener("keydown", function (event) {
            var activeElement = document.activeElement;

            // If focused element is a multi-line TextBox, handle Enter separately
            if (activeElement && activeElement.tagName === "TEXTAREA") {
                // Ctrl + Enter => trigger the button
                if (event.key === "Enter" && event.ctrlKey) {
                    event.preventDefault();
                    document.getElementById('<%= btnSearch.ClientID %>').click();
                }
                // Just Enter => allow new line (default behavior), do nothing
                return;
            }

            // Outside textarea: Enter triggers button click
            if (event.key === "Enter") {
                event.preventDefault();
                document.getElementById('<%= btnSearch.ClientID %>').click();
            }
        });
    });
</script>


    <div class="row">
        <div class="grid-margin stretch-card">
            <div class="card">
                <div class="card-body">
                    <asp:UpdatePanel ID="upnlSearch" runat="server">
                        <ContentTemplate>
                            <div class="row">
                                <div class="col-md-12">

                                    <%-- Search Filters --%>
                                    <div class="row g-3">
                                        <asp:Label ID="lblForm" runat="server" CssClass="message-label m-2" Style="font-size: 16px; font-weight: bold; color: blue; margin: 0;"></asp:Label>

                                        <%-- CONNECTION Dropdown --%>
                                        <asp:Panel CssClass="col-md-2 " ID="pnlConnectionList" runat="server" Visible="true">
                                            <asp:Label ID="lblConnectionList" CssClass="input-group" runat="server" Text="Connection"></asp:Label>
                                            <asp:DropDownList ID="ddlConnectionList" runat="server"
                                                AutoPostBack="true"
                                                OnSelectedIndexChanged="ddlConnectionList_SelectedIndexChanged"
                                                onchange="showServerLoader();"
                                                enabled="false"
                                                CssClass="form-select">
                                                <asp:ListItem Text="Select Connection" Value="" />
                                                <asp:ListItem Text="Local 19C ✔" Value="local1" Selected="True" />
                                                <asp:ListItem Text="Live 11g ⚠" Value="live1" />

                                            </asp:DropDownList>
                                        </asp:Panel>

                                        <%-- DataBase List Dropdown --%>
                                        <asp:Panel CssClass="col-md-2" ID="pnlDataBaseList" runat="server" Visible="true">
                                            <asp:Label ID="lblDataBaseList" runat="server" Text="Databases:"></asp:Label>
                                            <asp:DropDownList ID="ddlDataBaseList" runat="server"
                                                AutoPostBack="true"
                                                OnSelectedIndexChanged="ddlDataBaseList_SelectedIndexChanged"
                                                onchange="showServerLoader();"
                                                enabled="false"

                                                CssClass="form-select">
                                                <asp:ListItem Text="Select Database" Value="" />
                                            </asp:DropDownList>
                                        </asp:Panel>

                                        <%-- Tables List Dropdown --%>
                                        <asp:Panel CssClass="col-md-2" ID="pnlTablesList" runat="server" Visible="true">
                                            <asp:Label ID="lblTablesList" runat="server" Text="Tables List:"></asp:Label>
                                            <asp:DropDownList ID="ddlTablesList" runat="server"
                                                AutoPostBack="true"
                                                OnSelectedIndexChanged="ddlTablesList_SelectedIndexChanged"
                                                onchange="showServerLoader();"
                                                CssClass="form-select">
                                                <asp:ListItem Text="Select Tables" Value="" />
                                            </asp:DropDownList>
                                        </asp:Panel>

                                        <%-- Language List Dropdown --%>
                                        <asp:Panel CssClass="col-md-3" ID="pnlLanguageList" runat="server" Visible="true">
                                            <asp:Label ID="lblLanguageList" runat="server" Text="Conversation Type:"></asp:Label>
                                            <asp:DropDownList ID="ddlLanguageList" runat="server"
                                                AutoPostBack="true"
                                                OnSelectedIndexChanged="ddlLanguageList_SelectedIndexChanged"
                                                onchange="showServerLoader();"
                                                CssClass="form-select">
                                                <asp:ListItem Text="--Select Language--" Value="" />
                                            </asp:DropDownList>
                                        </asp:Panel>


                                        </div>
                                    <div class="row g-3">
                                        <%-- Query TextBox --%>
                                        <asp:Panel CssClass="col-md-12 mt-4
                                            " ID="pnlQuery" runat="server" Visible="true">
                                            <asp:Label ID="lblQuery" runat="server" Text="Query:"></asp:Label>
                                              <%--<asp:TextBox ID="txtQuery" runat="server" CssClass="form-control"
                                                    Style="width: 100%; height: 150px;"
                 TextMode="MultiLine" Rows="5" Columns="100" MaxLength="1000" />--%>

                                           <!-- Main Editor Container -->
<div style="display: flex; border: 1px solid #ccc; width: 100%; height: 350px; font-family: monospace; position: relative;">

    <!-- Line Numbers -->
    <div id="lineNumbers"
         style="background: #f0f0f0; padding: 8px; color: #999; text-align: right;
                user-select: none; overflow: hidden; line-height: 1.6; min-width: 50px;">
        1
    </div>

    <!-- Syntax Highlighting Background -->
    <div id="highlightDiv"
         style="position: absolute; top: 0; left: 30px; right: 0; bottom: 0;
                padding: 8px; white-space: pre-wrap; color: transparent;
                pointer-events: none; overflow: auto; font-family: monospace;
                line-height: 1.6; z-index: 1;">
    </div>

    <!-- Server-side TextBox -->
    <asp:TextBox ID="txtQuery" runat="server"
                 CssClass="form-control"
                 TextMode="MultiLine"
                 Style="border: none; resize: none; width: 100%; height: 100%;
                        padding: 8px; line-height: 1.6; overflow: auto;
                        background: transparent; position: relative; z-index: 2;"
                 onkeyup="onEditorInput(this); showSuggestions(this);"
                 onscroll="onEditorScroll(this)">
    </asp:TextBox>

</div>

<!-- Autocomplete Suggestion Box -->
<div id="suggestionBox"
     style="position: absolute; background: white; border: 1px solid #ccc; 
            z-index: 9999; font-family: monospace; max-height: 150px;
            overflow-y: auto; display: none;"></div>

<!-- Find & Replace + Export -->
<div style="margin-top: 10px;">
    <input type="text" id="findText" placeholder="Find..." />
    <input type="text" id="replaceText" placeholder="Replace with..." />
    <button type="button" onclick="findAndReplace()">Replace All</button>
    <button type="button" onclick="exportSql()">Export as .SQL</button>
</div>

<!-- Script Section -->
<script type="text/javascript">
    const keywords = ["SELECT", "FROM", "WHERE", "AND", "OR", "INSERT", "UPDATE", "DELETE", "JOIN", "INNER", "LEFT", "RIGHT", "ON", "INTO", "VALUES", "GROUP BY", "ORDER BY"];

    function escapeHtml(text) {
        return text.replace(/[&<>"']/g, function (m) {
            return { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[m];
        });
    }

    function highlightSyntax(inputText) {
        let html = escapeHtml(inputText);
        keywords.forEach(keyword => {
            const regex = new RegExp("\\b" + keyword + "\\b", "gi");
            html = html.replace(regex, match => `<span style="color: blue;">${match}</span>`);
        });
        html = html.replace(/('[^']*')/g, `<span style="color: green;">$1</span>`);
        return html;
    }

    function updateLineNumbers(textarea) {


        const lineNumbers = document.getElementById('lineNumbers');
        const lines = textarea.value.split('\n').length;
        lineNumbers.innerHTML = '';
        for (let i = 1; i <= lines; i++) {
            lineNumbers.innerHTML += i + '<br />';
        }
    }

    function onEditorInput(textarea) {
        updateLineNumbers(textarea);
      //document.getElementById("highlightDiv").innerHTML = highlightSyntax(textarea.value);
        onEditorScroll(textarea);
    }

    function onEditorScroll(textarea) {
        const highlightDiv = document.getElementById('highlightDiv');
        const lineNumbers = document.getElementById('lineNumbers');
        highlightDiv.scrollTop = textarea.scrollTop;
        highlightDiv.scrollLeft = textarea.scrollLeft;
        lineNumbers.scrollTop = textarea.scrollTop;
    }

    function showSuggestions(textarea) {
        const box = document.getElementById("suggestionBox");
        const words = textarea.value.substring(0, textarea.selectionStart).split(/\s+/);
        const lastWord = words[words.length - 1].toUpperCase();
        if (!lastWord) {
            box.style.display = "none";
            return;
        }

        const matches = keywords.filter(k => k.startsWith(lastWord));
        if (matches.length === 0) {
            box.style.display = "none";
            return;
        }

        box.innerHTML = matches.map(m => `<div style="padding: 5px; cursor: pointer;" onclick="insertKeyword('${m}', '${textarea.id}')">${m}</div>`).join('');

        const rect = textarea.getBoundingClientRect();
        box.style.left = rect.left + 40 + "px";
        box.style.top = rect.top + textarea.scrollTop + 5 + "px";
        box.style.display = "block";
    }

    function insertKeyword(keyword, textareaId) {
        const textarea = document.getElementById(textareaId);
        const pos = textarea.selectionStart;
        const text = textarea.value;
        const before = text.substring(0, pos).replace(/\w+$/, '');
        const after = text.substring(pos);
        textarea.value = before + keyword + after;
        textarea.focus();
        textarea.selectionStart = textarea.selectionEnd = before.length + keyword.length;
        document.getElementById("suggestionBox").style.display = "none";
        onEditorInput(textarea);
    }

    function findAndReplace() {
        const findVal = document.getElementById("findText").value;
        const replaceVal = document.getElementById("replaceText").value;
        const textarea = document.getElementById('<%= txtQuery.ClientID %>');
        if (!findVal) return;

        textarea.value = textarea.value.split(findVal).join(replaceVal);
        onEditorInput(textarea);
    }

    function exportSql() {
        const textarea = document.getElementById('<%= txtQuery.ClientID %>');
        const content = textarea.value;
        const blob = new Blob([content], { type: 'text/sql' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement("a");
        a.href = url;
        a.download = "query_export.sql";
        a.click();
        URL.revokeObjectURL(url);
    }

    window.onload = function () {
        const textarea = document.getElementById('<%= txtQuery.ClientID %>');
        onEditorInput(textarea);
    };
</script>






                                        </asp:Panel>


                                    </div>
                                  

                                    <div class="row mt-3">
                                        <div class="col-md-12 d-flex mb-3">
                                       <!-- Hidden fields -->
<asp:HiddenField ID="hdnPasscode" runat="server" />
<asp:HiddenField ID="hdnSessionValid" runat="server" />

<!-- Main button -->
<asp:Button 
    ID="btnSearch" 
    runat="server" 
    Text="🔍 Search" 
    CssClass="btn btn-primary me-2" 
    OnClientClick="return validateBeforeFetch();" 
    OnClick="btnSearch_Click" />

<!-- Hidden validation trigger -->
<asp:Button 
    ID="btnValidatePasscode" 
    runat="server" 
    Style="display:none;" 
    OnClick="btnValidatePasscode_Click" />

                                            <script type="text/javascript">
                                                function validateBeforeFetch() {
                                                    var sessionValid = document.getElementById('<%= hdnSessionValid.ClientID %>').value;
        if (sessionValid === "true") return true; // Session is valid, allow server postback

        var input = prompt("Enter access passcode:");
        if (input != null && input.trim() !== "") {
            document.getElementById('<%= hdnPasscode.ClientID %>').value = input;
            __doPostBack('<%= btnValidatePasscode.UniqueID %>', ''); // Trigger passcode validation
                                                        return false; // Block original button postback for now
                                                    }

                                                    return false; // No input entered
                                                }
                                            </script>

                                            
                                            
                                            
                                            
                                            <asp:Button OnDataBaseClick="showServerLoader(); return true;" runat="server" CssClass="btn btn-primary me-2" ID="btnReset" Text="Reset"  OnClick="btnReset_Click" />
                                            <asp:Button OnDataBaseClick="showServerLoader(); return true;" runat="server" CssClass="btn btn-outline-primary" ID="btnExit" Text="Exit"  OnClick="btnExit_Click" />
                                            <asp:TextBox ID="txtMessage" runat="server"
                                                placeholder="Message..."
                                                Enabled="false"
                                                CssClass="form-control ms-2 w-100 " MaxLength="500" />

                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-12 mt-2">

                                    <%-- SearchInGrid TextBox --%>
                                    <asp:Panel CssClass="col-md-3" ID="pnlSearchInGrid" runat="server" Visible="true">
                                        <asp:TextBox ID="txtSearchInGrid" runat="server"
                                            CssClass="form-control"
                                            placeholder="Search..."
                                            MaxLength="100"
                                            AutoPostBack="true"
                                            Visible="false"
                                            OnTextChanged="txtSearchInGrid_TextChanged" />
                                        <asp:HiddenField ID="hfTriggerSearch" runat="server" />
                                        <script type="text/javascript">
                                            document.addEventListener("DOMContentLoaded", function () {
                                                const searchInput = document.getElementById('<%= txtSearchInGrid.ClientID %>');
                                                if (searchInput) {
                                                    searchInput.addEventListener("keydown", function (e) {
                                                        if (e.key === "Enter") {
                                                            e.preventDefault(); // Prevent form submission or other global Enter triggers
                                                            __doPostBack('<%= txtSearchInGrid.UniqueID %>', '');
                                                       }
                                                   });
                                                }
                                            });
                                        </script>



                                    </asp:Panel>
                                    <div class="row g-3">
                                        <asp:Label ID="lblDataBaseListMessage" runat="server" CssClass="message-label m-2"
                                            Style="font-size: 16px; font-weight: bold; color: blue;"></asp:Label>
                                        <div class="table-responsive" style="overflow-y: auto; overflow-x: auto; height: 400px;">
                                            <asp:GridView ID="globalGridOne" runat="server"
                                                AutoGenerateColumns="False"
                                                AllowPaging="true"
                                                PageSize="20"
                                                GridLines="Both"
                                                BorderStyle="Solid" BorderWidth="1px"
                                                CssClass="custom-grid auto-width-table"
                                                OnPageIndexChanging="globalGridOne_PageIndexChanging"
                                                OnRowCommand="globalGridOne_RowCommand"
                                                OnRowDataBound="globalGridOne_RowDataBound">
                                            </asp:GridView>

                                            
                                    <style>
                                        .custom-grid {
                                            width: 100%;
                                            border-collapse: collapse;
                                        }

                                            .custom-grid th,
                                            .custom-grid td {
                                                padding: 8px;
                                                border: 1px solid #ddd;
                                                font-family: 'Segoe UI', sans-serif;
                                                font-size: 14px;
                                            }

                                            .custom-grid th {
                                                background-color: #f2f2f2;
                                                text-align: left;
                                            }

                                            .custom-grid tr:hover {
                                                background-color: #f9f9f9;
                                            }

                                        .auto-width-table {
                                            table-layout: auto;
                                            width: 100%;
                                            border-collapse: collapse; /* Makes borders more neat */
                                        }

                                            .auto-width-table th, .auto-width-table td {
                                                padding: 8px;
                                                text-align: left;
                                                border: 1px solid #ddd; /* Border for clean separation */
                                                white-space: nowrap; /* Prevents text wrapping and forces columns to adjust */
                                            }

                                            .auto-width-table th {
                                                background-color: #f2f2f2;
                                            }

                                            /* Optional: Make it look better when rows are hovered */
                                            .auto-width-table tr:hover {
                                                background-color: #f9f9f9;
                                            }
                                    </style>
                                    <link rel="stylesheet" href="https://code.jquery.com/ui/1.13.2/themes/base/jquery-ui.css">
                                    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
                                    <script src="https://code.jquery.com/ui/1.13.2/jquery-ui.min.js"></script>
                                    <script>
                                        $(document).ready(function () {
                                            $("#<%= globalGridOne.ClientID %> th").resizable({
                                                handles: "e", // east (right) edge
                                                minWidth: 50,
                                                maxWidth: 500
                                            });
                                        });
                                    </script>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlConnectionList" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlDataBaseList" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlTablesList" EventName="SelectedIndexChanged" />

                            <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="btnReset" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="btnExit" EventName="Click" />
                            
                            <asp:AsyncPostBackTrigger ControlID="ddlLanguageList" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="txtSearchInGrid" EventName="TextChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                  

                </div>
            </div>
        </div>
    </div>
</asp:Content>
