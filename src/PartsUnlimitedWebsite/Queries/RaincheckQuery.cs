// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using PartsUnlimited.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PartsUnlimited.Queries
{
    public class RaincheckQuery : IRaincheckQuery
    {
        private readonly PartsUnlimitedContext _context;

        public RaincheckQuery(PartsUnlimitedContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Raincheck>> GetAllAsync()
        {
            var rainchecks = _context.RainChecks.ToList();

            foreach (var raincheck in rainchecks)
            {
                await FillRaincheckValuesAsync(raincheck);
            }

            return rainchecks;
        }

        public async Task<Raincheck> FindAsync(int id)
        {
            var raincheck = _context.RainChecks.FirstOrDefault(r => r.RaincheckId == id);

            if (raincheck == null)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            await FillRaincheckValuesAsync(raincheck);

            return raincheck;
        }

        public async Task<int> AddAsync(Raincheck raincheck)
        {
            var addedRaincheck = _context.RainChecks.Add(raincheck);

            await _context.SaveChangesAsync(CancellationToken.None);

            return addedRaincheck.Entity.RaincheckId;
        }

        /// <summary>
        /// Lazy loading is not currently available with EF 7.0, so this loads the Store/Product/Category information
        /// </summary>
        private async Task FillRaincheckValuesAsync(Raincheck raincheck)
        {
            raincheck.IssuerStore =  _context.Stores.First(s => s.StoreId == raincheck.StoreId);
            raincheck.Product = _context.Products.First(p => p.ProductId == raincheck.ProductId);
            raincheck.Product.Category = _context.Categories.ToList().First(c => c.CategoryId == raincheck.Product.CategoryId);
        }
    }
}