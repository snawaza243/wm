<%@ Page Language="C#" MasterPageFile="~/vmSite.Master" AutoEventWireup="true" 
    CodeBehind="investormerge.aspx.cs" Inherits="WM.Masters.investormerge" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <%--<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">--%>
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
<script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
<script> 
    function showValue() {
        debugger     
        var clientCode = $('#<%= hdnClientID.ClientID %>').val();        
        if (clientCode) 
            
            $.ajax({
                url: "/masters/investormerge.aspx/GetInvestorForMerge",
                method: "post",
                data: JSON.stringify({ code: clientCode }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    let { data } = JSON.parse(res.d);
                    if (data == null || data.length == 0) {
                        let rmOptions = "<tr><td colspan='8'><p class='mb-0 text-danger'>Investors not found</p></td></tr>";
                        $(".table-investor-selection tbody").html(rmOptions);
                    } else {
                        let rmOptions = data.map(investor => `
                            <tr>
                                <td data-client-code="${investor.TOTALCOUNT}" title='${investor.TOTALCOUNT}'>${investor.TOTALCOUNT}</td>
                                <td data-client-code="${investor.INVESTOR_NAME}" title='${investor.INVESTOR_NAME}'>${investor.INVESTOR_NAME}</td>
                                <td data-client-code="${investor.INV_CODE}" title='${investor.INV_CODE}'>${investor.INV_CODE}</td>
                                <td title='${(investor.ADDRESS1 == null ? "" : investor.ADDRESS1)}'>${(investor.ADDRESS1 == null ? "" : investor.ADDRESS1)}</td>
                                <td title='${(investor.ADDRESS2 == null ? "" : investor.ADDRESS2)}'>${(investor.ADDRESS2 == null ? "" : investor.ADDRESS2)}</td>
                                <td title='${(investor.CITY_NAME == null ? "" : investor.CITY_NAME)}'>${(investor.CITY_NAME == null ? "" : investor.CITY_NAME)}</td>
                                <td title='${(investor.PINCODE == null ? "" : investor.PINCODE)}'>${(investor.PINCODE == null ? "" : investor.PINCODE)}</td>
                                <td title='${(investor.KYC == null ? "" : investor.KYC)}'>${(investor.KYC == null ? "" : investor.KYC)}</td>
                            </tr>`).join('');
                        $(".table-investor-selection tbody").html(rmOptions);
                    }
                },
                error: function () {
                    console.error("An error occurred while processing your request.");
                },
            });
        
    }  
        
   
