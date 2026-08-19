using Microsoft.AspNetCore.Mvc;
using RENT_MVC_PROJECT.Models.MenuModel;
using RENT_MVC_PROJECT.Repository;
using System.Net;
using RENT_MVC_PROJECT.Repository;


namespace RENT_MVC_PROJECT.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DecryptionRepo _repo;
        private readonly GetDataRepo _Grepo;
        private readonly PostDataRepo _Prepo;
        private readonly string baseurl;
        private readonly string rootfolder;
        private readonly Util _drepo;

        private readonly IConfiguration _configuration;

        public object Session { get; private set; }
        public object JsonConvert { get; private set; }

        public string MainHeadID = "1";

        public HomeController(ILogger<HomeController> logger, DecryptionRepo repo, GetDataRepo grepo, PostDataRepo prepo, IConfiguration configuration, Util drepo)
        {
            _logger = logger;
            _repo = repo;
            _Grepo = grepo;
            _Prepo = prepo;
            _configuration = configuration;
            baseurl = _configuration.GetValue<string>("BasValues:BaseUrl");
            rootfolder = _configuration.GetValue<string>("BasValues:root");
            _drepo = drepo;

        }



        public IActionResult Index(string id)
        {
            HttpContext.Session.SetString("headname", "Customer");
            var processid = id;


            if (string.IsNullOrEmpty(processid))
            {
                processid = HttpContext.Session.GetString("SessionVal");
            }
            HttpContext.Session.SetString("SessionVal", processid);
            String[] resession = processid.ToString().Split("¥");    //¥
            id = resession[0];

            var pathBase = "";
            string strHeader = "";
            String[] arrStr;
            String[] arrStrCode;
            string indata = "";
            string ApiPath = "";



            if (resession.Length == 1)     // FLUTTER PORTAL 
            {
                pathBase = WebUtility.UrlDecode(id);
                //string result1 = _repo.FromHexToBase64(pathBase);
                //strHeader = _repo.DecryptStringAES(result1);

                ApiPath = "PortalSessionApi/api/PortalSession/PortalDecrypt";
                var NewSessionRes = _Prepo.ApiSessionDecrypt(pathBase, baseurl, ApiPath);
                var newsess = NewSessionRes.Result;
                var parsedData1 = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(newsess);

                //var parsedData1 = JsonConvert.DeserializeObject<dynamic>(newsess);
                strHeader = Convert.ToString(parsedData1.response);


                arrStr = strHeader.ToString().Split("|");
                arrStrCode = arrStr[2].ToString().Split("!");
                indata = arrStrCode[0];

                HttpContext.Session.SetString("EmpName", arrStr[3]);
                HttpContext.Session.SetString("ecode", arrStrCode[0]);
                HttpContext.Session.SetString("BrName", arrStr[1]);
                HttpContext.Session.SetString("UserId", arrStrCode[0]);
                HttpContext.Session.SetString("BrID", arrStr[0]);

            }

            else    //  .NET PORTAL
            {
                pathBase = WebUtility.UrlDecode(id);
                strHeader = _drepo.Decrypt(pathBase);
                arrStr = strHeader.ToString().Split("|");
                arrStrCode = arrStr[2].ToString().Split("!");
                indata = arrStrCode[0];

            }


            ApiPath = "MebsAppModuleApi/api/HRMModuleAPI/GetDataHRM/";

            ViewData["baseurl"] = baseurl;
            ViewData["root"] = rootfolder;
            string UserID = indata;
            indata = indata + "~" + pathBase;


            string flag = "EMPDTLS";

            var resData = _Grepo.GetInternalPageData(indata, flag, baseurl, ApiPath);

            resData = resData.Replace(@"{""Result"":", @"");
            resData = resData.Replace(@"]}", @"]");
            resData = resData.Replace(@"}]", @"}");
            resData = resData.Replace(@"[{", @"{");

            var parsedData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(resData);

            //var parsedData = JsonConvert.DeserializeObject<dynamic>(resData);


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

                HttpContext.Session.SetString("ecode", arrStrCode[0]);

                HttpContext.Session.SetString("UserId", arrStrCode[0]);
            }


            return View();

        }



        public IActionResult LoginPage()
        {
            ViewData["baseurl"] = baseurl;

            return View();
        }
        public IActionResult Logout(string datas)
        {
            HttpContext.Session.Clear();

            return View();

        }

        public IActionResult SessionCheck(string processid)
        {
            string decryptedData = " ";

            if (string.IsNullOrEmpty(processid))
            {
                processid = HttpContext.Session.GetString("SessionVal");
            }


            String[] resession = processid.ToString().Split("&");
            processid = resession[0];

            var pathBase = "";
            var strHeader = "";
            if (resession[1] == null)
            {

                pathBase = WebUtility.UrlDecode(processid);
                //string strHeaderN = string.Empty;
                strHeader = _drepo.Decrypt(pathBase);
                //var strHeader = _repo.DecryptStringAES(pathBase);

            }

            else
            {
                pathBase = processid;
                string result1 = _repo.FromHexToBase64(pathBase);
                strHeader = _repo.DecryptStringAES(result1);
            }


            String[] arrStr;
            String[] arrStrCode;

            arrStr = strHeader.ToString().Split("|");
            arrStrCode = arrStr[2].ToString().Split("!");

            ViewData["baseurl"] = baseurl;

            // HttpContext.Session.SetString("EmpName", arrStr[3]);
            HttpContext.Session.SetString("EmpName", arrStr[3]);
            HttpContext.Session.SetString("ecode", arrStrCode[0]);
            HttpContext.Session.SetString("BrName", arrStr[1]);
            HttpContext.Session.SetString("UserId", arrStrCode[0]);
            // HttpContext.Session.SetString("SessionVal", session);
            HttpContext.Session.SetString("BrID", arrStr[0]);

            //var passdata = arrStr[0] +"~"+ arrStr[1] + "~" + arrStrCode[0] + "~" + arrStr[3] + "~" + arrStrCode[1];
            var passdata = arrStrCode[0];


            var protocol = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}";
            var virtualpath = HttpContext.Request.Path;



            string indata = baseurl + "" + rootfolder;
            string url = indata + "/Home/Dashboard?session=" + processid;


            ViewData["baseurl"] = baseurl;
            ViewData["root"] = rootfolder;

            return RedirectToAction("closer", "Home", new { datas = url });


        }

        public IActionResult Closer(string datas)
        {
            HttpContext.Session.Clear();

            //string k = "Firefox " + datas + "";
            string k = datas;

            ViewBag.Url = k;
            return View();

        }

        public IActionResult PageNotFound()
        {
            ViewData["baseurl"] = baseurl;
            ViewData["root"] = rootfolder;
            ViewData["HeadName"] = "nopage";

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


    }
}