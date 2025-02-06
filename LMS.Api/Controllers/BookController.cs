using AutoMapper;
using LMS.Api.DTOs;
using LMS.Core.Entities;
using LMS.Core.Interfaces;
using LMS.Infrastructure.Repositories;
using LMS.Infrastructure.Services.AuthService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using System.Diagnostics;
using System.Net;
using System.Text.Json.Serialization;
using static System.Reflection.Metadata.BlobBuilder;

namespace LMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper mapper;
        private readonly IFileService fileService;
        protected readonly APIResponse response;
      


        public BookController(IUnitOfWork _unitOfWork, IMapper mapper, IFileService fileService)//, ILogger<BookController> logger
        {
            this._unitOfWork = _unitOfWork;
            this.mapper = mapper;
            this.response = new APIResponse();
            this.fileService = fileService;
          


        }

        [HttpGet]
        [Authorize(Roles = "Librarian")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ResponseCache(Duration = 30)]


        //en al admin yezahrlo kol el kotob  b anwa3ha bely 3amlenlha borrow currently 
        public async Task<ActionResult<APIResponse>> GetAllBooks( int pagesize = 0, int pagenumber = 1)
        {
            try
            {


                var res = await _unitOfWork.BookRepository.getAllAsync(b => !b.IsDeleted, new[] { "Borrows.User", "BookGenres.Genre" },pagenumber:pagenumber,pagesize:pagesize);
                if (!res.Any())//mesh !=null l2enha law mal2tsh hatraga3e mpty list msh null
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.Errors.Add("No Books are Available");
                    return NotFound(response);
                }
                //mapping to avoid serialization issues
                var bg = mapper.Map<List<BookWithGenreAndBorrowsDTO>>(res);

                response.StatusCode = HttpStatusCode.OK;
                response.IsSuccess = true;
                response.Result = bg;
                return Ok(response);

            }
            catch (Exception ex)
            {

                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.Errors.Add(ex.Message);
                return StatusCode(500, response);

            }

        }
        [HttpGet("AvailableBooks")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ResponseCache(Duration =30)]
        //el user yshoof el books el available to borrow
        public async Task<ActionResult<APIResponse>> GetAvailableBooks(int pagesize = 0, int pagenumber = 1)
        {
            try
            {
                var books = await _unitOfWork.BookRepository.getAllAsync(b => !b.IsDeleted && b.IsAvailable, new[] { "BookGenres.Genre" },pagesize:pagesize,pagenumber:pagenumber);

                if (!books.Any())
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.Errors.Add("No Books are Available");
                    return NotFound(response);
                }

                //map book to BookwithGenreDto to avoid cycles when serializing

                List<BookWithGenreDTO> bg = mapper.Map<List<BookWithGenreDTO>>(books);

                response.StatusCode = HttpStatusCode.OK;
                response.IsSuccess = true;
                response.Result = bg;
                return Ok(response);

            }
            catch (Exception ex)
            {

                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.Errors.Add(ex.Message);
                return StatusCode(500, response);

            }

        }

        [HttpGet("bygenre/{Genre:alpha}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ResponseCache(Duration = 30)]


        //el user ye3ml search hasab el genre
        public async Task<ActionResult<APIResponse>> GetBooksByGenre(string Genre, int pagesize = 0, int pagenumber = 1)
        {
            try
            {
                var books = await _unitOfWork.BookRepository.getAllAsync(b => !b.IsDeleted && b.BookGenres.Any(bg => bg.Genre.Name.ToLower() == Genre.Trim().ToLower()), new[] { "BookGenres.Genre" },pagesize:pagesize,pagenumber:pagenumber);
                //map book to BookwithGenreDto to avoid cycles when serializing
                if (!books.Any())
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.Errors.Add("No Books are Available");
                    return NotFound(response);
                }

                /*   Used AutoMappr instead    
                 *   List<BookWithGenreDTO> bg = new List<BookWithGenreDTO>();

                    foreach (var book in books)
                    {
                        bg.Add(new BookWithGenreDTO()
                        {
                            Name = book.Name,
                            Author = book.Author,
                            Description = book.Description,
                            Title = book.Title,
                            genres = book.BookGenres?.Select(bg => bg.Genre?.Name).ToList() ?? new List<string>()
                        });
                    }
    */

                var bg = mapper.Map<List<BookWithGenreDTO>>(books);
                response.StatusCode = HttpStatusCode.OK;
                response.IsSuccess = true;
                response.Result = bg;
                return Ok(response);

            }
            catch (Exception ex)
            {

                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.Errors.Add(ex.Message);
                return StatusCode(500, response);

            }

        }

        [HttpGet("byname/{Name}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]//ata2ked keda
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ResponseCache(Duration = 30)]


        public async Task<ActionResult<APIResponse>> GetBookByName(string Name)//el user masaln 3ayz yshoof el ketab ely ekhtaro full screen
        {
            try
            {
               
                var book = await _unitOfWork.BookRepository.getAsync(b => !b.IsDeleted && b.Name.ToLower().Contains(Name.Trim().ToLower()), false, new[] { "BookGenres.Genre" });
                if (book == null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.Errors.Add("The requested book with the given Name was not found");
                    return NotFound(response);
                }
               

                var bg = mapper.Map<BookWithGenreDTO>(book);
                response.StatusCode = HttpStatusCode.OK;
                response.IsSuccess = true;
                response.Result = bg;
                return Ok(response);

            }
            catch (Exception ex)
            {

                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.Errors.Add(ex.Message);
                return StatusCode(500, response);

            }

        }









        [HttpGet("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]//ata2ked keda
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ResponseCache(Duration = 30)]


        public async Task<ActionResult<APIResponse>> GetBookById(int id)//el user masaln 3ayz yshoof el ketab ely ekhtaro full screen
        {
            try
            {
                if (id <= 0)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    response.Errors.Add("Invalid book ID.");
                    return BadRequest(response);
                }
                var book = await _unitOfWork.BookRepository.getAsync(b => !b.IsDeleted && b.Id == id, true, new[] { "BookGenres.Genre" });
                if (book == null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.Errors.Add("The requested book with the given ID was not found");
                    return NotFound(response);
                }
                //map book to BookwithGenreDto to avoid cycles when serializing
                //mapping from book to bookgenre

                /* Used Auto Mapper instead
                 * BookWithGenreDTO bg = new BookWithGenreDTO()
                 {
                     Name = book.Name,
                     Author = book.Author,
                     Description = book.Description,
                     Title = book.Title,
                     genres = book.BookGenres?.Select(bg => bg.Genre?.Name).ToList() ?? new List<string>()
                 };*/

                var bg = mapper.Map<BookWithGenreDTO>(book);
                response.StatusCode = HttpStatusCode.OK;
                response.IsSuccess = true;
                response.Result = bg;
                return Ok(response);

            }
            catch (Exception ex)
            {

                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.Errors.Add(ex.Message);
                return StatusCode(500, response);

            }

        }

        [HttpPost]
        [Authorize(Roles = "Librarian")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> AddBookWithAGenre([FromForm] BookCreateDTO bookfromreq)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    if (bookfromreq.Image == null || bookfromreq.Image.Length == 0)
                    {
                        return BadRequest("No Image uploaded.");
                    }


                    Book found = await _unitOfWork.BookRepository.getAsync(b => !b.IsDeleted && (b.Name == bookfromreq.Name || b.Title == bookfromreq.Title), tracking: false);
                    if (found != null)
                    {
                        response.StatusCode = HttpStatusCode.Conflict;
                        response.IsSuccess = false;
                        response.Errors.Add("Duplicate Name or Title");

                        return Conflict(response);
                    }

                    Genre gg = await _unitOfWork.GenreRepository.getAsync(g => !g.IsDeleted && g.Id == bookfromreq.GenreId);
                    if (gg == null)
                    {
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.IsSuccess = false;
                        response.Errors.Add("No Genre With This Id is found ");
                        return BadRequest(response);
                    }

                    Book b = mapper.Map<Book>(bookfromreq);


                    var imageUrl = fileService.SaveImage(bookfromreq.Image);
                    if (imageUrl.Item1 == 1)
                    {
                        b.Image = imageUrl.Item2; // getting image url
                    }
                
                    //transaction ensuring both are saved to database or none
                    using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {

                        //add book
                        await _unitOfWork.BookRepository.AddAsync(b);
                      int result1= await _unitOfWork.CompleteAsync();//mehtaga a3ml complete 3shan a3raf ageeb el bookId taht
                        Debug.WriteLine($"Book save affected rows: {result1}");


                        BookGenre bg = new BookGenre()
                        {
                            BookId = b.Id,
                            GenreId = gg.Id
                        };
                        //assign genre to book
                        await _unitOfWork.BookGenreRepository.AddAsync(bg);
                        int result2= await _unitOfWork.CompleteAsync();
                        Debug.WriteLine($"Book genre save affected rows: {result2}");

                        await transaction.CommitAsync();
                    
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }

                    BookWithGenreDTO bcd = mapper.Map<BookWithGenreDTO>(b);


                    response.StatusCode = HttpStatusCode.Created;
                    response.IsSuccess = true;
                    response.Result = bcd;

                    return CreatedAtAction(nameof(GetBookById), new { id = b.Id }, response);
                    //3shan ashoof ely rege3 fel response

                }
                else
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    response.Errors.AddRange(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));

                    return BadRequest(response);

                }
            }
            catch (Exception ex)
            {

                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.Errors.Add(ex.Message);
                return StatusCode(500, response);

            }


        }
