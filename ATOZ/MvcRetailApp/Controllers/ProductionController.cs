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
using CodeFirstServices.Services;
using MvcRetailApp.ModelBinder;
using System.Text;
using System.Reflection;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Configuration;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Web.Routing;
using MvcRetailApp.Filters;

namespace MvcRetailApp.Controllers
{
    //ATTRIBUTE FOR NO ACCESS TO CHANGE URL
    [NoDirectAccess]
    [SessionExpireFilter]
    public class ProductionController : Controller
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

        private readonly IUtilityService _UtilityService;
        private readonly IModuleService _ModuleService;
        private readonly IUserCredentialService _IUserCredentialService;
        private readonly IOutwardForProductionService _OutwardForProductionService;
        private readonly IOutwardForProductionItemService _OutwardForProductionItemService;
        private readonly IOutwardForProductionExpectedInwardService _OutwardForProductionExpectedInwardService;
        private readonly IJobWorkTypeService _JobWorkTypeService;
        private readonly IJobWorkerService _JobWorkerService;
        private readonly IEmployeeMasterService _EmployeeMasterService;
        private readonly IShopStockService _ShopStockService;
        private readonly IGodownStockService _GodownStockService;
        private readonly IItemService _ItemService;
        private readonly IUnitService _UnitService;
        private readonly ITypeOfMaterialService _TypeOfMaterialService;
        private readonly IColorCodeService _ColorCodeService;
        private readonly IReceivedReadyGoodService _ReceivedReadyGoodService;
        private readonly IReceivedReadyGoodItemService _ReceivedReadyGoodItemService;
        private readonly IStockItemDistributionService _StockItemDistributionService;
        private readonly IEntryStockItemService _EntryStockItemService;
        private readonly IOpeningStockService _OpeningStockService;
        private readonly IEntryStockService _EntryStockService;

