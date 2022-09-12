using Casestudy.APIHelpers;
using Casestudy.DAL.DomainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using Casestudy.Helpers;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;

namespace Casestudy.DAL.DAO
{
    public class OrderDAO
    {
        private AppDbContext _db;
        public OrderDAO(AppDbContext ctx)
        {
            _db = ctx;
        }
        public List<Order> GetAll(int id)
        {
            return _db.Orders.Where(order => order.UserId == id).ToList<Order>();
        }

        public List<OrderDetailsHelper> GetOrderDetails(int oid, string email)
        {
            Customer customer = _db.Customers.FirstOrDefault(customer => customer.Email == email);
            List<OrderDetailsHelper> allDetails = new List<OrderDetailsHelper>();
            // LINQ way of doing INNER JOINS
            var results = from o in _db.Orders
                          join oli in _db.OrderLineItems on o.Id equals oli.OrderId
                          join p in _db.Products on oli.ProductId equals p.Id
                          where (o.UserId == customer.Id && o.Id == oid)
                          select new OrderDetailsHelper
                          {
                              OrderId = o.Id,
                              ProductId = oli.ProductId,
                              ProductName = p.ProductName,
                              Description = p.Description,
                              UserId = customer.Id,
                              DateCreated = o.OrderDate.ToString("yyyy/MM/dd - hh:mm tt"),
                              QtySold = oli.QtySold,
                              QtyOrdered = oli.QtyOrdered,
                              QtyBackOrdered = oli.QtyBackOrdered,
                              OrderAmount = (p.MSRP * oli.QtySold),
                              ProductPrice = p.MSRP
                };
            allDetails = results.ToList<OrderDetailsHelper>();
            return allDetails;
        }

        public int AddOrder(int customerid, OrderSelectionHelper[] selections)
        {
            int OrderId = -1;
            using (_db)
            {
                // we need a transaction as multiple entities involved
                using (var _trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        Order order = new Order(); 
                        order.UserId = customerid;
                        order.OrderDate = System.DateTime.Now;
                        order.OrderAmount = 0.0M;
                        // calculate the totals and then add the order row to the table
                        foreach (OrderSelectionHelper selection in selections)
                        {
                            order.OrderAmount += Convert.ToDecimal(selection.item.MSRP * selection.Qty);
                        }
                        _db.Orders.Add(order);
                        _db.SaveChanges();
                        // then add each item to the Order Item table
                        foreach (OrderSelectionHelper selection in selections)
                        {
                            OrderLineItem tItem = new OrderLineItem();
                            tItem.QtyOrdered = selection.Qty;
                            tItem.ProductId = selection.item.Id;
                            tItem.OrderId = order.Id;
                            tItem.SellingPrice = Convert.ToDecimal(selection.item.MSRP * selection.Qty);
                            if (selection.item.QtyOnHand > selection.Qty)
                            {
                                selection.item.QtyOnHand -= selection.Qty;
                                tItem.QtySold = selection.Qty;
                                tItem.QtyBackOrdered = 0;
                                _db.Products.Update(selection.item);
                                _db.SaveChanges();
                            }
                            else
                            {
                                selection.item.QtyOnBackOrder += (selection.Qty - selection.item.QtyOnHand);
                                tItem.QtyBackOrdered = (selection.Qty - selection.item.QtyOnHand);
                                tItem.QtySold = selection.item.QtyOnHand;
                                _db.Products.Update(selection.item);
                                _db.SaveChanges();
                                selection.item.QtyOnHand = 0;
                            }
                            _db.OrderLineItems.Add(tItem);
                            _db.SaveChanges();
                        }
                        _trans.Commit();
                        OrderId = order.Id;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        _trans.Rollback();
                    }
                }
            }
            return OrderId;
        }
    }
}
