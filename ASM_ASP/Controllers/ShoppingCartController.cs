using ASM_ASP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static ASM_ASP.Models.Order;

namespace ASM_ASP.Controllers
{
    public class ShoppingCartController : Controller
    {
        private static string SHOPPING_CART_NAME = "shoppingCart";
        private MyDbContext db = new MyDbContext();
        // GET: ShoppingCart
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddCart(int productId, int quantity)
        {
            // Check số lượng có hợp lệ không?
            if (quantity <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid Quantity");
            }
            // Check sản phẩm có hợp lệ không?
            var product = db.Products.Find(productId);
            if (product == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Product's' not found");
            }
            // Lấy thông tin shopping cart từ session.
            var sc = LoadShoppingCart();
            // Thêm sản phẩm vào shopping cart.
            sc.AddCart(product, quantity);
            // lưu thông tin cart vào session.
            SaveShoppingCart(sc);
            return Redirect("/ShoppingCart/ShowCart");
        }

        public ActionResult UpdateCart(int productId, int quantity)
        {
            // Check số lượng có hợp lệ không?
            if (quantity <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid Quantity");
            }
            // Check sản phẩm có hợp lệ không?
            var product = db.Products.Find(productId);
            if (product == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Product's' not found");
            }
            // Lấy thông tin shopping cart từ session.
            var sc = LoadShoppingCart();
            // Thêm sản phẩm vào shopping cart.
            sc.UpdateCart(product, quantity);
            // lưu thông tin cart vào session.
            SaveShoppingCart(sc);
            return Redirect("/ShoppingCart/ShowCart");
        }
        public ActionResult RemoveCart(int productId)
        {
            var product = db.Products.Find(productId);
            if (product == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Product's' not found");
            }
            // Lấy thông tin shopping cart từ session.
            var sc = LoadShoppingCart();
            // Thêm sản phẩm vào shopping cart.
            sc.RemoveFromCart(product.Id);
            // lưu thông tin cart vào session.
            SaveShoppingCart(sc);
            return Redirect("/ShoppingCart/ShowCart");
        }
        public ActionResult GetListOrders(int? page, string sortOrder, string searchString, string currentFilter)
        {
            ViewBag.CurrentSort = sortOrder;
            // lúc đầu vừa vào thì sortOrder là null, cho nên gán NameSortParm = name_desc
            // Ấn vào link Full name thì lúc đó NameSortParm có giá trị là name_desc, sortOrder trên view được gán = NameSortParm cho nên sortOrder != null
            // và NameSortParm = ""
            // Ấn tiếp vào link Full Name thì sortOrder = "" cho nên NameSortParm = name_desc
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            var ordersList = db.Orders.Where(s => s.Status != Order.OrderStatus.Delete).OrderBy(s => s.Id);

            switch (sortOrder)
            {
                case "Date":
                    ordersList = ordersList.OrderBy(s => s.CreatedAt);
                    break;
                case "date_desc":
                    ordersList = ordersList.OrderByDescending(s => s.CreatedAt);
                    break;
                default:
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            ViewBag.currentPage = pageNumber;
            ViewBag.totalPage = Math.Ceiling((double)ordersList.Count() / pageSize);
            // nếu page == null thì lấy giá trị là 1, nếu không thì giá trị là page
            //return View(students.ToList().ToPagedList(pageNumber, pageSize));
            return View(ordersList.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList());
            //if (page == null) page = 1;

            //int pageSize = 3;

            //var orders = db.Orders.ToList().OrderBy(x => x.Id);

            //int pageNumber = (page ?? 1);

            //if (page == null) page = 1;

            //int pageSize = 1;

            //int skip = (int)(page - 1) * pageSize;

            //var orders = db.Orders.ToList()
            //       .OrderBy(p => p.Id)
            //       .Skip(skip)
            //       .Take(pageSize);

            //var count = db.Products.Count();

            //var resultAsPagedList = new StaticPagedList<Order>(orders, (int)page, pageSize, count * pageSize);

            //return View(resultAsPagedList);
        }
        public ActionResult ShowCart()
        {
            ViewBag.shoppingCart = LoadShoppingCart();
            return View();
        }
        public ActionResult DisplayCartAfterCreateOrder(int orderId)
        {
            var order = db.Orders.Find(orderId);
            return View(order);
        }
        [HttpPost]
        public ActionResult CreateOrder(CartInformation cartInfo)
        {
            // load cart trong session.
            var shoppingCart = LoadShoppingCart();
            if (shoppingCart.GetCartItems().Count <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Bad request");
            }
            // chuyển thông tin shopping cart thành Order.
            var order = new Order
            {
                TotalPrice = shoppingCart.GetTotalPrice(),
                MemberId = 1,
                PaymentTypeId = (PaymentType) Enum.Parse(typeof(PaymentType), cartInfo.PaymentTypeId),
                ShipName = cartInfo.ShipName,
                ShipPhone = cartInfo.ShipPhone,
                ShipAddress = cartInfo.ShipAddress,
                OrderDetails = new List<OrderDetail>()
            };
            // Tạo order detail từ cart item.
            foreach (var cartItem in shoppingCart.GetCartItems())
            {
                var orderDetail = new OrderDetail()
                {
                    ProductId = cartItem.Value.ProductId,
                    OrderId = order.Id,
                    Quantity = cartItem.Value.Quantity,
                    UnitPrice = cartItem.Value.Price
                };
                order.OrderDetails.Add(orderDetail);
            }
            db.Orders.Add(order);
            db.SaveChanges();
            ClearCart();
            //// lưu vào database.
            //var transaction = db.Database.BeginTransaction();
            //try
            //{

            //    transaction.Commit();
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //    transaction.Rollback();
            //}
            return RedirectToAction("DisplayCartAfterCreateOrder", new { orderId = order.Id });
        }
        private void ClearCart()
        {
            Session.Remove(SHOPPING_CART_NAME);
        }

        /**
         * Tham số nhận vào là một đối tượng shopping cart.
         * Hàm sẽ lưu đối tượng vào session với key được define từ trước.
         */
        private void SaveShoppingCart(ShoppingCart shoppingCart)
        {
            Session[SHOPPING_CART_NAME] = shoppingCart;
        }

        /**
         * Lấy thông tin shopping cart từ trong session ra. Trong trường hợp không tồn tại
         * trong session thì tạo mới đối tượng shopping cart.
         */
        private ShoppingCart LoadShoppingCart()
        {
            // lấy thông tin giỏ hàng ra.
            if (!(Session[SHOPPING_CART_NAME] is ShoppingCart sc))
            {
                sc = new ShoppingCart();
            }
            return sc;
        }

        public ActionResult GetListProductsInOrder(int id)
        {
            var lstOrderDetail = db.OrderDetails.Where(s => s.OrderId == id).ToList();
            var lstProduct = new List<Product>();
            double totalPrice = 0;
            foreach(var item in lstOrderDetail)
            {
                var product = db.Products.Find(item.ProductId);
                lstProduct.Add(product);
                totalPrice += item.Quantity * product.Price;
            }

            ViewBag.totalPrice = totalPrice;

            return View(lstProduct);
        }
    }
}