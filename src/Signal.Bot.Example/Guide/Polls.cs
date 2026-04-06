namespace Signal.Bot.Example.Guide;

public class Polls
{
    private readonly SignalBotClient client = null!;

    public async Task CreatingPoll()
    {
        #region CreatingPoll
        await client.AddPollAsync(true,
            new[] { "Option 1", "Option 2", "Option 3" },
            "Question",
            "+1111111111");
        #endregion
    }
    
    public async Task ClosingPoll()
    {
        #region ClosingPoll
        await client.ClosePollAsync( new DateTime(2026,04,06,12,00,00),
            "+1111111111");
        #endregion
    }
    
    public async Task VotingInPoll()
    {
        #region VotingInPoll

        await client.VotePollAsync("+2222222222",
            new DateTime(2026, 04, 06, 12, 00, 00),
            "+1111111111",
            [0, 1]);
        #endregion
    }
}