</script>
  

 
 
    <link href="../assets/css/investor.css" rel="stylesheet" />
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

       
         
      
            .btn-gradient-primary-local {
                background-image: linear-gradient(to right, #B78939 0%, #D3AC69 51%, #B78939 100%);
                padding: 15px 45px;
                text-align: center;
                text-transform: uppercase;
                transition: 0.5s;
                background-size: 200% auto;
                color: white;
                box-shadow: 0 0 20px #eee;
            }
    .

        


         

    </style>
    <style>
        /* Custom style for the blocking UI */
        #blockUI {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.7);
            z-index: 1050;
            color: white;
            text-align: center;
            padding-top: 20%;
        }
    </style>
     <div id="blockUI">
        <div class="spinner-border text-light" role="status">
            <span class="sr-only">Loading...</span>
        </div>
        <p>Please wait...</p>
    </div>
    <div class="page-header">
        <h3 class="page-title">Investor Merge</h3>
    </div>
        <div class=" row">
            <div class="col-md-12 grid-margin stretch-card">
                <div class="card">
                    <div class="card-body">

                        <h4 class="card-title">Select client whose investor to merge</h4>

                        <div class=" row g-3">
                            <div class=" col-md-4">
                                <label class=" form-label" for="branch">Branch</label>
                                <asp:DropDownList runat="server" CssClass="form-select branch-search" ID="ddlBranch">
                                    <asp:ListItem Text="Select Branch" Value=""></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class=" col-md-4">
                                <label class=" form-label" for="rm">RM</label>
                                <asp:DropDownList runat="server" CssClass="form-select" ID="ddlRM">
                                    <asp:ListItem Text="Select RM" Value=""></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class=" col-md-4">
                                <label class=" form-label" for="client">Client</label>
                                <asp:DropDownList runat="server" CssClass="form-select" OnSelectedIndexChanged="ddlClient_SelectedIndexChanged" ID="ddlClient">
                                    <asp:ListItem Text="Select Client" Value=""></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class=" col-md-3">
                                <%--<button  class=" btn btn-primary">
                                    Search
                                </button>--%>
                           <a class="btn btn-primary btn-gradient-primary" runat="server" id="btnSearch" role="button" href="SearchInvestorMerge.aspx" > Search</a>
                            </div>
                            <div class=" col-md-3"></div>
                            <div class=" col-md-3"></div>
                            <div class=" col-md-3">
                            <a class="btn  btn-primary btn-gradient-primary" runat="server" id="btnshowData" onclick="showValue()"  style="display:none" role ="button" > Show Data</a>
                                </div>

                        </div>
                    </div>
                </div>
            </div>
        </div>


        <div class=" row g-3">
            <div class="col-md-5 ">
                <div class="card ">
                    <div class=" card-body">
                        <h4 class="card-title"> Investors</h4>
                        <div style=" overflow: auto; height: 450px; ">
                            <table class="table table-investor-selection" id="investor_selection">
                                <thead>
                                    <tr>
                                        <th>Total Transaction</th>
                                        <th>Investor Name</th>
                                        <th>Investor Code</th>
                                        <th>Address 1</th>
                                        <th>Address 2</th>
                                        <th>City Name</th>
                                        <th>Pin Code</th>
                                        <th>KYC</th>
                                        
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                  <%--  <tr>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>--%>

                                </tbody>
                            </table>
                        </div>



                        <p class=" text-danger  my-4 ">Select investor from this data sheet to include it
                            into the
                            data sheets on the righ side.
                        </p>
                    </div>
                </div>
            </div>

            <div class="col-md-3 ">

                <div class="card ">
                    <div class=" card-body">
                        <button type="button" class=" btn mb-3 btn-primary  w-100" id="btnSetAsMain">Set As Main Investor</button>
                        <button type="button" class=" btn mb-3 btn-primary  w-100" id="btnSelectToMerge">Select Investor To Merge</button>
                        <div class=" fs-2 text-danger my-2 text-center">
                            <i class="mdi  mdi-chevron-right"></i>
                            <i class="mdi  mdi-chevron-right"></i>
                            <i class="mdi  mdi-chevron-right"></i>
                            <i class="mdi  mdi-chevron-right"></i>
                        </div>

                        <button type="button" class=" btn mb-3 btn-outline-primary w-100" id="btnInvestorMerge"> Merge</button>
                        <a href="/masters/investormerge" role="button" class="btn mb-3 btn-outline-primary btn-sm w-100">Reset</a>
                        <asp:Button ID="btnExit" runat="server" class="btn mb-3 btn-outline-primary btn-sm w-100" Text="Exit" OnClick="btnExit_Click" />
                    </div>
                </div>
            </div>
            <div class="col-md-4 ">
                <div class="card ">
                    <div class=" card-body">
                        <h4 class="card-title ">Selected List of Investors</h4>
                        <h5 class=" mb-3">Main</h5>
                        <div class="table-responsive mb-5">
                            <table class="table table-hover" id="main_investor">
                                <thead>
                                    <tr>
                                        <th>Total Tr.</th>
                                        <th>Investor Name</th>
                                        <th>Investor Code</th>
                                        <th>Address 1</th>
                                        <th>Address 2</th>
                                        <th>City Name</th>
                                        <th>Pin Code</th>
                                        <th>KYC</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>

                        <h5 class=" mb-3">Investors To Merge</h5>

                        <div class="table-responsive">
                            <table class="table table-hover" id="merging_investor">
                                <thead>
                                    <tr>
                                        <th>Total Tr.</th>
                                        <th>Investor Name</th>
                                        <th>Investor Code</th>
                                        <th>Address 1</th>
                                        <th>Address 2</th>
                                        <th>City Name</th>
                                        <th>Pin Code</th>
                                        <th>KYC</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>


                        <p class=" text-danger my-4 ">Double click on a row or press
                            delete key
                        </p>
                        
        

                    </div>
                </div>

            </div>
        </div>
    <asp:HiddenField runat="server" ID="hdnClientID" />
   

    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
    <Scripts>
        <asp:ScriptReference Path="~/assets/js/investor-merge.js" />
    </Scripts>
</asp:ScriptManagerProxy>   
</asp:Content>