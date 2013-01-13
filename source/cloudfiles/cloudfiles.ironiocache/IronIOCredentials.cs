using System.IO;

namespace cloudfiles.ironiocache
{
    public class IronIOCredentials
    {
        public static IronIOCredentials LoadFrom(string filename)
        {
            var lines = File.ReadAllLines(filename);
            return new IronIOCredentials(lines[0], lines[1]);
        }


        public string Token { get; private set; }
        public string ProjectId { get; private set; }

        public IronIOCredentials(string token, string projectId)
        {
            Token = token;
            ProjectId = projectId;
        }
    }
}