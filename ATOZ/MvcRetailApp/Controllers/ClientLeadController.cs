using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeFirstEntities;
using CodeFirstData;
using CodeFirstServices.Interfaces;
using MvcRetailApp.ModelBinder;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Routing;
using MvcRetailApp.Filters;

namespace MvcRetailApp.Controllers
{
    //[NoDirectAccess]
    [SessionExpireFilter]
    public class ClientLeadController : Controller
    {
       
        private string UserEmail
        {
            get { return (string)HttpContext.Session["UserEmail"]; }
            set { HttpContext.Session["UserEmail"] = value; }
        }
        private string CompanyCode
        {
            get { return (string)HttpContext.Session["CompanyCode"]; }
            set { HttpContext.Session["CompanyCode"] = value; }
        }
        private string CompanyName
        {
            get { return (string)HttpContext.Session["CompanyName"]; }
            set { HttpContext.Session["CompanyName"] = value; }
        }
        private string FinancialYear
        {
            get { return (string)HttpContext.Session["FinancialYear"]; }
            set { HttpContext.Session["FinancialYear"] = value; }
        }
        private readonly IClientMasterService _ClientMasterService;
        private readonly ICountryService _CountryService;
        private readonly IStateService _StateService;
        private readonly IDistrictService _DistrictService;
        private readonly IBankService _BankService;
        private readonly IBankNameService _BankNameService;
        private readonly IUtilityService _UtilityService;
        private readonly IClientBankDetailService _ClientBankDetailService;
        private readonly IUserCredentialService _IUserCredentialService;
        private readonly IModuleService _iIModuleService;
        private readonly IClientLeadService _ClientLeadService;



