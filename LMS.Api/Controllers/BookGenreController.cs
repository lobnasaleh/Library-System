using AutoMapper;
using Azure;
using LMS.Api.DTOs;
using LMS.Core.Entities;
using LMS.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookGenreController : ControllerBase
    {


        private readonly IUnitOfWork unitOfWork;
        private readonly IAuthService authService;
        private readonly IMapper mapper;
        private APIResponse response;
        public BookGenreController(IUnitOfWork unitOfWork, IMapper mapper, IAuthService authService)
        {
            this.unitOfWork = unitOfWork;
            this.authService = authService;
            this.mapper = mapper;
            response = new APIResponse();
        }



        [HttpPost]
        [Authorize(Roles = "Librarian")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]

        public async Task<ActionResult<APIResponse>> AssignGenreToBook(AssignGenreToBookDTO bookandgenrefromreq)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    response.Errors.AddRange(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return BadRequest(response);
                }

                //check if this genre and this book are already there ( not marked deleted) 
                Book b = await unitOfWork.BookRepository.getAsync(b => !b.IsDeleted && b.Id == bookandgenrefromreq.BookId);
                if (b == null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.Errors.Add("No Book With This Id is found ");
                    return NotFound(response);
                }
                Genre gg = await unitOfWork.GenreRepository.getAsync(g => !g.IsDeleted && g.Id == bookandgenrefromreq.GenreId);
                if (gg == null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.Errors.Add("No Genre With This Id is found ");
                    return NotFound(response);
                }
                //check that this genre is not assigned to this book before
                BookGenre bg =  await unitOfWork.BookGenreRepository.getAsync(bg=>bg.GenreId==bookandgenrefromreq.GenreId &&  bg.BookId==bookandgenrefromreq.BookId
              ,false)   ;
                if (bg != null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    response.Errors.Add("Book is already assigned to This Genre");
                    return BadRequest(response);
                }

                //map bookandgenrefromreq to bookgenre to ADD
                BookGenre bookgenre= mapper.Map<BookGenre>(bookandgenrefromreq);
                //add to table
                await unitOfWork.BookGenreRepository.AddAsync(bookgenre);
                await unitOfWork.CompleteAsync();


               IEnumerable<Book> book2= await unitOfWork.BookRepository.getAllAsync(b => b.Id == bookandgenrefromreq.BookId ,  new[] { "BookGenres.Genre" });
                //map list of books to BookGenreDTO to avoid Cycles
                List<BookWithGenreDTO> res = mapper.Map<List<BookWithGenreDTO>>(book2);

                response.StatusCode = HttpStatusCode.Created;
                response.IsSuccess = true;
                response.Result = res;

                return StatusCode(StatusCodes.Status201Created, response);
            }
            catch (Exception ex) {

                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.Errors.Add(ex.Message);
                return StatusCode(500, response);

            }


            }
        [HttpDelete("Book/{BookId:int}/Genre/{GenreId:int}")]
        [Authorize(Roles = "Librarian")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]

        public async Task<ActionResult<APIResponse>> DessignGenreFromBook([FromRoute]int BookId, [FromRoute] int GenreId)
        {
            try
            {
                
                //check that this genre is assigned to this book before
                BookGenre bg = await unitOfWork.BookGenreRepository.getAsync(bg => bg.GenreId == GenreId && bg.BookId == BookId
              , false);
                if (bg == null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.Errors.Add("Book is not assigned to This Genre");
                    return NotFound(response);
                }

                 unitOfWork.BookGenreRepository.Delete(bg);
                await unitOfWork.CompleteAsync();
                IEnumerable<Book> book2 = await unitOfWork.BookRepository.getAllAsync(b => b.Id == BookId, new[] { "BookGenres.Genre" });
                //map list of books to BookGenreDTO to avoid Cycles
                List<BookWithGenreDTO> res = mapper.Map<List<BookWithGenreDTO>>(book2);
                response.Result = res;
                response.StatusCode = HttpStatusCode.NoContent; //not sure
                response.IsSuccess = true;
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
    } 
}
