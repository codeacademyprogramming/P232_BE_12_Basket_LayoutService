using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pustok.DAL;
using Pustok.ViewModels;

namespace Pustok.Controllers
{
    public class BookController : Controller
    {
        private readonly PustokDbContext _context;

        public BookController(PustokDbContext context)
        {
            _context = context;
        }

        public IActionResult GetBookModal(int id)
        {
            var book = _context.Books
                .Include(x=>x.Genre)
                .Include(x => x.Author)
                .Include(x => x.BookImages)
                .FirstOrDefault(x=>x.Id == id);

            return PartialView("_BookModalPartial",book);
        }

        public IActionResult AddToBasket(int id)
        {
            List<BasketItemViewModel> basketItems;
            var basket = HttpContext.Request.Cookies["basket"];

            if (basket == null)
                basketItems = new List<BasketItemViewModel>();
            else
                basketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basket);

            var wantedBook = basketItems.FirstOrDefault(x => x.BookId == id);
            
            if (wantedBook == null)
                basketItems.Add(new BasketItemViewModel { Count = 1, BookId = id });
            else
                wantedBook.Count++;


            HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketItems));
            return RedirectToAction("index", "home");
        }

        public IActionResult ShowBasket()
        {
            var basket = HttpContext.Request.Cookies["basket"];
            var basketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basket);


            return Json(basketItems);
        }

    }
}
