using System;
using System.Text;

namespace QueryMyData.Models
{
    public abstract class PathBuilder
    {
        public string GetPathBuilt(string hash, string ext)
        {
            var pathBuilder = new StringBuilder();
            pathBuilder.Append(Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
                .Append("\\")
                .Append(hash)
                .Append(ext);

            return pathBuilder.ToString();
        }
    }
}
