namespace fIT.WebApi.Models
{    
    // Defines one entry from the server    
    public class EntryModel<T>
    {        
        // Id of an entity
        [Required(ErrorMessageResourceName = "Error_Required", ErrorMessageResourceType = typeof(Resources))]
        public T Id { get; set; }

        // Name of an Entity
        [Required(ErrorMessageResourceName = "Error_Required", ErrorMessageResourceType = typeof(Resources))]
        public string Name { get; set; }

        // Url to receive this entity
        public string Url { get; set; }
    }
}
