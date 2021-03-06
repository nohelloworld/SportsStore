﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers
{
    public class CartController : Controller
    {
        private IProductsRepository repository;
        private IOrderProcessor orderProcessor;

        public ViewResult Index(Cart cart,string returnUrl)
        {
            return View(new CartIndexViewModel {ReturnUrl = returnUrl, Cart = cart 
        });
            
        }

        // GET: Cart
        public CartController(IProductsRepository repo,IOrderProcessor proc)
        {
            repository = repo;
            orderProcessor = proc;
        }
        public RedirectToRouteResult  AddToCart(Cart cart, int productId,String returnUrl)
        {
            Product product = repository.Products
                .FirstOrDefault(p => p.ProductID == productId);
            if (product != null)
            {
                cart.AddItem(product,1);
            }
            return RedirectToAction("Index", new { returnUrl });
        }
        public RedirectToRouteResult RemoveFromCart(Cart cart,int productId,string returnUrl)
        {
            Product product = repository.Products
                .FirstOrDefault(p=>p.ProductID==productId);
            if(product != null)
            {
                cart.RemoveLine(product);
            }
            return RedirectToAction("Index", new { returnUrl });
        }

        public PartialViewResult Summary(Cart cart)
        {
            return PartialView(cart);
        }

        public ViewResult Checkout()
        {
            return View(new ShoppingDetails());
        }

        [HttpPost]
        public ViewResult Checkout(Cart cart,ShoppingDetails shoppingDetails)
        {
            if (cart.Lines.Count() == 0)
            {
                ModelState.AddModelError("", "Sorry,your cart is empty!");
            }
            if (ModelState.IsValid)
            {
                orderProcessor.ProcessOrder(cart, shoppingDetails);
                cart.Clear();
                return View("Completed");
            }
            else
            {
                return View(new ShoppingDetails());
            }
           
        }

        //private Cart GetCart()
        //{
        //    Cart cart = (Cart)Session["Cart"];
        //    if (cart == null)
        //    {
        //        cart = new Cart();
        //        Session["Cart"] = cart;
        //    }
        //    return cart;
        //}
    }
}