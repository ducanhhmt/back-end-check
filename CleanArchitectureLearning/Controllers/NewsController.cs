using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.News;
using CleanArchitecture.Application.Services_Interface;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureLearning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class NewsController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IMessageBus _messageBus;
        public NewsController(ISender sender, IMessageBus messageBus)
        {
            _sender = sender;
            _messageBus = messageBus;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<NewsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<List<NewsDTO>> GetAll()
        {
            var result = await _sender.Send(new GetAllNewsQueries());
            return result;
        }

        [HttpGet("Search")]
        [ProducesResponseType(typeof(List<NewsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchAsync(string keyword, string requestId)
        {
            await _messageBus.PublishAsync(new SearchNewsMessage
            {
                RequestId = requestId,
                Keyword = keyword
            }, "news_searching_queue");

            return Ok(new { requestId });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(NewsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<NewsDTO> GetById(string id)
        {
            var product = await _sender.Send(new GetbyIdNewsQueries { Id = id });
            return product;
        }

        [HttpPost]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<bool> Create([FromBody] AddNewsCommand command)
        {
            var result = await _sender.Send(command);
            return result;
        }

        [HttpPut]
        [ProducesResponseType(typeof(NewsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<bool> Update([FromBody] UpdateNewsCommand command)
        {
            var result = await _sender.Send(command);
            return result;
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _sender.Send(new DeleteNewsCommand { Id = id });
            return success ? Ok(success) : NotFound();
        }
    }
}
