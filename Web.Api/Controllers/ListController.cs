using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Dto.Request;
using Web.Api.Dto.Response;
using Web.Api.Persistence.Repositories;

namespace Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ListController
    {
        [HttpPost(Name = "CreateList")]
        public ListDto CreateList(ListCreateDto createListDto)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{listId}", Name = "GetListById")]
        public ListDto GetListById(int ListId)
        {
            throw new NotImplementedException();
        }

        [HttpGet( Name = "GetAllList")]
        public List<ListDto> GetAllList()
        {
            throw new NotImplementedException();
        }

        [HttpPost("{listId}/move-task", Name = "MoveTaskToList")]
        public void MoveTaskToList(Task task)
        {
            throw new NotImplementedException();
        }
        private readonly ListRepo _listRepository;
      

    }

}
