namespace Nancy.Raygun.Tests
{
    using Xunit;

    public class RaygunMessageBuilderTests
    {
        [Fact]
        public void Sends_user_identity_when_set()
        {
            const string expectedUserIdentity = "a.user@example.com";
            var message = RaygunMessageBuilder.New
                                              .SetUser(expectedUserIdentity)
                                              .Build();

            Assert.Equal(expectedUserIdentity, message.Details.User.Identifier);
        }

        [Fact]
        public void Does_not_send_user_identity_when_not_set()
        {
            var message = RaygunMessageBuilder.New
                                              .Build();

            Assert.Null(message.Details.User);
        }

        [Fact]
        public void Does_not_send_user_identity_when_set_to_null()
        {
            var message = RaygunMessageBuilder.New
                                              .SetUser(null)
                                              .Build();

            Assert.Null(message.Details.User);
        }

        [Fact]
        public void Does_not_send_user_identity_when_set_to_empty_string()
        {
            var message = RaygunMessageBuilder.New
                                              .SetUser("")
                                              .Build();

            Assert.Null(message.Details.User);
        }
    }
}
