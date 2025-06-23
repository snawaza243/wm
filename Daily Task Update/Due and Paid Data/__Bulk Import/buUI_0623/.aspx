<%@ Page Title="Due and Paid Data Importing" MasterPageFile="~/vmSite.Master" Language="C#" AutoEventWireup="true" CodeBehind="DueAndPaidDataImporting.aspx.cs" Inherits="WM.Masters.DueAndPaidDataImporting" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <link rel="shortcut icon" href="../assets/images/favicon.png" />

    <link rel="stylesheet" href="../assets/vendors/mdi/css/materialdesignicons.min.css">
    <link rel="stylesheet" href="../assets/vendors/ti-icons/css/themify-icons.css">
    <link rel="stylesheet" href="../assets/vendors/css/vendor.bundle.base.css">
    <link rel="stylesheet" href="../assets/vendors/font-awesome/css/font-awesome.min.css">
    <link rel="stylesheet" href="../assets/vendors/bootstrap-datepicker/bootstrap-datepicker.min.css">
    <link rel="stylesheet" href="../assets/css/style.css">

    <script src="../assets/vendors/js/vendor.bundle.base.js"></script>
    <script src="../assets/vendors/chart.js/chart.umd.js"></script>
    <script src="../assets/vendors/bootstrap-datepicker/bootstrap-datepicker.min.js"></script>
    <script src="../assets/js/off-canvas.js"></script>
    <script src="../assets/js/misc.js"></script>
    <script src="../assets/js/settings.js"></script>
    <script src="../assets/js/todolist.js"></script>
    <script src="../assets/js/jquery.cookie.js"></script>
    <script src="../assets/js/dashboard.js"></script>

    <script src="https://cdnjs.cloudflare.com/ajax/libs/xlsx/0.17.5/xlsx.full.min.js"></script>
    <script src="https://code.jquery.com/jquery-3.3.1.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/xlsx/0.16.9/xlsx.full.min.js"></script>

    <!-- Loader HTML, CSS, JS -->
 <div id="serverLoader" class="loader-overlay" style="display: none;">
     <div class="spinner-border text-primary" role="status">
         <span class="visually-hidden">Loading...</span>
     </div>
 </div>
 <style>
     .loader-overlay {
         position: fixed;
         top: 0;
         left: 0;
         width: 100%;
         height: 100%;
         background-color: rgba(0, 0, 0, 0.5);
         display: flex;
         align-items: center;
         justify-content: center;
         z-index: 9999; /* Ensure it's above other elements */
     }
 </style>
  <script type="text/javascript">
      function showServerLoader() {
          document.getElementById('serverLoader').style.display = 'flex';
      }

      function hideServerLoader() {
          document.getElementById('serverLoader').style.display = 'none';
      }

      var lastFocusedElement;
      var prm = Sys.WebForms.PageRequestManager.getInstance();
      prm.add_beginRequest(function () {
          showServerLoader();

      });

      prm.add_endRequest(function () {
          hideServerLoader();
      });
  </script>


  <%-- END OF LODER CONTENT --%>

    <div class="page-header">
        <h3 class="page-title">Due and Paid Data Importing</h3>
    </div>
     
            <div class="row">
                <!-- Associate Registration Form -->
                <div class="grid-margin stretch-card w-100">
                    <div class="card">
                        <div class="card-body">
                            <div class="row">

                                <div class="col-md-6">
                                    <asp:Label ID="lblFIleName" runat="server" CssClass="form-label" Text="File Name: <span class='text-danger'>*</span>"></asp:Label>
                                    <div class=" d-flex">
                                        <asp:FileUpload ID="fileInput" runat="server" CssClass="form-control mr-4" OnChanged="FileInput_Changed" />
                                        <asp:Button ID="uploadButton" runat="server" Text="Upload" OnClick="UploadButton_Click" CssClass="btn btn-primary" />
                                        <asp:Label ID="lblMessage" runat="server"></asp:Label>
                                    </div>
                                </div>

                                <!-- Excel Sheet Dropdown Section -->
                                <div class="col-md-2">
                                    <label for="excelSheetSelect" class="form-label">Excel Sheet: <span class='text-danger'>*</span></label>
                                    <asp:DropDownList ID="excelSheetSelect" runat="server" CssClass="form-select" OnSelectedIndexChanged="ExcelSheetSelect_SelectedIndexChanged" AutoPostBack="true">
                                        <asp:ListItem Text="Select Sheet" Value="" />
                                        <asp:ListItem Text="Select 1" Value="1" />
                                        <asp:ListItem Text="Select 2" Value="2" />
                                    </asp:DropDownList>
                                </div>

                                <!-- TextBox to display headers -->
                                <div class="col-md-6" style="display: none">
                                    <label for="headerTextBox">Sheet Headers:</label>
                                    <asp:TextBox ID="headerTextBox" runat="server" CssClass="form-control" ReadOnly="true" />
                                </div>

                                <!-- Month Dropdown -->
                                <div class="col-md-2">
                                    <label for="monthSelect" class="form-label">Month</label>
                                    <asp:DropDownList ID="monthSelect" CssClass="form-select" runat="server">
                                        <asp:ListItem Value="">Select Month</asp:ListItem>
                                        <asp:ListItem Value="1">January</asp:ListItem>
                                        <asp:ListItem Value="2">February</asp:ListItem>
                                        <asp:ListItem Value="3">March</asp:ListItem>
                                        <asp:ListItem Value="4">April</asp:ListItem>
                                        <asp:ListItem Value="5">May</asp:ListItem>
                                        <asp:ListItem Value="6">June</asp:ListItem>
                                        <asp:ListItem Value="7">July</asp:ListItem>
                                        <asp:ListItem Value="8">August</asp:ListItem>
                                        <asp:ListItem Value="9">September</asp:ListItem>
                                        <asp:ListItem Value="10">October</asp:ListItem>
                                        <asp:ListItem Value="11">November</asp:ListItem>
                                        <asp:ListItem Value="12">December</asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <!-- Year Dropdown -->
                                <div class="col-md-2">
                                    <label for="yearSelect" class="form-label">Year</label>
                                    <asp:DropDownList ID="yearSelect" CssClass="form-select" runat="server">
                                        <asp:ListItem>Select Year</asp:ListItem>
                                        <asp:ListItem>2022</asp:ListItem>
                                        <asp:ListItem>2023</asp:ListItem>
                                        <asp:ListItem>2024</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <!-- Display Import Count -->
                            <div class="row mt-4">
                                <!-- Sheet Headers Section -->
                                <div class="col-md-6 ">
                                    <div class=" row md-12 mt-3">
                                        <div class="col-md-6 mt-1">
                                            <label for="sheetHeaderCount" class="form-label">Import Count</label>
                                            <br />
                                            <asp:TextBox CssClass="form-control" runat="server" ID="txtSheetHeaderCount" Text="0" ReadOnly="true" />

                                        </div>
                                        <div class="col-md-6 mt-1">
                                            <label for="ddlImportDataType" class="form-label">Import Data Type <span class='text-danger'>*</span></label>
                                            <asp:DropDownList ID="ddlImportDataType" CssClass="form-select" runat="server" OnSelectedIndexChanged="ddlDataTypeSelect_SelectedIndexChanged" AutoPostBack="true" OnClientClick="showLoader();">
                                                <asp:ListItem Text="Select Type" Value=""></asp:ListItem>
                                                <asp:ListItem Text="Due" Value="DUE"></asp:ListItem>
                                                <asp:ListItem Text="Lapsed" Value="LAPSED"></asp:ListItem>
                                                <asp:ListItem Text="Paid" Value="PAID"></asp:ListItem>
                                                <asp:ListItem Text="Reinstate" Value="REINS"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-md-12 mt-3">
                                        <label for="ddlFillPrevious" class="form-label">Previous Mapped <span class='text-danger'>*</span></label>
                                        <asp:DropDownList ID="ddlFillPrevious2" CssClass="form-select" runat="server">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-md-12 mt-3 ">
                                        <label for="ddDataType" class="form-label">Data Type <span class='text-danger'>*</span></label>
                                        <br />
                                        <asp:RadioButton ID="chkOptDue" runat="server" Text="Due Data" GroupName="DataTypeGroup" CssClass="gap-2 form-check-input" />
                                        <asp:RadioButton ID="chkOptPaid" runat="server" Text="Paid Data" GroupName="DataTypeGroup" CssClass="gap-2 form-check-input" />
                                        <asp:RadioButton ID="chkOptLap" runat="server" Text="Lapsed Data" GroupName="DataTypeGroup" CssClass="gap-2 form-check-input" />
                                        <asp:RadioButton ID="chkOptRein" runat="server" Text="Reinstate Data" GroupName="DataTypeGroup" CssClass="gap-2 form-check-input" />

                                    </div>

                                </div>
                                <div class="col-md-6">
                                    <div class="bg-light p-3 rounded" style="border: 1px solid #ccc; height: 250px; overflow-y: auto;">

                                        <div id="sheetHeaderListHead" runat="server">
                                            <asp:Label ID="lblsheetHeaderListHead" runat="server" CssClass="form-label mb-2 fw-bold" Text=""></asp:Label>

                                        </div>
                                        <div id="sheetHeaderListBody" runat="server">
                                            <asp:Label ID="lblsheetHeaderListBody" runat="server" CssClass="form-label" Text=""></asp:Label>

                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="row mt-4">
                                <div class="col-md-12">
                                    <%--<asp:Button ID="btnMapFields" CssClass="btn btn-outline-primary" Text="Map Fields" runat="server" OnClientClick="openMapFieldsModal(); return false;" />--%>
                                    <asp:Button ID="btnMapFields" CssClass="btn btn-outline-primary" Text="Map Fields" runat="server" OnClick="Map_Click" Enabled="true" />


                                    <%--<asp:Button class="btn btn-outline-primary" runat="server" Text="See Mapping Plan" OnClick="MappingPlanButton_Click" />--%>
                                    <asp:Button ID="btnImport" class="btn btn-outline-success m-1" runat="server" Text="Import" OnClick="ImportButton_Click" Enabled="true" />
                                    <asp:Button ID="btnExport" class="btn btn-outline-primary m-1" runat="server" Text="Export Imported Policy" OnClick="ExportImportedPolicyButton_Click" />
                                    <asp:Button ID="btnReloadPage" class="btn btn-outline-warning m-1" runat="server" Text="Reset" OnClick="btnReloadPage_Click" />
                                    <asp:Button class="btn btn-outline-danger me-4" runat="server" Text="Exit" OnClick="ExitButton_Click" />

                                </div>
                            </div>


                            <div class="row mt-4">
                                <div class="col-md-6">
                                    <div class="d-flex flex-column" style="height: 100%; justify-content: flex-end;">

                                        <asp:Label runat="server" ID="lblMesageShow" CssClass="form-label fw-bold  " Text="Reading:" Style="border-radius: 5px" class="bg-success" />
                                    </div>
                                </div>
                            </div>

                            <div class="row " style="overflow: auto; height: 350px">

                                <asp:GridView ID="GridView1" runat="server" CssClass="table table-bordered" AutoGenerateColumns="true"></asp:GridView>
                            </div>

                        </div>
                    </div>
                </div>
            </div>
       

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" >
        <ProgressTemplate>
            <!-- Loading indicator content -->
            <div class="loading">Processing, please wait...</div>
        </ProgressTemplate>
    </asp:UpdateProgress>



</asp:Content>
