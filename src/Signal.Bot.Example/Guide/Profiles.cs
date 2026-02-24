namespace Signal.Bot.Example.Guide;

public class Profiles
{
    private readonly SignalBotClient client = null!;

    public async Task UpdatingProfileName()
    {
        #region UpdatingProfileName
        var client = new SignalBotClient(builder => builder.WithNumber("1234567890"));

        await client.UpdateProfileAsync("My Helpfil Bot");
        #endregion UpdatingProfileName
    }

    public async Task UpdatingAbout()
    {
        #region About 
        await client.UpdateProfileAsync(about: "I'm a bot that helps with daily tasks! 🤖");
        #endregion About
    }

    public async Task UpdatingAvatar()
    {
        #region UpdatingAvatar
        var bytes = await File.ReadAllBytesAsync("/path/to/avatar.jpg");
        await client.UpdateProfileAsync(avatar: bytes);
        #endregion UpdatingAvatar
    }

    public async Task UpdateEverythingAtOnce()
    {
        #region UpdateEverything
        var avatar = await File.ReadAllBytesAsync("/path/to/avatar.jpg");
        await client.UpdateProfileAsync(
            name: "Support Bot",
            about: "24/7 automated support",
            avatar: avatar);
        #endregion UpdateEverything
    }
}