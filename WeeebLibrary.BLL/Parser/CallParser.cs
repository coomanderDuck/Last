using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using WeeebLibrary.DAL.Database.Entitys;
using WeeebLibrary.DAL.InterfacesDLL;

namespace WeeebLibrary.BLL.Parser
{
    public class CallParser
    {
        public static async Task BooksParsingAsync(IApplicationBuilder app)
        {
            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();

            var scope = scopeFactory.CreateScope();

            var bookRepository = scope.ServiceProvider.GetRequiredService<IRepository<Book>>();

           // await Parser.Parsing(bookRepository);
        }
    }
}
