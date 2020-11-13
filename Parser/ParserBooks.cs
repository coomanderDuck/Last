using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WeeebLibrary.DAL.Database.Entitys;
using WeeebLibrary.DAL.InterfacesDLL;

namespace Parser 
{
    public class ParserBooks : IParserBooks
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<ParserBooks> logger;
        private readonly IHttpClientFactory clientFactory;

        public ParserBooks(IServiceProvider serviceProvider, ILogger<ParserBooks> logger, IHttpClientFactory clientFactory)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.clientFactory = clientFactory;
        }
        public async Task Pars(int parsCount)
        {
            using var scope = serviceProvider.CreateScope();
            var bookRepository = scope.ServiceProvider.GetService<IRepository<Book>>();
            var lastRepository = scope.ServiceProvider.GetService<ILastIdRepository>();
            var books = bookRepository.GetAll();
            var httpСlient = clientFactory.CreateClient();
            var startId = lastRepository.Get();
            var b = 0;

            if (startId == null)
            {
                startId = new LastParserId { LastId = 242295 };
                lastRepository.Create(startId);
            }

            for (int i = 0; i < parsCount; i++)
            {
                //получение страницы сайта "Читай город"                   
                var response = await httpСlient.GetAsync("https://www.chitai-gorod.ru/catalog/book/" + startId.LastId);
                var pageContents = await response.Content.ReadAsStringAsync();
                var pageDocument = new HtmlDocument();
                pageDocument.LoadHtml(pageContents);

                //проверка на существование страницы 
                if ((pageDocument.DocumentNode.SelectSingleNode("//title") == null) ^ (pageDocument.DocumentNode.SelectSingleNode("//title").InnerText == "Читай-город 404"))
                {
                    startId.LastId++;
                    parsCount++;
                    continue;
                }

                //параметры книги 
                var bookName = "Название не найдено";
                var bookAutor = "Автор не найден";
                var bookPublisher = "Издатель не найден";
                var bookGenre = "Жанр не найден";
                var bookDesc = "Это книга.";
                var bookImg = pageDocument.DocumentNode.SelectSingleNode("//div[@class='product__image']//img[@class='lazyload']").Attributes["data-src"].Value;

                //проверка на существование параметров и присвоение их
                if (pageDocument.DocumentNode.SelectSingleNode("//meta[@property='og:title']") != null)
                {
                    bookName = pageDocument.DocumentNode.SelectSingleNode("//meta[@property='og:title']").Attributes["content"].Value;
                }

                if (pageDocument.DocumentNode.SelectSingleNode("//meta[@property='book:author']") != null)
                {
                    bookAutor = pageDocument.DocumentNode.SelectSingleNode("//meta[@property='book:author']").Attributes["content"].Value;
                }

                if (pageDocument.DocumentNode.SelectSingleNode("(//div[@class='product-prop__value']//a[@class='link'])[1]") != null)
                {
                    bookPublisher = pageDocument.DocumentNode.SelectSingleNode("(//div[@class='product-prop__value']//a[@class='link'])[1]").InnerText;
                }

                if (pageDocument.DocumentNode.SelectSingleNode("(//div[@class='container']//span[@itemprop='name'])[4]") != null)
                {
                    bookGenre = pageDocument.DocumentNode.SelectSingleNode("(//div[@class='container']//span[@itemprop='name'])[4]").InnerText;
                }

                if (pageDocument.DocumentNode.SelectSingleNode("//div[@itemprop='description']") != null)
                {
                    bookDesc = pageDocument.DocumentNode.SelectSingleNode("//div[@itemprop='description']").InnerText;
                }

                //скачивание обложки
                var path = "/Files/" + bookName.Replace("/", "") + ".jpg";
                var localFilename = "wwwroot" + path;
                var webClient = new WebClient();
                {
                    webClient.DownloadFile(bookImg, localFilename);
                }

                var book = new Book
                {
                    Name = bookName,
                    Autor = bookAutor,
                    Publisher = bookPublisher,
                    Genre = bookGenre,
                    Desc = bookDesc,
                    Img = bookName,
                    ImgPath = path
                };

                await bookRepository.CreateAsync(book);
                startId.LastId++;
                lastRepository.Update(startId);
                b++;
                logger.LogInformation(b + " Добавлена книга: " + bookName);
            }
        }
    }
}
