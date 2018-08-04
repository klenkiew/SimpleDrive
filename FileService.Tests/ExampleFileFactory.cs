using System;
using System.Globalization;
using FileService.Model;

namespace FileService.Tests
{
    public class ExampleFileFactory
    {
        public File CreateFile()
        {
            DateTime dateCreated = DateTime.ParseExact("05/05/2030", "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var owner = new User(Guid.Empty.ToString(), "testUser");
            return new File("testFile", "desc", 5, "text/plain", dateCreated, owner);
        }
    }
}