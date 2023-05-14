﻿namespace ECommerce1.Models
{
    /// <summary>
    /// Product model
    /// </summary>
    public class Product : AModel
    {
        /// <summary>
        /// Product name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Product creation time
        /// </summary>
        public DateTime CreationTime { get; set; }
        /// <summary>
        /// Product description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Product price
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Whether in stock or not
        /// </summary>
        public bool? InStock { get; set; }

        /// <summary>
        /// Product category
        /// </summary>
        public Category Category { get; set; }
        /// <summary>
        /// Product seller
        /// </summary>
        public Seller Seller { get; set; }
        /// <summary>
        /// Product photos
        /// </summary>
        public IList<ProductPhoto> ProductPhotos { get; set; }
        /// <summary>
        /// Product reviews
        /// </summary>
        public IList<Review> Reviews { get; set; }
        /// <summary>
        /// Can be used to determine which users have added this product to their cart
        /// </summary>
        public IList<CartItem> CartItems { get; set; }
        /// <summary>
        /// Can be used to determine which users have ordered this product
        /// </summary>
        public IList<Order> Orders { get; set; }
        /// <summary>
        /// Can be used to determine which users have added this product to their favourites
        /// </summary>
        public IList<FavouriteItem> FavouriteItems { get; set; }

        public Product()
        {
            ProductPhotos = new List<ProductPhoto>();
            Reviews = new List<Review>();
            CartItems = new List<CartItem>();
            Orders = new List<Order>();
            FavouriteItems = new List<FavouriteItem>();
        }
    }
}