        public ProductionController(IUtilityService UtilityService, IModuleService ModuleService, IUserCredentialService IUserCredentialService,
            IOutwardForProductionService OutwardForProductionService, IOutwardForProductionItemService OutwardForProductionItemService, IOutwardForProductionExpectedInwardService OutwardForProductionExpectedInwardService,
            IJobWorkTypeService JobWorkTypeService, IJobWorkerService JobWorkerService, IEmployeeMasterService EmployeeMasterService,
            IShopStockService ShopStockService, IGodownStockService GodownStockService, IItemService ItemService, IUnitService UnitService,
            ITypeOfMaterialService TypeOfMaterialService, IColorCodeService ColorCodeService, IReceivedReadyGoodService ReceivedReadyGoodService, IReceivedReadyGoodItemService ReceivedReadyGoodItemService, IStockItemDistributionService StockItemDistributionService,
            IEntryStockItemService EntryStockItemService, IOpeningStockService OpeningStockService, IEntryStockService EntryStockService)
        {
            this._UtilityService = UtilityService;
            this._ModuleService = ModuleService;
            this._IUserCredentialService = IUserCredentialService;
            this._OutwardForProductionService = OutwardForProductionService;
            this._OutwardForProductionItemService = OutwardForProductionItemService;
            this._OutwardForProductionExpectedInwardService = OutwardForProductionExpectedInwardService;
            this._JobWorkTypeService = JobWorkTypeService;
            this._JobWorkerService = JobWorkerService;
            this._EmployeeMasterService = EmployeeMasterService;
            this._ShopStockService = ShopStockService;
            this._GodownStockService = GodownStockService;
            this._ItemService = ItemService;
            this._UnitService = UnitService;
            this._TypeOfMaterialService = TypeOfMaterialService;
            this._ColorCodeService = ColorCodeService;
            this._ReceivedReadyGoodService = ReceivedReadyGoodService;
            this._ReceivedReadyGoodItemService = ReceivedReadyGoodItemService;
            this._StockItemDistributionService = StockItemDistributionService;
            this._EntryStockItemService = EntryStockItemService;
            this._OpeningStockService = OpeningStockService;
            this._EntryStockService = EntryStockService;
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


        //CRAETE OUTWARD FOR PRODUCTION
        [HttpGet]
        public ActionResult OutwardForProduction()
        {
            MainApplication model = new MainApplication()
            {
                OutwardForProductionDetails = new OutwardForProduction(),
            };

            string year = FinancialYear;
            string[] yr = year.Split(' ', '-');
            string FinYr = "/" + yr[2].Substring(2) + "-" + yr[6].Substring(2);

            string OutwardCode = string.Empty;

            var details = _OutwardForProductionService.GetLastRowrByFinYr(FinYr);
            int val = 0;
            int length = 0;
            if (details != null)
            {
                OutwardCode = details.OutwardCode.Substring(4, 6);
                length = (Convert.ToInt32(OutwardCode) + 1).ToString().Length;
                val = Convert.ToInt32(OutwardCode) + 1;

                val = details.Id;
                val = val + 1;
                length = val.ToString().Length;
            }
            else
            {
                val = 1;
                length = 1;
            }
            OutwardCode = _UtilityService.getName("OWFP", length, val);
            OutwardCode = OutwardCode + FinYr;
            model.OutwardForProductionDetails.OutwardCode = OutwardCode;
            TempData["PreviousOutForProd"] = OutwardCode;

            //GET LOGIN SHOP DETAILS
            var username = HttpContext.Session["UserName"].ToString();
            // IF EXCEPT SUPERADMIN LOGIN THEN SHOW SHOP OR GODOWN
            if (username != "SuperAdmin")
            {
                string shopcode = string.Empty;
                int modulelastcount = _ModuleService.GetLastRow().Id;
                for (int i = 105; i <= modulelastcount; i++)
                {
                    var assigndetails = _IUserCredentialService.GetDetailsByEmailStatusAndModuleId(UserEmail, i);
                    if (assigndetails != null)
                    {
                        if (assigndetails.AssignRightsCode.Contains("SH"))
                        {
                            Session["LOGINSHOPGODOWNCODEOWGS"] = assigndetails.AssignRightsCode;
                            Session["SHOPGODOWNNAMEOWGS"] = assigndetails.Modules;
                        }
                        else
                        {
                            Session["LOGINSHOPGODOWNCODEOWGS"] = assigndetails.AssignRightsCode;
                            Session["SHOPGODOWNNAMEOWGS"] = assigndetails.Modules;
                        }
                        break;
                    }
                }
            }

            model.userCredentialList = _IUserCredentialService.GetUserCredentialsByEmail(UserEmail);
            model.modulelist = _ModuleService.getAllModules();
            model.CompanyCode = CompanyCode;
            model.CompanyName = CompanyName;
            model.FinancialYear = FinancialYear;
            model.EmpList = _EmployeeMasterService.GetEmpByProductionDepartment();
            model.JobWorkerList = _JobWorkerService.GetAll();
            model.UnitList = _UnitService.getallsize();
            model.typeOfMaterialList = _TypeOfMaterialService.getAllTypeOfMaterial();
            model.ColorCodeList = _ColorCodeService.getAllColorCode();
            return View(model);
        }

        //GET SALES BILL ON ONLOAD FUNCTION
        [HttpGet]
        public JsonResult GetDetailsByEmpName(string empname)
        {
            var data = _EmployeeMasterService.getEmpByName(empname);
            return Json(new { data.MobileNo, data.Designation }, JsonRequestBehavior.AllowGet);
        }

        //ITEM LIST FROM STOCK DISTRIBUTION AND SHOP/GODOWN STOCK
        [HttpGet]
        public JsonResult GetItemsFromShopAndGodownStock(string LoginShopGodown, string BatchNo)
        {
            if (LoginShopGodown.Contains("SH"))
            {
                var stkdistrbtnlist = _ShopStockService.GetItemsByBatchAndShopCode(BatchNo, LoginShopGodown);
                var items = stkdistrbtnlist.Select(m => new SelectListItem
                {
                    Text = m.ItemName,
                    Value = m.ItemCode
                });
                return Json(items, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var stkdistrbtnlist = _GodownStockService.GetItemsByBatchAndGodownCode(BatchNo, LoginShopGodown);
                var items = stkdistrbtnlist.Select(m => new SelectListItem
                {
                    Text = m.ItemName,
                    Value = m.ItemCode
                });
                return Json(items, JsonRequestBehavior.AllowGet);
            }
        }

        //DISPLAY ITEM DISTAILS FROM SHOP STOCK
        [HttpGet]
        public JsonResult GetItemDetails(string barcode, string shopgodown)
        {
            barcode = barcode.ToUpper() + ".png";
            //calculate total quantity (shop quantity + godown quantity)
            double? shopqty = 0;
            double? godownqty = 0;
            double? totalstkqty = 0;
            var shopstockitems = _ShopStockService.GetRowsByBarcodeWithBatch(barcode);
            foreach (var item in shopstockitems)
            {
                shopqty = shopqty + item.Quantity;
            }

            var godownstockitems = _GodownStockService.GetRowsByBarcodeWithBatch(barcode);
            foreach (var item in godownstockitems)
            {
                godownqty = godownqty + item.Quantity;
            }
            totalstkqty = shopqty + godownqty;

            if (shopgodown.Contains("SH"))
            {
                var itemdetails = _ShopStockService.GetDetailsByBarcodeBatchAndShopCode(barcode, shopgodown);
                if (itemdetails != null)
                {
                    string itemcode = _ShopStockService.GetDetailsByBarcodeBatchAndShopCode(barcode, shopgodown).ItemCode;
                    string costprice = _ItemService.GetDescriptionByItemCode(itemcode).costprice;
                    if (itemdetails.Quantity != 0)
                    {
                        return Json(new
                        {
                            itemdetails.ItemCode,
                            //itemdetails.BatchNo,
                            itemdetails.ItemName,
                            itemdetails.Description,
                            itemdetails.Quantity,
                            costprice,
                            itemdetails.SellingPrice,
                            itemdetails.MRP,
                            itemdetails.Unit,
                            itemdetails.NumberType,
                            itemdetails.Color,
                            itemdetails.Material,
                            itemdetails.Size,
                            itemdetails.Brand,
                            itemdetails.DesignCode,
                            itemdetails.DesignName,
                            totalstkqty
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("No Stock", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    string msg = "Fail";
                    return Json(msg, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var itemdetails = _GodownStockService.GetDetailsByBarcodeBatchAndGodownCode(barcode, shopgodown);
                if (itemdetails != null)
                {
                    string costprice = _ItemService.GetDescriptionByItemCode(itemdetails.ItemCode).costprice;
                    if (itemdetails.Quantity != 0)
                    {
                        return Json(new
                        {
                            itemdetails.ItemCode,
                            itemdetails.BatchNo,
                            itemdetails.ItemName,
                            itemdetails.Description,
                            itemdetails.Quantity,
                            costprice,
                            itemdetails.SellingPrice,
                            itemdetails.MRP,
                            itemdetails.Unit,
                            itemdetails.NumberType,
                            itemdetails.Color,
                            itemdetails.Material,
                            itemdetails.Size,
                            itemdetails.Brand,
                            itemdetails.DesignCode,
                            itemdetails.DesignName,
                            totalstkqty
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("No Stock", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    string msg = "Fail";
                    return Json(msg, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpGet]
        public JsonResult GetAllUnits()
        {
            var unitlist = _UnitService.getallsize();
            var items = unitlist.Select(m => new SelectListItem
            {
                Text = m.UnitName,
                Value = m.UnitName
            });
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllColors()
        {
            var unitlist = _ColorCodeService.getAllColorCode();
            var items = unitlist.Select(m => new SelectListItem
            {
                Text = m.colorName,
                Value = m.colorName
            });
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllMaterials()
        {
            var unitlist = _TypeOfMaterialService.getAllTypeOfMaterial();
            var items = unitlist.Select(m => new SelectListItem
            {
                Text = m.MaterialName,
                Value = m.MaterialName
            });
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        //GET LOGIN EMPLOPYEE DETAILS BY EMPLOYEE EMAIL
        [HttpGet]
        public JsonResult GetPreparedByEmpDetails(string email)
        {
            var data = _EmployeeMasterService.getEmpByEmail(email);
            return Json(new { data.Name, data.MobileNo, data.Email }, JsonRequestBehavior.AllowGet);
        }

        //GET BATCH NO DDL
        [HttpGet]
        public JsonResult GetBatchesByShopGodownCode(string code)
        {
            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@shopgodowncode", code)
            };
            string procname = "GetBatchesByShopGodownCode" + " @shopgodowncode";
            var Batches = _StockItemDistributionService.GetBatchesByShopGodownCodeUsingProc(procname, param);
            var BatchList = Batches.Select(m => new SelectListItem
            {
                Text = m.BatchNo,
                Value = m.BatchNo
            });
            return Json(BatchList, JsonRequestBehavior.AllowGet);
        }

        //SAVE OUTWARD FOR PRODUCTION
        [HttpPost]
        public ActionResult OutwardForProduction(MainApplication mainapp, FormCollection frmcol)
        {
            MainApplication model = new MainApplication()
            {
                OutwardForProductionItemDetails = new OutwardForProductionItem(),
                OutwardForProductionExpectedInwardDetails = new OutwardForProductionExpectedInward(),
            };

            string year = FinancialYear;
            string[] yr = year.Split(' ', '-');
            string FinYr = "/" + yr[2].Substring(2) + "-" + yr[6].Substring(2);

            string OutwardCode = string.Empty;

            var details = _OutwardForProductionService.GetLastRowrByFinYr(FinYr);
            int val = 0;
            int length = 0;
            if (details != null)
            {
                OutwardCode = details.OutwardCode.Substring(4, 6);
                length = (Convert.ToInt32(OutwardCode) + 1).ToString().Length;
                val = Convert.ToInt32(OutwardCode) + 1;

                val = details.Id;
                val = val + 1;
                length = val.ToString().Length;
            }
            else
            {
                val = 1;
                length = 1;
            }
            OutwardCode = _UtilityService.getName("OWFP", length, val);
            OutwardCode = OutwardCode + FinYr;
            mainapp.OutwardForProductionDetails.OutwardCode = OutwardCode;
            TempData["PreviousOutForProd"] = OutwardCode;

            mainapp.OutwardForProductionDetails.Date = DateTime.Now;
            mainapp.OutwardForProductionDetails.TotalAmount = Convert.ToDouble(frmcol["TotalAmountval"]);
            mainapp.OutwardForProductionDetails.ProductionGrandTotal = Convert.ToDouble(frmcol["GrandTotalval"]);
            mainapp.OutwardForProductionDetails.ShopCode = Session["LOGINSHOPGODOWNCODE"].ToString();
            mainapp.OutwardForProductionDetails.ShopName = Session["SHOPGODOWNNAME"].ToString();
            mainapp.OutwardForProductionDetails.Status = "Active";
            mainapp.OutwardForProductionDetails.ModifiedOn = DateTime.Now;
            _OutwardForProductionService.Create(mainapp.OutwardForProductionDetails);

            //SAVE OUTWARD FOR PRODUCTION ITEMS
            int itemcount = Convert.ToInt32(frmcol["hdnRowCount"]);
            if (itemcount != 0)
            {
                for (int i = 1; i <= itemcount; i++)
                {
                    string BatchNo = "BatchNo" + i;
                    string Barcode = "Barcode" + i;
                    string ItemCode = "ItemCode" + i;
                    string Quantity = "Quantity" + i;
                    string ItemName = "ItemName" + i;
                    string Description = "Description" + i;
                    string UnitVal = "UnitVal" + i;
                    string NumberType = "NumberType" + i;
                    string CostPriceVal = "CostPriceVal" + i;
                    string ColorVal = "ColorVal" + i;
                    string MaterialVal = "MaterialVal" + i;
                    string AmountVal = "AmountVal" + i;
                    string SellingPrice = "SellingPrice" + i;
                    string MRP = "MRP" + i;
                    string Brand = "Brand" + i;
                    string Size = "Size" + i;
                    string DesignCode = "DesignCode" + i;
                    string DesignName = "DesignName" + i;
                    string ProdItemVal = "ProdItemVal" + i;

                    if (frmcol[ItemCode] != null)
                    {
                        model.OutwardForProductionItemDetails.OutwardCode = mainapp.OutwardForProductionDetails.OutwardCode;
                        model.OutwardForProductionItemDetails.BatchNo = frmcol[BatchNo];
                        model.OutwardForProductionItemDetails.Barcode = frmcol[Barcode].ToUpper() + ".png";
                        model.OutwardForProductionItemDetails.ItemCode = frmcol[ItemCode];
                        model.OutwardForProductionItemDetails.ItemName = frmcol[ItemName];
                        model.OutwardForProductionItemDetails.Quantity = frmcol[Quantity];
                        model.OutwardForProductionItemDetails.Description = frmcol[Description];
                        model.OutwardForProductionItemDetails.Unit = frmcol[UnitVal];
                        model.OutwardForProductionItemDetails.NumberType = frmcol[NumberType];
                        model.OutwardForProductionItemDetails.CostPrice = Convert.ToDouble(frmcol[CostPriceVal]);
                        model.OutwardForProductionItemDetails.SellingPrice = Convert.ToDouble(frmcol[SellingPrice]);
                        model.OutwardForProductionItemDetails.MRP = Convert.ToDouble(frmcol[MRP]);
                        model.OutwardForProductionItemDetails.Size = frmcol[Size];
                        model.OutwardForProductionItemDetails.Brand = frmcol[Brand];
                        model.OutwardForProductionItemDetails.DesignCode = frmcol[DesignCode];
                        model.OutwardForProductionItemDetails.DesignName = frmcol[DesignName];
                        model.OutwardForProductionItemDetails.Color = frmcol[ColorVal];
                        model.OutwardForProductionItemDetails.Material = frmcol[MaterialVal];
                        model.OutwardForProductionItemDetails.ProductionItem = frmcol[ProdItemVal];
                        model.OutwardForProductionItemDetails.Amount = Convert.ToDouble(frmcol[AmountVal]);
                        model.OutwardForProductionItemDetails.Status = "Active";
                        model.OutwardForProductionItemDetails.ModifiedOn = DateTime.Now;
                        model.OutwardForProductionItemDetails.InwardStatus = "Active";
                        _OutwardForProductionItemService.Create(model.OutwardForProductionItemDetails);

                        //MINUS QUANTITY INTO STOCK

                        var loginshopgodowncode = Session["LOGINSHOPGODOWNCODE"].ToString();
                        if (loginshopgodowncode.Contains("SH"))
                        {
                            //UPDATE SHOP STOCK (SHOP STOCK QUANTITY - OUTWARD QUANTITY)
                            var shopstockdata = _ShopStockService.GetDetailsByBarcodeBatchAndShopCode(model.OutwardForProductionItemDetails.Barcode, loginshopgodowncode);
                            shopstockdata.Quantity = shopstockdata.Quantity - Convert.ToDouble(frmcol[Quantity]);
                            if (shopstockdata.Quantity == 0)
                            {
                                shopstockdata.Status = "InActive";
                            }
                            _ShopStockService.Update(shopstockdata);

                            //UPDATE STOCK DISTRIBUTION (STOCK DISTRIBUTION QUANTITY - OUTWARD QUANTITY)
                            var stkdisdata = _StockItemDistributionService.GetItemDetailsByBarcodeBatchAndGodownShopCode(model.OutwardForProductionItemDetails.Barcode, loginshopgodowncode);
                            stkdisdata.ItemQuantity = stkdisdata.ItemQuantity - Convert.ToDouble(frmcol[Quantity]);
                            if (stkdisdata.ItemQuantity == 0)
                            {
                                stkdisdata.Status = "InActive";
                            }
                            _StockItemDistributionService.Update(stkdisdata);

                            //UPDATE ENTRY STOCK (ENTRY STOCK QUANTITY - OUTWARD QUANTITY)
                            var entrystkdata = _EntryStockItemService.GetItemDetailsByBarcodeBatch(model.OutwardForProductionItemDetails.Barcode);
                            //if item not present in entry stock then opening stock
                            if (entrystkdata != null)
                            {
                                entrystkdata.TotalQuantity = entrystkdata.TotalQuantity - Convert.ToDouble(frmcol[Quantity]);
                                if (entrystkdata.TotalQuantity == 0)
                                {
                                    entrystkdata.Status = "InActive";
                                }
                                _EntryStockItemService.Update(entrystkdata);
                            }
                            //UPDATE OPENING STOCK (OPENING STOCK QUANTITY - OUTWARD QUANTITY)
                            else
                            {
                                var openingstkdata = _OpeningStockService.GetItemsByBarcodeWithBatch(model.OutwardForProductionItemDetails.Barcode);
                                openingstkdata.TotalQuantity = openingstkdata.TotalQuantity - Convert.ToDouble(frmcol[Quantity]);
                                if (openingstkdata.TotalQuantity == 0)
                                {
                                    openingstkdata.Status = "InActive";
                                    openingstkdata.BatchNoStatus = "InActive";
                                }
                                _OpeningStockService.UpdateStock(openingstkdata);
                            }
                        }
                        else if (loginshopgodowncode.Contains("GD"))
                        {
                            //UPDATE GODOWN STOCK (GODOWN STOCK QUANTITY - OUTWARD QUANTITY)
                            var godownstockdata = _GodownStockService.GetDetailsByBarcodeBatchAndGodownCode(model.OutwardForProductionItemDetails.Barcode, loginshopgodowncode);
                            godownstockdata.Quantity = godownstockdata.Quantity - Convert.ToDouble(frmcol[Quantity]);
                            if (godownstockdata.Quantity == 0)
                            {
                                godownstockdata.Status = "InActive";
                            }
                            _GodownStockService.Update(godownstockdata);

                            //UPDATE STOCK DISTRIBUTION (STOCK DISTRIBUTION QUANTITY - OUTWARD QUANTITY)
                            var stkdisdata = _StockItemDistributionService.GetItemDetailsByBarcodeBatchAndGodownShopCode(model.OutwardForProductionItemDetails.Barcode, loginshopgodowncode);
                            stkdisdata.ItemQuantity = stkdisdata.ItemQuantity - Convert.ToDouble(frmcol[Quantity]);
                            if (stkdisdata.ItemQuantity == 0)
                            {
                                stkdisdata.Status = "InActive";
                            }
                            _StockItemDistributionService.Update(stkdisdata);

                            //UPDATE ENTRY STOCK (ENTRY STOCK QUANTITY - OUTWARD QUANTITY)
                            var entrystkdata = _EntryStockItemService.GetItemDetailsByBarcodeBatch(model.OutwardForProductionItemDetails.Barcode);
                            //if item not present in entry stock then opening stock
                            if (entrystkdata != null)
                            {
                                entrystkdata.TotalQuantity = entrystkdata.TotalQuantity - Convert.ToDouble(frmcol[Quantity]);
                                if (entrystkdata.TotalQuantity == 0)
                                {
                                    entrystkdata.Status = "InActive";
                                }
                                _EntryStockItemService.Update(entrystkdata);
                            }
                            //UPDATE OPENING STOCK (OPENING STOCK QUANTITY - OUTWARD QUANTITY)
                            else
                            {
                                var openingstkdata = _OpeningStockService.GetItemsByBarcodeWithBatch(model.OutwardForProductionItemDetails.Barcode);
                                openingstkdata.TotalQuantity = openingstkdata.TotalQuantity - Convert.ToDouble(frmcol[Quantity]);
                                if (openingstkdata.TotalQuantity == 0)
                                {
                                    openingstkdata.Status = "InActive";
                                    openingstkdata.BatchNoStatus = "InActive";
                                }
                                _OpeningStockService.UpdateStock(openingstkdata);
                            }
                        }
                    }
                }
            }

            //SAVE OUTWARD FOR PRODUCTION EXPECTED INWAEDS
            int inwarditemcount = Convert.ToInt32(frmcol["hdnRowCountForInward"]);
            if (inwarditemcount != 0)
            {
                for (int i = 1; i <= inwarditemcount; i++)
                {
                    string InwardItem = "InwardItem" + i;
                    string InwardQuantity = "InwardQuantity" + i;
                    string InwardColor = "InwardColor" + i;
                    string InwardMaterial = "InwardMaterial" + i;
                    string InwardUnit = "InwardUnit" + i;
                    string InwardProdItemVal = "InwardProdItemVal" + i;

                    if (frmcol[InwardItem] != null)
                    {
                        model.OutwardForProductionExpectedInwardDetails.OutwardCode = mainapp.OutwardForProductionDetails.OutwardCode;
                        model.OutwardForProductionExpectedInwardDetails.ItemName = frmcol[InwardItem];
                        model.OutwardForProductionExpectedInwardDetails.Quantity = frmcol[InwardQuantity];
                        model.OutwardForProductionExpectedInwardDetails.Unit = frmcol[InwardUnit];
                        model.OutwardForProductionExpectedInwardDetails.Color = frmcol[InwardColor];
                        model.OutwardForProductionExpectedInwardDetails.Material = frmcol[InwardMaterial];
                        model.OutwardForProductionExpectedInwardDetails.ProductionItem = frmcol[InwardProdItemVal];
                        model.OutwardForProductionExpectedInwardDetails.Status = "Active";
                        model.OutwardForProductionExpectedInwardDetails.ModifiedOn = DateTime.Now;
                        model.OutwardForProductionExpectedInwardDetails.InwardStatus = "Active";
                        _OutwardForProductionExpectedInwardService.Create(model.OutwardForProductionExpectedInwardDetails);
                    }
                }
            }
            return RedirectToAction("OutwardForProduction");


        }


        // *********************************************** RECEIVED READY GOODS **************************************************

        //CREATE RECEIVED READY GOODS (INWARD FROM OUTWARD FOR PRODUCTION)
        [HttpGet]
        public ActionResult ReceivedReadyGoods()
        {
            MainApplication model = new MainApplication()
            {
                ReceivedReadyGoodDetails = new ReceivedReadyGood(),
            };

            string year = FinancialYear;
            string[] yr = year.Split(' ', '-');
            string FinYr = "/" + yr[2].Substring(2) + "-" + yr[6].Substring(2);
            string InwardCode = string.Empty;

            var details = _ReceivedReadyGoodService.GetLastRowrByFinYr(FinYr);
            int val = 0;
            int length = 0;
            if (details != null)
            {
                InwardCode = details.InwardCode.Substring(4, 6);
                length = (Convert.ToInt32(InwardCode) + 1).ToString().Length;
                val = Convert.ToInt32(InwardCode) + 1;

                val = details.Id;
                val = val + 1;
                length = val.ToString().Length;
            }
            else
            {
                val = 1;
                length = 1;
            }
            InwardCode = _UtilityService.getName("IWRG", length, val);
            InwardCode = InwardCode + FinYr;
            model.ReceivedReadyGoodDetails.InwardCode = InwardCode;

            //GET LOGIN SHOP DETAILS
            var username = HttpContext.Session["UserName"].ToString();
            // IF EXCEPT SUPERADMIN LOGIN THEN SHOW SHOP OR GODOWN
            if (username != "SuperAdmin")
            {
                string shopcode = string.Empty;
                int modulelastcount = _ModuleService.GetLastRow().Id;
                for (int i = 105; i <= modulelastcount; i++)
                {
                    var assigndetails = _IUserCredentialService.GetDetailsByEmailStatusAndModuleId(UserEmail, i);
                    if (assigndetails != null)
                    {
                        if (assigndetails.AssignRightsCode.Contains("SH"))
                        {
                            Session["LOGINSHOPGODOWNCODEOWGS"] = assigndetails.AssignRightsCode;
                            Session["SHOPGODOWNNAMEOWGS"] = assigndetails.Modules;
                        }
                        else
                        {
                            Session["LOGINSHOPGODOWNCODEOWGS"] = assigndetails.AssignRightsCode;
                            Session["SHOPGODOWNNAMEOWGS"] = assigndetails.Modules;
                        }
                        break;
                    }
                }
            }

            model.userCredentialList = _IUserCredentialService.GetUserCredentialsByEmail(UserEmail);
            model.modulelist = _ModuleService.getAllModules();
            model.CompanyCode = CompanyCode;
            model.CompanyName = CompanyName;
            model.FinancialYear = FinancialYear;
            return View(model);
        }

        // OUTWARD FOR PRODUCTION NO AUTO COMPLETE
        [HttpGet]
        public JsonResult GetOutwardForProdctionNos(string term)
        {
            var data = _OutwardForProductionService.GetActiveOutwardNos(term);
            List<string> names = new List<string>();
            foreach (var item in data)
            {
                names.Add(item.OutwardCode);
            }
            return Json(names, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetOutwardForProductionDetails(string OutwardNo)
        {
            MainApplication model = new MainApplication();
            model.OutwardForProductionDetails = _OutwardForProductionService.GetDetailsByOutwardNo(OutwardNo);
            model.OutwardForProductionItemList = _OutwardForProductionItemService.GetRowsByOutwardNo(OutwardNo);
            model.OutwardForProductionExpectedInwardList = _OutwardForProductionExpectedInwardService.GetRowsByOutwardNo(OutwardNo);
            return View(model);
        }

        [HttpGet]
        public JsonResult GetTotalOutwardItemsAmount(string ProdItemNo, string OutNo)
        {
            double TotalOutAmount = 0;
            var outwarddata = _OutwardForProductionItemService.GetRowsByOutwardAndItemNo(ProdItemNo, OutNo);
            foreach (var item in outwarddata)
            {
                TotalOutAmount = Convert.ToDouble(TotalOutAmount) + Convert.ToDouble(item.Amount);
            }
            return Json(TotalOutAmount, JsonRequestBehavior.AllowGet);
        }

        //SAVE RECEIVED READY GOODS
        [HttpPost]
        public ActionResult ReceivedReadyGoods(MainApplication mainapp, FormCollection frmcol)
        {
            MainApplication model = new MainApplication()
            {
                ReceivedReadyGoodItemDetails = new ReceivedReadyGoodItem(),
            };

            string year = FinancialYear;
            string[] yr = year.Split(' ', '-');
            string FinYr = "/" + yr[2].Substring(2) + "-" + yr[6].Substring(2);
            string InwardCode = string.Empty;

            var details = _ReceivedReadyGoodService.GetLastRowrByFinYr(FinYr);
            int val = 0;
            int length = 0;
            if (details != null)
            {
                InwardCode = details.InwardCode.Substring(4, 6);
                length = (Convert.ToInt32(InwardCode) + 1).ToString().Length;
                val = Convert.ToInt32(InwardCode) + 1;

                val = details.Id;
                val = val + 1;
                length = val.ToString().Length;
            }
            else
            {
                val = 1;
                length = 1;
            }
            InwardCode = _UtilityService.getName("IWRG", length, val);
            InwardCode = InwardCode + FinYr;
            mainapp.ReceivedReadyGoodDetails.InwardCode = InwardCode;


            //SAVE OUTWARD FOR PRODUCTION ITEMS
            double ttllabourcost = 0;
            double ttlotherexpenses = 0;
            int itemcount = Convert.ToInt32(frmcol["hdnRowCount"]);
            if (itemcount != 0)
            {
                for (int i = 1; i <= itemcount; i++)
                {

                    string ItemName = "ItemName" + i;
                    string Unit = "Unit" + i;
                    string Size = "Size" + i;
                    string Description = "Description" + i;
                    string Quantity = "Quantity" + i;
                    string Color = "Color" + i;
                    string LabourCost = "LabourCost" + i;
                    string OtherExpenses = "OtherExpenses" + i;
                    string CostPrice = "CostPriceVal" + i;

                    if (frmcol[Quantity] != null)
                    {
                        model.ReceivedReadyGoodItemDetails.InwardCode = mainapp.ReceivedReadyGoodDetails.InwardCode;
                        model.ReceivedReadyGoodItemDetails.ItemName = frmcol[ItemName];
                        model.ReceivedReadyGoodItemDetails.Unit = frmcol[Unit];
                        model.ReceivedReadyGoodItemDetails.Size = frmcol[Size];
                        model.ReceivedReadyGoodItemDetails.Quantity = Convert.ToDouble(frmcol[Quantity]);
                        model.ReceivedReadyGoodItemDetails.Description = frmcol[Description];
                        model.ReceivedReadyGoodItemDetails.Color = frmcol[Color];
                        model.ReceivedReadyGoodItemDetails.CostPrice = frmcol[CostPrice];
                        model.ReceivedReadyGoodItemDetails.LabourCost = Convert.ToDouble(frmcol[LabourCost]);
                        model.ReceivedReadyGoodItemDetails.OtherExpenses = Convert.ToDouble(frmcol[OtherExpenses]);
                        model.ReceivedReadyGoodItemDetails.CostPrice = frmcol[CostPrice];
                        model.ReceivedReadyGoodItemDetails.Status = "Active";
                        model.ReceivedReadyGoodItemDetails.ModifiedOn = DateTime.Now;
                        _ReceivedReadyGoodItemService.Create(model.ReceivedReadyGoodItemDetails);

                        ttllabourcost = ttllabourcost + Convert.ToDouble(frmcol[LabourCost]);
                        ttlotherexpenses = ttlotherexpenses + Convert.ToDouble(frmcol[OtherExpenses]);
                    }
                }
            }

            mainapp.ReceivedReadyGoodDetails.Date = DateTime.Now;
            mainapp.ReceivedReadyGoodDetails.TotalAmount = Convert.ToDouble(frmcol["TotalAmountVal"]);


            mainapp.ReceivedReadyGoodDetails.ActualLabourCost = ttllabourcost;
            mainapp.ReceivedReadyGoodDetails.OtherExpenses = ttlotherexpenses;
            mainapp.ReceivedReadyGoodDetails.ProductionGrandTotal = Convert.ToDouble(frmcol["GrandTotalVal"]);
            mainapp.ReceivedReadyGoodDetails.ShopCode = Session["LOGINSHOPGODOWNCODE"].ToString();
            mainapp.ReceivedReadyGoodDetails.ShopName = Session["SHOPGODOWNNAME"].ToString();
            mainapp.ReceivedReadyGoodDetails.Status = "Active";
            mainapp.ReceivedReadyGoodDetails.ModifiedOn = DateTime.Now;
            _ReceivedReadyGoodService.Create(mainapp.ReceivedReadyGoodDetails);


            var outwarddata = _OutwardForProductionService.GetDetailsByOutwardNo(mainapp.ReceivedReadyGoodDetails.OutwardForProductionCode);
            outwarddata.Status = "InActive";
            _OutwardForProductionService.Update(outwarddata);

            var OutwardItemData = _OutwardForProductionItemService.GetRowsByOutwardNo(mainapp.ReceivedReadyGoodDetails.OutwardForProductionCode);
            foreach (var item in OutwardItemData)
            {
                item.Status = "InActive";
                _OutwardForProductionItemService.Update(item);
            }

            var ExpectedInwardData = _OutwardForProductionExpectedInwardService.GetRowsByOutwardNo(mainapp.ReceivedReadyGoodDetails.OutwardForProductionCode);
            foreach (var data in ExpectedInwardData)
            {
                data.Status = "InActive";
                _OutwardForProductionExpectedInwardService.Update(data);
            }

            return RedirectToAction("ReceivedReadyGoods");

        }

        // *********************************** PRODUCTION REPORT **********************************************
        [HttpGet]
        public ActionResult ProductionReport()
        {
            MainApplication model = new MainApplication();
            model.userCredentialList = _IUserCredentialService.GetUserCredentialsByEmail(UserEmail);
            model.modulelist = _ModuleService.getAllModules();
            model.CompanyCode = CompanyCode;
            model.CompanyName = CompanyName;
            model.FinancialYear = FinancialYear;
            return View(model);
        }

        [HttpGet]
        public ActionResult GetProductionByDate(string FromDate, string ToDate)
        {
            MainApplication model = new MainApplication();
            var FDate = Convert.ToDateTime(FromDate);
            var TDate = Convert.ToDateTime(ToDate);
            model.OutwardForProductionList = _OutwardForProductionService.GetReportByDate(FDate, TDate);
            return View(model);
        }

        [HttpGet]
        public JsonResult GetReadyReceivedGoods(string OutwardCode)
        {
            var inwarddetails = _ReceivedReadyGoodService.GetDetailsByInwardNo(OutwardCode);
            return Json(new { inwarddetails.InwardCode }, JsonRequestBehavior.AllowGet);
        }
    }
}
