using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using OpenAI;
using Sample.SemanticKernel.WebApi.Models;

namespace Sample.SemanticKernel.WebApi.Controllers;

[ApiController]
[Route("/chat")]
[Experimental("SKEXP0110")]
public class ChatController : ControllerBase
{
    private readonly ILogger<ChatController> _logger;
    private readonly Kernel _genericKernel;

    public ChatController(
        ILogger<ChatController> logger,
        IConfiguration configuration,
        OpenAIClient openAiClient)
    {
        _logger = logger;

        // Setup chat
        var kernelBuilder = Kernel.CreateBuilder()
            .AddOpenAIChatCompletion(
                configuration.GetValue<string>("OpenAI:ModelId")!,
                openAiClient);
        kernelBuilder.Services
            .AddSingleton(configuration)
            .AddSingleton(openAiClient);

        _genericKernel = kernelBuilder.Build();
    }

    [HttpPost("", Name = "CallChat")]
    public async Task<CallChatResponse> CallChat([FromBody] CallChatRequest request)
    {
        var response = await _genericKernel.InvokePromptAsync(request.Message);
        return new CallChatResponse(response.GetValue<string>() ?? "An error happened.");
    }
}