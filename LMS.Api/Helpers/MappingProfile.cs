using AutoMapper;
using LMS.Api.DTOs;
using LMS.Core.Contracts;
using LMS.Core.Entities;

namespace LMS.Api.Helpers
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterRequestDTO, RegisterRequest>().ReverseMap();
            CreateMap<LoginRequestDTO, LoginRequest>().ReverseMap();
            CreateMap<AuthResponseDTO, AuthResponse>().ReverseMap();


            CreateMap<BookCreateDTO, Book>()
                .ForMember(dest=>dest.Image, opt => opt.Ignore()).ReverseMap();


            CreateMap<BookUpdateDTO, Book>().ReverseMap();
            CreateMap<Book, BookWithGenreDTO>()
                .ForMember(dest => dest.genres, opt => opt.MapFrom(src => src.BookGenres.Select(b => b.Genre.Name).ToList()));
            
            CreateMap<Book, BookWithGenreAndBorrowsDTO>()
                .ForMember(dest => dest.genres, opt => opt.MapFrom(src => src.BookGenres.Select(b => b.Genre.Name).ToList()))
                .ForMember(dest => dest.Borrowers, opt => opt.MapFrom(src =>
        src.Borrows.Where(b => b.User != null && b.ReturnDate==null).Select(b => b.User.Name).ToList()));


            CreateMap<GenreCreateDTO, Genre>().ReverseMap();
            CreateMap<GenreUpdateDTO, Genre>().ReverseMap();


            CreateMap<BorrowedReturnedBookDTO, Borrow>().ReverseMap();

            
            CreateMap<BorrowBookDTO, Borrow>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore()) // Ignore UserId when mapping from CreateBorrowDTO to Borrow
                .ForMember(dest => dest.PenaltyAmount, opt => opt.Ignore()) // Ignore calculated property
                .ReverseMap();

            CreateMap<Borrow, BorrowAndPenaltyDTO>()
              .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
              .ForMember(dest => dest.BookName ,opt => opt.MapFrom(src => src.Book.Name))
              .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Book.Author));

            CreateMap<AssignGenreToBookDTO,BookGenre>().ReverseMap();
          /*  CreateMap<Book, BookWithGenreDTO>()
               .ForMember(dest => dest.genres, opt => opt.MapFrom(src => src.BookGenres.Select(b => b.Genre.Name).ToList()));
*/

        }
    }
}
