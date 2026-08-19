window.onload = function () {
    document.getElementById("datas").style.display = "none";
    document.getElementById("btn1").style.display = "none";
    document.getElementById("txt-remark1").style.display = "none";
    document.getElementById("btn2").style.display = "none";
    document.getElementById('txt-remark').value = '';

    loadMainMenu();
};

// Helper to generate the exact API path based on hosting environment
function getApiUrl() {
    if (document.location.hostname === 'localhost') {
        return "/Rent/getAPIData12";
    } else {
        var root = typeof rootValue !== 'undefined' ? rootValue : "YourAppName";
        return "/" + root + "/Rent/getAPIData12";
    }
}
// Helper utility to safely clear and lock lower hierarchy fields
function resetDropdown(elementId, defaultText) {
    $(elementId).empty().append('<option value="">' + defaultText + '</option>').prop("disabled", true);
}

// 1. Triggered automatically on window onload
function loadMainMenu() {
    debugger;

    let indata = "PROC_CUSTOMER_ACCESS^CHECKERDROP^1^1^1";

    

    $.ajax({
        type: "GET",
        url: getApiUrl(),
        data: { datas: indata },
        success: function (response) {
            let data = (typeof response === "string") ? JSON.parse(response) : response;
            let ddlMain = $("#ddl-main-menu");

            ddlMain.empty().append('<option value="">-- Select Main Menu --</option>');
            resetDropdown("#ddl-sub-menu", "-- Select Sub Menu --");
            resetDropdown("#ddl-child-menu", "-- Select Child Menu --");

            $.each(data, function (index, item) {
                ddlMain.append($('<option></option>').val(item.REQUEST_ID || item.menu_id).html(item.REQUEST_VAL || item.menu_name));
            });
        },
        error: function (xhr, status, error) {
            console.error("Main Menu Load Failed: ", error);
        }
    });
}



function loadData() {
    let requestId = $("#ddl-main-menu").val();

    // ADDED: Validation check for selected option -1

    document.getElementById("datas").style.display = "block";
    document.getElementById("btn1").style.display = "block";
    document.getElementById("btn2").style.display = "block";

    document.getElementById("txt-remark1").style.display = "block";



    if (requestId === "" || requestId === "") {
        alert("Data not available");
        document.getElementById("txt-remark1").style.display = "none";
        document.getElementById('datas').value = '';
        document.getElementById('txt-remark').value = '';
        // Optional: Hide or clear data panels if a user selects the invalid option later
        document.getElementById("datas").style.display = "none";
        $("#dynamic-fields-form").empty();
        return; // Halt further execution of the AJAX call
    }



    let indata = "PROC_CUSTOMER_ACCESS^CHECKERDATA^" + requestId + "^1^1";
    let formContainer = $("#dynamic-fields-form");

    // Display a loading spinner inside the container before the request begins
    formContainer.html(`
        <div style="display: flex; flex-direction: column; align-items: center; justify-content: center; min-height: 200px; width: 100%; text-align: center;">
            <div class="simple-loader"></div>
            <p style="margin-top: 12px; font-size: 16px; color: #555; font-weight: 500; font-family: sans-serif;">
                Loading data, please wait<span class="loading-dots">...</span>
            </p>
        </div>
    `);

    $.ajax({
        type: "GET",
        url: getApiUrl(),
        data: { datas: indata },
        success: function (response) {
            let data = (typeof response === "string") ? JSON.parse(response) : response;

            // Clear the spinner container
            formContainer.empty();

            if (!data || data.length === 0) {
                formContainer.append('<div class="alert-info">No active field structures returned for this flag block.</div>');
                return;
            }

            let firstRecord = data[0];
            let fieldCount = 0;

            $.each(firstRecord, function (key, value) {
                if (value === null || value === undefined || value === '') {
                    return true;
                }

                fieldCount++;

                let cleanLabel = key.replace(/_/g, ' ').toUpperCase();
                let normalizedKey = key.replace(/_/g, ' ').toUpperCase().trim();

                let isClobRow = (normalizedKey === 'POST ACCESS' ||
                    normalizedKey === 'DEPARTMENT ACCESS' ||
                    normalizedKey === 'EMPLOYEE ACCESS');

                let groupingClass = 'field-group field-audit';
                if (isClobRow) groupingClass += ' full-width-row';

                let displayValue = value;

                let inputHtml = '';
                if (isClobRow) {
                    inputHtml = `
                        <div class="${groupingClass}">
                            <label>${cleanLabel}</label>
                            <div id="dyn_${key.replace(/\s+/g, '_')}" 
                                 class="form-control-field clob-container" 
                                 style="min-height: 300px; max-height: 700px; overflow-y: auto; background-color: #f9f9f9; border: 1px solid #ccc; padding: 12px; border-radius: 4px; width: 100%;">
                            </div>
                        </div>`;

                    formContainer.append(inputHtml);
                    $(`#dyn_${key.replace(/\s+/g, '_')}`).html(displayValue);
                } else {
                    inputHtml = `
                        <div class="${groupingClass}">
                            <label for="dyn_${key.replace(/\s+/g, '_')}">${cleanLabel}</label>
                            <input type="text" 
                                   id="dyn_${key.replace(/\s+/g, '_')}" 
                                   name="${key}" 
                                   value="${displayValue}" 
                                   class="form-control-field" 
                                   readonly="readonly" />
                        </div>`;

                    formContainer.append(inputHtml);
                }
            });

            console.log(`Successfully allocated ${fieldCount} processing inputs dynamically.`);
        },
        error: function (xhr, status, error) {
            console.error("Dynamic Field Allocation Failed: ", error);
            formContainer.html('<div class="alert-danger">Critical error mapping dynamic dataset schema.</div>');
        }
    });
}

