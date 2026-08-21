

function VIEW_REPORT() {

    var status = $("#ddlStatus").val();
    var category = $("#ddlCategory").val();
    var firmId = $("#LUserId").val();
    var branchId = $("#LBranchId").val();

    if (status == "") {
        alert("Please Select Status");
        return;
    }

    if (category == "") {
        alert("Please Select Category");
        return;
    }

    let _link = (location.hostname === "localhost")
        ? "/Rent/getAPIDataRent"
        : "/" + rootValue + "/Rent/getAPIDataRent";

    var reportDataString =
        "PROC_AGREEMENT_STATUS_REPORT^REPORT^" +
        status + "^" +
        category + "^" +
        firmId + "^" +
        branchId;

    $.ajax({
        type: "GET",
        url: _link,
        data: { datas: reportDataString },

        success: function (response) {

            if (response == null || response == "") {
                alert("No Data Found");
                return;
            }

            if (typeof response === "string" &&
                response.trim() !== "") {

                response = JSON.parse(response);
            }

            sessionStorage.setItem(
                "AgreementReportData",
                JSON.stringify(response)
            );

            sessionStorage.setItem(
                "AgreementCategoryId",
                category
            );

            sessionStorage.setItem(
                "AgreementStatusId",
                status
            );

            sessionStorage.setItem(
                "AgreementStatus",
                $("#ddlStatus option:selected").text()
            );

            sessionStorage.setItem(
                "AgreementCategory",
                $("#ddlCategory option:selected").text()
            );

            sessionStorage.setItem(
                "AgreementFirmId",
                firmId
            );

            sessionStorage.setItem(
                "AgreementBranchId",
                branchId
            );

            sessionStorage.setItem(
                "AgreementBranchName",
                $("#LBranchName").val()
            );
            var branchNameDataString =
                "PROC_AGREEMENT_STATUS_REPORT^BRANCH_NAME^" +
                branchId + "^" +
                1 + "^" +
                firmId + "^" +
                1;

            $.ajax({
                type: "GET",
                url: _link,
                data: { datas: branchNameDataString },
                async: false,

                success: function (branchResponse) {

                    if (typeof branchResponse === "string" &&
                        branchResponse.trim() !== "") {

                        branchResponse = JSON.parse(branchResponse);
                    }

                    var branchName = "";

                    if (branchResponse &&
                        branchResponse.length > 0) {

                        branchName =
                            branchResponse[0].BRANCH_OFFICE_SPACE;
                    }

                    sessionStorage.setItem(
                        "AgreementBranchName",
                        branchName
                    );
                },

                error: function () {

                    sessionStorage.setItem(
                        "AgreementBranchName",
                        ""
                    );
                }
            });
           
            // SUMMARY CALL

            var summaryDataString =
                "PROC_AGREEMENT_STATUS_REPORT^SUMMARY^" +
                status + "^" +
                category + "^" +
                firmId + "^" +
                branchId;

            $.ajax({
                type: "GET",
                url: _link,
                data: { datas: summaryDataString },
                async: false,

                success: function (summaryResponse) {

                    if (typeof summaryResponse === "string" &&
                        summaryResponse.trim() !== "") {

                        summaryResponse = JSON.parse(summaryResponse);
                    }
                    console.log("SUMMARY RESPONSE");
                   
                    console.log(summaryResponse);
                    sessionStorage.setItem(
                        "AgreementSummaryData",
                        JSON.stringify(summaryResponse)
                    );
                },

                error: function () {

                    sessionStorage.setItem(
                        "AgreementSummaryData",
                        "[]"
                    );
                }
            });

            window.location.href = "/ModernRentApp/Rent/Agreestatus_Report1";
                },

               

        error: function (xhr) {

            console.log(xhr.responseText);
            alert("Error while fetching report.");
        }
    });
}


$("#btnGenerate").click(function () {
    VIEW_REPORT();
});
function bindDeclarationTable(data) {

    if (!data || data.length === 0) {
        $("#table-report").html("");
        $("#tbl-content").hide();
        return;
    }

    let html = "<thead><tr>";

    // Create Header Dynamically
    Object.keys(data[0]).forEach(function (key) {

        let columnName = key
            .replace(/_/g, " ")
            .toUpperCase();

        html += "<th>" + columnName + "</th>";
    });

    html += "</tr></thead><tbody>";

    // Create Rows Dynamically
    $.each(data, function (i, row) {

        html += "<tr>";

        Object.keys(row).forEach(function (key) {

            html += "<td>" +
                (row[key] == null ? "" : row[key]) +
                "</td>";
        });

        html += "</tr>";
    });

    html += "</tbody>";

    $("#table-report").html(html);
    $("#tbl-content").show();
}

function ExportToExcel() {

    var table = document.getElementById("tblAgreementReport");

    var workbook = XLSX.utils.table_to_book(
        table,
        { sheet: "Agreement Report" }
    );

    XLSX.writeFile(
        workbook,
        "Agreement_Report.xlsx"
    );
}

function Exit1() {
    window.location.href = "/ModernRentApp/Rent/Index";


    /*window.location.href = "/MebsCustomerApp/Customer/Index";*/

}
