using System.Reflection;
using FileService.Model;

namespace FileService.Tests.Helpers
{
    public class EntityHelper
    {
        public static void SetId<T>(T entity, string id) where T : IEntity
        {
            // set the id property on file to prevent bugs due to null id
            PropertyInfo prop = entity.GetType().GetProperty("Id", BindingFlags.NonPublic | BindingFlags.Instance 
                                                                                          | BindingFlags.Public);
            prop.SetValue(entity, id);
        }
    }
}