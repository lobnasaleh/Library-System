using AutoMapper;
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
    public class GenreController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        protected APIResponse response;

        public GenreController(IMapper _mapper, IUnitOfWork _unitOfWork)
        {
          this._mapper = _mapper;
          this._unitOfWork = _unitOfWork;
          response= new APIResponse();
            
        }
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        //en al admin yezahrlo kol el kotob b anwa3ha bely 3amlenlha borrow
        public async Task<ActionResult<APIResponse>> GetAllGenres(int pageNumber,int pagesize)
        {
            try
            {


                var res = await _unitOfWork.GenreRepository.getAllAsync(g=>!g.IsDeleted,pagenumber:pageNumber,pagesize:pagesize);
                if (!res.Any())//mesh !=null l2enha law mal2tsh hatraga3e mpty list msh null
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.Errors.Add("No Genres Available");
                    return NotFound(response);
                }
              
                response.StatusCode = HttpStatusCode.OK;
                response.IsSuccess = true;
                response.Result = res;
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]

        public async Task<ActionResult<APIResponse>> GetGenreById(int id)//el user masaln 3ayz yshoof el ketab ely ekhtaro full screen
        {
            try
            {
                if (id <= 0)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    response.Errors.Add("Invalid Genre ID.");
                    return BadRequest(response);
                }

                var genre = await _unitOfWork.GenreRepository.getAsync(g=> !g.IsDeleted && g.Id == id);
                if (genre == null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.Errors.Add("The requested Genre with the given ID was not found");
                    return NotFound(response);
                }
              
                response.StatusCode = HttpStatusCode.OK;
                response.IsSuccess = true;
                response.Result = genre;
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> AddGenre([FromBody] GenreCreateDTO genrefromreq)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Genre found = await _unitOfWork.GenreRepository.getAsync(g => !g.IsDeleted  && g.Name == genrefromreq.Name , tracking: false);
                    if (found != null)
                    {
                        response.StatusCode = HttpStatusCode.Conflict;
                        response.IsSuccess = false;
                        response.Errors.Add("Duplicate Genre Name ");

                        return Conflict(response);
                    }


                    //mapping genrecreateDto to genre
                    Genre g = _mapper.Map<Genre>(genrefromreq);
                    await _unitOfWork.GenreRepository.AddAsync(g);

                    response.StatusCode = HttpStatusCode.Created;
                    response.IsSuccess = true;
                    response.Result = g;

                    await _unitOfWork.CompleteAsync();
                    // EF 3amla tracking lel object bmogarad ma3mlt save el id got assigned lel tracked object
                    return CreatedAtAction(nameof(GetGenreById), new { id = g.Id }, response);
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

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Librarian")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> DeleteGenre(int id)
        {
            try
            {
                var genre = await _unitOfWork.GenreRepository.getAsync(b =>! b.IsDeleted && b.Id == id , true, new[] { "BookGenres" });

                if (genre == null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.Errors.Add("Genre Not Found !");
                    return NotFound(response);
                }
                //checking if Genre is assigned to a book before deleting it
                if (genre.BookGenres.Any(bg => bg.GenreId == id))
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    response.Errors.Add("Genre is currently assigned to a Book and cannot be deleted.");
                    return Conflict(response);
                }

                //mark deleted -->soft delete

                genre.IsDeleted = true;
                _unitOfWork.GenreRepository.Update(genre);
                await _unitOfWork.CompleteAsync();

                response.StatusCode = HttpStatusCode.NoContent;
                response.IsSuccess = true;
                response.Result = genre;

                //  _unitOfWork.GenreRepository.Delete(genre);

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
        public async Task<ActionResult<APIResponse>> UpdateGenre(int id, [FromBody] GenreUpdateDTO GenreFromReq)
        {
            try
            {
                var genre = await _unitOfWork.GenreRepository.getAsync(g => !g.IsDeleted && g.Id == id, tracking: false);
                if (genre == null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.Errors.Add("Genre Not Found !");
                    return NotFound(response);
                }
                if (ModelState.IsValid)
                {
                    //map genreFromReq to genre
                    var toupdate = _mapper.Map<Genre>(GenreFromReq);
                    toupdate.Id = genre.Id; //ghaleban law shelt da msh hayhsal haga l2en Ef is tracking the object 
                    _unitOfWork.GenreRepository.Update(toupdate);
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
