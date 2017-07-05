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
    //ATTRIBUTE FOR NO ACCESS TO CHANGE URL
    [NoDirectAccess]
    [SessionExpireFilter]
    public class ItemTaxController : Controller
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
        private static string DatabaseName
        {
            set { ((dynamic)System.Web.HttpContext.Current.ApplicationInstance).DynamicDatabase = value; }
        }
        private readonly IUtilityService _utilityService;
        private readonly IItemTaxService _itemtaxService;
        private readonly IItemService _itemservice;
        private readonly IItemCategoryService _itemcategoryService;
        private readonly IItemSubCategoryService _itemsubcategoryservice;
        private readonly ICityService _cityservice;
        private readonly IUserCredentialService _IUserCredentialService;
        private readonly IModuleService _iIModuleService;

        public ItemTaxController(IUtilityService utilityService, IItemTaxService itemtaxService, IItemService itemservice, IItemCategoryService itemcategoryService, IItemSubCategoryService itemsubcategoryservice, ICityService cityservice, IUserCredentialService usercredentialservice, IModuleService iIModuleService)
        {
            this._utilityService = utilityService;
            this._itemtaxService = itemtaxService;
            this._itemservice = itemservice;
            this._itemcategoryService = itemcategoryService;
            this._itemsubcategoryservice = itemsubcategoryservice;
            this._cityservice = cityservice;
            this._IUserCredentialService = usercredentialservice;
            this._iIModuleService = iIModuleService;
        }

        //THIS IS FOR NO ACCESS TO CHANGE URL IF ANYONE TRY TO CHANGE THEN GOES TO LOGIN PAGE
        //using System.Web.Routing is required for this..
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

        //encode the id value which is display in the details page..
        public string Encode(string encodeMe)
        {
            byte[] encoded = System.Text.Encoding.UTF8.GetBytes(encodeMe);
            return Convert.ToBase64String(encoded);
        }

        //decode the id value which is display in the details page..
        public int Decode(string decodeMe)
        {
            byte[] decoded = Convert.FromBase64String(decodeMe);
            string decodedvalue = System.Text.Encoding.UTF8.GetString(decoded);
            return Convert.ToInt32(decodedvalue);
        }

        //CREATE TAX MASTER
        [HttpGet]
        public ActionResult Create()
        {
            DatabaseName = CompanyName + " " + FinancialYear;
            MainApplication model = new MainApplication();
            model.ItemCategoryList = _itemcategoryService.GetAllItemCategories();
            model.ItemSubCategoryList = _itemsubcategoryservice.getSubCategory();
            model.CityList = _cityservice.getAllCities();
            model.userCredentialList = _IUserCredentialService.GetUserCredentialsByEmail(UserEmail);
            model.modulelist = _iIModuleService.getAllModules();
            model.CompanyCode = CompanyCode;
            model.CompanyName = CompanyName;
            model.FinancialYear = FinancialYear;
            return View(model);
        }

        //GET SUBCATEGORY BY CATEGORY
        [HttpGet]
        public JsonResult LoadSubCategoryByCategory(string cat)
        {
            var subcat = _itemsubcategoryservice.GetSubCategoryByCategory(cat);
            MainApplication model = new MainApplication();
            model.ItemSubCategoryList = subcat;
            var modeldata = model.ItemSubCategoryList.Select(m => new SelectListItem()
            {
                Text = m.subCategoryName,
                Value = m.subCategoryName
            });
            return Json(modeldata, JsonRequestBehavior.AllowGet);
        }

        //SAVE TAX MASTER
        [HttpPost]
        public ActionResult Create(MainApplication mainapp, FormCollection frmcollection)
        {
            MainApplication model = new MainApplication()
           {
               ItemTaxDetails = new ItemTaxMaster(),
           };

            int lastitembefore = 0;
            var data = _itemtaxService.GetLastRow();
            if (data == null)
            {
                lastitembefore = 1;
            }
            else
            {
                lastitembefore = data.TaxItemId + 1;
            }

            //SAVE DYNAMIOS ROWS
            string taxitem = frmcollection["hdnRowCount"].ToString();

            if (!string.IsNullOrEmpty(taxitem))
            {
                int count = Convert.ToInt32(taxitem);
                for (int i = 1; i <= count; i++)
                {
                    string subcat = "subcat" + i;
                    string taxtype = "taxtype" + i;
                    string percentage = "percentage" + i;

                    string finalsubcat = frmcollection[subcat];
                    string finaltaxtype = frmcollection[taxtype];
                    double finalpercentage = Convert.ToDouble(frmcollection[percentage]);

                    if (!string.IsNullOrEmpty(finalsubcat))
                    {
                        model.ItemTaxDetails.SubCategory = finalsubcat;
                        model.ItemTaxDetails.TaxType = finaltaxtype;
                        model.ItemTaxDetails.Percentage = finalpercentage;
                        model.ItemTaxDetails.Status = "Active";
                        model.ItemTaxDetails.ModifiedOn = DateTime.Now;

                        model.ItemTaxDetails.Category = mainapp.ItemTaxDetails.Category;
                        model.ItemTaxDetails.City = mainapp.ItemTaxDetails.City;
                        model.ItemTaxDetails.FromDate = mainapp.ItemTaxDetails.FromDate;
                        model.ItemTaxDetails.ToDate = mainapp.ItemTaxDetails.ToDate;

                        _itemtaxService.Create(model.ItemTaxDetails);
                    }
                }
            }
            int lastitemafter = _itemtaxService.GetLastRow().TaxItemId;
            var ItemBefore = Encode(lastitembefore.ToString());
            var ItemAfter = Encode(lastitemafter.ToString());
            return RedirectToAction("CreateDetails", "ItemTax", new { lastrowbefore = ItemBefore, lastitemafter = ItemAfter });
        }

        //SHOW DETAILS AFTER CREATE
        [HttpGet]
        public ActionResult CreateDetails(string lastrowbefore, string lastitemafter)
        {
            DatabaseName = CompanyName + " " + FinancialYear;
            MainApplication model = new MainApplication();
            int BeforeId = Decode(lastrowbefore);
            int AfterId = Decode(lastitemafter);
            model.ItemTaxList = _itemtaxService.GetInsertedRows(BeforeId, AfterId);
            model.userCredentialList = _IUserCredentialService.GetUserCredentialsByEmail(UserEmail);
            model.modulelist = _iIModuleService.getAllModules();
            model.CompanyCode = CompanyCode;
            model.CompanyName = CompanyName;
            model.FinancialYear = FinancialYear;
            return View(model);
        }

        //EDIT TAX MASTER
        [HttpGet]
        public ActionResult Edit()
        {
            DatabaseName = CompanyName + " " + FinancialYear;
            MainApplication model = new MainApplication();
            model.ItemTaxList = _itemtaxService.GetAll();
            model.userCredentialList = _IUserCredentialService.GetUserCredentialsByEmail(UserEmail);
            model.modulelist = _iIModuleService.getAllModules();
            model.CompanyCode = CompanyCode;
            model.CompanyName = CompanyName;
            model.FinancialYear = FinancialYear;
            return View(model);
        }

        //SHOW EDIT DETAILS
        [HttpGet]
        public ActionResult EditPartial(string fromdate, string todate, string taxtype)
        {
            MainApplication model = new MainApplication();
            DateTime FDate = Convert.ToDateTime(fromdate);
            DateTime TDate = Convert.ToDateTime(todate);
            model.ItemTaxList = _itemtaxService.GetDetailsByDate(FDate, TDate, taxtype);
            TempData["TaxItems"] = model.ItemTaxList;
            return View(model);
        }

        //SAVE EDIT DETAILS
        [HttpPost]
        public ActionResult EditPartial(MainApplication mainapp, FormCollection fromcol)
        {
            mainapp.ItemTaxDetails.Status = "Active";
            mainapp.ItemTaxDetails.ModifiedOn = DateTime.Now;

            mainapp.ItemTaxList = TempData["TaxItems"] as IEnumerable<ItemTaxMaster>;
            string todate = string.Empty;
            int count = 1;

            int lastitembefore = 0;
            var row = _itemtaxService.GetLastRow();
            if (row == null)
            {
                lastitembefore = 1;
            }
            else
            {
                lastitembefore = row.TaxItemId + 1;
            }

            foreach (var data in mainapp.ItemTaxList)
            {

                todate = "todate" + count;
                if (fromcol[todate] != null)
                {
                    mainapp.ItemTaxDetails.City = data.City;
                    mainapp.ItemTaxDetails.Category = data.Category;
                    mainapp.ItemTaxDetails.SubCategory = data.SubCategory;
                    mainapp.ItemTaxDetails.TaxType = data.TaxType;
                    mainapp.ItemTaxDetails.FromDate = data.FromDate;

                    mainapp.ItemTaxDetails.Percentage = data.Percentage;
                    mainapp.ItemTaxDetails.Status = "Active";
                    mainapp.ItemTaxDetails.ModifiedOn = DateTime.Now;
                    if (Convert.ToDateTime(fromcol[todate]) != data.ToDate)
                    {
                        mainapp.ItemTaxDetails.ToDate = Convert.ToDateTime(fromcol[todate]);
                        _itemtaxService.Create(mainapp.ItemTaxDetails);
                    }
                    count = count + 1;
                }
            }
            int lastitemafter = _itemtaxService.GetLastRow().TaxItemId;
            return RedirectToAction("UpdateDetails", "ItemTax", new { lastrowbefore = lastitembefore, lastrowafter = lastitemafter });
        }

        [HttpGet]
        public ActionResult UpdateDetails(int lastrowbefore, int lastrowafter)
        {
            DatabaseName = CompanyName + " " + FinancialYear;
            MainApplication model = new MainApplication();
            model.userCredentialList = _IUserCredentialService.GetUserCredentialsByEmail(UserEmail);
            model.modulelist = _iIModuleService.getAllModules();
            model.CompanyCode = CompanyCode;
            model.CompanyName = CompanyName;
            model.FinancialYear = FinancialYear;
            model.ItemTaxList = _itemtaxService.GetInsertedRows(lastrowbefore, lastrowafter);
            return View(model);
        }

        [HttpGet]
        public JsonResult GetFinancialYear()
        {
            string year = FinancialYear;
            string[] FinYr = year.Split(' ', '-');
            string fromdate = string.Empty;
            string todate = string.Empty;
            fromdate = FinYr[1] + "/" + FinYr[0] + "/" + FinYr[2];
            todate = FinYr[5] + "/" + FinYr[4] + "/" + FinYr[6];
            fromdate = Convert.ToDateTime(fromdate).ToShortDateString();
            todate = Convert.ToDateTime(todate).ToShortDateString();
            return Json(new { fromdate, todate }, JsonRequestBehavior.AllowGet);
        }

    }
}
