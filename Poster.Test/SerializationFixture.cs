using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using SharpYaml;
using SharpYaml.Events;

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
                EventReader events = new EventReader(new Parser(reader));
                events.Expect<StreamStart>();
                events.Expect<DocumentStart>();

                Dictionary<object, object> result = new SharpYaml.Serialization.Serializer().Deserialize<Dictionary<object, object>>(events);

                Console.WriteLine(result);
                Console.WriteLine(reader.ReadToEnd());
            }

        }
    }
}
