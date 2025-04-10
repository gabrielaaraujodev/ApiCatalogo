namespace ApiCatalogo.Repositories
{
    public interface IUnitOfWork
    {
        /*
            Usar esta abordagem de repositórios específicos faz com que
            se tenha acesso tanto aos métodos do repositório genérico
            e dos que, porventura, tenham no específico.
         */
        IProductRepository ProductRepository { get; }
        ICategoryRepository CategoryRepository { get; }

        Task CommitAsync();
    }
}
