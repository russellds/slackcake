#addin "Cake.Slack"
var slackWebHookUrl = EnvironmentVariable("slackWebHookUrl");
var slackChannel = "#cake";
var target = Argument("target", "Default");

public void SendSlackMessage(string MessageText, string AttachmentPretext, string AttachmentText, string AttachmentTitle, string AttachmentValue )
{
    var slackAssemblyFieldAttachment = new SlackChatMessageAttachmentField[]
    {
                new SlackChatMessageAttachmentField
                {
                    Title =  AttachmentTitle,
                    Value =  AttachmentValue
                }
    };
    var postMessageResult = Slack.Chat.PostMessage(
        channel:slackChannel,
        text:MessageText,
        messageAttachments:new SlackChatMessageAttachment[]
        {
                    new SlackChatMessageAttachment
                    {
                                Text = AttachmentText,
                                Pretext = AttachmentPretext,
                                Color = "#67A0E1",
                                Fields = slackAssemblyFieldAttachment
                    }
        },
        messageSettings:new SlackChatMessageSettings { IncomingWebHookUrl = slackWebHookUrl });

    if (postMessageResult.Ok)
    {
        Information("Message successfully sent");
    }
    else
    {
        Error("Failed to send message: {0}", postMessageResult.Error);
    }
}

// Task Setup
TaskSetup((TaskSetupContext) =>
{
  string taskName = "Task: " + TaskSetupContext.Task.Name;
  SendSlackMessage("SlackCake", null, null, taskName, "Starting...");
});

// Task Teardown
TaskTeardown((TaskTeardownContext) =>
{
  string taskName = "Task: " + TaskTeardownContext.Task.Name;
  string completed = "Completed: " + TaskTeardownContext.Duration.ToString("g");
  SendSlackMessage("SlackCake", null, null, taskName, completed);
});

Task("Default")
  .Does(() =>
{
  Information("Hello World!");
});

RunTarget(target);
