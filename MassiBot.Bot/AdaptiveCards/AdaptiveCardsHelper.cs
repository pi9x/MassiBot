using AdaptiveCards;
using Microsoft.Bot.Schema;

namespace MassiBot.Bot.AdaptiveCards;

public static class AdaptiveCardsHelper
{
    /// <summary>
    /// Converts AdaptiveCard to Attachment. This is a convenience method for creating attachments from AdaptiveCard.
    /// </summary>
    /// <param name="card">card to convert to Attachment. This can be an adapter or a card.</param>
    /// <returns>Attachment created from AdaptiveCard or null if conversion failed for any reason. Note that it is possible to have more than one attachment</returns>
    public static Attachment ToAttachment(this AdaptiveCard card)
    {
        return new Attachment
        {
            ContentType = "application/vnd.microsoft.card.adaptive",
            Content = card
        };
    }
}