using System.Collections.ObjectModel;

namespace fITNat
{
    /// <summary>
    /// Uebung fuer Trainingspläne
    /// </summary>
    public class Exercise
    {
        #region ctor
        public Exercise(int id = -1, string name = "", string description = "", Collection<Schedule> schedules = null)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Schedules = Schedules;
        }

        public Exercise()
            : this(-1)
        {

        }
        #endregion

        #region Properties
        /// <summary>
        /// ID der Uebung
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// DisplayName
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Beschreibung
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Trainingsplaene, die diese Uebung enthalten
        /// </summary>
        public virtual Collection<Schedule> Schedules { get; set; }
        #endregion

    }
}