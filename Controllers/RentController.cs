
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;
using System.Diagnostics;
using RENT_MVC_PROJECT.Repository;
using Microsoft.AspNetCore.Authorization;
using RENT_MVC_PROJECT.Controllers;
using RENT_MVC_PROJECT.Models.MenuModel;
//using RENT_MVC_PROJECT.Repository;

namespace RENT_MVC_PROJECT.Controllers
{
    public class RentController : Controller
    {


        private readonly ILogger<HomeController> _logger;
        private readonly DecryptionRepo _repo;
        private readonly GetDataRepo _Grepo;
        private readonly PostDataRepo _Prepo;
        private readonly Util _drepo;

        private readonly string baseurl;
        private readonly string rootfolder;

        private readonly IConfiguration _configuration;
        public string MainHeadID = "11";


        public RentController(ILogger<HomeController> logger, DecryptionRepo repo, Util drepo, GetDataRepo grepo, PostDataRepo Prepo, IConfiguration configuration)
        {
            _logger = logger;
            _repo = repo;
            _drepo = drepo;

            _Grepo = grepo;
            _Prepo = Prepo;
            _configuration = configuration;
            baseurl = _configuration.GetValue<string>("BasValues:BaseUrl");
            rootfolder = _configuration.GetValue<string>("BasValues:root");
        }
        public object Session { get; private set; }


        //-------------- index page ---------------------
        //-------------- index page ---------------------
        public IActionResult Index(string id)
        {
            HttpContext.Session.SetString("headname", "Customer");

            var processid = id;

            if (string.IsNullOrEmpty(processid))
            {
                processid = HttpContext.Session.GetString("SessionVal");
            }

            if (string.IsNullOrEmpty(processid))
            {
                return Content("Error: Session identifier is missing.");
            }

            HttpContext.Session.SetString("SessionVal", processid);
            string[] resession = processid.ToString().Split("¥");    //¥
            id = resession[0];

            var pathBase = "";
            var strHeader = "";
            string[] arrStr = Array.Empty<string>();
            string[] arrStrCode = Array.Empty<string>();
            string indata = "";
            string UserID = "";
            string ApiPath = "";

            if (resession.Length == 1)     // FLUTTER PORTAL 
            {
                pathBase = WebUtility.UrlDecode(id);
                ApiPath = "PortalSessionApi/api/PortalSession/PortalDecrypt";

                var NewSessionRes = _repo.ApiSessionDecrypt(pathBase, baseurl, ApiPath);
                var newsess = NewSessionRes.Result;

                // Safety Check: Verify that the API actually returned data
                if (string.IsNullOrEmpty(newsess))
                {
                    return Content("Error: The decryption API returned an empty or null session string.");
                }

                var parsedData1 = JsonConvert.DeserializeObject<dynamic>(newsess);
                if (parsedData1 == null || parsedData1.response == null)
                {
                    return Content("Error: Failed to parse session json structure, response node is missing.");
                }

                strHeader = Convert.ToString(parsedData1.response);

                arrStr = strHeader.ToString().Split("|");
                if (arrStr.Length < 3) return Content("Error: Decrypted string format is invalid.");

                arrStrCode = arrStr[2].ToString().Split("!");
                indata = arrStrCode[0];

                HttpContext.Session.SetString("ecode", arrStrCode[0]);
                HttpContext.Session.SetString("BrName", arrStr[1]);
                HttpContext.Session.SetString("UserId", arrStrCode[0]);
                HttpContext.Session.SetString("BrID", arrStr[0]);
                HttpContext.Session.SetString("UserSession", newsess);
            }
            else    //  .NET PORTAL
            {
                pathBase = WebUtility.UrlDecode(id);
                strHeader = _drepo.Decrypt(pathBase);

                if (string.IsNullOrEmpty(strHeader))
                {
                    return Content("Error: Portal decryption returned no data.");
                }

                arrStr = strHeader.ToString().Split("|");
                if (arrStr.Length < 3) return Content("Error: Decrypted data structure mismatch.");

                arrStrCode = arrStr[2].ToString().Split("!");
                indata = arrStrCode[0];
            }

            UserID = indata;
            ApiPath = "MebsAppModuleApi/api/HRMModuleAPI/GetDataHRM/";

            ViewData["baseurl"] = baseurl;
            ViewData["root"] = rootfolder;

            indata = indata + "~" + pathBase;
            string flag = "EMPDTLS";

            var resData = _Grepo.GetInternalPageData(indata, flag, baseurl, ApiPath);

            if (string.IsNullOrEmpty(resData))
            {
                return Content("Error: No employee details data found from internal API.");
            }

            resData = resData.Replace(@"{""Result"":", @"");
            resData = resData.Replace(@"]}", @"]");
            resData = resData.Replace(@"}]", @"}");
            resData = resData.Replace(@"[{", @"{");

            var parsedData = JsonConvert.DeserializeObject<dynamic>(resData);
            if (parsedData == null) return Content("Error: Failed to deserialize employee details mapping.");

            var res = parsedData.RES;

            if (res == "99")
            {
                return RedirectToRoute(new { controller = "Home", action = "closer" });
            }
            else
            {
                string bid = Convert.ToString(parsedData.BRANCH_ID);
                string bname = Convert.ToString(parsedData.BRANCH_NAME);
                string ename = Convert.ToString(parsedData.EMP_NAME);
                string accid = Convert.ToString(parsedData.ACCESS_ID);
                string frmid = Convert.ToString(parsedData.FIRM_ID);
                string depid = Convert.ToString(parsedData.DEPARTMENT_ID);

                HttpContext.Session.SetString("BrID", bid);
                HttpContext.Session.SetString("BrName", bname);
                HttpContext.Session.SetString("EmpName", ename);

                if (arrStrCode.Length > 0)
                {
                    HttpContext.Session.SetString("ecode", arrStrCode[0]);
                    HttpContext.Session.SetString("UserId", arrStrCode[0]);
                }
            }

            MenuListModel model = new MenuListModel();
            model = (MenuListModel)_Grepo.GetMainMenuData(UserID, baseurl, MainHeadID);

            return View(model);
        }

