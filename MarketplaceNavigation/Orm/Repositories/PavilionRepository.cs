using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using SQLite;

using MarketplaceNavigation.Models;

namespace MarketplaceNavigation.Orm
{
    public class PavilionRepository : Repository<Pavilion>, IPavilionRepository
    {
        private const string tag = "PavilionRepository";
        private int floorId;
        public PavilionRepository(SQLiteAsyncConnection db, int floorId)
            : base(db)
        {
            this.floorId = floorId;
        }

        public override Pavilion Get(int id)
        {
            return db.Table<Pavilion>()
                .FirstOrDefaultAsync(p => p.Id == id && p.FloorId == floorId)
                .Result;
        }

        public Pavilion GetByPosition(int pos)
        {
            return db.Table<Pavilion>()
                        .Where(p => p.FloorId == floorId)
                        .OrderBy(p => p.Number)
                        .ElementAtAsync(pos)
                        .Result;
        }

        public Pavilion GetByNum(int num)
        {
            return db.Table<Pavilion>()
                .FirstOrDefaultAsync(p => p.Number == num && p.FloorId == floorId).Result;
        }

        public int GetPavilionPosition(Pavilion pavilion)
        {
            var pavilions = db.Table<Pavilion>()
                            .Where(p => p.FloorId == floorId)
                            .OrderBy(p => p.Number)
                            .ToListAsync()
                            .Result;
            for (int i = 0; i < pavilions.Count; i++)
                if (pavilions[i].Id == pavilion.Id)
                    return i;
            return -1;
        }

        public override IEnumerable<Pavilion> GetList()
        {
            return db.Table<Pavilion>()
                .Where(p => p.FloorId == floorId)
                .ToListAsync()
                .Result;
        }

        public override Pavilion[] GetArray()
        {
            return db.Table<Pavilion>()
                .Where(p => p.FloorId == floorId)
                .ToArrayAsync()
                .Result;
        }

        public void SortWithUpdate(int beginPosition, int count)
        {
            Pavilion pavilion1;
            Log.Info(tag, $"starting sort from {beginPosition.ToString()} to {(count + beginPosition).ToString()}");
            if (beginPosition >= 0)
                pavilion1 = GetByPosition(beginPosition);
            else
            {
                pavilion1 = GetByPosition(beginPosition + 1);
                pavilion1.Number -= 2;
            }

            var pavilions = db.Table<Pavilion>()
                                    .Where(p => p.FloorId == floorId)
                                    .OrderBy(p => p.Number)
                                    .ToListAsync()
                                    .Result
                                    .GetRange(beginPosition + 1, count);

            foreach (var pavilion2 in pavilions)
            {
                Log.Info(tag, $"pavilion1 num #{pavilion1.Number}; pavilion2 num#{pavilion2.Number}");
                if(pavilion2.Number - pavilion1.Number > 1)
                {
                    pavilion2.Number = pavilion1.Number + 1;
                    Update(pavilion2);
                }
                pavilion1 = pavilion2;
            }
            Log.Info(tag, "end of update");
        }
        //public void SortWithUpdate(Pavilion startPavilion, int endPosition)
        //{
        //    int beginPosition = GetPavilionPosition(startPavilion);
        //    Log.Info(tag, $"begin position {beginPosition.ToString()}");
        //    foreach (var pavilion2 in db.Table<Pavilion>()
        //                            .Where(p => p.FloorId == floorId)
        //                            .OrderBy(p => p.Number)
        //                            .ToListAsync()
        //                            .Result
        //                            .GetRange(beginPosition + 1, endPosition - beginPosition - 1))
        //    {
        //        if (pavilion2.Number - startPavilion.Number > 1)
        //        {
        //            pavilion2.Number--;
        //            Update(pavilion2);
        //        }
        //        startPavilion = pavilion2;
        //    }
        //}

        public IEnumerable<Pavilion> Search(string query)
        {
            IEnumerable<int> pavilionsId = db.Table<Shop>()
                                .ToArrayAsync().Result
                                .Where(s => s.Name.Contains(query))
                                .Select(s => s.PavilionId);

            IEnumerable<Pavilion> pavilions = db.Table<Pavilion>()
                                .ToArrayAsync().Result
                                .Where(p => p.FloorId == floorId)
                                .Where(p => p.Number.ToString().Contains(query) ||
                                        pavilionsId.Contains(p.Id));

            return pavilions;
        }

        public override int Insert(Pavilion pavilion)
        {
            pavilion.FloorId = floorId;
            return db.InsertAsync(pavilion).Result;
        }

        public override int Update(Pavilion pavilion)
        {
            return db.UpdateAsync(pavilion).Result;
        }

        public override int Delete(int id)
        {
            return db.DeleteAsync<Pavilion>(id).Result;
        }

        public override int Save(Pavilion floor)
        {
            return db.InsertOrReplaceAsync(floor).Result;
        }

        public override int Count
        {
            get => db.Table<Pavilion>()
                .Where(p => p.FloorId == floorId)
                .CountAsync()
                .Result;
        }

        public bool CanInsert(Pavilion pavilion)
        {
            int resultSize
                = db.Table<Floor>()
                .FirstOrDefaultAsync(f => f.Id == floorId)
                .Result
                .EmptySize - pavilion.Size;
            if (resultSize < 0)
                return false;
            else
                return true;
        }

        public bool CanUpdate(Pavilion pavilion)
        {
            int newEmptySize = db.Table<Pavilion>()
                .FirstOrDefaultAsync(p => p.Id == pavilion.Id)
                .Result
                .Size - pavilion.Size;

            int floorEmptySize =
                db.Table<Floor>()
                .FirstOrDefaultAsync(f => f.Id == floorId)
                .Result
                .EmptySize;

            if (floorEmptySize + newEmptySize < 0)
                return false;
            else
                return true;
        }

    }
}