namespace fIT.WebApi.Client.Data.Models.Shared
{
  /// <summary>
  /// Defines one entry from the server
  /// </summary>
  /// <typeparam name="T">Type of id-property</typeparam>
    public class EntryModel<T>
  {
      /// <summary>
      /// Id of an entity
      /// </summary>
      public T Id { get; set; }
      
      /// <summary>
      /// Name of an Entity
      /// </summary>
      public string Name { get; set; }
      
      /// <summary>
      /// Url to receive this entity
      /// </summary>
      public string Url { get; set; }
    }
}
