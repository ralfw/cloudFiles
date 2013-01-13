using System.IO;

namespace exploring.ironcache
{
    class Credentials
    {
        public static Credentials LoadFrom(string filename)
        {
            var lines = File.ReadAllLines(filename);
            return new Credentials(lines[0], lines[1]);
        }


        public string Token { get; private set; }
        public string ProjectId { get; private set; }

        private Credentials(string token, string projectId)
        {
            Token = token;
            ProjectId = projectId;
        }
    }
}