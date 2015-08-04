using System.ComponentModel.DataAnnotations;
using fIT.WebApi.Repository.Interfaces.CRUD;

namespace fIT.WebApi.Models
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
        [Required(ErrorMessageResourceName = "Error_Required", ErrorMessageResourceType = typeof(Resources))]
        [Display(ResourceType = typeof(Resources), Name = "Label_Id")]
        public T Id { get; set; }

        /// <summary>
        /// Name of an Entity
        /// </summary>
        [Required(ErrorMessageResourceName = "Error_Required", ErrorMessageResourceType = typeof(Resources))]
        [Display(ResourceType = typeof(Resources), Name = "Label_Name")]
        public string Name { get; set; }

        /// <summary>
        /// Url to receive this entity
        /// </summary>
        [Display(ResourceType = typeof(Resources), Name = "Label_Url")]
        public string Url { get; set; }
    }
}
