using HtmlAgilityPack;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WeeebLibrary.DAL.Database.Entitys;
using WeeebLibrary.DAL.InterfacesDLL;

namespace WeeebLibrary.BLL.Parser2
{
    public class Parser2
    {/*
        public async static Task Parsing(IRepository<Book> bookRepository)
        {
            var books = bookRepository.GetAll();
            var startId = 242295;
            var booksCount = 10;

            for (int i = 0; i <= booksCount; i++)
            {
                //получение страницы сайта "Читай город"

                var httpСlient = new HttpClient();
                var response = await httpСlient.GetAsync("https://www.chitai-gorod.ru/catalog/book/" + startId);
                var pageContents = await response.Content.ReadAsStringAsync();
                var pageDocument = new HtmlDocument();
                pageDocument.LoadHtml(pageContents);

                //проверка на существование страницы 
                if (pageDocument.DocumentNode.SelectSingleNode("//title").InnerText == "Читай-город 404")
                {
                    startId++;
                    continue;
                }
                var bookName = pageDocument.DocumentNode.SelectSingleNode("//meta[@property='og:title']").Attributes["content"].Value;
                var bookImg = pageDocument.DocumentNode.SelectSingleNode("//div[@class='product__image']//img[@class='lazyload']").Attributes["data-src"].Value;

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
                    Autor = pageDocument.DocumentNode.SelectSingleNode("//meta[@property='book:author']").Attributes["content"].Value,
                    Publisher = pageDocument.DocumentNode.SelectSingleNode("(//div[@class='product-prop__value']//a[@class='link'])[1]").InnerText,
                    Genre = pageDocument.DocumentNode.SelectSingleNode("(//div[@class='container']//span[@itemprop='name'])[4]").InnerText,
                    Desc = pageDocument.DocumentNode.SelectSingleNode("//div[@itemprop='description']").InnerText,
                    Img = bookName,
                    ImgPath = path
                };

                bookRepository.Create(book);
                startId++;
            }
        }*/
    }
}
