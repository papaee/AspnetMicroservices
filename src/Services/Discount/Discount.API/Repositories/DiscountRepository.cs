using Discount.API.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Discount.API.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _configuration;

        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            using (var newConnection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString")))
            {
                var coupon = await newConnection.QueryFirstOrDefaultAsync<Coupon>("SELECT * from Coupon WHERE ProductName = @ProductName", new { ProductName = productName});

                if (coupon == null)
                    return new Coupon() { ProductName = "No Discount", Amount = 0, Description= "No Discount Desc" };

                return coupon;
            }
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            using (var newConnection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString")))
            {
                var affected = await newConnection.ExecuteAsync("INSERT into Coupon (ProductName, Description, Amount) values (@ProductName, @Description, @Amount)", new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });

                if (affected == 0)
                    return false;

                return true;
            }
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using (var newConnection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString")))
            {
                var affected = await newConnection.ExecuteAsync("UPDATE Coupon Set Description = @Description, Amount = @Amount, ProductName = @ProductName WHERE Id = @Id", new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount, Id = coupon.Id });

                if (affected == 0)
                    return false;

                return true;
            }
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            using (var newConnection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString")))
            {
                var affected = await newConnection.ExecuteAsync("DELETE from Coupon where ProductName = @ProductName", new { ProductName = productName });

                if (affected == 0)
                    return false;
                return true;
            }
        }
    }
}
