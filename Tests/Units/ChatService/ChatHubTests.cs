using System.Collections.ObjectModel;
using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Defaults;
using HealthHub.Source.Hubs;
using HealthHub.Source.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

public class ChatHubTests
{
  private readonly DbContextOptions<ApplicationContext> _options;
  private readonly ApplicationContext _appContext;
  private readonly Mock<IHubContext<ChatHub>> _mockHubContext;
  private readonly Mock<FileService> _mockFileService;
  private readonly Mock<ILogger<ChatService>> _mockLoggerChatService;
  private readonly Mock<ILogger<FileService>> _mockLoggerFileService;
  private readonly Mock<IHubCallerClients> _mockClients;
  private readonly Mock<IClientProxy> _mockClientProxy;
  private readonly ChatHub _chatHub;
  private readonly Mock<ChatService> _mockChatService;
  private readonly Mock<HubCallerContext> _mockCallerContext;
  private readonly Mock<HttpContext> _mockHttpContext;

  public ChatHubTests()
  {
    // Setup mocks
    _mockHubContext = new Mock<IHubContext<ChatHub>>();
    _mockClients = new Mock<IHubCallerClients>();
    _mockClientProxy = new Mock<IClientProxy>();

    _mockLoggerChatService = new Mock<ILogger<ChatService>>();
    _mockLoggerFileService = new Mock<ILogger<FileService>>();

    _options = new DbContextOptionsBuilder<ApplicationContext>()
      .UseInMemoryDatabase(databaseName: "TestDatabase")
      .Options;
    _appContext = new ApplicationContext(_options);

    _mockFileService = new Mock<FileService>(_appContext, _mockLoggerFileService.Object);
    _mockChatService = new Mock<ChatService>(
      _appContext,
      _mockFileService.Object,
      _mockLoggerChatService.Object
    );

    _mockCallerContext = new Mock<HubCallerContext>();
    _mockHttpContext = new Mock<HttpContext>();

    _mockCallerContext.Setup(context => context.GetHttpContext()).Returns(_mockHttpContext.Object);
    _mockHttpContext
      .Setup(context => context.Request.Cookies[AuthDefaults.User.UserId])
      .Returns("DB1CA3D9-8F05-444E-9CF8-E8E3F20DD38E");

    // Assign HttpContext to the HubCallerContext (mocking access to HttpContext from HubCallerContext)
    _mockCallerContext.Setup(context => context.ConnectionId).Returns("TestConnectionId");
    _mockCallerContext.Setup(context => context.UserIdentifier).Returns("TestUserId");

    // Setup the mock clients
    _mockClients
      .Setup(clients => clients.User(It.IsAny<string>()))
      .Returns(_mockClientProxy.Object);

    // Create the ChatHub instance with the mocked services
    _chatHub = new ChatHub(_mockChatService.Object)
    {
      Clients = _mockClients.Object,
      Context = _mockCallerContext.Object
    };
  }

  [Fact]
  public async Task SendMessage_ShouldSendMessageToClients()
  {
    // Arrange
    Guid receiverId = Guid.Parse("DB1CA3D9-8F05-444E-9CF8-E8E3F20DD38E");
    string messageText = "Hello, how are you doing";
    List<CreateFileDto> files = new List<CreateFileDto>();

    // Act
    await _chatHub.SendMessage(receiverId, messageText, files);

    // Assert
    _mockClientProxy.Verify(
      client =>
        client.SendCoreAsync(
          ChatEvents.ReceiveMessage.ToString(),
          It.Is<object[]>(o => o[0] is MessageDto),
          default
        ),
      Times.Once
    );
  }
}
