using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RestaurantAPI.Services
{
    public interface IRestaurantService
    {
        RestaurantDto GetById (int id);
        IEnumerable<RestaurantDto> GetAll();
        int Create(CreateRestaurantDto dto);
        bool Delete(int id);
        bool Modify(int id, UpdaateRestaurantDto dto);
    }

    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public RestaurantService(RestaurantDbContext dbContext, IMapper mapper, ILogger<RestaurantService> logger) 

        { 
            _dbContext = dbContext;
            _mapper = mapper;
            _logger =  logger;
        }

        public RestaurantDto GetById(int id)
        {
            var restaurants = _dbContext.Restaurants.
                Include(r => r.Address)
                .Include(r => r.Dishes)
                .FirstOrDefault(r => r.Id == id);

            if (restaurants == null) return null;

            var results = _mapper.Map<RestaurantDto>(restaurants);
            return results;
        }

        public IEnumerable<RestaurantDto> GetAll() 
        {  
                var restaurants = _dbContext.Restaurants.
                    Include(r => r.Address)
                    .Include(r => r.Dishes)
                    .ToList();

                var results = _mapper.Map<List<RestaurantDto>>(restaurants);
                return results;

        }

        public int Create(CreateRestaurantDto dto)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();

            return restaurant.Id;
        }

        public bool Delete(int id)
        {
            _logger.LogError($"Restaurant with id: {id} DELETE action invoked");

            var restaurants = _dbContext.Restaurants
              .FirstOrDefault(r => r.Id == id);

            if (restaurants == null)
            {
                return false;
            }

            _dbContext.Restaurants.Remove(restaurants);
            _dbContext.SaveChanges();

            return true;
        }

        public bool Modify(int id, UpdaateRestaurantDto dto)
        {
            var restaurants = _dbContext.Restaurants
              .FirstOrDefault(r => r.Id == id);

            if (restaurants == null)
                return false;

            restaurants.Name = dto.Name;
            restaurants.Description = dto.Description;
            restaurants.HasDelivery = dto.HasDelivery;

            _dbContext.SaveChanges();

            return true;

        }

    }
}
