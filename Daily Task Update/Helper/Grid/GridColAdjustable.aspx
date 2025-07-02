<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Resizable GridView Columns</title>
    <style type="text/css">
        .gridview {
            border-collapse: collapse;
            width: 100%;
        }
        .gridview th {
            background-color: #4CAF50;
            color: white;
            padding: 8px;
            position: relative;
            cursor: col-resize;
            border: 1px solid #ddd;
        }
        .gridview td {
            padding: 8px;
            border: 1px solid #ddd;
        }
        .gridview tr:nth-child(even) {
            background-color: #f2f2f2;
        }
        .gridview tr:hover {
            background-color: #ddd;
        }
        .resizer {
            position: absolute;
            top: 0;
            right: 0;
            width: 5px;
            height: 100%;
            background: rgba(0,0,0,0.2);
            cursor: col-resize;
        }
        .resizer:hover, .resizer:active {
            background: rgba(0,0,0,0.5);
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <div>
            <h1>Resizable GridView Columns</h1>
            
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <asp:GridView ID="GridView1" runat="server" CssClass="gridview" AutoGenerateColumns="false"
                        OnRowDataBound="GridView1_RowDataBound">
                        <Columns>
                            <asp:BoundField DataField="ID" HeaderText="ID" />
                            <asp:BoundField DataField="Name" HeaderText="Name" />
                            <asp:BoundField DataField="Email" HeaderText="Email" />
                            <asp:BoundField DataField="Department" HeaderText="Department" />
                            <asp:BoundField DataField="JoinDate" HeaderText="Join Date" DataFormatString="{0:d}" />
                            <asp:BoundField DataField="Salary" HeaderText="Salary" DataFormatString="{0:C}" />
                        </Columns>
                    </asp:GridView>
                    
                    <asp:Button ID="btnRefresh" runat="server" Text="Refresh Data" OnClick="btnRefresh_Click" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </form>
    
    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', function () {
            // Make GridView columns resizable
            const thElements = document.querySelectorAll('.gridview th');
            
            thElements.forEach(th => {
                // Add resizer element
                const resizer = document.createElement('div');
                resizer.classList.add('resizer');
                th.appendChild(resizer);
                
                // Store initial width and mouse position
                let startX, startWidth;
                
                resizer.addEventListener('mousedown', initResize, false);
                
                function initResize(e) {
                    e.preventDefault();
                    e.stopPropagation();
                    
                    startX = e.clientX;
                    startWidth = parseInt(document.defaultView.getComputedStyle(th).width, 10);
                    
                    document.documentElement.addEventListener('mousemove', doResize, false);
                    document.documentElement.addEventListener('mouseup', stopResize, false);
                }
                
                function doResize(e) {
                    const width = startWidth + e.clientX - startX;
                    th.style.width = width + 'px';
                    
                    // Update all cells in the column to match header width
                    const colIndex = Array.from(th.parentNode.children).indexOf(th);
                    const rows = document.querySelectorAll('.gridview tr');
                    
                    rows.forEach(row => {
                        const cell = row.children[colIndex];
                        if (cell) {
                            cell.style.width = width + 'px';
                        }
                    });
                }
                
                function stopResize(e) {
                    document.documentElement.removeEventListener('mousemove', doResize, false);
                    document.documentElement.removeEventListener('mouseup', stopResize, false);
                }
            });
        });
    </script>
</body>
</html>