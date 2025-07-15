<%@ Page Title="" Language="C#" MasterPageFile="~/vmSite.Master" AutoEventWireup="true" CodeBehind="ClientMerge.aspx.cs" Inherits="WM.Masters.ClientMerge" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .table-responsive {
            max-height: 500px;
        }

        table i {
            cursor: pointer;
        }

        .table tr th,
        .table tr td {
            padding: 10px;
            text-align: left;
        }

        .table tr th {
            background-color: #d9d9d9;
        }

        .table tr td {
            color: #999595;
        }

        table tbody tr.selected > td {
            background-color: rgb(183, 137, 57);
            color: white;
            user-select: none;
        }

        .merge-section .client-sheet {
            min-height: 220px;
        }

        .fa-spinner {
            margin-left: 10px;
            animation: spin 1.5s linear infinite;
        }

        @keyframes spin {
            0% {
                transform: rotate(0deg);
            }

            50% {
                transform: rotate(180deg);
            }

            100% {
                transform: rotate(360deg);
            }
        }

    </style>

    <div class="page-header">
        <h3 class="page-title">Client Merging</h3>
    </div>
    <div class="row">
        <div class="grid-margin stretch-card">
            <div class="card">
                <div class="card-body">
                    <h5 class="card-title">Select client to merge</h5>
                    <div class="row g-3">
                        <div class=" col-md-6">
                            <label class="form-label">Branch</label>
                            <asp:DropDownList runat="server" ID="Branch_Dropdown" CssClass="form-select" OnSelectedIndexChanged="Branch_Dropdown_SelectedIndexChanged" AutoPostBack="true">
                                <asp:ListItem Text="Select Branch" Value=""></asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator runat="server" ErrorMessage="Select Branch" CssClass="text-danger small mt-1" ControlToValidate="Branch_Dropdown"></asp:RequiredFieldValidator>
                        </div>
                        <div class=" col-md-6">
                            <label class="form-label">Select RM (<%= RM_Dropdown.Items.Count - 1 %>)</label>
                            <asp:DropDownList runat="server" ID="RM_Dropdown" CssClass="form-select">
                                <asp:ListItem Text="Select Branch First" Value=""></asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator runat="server" ErrorMessage="Select RM" CssClass="text-danger small mt-1" ControlToValidate="RM_Dropdown"></asp:RequiredFieldValidator>
                        </div>
                        <div class="col-3">
                            <button type="button" class="btn btn-sm btn-primary w-100" data-bs-toggle="modal" data-bs-target="#searchModal"><i class="fa fa-search me-1"></i>Search</button>
                        </div>
                        <div class="col-3">
                            <button type="button" class="btn btn-sm btn-outline-primary w-100" data-bs-toggle="modal" data-bs-target="#searchModal"><i class="fa fa-search me-1"></i>Investor Search</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="row g-2 merge-section">
        <div class="col-md-5">
            <div class="card">
                <div class="card-body p-3">
                    <h4 class="card-title">Clients (<span class="total-client-count">0</span>) <span class="float-end">Selected (<span class="selected-client-count">0</span>)</span></h4>
                    <div class="table-responsive client-sheet border">
                        <table id="client_sheet" class="table">
                            <thead>
                                <tr>
                                    <th>Create Date</th>
                                    <th>Client Name</th>
                                    <th>Client Code</th>
                                    <th>Address 1</th>
                                    <th>Address 2</th>
                                    <th>City</th>
                                    <th>Branch</th>
                                    <th>Phone</th>
                                    <th>Mobile</th>
                                    <th>RM</th>
                                    <th>KYC</th>
                                    <th>Last Tran Date</th>
                                    <th>Approved Flag</th>
                                    <th>Action</th>
                                </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>
                    <p class="text-danger mt-4 small">To select un-select 'Double,' click on the row.</p>
                    <p class="text-danger small">Select client from this data sheet to include it into the data sheets on the righ side.</p>
                </div>
            </div>
        </div>
        <div class="col-md-2">
            <div class="card h-100 pt-5">
                <div class="card-body p-2 mt-4">
                    <button id="btnSetAsMain" type="button" class="btn mb-3 btn-primary btn-sm w-100">Set As Main Client</button>
                    <button id="btnSelectToMerge" type="button" class="btn mb-3 btn-primary btn-sm w-100">Select To Merge</button>

                    <div class="fs-3 text-danger my-4 text-center">
                        <i class="mdi  mdi-chevron-right"></i>
                        <i class="mdi  mdi-chevron-right"></i>
                        <i class="mdi  mdi-chevron-right"></i>
                        <i class="mdi  mdi-chevron-right"></i>
                    </div>

                    <button type="button" id="btnClientMerge" class="btn mb-3 btn-outline-primary btn-sm w-100">Merge</button>
                    <a href="/masters/clientmerge" role="button" class="btn mb-3 btn-outline-primary btn-sm w-100">Reset</a>
                    <a href="/welcome" role="button" class="btn mb-3 btn-outline-primary btn-sm w-100">Exit</a>
                </div>
            </div>
        </div>

        <div class="col-md-5">
            <div class="card h-100">
                <div class="card-body p-3">
                    <h4 class="card-title ">Selected List of Clients</h4>
                    <h5 class="mb-3">Main (<span id="main_sheet_count">0</span>)</h5>
                    <div class="table-responsive mb-4 border">
                        <table id="main_sheet" class="table table-hover">
                            <thead>
                                <tr>
                                    <th>Create Date</th>
                                    <th>Client Name</th>
                                    <th>Client Code</th>
                                    <th>Address 1</th>
                                    <th>Address 2</th>
                                    <th>City</th>
                                    <th>Branch</th>
                                    <th>Phone</th>
                                    <th>Mobile</th>
                                    <th>RM</th>
                                    <th>KYC</th>
                                    <th>Last Tran Date</th>
                                    <th>Approved Flag</th>
                                    <th>Action</th>
                                </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>
                    <h5 class="mb-3">Clients To Merge (<span id="other_sheet_count">0</span>)</h5>
                    <div class="table-responsive border">
                        <table id="other_sheet" class="table">
                            <thead>
                                <tr>
                                    <th>Create Date</th>
                                    <th>Client Name</th>
                                    <th>Client Code</th>
                                    <th>Address 1</th>
                                    <th>Address 2</th>
                                    <th>City</th>
                                    <th>Branch</th>
                                    <th>Phone</th>
                                    <th>Mobile</th>
                                    <th>RM</th>
                                    <th>KYC</th>
                                    <th>Last Tran Date</th>
                                    <th>Approved Flag</th>
                                    <th>Action</th>
                                </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>
                </div>
            </div>

        </div>
    </div>

    <!--Search Modal -->
    <div class="modal fade" id="searchModal" tabindex="-1" aria-labelledby="searchModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content bg-white">
                <div class="modal-header">
                    <h5 class="modal-title" id="searchModalLabel">Client/Agent Search</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row g-2 client-search-section">
                        <div class="col-3">
                            <label class="form-label small">Category</label>
                            <input type="text" id="categorySearch" class="form-control form-control-sm bg-light" value="Client" readonly="readonly" />
                        </div>
                        <div class="col-3">
                            <label class="form-label small">Branch</label>
                            <asp:DropDownList runat="server" ID="BranchSearch_Dropdown" CssClass="form-control form-control-sm branch-search">
                                <asp:ListItem Text="Select Branch" Value=""></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-3">
                            <label class="form-label small">RM</label>
                            <select id="RMSearch_Dropdown" class="form-control form-control-sm">
                                <option value="">Select Branch First</option>
                            </select>
                        </div>
                        <div class="col-3">
                            <label class="form-label small">City</label>
                            <asp:DropDownList runat="server" ID="CitySearch_Dropdown" CssClass="form-control form-control-sm city-search">
                                <asp:ListItem Text="Select City" Value=""></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-3">
                            <label class="form-label small">Client Code</label>
                            <input type="text" id="clientCodeSearch" class="form-control form-control-sm" />
                        </div>
                        <div class="col-3">
                            <label class="form-label small">Name</label>
                            <input type="text" id="clientNameSearch" class="form-control form-control-sm" />
                        </div>
                        <div class="col-3">
                            <label class="form-label small">Pan</label>
                            <input type="text" id="panNoSearch" class="form-control form-control-sm" />
                        </div>
                        <div class="col-3">
                            <label class="form-label small">Phone</label>
                            <input type="text" id="phoneSearch" class="form-control form-control-sm" />
                        </div>
                        <div class="col-3">
                            <label class="form-label small">Mobile</label>
                            <input type="text" id="mobileSearch" class="form-control form-control-sm" />
                        </div>
                        <div class="col-3">
                            <label class="form-label small">Address 1</label>
                            <input type="text" id="address1Search" class="form-control form-control-sm" />
                        </div>
                        <div class="col-3">
                            <label class="form-label small">Address 2</label>
                            <input type="text" id="address2Search" class="form-control form-control-sm" />
                        </div>
                        <div class="col-3">
                            <label class="form-label small">Sort by</label>
                            <select id="sortBy2Search" class="form-control form-control-sm">
                                <option value="">Select Sort</option>
                                <option value="CATNAME_NAME" selected>Name</option>
                                <option value="ADDRESS1">Address 1</option>
                                <option value="ADDRESS2">Address 2</option>
                                <option value="CITY_NAME">City</option>
                                <option value="PHONE">Phone</option>
                            </select>
                        </div>
                        <div class="col-3 pt-4">
                            <div class="mt-1">
                                <button id="btnSearh" type="button" class="btn btn-primary btn-sm"><i class="fa fa-search me-1"></i>Search</button>
                                <button id="btnSearhReset" type="button" class="btn btn-outline-primary btn-sm ms-2">Reset</button>
                                <button type="button" class="btn btn-outline-primary btn-sm ms-2" data-bs-dismiss="modal">Exit</button>
                            </div>
                        </div>
                    </div>
                    <div class="row mt-3">
                        <div class="col-12">
                            <p class="small text-primary mb-1">To select 'Double,' click on the row.</p>
                            <div class="table-responsive border">
                                <table id="searchTable" class="table">
                                    <thead>
                                        <tr>
                                            <th>Client/Agent Name</th>
                                            <th>Address 1</th>
                                            <th>Address 2</th>
                                            <th>City</th>
                                            <th>Branch</th>
                                            <th>Phone</th>
                                            <th>Mobile</th>
                                            <th>RM</th>
                                            <th>Account Status</th>
                                            <th>Create Date</th>
                                            <th>Last Tran Date</th>
                                            <th>Approved Flag</th>
                                        </tr>
                                    </thead>
                                    <tbody></tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Modal -->
    <div class="modal fade" id="panUpdateModal" tabindex="-1" aria-labelledby="panUpdateModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content bg-white">
                <div class="modal-header">
                    <h5 class="modal-title" id="panUpdateModalLabel">Pan Update Form</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="table-responsive" style="max-height:400px;">
                        <table id="panUpdateTable" class="table">
                            <thead>
                                <tr>
                                    <th>Inv Code</th>
                                    <th>Investor Name</th>
                                    <th>Pan</th>
                                </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-outline-primary btn-sm" data-bs-dismiss="modal">Exit</button>
                </div>
            </div>
        </div>
    </div>
   <script src="/assets/js/client-merege.js"></script>
</asp:Content>
