namespace cw10.DTOs;

public class PagesDto<T>
{
    public int PageNum { get; set; }
    public int PageSize { get; set; }
    public int AllPages { get; set; }
    public IEnumerable<T> Trips { get; set; }
}