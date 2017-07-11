﻿using System;
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
    [NoDirectAccess]
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
            var ClientLeadDetails = _ClientLeadService.GetLastInsertedClientLead();
           
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
          
          //  var ClientLeadDetails = _ClientLeadService.GetLastInsertedClientLead();
           

            ClientLead obj = new ClientLead();
                obj.ClientName = model.ClientLeads.ClientName;
              obj.ContactNo1 = model.ClientLeads.ContactNo1;
              obj.ContactNo2 = model.ClientLeads.ContactNo2;
              obj.Address = model.ClientLeads.Address;
              obj.Requriment = model.ClientLeads.Requriment;
              obj.Date = System.DateTime.Now;
            obj.ScheduleDate = model.ClientLeads.ScheduleDate;
            obj.Remark = model.ClientLeads.Remark;




            //  _ClientLeadService.CreateClientLead(model.ClientLeadDetails);
              _ClientLeadService.CreateClientLead(obj);

            //var client_id = obj.ClientLeadId;


            //  _ClientMasterService.CreateClient(model.ClientDetails);

            //ClientLead  c = _ClientLeadService.getClientById(client_id);


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

        public ActionResult EditPartial(int id)
        {
            MainApplication model = new MainApplication();
            model.ClientLeads = _ClientLeadService.GetClientLeadById(id);
            return View(model);
        }


        [HttpGet]
        public ActionResult ClientLeadDetails(int id)
        {
            MainApplication model = new MainApplication()
            {
                // ClientDetails = new ClientMaster(),
                //ClientBankDetails = new ClientBankDetail(),
               ClientLeads = new ClientLead(),

            };
            //int Id = Decode(id);
            model.ClientLeads = _ClientLeadService.GetClientLeadById(id);
            //model.ClientBankDetailList = _ClientBankDetailService.GetDetailsFromBank(model.ClientDetails.ClientCode);
            model.userCredentialList = _IUserCredentialService.GetUserCredentialsByEmail(UserEmail);
            model.modulelist = _iIModuleService.getAllModules();
            model.CompanyCode = CompanyCode;
            model.CompanyName = CompanyName;
            model.FinancialYear = FinancialYear;

           // string previousclient = TempData["clientcode"].ToString();
            //if (previousclient != model.ClientDetails.ClientCode)
            //{
            //    ViewData["clientchanged"] = previousclient + " is replaced with " + model.ClientDetails.ClientCode + " because " + previousclient + " is acquired by another person";
            //}
            //TempData["clientcode"] = previousclient;

            return View(model);
        }



        public ActionResult AcceptClientLead(MainApplication model,int id)
        {
           model.ClientLeadDetails= _ClientLeadService.GetClientLeadById(id);
            ClientMaster cm = new ClientMaster();
         cm.Address = model.ClientLeadDetails.Address;
           cm.ClientId = model.ClientLeadDetails.ClientId;
          cm.ClientName = model.ClientLeadDetails.ClientName;
           cm.ContactNo1 = model.ClientLeadDetails.ContactNo1;
           cm.ContactNo2 = model.ClientLeadDetails.ContactNo2;
            // _ClientLeadService.CreateClientLead(obj);
            _ClientMasterService.CreateClient(cm);
            // _ClientMasterService.
            return View(model);
          
        }


        public ActionResult tp()
        {
            return View();
        }


      






















    }
}
