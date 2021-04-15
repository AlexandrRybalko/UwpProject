﻿using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Repositories
{
    public class CameraRepository : IDisposable
    {
        private DatabaseContext ctx;

        public CameraRepository()
        {
            ctx = new DatabaseContext(); 
        }

        public IEnumerable<CameraEntity> GetAll()
        {
            return ctx.Cameras.Include(x => x.CamerasCategories).ToList();
        }

        public void Add(CameraEntity camera)
        {
            ctx.Cameras.Add(camera);

            ctx.SaveChanges();
        }

        public void AddToCategory(int cameraId, string categoryName)
        {
            ctx = new DatabaseContext();
            var category = ctx.Categories.FirstOrDefault(x => x.Title.Equals(categoryName));

            if (category != null)
            {
                var entity = new CamerasCategories
                {
                    CameraId = cameraId,
                    CategoryId = category.Id,
                    UpdatedTime = DateTime.UtcNow
                };

                var entityToUpdate = ctx.CamerasCategories.FirstOrDefault(x => x.CameraId == entity.CameraId &&
                x.CategoryId == entity.CategoryId);

                if (entityToUpdate == null)
                {
                    ctx.CamerasCategories.Add(entity);
                }
                else
                {
                    entityToUpdate.UpdatedTime = entity.UpdatedTime;
                }
            }
            
            ctx.SaveChanges();
        }

        public CameraEntity GetById(int id)
        {
            return ctx.Cameras.FirstOrDefault(x => x.Id == id);
        }

        public void UpdateCameras(IEnumerable<CameraEntity> newCameras)
        {
            if (newCameras == null)
            {
                throw new ArgumentNullException(nameof(newCameras));
            }

            var allCameras = this.GetAll().ToList();
            List<CameraEntity> camerasToDelete = new List<CameraEntity>();
            List<CameraEntity> camerasToAdd = new List<CameraEntity>();

            foreach (var cameraToDelete in allCameras)
            {
                if (!newCameras.Contains(cameraToDelete, new CameraEqualityComparer()))
                {
                    camerasToDelete.Add(cameraToDelete);
                }
            }

            foreach (var cameraToAdd in newCameras)
            {
                if (!allCameras.Contains(cameraToAdd, new CameraEqualityComparer()))
                {
                    camerasToAdd.Add(cameraToAdd);
                }
            }

            ctx.Cameras.RemoveRange(camerasToDelete);
            ctx.Cameras.AddRange(camerasToAdd);

            ctx.SaveChanges();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            ctx.Dispose();
        }
    }

    class CameraEqualityComparer : IEqualityComparer<CameraEntity>
    {
        public bool Equals(CameraEntity x, CameraEntity y)
        {
            if (x.RtspAddress.Equals(y.RtspAddress) && x.Country.Equals(y.Country) && x.City.Equals(y.City))
            {
                return true;
            }

            return false;
        }

        public int GetHashCode(CameraEntity obj)
        {
            return obj.Id;
        }
    }
}
