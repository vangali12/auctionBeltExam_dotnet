using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using beltexam.Models;
using Microsoft.EntityFrameworkCore;

namespace beltexam.Controllers
{
    public class StoreController : Controller
    {
        private beltexamContext _context;

        public StoreController(beltexamContext context)
        {
            _context = context;
        }

// ***************************************** CALL FUNCTIONS ***************************************** //

        public void UpdateWallet() {
            int? c = HttpContext.Session.GetInt32("current");
            int currentUserId = (int)c;
            User currentUser = _context.Users.SingleOrDefault(u => u.userid == currentUserId);
            
            int subtract = 0;
            List<Bid> Withdrawls = _context.Bids.Include(b => b.Bidder).Where(b => b.Bidder.userid == currentUserId).Where(b => b.Product.End < DateTime.Now).ToList();
            foreach (Bid withdrawl in Withdrawls) {
                subtract += withdrawl.Amount;
            }
            
            int add = 0;
            List<Bid> Deposits = _context.Bids.Include(b => b.Product).ThenInclude(p => p.Seller).Where(d => d.Product.Seller.userid == currentUserId).Where(b => b.Product.End < DateTime.Now).ToList();
            foreach (Bid deposit in Deposits) {
                add += deposit.Amount;
            }
            
            currentUser.Wallet = 1000 - subtract + add;
            _context.SaveChanges();
        }

// ***************************************** GETS ***************************************** //

        [HttpGet]
        [Route("dashboard")]
        public IActionResult DashboardRedirect()
        {   
            int? c = HttpContext.Session.GetInt32("current");
            if (c == null) {
                return RedirectToAction("Index", "Home");
            }
            int currentUserId = (int)c;
            return RedirectToAction("Dashboard", new { num = currentUserId });
        }

        [HttpGet]
        [Route("dashboard/{num}")]
        public IActionResult Dashboard(int num)
        {   
            // SOMEHOW IMPLEMENT TOP5
            int? c = HttpContext.Session.GetInt32("current");
            if (c == null) {
                return RedirectToAction("Index", "Home");
            }
            int currentUserId = (int)c;
            User currentUser = _context.Users.SingleOrDefault(u => u.userid == currentUserId);
            ViewBag.currentUser = currentUser;

            List<Product> AllProducts = _context.Products.Include(w => w.Seller).Where(p => p.End > DateTime.Now).OrderBy(ms => ms.End).ToList();
            ViewBag.AllProducts = AllProducts;

            List<Bid> BidsWon = _context.Bids.Include(b => b.Product).Where(b => b.Product.End < DateTime.Now).Where(buy => buy.userid == currentUserId).Where(b => b.Amount == b.Product.Bid).OrderByDescending(ms => ms.UpdatedAt).ToList();
            ViewBag.BidsWon = BidsWon;

            UpdateWallet();
            return View("Dashboard");
        }

        [HttpGet]
        [Route("createProduct")]
        public IActionResult CreateProductPage()
        {   
            int? c = HttpContext.Session.GetInt32("current");
            if (c == null) {
                return RedirectToAction("Index", "Home");
            }
            int currentUserId = (int)c;
            return View("NewProduct");
        }

        [HttpGet]
        [Route("product/{num}")]
        public IActionResult DisplayProduct(int num)
        {
            int? c = HttpContext.Session.GetInt32("current");
            if (c == null) {
                return RedirectToAction("Index", "Home");
            }
            int currentUserId = (int)c;
            Product CurrentProduct = _context.Products.Include(u => u.Seller).SingleOrDefault(p => p.productid == num);
            ViewBag.CurrentProduct = CurrentProduct;
            return View("ProductPage");
        }

        [HttpGet]
        [Route("deleteProduct/{num}")]
        public IActionResult DeleteProductGet() {
            return RedirectToAction("Index", "Home");
        }

// ***************************************** POSTS ***************************************** //

        [HttpPost]
        [Route("createProduct")]
        public IActionResult CreateProduct(ProductViewModel model)
        {
            int? c = HttpContext.Session.GetInt32("current");
            if (c == null) {
                return View("Index", "Home");
            }
            int currentUserId = (int)c;
            
            if (ModelState.IsValid) {
                Product NewProduct = new Product();
                NewProduct.Name = model.Name;
                NewProduct.Description = model.Description;
                NewProduct.Bid = model.Bid;
                NewProduct.End = model.End;
                NewProduct.CreatedAt = DateTime.Now;
                NewProduct.UpdatedAt = DateTime.Now;
                NewProduct.userid = currentUserId;
                _context.Products.Add(NewProduct);
                _context.SaveChanges();
                return RedirectToAction("DashboardRedirect");
            }
            return View("NewProduct");
        }

        [HttpPost]
        [Route("deleteProduct/{num}")]
        public IActionResult DeleteProduct(int num) {
            int? c = HttpContext.Session.GetInt32("current");
            int currentUserId = (int)c;

            Product toDelete = _context.Products.SingleOrDefault(p => p.productid == num);

            List<Bid> associations = _context.Bids.Include(b => b.Bidder).Include(b => b.Product).ThenInclude(p => p.Seller).Where(b => b.userid == currentUserId).Concat(_context.Bids.Include(b => b.Bidder).Include(b => b.Product).ThenInclude(p => p.Seller).Where(b => b.Product.Seller.userid == currentUserId)).ToList();

            foreach (Bid association in associations) {
                _context.Bids.Remove(association);
            }
            _context.Products.Remove(toDelete);
            _context.SaveChanges();
            return RedirectToAction("DashboardRedirect");
        }

        [HttpPost]
        [Route("createBid/{num}")]
        public IActionResult CreateBid(int num, BidViewModel model)
        {
            int? c = HttpContext.Session.GetInt32("current");
            int currentUserId = (int)c;
            Product currentProduct = _context.Products.Include(p => p.Seller).SingleOrDefault(p => p.productid == num);
            UpdateWallet();
            User currentUser = _context.Users.SingleOrDefault(u => u.userid == currentUserId);
            if (ModelState.IsValid) {
                if (model.Amount > currentProduct.Bid) {
                    if (currentUser.Wallet >= model.Amount) {
                        if (currentProduct.Seller.userid != currentUserId) {
                            Bid NewBid = new Bid();
                            NewBid.Amount = model.Amount;
                            NewBid.CreatedAt = DateTime.Now;
                            NewBid.UpdatedAt = DateTime.Now;
                            NewBid.userid = currentUserId;
                            NewBid.productid = num;

                            currentProduct.Bid = model.Amount;

                            _context.Bids.Add(NewBid);
                            _context.SaveChanges();
                            return RedirectToAction("DashboardRedirect");
                        } else {
                            ViewBag.upBid = "You cannot bid on your own item.";
                        }
                    } else {
                        ViewBag.notEnough = "You do not have enough money for that bid.";
                        ViewBag.CurrentProduct = currentProduct;
                        return View("ProductPage");
                    }
                } else {
                    ViewBag.bidMore = "Your bid must be greater than the current value.";
                    ViewBag.CurrentProduct = currentProduct;
                    return View("ProductPage");
                }
            }
            ViewBag.CurrentProduct = currentProduct;
            return View("ProductPage");
        }

    }
}