namespace fITNat
{
    /// <summary>
    /// Definiert einen Trainingsplan
    /// </summary>
    public class Schedule
    {
        #region ctor
        public Schedule(int id, string name = "", string userId = "")
        {
            this.Id = id;
            this.Name = name;
            this.UserID = userId;
        }

        public Schedule()
            : this(-1)
        {

        }
        #endregion

        #region Properties
        /// <summary>
        /// DB ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// DisplayName des Trainingsplans
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Fremdschlüssel zum Nutzer
        /// </summary>        
        public string UserID { get; set; }

        #endregion
    }
}