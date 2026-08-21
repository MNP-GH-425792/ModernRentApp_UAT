window.onload = function () {
    categoryload();
};
function categoryload() {
    $('#ddlBranch').empty();
    $('#ddlBuilding').empty();
    $('#ddlCustomer').empty();
    var flag = 'LDCATEGORY';

    let _link = (location.hostname === "localhost")
        ? "/Rent/getAPIDataRent"
        : "/" + rootValue + "/Rent/getAPIDataRent";

    let indata = "PROC_RENTCUST_PANUPD^" + flag + "^1^1^1";

    $.ajax({
        type: "GET",
        url: _link,
        data: { datas: indata },
        dataType: "json",

        success: function (response) {

            $("#ddlCategory").empty();

            $.each(response, function (i, item) {

                $("#ddlCategory").append(
                    $("<option></option>")
                        .val(item.RENT_CATEGORY_ID)
                        .text(item.RENT_CATEGORY_NAME)
                );

            });

        },
        error: function (xhr) {
            console.log(xhr.responseText);
            alert("Error loading category");
        }
    });
}
function branchLoad()
{
    $("#ddlBranch").empty();
    $("#ddlBuilding").empty();
    $("#ddlCustomer").empty();

    $("#txtAddress").val("");
    $("#txtPhoneNumber").val("");
    $("#txtPanNumber").val("");
    
    var brid = $("#LBranchId").val();
    var USRID = $("#LUserId").val();
    var firmid = '1';
    
    var categoryId = $("#ddlCategory").val();
    var FirstValue = categoryId.split('~')[0];
    
    var flag = 'LOADBRID';

    let _link = (location.hostname === "localhost")
        ? "/Rent/getAPIDataRent"
        : "/" + rootValue + "/Rent/getAPIDataRent";

    let indata = "PROC_RENTCUST_PANUPD^" + flag + "^" + firmid + "^" + FirstValue+"^"+1;

    $.ajax({
        type: "GET",
        url: _link,
        data: { datas: indata },
        dataType: "json",

        success: function (response) {

            $("#ddlBranch").empty();

            $.each(response, function (i, item) {

                $("#ddlBranch").append(
                    $("<option></option>")
                        .val(item.BRANCH_ID)
                        .text(item.BRANCH_NAME)
                );

            });

        },
        error: function (xhr) {
            console.log(xhr.responseText);
            alert("Error loading Branch");
        }
    });
}
function loadBuilding()
{
    $("#ddlBuilding").empty();
    $("#ddlCustomer").empty();

    $("#txtAddress").val("");
    $("#txtPhoneNumber").val("");
    $("#txtPanNumber").val("");
    var brid = $("#LBranchId").val();
    var USRID = $("#LUserId").val();
    var firmid = '1';

    var categoryId = $("#ddlCategory").val();
    var FirstValue = categoryId.split('~')[0];

    var flag = 'LOADBUILD';
    let _link = (location.hostname === "localhost")
        ? "/Rent/getAPIDataRent"
        : "/" + rootValue + "/Rent/getAPIDataRent";

    let indata = "PROC_RENTCUST_PANUPD^" + flag + "^" + firmid + "^" + FirstValue + "^" + brid;
    $.ajax({
        type: "GET",
        url: _link,
        data: { datas: indata },
        dataType: "json",
        success: function (response) {

            $("#ddlBuilding").empty();

            $.each(response, function (i, item) {

                $("#ddlBuilding").append(
                    $("<option></option>")
                        .val(item.FLAT_NO)
                        .text(item.FLATNAME)
                );

            });

        },
        error: function (xhr) {
            console.log(xhr.responseText);
            alert("Error loading Building");
        }
    });
}
function loadCustomer()
{
    

    $("#txtAddress").val("");
    $("#txtPhoneNumber").val("");
    $("#txtPanNumber").val("");
    var brid = $("#LBranchId").val();
    var USRID = $("#LUserId").val();
    var firmid = '1';

    var categoryId = $("#ddlCategory").val();
    var FirstValue = categoryId.split('~')[0];
    var flat = $("#ddlBuilding").val();
    var flatno = flat.split('~')[1];
    var val = brid + "~" + flat;
    var flag = 'CUSTLOAD';
    let _link = (location.hostname === "localhost")
        ? "/Rent/getAPIDataRent"
        : "/" + rootValue + "/Rent/getAPIDataRent";

    let indata = "PROC_RENTCUST_PANUPD^" + flag + "^" + firmid + "^" + FirstValue + "^" + val;
    $.ajax({
        type: "GET",
        url: _link,
        data: { datas: indata },
        dataType: "json",

        success: function (response) {

            $("#ddlCustomer").empty();

            $.each(response, function (i, item) {

                $("#ddlCustomer").append(
                    $("<option></option>")
                        .val(item.CUSTOMER_ID)
                        .text(item.CUSTOMER_NAME)
                );

            });

        },
        error: function (xhr) {
            console.log(xhr.responseText);
            alert("Error loading Customer");
        }
    });
}
function loadDetails() {

    var firmid = '1';

    // Customer ID directly
    var cust1 = $("#ddlCustomer").val();

    var flag = 'CUSTADRS';

    let _link = (location.hostname === "localhost")
        ? "/Rent/getAPIDataRent"
        : "/" + rootValue + "/Rent/getAPIDataRent";

    let indata = "PROC_RENTCUST_PANUPD^" + flag + "^" + firmid + "^" + cust1 + "^1";

    $.ajax({
        type: "GET",
        url: _link,
        data: { datas: indata },
        dataType: "json",

        success: function (response) {

            if (response && response.length > 0) {

                var item = response[0];

                // PAN
                $("#txtPanNumber").val(item.PAN || "");

                // Address
                $("#txtAddress").val(item.ADDRESS || "");

                // Phone Number
                var phoneNo = "";

                if (item.PHONE2 != null &&
                    item.PHONE2 != "" &&
                    item.PHONE2 != "{}") {
                    phoneNo = item.PHONE2;
                }

                $("#txtPhoneNumber").val(phoneNo);

                // Force editable
                $("#txtPanNumber").prop("readonly", false);
                $("#txtPanNumber").prop("disabled", false);

                // Remove any attributes
                document.getElementById("txtPanNumber").removeAttribute("readonly");
                document.getElementById("txtPanNumber").removeAttribute("disabled");

                // CSS safeguard
                $("#txtPanNumber").css({
                    "pointer-events": "auto",
                    "opacity": "1",
                    "background-color": "#fff"
                });

                // Focus
                setTimeout(function () {
                    $("#txtPanNumber").focus();
                }, 100);
            }
        },

        error: function (xhr) {
            console.log(xhr.responseText);
            alert("Error loading Customer Details");
        }
    });
}
function confirm()
{
    var panNumber = $("#txtPanNumber").val();
    var cust1 = $("#ddlCustomer").val();

    var flag = 'CONFIRM';

    let _link = (location.hostname === "localhost")
        ? "/Rent/getAPIDataRent"
        : "/" + rootValue + "/Rent/getAPIDataRent";

    let indata = "PROC_RENTCUST_PANUPD^" + flag + "^" + panNumber + "^" + cust1 + "^1";

    $.ajax({
        type: "GET",
        url: _link,
        data: { datas: indata },
        dataType: "json",

        success: function (response) {

            if (response && response.length > 0) {

                alert(response[0].RESULT);

                if (response[0].RESULT === "UPDATED SUCCESSFULLY") {

                    

              

                    // Optional Redirect
                    window.location.href = "/ModernRentApp/Rent/rentcustomerpanupdate";
                }
            }
        },

        error: function (xhr) {
            console.log(xhr.responseText);
            alert("Error while updating PAN");
        }
    });
}

function exit() {
    window.location.href = "/ModernRentApp/Rent/Index";
}