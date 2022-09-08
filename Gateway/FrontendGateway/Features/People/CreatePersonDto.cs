namespace SIS.Application.Features.People;

public class CreatePersonDto
{
    public NodaTime.LocalDate BirthDate { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
}
