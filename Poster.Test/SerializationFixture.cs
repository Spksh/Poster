using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Internal;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace Poster.Test
{
    [TestFixture]
    public class SerializationFixture
    {
        [TestCase(@"D:\Users\Spksh\Documents\Projects\Poster\Poster.Sample.EmptyAspNetWebApp\App_Data\config.yml")]
        public void DeserializeConfiguration(string filePath)
        {
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            using (StreamReader reader = new StreamReader(file, Encoding.UTF8))
            {
                //// Set up YAML parser
                //// We only want to read the first YAML "document"
                //// So we instruct the parser to read StreamStart, then DocumentStart
                //Parser parser = new Parser(reader);
                //parser.Expect<StreamStart>();
                //parser.Expect<DocumentStart>();
            }

        }
    }
}
