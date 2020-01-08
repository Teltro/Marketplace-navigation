using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using Android.Util;

using MarketplaceNavigation.Models;

namespace MarketplaceNavigation.Orm
{
    public class FloorRepository : Repository<Floor>, IFloorRepository
    {
        private const string tag = "FloorRepository";
        //private const int FLOOR_COUNT = 9;

        public FloorRepository(SQLiteAsyncConnection db)
            : base(db)
        {
        }

        public Floor GetByPosition(int pos)
        {
            return db.Table<Floor>()
                .OrderBy(f => f.Number)
                .ElementAtAsync(pos)
                .Result;
        }

        public IEnumerable<Floor> PavilionSearching(string query)
        {
            IEnumerable<int> pavilionsId = db.Table<Shop>()
                                .ToListAsync().Result
                                .Where(s => s.Name.StartsWith(query))
                                .Select(s => s.PavilionId)
                                .ToArray();

            Log.Info(tag, $"pavilionsId count = {pavilionsId.Count()}");

            IEnumerable<int> floorsId = db.Table<Pavilion>()
                                .ToListAsync().Result
                                .Where(p => p.Number.ToString().StartsWith(query) ||
                                        pavilionsId.Contains(p.Id))
                                .Select(p => p.FloorId)
                                .ToArray();

            Log.Info(tag, $"floorsId count = {floorsId.Count()}");

            IEnumerable<Floor> floors = db.Table<Floor>()
                                .ToListAsync().Result
                                .Where(f => floorsId.Contains(f.Id))
                                .ToList();

            Log.Info(tag, $"floors count = {floors.Count()}");

            return floors;
        }

    }
}