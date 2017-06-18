using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Poster.Test
{
    [TestFixture]
    public class SerializationFixture
    {
        [OneTimeSetUp]
        public void Setup()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        [TestCase(@"..\..\..\Poster.Sample.EmptyAspNetWebApp\App_Data\comments-enabled.md")]
        public void DeserializeConfiguration(string filePath)
        {
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            using (StreamReader reader = new StreamReader(file, Encoding.UTF8))
            {
                // Set up YAML parser
                // We only want to read the first YAML "document"
                // So we instruct the parser to read StreamStart, then DocumentStart
                Parser parser = new Parser(reader);
                parser.Expect<StreamStart>();
                parser.Expect<DocumentStart>();

                Dictionary<object, object> result = new DeserializerBuilder().Build().Deserialize<Dictionary<object, object>>(parser);

                Console.WriteLine(result);
                Console.WriteLine(reader.ReadToEnd());
            }

        }
    }
}
