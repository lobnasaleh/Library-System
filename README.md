# **LMS API Documentation**

## **Design & Architecture**
- **Repository Pattern + Unit of Work** for data access
- **DTOs** for clean request/response models
- **AutoMapper** for object mapping
- **Transactional Operations** for critical workflows
- **Global Error Handling** with APIResponse model
- **Email Service Integration** for notifications

---

## **Endpoints**

### **BOOKS**

1. **GET** `/api/books`  
   **Description:** Returns a list of all books, including details like title, author, genre, and borrow status.

2. **GET** `/api/books/AvailableBooks`  
   **Description:** Returns books that are available for borrowing.

3. **GET** `/api/books/bygenre/{Genre}`  
   **Description:** Allows users to search books by genre.

4. **GET** `/api/books/byname/{Name}`  
   **Description:** Allows users to search books by their name.

5. **GET** `/api/books/{id}`  
   **Description:** Retrieves detailed information about a specific book by ID.

6. **POST** `/api/books`  
   **Description:** Allows librarians to add a new book with its genre and image. Validations ensure no duplicate entries.

7. **DELETE** `/api/books/{id}`  
   **Description:** Marks a book as deleted and unavailable for borrowing, provided it is not currently borrowed.

8. **PUT** `/api/books/{id}`  
   **Description:** Allows librarians to update the details of an existing book.

---

### **AUTH**

1. **POST** `/api/auth/login`  
   **Description:** Authenticates a user with the provided credentials (email and password). On successful authentication, a token is returned.

2. **POST** `/api/auth/register`  
   **Description:** Registers a new user by providing necessary details such as email, password, and role.

---

### **BOOKGENRE**

1. **POST** `/api/bookgenre`  
   **Description:** Assigns a genre to a book. Ensures that the genre and book exist, and that the book is not already assigned to the genre.
   **Authorization:** Librarian role required.

3. **DELETE** `/api/bookgenre/Book/{BookId}/Genre/{GenreId}`  
   **Description:** Disassociates a genre from a book. Checks if the genre is already assigned to the book before performing the action.
   **Authorization:** Librarian role required.

---

### **GENRE**

1. **GET** `/api/genre`  
   **Description:** Retrieves a paginated list of all genres that are not deleted.  
   **Authorization:** Required.

2. **GET** `/api/genre/{id}`  
   **Description:** Retrieves a specific genre by its ID.  
   **Authorization:** Required.

3. **POST** `/api/genre`  
   **Description:** Creates a new genre. Ensures that the genre name is unique before adding.  
   **Authorization:** Librarian role required.

4. **PUT** `/api/genre/{id}`  
   **Description:** Updates an existing genre by ID. Performed after validating the genre exists.  
   **Authorization:** Librarian role required.

5. **DELETE** `/api/genre/{id}`  
   **Description:** Marks a genre as deleted. If the genre is assigned to any book, it cannot be deleted.  
   **Authorization:** Librarian role required.

---

### **BORROW**

1. **GET** `/api/borrow/ActiveBorrows`  
   **Description:** Retrieves active borrow records (not returned).  
   **Authorization:** Librarian role required.  
   **Query:** `pageNumber`, `pageSize`.

2. **GET** `/api/borrow/{id:int}`  
   **Description:** Retrieves a borrow record by ID (with book & user details).  
   **Authorization:** Member and Librarian roles allowed.  
   **Route:** `id`.

3. **GET** `/api/borrow/User`  
   **Description:** Retrieves the borrow history for the authenticated user.  
   **Authorization:** Member role required.

4. **POST** `/api/borrow`  
   **Description:** Allows a member to borrow a book (checks availability and borrowing limits).  
   **Authorization:** Member role required.  
   **Body:** `BookId`, `StartDate`, `DueDate`.

5. **PUT** `/api/borrow/Return/{id:int}`  
   **Description:** Marks a book as returned by the librarian.  
   **Authorization:** Librarian role required.  
   **Route:** `id`.