        public ClientLeadController(IClientMasterService ClientMasterService, ICountryService CountryService, IStateService StateService,
            IDistrictService DistrictService, IBankService BankService, IBankNameService BankNameService, IUtilityService UtilityService, IUserCredentialService usercredentialservice, IModuleService iIModuleService,
            IClientBankDetailService ClientBankDetailService, IClientLeadService ClientLeadService)
        {
            this._ClientMasterService = ClientMasterService;
            this._CountryService = CountryService;
            this._StateService = StateService;
            this._DistrictService = DistrictService;
            this._BankService = BankService;
            this._BankNameService = BankNameService;
            this._UtilityService = UtilityService;
            this._ClientBankDetailService = ClientBankDetailService;
            this._IUserCredentialService = usercredentialservice;
            this._iIModuleService = iIModuleService;
            this._ClientLeadService = ClientLeadService;

        }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
        public class NoDirectAccessAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                if (filterContext.HttpContext.Request.UrlReferrer == null ||
                            filterContext.HttpContext.Request.Url.Host != filterContext.HttpContext.Request.UrlReferrer.Host)
                {
                    filterContext.Result = new RedirectToRouteResult(new
                              RouteValueDictionary(new { controller = "User", action = "Login", area = "" }));
                }
            }
        }

        public string Encode(string encodeMe)
        {
            byte[] encoded = System.Text.Encoding.UTF8.GetBytes(encodeMe);
            return Convert.ToBase64String(encoded);
        }

        public int Decode(string decodeMe)
        {
            byte[] decoded = Convert.FromBase64String(decodeMe);
            string decodedvalue = System.Text.Encoding.UTF8.GetString(decoded);
            return Convert.ToInt32(decodedvalue);
        }

        [HttpGet]
        public ActionResult Create()
        {
            MainApplication model = new MainApplication()
            {
                ClientLeads = new ClientLead()
            };

            var ClientLeads = _ClientLeadService.GetLastInsertedClient();
            int lastid = 0;
            int length = 0;
            if (ClientLeads != null)
            {
                lastid = ClientLeads.ClientLeadId;
                lastid = lastid + 1;
                length = lastid.ToString().Length;
            }
            else
            {
                lastid = 1;
                length = 1;
            }
            string catCode = _UtilityService.getName("CL", length, lastid);
            model.ClientLeads.ClientLeadCode = catCode;
            TempData["clientleadcode"] = catCode;

            //var codelist = _ClientBankDetailService.GetDetailsFromBank(catCode);
            model.ClientLeads.StateList = _StateService.GetStateByCountry(1);

            Session["ClientLeadCode"] = catCode;
            model.ClientLeads.CountryList = _CountryService.getallcountries();
            model.ClientLeads.Date =System.DateTime.Now;
            model.userCredentialList = _IUserCredentialService.GetUserCredentialsByEmail(UserEmail);
            model.modulelist = _iIModuleService.getAllModules();
            model.CompanyCode = CompanyCode;
            model.CompanyName = CompanyName;
            model.FinancialYear = FinancialYear;
           
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(MainApplication model)
        {
            var ClientLeads = _ClientLeadService.GetLastInsertedClient();
            int lastid = 0;
            int length = 0;
            if (ClientLeads != null)
            {
                lastid = ClientLeads.ClientLeadId;
                lastid = lastid + 1;
                length = lastid.ToString().Length;
            }
            else
            {
                lastid = 1;
                length = 1;
            }
            string catCode = _UtilityService.getName("CL", length, lastid);

            string LogedinUSerName = HttpContext.Session["UserName"].ToString();

            int did = Convert.ToInt32(model.ClientLeads.District);
            model.ClientLeads.District = _DistrictService.GetDistrictNamebyId(did);
            string Clientname = model.ClientLeads.ClientName;
            model.ClientLeads.ClientName = Clientname.First().ToString().ToUpper() + Clientname.Substring(1);

            ClientLead obj = new ClientLead();
            obj.ClientName = model.ClientLeads.ClientName;
            obj.ContactNo1 = model.ClientLeads.ContactNo1;
            obj.ContactNo2 = model.ClientLeads.ContactNo2;
            obj.Address = model.ClientLeads.Address;
            obj.Requriment = model.ClientLeads.Requriment;
            obj.Date = System.DateTime.Now;
            obj.ScheduleDate = model.ClientLeads.ScheduleDate;
            obj.Remark = model.ClientLeads.Remark;
            obj.Country = model.ClientLeads.Country;
            obj.District = model.ClientLeads.District;
            obj.State = model.ClientLeads.State;
            obj.City = model.ClientLeads.City;
            obj.ClientLeadCode = model.ClientLeads.ClientLeadCode;
              _ClientLeadService.CreateClientLead(obj);
            Response.Write("<script>alert('you did it')</script>");
            return RedirectToAction("ClientLeadDetails/" + obj.ClientLeadId, "ClientLead");
        }

        [HttpGet]
        public ActionResult Edit()
        {
            MainApplication model = new MainApplication();
            model.userCredentialList = _IUserCredentialService.GetUserCredentialsByEmail(UserEmail);
            model.modulelist = _iIModuleService.getAllModules();
            model.CompanyCode = CompanyCode;
            model.CompanyName = CompanyName;
            model.FinancialYear = FinancialYear;
            TempData["ViewType"] = "Edit";
            return View(model);
        }
        public ActionResult ClientLeadList(string name)
        {
            MainApplication model = new MainApplication();
            IEnumerable<ClientLead>ListofLeadClient= TempData["ClientLeadList"] as IEnumerable<ClientLead>;
            List<ClientLead> FinalLeadList = new List<ClientLead>();
            FinalLeadList = FinalLeadList.Concat(ListofLeadClient.Where(x => x.ClientName.ToLower().Contains(name.ToLower()))).Distinct().ToList();
            model.ClientLeadList = FinalLeadList;
            TempData["ClientLeadList"] = FinalLeadList;
            return View(model);
        }

        public ActionResult LoadNamesByClientLead()
        {
            MainApplication model = new MainApplication();
            model.ClientLeadList = _ClientLeadService.GetActiveClientLead();
            TempData["ClientLeadList"] = model.ClientLeadList;
            if (model.ClientLeadList.Count() == 0)
            {
                return RedirectToAction("EmptyList", "ClientLead");
            }
            return View(model);
        }

        public JsonResult EmptyList()
        {
            return Json("Empty", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EditPartial(int id)
        {
            MainApplication model = new MainApplication();
            model.ClientLeads = _ClientLeadService.GetClientLeadById(id);
            model.ClientLeads.CountryList = _CountryService.getallcountries();
            var countryData = _CountryService.getidbyname(model.ClientLeads.Country);
            model.ClientLeads.StateList = _StateService.GetStateByCountry(countryData);
            int stateid = _StateService.GetStateIdByName(model.ClientLeads.State);
            model.ClientLeads.DistrictList = _DistrictService.getDistrictbyState(stateid);
            return View(model);
        }

        [HttpGet]
        public ActionResult ClientLeadDetails(int id)
        {
            MainApplication model = new MainApplication()
            {
               ClientLeads = new ClientLead(),
            };
           // int Id = Decode(id.ToString());
            model.ClientLeads = _ClientLeadService.GetClientLeadById(id);
            model.userCredentialList = _IUserCredentialService.GetUserCredentialsByEmail(UserEmail);
            model.modulelist = _iIModuleService.getAllModules();
            model.CompanyCode = CompanyCode;
            model.CompanyName = CompanyName;
            model.FinancialYear = FinancialYear;
            string previousclient = TempData["clientleadcode"].ToString();
            if (previousclient != model.ClientLeads.ClientLeadCode)
            {
                ViewData["clientchanged"] = previousclient + " is replaced with " + model.ClientLeads.ClientLeadCode + " because " + previousclient + " is acquired by another person";
            }
            TempData["clientleadcode"] = previousclient;
            return View(model);
        }
        public ActionResult AcceptClientLead(MainApplication model,int id)
        {
           model.ClientLeadDetails= _ClientLeadService.GetClientLeadById(id);
            ClientMaster cm = new ClientMaster();
         cm.Address = model.ClientLeadDetails.Address;
           cm.ClientId = id;
          cm.ClientName = model.ClientLeadDetails.ClientName;
           cm.ContactNo1 = model.ClientLeadDetails.ContactNo1;
           cm.ContactNo2 = model.ClientLeadDetails.ContactNo2;
            cm.Address = model.ClientLeadDetails.Address;
            cm.Country = model.ClientLeadDetails.Country;
            cm.State = model.ClientLeadDetails.State;
            cm.District = model.ClientLeadDetails.District;
            cm.City = model.ClientLeadDetails.City;
            cm.Requriment = model.ClientLeadDetails.Requriment;
            cm.Remark = model.ClientLeadDetails.Remark;
            cm.Date = model.ClientLeadDetails.Date;
            cm.ScheduleDate = model.ClientLeadDetails.ScheduleDate;
            cm.ClientCode = model.ClientLeadDetails.ClientLeadCode;
            // _ClientLeadService.CreateClientLead(obj);
            _ClientMasterService.CreateClient(cm);
            // _ClientMasterService.
            return RedirectToAction("Edit");
        }
        [HttpPost]
        public ActionResult EditPartial(ClientLead clnt)
        {
            _ClientLeadService.UpdateClientLead(clnt);
             return RedirectToAction("ResultClientLead/" + clnt.ClientLeadId, "ClientLead");
           // return RedirectToAction("EditPartial");
        }

        public ActionResult ResultClientLead(int id)
        {
            MainApplication model = new MainApplication();
            model.ClientLeads = _ClientLeadService.GetClientLeadById(id);
            TempData["ClientLeadList"] = _ClientLeadService.GetAllClients();

            return View(model);
        }

        [HttpGet]
        public ActionResult Delete()
        {
            MainApplication model = new MainApplication();
            model.userCredentialList = _IUserCredentialService.GetUserCredentialsByEmail(UserEmail);
            model.modulelist = _iIModuleService.getAllModules();
            model.CompanyCode = CompanyCode;
            model.CompanyName = CompanyName;
            model.FinancialYear = FinancialYear;
            TempData["ViewType"] = "Delete";
            return View(model);
        }

        [HttpGet]
        public ActionResult DeletePartial(int id)
        {
            MainApplication model = new MainApplication();
            model.ClientLeads = _ClientLeadService.getClientById(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult DeletePartial(ClientLead clnt)
        {
            _ClientLeadService.DeleteClientLead(clnt);
            return RedirectToAction("ResultClientLead/" + clnt.ClientLeadId, "ClientLead");
        }
        [HttpGet]
        public JsonResult LoadDistrictByState(string statename)
        {
            if (!string.IsNullOrEmpty(statename))
            {
                int stateid = _StateService.GetStateIdByName(statename);
               ClientLead clientlead = new ClientLead();
                clientlead.DistrictList = _DistrictService.getDistrictbyState(stateid);
                var modelData = clientlead.DistrictList.Select(m => new SelectListItem()
                {
                    Text = m.DistrictName,
                    Value = m.DistrictId.ToString()
                });
                return Json(modelData, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var modelData = string.Empty;
                return Json(modelData, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult LoadStateByCountry(string countryname)
        {
            if (!string.IsNullOrEmpty(countryname))
            {
                int countryid = _CountryService.getidbyname(countryname);
                ClientLead clientlead = new ClientLead();
                clientlead.StateList = _StateService.GetStateByCountry(countryid);
                var modelData = clientlead.StateList.Select(m => new SelectListItem()
                {
                    Text = m.StateName,
                    Value = m.StateName
                });
                return Json(modelData, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var modelData = string.Empty;
                return Json(modelData, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
 