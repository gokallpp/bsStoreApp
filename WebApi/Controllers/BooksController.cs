using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Repositories.EFCore;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly RepositoryContext _context;
        public BooksController(RepositoryContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAllBooks()
        {
            try
            {
                var books = _context.Books.ToList();
                return Ok(books);
            }
            catch (Exception ex)
            {
                throw new($"Kitaplar getirilirken bir hata oluştu: {ex.Message}");

            }
        }

        [HttpGet("{id:int}")]
        public IActionResult GetOneBooks([FromRoute(Name = "id")] int id)
        {
            try
            {
                var book = _context
                    .Books
                    .Where(b => b.Id.Equals(id))
                    .SingleOrDefault(); // Verilen id'ye sahip kitabı veritabanından bulmaya çalışır. Eğer kitap bulunmazsa, null döner.

                if (book is null)
                    return NotFound(); // Kitap bulunamazsa, 404 Bulunamadı yanıtı gönder

                return Ok(book);

            }

            catch (Exception ex)
            {
                throw new Exception($"Kitap getirilirken bir hata oluştu: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult CreateOneBook([FromBody] Book book)// Kitap oluşturma işlemi için HTTP POST isteği
        {
            // Kitap oluşturma işlemi burada yapılır (örneğin, kitap veritabanına eklenir)
            try
            {
                if (book is null) // Gönderilen kitap nesnesi null ise, geçersiz istek olarak kabul edilir
                    return BadRequest(); // Geçersiz istek durumunda 400 Bad Request yanıtı gönder

                _context.Books.Add(book); // Kitabı kitap listesine ekle
                _context.SaveChanges();
                return StatusCode(201, book); // Başarılı oluşturma durumunda 201 Created yanıtı gönder ve oluşturulan kitabı döndür

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpPut("{id:int}")]
        public IActionResult UpdateOneBook([FromRoute(Name = "id")] int id, [FromBody] Book book) // Kitap güncelleme işlemi için HTTP PUT isteği
        {
            try
            {
                var entity = _context
                .Books
                .Where(b => b.Id.Equals(id)) // Verilen id'ye sahip kitabı kitap listesinde bulmaya çalışır
                .SingleOrDefault(); // Eğer kitap bulunmazsa, null döner

                if (entity is null)
                    return NotFound(); // Kitap bulunamazsa, 404 Bulunamadı yanıtı gönder

                //check id
                if (id != book.Id)
                    return BadRequest(); //404

                entity.Title = book.Title; // Kitabın başlığını güncelle
                entity.Price = book.Price; // Kitabın fiyatını güncelle

                _context.SaveChanges();

                return Ok(book); // Başarılı güncelleme durumunda 200 OK yanıtı gönder ve güncellenmiş kitabı döndür

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }    

        }


        [HttpDelete("{id:int}")]
        public IActionResult DeleteOneBook([FromRoute(Name = "id")] int id)
        {
            try
            {
                var entity = _context.Books.Where(b => b.Id.Equals(id)).SingleOrDefault();

                if (entity is null)
                    return NotFound(new
                    {
                        StatusCode = 404,
                        message = $"Id'si {id} olan kitap bulunamadı."
                    });

                _context.Books.Remove(entity); // Kitabı kitap listesinden kaldır
                _context.SaveChanges();

                return NoContent();

            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
           
        }

        [HttpPatch("{id:int}")] // Kitap kısmi güncelleme işlemi için HTTP PATCH isteği
        public IActionResult PartialUpdateOneBook([FromRoute(Name = "id")] int id, [FromBody] JsonPatchDocument<Book> bookPatch)
        {
            try
            {
                var entity = _context.Books.Where(b => b.Id.Equals(id)).SingleOrDefault();
                if (entity is null)
                    return NotFound(); //404

                bookPatch.ApplyTo(entity);
                _context.SaveChanges(); 

                return NoContent();

            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }
    }
}
