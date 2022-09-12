using System;
using System.Collections.Generic;
using Casestudy.DAL;
using Casestudy.DAL.DAO;
using Casestudy.DAL.DomainClasses;
using Casestudy.APIHelpers;
using Casestudy.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Casestudy.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        AppDbContext _ctx;
        public OrderController(AppDbContext context) // injected here
        {
            _ctx = context;
        }
        [HttpGet("{orderid}/{email}")]
        public ActionResult<List<OrderDetailsHelper>> GetTrayDetails(int orderid, string email)
        {
            OrderDAO dao = new OrderDAO(_ctx);
            return dao.GetOrderDetails(orderid, email);
        }

        [HttpGet("{email}")]
        public ActionResult<List<Order>> List(string email)
        {
            List<Order> orders = new List<Order>();
            CustomerDAO cDao = new CustomerDAO(_ctx);
            Customer orderOwner = cDao.GetByEmail(email);
            OrderDAO oDao = new OrderDAO(_ctx);
            orders = oDao.GetAll(orderOwner.Id);
            return orders;
        }

        [HttpPost]
        [Produces("application/json")]
        public ActionResult<string> Index(OrderHelper helper)
        {
            string retVal = "";
            try
            {
                CustomerDAO cDao = new CustomerDAO(_ctx);
                Customer OrderOwner = cDao.GetByEmail(helper.email);
                OrderDAO tDao = new OrderDAO(_ctx);
                int orderId = tDao.AddOrder(OrderOwner.Id, helper.selections);
                if (orderId > 0)
                {
                    retVal = "Order " + orderId + " saved!";
                }
                else
                {
                    retVal = "Order not saved";
                }
            }
            catch (Exception ex)
            {
                retVal = "Order not saved " + ex.Message;
            }
            return retVal;
        }
    }
}