window.onload = function () {


    alert('hello');
    categoryload();
};
function categoryload() {

    var flag = 'LDCATEGORY';

    let _link = (location.hostname === "localhost")
        ? "/Rent/getAPIDataRent"
        : "/" + rootValue + "/Rent/getAPIDataRent";

    let indata = "PROC_RENTCUST_PANUPD^" + flag + "^1^2^1";

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
    var flag = 'LOADBRID';

    let _link = (location.hostname === "localhost")
        ? "/Rent/getAPIDataRent"
        : "/" + rootValue + "/Rent/getAPIDataRent";

    let indata = "PROC_RENTCUST_PANUPD^" + flag + "^1^2^1";

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