        public string getAPIData1(string datas)
        {


            string ApiPath = "customer_report_api/api/CustomerReportAPI/GetCustomerData/";


            string[] DataArray = datas.Split('^');

            string flag = DataArray[0];
            string indata = DataArray[1];
            string indata1 = DataArray[2];//emp code
            string indata3 = DataArray[3];//
            string indata4 = DataArray[4];

            var resData = _Grepo.GetInternalPageData1(indata, indata1, indata3, indata4, flag, baseurl, ApiPath);
            return resData;
        }


        public string getAPIData12(string datas)



        {
            string[] DataArray = datas.Split('^');
            string ApiPath = "MebsAmlApi/api/AmlModuleAPI/GetDataAml/";

            string flag = DataArray[0];
            string indata = DataArray[1];
            string indata1 = DataArray[2];//emp code
            string indata3 = DataArray[3];
            string indata4 = DataArray[4];

            var resData = _Grepo.GetInternalPageData1(indata, indata1, indata3, indata4, flag, baseurl, ApiPath);
            return resData;
        }
        public string getAPIDataRent(string datas)



        {
            string[] DataArray = datas.Split('^');
            string ApiPath = "ModernRentAPI/api/ModernRentAPI/GetRentData/";
            //string ApiPath = "https://localhost:44374/api/Menu/GetRentData?pFlag=PROC_RENTCUST_PANUPD&pPageVal=LDCATEGORY&pParaVal=1&pParaVal2=2";
            string flag = DataArray[0];
            string indata = DataArray[1];
            string indata1 = DataArray[2];//emp code
            string indata3 = DataArray[3];
            string indata4 = DataArray[4];

            var resData = _Grepo.GetInternalPageDataRent(indata, indata1, indata3, indata4, flag, baseurl, ApiPath);
            return resData;
        }


        public async Task<dynamic> postAPIData(string datas)

        {

            string ApiPath = "customer_report_api/api/CustomerReportAPI/GetCustomerData/";
            //string ApiPath = "/api/AmlModuleAPI/PostDataAML";
            string[] DataArray = datas.Split('^');

            string flag = DataArray[0];
            string indata = DataArray[1];
            string indata1 = DataArray[2];
            string indata2 = DataArray[3];
            string indata3 = DataArray[4];
            string indata4 = DataArray[5];
            var response = await _Prepo.PostInternalPageData1(
                indata, indata1, indata2, indata3, indata4, flag, baseurl, ApiPath);

            return response.ToString();
        }


        public async Task<JsonResult> GetAPIData2(string custid)
        {
            string apiPath = "https://ind-orion.hyperverge.co/v2/application/delete/hard";
            var resData = await _Grepo.GetInternalPageData2(custid, apiPath);
            return Json(resData);
        }





       
        

