using Microsoft.EntityFrameworkCore;
using System.Net;

namespace WebApi
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<StudentService> _logger;
        private readonly IRedisCacheService _cacheService;
        public StudentService(
            ApplicationDbContext dbContext,
            ILogger<StudentService> logger,
            IRedisCacheService cacheService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _cacheService = cacheService;
        }
        public async Task<Response<AddStudentDto>> AddStudentAsync(AddStudentDto model, CancellationToken cancellationToken)
        {
            var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.PhoneNumber == model.PhoneNumber, cancellationToken);
            if (student != null) return new Response<AddStudentDto>(HttpStatusCode.BadRequest, "A student with this number already exists");

            var newStudent = new Student
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Age = model.Age,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            await _dbContext.Students.AddAsync(newStudent, cancellationToken);

            var students = await _cacheService.GetAsync<List<GetStudentDto>>(DefaultStudentCacheKey.Students);
            
            students.Add(new GetStudentDto
            {
                Id = newStudent.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Age = model.Age,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            });
            var expirityTime = DateTimeOffset.Now.AddMinutes(2);
            await _cacheService.AddAsync(DefaultStudentCacheKey.Students, students, expirityTime);

            var result = await _dbContext.SaveChangesAsync(cancellationToken);

            return result > 0
                ? new Response<AddStudentDto>(HttpStatusCode.OK, "Student data successfully added !")
                : new Response<AddStudentDto>(HttpStatusCode.OK, "Student data not added !");
        }

        public async Task<Response<string>> DeleteStudentAsync(int studentId, CancellationToken cancellationToken)
        {
            var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == studentId, cancellationToken);
            if (student == null) return new Response<string>(HttpStatusCode.NotFound, "Student not found !");

            _dbContext.Students.Remove(student);
            await _cacheService.RemoveByKeyAsync($"student{studentId}");

            var result = await _dbContext.SaveChangesAsync(cancellationToken);

            return result > 0
                ? new Response<string>(HttpStatusCode.OK, "Student data successfully deleted !")
                : new Response<string>(HttpStatusCode.OK, "Student data not deleted !");
        }

        public async Task<List<GetStudentDto>> GetAllStudentsAsync(StudentFilter filter, CancellationToken cancellationToken)
        {
            var studentsDataInCache = await _cacheService.GetAsync<List<GetStudentDto>>(DefaultStudentCacheKey.Students);

            if (studentsDataInCache!=null)
            {
                _logger.LogInformation("Data retrieved from cache");
                return studentsDataInCache;
            }
            var query = _dbContext.Students.OrderBy(x => x.Id).AsQueryable();

            if (filter.FirstNameOrLastName != null)
            {
                query = query.Where(x => x.FirstName.ToLower().Contains(filter.FirstNameOrLastName.ToLower()) ||
                                       x.LastName.ToLower().Contains(filter.FirstNameOrLastName.ToLower()));
            }
            if (filter.PhoneNumber != null)
            {
                query = query.Where(x => x.PhoneNumber == filter.PhoneNumber);
            }

            var allTotalRecord = await query.CountAsync(cancellationToken);

            var students = await query.Select(student => new GetStudentDto
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Age = student.Age,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber
            }).ToListAsync(cancellationToken);

            var expirityTime = DateTimeOffset.Now.AddMinutes(2);
            await _cacheService.AddAsync(DefaultStudentCacheKey.Students, students, expirityTime);

            _logger.LogInformation("Data retrieved from Database");
            return students;
        }

        public async Task<Response<GetStudentDto>> GetStudentByIdAsync(int studentId, CancellationToken cancellationToken)
        {
            var studentDataInCache = await _cacheService.GetAsync<GetStudentDto>(studentId.ToString(), cancellationToken);
            if(studentDataInCache==null)
            {
                var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == studentId, cancellationToken);
                if (student == null) return new Response<GetStudentDto>(HttpStatusCode.NotFound, "Student not found !");

                var model = new GetStudentDto
                {
                    Id = student.Id,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    Age = student.Age,
                    Email = student.Email,
                    PhoneNumber = student.PhoneNumber
                };

                var expirityTime = DateTimeOffset.Now.AddSeconds(30);
                await _cacheService.AddAsync(DefaultStudentCacheKey.Students, model, expirityTime);

                return new Response<GetStudentDto>(HttpStatusCode.OK, model);
            }

            return new Response<GetStudentDto>(HttpStatusCode.OK, studentDataInCache);
        }

        public async Task<Response<UpdateStudentDto>> UpdateStudentAsync(UpdateStudentDto model, CancellationToken cancellationToken)
        {
            var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == model.Id, cancellationToken);
            if (student == null) return new Response<UpdateStudentDto>(HttpStatusCode.NotFound, "Student not found !");

            student.FirstName = model.FirstName;
            student.LastName = model.LastName;
            student.Age = model.Age;
            student.Email = model.Email;
            student.PhoneNumber = model.PhoneNumber;

            var result = await _dbContext.SaveChangesAsync(cancellationToken);

            return result > 0
                ? new Response<UpdateStudentDto>(HttpStatusCode.OK, "Student data successfully updated !")
                : new Response<UpdateStudentDto>(HttpStatusCode.OK, "Student data not updated !");
        }
    }
}