/*
        public async Task <string> uploadPhoto(IFormFile imageinput)
        {

            try
            {
                string fileName = Path.GetFileName(imageinput.FileName);
                string filePath = Path.Combine(_imageFolderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageinput.CopyToAsync(stream);
                }

                return  $"/UploadedFiles/{fileName}";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }*/
       




        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Librarian")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> DeleteBook(int id)
        {
            try
            {
                var book = await _unitOfWork.BookRepository.getAsync(b => b.Id == id, true, new[] { "Borrows" });

                if (book == null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.Errors.Add("Book Not Found !");
                    return NotFound(response);
                }
                //checking if book is borrowed before deleting it
                if (book.Borrows.Any(bo => bo.ReturnDate == null))
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    response.Errors.Add("Book is currently borrowed and cannot be deleted.");
                    return Conflict(response);
                }

                //mark deleted -->soft delete
                book.IsAvailable = false;
                book.IsDeleted = true;
                _unitOfWork.BookRepository.Update(book);
                await _unitOfWork.CompleteAsync();

                response.StatusCode = HttpStatusCode.NoContent;
                response.IsSuccess = true;
                response.Result = book;

                //  _unitOfWork.BookRepository.Delete(book);

                return Ok(response);
                //return NoContent();
            }
            catch (Exception ex)
            {

                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.Errors.Add(ex.Message);
                return StatusCode(500, response);

            }

        }


        [HttpPut("{id:int}")]
        [Authorize(Roles = "Librarian")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> UpdateBook(int id, [FromBody] BookUpdateDTO BookFromReq)
        {
            try
            {
                var book = await _unitOfWork.BookRepository.getAsync(b => b.Id == id, tracking: false);
                if (book == null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.Errors.Add("Book Not Found !");
                    return NotFound(response);
                }
                if (ModelState.IsValid)
                {
                    //map bookFromReq to Book
                    var toupdate = mapper.Map<Book>(BookFromReq);
                    toupdate.Id = book.Id;
                    _unitOfWork.BookRepository.Update(toupdate);
                    await _unitOfWork.CompleteAsync();
                    response.StatusCode = HttpStatusCode.NoContent;
                    response.IsSuccess = true;
                    response.Result = toupdate;

                    // return NoContent();
                    return Ok(response);
                }
                else
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    response.Errors.AddRange(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));

                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {

                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.Errors.Add(ex.Message);
                return StatusCode(500, response);
            }

        }



    }
}
