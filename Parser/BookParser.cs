using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WeeebLibrary.DAL.Database.Entitys;
using WeeebLibrary.DAL.InterfacesDLL;

namespace WeeebLibrary.Parser
{
    public class BookParser : IJob
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<BookParser> logger;
        private readonly IHttpClientFactory clientFactory;

        public BookParser(IServiceProvider serviceProvider, ILogger<BookParser> logger, IHttpClientFactory clientFactory)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.clientFactory = clientFactory;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var bookRepository = scope.ServiceProvider.GetService<IRepository<Book>>();
                var lastRepository = scope.ServiceProvider.GetService<ILastIdRepository>();

                var books = bookRepository.GetAll();
                var booksCount = 50;
                var httpСlient = clientFactory.CreateClient();
                var startId = lastRepository.Get();

                if (startId == null)
                {
                    startId = new LastParserId { LastId = 242295 };
                    lastRepository.Create(startId);
                }

                for (int i = 0; i <= booksCount; i++)
                {
                    //получение страницы сайта "Читай город"                   
                    var response = await httpСlient.GetAsync("https://www.chitai-gorod.ru/catalog/book/" + startId.LastId);
                    var pageContents = await response.Content.ReadAsStringAsync();
                    var pageDocument = new HtmlDocument();
                    pageDocument.LoadHtml(pageContents);

                    //проверка на существование страницы 
                    if (pageDocument.DocumentNode.SelectSingleNode("//title").InnerText == "Читай-город 404")
                    {
                        startId.LastId++;
                        booksCount++;
                        continue;
                    }

                    //параметры книги 
                    string bookName, bookAutor, bookPublisher, bookGenre, bookDesc;
                    var bookImg = pageDocument.DocumentNode.SelectSingleNode("//div[@class='product__image']//img[@class='lazyload']").Attributes["data-src"].Value;

                    //проверка на существование параметров и присвоение их
                    if (pageDocument.DocumentNode.SelectSingleNode("//meta[@property='og:title']") != null)
                    {
                        bookName = pageDocument.DocumentNode.SelectSingleNode("//meta[@property='og:title']").Attributes["content"].Value;
                    }
                    else
                    {
                        bookName = "Название не найдено";
                    }

                    if (pageDocument.DocumentNode.SelectSingleNode("//meta[@property='book:author']") != null)
                    {
                        bookAutor = pageDocument.DocumentNode.SelectSingleNode("//meta[@property='book:author']").Attributes["content"].Value;
                    }
                    else
                    {
                        bookAutor = "Автор не найден";
                    }

                    if (pageDocument.DocumentNode.SelectSingleNode("(//div[@class='product-prop__value']//a[@class='link'])[1]") != null)
                    {
                        bookPublisher = pageDocument.DocumentNode.SelectSingleNode("(//div[@class='product-prop__value']//a[@class='link'])[1]").InnerText;
                    }
                    else
                    {
                        bookPublisher = "Издатель не найден";
                    }

                    if (pageDocument.DocumentNode.SelectSingleNode("(//div[@class='container']//span[@itemprop='name'])[4]") != null)
                    {
                        bookGenre = pageDocument.DocumentNode.SelectSingleNode("(//div[@class='container']//span[@itemprop='name'])[4]").InnerText;
                    }
                    else
                    {
                        bookGenre = "Жанр не найден";
                    }

                    if (pageDocument.DocumentNode.SelectSingleNode("//div[@itemprop='description']") != null)
                    {
                        bookDesc = pageDocument.DocumentNode.SelectSingleNode("//div[@itemprop='description']").InnerText;
                    }
                    else
                    {
                        bookDesc = "Это книга.";
                    }

                    //скачивание обложки
                    var path = "/Files/" + bookName + ".jpg";
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
                    logger.LogInformation("Добавлена книга: " + bookName);
                }
            }
            await Task.CompletedTask;
        }
    }
}