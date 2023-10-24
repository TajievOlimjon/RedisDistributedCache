namespace WebApi
{
    public interface IStudentService
    {
        Task<List<GetStudentDto>> GetAllStudentsAsync(StudentFilter filter, CancellationToken cancellationToken);
        Task<Response<GetStudentDto>> GetStudentByIdAsync(int studentId, CancellationToken cancellationToken);
        Task<Response<string>> DeleteStudentAsync(int studentId, CancellationToken cancellationToken);
        Task<Response<UpdateStudentDto>> UpdateStudentAsync(UpdateStudentDto model, CancellationToken cancellationToken);
        Task<Response<AddStudentDto>> AddStudentAsync(AddStudentDto model, CancellationToken cancellationToken);
    }
}

