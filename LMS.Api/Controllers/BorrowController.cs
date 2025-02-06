using AutoMapper;
using LMS.Api.DTOs;
using LMS.Core.Entities;
using LMS.Core.Interfaces;
using LMS.Infrastructure.Repositories;
using LMS.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Net;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IAuthService authService;
        readonly IEmailSender emailSender;
        private readonly IMapper mapper;
        private APIResponse response;
        public BorrowController(IUnitOfWork unitOfWork, IMapper mapper, IAuthService authService, IEmailSender emailSender)
        {
            this.unitOfWork = unitOfWork;
            this.authService = authService;
            this.mapper = mapper;
            this.emailSender = emailSender;
            response = new APIResponse();
        }
        //borrows active -->Librarian

        [HttpGet("ActiveBorrows")]
        [Authorize(Roles = "Librarian")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async Task<ActionResult<APIResponse>> getActiveBorrowsAndPenalties(int pageNumber,int pageSize)
        {
            try
            {
                //book is not returned so it is still borrowed
                var activeBorrows = await unitOfWork.BorrowRepository.getAllAsync(bo => bo.ReturnDate == null, new[] { "User", "Book" },pagenumber:pageNumber,pagesize:pageSize);
                if (!activeBorrows.Any())
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.Errors.Add("No Borrows Found");

                    return NotFound(response);
                }

                //mapping borrow to borrowandpenaltydto
                var res = mapper.Map<List<BorrowAndPenaltyDTO>>(activeBorrows);

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
        //Borrow with Id -->Both

        [HttpGet("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]//ata2ked keda
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async Task<ActionResult<APIResponse>> getBorrowAndPenaltyByBorrowId(int id) //borrowid
        {
            try
            {
                if (id <= 0)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    response.Errors.Add("Invalid Borrow Id");
                    return BadRequest(response);
                }
                //returned and not returned -->calculating penalty if found

                var Borrow = await unitOfWork.BorrowRepository.getAsync(bo => bo.Id == id, true, new[] { "User", "Book" });
                if (Borrow is null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.Errors.Add("No Borrow with This Id is Found");
                    return NotFound(response);
                }
                //mapping borrow to borrowandpenaltydto
                var res = mapper.Map<BorrowAndPenaltyDTO>(Borrow);

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
        //borrows given UserId not BorrowId -->Both ( showing Borrows and penalties calculating penalties for returned books 
        [HttpGet("User")]
        [Authorize(Roles = "Member")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]//ata2ked keda
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //user checking his current and previous Borrow history

        public async Task<ActionResult<APIResponse>> getBorrowsAndPenaltiesByUserId()
        {//momkn tkoon borrows 2adeema aw geededa el 2 law 3yza momkn a3melhawa tb2a esmaha active borrows of user
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await authService.getUserById(userId);
                if (user is null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.Errors.Add("No User with This Id is Found");
                    return NotFound(response);
                }

                var borrows = await unitOfWork.BorrowRepository.getAllAsync(b => b.UserId == userId, new[] { "User", "Book" });
                if (!borrows.Any())
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.Errors.Add("User did not borrow any Book");
                    return NotFound(response);
                }
                //map borrows to a list of BorrowAndPenaltyDTO
                var result = mapper.Map<List<BorrowAndPenaltyDTO>>(borrows);

                response.StatusCode = HttpStatusCode.OK;
                response.IsSuccess = true;
                response.Result = result;
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

        //(Add Borrow)Borrow Book -->Member
        [HttpPost]
        [Authorize(Roles = "Member")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> BorrowBook(BorrowBookDTO borrowbookreq)
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

                var currentUtcTime = DateTime.UtcNow;
                var dateErrors = new List<string>();

                // Ensure StartDate is not in the past
                if (borrowbookreq.StartDate < currentUtcTime)
                    dateErrors.Add("StartDate cannot be in the past And you Can not borrow this book at the same day");

                // Ensure DueDate is not in the past
                if (borrowbookreq.DueDate <= currentUtcTime)
                    dateErrors.Add("DueDate cannot be in the past");

                //already added that startdate must be graeter than duedate in custom validation

                if (dateErrors.Any())
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    response.Errors.AddRange(dateErrors);
                    return BadRequest(response);
                }

                // check if book is available ,not deleted,and having this id

                var book = await unitOfWork.BookRepository.getAsync(b => !b.IsDeleted && b.IsAvailable && b.Id == borrowbookreq.BookId, tracking: false);
                if (book is null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    response.Errors.Add("Your requested book is Unavailable");
                    return BadRequest(response);
                }
                //get userId
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    response.StatusCode = HttpStatusCode.Unauthorized;
                    response.IsSuccess = false;
                    response.Errors.Add("User is not Authenticated");
                    return Unauthorized(response);
                }
                //same user can not borrow more than 3 books (at the same time) so i am checking this
                int noOfBorrowsByUser = await unitOfWork.BorrowRepository.CountAsync(bo => bo.UserId == userId && bo.ReturnDate == null);
                if (noOfBorrowsByUser >= 3)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    response.Errors.Add("Sorry,You can not borrow more than 3 Books simultaneously");
                    return BadRequest(response);
                }

                //map borrowbookdto to borrow
                Borrow bb = mapper.Map<Borrow>(borrowbookreq);
                bb.UserId = userId;

                //save Borrow
                await unitOfWork.BorrowRepository.AddAsync(bb);
                await unitOfWork.CompleteAsync();

                //make this book unavaialable
                var booktoupdate = await unitOfWork.BookRepository.getAsync(b => b.Id == borrowbookreq.BookId);
                booktoupdate.IsAvailable = false;
                await unitOfWork.CompleteAsync();

                //map borrow to borrowreturnedbookdto to avoid cycles 
                BorrowedReturnedBookDTO bbtoreturn = mapper.Map<BorrowedReturnedBookDTO>(bb);
                bbtoreturn.BookName=book.Name;

                response.StatusCode = HttpStatusCode.Created;
                response.IsSuccess = true;
                response.Result = bbtoreturn;


                string userEmail = User.FindFirstValue(ClaimTypes.Email);

                await emailSender.SendEmailAsync(userEmail,"Book Borrowed Successfully",$"The book {book.Name}is borrowed starting from {borrowbookreq.StartDate} And the Due Date is {borrowbookreq.DueDate} ");


                return CreatedAtAction("getBorrowAndPenaltyByBorrowId", new { id = bb.Id }, response);
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.Errors.Add(ex.Message);
                return StatusCode(500, response);
            }
        }

        [HttpPut("Return/{id:int}")]
        [Authorize(Roles = "Librarian")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> ReturnBook(int id)//borrowid
        {
            try
            {

                //set status of given book to available
                //set return date to today's date

                var borrowed = await unitOfWork.BorrowRepository.getAsync(bo => bo.Id == id, true, new[] { "Book", "User" });

                if (borrowed == null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    response.Errors.Add("Borrow record not found.");
                    return BadRequest(response);
                }
                if (borrowed.ReturnDate != null)
                {

                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    response.Errors.Add("Book is already returned");
                    return BadRequest(response);

                }

                borrowed.Book.IsAvailable = true;
                borrowed.ReturnDate = DateTime.UtcNow;
                //  await unitOfWork.BorrowRepository.Update();
                await unitOfWork.CompleteAsync();

                //map 
                BorrowAndPenaltyDTO res = mapper.Map<BorrowAndPenaltyDTO>(borrowed);


                response.StatusCode = HttpStatusCode.NoContent;
                response.IsSuccess = true;
                response.Result = res;

                // return NoContent();
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


        //Delete Borrow -->will not delete it  or will only allow Librian if mistaken

       /* [HttpDelete("{id:int}")]
        [Authorize(Roles = "Librarian")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> DeleteBorrow(int id)
        {
            try
            {
                var borrow = await unitOfWork.BorrowRepository.getAsync(b => b.Id == id, true);

                if (borrow == null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.Errors.Add("No Borrowed Book with This Id is Not Found !");
                    return NotFound(response);
                }

                //mark deleted -->soft delete
                borrow.IsDeleted = true;
                await  _unitOfWork.BorrowRepository.Update(borrow);
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

*/
    }
}