        public IActionResult checker(string datas)
        {

            ViewData["baseurl"] = baseurl;
            ViewData["root"] = rootfolder;
            ViewData["HeadName"] = datas;

            ViewData["user"] = HttpContext.Session.GetString("ecode");
            var empcode = HttpContext.Session.GetString("ecode");
            var empname = HttpContext.Session.GetString("EmpName");
            var branchname = HttpContext.Session.GetString("BrName");
            var UserId = HttpContext.Session.GetString("UserId");
            var brID = HttpContext.Session.GetString("BrID");

            ViewData["BrID"] = brID;
            MenuListModel model = new MenuListModel();
            model = (MenuListModel)_Grepo.GetMainMenuData(UserId, baseurl, MainHeadID);

            ViewData["EmpCode"] = UserId;
            ViewData["EmpName"] = empname;
            return View(model);

        }

        public IActionResult rentcustomerpanupdate(string datas)
        {

            ViewData["baseurl"] = baseurl;
            ViewData["root"] = rootfolder;
            ViewData["HeadName"] = datas;

            ViewData["user"] = HttpContext.Session.GetString("ecode");
            var empcode = HttpContext.Session.GetString("ecode");
            var empname = HttpContext.Session.GetString("EmpName");
            var branchname = HttpContext.Session.GetString("BrName");
            var UserId = HttpContext.Session.GetString("UserId");
            var brID = HttpContext.Session.GetString("BrID");

            ViewData["BrID"] = brID;
            MenuListModel model = new MenuListModel();
            model = (MenuListModel)_Grepo.GetMainMenuData(UserId, baseurl, MainHeadID);

            ViewData["EmpCode"] = UserId;
            ViewData["EmpName"] = empname;
            return View(model);

        }

        public IActionResult Agreestatus_Report(string datas)
        {

            ViewData["baseurl"] = baseurl;
            ViewData["root"] = rootfolder;
            ViewData["HeadName"] = datas;

            ViewData["user"] = HttpContext.Session.GetString("ecode");
            var empcode = HttpContext.Session.GetString("ecode");
            var empname = HttpContext.Session.GetString("EmpName");
            var branchname = HttpContext.Session.GetString("BrName");
            var UserId = HttpContext.Session.GetString("UserId");
            var brID = HttpContext.Session.GetString("BrID");

            ViewData["BrID"] = brID;
            MenuListModel model = new MenuListModel();
            model = (MenuListModel)_Grepo.GetMainMenuData(UserId, baseurl, MainHeadID);

            ViewData["EmpCode"] = UserId;
            ViewData["EmpName"] = empname;
            return View(model);

        }

        public IActionResult Agreestatus_Report1(string datas)
        {

            ViewData["baseurl"] = baseurl;
            ViewData["root"] = rootfolder;
            ViewData["HeadName"] = datas;

            ViewData["user"] = HttpContext.Session.GetString("ecode");
            var empcode = HttpContext.Session.GetString("ecode");
            var empname = HttpContext.Session.GetString("EmpName");
            var branchname = HttpContext.Session.GetString("BrName");
            var UserId = HttpContext.Session.GetString("UserId");
            var brID = HttpContext.Session.GetString("BrID");

            ViewData["BrID"] = brID;
            MenuListModel model = new MenuListModel();
            model = (MenuListModel)_Grepo.GetMainMenuData(UserId, baseurl, MainHeadID);

            ViewData["EmpCode"] = UserId;
            ViewData["EmpName"] = empname;
            return View(model);

        }
        public IActionResult RentProcessedList(string datas)
        {

            ViewData["baseurl"] = baseurl;
            ViewData["root"] = rootfolder;
            ViewData["HeadName"] = datas;

            ViewData["user"] = HttpContext.Session.GetString("ecode");
            var empcode = HttpContext.Session.GetString("ecode");
            var empname = HttpContext.Session.GetString("EmpName");
            var branchname = HttpContext.Session.GetString("BrName");
            var UserId = HttpContext.Session.GetString("UserId");
            var brID = HttpContext.Session.GetString("BrID");

            ViewData["BrID"] = brID;
            MenuListModel model = new MenuListModel();
            model = (MenuListModel)_Grepo.GetMainMenuData(UserId, baseurl, MainHeadID);

            ViewData["EmpCode"] = UserId;
            ViewData["EmpName"] = empname;
            return View(model);

        }
        [HttpPost]
        public IActionResult LoadReportPage(string month, string year)
        {

            string indataString = "PROC_RENTPROCESS_REPORT^VIEW_LIST^" + month + "^" + year + "^1";

            string rawJsonResult = getAPIDataRent(indataString);
            var reportDataList = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, string>>>(rawJsonResult);
            ViewBag.SelectedMonth = month;
            ViewBag.SelectedYear = year;
            ViewBag.BranchID = HttpContext.Session.GetString("BrID") ?? "0";
            ViewBag.BranchName = HttpContext.Session.GetString("BrName") ?? "A.O.VALAPAD";

            return View("RentProcessedReport", reportDataList);
        }
    }

}