using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASM_ASP.Models
{
    public class ShoppingCart
    {
        private Dictionary<int, CartItem> _cartItems = new Dictionary<int, CartItem>();
        private double _totalPrice = 0;

        public double GetTotalPrice()
        {
            this._totalPrice = 0;
            foreach (var item in _cartItems.Values)
            {
                this._totalPrice += item.Price * item.Quantity;
            }
            return this._totalPrice;
        }

        public Dictionary<int, CartItem> GetCartItems()
        {
            return _cartItems;
        }

        public void SetCartItems(Dictionary<int, CartItem> cartItems)
        {
            this._cartItems = cartItems;
        }

        /**
         * Thêm một sản phẩm vào giỏ hàng.
         * Trong trường hợp tồn tại sản phẩm trong giỏ hàng thì update số lượng.
         * Trong trường hợp không tồn tại thì thêm mới.
         */
        public void AddCart(Product product, int quantity)
        {
            if (_cartItems.ContainsKey(product.Id))
            {
                var item = _cartItems[product.Id];
                item.Quantity += quantity;
                _cartItems[product.Id] = item;
                return;
            }
            var cartItem = new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = quantity
            };
            // đưa cart item tương ứng với sản phẩm (ở trên) vào danh sách.
            _cartItems.Add(cartItem.ProductId, cartItem);
        }

        public void UpdateCart(Product product, int quantity)
        {
            if (_cartItems.ContainsKey(product.Id))
            {
                var item = _cartItems[product.Id];
                item.Quantity = quantity;
                _cartItems[product.Id] = item;
            }
        }

        public void RemoveFromCart(int productId)
        {
            _cartItems.Remove(productId);
        }
    }
}