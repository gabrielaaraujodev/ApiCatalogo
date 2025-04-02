using ApiCatalogo.Context;

namespace ApiCatalogo.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private IProductRepository? _productRepo;

        private ICategoryRepository? _categoryRepo;

        public AppDbContext _context;

        public UnitOfWork (AppDbContext context)
        {
            _context = context;
        }

        public IProductRepository ProductRepository
        {
            get
            {
                return _productRepo = _productRepo ?? new ProductRepository(_context);

                /*
                    if (_productRepo == null)
                        _productRepo = new ProductRepository(_context);

                    return _productRepo;
                 */
            }
        }

        public ICategoryRepository CategoryRepository
        {
            get
            {
                return _categoryRepo = _categoryRepo ?? new CategoryRepository(_context);
            }
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
