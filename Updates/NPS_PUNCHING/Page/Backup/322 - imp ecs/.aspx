<%@ Page Title="Import Export Excel" Language="C#" MasterPageFile="~/vmSite.Master" AutoEventWireup="true" CodeBehind="ImportExportExcel.aspx.cs" Inherits="WM.Tree.ImportExportExcel" EnableViewState="true" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link rel="shortcut icon" href="../assets/images/favicon.png" />
    <link rel="stylesheet" href="../assets/vendors/mdi/css/materialdesignicons.min.css">
    <link rel="stylesheet" href="../assets/vendors/ti-icons/css/themify-icons.css">
    <link rel="stylesheet" href="../assets/vendors/css/vendor.bundle.base.css">
    <link rel="stylesheet" href="../assets/vendors/font-awesome/css/font-awesome.min.css">
    <link rel="stylesheet" href="../assets/vendors/bootstrap-datepicker/bootstrap-datepicker.min.css">
    <link rel="stylesheet" href="../assets/css/style.css">

    <%-- CURSSOR WAIT AND FREE --%>
    <script type="text/javascript">
        function showLoadingCursor() {
            document.body.style.cursor = 'wait';
        }

        function resetCursor() {
            document.body.style.cursor = 'default';
        }
    </script>

    <%-- Number input validation --%>
    <script type="text/javascript">
        // Function to trim the input if it exceeds the max length
        function trimInput(inputElement, maxLength) {
            if (inputElement.value.length > maxLength) {
                inputElement.value = inputElement.value.substring(0, maxLength);
            }
        }

        // Function to ensure only numeric values are allowed while typing
        function validateNumericInput(inputElement) {
            // Replace all non-numeric characters with empty string
            inputElement.value = inputElement.value.replace(/\D/g, '');
        }

        // Function to prevent pasting of non-numeric characters
        function validatePaste(event, inputElement) {
            // Get pasted data
            var pastedData = (event.clipboardData || window.clipboardData).getData('text');

            // Only allow numbers in the pasted data
            if (/\D/.test(pastedData)) {
                event.preventDefault(); // Prevent paste if it's not a number
            }
        }
    </script>

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
            var loader = document.getElementById('serverLoader');
            if (loader) {
                loader.style.display = 'flex';
            }
        }

        function hideServerLoader() {
            var loader = document.getElementById('serverLoader');
            if (loader) {
                loader.style.display = 'none';
            }
        }

        // Ensure the loader hides after an UpdatePanel postback
        if (typeof (Sys) !== "undefined" && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_endRequest(function () {
                hideServerLoader();
            });
        }
    </script>

    <%-- Default date setting --%>
    <script>
        $(document).ready(function () {
            $('.date').datepicker({
                format: 'dd/mm/yyyy',
                autoclose: true,
                todayHighlight: true
            });
        });
    </script>

    <%-- Date input validation dd/mm/yyyy --%>
    <script>
        function formatAndValidateDate(input) {
            // Remove any non-numeric or non-slash characters
            let value = input.value.replace(/[^0-9]/g, '');

            // Insert '/' automatically after DD and MM
            if (value.length >= 2 && value.charAt(2) !== '/') {
                value = value.slice(0, 2) + '/' + value.slice(2);
            }
            if (value.length >= 5 && value.charAt(5) !== '/') {
                value = value.slice(0, 5) + '/' + value.slice(5);
            }

            // Restrict maximum length to 10 characters (dd/mm/yyyy)
            input.value = value.substring(0, 10);
        }
    </script>



    <div class="content-wrapper">
        <div class="page-header">
            <h3 class="page-title">Preformation - NCIS Acknowledgement to the subscriber (To be filled by POP/POP-SP)
            </h3>
        </div>
        <div class="row">
            <div class="grid-margin stretch-card">
                <div class="card">
                    <asp:UpdatePanel ID="updMain" runat="server">
                        <ContentTemplate>
                            <div class="card-body">
                                <asp:Label ID="lblNPSECSNonECSPage" runat="server" Text="Import ECS/Non ECS Transaction" CssClass="page-title" />
                                <div class="row g-3 mt-2">
                                    <%-- File Upload  --%>
                                    <div class="col-md-6">
                                        <asp:Label ID="npsEcsLblFileName" runat="server" CssClass="form-label" Text="File Name: "></asp:Label>
                                        <div class=" d-flex">
                                            <asp:FileUpload ID="NpsEcsFileInput" runat="server" CssClass="form-control mr-4" OnChanged="FileInput_Changed" />
                                            <asp:HiddenField ID="hfFileSelected" runat="server" />
                                            <script>
                                                document.addEventListener('DOMContentLoaded', function () {
                                                    document.getElementById('<%= NpsEcsFileInput.ClientID %>').addEventListener('change', function () {
                                                        document.getElementById('<%= hfFileSelected.ClientID %>').value = '1'; // File selected
                                                    });
                                                });
                                            </script>
                                            <asp:Button ID="uploadButton" runat="server" Text="Upload" OnClick="UploadButton_Click" CssClass="btn btn-primary" OnClientClick="showServerLoader(); return true;" />
                                        </div>
                                    </div>

                                    <!-- Dropdown npsEcsDdlSheetlist -->
                                    <div class="col-md-3">
                                        <asp:Label ID="npsEcsLblSheetlist" runat="server" CssClass="form-label" Text="Select Sheet: "></asp:Label>
                                        <asp:DropDownList ID="npsEcsDdlSheetlist" runat="server" CssClass="form-select"
                                            OnSelectedIndexChanged="NpsEcsExcelSheetSelect_SelectedIndexChanged"
                                            Enabled="false"
                                            AutoPostBack="true"
                                            onchange="showServerLoader();">
                                            <asp:ListItem Text="Select" Value="Sheet 1" />
                                        </asp:DropDownList>

                                    </div>

                                    <!-- Dropdown npsEcsDdlCompany -->
                                    <div class="col-md-3">
                                        <asp:Label ID="npsEcsLblCompany" runat="server" CssClass="form-label" Text="Select Company: "></asp:Label>
                                        <asp:DropDownList ID="npsEcsDdlCompany" runat="server" CssClass="form-select">
                                            <asp:ListItem Text="Select" Value="1" />
                                        </asp:DropDownList>
                                    </div>

                                    <!-- Dropdown npExsDdlStatus-->
                                    <div class="col-md-2">
                                        <asp:Label ID="npsEcsLblStatus" runat="server" CssClass="form-label" Text="Status: "></asp:Label>
                                        <asp:DropDownList ID="npExsDdlStatus" runat="server" CssClass="form-select">
                                        </asp:DropDownList>
                                    </div>

                                    <!-- Input Field Name -->
                                    <div class="col-md-4">
                                        <asp:Label ID="npsEcsLblInputField" runat="server" CssClass="form-label" Text="Input Field: "></asp:Label>
                                        <asp:TextBox ID="npsEcsInputField" runat="server" CssClass="form-control" />
                                    </div>

                                    <!-- Buttons -->
                                    <div class="col-md-6 ">
                                        <div class="d-flex justify-content-end gap-2 mt-4">

                                            <asp:Button ID="npsEcsSubmitButton" OnClientClick="showServerLoader(); return true;" runat="server" CssClass="btn btn-primary" Text="Import" OnClick="NpsEcsSubmit_Click" />
                                            <asp:Button ID="npsEcsResetButton" OnClientClick="showServerLoader(); return true;" runat="server" CssClass="btn btn-outline-primary" Text="Reset" OnClick="NpsEcsReset_Click" />
                                            <asp:Button ID="npsEcsExitButton" runat="server" CssClass="btn btn-outline-primary" Text="Exit" OnClick="NpsEcsExit_Click" />
                                        </div>
                                    </div>

                                    <!-- Success Message Label -->
                                    <div class="col-md-12">
                                        <asp:Label ID="npsEcsSuccessMessage" runat="server" CssClass="text-success" />
                                    </div>
                                </div>
                                <div class="row mt-3">
                                    <div class="table-responsive" style="overflow-y: auto; overflow-x: auto; max-height: 300px; min-height: 1px">
                                        <asp:GridView ID="GridView1" runat="server"
                                            AllowPaging="true"
                                            PageSize="10"
                                            OnPageIndexChanging="PaginationGridView1_PageIndexChanging"
                                            CssClass="table table-bordered"
                                            AutoGenerateColumns="true">
                                        </asp:GridView>
                                    </div>

                                </div>
                            </div>

                        </ContentTemplate>

                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="npsEcsSubmitButton" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="npsEcsResetButton" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="npsEcsDdlSheetlist" EventName="SelectedIndexChanged" />
                            <asp:PostBackTrigger ControlID="uploadButton" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

