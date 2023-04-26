using AutoMapper;
using ECommerice.Api.DTOs;
using ECommerice.Api.Helpers;
using ECommerice.Api.Helpers.Extensions;
using ECommerice.Core.Entities;
using ECommerice.Core.IRepository;
using ECommerice.Core.Specification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerice.Api.Controllers
{
    public class ProductController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<ProductImage> _productImagesRepo;
        private readonly IUploaderRepo _uploaderRepo;
        private readonly IGenericRepository<Category> _category;
        public readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private IHostingEnvironment _hostingEnvironment;

        public ProductController(IGenericRepository<Product> productRepo,
            IGenericRepository<ProductImage> productImagesRepo,
            IGenericRepository<Category> category,
            IMapper mapper,
            UserManager<AppUser> userManager,
            IUploaderRepo uploaderRepo,
            IHostingEnvironment hostingEnvironment)
        {
            _productRepo = productRepo;
            _category = category;
            _mapper = mapper;
            _userManager = userManager;
            _productImagesRepo = productImagesRepo;
            _uploaderRepo = uploaderRepo;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet("getProducts")]
        public async Task<ActionResult<Pagination<ProductToReturnDTO>>> GetProducts([FromQuery]ProductSpecParam productParams)
         {
            var spec = new ProductsWithTypesAndBrandsSpecification(productParams);
            var countSpec = new ProductsWithFiltersForCountSpecification(productParams);
            var totalItems = await _productRepo.CountAsync(countSpec);
            var products = await _productRepo.ListAsync(spec);
            var data = _mapper
                .Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDTO>>(products);

            return Ok(new Pagination<ProductToReturnDTO> 
            { 
               PageIndex = productParams.PageIndex,
               PageSize = productParams.PageSize,
               Count = totalItems,
               Data = data
            });
         }

        [HttpGet("getProduct/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDTO>> GetProduct(int id)
         {
            var spec = new ProductsWithTypesAndBrandsSpecification(id);
            var product = await _productRepo.GetEntityWithSpec(spec);
            
            if (product == null) return NotFound(new ApiResponse(400));

            return _mapper.Map<Product, ProductToReturnDTO>(product);
         }

        [HttpGet("getLatestProducts")]
        public async Task<ActionResult<Pagination<ProductToReturnDTO>>> GetLatestProducts()
        {
            var products = await _productRepo.GetLatestData(3,"ProductImages", "ProductReviews");
            var data = _mapper
                .Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDTO>>(products);

            return Ok(data);
        }

        [HttpGet("checkProductQtyAva")]
        public async Task<ActionResult<Pagination<ProductToReturnDTO>>> CheckProductQtyAva(int productId, int qtyReq)
        {
            var _product = await _productRepo.GetWhereObject(p => p.Id == productId);
            if (_product.Quantity == 0)
            {
                return Ok(new
                {
                    message = "Quantity not available in stock",
                    status = false
                });
            }

            if (qtyReq > _product.Quantity)
            {
                return Ok(new
                {
                    message = "Quantity request greater than in stock",
                    status = false
                });
            }

            return Ok(new
            {
                message = "Quantity available",
                status = true
            });

        }


        #region admin
        [Authorize]
        [HttpPost("addProduct")]
        public async Task<ActionResult<Category>> AddProduct(Product product)
        {
            var user = await _userManager.FindByEmailFromClaimPrincipleAsync(HttpContext.User);
            if (user.UserType == "user") return Unauthorized(new ApiResponse(401));

            product.CreatedDate = DateTime.Now;
            _productRepo.Add(product);
            

            if (await _productRepo.SaveChanges())
            {
                foreach (var pImage in product.ProductImages)
                {
                    pImage.Id = product.Id;
                    _productImagesRepo.Add(pImage);
                    await _productImagesRepo.SaveChanges();
                }
               
                return Ok();
            }
            return BadRequest();
        }

        [Authorize]
        [HttpPost("updateProduct")]
        public async Task<ActionResult<Category>> UpdateProduct(Product product)
        {
            var user = await _userManager.FindByEmailFromClaimPrincipleAsync(HttpContext.User);
            if (user.UserType == "user") return Unauthorized(new ApiResponse(401));

            _productRepo.Update(product);

            if (await _productRepo.SaveChanges())
                return Ok();

            return BadRequest();
        }


        [HttpPost("getProductsPaging")]
        public async Task<ActionResult<Pagination<ProductToReturnDTO>>> getProductsPaging(CommonSearchDTO commonSearch)
         {
            var user = await _userManager.FindByEmailFromClaimPrincipleAsync(HttpContext.User);
            if (user.UserType == "user") return Unauthorized(new ApiResponse(401));

           if(commonSearch.SearchText == null || commonSearch.SearchText == "")
            {
                var _products = await _productRepo.GetAllWithInclude("Category", "ProductImages");
                var products = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductToReturnDTO>>(_products)
                         .Skip((commonSearch.PageNo - 1) * commonSearch.pageSize).Take(commonSearch.pageSize);

                return Ok(new
                {
                    Data = products,
                    Count = _products.Count()
                });
            }
            else
            {
                var _products = await _productRepo.GetWhereWithInclude(p => p.Name.Contains(commonSearch.SearchText),"Category");
                var products = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductToReturnDTO>>(_products)
                         .Skip((commonSearch.PageNo - 1) * commonSearch.pageSize).Take(commonSearch.pageSize);

                return Ok(new
                {
                    Data = products,
                    Count = _products.Count()
                });
            }
         }

        [Authorize]
        [HttpDelete("deleteProduct/{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var user = await _userManager.FindByEmailFromClaimPrincipleAsync(HttpContext.User);
            if (user.UserType == "user") return Unauthorized(new ApiResponse(401));

            var product = await _productRepo.GetByIdAsync(id);
            _productRepo.Delete(product);

            if (await _productRepo.SaveChanges())
                return Ok(true);

            return BadRequest(false);
        }

        [Authorize]
        [HttpDelete("deleteProductImage/{id}")]
        public async Task<ActionResult> DeleteProductImage(int id)
        {
            var user = await _userManager.FindByEmailFromClaimPrincipleAsync(HttpContext.User);
            if (user.UserType == "user") return Unauthorized(new ApiResponse(401));

            var productImage = await _productImagesRepo.GetByIdAsync(id);

            _productImagesRepo.Delete(productImage);

            if (await _productRepo.SaveChanges())
            {
                _uploaderRepo.Delete(productImage.Url, _hostingEnvironment);
                return Ok(true);
            }
            return BadRequest(false);
        }

        [Authorize]
        [HttpDelete("setMain/{id}")]
        public async Task<ActionResult> SetImageMain(int id)
        {
            var user = await _userManager.FindByEmailFromClaimPrincipleAsync(HttpContext.User);
            if (user.UserType == "user") return Unauthorized(new ApiResponse(401));

            var productImage = await _productImagesRepo.GetByIdAsync(id);
            var currentMainPhoto = await _productImagesRepo.GetWhereObject(p => p.ProductId == productImage.ProductId);
            
            currentMainPhoto.IsMain = false;
            productImage.IsMain = true;

            _productImagesRepo.Update(productImage);
            _productImagesRepo.Update(currentMainPhoto);

            if (await _productRepo.SaveChanges())
            {
                _uploaderRepo.Delete(productImage.Url, _hostingEnvironment);
                return Ok(true);
            }
            return BadRequest(false);
        }
        #endregion
    }
}