function validateRemark(input) {
    let value = input.value;

    // 1. Strip any leading spaces immediately
    value = value.replace(/^\s+/, '');

    // 2. Filter out anything that is NOT a letter or a space
    value = value.replace(/[^a-zA-Z\s]/g, '');

    // 3. Collapse multiple spaces into a single space
    value = value.replace(/\s{2,}/g, ' ');

    // Assign the cleaned value back to the field
    input.value = value;
}



function Confirm() {

       let UserEmployeeCode = $("#LUserId").val();
    //let UserEmployeeCode = "420568";

    let requestId = $("#ddl-main-menu").val();

    // CHANGE: Trim spaces to prevent users from just typing spaces
    let remark = $("#txt-remark").val() ? $("#txt-remark").val().trim() : "";

    // FIXED: Corrected invalid JavaScript syntax and added a mandatory return stop
    if (remark === "") {
        alert("Please fill remark.");
        $("#txt-remark").focus(); // Automatically put the cursor back in the textbox
        return false; // Stop the execution here so AJAX does not run
    }

    // 4. Standalone parameters
    // Hardcoded value or fallback to $("#LUserId").val()
    let procedureName = "PROC_CUSTOMER_ACCESS";
    let flag = "CHECKERCONFIRM";
    //let flag = "EXSISTING_DATA";

    // 5. Build final query string parameter payload matching your system layout
    let inputDataString = procedureName + "^" + flag + "^" + UserEmployeeCode + "^" + requestId + "^" + remark;

    // 6. Execute AJAX request to backend
    $.ajax({
        type: "GET",
        url: typeof getApiUrl === "function" ? getApiUrl() : "YourHandler.ashx",
        data: {
            datas: inputDataString
        },
        dataType: "json",
        success: function (response) {
            // Check if response exists and has at least one item
            if (response && response.length > 0) {
                var message = response[0].SAN;
                alert(message); // Displays "Requested..."
                full_clear();
            } else {
                alert("Data submitted successfully, but no message received.");
            }

            document.getElementById("datas").style.display = "none";
            document.getElementById("btn1").style.display = "none";
            document.getElementById("btn2").style.display = "none";
            document.getElementById("txt-remark1").style.display = "none";
        },

        error: function (xhr, status, error) {
            console.error("Submission failed: ", error);
            alert("An error occurred while saving your data.");
        }
    });
}

function full_clear() {

    document.getElementById('datas').value = '';
    document.getElementById('txt-remark').value = '';
    document.getElementById('ddl-main-menu').value = '';
}


function CancelFields() {

    let UserEmployeeCode = $("#LUserId").val();
    //let UserEmployeeCode = "420568";

    let requestId = $("#ddl-main-menu").val();

    // CHANGE: Trim spaces to prevent users from just typing spaces
    let remark = $("#txt-remark").val() ? $("#txt-remark").val().trim() : "";

    // FIXED: Corrected invalid JavaScript syntax and added a mandatory return stop
    if (remark === "") {
        alert("Please fill remark.");
        $("#txt-remark").focus(); // Automatically put the cursor back in the textbox
        return false; // Stop the execution here so AJAX does not run
    }

    // 4. Standalone parameters
    // Hardcoded value or fallback to $("#LUserId").val()
    let procedureName = "PROC_CUSTOMER_ACCESS";
    let flag = "CHECKERREJECT";
    //let flag = "EXSISTING_DATA";

    // 5. Build final query string parameter payload matching your system layout
    let inputDataString = procedureName + "^" + flag + "^" + UserEmployeeCode + "^" + requestId + "^" + remark;

    // 6. Execute AJAX request to backend
    $.ajax({
        type: "GET",
        url: typeof getApiUrl === "function" ? getApiUrl() : "YourHandler.ashx",
        data: {
            datas: inputDataString
        },
        dataType: "json",
        success: function (response) {
            // Check if response exists and has at least one item
            if (response && response.length > 0) {
                var message = response[0].SAN;
                alert(message); // Displays "Requested..."
                full_clear();
            } else {
                alert("Data submitted successfully, but no message received.");
            }

            document.getElementById("datas").style.display = "none";
            document.getElementById("btn1").style.display = "none";
            document.getElementById("btn2").style.display = "none";
            document.getElementById("txt-remark1").style.display = "none";
        },

        error: function (xhr, status, error) {
            console.error("Submission failed: ", error);
            alert("An error occurred while saving your data.");
        }
    });
}
