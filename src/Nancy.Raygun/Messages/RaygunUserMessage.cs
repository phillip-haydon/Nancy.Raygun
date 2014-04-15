namespace Nancy.Raygun.Messages
{
    public class RaygunUserMessage
    {
        public RaygunUserMessage(string identifier)
        {
            Identifier = identifier;
        }

        public string Identifier { get; private set; }
    }
}
