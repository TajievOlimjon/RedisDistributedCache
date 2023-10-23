using Microsoft.EntityFrameworkCore;
using System.Net;

namespace WebApi
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<StudentService> _logger;
        private readonly ICacheService _cacheService;
        public StudentService(
            ApplicationDbContext dbContext,
            ILogger<StudentService> logger,
            ICacheService cacheService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _cacheService = cacheService;
        }
        public async Task<Response<AddStudentDto>> AddStudentAsync(AddStudentDto model)
        {
            var student = await _dbContext.Students.FirstOrDefaultAsync(s=>s.PhoneNumber==model.PhoneNumber);
            if (student != null) return new Response<AddStudentDto>(HttpStatusCode.BadRequest, "A student with this number already exists");

            var newStudent = new Student
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Age = model.Age,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            await _dbContext.Students.AddAsync(newStudent);

            var expirityTime = DateTimeOffset.Now.AddSeconds(30);
            _cacheService.AddData($"student{newStudent.Id}", newStudent, expirityTime);

            var result = await _dbContext.SaveChangesAsync();

            return result > 0 
                ? new Response<AddStudentDto>(HttpStatusCode.OK, "Student data successfully added !")
                : new Response<AddStudentDto>(HttpStatusCode.OK, "Student data not added !");
        }

        public async Task<Response<string>> DeleteStudentAsync(int studentId)
        {
            var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == studentId);
            if (student == null) return new Response<string>(HttpStatusCode.NotFound, "Student not found !");

            _dbContext.Students.Remove(student);
            _cacheService.DeleteDataByKey($"student{studentId}");
            var result = await _dbContext.SaveChangesAsync();

            return result > 0
                ? new Response<string>(HttpStatusCode.OK, "Student data successfully deleted !")
                : new Response<string>(HttpStatusCode.OK, "Student data not deleted !");
        }

        public async Task<List<GetStudentDto>> GetAllStudentsAsync(StudentFilter filter)
        {
            var cacheData = _cacheService.GetData<List<GetStudentDto>>(DefaultStudentCacheKey.Students);

            if (cacheData != null && cacheData.Count > 0)
            {
                return cacheData;
            }

            var query = _dbContext.Students.OrderBy(x=>x.Id).AsQueryable();

            if (filter.FirstNameOrLastName != null)
            {
                query = query.Where(x=>x.FirstName.ToLower().Contains(filter.FirstNameOrLastName.ToLower()) ||
                                       x.LastName.ToLower().Contains(filter.FirstNameOrLastName.ToLower()));
            }
            if (filter.PhoneNumber != null)
            {
                query = query.Where(x => x.PhoneNumber == filter.PhoneNumber);
            }

            var allTotalRecord = await query.CountAsync();

            var students = await query.Select(student => new GetStudentDto
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Age = student.Age,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber
            }).ToListAsync();

            var expirityTime = DateTimeOffset.Now.AddSeconds(30);
            _cacheService.AddData(DefaultStudentCacheKey.Students,students, expirityTime);

            return students;
        }

        public async Task<Response<GetStudentDto>> GetStudentByIdAsync(int studentId)
        {
            var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == studentId);
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

            return new Response<GetStudentDto>(HttpStatusCode.OK,model);
        }

        public async Task<Response<UpdateStudentDto>> UpdateStudentAsync(UpdateStudentDto model)
        {
            var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == model.Id);
            if (student == null) return new Response<UpdateStudentDto>(HttpStatusCode.NotFound, "Student not found !");

            student.FirstName = model.FirstName;
            student.LastName = model.LastName;
            student.Age = model.Age;
            student.Email = model.Email;
            student.PhoneNumber = model.PhoneNumber;

            var result = await _dbContext.SaveChangesAsync();

            return result > 0
                ? new Response<UpdateStudentDto>(HttpStatusCode.OK, "Student data successfully updated !")
                : new Response<UpdateStudentDto>(HttpStatusCode.OK, "Student data not updated !");

        }
    }
}
