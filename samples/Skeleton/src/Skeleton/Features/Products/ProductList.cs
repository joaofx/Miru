using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Miru.Mvc;
using Skeleton.Database;

namespace Skeleton.Features.Products
{
    public class ProductList
    {
        public class Query : IRequest<Result>
        {
        }

        public class Result
        {    
            public IReadOnlyList<Item> Items { get; set; } = new List<Item>();
        }

        public class Item
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly SkeletonDbContext _db;
            
            public Handler(SkeletonDbContext db)
            {
                _db = db;
            }
            
            public async Task<Result> Handle(Query request, CancellationToken ct)
            {
                return new Result
                {
                    Items = await _db.Products
                        .Select(m => new Item
                        {
                            Id = m.Id,
                            Name = m.Name
                        })
                        .ToListAsync(ct)
                };
            }
        }
        
        public class ProductsController : MiruController
        {
            [Route("/Products")]
            public async Task<Result> List(Query request) => await Send(request);
        }
    }
}