﻿using ECommerce1.Models;
using ECommerce1.Models.Validators;
using ECommerce1.Models.ViewModels;
using ECommerce1.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static ECommerce1.Models.ViewModels.ProductsViewModel;

namespace ECommerce1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly ResourceDbContext resourceDbContext;
        public BlobWorker BlobWorker { get; set; }

        public ReviewController(ResourceDbContext resourceDbContext,
            BlobWorker blobWorker)
        {
            this.resourceDbContext = resourceDbContext;
            BlobWorker = blobWorker;
        }

        /// <summary>
        /// Get all reviews for specified product
        /// </summary>
        /// <param name="guid">Product ID</param>
        /// <param name="page"></param>
        /// <param name="onPage">Products on page</param>
        /// <returns></returns>
        [HttpGet("product/{guid}")]
        public async Task<ActionResult<ReviewsViewModel>> GetByProductId(string guid, int page = 1, int onPage = 20)
        {
            var product = await resourceDbContext.Products
                .Include(p => p.Reviews)
                .ThenInclude(r => r.User)
                .Include(p => p.Reviews)
                .ThenInclude(r => r.Photos)
                .FirstOrDefaultAsync(p => p.Id.ToString() == guid);

            if (product == null)
            {
                return NotFound(new { error_message = "No such product exists" });
            }

            double rating = product.Reviews.Count == 0 ? 0 : product.Reviews.Average(r => r.Quality);

            var reviews = product.Reviews
                .Skip((page - 1) * onPage)
                .Take(onPage)
                .Select(r => new ReviewReviewsModel
                {
                    Id = r.Id,
                    Quality = r.Quality,
                    ReviewText = r.ReviewText,
                    Photos = r.Photos.Select(p => p.Url).ToList(),
                    BuyerName = $"{r.User.FirstName} {r.User.LastName[0]}.",
                    Initials = $"{r.User.FirstName[0]}{r.User.LastName[0]}"
                }).ToList();

            return new ReviewsViewModel
            {
                Reviews = reviews,
                TotalProductCount = product.Reviews.Count,
                OnPageProductCount = onPage,
                CurrentPage = page,
                TotalPageCount = (int)Math.Ceiling((double)product.Reviews.Count / onPage),
                Rating = rating
            };
        }

        /// <summary>
        /// Add review for specified product
        /// </summary>
        /// <param name="review"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> AddReview([FromForm] AddReviewViewModel review)
        {
            string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Profile? user = await resourceDbContext.Profiles.FirstOrDefaultAsync(p => p.AuthId == id);
            if (user == null)
            {
                return BadRequest(new { error_message = "No such user exists" });
            }
            Product? product = await resourceDbContext.Products.FirstOrDefaultAsync(c => c.Id.ToString() == review.ProductId);
            if(product == null)
            {
                return BadRequest(new { error_message = "No such product exists" });
            }
            Review? review1 = await resourceDbContext.Reviews.FirstOrDefaultAsync(r => r.User.AuthId == id && r.Product.Id == product.Id);
            if (review1 != null)
            {
                return BadRequest(new
                {
                    error_message = "You have already submitted a review for this product"
                });
            }

            IEnumerable<string> references = await BlobWorker.AddPublicationPhotos(review.Photos);
            List<ReviewPhoto> productPhotos = new();
            foreach (string reference in references)
            {
                productPhotos.Add(new ReviewPhoto()
                {
                    Url = reference
                });
            }

            Review rev = new()
            {
                User = user,
                Product = product,
                ReviewText = review.Text,
                Quality = review.Rating,
                Photos = productPhotos
            };

            await resourceDbContext.Reviews.AddAsync(rev);
            await resourceDbContext.SaveChangesAsync();
            return Ok(rev.Id);
        }

        /// <summary>
        /// Delete review
        /// </summary>
        /// <param name="guid">Review ID</param>
        /// <returns></returns>
        [HttpDelete("delete")]
        [Authorize(Roles ="User,Admin")]
        public async Task<IActionResult> DeleteAsync(string guid)
        {
            Review? review = await resourceDbContext.Reviews.Include(p => p.User).FirstOrDefaultAsync(p => p.Id.ToString() == guid);
            if (review == null)
            {
                return BadRequest(new
                {
                    error_message = "No review with such id exists"
                });
            }
            if (User.IsInRole("User") && User.FindFirstValue(ClaimTypes.NameIdentifier) != review.User.AuthId)
            {
                return BadRequest(new
                {
                    error_message = "Not you"
                });
            }

            resourceDbContext.Reviews.Remove(review);
            await resourceDbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
