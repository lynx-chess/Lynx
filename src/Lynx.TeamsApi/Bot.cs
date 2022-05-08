using System.Collections.Concurrent;
using System.Threading.Channels;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Lynx.TeamsApi;

public class Bot : ActivityHandler
{
    private readonly ChannelWriter<string> _writer;

    // Message to send to users when the bot receives a Conversation Update event
    private const string WelcomeMessage = "Welcome to the Lynx bot. Type any UCI commands to play a game against Lynx chess engine";

    // Dependency injected dictionary for storing ConversationReference objects used in NotifyController to proactively message users
    private readonly ConcurrentDictionary<string, ConversationReference> _conversationReferences;

    public Bot(ChannelWriter<string> writer, ConcurrentDictionary<string, ConversationReference> conversationReferences)
    {
        _writer = writer;
        _conversationReferences = conversationReferences;
    }

    private void AddConversationReference(Activity activity)
    {
        var conversationReference = activity.GetConversationReference();
        _conversationReferences.AddOrUpdate(conversationReference.User.Id, conversationReference, (key, newValue) => conversationReference);
    }

    protected override Task OnConversationUpdateActivityAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
    {
        if (turnContext.Activity is Activity myActivity)
        {
            AddConversationReference(myActivity);

            return base.OnConversationUpdateActivityAsync(turnContext, cancellationToken);
        }

        return Task.CompletedTask;
    }

    protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
    {
        foreach (var member in membersAdded)
        {
            // Greet anyone that was not the target (recipient) of this message.
            if (member.Id != turnContext.Activity.Recipient.Id)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(WelcomeMessage), cancellationToken);
            }
        }
    }

    protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    {
        if (turnContext.Activity is Activity myActivity)
        {
            await _writer.WriteAsync(turnContext.Activity.Text);
            AddConversationReference(myActivity);
            // Echo back what the user said
            //await turnContext.SendActivityAsync(MessageFactory.Text($"You sent '{turnContext.Activity.Text}'"), cancellationToken);
        }
    }
}
